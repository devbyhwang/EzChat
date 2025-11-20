using EzChat.Web.Services;

namespace EzChat.Web.Middleware;

/// <summary>
/// IP 차단 미들웨어
/// </summary>
public class IpBanMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<IpBanMiddleware> _logger;

    public IpBanMiddleware(RequestDelegate next, ILogger<IpBanMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, IIpBanService banService)
    {
        var ipAddress = context.Connection.RemoteIpAddress?.ToString();

        if (await banService.IsIpBannedAsync(ipAddress))
        {
            _logger.LogWarning("Blocked request from banned IP: {IpAddress}", ipAddress);

            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            context.Response.ContentType = "text/html; charset=utf-8";
            await context.Response.WriteAsync("<h1>접근이 거부되었습니다.</h1><p>귀하의 IP 주소가 차단되었습니다.</p>");
            return;
        }

        await _next(context);
    }
}
