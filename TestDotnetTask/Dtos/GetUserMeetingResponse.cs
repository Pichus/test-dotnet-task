namespace TestDotnetTask.Dtos;

public class GetUserMeetingResponse
{
    public int Id { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
}