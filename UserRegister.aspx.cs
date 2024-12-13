using System;
using System.Configuration;
using System.Data.OleDb;

public partial class UserRegister : System.Web.UI.Page
{
    protected string m_Msg;
    protected string m_MsgEmail;
    protected string m_MsgCaptcha;

    protected string m_eMail;
    protected string m_Password;
    protected string m_FirstName;
    protected string m_MiddleName;
    protected string m_LastName;
    protected string m_Company;
    protected string m_Address;
    protected string m_Address2;
    protected string m_City;
    protected string m_State;
    protected string m_Country;
    protected string m_ZIP;
    protected string m_Phone;
    protected string m_Phone2;
    protected string m_Phone3;
    protected string m_VehicleInfo;
    protected string m_captcha;
    protected void Page_Load(object sender, EventArgs e)
    {
        m_Msg = "";
        m_MsgEmail = "";
        m_MsgCaptcha = "";

        m_eMail = Utilities.SForm("EMAIL");
        m_Password = Utilities.SForm("PASSWORD");
        m_FirstName = Utilities.SForm("FIRSTNAME");
        m_MiddleName = Utilities.SForm("MIDDLENAME");
        m_LastName = Utilities.SForm("LASTNAME");
        m_Company = Utilities.SForm("COMPANY");
        m_Address = Utilities.SForm("ADDRESS");
        m_Address2 = Utilities.SForm("ADDRESS2");
        m_City = Utilities.SForm("CITY");

        m_Country = Utilities.SForm("COUNTRY");
        if (m_Country == "US")
            m_State = Utilities.SForm("STATE_US").ToUpper();
        else if (m_Country == "CA")
            m_State = Utilities.SForm("STATE_CA").ToUpper();
        else
            m_State = Utilities.SForm("STATE_INTERNATIONAL").ToUpper();

        m_ZIP = Utilities.SForm("ZIP");
        m_Phone = Utilities.SForm("PHONE");
        m_Phone2 = Utilities.SForm("PHONE2");
        m_Phone3 = Utilities.SForm("PHONE3");

        m_VehicleInfo = Utilities.SForm("VEHICLEINFO").ToUpper();
        m_captcha = Utilities.SForm("captcha").ToString();
        
        //hiddenSessionValue.Value= Session["CaptchaVerify"].ToString();
        //string captchaChallenge = Utilities.SForm("recaptcha_challenge_field");
        //string captchaResponse = Utilities.SForm("recaptcha_response_field");

        //if (m_eMail.Length == 0 || m_FirstName.Length == 0 || m_LastName.Length == 0 || captchaChallenge.Length == 0 || captchaResponse.Length == 0)
        //    return;

        if (m_eMail.Length == 0 || m_FirstName.Length == 0 || m_LastName.Length == 0)
            return;

        // Validate the captcha
        //RecaptchaValidator captcha = new RecaptchaValidator();
        //captcha.Challenge = captchaChallenge;
        //captcha.Response = captchaResponse;
        //captcha.RemoteIP = Request.ServerVariables["REMOTE_ADDR"];
        //captcha.PrivateKey = "6LdcjAgAAAAAAJ6qeZQoJSMHNKYTJBEgjXUQ8RfD";
        //if (captcha.Validate().IsValid)
        //{
        bool IsCaptchaValid = false;
        if (Session["CaptchaVerify"] != null)
        {
            string sessionValue = Session["CaptchaVerify"].ToString();
            if (m_captcha == sessionValue)
                IsCaptchaValid = true;
        }
        if (IsCaptchaValid)
        {
            if (IsEmailUsed())
            {
                // e-mail already exists!!!
                m_MsgEmail = "<tr><td colspan=\"2\" style=\"color: #e00; font-size: 12px; font-weight: bold;\">The e-mail entered is already in use by someone, please enter a different one.</td></tr>";
            }
            else
            {
                int UserID = SaveDB_User();
                if (UserID > 0)
                {
                    Session["UserID"] = UserID;
                    Session["UserEMail"] = m_eMail;

                    Response.Redirect(Utilities.GetLastLoginURL());
                }
            }

        }
        else 
        {
            m_Msg = "<tr><td colspan=\"2\" style=\"color: #e00; font-size: 12px; font-weight: bold;\">The text entered for the captcha image below did not match. Please try again.</td></tr>";
        }
        //}
        //else
        //{
            // wrong captcha!!!
        //    m_MsgCaptcha = "<br /><span style=\"color: #e00; font-size: 12px; font-weight: bold;\">The text entered for the image below did not match. Please try again.</span><br />";
        //}
        if (m_MsgEmail.Length > 0 || m_MsgCaptcha.Length > 0)
        {
            m_Msg = "<tr><td colspan=\"2\" style=\"color: #e00; font-size: 12px; font-weight: bold;\">There is invalid information in the form.<br />Please correct it and try again.</td></tr>";
        }
    }

