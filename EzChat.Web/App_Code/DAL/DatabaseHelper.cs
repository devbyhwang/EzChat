using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace EzChat.Web.App_Code.DAL
{
    /// <summary>
    /// 데이터베이스 헬퍼 클래스
    /// </summary>
    public static class DatabaseHelper
    {
        private static readonly string ConnectionString = ConfigurationManager.ConnectionStrings["EzChatConnection"].ConnectionString;

        /// <summary>
        /// 데이터베이스 연결 생성
        /// </summary>
        public static SqlConnection GetConnection()
        {
            return new SqlConnection(ConnectionString);
        }

        /// <summary>
        /// 쿼리 실행 (SELECT)
        /// </summary>
        public static DataTable ExecuteQuery(string query, params SqlParameter[] parameters)
        {
            using (SqlConnection conn = GetConnection())
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    if (parameters != null)
                    {
                        cmd.Parameters.AddRange(parameters);
                    }

                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    return dt;
                }
            }
        }

        /// <summary>
        /// 쿼리 실행 (INSERT, UPDATE, DELETE)
        /// </summary>
        public static int ExecuteNonQuery(string query, params SqlParameter[] parameters)
        {
            using (SqlConnection conn = GetConnection())
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    if (parameters != null)
                    {
                        cmd.Parameters.AddRange(parameters);
                    }

                    conn.Open();
                    return cmd.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// 스칼라 값 조회
        /// </summary>
        public static object ExecuteScalar(string query, params SqlParameter[] parameters)
        {
            using (SqlConnection conn = GetConnection())
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    if (parameters != null)
                    {
                        cmd.Parameters.AddRange(parameters);
                    }

                    conn.Open();
                    return cmd.ExecuteScalar();
                }
            }
        }

        /// <summary>
        /// 데이터베이스 및 테이블 초기화
        /// </summary>
        public static void InitializeDatabase()
        {
            string createTablesQuery = @"
                -- 사용자 테이블
                IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Users' AND xtype='U')
                CREATE TABLE Users (
                    Id INT IDENTITY(1,1) PRIMARY KEY,
                    Email NVARCHAR(100) NOT NULL UNIQUE,
                    PasswordHash NVARCHAR(256) NOT NULL,
                    DisplayName NVARCHAR(50),
                    IsAdmin BIT DEFAULT 0,
                    IsActive BIT DEFAULT 1,
                    CreatedAt DATETIME DEFAULT GETUTCDATE(),
                    LastLoginAt DATETIME NULL
                );

                -- 채팅방 테이블
                IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='ChatRooms' AND xtype='U')
                CREATE TABLE ChatRooms (
                    Id INT IDENTITY(1,1) PRIMARY KEY,
                    Name NVARCHAR(100) NOT NULL,
                    Description NVARCHAR(500),
                    CreatedById INT NOT NULL,
                    CreatedAt DATETIME DEFAULT GETUTCDATE(),
                    IsActive BIT DEFAULT 1,
                    MaxUsers INT DEFAULT 50,
                    FOREIGN KEY (CreatedById) REFERENCES Users(Id)
                );

                -- 채팅 메시지 테이블
                IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='ChatMessages' AND xtype='U')
                CREATE TABLE ChatMessages (
                    Id INT IDENTITY(1,1) PRIMARY KEY,
                    RoomId INT NOT NULL,
                    UserId INT NOT NULL,
                    Content NVARCHAR(2000) NOT NULL,
                    SentAt DATETIME DEFAULT GETUTCDATE(),
                    IsDeleted BIT DEFAULT 0,
                    FOREIGN KEY (RoomId) REFERENCES ChatRooms(Id),
                    FOREIGN KEY (UserId) REFERENCES Users(Id)
                );

                -- 게시글 테이블
                IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='BoardPosts' AND xtype='U')
                CREATE TABLE BoardPosts (
                    Id INT IDENTITY(1,1) PRIMARY KEY,
                    Title NVARCHAR(200) NOT NULL,
                    Content NVARCHAR(MAX) NOT NULL,
                    AuthorId INT NOT NULL,
                    CreatedAt DATETIME DEFAULT GETUTCDATE(),
                    UpdatedAt DATETIME NULL,
                    ViewCount INT DEFAULT 0,
                    IsDeleted BIT DEFAULT 0,
                    FOREIGN KEY (AuthorId) REFERENCES Users(Id)
                );

                -- 댓글 테이블
                IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Comments' AND xtype='U')
                CREATE TABLE Comments (
                    Id INT IDENTITY(1,1) PRIMARY KEY,
                    PostId INT NOT NULL,
                    AuthorId INT NOT NULL,
                    Content NVARCHAR(1000) NOT NULL,
                    CreatedAt DATETIME DEFAULT GETUTCDATE(),
                    IsDeleted BIT DEFAULT 0,
                    FOREIGN KEY (PostId) REFERENCES BoardPosts(Id),
                    FOREIGN KEY (AuthorId) REFERENCES Users(Id)
                );

                -- IP 차단 테이블
                IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='IpBans' AND xtype='U')
                CREATE TABLE IpBans (
                    Id INT IDENTITY(1,1) PRIMARY KEY,
                    IpAddress NVARCHAR(45) NOT NULL,
                    Reason NVARCHAR(500),
                    BannedById INT NOT NULL,
                    BannedAt DATETIME DEFAULT GETUTCDATE(),
                    ExpiresAt DATETIME NULL,
                    IsActive BIT DEFAULT 1,
                    FOREIGN KEY (BannedById) REFERENCES Users(Id)
                );

                -- 감사 로그 테이블
                IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='AuditLogs' AND xtype='U')
                CREATE TABLE AuditLogs (
                    Id INT IDENTITY(1,1) PRIMARY KEY,
                    AdminId INT NOT NULL,
                    Action NVARCHAR(100) NOT NULL,
                    TargetType NVARCHAR(50),
                    TargetId NVARCHAR(100),
                    Details NVARCHAR(1000),
                    Timestamp DATETIME DEFAULT GETUTCDATE(),
                    IpAddress NVARCHAR(45),
                    FOREIGN KEY (AdminId) REFERENCES Users(Id)
                );

                -- 인덱스 생성
                IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name='IX_IpBans_IpAddress')
                CREATE INDEX IX_IpBans_IpAddress ON IpBans(IpAddress);

                IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name='IX_BoardPosts_CreatedAt')
                CREATE INDEX IX_BoardPosts_CreatedAt ON BoardPosts(CreatedAt DESC);

                IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name='IX_ChatMessages_RoomId')
                CREATE INDEX IX_ChatMessages_RoomId ON ChatMessages(RoomId, SentAt DESC);
            ";

            try
            {
                ExecuteNonQuery(createTablesQuery);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Database initialization error: {ex.Message}");
            }
        }
    }
}
