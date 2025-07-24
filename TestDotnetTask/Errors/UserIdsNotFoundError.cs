namespace TestDotnetTask.Errors;

public class UserIdsNotFoundError : Error
{
    public UserIdsNotFoundError(List<int> notFoundUserIds)
    {
        NotFoundUserIds = notFoundUserIds;
    }

    public UserIdsNotFoundError(string message, List<int> notFoundUserIds) : base(message)
    {
        NotFoundUserIds = notFoundUserIds;
    }
    
    public List<int> NotFoundUserIds { get; set; }
}