using System;
using System.Data.OleDb;
using System.Text;

public partial class UC_Parts_Body : System.Web.UI.UserControl
{
    private OleDbConnection conn;

    protected string m_rnd;
    protected string m_source;
    protected string m_category;
    protected string m_brand;
    protected string m_vid;

    protected void Page_Load(object sender, EventArgs e)
    {
        conn = new OleDbConnection(System.Configuration.ConfigurationManager.AppSettings["connString"]);
        conn.Open();

        m_rnd = DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString() + DateTime.Now.Millisecond.ToString();

        m_source = this.Page.ToString().ToLower().Replace("asp.parts", "").Replace("_aspx", "");
  
        m_category = Utilities.QueryString("category");
        m_brand = Utilities.QueryString("brand");
        m_vid = Utilities.QueryString("vid");

        // diagram is not neede here in the aspx code (it is used in the JS code just to jump to the diagram using animate)
    }

    protected string GetModelFromVID(string vid)
    {
        if (vid.Length != 5)
            return "";

        string modelCaption = "";

        OleDbCommand cmd = new OleDbCommand();
        cmd.Connection = conn;
        cmd.CommandTimeout = 120;
                
        cmd.CommandText = @"SELECT fztyp_mospid, fztyp_model, fztyp_useprod,
                                                (SELECT MIN(fztyp_prodmin) FROM etk_fztyp WHERE fztyp_mospid=x.fztyp_mospid) AS prodmin,
                                                (SELECT MAX(fztyp_prodmax) FROM etk_fztyp WHERE fztyp_mospid=x.fztyp_mospid) AS prodmax
                                        FROM etk_fztyp AS x
                                        WHERE fztyp_mospid = ?";

        cmd.Parameters.Clear();
        cmd.Parameters.AddWithValue("@fztyp_mospid", vid);
        OleDbDataReader dr = cmd.ExecuteReader();
        if (dr.Read())
        {
            modelCaption = "";
            if (dr["fztyp_useprod"] != DBNull.Value && Convert.ToBoolean(dr["fztyp_useprod"]) == true)
            {
                modelCaption = dr["fztyp_model"].ToString() + " " + dr["prodmin"].ToString().Substring(2, 2) + "-" + dr["prodmax"].ToString().Substring(2, 2);
            }
            else
            {
                modelCaption = dr["fztyp_model"].ToString();
            }
        }
        dr.Close();
        cmd.Dispose();

        return modelCaption;
    }

    protected void Page_Unload(object sender, EventArgs e)
    {
        if (conn != null)
        {
            conn.Close();
            conn.Dispose();
        }
    }

