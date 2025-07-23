namespace TestDotnetTask.Dtos;

public class CreateMeetingResponse
{
    public int Id { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
}