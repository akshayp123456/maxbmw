using System;

public partial class UserLogout : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        Session["UserID"] = null;
        Session["UserEMail"] = null;
        Session["UserFirstName"] = null;
        Session["UserLastName"] = null;

        Response.Redirect(Utilities.GetLastLoginURL());               
    }
}
