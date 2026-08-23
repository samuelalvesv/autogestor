using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Autogestor.Contract.Requests;

[DataContract]
public abstract record PagedRequest : Request
{
    [DataMember(Order = 2)]
    [Range(ContractDefaults.MinPageNumber, ContractDefaults.MaxPageNumber)]
    public required int PageNumber { get; init; }

    [DataMember(Order = 3)]
    [Range(ContractDefaults.MinPageSize, ContractDefaults.MaxPageSize)]
    public required int PageSize { get; init; }
}
