using System;
using System.Configuration;
using System.Data.OleDb;

public partial class UserEdit : System.Web.UI.Page
{
    protected string m_Msg;
    protected string m_eMail;
    protected string m_Password;
    protected bool m_IsWholesale;
    protected string m_PercentageDiscount;
    protected string m_MinimumPurchase;
    protected string m_Company;
    protected string m_FirstName;
    protected string m_MiddleName;
    protected string m_LastName;
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
    
    protected void Page_Load(object sender, EventArgs e)
    {
        m_Msg = "";

        if (Utilities.SForm("APPLY") == "1")
        {            
            m_Password = Utilities.SForm("PASSWORD");
            m_eMail = Utilities.SForm("EMAIL");
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
            m_VehicleInfo = Utilities.SForm("VEHICLEINFO");
            
            // save on db
            if (SaveDB_User())
            {
                GetDB_User();
                m_Msg = "<tr><td style=\"color: #e00;font-size: 12px;font-weight: bold;\">Your information has been saved!</td></tr>";
            }
            else
            {
                m_Msg = "<tr><td style=\"color: #e00;font-size: 12px;font-weight: bold;\">En error occured while saving your profile information.<br />Please check the information entered and try again.</td></tr>";
            }
        }
        else
        {
            // retrieve info from db
            GetDB_User();
        }
    }

    private bool SaveDB_User()
    {
        if (Session["UserID"]==null)
            return false;
               
        try
        {
            string PasswordHash = "";

            if (m_Password.Length >= 6)
            {
                PasswordHash = SimpleHash.ComputeHash512(m_Password, null);
            }
            
            OleDbConnection conn = new OleDbConnection(ConfigurationManager.AppSettings["connString"]);
            conn.Open();
            OleDbCommand cmd = new OleDbCommand();
            cmd.Connection = conn;

            string qry;
            cmd.Parameters.Clear();
            if (PasswordHash.Length > 0)
            {
                qry = @"UPDATE Customers SET PasswordHash=?, PasswordAutoGenerated=0, Locked=0, LoginRetries=0, " +
                                                   "FirstName=?, MiddleName=?, LastName=?, Company=?, Address=?, Address2=?, City=?, State=?, Country=?, ZIP=?, Phone=?, Phone2=?, Phone3=?, " +
                                                   "VehicleInfo=? " +
                                                   "WHERE UserID=?";
                cmd.Parameters.AddWithValue("@PasswordHash", PasswordHash);
            }
            else
            {
                qry = @"UPDATE Customers SET Locked=0, LoginRetries=0, " +
                                                   "FirstName=?, MiddleName=?, LastName=?, Company=?, Address=?, Address2=?, City=?, State=?, Country=?, ZIP=?, Phone=?, Phone2=?, Phone3=?, " +
                                                   "VehicleInfo=? " +
                                                   "WHERE UserID=?";
            }
            cmd.CommandText = qry;           
            cmd.Parameters.AddWithValue("@FirstName", m_FirstName);
            cmd.Parameters.AddWithValue("@MiddleName", m_MiddleName);
            cmd.Parameters.AddWithValue("@LastName", m_LastName);
            cmd.Parameters.AddWithValue("@Company", m_Company);
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
            cmd.Parameters.AddWithValue("@UserID", (Int32)Session["UserID"]);

            cmd.ExecuteNonQuery();

            cmd.Dispose();
            conn.Close();
            conn.Dispose();
        }
        catch {}

        return true;
    }


    private bool GetDB_User()
    {
        if (Session["UserID"] == null)
            return false;

        try
        { 
            OleDbConnection conn = new OleDbConnection(ConfigurationManager.AppSettings["connString"]);
            conn.Open();
            OleDbCommand cmd = new OleDbCommand();
            cmd.Connection = conn;

            string qry = @"SELECT eMail, IsWholesale, PercentageDiscount, MinimumPurchase, Company, FirstName, MiddleName, LastName, Address, Address2, City, State, Country, ZIP, Phone, Phone2, Phone3, " +
                                 "VehicleInfo " +
                                 "FROM Customers WHERE UserID=?";                
            cmd.CommandText = qry;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@UserID", (Int32)Session["UserID"]);
            OleDbDataReader dr = cmd.ExecuteReader();

            if (dr.Read())
            {
                m_eMail = dr["eMail"].ToString();
                m_IsWholesale = (dr["IsWholesale"] != DBNull.Value ? Convert.ToBoolean(dr["IsWholesale"]) : false);                
                m_PercentageDiscount = (Convert.ToInt16(dr["PercentageDiscount"])>0 ? "%" + dr["PercentageDiscount"].ToString() : "");
                m_MinimumPurchase = (Convert.ToDouble(dr["MinimumPurchase"]) > 0.0 ? string.Format("{0:C}", dr["MinimumPurchase"]) : "");
                m_Company = dr["Company"].ToString();
                m_FirstName = dr["FirstName"].ToString();
                m_MiddleName = dr["MiddleName"].ToString();
                m_LastName = dr["LastName"].ToString();
                m_Address = dr["Address"].ToString();
                m_Address2 = dr["Address2"].ToString();
                m_City = dr["City"].ToString();
                m_State = dr["State"].ToString();
                m_Country = dr["Country"].ToString();
                m_ZIP = dr["ZIP"].ToString();
                m_Phone = dr["Phone"].ToString();
                m_Phone2 = dr["Phone2"].ToString();
                m_Phone3 = dr["Phone3"].ToString();
                m_VehicleInfo = dr["VehicleInfo"].ToString();
            }

            cmd.Dispose();
            conn.Close();
            conn.Dispose();
        }
        catch { }

        return true;
    }   
}