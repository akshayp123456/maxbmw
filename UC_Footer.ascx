<%@ Control Language="C#" AutoEventWireup="true" CodeFile="UC_Footer.ascx.cs" Inherits="UC_Footer" %>
<div id="Footer">
    <table cellpadding="0" cellspacing="0" width="100%" style="padding-top: 10px; padding-bottom: 7px; padding-left: 7px; padding-right: 7px;">
        <tr valign="middle">
            <td width="200">&copy; MAX BMW Motorcycles - <%=DateTime.Now.Year %></td>
            <td width="15">&nbsp;</td>
            <td width="30"><a href="http://www.facebook.com/MAXBMWMotorcycles" target="_blank" style="text-decoration: none;"><img alt="Facebook" src="images/facebook.png" border="0" title="See us on Facebook" /></a></td>
            <td width="30"><a href="http://maxbmwmotorcycles.smugmug.com" target="_blank" style="text-decoration: none;"><img alt="SmugMug" src="images/smugmug.png" border="0" title="Watch our pictures on SmugMug" /></a></td>
            <td width="30"><a href="http://www.youtube.com/user/MAXBMWNH" target="_blank" style="text-decoration: none;"><img alt="YouTube" src="images/youtube.png" border="0" title="Watch our videos on youtube" /></a></td>
            <td width="30"><a href="http://216.15.127.178:8000/listen.m3u" target="_blank" style="text-decoration: none;"><img alt="Radio" src="images/radio.png" border="0" title="Listen to the MAX BMW on-line radio" /></a></td>
            <td width="50" align="right"><img alt="AppleReady" src="images/AppleReady.png" title="iPad & iPhone ready" /></td>
            <td width="120">&nbsp;iPad &amp; iPhone ready</td>
            <td><a href="FAQ.pdf" target="_blank" style="text-decoration: none; color: #ccc;">See our FAQ</a></td>
            <td width="160" align="right">NH <%=DateTime.Now.ToString() %></td>
        </tr>
    </table>
</div>