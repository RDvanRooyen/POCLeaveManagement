namespace Shared;

public class ApiResult
{
    public bool Succeeded { get; private set; }
    public string? Message { get; private set; }

    public ApiResult(bool succeeded, string? message)
    {
        Succeeded = succeeded;
        Message = message;
    }

    public static ApiResult Success(string message)
    {
        return new(true, message);
    }

    public static ApiResult Failure(string message)
    {
        return new(false, message);
    }
}

public class ApiResult<T> : ApiResult
{
    public ApiResult(bool succeeded, string? message, T? value = default) : base(succeeded, message)
    {
    }

    public T? Value { get; private set; }

    public static ApiResult<T> Success(T value, string message)
    {
        return new(true, message, value);
    }

    public new static ApiResult<T> Failure(string message)
    {
        return new(false, message, default);
    }
}