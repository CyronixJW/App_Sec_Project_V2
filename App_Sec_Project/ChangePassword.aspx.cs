using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace App_Sec_Project
{
    public partial class ChangePassword : System.Web.UI.Page
    {
        string MYDBConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["AS_DB"].ConnectionString;
        static string finalHash;
        static string oldpasswordhash;
        static string passwordagehash;
        static string salt;
        string userid = null;

        byte[] Key;
        byte[] IV;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserID"] != null && Session["AuthToken"] != null && Request.Cookies["AuthToken"] != null)
            {
                if (!Session["AuthToken"].ToString().Equals(Request.Cookies["AuthToken"].Value))
                {
                    Response.Redirect("~/Login.aspx");
                }
                else
                {
                    userid = (string)Session["UserID"];
                    
                }



            }
            else
            {
                Response.Redirect("~/Login.aspx");
            }

        }

        protected void btn_changepassword_Click(object sender, EventArgs e)
        {

            string oldpassword = tb_oldpassword.Text.ToString().Trim();
            string newpassword = tb_newpassword.Text.ToString().Trim();
             userid = Session["UserID"].ToString();
            string dbHash = getDBHash(userid);
            string dbSalt = getDBSalt(userid);
            passwordagehash = getoldDBHash(userid);
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            byte[] saltByte = new byte[8];
            //Fills array of bytes with a cryptographically strong sequence of random values.
            rng.GetBytes(saltByte);
            salt = Convert.ToBase64String(saltByte);
            SHA512Managed hashing = new SHA512Managed();
            string pwdWithSalt = newpassword + dbSalt;
            byte[] hashWithSalt = hashing.ComputeHash(Encoding.UTF8.GetBytes(pwdWithSalt));
            finalHash = Convert.ToBase64String(hashWithSalt);
            RijndaelManaged cipher = new RijndaelManaged();
            cipher.GenerateKey();
            Key = cipher.Key;
            IV = cipher.IV;

            string oldpwdWithSalt = oldpassword + dbSalt;
            byte[] oldhashWithSalt = hashing.ComputeHash(Encoding.UTF8.GetBytes(oldpwdWithSalt));
            oldpasswordhash = Convert.ToBase64String(oldhashWithSalt);
            


            try
            {
                if (dbSalt != null && dbSalt.Length > 0 && dbHash != null && dbHash.Length > 0)
                {
                   
                    if (oldpasswordhash.Equals(dbHash))

                    {
                        lbl_errormsg.Text = CheckMinimumPasswordAge();
                        







                    }
                   
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally { }




        }
        
    

        public string ChangeNewPassword()
        {
            string userid = Session["UserID"].ToString();
            DataSet ds = new DataSet();

           


            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "select * from Users Where Email= @Email;Update Users set PasswordHash = @NewPasswordHash ,oldpasswordhash = @oldpasswordhash,changedpasswordtime = @changedpasswordtime  Where Email= @Email;";


            SqlCommand cmd = new SqlCommand(sql, connection);
            cmd.Parameters.AddWithValue("@Email", userid);
            cmd.Parameters.AddWithValue("@NewPasswordHash", finalHash);
            cmd.Parameters.AddWithValue("@oldpasswordhash", oldpasswordhash);
            cmd.Parameters.AddWithValue("@changedpasswordtime", DateTime.Now);
            SqlDataAdapter sda = new SqlDataAdapter();
            sda.SelectCommand = cmd;
            connection.Open();


            
            
            if(finalHash == oldpasswordhash)
            {
                return "Password cannot be the same";
            }
            else if(passwordagehash == finalHash)
            {
                return "Password cannot be reused as you have used it before";
            }
            sda.Fill(ds);
            if (ds.Tables[0].Rows.Count > 0)
            {
                
                return "Password Changed";
            }
            else
            {
                return "Current password is wrong";
            }




        }

        public string CheckMinimumPasswordAge()
        {
            SqlConnection connection = new SqlConnection(MYDBConnectionString);

            string sqlselect = "Select * FROM Users WHERE Email = @Email";

            SqlCommand selectsql = new SqlCommand(sqlselect, connection);
            connection.Open();



            selectsql.Parameters.AddWithValue("@Email", userid);
            SqlDataReader myReader = selectsql.ExecuteReader();
            string changedpasswordtime = "";
            while (myReader.Read())
            {
                changedpasswordtime = myReader.GetValue(19).ToString();
            }
            myReader.Close();
            TimeSpan timeSpan = new TimeSpan();
            if (changedpasswordtime != "")
            {
                string dst = DateTime.Now.ToString();
                DateTime lockout = DateTime.Parse(changedpasswordtime);
                DateTime dsnow = DateTime.Parse(dst);
                timeSpan = dsnow - lockout;
            }
            if (timeSpan.TotalMinutes < 5)
            {
                return "You have just changed a password please wait for 5 mins";

            }
            else
            {
                return ChangeNewPassword();
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
        protected string getoldDBHash(string userid)
        {
            string h = null;
            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "select oldpasswordhash FROM Users WHERE Email=@Email";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Email", userid);
            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {

                    while (reader.Read())
                    {
                        if (reader["oldpasswordhash"] != null)
                        {
                            if (reader["oldpasswordhash"] != DBNull.Value)
                            {
                                h = reader["oldpasswordhash"].ToString();
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