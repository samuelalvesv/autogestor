using System.ComponentModel.DataAnnotations;

namespace Autogestor.Contract.Requests;

public abstract record PagedRequest : Request
{
    [Range(ContractDefaults.MinPageNumber, ContractDefaults.MaxPageNumber)]
    public int PageNumber { get; init; } = ContractDefaults.DefaultPageNumber;

    [Range(ContractDefaults.MinPageSize, ContractDefaults.MaxPageSize)]
    public int PageSize { get; init; } = ContractDefaults.DefaultPageSize;
}
