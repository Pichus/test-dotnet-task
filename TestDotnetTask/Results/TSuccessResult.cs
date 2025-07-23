using TestDotnetTask.Errors;

namespace TestDotnetTask.Results;

public class Result<TSuccessValue> : Result
{
    protected Result()
    {
    }

    protected Result(Error error) : base(error)
    {
    }

    protected Result(TSuccessValue value)
    {
        Value = value;
    }

    public TSuccessValue? Value { get; set; }

    public static Result<TSuccessValue> Success(TSuccessValue value)
    {
        return new Result<TSuccessValue>(value);
    }

    public static Result<TSuccessValue> Failure(Error error)
    {
        return new Result<TSuccessValue>(error);
    }
}