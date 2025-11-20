<%@ Page Title="사용자 관리" Language="C#" MasterPageFile="~/MasterPages/Site.Master" AutoEventWireup="true" CodeBehind="Users.aspx.cs" Inherits="EzChat.Web.Admin.UsersPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <h1>사용자 관리</h1>

    <asp:GridView ID="gvUsers" runat="server" AutoGenerateColumns="false" CssClass="table"
        OnRowCommand="gvUsers_RowCommand" DataKeyNames="Id">
        <Columns>
            <asp:BoundField DataField="Email" HeaderText="이메일" />
            <asp:BoundField DataField="DisplayName" HeaderText="닉네임" />
            <asp:BoundField DataField="CreatedAt" HeaderText="가입일" DataFormatString="{0:yyyy-MM-dd}" />
            <asp:BoundField DataField="LastLoginAt" HeaderText="마지막 로그인" DataFormatString="{0:yyyy-MM-dd HH:mm}" NullDisplayText="-" />
            <asp:TemplateField HeaderText="상태">
                <ItemTemplate>
                    <%# Convert.ToBoolean(Eval("IsActive")) ? "<span class='badge badge-success'>활성</span>" : "<span class='badge badge-danger'>비활성</span>" %>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="작업">
                <ItemTemplate>
                    <asp:Button ID="btnToggle" runat="server" CommandName="ToggleActive" CommandArgument='<%# Eval("Id") %>'
                        Text='<%# Convert.ToBoolean(Eval("IsActive")) ? "비활성화" : "활성화" %>' CssClass="btn btn-sm btn-warning" />
                    <asp:Button ID="btnDelete" runat="server" CommandName="DeleteUser" CommandArgument='<%# Eval("Id") %>'
                        Text="삭제" CssClass="btn btn-sm btn-danger"
                        OnClientClick="return confirm('정말 삭제하시겠습니까?');" />
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>

    <a href="/Admin/Default.aspx" class="btn btn-secondary">대시보드로 돌아가기</a>
</asp:Content>
