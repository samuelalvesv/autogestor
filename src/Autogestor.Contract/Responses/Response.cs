namespace Autogestor.Contract.Responses;

public class Response(string data, string message, int code)
{
    public string? Data { get; set; } = data;
    public string Message { get; set; } = message;
    public bool IsSuccess => code is >= 200 and <= 299;
}
