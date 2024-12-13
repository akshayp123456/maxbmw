<%@ Page Language="C#" AutoEventWireup="true" CodeFile="CheckoutProcessed.aspx.cs" Inherits="CheckoutProcessed" %>
<%@ Register TagPrefix="uc" TagName="UC_MainMenu" Src="~/UC_MainMenu.ascx" %>
<%@ Register TagPrefix="uc" TagName="UC_Footer" Src="~/UC_Footer.ascx" %>

<!DOCTYPE html>
<html>
<head>
    <title>MAX BMW Motorcycles - Checkout Processed</title>
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
    <link href="css/Forms.css" rel="stylesheet" type="text/css" media="all" />
</head>
<body>
<div id="centered">
    <uc:UC_MainMenu runat="server" />
                       
    <table cellpadding="0" cellspacing="0" border="0" class="NavigationBar">
        <tr>
            <td width="150">
                <span class="BackButton" onclick="window.location='<%=Utilities.GetLastCheckoutURL() %>';">
                    Continue Shopping   
                </span>           
            </td>
            <td class="BarSeparator">&nbsp;</td>
            <td style="text-align: center;">
                Checkout Status
            </td>
        </tr>
    </table>                                 
    
    <table class="Panel RoundedBottom" cellpadding="0" cellspacing="0" style="padding-left: 25px;">
        <tr>
            <td align="center">
                <%=m_PayFlowResponse %>
            </td>
        </tr>
    </table>
    <div id="ClearFooter"></div> 
</div>

<uc:UC_Footer runat="server" /> </body>
</html>
