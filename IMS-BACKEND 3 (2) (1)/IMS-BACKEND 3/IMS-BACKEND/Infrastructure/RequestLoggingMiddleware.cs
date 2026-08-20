using System.Diagnostics;

namespace IMSBackend.Infrastructure;

public sealed class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(
        RequestDelegate next,
        ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var watch = Stopwatch.StartNew();

        await _next(context);

        watch.Stop();

        _logger.LogInformation(
            "{Method} {Path} => {StatusCode} ({Elapsed} ms)",
            context.Request.Method,
            context.Request.Path,
            context.Response.StatusCode,
            watch.ElapsedMilliseconds);
    }
}