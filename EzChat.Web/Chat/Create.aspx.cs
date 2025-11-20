using System;
using EzChat.Web.App_Code.BLL;

namespace EzChat.Web.Chat
{
    public partial class CreatePage : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void btnCreate_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid) return;

            int userId = Convert.ToInt32(Session["UserId"]);
            string name = txtName.Text.Trim();
            string description = txtDescription.Text.Trim();
            int maxUsers = int.Parse(txtMaxUsers.Text);

            int roomId = ChatBLL.CreateRoom(name, description, userId, maxUsers);

            Session["SuccessMessage"] = "채팅방이 생성되었습니다.";
            Response.Redirect($"/Chat/Room.aspx?id={roomId}");
        }
    }
}
