using System;
using System.Data;
using System.Data.SqlClient;
using EzChat.Web.App_Code.DAL;

namespace EzChat.Web.App_Code.BLL
{
    /// <summary>
    /// IP 차단 비즈니스 로직
    /// </summary>
    public static class IpBanBLL
    {
        /// <summary>
        /// IP 차단 여부 확인
        /// </summary>
        public static bool IsIpBanned(string ipAddress)
        {
            if (string.IsNullOrEmpty(ipAddress))
            {
                return false;
            }

            string query = @"SELECT COUNT(*) FROM IpBans
                            WHERE IpAddress = @IpAddress
                            AND IsActive = 1
                            AND (ExpiresAt IS NULL OR ExpiresAt > GETUTCDATE())";

            int count = Convert.ToInt32(DatabaseHelper.ExecuteScalar(query,
                new SqlParameter("@IpAddress", ipAddress)));

            return count > 0;
        }

        /// <summary>
        /// 활성 IP 차단 목록 조회
        /// </summary>
        public static DataTable GetActiveBans()
        {
            string query = @"
                SELECT b.*, u.DisplayName AS BannedByName, u.Email AS BannedByEmail
                FROM IpBans b
                INNER JOIN Users u ON b.BannedById = u.Id
                WHERE b.IsActive = 1 AND (b.ExpiresAt IS NULL OR b.ExpiresAt > GETUTCDATE())
                ORDER BY b.BannedAt DESC";

            return DatabaseHelper.ExecuteQuery(query);
        }

        /// <summary>
        /// IP 차단
        /// </summary>
        public static bool BanIp(string ipAddress, string reason, int bannedById, DateTime? expiresAt)
        {
            // 이미 차단된 IP인지 확인
            if (IsIpBanned(ipAddress))
            {
                return false;
            }

            string query = @"INSERT INTO IpBans (IpAddress, Reason, BannedById, ExpiresAt)
                            VALUES (@IpAddress, @Reason, @BannedById, @ExpiresAt)";

            return DatabaseHelper.ExecuteNonQuery(query,
                new SqlParameter("@IpAddress", ipAddress),
                new SqlParameter("@Reason", reason ?? (object)DBNull.Value),
                new SqlParameter("@BannedById", bannedById),
                new SqlParameter("@ExpiresAt", expiresAt ?? (object)DBNull.Value)) > 0;
        }

        /// <summary>
        /// IP 차단 해제
        /// </summary>
        public static bool UnbanIp(int banId)
        {
            string query = "UPDATE IpBans SET IsActive = 0 WHERE Id = @Id";
            return DatabaseHelper.ExecuteNonQuery(query, new SqlParameter("@Id", banId)) > 0;
        }

        /// <summary>
        /// 총 차단 IP 수
        /// </summary>
        public static int GetTotalBans()
        {
            string query = "SELECT COUNT(*) FROM IpBans WHERE IsActive = 1";
            return Convert.ToInt32(DatabaseHelper.ExecuteScalar(query));
        }

        /// <summary>
        /// 감사 로그 기록
        /// </summary>
        public static void LogAction(int adminId, string action, string targetType, string targetId, string details, string ipAddress)
        {
            string query = @"INSERT INTO AuditLogs (AdminId, Action, TargetType, TargetId, Details, IpAddress)
                            VALUES (@AdminId, @Action, @TargetType, @TargetId, @Details, @IpAddress)";

            DatabaseHelper.ExecuteNonQuery(query,
                new SqlParameter("@AdminId", adminId),
                new SqlParameter("@Action", action),
                new SqlParameter("@TargetType", targetType ?? (object)DBNull.Value),
                new SqlParameter("@TargetId", targetId ?? (object)DBNull.Value),
                new SqlParameter("@Details", details ?? (object)DBNull.Value),
                new SqlParameter("@IpAddress", ipAddress ?? (object)DBNull.Value));
        }

        /// <summary>
        /// 감사 로그 조회
        /// </summary>
        public static DataTable GetAuditLogs(int count = 100)
        {
            string query = @"
                SELECT TOP (@Count) l.*, u.DisplayName AS AdminName, u.Email AS AdminEmail
                FROM AuditLogs l
                INNER JOIN Users u ON l.AdminId = u.Id
                ORDER BY l.Timestamp DESC";

            return DatabaseHelper.ExecuteQuery(query, new SqlParameter("@Count", count));
        }
    }
}
