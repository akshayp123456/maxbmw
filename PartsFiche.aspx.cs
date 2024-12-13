using System;
using System.Data.OleDb;
using System.Text;
using System.IO;

public partial class PartsFiche : System.Web.UI.Page
{
    private OleDbConnection conn;

    protected string m_rnd;
    protected string m_model;

    protected void Page_Load(object sender, EventArgs e)
    {
        Utilities.CheckValidURL();
        Utilities.SetLastURL();

        conn = new OleDbConnection(System.Configuration.ConfigurationManager.AppSettings["connString"]);
        conn.Open();

        m_rnd = DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString() + DateTime.Now.Millisecond.ToString();

        m_model = Utilities.QueryString("model").ToLower();
    }

    protected void Page_Unload(object sender, EventArgs e)
    {
        if (conn != null)
        {
            conn.Close();
            conn.Dispose();
        }
    }

    protected string ShowBikesList(string BikeModels)
    {
        StringBuilder txt = new StringBuilder("");

        string[] Models = BikeModels.Split(',');

        OleDbCommand cmd = new OleDbCommand();
        cmd.Connection = conn;
        OleDbDataReader dr;

        string vehicletext1;
        string vehicletext2;
        string icon;

        foreach (string model in Models)
        {
            cmd.CommandText = @"SELECT DISTINCT fztyp_mospid,
                                                fztyp_model,
                                                fztyp_baureihe,
                                                fztyp_ktlgausf,
                                                fztyp_show,
                                                fztyp_useprod,
                                                (SELECT MIN(fztyp_prodmin) FROM etk_fztyp WHERE fztyp_mospid=x.fztyp_mospid) AS prodmin,
                                                (SELECT MAX(fztyp_prodmax) FROM etk_fztyp WHERE fztyp_mospid=x.fztyp_mospid) AS prodmax
                                        FROM etk_fztyp AS x
                                        WHERE fztyp_show=1 AND fztyp_model LIKE '" + model + "' " + @"
                                        ORDER BY fztyp_model, prodmin, fztyp_ktlgausf";

            dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                vehicletext1 = dr["fztyp_model"].ToString() + " (" + dr["fztyp_baureihe"].ToString() + ")";
                vehicletext2 = dr["prodmin"].ToString().Substring(4, 2) + "/" + dr["prodmin"].ToString().Substring(2, 2) + "-" + dr["prodmax"].ToString().Substring(4, 2) + "/" + dr["prodmax"].ToString().Substring(2, 2) + "&nbsp;&nbsp;&nbsp;" + dr["fztyp_ktlgausf"].ToString();

                icon = dr["fztyp_model"].ToString().Replace("/", " ").Replace("+", "p");
                if (dr["fztyp_useprod"] != DBNull.Value && Convert.ToBoolean(dr["fztyp_useprod"]) == true)
                {
                    icon += " " + dr["prodmin"].ToString().Substring(2, 2) + "-" + dr["prodmax"].ToString().Substring(2, 2);
                }
                if (!File.Exists(MapPath("../fiche/VehiclesIcons/" + icon + ".jpg")))
                {
                    icon = "NoIcon";
                }

                string modelCaption = "";
                if (dr["fztyp_useprod"] != DBNull.Value && Convert.ToBoolean(dr["fztyp_useprod"]) == true)
                {
                    modelCaption = dr["fztyp_model"].ToString() + "&nbsp;" + dr["prodmin"].ToString().Substring(2, 2) + "-" + dr["prodmax"].ToString().Substring(2, 2);
                    txt.Append("<a class=\"TooltipLink\" onmouseover=\"SetVehicleIcon('" + vehicletext1 + "','" + vehicletext2 + "','" + icon + "');\" href=\"javascript:BikeModelClicked('" + modelCaption + "'," + dr["fztyp_mospid"].ToString() + ");\">" + modelCaption);

                }
                else
                {
                    modelCaption = dr["fztyp_model"].ToString();
                    txt.Append("<a class=\"TooltipLink\" onmouseover=\"SetVehicleIcon('" + vehicletext1 + "','" + vehicletext2 + "','" + icon + "');\" href=\"javascript:BikeModelClicked('" + modelCaption + "'," + dr["fztyp_mospid"].ToString() + ");\">" + modelCaption);
                }
                txt.Append("</a><br />");
            }
            dr.Close();
        }

        cmd.Dispose();

        return txt.ToString();
    }
}
