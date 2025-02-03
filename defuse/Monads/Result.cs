namespace Defuse.Monads;

public static class Result
{
    public static Result<T> Success<T>(T value) => new(value, null);

    public static Result<T> Failure<T>(string error) => new(default!, error);
}

public class Result<T>(T value, string? error)
{
    public T Value { get; } = value;
    public string? Error { get; } = error;

    public bool IsSuccess => Error == null;
    public bool IsFailure => Error != null;

    public static implicit operator Result<T>(T value) => Result.Success(value);

    public static implicit operator Result<T>(string error) => Result.Failure<T>(error);
}
