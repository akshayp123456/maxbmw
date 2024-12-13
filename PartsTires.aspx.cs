using System;

public partial class PartsTires : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        Utilities.CheckValidURL();
        Utilities.SetLastURL();
    }    
}
