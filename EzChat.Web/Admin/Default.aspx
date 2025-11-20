<%@ Page Title="관리자 대시보드" Language="C#" MasterPageFile="~/MasterPages/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="EzChat.Web.Admin.DefaultPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <h1>관리자 대시보드</h1>

    <div class="dashboard-stats">
        <div class="stat-card">
            <h3>총 사용자</h3>
            <p class="stat-number"><asp:Literal ID="litTotalUsers" runat="server"></asp:Literal></p>
        </div>
        <div class="stat-card">
            <h3>활성 사용자</h3>
            <p class="stat-number"><asp:Literal ID="litActiveUsers" runat="server"></asp:Literal></p>
        </div>
        <div class="stat-card">
            <h3>채팅방</h3>
            <p class="stat-number"><asp:Literal ID="litTotalRooms" runat="server"></asp:Literal></p>
        </div>
        <div class="stat-card">
            <h3>게시글</h3>
            <p class="stat-number"><asp:Literal ID="litTotalPosts" runat="server"></asp:Literal></p>
        </div>
        <div class="stat-card">
            <h3>IP 차단</h3>
            <p class="stat-number"><asp:Literal ID="litTotalBans" runat="server"></asp:Literal></p>
        </div>
        <div class="stat-card">
            <h3>오늘 로그인</h3>
            <p class="stat-number"><asp:Literal ID="litTodayLogins" runat="server"></asp:Literal></p>
        </div>
    </div>

    <div class="admin-menu">
        <h2>관리 메뉴</h2>
        <ul class="admin-links">
            <li><a href="/Admin/Users.aspx">사용자 관리</a></li>
            <li><a href="/Admin/IpBans.aspx">IP 차단 관리</a></li>
            <li><a href="/Admin/Rooms.aspx">채팅방 관리</a></li>
            <li><a href="/Admin/AuditLogs.aspx">감사 로그</a></li>
        </ul>
    </div>
</asp:Content>
