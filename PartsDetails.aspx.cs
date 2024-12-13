using System;
using System.Configuration;
using System.Web;
using System.Data.OleDb;
using System.IO;
using System.Text;
using System.Collections.Generic;


public partial class PartsDetails : System.Web.UI.Page
{
    private OleDbConnection conn;

    protected string m_rnd;

    protected string m_BackButtonURL;
    protected string m_BackButtonCaption;

    protected string m_source;
    protected string m_category;
    protected string m_brand;
    protected string m_partnumber;
    protected string m_vid;
    protected string m_Diagram;

    protected string m_image;
    protected string m_imageBig;
    protected string m_imagetext;

    protected string m_DiagramCaption; // this will be the imagetext or the one from the diagram note (if any)

    protected string m_DiagramNoteComments;
    protected string m_DiagramNoteSpecs;

    protected List<string> m_ProductImages;
    protected List<string> m_ProductVideos;
    protected List<string> m_ProductPDFs;

    protected void Page_Load(object sender, EventArgs e)
    {
        Utilities.CheckValidURL();
        Utilities.SetLastURL();

        m_rnd = DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString() + DateTime.Now.Millisecond.ToString();

        m_DiagramNoteComments = "";
        m_DiagramNoteSpecs = "";
        m_DiagramCaption = "";

        m_ProductImages = new List<string>();
        m_ProductVideos = new List<string>();
        m_ProductPDFs = new List<string>();

        m_vid = Utilities.QueryString("vid");
        m_Diagram = Utilities.QueryString("diagram").ToUpper();
        m_source = Utilities.QueryString("source").ToLower();
        m_category = Utilities.QueryString("category");
        m_brand = Utilities.QueryString("brand");
        m_partnumber = Utilities.QueryString("partnumber").ToUpper();
        string viewmode = Utilities.QueryString("viewmode").ToUpper();

        // Build the back button captions and url
        switch (m_source)
        {
            case "catalog":
                m_BackButtonCaption = "Catalog";
                m_BackButtonURL = "PartsCatalog.aspx";                
                break;
            case "tires":
                m_BackButtonCaption = "Tires";
                m_BackButtonURL = "PartsTires.aspx";
                break;

            case "apparel":
                m_BackButtonCaption = "Apparel";
                m_BackButtonURL = "PartsApparel.aspx";
                break;

            default:
                m_BackButtonURL = "fiche.aspx";
                m_BackButtonCaption = "Main Menu";
                break;
        }
        
        string BackButtonQuerystring = "";

        if (m_vid != "")
            BackButtonQuerystring += (BackButtonQuerystring == "" ? "?" : "&") + "vid=" + m_vid;
        if (m_category != "")
            BackButtonQuerystring += (BackButtonQuerystring == "" ? "?" : "&") + "category=" + HttpUtility.UrlEncode(m_category);
        if (m_brand != "")
            BackButtonQuerystring += (BackButtonQuerystring == "" ? "?" : "&") + "brand=" + HttpUtility.UrlEncode(m_brand);
        if (m_Diagram != "")
            BackButtonQuerystring += (BackButtonQuerystring == "" ? "?" : "&") + "diagram=" + HttpUtility.UrlEncode(m_Diagram);
        if (viewmode != "")
            BackButtonQuerystring += (BackButtonQuerystring == "" ? "?" : "&") + "viewmode=" + viewmode;


        m_BackButtonURL += BackButtonQuerystring;


        if (m_vid != "" || m_Diagram != "")
        {
            conn = new OleDbConnection(ConfigurationManager.AppSettings["connString"]);
            conn.Open();

            string qry;
            OleDbCommand cmd = new OleDbCommand();
            cmd.Connection = conn;

            //**************************************
            // Retrieve the Diagram
            //**************************************
            if (m_Diagram.IndexOf("76") == 0)
            {
                // Apparel
                qry = @"SELECT parts_image, parts_imagetext, 0 AS IsCustom FROM PartsFiche 
                            WHERE parts_diagram=?
                        UNION
                        SELECT parts_image, parts_imagetext, 1 AS IsCustom FROM PartsCustom
                            WHERE parts_diagram=?";
                cmd.CommandText = qry;
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@parts_diagram", m_Diagram);
                cmd.Parameters.AddWithValue("@parts_diagram", m_Diagram);
            }
            else
            {
                if (m_vid == "")
                    m_vid = "0";

                qry = @"SELECT parts_image, parts_imagetext, 0 AS IsCustom FROM PartsFiche
                            WHERE parts_diagram=? AND parts_vid=?
                        UNION
                        SELECT parts_image, parts_imagetext, 1 AS IsCustom FROM PartsCustom
                            WHERE parts_diagram=? AND parts_vid=?
                        UNION
                        SELECT parts_image, parts_imagetext, 1 AS IsCustom FROM PartsCustom
                            WHERE parts_diagram=?
                        ORDER BY IsCustom";

                cmd.CommandText = qry;
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@parts_diagram", m_Diagram);
                cmd.Parameters.AddWithValue("@parts_vid", m_vid);
                cmd.Parameters.AddWithValue("@parts_diagram", m_Diagram);
                cmd.Parameters.AddWithValue("@parts_vid", m_vid);
                cmd.Parameters.AddWithValue("@parts_diagram", m_Diagram);
            }

            OleDbDataReader dr = cmd.ExecuteReader();

            m_image = "";
            m_imageBig = "";
            m_imagetext = "";

            if (dr.Read())
            {
                if (Convert.ToInt16(dr["IsCustom"]) == 0)
                {
                    m_image = (dr["parts_image"] != DBNull.Value ? "DiagramsMid/" + dr["parts_image"].ToString() + ".png" : "");
                    m_imageBig = (dr["parts_image"] != DBNull.Value ? "Diagrams/" + dr["parts_image"].ToString() + ".png" : "");
                }
                else
                {
                    m_image = (dr["parts_image"] != DBNull.Value ? "DiagramsCustom/" + dr["parts_image"].ToString() + "_M.jpg" : "");
                    m_imageBig = (dr["parts_image"] != DBNull.Value ? "DiagramsCustom/" + dr["parts_image"].ToString() + "_B.jpg" : "");
                }

                m_imagetext = (dr["parts_imagetext"] != DBNull.Value ? dr["parts_imagetext"].ToString() : "");
                m_DiagramCaption = m_imagetext;
            }
            dr.Close();
            
            //**************************************
            // Retrieve the DiagramNotes
            //**************************************                        
            qry = @"SELECT TOP 1 Description, Comments, Specs, YouTubeLinks FROM NotesDiagrams WHERE Diagram=?";
            cmd.CommandText = qry;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@Diagram", m_Diagram);
            dr = cmd.ExecuteReader();

            if (dr.Read())
            {
                m_DiagramCaption = (dr["Description"] != DBNull.Value && dr["Description"].ToString().Trim()!="" ? dr["Description"].ToString() : m_DiagramCaption);
                m_DiagramNoteComments = dr["Comments"].ToString();
                m_DiagramNoteSpecs = (dr["Specs"] == DBNull.Value ? "" : "<b>Specs:</b><br />" + dr["Specs"].ToString());

                string imgName;
                for (int i = 1; i <= 4; i++)
                {
                    imgName = m_Diagram + "_" + i;
                    if (File.Exists(MapPath("NotesDiagrams/" + imgName + "_N.jpg")))
                    {
                        m_ProductImages.Add("NotesDiagrams/" + imgName + "_N.jpg");
                    }
                }

                // check if there are any YouTube videos available
                string[] YouTubeLinks = dr["YouTubeLinks"].ToString().Split(',', ';');
                for (int i = 0; i <= YouTubeLinks.GetUpperBound(0); i++)
                {
                    if (YouTubeLinks[i].Length > 0)
                    {
                        m_ProductVideos.Add(YouTubeLinks[i]);
                    }
                }

                // look for PDFs for diagrams
                string pdfName;
                for (int i = 1; i <= 4; i++)
                {
                    pdfName = m_Diagram + "_" + i;
                    if (File.Exists(MapPath("NotesDiagrams/" + pdfName + ".pdf")))
                    {
                        m_ProductPDFs.Add("NotesDiagrams/" + pdfName + ".pdf");
                    }
                }
            }
            dr.Close();
            dr.Dispose();
        }
    }


