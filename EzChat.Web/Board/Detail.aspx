<%@ Page Title="게시글" Language="C#" MasterPageFile="~/MasterPages/Site.Master" AutoEventWireup="true" CodeBehind="Detail.aspx.cs" Inherits="EzChat.Web.Board.DetailPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="post-detail">
        <h1><asp:Literal ID="litTitle" runat="server"></asp:Literal></h1>

        <div class="post-meta">
            <span>작성자: <asp:Literal ID="litAuthor" runat="server"></asp:Literal></span>
            <span>작성일: <asp:Literal ID="litCreatedAt" runat="server"></asp:Literal></span>
            <span>조회수: <asp:Literal ID="litViewCount" runat="server"></asp:Literal></span>
        </div>

        <div class="post-content">
            <asp:Literal ID="litContent" runat="server"></asp:Literal>
        </div>

        <div class="post-actions">
            <a href="/Board/Default.aspx" class="btn btn-secondary">목록</a>
            <asp:HyperLink ID="lnkEdit" runat="server" CssClass="btn btn-primary" Visible="false">수정</asp:HyperLink>
            <asp:Button ID="btnDelete" runat="server" Text="삭제" CssClass="btn btn-danger" Visible="false"
                OnClick="btnDelete_Click" OnClientClick="return confirm('정말 삭제하시겠습니까?');" />
        </div>
    </div>

    <div class="comments-section">
        <h3>댓글 (<asp:Literal ID="litCommentCount" runat="server"></asp:Literal>)</h3>

        <% if (Request.IsAuthenticated) { %>
        <div class="comment-form">
            <asp:TextBox ID="txtComment" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="3"
                placeholder="댓글을 입력하세요..."></asp:TextBox>
            <asp:Button ID="btnAddComment" runat="server" Text="댓글 작성" CssClass="btn btn-primary" OnClick="btnAddComment_Click" />
        </div>
        <% } %>

        <div class="comments-list">
            <asp:Repeater ID="rptComments" runat="server" OnItemCommand="rptComments_ItemCommand">
                <ItemTemplate>
                    <div class="comment">
                        <div class="comment-header">
                            <strong><%# Eval("AuthorName") ?? Eval("AuthorEmail") %></strong>
                            <span><%# Convert.ToDateTime(Eval("CreatedAt")).ToString("yyyy-MM-dd HH:mm") %></span>
                        </div>
                        <div class="comment-content"><%# Eval("Content") %></div>
                        <asp:Button ID="btnDeleteComment" runat="server" Text="삭제" CssClass="btn btn-sm btn-danger"
                            CommandName="Delete" CommandArgument='<%# Eval("Id") %>'
                            Visible='<%# CanDeleteComment(Eval("AuthorId")) %>'
                            OnClientClick="return confirm('댓글을 삭제하시겠습니까?');" />
                    </div>
                </ItemTemplate>
            </asp:Repeater>
        </div>
    </div>
</asp:Content>
