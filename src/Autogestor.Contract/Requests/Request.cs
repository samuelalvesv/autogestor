namespace Autogestor.Contract.Requests;

public abstract record Request
{
    public Guid UserId { get; init; }
}