    protected void Page_Unload(object sender, EventArgs e)
    {
        if (conn != null)
        {
            conn.Close();
            conn.Dispose();
        }
    }


    public string ShowParts()
    {
        StringBuilder txt = new StringBuilder("");
        StringBuilder trs = new StringBuilder("");

        string dpos;
        string PartNumber;
        string newPartNumber;
        string Description;
        string parts_from;
        string parts_to;
        string Qty;
        bool NLA;
        string NLA_date;
        string Prohibited;
        string Weight;
        string Price;
        bool IsSpecial;
        string SpecialsText;
        string PartDetails;
        string PartModelsFits;
        string PriceListDescription = null;

        OleDbCommand cmd = new OleDbCommand();
        cmd.Connection = conn;
        cmd.CommandText = "";

        if (m_Diagram.Length < 2)
            return "";

        if (m_Diagram.Substring(0, 2) == "76")
        {
            // Apparel - vid does not matter here
            cmd.CommandText = @"SELECT parts_pos, parts_diagram_pos, parts_partnumber, parts_partnumber_new, parts_description, parts_from, parts_to, parts_qty, parts_nla, parts_nla_date, parts_prohibited, parts_weight_lb,
                                        (SELECT TOP 1 Retail FROM PriceList WHERE PartNumber=parts_partnumber) As Retail,
                                        0 AS SpecificModel
                                    FROM PartsFiche
                                    WHERE parts_diagram=?
                                UNION
                                SELECT parts_pos, parts_diagram_pos, parts_partnumber, parts_partnumber_new, parts_description, parts_from, parts_to, parts_qty, parts_nla, parts_nla_date, parts_prohibited, parts_weight_lb,
                                        (SELECT TOP 1 Retail FROM PriceList WHERE PartNumber=parts_partnumber) As Retail,
                                        0 AS SpecificModel
                                    FROM PartsCustom
                                    WHERE parts_diagram=?
                                ORDER BY parts_pos";

            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@parts_diagram", m_Diagram);
            cmd.Parameters.AddWithValue("@parts_diagram", m_Diagram);
        }
        else
        {
            // Tires and Catalog
            if (m_vid != "0" && m_vid!="" && m_vid != "99999")
            {
                // a specific vid was used, so:
                // first for ALL models (vid=0)
                // then a specific model only
                cmd.CommandText = @"SELECT  parts_pos, parts_brand, parts_diagram_pos, parts_partnumber, parts_partnumber_new, parts_description, parts_from, parts_to, parts_qty, parts_nla, parts_nla_date, parts_prohibited, parts_weight_lb,
                                            (SELECT TOP 1 Retail FROM PriceList WHERE PartNumber=parts_partnumber) As Retail,
                                            0 AS SpecificModel                                        
                                        FROM PartsFiche
                                        WHERE parts_diagram=? AND parts_vid=0
                                    UNION
                                    SELECT  parts_pos, parts_brand, parts_diagram_pos, parts_partnumber, parts_partnumber_new, parts_description, parts_from, parts_to, parts_qty, parts_nla, parts_nla_date, parts_prohibited, parts_weight_lb,
                                            (SELECT TOP 1 Retail FROM PriceList WHERE PartNumber=parts_partnumber OR PartNumber=REPLACE(parts_partnumber,'-','')) As Retail,
                                            0 AS SpecificModel
                                        FROM PartsCustom
                                        WHERE parts_diagram=? AND parts_vid=0
                                    UNION
                                    SELECT  parts_pos, parts_brand, parts_diagram_pos, parts_partnumber, parts_partnumber_new, parts_description, parts_from, parts_to, parts_qty, parts_nla, parts_nla_date, parts_prohibited, parts_weight_lb,
                                            (SELECT TOP 1 Retail FROM PriceList WHERE PartNumber=parts_partnumber) As Retail,
                                            1 AS SpecificModel                                        
                                        FROM PartsFiche
                                        WHERE parts_diagram=? AND parts_vid=?
                                    UNION
                                    SELECT  parts_pos, parts_brand, parts_diagram_pos, parts_partnumber, parts_partnumber_new, parts_description, parts_from, parts_to, parts_qty, parts_nla, parts_nla_date, parts_prohibited, parts_weight_lb,
                                            (SELECT TOP 1 Retail FROM PriceList WHERE PartNumber=parts_partnumber OR PartNumber=REPLACE(parts_partnumber,'-','')) As Retail,
                                            1 AS SpecificModel
                                        FROM PartsCustom
                                        WHERE parts_diagram=? AND parts_vid=?
                                    ORDER BY SpecificModel, parts_pos";

                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@parts_diagram", m_Diagram);
                cmd.Parameters.AddWithValue("@parts_diagram", m_Diagram);
                cmd.Parameters.AddWithValue("@parts_diagram", m_Diagram);
                cmd.Parameters.AddWithValue("@parts_vid", m_vid);
                cmd.Parameters.AddWithValue("@parts_diagram", m_Diagram);
                cmd.Parameters.AddWithValue("@parts_vid", m_vid);
            }
            else
            {
                // no specific vid was used, so:
                // first for ALL models (vid=0)
                // then a for vids != 0 (that need a specific vid)
                cmd.CommandText = @"SELECT  parts_pos, parts_brand, parts_diagram_pos, parts_partnumber, parts_partnumber_new, parts_description, parts_from, parts_to, parts_qty, parts_nla, parts_nla_date, parts_prohibited, parts_weight_lb,
                                            (SELECT TOP 1 Retail FROM PriceList WHERE PartNumber=parts_partnumber) As Retail,
                                            0 AS SpecificModel                                        
                                        FROM PartsFiche
                                        WHERE parts_diagram=? AND parts_vid=0
                                    UNION
                                    SELECT  parts_pos, parts_brand, parts_diagram_pos, parts_partnumber, parts_partnumber_new, parts_description, parts_from, parts_to, parts_qty, parts_nla, parts_nla_date, parts_prohibited, parts_weight_lb,
                                            (SELECT TOP 1 Retail FROM PriceList WHERE PartNumber=parts_partnumber OR PartNumber=REPLACE(parts_partnumber,'-','')) As Retail,
                                            0 AS SpecificModel
                                        FROM PartsCustom
                                        WHERE parts_diagram=? AND parts_vid=0
                                    UNION
                                    SELECT  parts_pos, parts_brand, parts_diagram_pos, parts_partnumber, parts_partnumber_new, parts_description, parts_from, parts_to, parts_qty, parts_nla, parts_nla_date, parts_prohibited, parts_weight_lb,
                                            (SELECT TOP 1 Retail FROM PriceList WHERE PartNumber=parts_partnumber) As Retail,
                                            1 AS SpecificModel                                        
                                        FROM PartsFiche
                                        WHERE parts_diagram=? AND parts_vid<>0
                                    UNION
                                    SELECT  parts_pos, parts_brand, parts_diagram_pos, parts_partnumber, parts_partnumber_new, parts_description, parts_from, parts_to, parts_qty, parts_nla, parts_nla_date, parts_prohibited, parts_weight_lb,
                                            (SELECT TOP 1 Retail FROM PriceList WHERE PartNumber=parts_partnumber OR PartNumber=REPLACE(parts_partnumber,'-','')) As Retail,
                                            1 AS SpecificModel
                                        FROM PartsCustom
                                        WHERE parts_diagram=? AND parts_vid<>0
                                    ORDER BY SpecificModel, parts_pos";

                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@parts_diagram", m_Diagram);
                cmd.Parameters.AddWithValue("@parts_diagram", m_Diagram);
                cmd.Parameters.AddWithValue("@parts_diagram", m_Diagram);
                cmd.Parameters.AddWithValue("@parts_diagram", m_Diagram);
            }
        }

        if (cmd.CommandText != "")
        {
            OleDbDataReader dr = cmd.ExecuteReader();

            int RowID = 0;
            string bg_color;
            string border_bottom;
            bool SpecificModel = false;

            while (dr.Read())
            {
                bg_color = ((RowID % 2) == 0 ? "#fff" : "#ddd");

                dpos = dr["parts_diagram_pos"].ToString();
                PartNumber = dr["parts_partnumber"].ToString();

                Description = dr["parts_description"].ToString();
                parts_from = dr["parts_from"].ToString();
                parts_to = dr["parts_to"].ToString();

                SpecificModel = Convert.ToBoolean(dr["SpecificModel"]);

                if (parts_from.Length == 6 && parts_to.Length == 6)
                    Description += "&nbsp;(" + parts_from.Substring(4, 2) + "/" + parts_from.Substring(2, 2) + " to " + parts_to.Substring(4, 2) + "/" + parts_to.Substring(2, 2) + ")";
                else if (parts_from.Length == 6)
                    Description += "&nbsp;(from " + parts_from.Substring(4, 2) + "/" + parts_from.Substring(2, 2) + ")";
                else if (parts_to.Length == 6)
                    Description += "&nbsp;(to " + parts_to.Substring(4, 2) + "/" + parts_to.Substring(2, 2) + ")";

                Qty = dr["parts_qty"].ToString();
                NLA = Convert.ToBoolean(dr["parts_nla"]);
                NLA_date = dr["parts_nla_date"].ToString();
                newPartNumber = (NLA_date != "" ? dr["parts_partnumber_new"].ToString() : "");
                Prohibited = dr["parts_prohibited"].ToString().Trim();
                Weight = (dr["parts_weight_lb"] == DBNull.Value ? "0.00" : string.Format("{0:0.00}", (double)dr["parts_weight_lb"]));
                Weight = (Weight == "0.00" ? "&nbsp;" : Weight);

                Price = (dr["Retail"] == DBNull.Value ? "" : string.Format("{0:0.00}", (double)dr["Retail"]));

                IsSpecial = Utilities.GetSpecials(PartNumber, ref Price, out SpecialsText, out PriceListDescription, ref conn);

                PartDetails = GetPartDetails(PartNumber, RowID, newPartNumber, Prohibited, ref Weight);
                PartModelsFits = ((m_vid == "0" || m_vid == "") && SpecificModel ? GetModelsFits(PartNumber, m_Diagram) : "");
                border_bottom = " border-bottom: solid 1px " + (PartDetails == "" && PartModelsFits == "" ? "#666;" : "#ccc;");
                     
                // parts row
                trs.Append("<tr style=\"vertical-align: middle; font-family: Arial; font-size: 11px; background-color: " + bg_color + ";\">" +
                            "<td align=\"center\" style=\"width: 17px;" + border_bottom + "\">" + dpos + "</td>" +
                            "<td align=\"left\" style=\"width: 75px; border-left: solid 1px #ccc;" + border_bottom + " font-weight: bold;\" class=\"PartNumber\">&nbsp;" + PartNumber + "</td>" +
                            "<td align=\"left\" style=\"width: 285px; border-left: solid 1px #ccc;" + border_bottom + " font-family: Arial Narrow;\">&nbsp;" + Description + "</td>" +
                            "<td align=\"right\" style=\"width: 30px; border-left: solid 1px #ccc;" + border_bottom + "\">" + Weight + "&nbsp;</td>" +
                            "<td align=\"right\" style=\"width: 35px; border-left: solid 1px #ccc;" + border_bottom + "\">" + Qty + "&nbsp;</td>");

                if (!NLA && Prohibited == "" && Price.Length > 0 && Convert.ToDouble(Price) > 0.0)
                {
                    trs.Append("<td align=\"right\" style=\"border-left: solid 1px #ccc;" + border_bottom + " width: 70px;" + (IsSpecial ? "font-weight: bold; color: #e60;" : "") + "\"" +
                               (IsSpecial ? " title=\"" + SpecialsText + "\"" : "") + ">$" + Price +
                               "<img style=\"cursor: pointer;\" src=\"images/addtocart.gif\" alt=\"ADD TO CART\"" +
                               " onclick=\"AddToCart('" + PartNumber + "','" + Qty + "','" + HttpContext.Current.Server.UrlEncode(Description) + "','" + Weight + "');\"" +
                               "/></td>");
                }
                else
                {
                    if (Prohibited == "A") // temporary solution for apparel
                        trs.Append("<td align=\"center\" style=\"border-left: solid 1px #ddd;" + border_bottom + " width: 70px; color: #f00; font-weight: bold; cursor: help;\" title=\"Sorry we are currently updating prices for this item.\nPlease check back on-line in a few days.\n\nFor ordering as an alternative you may call us at the 1-603-964-2877 and press the #2, during our business hours:\nTue-Fri 9am to 6pm\nSat 9am to 4pm\">CALL<img alt=\"CALL\" src=\"images/Help.png\" /></td>");
                    else if (NLA_date.Length == 6)
                        trs.Append("<td align=\"center\" style=\"border-left: solid 1px #ccc;" + border_bottom + " width: 70px; color: #f00; font-weight: bold; cursor: help;\" title=\"No Longer Available (NLA) since " + NLA_date + "\">NA<img alt=\"No Longer Available (NLA) since " + NLA_date.Substring(4, 2) + "/" + NLA_date.Substring(0, 4) + "\" src=\"images/Help.png\" /></td>");
                    else
                        trs.Append("<td align=\"center\" style=\"border-left: solid 1px #ccc;" + border_bottom + " width: 70px; color: #f00; font-weight: bold;\">NA</td>");
                }
                trs.Append("</tr>");
                                                
                if (PartDetails != "" || PartModelsFits != "")
                {
                    trs.Append("<tr style=\"background-color: " + bg_color + ";\"><td colspan=\"6\" style=\"border-bottom: solid 1px #666;\">");
                    trs.Append(PartDetails);
                    trs.Append(PartModelsFits);
                    trs.Append("</td></tr>");
                }

                RowID++;
            }
        }

        cmd.Dispose();

        if (trs.Length > 0)
        {            
            if (m_vid != "0" && m_vid != "" && m_vid != "99999")
            {
                // if a specific model was chosen, then show it here
                txt.Append("<br /><label style=\"font-size: 13px; font-weight: bold;\">These parts fit the <label class=\"ModelsFits\">&nbsp;" + GetFullModelFromVID(m_vid) + "<label class=\"ModelX\" onclick=\"SetThisPageVIDFilter('');\" title=\"Remove this filter and show parts for all models\">x</label></label>:</label><br /><br />");
            }

            // table and header
            txt.Append("<table cellpadding=\"0\" cellspacing=\"0\" style=\"width: 512px; border: solid 1px #666;\">" +
                    "<tr style=\"font-family: Arial; font-size: 11px; font-weight: bold; background-color: #666; color: #fff;\">" +
                        "<td align=\"center\" style=\"width: 17px; border-top: solid 1px #666; border-bottom: solid 1px #666; border-right: solid 1px #666;\">#</td>" +
                        "<td align=\"center\" style=\"width: 75px; border: solid 1px #666;\">Part Number</td>" +
                        "<td align=\"left\"   style=\"width: 285px; border: solid 1px #666;\">&nbsp;&nbsp;Description</td>" +
                        "<td align=\"center\" style=\"width: 30px; border: solid 1px #666; cursor: help;\" title=\"Weight in Pounds\">lb</td>" +
                        "<td align=\"center\" style=\"width: 35px; border: solid 1px #666; cursor: help;\" title=\"This is the recomended quantity.\nPrice is for each one.\nOnce you click on BUY, you will be able to specify how many items you want to add to your cart.\n\n'X' means as necessary (could also be length in meters).\">Qty<img alt=\"\" src=\"images/Help.png\" /></td>" +
                        "<td align=\"center\" style=\"border: solid 1px #666;\">Each</td>" +
                    "</tr>" + trs + "</table>");
        }
        return txt.ToString();
    }

