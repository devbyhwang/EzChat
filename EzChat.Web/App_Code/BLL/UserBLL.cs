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
            string adminEmail = ConfigurationManager.AppSettings["AdminEmail"] ?? "admin@ezchat.local";
            string adminPassword = ConfigurationManager.AppSettings["AdminPassword"] ?? "Admin@123!";

            if (GetUserByEmail(adminEmail) == null)
            {
                string passwordHash = SecurityHelper.HashPassword(adminPassword);

                string query = @"INSERT INTO Users (Email, PasswordHash, DisplayName, IsAdmin, IsActive)
                                VALUES (@Email, @PasswordHash, @DisplayName, 1, 1)";

                DatabaseHelper.ExecuteNonQuery(query,
                    new SqlParameter("@Email", adminEmail),
                    new SqlParameter("@PasswordHash", passwordHash),
                    new SqlParameter("@DisplayName", "Administrator"));
            }
        }

        /// <summary>
        /// 사용자 등록
        /// </summary>
        public static bool Register(string email, string password, string displayName, out string errorMessage)
        {
            errorMessage = string.Empty;

            if (!SecurityHelper.IsValidEmail(email))
            {
                errorMessage = "올바른 이메일 형식이 아닙니다.";
                return false;
            }

            if (!SecurityHelper.ValidatePasswordPolicy(password, out errorMessage))
            {
                return false;
            }

            if (GetUserByEmail(email) != null)
            {
                errorMessage = "이미 사용 중인 이메일입니다.";
                return false;
            }

            string passwordHash = SecurityHelper.HashPassword(password);
            string sanitizedDisplayName = SecurityHelper.SanitizeInput(displayName);

            string query = @"INSERT INTO Users (Email, PasswordHash, DisplayName, IsAdmin, IsActive)
                            VALUES (@Email, @PasswordHash, @DisplayName, 0, 1)";

            int result = DatabaseHelper.ExecuteNonQuery(query,
                new SqlParameter("@Email", email),
                new SqlParameter("@PasswordHash", passwordHash),
                new SqlParameter("@DisplayName", sanitizedDisplayName));

            return result > 0;
        }

        /// <summary>
        /// 로그인 검증
        /// </summary>
        public static User ValidateLogin(string email, string password, out string errorMessage)
        {
            errorMessage = string.Empty;

            User user = GetUserByEmail(email);
            if (user == null)
            {
                errorMessage = "이메일 또는 비밀번호가 올바르지 않습니다.";
                return null;
            }

            if (!user.IsActive)
            {
                errorMessage = "비활성화된 계정입니다.";
                return null;
            }

            if (!SecurityHelper.VerifyPassword(password, user.PasswordHash))
            {
                errorMessage = "이메일 또는 비밀번호가 올바르지 않습니다.";
                return null;
            }

            // 마지막 로그인 시간 업데이트
            UpdateLastLogin(user.Id);

            return user;
        }

        /// <summary>
        /// 이메일로 사용자 조회
        /// </summary>
        public static User GetUserByEmail(string email)
        {
            string query = "SELECT * FROM Users WHERE Email = @Email";
            DataTable dt = DatabaseHelper.ExecuteQuery(query, new SqlParameter("@Email", email));

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
            string query = "SELECT * FROM Users WHERE Id = @Id";
            DataTable dt = DatabaseHelper.ExecuteQuery(query, new SqlParameter("@Id", id));

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
            string query = "SELECT * FROM Users ORDER BY CreatedAt DESC";
            return DatabaseHelper.ExecuteQuery(query);
        }

        /// <summary>
        /// 마지막 로그인 시간 업데이트
        /// </summary>
        public static void UpdateLastLogin(int userId)
        {
            string query = "UPDATE Users SET LastLoginAt = GETUTCDATE() WHERE Id = @Id";
            DatabaseHelper.ExecuteNonQuery(query, new SqlParameter("@Id", userId));
        }

        /// <summary>
        /// 사용자 활성화/비활성화 토글
        /// </summary>
        public static bool ToggleUserActive(int userId)
        {
            string query = "UPDATE Users SET IsActive = ~IsActive WHERE Id = @Id";
            return DatabaseHelper.ExecuteNonQuery(query, new SqlParameter("@Id", userId)) > 0;
        }

        /// <summary>
        /// 사용자 삭제 (소프트 삭제)
        /// </summary>
        public static bool DeleteUser(int userId)
        {
            string query = @"UPDATE Users SET
                            IsActive = 0,
                            Email = 'deleted_' + CAST(Id AS NVARCHAR) + '@deleted.local'
                            WHERE Id = @Id";
            return DatabaseHelper.ExecuteNonQuery(query, new SqlParameter("@Id", userId)) > 0;
        }

        /// <summary>
        /// 대시보드 통계
        /// </summary>
        public static int GetTotalUsers()
        {
            string query = "SELECT COUNT(*) FROM Users";
            return Convert.ToInt32(DatabaseHelper.ExecuteScalar(query));
        }

        public static int GetActiveUsers()
        {
            string query = "SELECT COUNT(*) FROM Users WHERE IsActive = 1";
            return Convert.ToInt32(DatabaseHelper.ExecuteScalar(query));
        }

        public static int GetTodayLogins()
        {
            string query = "SELECT COUNT(*) FROM Users WHERE CAST(LastLoginAt AS DATE) = CAST(GETUTCDATE() AS DATE)";
            return Convert.ToInt32(DatabaseHelper.ExecuteScalar(query));
        }

        private static User MapUser(DataRow row)
        {
            return new User
            {
                Id = Convert.ToInt32(row["Id"]),
                Email = row["Email"].ToString(),
                PasswordHash = row["PasswordHash"].ToString(),
                DisplayName = row["DisplayName"]?.ToString(),
                IsAdmin = Convert.ToBoolean(row["IsAdmin"]),
                IsActive = Convert.ToBoolean(row["IsActive"]),
                CreatedAt = Convert.ToDateTime(row["CreatedAt"]),
                LastLoginAt = row["LastLoginAt"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(row["LastLoginAt"])
            };
        }
    }
}
