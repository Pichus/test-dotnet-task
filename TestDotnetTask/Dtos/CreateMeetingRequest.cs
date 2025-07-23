namespace TestDotnetTask.Dtos;

public class CreateMeetingRequest
{
    public List<int> ParticipantIds { get; set; }
    public int DurationMinutes { get; set; }
    public DateTime EarliestStart { get; set; }
    public DateTime LatestEnd { get; set; }
}