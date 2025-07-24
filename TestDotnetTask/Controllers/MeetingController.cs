using Microsoft.AspNetCore.Mvc;
using TestDotnetTask.Dtos;
using TestDotnetTask.Errors;
using TestDotnetTask.Results;
using TestDotnetTask.Services;

namespace TestDotnetTask.Controllers;

[Route("/meetings")]
[ApiController]
public class MeetingController : ControllerBase
{
    private readonly IMeetingService _meetingService;

    public MeetingController(IMeetingService meetingService)
    {
        _meetingService = meetingService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateMeeting(CreateMeetingRequest request)
    {
        var validateMeetingResult = IsCreateMeetingRequestValid(request);

        if (!validateMeetingResult.IsSuccess) return BadRequest(validateMeetingResult.Error);

        var createMeetingResult = await _meetingService.CreateMeetingIfPossible(request.ParticipantIds,
            new MeetingTimeRange(request.EarliestStart, request.LatestEnd), request.DurationMinutes);


        if (!createMeetingResult.IsSuccess)
        {
            if (createMeetingResult.Error is UserIdsNotFoundError error)
            {
                var notFoundResponse = new CreateMeetingUsersNotFoundResponse
                {
                    Message = error.Message,
                    NotFoundUserIds = error.NotFoundUserIds
                };
                
                return NotFound(notFoundResponse);
            }
            return BadRequest(createMeetingResult.Error);
        }

        var meeting = createMeetingResult.Value;

        var response = new CreateMeetingResponse
        {
            Id = meeting!.Id,
            StartTime = meeting!.StartTime,
            EndTime = meeting!.EndTime
        };

        return CreatedAtAction(nameof(CreateMeeting), response);
    }

    private Result IsCreateMeetingRequestValid(CreateMeetingRequest request)
    {
        var isLatestEndLaterThanEarliestStart = request.LatestEnd > request.EarliestStart;

        if (!isLatestEndLaterThanEarliestStart)
            return Result.Failure(new Error("Latest end time must be after earliest start time"));

        const int businessHoursStartHour = 9;
        const int businessHoursEndHour = 17;

        var isMeetingInBusinessHours =
            request.EarliestStart.TimeOfDay >= TimeSpan.FromHours(businessHoursStartHour) &&
            request.LatestEnd.TimeOfDay <= TimeSpan.FromHours(businessHoursEndHour);

        if (!isMeetingInBusinessHours)
            return Result.Failure(new Error("Meeting must be scheduled between 9:00 AM and 5:00 PM."));

        var timeBetweenEarliestStartAndLatestEnd = request.LatestEnd - request.EarliestStart;

        var durationFitsInSpecifiedInterval =
            request.DurationMinutes <= timeBetweenEarliestStartAndLatestEnd.TotalMinutes;

        if (!durationFitsInSpecifiedInterval)
            return Result.Failure(new Error("Duration exceeds the time between earliest start and latest end."));

        return Result.Success();
    }
}