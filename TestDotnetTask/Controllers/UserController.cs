using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TestDotnetTask.Database;
using TestDotnetTask.Dtos;
using TestDotnetTask.Models;

namespace TestDotnetTask.Controllers;

[Route("/users")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly TestDotnetTaskContext _context;
    
    public UserController(TestDotnetTaskContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<ActionResult> CreateUser(CreateUserRequest request)
    {
        var user = new User
        {
            Name = request.Name
        };

        _context.Users.Add(user);

        await _context.SaveChangesAsync();

        var response = new CreateUserResponse
        {
            Id = user.Id,
            Name = user.Name
        };

        return CreatedAtAction(nameof(CreateUser), response);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<GetUserResponse>>> GetUsers()
    {
        var users = await _context.Users.Select(user => new GetUserResponse()
        {
            Id = user.Id,
            Name = user.Name
        }).ToListAsync();
        
        return Ok(users);
    }

    [HttpGet("{id}/meetings")]
    public async Task<ActionResult<IEnumerable<GetUserMeetingResponse>>> GetUserMeetings(int id)
    {
        var user = await _context.Users
            .Include(user1 => user1.Meetings)
            .FirstOrDefaultAsync(user1 => user1.Id == id);

        if (user is null)
        {
            return NotFound($"user with id {id} does not exist");
        }

        var userMeetings = user.Meetings.Select(meeting => new GetUserMeetingResponse
        {
            Id = meeting.Id,
            StartTime = meeting.StartTime,
            EndTime = meeting.EndTime
        }).ToList();

        return userMeetings;
    }
}