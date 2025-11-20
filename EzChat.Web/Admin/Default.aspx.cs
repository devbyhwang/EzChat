using System;
using EzChat.Web.App_Code.BLL;

namespace EzChat.Web.Admin
{
    public partial class DefaultPage : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadDashboardStats();
            }
        }

        private void LoadDashboardStats()
        {
            litTotalUsers.Text = UserBLL.GetTotalUsers().ToString();
            litActiveUsers.Text = UserBLL.GetActiveUsers().ToString();
            litTotalRooms.Text = ChatBLL.GetTotalRooms().ToString();
            litTotalPosts.Text = BoardBLL.GetTotalPostCount().ToString();
            litTotalBans.Text = IpBanBLL.GetTotalBans().ToString();
            litTodayLogins.Text = UserBLL.GetTodayLogins().ToString();
        }
    }
}
