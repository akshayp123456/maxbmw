using System;
using System.Web;

public partial class Error : System.Web.UI.Page
{
    protected string m_Injection;

    protected void Page_Load(object sender, EventArgs e)
    {
        m_Injection = (Request.QueryString["inj"]!=null?Request.QueryString["inj"]:"");
        if (m_Injection=="1")
        {
            m_Injection = "It seems like some of the parameters entered in the form are incorrect.";
        }

        // aspxerrorpath is a .net query string that tells the error path, example:
        //?aspxerrorpath=/fiche/PartsSearch.aspx
        string msg = "Error.aspx - aspxerrorpath=" + Request.QueryString["aspxerrorpath"] + "<br />" + Environment.NewLine + Environment.NewLine;

        msg += "IP: " + HttpContext.Current.Request.UserHostAddress + "<br />" + Environment.NewLine;
        msg += "REFERER: " + HttpContext.Current.Request.ServerVariables["HTTP_REFERER"] + "<br />" + Environment.NewLine;
        msg += "URL: " + HttpContext.Current.Request.ServerVariables["HTTP_URL"] + "<br />" + Environment.NewLine;
        msg += "QUERYSTRING: " + HttpContext.Current.Request.QueryString.ToString() + "<br />" + Environment.NewLine;
        msg += "BROWSER: " + HttpContext.Current.Request.ServerVariables["HTTP_USER_AGENT"] + "<br />" + Environment.NewLine;
        msg += "FORMVALUES: " + HttpContext.Current.Request.Form.ToString() + "<br />" + Environment.NewLine + Environment.NewLine;
        msg += "USERID (session): " + (Session != null && Session["UserID"] != null ? Session["UserID"].ToString() : "NOT LOGED IN") + Environment.NewLine + Environment.NewLine;
                
        if (Session != null && Session["CartItems"] != null)
        {
            string CartContent = "";
            CartItemCollection c = (CartItemCollection)Session["CartItems"];
            foreach (CartItem i in c)
            {
                try
                {
                    CartContent += "ID=" + i.ID + Environment.NewLine;
                    CartContent += "PartNumber='" + i.PartNumber + "'" + Environment.NewLine;
                    CartContent += "Quantity=" + i.Quantity + Environment.NewLine;
                    CartContent += "Price=" + i.Price + Environment.NewLine;
                    CartContent += "PriceRetail=" + i.PriceRetail + Environment.NewLine;
                    CartContent += "PriceSpecials=" + i.PriceSpecials + Environment.NewLine;
                    CartContent += "IsSpecial=" + i.IsSpecial + Environment.NewLine;
                    CartContent += "Weight=" + i.Weight + Environment.NewLine;
                    CartContent += "Description='" + i.Description + "'" + Environment.NewLine;
                    CartContent += "Comments='" + i.Comments + "'" + Environment.NewLine;
                    CartContent += "AdditionalShipping=" + i.AdditionalShipping + Environment.NewLine;
                    CartContent += "CommentsShipping='" + i.CommentsShipping + "'" + Environment.NewLine;                    
                    CartContent += "SpecialsText='" + i.SpecialsText + "'" + Environment.NewLine;
                    CartContent += "SpecialsText2='" + i.SpecialsText2 + "'" + Environment.NewLine;                    
                }
                catch { }
            }

            msg += "CARTITEMS (session): " + Environment.NewLine;
            msg += CartContent + Environment.NewLine + Environment.NewLine;        
        }
        
        Exception err = Server.GetLastError();
        if (err != null)
        {
            msg += "ERR - Message: " + err.Message.ToString() + "<br />" + Environment.NewLine + Environment.NewLine;
            msg += "ERR - Source: " + err.Source.ToString() + "<br />" + Environment.NewLine + Environment.NewLine;
            msg += "ERR - Data: " + err.Data.ToString() + "<br />" + Environment.NewLine + Environment.NewLine;
            msg += "ERR - StackTrace: " + err.StackTrace.ToString() + "<br />" + Environment.NewLine + Environment.NewLine;
            msg += "ERR - all: " + err.ToString() + "<br />" + Environment.NewLine + Environment.NewLine;
        }

        bool IsBot = (HttpContext.Current.Request.ServerVariables["HTTP_USER_AGENT"].ToLower().IndexOf("bot") >= 0 || HttpContext.Current.Request.ServerVariables["HTTP_USER_AGENT"].ToLower().IndexOf("spider")>=0);
        msg += (IsBot ? "IS A BOT!" : "IS A USER") + "<br />" + Environment.NewLine;

        if (Request.QueryString["inj"]!=null)
            msg += "Injection=" + Request.QueryString["inj"];

        if (!IsBot)
        {
            //EmailSender.SendEmail("berczely@hotmail.com", "no-reply@maxbmw.com", "MAXBMW - Error", "", "", "Error in Page", msg);
            //Auditing.InsertComment(msg);
        }
    }
}
