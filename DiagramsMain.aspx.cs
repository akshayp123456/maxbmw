using System;
using System.Configuration;
using System.Data.OleDb;
using System.IO;
using System.Text;

public partial class DiagramsMain : System.Web.UI.Page
{
    protected string m_rnd;
    protected string m_vid;

    protected string m_BackButtonOnClick;
    protected string m_BackButtonCaption;

    protected string m_VehicleIcon;
    protected string m_model;
    protected string m_model2;
    protected string m_Diagram;
    protected string m_parts_string;
    protected string m_vin; //optional
    protected string m_ManufacturingDate; //optional

    private OleDbConnection conn;
    private OleDbCommand cmd;
    private OleDbDataReader dr;

    protected void Page_Load(object sender, EventArgs e)
    {
        Utilities.CheckValidURL();
        Utilities.SetLastURL();

        m_rnd = DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString() + DateTime.Now.Millisecond.ToString();

        m_VehicleIcon = "";
        m_vid = Utilities.QueryString("vid"); //51568 = HP2 Sport USA
        if (m_vid.Length > 5)
        {
            m_vid = m_vid.Substring(0, 5);
        }
        m_Diagram = Utilities.QueryString("diagram");
        m_parts_string = Utilities.QueryString("parts");
        m_vin = Utilities.QueryString("vin");
        m_ManufacturingDate = Utilities.QueryString("md");

        if (m_vid == "51984")
            m_vid = "54122"; // the old mospid for the S1000RR changed to 54122 and 54124 (the first one is for the older model)

        string menu = Utilities.QueryString("menu").ToLower(); // in case there is an old link!
        string redirected = "";
        if (menu == "apparel")
        {
            redirected = "PartsApparel.aspx";
        }
        else if (menu == "aftermarket")
        {
            redirected = "PartsCatalog.aspx?vid=" + m_vid + (Utilities.QueryString("Diagram").Length == 7 ? "&diagram=" + Utilities.QueryString("Diagram") : "");
        }
        else if (m_vid == "")
        {
            redirected = "PartsFiche.aspx";
        }
        if (redirected != "")
        {
            Response.Redirect(redirected);
        }
    

        if (Utilities.GetUrlReferrer().IndexOf("PartsSearch.aspx") > 0)
        {
            m_BackButtonCaption = "Search";
            m_BackButtonOnClick = "history.go(-1);";
        }
        else
        {
            m_BackButtonCaption = "Change Model";
            m_BackButtonOnClick = "window.location='PartsFiche.aspx';";
        }


        conn = new OleDbConnection(ConfigurationManager.AppSettings["connString"]);
        conn.Open();
        cmd = new OleDbCommand();
        cmd.Connection = conn;
        cmd.CommandTimeout = 120;
        
        // get model and production
        m_model = "";
        m_model2 = "";
        cmd.CommandText = @"SELECT DISTINCT fztyp_baureihe,                                            
                                            fztyp_model,
                                            fztyp_useprod,
                                            fztyp_ktlgausf,
                                            (SELECT MIN(fztyp_prodmin) FROM etk_fztyp WHERE fztyp_mospid=x.fztyp_mospid) AS prodmin,
                                            (SELECT MAX(fztyp_prodmax) FROM etk_fztyp WHERE fztyp_mospid=x.fztyp_mospid) AS prodmax
                                FROM etk_fztyp AS x
                                WHERE fztyp_mospid=?";
        cmd.Parameters.Clear();
        cmd.Parameters.AddWithValue("@fztyp_mospid", m_vid);
        dr = cmd.ExecuteReader();

        if (dr.Read())
        {
            m_model = dr["fztyp_model"].ToString() + " (" + dr["fztyp_baureihe"].ToString() + ")";

            if (m_vin == "")
            {
                m_model2 = "Production: " + dr["prodmin"].ToString().Substring(4, 2) + "/" + dr["prodmin"].ToString().Substring(2, 2) + "-" + dr["prodmax"].ToString().Substring(4, 2) + "/" + dr["prodmax"].ToString().Substring(2, 2) + "&nbsp;&nbsp;" + dr["fztyp_ktlgausf"].ToString();
            }
            else
            {
                OleDbCommand cmd2 = new OleDbCommand();
                cmd2.Connection = conn;
                cmd2.CommandTimeout = 120;

                cmd2.CommandText = @"SELECT TOP 1 fgstnr_prod FROM etk_fgstnr WHERE fgstnr_von<=? AND fgstnr_bis>=?";
                cmd2.Parameters.Clear();
                cmd2.Parameters.AddWithValue("@fgstnr_von", m_vin);
                cmd2.Parameters.AddWithValue("@fgstnr_bis", m_vin);
                OleDbDataReader dr2 = cmd2.ExecuteReader();

                if (dr2.Read())
                {
                    m_model2 = "VIN:" + m_vin + "&nbsp;&nbsp;Produced: " + dr2["fgstnr_prod"].ToString().Substring(4, 2) + "/" + dr2["fgstnr_prod"].ToString().Substring(2, 2) + "&nbsp;&nbsp;" + dr["fztyp_ktlgausf"].ToString();
                }
            }

            if (dr["fztyp_useprod"] != DBNull.Value && Convert.ToBoolean(dr["fztyp_useprod"]) == true)
            {
                m_VehicleIcon = dr["fztyp_model"].ToString().Replace("/", " ") + " " + dr["prodmin"].ToString().Substring(2, 2) + "-" + dr["prodmax"].ToString().Substring(2, 2) + ".jpg";
            }
            else
            {
                m_VehicleIcon = dr["fztyp_model"].ToString().Replace("/", " ").Replace("+", "p") + ".jpg";
            }
            if (!File.Exists(MapPath("../fiche/VehiclesIcons/" + m_VehicleIcon)))
            {
                m_VehicleIcon = "NoIcon.jpg";
            }
        }
        dr.Close();
    }

