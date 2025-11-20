using System;

namespace EzChat.Web.App_Code.Models
{
    /// <summary>
    /// 사용자 모델
    /// </summary>
    public class User
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string DisplayName { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastLoginAt { get; set; }
    }

    /// <summary>
    /// 채팅방 모델
    /// </summary>
    public class ChatRoom
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int CreatedById { get; set; }
        public string CreatedByName { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; }
        public int MaxUsers { get; set; }
    }

    /// <summary>
    /// 채팅 메시지 모델
    /// </summary>
    public class ChatMessage
    {
        public int Id { get; set; }
        public int RoomId { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Content { get; set; }
        public DateTime SentAt { get; set; }
    }

    /// <summary>
    /// 게시글 모델
    /// </summary>
    public class BoardPost
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public int AuthorId { get; set; }
        public string AuthorName { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int ViewCount { get; set; }
        public int CommentCount { get; set; }
    }

    /// <summary>
    /// 댓글 모델
    /// </summary>
    public class Comment
    {
        public int Id { get; set; }
        public int PostId { get; set; }
        public int AuthorId { get; set; }
        public string AuthorName { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    /// <summary>
    /// IP 차단 모델
    /// </summary>
    public class IpBan
    {
        public int Id { get; set; }
        public string IpAddress { get; set; }
        public string Reason { get; set; }
        public int BannedById { get; set; }
        public string BannedByName { get; set; }
        public DateTime BannedAt { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public bool IsActive { get; set; }
    }

    /// <summary>
    /// 감사 로그 모델
    /// </summary>
    public class AuditLog
    {
        public int Id { get; set; }
        public int AdminId { get; set; }
        public string AdminName { get; set; }
        public string Action { get; set; }
        public string TargetType { get; set; }
        public string TargetId { get; set; }
        public string Details { get; set; }
        public DateTime Timestamp { get; set; }
        public string IpAddress { get; set; }
    }
}
