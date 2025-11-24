using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using EzChat.Web.App_Code.DAL;
using EzChat.Web.App_Code.Models;

namespace EzChat.Web.App_Code.BLL
{
    /// <summary>
    /// 사용자 비즈니스 로직
    /// </summary>
    public static class UserBLL
    {
        /// <summary>
        /// 기본 관리자 계정 생성
        /// </summary>
        public static void CreateDefaultAdmin()
        {
            string adminLoginId = ConfigurationManager.AppSettings["AdminLoginID"] ?? "admin";
            string adminPassword = ConfigurationManager.AppSettings["AdminPassword"] ?? "Admin@123!";

            if (GetUserByLoginID(adminLoginId) == null)
            {
                string passwordHash = SecurityHelper.HashPassword(adminPassword);

                string query = @"INSERT INTO Users (Username, UserLoginID, PasswordHash, IsAdmin)
                                VALUES (@Username, @UserLoginID, @PasswordHash, 1)";

                DatabaseHelper.ExecuteNonQuery(query,
                    new SqlParameter("@Username", "관리자"),
                    new SqlParameter("@UserLoginID", adminLoginId),
                    new SqlParameter("@PasswordHash", passwordHash));
            }
        }

        /// <summary>
        /// 사용자 등록
        /// </summary>
        public static bool Register(string userLoginId, string password, string username, out string errorMessage)
        {
            errorMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(userLoginId) || userLoginId.Length < 4)
            {
                errorMessage = "아이디는 4자 이상이어야 합니다.";
                return false;
            }

            if (!SecurityHelper.ValidatePasswordPolicy(password, out errorMessage))
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(username))
            {
                errorMessage = "사용자 이름을 입력해주세요.";
                return false;
            }

            if (GetUserByLoginID(userLoginId) != null)
            {
                errorMessage = "이미 사용 중인 아이디입니다.";
                return false;
            }

            string passwordHash = SecurityHelper.HashPassword(password);
            string sanitizedUsername = SecurityHelper.SanitizeInput(username);
            string sanitizedLoginId = SecurityHelper.SanitizeInput(userLoginId);

            string query = @"INSERT INTO Users (Username, UserLoginID, PasswordHash, IsAdmin)
                            VALUES (@Username, @UserLoginID, @PasswordHash, 0)";

            int result = DatabaseHelper.ExecuteNonQuery(query,
                new SqlParameter("@Username", sanitizedUsername),
                new SqlParameter("@UserLoginID", sanitizedLoginId),
                new SqlParameter("@PasswordHash", passwordHash));

            return result > 0;
        }

        /// <summary>
        /// 로그인 검증
        /// </summary>
        public static User ValidateLogin(string userLoginId, string password, out string errorMessage)
        {
            errorMessage = string.Empty;

            User user = GetUserByLoginID(userLoginId);
            if (user == null)
            {
                errorMessage = "아이디 또는 비밀번호가 올바르지 않습니다.";
                return null;
            }

            if (!SecurityHelper.VerifyPassword(password, user.PasswordHash))
            {
                errorMessage = "아이디 또는 비밀번호가 올바르지 않습니다.";
                return null;
            }

            return user;
        }

        /// <summary>
        /// 로그인 ID로 사용자 조회
        /// </summary>
        public static User GetUserByLoginID(string userLoginId)
        {
            string query = "SELECT * FROM Users WHERE UserLoginID = @UserLoginID";
            DataTable dt = DatabaseHelper.ExecuteQuery(query, new SqlParameter("@UserLoginID", userLoginId));

            if (dt.Rows.Count == 0)
            {
                return null;
            }

            return MapUser(dt.Rows[0]);
        }

        /// <summary>
        /// ID로 사용자 조회
        /// </summary>
        public static User GetUserById(int id)
        {
            string query = "SELECT * FROM Users WHERE UserID = @UserID";
            DataTable dt = DatabaseHelper.ExecuteQuery(query, new SqlParameter("@UserID", id));

            if (dt.Rows.Count == 0)
            {
                return null;
            }

            return MapUser(dt.Rows[0]);
        }

        /// <summary>
        /// 모든 사용자 조회
        /// </summary>
        public static DataTable GetAllUsers()
        {
            string query = "SELECT * FROM Users ORDER BY UserID DESC";
            return DatabaseHelper.ExecuteQuery(query);
        }

        /// <summary>
        /// 사용자 삭제
        /// </summary>
        public static bool DeleteUser(int userId)
        {
            // 먼저 해당 사용자의 게시글 삭제
            string deletePostsQuery = "DELETE FROM Posts WHERE UserID = @UserID";
            DatabaseHelper.ExecuteNonQuery(deletePostsQuery, new SqlParameter("@UserID", userId));

            // 사용자 삭제
            string query = "DELETE FROM Users WHERE UserID = @UserID AND IsAdmin = 0";
            return DatabaseHelper.ExecuteNonQuery(query, new SqlParameter("@UserID", userId)) > 0;
        }

        /// <summary>
        /// 전체 사용자 수
        /// </summary>
        public static int GetTotalUsers()
        {
            string query = "SELECT COUNT(*) FROM Users";
            return Convert.ToInt32(DatabaseHelper.ExecuteScalar(query));
        }

        private static User MapUser(DataRow row)
        {
            return new User
            {
                UserID = Convert.ToInt32(row["UserID"]),
                Username = row["Username"].ToString(),
                UserLoginID = row["UserLoginID"].ToString(),
                PasswordHash = row["PasswordHash"].ToString(),
                IsAdmin = Convert.ToBoolean(row["IsAdmin"])
            };
        }
    }
}
