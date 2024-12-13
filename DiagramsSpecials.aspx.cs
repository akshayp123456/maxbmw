using System;
using System.Configuration;
using System.Data.OleDb;
using System.IO;
using System.Text;

public partial class DiagramsSpecials : System.Web.UI.Page
{
    private OleDbConnection conn;
    protected string m_rnd;
    public string m_vid;
    public string m_menu;
        
    protected void Page_Load(object sender, EventArgs e)
    {
        m_rnd = DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString() + DateTime.Now.Millisecond.ToString();

        m_vid = Utilities.QueryString("vid");
        
        if (m_vid != "")
        {
            if (m_vid.Length > 5)
            {
                m_vid = m_vid.Substring(0, 5);
            }
            conn = new OleDbConnection(ConfigurationManager.AppSettings["connString"]);
            conn.Open();               
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

    public string ShowSpecials()
    {
        //string txt = "";
        StringBuilder txt = new StringBuilder("");

        if (m_vid != "" && m_vid != "99999" && m_vid != "0")
        {            
            OleDbCommand cmd = new OleDbCommand();
            cmd.Connection = conn;
            cmd.CommandText = @"SELECT DISTINCT parts_brand,
                                                parts_partnumber,
                                                parts_description, 
                                                parts_diagram,
                                                parts_diagram_mg,
                                                parts_imagetext,
                                                parts_image,
                                                SpecialsDiscount,
                                                SpecialsPrice,
                                                Retail,
                                                0 AS IsCustom           
                                FROM PartsFiche, NotesParts, PriceList
                                WHERE parts_vid=? AND SpecialsShowInRightPanel=1 AND parts_partnumber=NotesParts.PartNumber AND PriceList.PartNumber=NotesParts.PartNumber
                                UNION
                                SELECT DISTINCT parts_brand,
                                                parts_partnumber, 
                                                parts_description,
                                                parts_diagram,
                                                parts_diagram_mg,
                                                parts_imagetext,
                                                parts_image,
                                                SpecialsDiscount,
                                                SpecialsPrice,
                                                Retail,
                                                1 AS IsCustom
                                FROM PartsCustom, NotesParts, PriceList
                                WHERE parts_vid=? AND SpecialsShowInRightPanel=1 AND parts_partnumber=NotesParts.PartNumber AND PriceList.PartNumber=NotesParts.PartNumber
                                ORDER BY IsCustom";
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@parts_vid", m_vid);
            cmd.Parameters.AddWithValue("@parts_vid", m_vid);

            OleDbDataReader dr = cmd.ExecuteReader();

            
            if (dr.HasRows)
            {
                string ImageText = "";
                string Diagram = "";
                string DiagramMG = "";
                string PartNumber = "";
                string Description = "";
                string PartsImage = "";                
                bool IsCustom = false;                
                string PriceListDescription = "";
                string Comments = "";
                string Price = "";
                string SpecialsText = "";

                bool IsSpecial = false;

                double dRetail = 0.0;
                double dSpecialsPrice = 0.0;
                double dSpecialsDiscount = 0.0;


                txt.Append("<label style=\"font-weight: bold; font-size: 14px; color: #333;\">&nbsp;Featured Items:</label><br />");
                txt.Append("<table cellspacing=\"5\" cellpadding=\"4\" border=\"0\">");

                OleDbCommand cmdNotes = new OleDbCommand();
                cmdNotes.Connection = conn;
                 
                int Column = 0;
                while (dr.Read())
                {
                    Column++;

                    PartNumber = dr["parts_partnumber"].ToString();
                    Description = dr["parts_description"].ToString();
                    Description = (Description.Length > 15 ? Description.Substring(0, 15) + "..." : Description);
                    Diagram = dr["parts_diagram"].ToString();
                    DiagramMG = dr["parts_diagram_mg"].ToString();
                    ImageText = dr["parts_imagetext"].ToString();
                    PartsImage = dr["parts_image"].ToString();
                    IsCustom = Convert.ToBoolean(dr["IsCustom"]);

                    dRetail = Convert.ToDouble(dr["Retail"]);
                    dSpecialsDiscount = (DBNull.Value != dr["SpecialsDiscount"] ? Convert.ToDouble(dr["SpecialsDiscount"]) : 0.0);
                    dSpecialsPrice = (DBNull.Value != dr["SpecialsPrice"] ? Convert.ToDouble(dr["SpecialsPrice"]) : 0.0);
                    if (dSpecialsDiscount != 0.0)
                    {
                        IsSpecial = true;
                        Price = (dRetail - ((dRetail * dSpecialsDiscount) / 100.0)).ToString("N");
                        SpecialsText = dSpecialsDiscount.ToString("N") + "% OFF";
                    }
                    else if (dSpecialsPrice != 0.0)
                    {
                        IsSpecial = true;
                        Price = dSpecialsPrice.ToString("N");
                        SpecialsText = "Was $" + dRetail.ToString("N");
                    }
                    else
                    {
                        IsSpecial = false;
                        Price = dRetail.ToString("N");
                    }

                    string image;
                    if (PartsImage.IndexOf("B000") == 0)
                        image = "DiagramsThumb/" + PartsImage + ".png";
                    else
                        image = "DiagramsCustom/" + PartsImage + "_T.jpg";

                    if (File.Exists(MapPath(image)))
                    {
                        string menu = "Fiche";
                        int imaingroup = 0;
                        try
                        {
                            imaingroup = Convert.ToInt16(DiagramMG);
                        }
                        catch { }

                        if (DiagramMG == "76")
                            menu = "Apparel";
                        if (DiagramMG == "WT")
                            menu = "Tires";
                        else if (imaingroup < 1 || imaingroup > 99)
                            menu = "Catalog";                   
                    
                                                                               
                        cmdNotes.CommandText = @"SELECT Comments, SpecialsComments FROM NotesParts WHERE PartNumber=?";
                        cmdNotes.Parameters.Clear();
                        cmdNotes.Parameters.AddWithValue("@PartNumber", PartNumber);
                        OleDbDataReader drNotes = cmdNotes.ExecuteReader();
                        if (drNotes.Read())
                        {
                            Comments = (drNotes["Comments"].ToString().Length > 0 ? drNotes["Comments"].ToString() : "");
                            SpecialsText += (drNotes["SpecialsComments"].ToString().Length > 0 ? "-" + drNotes["SpecialsComments"].ToString() : "");                        
                        }
                        drNotes.Close();
                        drNotes.Dispose();
                                    

                        if (Column == 1)
                            txt.Append("<tr>");

                        txt.Append("<td align=\"center\" style=\"border: 2px solid #ddd; background-color: #fff; width: 255px; cursor: pointer; font-size: 11px;\" ");
                    
                        if (menu=="Fiche")
                            txt.Append("onclick=\"parent.parent.window.location='DiagramsMain.aspx?vid=" + m_vid + "&diagram=" + Diagram + "';\" onmouseover=\"this.style.borderColor='#cc3333'\" onmouseout=\"this.style.borderColor='#ddd'\">");
                        else
                            txt.Append("onclick=\"parent.parent.window.location='PartsDetails.aspx?vid=" + m_vid + "&diagram=" + Diagram + "';\" onmouseover=\"this.style.borderColor='#cc3333'\" onmouseout=\"this.style.borderColor='#ddd'\">");
                    

                        txt.Append("<label style=\"color: #333; font-size: 9px; font-weight: bold;\">" + ImageText + "</label><br />");
                    
                        if (PriceListDescription!=ImageText)
                            txt.Append("<label style=\"color: #333; font-size: 9px; font-weight: bold;\">" + PriceListDescription + "</label><br />");
                                      
                        txt.Append("<img alt=\"\" src=\"" + image + "\" /><br />");    
                        txt.Append("<label style=\"color: #333; font-size: 10px; font-weight: bold;\">" + PartNumber + " - " + Description + " - </label>");
                        txt.Append("<label style=\"color: " + (IsSpecial ? "#d00;" : "#333;") + " font-size: 10px; font-weight: bold;\">$" + Price + "</label><br />");                    
                        txt.Append((SpecialsText != "" ? "<label style=\"color: #d00; font-size: 10px; font-weight: bold;\">" + SpecialsText + "</label>" : ""));
                        txt.Append("</td>");
                    }

                    if (Column == 2)
                    {
                        Column = 0;
                        txt.Append("</tr>");
                    }
                }
                cmdNotes.Dispose();

                for (int i = Column - 1; i > 0; i--)
                {
                    txt.Append("<td align=\"center\" style=\"width: 255px; max-width: 255px;\">&nbsp;</td>");
                }

                if (txt.ToString().Substring(txt.Length - 5, 5) != "</tr>")
                    txt.Append("</tr>");

                txt.Append("</table>");
            }

            dr.Close();
            dr.Dispose();
            cmd.Dispose();
        }
        return txt.ToString();
    }    
}
