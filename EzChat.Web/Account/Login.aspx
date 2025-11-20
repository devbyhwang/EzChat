<%@ Page Title="로그인" Language="C#" MasterPageFile="~/MasterPages/Site.Master" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="EzChat.Web.Account.LoginPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="auth-form">
        <h1>로그인</h1>

        <asp:Panel ID="pnlError" runat="server" CssClass="alert alert-danger" Visible="false">
            <asp:Literal ID="litError" runat="server"></asp:Literal>
        </asp:Panel>

        <div class="form-group">
            <asp:Label ID="lblEmail" runat="server" AssociatedControlID="txtEmail" Text="이메일"></asp:Label>
            <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" TextMode="Email"></asp:TextBox>
            <asp:RequiredFieldValidator ID="rfvEmail" runat="server" ControlToValidate="txtEmail"
                ErrorMessage="이메일을 입력해주세요." CssClass="text-danger" Display="Dynamic"></asp:RequiredFieldValidator>
        </div>

        <div class="form-group">
            <asp:Label ID="lblPassword" runat="server" AssociatedControlID="txtPassword" Text="비밀번호"></asp:Label>
            <asp:TextBox ID="txtPassword" runat="server" CssClass="form-control" TextMode="Password"></asp:TextBox>
            <asp:RequiredFieldValidator ID="rfvPassword" runat="server" ControlToValidate="txtPassword"
                ErrorMessage="비밀번호를 입력해주세요." CssClass="text-danger" Display="Dynamic"></asp:RequiredFieldValidator>
        </div>

        <div class="form-group form-check">
            <asp:CheckBox ID="chkRememberMe" runat="server" />
            <asp:Label ID="lblRememberMe" runat="server" AssociatedControlID="chkRememberMe" Text="로그인 상태 유지"></asp:Label>
        </div>

        <asp:Button ID="btnLogin" runat="server" Text="로그인" CssClass="btn btn-primary" OnClick="btnLogin_Click" />

        <p class="auth-link">
            계정이 없으신가요? <a href="/Account/Register.aspx">회원가입</a>
        </p>
    </div>
</asp:Content>
