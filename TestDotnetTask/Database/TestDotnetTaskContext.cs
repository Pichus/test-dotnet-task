using Microsoft.EntityFrameworkCore;

namespace TestDotnetTask.Database;

public class TestDotnetTaskContext : DbContext
{
    public TestDotnetTaskContext(DbContextOptions<TestDotnetTaskContext> options)
        : base(options)
    {
        
    }
}