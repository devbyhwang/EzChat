using System;
using System.Web.UI.WebControls;
using EzChat.Web.App_Code.BLL;

namespace EzChat.Web.Admin
{
    public partial class UsersPage : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadUsers();
            }
        }

        private void LoadUsers()
        {
            gvUsers.DataSource = UserBLL.GetAllUsers();
            gvUsers.DataBind();
        }

        protected void gvUsers_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int userId = Convert.ToInt32(e.CommandArgument);
            int adminId = Convert.ToInt32(Session["UserId"]);
            string ipAddress = Request.ServerVariables["REMOTE_ADDR"];

            if (e.CommandName == "ToggleActive")
            {
                UserBLL.ToggleUserActive(userId);
                IpBanBLL.LogAction(adminId, "ToggleUserActive", "User", userId.ToString(), null, ipAddress);
                Session["SuccessMessage"] = "사용자 상태가 변경되었습니다.";
            }
            else if (e.CommandName == "DeleteUser")
            {
                if (userId == adminId)
                {
                    Session["ErrorMessage"] = "자신의 계정은 삭제할 수 없습니다.";
                }
                else
                {
                    UserBLL.DeleteUser(userId);
                    IpBanBLL.LogAction(adminId, "DeleteUser", "User", userId.ToString(), null, ipAddress);
                    Session["SuccessMessage"] = "사용자가 삭제되었습니다.";
                }
            }

            Response.Redirect(Request.Url.AbsoluteUri);
        }
    }
}
