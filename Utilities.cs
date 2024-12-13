using System;
using System.Configuration;
using System.Web;
using System.Data.OleDb;
using System.Text;
using System.Security.Cryptography;
using System.Net;
using System.Net.Mail;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;


public class Auditing
{
    public static void InsertComment(string Description)
    {
        string StackTrace = "";
        string URL = HttpContext.Current.Request.Url.ToString();

        int UserID = 0;
        if ((HttpContext.Current.Session != null) && (HttpContext.Current.Session["UserID"] != null))
            UserID = Convert.ToInt32(HttpContext.Current.Session["UserID"]);

        string IPAddress = HttpContext.Current.Request.UserHostAddress;

        string SessionID = "";
        if (HttpContext.Current.Session != null)
            SessionID = HttpContext.Current.Session.SessionID.ToString();

        string QueryString = HttpContext.Current.Request.QueryString.ToString();
        string FormValues = HttpContext.Current.Request.Form.ToString();
        string ServerVariables = HttpContext.Current.Request.ServerVariables["ALL_HTTP"];

        OleDbConnection conn = new OleDbConnection(ConfigurationManager.AppSettings["connString"]);
        conn.Open();
        OleDbCommand cmd = new OleDbCommand();
        cmd.Connection = conn;
        string qry = @"INSERT INTO AuditsErrors (  ErrorDate, URL, Description, StackTrace, UserID, IPAddress, SessionID, QueryString, FormValues, ServerVariables)" +
                            " VALUES ( ?, ?, ?, ?, ?, ?, ?, ?, ?, ?); SELECT SCOPE_IDENTITY();";

        cmd.CommandText = qry;

        cmd.Parameters.Clear();
        cmd.Parameters.AddWithValue("@ErrorDate", DateTime.Now);
        cmd.Parameters.AddWithValue("@URL", URL);
        cmd.Parameters.AddWithValue("@Description", Description);
        cmd.Parameters.AddWithValue("@StackTrace", StackTrace);
        cmd.Parameters.AddWithValue("@UserID", UserID);
        cmd.Parameters.AddWithValue("@IPAddress", IPAddress);
        cmd.Parameters.AddWithValue("@SessionID", SessionID);
        cmd.Parameters.AddWithValue("@QueryString", QueryString);
        cmd.Parameters.AddWithValue("@FormValues", FormValues);
        cmd.Parameters.AddWithValue("@ServerVariables", ServerVariables);

        cmd.ExecuteScalar();

        cmd.Dispose();
        conn.Close();
        conn.Dispose();

        //EmailSender.SendEmail("berczely@hotmail.com", "error@maxbmw.com", "Site Errors", "", "", "Error in MAX BMW" + " - InsertComment", Description);
    }

    public static void InsertError(Exception e)
    {
        string Description = e.Message + "  -  " + e.Source;
        string StackTrace = e.StackTrace;
        string URL = HttpContext.Current.Request.Url.ToString();

        int UserID = 0;
        if ((HttpContext.Current.Session != null) && (HttpContext.Current.Session["UserID"] != null))
            UserID = Convert.ToInt32(HttpContext.Current.Session["UserID"]);

        string IPAddress = HttpContext.Current.Request.UserHostAddress;

        string SessionID = "";
        if (HttpContext.Current.Session != null)
            SessionID = HttpContext.Current.Session.SessionID.ToString();

        string QueryString = HttpContext.Current.Request.QueryString.ToString();
        string FormValues = HttpContext.Current.Request.Form.ToString();
        string ServerVariables = HttpContext.Current.Request.ServerVariables["ALL_HTTP"];
        
        OleDbConnection conn = new OleDbConnection(ConfigurationManager.AppSettings["connString"]);
        conn.Open();
        OleDbCommand cmd = new OleDbCommand();
        cmd.Connection = conn;
        string qry = @"INSERT INTO AuditsErrors (  ErrorDate, URL, Description, StackTrace, UserID, IPAddress, SessionID, QueryString, FormValues, ServerVariables)" +
                            " VALUES ( ?, ?, ?, ?, ?, ?, ?, ?, ?, ?); SELECT SCOPE_IDENTITY();";

        cmd.CommandText = qry;

        cmd.Parameters.Clear();
        cmd.Parameters.AddWithValue("@ErrorDate", DateTime.Now);
        cmd.Parameters.AddWithValue("@URL", URL);        
        cmd.Parameters.AddWithValue("@Description", Description);
        cmd.Parameters.AddWithValue("@StackTrace", StackTrace);
        cmd.Parameters.AddWithValue("@UserID", UserID);
        cmd.Parameters.AddWithValue("@IPAddress", IPAddress);
        cmd.Parameters.AddWithValue("@SessionID", SessionID);
        cmd.Parameters.AddWithValue("@QueryString", QueryString);
        cmd.Parameters.AddWithValue("@FormValues", FormValues);
        cmd.Parameters.AddWithValue("@ServerVariables", ServerVariables);

        cmd.ExecuteScalar();

        cmd.Dispose();
        conn.Close();
        conn.Dispose();
    }
    
    public static void InsertHack()
    {
        string URL = HttpContext.Current.Request.Url.ToString();
        
        int UserID = 0;
        if ((HttpContext.Current.Session != null) && (HttpContext.Current.Session["UserID"] != null))
            UserID = Convert.ToInt32(HttpContext.Current.Session["UserID"]);

        string IPAddress = HttpContext.Current.Request.UserHostAddress;

        string SessionID="";
        if (HttpContext.Current.Session != null)
            SessionID = HttpContext.Current.Session.SessionID.ToString();

        string QueryString = HttpContext.Current.Request.QueryString.ToString();
        string FormValues = HttpContext.Current.Request.Form.ToString();
        string ServerVariables = HttpContext.Current.Request.ServerVariables["ALL_HTTP"];

        OleDbConnection conn = new OleDbConnection(ConfigurationManager.AppSettings["connString"]);
        conn.Open();
        OleDbCommand cmd = new OleDbCommand();
        cmd.Connection = conn;
        string qry = @"INSERT INTO AuditsHacks (  ErrorDate, URL, UserID, IPAddress, SessionID, QueryString, FormValues, ServerVariables)" +
                            " VALUES ( ?, ?, ?, ?, ?, ?, ?, ?); SELECT SCOPE_IDENTITY();";

        cmd.CommandText = qry;

        cmd.Parameters.Clear();
        cmd.Parameters.AddWithValue("@ErrorDate", DateTime.Now);
        cmd.Parameters.AddWithValue("@URL", URL);
        cmd.Parameters.AddWithValue("@UserID", UserID);
        cmd.Parameters.AddWithValue("@IPAddress", IPAddress);
        cmd.Parameters.AddWithValue("@SessionID", SessionID);
        cmd.Parameters.AddWithValue("@QueryString", QueryString);
        cmd.Parameters.AddWithValue("@FormValues", FormValues);
        cmd.Parameters.AddWithValue("@ServerVariables", ServerVariables);

        cmd.ExecuteScalar();

        cmd.Dispose();
        conn.Close();
        conn.Dispose();
    }
}

public class Cookie
{    
    public static void InsertCookie(string CookieName, string CookieValue)
    {
        HttpCookie oCookie2 = new HttpCookie(CookieName);
        oCookie2.Value = CookieValue;
        oCookie2.Expires = DateTime.Now.AddYears(1);
        HttpContext.Current.Response.Cookies.Add(oCookie2);
    }
    public static string GetCookieValue(string CookieName)
    {
        if (HttpContext.Current.Request.Cookies[CookieName] != null)
        {
            return HttpContext.Current.Request.Cookies[CookieName].Value;
        }
        else
        {
            return "";
        }
    }
}

public class CartItem
{
    private long _ID;
    private string _PartNumber;
    private string _Description;
    private double _Weight;
    private string _Dimensions;
    private int _Quantity;
    private double _Price;
    private double _PriceSpecials;
    private double _PriceRetail;
    private bool _IsSpecial;
    private string _SpecialsText;
    private string _SpecialsText2;
    private double _AdditionalShipping;
    private string _CommentsShipping;
    private string _Comments;

    public CartItem()
    {
        _ID = 0;
        _PartNumber = "";
        _Quantity = 0;
        _Price = 0.0;
        _PriceRetail = 0.0;
        _PriceSpecials = 0.0;
        _Description = "";
        _Weight = 0.0;
        _Dimensions = "";
        _AdditionalShipping = 0.0;
        _Comments = "";
        _CommentsShipping = "";
        _IsSpecial = false;
        _SpecialsText = "";
        _SpecialsText2 = "";
    }

    public long ID { get { return _ID; } set { _ID = value; } }
    public string PartNumber { get { return _PartNumber; } set { _PartNumber = value; } }
    public string Description { get { return _Description; } set { _Description = value; } }
    public double Weight { get { return _Weight; } set { _Weight = value; } }
    public string Dimensions { get { return _Dimensions; } set { _Dimensions = value; } }
    public int Quantity { get { return _Quantity; } set { _Quantity = value; } }
    public double Price { get { return _Price; } set { _Price = value; } }
    public double PriceRetail { get { return _PriceRetail; } set { _PriceRetail = value; } }
    public double PriceSpecials { get { return _PriceSpecials; } set { _PriceSpecials = value; } }
    public bool IsSpecial { get { return _IsSpecial; } set { _IsSpecial = value; } }
    public string SpecialsText { get { return _SpecialsText; } set { _SpecialsText = value; } }
    public string SpecialsText2 { get { return _SpecialsText2; } set { _SpecialsText2 = value; } }
    public double AdditionalShipping { get { return _AdditionalShipping; } set { _AdditionalShipping = value; } }
    public string CommentsShipping { get { return _CommentsShipping; } set { _CommentsShipping = value; } }
    public string Comments { get { return _Comments; } set { _Comments = value; } }
}

public class CartItemCollection : System.Collections.CollectionBase
{
    public CartItemCollection()
    {
    }

    public CartItem this[int index]
    {
        get { return (CartItem)this.List[index]; }
        set { this.List[index] = value; }
    }

    public int IndexOf(CartItem item)
    {
        return base.List.IndexOf(item);
    }

    public int Add(CartItem item)
    {
        // first find if there the same PartNumber exist
        long maxID = 0;
        foreach (CartItem cartItem in this)
        {
            if (cartItem.PartNumber == item.PartNumber)
            {
                cartItem.Quantity += item.Quantity;
                return 0;
            }

            if (cartItem.ID > maxID)
                maxID = cartItem.ID;
        }

        item.ID = maxID + 1;

        return this.List.Add(item);
    }

    public void Remove(CartItem item)
    {
        this.InnerList.Remove(item);
    }

    public bool IsFreeShipping()
    {
        bool FreeShipping = true;
        foreach (CartItem cartItem in this)
        {
            if (cartItem.AdditionalShipping != -1.0)
            {
                FreeShipping = false;
                break;
            }            
        }
        return FreeShipping;
    }
}

public class CartHelper : System.Web.UI.Page
{
    public bool IsCartEmpty()
    {
        CartItemCollection myCartItems = (CartItemCollection)Session["CartItems"];
        if (myCartItems != null && myCartItems.Count > 0)
        {
            return false;
        }
        return true;
    }

    public int CartCount()
    {
        CartItemCollection myCartItems = (CartItemCollection)Session["CartItems"];
        if (myCartItems != null && myCartItems.Count > 0)
        {
            int Count = 0;
            foreach (CartItem cartItem in myCartItems)
            {
                Count += cartItem.Quantity;
            }
            return Count;
        }
        return 0;
    }

    public void Clear()
    {
        CartItemCollection myCartItems = (CartItemCollection)Session["CartItems"];
        myCartItems.Clear();
        Session["CartItems"] = myCartItems;
    }

