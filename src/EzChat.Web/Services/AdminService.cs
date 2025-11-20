using Microsoft.EntityFrameworkCore;
using EzChat.Web.Data;
using EzChat.Web.Models;

namespace EzChat.Web.Services;

/// <summary>
/// 관리자 서비스 구현
/// </summary>
public class AdminService : IAdminService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<AdminService> _logger;

    public AdminService(ApplicationDbContext context, ILogger<AdminService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IEnumerable<ApplicationUser>> GetAllUsersAsync()
    {
        return await _context.Users
            .OrderByDescending(u => u.CreatedAt)
            .ToListAsync();
    }

    public async Task<ApplicationUser?> GetUserByIdAsync(string id)
    {
        return await _context.Users.FindAsync(id);
    }

    public async Task<bool> DeleteUserAsync(string userId, string adminId, string ipAddress)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null) return false;

        // 관리자 자신은 삭제할 수 없음
        if (userId == adminId)
        {
            _logger.LogWarning("Admin {AdminId} attempted to delete themselves", adminId);
            return false;
        }

        // 소프트 삭제 (계정 비활성화)
        user.IsActive = false;
        user.Email = $"deleted_{user.Id}@deleted.local";
        user.NormalizedEmail = user.Email.ToUpper();
        user.UserName = $"deleted_{user.Id}";
        user.NormalizedUserName = user.UserName.ToUpper();

        await LogActionAsync(adminId, "DeleteUser", "User", userId, $"Deleted user: {user.DisplayName}", ipAddress);
        await _context.SaveChangesAsync();

        _logger.LogInformation("User {UserId} deleted by admin {AdminId}", userId, adminId);
        return true;
    }

    public async Task<bool> ToggleUserActiveAsync(string userId, string adminId, string ipAddress)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null) return false;

        user.IsActive = !user.IsActive;

        var action = user.IsActive ? "ActivateUser" : "DeactivateUser";
        await LogActionAsync(adminId, action, "User", userId, null, ipAddress);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<IEnumerable<IpBan>> GetActiveBansAsync()
    {
        return await _context.IpBans
            .Include(b => b.BannedBy)
            .Where(b => b.IsActive && (b.ExpiresAt == null || b.ExpiresAt > DateTime.UtcNow))
            .OrderByDescending(b => b.BannedAt)
            .ToListAsync();
    }

    public async Task<bool> BanIpAsync(string ipAddress, string? reason, string adminId, DateTime? expiresAt, string adminIpAddress)
    {
        // 기존 활성 차단 확인
        var existingBan = await _context.IpBans
            .FirstOrDefaultAsync(b => b.IpAddress == ipAddress && b.IsActive);

        if (existingBan != null)
        {
            _logger.LogWarning("IP {IpAddress} is already banned", ipAddress);
            return false;
        }

        var ban = new IpBan
        {
            IpAddress = ipAddress,
            Reason = reason,
            BannedById = adminId,
            ExpiresAt = expiresAt
        };

        _context.IpBans.Add(ban);
        await LogActionAsync(adminId, "BanIp", "IpBan", ipAddress, reason, adminIpAddress);
        await _context.SaveChangesAsync();

        _logger.LogInformation("IP {IpAddress} banned by admin {AdminId}", ipAddress, adminId);
        return true;
    }

    public async Task<bool> UnbanIpAsync(int banId, string adminId, string ipAddress)
    {
        var ban = await _context.IpBans.FindAsync(banId);
        if (ban == null) return false;

        ban.IsActive = false;

        await LogActionAsync(adminId, "UnbanIp", "IpBan", ban.IpAddress, null, ipAddress);
        await _context.SaveChangesAsync();

        _logger.LogInformation("IP {IpAddress} unbanned by admin {AdminId}", ban.IpAddress, adminId);
        return true;
    }

    public async Task<IEnumerable<ChatRoom>> GetAllRoomsAsync()
    {
        return await _context.ChatRooms
            .Include(r => r.CreatedBy)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
    }

    public async Task<bool> DeleteRoomAsync(int roomId, string adminId, string ipAddress)
    {
        var room = await _context.ChatRooms.FindAsync(roomId);
        if (room == null) return false;

        room.IsActive = false;

        await LogActionAsync(adminId, "DeleteRoom", "ChatRoom", roomId.ToString(), $"Deleted room: {room.Name}", ipAddress);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Room {RoomId} deleted by admin {AdminId}", roomId, adminId);
        return true;
    }

    public async Task<IEnumerable<AdminAuditLog>> GetAuditLogsAsync(int count = 100)
    {
        return await _context.AdminAuditLogs
            .Include(l => l.Admin)
            .OrderByDescending(l => l.Timestamp)
            .Take(count)
            .ToListAsync();
    }

    public async Task LogActionAsync(string adminId, string action, string? targetType, string? targetId, string? details, string? ipAddress)
    {
        var log = new AdminAuditLog
        {
            AdminId = adminId,
            Action = action,
            TargetType = targetType,
            TargetId = targetId,
            Details = details,
            IpAddress = ipAddress
        };

        _context.AdminAuditLogs.Add(log);
        await _context.SaveChangesAsync();
    }

    public async Task<DashboardStats> GetDashboardStatsAsync()
    {
        var today = DateTime.UtcNow.Date;

        return new DashboardStats
        {
            TotalUsers = await _context.Users.CountAsync(),
            ActiveUsers = await _context.Users.CountAsync(u => u.IsActive),
            TotalRooms = await _context.ChatRooms.CountAsync(r => r.IsActive),
            TotalPosts = await _context.BoardPosts.CountAsync(p => !p.IsDeleted),
            TotalBans = await _context.IpBans.CountAsync(b => b.IsActive),
            TodayLogins = await _context.Users.CountAsync(u => u.LastLoginAt >= today)
        };
    }
}
