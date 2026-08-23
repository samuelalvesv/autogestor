using System.Text.Json.Serialization;

namespace Autogestor.Contract.Responses;

public record Response<T>
{
    private readonly int _code;

    public T? Data { get; init; }
    public string Message { get; init; } = string.Empty;
    [JsonIgnore]
    public bool IsSuccess => _code is >= 200 and <= 299;

    [JsonConstructor]
    protected Response() { }

    public Response(T data, int code, string message)
    {
        Data = data;
        Message = message;
        _code = code;
    }
}