    public string ShowCart(string NewPartNumber, string NewQuantity, string NewDescription, string NewWeight, bool AllowUpdates, bool AllowSpecials)
    {
        string txt = "";
        string trs = "";
        bool bg_alternate = false;

        //****************************************************************
        // Current Cart Items
        //**************************************************************** 
        string q;       //quantity
        string up;      //unit price
        string tp;      //total price
        string qID;     //quantity ID
        string upID;    //unit price ID
        string tpID;    //total price ID
        string asID;    //additional shipping ID
        string dID;     //dimension ID
        string wID;     //weight ID

        string Description; //temp

        CartItemCollection myCartItems = (CartItemCollection)Session["CartItems"];
        if (myCartItems != null && myCartItems.Count > 0)
        {
            // header
            trs = "<tr style=\"font-family: Arial; font-size: 11px; font-weight: bold; background-color: #666; color: #fff;\">" +
                        "<td align=\"center\" style=\"width: 85px; border: solid 1px #666;\">Part Number</td>" +
                        "<td align=\"left\"   style=\"width: 400px; border: solid 1px #666;\">&nbsp;&nbsp;Description</td>" +
                        "<td align=\"center\" style=\"width: 30px; border: solid 1px #666; cursor: help;\" title=\"Weight in Pounds\">lb</td>" +
                        "<td align=\"center\" style=\"width: 80px; border: solid 1px #666; cursor: help;\" title=\"Recomended quantity. Price is for each one.\">Qty<img alt=\"Recomended quantity. Price is for each one.\" src=\"images/infoheader.gif\" /></td>" +
                        "<td align=\"center\" style=\"width: 60px; border: solid 1px #666;\">Each</td>" +
                        "<td align=\"center\" style=\"width: 65px; border: solid 1px #666;\">Total</td>" +
                    "</tr>";

            foreach (CartItem cartItem in myCartItems)
            {
                qID = "Q_" + cartItem.ID;
                upID = "UP_" + cartItem.ID;
                tpID = "TP_" + cartItem.ID;
                asID = "AS_" + cartItem.ID;
                dID = "DIM_" + cartItem.ID;
                wID = "W_" + cartItem.ID;

                q = "document.getElementById('" + qID + "').value";
                up = "document.getElementById('" + upID + "').value";
                tp = "document.getElementById('" + tpID + "').value";

                Description = cartItem.Description;
                if (cartItem.AdditionalShipping > 0.0)
                    Description += "<br />&nbsp;&nbsp;&nbsp;<label style=\"color: #e00; font-weight: bold;\">Additional Shipping charges of " + string.Format("{0:C}", cartItem.AdditionalShipping) + " each!</label>";
                if (cartItem.Dimensions.Length > 0)
                    Description += "<br />&nbsp;&nbsp;&nbsp;<label style=\"color: #e00; font-weight: bold;\">Size \"" + cartItem.Dimensions + "\" (affects shipping costs)</label>";
                if (cartItem.CommentsShipping.Length > 0)
                    Description += "<br />&nbsp;&nbsp;&nbsp;" + cartItem.CommentsShipping;

                // handle specials and AllowSpecials flag (for example, Wholesale users cannot buy at specials prices
                if (!AllowSpecials && cartItem.IsSpecial)
                {
                    cartItem.Price = cartItem.PriceRetail;
                    cartItem.SpecialsText2 = "Due to your profile, you are not allowed to use the discounted price (you might have other discounts already).";
                }

                // Current cart items


                trs += "<tr style=\"vertical-align: middle; font-family: Arial; font-size: 11px; background-color: " + (bg_alternate ? "#f9f9f9" : "#ddd") + ";\">" +
                            "<td align=\"left\" style=\"border-left: solid 1px #666666; font-weight: bold;\">&nbsp;" + cartItem.PartNumber + "</td>" +
                            "<td align=\"left\" style=\"font-family: Arial Narrow; border-left: solid 1px #666;\">&nbsp;" + Description + "</td>" +
                            "<td align=\"right\" style=\"border-left: solid 1px #666;\">" + (cartItem.Weight == 0.0 ? "&nbsp;" : string.Format("{0:0.00}", cartItem.Weight)) + "&nbsp;</td>" +
                            "<td align=\"center\" style=\"width: 70px; border-left: solid 1px #666;\">" +
                                (AllowUpdates ? "<img src=\"images/minus.gif\" style=\"cursor: pointer; width: 10px;\" alt=\"Decrease quantity\"" +
                                " onclick=\"g_IsDirty=true; if (" + q + ">0) " + q + "--; " + tp + "=NumToCurrency(CurrencyToNum(" + up + ")*" + q + "); UpdateFields();\">" : "") +
                                "<input name=\"" + qID + "\" id=\"" + qID + "\" value=\"" + cartItem.Quantity + "\" type=\"text\" readonly=\"readonly\" style=\"width: 33px; border: 0px; background-color: transparent; text-align: center; font-family: Arial; font-size: 8pt;\" />" +
                                (AllowUpdates ? "<img src=\"images/plus.gif\" style=\"cursor: pointer; width: 10px;\" alt=\"Increase quantity\"" +
                                " onclick=\"g_IsDirty=true; if (" + q + "<999) " + q + "++; " + tp + "=NumToCurrency(CurrencyToNum(" + up + ")*" + q + "); UpdateFields();\">" : "") +
                            "</td>" +
                            "<td align=\"right\" style=\"border-left: solid 1px #666;\"" +
                            (cartItem.IsSpecial ? " title=\"" + (cartItem.SpecialsText2 == "" ? cartItem.SpecialsText : cartItem.SpecialsText2) + "\"" : "") + ">" +
                                "<input type=\"text\" readonly=\"readonly\" id=\"" + upID + "\" value=\"" + string.Format("{0:C}", cartItem.Price) + "\" style=\"width: 58px; border: 0px; background-color: transparent; text-align: right; font-family: Arial; font-size: 8pt;" + (cartItem.IsSpecial ? "cursor: help; font-weight: bold; color: #e60;" : "") + "\" />" +
                            "</td>" +
                            "<td align=\"right\" style=\"border-left: solid 1px #666;\">" +
                                "<input type=\"text\" readonly=\"readonly\" id=\"" + tpID + "\" value=\"" + string.Format("{0:C}", (cartItem.Price * cartItem.Quantity)) + "\" style=\"width: 63px; border: 0px; background-color: transparent; text-align: right; font-family: Arial; font-size: 8pt;\" />" +
                                "<input type=\"hidden\" id=\"" + asID + "\" value=\"" + string.Format("{0:C}", cartItem.AdditionalShipping) + "\" />" +                                
                                "<input type=\"hidden\" id=\"" + dID + "\" value=\"" + cartItem.Dimensions + "\" />" +
                                "<input type=\"hidden\" id=\"" + wID + "\" value=\"" + (cartItem.Weight == 0.0 ? "&nbsp;" : string.Format("{0:0.00}", cartItem.Weight)) + "\" />" +
                            "</td>" +
                        "</tr>";

                bg_alternate = !bg_alternate;
            }
        }


        //****************************************************************
        // New Item to Add to the cart
        //****************************************************************
        CartItem NewCartItem = new CartItem();

        NewCartItem.PartNumber = NewPartNumber;
        if (NewQuantity.Length > 0)
        {
            try
            {
                NewCartItem.Quantity = Convert.ToInt16(NewQuantity);
            }
            catch
            {
                NewCartItem.Quantity = 1;
            }
            
            NewCartItem.Description = NewDescription;

            try
            {
                NewCartItem.Weight = Convert.ToDouble(NewWeight);
            }
            catch
            {
                NewCartItem.Weight = 0.0;
            }
        }

        if (NewCartItem.PartNumber.Length > 0 && NewCartItem.Quantity > 0)
        {            
            GetPartDetails(ref NewCartItem);

            trs += "<tr style=\"font-family: Arial; font-size: 11px; font-weight: bold; background-color: #666; color: #fff;\">" +
                        "<td colspan=\"6\" align=\"center\" style=\"border: solid 1px #666;\">Adding new item to cart:</td>" +
                    "</tr>";

            // only show header if there are no items in the cart yet...
            if (myCartItems == null || myCartItems.Count == 0)
            {
                trs += "<tr style=\"font-family: Arial; font-size: 11px; font-weight: bold; background-color: #666; color: #fff;\">" +
                            "<td align=\"center\" style=\"width: 85px; border: solid 1px #666;\">Part Number</td>" +
                            "<td align=\"left\"   style=\"width: 386px; border: solid 1px #666;\">&nbsp;&nbsp;Description</td>" +
                            "<td align=\"center\" style=\"width: 30px; border: solid 1px #666; cursor: help;\" title=\"Weight in Pounds\">lb</td>" +
                            "<td align=\"center\" style=\"width: 80px; border: solid 1px #666; cursor: help;\" title=\"Recomended quantity. Price is for each one.\">Qty<img alt=\"Recomended quantity. Price is for each one.\" src=\"images/infoheader.gif\" /></td>" +
                            "<td align=\"center\" style=\"width: 60px; border: solid 1px #666;\">Each</td>" +
                            "<td align=\"center\" style=\"width: 65px; border: solid 1px #666;\">Total</td>" +
                        "</tr>";
            }

            qID = "Q_ADDED";
            upID = "UP_ADDED";
            tpID = "TP_ADDED";
            asID = "AS_ADDED";
            dID = "D_ADDED";
            wID = "W_ADDED";

            q = "document.getElementById('" + qID + "').value";
            up = "document.getElementById('" + upID + "').value";
            tp = "document.getElementById('" + tpID + "').value";

            Description = NewCartItem.Description;
            if (NewCartItem.AdditionalShipping > 0.0)
                Description += "<br />&nbsp;&nbsp;&nbsp;<label style=\"color: #e00; font-weight: bold;\">Additional Shipping charges of " + string.Format("{0:C}", NewCartItem.AdditionalShipping) + " each</label>";
            if (NewCartItem.Dimensions.Length > 0)
                Description += "<br />&nbsp;&nbsp;&nbsp;<label style=\"color: #e00; font-weight: bold;\">Size \"" + NewCartItem.Dimensions + "\" (affects shipping costs)</label>";
            if (NewCartItem.CommentsShipping.Length > 0)
                Description += "<br />&nbsp;&nbsp;&nbsp;" + NewCartItem.CommentsShipping;

            trs += "<tr style=\"vertical-align: middle; font-family: Arial; font-size: 11px; background-color: #f9f9f9;\">" +
                        "<td align=\"left\" style=\"border-left: solid 1px #666666; font-weight: bold;\">&nbsp;" + NewCartItem.PartNumber +
                            "<input type=\"hidden\" id=\"PARTNUMBER_ADDED\" name=\"PARTNUMBER_ADDED\" value=\"" + NewCartItem.PartNumber + "\" />" +
                        "</td>" +
                        "<td align=\"left\" style=\"font-family: Arial Narrow; border-left: solid 1px #666;\">&nbsp;" + Description +
                            "<input type=\"hidden\" id=\"DESCRIPTION_ADDED\" name=\"DESCRIPTION_ADDED\" value=\"" + NewCartItem.Description + "\" />" +
                        "</td>" +
                        "<td align=\"right\" style=\"border-left: solid 1px #666;\">" + (NewCartItem.Weight == 0.0 ? "&nbsp;" : string.Format("{0:0.00}", NewCartItem.Weight)) + "&nbsp;</td>" +
                        "<td align=\"center\" style=\"border-left: solid 1px #666; width: 70px;\">" +
                            "<img border=\"0\" src=\"images/minus.gif\" style=\"cursor: pointer;\" alt=\"Decrease quantity\"" +
                            " onclick=\"g_IsDirty=true; if (" + q + ">0) " + q + "--; " + tp + "=NumToCurrency(CurrencyToNum(" + up + ")*" + q + "); UpdateFields();\">" +
                            "<input id=\"" + qID + "\" name=\"" + qID + "\" value=\"" + NewCartItem.Quantity + "\" type=\"text\" readonly=\"readonly\" style=\"width: 33px; border: 0px; background-color: transparent; text-align: center; font-family: Arial; font-size: 8pt;\" />" +
                            "<img border=\"0\" src=\"images/plus.gif\" style=\"cursor: pointer;\" alt=\"Increase quantity\"" +
                            " onclick=\"g_IsDirty=true; if (" + q + "<999) " + q + "++; " + tp + "=NumToCurrency(CurrencyToNum(" + up + ")*" + q + "); UpdateFields();\">" +
                        "</td>" +
                        "<td align=\"right\" style=\"border-left: solid 1px #666;\"" +
                        (NewCartItem.IsSpecial ? " title=\"" + (NewCartItem.SpecialsText2 == "" ? NewCartItem.SpecialsText : NewCartItem.SpecialsText2) + "\"" : "") + ">" +
                            "<input type=\"text\" readonly=\"readonly\" id=\"" + upID + "\" value=\"" + string.Format("{0:C}", NewCartItem.Price) + "\" style=\"width: 58px; border: 0px; background-color: transparent; text-align: right; font-family: Arial; font-size: 8pt;" + (NewCartItem.IsSpecial ? "cursor: help; font-weight: bold; color: #e60;" : "") + "\" />" +
                        "</td>" +
                        "<td align=\"right\" style=\"border-left: solid 1px #666;\">" +
                            "<input type=\"text\" readonly=\"readonly\" id=\"" + tpID + "\" value=\"" + string.Format("{0:C}", (NewCartItem.Price * NewCartItem.Quantity)) + "\" style=\"width: 63px; border: 0px; background-color: transparent; text-align: right; font-family: Arial; font-size: 8pt;\" />" +
                            "<input type=\"hidden\" id=\"" + asID + "\" value=\"" + string.Format("{0:C}", NewCartItem.AdditionalShipping) + "\" />" +
                            "<input type=\"hidden\" id=\"" + dID + "\" value=\"" + NewCartItem.Dimensions + "\" />" +
                            "<input type=\"hidden\" id=\"" + wID + "\" name=\"" + wID + "\" value=\"" + (NewCartItem.Weight == 0.0 ? "&nbsp;" : string.Format("{0:0.00}", NewCartItem.Weight)) + "\" />" +                            
                        "</td>" +
                    "</tr>";
        }


        if (trs.Length > 0)
        {
            //****************************************************************
            // SubTotal and Total Weight
            //****************************************************************
            trs += "<tr style=\"font-family: Arial; font-size: 11px; font-weight: bold; background-color: #666; color: #fff;\">" +
                        "<td colspan=\"3\" align=\"right\">" +
                            "Total Weight:<input size=\"3\" type=\"text\" readonly=\"readonly\" name=\"TotalWeight\" id=\"TotalWeight\" value=\"0.0\" style=\"border: 0px; background-color: transparent; text-align: right; font-family: Arial; font-size: 8pt; font-weight: bold; color: #fff\" />lbs" +
                        "</td>" +
                        "<td colspan=\"3\" align=\"right\">" +
                            "Sub Total:<input size=\"6\" type=\"text\" readonly=\"readonly\" name=\"SubTotal\" id=\"SubTotal\" value=\"$0.0\" style=\"border: 0px; background-color: transparent; text-align: right; font-family: Arial; font-size: 8pt; font-weight: bold; color: #fff\" />" +
                        "</td>" +
                    "</tr>";


            //****************************************************************
            // Additional Shipping
            //****************************************************************
            trs += "<tr id=\"TR_ADDITIONALSHIPPING\" style=\"display: none; font-family: Arial; font-size: 11px; font-weight: bold; background-color: #666; color: #fff;\">" +
                        "<td colspan=\"6\" align=\"right\">" +
                            "Additional Shipping Charges:<input size=\"6\" type=\"text\" readonly=\"readonly\" name=\"AdditionalShipping\" id=\"AdditionalShipping\" value=\"-$0.0\" style=\"border: 0px; background-color: transparent; text-align: right; font-family: Arial; font-size: 8pt; font-weight: bold; color: #fff\" />" +
                        "</td>" +
                    "</tr>";


            txt = "<table cellpadding=\"0\" cellspacing=\"0\" style=\"border: solid 1px #666\">" + trs + "</table>";
        }

        return txt;
    }

