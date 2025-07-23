using Microsoft.AspNetCore.Mvc;
using TestDotnetTask.Dtos;

namespace TestDotnetTask.Controllers;

[Route("/meetings")]
[ApiController]
public class MeetingController : ControllerBase
{
    [HttpPost]
    public async Task CreateMeeting(CreateMeetingRequest request)
    {
    }

    private MeetingValidationResult IsCreateMeetingRequestValid(CreateMeetingRequest request)
    {
        var result = new MeetingValidationResult();
        
        var isLatestEndLaterThanEarliestStart = request.LatestEnd > request.EarliestStart;

        if (!isLatestEndLaterThanEarliestStart)
        {
            result.Errors.Add("Latest end time must be after earliest start time");
        }

        const int businessHoursStartHour = 9;
        const int businessHoursEndHour = 17;

        var isMeetingInBusinessHours =
            request.EarliestStart.TimeOfDay >= TimeSpan.FromHours(businessHoursStartHour) &&
            request.LatestEnd.TimeOfDay <= TimeSpan.FromHours(businessHoursEndHour);

        if (!isMeetingInBusinessHours)
        {
            result.Errors.Add("Meeting must be scheduled between 9:00 AM and 5:00 PM.");
        }

        var timeBetweenEarliestStartAndLatestEnd = request.LatestEnd - request.EarliestStart;

        var durationFitsInSpecifiedInterval =
            request.DurationMinutes < timeBetweenEarliestStartAndLatestEnd.Duration().Minutes;

        if (!durationFitsInSpecifiedInterval)
        {
            result.Errors.Add("Duration exceeds the time between earliest start and latest end.");
        }

        result.IsValid = result.Errors.Count == 0;
        return result;
    }
}