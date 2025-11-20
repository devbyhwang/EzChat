using System;
using System.Web.UI.WebControls;
using EzChat.Web.App_Code.BLL;

namespace EzChat.Web.Admin
{
    public partial class RoomsPage : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadRooms();
            }
        }

        private void LoadRooms()
        {
            gvRooms.DataSource = ChatBLL.GetAllRooms();
            gvRooms.DataBind();
        }

        protected void gvRooms_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "DeleteRoom")
            {
                int roomId = Convert.ToInt32(e.CommandArgument);
                int adminId = Convert.ToInt32(Session["UserId"]);
                string ipAddress = Request.ServerVariables["REMOTE_ADDR"];

                ChatBLL.DeleteRoom(roomId, adminId, true);
                IpBanBLL.LogAction(adminId, "DeleteRoom", "ChatRoom", roomId.ToString(), null, ipAddress);
                Session["SuccessMessage"] = "채팅방이 삭제되었습니다.";

                Response.Redirect(Request.Url.AbsoluteUri);
            }
        }
    }
}
