using System;
using System.Data.OleDb;

public partial class Fiche : System.Web.UI.Page
{
    protected OleDbConnection conn;

    protected void Page_Load(object sender, EventArgs e)
    {
        Utilities.CheckValidURL();
        Utilities.SetLastURL();
    }

    protected void Page_Unload(object sender, EventArgs e)
    {
        if (conn != null)
        {
            conn.Close();
            conn.Dispose();
        }
    }
}
