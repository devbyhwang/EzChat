using System;
using System.Data;
using System.Data.SqlClient;
using EzChat.Web.App_Code.DAL;
using EzChat.Web.App_Code.Models;

namespace EzChat.Web.App_Code.BLL
{
    /// <summary>
    /// 게시판 비즈니스 로직
    /// </summary>
    public static class BoardBLL
    {
        /// <summary>
        /// 게시글 목록 조회 (페이지네이션)
        /// </summary>
        public static DataTable GetPosts(int page, int pageSize, string searchTerm = null)
        {
            int offset = (page - 1) * pageSize;

            string query = @"
                SELECT p.*, u.Username
                FROM Posts p
                INNER JOIN Users u ON p.UserID = u.UserID
                WHERE 1=1";

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query += " AND (p.Title LIKE @SearchTerm OR p.Content LIKE @SearchTerm)";
            }

            query += @" ORDER BY p.CreatedAt DESC
                       OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

            if (!string.IsNullOrEmpty(searchTerm))
            {
                return DatabaseHelper.ExecuteQuery(query,
                    new SqlParameter("@SearchTerm", string.Concat("%", searchTerm, "%")),
                    new SqlParameter("@Offset", offset),
                    new SqlParameter("@PageSize", pageSize));
            }

            return DatabaseHelper.ExecuteQuery(query,
                new SqlParameter("@Offset", offset),
                new SqlParameter("@PageSize", pageSize));
        }

        /// <summary>
        /// 전체 게시글 수
        /// </summary>
        public static int GetTotalPostCount(string searchTerm = null)
        {
            string query = "SELECT COUNT(*) FROM Posts";

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query += " WHERE (Title LIKE @SearchTerm OR Content LIKE @SearchTerm)";
                return Convert.ToInt32(DatabaseHelper.ExecuteScalar(query,
                    new SqlParameter("@SearchTerm", string.Concat("%", searchTerm, "%"))));
            }

            return Convert.ToInt32(DatabaseHelper.ExecuteScalar(query));
        }

        /// <summary>
        /// 게시글 상세 조회
        /// </summary>
        public static Post GetPostById(int id)
        {
            string query = @"
                SELECT p.*, u.Username
                FROM Posts p
                INNER JOIN Users u ON p.UserID = u.UserID
                WHERE p.PostID = @PostID";

            DataTable dt = DatabaseHelper.ExecuteQuery(query, new SqlParameter("@PostID", id));

            if (dt.Rows.Count == 0)
            {
                return null;
            }

            DataRow row = dt.Rows[0];
            return new Post
            {
                PostID = Convert.ToInt32(row["PostID"]),
                UserID = Convert.ToInt32(row["UserID"]),
                Username = row["Username"].ToString(),
                Title = row["Title"].ToString(),
                Content = row["Content"].ToString(),
                CreatedAt = Convert.ToDateTime(row["CreatedAt"])
            };
        }

        /// <summary>
        /// 게시글 작성
        /// </summary>
        public static int CreatePost(string title, string content, int userId)
        {
            string sanitizedTitle = SecurityHelper.SanitizeInput(title);
            string sanitizedContent = SecurityHelper.SanitizeInput(content);

            string query = @"INSERT INTO Posts (Title, Content, UserID)
                            OUTPUT INSERTED.PostID
                            VALUES (@Title, @Content, @UserID)";

            object result = DatabaseHelper.ExecuteScalar(query,
                new SqlParameter("@Title", sanitizedTitle),
                new SqlParameter("@Content", sanitizedContent),
                new SqlParameter("@UserID", userId));

            return Convert.ToInt32(result);
        }

        /// <summary>
        /// 게시글 수정
        /// </summary>
        public static bool UpdatePost(int id, string title, string content, int userId, bool isAdmin)
        {
            string sanitizedTitle = SecurityHelper.SanitizeInput(title);
            string sanitizedContent = SecurityHelper.SanitizeInput(content);

            string query = isAdmin
                ? @"UPDATE Posts SET Title = @Title, Content = @Content WHERE PostID = @PostID"
                : @"UPDATE Posts SET Title = @Title, Content = @Content WHERE PostID = @PostID AND UserID = @UserID";

            if (isAdmin)
            {
                return DatabaseHelper.ExecuteNonQuery(query,
                    new SqlParameter("@PostID", id),
                    new SqlParameter("@Title", sanitizedTitle),
                    new SqlParameter("@Content", sanitizedContent)) > 0;
            }

            return DatabaseHelper.ExecuteNonQuery(query,
                new SqlParameter("@PostID", id),
                new SqlParameter("@Title", sanitizedTitle),
                new SqlParameter("@Content", sanitizedContent),
                new SqlParameter("@UserID", userId)) > 0;
        }

        /// <summary>
        /// 게시글 삭제
        /// </summary>
        public static bool DeletePost(int id, int userId, bool isAdmin)
        {
            string query = isAdmin
                ? "DELETE FROM Posts WHERE PostID = @PostID"
                : "DELETE FROM Posts WHERE PostID = @PostID AND UserID = @UserID";

            if (isAdmin)
            {
                return DatabaseHelper.ExecuteNonQuery(query, new SqlParameter("@PostID", id)) > 0;
            }

            return DatabaseHelper.ExecuteNonQuery(query,
                new SqlParameter("@PostID", id),
                new SqlParameter("@UserID", userId)) > 0;
        }

        /// <summary>
        /// 모든 게시글 조회 (관리자용)
        /// </summary>
        public static DataTable GetAllPosts()
        {
            string query = @"
                SELECT p.*, u.Username
                FROM Posts p
                INNER JOIN Users u ON p.UserID = u.UserID
                ORDER BY p.CreatedAt DESC";

            return DatabaseHelper.ExecuteQuery(query);
        }
    }
}
