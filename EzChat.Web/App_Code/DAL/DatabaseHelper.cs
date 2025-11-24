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
                    UserID INT IDENTITY(1,1) PRIMARY KEY,
                    Username NVARCHAR(50) NOT NULL,
                    UserLoginID NVARCHAR(100) NOT NULL UNIQUE,
                    PasswordHash NVARCHAR(256) NOT NULL,
                    IsAdmin BIT DEFAULT 0
                );

                -- 게시글 테이블
                IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Posts' AND xtype='U')
                CREATE TABLE Posts (
                    PostID INT IDENTITY(1,1) PRIMARY KEY,
                    UserID INT NOT NULL,
                    Title NVARCHAR(200) NOT NULL,
                    Content NVARCHAR(MAX) NOT NULL,
                    CreatedAt DATETIME DEFAULT GETUTCDATE(),
                    FOREIGN KEY (UserID) REFERENCES Users(UserID)
                );

                -- 인덱스 생성
                IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name='IX_Posts_CreatedAt')
                CREATE INDEX IX_Posts_CreatedAt ON Posts(CreatedAt DESC);

                IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name='IX_Users_UserLoginID')
                CREATE INDEX IX_Users_UserLoginID ON Users(UserLoginID);
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