    public bool GetPartDetails(ref CartItem cartItem)
    {
        // retrieve part info from DB
        OleDbConnection conn = new OleDbConnection(ConfigurationManager.AppSettings["connString"]);
        conn.Open();

        string qry;
        OleDbCommand cmd = new OleDbCommand();
        cmd.Connection = conn;
        OleDbDataReader dr;
        
        string Price = "";
        string PriceRetail = "";
        string SpecialsText = "";
        string PriceListDescription = "";
        cartItem.IsSpecial = Utilities.GetPrice(cartItem.PartNumber, out Price, out PriceRetail, out SpecialsText, out PriceListDescription, ref conn);
        cartItem.Price = Convert.ToDouble(Price);
        cartItem.PriceSpecials = cartItem.Price;
        cartItem.PriceRetail = Convert.ToDouble(PriceRetail);
        cartItem.SpecialsText = SpecialsText;
        cartItem.SpecialsText2 = "";
        if (cartItem.Description=="")
        {
            cartItem.Description = PriceListDescription;
        }        

        // now get data from NotesParts
        qry = @"SELECT ID, Comments, Weight, Dimensions, AdditionalShipping, CommentsShipping FROM NotesParts WHERE PartNumber='" + cartItem.PartNumber + "'";
        cmd.CommandText = qry;
        dr = cmd.ExecuteReader();
        if (dr.Read())
        {
            if (dr["Weight"].ToString().Length > 0 && (double)dr["Weight"] > 0.0)
                cartItem.Weight = (double)dr["Weight"];

            cartItem.Dimensions = dr["Dimensions"].ToString();

            if (dr["AdditionalShipping"].ToString().Length > 0)
                cartItem.AdditionalShipping = (double)dr["AdditionalShipping"];

            cartItem.CommentsShipping = dr["CommentsShipping"].ToString();

            cartItem.Comments = dr["Comments"].ToString();
        }
        dr.Close();
        cmd.Dispose();

        return true;
    }
}

public class Utilities
{
    public const string VERSION = "10232024";

    public static void DebugWrite(string FileName, string txt)
    {
        File.Delete(FileName);
        TextWriter tw = new StreamWriter(FileName);        
        tw.WriteLine(txt);
        tw.Close();
    }

    public static string URL_HOME()
    {
        //if (HttpContext.Current.Request.Url.AbsoluteUri.IndexOf("://localhost")>0)
            return "http://localhost";
        //else if (HttpContext.Current.Request.Url.AbsoluteUri.IndexOf("://69.168.8.202")>0)
        //    return "http://69.168.8.202";
        //else
        //    return "http://shop.maxbmw.com";                        
        
    }
    
    public static string URL_FICHE(bool HTTPS)
    {
        //if (HttpContext.Current.Request.Url.AbsoluteUri.IndexOf("://localhost")>0)
            return "http://localhost:50368/fiche";
        //else if (HttpContext.Current.Request.Url.AbsoluteUri.IndexOf("://69.168.8.202")>0)
        //    return (HTTPS ? "https" : "http") + "://69.168.8.202/fiche";
        //else
        //    return (HTTPS ? "https" : "http") + "://shop.maxbmw.com/fiche";
    }

    public static void CheckValidURL()
    {
        if (HttpContext.Current.Request.Url.Host != "localhost" && HttpContext.Current.Request.Url.Host != "69.168.8.202")
        {
            if (HttpContext.Current.Request.Url.Host.ToLower() != "shop.maxbmw.com")
            {
                HttpContext.Current.Response.Redirect(HttpContext.Current.Request.Url.Scheme + "://shop.maxbmw.com" + HttpContext.Current.Request.Url.PathAndQuery);
            }            
        }
    }
    
    public static string QueryString(string Parameter)
    {
        Object obj = HttpContext.Current.Request.QueryString[Parameter];
        if (obj != null)
            return obj.ToString().Trim();
        else
            return "";
    }

    public static string SForm(string Parameter)
    {
        Object obj = HttpContext.Current.Request.Form[Parameter];
        if (obj != null)
            return obj.ToString().Trim();
        else
            return "";
    }

    public static double DForm(string Parameter)
    {
        string s = "0.0";
        try
        {
            Object obj = HttpContext.Current.Request.Form[Parameter];
            if (obj != null)
                s = obj.ToString().Replace("$", "").Replace("%", "");

            if (s.Length == 0)
                s = "0.0";
        }
        catch { }
        double d = 0.0;

        try
        {
            d = Convert.ToDouble(s);
        }
        catch
        {
            d = 0.00;
        }

        return d;
    }

    public static int IForm(string Parameter)
    {
        string s = "0";
        try
        {
            Object obj = HttpContext.Current.Request.Form[Parameter];
            if (obj != null)
                s = obj.ToString().Replace("$", "").Replace("%", "");

            if (s.Length == 0)
                s = "0";
        }
        catch { }
        int i = 0;
        try
        {
            i = Convert.ToInt32(s);
        }
        catch
        {
            i = 0;
        }
        return i;
    }
        
    public static void SetLastURL()
    {
        HttpContext.Current.Session["LastCheckoutURL"] = HttpContext.Current.Request.Url.ToString();
        HttpContext.Current.Session["LastLoginURL"] = HttpContext.Current.Request.Url.ToString();
    }

    public static void SetLastURL(string URL)
    {
        HttpContext.Current.Session["LastCheckoutURL"] = URL;
        HttpContext.Current.Session["LastLoginURL"] = URL;
    }

    public static string GetLastCheckoutURL()
    {
        if (HttpContext.Current.Session["LastCheckoutURL"] == null || HttpContext.Current.Session["LastCheckoutURL"].ToString().Length == 0)
            return "fiche.aspx";
        else
            return HttpContext.Current.Session["LastCheckoutURL"].ToString();
    }

    public static void SetLastCheckoutURL()
    {
        HttpContext.Current.Session["LastCheckoutURL"] = HttpContext.Current.Request.Url.ToString();
    }

    public static void SetLastCheckoutURL(string URL)
    {
        HttpContext.Current.Session["LastCheckoutURL"] = URL;
    }    

    public static string GetLastLoginURL()
    {
        if (HttpContext.Current.Session["LastLoginURL"] == null || HttpContext.Current.Session["LastLoginURL"].ToString().Length == 0)
            return "fiche.aspx";
        else
            return HttpContext.Current.Session["LastLoginURL"].ToString();
    }

    public static void SetLastLoginURL()
    {
        HttpContext.Current.Session["LastLoginURL"] = HttpContext.Current.Request.Url.ToString();
    }

    public static void SetLastLoginURL(string URL)
    {
        HttpContext.Current.Session["LastLoginURL"] = URL;
    }

    public static string GetUrlReferrer()
    {
        if (HttpContext.Current.Request.ServerVariables["HTTP_REFERER"] != null)
            return HttpContext.Current.Request.ServerVariables["HTTP_REFERER"].ToString();
        else
            return "";
    }
       
