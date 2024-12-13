using System;

public partial class DiagramsByPartNumber : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        string parts = Utilities.QueryString("parts").ToUpper();
        parts = parts.Replace("\r", "");

        string redirected = "PartsSearch.aspx?parts=" + parts;

        Response.Redirect(redirected);
    }
}
