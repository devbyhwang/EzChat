using EzChat.Web.Models;
using EzChat.Web.ViewModels;

namespace EzChat.Web.Services;

/// <summary>
/// 게시판 서비스 인터페이스
/// </summary>
public interface IBoardService
{
    Task<(IEnumerable<BoardPost> Posts, int TotalCount)> GetPostsAsync(int page, int pageSize, string? searchTerm = null);
    Task<BoardPost?> GetPostByIdAsync(int id);
    Task<BoardPost> CreatePostAsync(CreatePostViewModel model, string userId);
    Task<bool> UpdatePostAsync(int id, EditPostViewModel model, string userId);
    Task<bool> DeletePostAsync(int id, string userId, bool isAdmin = false);
    Task IncrementViewCountAsync(int id);
    Task<IEnumerable<Comment>> GetCommentsAsync(int postId);
    Task<Comment> AddCommentAsync(int postId, string content, string userId);
    Task<bool> DeleteCommentAsync(int commentId, string userId, bool isAdmin = false);
}