    public string ShowProductsList()
    {
        if (m_source == "fiche")
            return "";

        OleDbCommand cmd = new OleDbCommand();
        cmd.Connection = conn;
        cmd.CommandTimeout = 120;

        StringBuilder txt = new StringBuilder("");
        int ProductsCount = 0;

        string MGFilter = "";
        string SubGroup = "";

        if (m_source == "")
            return "";

        GetFilters(m_source, m_category, out MGFilter, out SubGroup);

        cmd.CommandText = @"SELECT DISTINCT 
                                parts_diagram,
                                parts_diagram_mg,
                                parts_image,
                                (SELECT TOP 1 ReplaceDiagram FROM NotesDiagrams WHERE Diagram=parts_diagram) AS notesdiagrams_replacediagram,
                                parts_imagetext,
                                (SELECT TOP 1 Description FROM NotesDiagrams WHERE Diagram=parts_diagram) AS notesdiagrams_description,
                                parts_subgroup,
                                parts_diagram_order,
                                1 AS IsCustom,
                                (SELECT TOP 1 SuppressDiagram FROM NotesDiagrams WHERE Diagram=parts_diagram) AS Suppress
                            FROM PartsCustom
                            WHERE"
                            + (m_brand != "" ? " parts_brand=?" : " 1=1")
                            + (m_vid != "" ? " AND (parts_vid=? OR parts_vid=0 OR parts_vid=99999)" : "")
                            + (SubGroup != "" ? " AND parts_subgroup=?" : "")
                            + " AND " + MGFilter;

        if (m_source == "apparel")
        {
            // for apparel, we need to retrieve the PartsFiche MG=76
            cmd.CommandText += @" UNION
                                    SELECT DISTINCT 
                                        parts_diagram,
                                        parts_diagram_mg,
                                        parts_image,
                                        (SELECT TOP 1 ReplaceDiagram FROM NotesDiagrams WHERE Diagram=parts_diagram) AS notesdiagrams_replacediagram,
                                        parts_imagetext,
                                        (SELECT TOP 1 Description FROM NotesDiagrams WHERE Diagram=parts_diagram) AS notesdiagrams_description,
                                        parts_subgroup,
                                        parts_diagram_order,
                                        0 AS IsCustom,
                                        (SELECT TOP 1 SuppressDiagram FROM NotesDiagrams WHERE Diagram=parts_diagram) AS Suppress
                                    FROM PartsFiche
                                    WHERE"
                                    + (m_brand != "" ? " parts_brand=?" : " 1=1")
                                    + (SubGroup != "" ? " AND parts_subgroup=?" : "")
                                    + " AND " + MGFilter;
        }

        cmd.CommandText += @" ORDER BY parts_diagram_order, IsCustom ASC, parts_imagetext";
        cmd.Parameters.Clear();

        if (m_brand != "")
            cmd.Parameters.AddWithValue("@parts_brand", m_brand);

        if (m_vid != "")
            cmd.Parameters.AddWithValue("@parts_vid", m_vid);

        if (SubGroup != "")
            cmd.Parameters.AddWithValue("@parts_subgroup", SubGroup);

        if (m_source == "apparel")
        {
            if (m_brand != "")
                cmd.Parameters.AddWithValue("@parts_brand", m_brand);

            if (SubGroup != "")
                cmd.Parameters.AddWithValue("@parts_subgroup", SubGroup);
        }

        if (cmd.CommandText != "")
        {
            OleDbDataReader dr = cmd.ExecuteReader();

            string subgroup = "";
            string parts_diagram;
            string parts_image;
            string parts_imagetext;
            string parts_subgroup;
            string Comments;
            string Description;
            bool ReplaceDiagram;
            int Column = 0;

            string viewmode = Utilities.QueryString("viewmode").ToLower();

            if (viewmode == "list")
            {
                // Details View
                while (dr.Read())
                {
                    if (!(DBNull.Value != dr["Suppress"] && Convert.ToBoolean(dr["Suppress"])))
                    {
                        ProductsCount++;

                        parts_diagram = dr["parts_diagram"].ToString();
                        parts_subgroup = dr["parts_subgroup"].ToString();
                        parts_image = dr["parts_image"].ToString();
                        ReplaceDiagram = (DBNull.Value != dr["notesdiagrams_replacediagram"] ? Convert.ToBoolean(dr["notesdiagrams_replacediagram"]) : false);

                        parts_imagetext = dr["parts_imagetext"].ToString().ToUpper();
                        Description = dr["notesdiagrams_description"].ToString().ToUpper().Trim();
                        // use the name of the product based on the NotesDiagrams instead
                        if (Description != "")
                            parts_imagetext = Description;

                        GetDiagramNotesComments(parts_diagram, out Description, out Comments);
                    

                        if (subgroup != parts_subgroup)
                        {
                            subgroup = parts_subgroup;

                            txt.Append("<tr><td id=\"QUICKLINKS_" + subgroup.Replace(" ", "").Replace(".", "").Replace("&", "") + "\" colspan=\"3\" style=\"font-size: " + (ProductsCount == 1 ? "0px" : "10px") + ";\">");
                            txt.Append("</td></tr>");

                            txt.Append("<tr><td colspan=\"3\" align=\"left\" class=\"MainGroupChange\">");
                            txt.Append(subgroup);
                            txt.Append("</td></tr>");
                        }

                        txt.Append("<tr>");
                        txt.Append("<td valign=\"top\" align=\"center\" id=\"Diagram_" + parts_diagram.Replace(" ", "").Replace(".", "") + "\" onclick=\"ShowPartsDetails('" + parts_diagram + "','');\">");
                                        
                        txt.Append("<table class=\"Product\" cellspacing=\"0\" cellpadding=\"0\" border=\"0\">");
                        txt.Append("<tr>");
                        txt.Append("<td align=\"left\" width=\"725\" colspan=\"2\" class=\"ProductTitle\">");
                        txt.Append(parts_imagetext);
                        txt.Append("</td>");
                        txt.Append("</tr>");
                        txt.Append("<tr>");
                        txt.Append("<td width=\"220\" align=\"center\" valign=\"top\" style=\"border-left: 1px solid #333; border-bottom: 1px solid #333;" + (parts_image.IndexOf("B0") == 0 ? "height: 139px;":"height: 155px;") + " background-color: #fff;\">");
                        if (parts_image.IndexOf("B0") == 0)
                        {
                            txt.Append("<img style=\"min-height: 138px;\" src=\"DiagramsThumb/" + parts_image + ".png?v=" + Utilities.VERSION + "\" alt=\"\" />");
                        }
                        else
                        {
                            txt.Append("<img src=\"DiagramsCustom/" + parts_image + "_T.jpg?v=" + Utilities.VERSION + "\" alt=\"\" />");
                        }
                        txt.Append("</td>");

                        txt.Append("<td width=\"505\" align=\"left\" valign=\"top\" style=\"padding: 8px; border-right: 1px solid #333; border-bottom: 1px solid #333; background-color: #ccc; color: #333; font-size: 12px;\">");
                        txt.Append(Comments);
                        txt.Append("</td>");
                        txt.Append("</tr>");
                        txt.Append("</table>");
                                       
                        txt.Append("</td>");
                    }
                }
            }
            else
            {
                // Thumbnails view
                while (dr.Read())
                {
                    if (!(DBNull.Value != dr["Suppress"] && Convert.ToBoolean(dr["Suppress"])))
                    {
                        ProductsCount++;
                        Column++;

                        parts_diagram = dr["parts_diagram"].ToString();
                        parts_subgroup = dr["parts_subgroup"].ToString();
                        parts_image = dr["parts_image"].ToString();
                        ReplaceDiagram = (dr["notesdiagrams_replacediagram"] != DBNull.Value ? Convert.ToBoolean(dr["notesdiagrams_replacediagram"]) : false);

                        parts_imagetext = dr["parts_imagetext"].ToString().ToUpper();
                        Description = dr["notesdiagrams_description"].ToString().ToUpper().Trim();
                        // use the name of the product based on the NotesDiagrams instead
                        if (Description != "")
                            parts_imagetext = Description;

                        if (subgroup != parts_subgroup)
                        {
                            // finish row first (if not first row)
                            while (Column > 1 && Column <= 3)
                            {
                                txt.Append("<td>&nbsp;</td>");
                                Column++;
                            }
                            if (Column > 1)
                            {
                                txt.Append("</tr>");
                                Column = 1;
                            }

                            subgroup = parts_subgroup;

                            txt.Append("<tr><td id=\"QUICKLINKS_" + subgroup.Replace(" ", "").Replace(".", "").Replace("&", "") + "\" colspan=\"3\" style=\"font-size: " + (ProductsCount == 1 ? "0px" : "10px") + ";\">");
                            txt.Append("</td></tr>");

                            txt.Append("<tr><td colspan=\"3\" align=\"left\" class=\"MainGroupChange\">");
                            txt.Append(subgroup);
                            txt.Append("</td></tr>");
                        }

                        if (Column == 1)
                        {
                            txt.Append("<tr>"); // begining of a row
                        }

                        txt.Append("<td valign=\"top\" align=\"center\" id=\"Diagram_" + parts_diagram.Replace(" ", "").Replace(".", "") + "\" onclick=\"ShowPartsDetails('" + parts_diagram + "','');\">");

                        txt.Append("<table class=\"Product\" cellspacing=\"0\" cellpadding=\"0\" border=\"0\">");
                        txt.Append("<tr>");
                        txt.Append("<td align=\"left\" width=\"220\" class=\"ProductTitleThumbs\">");
                        txt.Append(parts_imagetext);
                        txt.Append("</td>");
                        txt.Append("</tr>");
                        txt.Append("<tr>");
                        txt.Append("<td align=\"center\" valign=\"top\" width=\"220\" style=\"border: 1px solid #333; " + (parts_image.IndexOf("B0") == 0 ? "height: 139px;" : "height: 155px;") + " background-color: #fff;\">");

                        if (ReplaceDiagram)
                        {
                            txt.Append("<img src=\"NotesDiagrams/" + parts_diagram + "_1_T.jpg?v=" + Utilities.VERSION + "\" alt=\"\" />");
                        }
                        else
                        {
                            if (parts_image.IndexOf("B0") == 0)
                            {
                                txt.Append("<img style=\"min-height: 138px;\" src=\"DiagramsThumb/" + parts_image + ".png?v=" + Utilities.VERSION + "\" alt=\"\" />");
                            }
                            else
                            {
                                txt.Append("<img src=\"DiagramsCustom/" + parts_image + "_T.jpg?v=" + Utilities.VERSION + "\" alt=\"\" />");
                            }
                        }
                        txt.Append("</td>");
                        txt.Append("</tr>");
                        txt.Append("</table>");

                        txt.Append("</td>");

                        if (Column == 3)
                        {
                            txt.Append("</tr>"); // natural end of a row
                            Column = 0;
                        }
                    }
                }

                // finish row left over
                while (Column <= 3)
                {
                    txt.Append("<td>&nbsp;</td>");
                    Column++;
                }
                if (Column > 1)
                {
                    txt.Append("</tr>");
                }
            }

            if (txt.Length == 0)
            {
                txt.Append("<tr><td><br /><br /><br />&nbsp;&nbsp;No items found for this model yet.</td></tr>");
            }
        }

        cmd.Dispose();

        return "<div id=\"ProductsCountInvisible\" style=\"display: none;\">" + ProductsCount + " product" + (ProductsCount == 1 ? "" : "s") + " found</div><table cellspacing=\"8\" cellpadding=\"0\">" + txt.ToString() + "</table>";
    }


