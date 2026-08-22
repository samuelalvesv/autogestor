using System.ComponentModel.DataAnnotations;

namespace Autogestor.Contracts.Requests;

public abstract class PagedRequest : Request
{
    [Range(ContractDefaults.MinPageNumber, ContractDefaults.MaxPageNumber)]
    public int PageNumber { get; set; } = ContractDefaults.DefaultPageNumber;

    [Range(ContractDefaults.MinPageSize, ContractDefaults.MaxPageSize)]
    public int PageSize { get; set; } = ContractDefaults.DefaultPageSize;
}
