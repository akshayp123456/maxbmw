using System;
using System.Configuration;
using System.Data.OleDb;

public partial class UserLogin : System.Web.UI.Page
{
    protected string m_eMail;
    protected string m_Msg;
        
    protected void Page_Load(object sender, EventArgs e)
    {        
        m_eMail = Utilities.SForm("EMAIL");
        string Password = Utilities.SForm("PASSWORD");
        string CookieEMAIL = Cookie.GetCookieValue("CookieEMAIL");

        if (m_eMail.Length == 0 && CookieEMAIL.Length > 3)
        {
            m_eMail = CookieEMAIL;
        }       
        

        m_Msg = "Please enter your login information:";

        Session["UserID"] = null;
        Session["UserEMail"] = null;
        Session["UserFirstName"] = null;
        Session["UserLastName"] = null;

        if (m_eMail.Length > 0 && Password.Length > 0)
        {
            OleDbConnection conn = new OleDbConnection(ConfigurationManager.AppSettings["connString"]);
            conn.Open();

            string qry;
            OleDbCommand cmd = new OleDbCommand();
            cmd.Connection = conn;
            qry = @"SELECT UserID, eMail, FirstName, LastName, PasswordHash, LoginRetries, Locked, DateLastLogin FROM Customers " +
                   "WHERE {fn UCase(eMail)}=?";
            cmd.CommandText = qry;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@eMail", m_eMail.ToUpper());

            OleDbDataReader dr=null;

            try
            {
                dr = cmd.ExecuteReader();
            }
            catch(Exception ex)
            {
                Auditing.InsertError(ex);
            }

            if (dr.Read())
            {
                int UserID = Convert.ToInt32(dr["UserID"]);
                string PasswordHashDB = dr["PasswordHash"].ToString();
                int LoginRetries = (dr["LoginRetries"] != DBNull.Value ? (Int16)Convert.ToInt16(dr["LoginRetries"]) : (Int16)0);
                bool Locked = (dr["Locked"]!=DBNull.Value ? Convert.ToBoolean(dr["Locked"]) : false);
                DateTime DateLastLogin = (dr["DateLastLogin"]!=DBNull.Value ? Convert.ToDateTime(dr["DateLastLogin"]) : DateTime.Now);

                if (Locked)
                {
                    if (LoginRetries > 10)
                    {
                        Response.Redirect("UserRecover.aspx?locked=1");
                    }

                    TimeSpan ts = DateTime.Now.Subtract(DateLastLogin);
                    if (ts.Minutes < 5)  // allow to continue if there were more then 5 mins since last login attempt
                    {
                        Response.Redirect("UserRecover.aspx?locked=1");
                    }
                }

                if (SimpleHash.VerifyHash512(Password, PasswordHashDB))
                {
                    // password is correct                    
                    Session["UserID"] = UserID;
                    Session["UserEMail"] = dr["eMail"].ToString();
                    Session["UserFirstName"] = dr["FirstName"].ToString();
                    Session["UserLastName"] = dr["LastName"].ToString();

                    // unlock and set LoginRetries=0
                    UpdateDB_Customers(UserID, false, 0);


                    Cookie.InsertCookie("CookieEMAIL", dr["eMail"].ToString());                    


                    Response.Redirect(Utilities.GetLastLoginURL());
                }

                LoginRetries++;

                // the password is incorrect at this point
                if (LoginRetries >= 5)
                {
                    // lock user account in the db
                    UpdateDB_Customers(UserID, true, LoginRetries);
                    Response.Redirect("UserRecover.aspx?locked=1");
                }
                else
                {
                    UpdateDB_Customers(UserID, false, LoginRetries);
                }
            }

            dr.Close();
            dr.Dispose();
            conn.Close();
            conn.Dispose();

            m_Msg = "<span style=\"color: #d00;\">Invalid User (e-mail) and/or Password.<br />Please re-enter your information:</span>";
        }
    }


    private void UpdateDB_Customers(int UserID, bool Locked, int LoginRetries)
    {
        OleDbConnection conn = new OleDbConnection(ConfigurationManager.AppSettings["connString"]);
        conn.Open();

        string qry;
        OleDbCommand cmd = new OleDbCommand();
        cmd.Connection = conn;
        qry = @"UPDATE Customers SET Locked=?, LoginRetries=?, DateLastLogin=? WHERE UserID=?";
        cmd.CommandText = qry;
        cmd.Parameters.Clear();
        cmd.Parameters.AddWithValue("@Locked", Locked);
        cmd.Parameters.AddWithValue("@LoginRetries", LoginRetries);
        cmd.Parameters.AddWithValue("@DateLastLogin", DateTime.Now);
        cmd.Parameters.AddWithValue("@UserID", UserID);

        try
        {
            cmd.ExecuteNonQuery();
        }
        catch(Exception e)
        {
            Auditing.InsertError(e);
        }

        conn.Close();
        conn.Dispose();
    }
}