    // returns true if it is a special price
    public static bool GetPrice(string PartNumber, out string Price, out string OriginalPrice, out string SpecialText, out string PriceListDescription, ref OleDbConnection conn)
    {
        bool IsSpecial = false;
        
        SpecialText = "";
        PriceListDescription = "";
        Price = "";
        OriginalPrice = "";
        
        if (PartNumber == null || PartNumber.Length == 0)
            return IsSpecial;
                
        OleDbCommand cmdPrice = new OleDbCommand();
        cmdPrice.Connection = conn;
        cmdPrice.CommandText = @"SELECT PriceList.Retail, PriceList.Description, NotesParts.SpecialsPrice, NotesParts.SpecialsDiscount, NotesParts.SpecialsComments FROM PriceList LEFT OUTER JOIN NotesParts ON PriceList.PartNumber=NotesParts.PartNumber WHERE PriceList.PartNumber=?";
        cmdPrice.Parameters.Clear();
        cmdPrice.Parameters.AddWithValue("@PartNumber", PartNumber);        
        OleDbDataReader drPrice = cmdPrice.ExecuteReader();
        
        if (drPrice.Read())
        {
            double dRetail;
            double dSpecialsPrice;
            double dSpecialsDiscount;

            dRetail = Convert.ToDouble(drPrice["Retail"]);

            if (PriceListDescription != null)
                PriceListDescription = (drPrice["Description"] != DBNull.Value ? drPrice["Description"].ToString() : "");

            dSpecialsPrice = (drPrice["SpecialsPrice"] != DBNull.Value ? Convert.ToDouble(drPrice["SpecialsPrice"]) : 0.0);
            dSpecialsDiscount = (drPrice["SpecialsDiscount"] != DBNull.Value ? Convert.ToDouble(drPrice["SpecialsDiscount"]) : 0.0);

            OriginalPrice = dRetail.ToString("N");

            if (dSpecialsDiscount != 0.0)
            {
                IsSpecial = true;
                Price = (dRetail - ((dRetail * dSpecialsDiscount) / 100.0)).ToString("N");
                SpecialText = Convert.ToDecimal(dSpecialsDiscount).ToString() + "% OFF";
            }
            else if (dSpecialsPrice != 0.0 && dRetail>0.0)
            {
                IsSpecial = true;
                Price = dSpecialsPrice.ToString("N");
                SpecialText = "Was $" + dRetail.ToString("N");                
            }
            else
            {
                Price = dRetail.ToString("N");
            }

            if (IsSpecial)
            {
                SpecialText += (drPrice["SpecialsComments"] != DBNull.Value ? " - " + drPrice["SpecialsComments"].ToString() : "");
            }
        }

        drPrice.Close();
        drPrice.Dispose();
        cmdPrice.Dispose();

        return IsSpecial;
    }

    public static bool GetSpecials(string PartNumber, ref string Price, out string SpecialText, out string PriceListDescription, ref OleDbConnection conn)
    {
        bool IsSpecial = false;

        SpecialText = "";
        PriceListDescription = "";

        if (PartNumber=="" || Price=="")
            return IsSpecial;

        OleDbCommand cmd = new OleDbCommand();
        cmd.Connection = conn;
        cmd.CommandText = @"SELECT SpecialsPrice, SpecialsDiscount, SpecialsComments FROM NotesParts WHERE PartNumber=?";
        cmd.Parameters.Clear();
        cmd.Parameters.AddWithValue("@PartNumber", PartNumber);
        OleDbDataReader dr = cmd.ExecuteReader();

        if (dr.Read())
        {
            double dRetail;
            double dSpecialsPrice;
            double dSpecialsDiscount;

            dRetail = Convert.ToDouble(Price);
            
            dSpecialsPrice = (dr["SpecialsPrice"] != DBNull.Value ? Convert.ToDouble(dr["SpecialsPrice"]) : 0.0);
            dSpecialsDiscount = (dr["SpecialsDiscount"] != DBNull.Value ? Convert.ToDouble(dr["SpecialsDiscount"]) : 0.0);

            if (dSpecialsDiscount != 0.0)
            {
                IsSpecial = true;
                Price = (dRetail - ((dRetail * dSpecialsDiscount) / 100.0)).ToString("N");
                SpecialText = Convert.ToDecimal(dSpecialsDiscount).ToString() + "% OFF";
            }
            else if (dSpecialsPrice != 0.0 && dRetail>0.0)
            {
                IsSpecial = true;
                Price = dSpecialsPrice.ToString("N");
                SpecialText = "Was $" + dRetail.ToString("N");
            }
            else
            {
                Price = dRetail.ToString("N");
            }

            if (IsSpecial)
            {
                SpecialText += (dr["SpecialsComments"] != DBNull.Value ? " - " + dr["SpecialsComments"].ToString() : "");
            }
        }

        dr.Close();
        dr.Dispose();
        cmd.Dispose();

        return IsSpecial;
    }

