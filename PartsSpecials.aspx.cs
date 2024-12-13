using System;
using System.Web;
using System.Data.OleDb;
using System.IO;
using System.Text;

public partial class PartsSpecials : System.Web.UI.Page
{
    private OleDbConnection conn;
    protected string m_rnd;
    protected string m_MG;
    protected string m_vid;
    protected string m_parts;
    protected string m_where_clause;
    protected string[] m_aParts;

    protected void Page_Load(object sender, EventArgs e)
    {
        conn = new OleDbConnection(System.Configuration.ConfigurationManager.AppSettings["connString"]);
        conn.Open();

        m_rnd = DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString() + DateTime.Now.Millisecond.ToString();

        m_MG = Utilities.QueryString("mg").ToUpper();
        m_vid = Utilities.QueryString("vid");
        m_parts = Utilities.QueryString("parts").ToUpper().Replace("\r", "");

        m_aParts = m_parts.Split('\n', ',', ';');

        m_where_clause = "";
            
        if (m_vid != "")
        {
            m_where_clause += " AND BikeVIDs LIKE '%" + m_vid + "%'";
        }

        if (m_aParts.Length>0)
        {
            string t = "";
            foreach (string p in m_aParts)
            {
                if (p != "")
                    t += "'" + p.Replace(" ", "") + "',";
            }
            if (t != "")
            {
                m_where_clause += " AND PartNumber IN(" + t.Substring(0, t.Length-1) + ")";
            }
        }

        if (m_MG != "" && m_MG.Length==2)
        {
            m_where_clause += " AND MGs LIKE '%" + m_MG + "%'";
        }
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


        
    protected string ShowParts()
    {
        StringBuilder txt = new StringBuilder("");
        
        OleDbCommand cmd = new OleDbCommand();
        cmd.Connection = conn;

        string PartNumber;
        string Description;
        string WeightFiche;
        string WeightCustom;
        string WeightNotes;
        string Weight;
        string Price;
        string SpecialsText;
        string border_bottom;
        string PartDetails;
        double dRetail;
        double dSpecialsPrice;
        double dSpecialsDiscount;
        string BikeVIDs;
        string MGs;
        string BikeModels;

        int RowID = 0;
        string bg_color = ((RowID % 2) == 0 ? "#fff" : "#ddd");

        cmd.CommandText = @"SELECT * FROM (
                            SELECT ID, PartNumber, Description, SpecialsComments, Comments, YouTubeLinks, Weight AS WeightNotes, Dimensions, AdditionalShipping, CommentsShipping,
                                (SELECT TOP 1 parts_image FROM PartsFiche WHERE PartsFiche.parts_partnumber=NotesParts.PartNumber) AS DiagramImageFiche,
                                (SELECT TOP 1 parts_image FROM PartsCustom WHERE PartsCustom.parts_partnumber=NotesParts.PartNumber) AS DiagramImageCustom,
                                (SELECT TOP 1 parts_weight_lb FROM PartsFiche WHERE PartsFiche.parts_partnumber=NotesParts.PartNumber) AS WeightFiche,
                                (SELECT TOP 1 parts_weight_lb FROM PartsCustom WHERE PartsCustom.parts_partnumber=NotesParts.PartNumber) AS WeightCustom,
                                (SELECT TOP 1 Retail FROM PriceList WHERE PriceList.PartNumber=NotesParts.PartNumber) AS Retail, SpecialsPrice, SpecialsDiscount, 
	                            STUFF(
		                            (
		                            SELECT DISTINCT
		                              ',' + parts_diagram_mg
		                            FROM (SELECT parts_partnumber, parts_diagram_mg FROM PartsFiche UNION SELECT parts_partnumber, parts_diagram_mg FROM PartsCustom) AS TableXXX WHERE TableXXX.parts_partnumber=NotesParts.PartNumber
		                            FOR XML PATH('')
		                            ), 1, 1, '') As MGs,
	                            STUFF(
		                            (
		                            SELECT DISTINCT
		                              ',' + CONVERT(varchar, parts_vid)
		                            FROM (SELECT parts_partnumber, parts_vid FROM PartsFiche UNION SELECT parts_partnumber, parts_vid FROM PartsCustom) AS TableYYY WHERE TableYYY.parts_partnumber=NotesParts.PartNumber
		                            FOR XML PATH('')
		                            ), 1, 1, '') As BikeVIDs
                            FROM NotesParts WHERE SpecialsShowInRightPanel=1 AND (SpecialsDiscount>0 OR SpecialsPrice>0)) AS OutputTable WHERE 1=1" + m_where_clause + " ORDER BY MGs DESC;";
                
        OleDbDataReader dr = cmd.ExecuteReader();

        while(dr.Read())
        {
            StringBuilder trs = new StringBuilder("");

            PartNumber = dr["PartNumber"].ToString();

            Description = dr["Description"].ToString().ToUpper();
            
            WeightNotes = (dr["WeightNotes"] == DBNull.Value ? "0.00" : string.Format("{0:0.00}", (double)dr["WeightNotes"]));
            WeightNotes = (WeightNotes == "0.00" ? "&nbsp;" : WeightNotes);

            Weight = "";
            if (WeightNotes != "&nbsp;")
            {
                Weight = WeightNotes + " lbs";
            }
            else 
            {
                WeightCustom = (dr["WeightCustom"] == DBNull.Value ? "0.00" : string.Format("{0:0.00}", (double)dr["WeightCustom"]));
                WeightCustom = (WeightCustom == "0.00" ? "&nbsp;" : WeightCustom);
                WeightFiche = (dr["WeightFiche"] == DBNull.Value ? "0.00" : string.Format("{0:0.00}", (double)dr["WeightFiche"]));
                WeightFiche = (WeightFiche == "0.00" ? "&nbsp;" : WeightFiche);
                Weight = (WeightCustom != "&nbsp;" ? WeightCustom : WeightFiche) + " lbs";
            }            

            BikeVIDs = (DBNull.Value != dr["BikeVIDs"] ? dr["BikeVIDs"].ToString() : "");

            BikeModels = GetBikeModels(BikeVIDs);
            
            MGs = (DBNull.Value != dr["MGs"] ? dr["MGs"].ToString() : "");

            dRetail = Convert.ToDouble(dr["Retail"]);
            dSpecialsPrice = (dr["SpecialsPrice"] != DBNull.Value ? Convert.ToDouble(dr["SpecialsPrice"]) : 0.0);
            dSpecialsDiscount = (dr["SpecialsDiscount"] != DBNull.Value ? Convert.ToDouble(dr["SpecialsDiscount"]) : 0.0);

            SpecialsText = "";
            if (dSpecialsDiscount != 0.0)
            {
                Price = (dRetail - ((dRetail * dSpecialsDiscount) / 100.0)).ToString("N");
                SpecialsText = Convert.ToDecimal(dSpecialsDiscount).ToString() + "% OFF";
            }
            else if (dSpecialsPrice != 0.0)
            {
                Price = dSpecialsPrice.ToString("N");
                SpecialsText = "Was $" + dRetail.ToString("N");
            }
            else
            {
                Price = dRetail.ToString("N");
            }

            SpecialsText += (dr["SpecialsComments"] != DBNull.Value ? " - " + dr["SpecialsComments"].ToString() : "");
            
            string NoteID = (DBNull.Value != dr["ID"] ? dr["ID"].ToString() : "");
            string Comments = (DBNull.Value != dr["Comments"] ? dr["Comments"].ToString() : "");
            string Dimensions = (DBNull.Value != dr["Dimensions"] ? dr["Dimensions"].ToString() : "");
            double AdditionalShipping = (DBNull.Value != dr["AdditionalShipping"] ? (double)dr["AdditionalShipping"] : 0.0);
            string CommentsShipping = (DBNull.Value != dr["CommentsShipping"] ? dr["CommentsShipping"].ToString() : "");
            string DiagramImageCustom = (DBNull.Value != dr["DiagramImageCustom"] ? dr["DiagramImageCustom"].ToString() : "");
            string DiagramImageFiche = (DBNull.Value != dr["DiagramImageFiche"] ? dr["DiagramImageFiche"].ToString() : "");
            string YouTubeLinks = (DBNull.Value != dr["YouTubeLinks"] ? dr["YouTubeLinks"].ToString() : "");
            PartDetails = GetPartDetails(NoteID, PartNumber, Comments, Dimensions, AdditionalShipping, CommentsShipping, DiagramImageCustom, DiagramImageFiche, BikeModels, BikeVIDs, YouTubeLinks, RowID);

            border_bottom = " border-bottom: solid 1px " + (PartDetails == "" ? "#666;" : "#ccc;");

            trs.Append("<tr style=\"vertical-align: middle; font-family: Arial; font-size: 11px; padding-top: 5px; background-color: " + bg_color + ";\">" +
                        "<td align=\"left\" style=\"width: 85px; border-left: solid 1px #ccc;" + border_bottom + " font-weight: bold;\" class=\"PartNumber\">&nbsp;" + FormatPartNumber(PartNumber) + "</td>" +
                        "<td align=\"left\" style=\"width: 310px; border-left: solid 1px #ccc;" + border_bottom + " font-family: Arial Narrow;\">&nbsp;" + Description + "</td>" +
                        "<td align=\"right\" style=\"width: 45px; border-left: solid 1px #ccc;" + border_bottom + "\">" + Weight + "&nbsp;</td>");
            
            trs.Append("<td align=\"right\" style=\"border-left: solid 1px #ccc;" + border_bottom + " width: 70px;font-weight: bold; color: #d00;\"" +
                        " title=\"" + SpecialsText + "\">$" + Price +
                        "<img style=\"cursor: pointer;\" src=\"images/addtocart.gif\" alt=\"ADD\" title=\"Add to cart\"" +
                        " onclick=\"AddToCart('" + PartNumber + "','1','" + HttpContext.Current.Server.UrlEncode(Description) + "','" + Weight + "');\"" +
                        "/></td>");

            trs.Append("</tr>");

            if (PartDetails != "")
            {
                trs.Append("<tr><td align=\"left\" colspan=\"4\">");
                trs.Append(PartDetails);
                trs.Append("</td></tr>");
            }

            txt.Append("<table cellpadding=\"0\" cellspacing=\"0\" style=\"margin-bottom: 8px; width: 600px; border: solid 1px #666;\">" + trs.ToString() + "</table>");
            RowID++;
        }
        dr.Close();
        cmd.Dispose();


        return txt.ToString();
    }

