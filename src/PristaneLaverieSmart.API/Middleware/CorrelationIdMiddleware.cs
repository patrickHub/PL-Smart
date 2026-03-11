namespace PristaneLaverieSmart.API.Middleware;

public sealed class CorrelationIdMiddleware : IMiddleware
{

    public const string HeaderName = "X-Correlation-ID";
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var correlationId =
            context.Request.Headers.TryGetValue(HeaderName, out var fromHeader) && !string.IsNullOrWhiteSpace(fromHeader)
                ? fromHeader.ToString()
                : Guid.NewGuid().ToString("N");
        context.Items[HeaderName] = correlationId;

        // return it to client for tracing
        context.Response.OnStarting(() =>
        {
            context.Response.Headers[HeaderName] = correlationId;
            return Task.CompletedTask;
        });

        await next(context);
    }
}