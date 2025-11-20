using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using EzChat.Web.Models;
using EzChat.Web.ViewModels;

namespace EzChat.Web.Controllers;

public class AccountController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly ILogger<AccountController> _logger;

    public AccountController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        ILogger<AccountController> logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _logger = logger;
    }

    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null)
        {
            ModelState.AddModelError(string.Empty, "이메일 또는 비밀번호가 올바르지 않습니다.");
            _logger.LogWarning("Failed login attempt for non-existent user: {Email}", model.Email);
            return View(model);
        }

        if (!user.IsActive)
        {
            ModelState.AddModelError(string.Empty, "비활성화된 계정입니다.");
            _logger.LogWarning("Login attempt for inactive user: {Email}", model.Email);
            return View(model);
        }

        var result = await _signInManager.PasswordSignInAsync(
            user.UserName!,
            model.Password,
            model.RememberMe,
            lockoutOnFailure: true);

        if (result.Succeeded)
        {
            user.LastLoginAt = DateTime.UtcNow;
            await _userManager.UpdateAsync(user);

            _logger.LogInformation("User {Email} logged in successfully", model.Email);

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("Index", "Home");
        }

        if (result.IsLockedOut)
        {
            _logger.LogWarning("User {Email} account locked out", model.Email);
            ModelState.AddModelError(string.Empty, "계정이 잠겼습니다. 잠시 후 다시 시도해주세요.");
            return View(model);
        }

        ModelState.AddModelError(string.Empty, "이메일 또는 비밀번호가 올바르지 않습니다.");
        _logger.LogWarning("Failed login attempt for user: {Email}", model.Email);
        return View(model);
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = new ApplicationUser
        {
            UserName = model.Email,
            Email = model.Email,
            DisplayName = model.DisplayName,
            EmailConfirmed = true // 간단한 프로젝트이므로 이메일 확인 생략
        };

        var result = await _userManager.CreateAsync(user, model.Password);

        if (result.Succeeded)
        {
            await _userManager.AddToRoleAsync(user, "User");
            _logger.LogInformation("User {Email} registered successfully", model.Email);

            await _signInManager.SignInAsync(user, isPersistent: false);
            return RedirectToAction("Index", "Home");
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, GetKoreanErrorMessage(error.Code));
        }

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        _logger.LogInformation("User logged out");
        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public IActionResult AccessDenied()
    {
        return View();
    }

    private static string GetKoreanErrorMessage(string errorCode)
    {
        return errorCode switch
        {
            "DuplicateUserName" => "이미 사용 중인 이메일입니다.",
            "DuplicateEmail" => "이미 사용 중인 이메일입니다.",
            "PasswordTooShort" => "비밀번호는 최소 8자 이상이어야 합니다.",
            "PasswordRequiresDigit" => "비밀번호에는 숫자가 포함되어야 합니다.",
            "PasswordRequiresLower" => "비밀번호에는 소문자가 포함되어야 합니다.",
            "PasswordRequiresUpper" => "비밀번호에는 대문자가 포함되어야 합니다.",
            "PasswordRequiresNonAlphanumeric" => "비밀번호에는 특수문자가 포함되어야 합니다.",
            _ => "오류가 발생했습니다."
        };
    }
}
