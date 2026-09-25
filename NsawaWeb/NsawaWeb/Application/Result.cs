namespace NsawaWeb.Application;

/// <summary>Outcome of a call the UI can show directly: either data or a message written for people.</summary>
public class Result
{
    protected Result(bool succeeded, string? message)
    {
        Succeeded = succeeded;
        Message = message;
    }

    public bool Succeeded { get; }
    public bool Failed => !Succeeded;

    /// <summary>Success confirmation from the server, or a friendly error.</summary>
    public string? Message { get; }

    public static Result Ok(string? message = null) => new(true, message);
    public static Result Fail(string message) => new(false, message);
}

public sealed class Result<T> : Result
{
    private Result(bool succeeded, T? value, string? message) : base(succeeded, message)
    {
        Value = value;
    }

    public T? Value { get; }

    public static Result<T> Ok(T value, string? message = null) => new(true, value, message);
    public static new Result<T> Fail(string message) => new(false, default, message);
}
