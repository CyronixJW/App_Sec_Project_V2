<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Register.aspx.cs" Inherits="App_Sec_Project.Register" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

                 <script>
                     $(function () {
                         $( "#<%= tb_dateofbirth.ClientID %>").datepicker();
});
                 </script>
    <script type="text/javascript">
        function validatepassword() {
            var pwdstr = document.getElementById('<%= tb_password.ClientID%>').textContent;
            var strength = document.getElementById('<%= lbl_pwd_validate.ClientID%>');
            var strongRegex = new RegExp("^(?=.{14,})(?=.*[A-Z])(?=.*[a-z])(?=.*[0-9])(?=.*\\W).*$", "g");
            var mediumRegex = new RegExp("^(?=.{10,})(((?=.*[A-Z])(?=.*[a-z]))|((?=.*[A-Z])(?=.*[0-9]))|((?=.*[a-z])(?=.*[0-9]))).*$", "g");
            var enoughRegex = new RegExp("(?=.{8,}).*", "g");
            var pwd = document.getElementById('<%= tb_password.ClientID%>');
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
    <script type="text/javascript">
        function validateCvv() {
            var cvvstr = document.getElementById('<%=tb_cc_cvv.ClientID%>');
            var lbl_validate = document.getElementById('<%=lbl_cvv_validate.ClientID%>')
            if (cvvstr.value.length == 0) {
                lbl_validate.innerHTML = "CVV cannot be null";
            }
            else if (cvvstr.value.length != 3) {
                lbl_validate.innerHTML = "Invalid CVV number";
            }
            else {
                lbl_validate.innerHTML = "Success";
            }
        }
    </script>
                <div class="form-group"><h2>Account Registration</h2></div>
                    
               
                <div class="form-group">
                    <p>
                            <asp:Literal runat="server" ID="StatusMessage" />
                        </p>

                </div>
                    
               
                    <div class="form-group">
                    First Name
                    <asp:TextBox runat="server" CssClass="form-control" ID="tb_fname"></asp:TextBox><asp:Label runat="server" ID="lbl_fname_validate"></asp:Label>
               </div>
               
                    
                
                    <div class="form-group">

                        Last Name
               
                    <asp:TextBox runat="server" CssClass="form-control" ID="tb_lname"></asp:TextBox><asp:Label runat="server" ID="lbl_lname_validate"></asp:Label>

                    </div>
                    
                
                        <div class="form-group">
                             Credit Card Name
                
                    <asp:TextBox ID="tb_cc_name" CssClass="form-control" runat="server"></asp:TextBox><asp:Label runat="server" ID="lbl_cc_name_validate"></asp:Label>

                        </div>
                   
                
                    <div class="form-group">
                        
                         Credit Card Number
               <asp:TextBox ID="tb_cc_number" CssClass="form-control" runat="server"></asp:TextBox><asp:Label runat="server" ID="lbl_cc_number_validate"></asp:Label>
                    </div>
              <div class="form-group">
                   Credit Card Expiry Date
                  <asp:Textbox ID="tb_cc_expiry" CssClass="form-control" runat="server"></asp:Textbox><asp:Label runat="server" ID="lbl_expirydate_validate"></asp:Label>
              </div>
                <div class="form-group">

                    CVV
                    <asp:TextBox runat="server" CssClass="form-control" ID="tb_cc_cvv"  onKeyUp="validateCvv();"></asp:TextBox><asp:Label runat="server" ID="lbl_cvv_validate"></asp:Label>
                </div>

                <div class="form-group">

                    Email(UserID)
                
                    <asp:TextBox runat="server" CssClass="form-control" ID="tb_email"></asp:TextBox><asp:Label runat="server" ID="lbl_email_validate"></asp:Label>

                </div>
              <div class="form-group">
                   Password
                <asp:TextBox runat="server" CssClass="form-control" ID="tb_password" TextMode="Password" onKeyPress="javascript:validatepassword();"></asp:TextBox><asp:Label runat="server" ID="lbl_pwd_validate"></asp:Label>
              </div>

            <div class="form-group">
                       Confirm Password
               <asp:TextBox runat="server" CssClass="form-control" ID="tb_confirm_password" TextMode="Password"></asp:TextBox>

                </div>
             <div class="form-group">
                  Date of birth
                <asp:TextBox ID="tb_dateofbirth" CssClass="form-control" runat="server"></asp:TextBox><asp:Label runat="server" ID="lbl_dob"></asp:Label>

                 </div>
                    <div class="form-group">


                        <asp:Button runat="server" CssClass="btn-primary btn" ID="btn_submit" Width="100%" Text="Submit" OnClick="btn_submit_Click" />
                    </div>
           
               
           
         
            
               
            
              
                
            
</asp:Content>
