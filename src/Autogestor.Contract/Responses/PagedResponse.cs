using System.Text.Json.Serialization;

namespace Autogestor.Contract.Responses;

public sealed record PagedResponse<T> : Response<T>
{
    public int TotalCount { get; init; }
    public int CurrentPage { get; init; } = ContractDefaults.DefaultPageNumber;
    public int PageSize { get; init; } = ContractDefaults.DefaultPageSize;
    public int TotalPage => PageSize > 0 ? (int)Math.Ceiling(TotalCount / (decimal)PageSize) : 0;

    [JsonConstructor]
    private PagedResponse() { }

    public PagedResponse(T data, int code, string message,
        int totalCount, int currentPage, int pageSize)
        : base(data, code, message)
    {
        TotalCount = totalCount;
        CurrentPage = currentPage;
        PageSize = pageSize;
    }
}
