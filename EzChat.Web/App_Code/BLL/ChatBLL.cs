using System;
using System.Data;
using System.Data.SqlClient;
using EzChat.Web.App_Code.DAL;
using EzChat.Web.App_Code.Models;

namespace EzChat.Web.App_Code.BLL
{
    /// <summary>
    /// 채팅 비즈니스 로직
    /// </summary>
    public static class ChatBLL
    {
        /// <summary>
        /// 활성 채팅방 목록 조회
        /// </summary>
        public static DataTable GetActiveRooms()
        {
            string query = @"
                SELECT r.*, u.DisplayName AS CreatedByName, u.Email AS CreatedByEmail
                FROM ChatRooms r
                INNER JOIN Users u ON r.CreatedById = u.Id
                WHERE r.IsActive = 1
                ORDER BY r.CreatedAt DESC";

            return DatabaseHelper.ExecuteQuery(query);
        }

        /// <summary>
        /// 모든 채팅방 조회 (관리자용)
        /// </summary>
        public static DataTable GetAllRooms()
        {
            string query = @"
                SELECT r.*, u.DisplayName AS CreatedByName, u.Email AS CreatedByEmail
                FROM ChatRooms r
                INNER JOIN Users u ON r.CreatedById = u.Id
                ORDER BY r.CreatedAt DESC";

            return DatabaseHelper.ExecuteQuery(query);
        }

        /// <summary>
        /// 채팅방 상세 조회
        /// </summary>
        public static ChatRoom GetRoomById(int id)
        {
            string query = @"
                SELECT r.*, u.DisplayName AS CreatedByName, u.Email AS CreatedByEmail
                FROM ChatRooms r
                INNER JOIN Users u ON r.CreatedById = u.Id
                WHERE r.Id = @Id AND r.IsActive = 1";

            DataTable dt = DatabaseHelper.ExecuteQuery(query, new SqlParameter("@Id", id));

            if (dt.Rows.Count == 0)
            {
                return null;
            }

            DataRow row = dt.Rows[0];
            return new ChatRoom
            {
                Id = Convert.ToInt32(row["Id"]),
                Name = row["Name"].ToString(),
                Description = row["Description"]?.ToString(),
                CreatedById = Convert.ToInt32(row["CreatedById"]),
                CreatedByName = row["CreatedByName"]?.ToString() ?? row["CreatedByEmail"].ToString(),
                CreatedAt = Convert.ToDateTime(row["CreatedAt"]),
                IsActive = Convert.ToBoolean(row["IsActive"]),
                MaxUsers = Convert.ToInt32(row["MaxUsers"])
            };
        }

        /// <summary>
        /// 채팅방 생성
        /// </summary>
        public static int CreateRoom(string name, string description, int createdById, int maxUsers)
        {
            string sanitizedName = SecurityHelper.SanitizeInput(name);
            string sanitizedDescription = SecurityHelper.SanitizeInput(description);

            string query = @"INSERT INTO ChatRooms (Name, Description, CreatedById, MaxUsers)
                            OUTPUT INSERTED.Id
                            VALUES (@Name, @Description, @CreatedById, @MaxUsers)";

            object result = DatabaseHelper.ExecuteScalar(query,
                new SqlParameter("@Name", sanitizedName),
                new SqlParameter("@Description", sanitizedDescription ?? (object)DBNull.Value),
                new SqlParameter("@CreatedById", createdById),
                new SqlParameter("@MaxUsers", maxUsers));

            return Convert.ToInt32(result);
        }

        /// <summary>
        /// 채팅방 삭제
        /// </summary>
        public static bool DeleteRoom(int id, int userId, bool isAdmin)
        {
            string query = isAdmin
                ? "UPDATE ChatRooms SET IsActive = 0 WHERE Id = @Id"
                : "UPDATE ChatRooms SET IsActive = 0 WHERE Id = @Id AND CreatedById = @CreatedById";

            if (isAdmin)
            {
                return DatabaseHelper.ExecuteNonQuery(query, new SqlParameter("@Id", id)) > 0;
            }

            return DatabaseHelper.ExecuteNonQuery(query,
                new SqlParameter("@Id", id),
                new SqlParameter("@CreatedById", userId)) > 0;
        }

        /// <summary>
        /// 최근 메시지 조회
        /// </summary>
        public static DataTable GetRecentMessages(int roomId, int count = 50)
        {
            string query = @"
                SELECT TOP (@Count) m.*, u.DisplayName AS UserName, u.Email AS UserEmail
                FROM ChatMessages m
                INNER JOIN Users u ON m.UserId = u.Id
                WHERE m.RoomId = @RoomId AND m.IsDeleted = 0
                ORDER BY m.SentAt DESC";

            DataTable dt = DatabaseHelper.ExecuteQuery(query,
                new SqlParameter("@RoomId", roomId),
                new SqlParameter("@Count", count));

            // 시간순으로 정렬
            dt.DefaultView.Sort = "SentAt ASC";
            return dt.DefaultView.ToTable();
        }

        /// <summary>
        /// 메시지 저장
        /// </summary>
        public static int SaveMessage(int roomId, int userId, string content)
        {
            string sanitizedContent = SecurityHelper.SanitizeInput(content);

            string query = @"INSERT INTO ChatMessages (RoomId, UserId, Content)
                            OUTPUT INSERTED.Id
                            VALUES (@RoomId, @UserId, @Content)";

            object result = DatabaseHelper.ExecuteScalar(query,
                new SqlParameter("@RoomId", roomId),
                new SqlParameter("@UserId", userId),
                new SqlParameter("@Content", sanitizedContent));

            return Convert.ToInt32(result);
        }

        /// <summary>
        /// 채팅방 수 조회
        /// </summary>
        public static int GetTotalRooms()
        {
            string query = "SELECT COUNT(*) FROM ChatRooms WHERE IsActive = 1";
            return Convert.ToInt32(DatabaseHelper.ExecuteScalar(query));
        }
    }
}
