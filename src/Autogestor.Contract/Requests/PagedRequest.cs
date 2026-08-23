using System.ComponentModel.DataAnnotations;

namespace Autogestor.Contract.Requests;

public abstract record PagedRequest : Request
{
    [Range(ContractDefaults.MinPageNumber, ContractDefaults.MaxPageNumber)]
    public required int PageNumber { get; init; } = ContractDefaults.DefaultPageNumber;

    [Range(ContractDefaults.MinPageSize, ContractDefaults.MaxPageSize)]
    public required int PageSize { get; init; } = ContractDefaults.DefaultPageSize;
}
