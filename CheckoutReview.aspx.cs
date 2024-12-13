using System;
using System.Configuration;
using System.Data.OleDb;


# region Coupon Class
public class Coupon
{
    private string _Code;
    private string _Description;    
    private double _DiscountMoney;
    private double _AvailableBalance;
    private int _DiscountPercentage;
    private string _eMails;
    private bool _SpecialsAccepted;
    private bool _FreeShipping;
    private bool _OncePerUser; // for each email  
    private bool _AllowWholesaleUser;
    private double _MinimumSubTotal;
    private int _MaxTimesAllowed;
    private DateTime _ValidFrom;
    private DateTime _ValidTo;  

    public Coupon()
    {
        _Code = "";
        _Description = "";
        _DiscountMoney = 0.0;
        _DiscountPercentage = 0;
        _eMails = "";
        _SpecialsAccepted = true;
        _FreeShipping = false;
        _OncePerUser = true;
        _AllowWholesaleUser = false;
        _MinimumSubTotal = 0.0;        
        _MaxTimesAllowed = 0;
        _ValidFrom = new DateTime(2013, 1, 1, 0, 0, 0);
        _ValidTo = new DateTime(2030, 12, 31, 0, 0, 0);
    }

    public string Code { get { return _Code; } set { _Code = value; } }
    public string Description { get { return _Description; } set { _Description = value; } }
    public double DiscountMoney { get { return _DiscountMoney; } set { _DiscountMoney = value; } }
    public double AvailableBalance { get { return _AvailableBalance; } set { _AvailableBalance = value; } }
    public int DiscountPercentage { get { return _DiscountPercentage; } set { _DiscountPercentage = value; } }
    public string eMails { get { return _eMails; } set { _eMails = value; } }
    public bool SpecialsAccepted { get { return _SpecialsAccepted; } set { _SpecialsAccepted = value; } }
    public bool FreeShipping { get { return _FreeShipping; } set { _FreeShipping = value; } }
    public bool OncePerUser { get { return _OncePerUser; } set { _OncePerUser = value; } }    
    public bool AllowWholesaleUser { get { return _AllowWholesaleUser; } set { _AllowWholesaleUser = value; } }
    public double MinimumSubTotal { get { return _MinimumSubTotal; } set { _MinimumSubTotal = value; } }
    public int MaxTimesAllowed { get { return _MaxTimesAllowed; } set { _MaxTimesAllowed = value; } }
    public DateTime ValidFrom { get { return _ValidFrom; } set { _ValidFrom = value; } }
    public DateTime ValidTo { get { return _ValidTo; } set { _ValidTo = value; } }
}

public class CouponsCollection : System.Collections.CollectionBase
{
    public CouponsCollection() {}

    public Coupon this[int index]
    {
        get { return (Coupon)this.List[index]; }
        set { this.List[index] = value; }
    }

    public int IndexOf(Coupon item)
    {
        return base.List.IndexOf(item);
    }

    public int Add(Coupon item)
    {
        // don't allow more than one coupon with the same code        
        foreach (Coupon c in this)
        {
            if (c.Code == item.Code)
            {                
                return 0; // found, so exit
            }
        }
        
        // none found, so add it
        return this.List.Add(item);
    }

    public void Remove(Coupon item)
    {
        this.InnerList.Remove(item);
    }

    public double GetTotalAvailableBalance()
    {
        double total = 0.0;
        foreach (Coupon c in this)
        {
            // Discount Percentage has priority over Discount Money, so the Money/AvailableBalance only needs to be added if the Percentage one is 0
            if (c.DiscountPercentage == 0)
                total += c.AvailableBalance;
        }
        return total;
    }

    public int GetTotalPercentage()
    {
        int total = 0;
        foreach (Coupon c in this)
        {
            if (c.DiscountPercentage > 0)
                total += c.DiscountPercentage;
        }
        return total;
    }
}
#endregion

public partial class CheckoutReview : System.Web.UI.Page
{
    protected CartHelper cartHelper;
    protected CouponsCollection Coupons;

    protected string m_Coupons;
    protected string m_eMail;
    protected bool m_IsWholesale;
    protected string m_Company;
    protected int m_WholesaleDiscount;
    protected double m_MinimumPurchase;
    protected string m_TaxID;
    protected double m_Taxes;
    protected string m_CheckFirstName;
    protected string m_CheckLastName;
    protected string m_CheckCompany;
    protected string m_CheckAddress;
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
    protected double m_TotalWeight; 
    protected string m_VehicleInfo;    
    protected double m_SubTotal;
    protected bool m_FreeShipping;
    protected string m_CardCountry;
    protected string m_ShippingOptions;

