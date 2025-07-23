using TestDotnetTask.Errors;

namespace TestDotnetTask.Results;

public class Result
{
    protected Result()
    {
        IsSuccess = true;
    }

    protected Result(Error error)
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