<%@ Page Title="관리자" Language="C#" MasterPageFile="~/MasterPages/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="EzChat.Web.Admin.DefaultPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <h1>관리자 페이지</h1>

    <asp:Panel ID="pnlSuccess" runat="server" CssClass="alert alert-success" Visible="false">
        <asp:Literal ID="litSuccess" runat="server"></asp:Literal>
    </asp:Panel>

    <asp:Panel ID="pnlError" runat="server" CssClass="alert alert-danger" Visible="false">
        <asp:Literal ID="litError" runat="server"></asp:Literal>
    </asp:Panel>

    <!-- 통계 -->
    <div class="dashboard-stats">
        <div class="stat-card">
            <h3>총 사용자</h3>
            <p class="stat-number"><asp:Literal ID="litTotalUsers" runat="server"></asp:Literal></p>
        </div>
        <div class="stat-card">
            <h3>총 게시글</h3>
            <p class="stat-number"><asp:Literal ID="litTotalPosts" runat="server"></asp:Literal></p>
        </div>
    </div>

    <!-- 탭 메뉴 -->
    <div class="admin-tabs" style="margin-top: 2rem;">
        <asp:LinkButton ID="lnkTabUsers" runat="server" CssClass="btn btn-primary" OnClick="lnkTabUsers_Click">사용자 관리</asp:LinkButton>
        <asp:LinkButton ID="lnkTabPosts" runat="server" CssClass="btn btn-secondary" OnClick="lnkTabPosts_Click">게시글 관리</asp:LinkButton>
    </div>

    <!-- 사용자 관리 탭 -->
    <asp:Panel ID="pnlUsers" runat="server" style="margin-top: 1rem;">
        <h2>사용자 목록</h2>
        <asp:GridView ID="gvUsers" runat="server" AutoGenerateColumns="false" CssClass="table"
            OnRowCommand="gvUsers_RowCommand" DataKeyNames="UserID">
            <Columns>
                <asp:BoundField DataField="UserID" HeaderText="ID" />
                <asp:BoundField DataField="UserLoginID" HeaderText="로그인 ID" />
                <asp:BoundField DataField="Username" HeaderText="사용자 이름" />
                <asp:TemplateField HeaderText="권한">
                    <ItemTemplate>
                        <%# Convert.ToBoolean(Eval("IsAdmin")) ? "<span class='badge badge-success'>관리자</span>" : "<span class='badge badge-secondary'>일반</span>" %>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="작업">
                    <ItemTemplate>
                        <asp:Button ID="btnDelete" runat="server" CommandName="DeleteUser" CommandArgument='<%# Eval("UserID") %>'
                            Text="삭제" CssClass="btn btn-sm btn-danger"
                            Visible='<%# !Convert.ToBoolean(Eval("IsAdmin")) %>'
                            OnClientClick="return confirm('정말 삭제하시겠습니까? 해당 사용자의 게시글도 모두 삭제됩니다.');" />
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
    </asp:Panel>

    <!-- 게시글 관리 탭 -->
    <asp:Panel ID="pnlPosts" runat="server" Visible="false" style="margin-top: 1rem;">
        <h2>게시글 목록</h2>
        <asp:GridView ID="gvPosts" runat="server" AutoGenerateColumns="false" CssClass="table"
            OnRowCommand="gvPosts_RowCommand" DataKeyNames="PostID">
            <Columns>
                <asp:BoundField DataField="PostID" HeaderText="ID" />
                <asp:TemplateField HeaderText="제목">
                    <ItemTemplate>
                        <a href="/Board/Detail.aspx?id=<%# Eval("PostID") %>"><%# Eval("Title") %></a>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="Username" HeaderText="작성자" />
                <asp:BoundField DataField="CreatedAt" HeaderText="작성일" DataFormatString="{0:yyyy-MM-dd HH:mm}" />
                <asp:TemplateField HeaderText="작업">
                    <ItemTemplate>
                        <asp:Button ID="btnDeletePost" runat="server" CommandName="DeletePost" CommandArgument='<%# Eval("PostID") %>'
                            Text="삭제" CssClass="btn btn-sm btn-danger"
                            OnClientClick="return confirm('정말 삭제하시겠습니까?');" />
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
    </asp:Panel>
</asp:Content>