    private string GetModelsFits(string PartNumber, string Diagram)
    {
        string ModelsFiting = "";

        OleDbCommand cmd = new OleDbCommand();
        cmd.Connection = conn;
        cmd.CommandText = @"SELECT DISTINCT parts_vid FROM (    
                                SELECT parts_vid FROM PartsFiche
                                    WHERE parts_diagram=? AND parts_partnumber=?
                                UNION
                                SELECT parts_vid FROM PartsCustom
                                    WHERE parts_diagram=? AND parts_partnumber=?
                            ) AS TempTable ORDER BY parts_vid";

        cmd.Parameters.Clear();
        cmd.Parameters.AddWithValue("@parts_diagram", Diagram);
        cmd.Parameters.AddWithValue("@parts_partnumber", PartNumber);
        cmd.Parameters.AddWithValue("@parts_diagram", Diagram);
        cmd.Parameters.AddWithValue("@parts_partnumber", PartNumber);


        //          maxcols=4
        // 1  2  3  4  
        // 5  6  7  8
        // 9  10 11 12  
        // .....
        const int maxcols = 4;
       
        string vid;
        string model;
        OleDbDataReader dr = cmd.ExecuteReader();

        int count = 0;
        int col = 1;
        string t = "";
        string r = "";
        while (dr.Read())
        {
            vid = dr["parts_vid"].ToString();
            model = GetFullModelFromVID(vid);

            r += "<td onclick=\"SetThisPageVIDFilter('" + vid + "');\" title=\"Click to narrow down by the " + model + "\" style=\"cursor: pointer;\">" +
                 "<img src=\"images/bulletModels.png\" />&nbsp;" + model + "</td>";
            col++;
            count++;

            if (col > maxcols)
            {
                t += "<tr>" + r + "</tr>";
                r = "";
                col = 1;
            }
        }

