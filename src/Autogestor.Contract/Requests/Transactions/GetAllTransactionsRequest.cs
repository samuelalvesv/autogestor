using System.Runtime.Serialization;

namespace Autogestor.Contract.Requests.Transactions;

[DataContract]
public sealed record GetAllTransactionsRequest : PagedRequest;
