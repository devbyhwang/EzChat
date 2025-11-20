<%@ Page Title="글쓰기" Language="C#" MasterPageFile="~/MasterPages/Site.Master" AutoEventWireup="true" CodeBehind="Create.aspx.cs" Inherits="EzChat.Web.Board.CreatePage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <h1>글쓰기</h1>

    <div class="form-container">
        <div class="form-group">
            <asp:Label ID="lblTitle" runat="server" AssociatedControlID="txtTitle" Text="제목"></asp:Label>
            <asp:TextBox ID="txtTitle" runat="server" CssClass="form-control" MaxLength="200"></asp:TextBox>
            <asp:RequiredFieldValidator ID="rfvTitle" runat="server" ControlToValidate="txtTitle"
                ErrorMessage="제목을 입력해주세요." CssClass="text-danger" Display="Dynamic"></asp:RequiredFieldValidator>
        </div>

        <div class="form-group">
            <asp:Label ID="lblContent" runat="server" AssociatedControlID="txtContent" Text="내용"></asp:Label>
            <asp:TextBox ID="txtContent" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="10"></asp:TextBox>
            <asp:RequiredFieldValidator ID="rfvContent" runat="server" ControlToValidate="txtContent"
                ErrorMessage="내용을 입력해주세요." CssClass="text-danger" Display="Dynamic"></asp:RequiredFieldValidator>
        </div>

        <asp:Button ID="btnCreate" runat="server" Text="작성" CssClass="btn btn-primary" OnClick="btnCreate_Click" />
        <a href="/Board/Default.aspx" class="btn btn-secondary">취소</a>
    </div>
</asp:Content>
