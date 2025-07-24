namespace TestDotnetTask.Dtos;

public class CreateMeetingUsersNotFoundResponse
{
    public string Message { get; set; }
    public List<int> NotFoundUserIds { get; set; }
}