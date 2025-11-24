using System;

namespace EzChat.Web.App_Code.Models
{
    /// <summary>
    /// 사용자 모델
    /// </summary>
    public class User
    {
        public int UserID { get; set; }
        public string Username { get; set; }
        public string UserLoginID { get; set; }
        public string PasswordHash { get; set; }
        public bool IsAdmin { get; set; }
    }

    /// <summary>
    /// 게시글 모델
    /// </summary>
    public class Post
    {
        public int PostID { get; set; }
        public int UserID { get; set; }
        public string Username { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