    private Coupon GetCouponFromDB(string CouponCode)
    {
        Coupon c = null;

        OleDbConnection conn = new OleDbConnection(ConfigurationManager.AppSettings["connString"]);
        conn.Open();
        OleDbCommand cmd = new OleDbCommand();
        cmd.Connection = conn;

        cmd.CommandText = @"SELECT * FROM Coupons WHERE CouponCode=?";
        cmd.Parameters.Clear();
        cmd.Parameters.AddWithValue("@CouponCode", CouponCode);
        OleDbDataReader dr = cmd.ExecuteReader();

        if (dr.Read())
        {
            c = new Coupon();
            c.Code = CouponCode.ToUpper();
            c.Description = (DBNull.Value != dr["Description"] ? dr["Description"].ToString().Trim() : "");
            c.DiscountMoney = (DBNull.Value != dr["DiscountMoney"] ? Convert.ToDouble(dr["DiscountMoney"]) : 0.0);
            c.AvailableBalance = (DBNull.Value != dr["AvailableBalance"] ? Convert.ToDouble(dr["AvailableBalance"]) : 0.0); // TODO!!! calculate available balance based on cart orders usage GetCouponMoneyUsed??
            c.DiscountPercentage = (DBNull.Value != dr["DiscountPercentage"] ? Convert.ToInt32(dr["DiscountPercentage"]) : 0);
            c.SpecialsAccepted = (DBNull.Value != dr["SpecialsAccepted"] ? Convert.ToBoolean(dr["SpecialsAccepted"]) : false);
            c.FreeShipping = (DBNull.Value != dr["FreeShipping"] ? Convert.ToBoolean(dr["FreeShipping"]) : false);
            c.OncePerUser = (DBNull.Value != dr["OncePerUser"] ? Convert.ToBoolean(dr["OncePerUser"]) : false);
            c.eMails = (DBNull.Value != dr["eMails"] ? dr["eMails"].ToString().Trim() : "");
            c.AllowWholesaleUser = (DBNull.Value != dr["AllowWholesaleUser"] ? Convert.ToBoolean(dr["AllowWholesaleUser"]) : false);
            c.MinimumSubTotal = (DBNull.Value != dr["MinimumSubTotal"] ? Convert.ToDouble(dr["MinimumSubTotal"]) : 0.0);
            c.MaxTimesAllowed = (DBNull.Value != dr["MaxTimesAllowed"] ? Convert.ToInt32(dr["MaxTimesAllowed"]) : 0);
            c.ValidFrom = (DBNull.Value != dr["ValidFrom"] ? Convert.ToDateTime(dr["ValidFrom"]) : new DateTime(2012, 1, 1, 0, 0, 0));
            c.ValidTo = (DBNull.Value != dr["ValidTo"] ? Convert.ToDateTime(dr["ValidTo"]) : new DateTime(2030, 12, 31, 0, 0, 0));
        }

        cmd.Dispose();
        conn.Close();
        conn.Dispose();

        return c;
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["ReviewPageCounter"] == null)
            Session["ReviewPageCounter"] = 1;
        else
            Session["ReviewPageCounter"] = Convert.ToInt32(Session["ReviewPageCounter"].ToString()) + 1;

        // if there are no items in the cart, then leave this page!
        CartItemCollection myCartItems = (CartItemCollection)Session["CartItems"];
        if (myCartItems == null || myCartItems.Count == 0)
        {
            Response.Redirect(Utilities.URL_FICHE(true) + "/CheckoutProcessed.aspx");
        }
                
        cartHelper = new CartHelper();
        
        Utilities.SetLastLoginURL();

        // retrieve form values
        m_Coupons = Utilities.SForm("COUPONS").ToUpper();
        m_eMail = Utilities.SForm("EMAIL");
        m_IsWholesale = false;
        m_WholesaleDiscount = 0;
        m_MinimumPurchase = 0.0;
        m_Company = Utilities.SForm("COMPANY").ToUpper();
        m_CheckFirstName = "";
        m_CheckLastName = "";
        m_CheckCompany = "";
        m_CheckAddress = "";
        m_FirstName = Utilities.SForm("FIRSTNAME").ToUpper();
        m_MiddleName = Utilities.SForm("MIDDLENAME").ToUpper();
        m_LastName = Utilities.SForm("LASTNAME").ToUpper();
        m_Address = Utilities.SForm("ADDRESS").ToUpper();
        m_Address2 = Utilities.SForm("ADDRESS2").ToUpper();
        m_City = Utilities.SForm("CITY").ToUpper();
        m_Country = Utilities.SForm("COUNTRY").ToUpper();
        if (m_Country == "US")
            m_State = Utilities.SForm("STATE_US").ToUpper();
        else if (m_Country == "CA")
            m_State = Utilities.SForm("STATE_CA").ToUpper();
        else
            m_State = Utilities.SForm("STATE_INTERNATIONAL").ToUpper();

