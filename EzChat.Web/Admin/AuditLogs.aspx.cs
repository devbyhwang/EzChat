using System;
using EzChat.Web.App_Code.BLL;

namespace EzChat.Web.Admin
{
    public partial class AuditLogsPage : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadLogs();
            }
        }

        private void LoadLogs()
        {
            gvLogs.DataSource = IpBanBLL.GetAuditLogs(100);
            gvLogs.DataBind();
        }
    }
}
