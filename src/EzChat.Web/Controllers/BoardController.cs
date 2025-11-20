using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using EzChat.Web.Models;
using EzChat.Web.Services;
using EzChat.Web.ViewModels;

namespace EzChat.Web.Controllers;

public class BoardController : Controller
{
    private readonly IBoardService _boardService;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<BoardController> _logger;
    private const int PageSize = 10;

    public BoardController(
        IBoardService boardService,
        UserManager<ApplicationUser> userManager,
        ILogger<BoardController> logger)
    {
        _boardService = boardService;
        _userManager = userManager;
        _logger = logger;
    }

    public async Task<IActionResult> Index(int page = 1, string? search = null)
    {
        var (posts, totalCount) = await _boardService.GetPostsAsync(page, PageSize, search);

        ViewBag.CurrentPage = page;
        ViewBag.TotalPages = (int)Math.Ceiling(totalCount / (double)PageSize);
        ViewBag.SearchTerm = search;

        return View(posts);
    }

    public async Task<IActionResult> Detail(int id)
    {
        var post = await _boardService.GetPostByIdAsync(id);

        if (post == null)
        {
            return NotFound();
        }

        await _boardService.IncrementViewCountAsync(id);
        return View(post);
    }

    [HttpGet]
    [Authorize]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreatePostViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var userId = _userManager.GetUserId(User)!;
        var post = await _boardService.CreatePostAsync(model, userId);

        TempData["Success"] = "게시글이 작성되었습니다.";
        return RedirectToAction(nameof(Detail), new { id = post.Id });
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> Edit(int id)
    {
        var post = await _boardService.GetPostByIdAsync(id);

        if (post == null)
        {
            return NotFound();
        }

        var userId = _userManager.GetUserId(User);
        if (post.AuthorId != userId)
        {
            return Forbid();
        }

        var model = new EditPostViewModel
        {
            Id = post.Id,
            Title = post.Title,
            Content = post.Content
        };

        return View(model);
    }

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, EditPostViewModel model)
    {
        if (id != model.Id)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var userId = _userManager.GetUserId(User)!;
        var result = await _boardService.UpdatePostAsync(id, model, userId);

        if (!result)
        {
            return NotFound();
        }

        TempData["Success"] = "게시글이 수정되었습니다.";
        return RedirectToAction(nameof(Detail), new { id });
    }

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var userId = _userManager.GetUserId(User)!;
        var isAdmin = User.IsInRole("Admin");

        var result = await _boardService.DeletePostAsync(id, userId, isAdmin);

        if (!result)
        {
            TempData["Error"] = "게시글 삭제에 실패했습니다.";
            return RedirectToAction(nameof(Detail), new { id });
        }

        TempData["Success"] = "게시글이 삭제되었습니다.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddComment(CreateCommentViewModel model)
    {
        if (!ModelState.IsValid)
        {
            TempData["Error"] = "댓글 내용을 입력해주세요.";
            return RedirectToAction(nameof(Detail), new { id = model.PostId });
        }

        var userId = _userManager.GetUserId(User)!;
        await _boardService.AddCommentAsync(model.PostId, model.Content, userId);

        TempData["Success"] = "댓글이 작성되었습니다.";
        return RedirectToAction(nameof(Detail), new { id = model.PostId });
    }

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteComment(int id, int postId)
    {
        var userId = _userManager.GetUserId(User)!;
        var isAdmin = User.IsInRole("Admin");

        var result = await _boardService.DeleteCommentAsync(id, userId, isAdmin);

        if (!result)
        {
            TempData["Error"] = "댓글 삭제에 실패했습니다.";
        }
        else
        {
            TempData["Success"] = "댓글이 삭제되었습니다.";
        }

        return RedirectToAction(nameof(Detail), new { id = postId });
    }
}
