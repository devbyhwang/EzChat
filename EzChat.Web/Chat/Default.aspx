<%@ Page Title="채팅방 목록" Language="C#" MasterPageFile="~/MasterPages/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="EzChat.Web.Chat.DefaultPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <h1>채팅방 목록</h1>

    <div class="chat-header">
        <a href="/Chat/Create.aspx" class="btn btn-primary">새 채팅방 만들기</a>
    </div>

    <div class="room-list">
        <asp:Repeater ID="rptRooms" runat="server">
            <ItemTemplate>
                <div class="room-card">
                    <h3><%# Eval("Name") %></h3>
                    <%# !string.IsNullOrEmpty(Eval("Description")?.ToString()) ? "<p class='room-description'>" + Eval("Description") + "</p>" : "" %>
                    <div class="room-meta">
                        <span>생성자: <%# Eval("CreatedByName") ?? Eval("CreatedByEmail") %></span>
                        <span>최대 인원: <%# Eval("MaxUsers") %>명</span>
                    </div>
                    <a href="/Chat/Room.aspx?id=<%# Eval("Id") %>" class="btn btn-primary">입장</a>
                </div>
            </ItemTemplate>
        </asp:Repeater>

        <asp:Panel ID="pnlNoRooms" runat="server" Visible="false">
            <p>채팅방이 없습니다. 새 채팅방을 만들어보세요!</p>
        </asp:Panel>
    </div>
</asp:Content>