    public string TruncateAtWord(string value, int length)
    {
        if (value == null || value.Length < length || value.IndexOf(" ", length) == -1)
            return value;

        return value.Substring(0, value.IndexOf(" ", length)) + " ... [more]";
    }


    private void GetDiagramNotesComments(string Diagram, out string Description, out string Comments)
    {
        Description = "";
        Comments = "";
        OleDbCommand cmd = new OleDbCommand();
        cmd.Connection = conn;
        cmd.CommandTimeout = 120;

        cmd.CommandText = "SELECT Description, Comments FROM NotesDiagrams WHERE Diagram=?";
        cmd.Parameters.Clear();
        cmd.Parameters.AddWithValue("@Diagram", Diagram);
        OleDbDataReader dr = cmd.ExecuteReader();

        if (dr.Read())
        {
            Description = (DBNull.Value != dr["Description"] ? dr["Description"].ToString().Trim() : "");

            Comments = (DBNull.Value != dr["Comments"] ? dr["Comments"].ToString().Trim() : "");
            Comments = TruncateAtWord(Comments, 700);
        }
        dr.Close();
        cmd.Dispose();
    }


    private bool GetFilters(string source, string category, out string MGFilter, out string SubGroup)
    {
        MGFilter = "";
        SubGroup = "";

        if (source == "apparel")
        {
            MGFilter = " parts_diagram_mg='76'";
            SubGroup = category.ToLower();
            return true; //false --> joint of (PartsCustom + PartsFiche) is needed!!!
        }
        else if (source == "tires")
        {
            MGFilter = " parts_diagram_mg='WT'";
            SubGroup = category.ToLower();

        }
        else if (source == "catalog")
        {
            MGFilter = " parts_diagram_mg!='WT' AND (parts_diagram_mg<'00' OR parts_diagram_mg>'99')";
            SubGroup = category.ToLower();
            SubGroup = SubGroup.Replace("&amp;", "&");
        }
        return false; //false --> no joint of (PartsCustom + PartsFiche) is needed  
    }

   
    // used for the models filter popup
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

