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
    public async Task GetUserMeetings(int id)
    {
        
    }
}