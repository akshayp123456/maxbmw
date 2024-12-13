using System;

public partial class UC_ButtonsMenu : System.Web.UI.UserControl
{
    protected string m_vid;

    protected void Page_Load(object sender, EventArgs e)
    {
        m_vid = Utilities.QueryString("vid");
    }
}