<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ChangePassword.aspx.cs" Inherits="App_Sec_Project.ChangePassword" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
     <script type="text/javascript">
        function validatepassword() {
            var pwdstr = document.getElementById('<%= tb_newpassword.ClientID%>').textContent;
            var strength = document.getElementById('<%= lbl_pwd_validate.ClientID%>');
            var strongRegex = new RegExp("^(?=.{14,})(?=.*[A-Z])(?=.*[a-z])(?=.*[0-9])(?=.*\\W).*$", "g");
            var mediumRegex = new RegExp("^(?=.{10,})(((?=.*[A-Z])(?=.*[a-z]))|((?=.*[A-Z])(?=.*[0-9]))|((?=.*[a-z])(?=.*[0-9]))).*$", "g");
            var enoughRegex = new RegExp("(?=.{8,}).*", "g");
            var pwd = document.getElementById('<%= tb_newpassword.ClientID%>');
            if (pwd.value.length == 0) {
                strength.innerHTML = 'Password cannot be null';
            } else if (false == enoughRegex.test(pwd.value)) {
                strength.innerHTML = 'Password must be more than 8 characters';
            } else if (strongRegex.test(pwd.value)) {
                strength.innerHTML = '<span style="color:green">Strong!</span>';
            } else if (mediumRegex.test(pwd.value)) {
                strength.innerHTML = '<span style="color:orange">Medium!</span>';
            } else {
                strength.innerHTML = '<span style="color:red">Weak!</span>';
            }

            
        }

     </script>
    <div class="form-group">

            <h2 class="h2">ChangePassword</h2>
        </div>
                    
               
              
                    <div class="form-group">
                    <asp:Label runat="server" CssClass="control-label">Current Password</asp:Label>
                        <asp:TextBox runat="server" ID="tb_oldpassword" CssClass="form-control" TextMode="password"></asp:TextBox><asp:Label runat="server" ID="lbl_email_validate"></asp:Label>
                        </div>
         <div class="form-group">
              <asp:Label runat="server" CssClass=" control-label" >New Password</asp:Label>
               <asp:TextBox runat="server" ID="tb_newpassword" onKeyPress="javascript:validatepassword();" CssClass="form-control" TextMode="Password"></asp:TextBox><asp:Label runat="server" ID="lbl_pwd_validate"></asp:Label>

             </div>
             <div class="form-group">
                  <asp:Button runat="server" CssClass="btn-primary btn" ID="btn_changepassword" Width="100%" Text="Change Password" OnClick="btn_changepassword_Click" />
                 </div>
        
             <asp:Label runat="server" ID="lbl_errormsg" CssClass=" control-label label-danger"></asp:Label>
</asp:Content>
