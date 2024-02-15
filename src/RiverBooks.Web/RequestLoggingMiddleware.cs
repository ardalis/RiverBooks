using Serilog;

/// <summary>
/// Source: https://blog.elmah.io/asp-net-core-request-logging-middleware/
/// </summary>
public class RequestLoggingMiddleware
{
  private readonly RequestDelegate _next;
  private readonly ILogger<RequestLoggingMiddleware> _logger;

  public RequestLoggingMiddleware(RequestDelegate next, ILoggerFactory loggerFactory)
  {
    _next = next;
    _logger = loggerFactory.CreateLogger<RequestLoggingMiddleware>();
  }

#pragma warning disable IDE1006 // Naming Styles
  public async Task Invoke(HttpContext context)
#pragma warning restore IDE1006 // Naming Styles
  {
    _logger.LogInformation(
        "Request {method} {url} => {statusCode}",
        context.Request?.Method,
        context.Request?.Path.Value,
        context.Response?.StatusCode);
    await _next(context);
  }
}
