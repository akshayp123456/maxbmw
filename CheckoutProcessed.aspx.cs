using System;

public partial class CheckoutProcessed : System.Web.UI.Page
{
    public string m_PayFlowResponse;

    protected void Page_Load(object sender, EventArgs e)
    {
        m_PayFlowResponse = "";

        // if there are items in the cart, then leave this page!
        CartItemCollection myCartItems = (CartItemCollection)Session["CartItems"];
        if (myCartItems != null && myCartItems.Count > 0)
        {
            Response.Redirect(Utilities.URL_FICHE(false) + "/fiche.aspx");
        }

        m_PayFlowResponse = (string)Session["PayFlowResponse"];
        if (m_PayFlowResponse != null && m_PayFlowResponse != "")
        {
            // Credit card (PayFlowPro) was used, so
            // use the reponse from the CheckoutCard page
        }
        else
        {
            // came from PayPal (not the PayFlow one)
            m_PayFlowResponse = "<br />";
            m_PayFlowResponse += "    < br />";
            m_PayFlowResponse += "    Your order has been submited.< br />";
            m_PayFlowResponse += "    < br />";
            m_PayFlowResponse += "    Your shopping cart is now empty.< br />";
            m_PayFlowResponse += "    < br />";
            m_PayFlowResponse += "    You will receive an email from us with your order details.< br />";
            m_PayFlowResponse += "    < br />";
            m_PayFlowResponse += "    < br />";
            m_PayFlowResponse += "    &nbsp; &nbsp; &nbsp; &nbsp;";
            m_PayFlowResponse += "    < span class=\"Button\" onclick=\"window.location='https://shop.maxbmw.com/fiche/fiche.aspx'\">&nbsp;&nbsp;&nbsp;OK&nbsp;&nbsp;&nbsp;</span><br />";
            m_PayFlowResponse += "    <br />< br /><br />";
            m_PayFlowResponse += "    <label style=\"font-size: 12px;\"> Please avoid using your browser's back/forward buttons to navigate</label>";
            m_PayFlowResponse += "    <br />< br />";
        }

        Session.Remove("PayFlowResponse");
    }    
}

