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
                SELECT p.*, u.DisplayName AS AuthorName, u.Email AS AuthorEmail,
                       (SELECT COUNT(*) FROM Comments WHERE PostId = p.Id AND IsDeleted = 0) AS CommentCount
                FROM BoardPosts p
                INNER JOIN Users u ON p.AuthorId = u.Id
                WHERE p.IsDeleted = 0";

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query += " AND (p.Title LIKE @SearchTerm OR p.Content LIKE @SearchTerm)";
            }

            query += @" ORDER BY p.CreatedAt DESC
                       OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

            if (!string.IsNullOrEmpty(searchTerm))
            {
                return DatabaseHelper.ExecuteQuery(query,
                    new SqlParameter("@SearchTerm", $"%{searchTerm}%"),
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
            string query = "SELECT COUNT(*) FROM BoardPosts WHERE IsDeleted = 0";

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query += " AND (Title LIKE @SearchTerm OR Content LIKE @SearchTerm)";
                return Convert.ToInt32(DatabaseHelper.ExecuteScalar(query,
                    new SqlParameter("@SearchTerm", $"%{searchTerm}%")));
            }

            return Convert.ToInt32(DatabaseHelper.ExecuteScalar(query));
        }

        /// <summary>
        /// 게시글 상세 조회
        /// </summary>
        public static BoardPost GetPostById(int id)
        {
            string query = @"
                SELECT p.*, u.DisplayName AS AuthorName, u.Email AS AuthorEmail
                FROM BoardPosts p
                INNER JOIN Users u ON p.AuthorId = u.Id
                WHERE p.Id = @Id AND p.IsDeleted = 0";

            DataTable dt = DatabaseHelper.ExecuteQuery(query, new SqlParameter("@Id", id));

            if (dt.Rows.Count == 0)
            {
                return null;
            }

            DataRow row = dt.Rows[0];
            return new BoardPost
            {
                Id = Convert.ToInt32(row["Id"]),
                Title = row["Title"].ToString(),
                Content = row["Content"].ToString(),
                AuthorId = Convert.ToInt32(row["AuthorId"]),
                AuthorName = row["AuthorName"]?.ToString() ?? row["AuthorEmail"].ToString(),
                CreatedAt = Convert.ToDateTime(row["CreatedAt"]),
                UpdatedAt = row["UpdatedAt"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(row["UpdatedAt"]),
                ViewCount = Convert.ToInt32(row["ViewCount"])
            };
        }

        /// <summary>
        /// 게시글 작성
        /// </summary>
        public static int CreatePost(string title, string content, int authorId)
        {
            string sanitizedTitle = SecurityHelper.SanitizeInput(title);
            string sanitizedContent = SecurityHelper.SanitizeInput(content);

            string query = @"INSERT INTO BoardPosts (Title, Content, AuthorId)
                            OUTPUT INSERTED.Id
                            VALUES (@Title, @Content, @AuthorId)";

            object result = DatabaseHelper.ExecuteScalar(query,
                new SqlParameter("@Title", sanitizedTitle),
                new SqlParameter("@Content", sanitizedContent),
                new SqlParameter("@AuthorId", authorId));

            return Convert.ToInt32(result);
        }

        /// <summary>
        /// 게시글 수정
        /// </summary>
        public static bool UpdatePost(int id, string title, string content, int authorId)
        {
            string sanitizedTitle = SecurityHelper.SanitizeInput(title);
            string sanitizedContent = SecurityHelper.SanitizeInput(content);

            string query = @"UPDATE BoardPosts
                            SET Title = @Title, Content = @Content, UpdatedAt = GETUTCDATE()
                            WHERE Id = @Id AND AuthorId = @AuthorId";

            return DatabaseHelper.ExecuteNonQuery(query,
                new SqlParameter("@Id", id),
                new SqlParameter("@Title", sanitizedTitle),
                new SqlParameter("@Content", sanitizedContent),
                new SqlParameter("@AuthorId", authorId)) > 0;
        }

        /// <summary>
        /// 게시글 삭제
        /// </summary>
        public static bool DeletePost(int id, int userId, bool isAdmin)
        {
            string query = isAdmin
                ? "UPDATE BoardPosts SET IsDeleted = 1 WHERE Id = @Id"
                : "UPDATE BoardPosts SET IsDeleted = 1 WHERE Id = @Id AND AuthorId = @AuthorId";

            if (isAdmin)
            {
                return DatabaseHelper.ExecuteNonQuery(query, new SqlParameter("@Id", id)) > 0;
            }

            return DatabaseHelper.ExecuteNonQuery(query,
                new SqlParameter("@Id", id),
                new SqlParameter("@AuthorId", userId)) > 0;
        }

        /// <summary>
        /// 조회수 증가
        /// </summary>
        public static void IncrementViewCount(int id)
        {
            string query = "UPDATE BoardPosts SET ViewCount = ViewCount + 1 WHERE Id = @Id";
            DatabaseHelper.ExecuteNonQuery(query, new SqlParameter("@Id", id));
        }

        /// <summary>
        /// 댓글 목록 조회
        /// </summary>
        public static DataTable GetComments(int postId)
        {
            string query = @"
                SELECT c.*, u.DisplayName AS AuthorName, u.Email AS AuthorEmail
                FROM Comments c
                INNER JOIN Users u ON c.AuthorId = u.Id
                WHERE c.PostId = @PostId AND c.IsDeleted = 0
                ORDER BY c.CreatedAt ASC";

            return DatabaseHelper.ExecuteQuery(query, new SqlParameter("@PostId", postId));
        }

        /// <summary>
        /// 댓글 작성
        /// </summary>
        public static int AddComment(int postId, string content, int authorId)
        {
            string sanitizedContent = SecurityHelper.SanitizeInput(content);

            string query = @"INSERT INTO Comments (PostId, Content, AuthorId)
                            OUTPUT INSERTED.Id
                            VALUES (@PostId, @Content, @AuthorId)";

            object result = DatabaseHelper.ExecuteScalar(query,
                new SqlParameter("@PostId", postId),
                new SqlParameter("@Content", sanitizedContent),
                new SqlParameter("@AuthorId", authorId));

            return Convert.ToInt32(result);
        }

        /// <summary>
        /// 댓글 삭제
        /// </summary>
        public static bool DeleteComment(int id, int userId, bool isAdmin)
        {
            string query = isAdmin
                ? "UPDATE Comments SET IsDeleted = 1 WHERE Id = @Id"
                : "UPDATE Comments SET IsDeleted = 1 WHERE Id = @Id AND AuthorId = @AuthorId";

            if (isAdmin)
            {
                return DatabaseHelper.ExecuteNonQuery(query, new SqlParameter("@Id", id)) > 0;
            }

            return DatabaseHelper.ExecuteNonQuery(query,
                new SqlParameter("@Id", id),
                new SqlParameter("@AuthorId", userId)) > 0;
        }
    }
}
