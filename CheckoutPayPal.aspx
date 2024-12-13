<%@ Page Language="C#" AutoEventWireup="true" CodeFile="CheckoutPayPal.aspx.cs" Inherits="CheckoutPayPal" %>
<%@ Register TagPrefix="uc" TagName="UC_MainMenu" Src="~/UC_MainMenu.ascx" %>
<%@ Register TagPrefix="uc" TagName="UC_CheckoutTermsAndConditions" Src="~/UC_CheckoutTermsAndConditions.ascx" %>
<%@ Register TagPrefix="uc" TagName="UC_Footer" Src="~/UC_Footer.ascx" %>

<!DOCTYPE html>
<html>
<head>
    <title>MAX BMW Motorcycles - Checkout</title>
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

    <script src="https://code.jquery.com/jquery-1.12.0.min.js"></script>
    <script src="https://code.jquery.com/jquery-migrate-1.2.1.min.js"></script>
</head>
<body>
<div id="centered">
    <uc:UC_MainMenu runat="server" />
                       
    <table cellpadding="0" cellspacing="0" border="0" class="NavigationBar">
        <tr>
            <td width="150">
                <span class="BackButton" onclick="window.location='fiche.aspx';">
                    Main Menu   
                </span>           
            </td>
            <td class="BarSeparator">&nbsp;</td>
            <td style="text-align: center;">
                Checkout - PayPal
            </td>
        </tr>
    </table>                                 
    
    <uc:UC_CheckoutTermsAndConditions runat="server" />

    <table class="Panel RoundedBottom" cellpadding="0" cellspacing="0" style="padding-left: 25px;">
        <tr>
            <td>
                <br />
                <br />
                <p style="font-weight: bold; font-size: 14px;">You are being redirected to PayPal...</p><br />
                <form action="https://www.paypal.com/cgi-bin/webscr" method="POST" id="FORM1" name="FORM1">
                    <input type="hidden" name="cmd" value="_xclick" />
                    <input type="hidden" name="business" value="<%= ConfigurationManager.AppSettings["EmailPaypal"] %>" />
                    <input type="hidden" name="item_name" value="WEB ORDER #<%= c.OrderID.ToString() %>" />
                    <input type="hidden" name="amount" value="<%= string.Format("{0:0.00}", c.Total) %>" />
                    <br /><br /><p style="font-weight: bold; font-size: 14px;">Click this button if you were not redirected automatically to PayPal after 10 seconds:</p>
                    <input type="submit" value="Go to PayPal" />
                </form>
                <script type="text/javascript">
        	        document.getElementById('FORM1').submit();
                </script>
            </td>
        </tr>
    </table>  
</div>   
<br />          
</body>
</html>
