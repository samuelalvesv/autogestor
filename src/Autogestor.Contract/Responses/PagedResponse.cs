using System.Runtime.Serialization;

namespace Autogestor.Contract.Responses;

[DataContract]
public sealed record PagedResponse<T>
{
    [DataMember(Order = 1)]
    public required IReadOnlyList<T> Data { get; init; }

    [DataMember(Order = 2)]
    public required bool HasNextPage { get; init; }

    [DataMember(Order = 3)]
    public required Guid? NextCursor { get; init; }
}
