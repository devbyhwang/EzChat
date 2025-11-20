using System.ComponentModel.DataAnnotations;

namespace EzChat.Web.ViewModels;

/// <summary>
/// 게시글 생성 뷰모델
/// </summary>
public class CreatePostViewModel
{
    [Required(ErrorMessage = "제목을 입력해주세요.")]
    [StringLength(200, MinimumLength = 1, ErrorMessage = "제목은 1자 이상 200자 이하로 입력해주세요.")]
    [Display(Name = "제목")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "내용을 입력해주세요.")]
    [Display(Name = "내용")]
    public string Content { get; set; } = string.Empty;
}

/// <summary>
/// 게시글 수정 뷰모델
/// </summary>
public class EditPostViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "제목을 입력해주세요.")]
    [StringLength(200, MinimumLength = 1, ErrorMessage = "제목은 1자 이상 200자 이하로 입력해주세요.")]
    [Display(Name = "제목")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "내용을 입력해주세요.")]
    [Display(Name = "내용")]
    public string Content { get; set; } = string.Empty;
}

/// <summary>
/// 댓글 생성 뷰모델
/// </summary>
public class CreateCommentViewModel
{
    public int PostId { get; set; }

    [Required(ErrorMessage = "댓글 내용을 입력해주세요.")]
    [StringLength(1000, ErrorMessage = "댓글은 1000자 이하로 입력해주세요.")]
    [Display(Name = "댓글")]
    public string Content { get; set; } = string.Empty;
}
