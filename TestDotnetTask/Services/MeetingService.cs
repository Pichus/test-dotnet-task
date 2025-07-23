using Microsoft.EntityFrameworkCore;
using TestDotnetTask.Database;
using TestDotnetTask.Dtos;
using TestDotnetTask.Models;

namespace TestDotnetTask.Services;

public record CreateMeetingResult
{
    public bool IsSuccess { get; set; } = true;
    public Meeting? Meeting { get; set; }
}

public class MeetingService : IMeetingService
{
    private readonly TestDotnetTaskContext _context;

    public MeetingService(TestDotnetTaskContext context)
    {
        _context = context;
    }

    public async Task<CreateMeetingResult> CreateMeetingIfPossible(List<int> userIds, MeetingTimeRange desiredRange,
        int newMeetingDurationMinutes)
    {
        var result = new CreateMeetingResult();

        var meetings = await GetUserMeetingsInSpecifiedRange(userIds, desiredRange);

        var timeRangeForNewMeeting = new MeetingTimeRange();

        var timeRangeForNewMeetingExists = TryFindTimeRangeForNewMeeting(meetings, desiredRange,
            newMeetingDurationMinutes, ref timeRangeForNewMeeting);

        if (!timeRangeForNewMeetingExists) result.IsSuccess = false;

        var users = await GetUsersByIds(userIds);

        var meeting = new Meeting
        {
            StartTime = timeRangeForNewMeeting.Start,
            EndTime = timeRangeForNewMeeting.End,
            Users = users
        };

        result.Meeting = meeting;

        return result;
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

    private async Task<List<User>> GetUsersByIds(List<int> userIds)
    {
        return await _context.Users.Where(user => userIds.Contains(user.Id)).ToListAsync();
    }

    private bool TryFindTimeRangeForNewMeeting(List<MeetingTimeRange> userMeetings,
        MeetingTimeRange desiredMeetingTimeRange,
        int newMeetingDurationMinutes, ref MeetingTimeRange newMeetingTimeRange)
    {
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


        return timeRangeForNewMeetingExists;
    }

    private DateTime DateTimeMax(DateTime first, DateTime second)
    {
        if (first > second) return first;

        return second;
    }
}