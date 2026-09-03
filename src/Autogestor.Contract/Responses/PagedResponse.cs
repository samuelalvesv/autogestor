using System.Runtime.Serialization;

namespace Autogestor.Contract.Responses;

[DataContract]
public sealed record PagedResponse<T>
{
    [DataMember(Order = 1)]
    public required IEnumerable<T>? Data { get; init; }

    [DataMember(Order = 2)]
    public required string? Message { get; init; }

    [DataMember(Order = 3)]
    public required int TotalCount { get; init; }

    [DataMember(Order = 4)]
    public required int CurrentPage { get; init; }

    [DataMember(Order = 5)]
    public required int PageSize { get; init; }

    public int TotalPage => PageSize > 0 ? (int)Math.Ceiling(TotalCount / (decimal)PageSize) : 0;
}
