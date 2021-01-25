using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace App_Sec_Project
{
    public partial class SiteMaster : MasterPage
    {
        string MYDBConnectionString =
        System.Configuration.ConfigurationManager.ConnectionStrings["AS_DB"].ConnectionString;
        byte[] Key;
        byte[] IV;
        byte[] fname = null;
        byte[] lname = null;
        string userID = null;
        public string f_name { get; set; }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserID"] != null)
            {
                userID = (string)Session["userID"];

                btn_logout.Visible = true;
                displayUserProfile(userID);
            }
        }

        protected string decryptData(byte[] cipherText)
        {
            string plainText = null;
            try
            {
                RijndaelManaged cipher = new RijndaelManaged();
                cipher.IV = IV;
                cipher.Key = Key;
                // Create a decrytor to perform the stream transform.
                ICryptoTransform decryptTransform = cipher.CreateDecryptor();

                using (MemoryStream msDecrypt = new MemoryStream(cipherText))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptTransform, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            plainText = srDecrypt.ReadToEnd();
                        }
                    }


                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally { }
            return plainText;
        }
        protected void displayUserProfile(string userid)
        {
            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "SELECT * FROM Users WHERE Email=@Email";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Email", userid);
            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader["Email"] != DBNull.Value)
                        {
                            string tempusrid = "";
                            tempusrid = reader["Email"].ToString();
                        }
                        if (reader["FirstName"] != DBNull.Value)
                        {
                            fname = Convert.FromBase64String(reader["FirstName"].ToString());
                        }
                        if (reader["LastName"] != DBNull.Value)
                        {
                            lname = Convert.FromBase64String(reader["LastName"].ToString());
                        }
                        if (reader["IV"] != DBNull.Value)
                        {
                            IV = Convert.FromBase64String(reader["IV"].ToString());
                        }
                        if (reader["KEY"] != DBNull.Value)
                        {
                            Key = Convert.FromBase64String(reader["KEY"].ToString());
                        }
                    }
                     f_name = decryptData(fname);
                    string l_name = decryptData(lname);

                    

                }
            }//try
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally
            {
                connection.Close();
            }
        }

        protected void btn_logout_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
            Session.RemoveAll();
            Response.Redirect("~/Login.aspx", false);
            if (Request.Cookies["ASP.NET_SessionId"] != null)
            {
                Response.Cookies["ASP.NET_SessionId"].Value = string.Empty;
                Response.Cookies["ASP.NET_SessionId"].Expires = DateTime.Now.AddMonths(-20);

            }
            if (Request.Cookies["AuthToken"] != null)
            {
                Response.Cookies["AuthToken"].Value = string.Empty;
                Response.Cookies["AuthToken"].Expires = DateTime.Now.AddMonths(-20);
            }
        }
    }
}