    private string GetBikeModels(string VIDs)
    {
        string model = "";

        if (VIDs != "")
        {
            OleDbCommand cmd = new OleDbCommand();
            OleDbDataReader dr;

            cmd.Connection = conn;
            cmd.CommandTimeout = 120;

            // get model and production            
            cmd.CommandText = @"SELECT DISTINCT fztyp_model, fztyp_ktlgausf FROM etk_fztyp WHERE fztyp_mospid in (" + VIDs + @")";
            dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                model += dr["fztyp_model"].ToString().Trim() + " " + (dr["fztyp_ktlgausf"].ToString().Trim() == "ECE" ? "ECE" : "") + ", ";
            }
            dr.Close();
            cmd.Dispose();
        }

        if (model != "")
            return model.Substring(0, model.Length - 2);
        
        return "";
    }


    private string GetPartDetails(  string NoteID, 
                                    string PartNumber,
                                    string Comments,
                                    string Dimensions, 
                                    double AdditionalShipping,
                                    string CommentsShipping,
                                    string DiagramImageCustom,
                                    string DiagramImageFiche,                                    
                                    string BikeModels,
                                    string VIDs,
                                    string YouTubeLinks,
                                    int RowID)
    {
        string TR_HTML = "";
        string txt = "";
        string txt2 = "";
        string[] aYouTubeLinks = "".Split('.');

        aYouTubeLinks = YouTubeLinks.Split(',', ';');
        txt += (Dimensions != "" ? "<label style=\"color: #e00; font-weight: bold;\">Part's size is \"" + Dimensions + "\" (affects shipping costs)</label>" + "<br />" : "");
        txt += (AdditionalShipping > 0.0 ? "<label style=\"color: #e00; font-weight: bold;\">Additional shipping charges of " + string.Format("{0:C}", AdditionalShipping) + " each apply!</label>" + "<br />" : "");
        txt += (CommentsShipping != "" ? CommentsShipping + "<br />" : "");
        txt += (Comments != "" ? Comments + "<br />" : "");
        txt2 += (BikeModels != "" ? "<strong>Fits models:</strong> " + BikeModels + "<br />" : "");
        //txt2 += (VIDs != "" ? "<strong>VIDs:</strong> " + VIDs.Replace(",",", ") + "<br />" : "");
        txt2 += ShowPartNoteImage(PartNumber, DiagramImageCustom, DiagramImageFiche);
        txt2 += ShowPartNoteYouTube(aYouTubeLinks);
        txt2 += ShowPartNotePDF(PartNumber);

        if (txt!="")
        {
            TR_HTML += "<tr><td valign=\"top\" align=\"left\">" + txt + "</td></tr>";
        }
        if (txt2 != "")
        {        
            TR_HTML += "<tr><td valign=\"top\" align=\"left\">" + txt2 + "</td></tr>";
        }

        if (TR_HTML != "")
        {
            TR_HTML = "<table cellpadding=\"0\" cellspacing=\"3\" style=\"width: 510px; font-size: 11px;\">" + TR_HTML + "</table>";
        }
        return TR_HTML;
    }

    private string ShowPartNoteImage(string PartNumber, string DiagramImageCustom, string DiagramImageFiche)
    {
        string txt = "";
        int counter = 0;

        PartNumber = PartNumber.Replace("\"", "");

        for (int i = 1; i <= 4; i++)
        {
            if (File.Exists(MapPath("NotesParts/" + PartNumber + "_" + i.ToString() + "_N.jpg")))
            {
                txt += "<img class=\"PartThumbnailImg\" alt=\"Part Thumbnail\" title=\"Click to view the large image\" src=\"NotesParts/" + PartNumber + "_" + i.ToString() + "_N.jpg?rnd=" + m_rnd + "\" />&nbsp;";
                counter++;
                if (counter == 2)
                    txt += "<br />";
            }
        }
        if (txt != "")
        {
            txt += "<br />";
        }
        else if (DiagramImageCustom != "")
        {
            // fetch by diagram image
            txt += "<img class=\"PartThumbnailImg\" alt=\"Part Thumbnail\" title=\"Click to view the large image\" src=\"DiagramsCustom/" + DiagramImageCustom + "_T.jpg?rnd=" + m_rnd + "\" />&nbsp;";
        }
        else if (DiagramImageFiche != "")
        {
            // fetch by diagram image
            txt += "<img class=\"PartThumbnailImg\" alt=\"Part Thumbnail\" title=\"Click to view the large image\" src=\"DiagramsThumb/" + DiagramImageFiche + ".png?rnd=" + m_rnd + "\" />&nbsp;";
        }

        return txt;
    }

    private string ShowPartNotePDF(string PartNumber)
    {
        string txt = "";

        PartNumber = PartNumber.Replace("\"", "");

        for (int i = 1; i <= 4; i++)
        {
            if (File.Exists(MapPath("NotesParts/" + PartNumber + "_" + i.ToString() + ".pdf")))
            {
                string pdfPath = "NotesParts/" + PartNumber + "_" + i.ToString() + ".pdf";
                txt += "<img class=\"ThumbnailPDF\" src=\"images/PDF.jpg\" alt=\"" + pdfPath + "\" />&nbsp;";
            }
        }
        if (txt != "")
            txt += "<br />";
        return txt;
    }

    private string ShowPartNoteYouTube(string[] YouTubeLinks)
    {
        string txt = "";
        
        for (int i = 0; i <= YouTubeLinks.GetUpperBound(0); i++)
        {
            if (YouTubeLinks[i].Length > 0)
            {
                txt += "<img class=\"ThumbnailVideo\" src=\"images/YouTubeVideos.jpg\" alt=\"" + YouTubeLinks[i] + "\" />&nbsp;";
            }
        }
        if (txt != "")
            txt += "<br />";
        return txt;
    }

    private string FormatPartNumber(string part)
    {
        string p = part;
        if (part.Length == 11)
        {
            double result;
            if (Double.TryParse(part, System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.CurrentCulture, out result))
            {
                // is a BMW part number
                p = p[0].ToString() + p[1].ToString() + " " + p[2] + p[3] + " " + p[4] + " " + p[5] + p[6] + p[7] + " " + p[8] + p[9] + p[10];
            }
        }
        return p;
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

    protected string GetMGDescription(string MG)
    {
        if (MG.Length != 2)
            return "";

        string Description = "";

        switch (MG.ToUpper())
        {
            case "01":
                Description = "01-Literature";
                break;
            case "11":
                Description = "11-Engine";
                break;
            case "12":
                Description = "12-Engine Electrics";
                break;
            case "13":
                Description = "13-Fuel Preparation";
                break;
            case "16":
                Description = "16-Fuel Supply";
                break;
            case "17":
                Description = "17-Cooling";
                break;
            case "18":
                Description = "18-Exhaust System";
                break;
            case "21":
                Description = "21-Clutch";
                break;
            case "22":
                Description = "22-Engine & Transmission";
                break;
            case "23":
                Description = "23-Manual Transmission";
                break;                      
            case "26":
                Description = "26-Drive Shaft";
                break;
            case "27":
                Description = "27-Chain Drive";
                break;
            case "31":
                Description = "31-Front Suspension";
                break;
            case "32":
                Description = "32-Steering";
                break;
            case "33":
                Description = "33-Rear Axle & Suspension";
                break;
            case "34":
                Description = "34-Brakes";
                break;
            case "35":
                Description = "35-Pedals";
                break;
            case "36":
                Description = "36-Wheels";
                break;           
            case "46":
                Description = "46-Frame, Fairing, Cases";
                break;
            case "51":
                Description = "51-Vehicle Trim";
                break;
            case "52":
                Description = "Seat";
                break;
            case "61":
                Description = "61-Electrical System";
                break;
            case "62":
                Description = "62-Instruments Dash";
                break;
            case "63":
                Description = "63-Lighting";
                break;
            case "65":
                Description = "65-GPS, Alarm, Radios";
                break;            
            case "71":
                Description = "71-Equipment Parts";
                break;
            case "72":
                Description = "72-Riders Equipment";
                break;
            case "76":
                Description = "76-Apparel";
                break;
            case "77":
                Description = "77-Accessories";
                break;
            case "80":
                Description = "80-Accessories";
                break;
            case "83":
                Description = "83-Service & Tools";
                break;
            case "WT":
                Description = "Tires";
                break;
            case "PE":
                Description = "Performance";
                break;
            case "EX":
                Description = "Exhaust";
                break;
            case "SU":
                Description = "Suspension";
                break;
            case "EL":
                Description = "Electrical";
                break;
            case "LI":
                Description = "Lighting";
                break;
            case "FA":
                Description = "Fairing";
                break;
            case "ST":
                Description = "Service & Tools";
                break;
            case "OT":
                Description = "Other";
                break;
        }
        return Description;
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
