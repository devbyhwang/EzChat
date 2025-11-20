using System;
using EzChat.Web.App_Code.BLL;
using EzChat.Web.App_Code.Models;

namespace EzChat.Web.Chat
{
    public partial class RoomPage : System.Web.UI.Page
    {
        private int RoomId;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!int.TryParse(Request.QueryString["id"], out RoomId))
            {
                Response.Redirect("/Chat/Default.aspx");
                return;
            }

            hdnRoomId.Value = RoomId.ToString();

            if (!IsPostBack)
            {
                LoadRoom();
                LoadMessages();
            }
        }

        private void LoadRoom()
        {
            ChatRoom room = ChatBLL.GetRoomById(RoomId);

            if (room == null)
            {
                Response.Redirect("/Chat/Default.aspx");
                return;
            }

            Page.Title = room.Name;
            litRoomName.Text = room.Name;

            if (!string.IsNullOrEmpty(room.Description))
            {
                litDescription.Text = $"<p>{room.Description}</p>";
            }

            // 삭제 권한 확인
            if (Session["UserId"] != null)
            {
                int userId = Convert.ToInt32(Session["UserId"]);
                bool isAdmin = Session["IsAdmin"] != null && Convert.ToBoolean(Session["IsAdmin"]);

                if (room.CreatedById == userId || isAdmin)
                {
                    btnDelete.Visible = true;
                }
            }
        }

        private void LoadMessages()
        {
            var messages = ChatBLL.GetRecentMessages(RoomId, 50);
            rptMessages.DataSource = messages;
            rptMessages.DataBind();
        }

        protected void btnSend_Click(object sender, EventArgs e)
        {
            if (Session["UserId"] == null || string.IsNullOrWhiteSpace(txtMessage.Text)) return;

            int userId = Convert.ToInt32(Session["UserId"]);
            ChatBLL.SaveMessage(RoomId, userId, txtMessage.Text.Trim());
            txtMessage.Text = "";

            LoadMessages();
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            if (Session["UserId"] == null) return;

            int userId = Convert.ToInt32(Session["UserId"]);
            bool isAdmin = Session["IsAdmin"] != null && Convert.ToBoolean(Session["IsAdmin"]);

            ChatBLL.DeleteRoom(RoomId, userId, isAdmin);
            Session["SuccessMessage"] = "채팅방이 삭제되었습니다.";
            Response.Redirect("/Chat/Default.aspx");
        }
    }
}
