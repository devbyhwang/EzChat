using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using EzChat.Web.Models;
using EzChat.Web.Services;
using EzChat.Web.ViewModels;

namespace EzChat.Web.Controllers;

[Authorize]
public class ChatController : Controller
{
    private readonly IChatService _chatService;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<ChatController> _logger;

    public ChatController(
        IChatService chatService,
        UserManager<ApplicationUser> userManager,
        ILogger<ChatController> logger)
    {
        _chatService = chatService;
        _userManager = userManager;
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        var rooms = await _chatService.GetActiveRoomsAsync();
        return View(rooms);
    }

    public async Task<IActionResult> Room(int id)
    {
        var room = await _chatService.GetRoomByIdAsync(id);

        if (room == null)
        {
            return NotFound();
        }

        var messages = await _chatService.GetRecentMessagesAsync(id);
        ViewBag.Messages = messages;

        return View(room);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateRoomViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var userId = _userManager.GetUserId(User)!;
        var room = await _chatService.CreateRoomAsync(model, userId);

        TempData["Success"] = "채팅방이 생성되었습니다.";
        return RedirectToAction(nameof(Room), new { id = room.Id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var userId = _userManager.GetUserId(User)!;
        var isAdmin = User.IsInRole("Admin");

        var result = await _chatService.DeleteRoomAsync(id, userId, isAdmin);

        if (!result)
        {
            TempData["Error"] = "채팅방 삭제에 실패했습니다.";
            return RedirectToAction(nameof(Room), new { id });
        }

        TempData["Success"] = "채팅방이 삭제되었습니다.";
        return RedirectToAction(nameof(Index));
    }
}
