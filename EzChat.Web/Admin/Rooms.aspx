<%@ Page Title="채팅방 관리" Language="C#" MasterPageFile="~/MasterPages/Site.Master" AutoEventWireup="true" CodeBehind="Rooms.aspx.cs" Inherits="EzChat.Web.Admin.RoomsPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <h1>채팅방 관리</h1>

    <asp:GridView ID="gvRooms" runat="server" AutoGenerateColumns="false" CssClass="table"
        OnRowCommand="gvRooms_RowCommand" DataKeyNames="Id">
        <Columns>
            <asp:BoundField DataField="Id" HeaderText="ID" />
            <asp:BoundField DataField="Name" HeaderText="이름" />
            <asp:TemplateField HeaderText="생성자">
                <ItemTemplate>
                    <%# Eval("CreatedByName") ?? Eval("CreatedByEmail") %>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:BoundField DataField="CreatedAt" HeaderText="생성일" DataFormatString="{0:yyyy-MM-dd HH:mm}" />
            <asp:TemplateField HeaderText="상태">
                <ItemTemplate>
                    <%# Convert.ToBoolean(Eval("IsActive")) ? "<span class='badge badge-success'>활성</span>" : "<span class='badge badge-danger'>삭제됨</span>" %>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="작업">
                <ItemTemplate>
                    <asp:Button ID="btnDelete" runat="server" CommandName="DeleteRoom" CommandArgument='<%# Eval("Id") %>'
                        Text="삭제" CssClass="btn btn-sm btn-danger"
                        Visible='<%# Convert.ToBoolean(Eval("IsActive")) %>'
                        OnClientClick="return confirm('채팅방을 삭제하시겠습니까?');" />
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>

    <a href="/Admin/Default.aspx" class="btn btn-secondary">대시보드로 돌아가기</a>
</asp:Content>
