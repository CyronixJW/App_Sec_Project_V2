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
    public partial class Welcome : System.Web.UI.Page
    {
        string MYDBConnectionString =
        System.Configuration.ConfigurationManager.ConnectionStrings["AS_DB"].ConnectionString;
        byte[] Key;
        byte[] IV;

        byte[] fname = null;
        byte[] lname = null;
        string userID = null;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserID"] != null && Session["AuthToken"] != null && Request.Cookies["AuthToken"] != null)
            {
                if(!Session["AuthToken"].ToString().Equals(Request.Cookies["AuthToken"].Value))
                {
                    Response.Redirect("~/Login.aspx");
                }
                else
                {
                    userID = (string)Session["userID"];
                    displayUserProfile(userID);
                }

                
               
            }
            else
            {
                Response.Redirect("~/Login.aspx");
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
                            lbl_userid.Text = reader["Email"].ToString();
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
                    string f_name = decryptData(fname);
                    string l_name = decryptData(lname);

                    lbl_name.Text = f_name + " " + l_name;

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

       
    }
}