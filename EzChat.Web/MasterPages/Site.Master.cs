using System;
using System.Web;
using System.Web.Security;

namespace EzChat.Web.MasterPages
{
    public partial class SiteMaster : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // 세션에서 메시지 확인
            if (Session["SuccessMessage"] != null)
            {
                ShowSuccess(Session["SuccessMessage"].ToString());
                Session.Remove("SuccessMessage");
            }

            if (Session["ErrorMessage"] != null)
            {
                ShowError(Session["ErrorMessage"].ToString());
                Session.Remove("ErrorMessage");
            }
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            FormsAuthentication.SignOut();
            Session.Clear();
            Session.Abandon();
            Response.Redirect("~/Default.aspx");
        }

        public void ShowSuccess(string message)
        {
            pnlSuccess.Visible = true;
            litSuccess.Text = message;
        }

        public void ShowError(string message)
        {
            pnlError.Visible = true;
            litError.Text = message;
        }
    }
}