    protected void Page_Unload(object sender, EventArgs e)
    {
        if (dr != null)
            dr.Dispose();

        if (cmd != null)
            cmd.Dispose();

        if (conn != null)
        {
            conn.Close();
            conn.Dispose();
        }        
    }

    protected string ShowServiceSchedules()
    {
        string txt = "";
        try
        {
            string dName;
            dName = "Service/" + m_VehicleIcon.Substring(0, m_VehicleIcon.Length - 4);
            if (m_VehicleIcon.Length > 4 && Directory.Exists(MapPath(dName)))
            {
                string[] FilesFound = Directory.GetFiles(MapPath(dName));

                foreach (string f in FilesFound)
                {
                    string fName = f.Substring(f.LastIndexOf("\\") + 1, f.Length - 4 - f.LastIndexOf("\\") - 1);
                    txt += "<a class=\"MainGroupLink\" href=\"#\" onclick=\"window.open('" + dName + "/" + fName + ".pdf', '_blank', 'menubar=no,toolbar=no,scrollbars=yes');\">- " + fName + " miles</a><br />";
                }
            }

            if (txt != "")
                txt = "Service Schedules:<br />" + txt;
        }
        catch
        { }

        return txt;
    }

    protected string ShowMenuItem(string MG, string Description, string Style)
    {
        string txt = "";       
           
        cmd.CommandText = @"SELECT 1 AS HasDiagrams 
                                FROM PartsFiche
                                WHERE parts_vid=? AND parts_diagram_mg=?";
            
        cmd.Parameters.Clear();
        cmd.Parameters.AddWithValue("@parts_vid", m_vid);
        cmd.Parameters.AddWithValue("@parts_diagram_mg", MG);
        dr = cmd.ExecuteReader();

        if (dr.HasRows) 
        {
            txt = "<a href=\"javascript: AnimateToMG('" + MG + "')\" class=\"MainGroupLink\" style=\"" + Style + "\">" + Description + "</a><br />";
        }                      

        dr.Close();            

        return txt;
    }

    protected string ShowSpecials(string Description, string Style)
    {
        string txt = "";
                
        cmd.CommandText = @"SELECT TOP 1 parts_partnumber
                            FROM PartsFiche, NotesParts
                            WHERE parts_vid=? AND SpecialsShowInRightPanel=1 AND parts_partnumber=PartNumber
                            UNION
                            SELECT TOP 1 parts_partnumber
                            FROM PartsCustom, NotesParts
                            WHERE parts_vid=? AND SpecialsShowInRightPanel=1 AND parts_partnumber=PartNumber";
        cmd.Parameters.Clear();
        cmd.Parameters.AddWithValue("@parts_vid", m_vid);
        cmd.Parameters.AddWithValue("@parts_vid", m_vid);
        dr = cmd.ExecuteReader();
        if (dr.HasRows)
            txt = txt = "<a href=\"#\" onclick=\"document.getElementById('IFRAMERIGHTPANEL').src='DiagramsSpecials.aspx?rnd=" + m_rnd + "&vid=" + m_vid + "';\" class=\"MainGroupLink\" style=\"" + Style + "\">" + Description + "</a><br />";
           
        dr.Close();
               
        return txt;
    }


