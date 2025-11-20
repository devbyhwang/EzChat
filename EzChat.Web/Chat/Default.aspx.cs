using System;
using EzChat.Web.App_Code.BLL;

namespace EzChat.Web.Chat
{
    public partial class DefaultPage : System.Web.UI.Page
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
            var rooms = ChatBLL.GetActiveRooms();
            rptRooms.DataSource = rooms;
            rptRooms.DataBind();

            pnlNoRooms.Visible = rooms.Rows.Count == 0;
        }
    }
}