    public static string GetFullCountryName(string Country2)
    {
        string CountryFull = "";

        if (Country2 == "US") { CountryFull = "United States of America"; }
        else if (Country2 == "CA") { CountryFull = "Canada"; }
        else if (Country2 == "AF") { CountryFull = "Afghanistan"; }
        else if (Country2 == "AL") { CountryFull = "Albania"; }
        else if (Country2 == "DZ") { CountryFull = "Algeria"; }
        else if (Country2 == "AD") { CountryFull = "Andorra"; }
        else if (Country2 == "AO") { CountryFull = "Angola"; }
        else if (Country2 == "AI") { CountryFull = "Anguilla"; }
        else if (Country2 == "AG") { CountryFull = "Antigua and Barbuda"; }
        else if (Country2 == "AR") { CountryFull = "Argentina"; }
        else if (Country2 == "AM") { CountryFull = "Armenia"; }
        else if (Country2 == "AW") { CountryFull = "Aruba"; }
        else if (Country2 == "AU") { CountryFull = "Australia"; }
        else if (Country2 == "AT") { CountryFull = "Austria"; }
        else if (Country2 == "AZ") { CountryFull = "Azerbaijan"; }
        else if (Country2 == "BS") { CountryFull = "Bahamas"; }
        else if (Country2 == "BH") { CountryFull = "Bahrain"; }
        else if (Country2 == "BD") { CountryFull = "Bangladesh"; }
        else if (Country2 == "BB") { CountryFull = "Barbados"; }
        else if (Country2 == "BY") { CountryFull = "Belarus"; }
        else if (Country2 == "BE") { CountryFull = "Belgium"; }
        else if (Country2 == "BZ") { CountryFull = "Belize"; }
        else if (Country2 == "BJ") { CountryFull = "Benin"; }
        else if (Country2 == "BM") { CountryFull = "Bermuda"; }
        else if (Country2 == "BT") { CountryFull = "Bhutan"; }
        else if (Country2 == "BO") { CountryFull = "Bolivia"; }
        else if (Country2 == "BA") { CountryFull = "Bosnia-Herzegovina"; }
        else if (Country2 == "BW") { CountryFull = "Botswana"; }
        else if (Country2 == "BR") { CountryFull = "Brazil"; }
        else if (Country2 == "IO") { CountryFull = "British Virgin Islands"; }
        else if (Country2 == "BN") { CountryFull = "Brunei Darussalam"; }
        else if (Country2 == "BG") { CountryFull = "Bulgaria"; }
        else if (Country2 == "BF") { CountryFull = "Burma"; }
        else if (Country2 == "BI") { CountryFull = "Burundi"; }
        else if (Country2 == "KH") { CountryFull = "Cambodia"; }
        else if (Country2 == "CM") { CountryFull = "Cameroon"; }
        else if (Country2 == "CV") { CountryFull = "Cape Verde"; }
        else if (Country2 == "KY") { CountryFull = "Cayman Islands"; }
        else if (Country2 == "CF") { CountryFull = "Central African Republic"; }
        else if (Country2 == "TD") { CountryFull = "Chad"; }
        else if (Country2 == "CL") { CountryFull = "Chile"; }
        else if (Country2 == "CN") { CountryFull = "China"; }
        else if (Country2 == "CO") { CountryFull = "Colombia"; }
        else if (Country2 == "KM") { CountryFull = "Comoros"; }
        else if (Country2 == "CD") { CountryFull = "Congo, Democratic Republic of the"; }
        else if (Country2 == "CG") { CountryFull = "Congo, Republic of the"; }
        else if (Country2 == "CR") { CountryFull = "Costa Rica"; }
        else if (Country2 == "CI") { CountryFull = "Cote d’Ivoire"; }
        else if (Country2 == "HR") { CountryFull = "Croatia"; }
        else if (Country2 == "CU") { CountryFull = "Cuba"; }
        else if (Country2 == "CY") { CountryFull = "Cyprus"; }
        else if (Country2 == "CZ") { CountryFull = "Czech Republic"; }
        else if (Country2 == "DK") { CountryFull = "Denmark"; }
        else if (Country2 == "DJ") { CountryFull = "Djibouti"; }
        else if (Country2 == "DM") { CountryFull = "Dominica"; }
        else if (Country2 == "DO") { CountryFull = "Dominican Republic"; }
        else if (Country2 == "EC") { CountryFull = "Ecuador"; }
        else if (Country2 == "EG") { CountryFull = "Egypt"; }
        else if (Country2 == "SV") { CountryFull = "El Salvador"; }
        else if (Country2 == "GQ") { CountryFull = "Equatorial Guinea"; }
        else if (Country2 == "ER") { CountryFull = "Eritrea"; }
        else if (Country2 == "EE") { CountryFull = "Estonia"; }
        else if (Country2 == "ET") { CountryFull = "Ethiopia"; }
        else if (Country2 == "FO") { CountryFull = "Faroe Islands"; }
        else if (Country2 == "FJ") { CountryFull = "Fiji"; }
        else if (Country2 == "FI") { CountryFull = "Finland"; }
        else if (Country2 == "FR") { CountryFull = "France"; }
        else if (Country2 == "GF") { CountryFull = "French Guiana"; }
        else if (Country2 == "PF") { CountryFull = "French Polynesia"; }
        else if (Country2 == "GA") { CountryFull = "Gabon"; }
        else if (Country2 == "GM") { CountryFull = "Gambia"; }
        else if (Country2 == "GE") { CountryFull = "Georgia, Republic of"; }
        else if (Country2 == "DE") { CountryFull = "Germany"; }
        else if (Country2 == "GH") { CountryFull = "Ghana"; }
        else if (Country2 == "GI") { CountryFull = "Gibraltar"; }
        else if (Country2 == "GB") { CountryFull = "Great Britain and Northern Ireland"; }
        else if (Country2 == "GR") { CountryFull = "Greece"; }
        else if (Country2 == "GL") { CountryFull = "Greenland"; }
        else if (Country2 == "GD") { CountryFull = "Grenada"; }
        else if (Country2 == "GP") { CountryFull = "Guadeloupe"; }
        else if (Country2 == "GT") { CountryFull = "Guatemala"; }
        else if (Country2 == "GN") { CountryFull = "Guinea"; }
        else if (Country2 == "GW") { CountryFull = "Guinea–Bissau"; }
        else if (Country2 == "GY") { CountryFull = "Guyana"; }
        else if (Country2 == "HT") { CountryFull = "Haiti"; }
        else if (Country2 == "HN") { CountryFull = "Honduras"; }
        else if (Country2 == "HK") { CountryFull = "Hong Kong"; }
        else if (Country2 == "HU") { CountryFull = "Hungary"; }
        else if (Country2 == "IS") { CountryFull = "Iceland"; }
        else if (Country2 == "IN") { CountryFull = "India"; }
        else if (Country2 == "ID") { CountryFull = "Indonesia"; }
        else if (Country2 == "IR") { CountryFull = "Iran"; }
        else if (Country2 == "IQ") { CountryFull = "Iraq"; }
        else if (Country2 == "IE") { CountryFull = "Ireland"; }
        else if (Country2 == "IL") { CountryFull = "Israel"; }
        else if (Country2 == "IT") { CountryFull = "Italy"; }
        else if (Country2 == "JM") { CountryFull = "Jamaica"; }
        else if (Country2 == "JP") { CountryFull = "Japan"; }
        else if (Country2 == "JO") { CountryFull = "Jordan"; }
        else if (Country2 == "KZ") { CountryFull = "Kazakhstan"; }
        else if (Country2 == "KE") { CountryFull = "Kenya"; }
        else if (Country2 == "KI") { CountryFull = "Kiribati"; }
        else if (Country2 == "KP") { CountryFull = "Korea, Democratic People’s Republic"; }
        else if (Country2 == "KR") { CountryFull = "Korea, Republic of (South Korea)"; }
        else if (Country2 == "KW") { CountryFull = "Kuwait"; }
        else if (Country2 == "KG") { CountryFull = "Kyrgyzstan"; }
        else if (Country2 == "LA") { CountryFull = "Laos"; }
        else if (Country2 == "LV") { CountryFull = "Latvia"; }
        else if (Country2 == "LB") { CountryFull = "Lebanon"; }
        else if (Country2 == "LS") { CountryFull = "Lesotho"; }
        else if (Country2 == "LR") { CountryFull = "Liberia"; }
        else if (Country2 == "LY") { CountryFull = "Libya"; }
        else if (Country2 == "LI") { CountryFull = "Liechtenstein"; }
        else if (Country2 == "LT") { CountryFull = "Lithuania"; }
        else if (Country2 == "LU") { CountryFull = "Luxembourg"; }
        else if (Country2 == "MO") { CountryFull = "Macao"; }
        else if (Country2 == "MK") { CountryFull = "Macedonia, Republic of"; }
        else if (Country2 == "MG") { CountryFull = "Madagascar"; }
        else if (Country2 == "MW") { CountryFull = "Malawi"; }
        else if (Country2 == "MY") { CountryFull = "Malaysia"; }
        else if (Country2 == "MV") { CountryFull = "Maldives"; }
        else if (Country2 == "ML") { CountryFull = "Mali"; }
        else if (Country2 == "MT") { CountryFull = "Malta"; }
        else if (Country2 == "MQ") { CountryFull = "Martinique"; }
        else if (Country2 == "MR") { CountryFull = "Mauritania"; }
        else if (Country2 == "MU") { CountryFull = "Mauritius"; }
        else if (Country2 == "MX") { CountryFull = "Mexico"; }
        else if (Country2 == "FM") { CountryFull = "Moldova"; }
        else if (Country2 == "MN") { CountryFull = "Mongolia"; }
        else if (Country2 == "ME") { CountryFull = "Montenegro"; }
        else if (Country2 == "MS") { CountryFull = "Montserrat"; }
        else if (Country2 == "MA") { CountryFull = "Morocco"; }
        else if (Country2 == "MZ") { CountryFull = "Mozambique"; }
        else if (Country2 == "NA") { CountryFull = "Namibia"; }
        else if (Country2 == "NR") { CountryFull = "Nauru"; }
        else if (Country2 == "NP") { CountryFull = "Nepal"; }
        else if (Country2 == "NL") { CountryFull = "Netherlands"; }
        else if (Country2 == "AN") { CountryFull = "Netherlands Antilles"; }
        else if (Country2 == "NC") { CountryFull = "New Caledonia"; }
        else if (Country2 == "NZ") { CountryFull = "New Zealand"; }
        else if (Country2 == "NI") { CountryFull = "Nicaragua"; }
        else if (Country2 == "NE") { CountryFull = "Niger"; }
        else if (Country2 == "NG") { CountryFull = "Nigeria"; }        
        else if (Country2 == "NO") { CountryFull = "Norway"; }
        else if (Country2 == "OM") { CountryFull = "Oman"; }
        else if (Country2 == "PK") { CountryFull = "Pakistan"; }
        else if (Country2 == "PA") { CountryFull = "Panama"; }
        else if (Country2 == "PG") { CountryFull = "Papua New Guinea"; }
        else if (Country2 == "PY") { CountryFull = "Paraguay"; }
        else if (Country2 == "PE") { CountryFull = "Peru"; }
        else if (Country2 == "PH") { CountryFull = "Philippines"; }
        else if (Country2 == "PN") { CountryFull = "Pitcairn Island"; }
        else if (Country2 == "PL") { CountryFull = "Poland"; }
        else if (Country2 == "PT") { CountryFull = "Portugal"; }
        else if (Country2 == "QA") { CountryFull = "Qatar"; }
        else if (Country2 == "RE") { CountryFull = "Reunion"; }
        else if (Country2 == "RO") { CountryFull = "Romania"; }
        else if (Country2 == "RU") { CountryFull = "Russia"; }
        else if (Country2 == "RW") { CountryFull = "Rwanda"; }
        else if (Country2 == "SH") { CountryFull = "Saint Helena"; }
        else if (Country2 == "LC") { CountryFull = "Saint Lucia"; }
        else if (Country2 == "PM") { CountryFull = "Saint Pierre and Miquelon"; }
        else if (Country2 == "VC") { CountryFull = "Saint Vincent and the Grenadines"; }
        else if (Country2 == "SM") { CountryFull = "San Marino"; }
        else if (Country2 == "ST") { CountryFull = "Sao Tome and Principe"; }
        else if (Country2 == "SA") { CountryFull = "Saudi Arabia"; }
        else if (Country2 == "SN") { CountryFull = "Senegal"; }
        else if (Country2 == "CS") { CountryFull = "Serbia, Republic of"; }
        else if (Country2 == "SC") { CountryFull = "Seychelles"; }
        else if (Country2 == "SL") { CountryFull = "Sierra Leone"; }
        else if (Country2 == "SG") { CountryFull = "Singapore"; }
        else if (Country2 == "SI") { CountryFull = "Slovenia"; }
        else if (Country2 == "SB") { CountryFull = "Solomon Islands"; }
        else if (Country2 == "SO") { CountryFull = "Somalia"; }
        else if (Country2 == "ZA") { CountryFull = "South Africa"; }
        else if (Country2 == "ES") { CountryFull = "Spain"; }
        else if (Country2 == "LK") { CountryFull = "Sri Lanka"; }
        else if (Country2 == "SD") { CountryFull = "Sudan"; }
        else if (Country2 == "SR") { CountryFull = "Suriname"; }
        else if (Country2 == "SZ") { CountryFull = "Swaziland"; }
        else if (Country2 == "SE") { CountryFull = "Sweden"; }
        else if (Country2 == "CH") { CountryFull = "Switzerland"; }
        else if (Country2 == "TW") { CountryFull = "Taiwan"; }
        else if (Country2 == "TJ") { CountryFull = "Tajikistan"; }
        else if (Country2 == "TZ") { CountryFull = "Tanzania"; }
        else if (Country2 == "TH") { CountryFull = "Thailand"; }
        else if (Country2 == "TG") { CountryFull = "Togo"; }
        else if (Country2 == "TO") { CountryFull = "Tonga"; }
        else if (Country2 == "TT") { CountryFull = "Trinidad and Tobago"; }
        else if (Country2 == "TN") { CountryFull = "Tunisia"; }
        else if (Country2 == "TR") { CountryFull = "Turkey"; }
        else if (Country2 == "TM") { CountryFull = "Turkmenistan"; }
        else if (Country2 == "TC") { CountryFull = "Turks and Caicos Islands"; }
        else if (Country2 == "TV") { CountryFull = "Tuvalu"; }
        else if (Country2 == "UG") { CountryFull = "Uganda"; }
        else if (Country2 == "UA") { CountryFull = "Ukraine"; }
        else if (Country2 == "AE") { CountryFull = "United Arab Emirates"; }
        else if (Country2 == "UY") { CountryFull = "Uruguay"; }
        else if (Country2 == "UZ") { CountryFull = "Uzbekistan"; }
        else if (Country2 == "VU") { CountryFull = "Vanuatu"; }
        else if (Country2 == "VE") { CountryFull = "Venezuela"; }
        else if (Country2 == "VN") { CountryFull = "Vietnam"; }
        else if (Country2 == "WF") { CountryFull = "Wallis and Futuna Islands"; }
        else if (Country2 == "EH") { CountryFull = "Western Samoa"; }
        else if (Country2 == "YE") { CountryFull = "Yemen"; }
        else if (Country2 == "ZM") { CountryFull = "Zambia"; }
        else if (Country2 == "ZW") { CountryFull = "Zimbabwe"; }

        return CountryFull;
    }
}

public class Checkout
{
    public string IPAddress;
    public string SessionID;
    public string UserID;
    public string eMail;
    public string Coupons;
    public string FirstName;
    public string MiddleName;
    public string LastName;
    public string Address;
    public string Address2;
    public string City;
    public string State;
    public string Country;
    public string ZIP;
    public string Phone;
    public string Phone2;
    public string Phone3;
    public string PaymentType;
    public string CardFirstName;
    public string CardMiddleName;
    public string CardLastName;
    public string CardNumber;
    public string CardExpMonth;
    public string CardExpYear;
    public string CardCVV2;

    public string CardType;
    public string CardLast4;
    public string CardAddress;
    public string CardAddress2;
    public string CardState;
    public string CardCity;
    public string CardZIP;
    public string CardCountry;
    public string CardPhone;
    public string VehicleInfo;
    public string Comments;
    public bool SignUp;

    public double TotalWeight;
    public double SubTotal;
    public string ShippingMethod;
    public double Shipping;
    public double AdditionalShipping;
    public double VOR;
    public double CouponsDiscount;
    public double CouponsPercentage;
    public double CouponsPercentageMoney;

    public string TaxID;
    public double Taxes;
    public double TaxesMoney;
    public double Total;

    public bool IsWholesale;
    public int WholesaleDiscount;
    public double WholesaleDiscountMoney;
    public double WholesaleMinimumPurchase;
    public string WholesaleCompany;
    public string WholesaleCheckFirstName;
    public string WholesaleCheckLastName;
    public string WholesaleCheckCompany;
    public string WholesaleCheckAddress;

    public int OrderID;
    public string TransactionID;
    public string TransactionResponse;
    public bool TransactionApproved;
    public string TransactionAVSZIP;
    public string TransactionAVSADDR;
    public string TransactionCVV;
    public int TransactionCode;

    public Checkout()
    {
        IPAddress = HttpContext.Current.Request.UserHostAddress;
        SessionID = HttpContext.Current.Session.SessionID.ToString();

        UserID = "";
        eMail = "";
        Coupons = "";
        FirstName = "";
        MiddleName = "";
        LastName = "";
        Address = "";
        Address2 = "";
        City = "";
        State = "";
        Country = "";
        ZIP = "";
        Phone = "";
        Phone2 = "";
        Phone3 = "";

        PaymentType = "";

        CardFirstName = "";
        CardMiddleName = "";
        CardLastName = "";
        CardExpMonth = "";
        CardExpYear = "";
        CardCVV2 = "";
        CardCountry = "";
        CardState = "";
        CardAddress = "";
        CardAddress2 = "";
        CardCity = "";
        CardZIP = "";
        CardPhone = "";
        CardType = "";
        CardLast4 = "";

        VehicleInfo = "";
        Comments = "";
        SignUp = false;
        ShippingMethod = "";
        TotalWeight = 0.0;
        SubTotal = 0.0;
        Shipping = 0.0;
        AdditionalShipping = 0.0;
        VOR = 0.0;
        CouponsDiscount = 0.0;
        CouponsPercentage = 0.0;
        CouponsPercentageMoney = 0.0;

        WholesaleDiscount = 0;
        WholesaleDiscountMoney = 0.0;
        IsWholesale = false;
        WholesaleDiscount = 0;
        WholesaleDiscountMoney = 0.0;
        WholesaleMinimumPurchase = 0.0;
        WholesaleCompany = "";
        WholesaleCheckFirstName = "";
        WholesaleCheckLastName = "";
        WholesaleCheckCompany = "";
        WholesaleCheckAddress = "";

        TaxID = "";
        Taxes = 0.0;
        TaxesMoney = 0.0;
        Total = 0.0;

        OrderID = -1;
        TransactionID = "";
        TransactionResponse = "";
        TransactionApproved = false;
        TransactionAVSZIP = "";
        TransactionAVSADDR = "";
        TransactionCVV = "";
        TransactionCode = 0;
}

