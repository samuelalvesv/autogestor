namespace Autogestor.Contract.Requests;

public abstract record Request
{
    public required Guid UserId { get; init; }
}
