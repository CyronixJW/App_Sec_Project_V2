<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Welcome.aspx.cs" Inherits="App_Sec_Project.Welcome" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <div class="form-group">

        <h2 class="h2">User Profile</h2>
    </div>
    <div class="form-group">

        
    Welcome <asp:Label runat="server" ID="lbl_name"></asp:Label>
        Your UserID is <asp:Label runat="server" ID="lbl_userid"></asp:Label>
    </div>
</asp:Content>
