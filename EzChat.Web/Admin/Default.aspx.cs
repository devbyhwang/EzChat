using System;
using System.Web.UI.WebControls;
using EzChat.Web.App_Code.BLL;

namespace EzChat.Web.Admin
{
    public partial class DefaultPage : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // 세션 메시지 표시
                if (Session["SuccessMessage"] != null)
                {
                    pnlSuccess.Visible = true;
                    litSuccess.Text = Session["SuccessMessage"].ToString();
                    Session.Remove("SuccessMessage");
                }

                if (Session["ErrorMessage"] != null)
                {
                    pnlError.Visible = true;
                    litError.Text = Session["ErrorMessage"].ToString();
                    Session.Remove("ErrorMessage");
                }

                LoadDashboardStats();
                LoadUsers();
            }
        }

        private void LoadDashboardStats()
        {
            litTotalUsers.Text = UserBLL.GetTotalUsers().ToString();
            litTotalPosts.Text = BoardBLL.GetTotalPostCount().ToString();
        }

        private void LoadUsers()
        {
            gvUsers.DataSource = UserBLL.GetAllUsers();
            gvUsers.DataBind();
        }

        private void LoadPosts()
        {
            gvPosts.DataSource = BoardBLL.GetAllPosts();
            gvPosts.DataBind();
        }

        protected void lnkTabUsers_Click(object sender, EventArgs e)
        {
            pnlUsers.Visible = true;
            pnlPosts.Visible = false;
            lnkTabUsers.CssClass = "btn btn-primary";
            lnkTabPosts.CssClass = "btn btn-secondary";
            LoadUsers();
        }

        protected void lnkTabPosts_Click(object sender, EventArgs e)
        {
            pnlUsers.Visible = false;
            pnlPosts.Visible = true;
            lnkTabUsers.CssClass = "btn btn-secondary";
            lnkTabPosts.CssClass = "btn btn-primary";
            LoadPosts();
        }

        protected void gvUsers_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "DeleteUser")
            {
                int userId = Convert.ToInt32(e.CommandArgument);
                int adminId = Convert.ToInt32(Session["UserId"]);

                if (userId == adminId)
                {
                    Session["ErrorMessage"] = "자신의 계정은 삭제할 수 없습니다.";
                }
                else
                {
                    UserBLL.DeleteUser(userId);
                    Session["SuccessMessage"] = "사용자가 삭제되었습니다.";
                }

                Response.Redirect(Request.Url.AbsoluteUri);
            }
        }

        protected void gvPosts_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "DeletePost")
            {
                int postId = Convert.ToInt32(e.CommandArgument);
                int adminId = Convert.ToInt32(Session["UserId"]);

                BoardBLL.DeletePost(postId, adminId, true);
                Session["SuccessMessage"] = "게시글이 삭제되었습니다.";

                Response.Redirect(Request.Url.AbsoluteUri);
            }
        }
    }
}
