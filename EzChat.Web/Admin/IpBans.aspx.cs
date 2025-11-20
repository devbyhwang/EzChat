using System;
using System.Web.UI.WebControls;
using EzChat.Web.App_Code.BLL;

namespace EzChat.Web.Admin
{
    public partial class IpBansPage : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadBans();
            }
        }

        private void LoadBans()
        {
            gvBans.DataSource = IpBanBLL.GetActiveBans();
            gvBans.DataBind();
        }

        protected void btnBan_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid) return;

            string ipAddress = txtIpAddress.Text.Trim();
            string reason = txtReason.Text.Trim();
            DateTime? expiresAt = null;

            if (!string.IsNullOrEmpty(txtExpiresAt.Text))
            {
                expiresAt = DateTime.Parse(txtExpiresAt.Text);
            }

            int adminId = Convert.ToInt32(Session["UserId"]);
            string adminIp = Request.ServerVariables["REMOTE_ADDR"];

            bool success = IpBanBLL.BanIp(ipAddress, reason, adminId, expiresAt);

            if (success)
            {
                IpBanBLL.LogAction(adminId, "BanIp", "IpBan", ipAddress, reason, adminIp);
                Session["SuccessMessage"] = $"IP {ipAddress}가 차단되었습니다.";
                txtIpAddress.Text = "";
                txtReason.Text = "";
                txtExpiresAt.Text = "";
            }
            else
            {
                Session["ErrorMessage"] = "이미 차단된 IP입니다.";
            }

            Response.Redirect(Request.Url.AbsoluteUri);
        }

        protected void gvBans_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Unban")
            {
                int banId = Convert.ToInt32(e.CommandArgument);
                int adminId = Convert.ToInt32(Session["UserId"]);
                string adminIp = Request.ServerVariables["REMOTE_ADDR"];

                IpBanBLL.UnbanIp(banId);
                IpBanBLL.LogAction(adminId, "UnbanIp", "IpBan", banId.ToString(), null, adminIp);
                Session["SuccessMessage"] = "IP 차단이 해제되었습니다.";

                Response.Redirect(Request.Url.AbsoluteUri);
            }
        }
    }
}
