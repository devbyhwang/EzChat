using System;
using EzChat.Web.App_Code.BLL;
using EzChat.Web.App_Code.Models;

namespace EzChat.Web.Board
{
    public partial class EditPage : System.Web.UI.Page
    {
        private int PostId;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Request.IsAuthenticated)
            {
                Response.Redirect("/Account/Login.aspx");
                return;
            }

            if (!int.TryParse(Request.QueryString["id"], out PostId))
            {
                Response.Redirect("/Board/Default.aspx");
                return;
            }

            if (!IsPostBack)
            {
                LoadPost();
            }
        }

        private void LoadPost()
        {
            BoardPost post = BoardBLL.GetPostById(PostId);

            if (post == null)
            {
                Response.Redirect("/Board/Default.aspx");
                return;
            }

            // 작성자 확인
            int userId = Convert.ToInt32(Session["UserId"]);
            if (post.AuthorId != userId)
            {
                Response.Redirect($"/Board/Detail.aspx?id={PostId}");
                return;
            }

            txtTitle.Text = post.Title;
            txtContent.Text = post.Content;
            lnkCancel.NavigateUrl = $"/Board/Detail.aspx?id={PostId}";
        }

        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid) return;

            int userId = Convert.ToInt32(Session["UserId"]);
            string title = txtTitle.Text.Trim();
            string content = txtContent.Text.Trim();

            bool success = BoardBLL.UpdatePost(PostId, title, content, userId);

            if (success)
            {
                Session["SuccessMessage"] = "게시글이 수정되었습니다.";
                Response.Redirect($"/Board/Detail.aspx?id={PostId}");
            }
            else
            {
                Session["ErrorMessage"] = "게시글 수정에 실패했습니다.";
            }
        }
    }
}
