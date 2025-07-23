using Microsoft.EntityFrameworkCore;
using TestDotnetTask.Models;

namespace TestDotnetTask.Database;

public class TestDotnetTaskContext : DbContext
{
    public TestDotnetTaskContext(DbContextOptions<TestDotnetTaskContext> options)
        : base(options)
    {
        
    }
    
    public DbSet<User> Users { get; set; }
    public DbSet<Meeting> Meetings { get; set; }
}