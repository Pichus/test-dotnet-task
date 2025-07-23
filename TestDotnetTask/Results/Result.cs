using TestDotnetTask.Errors;

namespace TestDotnetTask.Results;

public class Result
{
    private Result()
    {
        IsSuccess = true;
    }

    private Result(Error error)
    {
        IsSuccess = false;
        Error = error;
    }

    public bool IsSuccess { get; set; }
    public Error Error { get; set; }

    public static Result Success()
    {
        return new Result();
    }

    public static Result Failure(Error error)
    {
        return new Result(error);
    }
}