                string modelCaption = "";
                if (dr["fztyp_useprod"] != DBNull.Value && Convert.ToBoolean(dr["fztyp_useprod"]) == true)
                {
                    modelCaption = dr["fztyp_model"].ToString() + "&nbsp;" + dr["prodmin"].ToString().Substring(2, 2) + "-" + dr["prodmax"].ToString().Substring(2, 2);
                    txt.Append("<a class=\"TooltipLink\" onmouseover=\"SetVehicleIcon('" + vehicletext1 + "','" + vehicletext2 + "','" + icon + "');\" href=\"#\" onclick=\"BikeModelClicked('" + modelCaption + "'," + dr["fztyp_mospid"].ToString() + ");\">" + modelCaption);

                }
                else
                {
                    modelCaption = dr["fztyp_model"].ToString();
                    txt.Append("<a class=\"TooltipLink\" onmouseover=\"SetVehicleIcon('" + vehicletext1 + "','" + vehicletext2 + "','" + icon + "');\" href=\"#\" onclick=\"BikeModelClicked('" + modelCaption + "'," + dr["fztyp_mospid"].ToString() + ");\">" + modelCaption);
                }
                txt.Append("</a><br />");
            }
            dr.Close();
        }

        cmd.Dispose();

        return txt.ToString();
    }
}