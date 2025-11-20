using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace EzChat.Web.Models;

/// <summary>
/// 애플리케이션 사용자 모델 (Identity 확장)
/// </summary>
public class ApplicationUser : IdentityUser
{
    [StringLength(50)]
    public string? DisplayName { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? LastLoginAt { get; set; }

    public bool IsActive { get; set; } = true;

    // Navigation properties
    public virtual ICollection<ChatRoom> CreatedRooms { get; set; } = new List<ChatRoom>();
    public virtual ICollection<ChatMessage> Messages { get; set; } = new List<ChatMessage>();
    public virtual ICollection<BoardPost> Posts { get; set; } = new List<BoardPost>();
    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
}
