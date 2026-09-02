using System.Runtime.Serialization;

namespace Autogestor.Contract.Requests.Categories;

[DataContract]
public sealed record GetAllCategoriesRequest : PagedRequest;
