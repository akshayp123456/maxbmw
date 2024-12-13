using System;
using System.Collections.Generic;
using System.Web.Script.Serialization;
using System.Data.OleDb;


public partial class JSONPartDescriptions : System.Web.UI.Page
{
    private class csPartNumber
    {
        public string id; // used to do a search, contains the source in the string
        public string icon; // fiche / aftermarket / tires / apparel
        public string firstline; 
        public string secondline;
        public string token;        
    }

    private string RemoveDangerousChars(string txt)
    {
        string t = txt.Replace("\'", "").Replace("!", "").Replace("\"", "").Replace("%", "").Replace("*", "").Replace("(", "").Replace(")", "").Replace("@", "").Replace(";", "");
        return t;
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        string sJSON = "";

        //if (Request.UrlReferrer.AbsolutePath.ToLower().IndexOf("/partssearch.aspx") < 0)
        //    return;

        string q = Utilities.QueryString("q");
        string brand = Utilities.QueryString("brand");
        string vid = Utilities.QueryString("vid");
        string category = Utilities.QueryString("category").ToLower();

        string where_clause = "";
        if (brand != "")
        {
            where_clause += "AND parts_brand='" + brand + "' ";
        }
        if (vid != "")
        {
            where_clause += "AND parts_vid='" + vid + "' ";
        }
        if (category == "tires")
        {            
            where_clause += "AND parts_diagram_mg='WT' ";
        }
        if (category == "fiche")
        {
            where_clause += "AND (parts_diagram_mg>'00' AND parts_diagram_mg<'99') AND parts_diagram<>'76' ";
        }
        if (category == "catalog")
        {
            where_clause += "AND (parts_diagram_mg<'00' AND parts_diagram_mg>'99') AND parts_diagram<>'WT' ";
        }
        if (category == "apparel")
        {
            where_clause += "AND parts_diagram_mg='76' ";
        }

        int max = 12;

        q = RemoveDangerousChars(q);

        string qnospace = q.Replace(" ", "");
        if (qnospace.Length == 11)
            q = qnospace;

        if (q.Length > 0 && q.Length <= 15)
        {
            List<csPartNumber> pnList = new List<csPartNumber>();
            csPartNumber pn;
            JavaScriptSerializer oSerializer = new JavaScriptSerializer();

            OleDbConnection conn;
            conn = new OleDbConnection(System.Configuration.ConfigurationManager.AppSettings["connString"]);
            conn.Open();

            OleDbCommand cmd = new OleDbCommand();
            cmd.Connection = conn;
            OleDbDataReader dr;


            // First search by part number
            cmd.CommandText = @"SELECT DISTINCT TOP " + max + " parts_diagram_mg, parts_partnumber, parts_description, parts_brand FROM PartsCustom WHERE 1=1 " + where_clause + "AND parts_partnumber = '" + q + "' ORDER BY parts_description;";
            dr = cmd.ExecuteReader();
            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    pn = new csPartNumber();

                    pn.icon = GetSourceIcon(dr["parts_diagram_mg"].ToString());
                    pn.id = "PARTS-" + pn.icon.ToUpper() + "-" + dr["parts_partnumber"].ToString();
                    pn.firstline = dr["parts_partnumber"].ToString();
                    pn.secondline = dr["parts_brand"].ToString() + " - " + dr["parts_description"].ToString();
                    pn.token = dr["parts_partnumber"].ToString();                    
                    pnList.Add(pn);
                }
            }
            dr.Close();

