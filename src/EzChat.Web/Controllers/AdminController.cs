using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using EzChat.Web.Models;
using EzChat.Web.Services;
using EzChat.Web.ViewModels;

namespace EzChat.Web.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly IAdminService _adminService;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<AdminController> _logger;

    public AdminController(
        IAdminService adminService,
        UserManager<ApplicationUser> userManager,
        ILogger<AdminController> logger)
    {
        _adminService = adminService;
        _userManager = userManager;
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        var stats = await _adminService.GetDashboardStatsAsync();
        return View(stats);
    }

    public async Task<IActionResult> Users()
    {
        var users = await _adminService.GetAllUsersAsync();
        return View(users);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteUser(string id)
    {
        var adminId = _userManager.GetUserId(User)!;
        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";

        var result = await _adminService.DeleteUserAsync(id, adminId, ipAddress);

        if (result)
        {
            TempData["Success"] = "사용자가 삭제되었습니다.";
        }
        else
        {
            TempData["Error"] = "사용자 삭제에 실패했습니다.";
        }

        return RedirectToAction(nameof(Users));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleUserActive(string id)
    {
        var adminId = _userManager.GetUserId(User)!;
        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";

        var result = await _adminService.ToggleUserActiveAsync(id, adminId, ipAddress);

        if (result)
        {
            TempData["Success"] = "사용자 상태가 변경되었습니다.";
        }
        else
        {
            TempData["Error"] = "사용자 상태 변경에 실패했습니다.";
        }

        return RedirectToAction(nameof(Users));
    }

    public async Task<IActionResult> IpBans()
    {
        var bans = await _adminService.GetActiveBansAsync();
        return View(bans);
    }

    [HttpGet]
    public IActionResult BanIp()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> BanIp(BanIpViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var adminId = _userManager.GetUserId(User)!;
        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";

        var result = await _adminService.BanIpAsync(
            model.IpAddress,
            model.Reason,
            adminId,
            model.ExpiresAt,
            ipAddress);

        if (result)
        {
            TempData["Success"] = $"IP {model.IpAddress}가 차단되었습니다.";
            return RedirectToAction(nameof(IpBans));
        }

        ModelState.AddModelError(string.Empty, "이미 차단된 IP입니다.");
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UnbanIp(int id)
    {
        var adminId = _userManager.GetUserId(User)!;
        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";

        var result = await _adminService.UnbanIpAsync(id, adminId, ipAddress);

        if (result)
        {
            TempData["Success"] = "IP 차단이 해제되었습니다.";
        }
        else
        {
            TempData["Error"] = "IP 차단 해제에 실패했습니다.";
        }

        return RedirectToAction(nameof(IpBans));
    }

    public async Task<IActionResult> Rooms()
    {
        var rooms = await _adminService.GetAllRoomsAsync();
        return View(rooms);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteRoom(int id)
    {
        var adminId = _userManager.GetUserId(User)!;
        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";

        var result = await _adminService.DeleteRoomAsync(id, adminId, ipAddress);

        if (result)
        {
            TempData["Success"] = "채팅방이 삭제되었습니다.";
        }
        else
        {
            TempData["Error"] = "채팅방 삭제에 실패했습니다.";
        }

        return RedirectToAction(nameof(Rooms));
    }

    public async Task<IActionResult> AuditLogs()
    {
        var logs = await _adminService.GetAuditLogsAsync();
        return View(logs);
    }
}
