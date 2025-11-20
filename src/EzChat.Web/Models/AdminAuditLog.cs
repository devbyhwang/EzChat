using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EzChat.Web.Models;

/// <summary>
/// 관리자 감사 로그 모델
/// </summary>
public class AdminAuditLog
{
    public int Id { get; set; }

    [Required]
    public string AdminId { get; set; } = string.Empty;

    [ForeignKey(nameof(AdminId))]
    public virtual ApplicationUser Admin { get; set; } = null!;

    [Required]
    [StringLength(100)]
    public string Action { get; set; } = string.Empty;

    [StringLength(50)]
    public string? TargetType { get; set; }

    [StringLength(100)]
    public string? TargetId { get; set; }

    [StringLength(1000)]
    public string? Details { get; set; }

    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    [StringLength(45)]
    public string? IpAddress { get; set; }
}
