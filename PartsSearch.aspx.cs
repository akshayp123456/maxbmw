using System;
using System.Web;
using System.Data.OleDb;
using System.IO;
using System.Text;

public partial class PartsSearch : System.Web.UI.Page
{
    private OleDbConnection conn;

    protected string m_rnd;
    protected string m_source;
    protected string m_category;
    protected string m_brand;
    protected string m_vid;
    protected string m_parts;
    protected string m_product;
    protected string m_diagram;
    protected string m_where_clause;
    protected string m_SearchType;
    protected string[] m_aParts;


    protected void Page_Load(object sender, EventArgs e)
    {
        conn = new OleDbConnection(System.Configuration.ConfigurationManager.AppSettings["connString"]);
        conn.Open();

        m_rnd = DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString() + DateTime.Now.Millisecond.ToString();

        m_source = this.Page.ToString().ToLower().Replace("asp.parts", "").Replace("_aspx", "");

        m_category = Utilities.QueryString("category").ToLower();
        m_brand = Utilities.QueryString("brand").ToUpper();
        m_vid = Utilities.QueryString("vid");

        m_parts = Utilities.QueryString("parts").ToUpper().Trim();
        m_product = Utilities.QueryString("product").ToUpper();

        m_SearchType = Utilities.QueryString("searchtype").ToLower();
        if (m_SearchType == "")
            m_SearchType = "partnumber";

        m_diagram = "";

        if (m_parts != "" || m_product != "")
        {
            
            m_where_clause = "";

            if (m_product != "")
            {
                if (m_product.Substring(0, 6) == "PARTS-")
                {
                    int posdash = m_product.Substring(6, m_product.Length - 6).IndexOf('-') + 6;
                    m_category = m_product.Substring(6, posdash - 6).ToLower();
                    m_parts = m_product.Substring(posdash + 1, m_product.Length - posdash - 1);
                }
                else if (m_product.Substring(0, 9) == "DIAGRAMS-")
                {
                    int posdash = m_product.Substring(9, m_product.Length - 9).IndexOf('-') + 9;
                    m_category = m_product.Substring(9, posdash - 9).ToLower();
                    m_diagram = m_product.Substring(posdash + 1, m_product.Length - posdash - 1);
                    m_parts = "";
                }
            }

            m_parts = m_parts.Replace("\r", "");
            m_aParts = m_parts.Split('\n', ',', ';');

            if (m_category == "tires")
            {
                m_where_clause += "AND parts_diagram_mg='WT' ";
            }
            if (m_category == "fiche")
            {
                m_where_clause += "AND (parts_diagram_mg>'00' AND parts_diagram_mg<'99') AND parts_diagram!='76' ";
            }
            if (m_category == "catalog")
            {
                m_where_clause += "AND (parts_diagram_mg<'00' OR parts_diagram_mg>'99') AND parts_diagram!='WT' ";
            }
            if (m_category == "apparel")
            {
                m_where_clause += "AND parts_diagram_mg='76' ";
            }

            if (m_brand != "")
            {
                m_where_clause += "AND parts_brand='" + m_brand + "' ";
            }

            if (m_vid != "")
            {
                m_where_clause += "AND parts_vid='" + m_vid + "' ";
            }
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
    
    private string GetPartDetails(string PartNumber, int RowID, string newPartNumber, string Prohibited, ref string Weight)
    {
        string TR_HTML = "";
        string NoteID = "";
        string Comments = "";
        string[] YouTubeLinks = "".Split('.');

        if (newPartNumber.Length > 0)
            Comments += PartNumber + " was superseded by " + newPartNumber + ".<br />";

        if (Prohibited == "P")
            Comments += "We cannot ship paints or hazardous materials." + "<br />";

        if (Prohibited == "L")
            Comments += "Needs to be coded/programmed by your dealer." + "<br />";

        // search for NotesParts
        OleDbCommand cmdNotes = new OleDbCommand();
        cmdNotes.Connection = conn;
        cmdNotes.CommandText = @"SELECT ID, Description, Comments, YouTubeLinks, Weight, Dimensions, AdditionalShipping, CommentsShipping  FROM NotesParts WHERE PartNumber=?";
        cmdNotes.Parameters.Clear();
        cmdNotes.Parameters.AddWithValue("@PartNumber", PartNumber);
        OleDbDataReader drNotes = cmdNotes.ExecuteReader();

        if (drNotes.Read())
        {
            NoteID = drNotes["ID"].ToString();
            YouTubeLinks = drNotes["YouTubeLinks"].ToString().Split(',', ';');
            Weight = (drNotes["Weight"] != DBNull.Value && (double)drNotes["Weight"] > 0.0 ? string.Format("{0:0.00}", (double)drNotes["Weight"]) : Weight);
            Comments += (drNotes["Dimensions"].ToString().Length > 0 ? "<label style=\"color: #e00; font-weight: bold;\">Part's size is \"" + drNotes["Dimensions"].ToString() + "\" (affects shipping costs)</label>" + "<br />" : "");
            Comments += (drNotes["AdditionalShipping"] != DBNull.Value && (double)drNotes["AdditionalShipping"] > 0.0 ? "<label style=\"color: #e00; font-weight: bold;\">Additional shipping charges of " + string.Format("{0:C}", (double)drNotes["AdditionalShipping"]) + " each apply!</label>" + "<br />" : "");
            Comments += (drNotes["CommentsShipping"].ToString().Length > 0 ? drNotes["CommentsShipping"].ToString() + "<br />" : "");
            Comments += (drNotes["Comments"].ToString().Length > 0 ? drNotes["Comments"].ToString() + "<br />" : "");
        }
        drNotes.Close();
        cmdNotes.Dispose();

        if (NoteID.Length > 0 || Comments.Length > 0)
        {
            TR_HTML = "<table cellpadding=\"0\" cellspacing=\"3\" style=\"width: 510px; font-size: 11px;\">";
            
            TR_HTML += "<tr><td valign=\"top\" align=\"left\">" + Comments + "</td></tr>";

            if (NoteID.Length > 0)
            {
                // could have images, check it
                TR_HTML += "<tr><td valign=\"top\" align=\"left\">";
                TR_HTML += ShowPartNoteImage(PartNumber);
                TR_HTML += ShowPartNoteYouTube(YouTubeLinks);
                TR_HTML += ShowPartNotePDF(PartNumber);
                TR_HTML += "</td></tr>";
            }

            TR_HTML += "</table>";
        }

        return TR_HTML;
    }

    private string ShowPartNoteImage(string PartNumber)
    {
        string txt = "";
        int counter = 0;
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
            txt += "<br />";
        return txt;
    }

    private string ShowPartNotePDF(string PartNumber)
    {
        string txt = "";
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

    // Checks if part exists on the Mobile Tradition table
    public bool MTAvailable(string PartNumber)
    {
        bool AvailableInMT = false;
        OleDbCommand cmd = new OleDbCommand();
        cmd.Connection = conn;

        cmd.CommandText = @"SELECT TOP(1) Retail FROM PriceListMT WHERE PartNumber=?";

        cmd.Parameters.Clear();
        cmd.Parameters.AddWithValue("@PartNumber", PartNumber);
        OleDbDataReader dr = cmd.ExecuteReader();
        if (dr.Read())
        {
            double Retail = (double)dr["Retail"];
            if (Retail > 0.0)
            {
                AvailableInMT = true;
            }
        }
        dr.Close();
        cmd.Dispose();
        return AvailableInMT;
    }

    public string ShowParts()
    {
        StringBuilder txt = new StringBuilder("");

        if (m_aParts != null && m_aParts.Length > 0 && m_aParts[0] != "")
        {
            StringBuilder trs = new StringBuilder("");

            string PartNumber;
            string newPartNumber;
            string Description;
            string Prohibited;
            string Weight;
            bool NLA;
            string NLA_date;
            string part;
            string Price;
            string OriginalPrice;
            bool IsSpecial;
            string SpecialsText;
            string PriceListDescription = null;

            OleDbCommand cmd = new OleDbCommand();
            cmd.Connection = conn;

            bool PartFoundInFiche;
            int RowID = 0;
            string bg_color;
            string border_bottom;
            string PartDetails;

            for (int i = 0; i < m_aParts.Length; i++)
            {
                PartFoundInFiche = false;

                bg_color = ((RowID % 2) == 0 ? "#fff" : "#ddd");

                part = m_aParts[i];

                // convert 11 digits BMW numbers that have spaces into a pure 11 digits BMW number
                if (part.Length > 11 && part.Replace(" ", "").Length == 11)
                {
                    double result;
                    if (Double.TryParse(part.Replace(" ", ""), System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.CurrentCulture, out result) == true)
                    {
                        part = part.Replace(" ", "");
                    }
                }

                cmd.CommandText = @"SELECT TOP 1 
                                        parts_partnumber,
                                        parts_partnumber_new,
                                        parts_description,
                                        parts_nla,
                                        parts_nla_date,
                                        parts_prohibited,
                                        parts_weight_lb,
                                        0 AS IsCustom  
                                    FROM PartsFiche
                                    WHERE 1=1 " + m_where_clause + @"AND parts_partnumber=?
                                    UNION
                                    SELECT TOP 1 
                                        parts_partnumber,
                                        parts_partnumber_new,
                                        parts_description,
                                        parts_nla,
                                        parts_nla_date,
                                        parts_prohibited,
                                        parts_weight_lb,
                                        1 AS IsCustom 
                                    FROM PartsCustom
                                    WHERE 1=1 " + m_where_clause + @"AND parts_partnumber=?
                                    ORDER BY IsCustom";

                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@parts_partnumber", part);
                cmd.Parameters.AddWithValue("@parts_partnumber", part);
                OleDbDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    PartFoundInFiche = true;

                    PartNumber = dr["parts_partnumber"].ToString();
                    Description = dr["parts_description"].ToString();
                    NLA = (bool)dr["parts_nla"];
                    NLA_date = dr["parts_nla_date"].ToString();
                    newPartNumber = (NLA_date != "" ? dr["parts_partnumber_new"].ToString() : "");
                    Prohibited = dr["parts_prohibited"].ToString().Trim();
                    Weight = string.Format("{0:0.00}", (double)dr["parts_weight_lb"]);
                    Weight = (Weight == "0.00" ? "&nbsp;" : Weight);

                    IsSpecial = Utilities.GetPrice(PartNumber, out Price, out OriginalPrice, out SpecialsText, out PriceListDescription, ref conn);

                    PartDetails = GetPartDetails(PartNumber, RowID, newPartNumber, Prohibited, ref Weight);
                    border_bottom = " border-bottom: solid 1px " + (PartDetails == "" ? "#666;" : "#ccc;");

                    trs.Append("<tr style=\"vertical-align: middle; font-family: Arial; font-size: 11px; background-color: " + bg_color + ";\">" +
                                "<td align=\"left\" style=\"width: 75px; border-left: solid 1px #ccc;" + border_bottom + " font-weight: bold;\" class=\"PartNumber\">&nbsp;" + PartNumber + "<a style=\"text-decoration: none; color: #000\" alt=\"\" href=\"#P" + PartNumber + "\">&nbsp;<img alt=\"\" src=\"images/ArrowDown.png\" border=\"0\" /></a></td>" +
                                "<td align=\"left\" style=\"width: 285px; border-left: solid 1px #ccc;" + border_bottom + " font-family: Arial Narrow;\">&nbsp;" + Description + "</td>" +
                                "<td align=\"right\" style=\"width: 30px; border-left: solid 1px #ccc;" + border_bottom + "\">" + Weight + "&nbsp;</td>");

                    if (!NLA && Prohibited == "" && Price.Length > 0 && Convert.ToDouble(Price) > 0.0)
                    {
                        trs.Append("<td align=\"right\" style=\"border-left: solid 1px #ccc;" + border_bottom + " width: 70px;" + (IsSpecial ? "font-weight: bold; color: #e60;" : "") + "\"" +
                                   (IsSpecial ? " title=\"" + SpecialsText + "\"" : "") + ">$" + Price +
                                   "<img style=\"cursor: pointer;\" src=\"images/addtocart.gif\" alt=\"ADD TO CART\"" +
                                   " onclick=\"AddToCart('" + PartNumber + "','1','" + HttpContext.Current.Server.UrlEncode(Description) + "','" + Weight + "');\"" +
                                   "/></td>");
                    }
                    else
                    {
                        // Check Mobile Tradition First - new change on 11/21/2024
                        if (MTAvailable(PartNumber))
                        {
                            trs.Append("<td align=\"right\" style=\"border-left: solid 1px #ccc;" + border_bottom + " width: 70px;" + (IsSpecial ? "font-weight: bold; color: #e60;" : "") + "\"" +
                                   (IsSpecial ? " title=\"" + SpecialsText + "\"" : "") + ">$" + Price +
                                   "<img style=\"cursor: pointer;\" src=\"images/addtocart.gif\" alt=\"ADD TO CART\"" +
                                   " onclick=\"AddToCart('" + PartNumber + "','1','" + HttpContext.Current.Server.UrlEncode(Description) + "','" + Weight + "');\"" +
                                   "/></td>");
                        }
                        else
                        {
                            if (NLA_date.Length == 6)
                                trs.Append("<td align=\"center\" style=\"border-left: solid 1px #ccc;" + border_bottom + " width: 70px; color: #f00; font-weight: bold; cursor: help;\" title=\"No Longer Available (NLA) since " + NLA_date + "\">NA<img alt=\"No Longer Available (NLA) since " + NLA_date.Substring(4, 2) + "/" + NLA_date.Substring(0, 4) + "\" src=\"images/Help.png\" /></td>");
                            else
                                trs.Append("<td align=\"center\" style=\"border-left: solid 1px #ccc;" + border_bottom + " width: 70px; color: #f00; font-weight: bold;\">NA</td>");
                        }
                    }
                    trs.Append("</tr>");

                    if (PartDetails != "")
                    {
                        trs.Append("<tr style=\"background-color: " + bg_color + ";\"><td align=\"left\" colspan=\"4\" style=\"border-bottom: solid 1px #666;\">");
                        trs.Append(PartDetails);
                        trs.Append("</td></tr>");
                    }
                }
                dr.Close();

                // If the part was not found in the PartsFiche nor on PartsCustom
                // then look for it in the PriceList
                if (!PartFoundInFiche)
                {
                    PartNumber = part;   // example: 001-NCOM-04

                    IsSpecial = Utilities.GetPrice(PartNumber, out Price, out OriginalPrice, out SpecialsText, out PriceListDescription, ref conn);

                    newPartNumber = "";
                    NLA_date = "";
                    Prohibited = "";
                    Weight = "&nbsp;";

                    PartDetails = GetPartDetails(PartNumber, RowID, newPartNumber, Prohibited, ref Weight);

                    if (Price.Length > 0 && Convert.ToDouble(Price) > 0.0)
                    {
                        border_bottom = " border-bottom: solid 1px " + (PartDetails == "" ? "#666;" : "#ccc;");

                        trs.Append("<tr style=\"vertical-align: middle; font-family: Arial; font-size: 11px; background-color: " + bg_color + ";\">" +
                                    "<td align=\"left\" style=\"width: 75px; border-left: solid 1px #ccc;" + border_bottom + " font-weight: bold;\" class=\"PartNumber\">&nbsp;" + PartNumber + "</td>" +
                                    "<td align=\"left\" style=\"width: 285px; border-left: solid 1px #ccc;" + border_bottom + " font-family: Arial Narrow;\">&nbsp;" + PriceListDescription + "</td>" +
                                    "<td align=\"right\" style=\"width: 30px; border-left: solid 1px #ccc;" + border_bottom + "\">" + Weight + "&nbsp;</td>");

                        trs.Append("<td align=\"right\" style=\"border-left: solid 1px #ccc;" + border_bottom + " width: 70px;" + "\">$" + Price +
                                       "<img style=\"cursor: pointer;\" src=\"images/addtocart.gif\" alt=\"ADD TO CART\"" +
                                       " onclick=\"AddToCart('" + PartNumber + "','1','" + HttpContext.Current.Server.UrlEncode(PriceListDescription) + "','" + Weight + "');\"" +
                                       "/></td>");
                        trs.Append("</tr>");

                        if (PartDetails != "")
                        {
                            trs.Append("<tr style=\"background-color: " + bg_color + ";\"><td colspan=\"4\" style=\"border-bottom: solid 1px #666;\">");
                            trs.Append(PartDetails);
                            trs.Append("</td></tr>");
                        }
                    }
                }

                RowID++;
            } // for loop
            cmd.Dispose();

            // Main table and header
            if (trs.Length > 0)
            {
                // table and header
                txt.Append("<table cellpadding=\"0\" cellspacing=\"0\" style=\"width: 512px; border: solid 1px #666;\">" +
                        "<tr style=\"font-family: Arial; font-size: 11px; font-weight: bold; background-color: #666; color: #fff;\">" +
                            "<td align=\"center\" style=\"width: 75px; border: solid 1px #666;\">Part Number</td>" +
                            "<td align=\"left\"   style=\"width: 285px; border: solid 1px #666;\">&nbsp;&nbsp;Description</td>" +
                            "<td align=\"center\" style=\"width: 30px; border: solid 1px #666; cursor: help;\" title=\"Weight in Pounds\">lb</td>" +
                            "<td align=\"center\" style=\"border: solid 1px #666;\">Each</td>" +
                        "</tr>" + trs + "</table>");
            }
            else
            {
                txt.Append("<table cellpadding=\"0\" cellspacing=\"0\" style=\"border: solid 1px #666\">" +
                        "<tr>" +
                            "<td>No parts found</td>" +
                        "</tr>" + "</table>");
            }
        }
        return txt.ToString();
    }

    public string ShowDiagramsByParts()
    {
        StringBuilder txt = new StringBuilder("");
        if (m_aParts != null && m_aParts.Length > 0 && m_aParts[0] != "")
        {
            string part;

            string vid;
            string Diagram;
            string image;
            string imagetext;

            for (int i = 0; i < m_aParts.Length; i++)
            {
                part = m_aParts[i];

                // convert 11 digits BMW numbers that have spaces into a pure 11 digits BMW number
                if (part.Length > 11 && part.Replace(" ", "").Length == 11)
                {
                    double result;
                    if (Double.TryParse(part.Replace(" ", ""), System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.CurrentCulture, out result) == true)
                    {
                        part = part.Replace(" ", "");
                    }
                }

                txt.Append("<a name=\"P" + part + "\"></a>");
                txt.Append("<span style=\"font-weight: bold; font-size: 14px; color: #000;\">Diagrams for Part Number '" + FormatPartNumber(part) + "':</span><br />");

                OleDbCommand cmd = new OleDbCommand();
                cmd.Connection = conn;

                cmd.CommandText = @"SELECT  parts_vid,
                                            (SELECT MIN(fztyp_prodmin) FROM etk_fztyp WHERE fztyp_mospid=parts_vid) AS fztyp_prodmin,
                                            (SELECT MAX(fztyp_prodmax) FROM etk_fztyp WHERE fztyp_mospid=parts_vid) AS fztyp_prodmax,
                                            (SELECT TOP 1 fztyp_ktlgausf FROM etk_fztyp WHERE fztyp_mospid=parts_vid) AS fztyp_ktlgausf,
                                            (SELECT TOP 1 fztyp_baureihe FROM etk_fztyp WHERE fztyp_mospid=parts_vid) AS fztyp_baureihe,
                                            (SELECT TOP 1 fztyp_model FROM etk_fztyp WHERE fztyp_mospid=parts_vid) AS fztyp_model,
                                            parts_diagram,
                                            parts_diagram_mg,
                                            parts_image,
                                            parts_imagetext,
                                            parts_brand,
                                            0 AS IsCustom
                                    FROM PartsFiche
                                    WHERE 1=1 " + m_where_clause + @"AND parts_partnumber=?
                                    UNION
                                    SELECT  parts_vid,                                            
                                            (SELECT MIN(fztyp_prodmin) FROM etk_fztyp WHERE fztyp_mospid=parts_vid) AS fztyp_prodmin,
                                            (SELECT MAX(fztyp_prodmax) FROM etk_fztyp WHERE fztyp_mospid=parts_vid) AS fztyp_prodmax,
                                            (SELECT TOP 1 fztyp_ktlgausf FROM etk_fztyp WHERE fztyp_mospid=parts_vid) AS fztyp_ktlgausf,
                                            (SELECT TOP 1 fztyp_baureihe FROM etk_fztyp WHERE fztyp_mospid=parts_vid) AS fztyp_baureihe,
                                            (SELECT TOP 1 fztyp_model FROM etk_fztyp WHERE fztyp_mospid=parts_vid) AS fztyp_model,
                                            parts_diagram,
                                            parts_diagram_mg,
                                            parts_image,
                                            parts_imagetext,
                                            parts_brand,
                                            1 AS IsCustom
                                    FROM PartsCustom
                                    WHERE 1=1 " + m_where_clause + @"AND parts_partnumber=?
                                    ORDER BY parts_vid, IsCustom, parts_diagram";

                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@parts_partnumber", part);
                cmd.Parameters.AddWithValue("@parts_partnumber", part);
                OleDbDataReader dr = cmd.ExecuteReader();

                int Column = 0;

                if (dr.HasRows == true)
                {
                    txt.Append("<table celpadding=\"0\" cellspacing=\"5\" border=\"0\">");

                    while (dr.Read())
                    {
                        Column++;
                        vid = dr["parts_vid"].ToString();
                        Diagram = dr["parts_diagram"].ToString();
                        imagetext = dr["parts_imagetext"].ToString();
                        
                        if (dr["parts_image"].ToString().IndexOf("B00") == 0)
                            image = "DiagramsThumb/" + dr["parts_image"].ToString() + ".png?rnd=" + m_rnd;
                        else
                            image = "DiagramsCustom/" + dr["parts_image"].ToString() + "_T.jpg?rnd=" + m_rnd;
                            

                        string Prod = "";
                        if (dr["fztyp_prodmin"].ToString().Length > 4 && dr["fztyp_prodmax"].ToString().Length > 4)
                        {
                            Prod = dr["fztyp_prodmin"].ToString().Substring(4, 2) + "/" + dr["fztyp_prodmin"].ToString().Substring(2, 2) + "-" + dr["fztyp_prodmax"].ToString().Substring(4, 2) + "/" + dr["fztyp_prodmax"].ToString().Substring(2, 2);
                        }
                                               
                        if (Column == 1)
                        {
                            txt.Append("<tr>");
                        }

                        string icon = GetSourceIcon(dr["parts_diagram_mg"].ToString());
                        string url = "";
                        if (icon == "Fiche")
                        {
                            url = "DiagramsMain.aspx?vid=" + vid + "&diagram=" + Diagram + "&partnumber=" + part;
                        }
                        else
                        {
                            // Example: http://localhost/fiche/PartsDetails.aspx?source=tires&diagram=WT_CO_TKC80_TWINDURO_F
                            url = "PartsDetails.aspx?source=" + icon.ToLower() + (vid != "" ? "&vid=" + vid : "") + "&diagram=" + Diagram;
                        }

                        txt.Append("<td align=\"center\" style=\"cursor: pointer; font-weight: bold; font-size: 10px; width: 222px;\" onclick=\"window.location='" + url + "';\">");

                        txt.Append("<table cellspacing=\"0\" cellpadding=\"0\" border=\"0\">");

                        txt.Append("<tr valign=\"middle\">");
                        txt.Append("<td class=\"SearchResultDiagram FirstRow Left\">");
                        txt.Append("<img src=\"images/Icon" + icon + "Small.png\" title=\"" + icon + "\" />");
                        txt.Append("</td>");
                        txt.Append("<td class=\"SearchResultDiagram FirstRow Right\">");
                        txt.Append(dr["parts_imagetext"].ToString());
                        txt.Append("</td>");
                        txt.Append("</tr>");

                        txt.Append("<tr>");
                        txt.Append("<td colspan=\"2\" align=\"center\" class=\"SearchResultDiagram SecondRow\">");
                        txt.Append(dr["parts_brand"].ToString() + " - " + dr["parts_diagram"].ToString());
                        txt.Append("</td>");
                        txt.Append("</tr>");

                        if (dr["fztyp_model"].ToString() != "")
                        {
                            txt.Append("<tr>");
                            txt.Append("<td colspan=\"2\" align=\"center\" class=\"SearchResultDiagram ThirdRow\">");

                            txt.Append("Fits: " + dr["fztyp_model"].ToString() + (dr["fztyp_baureihe"].ToString() != "" ? "&nbsp;&nbsp;(" + dr["fztyp_baureihe"].ToString() + ")" : "") +
                                       "<br />" + Prod + "&nbsp;&nbsp;" + dr["fztyp_ktlgausf"].ToString());
                            txt.Append("</td>");
                            txt.Append("</tr>");
                        }

                        txt.Append("<tr>");
                        txt.Append("<td colspan=\"2\" style=\"border: 1px solid #262626;\" align=\"center\">");
                        txt.Append("<img border=\"0\" alt=\"\" src=\"" + image + "\" />");
                        txt.Append("</td>");
                        txt.Append("</tr>");

                        txt.Append("</table>");

                        txt.Append("</td>");

                        if (Column == 3)
                        {
                            Column = 0;
                            txt.Append("</tr>");
                        }
                    }
                    txt.Append("</table><br /><br />");
                }
                else
                {
                    txt.Append("No diagrams found.<br />");
                }
                dr.Close();
                dr.Dispose();
                cmd.Dispose();
            }
        }
        return txt.ToString();
    }
        
    public string ShowDiagramsByDiagrams()
    {
        StringBuilder txt = new StringBuilder("");
        if (m_diagram != "")
        {
            string vid;
            string image;


            txt.Append("<a name=\"P" + m_diagram + "\"></a>");
            //txt.Append("<span style=\"font-weight: bold; font-size: 12px;\">Diagram: " + m_diagram + "</span><br />");

            OleDbCommand cmd = new OleDbCommand();
            cmd.Connection = conn;

            cmd.CommandText = @"SELECT  parts_vid,
                                        (SELECT MIN(fztyp_prodmin) FROM etk_fztyp WHERE fztyp_mospid=parts_vid) AS fztyp_prodmin,
                                        (SELECT MAX(fztyp_prodmax) FROM etk_fztyp WHERE fztyp_mospid=parts_vid) AS fztyp_prodmax,
                                        (SELECT TOP 1 fztyp_ktlgausf FROM etk_fztyp WHERE fztyp_mospid=parts_vid) AS fztyp_ktlgausf,
                                        (SELECT TOP 1 fztyp_baureihe FROM etk_fztyp WHERE fztyp_mospid=parts_vid) AS fztyp_baureihe,
                                        (SELECT TOP 1 fztyp_model FROM etk_fztyp WHERE fztyp_mospid=parts_vid) AS fztyp_model,
                                        parts_diagram,
                                        parts_diagram_mg,
                                        parts_image,
                                        parts_imagetext,
                                        parts_brand,
                                        0 AS IsCustom
                                FROM PartsFiche
                                WHERE 1=1 " + m_where_clause + @"AND parts_diagram=?
                                UNION
                                SELECT  parts_vid,                                            
                                        (SELECT MIN(fztyp_prodmin) FROM etk_fztyp WHERE fztyp_mospid=parts_vid) AS fztyp_prodmin,
                                        (SELECT MAX(fztyp_prodmax) FROM etk_fztyp WHERE fztyp_mospid=parts_vid) AS fztyp_prodmax,
                                        (SELECT TOP 1 fztyp_ktlgausf FROM etk_fztyp WHERE fztyp_mospid=parts_vid) AS fztyp_ktlgausf,
                                        (SELECT TOP 1 fztyp_baureihe FROM etk_fztyp WHERE fztyp_mospid=parts_vid) AS fztyp_baureihe,
                                        (SELECT TOP 1 fztyp_model FROM etk_fztyp WHERE fztyp_mospid=parts_vid) AS fztyp_model,
                                        parts_diagram,
                                        parts_diagram_mg,
                                        parts_image,
                                        parts_imagetext,
                                        parts_brand,
                                        1 AS IsCustom
                                FROM PartsCustom
                                WHERE 1=1 " + m_where_clause + @"AND parts_diagram=?
                                ORDER BY parts_vid, IsCustom, parts_diagram";

            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@parts_diagram", m_diagram);
            cmd.Parameters.AddWithValue("@parts_diagram", m_diagram);
            OleDbDataReader dr = cmd.ExecuteReader();

            int Column = 0;

            if (dr.HasRows == true)
            {
                txt.Append("<table celpadding=\"0\" cellspacing=\"5\" border=\"0\">");

                while (dr.Read())
                {
                    Column++;
                    vid = dr["parts_vid"].ToString();

                    if (dr["parts_image"].ToString().IndexOf("B000") == 0)
                        image = "DiagramsThumb/" + dr["parts_image"].ToString() + ".png";
                    else
                        image = "DiagramsCustom/" + dr["parts_image"].ToString() + "_T.jpg";

                    string Prod = "";
                    if (dr["fztyp_prodmin"].ToString().Length > 4 && dr["fztyp_prodmax"].ToString().Length > 4)
                    {
                        Prod = dr["fztyp_prodmin"].ToString().Substring(4, 2) + "/" + dr["fztyp_prodmin"].ToString().Substring(2, 2) + "-" + dr["fztyp_prodmax"].ToString().Substring(4, 2) + "/" + dr["fztyp_prodmax"].ToString().Substring(2, 2);
                    }

                    if (Column == 1)
                    {
                        txt.Append("<tr>");
                    }

                    string icon = GetSourceIcon(dr["parts_diagram_mg"].ToString());
                    string url = "";
                    if (icon == "Fiche")
                    {
                        url = "DiagramsMain.aspx?vid=" + vid + "&diagram=" + m_diagram;
                    }
                    else
                    {
                        // Example: http://localhost/fiche/PartsDetails.aspx?source=tires&diagram=WT_CO_TKC80_TWINDURO_F
                        url = "PartsDetails.aspx?source=" + icon.ToLower() + (vid != "" ? "&vid=" + vid : "") + "&diagram=" + m_diagram;
                    }

                    txt.Append("<td align=\"center\" style=\"cursor: pointer; font-weight: bold; font-size: 10px; width: 222px;\" onclick=\"window.location='" + url + "';\">");

                    txt.Append("<table cellspacing=\"0\" cellpadding=\"0\" border=\"0\">");

                    txt.Append("<tr valign=\"middle\">");
                    txt.Append("<td class=\"SearchResultDiagram FirstRow Left\">");
                    txt.Append("<img src=\"images/Icon" + icon + "Small.png\" title=\"" + icon + "\" />");
                    txt.Append("</td>");
                    txt.Append("<td class=\"SearchResultDiagram FirstRow Right\">");
                    txt.Append(dr["parts_imagetext"].ToString());
                    txt.Append("</td>");
                    txt.Append("</tr>");

                    txt.Append("<tr>");
                    txt.Append("<td colspan=\"2\" align=\"center\" class=\"SearchResultDiagram SecondRow\">");
                    txt.Append(dr["parts_brand"].ToString() + " - " + dr["parts_diagram"].ToString());
                    txt.Append("</td>");
                    txt.Append("</tr>");

                    if (dr["fztyp_model"].ToString() != "")
                    {
                        txt.Append("<tr>");
                        txt.Append("<td colspan=\"2\" align=\"center\" class=\"SearchResultDiagram ThirdRow\">");
                        txt.Append("Fits: " + dr["fztyp_model"].ToString() + (dr["fztyp_baureihe"].ToString() != "" ? "&nbsp;&nbsp;(" + dr["fztyp_baureihe"].ToString() + ")" : "") +
                                    "<br />" + Prod + "&nbsp;&nbsp;" + dr["fztyp_ktlgausf"].ToString());
                        txt.Append("</td>");
                        txt.Append("</tr>");
                    }

                    txt.Append("<tr>");
                    txt.Append("<td colspan=\"2\" style=\"border: 1px solid #262626;\" align=\"center\">");
                    txt.Append("<img border=\"0\" alt=\"\" src=\"" + image + "\" />");
                    txt.Append("</td>");
                    txt.Append("</tr>");

                    txt.Append("</table>");

                    txt.Append("</td>");
                    if (Column == 3)
                    {
                        Column = 0;
                        txt.Append("</tr>");
                    }
                }
                txt.Append("</table>");
            }
            else
            {
                txt.Append("No diagrams found.<br />");
            }
            dr.Close();
            dr.Dispose();
            cmd.Dispose();
        }
        return txt.ToString();
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

    private string GetSourceIcon(string MG)
    {
        string icon = "Fiche";

        switch (MG)
        {
            case "WT":
                icon = "Tires";
                break;

            case "PE":
            case "OT":
            case "CS":
            case "EL":
            case "EX":
            case "LI":
            case "ST":
            case "SU":
                icon = "Catalog";
                break;

            case "76":
                icon = "Apparel";
                break;
        }
        return icon;
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
