using TestDotnetTask.Dtos;
using TestDotnetTask.Models;
using TestDotnetTask.Results;

namespace TestDotnetTask.Services;

public interface IMeetingService
{
    Task<Result<Meeting, List<int>>> CreateMeetingIfPossible(List<int> userIds, MeetingTimeRange desiredRange,
        int newMeetingDurationMinutes);
}