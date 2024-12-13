using System;
using System.Configuration;
using PayPal.Payments.Common.Utility;
using PayPal.Payments.Communication;

public partial class CheckoutCard : System.Web.UI.Page
{
    protected Checkout c;
    protected CartHelper cartHelper;

    protected void Page_Load(object sender, EventArgs e)
    { 
        // if there are no items in the cart, then leave this page!
        CartItemCollection myCartItems = (CartItemCollection)Session["CartItems"];
        if (myCartItems == null || myCartItems.Count == 0)
        {
            Response.Redirect(Utilities.URL_FICHE(true) + "/CheckoutProcessed.aspx");
        }
                
        Utilities.SetLastLoginURL();

        cartHelper = new CartHelper();
        c = new Checkout();
        //c = (Checkout)Session["checkout"];

        if (Session["UserID"] == null)
            c.UserID = "NONE";
        else
            c.UserID = Session["UserID"].ToString();

        c.eMail = Utilities.SForm("EMAIL");
        c.Coupons = Utilities.SForm("COUPONS");
        c.FirstName = Utilities.SForm("FIRSTNAME").ToUpper();
        c.MiddleName = Utilities.SForm("MIDDLENAME").ToUpper();
        c.LastName = Utilities.SForm("LASTNAME").ToUpper();
        c.Address = Utilities.SForm("ADDRESS").ToUpper();
        c.Address2 = Utilities.SForm("ADDRESS2").ToUpper();
        c.City = Utilities.SForm("CITY").ToUpper();
        c.State = Utilities.SForm("STATE");
        c.Country = Utilities.SForm("COUNTRY");
        c.ZIP = Utilities.SForm("ZIP").ToUpper();
        c.Phone = Utilities.SForm("PHONE").ToUpper();
        c.Phone2 = Utilities.SForm("PHONE2").ToUpper();
        c.Phone3 = Utilities.SForm("PHONE3").ToUpper();

        c.PaymentType = Utilities.SForm("PAYMENTTYPE");

        c.CardCountry = Utilities.SForm("CARDCOUNTRY").ToUpper();
        if (c.CardCountry == "US")
            c.CardState = Utilities.SForm("CARDSTATE_US").ToUpper();
        else if (c.CardCountry == "CA")
            c.CardState = Utilities.SForm("CARDSTATE_CA").ToUpper();
        else
            c.CardState = "";

        // needed for PayPal PayFlowPro
        c.CardFirstName = Utilities.SForm("CARDFIRSTNAME").ToUpper();
        c.CardMiddleName = Utilities.SForm("CARDMIDDLENAME").ToUpper();
        c.CardLastName = Utilities.SForm("CARDLASTNAME").ToUpper();
        c.CardType = Utilities.SForm("CARDTYPE");
        c.CardNumber = Utilities.SForm("CARDNUMBER");
        c.CardExpMonth = Utilities.SForm("CARDEXPMONTH");
        c.CardExpYear = Utilities.SForm("CARDEXPYEAR");
        c.CardCVV2 = Utilities.SForm("CARDCVV2");
        c.CardCountry = Utilities.SForm("CARDCOUNTRY").ToUpper();
        c.CardAddress = Utilities.SForm("CARDADDRESS").ToUpper();
        c.CardAddress2 = Utilities.SForm("CARDADDRESS2").ToUpper();
        c.CardCity = Utilities.SForm("CARDCITY").ToUpper();
        c.CardZIP = Utilities.SForm("CARDZIP").ToUpper();
        c.CardPhone = Utilities.SForm("CARDPHONE").ToUpper();

        c.VehicleInfo = Utilities.SForm("VEHICLEINFO").ToUpper();
        c.Comments = Utilities.SForm("COMMENTS");
        c.SignUp = (Utilities.SForm("SIGNUP") == "on" ? true : false);
        c.ShippingMethod = Utilities.SForm("SHIPPINGMETHOD");
        c.TotalWeight = Utilities.DForm("TotalWeight");
        c.SubTotal = Utilities.DForm("SummarySubTotal");
        c.Shipping = Utilities.DForm("SummaryShipping");
        c.AdditionalShipping = Utilities.DForm("SummaryAdditionalShipping");
        c.VOR = Utilities.DForm("SummaryVOR");
        c.CouponsDiscount = Utilities.DForm("SummaryCouponsDiscount");
        c.CouponsPercentage = Utilities.DForm("SummaryCouponsPercentage");
        c.CouponsPercentageMoney = Utilities.DForm("SummaryCouponsPercentageMoney");
        c.WholesaleDiscount = Convert.ToInt32(Utilities.DForm("SummaryWholesaleDiscount")); //wholesale
        c.WholesaleDiscountMoney = Utilities.DForm("SummaryWholesaleDiscountMoney"); //wholesale
        c.TaxID = Utilities.SForm("SummaryTaxID");
        c.Taxes = Utilities.DForm("SummaryTaxes");
        c.TaxesMoney = Utilities.DForm("SummaryTaxesMoney");
        c.Total = Utilities.DForm("SummaryTotal");
        
         
        c.OrderID = c.SaveDB_Order();
        c.SaveDB_OrderItems((CartItemCollection)Session["CartItems"]);
 
        // Call PayPal PayFlowPro
        c.TransactionCode = PayFlowProProcess(c.OrderID, ref c.TransactionID, ref c.TransactionResponse, ref c.TransactionApproved, ref c.TransactionAVSZIP, ref c.TransactionAVSADDR, ref c.TransactionCVV);

        // Create the body of the email to be sent. This email body is also reused for the HTMLText to be used on this page
        string eMailSubject = "ORDER #" + c.OrderID.ToString() + "  PayFlow# " + c.TransactionID + "  " + c.eMail + " (UserID# " + c.UserID + ")";
        string eMailBody = c.CreateEmailBody();

        // Send an email to store (Devlin, Francis, Russ, etc)
        bool eMailSentToStore = EmailSender.SendEmail(ConfigurationManager.AppSettings["EmailOrders"], ConfigurationManager.AppSettings["EmailOrders"], "MAX BMW - Orders", "", ConfigurationManager.AppSettings["EmailOrdersBCC"], eMailSubject, eMailBody);

        // Send an email copy to the customer
        bool eMailSentToCustomer = false;
        string CopyEMailBody = "";
        string CopyEMailSubject = "Your MAX BMW ORDER #" + c.OrderID.ToString() + (c.PaymentType == "P" ? "  PAYPAL" : "  PayFlow# " + c.TransactionID);

        if (c.TransactionApproved && c.TransactionCode == 0)
        {
            CopyEMailBody += "PLEASE DO NOT REPLY TO THIS EMAIL, IT IS AN AUTOMATED EMAIL.<br /><br />";

            CopyEMailBody += "Thank you for your order!<br /><br />";
            CopyEMailBody += "A PARTS SALES PERSON WILL EMAIL YOU THE NEXT BUSINESS DAY WITH THE STATUS OF YOUR ORDER.<br /><br />";
            CopyEMailBody += "<br /><br /><br />";
            CopyEMailBody += eMailBody + "<br /><br /><br />";
            CopyEMailBody += "If you do not receive your email confirmation in your Inbox, please check your SPAM or JUNK folders before contacting us.<br /><br />";

        }

        eMailSentToCustomer = EmailSender.SendEmail(c.eMail, ConfigurationManager.AppSettings["EmailOrders"], "MAX BMW - " + ConfigurationManager.AppSettings["EmailNameOrders"], "", ConfigurationManager.AppSettings["EmailOrdersBCC"], CopyEMailSubject, CopyEMailBody);

        // Create the HTML text that will be displayed in the CheckoutComplete page
        string HTMLResponse = "";
        if (c.TransactionApproved && c.TransactionCode == 0)
        {
            HTMLResponse += "&nbsp;&nbsp;&nbsp;&nbsp;<span class=\"Button\" onclick=\"window.print();\">&nbsp;&nbsp;Print Order&nbsp;&nbsp;</span>";
            HTMLResponse += "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class=\"Button\" onclick=\"window.location='" + Utilities.URL_FICHE(false) + "/fiche.aspx';\">&nbsp;&nbsp;&nbsp;&nbsp;OK&nbsp;&nbsp;&nbsp;&nbsp;</span>";
            HTMLResponse += "<br />";
            HTMLResponse += "<p style=\"font-weight: bold;\">Thank you for your order!</p><br />";
            HTMLResponse += "<p style=\"font-weight: bold;\">You will be receiving an automated email from no-reply@maxbmw.com shortly. Please do not reply to this email. Please check your spam or junk folder if you do not receive it.</p><br /><br />";
            HTMLResponse += "<p style=\"font-weight: bold;\">IN THE NEXT BUSINESS DAY A PARTS SALES PERSON WILL EMAIL YOU WITH THE STATUS OF YOUR ORDER.</p><br />";
            HTMLResponse += "<p style=\"font-family: Courier New; font-size: 11px; font-weight: normal;\">";
            HTMLResponse += eMailBody;
            HTMLResponse += "</p>";
        }
        else
        {
            HTMLResponse += "&nbsp;&nbsp;&nbsp;&nbsp;<span class=\"Button\" onclick=\"window.print();\">&nbsp;&nbsp;Print Page&nbsp;&nbsp;</span>";
            HTMLResponse += "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class=\"Button\" onclick=\"history.go(-1)\">&nbsp;&nbsp;<< Edit Billing Information&nbsp;&nbsp;</span>";
            HTMLResponse += "<br />";
            HTMLResponse += "<p style=\"color: #e00; font-weight: bold;\">Your Credit Card was DECLINED or something went wrong processing it.<br />";

            HTMLResponse += "PayFlow ERROR: " + c.TransactionCode.ToString();
            HTMLResponse += "</p><br />";
            HTMLResponse += "<p style=\"font-family: Courier New; font-size: 11px; font-weight: normal;\">";
            HTMLResponse += eMailBody;
            HTMLResponse += "</p>";
        }

        // update status of Paypal and emails sent on DB        
        c.UpdateDB_Order();

        // clear the shopping cart items
        cartHelper.Clear();
        Session.Remove("CartItems");

        Session["PayFlowResponse"] = HTMLResponse;

        Response.Redirect(Utilities.URL_FICHE(true) + "/CheckoutProcessed.aspx");
    }