        m_CardCountry = Utilities.SForm("CARDCOUNTRY").ToUpper();
        if (m_CardCountry == "")
            m_CardCountry = m_Country;

        m_ZIP = Utilities.SForm("ZIP").ToUpper();
        m_Phone = Utilities.SForm("PHONE").ToUpper();
        m_Phone2 = Utilities.SForm("PHONE2").ToUpper();
        m_Phone3 = Utilities.SForm("PHONE3").ToUpper();
        m_SubTotal = Utilities.DForm("SubTotal");

        m_Taxes = 0.0;
        if (m_State == "CT")
        {
            m_Taxes = 6.35;
        }

        m_VehicleInfo = "";

        m_TotalWeight = Utilities.DForm("TotalWeight");
        if (m_TotalWeight < 1.0)
            m_TotalWeight = 1.0; // force to 1lb minimum

        if (Session["UserID"] != null)
        {
            GetDB_User();
        }      


        //***************************************************************
        // Check the coupons and update the Coupons variable
        //***************************************************************        
        Coupons = new CouponsCollection();
        Coupons.Clear();
        string[] sCoupons = m_Coupons.Split(',', ';');
        foreach (string sCoupon in sCoupons)
        {
            Coupon coupon = GetCouponFromDB(sCoupon);

            if (coupon != null && coupon.Code != "")
            {
                // validate coupon
                bool CouponValid = true;

                if (CouponValid && coupon.eMails != "" && coupon.eMails.ToLower().IndexOf(m_eMail.ToLower()) == -1)
                {
                    // this coupon requires the email from the customer to be in the list
                    // email of the customer not found in the coupon, so this coupon is not valid for him/her
                    CouponValid = false;
                    coupon.Description = "Invalid Coupon/email";
                    coupon.FreeShipping = false;
                    coupon.DiscountMoney = 0.0;
                    coupon.AvailableBalance = 0.0;
                    coupon.DiscountPercentage = 0;
                }

                if (CouponValid && DateTime.Compare(DateTime.Now, coupon.ValidTo) > 0)
                {
                    CouponValid = false;
                    coupon.Description = "Coupon Expired " + coupon.ValidTo.ToShortDateString();                        
                    coupon.FreeShipping = false;
                    coupon.DiscountMoney = 0.0;
                    coupon.AvailableBalance = 0.0;
                    coupon.DiscountPercentage = 0;          
                }

                if (CouponValid && DateTime.Compare(DateTime.Now, coupon.ValidFrom) < 0)
                {
                    CouponValid = false;
                    coupon.Description = "Not valid, starts " + coupon.ValidFrom.ToShortDateString();
                    coupon.FreeShipping = false;
                    coupon.DiscountMoney = 0.0;
                    coupon.AvailableBalance = 0.0;
                    coupon.DiscountPercentage = 0;
                }

                if (CouponValid && coupon.OncePerUser && CheckIfCouponWasUsed(sCoupon, m_eMail))
                {
                    CouponValid = false;
                    coupon.Description = "Coupon used already";
                    coupon.FreeShipping = false;
                    coupon.DiscountMoney = 0.0;
                    coupon.AvailableBalance = 0.0;
                    coupon.DiscountPercentage = 0;
                }

                if (CouponValid && coupon.MaxTimesAllowed > 0 && GetCouponTotalUsedTimes(sCoupon) >= coupon.MaxTimesAllowed)
                {
                    CouponValid = false;
                    coupon.Description = "Coupon used already - " + coupon.MaxTimesAllowed.ToString();
                    coupon.FreeShipping = false;
                    coupon.DiscountMoney = 0.0;
                    coupon.AvailableBalance = 0.0;
                    coupon.DiscountPercentage = 0;
                }

                if (CouponValid && !coupon.AllowWholesaleUser && m_IsWholesale)
                {
                    CouponValid = false;
                    coupon.Description = "Wholesale user not allowed";                        
                    coupon.FreeShipping = false;
                    coupon.DiscountMoney = 0.0;
                    coupon.AvailableBalance = 0.0;
                    coupon.DiscountPercentage = 0;
                }

                if (CouponValid && coupon.MinimumSubTotal > 0.0 && m_SubTotal < coupon.MinimumSubTotal)
                {
                    CouponValid = false;
                    coupon.Description = "Min. SubTotal needs to be " + coupon.MinimumSubTotal.ToString("C");                        
                    coupon.FreeShipping = false;
                    coupon.DiscountMoney = 0.0;
                    coupon.AvailableBalance = 0.0;
                    coupon.DiscountPercentage = 0;
                }

                // for a gift card, the AvailableBalance needs to be >0 and the MaxTimesAllowed=1
                if (CouponValid && coupon.DiscountMoney > 0.0 && coupon.MaxTimesAllowed==1 && coupon.AvailableBalance == 0.0)
                {
                    CouponValid = false;
                    coupon.Description = "Available balance is $0.00";
                    coupon.FreeShipping = false;
                    coupon.DiscountMoney = 0.0;
                    coupon.DiscountPercentage = 0;
                }
                if (CouponValid && coupon.DiscountMoney > 0.0 && coupon.MaxTimesAllowed == 1 && coupon.AvailableBalance > 0.0)
                {
                    coupon.Description = "Balance was " + coupon.AvailableBalance.ToString("C");
                    coupon.DiscountMoney = coupon.AvailableBalance;
                }
                

                if (CouponValid && coupon.FreeShipping)
                {
                    coupon.Description += "(FREE SHIPPING)";
                }

                Coupons.Add(coupon);                
            }
        }

