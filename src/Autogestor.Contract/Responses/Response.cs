using System.Text.Json.Serialization;

namespace Autogestor.Contract.Responses;

public class Response<T>
{
    private readonly int _code;

    public T? Data { get; set; }
    public string Message { get; set; }
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
