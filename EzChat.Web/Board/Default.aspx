<%@ Page Title="게시판" Language="C#" MasterPageFile="~/MasterPages/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="EzChat.Web.Board.DefaultPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <h1>게시판</h1>

    <div class="board-header">
        <asp:Panel ID="pnlSearch" runat="server" CssClass="search-form" DefaultButton="btnSearch">
            <asp:TextBox ID="txtSearch" runat="server" CssClass="form-control" placeholder="검색어 입력..."></asp:TextBox>
            <asp:Button ID="btnSearch" runat="server" Text="검색" CssClass="btn btn-secondary" OnClick="btnSearch_Click" />
        </asp:Panel>

        <% if (Request.IsAuthenticated) { %>
            <a href="/Board/Create.aspx" class="btn btn-primary">글쓰기</a>
        <% } %>
    </div>

    <asp:GridView ID="gvPosts" runat="server" AutoGenerateColumns="false" CssClass="table" ShowHeader="true">
        <Columns>
            <asp:BoundField DataField="Id" HeaderText="번호" />
            <asp:TemplateField HeaderText="제목">
                <ItemTemplate>
                    <a href="/Board/Detail.aspx?id=<%# Eval("Id") %>"><%# Eval("Title") %></a>
                    <% if (Convert.ToInt32(Eval("CommentCount")) > 0) { %>
                        <span class="comment-count">[<%# Eval("CommentCount") %>]</span>
                    <% } %>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="작성자">
                <ItemTemplate>
                    <%# Eval("AuthorName") ?? Eval("AuthorEmail") %>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:BoundField DataField="CreatedAt" HeaderText="작성일" DataFormatString="{0:yyyy-MM-dd}" />
            <asp:BoundField DataField="ViewCount" HeaderText="조회수" />
        </Columns>
    </asp:GridView>

    <div class="pagination">
        <asp:Repeater ID="rptPager" runat="server" OnItemCommand="rptPager_ItemCommand">
            <ItemTemplate>
                <asp:LinkButton ID="lnkPage" runat="server"
                    Text='<%# Eval("Text") %>'
                    CommandArgument='<%# Eval("Value") %>'
                    CssClass='<%# Convert.ToBoolean(Eval("IsCurrent")) ? "page-item active" : "page-item" %>'
                    Enabled='<%# !Convert.ToBoolean(Eval("IsCurrent")) %>'></asp:LinkButton>
            </ItemTemplate>
        </asp:Repeater>
    </div>
</asp:Content>
