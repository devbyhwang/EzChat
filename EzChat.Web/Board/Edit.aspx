<%@ Page Title="글 수정" Language="C#" MasterPageFile="~/MasterPages/Site.Master" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="EzChat.Web.Board.EditPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <h1>글 수정</h1>

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

        <asp:Button ID="btnUpdate" runat="server" Text="수정" CssClass="btn btn-primary" OnClick="btnUpdate_Click" />
        <asp:HyperLink ID="lnkCancel" runat="server" CssClass="btn btn-secondary">취소</asp:HyperLink>
    </div>
</asp:Content>