    public string ShowDiagramsLeft()
    {
        StringBuilder txt = new StringBuilder("");

        OleDbCommand cmd = new OleDbCommand();
        cmd.Connection = conn;
        cmd.CommandTimeout = 120;

        cmd.CommandText = @"SELECT DISTINCT 
                                parts_diagram,
                                parts_diagram_mg AS MG,
                                (SELECT TOP 1 ReplaceDiagram FROM NotesDiagrams WHERE Diagram=parts_diagram) AS ReplaceDiagram,
                                parts_image,
                                parts_imagetext,
                                0 As IsCustom
                            FROM PartsFiche 
                            WHERE                                    
                                parts_vid=?                                                                 
                            UNION
                            SELECT DISTINCT 
                                parts_diagram,
                                (SELECT TOP 1 AdditionalMG FROM NotesDiagrams WHERE Diagram=parts_diagram) AS MG,
                                (SELECT TOP 1 ReplaceDiagram FROM NotesDiagrams WHERE Diagram=parts_diagram) AS ReplaceDiagram,
                                parts_image,
                                parts_imagetext,
                                0 As IsCustom
                            FROM PartsFiche
                            WHERE 
                                parts_vid=?
                                AND Len((SELECT TOP 1 AdditionalMG FROM NotesDiagrams WHERE Diagram=parts_diagram))>0
                            ORDER BY MG, IsCustom, parts_diagram";
        
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@parts_vid", m_vid);
            cmd.Parameters.AddWithValue("@parts_vid", m_vid);
        
        OleDbDataReader dr = cmd.ExecuteReader();

        string mg = "";
        string parts_Diagram;
        string parts_Diagram_mg;

        int Column = 0;

        while (dr.Read())
        {
            parts_Diagram = dr["parts_diagram"].ToString();
            parts_Diagram_mg = dr["MG"].ToString(); // for fiche and aftermarket we show the parts_diagram_mg            

            if (mg != parts_Diagram_mg)
            {
                // new hg  ---> start a new row and show the mg                                        
                if (Column == 1) // finish the last column if any
                {
                    txt.Append("<td style=\"width: 202px; background-color: #fff; border: 2px #bbb solid;\">&nbsp;</td></tr>");
                }

                mg = parts_Diagram_mg;

                Column = 0;
                txt.Append("<tr><td align=\"center\" colspan=\"2\" style=\"width: 404px; max-width: 404px; background-color: #bbb; color: #333; height: 18px; font-size: 12px; font-weight: bold;\">");
                txt.Append("<a id=\"A_" + mg + "\"></a>" + mg + "-" + GetMGDescription(mg));
                txt.Append("</td></tr>");
            }

            if (Column == 0)
            {
                txt.Append("<tr>");
            }

            txt.Append("<td align=\"center\" id=\"Diagram" + parts_Diagram + "\"" +
                        " onclick=\"if (document.getElementById('IFRAMERIGHTPANEL')!=null) document.getElementById('IFRAMERIGHTPANEL').src='DiagramsRight.aspx?rnd=" + m_rnd + "&vid=" + m_vid + "&diagram=" + parts_Diagram + "';\"" +
                        " onmouseover=\"this.style.border='2px solid #cc3333';\"" +
                        " onmouseout=\"this.style.border='2px solid #bbb';\"" +
                        " style=\"width: 202px; max-width: 202px; border: 2px #bbb solid; cursor: pointer; background-color: #fff;\">" +
                        "<a name=\"" + parts_Diagram + "\"></a>" +
                        "<img src=\"images/1pixel.gif\" alt=\"\" style=\"height: 3px;\" /><br />" +
                        "<label style=\"font-size: 10px; font-weight: normal;\">" + dr["parts_imagetext"].ToString().ToUpper() + "</label>");

            if (dr["ReplaceDiagram"] != DBNull.Value && Convert.ToBoolean(dr["ReplaceDiagram"]) == true)
                txt.Append("<div style=\"width: 200px; height: 140px; text-align: center;\"><img style=\"max-height: 140px;\" src=\"NotesDiagrams/" + dr["parts_diagram"].ToString() + "_1_T.jpg\" alt=\"\" /></div>");
            else if (Convert.ToInt16(dr["IsCustom"]) == 0)
                txt.Append("<div style=\"width: 200px; height: 140px; text-align: center;\"><img style=\"max-height: 140px;\" src=\"DiagramsThumb/" + dr["parts_image"].ToString() + ".png?v=" + Utilities.VERSION + "\" alt=\"\" /></div>");
            else
                txt.Append("<div style=\"width: 200px; height: 140px; text-align: center;\"><img style=\"max-height: 140px;\" src=\"DiagramsCustom/" + dr["parts_image"].ToString() + "_T.jpg?v=" + Utilities.VERSION + "\" alt=\"\" /></div>");

            txt.Append("<label style=\"font-face: Arial; font-size: 11px; font-weight: normal;\">Diagram #" + dr["parts_diagram"].ToString() + "</label>" +
                        "<img src=\"images/1pixel.gif\" alt=\"\" style=\"height: 2px;\" /><br />" +
                        "</td>");

            if (Column == 1)
            {
                txt.Append("</tr>");
            }

            Column++;
            if (Column == 2)
                Column = 0;
        }

        if (Column == 1) // finish the last column if any
        {
            txt.Append("<td style=\"width: 202px; background-color: #fff; border: 2px #bbb solid;\">&nbsp;</td></tr>");
        }

        if (txt.Length == 0)
        {
            txt.Append("<tr><td><br /><br /><br />&nbsp;&nbsp;No items found for this model yet.<br />" +
                        "<br />&nbsp;&nbsp;We will be adding them soon...</td></tr>");
        }

        cmd.Dispose();
        return txt.ToString();
    }
        
