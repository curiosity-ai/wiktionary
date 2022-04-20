// See https://aka.ms/new-console-template for more information
public sealed class AddCacheHeaderMiddleware
{
    private readonly RequestDelegate _next;

    public AddCacheHeaderMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext httpContext)
    {
        if (httpContext.Response.HasStarted)
        {
            throw new InvalidOperationException("Can't mutate response after headers have been sent to client.");
        }
        httpContext.Response.Headers.CacheControl = new[] { "public", "max-age=2592000" };
        await _next(httpContext);
    }
}