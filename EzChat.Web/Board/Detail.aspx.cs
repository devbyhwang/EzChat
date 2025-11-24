using System;
using EzChat.Web.App_Code.BLL;
using EzChat.Web.App_Code.Models;

namespace EzChat.Web.Board
{
    public partial class DetailPage : System.Web.UI.Page
    {
        private int PostId;
        private Post CurrentPost;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!int.TryParse(Request.QueryString["id"], out PostId))
            {
                Response.Redirect("/Board/Default.aspx");
                return;
            }

            if (!IsPostBack)
            {
                LoadPost();
            }
            else
            {
                CurrentPost = BoardBLL.GetPostById(PostId);
            }
        }

        private void LoadPost()
        {
            CurrentPost = BoardBLL.GetPostById(PostId);

            if (CurrentPost == null)
            {
                Response.Redirect("/Board/Default.aspx");
                return;
            }

            Page.Title = CurrentPost.Title;
            litTitle.Text = Server.HtmlEncode(CurrentPost.Title);
            litAuthor.Text = Server.HtmlEncode(CurrentPost.Username);
            litCreatedAt.Text = CurrentPost.CreatedAt.ToString("yyyy-MM-dd HH:mm");
            litContent.Text = Server.HtmlEncode(CurrentPost.Content).Replace("\n", "<br/>");

            // 권한 확인
            if (Session["UserId"] != null)
            {
                int userId = Convert.ToInt32(Session["UserId"]);
                bool isAdmin = Session["IsAdmin"] != null && Convert.ToBoolean(Session["IsAdmin"]);

                if (CurrentPost.UserID == userId || isAdmin)
                {
                    btnEdit.Visible = true;
                    btnDelete.Visible = true;
                }
            }
        }

        protected void btnEdit_Click(object sender, EventArgs e)
        {
            CurrentPost = BoardBLL.GetPostById(PostId);
            if (CurrentPost == null) return;

            txtEditTitle.Text = CurrentPost.Title;
            txtEditContent.Text = CurrentPost.Content;
            pnlEdit.Visible = true;
        }

        protected void btnSaveEdit_Click(object sender, EventArgs e)
        {
            if (Session["UserId"] == null) return;

            int userId = Convert.ToInt32(Session["UserId"]);
            bool isAdmin = Session["IsAdmin"] != null && Convert.ToBoolean(Session["IsAdmin"]);

            string title = txtEditTitle.Text.Trim();
            string content = txtEditContent.Text.Trim();

            if (string.IsNullOrEmpty(title) || string.IsNullOrEmpty(content))
            {
                return;
            }

            BoardBLL.UpdatePost(PostId, title, content, userId, isAdmin);
            Response.Redirect($"/Board/Detail.aspx?id={PostId}");
        }

        protected void btnCancelEdit_Click(object sender, EventArgs e)
        {
            pnlEdit.Visible = false;
            LoadPost();
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            if (Session["UserId"] == null) return;

            int userId = Convert.ToInt32(Session["UserId"]);
            bool isAdmin = Session["IsAdmin"] != null && Convert.ToBoolean(Session["IsAdmin"]);

            BoardBLL.DeletePost(PostId, userId, isAdmin);
            Session["SuccessMessage"] = "게시글이 삭제되었습니다.";
            Response.Redirect("/Board/Default.aspx");
        }
    }
}