    private string FixCardPhone(string Phone, bool IsInternational)
    {
        string strippedPhone = Phone.Replace("-", "").Replace(".", "").Replace(" ", "").Replace("(", "").Replace(")", "").Replace("'", "").Replace("+", "");

        if (strippedPhone.Length == 10 && !IsInternational)
        {
            strippedPhone = strippedPhone.Substring(0, 3) + "-" + strippedPhone.Substring(3, 3) + "-" + strippedPhone.Substring(6, 4);
        }

        if (IsInternational)
            strippedPhone = "+" + strippedPhone;

        return strippedPhone;
    }

    private string FixPhone(string Phone)
    {
        return Phone.Replace("-", "").Replace(".", "").Replace(" ", "").Replace("(", "").Replace(")", "").Replace("'", "");
    }

    private string FixCanadianZIP(string ZIP)
    {
        string txt = ZIP;
        if (ZIP.Length == 6)
            txt = ZIP.Substring(0, 3) + "&nbsp;" + ZIP.Substring(3, 3);
        else if (ZIP.Length == 7)
            txt = ZIP.Substring(0, 3) + "&nbsp;" + ZIP.Substring(4, 3);

        return txt;
    }

    public string CreateEmailBody()
    {
        string Body = string.Empty;

        Body += "ORDER #" + OrderID.ToString() + (TransactionID == "PAYPAL" ? "  -  PAYPAL" : "  -  PayFlow# " + TransactionID) + Environment.NewLine;
        Body += "Customer: " + eMail + Environment.NewLine;
        Body += (WholesaleDiscountMoney != 0.0 ? "WHOLESALE CUSTOMER - " + WholesaleDiscount.ToString() + "%" + Environment.NewLine : "");

        Body += Environment.NewLine;
        Body += "SHIPPING INFO:" + Environment.NewLine;
        Body += "--------------------------------------------------------------------------------" + Environment.NewLine;
        Body += "Name: " + FirstName + " " + MiddleName + (MiddleName.Length > 0 ? ". " : "") + LastName + Environment.NewLine;
        Body += "Address: " + Address + Environment.NewLine;
        Body += (Address2.Length > 0 ? "         " + Address2 + Environment.NewLine : "");
        Body += "         " + City + ", " + State + " " + FixCanadianZIP(ZIP) + "   " + Utilities.GetFullCountryName(Country) + Environment.NewLine;
        Body += Environment.NewLine;
        Body += (Phone.Length > 0 ? "Home Phone: " + FixPhone(Phone) + Environment.NewLine : "");
        Body += (Phone2.Length > 0 ? "Work Phone: " + FixPhone(Phone2) + Environment.NewLine : "");
        Body += (Phone3.Length > 0 ? "Cell Phone: " + FixPhone(Phone3) + Environment.NewLine : "");
        Body += "e-mail: " + eMail + Environment.NewLine;

        Body += Environment.NewLine;
        if (ShippingMethod != "")
            Body += ShippingMethod.Replace("-", " - ") + "  (" + TotalWeight.ToString() + "lb)" + Environment.NewLine;

        Body += (VOR > 0 ? "VOR - " + string.Format("{0:C}", VOR) + Environment.NewLine : "");
        Body += (AdditionalShipping > 0 ? "Additional Shipping - " + string.Format("{0:C}", AdditionalShipping) + Environment.NewLine : "");
        Body += Environment.NewLine;
        Body += Environment.NewLine;
        Body += "BILLING INFO:" + Environment.NewLine;
        Body += "--------------------------------------------------------------------------------" + Environment.NewLine;
        if (PaymentType == "C")
        {
            Body += "Name: " + CardFirstName + " " + CardMiddleName + " " + CardLastName + "      ZIP: " + FixCanadianZIP(CardZIP) + Environment.NewLine;
            Body += "Card: " + GetCardType(CardType) + "  XXXX XXXX XXXX " + CardLast4;
            Body += (TransactionApproved ? (TransactionCode != 0 ? " - APPROVED (" + TransactionCode.ToString() + ")" : "") : " - DECLINED (" + TransactionCode.ToString() + ")");
            Body += (TransactionCVV == "N" ? " - CVV DOES NOT MATCH" : "") + Environment.NewLine;
        }
        else
        {
            Body += "PAYPAL payment from [" + eMail + "] for ORDER#" + OrderID.ToString() + Environment.NewLine;
        }
        Body += Environment.NewLine;
        Body += Environment.NewLine;


        CartItemCollection myCartItems = (CartItemCollection)HttpContext.Current.Session["CartItems"];
        if (myCartItems != null && myCartItems.Count > 0)
        {
            Body += "   Part#           |Description                             |  Each      |  Total" + Environment.NewLine;
            Body += "-------------------|----------------------------------------|------------|---------------------" + Environment.NewLine;

            foreach (CartItem cartItem in myCartItems)
            {
                Body += cartItem.Quantity.ToString().PadLeft(3, ' ') + "/" +
                        cartItem.PartNumber.PadRight(15, ' ') + "|" +
                        (cartItem.Description.Length > 40 ? cartItem.Description.Substring(0, 40).PadRight(40, ' ') : cartItem.Description.PadRight(40, ' ')) + "|" +
                        string.Format("{0:C}", cartItem.Price).PadLeft(12, ' ') + "|" +
                        string.Format("{0:C}", cartItem.Price * cartItem.Quantity).PadLeft(12, ' ') +
                        (cartItem.IsSpecial ? "  SPECIAL (" + (cartItem.SpecialsText2 == "" ? cartItem.SpecialsText : cartItem.SpecialsText2) + ")" : "");

                if (cartItem.AdditionalShipping > 0 || cartItem.CommentsShipping.Length > 0)
                {
                    Body += Environment.NewLine;
                    Body += "   --->";
                    Body += (cartItem.AdditionalShipping > 0 ? "  Additional shipping: " + string.Format("{0:C}", cartItem.AdditionalShipping) : "");
                    Body += (cartItem.CommentsShipping.Length > 0 ? "  " + cartItem.CommentsShipping : "");
                }
                Body += Environment.NewLine;
            }
        }
        Body += "-----------------------------------------------------------------------------------------------" + Environment.NewLine;

        int w = 4 + 16 + 41 + 13;
        Body += "SubTotal: ".PadLeft(w) + string.Format("{0:C}", SubTotal).PadLeft(12) + Environment.NewLine;
        Body += "Shipping: ".PadLeft(w) + string.Format("{0:C}", Shipping).PadLeft(12) + (ShippingMethod != "" ? "  (" + ShippingMethod + ")" : "") + Environment.NewLine;

        if (AdditionalShipping > 0)
            Body += "Additional Shipping: ".PadLeft(w) + string.Format("{0:C}", AdditionalShipping).PadLeft(12) + Environment.NewLine;

        if (VOR > 0)
            Body += "Express Ordering(VOR): ".PadLeft(w) + string.Format("{0:C}", VOR).PadLeft(12) + Environment.NewLine;

        if (CouponsDiscount != 0)
            Body += "Coupons Discounts: -".PadLeft(w) + string.Format("{0:C}", CouponsDiscount).PadLeft(12) + Environment.NewLine;

        if (CouponsPercentageMoney != 0)
            Body += "Coupons % Discounts: -".PadLeft(w) + string.Format("{0:C}", CouponsPercentageMoney).PadLeft(12) + "  (" + CouponsPercentage.ToString() + "%)" + Environment.NewLine;

        if (Coupons.Length > 0)
            Body += "Coupons: ".PadLeft(w) + Coupons.ToUpper() + Environment.NewLine;

        if (WholesaleDiscountMoney != 0.0)
            Body += "Wholesale Discount: -".PadLeft(w) + string.Format("{0:C}", WholesaleDiscountMoney).PadLeft(12) + "  (" + WholesaleDiscount.ToString() + "%)" + Environment.NewLine;

        if (TaxesMoney != 0.0)
            Body += "Taxes: -".PadLeft(w) + string.Format("{0:C}", TaxesMoney).PadLeft(12) + "  (" + Taxes.ToString() + "% - TaxID: " + TaxID + ")" + Environment.NewLine;

        Body += "-----------------------------------------------------------------------------------------------" + Environment.NewLine;
        Body += "TOTAL: ".PadLeft(w) + string.Format("{0:C}", Total).PadLeft(12) + Environment.NewLine;


        Body += Environment.NewLine;
        Body += Environment.NewLine;
        Body += Environment.NewLine;
        Body += "ADDITIONAL INFO:" + Environment.NewLine;
        Body += "--------------------------------------------------------------------------------" + Environment.NewLine;
        Body += VehicleInfo + Environment.NewLine;
        Body += Comments + Environment.NewLine;

        Body = Body.Replace(" ", "&nbsp;");
        Body = Body.Replace(Environment.NewLine, "<br />");

        return Body;
    }

    private string GetCardType(string CardType)
    {
        string txt = CardType;
        switch (CardType)
        {
            case "V":
                txt = "VISA";
                break;
            case "M":
                txt = "MASTERCARD";
                break;
            case "D":
                txt = "DISCOVER";
                break;
            case "A":
                txt = "AMEX";
                break;
        }
        return txt;
    }

    public int SaveDB_Order()
    {
        int OrderID = 0;

        OleDbConnection conn = new OleDbConnection(ConfigurationManager.AppSettings["connString"]);
        conn.Open();

        OleDbCommand cmd = new OleDbCommand();
        cmd.Connection = conn;

        string qry = @"INSERT INTO CartOrders ( IPAddress, SessionID, UserID, eMail, DatePurchased, Coupons, FirstName, MiddleName, LastName, Address, Address2, City, State, Country, ZIP, Phone, Phone2, Phone3, 
                                                PaymentType, CardNameOnCard, CardType, CardNumberLast4, CardAddress, CardAddress2, CardZIP, CardCity, CardState, CardCountry, CardPhone, 
                                                VehicleInfo, Comments, 
                                                TotalWeight, SubTotal, ShippingMethod, Shipping, AdditionalShipping, VOR, CouponsDiscount, CouponsPercentage, CouponsPercentageMoney, WholesaleDiscount, WholesaleDiscountMoney, TaxID, Taxes, TaxesMoney, Total, EmployeeID                                  
                                    ) VALUES ( ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?,  
                                               ?, ?, ?, ?, ?, ?, ?, ?, 
                                               ?, ?, 
                                               ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?); SELECT SCOPE_IDENTITY();";

        cmd.CommandText = qry;

        int UID = 0;
        if (UserID != "NONE" && UserID!="")
            UID = Convert.ToInt32(UserID);

