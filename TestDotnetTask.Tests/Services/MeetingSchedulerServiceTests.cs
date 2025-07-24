using TestDotnetTask.Dtos;
using TestDotnetTask.Services;
using TestDotnetTask.Tests.Fixtures;

namespace TestDotnetTask.Tests.Services;

public class MeetingSchedulerServiceTests
{
    private readonly MeetingSchedulerService _scheduler = new();

    [Fact]
    public void Should_ReturnSlot_When_NoExistingMeetings()
    {
        var existingMeetings = new MeetingFixture().Build();

        var desiredRange = new MeetingTimeRange(
            new DateTime(2025, 7, 24, 9, 0, 0),
            new DateTime(2025, 7, 24, 17, 0, 0)
        );

        var duration = 60;

        var result = _scheduler.FindTimeRangeForNewMeeting(existingMeetings, desiredRange, duration);

        Assert.True(result.IsSuccess);
        Assert.Equal(desiredRange.Start, result.Value!.Start);
        Assert.Equal(desiredRange.Start.AddMinutes(duration), result.Value.End);
    }

    [Fact]
    public void Should_ReturnSlot_BetweenMeetings()
    {
        var existingMeetings = new MeetingFixture()
            .Add(new DateTime(2025, 7, 24, 9, 0, 0), new DateTime(2025, 7, 24, 10, 0, 0))
            .Add(new DateTime(2025, 7, 24, 12, 0, 0), new DateTime(2025, 7, 24, 13, 0, 0))
            .Build();

        var desiredRange = new MeetingTimeRange(
            new DateTime(2025, 7, 24, 8, 0, 0),
            new DateTime(2025, 7, 24, 18, 0, 0)
        );

        var duration = 60;

        var result = _scheduler.FindTimeRangeForNewMeeting(existingMeetings, desiredRange, duration);

        Assert.True(result.IsSuccess);
        Assert.Equal(new DateTime(2025, 7, 24, 8, 0, 0), result.Value!.Start);
        Assert.Equal(new DateTime(2025, 7, 24, 9, 0, 0), result.Value.End);
    }

    [Fact]
    public void Should_ReturnSlot_AtEndOfWindow()
    {
        var existingMeetings = new MeetingFixture()
            .Add(new DateTime(2025, 7, 24, 9, 0, 0), new DateTime(2025, 7, 24, 16, 0, 0))
            .Build();

        var desiredRange = new MeetingTimeRange(
            new DateTime(2025, 7, 24, 9, 0, 0),
            new DateTime(2025, 7, 24, 17, 0, 0)
        );

        var duration = 60;

        var result = _scheduler.FindTimeRangeForNewMeeting(existingMeetings, desiredRange, duration);

        Assert.True(result.IsSuccess);
        Assert.Equal(new DateTime(2025, 7, 24, 16, 0, 0), result.Value!.Start);
        Assert.Equal(new DateTime(2025, 7, 24, 17, 0, 0), result.Value.End);
    }

    [Fact]
    public void Should_Fail_When_NoAvailableSlot()
    {
        var existingMeetings = new MeetingFixture()
            .Add(new DateTime(2025, 7, 24, 9, 0, 0), new DateTime(2025, 7, 24, 16, 30, 0))
            .Build();

        var desiredRange = new MeetingTimeRange(
            new DateTime(2025, 7, 24, 9, 0, 0),
            new DateTime(2025, 7, 24, 17, 0, 0)
        );

        var duration = 60;

        var result = _scheduler.FindTimeRangeForNewMeeting(existingMeetings, desiredRange, duration);

        Assert.False(result.IsSuccess);
    }

    [Fact]
    public void Should_ScheduleBetween_BackToBack()
    {
        var existingMeetings = new MeetingFixture()
            .Add(new DateTime(2025, 7, 24, 9, 0, 0), new DateTime(2025, 7, 24, 10, 0, 0))
            .Add(new DateTime(2025, 7, 24, 11, 0, 0), new DateTime(2025, 7, 24, 12, 0, 0))
            .Build();

        var desiredRange = new MeetingTimeRange(
            new DateTime(2025, 7, 24, 9, 0, 0),
            new DateTime(2025, 7, 24, 13, 0, 0)
        );

        var duration = 60;

        var result = _scheduler.FindTimeRangeForNewMeeting(existingMeetings, desiredRange, duration);

        Assert.True(result.IsSuccess);
        Assert.Equal(new DateTime(2025, 7, 24, 10, 0, 0), result.Value!.Start);
        Assert.Equal(new DateTime(2025, 7, 24, 11, 0, 0), result.Value.End);
    }
}