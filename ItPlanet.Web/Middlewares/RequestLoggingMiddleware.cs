namespace ItPlanet.Web.Middlewares;

public class RequestLoggingMiddleware
{
    private readonly ILogger<RequestLoggingMiddleware> _logger;
    private readonly RequestDelegate _next;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var correlationId = Guid.NewGuid();
        _logger.LogInformation("Request: {Method} {RequestPath}. CorrelationId: {CorrelationId}",
            context.Request.Method.ToUpper(), context.Request.Path, correlationId);
        await _next.Invoke(context);
        _logger.LogInformation("Response: {StatusCode}. CorrelationId: {CorrelationId}", context.Response.StatusCode,
            correlationId);
    }
}