using System;
using System.Web;
using System.Web.Security;
using EzChat.Web.App_Code.BLL;
using EzChat.Web.App_Code.Models;

namespace EzChat.Web.Account
{
    public partial class LoginPage : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.IsAuthenticated)
            {
                Response.Redirect("~/Default.aspx");
            }
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid) return;

            string email = txtEmail.Text.Trim();
            string password = txtPassword.Text;
            string errorMessage;

            User user = UserBLL.ValidateLogin(email, password, out errorMessage);

            if (user == null)
            {
                pnlError.Visible = true;
                litError.Text = errorMessage;
                return;
            }

            // 역할 정보를 티켓에 저장
            string roles = user.IsAdmin ? "Admin" : "User";

            FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(
                1,                              // 버전
                user.Email,                     // 사용자 이름
                DateTime.Now,                   // 발급 시간
                DateTime.Now.AddMinutes(30),    // 만료 시간
                chkRememberMe.Checked,          // 지속 여부
                roles,                          // 사용자 데이터 (역할)
                FormsAuthentication.FormsCookiePath
            );

            string encryptedTicket = FormsAuthentication.Encrypt(ticket);
            HttpCookie authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);

            if (chkRememberMe.Checked)
            {
                authCookie.Expires = ticket.Expiration;
            }

            Response.Cookies.Add(authCookie);

            // 세션에 사용자 정보 저장
            Session["UserId"] = user.Id;
            Session["UserEmail"] = user.Email;
            Session["UserName"] = user.DisplayName ?? user.Email;
            Session["IsAdmin"] = user.IsAdmin;

            // 리턴 URL 확인
            string returnUrl = Request.QueryString["ReturnUrl"];
            if (!string.IsNullOrEmpty(returnUrl) && returnUrl.StartsWith("/"))
            {
                Response.Redirect(returnUrl);
            }
            else
            {
                Response.Redirect("~/Default.aspx");
            }
        }
    }
}
