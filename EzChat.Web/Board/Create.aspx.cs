using System;
using EzChat.Web.App_Code.BLL;

namespace EzChat.Web.Board
{
    public partial class CreatePage : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Request.IsAuthenticated)
            {
                Response.Redirect($"/Account/Login.aspx?ReturnUrl={Server.UrlEncode(Request.Url.AbsolutePath)}");
            }
        }

        protected void btnCreate_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid) return;

            int userId = Convert.ToInt32(Session["UserId"]);
            string title = txtTitle.Text.Trim();
            string content = txtContent.Text.Trim();

            int postId = BoardBLL.CreatePost(title, content, userId);

            Session["SuccessMessage"] = "게시글이 작성되었습니다.";
            Response.Redirect($"/Board/Detail.aspx?id={postId}");
        }
    }
}
