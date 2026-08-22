using System.Text.Json.Serialization;

namespace Autogestor.Contract.Responses;

public class PagedResponse<T> : Response<T>
{
    public int TotalCount { get; set; }
    public int CurrentPage { get; set; } = ContractDefaults.DefaultPageNumber;
    public int PageSize { get; set; } = ContractDefaults.DefaultPageSize;
    public int TotalPage => (int)Math.Ceiling(TotalCount / (decimal)PageSize);

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
