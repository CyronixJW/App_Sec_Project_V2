using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace App_Sec_Project
{
    public partial class Login : System.Web.UI.Page
    {
        string MYDBConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["AS_DB"].ConnectionString;

        byte[] Key;
        byte[] IV;

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btn_login_Click(object sender, EventArgs e)
        {

            string pwd = tb_pwd.Text.ToString().Trim();
            string userid = tb_email.Text.ToString().Trim();
            SHA512Managed hashing = new SHA512Managed();
            string dbHash = getDBHash(userid);
            string dbSalt = getDBSalt(userid);


            try
            {
                if (dbSalt != null && dbSalt.Length > 0 && dbHash != null && dbHash.Length > 0)
                {
                    string pwdWithSalt = pwd + dbSalt;
                    byte[] hashWithSalt = hashing.ComputeHash(Encoding.UTF8.GetBytes(pwdWithSalt));
                    string userHash = Convert.ToBase64String(hashWithSalt);
                    if (userHash.Equals(dbHash))
                    {
                        Session["UserID"] = userid;

                        string guid = Guid.NewGuid().ToString();
                        Session["AuthToken"] = guid;

                        Response.Cookies.Add(new HttpCookie("AuthToken", guid));
                        Session["LoginCount"] = 0;
                        Response.Redirect("~/Welcome.aspx", false);
                    }
                    else
                    {
                        Session["LoginCount"] = Convert.ToInt32(Session["LoginCount"]) + 1;

                        if (Convert.ToInt32(Session["LoginCount"]) > 3)
                        {
                            lbl_errormsg.Text = DeactivateLoginAccount();
                        }
                        else
                        {
                            lbl_errormsg.Text = "Userid or password is not valid. Please try again.";
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally { }




        }
        public string DeactivateLoginAccount()
        {
            DataSet ds = new DataSet();
            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "select * from Users Where Email= @Email;Update Users set StatusId=0 Where Email= @Email;";


            SqlCommand cmd = new SqlCommand(sql, connection);
            cmd.Parameters.AddWithValue("@Email", tb_email.Text);
            SqlDataAdapter sda = new SqlDataAdapter();
            sda.SelectCommand = cmd;
            connection.Open();


            sda.Fill(ds);
            if (ds.Tables[0].Rows.Count > 0)
            {
                return "Your Account is Locked. Please contact to admin.";
            }
            else
            {
                return "Please enter a valid login detail.";
            }

        
    
           
        }
        
            
            
           
               
                
          
        
        protected string getDBHash(string userid)
        {
            string h = null;
            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "select PasswordHash FROM Users WHERE Email=@Email";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Email", userid);
            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {

                    while (reader.Read())
                    {
                        if (reader["PasswordHash"] != null)
                        {
                            if (reader["PasswordHash"] != DBNull.Value)
                            {
                                h = reader["PasswordHash"].ToString();
                            }
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally { connection.Close(); }
            return h;
        }

        protected string getDBSalt(string userid)
        {
            string s = null;
            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "select PasswordSalt FROM Users WHERE Email=@Email";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Email", userid);
            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader["PasswordSalt"] != null)
                        {
                            if (reader["PasswordSalt"] != DBNull.Value)
                            {
                                s = reader["PasswordSalt"].ToString();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally { connection.Close(); }
            return s;
        }

    }
}