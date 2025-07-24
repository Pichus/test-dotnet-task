using TestDotnetTask.Errors;

namespace TestDotnetTask.Results;

public class Result<TSuccessValue, TFailureValue> : Result<TSuccessValue>
{
    protected Result()
    {
    }

    protected Result(Error error) : base(error)
    {
    }

    protected Result(TSuccessValue value) : base(value)
    {
    }

    protected Result(Error error, TFailureValue failureValue)
    {
        IsSuccess = false;
        Error = error;
        FailureValue = failureValue;
    }

    public TFailureValue? FailureValue { get; set; }

    public static Result<TSuccessValue, TFailureValue> Failure(Error error)
    {
        return new Result<TSuccessValue, TFailureValue>(error);
    }
    
    public static Result<TSuccessValue, TFailureValue> Failure(Error error, TFailureValue failureValue)
    {
        return new Result<TSuccessValue, TFailureValue>(error, failureValue);
    }

    public static Result<TSuccessValue, TFailureValue> Success(TSuccessValue value)
    {
        return new Result<TSuccessValue, TFailureValue>(value);
    }
}