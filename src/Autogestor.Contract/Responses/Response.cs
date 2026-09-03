using System.Runtime.Serialization;

namespace Autogestor.Contract.Responses;

[DataContract]
public sealed record Response<T>
{
    [DataMember(Order = 1)]
    public required T? Data { get; init; }

    [DataMember(Order = 2)]
    public required string? Message { get; init; }
}
