using Microsoft.EntityFrameworkCore;
using TestDotnetTask.Database;
using TestDotnetTask.Dtos;
using TestDotnetTask.Errors;
using TestDotnetTask.Models;
using TestDotnetTask.Results;

namespace TestDotnetTask.Services;

public class MeetingService : IMeetingService
{
    private readonly TestDotnetTaskContext _context;

    public MeetingService(TestDotnetTaskContext context)
    {
        _context = context;
    }

    public async Task<Result<Meeting>> CreateMeetingIfPossible(List<int> userIds, MeetingTimeRange desiredRange,
        int newMeetingDurationMinutes)
    {
        var meetings = await GetUserMeetingsInSpecifiedRange(userIds, desiredRange);

        var timeRangeForNewMeeting = new MeetingTimeRange();

        var findTimeRangeForNewMeetingResult =
            FindTimeRangeForNewMeeting(meetings, desiredRange, newMeetingDurationMinutes);

        if (!findTimeRangeForNewMeetingResult.IsSuccess)
            return Result<Meeting>.Failure(findTimeRangeForNewMeetingResult.Error);


        var getUsersResult = await GetUsersByIds(userIds);

        if (!getUsersResult.IsSuccess) return Result<Meeting>.Failure(getUsersResult.Error);

        var users = getUsersResult.Value;

        var meeting = new Meeting
        {
            StartTime = timeRangeForNewMeeting.Start,
            EndTime = timeRangeForNewMeeting.End,
            Users = users!
        };

        _context.Meetings.Add(meeting);

        await _context.SaveChangesAsync();

        return Result<Meeting>.Success(meeting);
    }

    private async Task<Result<List<User>, List<int>>> GetUsersByIds(List<int> userIds)
    {
        var users = await _context.Users
            .Where(u => userIds.Contains(u.Id))
            .ToListAsync();

        var existingIds = users.Select(user => user.Id);

        var missingUsers = userIds.Except(existingIds).ToList();

        if (missingUsers.Any())
            return Result<List<User>, List<int>>.Failure(
                new Error("users with these ids do not exist"), missingUsers);

        return Result<List<User>, List<int>>.Success(users);
    }

    private async Task<List<MeetingTimeRange>> GetUserMeetingsInSpecifiedRange(ICollection<int> userIds,
        MeetingTimeRange desiredRange)
    {
        var meetings = await _context.Meetings
            .Where(meeting => meeting.StartTime >= desiredRange.Start && meeting.EndTime <= desiredRange.End)
            .Where(meeting => meeting.Users.Any(user => userIds.Contains(user.Id)))
            .OrderBy(meeting => meeting.StartTime)
            .ThenBy(meeting => meeting.EndTime)
            .Select(meeting => new MeetingTimeRange(meeting.StartTime, meeting.EndTime))
            .ToListAsync();

        return meetings;
    }

    private Result<MeetingTimeRange> FindTimeRangeForNewMeeting(List<MeetingTimeRange> userMeetings,
        MeetingTimeRange desiredMeetingTimeRange, int newMeetingDurationMinutes)
    {
        var newMeetingTimeRange = new MeetingTimeRange();

        var previousMeetingEndTime = desiredMeetingTimeRange.Start;

        var timeRangeForNewMeetingExists = false;

        foreach (var meeting in userMeetings)
        {
            var isDesiredMeetingTimeRangeExceeded =
                previousMeetingEndTime.AddMinutes(newMeetingDurationMinutes) > desiredMeetingTimeRange.End;

            if (isDesiredMeetingTimeRangeExceeded) break;

            if (meeting.Start > previousMeetingEndTime)
            {
                var minutesBetweenPreviousMeetingEndTimeAndCurrentMeetingStart = (int)Math.Floor(
                    (meeting.Start - previousMeetingEndTime).Duration().TotalMinutes);

                if (minutesBetweenPreviousMeetingEndTimeAndCurrentMeetingStart >= newMeetingDurationMinutes)
                {
                    var spareMinutes = minutesBetweenPreviousMeetingEndTimeAndCurrentMeetingStart -
                                       newMeetingDurationMinutes;
                    var threshold = spareMinutes / 2; // threshold to avoid back to back meetings if possible

                    var newMeetingStart = previousMeetingEndTime.AddMinutes(threshold);
                    var newMeetingEnd = newMeetingStart.AddMinutes(newMeetingDurationMinutes);

                    newMeetingTimeRange = new MeetingTimeRange(newMeetingStart, newMeetingEnd);

                    timeRangeForNewMeetingExists = true;
                    break;
                }
            }

            previousMeetingEndTime = DateTimeMax(previousMeetingEndTime, meeting.End);
        }

        if (!timeRangeForNewMeetingExists)
        {
            var endWindow = desiredMeetingTimeRange.End - previousMeetingEndTime;
            if (endWindow.TotalMinutes >= newMeetingDurationMinutes)
            {
                newMeetingTimeRange = new MeetingTimeRange(previousMeetingEndTime,
                    previousMeetingEndTime.AddMinutes(newMeetingDurationMinutes));
                timeRangeForNewMeetingExists = true;
            }
        }

        if (!timeRangeForNewMeetingExists)
            return Result<MeetingTimeRange>.Failure(new Error("no time range was found for meeting"));

        return Result<MeetingTimeRange>.Success(newMeetingTimeRange);
    }

    private DateTime DateTimeMax(DateTime first, DateTime second)
    {
        if (first > second) return first;

        return second;
    }
}