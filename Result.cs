namespace TaskTrackerCLI;

public record Result
{
    public string? ErrorMessage { get; set; }
    public bool IsSuccess => ErrorMessage == null;

    public static Result Success() => new();
    public static Result Failure(string errorMessage) => new() { ErrorMessage = errorMessage };
}

public record Result<T> : Result
{
    public T? Value { get; set; }

    public static Result<T> Success(T value) => new() { Value = value };
    public new static Result<T> Failure(string errorMessage) => new() { ErrorMessage = errorMessage };
}