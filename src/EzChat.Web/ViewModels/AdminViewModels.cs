using System.ComponentModel.DataAnnotations;

namespace EzChat.Web.ViewModels;

/// <summary>
/// IP 차단 뷰모델
/// </summary>
public class BanIpViewModel
{
    [Required(ErrorMessage = "IP 주소를 입력해주세요.")]
    [StringLength(45, ErrorMessage = "IP 주소는 45자 이하로 입력해주세요.")]
    [Display(Name = "IP 주소")]
    public string IpAddress { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "사유는 500자 이하로 입력해주세요.")]
    [Display(Name = "차단 사유")]
    public string? Reason { get; set; }

    [Display(Name = "만료 일시")]
    public DateTime? ExpiresAt { get; set; }
}
