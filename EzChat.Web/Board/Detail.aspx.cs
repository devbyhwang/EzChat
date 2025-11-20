using System;
using System.Web.UI.WebControls;
using EzChat.Web.App_Code.BLL;
using EzChat.Web.App_Code.Models;

namespace EzChat.Web.Board
{
    public partial class DetailPage : System.Web.UI.Page
    {
        private int PostId;
        private BoardPost CurrentPost;

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

            BoardBLL.IncrementViewCount(PostId);

            Page.Title = CurrentPost.Title;
            litTitle.Text = CurrentPost.Title;
            litAuthor.Text = CurrentPost.AuthorName;
            litCreatedAt.Text = CurrentPost.CreatedAt.ToString("yyyy-MM-dd HH:mm");
            litViewCount.Text = (CurrentPost.ViewCount + 1).ToString();
            litContent.Text = CurrentPost.Content.Replace("\n", "<br/>");

            // 권한 확인
            if (Session["UserId"] != null)
            {
                int userId = Convert.ToInt32(Session["UserId"]);
                bool isAdmin = Session["IsAdmin"] != null && Convert.ToBoolean(Session["IsAdmin"]);

                if (CurrentPost.AuthorId == userId)
                {
                    lnkEdit.NavigateUrl = $"/Board/Edit.aspx?id={PostId}";
                    lnkEdit.Visible = true;
                }

                if (CurrentPost.AuthorId == userId || isAdmin)
                {
                    btnDelete.Visible = true;
                }
            }

            LoadComments();
        }

        private void LoadComments()
        {
            var comments = BoardBLL.GetComments(PostId);
            litCommentCount.Text = comments.Rows.Count.ToString();
            rptComments.DataSource = comments;
            rptComments.DataBind();
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

        protected void btnAddComment_Click(object sender, EventArgs e)
        {
            if (Session["UserId"] == null || string.IsNullOrWhiteSpace(txtComment.Text)) return;

            int userId = Convert.ToInt32(Session["UserId"]);
            BoardBLL.AddComment(PostId, txtComment.Text.Trim(), userId);
            txtComment.Text = "";

            LoadComments();
        }

        protected void rptComments_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "Delete")
            {
                if (Session["UserId"] == null) return;

                int commentId = Convert.ToInt32(e.CommandArgument);
                int userId = Convert.ToInt32(Session["UserId"]);
                bool isAdmin = Session["IsAdmin"] != null && Convert.ToBoolean(Session["IsAdmin"]);

                BoardBLL.DeleteComment(commentId, userId, isAdmin);
                LoadComments();
            }
        }

        protected bool CanDeleteComment(object authorId)
        {
            if (Session["UserId"] == null) return false;

            int userId = Convert.ToInt32(Session["UserId"]);
            bool isAdmin = Session["IsAdmin"] != null && Convert.ToBoolean(Session["IsAdmin"]);
            int commentAuthorId = Convert.ToInt32(authorId);

            return commentAuthorId == userId || isAdmin;
        }
    }
}
