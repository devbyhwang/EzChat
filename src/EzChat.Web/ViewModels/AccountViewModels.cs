using System.ComponentModel.DataAnnotations;

namespace EzChat.Web.ViewModels;

/// <summary>
/// 로그인 뷰모델
/// </summary>
public class LoginViewModel
{
    [Required(ErrorMessage = "이메일을 입력해주세요.")]
    [EmailAddress(ErrorMessage = "올바른 이메일 형식이 아닙니다.")]
    [Display(Name = "이메일")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "비밀번호를 입력해주세요.")]
    [DataType(DataType.Password)]
    [Display(Name = "비밀번호")]
    public string Password { get; set; } = string.Empty;

    [Display(Name = "로그인 상태 유지")]
    public bool RememberMe { get; set; }
}

/// <summary>
/// 회원가입 뷰모델
/// </summary>
public class RegisterViewModel
{
    [Required(ErrorMessage = "이메일을 입력해주세요.")]
    [EmailAddress(ErrorMessage = "올바른 이메일 형식이 아닙니다.")]
    [Display(Name = "이메일")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "닉네임을 입력해주세요.")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "닉네임은 2자 이상 50자 이하로 입력해주세요.")]
    [Display(Name = "닉네임")]
    public string DisplayName { get; set; } = string.Empty;

    [Required(ErrorMessage = "비밀번호를 입력해주세요.")]
    [StringLength(100, MinimumLength = 8, ErrorMessage = "비밀번호는 최소 8자 이상이어야 합니다.")]
    [DataType(DataType.Password)]
    [Display(Name = "비밀번호")]
    public string Password { get; set; } = string.Empty;

    [DataType(DataType.Password)]
    [Display(Name = "비밀번호 확인")]
    [Compare("Password", ErrorMessage = "비밀번호가 일치하지 않습니다.")]
    public string ConfirmPassword { get; set; } = string.Empty;
}
