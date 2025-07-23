namespace TestDotnetTask.Models;

public class User
{
    public int Id { get; set; }
    public string Name { get; set; }
    
    public ICollection<Meeting> Meetings { get; set; }
}