        // finish table if any cols are left over
        while (col!=1 && col <= maxcols)
        {
            r += "<td>&nbsp;</td>";
            col++;

            if (col > maxcols)
            {
                t += "<tr>" + r + "</tr>";               
            }
        }

        if (count>0)
        {
            t = "<label style=\"font-weight: bold;\">&nbsp;Part '" + PartNumber + "' fits " + (count==1 ? " this bike" : " these bikes")+ " only:</label><br />" +
                "<table style=\"font-size: 11px; font-weight: bold;\" cellpadding=\"0\" cellspacing=\"2\" border=\"0\" width=\"510\">" + t + "</table>";
        }

        dr.Close();
        cmd.Dispose();

        if (count>0)
            ModelsFiting = t;

        return ModelsFiting;
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
            TR_HTML = "<table cellpadding=\"0\" cellspacing=\"3\" style=\"width: 510px; font-size: 11px;\">" +
                        "<tr>" +
                            "<td>";
            if (NoteID.Length > 0)
            {
                // could have images, check it
                TR_HTML += ShowPartNoteImage(PartNumber);
                TR_HTML += ShowPartNoteYouTube(YouTubeLinks);
                TR_HTML += ShowPartNotePDF(PartNumber);
            }
            else
            {
                TR_HTML += "&nbsp;";
            }

