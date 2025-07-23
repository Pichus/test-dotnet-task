using Microsoft.AspNetCore.Mvc;
using TestDotnetTask.Dtos;

namespace TestDotnetTask.Controllers;

[Route("/users")]
[ApiController]
public class UserController : ControllerBase
{
    [HttpPost]
    public async Task CreateUser(CreateUserRequest request)
    {
        
    }

    [HttpGet("{id}/meetings")]
    public async Task GetUserMeetings(int id)
    {
        
    }
}