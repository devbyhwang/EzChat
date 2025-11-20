<%@ Page Title="회원가입" Language="C#" MasterPageFile="~/MasterPages/Site.Master" AutoEventWireup="true" CodeBehind="Register.aspx.cs" Inherits="EzChat.Web.Account.RegisterPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="auth-form">
        <h1>회원가입</h1>

        <asp:Panel ID="pnlError" runat="server" CssClass="alert alert-danger" Visible="false">
            <asp:Literal ID="litError" runat="server"></asp:Literal>
        </asp:Panel>

        <div class="form-group">
            <asp:Label ID="lblEmail" runat="server" AssociatedControlID="txtEmail" Text="이메일"></asp:Label>
            <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" TextMode="Email"></asp:TextBox>
            <asp:RequiredFieldValidator ID="rfvEmail" runat="server" ControlToValidate="txtEmail"
                ErrorMessage="이메일을 입력해주세요." CssClass="text-danger" Display="Dynamic"></asp:RequiredFieldValidator>
            <asp:RegularExpressionValidator ID="revEmail" runat="server" ControlToValidate="txtEmail"
                ValidationExpression="^[^@\s]+@[^@\s]+\.[^@\s]+$"
                ErrorMessage="올바른 이메일 형식이 아닙니다." CssClass="text-danger" Display="Dynamic"></asp:RegularExpressionValidator>
        </div>

        <div class="form-group">
            <asp:Label ID="lblDisplayName" runat="server" AssociatedControlID="txtDisplayName" Text="닉네임"></asp:Label>
            <asp:TextBox ID="txtDisplayName" runat="server" CssClass="form-control" MaxLength="50"></asp:TextBox>
            <asp:RequiredFieldValidator ID="rfvDisplayName" runat="server" ControlToValidate="txtDisplayName"
                ErrorMessage="닉네임을 입력해주세요." CssClass="text-danger" Display="Dynamic"></asp:RequiredFieldValidator>
        </div>

        <div class="form-group">
            <asp:Label ID="lblPassword" runat="server" AssociatedControlID="txtPassword" Text="비밀번호"></asp:Label>
            <asp:TextBox ID="txtPassword" runat="server" CssClass="form-control" TextMode="Password"></asp:TextBox>
            <asp:RequiredFieldValidator ID="rfvPassword" runat="server" ControlToValidate="txtPassword"
                ErrorMessage="비밀번호를 입력해주세요." CssClass="text-danger" Display="Dynamic"></asp:RequiredFieldValidator>
            <small class="form-text">최소 8자, 대문자/소문자/숫자/특수문자 포함</small>
        </div>

        <div class="form-group">
            <asp:Label ID="lblConfirmPassword" runat="server" AssociatedControlID="txtConfirmPassword" Text="비밀번호 확인"></asp:Label>
            <asp:TextBox ID="txtConfirmPassword" runat="server" CssClass="form-control" TextMode="Password"></asp:TextBox>
            <asp:RequiredFieldValidator ID="rfvConfirmPassword" runat="server" ControlToValidate="txtConfirmPassword"
                ErrorMessage="비밀번호 확인을 입력해주세요." CssClass="text-danger" Display="Dynamic"></asp:RequiredFieldValidator>
            <asp:CompareValidator ID="cvPassword" runat="server" ControlToValidate="txtConfirmPassword"
                ControlToCompare="txtPassword" ErrorMessage="비밀번호가 일치하지 않습니다."
                CssClass="text-danger" Display="Dynamic"></asp:CompareValidator>
        </div>

        <asp:Button ID="btnRegister" runat="server" Text="회원가입" CssClass="btn btn-primary" OnClick="btnRegister_Click" />

        <p class="auth-link">
            이미 계정이 있으신가요? <a href="/Account/Login.aspx">로그인</a>
        </p>
    </div>
</asp:Content>
