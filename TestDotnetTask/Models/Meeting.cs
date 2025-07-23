namespace TestDotnetTask.Models;

public class Meeting
{
    public int Id { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }

    public ICollection<User> Users { get; set; }
}