using System.Text.Json.Serialization;

namespace IMSBackend.Contracts;

public sealed record ApiResponse<T>(
    bool Success,
    T? Data,
    string? Message,
    IReadOnlyDictionary<string, string[]>? Errors = null,
    string? TraceId = null)
{
    [JsonPropertyName("error")]
    public string? Error => Message;

    public static ApiResponse<T> Ok(T? data, string? message = null, string? traceId = null)
        => new(true, data, message, null, traceId);

    public static ApiResponse<T> Fail(
        string message,
        IReadOnlyDictionary<string, string[]>? errors = null,
        string? traceId = null)
        => new(false, default, message, errors ?? new Dictionary<string, string[]>(), traceId);
}
