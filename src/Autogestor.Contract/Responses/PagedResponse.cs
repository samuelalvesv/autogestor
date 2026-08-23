namespace Autogestor.Contract.Responses;

public sealed record PagedResponse<T> : Response<T>
{
    public required int TotalCount { get; init; }
    public required int CurrentPage { get; init; } = ContractDefaults.DefaultPageNumber;
    public required int PageSize { get; init; } = ContractDefaults.DefaultPageSize;
    public int TotalPage => PageSize > 0 ? (int)Math.Ceiling(TotalCount / (decimal)PageSize) : 0;
}