    private void SendOrderEmails()
    {
        // Create the body of the email to be sent. This email body is also reused for the HTMLText to be used on this page
        string eMailSubject = "ORDER #" + c.OrderID.ToString() + "  PayFlow# " + c.TransactionID + "  " + c.eMail + " (UserID# " + c.UserID + ")";
        string eMailBody = c.CreateEmailBody();


        // Send an email to store (Devlin, Francis, Russ, etc)
        bool eMailSentToStore = EmailSender.SendEmail(ConfigurationManager.AppSettings["EmailOrders"], ConfigurationManager.AppSettings["EmailOrders"], "MAX BMW - Orders", "", ConfigurationManager.AppSettings["EmailOrdersBCC"], eMailSubject, eMailBody);


        // Send an email copy to the customer
        bool eMailSentToCustomer = false;
        string CopyEMailBody = "";
        string CopyEMailSubject = "Your MAX BMW ORDER #" + c.OrderID.ToString() + (c.PaymentType == "P" ? "  PAYPAL" : "  PayFlow# " + c.TransactionID);

        if (c.TransactionApproved && c.TransactionCode == 0)
        {
            CopyEMailBody += "PLEASE DO NOT REPLY TO THIS EMAIL, IT IS AN AUTOMATED EMAIL.<br /><br />";

            CopyEMailBody += "Thank you for your order!<br /><br />";
            CopyEMailBody += "A PARTS SALES PERSON WILL EMAIL YOU THE NEXT BUSINESS DAY WITH THE STATUS OF YOUR ORDER.<br /><br />";
            CopyEMailBody += "<br /><br /><br />";
            CopyEMailBody += eMailBody + "<br /><br /><br />";
            CopyEMailBody += "If you do not receive your email confirmation in your Inbox, please check your SPAM or JUNK folders before contacting us.<br /><br />";

        }

        eMailSentToCustomer = EmailSender.SendEmail(c.eMail, ConfigurationManager.AppSettings["EmailOrders"], "MAX BMW - " + ConfigurationManager.AppSettings["EmailNameOrders"], "", ConfigurationManager.AppSettings["EmailOrdersBCC"], CopyEMailSubject, CopyEMailBody);

        // Create the HTML text that will be displayed in the CheckoutComplete page
        string HTMLResponse = "";
        if (c.TransactionApproved && c.TransactionCode == 0)
        {
            HTMLResponse += "&nbsp;&nbsp;&nbsp;&nbsp;<span class=\"Button\" onclick=\"window.print();\">&nbsp;&nbsp;Print Order&nbsp;&nbsp;</span>";
            HTMLResponse += "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class=\"Button\" onclick=\"window.location='" + Utilities.URL_FICHE(false) + "/fiche.aspx';\">&nbsp;&nbsp;&nbsp;&nbsp;OK&nbsp;&nbsp;&nbsp;&nbsp;</span>";
            HTMLResponse += "<br />";
            HTMLResponse += "<p style=\"font-weight: bold;\">Thank you for your order!</p><br />";
            HTMLResponse += "<p style=\"font-weight: bold;\">You will be receiving an automated email from no-reply@maxbmw.com shortly. Please do not reply to this email. Please check your spam or junk folder if you do not receive it.</p><br /><br />";
            HTMLResponse += "<p style=\"font-weight: bold;\">IN THE NEXT BUSINESS DAY A PARTS SALES PERSON WILL EMAIL YOU WITH THE STATUS OF YOUR ORDER.</p><br />";
            HTMLResponse += "<p style=\"font-family: Courier New; font-size: 11px; font-weight: normal;\">";
            HTMLResponse += eMailBody;
            HTMLResponse += "</p>";
        }
        else
        {
            HTMLResponse += "&nbsp;&nbsp;&nbsp;&nbsp;<span class=\"Button\" onclick=\"window.print();\">&nbsp;&nbsp;Print Page&nbsp;&nbsp;</span>";
            HTMLResponse += "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class=\"Button\" onclick=\"history.go(-1)\">&nbsp;&nbsp;<< Edit Billing Information&nbsp;&nbsp;</span>";
            HTMLResponse += "<br />";
            HTMLResponse += "<p style=\"color: #e00; font-weight: bold;\">Your Credit Card was DECLINED or something went wrong processing it.<br />";

            HTMLResponse += "PayFlow ERROR: " + c.TransactionCode.ToString();
            HTMLResponse += "</p><br />";
            HTMLResponse += "<p style=\"font-family: Courier New; font-size: 11px; font-weight: normal;\">";
            HTMLResponse += eMailBody;
            HTMLResponse += "</p>";
        }

        
    }

