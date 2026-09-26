using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Autogestor.Contract.Requests;

[DataContract]
public abstract record PagedRequest
{
    [DataMember(Order = 1)]
    public Guid? Cursor { get; init; }

    [DataMember(Order = 2)]
    [Range(minimum: ContractDefaults.MinPageSize, maximum: ContractDefaults.MaxPageSize)]
    public required int PageSize { get; init; }
}
