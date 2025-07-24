using TestDotnetTask.Dtos;

namespace TestDotnetTask.Tests.Fixtures;

public class MeetingFixture
{
    private readonly List<MeetingTimeRange> _meetings = new();

    public MeetingFixture Add(DateTime start, DateTime end)
    {
        _meetings.Add(new MeetingTimeRange(start, end));
        return this;
    }

    public List<MeetingTimeRange> Build()
    {
        return _meetings.OrderBy(m => m.Start).ThenBy(m => m.End).ToList();
    }
}