using TestDotnetTask.Dtos;

namespace TestDotnetTask.Services;

public interface IMeetingService
{
    Task<CreateMeetingResult> CreateMeetingIfPossible(List<int> userIds, MeetingTimeRange desiredRange,
        int newMeetingDurationMinutes);
}