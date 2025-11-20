using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EzChat.Web.Models;

/// <summary>
/// 게시판 글 모델
/// </summary>
public class BoardPost
{
    public int Id { get; set; }

    [Required]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Content { get; set; } = string.Empty;

    [Required]
    public string AuthorId { get; set; } = string.Empty;

    [ForeignKey(nameof(AuthorId))]
    public virtual ApplicationUser Author { get; set; } = null!;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    public int ViewCount { get; set; } = 0;

    public bool IsDeleted { get; set; } = false;

    // Navigation properties
    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
}