    private string CardPhone(string Phone, bool IsInternational)
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


    private int PayFlowProProcess(int OrderID, ref string TransactionID, ref string TransactionResponse, ref bool TransactionApproved, ref string TransactionAVSZIP, ref string TransactionAVSADDR, ref string TransactionCVV)
    {
        int TransactionCode = -1;
        TransactionApproved = false;
        TransactionID = "";
        TransactionResponse = "";
        TransactionAVSZIP = "N";
        TransactionAVSADDR = "N";
        TransactionCVV = "N";

        string Request = "TRXTYPE=S&ACCT=" + c.CardNumber + "&EXPDATE=" + c.CardExpMonth + c.CardExpYear + "&CVV2=" + c.CardCVV2 + "&TENDER=C&INVNUM=INV" + OrderID.ToString() + "&AMT=" + string.Format("{0:0.00}", c.Total) + "&PONUM=PO" + OrderID.ToString() +
                         "&USER=" + ConfigurationManager.AppSettings["PAYFLOW_USER"] + "&VENDOR=" + ConfigurationManager.AppSettings["PAYFLOW_VENDOR"] + "&PARTNER=" + ConfigurationManager.AppSettings["PAYFLOW_PARTNER"] + "&PWD=" + ConfigurationManager.AppSettings["PAYFLOW_PWD"];


        Request += "&" + GeneratePayFlowParam("EMAIL", c.eMail, 60);
        Request += "&" + GeneratePayFlowParam("CUSTIP", c.IPAddress, 15);
        Request += "&" + GeneratePayFlowParam("FIRSTNAME", c.CardFirstName, 30);
        Request += "&" + GeneratePayFlowParam("MIDDLENAME", c.CardMiddleName, 1);
        Request += "&" + GeneratePayFlowParam("LASTNAME", c.CardLastName, 30);
        Request += "&" + GeneratePayFlowParam("ZIP", c.CardZIP, 9);
        Request += "&" + GeneratePayFlowParam("STREET", c.CardAddress, 30);
        Request += "&" + GeneratePayFlowParam("STATE", c.CardState, 2);
        Request += "&" + GeneratePayFlowParam("CITY", c.CardCity, 20);
        Request += "&" + GeneratePayFlowParam("BILLTOCOUNTRY", c.CardCountry, 2);
        Request += "&" + GeneratePayFlowParam("PHONENUM", CardPhone(c.CardPhone, true), 20);
        Request += "&" + GeneratePayFlowParam("SHIPTOFIRSTNAME", c.FirstName, 30);
        Request += "&" + GeneratePayFlowParam("SHIPTOMIDDLENAME", c.MiddleName, 1);
        Request += "&" + GeneratePayFlowParam("SHIPTOLASTNAME", c.LastName, 30);
        Request += "&" + GeneratePayFlowParam("SHIPTOSTREET", c.Address, 30);
        Request += "&" + GeneratePayFlowParam("SHIPTOCITY", c.City, 30);
        Request += "&" + GeneratePayFlowParam("SHIPTOSTATE", c.State, 2);
        Request += "&" + GeneratePayFlowParam("SHIPTOCOUNTRY", c.Country, 2);
        Request += "&" + GeneratePayFlowParam("SHIPTOZIP", c.ZIP, 9);
        Request += "&" + GeneratePayFlowParam("SHIPTOPHONENUM", c.Phone, 10);
        Request += "&" + GeneratePayFlowParam("FREIGHTAMT", string.Format("{0:0.00}", c.Shipping + c.AdditionalShipping), 15);
        Request += "&" + GeneratePayFlowParam("TAXAMT", string.Format("{0:0.00}", c.TaxesMoney), 6);

        try
        {
            PayflowNETAPI payflowNETAPI = new PayflowNETAPI();
            TransactionResponse = payflowNETAPI.SubmitTransaction(Request, PayflowUtility.RequestId);
            PayflowUtility.GetStatus(TransactionResponse);
        }
        catch (Exception e)
        {
            TransactionResponse = "EXCEPTION ERROR (" + e.ToString() + ") :" + e.Message;
            Auditing.InsertError(e);
        }

        // Parse the PayPal PayFlowPro response
        // Example:  RESULT=0&PNREF=V78E1E6F6812&RESPMSG=Approved&AUTHCODE=010101&AVSADDR=X&AVSZIP=X&CVV2MATCH=Y&IAVS=N


        TransactionID = ParsePayPalResponse(TransactionResponse, "PNREF=", "&");
       
        string RESULT = ParsePayPalResponse(TransactionResponse, "RESULT=", "&");
        string RESPMSG = ParsePayPalResponse(TransactionResponse, "RESPMSG=", "&");

        TransactionCode = Convert.ToInt32(RESULT);

        if (RESPMSG.Length>=8 && RESPMSG.ToUpper().Substring(0,8) == "APPROVED")
        {
            TransactionApproved = true;
        }

        TransactionAVSZIP = ParsePayPalResponse(TransactionResponse, "AVSZIP=", "&");
        TransactionAVSADDR = ParsePayPalResponse(TransactionResponse, "AVSADDR=", "&");
        TransactionCVV = ParsePayPalResponse(TransactionResponse, "CVV2MATCH=", "&");
        //TransactionIAVS = ParsePayPalResponse(TransactionResponse, "IAVS=", "&");

        return TransactionCode;
    }


