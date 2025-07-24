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
    private readonly IMeetingSchedulerService _meetingScheduler;

    public MeetingService(TestDotnetTaskContext context, IMeetingSchedulerService meetingScheduler)
    {
        _context = context;
        _meetingScheduler = meetingScheduler;
    }

    public async Task<Result<Meeting>> CreateMeetingIfPossible(List<int> userIds,
        MeetingTimeRange desiredRange,
        int newMeetingDurationMinutes)
    {
        var getUsersResult = await GetUsersByIds(userIds);

        if (!getUsersResult.IsSuccess)
            return Result<Meeting>.Failure(getUsersResult.Error);

        var users = getUsersResult.Value;

        var meetings = await GetUserMeetingsInSpecifiedRange(userIds, desiredRange);

        var findTimeRangeForNewMeetingResult =
            _meetingScheduler.FindTimeRangeForNewMeeting(meetings, desiredRange, newMeetingDurationMinutes);

        if (!findTimeRangeForNewMeetingResult.IsSuccess)
            return Result<Meeting>.Failure(findTimeRangeForNewMeetingResult.Error);

        var timeRangeForNewMeeting = findTimeRangeForNewMeetingResult.Value;

        var meeting = new Meeting
        {
            StartTime = timeRangeForNewMeeting!.Start,
            EndTime = timeRangeForNewMeeting!.End,
            Users = users!
        };

        _context.Meetings.Add(meeting);

        await _context.SaveChangesAsync();

        return Result<Meeting>.Success(meeting);
    }

    private async Task<Result<List<User>>> GetUsersByIds(List<int> userIds)
    {
        var users = await _context.Users
            .Where(u => userIds.Contains(u.Id))
            .ToListAsync();

        var existingIds = users.Select(user => user.Id);

        var missingUsers = userIds.Except(existingIds).ToList();

        if (missingUsers.Any())
            return Result<List<User>>.Failure(
                new UserIdsNotFoundError("users with these ids do not exist", userIds));

        return Result<List<User>>.Success(users);
    }

    private async Task<List<MeetingTimeRange>> GetUserMeetingsInSpecifiedRange(ICollection<int> userIds,
        MeetingTimeRange desiredRange)
    {
        var meetings = await _context.Meetings
            .Where(meeting => meeting.EndTime >= desiredRange.Start && meeting.StartTime <= desiredRange.End)
            .Where(meeting => meeting.Users.Any(user => userIds.Contains(user.Id)))
            .OrderBy(meeting => meeting.StartTime)
            .ThenBy(meeting => meeting.EndTime)
            .Select(meeting => new MeetingTimeRange(meeting.StartTime, meeting.EndTime))
            .ToListAsync();

        return meetings;
    }
}