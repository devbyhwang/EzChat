using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EzChat.Web.Models;

/// <summary>
/// 채팅방 모델
/// </summary>
public class ChatRoom
{
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Description { get; set; }

    [Required]
    public string CreatedById { get; set; } = string.Empty;

    [ForeignKey(nameof(CreatedById))]
    public virtual ApplicationUser CreatedBy { get; set; } = null!;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public bool IsActive { get; set; } = true;

    public int MaxUsers { get; set; } = 50;

    // Navigation properties
    public virtual ICollection<ChatMessage> Messages { get; set; } = new List<ChatMessage>();
}
