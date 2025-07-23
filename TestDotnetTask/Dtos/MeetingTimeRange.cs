namespace TestDotnetTask.Dtos;

public class MeetingTimeRange
{
    public MeetingTimeRange()
    {
        Start = new DateTime();
        End = new DateTime();
    }

    public MeetingTimeRange(DateTime start, DateTime end)
    {
        Start = start;
        End = end;
    }

    public DateTime Start { get; set; }
    public DateTime End { get; set; }
}