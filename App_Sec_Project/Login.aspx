<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="App_Sec_Project.Login" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
     <script src="https://www.google.com/recaptcha/api.js?render=6LcPlEkaAAAAANdf6QMsUxJPuC-B8PLcJSMKVQPx"></script>
    <script>

        grecaptcha.ready(function () {

            grecaptcha.execute('6LcPlEkaAAAAANdf6QMsUxJPuC-B8PLcJSMKVQPx', { action: 'Login' }).then(function (token) {

                document.getElementById('<%=g.ClientID%>').value = token;
                console.log(token);
            });
            
        })
    </script>
    <div class="form-group">

            <h2 class="h2">Login</h2>
        </div>
                    
               
              
                    <div class="form-group">
                    <asp:Label runat="server" CssClass="control-label">Email Address</asp:Label>
                        <asp:TextBox runat="server" ID="tb_email" CssClass="form-control" TextMode="Email"></asp:TextBox><asp:Label runat="server" ID="lbl_email_validate"></asp:Label>
                        </div>
         <div class="form-group">
              <asp:Label runat="server" CssClass=" control-label" >Password</asp:Label>
               <asp:TextBox runat="server" ID="tb_pwd" CssClass="form-control" TextMode="Password"></asp:TextBox><asp:Label runat="server" ID="lbl_pwd_validate"></asp:Label>

             </div>
             <div class="form-group">
                  <asp:Button runat="server"  CssClass="btn-primary btn" ID="btn_login" Width="100%" Text="Login" OnClick="btn_login_Click" />
                 </div>
        
             <asp:Label runat="server" ID="lbl_errormsg" CssClass=" control-label label-danger"></asp:Label>
               <div class="form-group">
                  <asp:HiddenField runat="server" ID="g" />
                   <asp:Label runat="server" EnableViewState="false" ID="lbl_messsage"></asp:Label>
               </div>

</asp:Content>
