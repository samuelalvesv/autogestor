namespace Autogestor.Contract.Responses;

public record Response<T>
{
    public required T? Data { get; init; }
    public required string? Message { get; init; }
}
