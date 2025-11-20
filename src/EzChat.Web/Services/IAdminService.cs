using EzChat.Web.Models;

namespace EzChat.Web.Services;

/// <summary>
/// 관리자 서비스 인터페이스
/// </summary>
public interface IAdminService
{
    Task<IEnumerable<ApplicationUser>> GetAllUsersAsync();
    Task<ApplicationUser?> GetUserByIdAsync(string id);
    Task<bool> DeleteUserAsync(string userId, string adminId, string ipAddress);
    Task<bool> ToggleUserActiveAsync(string userId, string adminId, string ipAddress);
    Task<IEnumerable<IpBan>> GetActiveBansAsync();
    Task<bool> BanIpAsync(string ipAddress, string? reason, string adminId, DateTime? expiresAt, string adminIpAddress);
    Task<bool> UnbanIpAsync(int banId, string adminId, string ipAddress);
    Task<IEnumerable<ChatRoom>> GetAllRoomsAsync();
    Task<bool> DeleteRoomAsync(int roomId, string adminId, string ipAddress);
    Task<IEnumerable<AdminAuditLog>> GetAuditLogsAsync(int count = 100);
    Task LogActionAsync(string adminId, string action, string? targetType, string? targetId, string? details, string? ipAddress);
    Task<DashboardStats> GetDashboardStatsAsync();
}

/// <summary>
/// 대시보드 통계 모델
/// </summary>
public class DashboardStats
{
    public int TotalUsers { get; set; }
    public int ActiveUsers { get; set; }
    public int TotalRooms { get; set; }
    public int TotalPosts { get; set; }
    public int TotalBans { get; set; }
    public int TodayLogins { get; set; }
}
