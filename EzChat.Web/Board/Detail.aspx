<%@ Page Title="게시글" Language="C#" MasterPageFile="~/MasterPages/Site.Master" AutoEventWireup="true" CodeBehind="Detail.aspx.cs" Inherits="EzChat.Web.Board.DetailPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="post-detail">
        <h1><asp:Literal ID="litTitle" runat="server"></asp:Literal></h1>

        <div class="post-meta">
            <span>작성자: <asp:Literal ID="litAuthor" runat="server"></asp:Literal></span>
            <span>작성일: <asp:Literal ID="litCreatedAt" runat="server"></asp:Literal></span>
        </div>

        <div class="post-content">
            <asp:Literal ID="litContent" runat="server"></asp:Literal>
        </div>

        <div class="post-actions">
            <a href="/Board/Default.aspx" class="btn btn-secondary">목록</a>
            <asp:Button ID="btnEdit" runat="server" Text="수정" CssClass="btn btn-primary" Visible="false" OnClick="btnEdit_Click" />
            <asp:Button ID="btnDelete" runat="server" Text="삭제" CssClass="btn btn-danger" Visible="false"
                OnClick="btnDelete_Click" OnClientClick="return confirm('정말 삭제하시겠습니까?');" />
        </div>
    </div>

    <!-- 수정 폼 (숨김 상태) -->
    <asp:Panel ID="pnlEdit" runat="server" Visible="false" CssClass="form-container" style="margin-top: 2rem;">
        <h2>게시글 수정</h2>
        <div class="form-group">
            <asp:Label ID="lblEditTitle" runat="server" AssociatedControlID="txtEditTitle" Text="제목"></asp:Label>
            <asp:TextBox ID="txtEditTitle" runat="server" CssClass="form-control" MaxLength="200"></asp:TextBox>
        </div>
        <div class="form-group">
            <asp:Label ID="lblEditContent" runat="server" AssociatedControlID="txtEditContent" Text="내용"></asp:Label>
            <asp:TextBox ID="txtEditContent" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="10"></asp:TextBox>
        </div>
        <asp:Button ID="btnSaveEdit" runat="server" Text="저장" CssClass="btn btn-primary" OnClick="btnSaveEdit_Click" />
        <asp:Button ID="btnCancelEdit" runat="server" Text="취소" CssClass="btn btn-secondary" OnClick="btnCancelEdit_Click" CausesValidation="false" />
    </asp:Panel>
</asp:Content>
