using System;
using System.Configuration;
using System.Data.OleDb;

public partial class Checkout : System.Web.UI.Page
{
    protected CartHelper cartHelper;

    protected string m_Coupons;
    protected string m_eMail;
    protected bool m_IsWholesale;
    protected string m_WholesaleDiscount;
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
    protected string m_ShippingError;

    protected void Page_Load(object sender, EventArgs e)
    {
        cartHelper = new CartHelper();        

        Utilities.SetLastLoginURL();        

        if (Session["UserID"] == null)
        {
            // retrieve form values
            m_Coupons = Utilities.SForm("COUPONS");
            m_eMail = Utilities.SForm("EMAIL");
            m_IsWholesale = false;
            m_WholesaleDiscount = "";
            m_MinimumPurchase = "";
            m_Company = "";
            m_FirstName = Utilities.SForm("FIRSTNAME").ToUpper();
            m_MiddleName = Utilities.SForm("MIDDLENAME").ToUpper();
            m_LastName = Utilities.SForm("LASTNAME").ToUpper();
            m_Address = Utilities.SForm("ADDRESS").ToUpper();
            m_Address2 = Utilities.SForm("ADDRESS2").ToUpper();
            m_City = Utilities.SForm("CITY").ToUpper();
            
            m_Country = Utilities.SForm("COUNTRY");
            if (m_Country=="US")
                m_State = Utilities.SForm("STATE_US");
            else if (m_Country=="CA")
                m_State = Utilities.SForm("STATE_CA");
            else
                m_State = Utilities.SForm("STATE_INTERNATIONAL");

            m_ZIP = Utilities.SForm("ZIP").ToUpper();
            m_Phone = Utilities.SForm("PHONE").ToUpper();
            m_Phone2 = Utilities.SForm("PHONE2").ToUpper();
            m_Phone3 = Utilities.SForm("PHONE3").ToUpper();          
        }
        else
        {
            GetDB_User();
        }
        m_ShippingError = Server.UrlDecode(Utilities.QueryString("ShippingError"));
    }

    protected bool GetDB_User()
    {
        if (Session["UserID"] == null)
            return false;

        try
        {
            OleDbConnection conn = new OleDbConnection(ConfigurationManager.AppSettings["connString"]);
            conn.Open();
            OleDbCommand cmd = new OleDbCommand();
            cmd.Connection = conn;

            string qry = @"SELECT eMail, IsWholesale, PercentageDiscount, MinimumPurchase, Company, FirstName, MiddleName, LastName, Address, Address2, City, State, Country, ZIP, Phone, Phone2, Phone3 " +
                                 "FROM Customers WHERE UserID=?";
            cmd.CommandText = qry;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@UserID", (Int32)Session["UserID"]);
            OleDbDataReader dr = cmd.ExecuteReader();

            if (dr.Read())
            {
                m_eMail = dr["eMail"].ToString();
                m_IsWholesale = (dr["IsWholesale"] != DBNull.Value ? Convert.ToBoolean(dr["IsWholesale"]) : false);
                m_WholesaleDiscount = (Convert.ToInt16(dr["PercentageDiscount"]) > 0 ? dr["PercentageDiscount"].ToString() + "%" : "");
                m_MinimumPurchase = (Convert.ToDouble(dr["MinimumPurchase"]) > 0.0 ? string.Format("{0:C}", dr["MinimumPurchase"]) : "");
                m_Company = (dr["Company"] != DBNull.Value ? dr["Company"].ToString().ToUpper() : "");
                m_FirstName = dr["FirstName"].ToString().ToUpper();
                m_MiddleName = dr["MiddleName"].ToString().ToUpper();
                m_LastName = dr["LastName"].ToString().ToUpper();
                m_Country = dr["Country"].ToString();

                if (m_Country=="US" || m_Country=="CA") //COVID-19
                {
                    m_Address = dr["Address"].ToString().ToUpper();
                    m_Address2 = dr["Address2"].ToString().ToUpper();
                    m_City = dr["City"].ToString().ToUpper();
                    m_State = dr["State"].ToString();
                    m_ZIP = dr["ZIP"].ToString().ToUpper();
                } else
                {
                    m_Country = "US";
                    m_Address = "";
                    m_Address2 = "";
                    m_City = "";
                    m_State = "";
                    m_ZIP = "";
                }

                m_Phone = dr["Phone"].ToString().ToUpper();
                m_Phone2 = dr["Phone2"].ToString().ToUpper();
                m_Phone3 = dr["Phone3"].ToString().ToUpper();
            }

            cmd.Dispose();
            conn.Close();
            conn.Dispose();
        }
        catch { }

        return true;
    }
}

