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
}