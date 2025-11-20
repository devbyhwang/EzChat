using Microsoft.EntityFrameworkCore;
using EzChat.Web.Data;

namespace EzChat.Web.Services;

/// <summary>
/// IP 차단 서비스 구현
/// </summary>
public class IpBanService : IIpBanService
{
    private readonly ApplicationDbContext _context;

    public IpBanService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> IsIpBannedAsync(string? ipAddress)
    {
        if (string.IsNullOrEmpty(ipAddress))
            return false;

        return await _context.IpBans
            .AnyAsync(b => b.IpAddress == ipAddress
                && b.IsActive
                && (b.ExpiresAt == null || b.ExpiresAt > DateTime.UtcNow));
    }
}
