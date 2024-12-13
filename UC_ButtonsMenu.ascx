<%@ Control Language="C#" AutoEventWireup="true" CodeFile="UC_ButtonsMenu.ascx.cs" Inherits="UC_ButtonsMenu" %>
<div style="position: relative;">
    <table cellpadding="0" cellspacing="0" border="0" width="100%">
        <tr>
            <td colspan="9" style="height: 6px;"><img src="images/1pixel.gif" style="display: block" /></td>
        </tr>
        <tr>
            <td class="ButtonMenu<%=(this.Page.ToString().ToLower().IndexOf(".partsfiche")>0 ? " activelook":"") %>" onclick="$('#Loading').show(); window.location='<%=(m_vid!="" && m_vid!="0" && m_vid!="99999" ? "DiagramsMain.aspx?vid=" + m_vid : "PartsFiche.aspx") %>';" title="Fiche Technical Diagrams">
                Fiche<br />
                <img src="images/ButtonFicheBgd.png" alt="" />                
            </td>
            <td>&nbsp;&nbsp;</td>
            <td class="ButtonMenu<%=(this.Page.ToString().ToLower().IndexOf(".partscatalog")>0 ? " activelook":"") %>" onclick="$('#Loading').show(); window.location='PartsCatalog.aspx<%=(m_vid!="" && m_vid!="0" && m_vid!="99999" ? "?vid=" + m_vid : "") %>';" title="Catalog: Accessories, Performance and Aftermarket, filter by bike model or category">
                Catalog<br />
                <img src="images/ButtonCatalogBgd.png" alt="" />
            </td>
            <td>&nbsp;&nbsp;</td>
            <td class="ButtonMenu<%=(this.Page.ToString().ToLower().IndexOf(".partstires")>0 ? " activelook":"") %>" onclick="$('#Loading').show(); window.location='PartsTires.aspx<%=(m_vid!="" && m_vid!="0" && m_vid!="99999" ? "?vid=" + m_vid : "") %>';" title="Low promotional prices on many tires">
                Tires<br />
                <img src="images/ButtonTiresBgd.png" alt="" />
            </td>
            <td>&nbsp;&nbsp;</td>
            <td class="ButtonMenu<%=(this.Page.ToString().ToLower().IndexOf(".partsapparel")>0 ? " activelook":"") %>" onclick="$('#Loading').show(); window.location='PartsApparel.aspx<%=(m_vid!="" && m_vid!="0" && m_vid!="99999" ? "?vid=" + m_vid : "") %>';" title="BMW and aftermarket apparel, jackets, suits, pants, gloves, boots, heated gear, helmets and clothing">
                Apparel<br />
                <img src="images/ButtonApparelBgd.png" alt="" />
            </td>
            <td>&nbsp;&nbsp;</td>
            <td class="ButtonMenu<%=(this.Page.ToString().ToLower().IndexOf(".partssearch")>0 ? " activelook":"") %>" onclick="$('#Loading').show(); window.location='PartsSearch.aspx<%=(m_vid!="" && m_vid!="0" && m_vid!="99999" ? "?vid=" + m_vid : "") %>';" title="Search by Part Number, filter by model, fiche or non-fiche">
                Search<br />
                <img src="images/ButtonSearchBgd.png" alt="" />
            </td>
        </tr>
        <tr>
            <td colspan="9" style="height: 6px;"><img src="images/1pixel.gif" style="display: block" /></td>
        </tr>
    </table>
    <div id="HeaderBarTopArrow" style="width: 17px; position: absolute; bottom: -4px; display: none;"><img src="images/BarTopArrow.png" style="width: 17px; display:block; border: 0px;" /></div>
</div>