        cmd.Parameters.Clear();
        cmd.Parameters.AddWithValue("@IPAddress", IPAddress);
        cmd.Parameters.AddWithValue("@SessionID", SessionID);
        cmd.Parameters.AddWithValue("@UserID", UID);
        cmd.Parameters.AddWithValue("@eMail", eMail);
        cmd.Parameters.AddWithValue("@DatePurchased", DateTime.Now);
        cmd.Parameters.AddWithValue("@Coupons", (Coupons.Length <= 50 ? Coupons : Coupons.Substring(0, 50)));
        cmd.Parameters.AddWithValue("@FirstName", FirstName);
        cmd.Parameters.AddWithValue("@MiddleName", (MiddleName.Length <= 1 ? MiddleName : MiddleName.Substring(0, 1)));
        cmd.Parameters.AddWithValue("@LastName", LastName);
        cmd.Parameters.AddWithValue("@Address", Address);
        cmd.Parameters.AddWithValue("@Address2", Address2);
        cmd.Parameters.AddWithValue("@City", City);
        cmd.Parameters.AddWithValue("@State", State);
        cmd.Parameters.AddWithValue("@Country", Country);
        cmd.Parameters.AddWithValue("@ZIP", ZIP);
        cmd.Parameters.AddWithValue("@Phone", (Phone.Length <= 15 ? Phone : Phone.Substring(0, 15)));
        cmd.Parameters.AddWithValue("@Phone2", (Phone2.Length <= 15 ? Phone2 : Phone2.Substring(0, 15)));
        cmd.Parameters.AddWithValue("@Phone3", (Phone3.Length <= 15 ? Phone3 : Phone3.Substring(0, 15)));
        cmd.Parameters.AddWithValue("@PaymentType", PaymentType);
        cmd.Parameters.AddWithValue("@CardNameOnCard", CardFirstName + " " + CardMiddleName + " " + CardLastName);
        cmd.Parameters.AddWithValue("@CardType", CardType);
        cmd.Parameters.AddWithValue("@CardNumberLast4", CardLast4);
        cmd.Parameters.AddWithValue("@CardAddress", CardAddress);
        cmd.Parameters.AddWithValue("@CardAddress2", CardAddress2);
        cmd.Parameters.AddWithValue("@CardZIP", CardZIP);
        cmd.Parameters.AddWithValue("@CardCity", CardCity);
        cmd.Parameters.AddWithValue("@CardState", CardState);
        cmd.Parameters.AddWithValue("@CardCountry", CardCountry);
        cmd.Parameters.AddWithValue("@CardPhone", CardPhone);
        cmd.Parameters.AddWithValue("@VehicleInfo", VehicleInfo);
        cmd.Parameters.AddWithValue("@Comments", Comments);
        cmd.Parameters.AddWithValue("@TotalWeight", TotalWeight);
        cmd.Parameters.AddWithValue("@SubTotal", SubTotal);
        cmd.Parameters.AddWithValue("@ShippingMethod", ShippingMethod);
        cmd.Parameters.AddWithValue("@Shipping", Shipping);
        cmd.Parameters.AddWithValue("@AdditionalShipping", AdditionalShipping);
        cmd.Parameters.AddWithValue("@VOR", VOR);
        cmd.Parameters.AddWithValue("@CouponsDiscount", CouponsDiscount);
        cmd.Parameters.AddWithValue("@CouponsPercentage", CouponsPercentage);
        cmd.Parameters.AddWithValue("@CouponsPercentageMoney", CouponsPercentageMoney);
        cmd.Parameters.AddWithValue("@WholesaleDiscount", WholesaleDiscount);
        cmd.Parameters.AddWithValue("@WholesaleDiscountMoney", WholesaleDiscountMoney);
        cmd.Parameters.AddWithValue("@TaxID", TaxID);
        cmd.Parameters.AddWithValue("@Taxes", Taxes);
        cmd.Parameters.AddWithValue("@TaxesMoney", TaxesMoney);
        cmd.Parameters.AddWithValue("@Total", Total);
        cmd.Parameters.AddWithValue("@EmployeeID", 3);
        try
        {
            OrderID = Convert.ToInt32(cmd.ExecuteScalar());
        }
        catch (Exception e)
        {
            Auditing.InsertError(e);
        }

        cmd.Dispose();
        conn.Close();
        conn.Dispose();

        return OrderID;
    }

    public void SaveDB_OrderItems(CartItemCollection myCartItems)
    {
        int ID = 0;
        string qry;

        if (myCartItems != null && myCartItems.Count > 0)
        {
            OleDbConnection conn = new OleDbConnection(ConfigurationManager.AppSettings["connString"]);
            conn.Open();
            OleDbCommand cmd = new OleDbCommand();
            cmd.Connection = conn;


            foreach (CartItem cartItem in myCartItems)
            {
                qry = @"INSERT INTO CartOrdersDetails ( OrderID, PartNumber, Description, Qty, UnitPrice, Weight, Dimensions, AdditionalShipping, CommentsShipping, IsSpecial, SpecialsText) " +
                              "VALUES ( ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?); SELECT SCOPE_IDENTITY();";

                cmd.CommandText = qry;

                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@OrderID", OrderID);
                cmd.Parameters.AddWithValue("@PartNumber", cartItem.PartNumber);
                cmd.Parameters.AddWithValue("@Description", cartItem.Description);
                cmd.Parameters.AddWithValue("@Qty", cartItem.Quantity);
                cmd.Parameters.AddWithValue("@UnitPrice", cartItem.Price);
                cmd.Parameters.AddWithValue("@Weight", cartItem.Weight);
                cmd.Parameters.AddWithValue("@Dimensions", cartItem.Dimensions);
                cmd.Parameters.AddWithValue("@AdditionalShipping", cartItem.AdditionalShipping);
                cmd.Parameters.AddWithValue("@CommentsShipping", cartItem.CommentsShipping);
                cmd.Parameters.AddWithValue("@IsSpecial", cartItem.IsSpecial);
                cmd.Parameters.AddWithValue("@SpecialsText", (cartItem.SpecialsText2 == "" ? cartItem.SpecialsText : cartItem.SpecialsText2));

                try
                {
                    ID = Convert.ToInt32(cmd.ExecuteScalar());
                }
                catch (Exception e)
                {
                    Auditing.InsertError(e);
                }
            }
            cmd.Dispose();
            conn.Close();
            conn.Dispose();
        }
    }

    public void UpdateDB_Order()
    {
        OleDbConnection conn = new OleDbConnection(ConfigurationManager.AppSettings["connString"]);
        conn.Open();
        OleDbCommand cmd = new OleDbCommand();
        cmd.Connection = conn;

        string qry = @"UPDATE CartOrders SET TransactionID=?, TransactionResponse=?, TransactionCode=?, TransactionApproved=?, TransactionAVSZIP=?, TransactionAVSADDR=?, TransactionCVV=?, Status='CC' WHERE OrderID=?";

        cmd.CommandText = qry;

        cmd.Parameters.Clear();
        cmd.Parameters.AddWithValue("@TransactionID", TransactionID);
        cmd.Parameters.AddWithValue("@TransactionResponse", TransactionResponse);
        cmd.Parameters.AddWithValue("@TransactionCode", TransactionCode);
        cmd.Parameters.AddWithValue("@TransactionApproved", TransactionApproved);
        cmd.Parameters.AddWithValue("@TransactionAVSZIP", TransactionAVSZIP);
        cmd.Parameters.AddWithValue("@TransactionAVSADDR", TransactionAVSADDR);
        cmd.Parameters.AddWithValue("@TransactionCVV", TransactionCVV);
        cmd.Parameters.AddWithValue("@OrderID", OrderID);

        try
        {
            cmd.ExecuteNonQuery();
        }
        catch (Exception e)
        {
            Auditing.InsertError(e);
        }

        cmd.Dispose();

        conn.Close();
        conn.Dispose();

    }
}

public class Authorization
{
    public static void CheckForAuthorizedEmployee()
    {
        CheckForHTTPS();

        //if (HttpContext.Current.Request.Url.Host != "localhost")
        //{
            if (HttpContext.Current.Session["EmployeeID"] == null)
                HttpContext.Current.Response.Redirect("EmployeeLogin.aspx");

            if (HttpContext.Current.Session["EmployeeID"].ToString().Length == 0)
                HttpContext.Current.Response.Redirect("EmployeeLogin.aspx");

            if (HttpContext.Current.Session["EmployeeAccessRights"] == null)
                HttpContext.Current.Response.Redirect("EmployeeLogin.aspx");

            if (HttpContext.Current.Session["EmployeeAccessRights"].ToString().Length == 0)
                HttpContext.Current.Response.Redirect("EmployeeLogin.aspx");
        //}
    }

    public static void CheckForValidIPs()
    {
    }

    public static void CheckForHTTPS()
    {
        if (HttpContext.Current.Request.Url.Host != "localhost" && HttpContext.Current.Request.Url.Scheme == "http")
            HttpContext.Current.Response.Redirect(HttpContext.Current.Request.Url.AbsoluteUri.Replace("http://", "https://"));
    }
}

public class SimpleHash
{
    public static string ComputeHash512(string plainText, byte[] saltBytes)
    {        
        // If salt is not specified, generate it on the fly.
        if (saltBytes == null)
        {
            // Define min and max salt sizes.
            int minSaltSize = 4;
            int maxSaltSize = 8;
            // Generate a random number for the size of the salt.
            Random random = new Random();
            int saltSize = random.Next(minSaltSize, maxSaltSize);
            // Allocate a byte array, which will hold the salt.
            saltBytes = new byte[saltSize];
            // Initialize a random number generator.
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            // Fill the salt with cryptographically strong byte values.
            rng.GetNonZeroBytes(saltBytes);
        }

        // Convert plain text into a byte array.
        byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);

        // Allocate array, which will hold plain text and salt.
        byte[] plainTextWithSaltBytes =
                new byte[plainTextBytes.Length + saltBytes.Length];

        // Copy plain text bytes into resulting array.
        for (int i = 0; i < plainTextBytes.Length; i++)
            plainTextWithSaltBytes[i] = plainTextBytes[i];

        // Append salt bytes to the resulting array.
        for (int i = 0; i < saltBytes.Length; i++)
            plainTextWithSaltBytes[plainTextBytes.Length + i] = saltBytes[i];

        HashAlgorithm hash = new SHA512Managed();
                
        // Compute hash value of our plain text with appended salt.
        byte[] hashBytes = hash.ComputeHash(plainTextWithSaltBytes);

        // Create array which will hold hash and original salt bytes.
        byte[] hashWithSaltBytes = new byte[hashBytes.Length +
                                            saltBytes.Length];

        // Copy hash bytes into resulting array.
        for (int i = 0; i < hashBytes.Length; i++)
            hashWithSaltBytes[i] = hashBytes[i];

        // Append salt bytes to the result.
        for (int i = 0; i < saltBytes.Length; i++)
            hashWithSaltBytes[hashBytes.Length + i] = saltBytes[i];

        // Convert result into a base64-encoded string.
        string hashValue = Convert.ToBase64String(hashWithSaltBytes);

        // Return the result.
        return hashValue;
    }

    
    public static bool VerifyHash512(string plainText, string hashValue)
    {
        // Convert base64-encoded hash value into a byte array.
        byte[] hashWithSaltBytes = Convert.FromBase64String(hashValue);

        // We must know size of hash (without salt).
        int hashSizeInBits, hashSizeInBytes;

        hashSizeInBits = 512;        

        // Convert size of hash from bits to bytes.
        hashSizeInBytes = hashSizeInBits / 8;

        // Make sure that the specified hash value is long enough.
        if (hashWithSaltBytes.Length < hashSizeInBytes)
            return false;

        // Allocate array to hold original salt bytes retrieved from hash.
        byte[] saltBytes = new byte[hashWithSaltBytes.Length - hashSizeInBytes];

        // Copy salt from the end of the hash to the new array.
        for (int i = 0; i < saltBytes.Length; i++)
            saltBytes[i] = hashWithSaltBytes[hashSizeInBytes + i];

        // Compute a new hash string.
        string expectedHashString = ComputeHash512(plainText, saltBytes);

        // If the computed hash matches the specified hash,
        // the plain text value must be correct.
        return (hashValue == expectedHashString);
    }
}

