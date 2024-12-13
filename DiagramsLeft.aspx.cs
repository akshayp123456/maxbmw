using System;

public partial class DiagramsLeft : System.Web.UI.Page
{

    protected void Page_Load(object sender, EventArgs e)
    {
        string menu = Utilities.QueryString("menu").ToLower();
        string vid = Utilities.QueryString("vid").ToUpper();
        string redirected = "fiche.aspx";

        if (menu == "apparel")
        {
            redirected = "PartsApparel.aspx";
        }
        else if (menu == "aftermarket")
        {
            redirected = "PartsCatalog.aspx?vid=" + vid + (Utilities.QueryString("Diagram").Length == 7 ? "&diagram=" + Utilities.QueryString("Diagram") : "");
        }
        else
        {
            if (vid.Length == 5)
                redirected = "DiagramsMain.aspx?vid=" + vid + (Utilities.QueryString("Diagram").Length==7 ? "&diagram=" + Utilities.QueryString("Diagram") : "");
        }
        Response.Redirect(redirected);
    }
}
