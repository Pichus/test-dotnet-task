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

    private bool IsCreateMeetingRequestValid(CreateMeetingRequest request)
    {
        var isLatestEndLaterThanEarliestStart = request.LatestEnd > request.EarliestStart;

        const int businessHoursStartHour = 9;
        const int businessHoursEndHour = 17;

        var isMeetingInBusinessHours =
            request.EarliestStart.TimeOfDay >= TimeSpan.FromHours(businessHoursStartHour) &&
            request.LatestEnd.TimeOfDay <= TimeSpan.FromHours(businessHoursEndHour);

        var timeBetweenEarliestStartAndLatestEnd = request.LatestEnd - request.EarliestStart;

        var durationFitsInSpecifiedInterval = request.DurationMinutes < timeBetweenEarliestStartAndLatestEnd.Minutes;

        var requestValid = isLatestEndLaterThanEarliestStart && isMeetingInBusinessHours &&
                           durationFitsInSpecifiedInterval;

        return requestValid;
    }
}