using TestDotnetTask.Dtos;
using TestDotnetTask.Errors;
using TestDotnetTask.Results;

namespace TestDotnetTask.Services;

public class MeetingSchedulerService : IMeetingSchedulerService
{
    public Result<MeetingTimeRange> FindTimeRangeForNewMeeting(List<MeetingTimeRange> existingMeetings, MeetingTimeRange desiredMeetingTimeRange,
        int newMeetingDurationMinutes)
    {
                var newMeetingTimeRange = new MeetingTimeRange();

        var previousMeetingEndTime = desiredMeetingTimeRange.Start;

        var timeRangeForNewMeetingExists = false;

        foreach (var meeting in existingMeetings)
        {
            var isDesiredMeetingTimeRangeExceeded =
                previousMeetingEndTime.AddMinutes(newMeetingDurationMinutes) > desiredMeetingTimeRange.End;

            if (isDesiredMeetingTimeRangeExceeded) break;

            if (meeting.Start >= previousMeetingEndTime)
            {
                var minutesBetweenPreviousMeetingEndTimeAndCurrentMeetingStart = (int)Math.Floor(
                    (meeting.Start - previousMeetingEndTime).TotalMinutes);

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