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
        const int businessHoursStartHour = 9;
        const int businessHoursEndHour = 17;
        
        bool isMeetingInBusinessHours = 
            request.EarliestStart.TimeOfDay >= TimeSpan.FromHours(businessHoursStartHour) &&
            request.LatestEnd.TimeOfDay <= TimeSpan.FromHours(businessHoursEndHour);

        return isMeetingInBusinessHours;
    }
}