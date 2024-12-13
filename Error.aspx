<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Error.aspx.cs" Inherits="Error" %>
<%@ Register TagPrefix="uc" TagName="UC_MainMenu" Src="~/UC_MainMenu.ascx" %>
<%@ Register TagPrefix="uc" TagName="UC_Footer" Src="~/UC_Footer.ascx" %>

<!DOCTYPE html>
<html>
<head>
    <title>MAX BMW Motorcycles - Error</title>
    <meta http-equiv="Pragma" content="no-cache" />
    <meta http-equiv="CACHE-CONTROL" content="NO-CACHE" />
    <meta http-equiv="Expires" content="0" />
    <meta name="rating" content="general" />
    <meta name="author" content="MAX BMW Motorcycles" /> 
    <meta name="apple-mobile-web-app-capable" content="yes" />
    <meta name="apple-mobile-web-app-status-bar-style" content="black" />   
    <meta name="format-detection" content="telephone=no" />

    <link href="css/Global.css" rel="stylesheet" type="text/css" media="all" />
    <link href="css/MainMenu.css" rel="stylesheet" type="text/css" media="all" />

    <script type="text/javascript" src="js/ie6no.js" charset="utf-8"></script>
    <script type="text/javascript">
        ie6no({ runonload: true });
    </script>

    <!--#include file="images/GoogleAnalytics.inc" -->
</head>

<body>
<div id="centered">
    <uc:UC_MainMenu runat="server" />
    
    <table cellpadding="0" cellspacing="0" border="0" class="NavigationBar">
        <tr>
            <td style="text-align: center;">
                Error
            </td>
        </tr>
    </table>  

    <table class="Panel RoundedBottom" cellpadding="0" cellspacing="0" style="padding-left: 25px;">
        <tr>
            <td>
                <br />
                <br />
                Oppsss... there was an error on the page.<br />
                <br />
                <%=m_Injection %>
                <br />
                <br />
                <span class="Button" onclick="window.location='<%=Utilities.URL_HOME() %>/index.aspx';">&nbsp;&nbsp;Go Back&nbsp;&nbsp;</span><br />
                <br />
            </td>
        </tr>
    </table>      
              
    <div id="ClearFooter"></div> 
</div>

<uc:UC_Footer runat="server" /> 
   
</body>
</html>
