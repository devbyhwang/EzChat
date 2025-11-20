using System.ComponentModel.DataAnnotations;

namespace EzChat.Web.ViewModels;

/// <summary>
/// 채팅방 생성 뷰모델
/// </summary>
public class CreateRoomViewModel
{
    [Required(ErrorMessage = "채팅방 이름을 입력해주세요.")]
    [StringLength(100, MinimumLength = 1, ErrorMessage = "채팅방 이름은 1자 이상 100자 이하로 입력해주세요.")]
    [Display(Name = "채팅방 이름")]
    public string Name { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "설명은 500자 이하로 입력해주세요.")]
    [Display(Name = "설명")]
    public string? Description { get; set; }

    [Range(2, 100, ErrorMessage = "최대 인원은 2명 이상 100명 이하로 설정해주세요.")]
    [Display(Name = "최대 인원")]
    public int MaxUsers { get; set; } = 50;
}

/// <summary>
/// 채팅 메시지 뷰모델
/// </summary>
public class ChatMessageViewModel
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTime SentAt { get; set; }
}