        // check if Specials are accepted with the coupons
        // also check if freeshipping is accepted
        bool SpecialsAccepted = true;
        bool FreeShippingCoupons = false;
        foreach (Coupon coupon in Coupons)
        {
            if (!coupon.SpecialsAccepted)
                SpecialsAccepted = false;

            if (coupon.FreeShipping)
                FreeShippingCoupons = true;
        }

        // free shipping could be because of 2 reasons: the table NotesParts.AdditionalShipping==-1   OR   the FreeShippingCoupons is true 
        m_FreeShipping = (myCartItems.IsFreeShipping() || FreeShippingCoupons);

        string ShippingError = "";
        m_ShippingOptions = CalculateShippingOptions(m_State, m_Country, m_Address, m_City, m_ZIP, m_TotalWeight, m_SubTotal, ref ShippingError);

        if (m_ShippingOptions == "" || ShippingError != "")
        {
            //string eventTime = DateTime.Now.Year.ToString("D4") + DateTime.Now.Month.ToString("D2") + DateTime.Now.Day.ToString("D2") + "_" + DateTime.Now.Hour.ToString("D2") + "." + DateTime.Now.Minute.ToString("D2") + "." + DateTime.Now.Second.ToString("D2");
            //string Body = "USERID=" + m_eMail + " : " + HttpContext.Current.Request.Form.ToString() + "  ShippingError=" + ShippingError;
            //EmailSender.SendEmail("berczely@hotmail.com", "berczely@hotmail.com", "MAX BMW Alert", "", "", "MAX Alert m_ShippingOptions or ShippingError" + eventTime, Body);
        }
        if (ShippingError != "")
        {
            Response.Redirect("Checkout.aspx?ShippingError=" + Server.UrlEncode(ShippingError));
        }



        //******************************************************************
        // Update cart with the items from the form on CartCheckout.aspx.
        //
        // If a valid coupon was used, and if it cannot be applied towards
        // a Special item, then we need to change the price of the special
        // item to be the retail price. Also we need to update the SubTotal
        // to reflect this.
        //******************************************************************
        m_SubTotal = 0.0;
        if (myCartItems != null)
        {
            CartItem cartItem;
            
            for (int i = myCartItems.Count; i > 0; i--)
            {
                cartItem = myCartItems[i - 1];
                if (Request.Form["Q_" + cartItem.ID] != null)
                {
                    if (Request.Form["Q_" + cartItem.ID] == "0")
                        myCartItems.Remove(cartItem);
                    else
                    {
                        cartItem.Quantity = Convert.ToInt16(Utilities.SForm("Q_" + cartItem.ID));

                        // Recalculate all prices based if the Specials are accepted or not
                        if (cartItem.IsSpecial)
                        {
                            if (SpecialsAccepted)
                            {
                                cartItem.Price = cartItem.PriceSpecials;
                                cartItem.SpecialsText2 = "";
                            }
                            else
                            {
                                cartItem.Price = cartItem.PriceRetail;
                                cartItem.SpecialsText2 = "One of the coupons used cannot be used in conjunction with a discounted item. Regular retail price will be used for this product. Coupon discount will be applied on your Sub Total.";
                            }
                        }
                        else
                        {
                            cartItem.Price = cartItem.PriceRetail;
                            cartItem.SpecialsText = "";
                            cartItem.SpecialsText2 = "";
                        }
                        // recalculate subtotal as well
                        m_SubTotal += cartItem.Price;
                    }
                }
            }            
        }

