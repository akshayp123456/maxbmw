using System;
using System.Configuration;
using System.Web;
using System.Data.OleDb;
using System.IO;
using System.Net;
using Newtonsoft.Json.Linq;

public partial class CheckoutPayPal : System.Web.UI.Page
{
    protected CartHelper cartHelper;
    protected Checkout c;

    protected void Page_Load(object sender, EventArgs e)
    {
        // if there are no items in the cart, then leave this page!
        CartItemCollection myCartItems = (CartItemCollection)Session["CartItems"];
        if (myCartItems == null || myCartItems.Count == 0)
        {
            Response.Redirect(Utilities.URL_FICHE(true) + "/CheckoutProcessed.aspx");
        }        
        
        cartHelper = new CartHelper();
        c = new Checkout();

        if (Session["UserID"] == null)
            c.UserID = "NONE";
        else
            c.UserID = Session["UserID"].ToString();

        // retrieve form values
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

        c.PaymentType = "P"; // Utilities.SForm("PAYMENTTYPE");

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
        c.WholesaleCompany = Utilities.SForm("COMPANY");

        c.OrderID = c.SaveDB_Order();

        c.SaveDB_OrderItems((CartItemCollection)Session["CartItems"]);

        c.TransactionID = "PAYPAL";
        c.TransactionResponse = "";
        c.TransactionApproved = true;
        //c.TransactionAVSZIP = "P";
        //c.TransactionAVSADDR = "P";
        c.TransactionCVV = "P";
        c.TransactionCode = 0;

        c.UpdateDB_Order();


        // Create the body of the email to be sent. This email body is also reused for the HTMLText to be used on this page
        string eMailSubject = "ORDER #" + c.OrderID.ToString() + "  PAYPAL" + "  " + c.eMail + " (UserID# " + c.UserID + ")";
        string eMailBody = c.CreateEmailBody();
        bool eMailSentToStore = EmailSender.SendEmail(ConfigurationManager.AppSettings["EmailOrders"], ConfigurationManager.AppSettings["EmailOrders"], "MAX BMW - Orders", "", ConfigurationManager.AppSettings["EmailOrdersBCC"], eMailSubject, eMailBody);

        // Send an email copy to the customer
        string CopyEMailSubject = "Your MAX BMW ORDER #" + c.OrderID.ToString() + "  PAYPAL";
        string CopyEMailBody = "";
        CopyEMailBody += "This is the the list of items you have ordered.<br /><br />";
        CopyEMailBody += "PLEASE NOTE: if you have any questions, concerns, want to change, or would like to add or remove something from the below order, please reply to this email. You will receive a much faster response. Thank you.<br /><br />";
        CopyEMailBody += "After we have confirmed your PAYPAL payment, we will start processing your order.<br />";
        CopyEMailBody += "Within 24-72 hours you will receive an e-mail confirmation from our Internet Sales Department. Included will be an image of your order.<br /><br />";
        CopyEMailBody += "Any parts that have been sourced from Germany (BMWAG) will be mentioned in the confirmation to follow.";
        CopyEMailBody += "<br /><br /><br />";
        CopyEMailBody += eMailBody + "<br /><br /><br />";
        CopyEMailBody += "Internet Service Providers (ISP) have tightened their definitions of SPAM.<br />";
        CopyEMailBody += "Your ISP might categorize an email confirmation from us as potential spamand filter it into a Bulk or a predetermined SPAM folder you define.<br />";
        CopyEMailBody += "If you place an order request and do not receive your email confirmation inyour Inbox, please check in these areas BEFORE contacting us.<br /><br />";
        CopyEMailBody += "Thank you for your business!";
        bool eMailSentToCustomer = EmailSender.SendEmail(c.eMail, ConfigurationManager.AppSettings["EmailOrders"], "MAX BMW - " + ConfigurationManager.AppSettings["EmailNameOrders"], "", ConfigurationManager.AppSettings["EmailOrdersBCC"], CopyEMailSubject, CopyEMailBody);
        
        // clear the shopping cart items                
        cartHelper.Clear();
        Session.Remove("checkout");
    }
   
   }

