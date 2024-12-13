using System;
using System.Configuration;
using System.Web;
using System.Data.OleDb;
using System.IO;
using System.Text;
using System.Collections.Generic;

public partial class DiagramsRight : System.Web.UI.Page
{
    private OleDbConnection conn;
    
    protected string m_vid;
    protected string m_Diagram;
    protected string m_image;
    protected string m_imageBig;
    protected string m_imagetext;

    protected List<string> m_ProductImages;
    protected List<string> m_ProductVideos;
    protected List<string> m_ProductPDFs;

    protected string m_DiagramNoteComments;

    protected void Page_Load(object sender, EventArgs e)
    {
        m_ProductImages = new List<string>();
        m_ProductVideos = new List<string>();
        m_ProductPDFs = new List<string>();

        m_imagetext = "";
        m_image = "";
        m_imageBig = "";
        m_DiagramNoteComments = "";

        m_vid = Utilities.QueryString("vid");
        if (m_vid.Length > 5)
        {
            m_vid = m_vid.Substring(0, 5);
        } 
        m_Diagram = Utilities.QueryString("diagram");

        if (Utilities.GetUrlReferrer().IndexOf("DiagramsMain.aspx") < 0)
        {
            // not coming from fiche, so redirect
            string redirected = "fiche.aspx";
            string menu = Utilities.QueryString("menu").ToLower();
            
            if (menu == "apparel")
            {
                redirected = "PartsApparel.aspx";
            }
            else if (menu == "aftermarket")
            {
                redirected = "PartsCatalog.aspx?vid=" + m_vid + (m_Diagram.Length == 7 ? "&diagram=" + m_Diagram : "");
            }
            else
            {
                if (m_vid.Length == 5)
                {
                    if (Utilities.GetUrlReferrer() == "")
                        redirected = ""; // no redirect, a VIN is there, but GetUrlReferrer() is blank
                    else
                        redirected = "DiagramsMain.aspx?vid=" + m_vid + (m_Diagram.Length == 7 ? "&diagram=" + m_Diagram : "");
                }
            }
                        
            if (redirected != "")
            {
                //string emailBody = "Utilities.GetUrlReferrer()=[" + Utilities.GetUrlReferrer() + "] sent to [" + redirected + "]" + Environment.NewLine + "IP=" + HttpContext.Current.Request.UserHostAddress;
                //EmailSender.SendEmail("berczely@hotmail.com", "error@maxbmw.com", "Site Errors", "", "", "Redirected from DiagramsRight", emailBody);

                Response.Redirect(redirected);
                return;
            }
        }
              
    
        if (m_vid!="" || m_Diagram!="")
        {
            conn = new OleDbConnection(ConfigurationManager.AppSettings["connString"]);
            conn.Open();

            string qry;
            OleDbCommand cmd = new OleDbCommand();
            cmd.Connection = conn;

            //**************************************
            // Retrieve the Diagram
            //**************************************
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
            cmd.Parameters.AddWithValue("@parts_diagram", m_Diagram); // added 1/24/2011
            
            OleDbDataReader dr = cmd.ExecuteReader();

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
            }


            dr.Close();
            

            //**************************************
            // Retrieve the DiagramNotes
            //**************************************                        
            qry = @"SELECT TOP 1 Comments, YouTubeLinks FROM NotesDiagrams WHERE Diagram=?";
            cmd.CommandText = qry;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@Diagram", m_Diagram);
            dr = cmd.ExecuteReader();

            if (dr.Read())
            {
                m_DiagramNoteComments = dr["Comments"].ToString();

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
        else if (part.Length > 11)
        {

            double result;
            string partLeft = part.Substring(0, 11);
            if (Double.TryParse(partLeft, System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.CurrentCulture, out result))
            {
                // is a BMW part number
                p = p[0].ToString() + p[1].ToString() + " " + p[2] + p[3] + " " + p[4] + " " + p[5] + p[6] + p[7] + " " + p[8] + p[9] + p[10] + part.Substring(11, part.Length-11);
            }
        }
        return p;
    }

    private string GetKommText(int kommid, bool IsTopNote)
    {
        string kommText = "";

        if (kommid > 0)
        {
            OleDbCommand cmdKomm = new OleDbCommand();
            cmdKomm.Connection = conn;
            cmdKomm.CommandText = @"SELECT komm_text, komm_code, komm_vz FROM etk_komm WHERE komm_id=? ORDER BY komm_pos";
            cmdKomm.Parameters.Clear();
            cmdKomm.Parameters.AddWithValue("@komm_id", kommid);
            OleDbDataReader drKomm = cmdKomm.ExecuteReader();

            bool NOT = false;
            string komm_text;

            while (drKomm.Read())
            {
                if (drKomm["komm_vz"].ToString() == "-") 
                    NOT = true;

                komm_text = drKomm["komm_text"].ToString();
                komm_text = komm_text.Replace(", ZIERL --/--", "");
                komm_text = komm_text.Replace("*****", "");
    
                if (komm_text!="")
                    kommText += komm_text + (drKomm["komm_code"].ToString() != "" ? " (CODE: " + drKomm["komm_code"].ToString() + ")" : "") + (IsTopNote ? "<br />" : " ");
            }

            if (kommText != "")
            {
                
                // remove the last <br /> or space
                if (kommText.LastIndexOf(" ") > 0 && (kommText.LastIndexOf("<br />") == kommText.Length - 1))
                    kommText = kommText.Substring(0, kommText.Length - 1);

                if (kommText.LastIndexOf("<br />") > 0 && (kommText.LastIndexOf("<br />") == kommText.Length - 6))
                    kommText = kommText.Substring(0, kommText.Length - 6);

                // add the ending ":"
                if (!IsTopNote)
                    kommText += ":";

                if (NOT)
                    kommText = "NOT " + kommText;
            }
        }

        return kommText;
    }

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
            if (Retail>0.0)
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
        StringBuilder trs = new StringBuilder("");

        if (m_Diagram.Length>3)
        {
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
            string PriceListDescription = null;

            string sKommbt = "";
            string sKommnach = "";
            string sKommvor = "";

            OleDbCommand cmd = new OleDbCommand();
            cmd.Connection = conn;
        
            cmd.CommandText = @"SELECT  parts_pos, parts_brand, parts_diagram_pos, parts_partnumber, parts_partnumber_new, parts_description, parts_from, parts_to, parts_qty, parts_nla, parts_nla_date, parts_prohibited, parts_weight_lb,
                                        (SELECT TOP 1 Retail FROM PriceList WHERE PartNumber=parts_partnumber) As Retail,
                                        parts_kommbt, parts_kommvor, parts_kommnach                                        
                                    FROM PartsFiche
                                    WHERE parts_diagram=? AND parts_vid=?
                                UNION
                                SELECT  parts_pos, parts_brand, parts_diagram_pos, parts_partnumber, parts_partnumber_new, parts_description, parts_from, parts_to, parts_qty, parts_nla, parts_nla_date, parts_prohibited, parts_weight_lb,
                                        (SELECT TOP 1 Retail FROM PriceList WHERE PartNumber=parts_partnumber OR PartNumber=REPLACE(parts_partnumber,'-','')) As Retail,
                                        0 AS parts_kommbt, 0 AS parts_kommvor, 0 AS parts_kommnach
                                    FROM PartsCustom
                                    WHERE parts_diagram=? AND parts_vid=?
                                UNION
                                SELECT  parts_pos, parts_brand, parts_diagram_pos, parts_partnumber, parts_partnumber_new, parts_description, parts_from, parts_to, parts_qty, parts_nla, parts_nla_date, parts_prohibited, parts_weight_lb,
                                        (SELECT TOP 1 Retail FROM PriceList WHERE PartNumber=parts_partnumber OR PartNumber=REPLACE(parts_partnumber,'-','')) As Retail,
                                        0 AS parts_kommbt, 0 AS parts_kommvor, 0 AS parts_kommnach
                                    FROM PartsCustom
                                    WHERE parts_diagram=? AND parts_vid=0
                                ORDER BY parts_pos, parts_brand";               

            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@parts_diagram", m_Diagram);
            cmd.Parameters.AddWithValue("@parts_vid", m_vid);
            cmd.Parameters.AddWithValue("@parts_diagram", m_Diagram);
            cmd.Parameters.AddWithValue("@parts_vid", m_vid);
            cmd.Parameters.AddWithValue("@parts_diagram", m_Diagram);  
            OleDbDataReader dr = cmd.ExecuteReader();

            int RowID = 0;
            string bg_color;
            string border_bottom;

            string previous_sKommvor = "";
            string previous_sKommnach = "";

            while (dr.Read())
            {
                bg_color = ((RowID % 2) == 0 ? "#fff" : "#ddd");

                dpos = dr["parts_diagram_pos"].ToString();
                PartNumber = dr["parts_partnumber"].ToString();
                Description = dr["parts_description"].ToString();
                parts_from = dr["parts_from"].ToString();
                parts_to = dr["parts_to"].ToString();
         
                sKommbt = GetKommText(Convert.ToInt32(dr["parts_kommbt"]), true);
                sKommnach = GetKommText(Convert.ToInt32(dr["parts_kommnach"]), false);
                sKommvor = GetKommText(Convert.ToInt32(dr["parts_kommvor"]), false);

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
                Prohibited = (dr["parts_prohibited"] == DBNull.Value ? "" : dr["parts_prohibited"].ToString().Trim());
                Weight = (dr["parts_weight_lb"] == DBNull.Value ? "0.00" : string.Format("{0:0.00}", (double)dr["parts_weight_lb"]));
                Weight = (Weight == "0.00" ? "&nbsp;" : Weight);

                Price = (dr["Retail"] == DBNull.Value ? "" : string.Format("{0:0.00}", (double)dr["Retail"]));

                IsSpecial = Utilities.GetSpecials(PartNumber, ref Price, out SpecialsText, out PriceListDescription, ref conn);

                PartDetails = GetPartDetails(PartNumber, RowID, newPartNumber, Prohibited, ref Weight);
            
                border_bottom = " border-bottom: solid 1px " + (PartDetails == "" ? "#666;" : "#ccc;");

                if (sKommvor != "" && previous_sKommvor != sKommvor)
                {
                    trs.Append("<tr style=\"background-color: " + bg_color + ";\"><td colspan=\"6\" style=\"border-bottom: solid 1px #ddd; padding: 3px;\">");
                    trs.Append("<span style=\"font-size: 11px; color: #830;\">" + sKommvor + "</span>");
                    trs.Append("</td></tr>");
                }

                // parts row
                trs.Append("<tr style=\"vertical-align: middle; font-family: Arial; font-size: 11px; background-color: " + bg_color + ";\">" +
                            "<td align=\"center\" style=\"width: 17px;" + border_bottom + "\">" + dpos + "</td>" +
                            "<td align=\"left\" style=\"width: 95px; border-left: solid 1px #ddd;" + border_bottom + " font-weight: bold;\">&nbsp;" +
                                "<span title=\"Double Click on the part number to select it.\nThen press CTRL+C to copy it and CTRL+V to paste it.\" ondblclick=\"SelectText(this);\">" + FormatPartNumber(PartNumber) + "</span></td>" +
                            "<td align=\"left\" style=\"width: 225px; border-left: solid 1px #ddd;" + border_bottom + " font-family: Arial Narrow;\">&nbsp;" + Description + "</td>" +
                            "<td align=\"right\" style=\"width: 30px; border-left: solid 1px #ddd;" + border_bottom + "\">" + Weight + "&nbsp;</td>" +
                            "<td align=\"right\" style=\"width: 35px; border-left: solid 1px #ddd;" + border_bottom + "\">" + Qty + "&nbsp;</td>");

                if (!NLA && Prohibited == "" && Price.Length > 0 && Convert.ToDouble(Price) > 0.0)
                {
                    trs.Append("<td align=\"right\" style=\"border-left: solid 1px #ddd;" + border_bottom + " width: 100px;" + (IsSpecial ? "font-weight: bold; color: #d00;" : "") + "\"" +
                        (IsSpecial ? " title=\"" + SpecialsText + "\"" : "") + ">" + (IsSpecial ? "Sale " : "") + "$" + Price +
                               "<img style=\"cursor: pointer;\" src=\"images/addtocart.gif\" alt=\"ADD TO CART\"" +
                               " onclick=\"AddToCart('" + PartNumber + "','" + Qty + "','" + HttpContext.Current.Server.UrlEncode(Description) + "','" + Weight + "');\"" +
                               "/></td>");
                }
                else
                {
                    // Check Mobile Tradition First - new change on 11/21/2024
                    if (MTAvailable(PartNumber))
                    {
                        trs.Append("<td align=\"right\" style=\"border-left: solid 1px #ddd;" + border_bottom + " width: 100px;" + (IsSpecial ? "font-weight: bold; color: #d00;" : "") + "\"" +
                        (IsSpecial ? " title=\"" + SpecialsText + "\"" : "") + ">" + (IsSpecial ? "Sale " : "") + "$" + Price +
                               "<img style=\"cursor: pointer;\" src=\"images/addtocart.gif\" alt=\"ADD TO CART\"" +
                               " onclick=\"AddToCart('" + PartNumber + "','" + Qty + "','" + HttpContext.Current.Server.UrlEncode(Description) + "','" + Weight + "');\"" +
                               "/></td>");
                    }
                    else
                    {
                        if (NLA_date.Length == 6)
                            trs.Append("<td align=\"center\" style=\"border-left: solid 1px #ddd;" + border_bottom + " width: 70px; color: #555; font-weight: bold; cursor: help;\" title=\"No Longer Available (NLA) since " + NLA_date.Substring(4, 2) + "/" + NLA_date.Substring(0, 4) + "\">NA<img alt=\"NLA\" src=\"images/Help.png\" /></td>");
                        else
                            trs.Append("<td align=\"center\" style=\"border-left: solid 1px #ddd;" + border_bottom + " width: 70px; color: #555; font-weight: bold;\">NA</td>");

                    }

                    
                }
                trs.Append("</tr>");
            
                if (PartDetails != "")
                {
                    trs.Append("<tr style=\"background-color: " + bg_color + ";\"><td colspan=\"6\" style=\"border-bottom: solid 1px " + (sKommnach != "" ? "#ddd" : "#666") + ";\">");
                    trs.Append(PartDetails);
                    trs.Append("</td></tr>");
                }

                if (sKommnach != "" && previous_sKommnach != sKommnach)
                {
                    trs.Append("<tr style=\"background-color: " + bg_color + ";\"><td colspan=\"6\" style=\"border-bottom: solid 1px #666; padding: 3px; \">");
                    trs.Append("<span style=\"font-size: 11px; color: #830;\">" + sKommnach + "</span>");
                    trs.Append("</td></tr>");
                }

                previous_sKommnach = sKommnach;
                previous_sKommvor = sKommvor;

                RowID++;
            }
            dr.Close();
            cmd.Dispose();

            string btNote = "";
            if (sKommbt != "")
            {
                btNote = "<tr style=\"background-color: #666;\"><td colspan=\"6\" style=\"border-bottom: solid 1px #666; padding: 3px; font-size: 12px; color: #fc5;\">" +
                            "<b>Notes:</b><br />" + sKommbt + 
                         "</td></tr>";
            }

            if (trs.Length > 0)
            {
                // table and header      
                txt.Append("<table cellpadding=\"0\" cellspacing=\"0\" style=\"width: 512px; border: solid 1px #666;\">" +
                        (btNote != "" ? btNote : "") +
                        "<tr style=\"font-family: Arial; font-size: 11px; font-weight: bold; background-color: #666; color: #fff;\">" +
                            "<td align=\"center\" style=\"width: 17px; border-top: solid 1px #666; border-bottom: solid 1px #666; border-right: solid 1px #666;\">#</td>" +
                            "<td align=\"center\" style=\"width: 95px; border: solid 1px #666;\">Part Number</td>" +
                            "<td align=\"left\"   style=\"width: 225px; border: solid 1px #666;\">&nbsp;&nbsp;Description</td>" +
                            "<td align=\"center\" style=\"width: 30px; border: solid 1px #666; cursor: help;\" title=\"Weight in Pounds\">lb</td>" +
                            "<td align=\"center\" style=\"width: 35px; border: solid 1px #666; cursor: help;\" title=\"This is the recomended quantity.\nPrice is for each one.\nOnce you click on BUY, you will be able to specify how many items you want to add to your cart.\n\n'X' means as necessary (could also be length in meters).\">Qty<img alt=\"This is the recomended quantity.\nPrice is for each one.\nOnce you click on BUY, you will be able to specify how many items you want to add to your cart.\n\n'X' means as necessary (could also be length in meters).\" src=\"images/Help.png\" /></td>" +
                            "<td align=\"center\" style=\"border: solid 1px #666;\">Each</td>" +
                            "</tr>" + trs + "</table>");
            }
        }
        return txt.ToString();
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
                txt += "<img class=\"PartThumbnailImg\" alt=\"Part Thumbnail\" title=\"Click to view the large image\" src=\"NotesParts/" + PartNumber + "_" + i.ToString() + "_N.jpg\" />&nbsp;";
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
}