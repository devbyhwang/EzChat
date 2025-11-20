namespace EzChat.Web.Services;

/// <summary>
/// IP 차단 서비스 인터페이스
/// </summary>
public interface IIpBanService
{
    Task<bool> IsIpBannedAsync(string? ipAddress);
}
