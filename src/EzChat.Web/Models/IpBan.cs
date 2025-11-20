using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EzChat.Web.Models;

/// <summary>
/// IP 차단 모델
/// </summary>
public class IpBan
{
    public int Id { get; set; }

    [Required]
    [StringLength(45)] // IPv6 지원
    public string IpAddress { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Reason { get; set; }

    [Required]
    public string BannedById { get; set; } = string.Empty;

    [ForeignKey(nameof(BannedById))]
    public virtual ApplicationUser BannedBy { get; set; } = null!;

    public DateTime BannedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// 만료 일시 (null이면 영구 차단)
    /// </summary>
    public DateTime? ExpiresAt { get; set; }

    public bool IsActive { get; set; } = true;
}