    private string GetMGDescription(string MG)
    {
        if (MG.Length != 2)
            return "";

        string Description = "";

        switch (MG)
        {
            case "01":
                Description = "Literature";
                break;
            case "02":
                Description = "Service Items";
                break;
            case "11":
                Description = "Engine";
                break;
            case "12":
                Description = "Engine Electrics";
                break;
            case "13":
                Description = "Fuel Preparation";
                break;
            case "16":
                Description = "Fuel Supply";
                break;
            case "17":
                Description = "Cooling";
                break;
            case "18":
                Description = "Exhaust System";
                break;
            case "21":
                Description = "Clutch";
                break;
            case "22":
                Description = "Engine &amp; Transmission Mounting";
                break;
            case "23":
                Description = "Manual Transmission";
                break;
            case "24": // cars only
                Description = "Auto. Transmission";
                break;
            case "25": // cars only
                Description = "Gearshift";
                break;
            case "26":
                Description = "Drive Shaft";
                break;
            case "27":
                Description = "Chain Drive";
                break;
            case "31":
                Description = "Front Suspension";
                break;
            case "32":
                Description = "Steering";
                break;
            case "33":
                Description = "Rear Axle &amp; Suspension";
                break;
            case "34":
                Description = "Brakes";
                break;
            case "35":
                Description = "Pedals";
                break;
            case "36":
                Description = "Wheels";
                break;
            case "41": // cars only
                Description = "Bodywork";
                break;
            case "46":
                Description = "Frame, Fairing, Cases";
                break;
            case "51":
                Description = "Vehicle Trim";
                break;
            case "52":
                Description = "Seat";
                break;
            case "54": // cars only
                Description = "Roof &amp; Top";
                break;
            case "61":
                Description = "Electrical System";
                break;
            case "62":
                Description = "Instruments Dash";
                break;
            case "63":
                Description = "Lighting";
                break;
            case "64": // cars only
                Description = "Heating &amp; Air";
                break;
            case "65":
                Description = "GPS, Alarm, Radios";
                break;
            case "66": // cars only
                Description = "Distance &amp; Cruise Control";
                break;
            case "71":
                Description = "Equipment Parts";
                break;
            case "72":
                Description = "Riders Equipment";
                break;
            case "76":
                Description = "Apparel &amp; Gifts";
                break;
            case "77":
                Description = "Accessories";
                break;
            case "80":
                Description = "Accessories";
                break;
            case "82": // cars only
                Description = "Accessories";
                break;
            case "83":
                Description = "Maintenance &amp; Tools";
                break;
            case "84": // cars only
                Description = "Communication Sys.";
                break;
            case "91": // cars only
                Description = "Equipment";
                break;
        }
        return Description;
    }
}
