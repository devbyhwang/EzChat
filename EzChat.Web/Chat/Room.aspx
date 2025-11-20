<%@ Page Title="채팅방" Language="C#" MasterPageFile="~/MasterPages/Site.Master" AutoEventWireup="true" CodeBehind="Room.aspx.cs" Inherits="EzChat.Web.Chat.RoomPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="chat-room">
        <div class="chat-room-header">
            <h1><asp:Literal ID="litRoomName" runat="server"></asp:Literal></h1>
            <asp:Literal ID="litDescription" runat="server"></asp:Literal>
            <div class="chat-room-actions">
                <a href="/Chat/Default.aspx" class="btn btn-secondary">목록으로</a>
                <asp:Button ID="btnDelete" runat="server" Text="삭제" CssClass="btn btn-danger" Visible="false"
                    OnClick="btnDelete_Click" OnClientClick="return confirm('채팅방을 삭제하시겠습니까?');" />
            </div>
        </div>

        <div id="chat-messages" class="chat-messages">
            <asp:Repeater ID="rptMessages" runat="server">
                <ItemTemplate>
                    <div class="message <%# Convert.ToInt32(Eval("UserId")) == Convert.ToInt32(Session["UserId"]) ? "my-message" : "" %>">
                        <div class="message-header">
                            <strong><%# Eval("UserName") ?? Eval("UserEmail") %></strong>
                            <span><%# Convert.ToDateTime(Eval("SentAt")).ToString("HH:mm") %></span>
                        </div>
                        <div class="message-content"><%# Eval("Content") %></div>
                    </div>
                </ItemTemplate>
            </asp:Repeater>
        </div>

        <div class="chat-input">
            <asp:TextBox ID="txtMessage" runat="server" CssClass="form-control" placeholder="메시지를 입력하세요..." MaxLength="2000"></asp:TextBox>
            <asp:Button ID="btnSend" runat="server" Text="전송" CssClass="btn btn-primary" OnClick="btnSend_Click" />
        </div>
    </div>

    <asp:HiddenField ID="hdnRoomId" runat="server" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ScriptsContent" runat="server">
    <script>
        // 페이지 로드 시 스크롤을 맨 아래로
        document.getElementById("chat-messages").scrollTop = document.getElementById("chat-messages").scrollHeight;

        // 메시지 입력 시 엔터키로 전송
        document.getElementById('<%= txtMessage.ClientID %>').addEventListener('keypress', function(e) {
            if (e.key === 'Enter') {
                document.getElementById('<%= btnSend.ClientID %>').click();
                e.preventDefault();
            }
        });
    </script>
</asp:Content>
