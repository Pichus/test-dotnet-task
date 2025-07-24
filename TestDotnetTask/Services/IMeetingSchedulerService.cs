using TestDotnetTask.Dtos;
using TestDotnetTask.Results;

namespace TestDotnetTask.Services;

public interface IMeetingSchedulerService
{
    Result<MeetingTimeRange> FindTimeRangeForNewMeeting(List<MeetingTimeRange> existingMeetings,
        MeetingTimeRange desiredMeetingTimeRange, int newMeetingDurationMinutes);
}