    private string GeneratePayFlowParam(string ParamName, string ParamValue, int MaxLength)
    {
        string PayFlowParam = "";
        ParamValue = ParamValue.Replace("\"", "").Trim().ToUpper();
        ParamValue = (ParamValue.Length > MaxLength ? ParamValue.Substring(0, MaxLength) : ParamValue);


        PayFlowParam = ParamName.ToUpper() + "[" + ParamValue.Length + "]=" + ParamValue;
        return PayFlowParam;
    }


    private string ParsePayPalResponse(string Response, string Param, string Delimiter)
    {
        if (Response.Length == 0)
            return "";


        int FoundPos;
        int EndPos = -1;


        FoundPos = Response.IndexOf(Param);
        if (FoundPos >= 0)
        {
            FoundPos = FoundPos + Param.Length;
            EndPos = Response.IndexOf(Delimiter, FoundPos + 1);
        }


        if (FoundPos >= 0 && EndPos >= 0 && EndPos > FoundPos)
        {
            return Response.Substring(FoundPos, EndPos - FoundPos);
        }
        return "";
    }


    private string GetPayPalErrorDescription(int TransactionCode)
    {
        string PayPalErrorDescription = "";
        switch (TransactionCode)
        {
            case 12:
                PayPalErrorDescription = "DECLINED";
                break;
            case 23:
                PayPalErrorDescription = "INVALID CREDIT CARD NUMBER";
                break;
            case 24:
                PayPalErrorDescription = "INVALID CREDIT CARD EXPIRATION DATE";
                break;
        }
        return PayPalErrorDescription;
    }

}

