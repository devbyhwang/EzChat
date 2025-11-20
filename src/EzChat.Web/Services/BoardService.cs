using Microsoft.EntityFrameworkCore;
using Ganss.Xss;
using EzChat.Web.Data;
using EzChat.Web.Models;
using EzChat.Web.ViewModels;

namespace EzChat.Web.Services;

/// <summary>
/// 게시판 서비스 구현
/// </summary>
public class BoardService : IBoardService
{
    private readonly ApplicationDbContext _context;
    private readonly HtmlSanitizer _sanitizer;
    private readonly ILogger<BoardService> _logger;

    public BoardService(ApplicationDbContext context, ILogger<BoardService> logger)
    {
        _context = context;
        _logger = logger;
        _sanitizer = new HtmlSanitizer();
    }

    public async Task<(IEnumerable<BoardPost> Posts, int TotalCount)> GetPostsAsync(int page, int pageSize, string? searchTerm = null)
    {
        var query = _context.BoardPosts
            .Include(p => p.Author)
            .Where(p => !p.IsDeleted);

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(p => p.Title.Contains(searchTerm) || p.Content.Contains(searchTerm));
        }

        var totalCount = await query.CountAsync();

        var posts = await query
            .OrderByDescending(p => p.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (posts, totalCount);
    }

    public async Task<BoardPost?> GetPostByIdAsync(int id)
    {
        return await _context.BoardPosts
            .Include(p => p.Author)
            .Include(p => p.Comments.Where(c => !c.IsDeleted))
                .ThenInclude(c => c.Author)
            .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);
    }

    public async Task<BoardPost> CreatePostAsync(CreatePostViewModel model, string userId)
    {
        var post = new BoardPost
        {
            Title = _sanitizer.Sanitize(model.Title),
            Content = _sanitizer.Sanitize(model.Content),
            AuthorId = userId
        };

        _context.BoardPosts.Add(post);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Post {PostId} created by user {UserId}", post.Id, userId);
        return post;
    }

    public async Task<bool> UpdatePostAsync(int id, EditPostViewModel model, string userId)
    {
        var post = await _context.BoardPosts.FindAsync(id);
        if (post == null || post.IsDeleted) return false;

        // 작성자만 수정 가능
        if (post.AuthorId != userId) return false;

        post.Title = _sanitizer.Sanitize(model.Title);
        post.Content = _sanitizer.Sanitize(model.Content);
        post.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        _logger.LogInformation("Post {PostId} updated by user {UserId}", id, userId);
        return true;
    }

    public async Task<bool> DeletePostAsync(int id, string userId, bool isAdmin = false)
    {
        var post = await _context.BoardPosts.FindAsync(id);
        if (post == null || post.IsDeleted) return false;

        // 작성자 또는 관리자만 삭제 가능
        if (post.AuthorId != userId && !isAdmin) return false;

        post.IsDeleted = true;
        await _context.SaveChangesAsync();

        _logger.LogInformation("Post {PostId} deleted by user {UserId}", id, userId);
        return true;
    }

    public async Task IncrementViewCountAsync(int id)
    {
        var post = await _context.BoardPosts.FindAsync(id);
        if (post != null)
        {
            post.ViewCount++;
            await _context.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<Comment>> GetCommentsAsync(int postId)
    {
        return await _context.Comments
            .Include(c => c.Author)
            .Where(c => c.PostId == postId && !c.IsDeleted)
            .OrderBy(c => c.CreatedAt)
            .ToListAsync();
    }

    public async Task<Comment> AddCommentAsync(int postId, string content, string userId)
    {
        var comment = new Comment
        {
            PostId = postId,
            Content = _sanitizer.Sanitize(content),
            AuthorId = userId
        };

        _context.Comments.Add(comment);
        await _context.SaveChangesAsync();

        // Author 정보를 포함하여 반환
        await _context.Entry(comment).Reference(c => c.Author).LoadAsync();

        _logger.LogInformation("Comment {CommentId} added to post {PostId} by user {UserId}", comment.Id, postId, userId);
        return comment;
    }

    public async Task<bool> DeleteCommentAsync(int commentId, string userId, bool isAdmin = false)
    {
        var comment = await _context.Comments.FindAsync(commentId);
        if (comment == null || comment.IsDeleted) return false;

        // 작성자 또는 관리자만 삭제 가능
        if (comment.AuthorId != userId && !isAdmin) return false;

        comment.IsDeleted = true;
        await _context.SaveChangesAsync();

        _logger.LogInformation("Comment {CommentId} deleted by user {UserId}", commentId, userId);
        return true;
    }
}
