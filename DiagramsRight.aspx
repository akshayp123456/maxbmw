<%@ Page Language="C#" AutoEventWireup="true" CodeFile="DiagramsRight.aspx.cs" Inherits="DiagramsRight" %>

<!DOCTYPE html>
<html>
<head>
    <title></title>    
    <meta http-equiv="Pragma" content="no-cache" />
    <meta http-equiv="CACHE-CONTROL" content="NO-CACHE" />
    <meta http-equiv="Expires" content="0" />
    <meta name="format-detection" content="telephone=no" />
</head>
<body style="margin: 0px; font-family: Verdana;" onload="<%=(m_image!="" ? "top.loadRightPanelContainer(document);" : "") %>">
<div id="RightPanelContainer" style="margin: 0px; font-family: Verdana;">
    <div id="ytapiplayer" style="display: none;">
        You need Flash player 8+ and JavaScript enabled to view this video.
    </div>
    
    <table cellpadding="0" cellspacing="0" border="0" style="max-width: 520px; min-width: 520px; width: 520px;">
        <tr style="vertical-align: middle;">
            <td align="center" style="width: 500px; background-color: #fff; font-size: 12px; font-weight: bold; color: #333; height: 20px;">
                <%=m_Diagram + " - " + m_imagetext.ToUpper() %>
            </td>
            <td><img src="images/Print.png" style="cursor: pointer;" alt="" title="Print Parts List" onclick="PrintElem('#RightPanelContainer');" /></td>
        </tr>
    </table>
    <table cellpadding="0" cellspacing="0" border="0" style="max-width: 520px; min-width: 520px; width: 520px;">
        <tr>
            <td align="center">
                <div style="position: relative;">
                    <img width="510" id="ImgProduct" src="<%=m_image + "?v=" + Utilities.VERSION %>" />
                    <span id="ButtonZoomIn"></span>
                    <%if (m_ProductImages.Count > 0) { %>
                    <span class="Slider Prev"></span><span class="Slider Next"></span>
                    <%} %>
                </div>                    
            </td>
            <td valign="bottom">
                <img alt="View Parts" src="Images/PartsDown.gif" style="cursor: pointer;" onclick="ScrollToParts();" />
            </td>
        </tr>
    </table>
        
    <%if (m_ProductImages.Count > 0) { %>
    <table cellspacing="0" cellpadding="0" class="Thumbnails">
        <tr>
            <td class="ProductThumbnailTopBar Selected"><img src="images/1pixel.gif" alt="" /></td><td><img src="images/1pixel.gif" alt="" /></td>
            <%foreach (string imgPath in m_ProductImages) { %>
                <td class="ProductThumbnailTopBar"><img src="images/1pixel.gif" alt="" /></td><td><img src="images/1pixel.gif" alt="" /></td>
            <%} %>                      
            <td><img src="images/1pixel.gif" alt="" /></td>
        </tr>
        <tr>
            <td width="40"><img class="ProductThumbnailImg Selected" src="<%=m_image %>" alt="" /><td width="3"><img src="images/1pixel.gif" alt="" /></td>
            <%foreach (string imgPath in m_ProductImages) { %>
            <td width="40"><img class="ProductThumbnailImg" src="<%=imgPath %>" alt="" /></td><td width="3"><img src="images/1pixel.gif" alt="" /></td>
            <%} %>                      
            <td>&nbsp;</td>
        </tr>
    </table>
    <%} %>

    <%if (m_ProductVideos.Count > 0) { %>
    <table cellspacing="0" cellpadding="0" class="Thumbnails">
        <tr valign="middle">                        
            <%foreach (string videoPath in m_ProductVideos) { %>
            <td width="82"><img class="ThumbnailVideo" src="images/YouTubeVideos.jpg" alt="<%=videoPath %>" /></td>
            <%} %>                      
            <td>&nbsp;</td>
        </tr>
    </table>
    <%} %>

    <%if (m_ProductPDFs.Count > 0) { %>
    <table cellspacing="0" cellpadding="0" class="Thumbnails">
        <tr valign="middle">                        
            <%foreach (string pdfPath in m_ProductPDFs) { %>
            <td width="82"><img class="ThumbnailPDF" src="images/PDF.jpg" alt="<%=pdfPath %>" /></td>
            <%} %>                      
            <td>&nbsp;</td>
        </tr>
    </table>
    <%} %>
    <div style="padding: 5px; font-size: 13px; color: #333; width: 508px;">             
		<%=m_DiagramNoteComments %>
    </div>  
                                           
    <%=ShowParts() %>    
    <div style="display: none;"><%=DateTime.Now.ToString() %>&nbsp;</div>   
</div> 
</body>
</html>