public class EmailSender
{
    public static bool SendEmail(string Recepients, string FromEmail, string FromName, string CC, string BCC, string Subject, string Body)
    {
        bool Result = false;

        MailMessage msg = new MailMessage();

        MailAddress From = new MailAddress(FromEmail, FromName);
        msg.From = From;
        
        string [] ArrRecepients = Recepients.Split(',');
        foreach (string email in ArrRecepients)
        {
            if (email.Length > 0)
            {
                MailAddress t = new MailAddress(email);
                msg.To.Add(t);
            }
        }

        string[] ArrCC = CC.Split(',');
        foreach (string email in ArrCC)
        {
            if (email.Length > 0)
            {
                MailAddress t = new MailAddress(email);
                msg.CC.Add(t);
            }
        }

        string[] ArrBCC = BCC.Split(',');
        foreach (string email in ArrBCC)
        {
            if (email.Length > 0)
            {
                MailAddress t = new MailAddress(email);
                msg.Bcc.Add(t);
            }
        }


        msg.IsBodyHtml = true;
        msg.BodyEncoding = System.Text.Encoding.UTF8;
        msg.SubjectEncoding = System.Text.Encoding.UTF8;
        msg.Priority = MailPriority.Normal;
        msg.Subject = Subject;
        msg.Body = "<html><body style=\"font-family: Courier New; font-size: 12px;\">" + Body + "</body></html>";
               
        // Albert's configuration
        // SMTP Server:  mail.smtp2go.com
        // SMTP Port: 2525
        // Alternative ports: 8025, 587, 80 or 25.TLS is available on the same ports.
        // SSL is available on ports 465, 8465 and 443.

        // original TidalMediaGroup configuration
        //smtpClient = new SmtpClient("email.tidalhosting.com");
        //smtpClient.EnableSsl = false;
        //smtpClient.UseDefaultCredentials = true;

        SmtpClient smtpClient = new SmtpClient("mail.smtp2go.com");
        smtpClient.Port = 2525;
        smtpClient.EnableSsl = false;
        smtpClient.UseDefaultCredentials = true;

        try
        {
            if (!HttpContext.Current.Request.Url.AbsoluteUri.StartsWith("http://localhost"))
            {
                smtpClient.Send(msg);
            }
            Result = true;
        }
        catch (Exception ex)
        {
            Auditing.InsertError(ex);
            Auditing.InsertComment("Email Body=" + Body + "\nRecipients=" + Recepients + "\nSubject=" + Subject + "\nFromEmail=" + FromEmail);
            Result = false;
        }
        return Result;
    }
}

public class RandomPassword
{
    // Define default min and max password lengths.
    private static int DEFAULT_MIN_PASSWORD_LENGTH = 6;
    private static int DEFAULT_MAX_PASSWORD_LENGTH = 6;

    // Define supported password characters divided into groups.
    // You can add (or remove) characters to (from) these groups.
    private static string PASSWORD_CHARS_LCASE = "abcdefgijkmnopqrstwxyz";
    private static string PASSWORD_CHARS_UCASE = "ABCDEFGHJKLMNPQRSTWXYZ";
    private static string PASSWORD_CHARS_NUMERIC = "23456789";
    //private static string PASSWORD_CHARS_SPECIAL = "*$-+?_&=!%{}/";
    private static string PASSWORD_CHARS_SPECIAL = "$!%";
        
    public static string Generate()
    {
        return Generate(DEFAULT_MIN_PASSWORD_LENGTH,
                        DEFAULT_MAX_PASSWORD_LENGTH);
    }
        
    public static string Generate(int length)
    {
        return Generate(length, length);
    }
    
    public static string Generate(int minLength,
                                  int maxLength)
    {
        // Make sure that input parameters are valid.
        if (minLength <= 0 || maxLength <= 0 || minLength > maxLength)
            return null;

        // Create a local array containing supported password characters
        // grouped by types. You can remove character groups from this
        // array, but doing so will weaken the password strength.
        char[][] charGroups = new char[][] 
        {
            PASSWORD_CHARS_LCASE.ToCharArray(),
            PASSWORD_CHARS_UCASE.ToCharArray(),
            PASSWORD_CHARS_NUMERIC.ToCharArray(),
            PASSWORD_CHARS_SPECIAL.ToCharArray()
        };

        // Use this array to track the number of unused characters in each
        // character group.
        int[] charsLeftInGroup = new int[charGroups.Length];

        // Initially, all characters in each group are not used.
        for (int i = 0; i < charsLeftInGroup.Length; i++)
            charsLeftInGroup[i] = charGroups[i].Length;

        // Use this array to track (iterate through) unused character groups.
        int[] leftGroupsOrder = new int[charGroups.Length];

        // Initially, all character groups are not used.
        for (int i = 0; i < leftGroupsOrder.Length; i++)
            leftGroupsOrder[i] = i;

        // Because we cannot use the default randomizer, which is based on the
        // current time (it will produce the same "random" number within a
        // second), we will use a random number generator to seed the
        // randomizer.

        // Use a 4-byte array to fill it with random bytes and convert it then
        // to an integer value.
        byte[] randomBytes = new byte[4];

        // Generate 4 random bytes.
        RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
        rng.GetBytes(randomBytes);

        // Convert 4 bytes into a 32-bit integer value.
        int seed = (randomBytes[0] & 0x7f) << 24 |
                    randomBytes[1] << 16 |
                    randomBytes[2] << 8 |
                    randomBytes[3];

        // Now, this is real randomization.
        Random random = new Random(seed);

        // This array will hold password characters.
        char[] password = null;

        // Allocate appropriate memory for the password.
        if (minLength < maxLength)
            password = new char[random.Next(minLength, maxLength + 1)];
        else
            password = new char[minLength];

        // Index of the next character to be added to password.
        int nextCharIdx;

        // Index of the next character group to be processed.
        int nextGroupIdx;

        // Index which will be used to track not processed character groups.
        int nextLeftGroupsOrderIdx;

        // Index of the last non-processed character in a group.
        int lastCharIdx;

        // Index of the last non-processed group.
        int lastLeftGroupsOrderIdx = leftGroupsOrder.Length - 1;

        // Generate password characters one at a time.
        for (int i = 0; i < password.Length; i++)
        {
            // If only one character group remained unprocessed, process it;
            // otherwise, pick a random character group from the unprocessed
            // group list. To allow a special character to appear in the
            // first position, increment the second parameter of the Next
            // function call by one, i.e. lastLeftGroupsOrderIdx + 1.
            if (lastLeftGroupsOrderIdx == 0)
                nextLeftGroupsOrderIdx = 0;
            else
                nextLeftGroupsOrderIdx = random.Next(0,
                                                     lastLeftGroupsOrderIdx);

            // Get the actual index of the character group, from which we will
            // pick the next character.
            nextGroupIdx = leftGroupsOrder[nextLeftGroupsOrderIdx];

            // Get the index of the last unprocessed characters in this group.
            lastCharIdx = charsLeftInGroup[nextGroupIdx] - 1;

            // If only one unprocessed character is left, pick it; otherwise,
            // get a random character from the unused character list.
            if (lastCharIdx == 0)
                nextCharIdx = 0;
            else
                nextCharIdx = random.Next(0, lastCharIdx + 1);

            // Add this character to the password.
            password[i] = charGroups[nextGroupIdx][nextCharIdx];

            // If we processed the last character in this group, start over.
            if (lastCharIdx == 0)
                charsLeftInGroup[nextGroupIdx] =
                                          charGroups[nextGroupIdx].Length;
            // There are more unprocessed characters left.
            else
            {
                // Swap processed character with the last unprocessed character
                // so that we don't pick it until we process all characters in
                // this group.
                if (lastCharIdx != nextCharIdx)
                {
                    char temp = charGroups[nextGroupIdx][lastCharIdx];
                    charGroups[nextGroupIdx][lastCharIdx] =
                                charGroups[nextGroupIdx][nextCharIdx];
                    charGroups[nextGroupIdx][nextCharIdx] = temp;
                }
                // Decrement the number of unprocessed characters in
                // this group.
                charsLeftInGroup[nextGroupIdx]--;
            }

            // If we processed the last group, start all over.
            if (lastLeftGroupsOrderIdx == 0)
                lastLeftGroupsOrderIdx = leftGroupsOrder.Length - 1;
            // There are more unprocessed groups left.
            else
            {
                // Swap processed group with the last unprocessed group
                // so that we don't pick it until we process all groups.
                if (lastLeftGroupsOrderIdx != nextLeftGroupsOrderIdx)
                {
                    int temp = leftGroupsOrder[lastLeftGroupsOrderIdx];
                    leftGroupsOrder[lastLeftGroupsOrderIdx] =
                                leftGroupsOrder[nextLeftGroupsOrderIdx];
                    leftGroupsOrder[nextLeftGroupsOrderIdx] = temp;
                }
                // Decrement the number of unprocessed groups.
                lastLeftGroupsOrderIdx--;
            }
        }

        // Convert password characters into a string and return the result.
        return new string(password);
    }    
}

public class Imaging
{
    public static void ResizeImageFile(int MaxWidth, int MaxHeight, string SourceImageLocation, string TargetImageLocation)
    {
        // Create BitMap of the original photo.
        Bitmap fullSizeImg = new Bitmap(SourceImageLocation);

        double DesiredAR = (double)MaxWidth / (double)MaxHeight;
        int NewWidth;
        int NewHeight;

        // set the width and the height of the original photo
        int width = fullSizeImg.Width;
        int height = fullSizeImg.Height;
        double ar = (double)width / (double)height; // aspect ratio

        if (ar > DesiredAR)
        {
            // limit on width
            NewWidth = MaxWidth;
            NewHeight = (int)((double)height * ((double)NewWidth / (double)width));
        }
        else if (ar < DesiredAR)
        {
            // limit on height
            NewHeight = MaxHeight;
            NewWidth = (int)((double)width * ((double)NewHeight / (double)height));
        }
        else
        {
            NewWidth = MaxWidth;
            NewHeight = MaxHeight;
        }


        // Encoder parameter for image quality
        EncoderParameter qualityParam = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 100L);

        // jpeg image codec
        ImageCodecInfo codecInfo = GetEncoderInfo("image/jpeg");
        EncoderParameters encoderParams = new EncoderParameters(1);
        encoderParams.Param[0] = qualityParam;

        // create thumbnail bitmap
        Bitmap thumbnailBitmap = new Bitmap(NewWidth, NewHeight);
        Graphics thumbnailGraph = Graphics.FromImage(thumbnailBitmap);

        thumbnailGraph.CompositingQuality = CompositingQuality.HighQuality;
        thumbnailGraph.SmoothingMode = SmoothingMode.HighQuality;
        thumbnailGraph.InterpolationMode = InterpolationMode.HighQualityBicubic;

        thumbnailGraph.DrawImage(fullSizeImg, 0, 0, NewWidth, NewHeight);
        thumbnailBitmap.Save(TargetImageLocation, fullSizeImg.RawFormat);

        //Clean up / Dispose...
        fullSizeImg.Dispose();
        thumbnailGraph.Dispose();
        thumbnailBitmap.Dispose();
    }

    private static ImageCodecInfo GetEncoderInfo(string mimeType)
    {
        ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();
        for (int i = 0; i < codecs.Length; i++)
        {
            if (codecs[i].MimeType == mimeType)
            {
                return codecs[i];
            }
        }
        return null;
    }
}