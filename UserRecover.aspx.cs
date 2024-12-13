using System;
using System.Activities.Expressions;
using System.Configuration;
using System.Data.OleDb;

public partial class UserRecovery : System.Web.UI.Page
{
    protected string m_Msg;
    protected string m_MsgWrongInfo;
    protected string m_MsgCaptcha;
    protected string m_eMail;
    protected string m_FirstName;
    protected string m_LastName;
    protected string m_captcha;
    protected void Page_Load(object sender, EventArgs e)
    {
        m_Msg = "Did you forget your Password?<br />";
        m_MsgWrongInfo = "";
        m_MsgCaptcha = "";

        m_eMail = Utilities.SForm("EMAIL");
        m_FirstName = Utilities.SForm("FIRSTNAME");
        m_LastName = Utilities.SForm("LASTNAME");
        m_captcha = Utilities.SForm("captcha").ToString();
        // string captchaChallenge = Utilities.SForm("recaptcha_challenge_field");
        // string captchaResponse = Utilities.SForm("recaptcha_response_field");

        // if (m_eMail.Length == 0 || m_FirstName.Length == 0 || m_LastName.Length == 0 || captchaChallenge.Length == 0 || captchaResponse.Length == 0)
        //     return;
        if (m_eMail.Length == 0 || m_FirstName.Length == 0 || m_LastName.Length == 0)
            return;

        // Validate the captcha
        //RecaptchaValidator captcha = new RecaptchaValidator();
        //captcha.Challenge = captchaChallenge;
        //captcha.Response = captchaResponse;
        //captcha.RemoteIP = Request.ServerVariables["REMOTE_ADDR"];
        //captcha.PrivateKey = "6LeX0VUUAAAAAF­Zdo97GWnN1SgsAN22M_pvwO4S";
        //if (captcha.Validate().IsValid) already did captcha code but it was not working for them? I guess.
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


            OleDbConnection conn = new OleDbConnection(ConfigurationManager.AppSettings["connString"]);
            conn.Open();

            string qry;
            OleDbCommand cmd = new OleDbCommand();
            cmd.Connection = conn;
            qry = @"SELECT UserID, FirstName, LastName FROM Customers " +
                   "WHERE {fn UCase(eMail)}=?";
            cmd.CommandText = qry;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@email", m_eMail.ToUpper());
            OleDbDataReader dr = null;
            try
            {
                dr = cmd.ExecuteReader();
            }
            catch (Exception ex)
            {
                Auditing.InsertError(ex);
            }

            if (dr.Read())
            {
                if (dr["FirstName"].ToString().ToUpper() == m_FirstName.ToUpper() && dr["LastName"].ToString().ToUpper() == m_LastName.ToUpper())
                {
                    // Generate new random password
                    string NewPassword = RandomPassword.Generate();

                    // update db
                    int UserID = Convert.ToInt32(dr["UserID"]);
                    UpdateDB_CustomersNewPassword(UserID, NewPassword);

                    // email new password to customer - this is the only time the password is visible in the system
                    string Body = "Dear " + dr["FirstName"].ToString() + "," + Environment.NewLine;
                    Body += "This is a generated e-mail that contains your account information for MAX BMW." + Environment.NewLine;
                    Body += "If you think you should not have received this e-mail, please contact " + ConfigurationManager.AppSettings["EmailNameAccounts"] + " at " + ConfigurationManager.AppSettings["EmailAccounts"] + "." + Environment.NewLine + Environment.NewLine;
                    Body += "Your username is: " + m_eMail + Environment.NewLine;
                    Body += "Your new password is: " + NewPassword + Environment.NewLine;
                    Body += Environment.NewLine;
                    Body += "Once you log in, you can enter to your profile and edit your password." + Environment.NewLine;
                    Body += Environment.NewLine;
                    Body += Environment.NewLine;
                    Body += Environment.NewLine;
                    Body += Environment.NewLine;
                    Body += "Remote host IP Address: " + Request.ServerVariables["REMOTE_ADDR"];

                    Body = Body.Replace(Environment.NewLine, "<br />");

                    EmailSender.SendEmail(m_eMail, ConfigurationManager.AppSettings["EmailAccounts"], "MAX BMW - Users", "", ConfigurationManager.AppSettings["EmailAdmin"], "Your user account at MAX BMW", Body);

                    // redirect to UserLogin.aspx
                    Response.Redirect("UserLogin.aspx");
                }
            }
            // if we got here it means that some data above is incorrect
            m_MsgWrongInfo = "<tr><td style=\"color: #e00; font-size: 12px;\">It seems that the information provided does not match out records.</td></tr>";
            //}
            //else
            //{
            // wrong captcha!!!
            //    m_MsgCaptcha = "<br /><span style=\"color: #e00; font-size: 12px;\">The text entered for the image below did not match. Please try again.</span><br />";
            //}
        }
        else
        {
            m_MsgCaptcha = "<br /><span style=\"color: #e00; font-size: 12px;\">The text entered for the captcha image below did not match. Please try again.</span><br />";
        }
    }


    private void UpdateDB_CustomersNewPassword(int UserID, string NewPassword)
    {
        string PasswordHash = "";
        PasswordHash = SimpleHash.ComputeHash512(NewPassword, null);
        
        OleDbConnection conn = new OleDbConnection(ConfigurationManager.AppSettings["connString"]);
        conn.Open();

        string qry;
        OleDbCommand cmd = new OleDbCommand();
        cmd.Connection = conn;
        qry = @"UPDATE Customers SET PasswordHash=?, Locked=0, PasswordAutoGenerated=1, LoginRetries=3 WHERE UserID=?";
        cmd.CommandText = qry;
        cmd.Parameters.Clear();
        cmd.Parameters.AddWithValue("@PasswordHash", PasswordHash);  
        cmd.Parameters.AddWithValue("@UserID", UserID);

        try
        {
            cmd.ExecuteNonQuery();
        }
        catch (Exception ex)
        {
            Auditing.InsertError(ex);
        }
        
        conn.Close();
        conn.Dispose();
    }
}

