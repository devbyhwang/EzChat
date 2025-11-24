<%@ Page Title="홈" Language="C#" MasterPageFile="~/MasterPages/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="EzChat.Web.DefaultPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="hero">
        <h1>EzChat에 오신 것을 환영합니다</h1>
        <p>게시판 기능을 제공하는 커뮤니티 플랫폼입니다.</p>

        <% if (!Request.IsAuthenticated) { %>
            <div class="hero-buttons">
                <a href="/Account/Register.aspx" class="btn btn-primary">회원가입</a>
                <a href="/Account/Login.aspx" class="btn btn-secondary">로그인</a>
            </div>
        <% } else { %>
            <div class="hero-buttons">
                <a href="/Board/Default.aspx" class="btn btn-primary">게시판 보기</a>
            </div>
        <% } %>
    </div>

    <div class="features">
        <div class="feature">
            <h3>게시판</h3>
            <p>정보를 공유하고 의견을 나눌 수 있는 게시판입니다.</p>
        </div>
        <div class="feature">
            <h3>보안</h3>
            <p>안전한 인증 시스템과 보안 정책을 적용했습니다.</p>
        </div>
        <div class="feature">
            <h3>관리</h3>
            <p>효율적인 사용자 및 게시글 관리 기능을 제공합니다.</p>
        </div>
    </div>
</asp:Content>