            TR_HTML += "</td>" +
                            "<td valign=\"top\" align=\"left\">" +
                                Comments +
                            "</td>" +
                        "</tr>" +
                    "</table>";
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
                txt += "<img height=\"65\" class=\"PartThumbnailImg\" alt=\"Part Thumbnail\" title=\"Click to view the large image\" src=\"NotesParts/" + PartNumber + "_" + i.ToString() + "_N.jpg?rnd=" + m_rnd + "\" />&nbsp;";
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

    protected string GetFullModelFromVID(string VID)
    {
        string FullModel = "";

        if (VID != "0" && VID != "" && VID !="99999")
        {
            OleDbCommand cmd = new OleDbCommand();
            cmd.Connection = conn;
            cmd.CommandTimeout = 120;

            cmd.CommandText = @"SELECT DISTINCT fztyp_baureihe,                                            
                                            fztyp_model,
                                            fztyp_useprod,
                                            fztyp_ktlgausf,
                                            (SELECT MIN(fztyp_prodmin) FROM etk_fztyp WHERE fztyp_mospid=x.fztyp_mospid) AS prodmin,
                                            (SELECT MAX(fztyp_prodmax) FROM etk_fztyp WHERE fztyp_mospid=x.fztyp_mospid) AS prodmax
                                FROM etk_fztyp AS x
                                WHERE fztyp_mospid=?";
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@fztyp_mospid", VID);

            OleDbDataReader dr = cmd.ExecuteReader();

            if (dr.Read())
            {
                bool UseProd = (DBNull.Value == dr["fztyp_useprod"] ? false : Convert.ToBoolean(dr["fztyp_useprod"]));

                FullModel = dr["fztyp_model"].ToString();
                FullModel += (UseProd ? "&nbsp;&nbsp;" + dr["prodmin"].ToString().Substring(4, 2) + "/" + dr["prodmin"].ToString().Substring(2, 2) + "-" + dr["prodmax"].ToString().Substring(4, 2) + "/" + dr["prodmax"].ToString().Substring(2, 2) : "");
                FullModel += (dr["fztyp_ktlgausf"].ToString().ToUpper()=="USA" ? "" : "&nbsp;&nbsp;" + dr["fztyp_ktlgausf"].ToString());
            }

            dr.Close();
            cmd.Dispose();
        }
        return FullModel;
    }    
}