    private bool IsEmailUsed()
    {
        bool IsUsed = false;
        try
        {
            OleDbConnection conn = new OleDbConnection(ConfigurationManager.AppSettings["connString"]);
            conn.Open();
            OleDbCommand cmd = new OleDbCommand();
            cmd.Connection = conn;

            string qry = @"SELECT COUNT(*) FROM Customers WHERE eMail=?";
            cmd.CommandText = qry;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@eMail", m_eMail);

            int f = 0;
            try
            {
                f = Convert.ToInt32(cmd.ExecuteScalar());
            }
            catch (Exception e)
            {
                Auditing.InsertError(e);
            }

            if (f>0)
            {                
                IsUsed = true;
            }

            cmd.Dispose();
            conn.Close();
            conn.Dispose();
        }
        catch { }

        return IsUsed;
    }

    private int SaveDB_User()
    {
        int UserID = 0;
        
        
        try
        {
            string PasswordHash = SimpleHash.ComputeHash512(m_Password, null);
            string IPAddress = Request.ServerVariables["REMOTE_ADDR"];

            OleDbConnection conn = new OleDbConnection(ConfigurationManager.AppSettings["connString"]);
            conn.Open();
            OleDbCommand cmd = new OleDbCommand();
            cmd.Connection = conn;

            cmd.CommandText = @"INSERT INTO Customers (eMail, PasswordHash, PasswordAutoGenerated, Locked, LoginRetries, DateCreated, DateLastLogin, IPAddress, " +
                                                       "IsWholesale, PercentageDiscount, MinimumPurchase, Company, " +
                                                       "FirstName, MiddleName, LastName, Address, Address2, City, State, Country, ZIP, Phone, Phone2, Phone3, " +
                                                       "VehicleInfo" +
                                            ") VALUES ( ?, ?, 0, 0, 0, ?, ?, ?, " +
                                                       "0, 0, 0.0, ?, " + 
                                                       "?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, " +
                                                       "?); SELECT SCOPE_IDENTITY();";

            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@eMail", m_eMail);
            cmd.Parameters.AddWithValue("@PasswordHash", PasswordHash);
            cmd.Parameters.AddWithValue("@DateCreated", DateTime.Now.GetDateTimeFormats()[44]);
            cmd.Parameters.AddWithValue("@DateLastLogin", DateTime.Now.GetDateTimeFormats()[44]);
            cmd.Parameters.AddWithValue("@IPAddress", IPAddress);
            cmd.Parameters.AddWithValue("@Company", m_Company);
            cmd.Parameters.AddWithValue("@FirstName", m_FirstName);
            cmd.Parameters.AddWithValue("@MiddleName", m_MiddleName);
            cmd.Parameters.AddWithValue("@LastName", m_LastName);
            cmd.Parameters.AddWithValue("@Address", m_Address);
            cmd.Parameters.AddWithValue("@Address2", m_Address2);
            cmd.Parameters.AddWithValue("@City", m_City);
            cmd.Parameters.AddWithValue("@State", m_State);
            cmd.Parameters.AddWithValue("@Country", m_Country);
            cmd.Parameters.AddWithValue("@ZIP", m_ZIP);
            cmd.Parameters.AddWithValue("@Phone", m_Phone);
            cmd.Parameters.AddWithValue("@Phone2", m_Phone2);
            cmd.Parameters.AddWithValue("@Phone3", m_Phone3);
            cmd.Parameters.AddWithValue("@VehicleInfo", m_VehicleInfo);           
            
            try
            {
                UserID = Convert.ToInt32(cmd.ExecuteScalar());
            }
            catch (Exception e)
            {
                Auditing.InsertError(e);
            }

            cmd.Dispose();
            conn.Close();
            conn.Dispose();
        }
        catch {}

        return UserID;
    }
}