        // Save session cart
        Session["CartItems"] = myCartItems;
    }

    protected bool CheckIfValidCouponEmail(string CouponCode, string EMail)
    {
        bool Valid = false;
        
        OleDbConnection conn = new OleDbConnection(ConfigurationManager.AppSettings["connString"]);
        conn.Open();
        OleDbCommand cmd = new OleDbCommand();
        cmd.Connection = conn;

        cmd.CommandText = @"SELECT CouponCode, emails, DiscountPercentage, DiscountMoney FROM Coupons WHERE CouponCode=? AND emails=?";
        cmd.Parameters.Clear();
        cmd.Parameters.AddWithValue("@CouponCode", CouponCode);
        cmd.Parameters.AddWithValue("@emails", EMail);
        OleDbDataReader dr = cmd.ExecuteReader();

        if (dr.Read())
        {
            if (Convert.ToInt32(dr["DiscountPercentage"]) > 0 || Convert.ToDouble(dr["DiscountMoney"]) > 0.0)
            {
                Valid = true;
            }
        }

        cmd.Dispose();
        conn.Close();
        conn.Dispose();
        return Valid;
    }

    protected bool CheckIfCouponWasUsed(string CouponCode, string EMail)
    {
        bool Used = false;

        OleDbConnection conn = new OleDbConnection(ConfigurationManager.AppSettings["connString"]);
        conn.Open();
        OleDbCommand cmd = new OleDbCommand();
        cmd.Connection = conn;

        cmd.CommandText = @"SELECT Coupons FROM CartOrders WHERE Coupons=? AND email=?";
        cmd.Parameters.Clear();
        cmd.Parameters.AddWithValue("@Coupons", CouponCode);
        cmd.Parameters.AddWithValue("@emails", EMail);
        OleDbDataReader dr = cmd.ExecuteReader();

        if (dr.Read())
        {
            if (DBNull.Value != dr["Coupons"] && dr["Coupons"].ToString()==CouponCode)
            {
                Used = true;
            }
        }

        cmd.Dispose();
        conn.Close();
        conn.Dispose();
        return Used;
    }
        
    private int GetCouponTotalUsedTimes(string CouponCode)
    {
        int Count = 0;

        OleDbConnection conn = new OleDbConnection(ConfigurationManager.AppSettings["connString"]);
        conn.Open();
        OleDbCommand cmd = new OleDbCommand();
        cmd.Connection = conn;

        cmd.CommandText = @"SELECT COUNT(*) FROM CartOrders WHERE Coupons=?";
        cmd.Parameters.Clear();
        cmd.Parameters.AddWithValue("@Coupons", CouponCode);
        Count = Convert.ToInt16(cmd.ExecuteScalar());
        
        cmd.Dispose();
        conn.Close();
        conn.Dispose();

        return Count;
    }

    private int GetCouponMoneyUsed(string CouponCode, string EMail)
    {
        int Count = 0;

        OleDbConnection conn = new OleDbConnection(ConfigurationManager.AppSettings["connString"]);
        conn.Open();
        OleDbCommand cmd = new OleDbCommand();
        cmd.Connection = conn;

        cmd.CommandText = @"SELECT * FROM CartOrders WHERE Coupons=? AND email=?";
        cmd.Parameters.Clear();
        cmd.Parameters.AddWithValue("@Coupons", CouponCode);
        cmd.Parameters.AddWithValue("@email", EMail);
        OleDbDataReader dr = cmd.ExecuteReader();

        while (dr.Read())
        {          
                //TODO!!!
                // get total value used on this coupon
        }

        cmd.Dispose();
        conn.Close();
        conn.Dispose();

        return Count;
    }
    
    protected string ShowCouponsDiscounts()
    {
        string HTML = "";

        int i = 0;
        bool bg_alternate = false;
        foreach (Coupon coupon in Coupons)
        {
            HTML += "<tr style=\"vertical-align: middle; font-family: Arial; font-size: 11px; background-color: " + (bg_alternate ? "#f9f9f9" : "#ddd") + ";\">" +
                            "<td align=\"left\" style=\"font-weight: bold;\">&nbsp;" + coupon.Code + "</td>" +
                            "<td align=\"left\" style=\"font-family: Arial Narrow; border-left: solid 1px #666;\">&nbsp;" + coupon.Description + (coupon.DiscountPercentage == 0 && coupon.DiscountMoney> 0.0 ? " (" + string.Format("{0:C}", coupon.DiscountMoney) + ")" : "") + "&nbsp;</td>" +
                            "<td align=\"right\" style=\"font-family: Arial Narrow; border-left: solid 1px #666;\">&nbsp;" +
                                (coupon.DiscountPercentage > 0 ? coupon.DiscountPercentage + "%" : "-" + string.Format("{0:C}", coupon.AvailableBalance)) +
                            "</td>" +
                        "</tr>";
            i++;
            bg_alternate = !bg_alternate;
        }
         
        // TODO: coupons gift cards
        //double CouponsAvailableBalance = Coupons.GetTotalAvailableBalance();
        //if (CouponsAvailableBalance > 0.0)
        //{
        //    HTML += "<tr style=\"font-family: Arial; font-size: 11px; font-weight: bold; background-color: #666; color: #fff;\">" +
        //                "<td colspan=\"3\" align=\"right\" style=\"border: solid 1px #666;\">" +
        //                    "Total Coupons Discount:&nbsp;" +
        //                    "<input size=\"5\" type=\"text\" readonly=\"readonly\" id=\"CouponsDiscount\" value=\"-" + string.Format("{0:C}", CouponsAvailableBalance) + "\" style=\"border: 0px; background-color: transparent; text-align: right; font-family: Arial; font-size: 8pt; font-weight: bold; color: #fff\" />" +
        //                "</td>" +
        //            "</tr>";
        //}

        int CouponsPercentage = Coupons.GetTotalPercentage();
        if (CouponsPercentage > 0)
        {
            HTML += "<tr style=\"font-family: Arial; font-size: 11px; font-weight: bold; background-color: #666; color: #fff;\">" +
                        "<td colspan=\"3\" align=\"right\" style=\"border: solid 1px #666;\">" +
                            "Total Coupons" +
                            "<input type=\"text\" readonly=\"readonly\" id=\"CouponsPercentage\" value=\"" + CouponsPercentage + "%\" style=\"width: 35px; border: 0px; background-color: transparent; text-align: center; font-family: Arial; font-size: 8pt; font-weight: bold; color: #fff\" />" +
                            " Discount:&nbsp;" +
                            "<input size=\"5\" type=\"text\" readonly=\"readonly\" id=\"CouponsPercentageMoney\" value=\"0\" style=\"border: 0px; background-color: transparent; text-align: right; font-family: Arial; font-size: 8pt; font-weight: bold; color: #fff\" />" +
                        "</td>" +
                    "</tr>";
        }

        if (HTML.Length > 0)
        {            
            HTML = "<table cellpadding=\"0\" cellspacing=\"0\" style=\"border: solid 1px #666\">" +
                        "<tr style=\"font-family: Arial; font-size: 11px; font-weight: bold; background-color: #666; color: #fff;\">" +
                            "<td align=\"center\" style=\"border: solid 1px #666;\">&nbsp;Your Coupons&nbsp;</td>" +
                            "<td align=\"center\" style=\"border: solid 1px #666;\">&nbsp;Description&nbsp;</td>" +
                            "<td align=\"center\" style=\"border: solid 1px #666;\">&nbsp;Discounts&nbsp;</td>" +
                        "</tr>" +
                HTML + "</table>";
        }
        return HTML;
    }

    protected string ShowWholesaleDiscounts()
    {
        string txt = "";
        if (m_IsWholesale)
        {
            if (m_LastName.ToUpper() == m_CheckLastName.ToUpper() && m_FirstName.ToUpper() == m_CheckFirstName.ToUpper() && m_Address.ToUpper() == m_CheckAddress.ToUpper() && m_Company.ToUpper() == m_CheckCompany.ToUpper())
            {
                txt = "<span style=\"color: #d50; font-size: 11px; font-weight: bold;\">Wholesale discounts:<br />" +
                        "Your discount: " + m_WholesaleDiscount.ToString() + "%<br />" +
                        "Minimum purchase: " + string.Format("{0:C}", m_MinimumPurchase) +
                      "</span>";
                txt += "<input type=\"hidden\" id=\"WholesaleDiscount\" name=\"WholesaleDiscount\" value=" + m_WholesaleDiscount + " />";
                txt += "<input type=\"hidden\" id=\"MinimumPurchase\" name=\"MinimumPurchase\" value=" + m_MinimumPurchase + " />";
            }
            else
            {
                txt = "<span style=\"color: #e00; font-size: 11px; font-weight: bold;\">Your wholesale shipping information does not match our records.<br />No wholesale discounts were applied.</span>";
            }
        }
        return txt;
    }

    protected string CalculateShippingOptions(string toState, string toCountry, string toAddress, string toCity, string toZIP, double PkgWeight, double SubTotal, ref string ShippingError)
    {
        ShippingError = "";

        string ShippingOptions = ""; // this is the radiobuttons html code

        if (toCountry.Length == 0)
        {
            return "";
        }

        if (m_FreeShipping)
        {
            return "";
        }

        string ShippingNamesCustom = "";
        string ShippingNamesUSPS = "";
        string ShippingNamesUPS = "";
        string ShippingNamesFedEx = "";

        string RatesCustom = "";
        string RatesUSPS = "";
        string RatesUPS = "";
        string RatesFedEx = "";

        string NotesCustom = "";
        string NotesUSPS = "";
        string NotesUPS = "";
        string NotesFedEx = "";

        if (toCountry == "CA")
        {
            //Jordan Station, Ontario
            //Canada, L0R 1S0
            ShippingNamesUSPS = "Priority Mail International|Express Mail International";
            NotesUSPS = "|";
            RatesUSPS = "30.00|54.00";
        } 
        else if (toCountry != "US")
        {
            // International other than Canada
            ShippingNamesUSPS = "Express Mail International";
            NotesUSPS = "";
            RatesUSPS = "54.00";
        }
        else if (toCountry == "US")
        {
            if (toState == "AK" || toState == "HI" || toState == "PR" || toState == "AS" || toState == "FM" || toState == "MH" || toState == "MP" || toState == "PW" || toState == "VI")
            {
                ShippingNamesUSPS = "Priority Mail|Express Mail";
                NotesUSPS = "|";
                RatesUSPS = "15.00|25.00";
            }
            if (toState == "GU")
            {
                ShippingNamesUSPS = "Express Mail";
                NotesUSPS = "";
                RatesUSPS = "45.00";
            }
            else if (toState == "AE" || toState == "AP" || toState == "AA")
            {
                ShippingNamesUSPS = "Priority Mail Military|Express Mail Military";
                NotesUSPS = "|";
                RatesUSPS = "15.00|30.00";
            }
            else
            {
                // Default for normal US States
                ShippingNamesCustom = "Standard Mail";
                NotesCustom = "";
                RatesCustom = "12.00";

                ShippingNamesUSPS = "Express Mail";
                NotesUSPS = "";
                RatesUSPS = "25.00";

                ShippingNamesUPS = "2nd Day Air|Next Day Air";
                NotesUPS = "|";
                RatesUPS = "30.00|50.00";
            }
        }
        
        bool FirstRadioButton = true;

        if (toCountry == "US"
            && (toState != "AK"
                && toState != "HI"
                && toState != "PR"
                && toState != "AS"
                && toState != "FM"
                && toState != "MH"
                && toState != "MP"
                && toState != "PW"
                && toState != "VI"
                && toState != "AE"
                && toState != "AP"
                && toState != "AA")
            && SubTotal > 99.0
            && !m_IsWholesale)
        {
            // This is the section that would add free shipping only in the US
            // and only on orders above $99. Also it excludes all expensive territories.
            
            //ShippingOptions += "<input type=\"radio\" value=\"Free Shipping-$0.00\" checked=\"checked\" id=\"SHIPPINGMETHOD_FREE_0\" name=\"SHIPPINGMETHOD\" style=\"border: 0px; background-color: #fff;\" onclick=\"UpdateFields();\" /><label style=\"color: #e33;\">Free Shipping Continental US (3 to 5 days)" +
            //                    "&nbsp;&nbsp;-&nbsp;&nbsp;$0.00</label><br />";
            //FirstRadioButton = false;
        }
       
        if (ShippingNamesCustom.Length > 0)
        {
            ShippingOptions += GetShippingHTML("Custom", ShippingNamesCustom, NotesCustom, RatesCustom, ref FirstRadioButton);
        }
        if (ShippingNamesUPS.Length > 0)
        {
            ShippingOptions += GetShippingHTML("UPS", ShippingNamesUPS, NotesUPS, RatesUPS, ref FirstRadioButton);                       
        }
        if (ShippingNamesFedEx.Length > 0)
        {
            ShippingOptions += GetShippingHTML("FedEx", ShippingNamesFedEx, NotesFedEx, RatesFedEx, ref FirstRadioButton);
        }
        if (ShippingNamesUSPS.Length > 0)
        {
            ShippingOptions += GetShippingHTML("USPS", ShippingNamesUSPS, NotesUSPS, RatesUSPS, ref FirstRadioButton);
        }
        
        
        // information for in-store pickup in the NH Store, only allowed for US and Canadian orders
        if (toCountry == "US" || toCountry == "CA")
        {
            ShippingOptions += "<br />";
            ShippingOptions += "<input type=\"radio\" value=\"In Store Pickup-NH-$0.00\" id=\"SHIPPINGMETHOD_PICKUP_0\" name=\"SHIPPINGMETHOD\" style=\"border: 0px; background-color: #fff;\" onclick=\"UpdateFields();\" />In Store Pick-Up - North Hampton NH - " +
                                "<label style=\"font-size: 11px; cursor: pointer; text-decoration: underline;\" title=\"View Map in new window\" onclick=\"var w = window.open('https://goo.gl/maps/hVYxra1F6Nm6CAUT7','Maps', '');\">View Map</label>" +
                                "&nbsp;&nbsp;-&nbsp;&nbsp;$0.00<br />";
            ShippingOptions += "<input type=\"radio\" value=\"In Store Pickup-NY-$0.00\" id=\"SHIPPINGMETHOD_PICKUP_1\" name=\"SHIPPINGMETHOD\" style=\"border: 0px; background-color: #fff;\" onclick=\"UpdateFields();\" />In Store Pick-Up - Brunswick NY - " +
                                "<label style=\"font-size: 11px; cursor: pointer; text-decoration: underline;\" title=\"View Map in new window\" onclick=\"var w = window.open('https://goo.gl/maps/pmciscXJVwU6tvyD7','Maps', '');\">View Map</label>" +
                                "&nbsp;&nbsp;-&nbsp;&nbsp;$0.00<br />";
            ShippingOptions += "<input type=\"radio\" value=\"In Store Pickup-CT-NM-$0.00\" id=\"SHIPPINGMETHOD_PICKUP_2\" name=\"SHIPPINGMETHOD\" style=\"border: 0px; background-color: #fff;\" onclick=\"UpdateFields();\" />In Store Pick-Up - New Milford CT - " +
                                "<label style=\"font-size: 11px; cursor: pointer; text-decoration: underline;\" title=\"View Map in new window\" onclick=\"var w = window.open('https://goo.gl/maps/kwLbfYMwnfcz8uVN7','Maps', '');\">View Map</label>" +
                                "&nbsp;&nbsp;-&nbsp;&nbsp;$0.00<br />";
            ShippingOptions += "<input type=\"radio\" value=\"In Store Pickup-CT-SW-$0.00\" id=\"SHIPPINGMETHOD_PICKUP_3\" name=\"SHIPPINGMETHOD\" style=\"border: 0px; background-color: #fff;\" onclick=\"UpdateFields();\" />In Store Pick-Up - South Windsor CT - " +
                                "<label style=\"font-size: 11px; cursor: pointer; text-decoration: underline;\" title=\"View Map in new window\" onclick=\"var w = window.open('https://goo.gl/maps/rzy3uM4dkB3aY8MT6','Maps', '');\">View Map</label>" +
                                "&nbsp;&nbsp;-&nbsp;&nbsp;$0.00<br />";
        }

        return ShippingOptions;
    }

    protected string GetShippingHTML(string Carrier, string ShippingNames, string Notes, string DefaultRates, ref bool FirstRadioButton)
    {
        // Carrier: UPS, USPS, FedEx, Custom
        string txt = "";

        string[] arrShippingNames = ShippingNames.Split('|');
        string[] arrNotes = Notes.Split('|');
        string[] arrDefaultRates = DefaultRates.Split('|');

        string Rate;
        double dRate;

        for (int i = 0; i < arrShippingNames.Length; i++)
        {
            Rate = arrDefaultRates[i];
            dRate = Convert.ToDouble(Rate);

            string FullName = (Carrier=="Custom"?"":Carrier) + " " + arrShippingNames[i] + arrNotes[i] + "&nbsp;&nbsp;-&nbsp;&nbsp;" + string.Format("{0:C}", dRate);
            txt += "<input type=\"radio\" value=\"" + Carrier + " " + arrShippingNames[i] + "-" + string.Format("{0:C}", dRate) + "\" id=\"SHIPPINGMETHOD_" + Carrier + "_" + i + "\" name=\"SHIPPINGMETHOD\" " + (FirstRadioButton ? "checked=\"checked\"" : "") + "style=\"border: 0px; background-color: #fff;\" onclick=\"UpdateFields();\" />" + FullName + "<br />";
            FirstRadioButton = false;
        }

        return txt;
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

            cmd.CommandText = @"SELECT IsWholesale, PercentageDiscount, MinimumPurchase, TaxID, Taxes, Company, FirstName, LastName, Address, VehicleInfo " +
                                 "FROM Customers WHERE UserID=?";

            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@UserID", (Int32)Session["UserID"]);
            OleDbDataReader dr = cmd.ExecuteReader();

            if (dr.Read())
            {
                m_IsWholesale = (dr["IsWholesale"] != DBNull.Value ? Convert.ToBoolean(dr["IsWholesale"]) : false);
                m_WholesaleDiscount = (dr["PercentageDiscount"]!=DBNull.Value?Convert.ToInt32(dr["PercentageDiscount"]):0);
                m_MinimumPurchase = (dr["MinimumPurchase"]!=DBNull.Value?Convert.ToDouble(dr["MinimumPurchase"]):0.0);
                m_TaxID = (dr["TaxID"] != DBNull.Value ? dr["TaxID"].ToString() : "");
                m_Taxes = (dr["Taxes"] != DBNull.Value ? Convert.ToDouble(dr["Taxes"]) : m_Taxes);
                m_CheckCompany = (dr["Company"] != DBNull.Value ? dr["Company"].ToString().ToUpper() : "");
                m_CheckFirstName = dr["FirstName"].ToString().ToUpper();
                m_CheckLastName = dr["LastName"].ToString().ToUpper();
                m_CheckAddress = dr["Address"].ToString().ToUpper();
                m_VehicleInfo = dr["VehicleInfo"].ToString().ToUpper();
            }
            cmd.Dispose();
            conn.Close();
            conn.Dispose();
        }
        catch { }

        return true;
    }
}

