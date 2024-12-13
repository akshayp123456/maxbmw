using System;
using System.Web;



public partial class CartAdd : System.Web.UI.Page
{
    protected CartHelper cartHelper;

    protected void Page_Load(object sender, EventArgs e)
    {
        cartHelper = new CartHelper();

        if (Utilities.QueryString("updatecart") == "1")
        {
            CartItemCollection myCartItems = (CartItemCollection)Session["CartItems"];

            string partnumber = Utilities.QueryString("partnumber");
            string qty = Utilities.QueryString("qty");
            string description = Utilities.QueryString("description");
            description = HttpUtility.UrlDecode(description);
            string weight = Utilities.QueryString("weight");

            // Add new item to cart
            if (partnumber != null && partnumber != "" && qty != null)
            {
                try
                {
                    if (Convert.ToInt16(qty) > 0)
                    {
                        if (myCartItems == null)
                            myCartItems = new CartItemCollection();

                        CartItem NewCartItem = new CartItem();

                        NewCartItem.PartNumber = partnumber;
                        NewCartItem.Quantity = Convert.ToInt16(qty);
                        NewCartItem.Description = description;
                        try
                        {
                            NewCartItem.Weight = Convert.ToDouble(weight);
                        }
                        catch
                        {
                            NewCartItem.Weight = 0.0;
                        }

                        cartHelper.GetPartDetails(ref NewCartItem);

                        myCartItems.Add(NewCartItem);

                        // Save session cart
                        Session["CartItems"] = myCartItems;
                    }                    
                }
                catch
                {
                }
            }            
        }       
    }    
}
