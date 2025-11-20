using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EzChat.Web.Models;

/// <summary>
/// 댓글 모델
/// </summary>
public class Comment
{
    public int Id { get; set; }

    public int PostId { get; set; }

    [ForeignKey(nameof(PostId))]
    public virtual BoardPost Post { get; set; } = null!;

    [Required]
    public string AuthorId { get; set; } = string.Empty;

    [ForeignKey(nameof(AuthorId))]
    public virtual ApplicationUser Author { get; set; } = null!;

    [Required]
    [StringLength(1000)]
    public string Content { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public bool IsDeleted { get; set; } = false;
}
