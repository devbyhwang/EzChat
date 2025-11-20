using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EzChat.Web.Models;

/// <summary>
/// 채팅 메시지 모델
/// </summary>
public class ChatMessage
{
    public int Id { get; set; }

    public int RoomId { get; set; }

    [ForeignKey(nameof(RoomId))]
    public virtual ChatRoom Room { get; set; } = null!;

    [Required]
    public string UserId { get; set; } = string.Empty;

    [ForeignKey(nameof(UserId))]
    public virtual ApplicationUser User { get; set; } = null!;

    [Required]
    [StringLength(2000)]
    public string Content { get; set; } = string.Empty;

    public DateTime SentAt { get; set; } = DateTime.UtcNow;

    public bool IsDeleted { get; set; } = false;
}
