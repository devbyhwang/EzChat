using System;
using System.Web;
using System.Web.Security;
using EzChat.Web.App_Code.BLL;
using EzChat.Web.App_Code.DAL;

namespace EzChat.Web
{
    public class Global : HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            // 데이터베이스 초기화
            DatabaseHelper.InitializeDatabase();

            // 기본 관리자 계정 생성
            UserBLL.CreateDefaultAdmin();
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            // IP 차단 확인
            string clientIp = GetClientIpAddress();
            if (IpBanBLL.IsIpBanned(clientIp))
            {
                Response.StatusCode = 403;
                Response.Write("<h1>접근이 거부되었습니다.</h1><p>귀하의 IP 주소가 차단되었습니다.</p>");
                Response.End();
            }
        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {
            if (HttpContext.Current.User != null && HttpContext.Current.User.Identity.IsAuthenticated)
            {
                if (HttpContext.Current.User.Identity is FormsIdentity identity)
                {
                    FormsAuthenticationTicket ticket = identity.Ticket;
                    string[] roles = ticket.UserData.Split(',');
                    HttpContext.Current.User = new System.Security.Principal.GenericPrincipal(identity, roles);
                }
            }
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            Exception ex = Server.GetLastError();
            // 로깅 처리
            System.Diagnostics.Debug.WriteLine($"Application Error: {ex.Message}");
        }

        protected void Session_Start(object sender, EventArgs e)
        {
            // 세션 시작
        }

        protected void Session_End(object sender, EventArgs e)
        {
            // 세션 종료
        }

        private string GetClientIpAddress()
        {
            string ip = Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(ip))
            {
                ip = Request.ServerVariables["REMOTE_ADDR"];
            }
            return ip ?? "unknown";
        }
    }
}
