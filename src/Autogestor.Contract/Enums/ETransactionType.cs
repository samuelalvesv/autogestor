using System.Runtime.Serialization;

namespace Autogestor.Contract.Enums;

[DataContract]
public enum ETransactionType
{
    [EnumMember]
    Deposit = 1,

    [EnumMember]
    Withdraw = 2
}
