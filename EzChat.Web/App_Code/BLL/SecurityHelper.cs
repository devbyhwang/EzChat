using System;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace EzChat.Web.App_Code.BLL
{
    /// <summary>
    /// 보안 헬퍼 클래스
    /// </summary>
    public static class SecurityHelper
    {
        /// <summary>
        /// 비밀번호 해싱 (PBKDF2)
        /// </summary>
        public static string HashPassword(string password)
        {
            byte[] salt = new byte[16];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000, HashAlgorithmName.SHA256))
            {
                byte[] hash = pbkdf2.GetBytes(32);
                byte[] hashBytes = new byte[48];
                Array.Copy(salt, 0, hashBytes, 0, 16);
                Array.Copy(hash, 0, hashBytes, 16, 32);
                return Convert.ToBase64String(hashBytes);
            }
        }

        /// <summary>
        /// 비밀번호 검증
        /// </summary>
        public static bool VerifyPassword(string password, string storedHash)
        {
            try
            {
                byte[] hashBytes = Convert.FromBase64String(storedHash);
                byte[] salt = new byte[16];
                Array.Copy(hashBytes, 0, salt, 0, 16);

                using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000, HashAlgorithmName.SHA256))
                {
                    byte[] hash = pbkdf2.GetBytes(32);
                    for (int i = 0; i < 32; i++)
                    {
                        if (hashBytes[i + 16] != hash[i])
                        {
                            return false;
                        }
                    }
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 비밀번호 정책 검증
        /// </summary>
        public static bool ValidatePasswordPolicy(string password, out string errorMessage)
        {
            errorMessage = string.Empty;

            if (string.IsNullOrEmpty(password) || password.Length < 8)
            {
                errorMessage = "비밀번호는 최소 8자 이상이어야 합니다.";
                return false;
            }

            if (!Regex.IsMatch(password, @"[A-Z]"))
            {
                errorMessage = "비밀번호에는 대문자가 포함되어야 합니다.";
                return false;
            }

            if (!Regex.IsMatch(password, @"[a-z]"))
            {
                errorMessage = "비밀번호에는 소문자가 포함되어야 합니다.";
                return false;
            }

            if (!Regex.IsMatch(password, @"[0-9]"))
            {
                errorMessage = "비밀번호에는 숫자가 포함되어야 합니다.";
                return false;
            }

            if (!Regex.IsMatch(password, @"[!@#$%^&*(),.?""':{}|<>]"))
            {
                errorMessage = "비밀번호에는 특수문자가 포함되어야 합니다.";
                return false;
            }

            return true;
        }

        /// <summary>
        /// XSS 방지를 위한 HTML 인코딩
        /// </summary>
        public static string SanitizeInput(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return string.Empty;
            }

            return HttpUtility.HtmlEncode(input);
        }

        /// <summary>
        /// 이메일 형식 검증
        /// </summary>
        public static bool IsValidEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return false;
            }

            return Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        }

        /// <summary>
        /// SQL Injection 방지를 위한 문자열 정리
        /// </summary>
        public static string CleanSqlInput(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return string.Empty;
            }

            // 매개변수화된 쿼리를 사용하므로 기본적인 정리만 수행
            return input.Replace("'", "''");
        }
    }
}
