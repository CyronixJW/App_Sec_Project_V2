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


    public partial class Register : System.Web.UI.Page
    {
        string MYDBConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["AS_DB"].ConnectionString;
        static string finalHash;
        static string salt;
        byte[] Key;
        byte[] IV;
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btn_submit_Click(object sender, EventArgs e)
        {


            if (ValidateUserFields() == true)
                if (ValidateEmail() == true)
                {
                    if (ValidatePassword() == true)
                    {



                        {


                            string email = tb_email.Text.ToString();


                            string pwd = tb_password.Text.ToString().Trim();
                            //Generate random "salt"
                            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
                            byte[] saltByte = new byte[8];
                            //Fills array of bytes with a cryptographically strong sequence of random values.
                            rng.GetBytes(saltByte);
                            salt = Convert.ToBase64String(saltByte);
                            SHA512Managed hashing = new SHA512Managed();
                            string pwdWithSalt = pwd + salt;
                            byte[] plainHash = hashing.ComputeHash(Encoding.UTF8.GetBytes(pwd));
                            byte[] hashWithSalt = hashing.ComputeHash(Encoding.UTF8.GetBytes(pwdWithSalt));
                            finalHash = Convert.ToBase64String(hashWithSalt);
                            RijndaelManaged cipher = new RijndaelManaged();
                            cipher.GenerateKey();
                            Key = cipher.Key;
                            IV = cipher.IV;
                            createAccount();


                        }
                    }
                }

        }
        public bool ValidateEmail()
        {
            bool result = true;
            string _regexPattern = @"^(([\w-]+\.)+[\w-]+|([a-zA-Z]{1}|[\w-]{2,}))@"
                        + @"((([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\."
                        + @"([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])){1}|"
                        + @"([a-zA-Z]+[\w-]+\.)+[a-zA-Z]{2,4})$";
            if (string.IsNullOrEmpty(tb_email.Text) == false && System.Text.RegularExpressions.Regex.IsMatch(tb_email.Text, _regexPattern))
            {
                result = true;
                lbl_email_validate.Text = "";
            }
            else
            {
                result = false;
                lbl_email_validate.Text = "Not a valid email !";
            }
            return result;
        }
        public bool ValidatePassword()
        {
            bool result = true;
            if (tb_confirm_password.Text != tb_password.Text)
            {
                lbl_pwd_validate.Text = "Password and Confirm Password must be the same!";
                result = false;
            }
            else
            {
                lbl_pwd_validate.Text = "";
                result = true;
            }
            return result;
        }

        public bool ValidateUserFields()
        {
            bool result = true;


            if (string.IsNullOrEmpty(tb_fname.Text) == true)
            {
                lbl_fname_validate.Text = "Field cannot be empty!";
                result = false;
            }
            else
            {
                result = true;
                lbl_fname_validate.Text = "";
            }
            if (string.IsNullOrEmpty(tb_lname.Text) == true)
            {
                lbl_lname_validate.Text = "Field cannot be empty!";
                result = false;
            }
            else
            {
                lbl_fname_validate.Text = "";
                result = false;
            }
            if (string.IsNullOrEmpty(tb_cc_name.Text) == true)
            {
                lbl_cc_name_validate.Text = "Field cannot be empty!";
                result = false;
            }
            else
            {
                lbl_cc_name_validate.Text = "";
                result = true;
            }
            int cc_num_result = 0;
            if (tb_cc_number.Text == "" && tb_cc_number.Text.Length == 0)
            {
                lbl_cc_number_validate.Text = "Field cannot be empty !";
                result = false;

            }
            else if (Int32.TryParse(tb_cc_number.Text, out cc_num_result) == false)
            {
                lbl_cc_number_validate.Text = "Only numbers are allowed!";
                result = false;
            }

            else
            {
                lbl_cc_number_validate.Text = "";
                result = true;
            }



            string dss = DateTime.Today.ToString("MM/yy", CultureInfo.InvariantCulture);
            DateTime dsnow = DateTime.ParseExact(dss, "MM/yy", CultureInfo.InvariantCulture, DateTimeStyles.None);

            DateTime expiry_result;
            if (tb_dateofbirth.Text == "")
            {
                result = false;
                lbl_expirydate_validate.Text = "Field cannot be empty !";
            }
            else if (DateTime.TryParseExact(tb_cc_expiry.Text, "MM/yy", CultureInfo.InvariantCulture, DateTimeStyles.None, out expiry_result) == false)
            {
                lbl_expirydate_validate.Text = "Invalid Expiry Date";
                result = false;
            }
            else if (expiry_result < dsnow)
            {
                lbl_expirydate_validate.Text = "Card has expired !";
                result = false;
            }
            else
            {
                lbl_expirydate_validate.Text = "";
                result = true;
            }
            return result;

        }
        public void createAccount()
        {
            try
            {
                Guid guid = Guid.NewGuid();
                string userid = guid.ToString();

                using (SqlConnection con = new SqlConnection(MYDBConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("BEGIN IF NOT EXISTS(SELECT* FROM Users WHERE Email = @Email)" +
                        "BEGIN INSERT INTO Users VALUES(@Id,@Email,@FirstName,@LastName, @PasswordHash, @PasswordSalt, @DateTimeRegistered, @MobileVerified, @EmailVerified,@CreditCardName,@CreditCardNumber,@CreditCardExpiryDate,@CreditCardCVV,@DateofBirth, @IV, @KEY)" +
                        "END END;")
                        )
                    {
                        using (SqlDataAdapter sda = new SqlDataAdapter())
                        {
                            cmd.CommandType = CommandType.Text;
                            cmd.Parameters.AddWithValue("@Id", userid.Trim());
                            cmd.Parameters.AddWithValue("@Email", tb_email.Text.Trim());
                            cmd.Parameters.AddWithValue("@FirstName", Convert.ToBase64String(encryptData(tb_fname.Text)));
                            cmd.Parameters.AddWithValue("@LastName", Convert.ToBase64String(encryptData(tb_lname.Text)));
                            cmd.Parameters.AddWithValue("@PasswordHash", finalHash);
                            cmd.Parameters.AddWithValue("@PasswordSalt", salt);
                            cmd.Parameters.AddWithValue("@DateTimeRegistered", DateTime.Now);
                            cmd.Parameters.AddWithValue("@MobileVerified", DBNull.Value);
                            cmd.Parameters.AddWithValue("@EmailVerified", DBNull.Value);
                            cmd.Parameters.AddWithValue("@CreditCardName", Convert.ToBase64String(encryptData(tb_cc_name.Text)));
                            cmd.Parameters.AddWithValue("@CreditCardNumber", Convert.ToBase64String(encryptData(tb_cc_number.Text)));
                            cmd.Parameters.AddWithValue("@CreditCardExpiryDate", Convert.ToBase64String(encryptData(tb_cc_expiry.Text.ToString())));
                            cmd.Parameters.AddWithValue("@CreditCardCVV", Convert.ToBase64String(encryptData(tb_cc_cvv.Text.ToString())));
                            cmd.Parameters.AddWithValue("@DateofBirth", tb_dateofbirth.Text.ToString());
                            cmd.Parameters.AddWithValue("@IV", Convert.ToBase64String(IV));
                            cmd.Parameters.AddWithValue("@KEY", Convert.ToBase64String(Key));


                            cmd.Connection = con;
                            con.Open();

                            int checkquery = cmd.ExecuteNonQuery();

                            if (checkquery == -1)
                            {
                                StatusMessage.Text = "Email exists !";
                            }
                            else if (checkquery == 1)
                            {
                                StatusMessage.Text = "Register Successful";
                                Response.Redirect("~/Login.aspx");
                            }
                            con.Close();
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        protected byte[] encryptData(string data)
        {
            byte[] cipherText = null;
            try
            {
                RijndaelManaged cipher = new RijndaelManaged();
                cipher.IV = IV;
                cipher.Key = Key;
                ICryptoTransform encryptTransform = cipher.CreateEncryptor();
                //ICryptoTransform decryptTransform = cipher.CreateDecryptor();
                byte[] plainText = Encoding.UTF8.GetBytes(data);
                cipherText = encryptTransform.TransformFinalBlock(plainText, 0,
               plainText.Length);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally { }
            return cipherText;
        }


    }

}