namespace Folha360.Application;

public class Result<T>
{
    public bool IsSuccess => Errors.Count == 0;
    public T? Value { get; }
    public List<Error> Errors { get; } = new();

    private Result(T value)
    {
        Value = value;
    }

    private Result(List<Error> errors)
    {
        Errors = errors;
    }

    public static Result<T> Success(T value) => new(value);
    public static Result<T> Failure(string code, string message)
        => new(new List<Error> { new(code, message) });
    public static Result<T> Failure(List<Error> errors) => new(errors);
}

public record Error(string Code, string Message);

public class PaginatedResult<T>
{
    public bool IsSuccess => Errors.Count == 0;
    public IEnumerable<T> Items { get; }
    public int Page { get; }
    public int PageSize { get; }
    public int TotalCount { get; }
    public List<Error> Errors { get; } = new();

    public PaginatedResult(IEnumerable<T> items, int page, int pageSize, int totalCount)
    {
        Items = items;
        Page = page;
        PageSize = pageSize;
        TotalCount = totalCount;
    }

    private PaginatedResult(List<Error> errors)
    {
        Items = Array.Empty<T>();
        Errors = errors;
    }

    public static PaginatedResult<T> Success(IEnumerable<T> items, int page, int pageSize, int totalCount)
        => new(items, page, pageSize, totalCount);

    public static PaginatedResult<T> Failure(string code, string message)
        => new(new List<Error> { new(code, message) });
}
