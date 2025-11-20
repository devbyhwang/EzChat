<%@ Page Title="채팅방 만들기" Language="C#" MasterPageFile="~/MasterPages/Site.Master" AutoEventWireup="true" CodeBehind="Create.aspx.cs" Inherits="EzChat.Web.Chat.CreatePage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <h1>채팅방 만들기</h1>

    <div class="form-container">
        <div class="form-group">
            <asp:Label ID="lblName" runat="server" AssociatedControlID="txtName" Text="채팅방 이름"></asp:Label>
            <asp:TextBox ID="txtName" runat="server" CssClass="form-control" MaxLength="100"></asp:TextBox>
            <asp:RequiredFieldValidator ID="rfvName" runat="server" ControlToValidate="txtName"
                ErrorMessage="채팅방 이름을 입력해주세요." CssClass="text-danger" Display="Dynamic"></asp:RequiredFieldValidator>
        </div>

        <div class="form-group">
            <asp:Label ID="lblDescription" runat="server" AssociatedControlID="txtDescription" Text="설명"></asp:Label>
            <asp:TextBox ID="txtDescription" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="3" MaxLength="500"></asp:TextBox>
        </div>

        <div class="form-group">
            <asp:Label ID="lblMaxUsers" runat="server" AssociatedControlID="txtMaxUsers" Text="최대 인원"></asp:Label>
            <asp:TextBox ID="txtMaxUsers" runat="server" CssClass="form-control" TextMode="Number" Text="50"></asp:TextBox>
            <asp:RangeValidator ID="rvMaxUsers" runat="server" ControlToValidate="txtMaxUsers"
                MinimumValue="2" MaximumValue="100" Type="Integer"
                ErrorMessage="최대 인원은 2명 이상 100명 이하로 설정해주세요." CssClass="text-danger" Display="Dynamic"></asp:RangeValidator>
        </div>

        <asp:Button ID="btnCreate" runat="server" Text="만들기" CssClass="btn btn-primary" OnClick="btnCreate_Click" />
        <a href="/Chat/Default.aspx" class="btn btn-secondary">취소</a>
    </div>
</asp:Content>
