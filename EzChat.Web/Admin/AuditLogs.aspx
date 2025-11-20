<%@ Page Title="감사 로그" Language="C#" MasterPageFile="~/MasterPages/Site.Master" AutoEventWireup="true" CodeBehind="AuditLogs.aspx.cs" Inherits="EzChat.Web.Admin.AuditLogsPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <h1>감사 로그</h1>

    <asp:GridView ID="gvLogs" runat="server" AutoGenerateColumns="false" CssClass="table">
        <Columns>
            <asp:BoundField DataField="Timestamp" HeaderText="시간" DataFormatString="{0:yyyy-MM-dd HH:mm:ss}" />
            <asp:TemplateField HeaderText="관리자">
                <ItemTemplate>
                    <%# Eval("AdminName") ?? Eval("AdminEmail") %>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:BoundField DataField="Action" HeaderText="작업" />
            <asp:BoundField DataField="TargetType" HeaderText="대상 유형" NullDisplayText="-" />
            <asp:BoundField DataField="TargetId" HeaderText="대상 ID" NullDisplayText="-" />
            <asp:BoundField DataField="Details" HeaderText="상세" NullDisplayText="-" />
            <asp:BoundField DataField="IpAddress" HeaderText="IP" NullDisplayText="-" />
        </Columns>
    </asp:GridView>

    <a href="/Admin/Default.aspx" class="btn btn-secondary">대시보드로 돌아가기</a>
</asp:Content>
