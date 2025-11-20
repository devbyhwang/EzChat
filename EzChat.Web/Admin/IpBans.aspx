<%@ Page Title="IP 차단 관리" Language="C#" MasterPageFile="~/MasterPages/Site.Master" AutoEventWireup="true" CodeBehind="IpBans.aspx.cs" Inherits="EzChat.Web.Admin.IpBansPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <h1>IP 차단 관리</h1>

    <asp:Panel ID="pnlBanForm" runat="server" CssClass="form-container" style="margin-bottom: 2rem;">
        <h3>새 IP 차단</h3>
        <div class="form-group">
            <asp:Label ID="lblIpAddress" runat="server" AssociatedControlID="txtIpAddress" Text="IP 주소"></asp:Label>
            <asp:TextBox ID="txtIpAddress" runat="server" CssClass="form-control" placeholder="예: 192.168.1.1"></asp:TextBox>
            <asp:RequiredFieldValidator ID="rfvIpAddress" runat="server" ControlToValidate="txtIpAddress"
                ErrorMessage="IP 주소를 입력해주세요." CssClass="text-danger" Display="Dynamic" ValidationGroup="Ban"></asp:RequiredFieldValidator>
        </div>
        <div class="form-group">
            <asp:Label ID="lblReason" runat="server" AssociatedControlID="txtReason" Text="차단 사유"></asp:Label>
            <asp:TextBox ID="txtReason" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="2"></asp:TextBox>
        </div>
        <div class="form-group">
            <asp:Label ID="lblExpiresAt" runat="server" AssociatedControlID="txtExpiresAt" Text="만료 일시 (비워두면 영구 차단)"></asp:Label>
            <asp:TextBox ID="txtExpiresAt" runat="server" CssClass="form-control" TextMode="DateTimeLocal"></asp:TextBox>
        </div>
        <asp:Button ID="btnBan" runat="server" Text="차단" CssClass="btn btn-danger" OnClick="btnBan_Click" ValidationGroup="Ban" />
    </asp:Panel>

    <asp:GridView ID="gvBans" runat="server" AutoGenerateColumns="false" CssClass="table"
        OnRowCommand="gvBans_RowCommand" DataKeyNames="Id">
        <Columns>
            <asp:BoundField DataField="IpAddress" HeaderText="IP 주소" />
            <asp:BoundField DataField="Reason" HeaderText="사유" NullDisplayText="-" />
            <asp:TemplateField HeaderText="차단자">
                <ItemTemplate>
                    <%# Eval("BannedByName") ?? Eval("BannedByEmail") %>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:BoundField DataField="BannedAt" HeaderText="차단일" DataFormatString="{0:yyyy-MM-dd HH:mm}" />
            <asp:TemplateField HeaderText="만료일">
                <ItemTemplate>
                    <%# Eval("ExpiresAt") == DBNull.Value ? "영구" : Convert.ToDateTime(Eval("ExpiresAt")).ToString("yyyy-MM-dd HH:mm") %>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="작업">
                <ItemTemplate>
                    <asp:Button ID="btnUnban" runat="server" CommandName="Unban" CommandArgument='<%# Eval("Id") %>'
                        Text="해제" CssClass="btn btn-sm btn-warning"
                        OnClientClick="return confirm('차단을 해제하시겠습니까?');" />
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>

    <a href="/Admin/Default.aspx" class="btn btn-secondary">대시보드로 돌아가기</a>
</asp:Content>
