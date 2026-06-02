namespace AOT_UI.Middleware;

public class CorrelationIdMiddleware
{
    private const string CorrelationHeader = "X-Correlation-ID";
    private readonly RequestDelegate _next;

    public CorrelationIdMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, ILogger<CorrelationIdMiddleware> logger)
    {
        var correlationId = context.Request.Headers.TryGetValue(CorrelationHeader, out var incoming)
            ? incoming.ToString()
            : Guid.NewGuid().ToString("N");

        context.Response.Headers[CorrelationHeader] = correlationId;
        context.Items[CorrelationHeader] = correlationId;

        using (logger.BeginScope(new Dictionary<string, object>
               {
                   ["CorrelationId"] = correlationId
               }))
        {
            await _next(context);
        }
    }
}
