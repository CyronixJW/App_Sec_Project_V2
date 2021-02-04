using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
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

        public bool ValidateCaptcha()
        {
            bool result = true;
            string captchaResponse = Request.Form["g-recaptcha-response"];

            HttpWebRequest req = (HttpWebRequest)WebRequest.Create
                ("https://www.google.com/recaptcha/api/siteverify?secret=6LcPlEkaAAAAAJMYxLViADrqX3DlbgOouCvkmVmc &response=" + captchaResponse);
            try
            {
                using (WebResponse wResponse = req.GetResponse())
                {
                    using(StreamReader readStream = new StreamReader(wResponse.GetResponseStream()))
                    {
                        string jsonResponse = readStream.ReadToEnd();

                        JavaScriptSerializer js = new JavaScriptSerializer();

                        MyObject jsonObject = js.Deserialize<MyObject>(jsonResponse);

                        result = Convert.ToBoolean(jsonObject.success);
                    }
                }
                return result;
            }
            catch(WebException ex)
            {
                throw ex;
            }

        }
        protected void btn_login_Click(object sender, EventArgs e)
        {

            string pwd = tb_pwd.Text.ToString().Trim();
            string userid = tb_email.Text.ToString().Trim();
            SHA512Managed hashing = new SHA512Managed();
            string dbHash = getDBHash(userid);
            string dbSalt = getDBSalt(userid);
            if(ValidateCaptcha())
            {
                if (ValidatePassword() == true)
                {







                    try
                    {
                        if (dbSalt != null && dbSalt.Length > 0 && dbHash != null && dbHash.Length > 0)
                        {
                            string pwdWithSalt = pwd + dbSalt;
                            byte[] hashWithSalt = hashing.ComputeHash(Encoding.UTF8.GetBytes(pwdWithSalt));
                            string userHash = Convert.ToBase64String(hashWithSalt);
                            if (userHash.Equals(dbHash))
                            {

                                ReactivateLoginAccount();

                                Checkpasswordage();
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

            }
        }
        public void ReactivateLoginAccount()
        {
            DataSet ds = new DataSet();
           
            


            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "select * from Users Where Email= @Email;Update Users set StatusId=@StatusId,Lockoutdatetime = @Lockout Where Email= @Email;";
            string sqlselect = "Select * FROM Users WHERE Email = @Email";

            SqlCommand selectsql = new SqlCommand(sqlselect, connection);
            connection.Open();


            SqlCommand cmd = new SqlCommand(sql, connection);
            cmd.Parameters.AddWithValue("@Email", tb_email.Text);
            cmd.Parameters.AddWithValue("@Lockout", DBNull.Value);
            cmd.Parameters.AddWithValue("@StatusId", 1);
            selectsql.Parameters.AddWithValue("@Email", tb_email.Text);
            SqlDataReader myReader = selectsql.ExecuteReader();
            string lockouttime = "";
            while (myReader.Read())
            {
                lockouttime = myReader.GetValue(17).ToString();
            }
            myReader.Close();
            if (lockouttime != "")
            {
                string dst = DateTime.Now.ToString();
                DateTime lockout = DateTime.Parse(lockouttime);
                DateTime dsnow = DateTime.Parse(dst);
 
               
                TimeSpan timeSpan = dsnow - lockout;
                SqlDataAdapter sda = new SqlDataAdapter();
                sda.SelectCommand = cmd;



                if (timeSpan.TotalMinutes > 5)
                {
                    sda.Fill(ds);
                }

            }
        }
    

        public string DeactivateLoginAccount()
        {
            DataSet ds = new DataSet();

            DateTime dnow = DateTime.Now;


            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "select * from Users Where Email= @Email;Update Users set StatusId=0,Lockoutdatetime = @Lockout Where Email= @Email;";


            SqlCommand cmd = new SqlCommand(sql, connection);
            cmd.Parameters.AddWithValue("@Email", tb_email.Text);
            cmd.Parameters.AddWithValue("@Lockout", dnow);
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


        public void Checkpasswordage()
        {
            SqlConnection connection = new SqlConnection(MYDBConnectionString);

            string sqlselect = "Select * FROM Users WHERE Email = @Email";

            SqlCommand selectsql = new SqlCommand(sqlselect, connection);
            connection.Open();


           
            selectsql.Parameters.AddWithValue("@Email", tb_email.Text);
            SqlDataReader myReader = selectsql.ExecuteReader();
            string changedpasswordtime = "";
            while (myReader.Read())
            {
                changedpasswordtime = myReader.GetValue(19).ToString();
            }
            myReader.Close();
            if (changedpasswordtime != "")
            {
                string dst = DateTime.Now.ToString();
                DateTime lockout = DateTime.Parse(changedpasswordtime);
                DateTime dsnow = DateTime.Parse(dst);


                TimeSpan timeSpan = dsnow - lockout;

                if (timeSpan.TotalMinutes > 15)
                {
                    Session["UserID"] = tb_email.Text;


                    string guid = Guid.NewGuid().ToString();
                    Session["AuthToken"] = guid;

                    Response.Cookies.Add(new HttpCookie("AuthToken", guid));
                    Session["LoginCount"] = 0;
                    lbl_errormsg.Text = "Password has expired please change to a new password";
                    Response.Redirect("~/ChangePassword.aspx", false);

                }

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
        public bool ValidatePassword()
        {
            bool result = false;
            bool chkuserid = true;
            bool chkpwd = true;
            if (string.IsNullOrEmpty(tb_email.Text) == true)
            {
               lbl_email_validate.Text = " UserID cannot be null!";
                chkuserid = false;
            }
            else
            {
                lbl_email_validate.Text = "";
                chkuserid = true;
            }
            if (string.IsNullOrEmpty(tb_pwd.Text) == true)
            {
                lbl_pwd_validate.Text = " Password cannot be null!";
                chkpwd = false;
            }
            else
            {
                lbl_pwd_validate.Text = "";
                chkpwd = true;
            }
            if(chkpwd && chkuserid == true)
            {
                result = true;
            }
            else
            {
                result = false;
            }
            
            return result;
        }

    }
    public class MyObject
    {
        public string success { get; set; }
        public List<string> ErrorMessage { get; set; } 
    }

}