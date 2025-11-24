using System;
using EzChat.Web.App_Code.BLL;

namespace EzChat.Web.Account
{
    public partial class RegisterPage : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.IsAuthenticated)
            {
                Response.Redirect("~/Default.aspx");
            }
        }

        protected void btnRegister_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid) return;

            string loginId = txtLoginId.Text.Trim();
            string password = txtPassword.Text;
            string username = txtUsername.Text.Trim();
            string errorMessage;

            bool success = UserBLL.Register(loginId, password, username, out errorMessage);

            if (!success)
            {
                pnlError.Visible = true;
                litError.Text = errorMessage;
                return;
            }

            Session["SuccessMessage"] = "회원가입이 완료되었습니다. 로그인해주세요.";
            Response.Redirect("~/Account/Login.aspx");
        }
    }
}