            if (pnList.Count < max)
            {
                cmd.CommandText = @"SELECT DISTINCT TOP " + (max - pnList.Count) + " parts_diagram_mg, parts_partnumber, parts_description, parts_brand FROM PartsFiche WHERE 1=1 " + where_clause + "AND parts_partnumber = '" + q + "' ORDER BY parts_description;";
                dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        pn = new csPartNumber();
                        pn.icon = GetSourceIcon(dr["parts_diagram_mg"].ToString());
                        pn.id = "PARTS-" + pn.icon.ToUpper() + "-" + dr["parts_partnumber"].ToString();
                        pn.firstline = dr["parts_partnumber"].ToString();
                        pn.secondline = dr["parts_brand"].ToString() + " - " + dr["parts_description"].ToString();
                        pn.token = dr["parts_partnumber"].ToString();                        
                        pnList.Add(pn);
                    }
                }
                dr.Close();
            }

            // now search by descriptions
            if (pnList.Count < max)
            {
                cmd.CommandText = @"SELECT DISTINCT TOP " + (max - pnList.Count) + @" parts_diagram_mg, parts_diagram, parts_imagetext, parts_brand
                                FROM PartsCustom WHERE 1=1 " + where_clause + "AND parts_imagetext LIKE '%" + q + @"%'
                            UNION
                            SELECT DISTINCT TOP " + (max - pnList.Count) + @" parts_diagram_mg, parts_diagram, parts_imagetext, parts_brand
                                FROM PartsFiche WHERE 1=1 " + where_clause + "AND parts_imagetext LIKE '%" + q + @"%'
                            ORDER BY parts_imagetext;";
                dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        pn = new csPartNumber();
                        pn.icon = GetSourceIcon(dr["parts_diagram_mg"].ToString());
                        pn.id = "DIAGRAMS-" + pn.icon.ToUpper() + "-" + dr["parts_diagram"].ToString();
                        pn.firstline = dr["parts_imagetext"].ToString();
                        pn.secondline = dr["parts_brand"].ToString() + " - " + dr["parts_diagram"].ToString();
                        pn.token = dr["parts_imagetext"].ToString();                        
                        pnList.Add(pn);
                    }
                }
                dr.Close();
            }

            if (pnList.Count < max)
            {
                cmd.CommandText = @"SELECT DISTINCT TOP " + (max - pnList.Count) + " parts_diagram_mg, parts_partnumber, parts_description, parts_brand FROM PartsCustom WHERE 1=1 " + where_clause + "AND parts_description LIKE '%" + q + "%' ORDER BY parts_description;";
                dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        pn = new csPartNumber();
                        pn.icon = GetSourceIcon(dr["parts_diagram_mg"].ToString());
                        pn.id = "PARTS-" + pn.icon.ToUpper() + "-" + dr["parts_partnumber"].ToString();
                        pn.firstline = dr["parts_partnumber"].ToString();
                        pn.secondline = dr["parts_brand"].ToString() + " - " + dr["parts_description"].ToString();
                        pn.token = dr["parts_partnumber"].ToString();                       
                        pnList.Add(pn);
                    }
                }
                dr.Close();
            }

            if (pnList.Count < max)
            {
                // From PartsFiche
                cmd.CommandText = @"SELECT DISTINCT TOP " + (max - pnList.Count) + " parts_diagram_mg, parts_partnumber, parts_description, parts_brand FROM PartsFiche WHERE 1=1 " + where_clause + "AND parts_description LIKE '%" + q + "%' ORDER BY parts_description;";
                dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        pn = new csPartNumber();
                        pn.icon = GetSourceIcon(dr["parts_diagram_mg"].ToString());
                        pn.id = "PARTS-" + pn.icon.ToUpper() + "-" + dr["parts_partnumber"].ToString();
                        pn.firstline = dr["parts_partnumber"].ToString();
                        pn.secondline = dr["parts_brand"].ToString() + " - " + dr["parts_description"].ToString();
                        pn.token = dr["parts_partnumber"].ToString();                       
                        pnList.Add(pn);
                    }
                }
                dr.Close();
            }
        
            conn.Close();
                        
            sJSON = oSerializer.Serialize(pnList);
        }
                
        Response.ContentType = "application/json";
        Response.Write(sJSON);
        Response.End();
    }

    public static bool IsNumeric(System.Object Expression)
    {
        if(Expression == null || Expression is DateTime)
            return false;

        if(Expression is Int16 || Expression is Int32 || Expression is Int64 || Expression is Decimal || Expression is Single || Expression is Double || Expression is Boolean)
            return true;
   
        try 
        {
            if(Expression is string)
                Double.Parse(Expression as string);
            else
                Double.Parse(Expression.ToString());

            return true;
        } 
        catch {}

        return false;
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
}