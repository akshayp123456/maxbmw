<%@ Page Language="C#" AutoEventWireup="true" CodeFile="CartAdd.aspx.cs" Inherits="CartAdd" ValidateRequest="false" %>

<!DOCTYPE html>
<html>
<head>
    <title></title>
    <meta http-equiv="Pragma" content="no-cache" />
    <meta http-equiv="Cache-Control" content="no-cache" />
    <meta http-equiv="Expires" content="0" />        
</head>
<%if (Utilities.QueryString("updatecart") == "1") { %>
    <body>
    <!-- cart saved -->
<% } else { %>
    <body style="margin: 0px; font-family: Verdana;" onload="top.loadCartContainer(document);">
    <div id="CartContainer">
        <%if (!cartHelper.IsCartEmpty() || Utilities.QueryString("PartNumber").Length > 0) { %>      
            <label style="font-size: 13px; font-weight: bold;">Your shopping cart:</label><br />
            <br />  
            <table border="0" cellspacing="10" width="600">
                <tr>
                    <td align="center">
                        <%=cartHelper.ShowCart(Utilities.QueryString("PartNumber"), Utilities.QueryString("qty"), Utilities.QueryString("description"), Utilities.QueryString("weight"), false, true)%>
                    </td>
                </tr>
                <tr>
                    <td align="right">
                        <span class="CartButton" onclick="UpdateAndSaveCart();">&nbsp;&nbsp;<img src="images/cart.gif" alt="Update Cart" />&nbsp;Update and Save Cart&nbsp;&nbsp;</span><br /><br />
                    </td>
                </tr>                   
            </table>
            <br />
            <ul style="font-weight: bold; font-size: 8pt; color: #d00; text-align: left;">
                <li>The "Each" column depicts the price for an individual part, kit, or set.<br />Fiche defaults to what is required on the vehicle in that particular diagram.<br />Adjust quantities as needed.</li>
                <li>Overweight or oversized items may be susceptible to additional shipping charges.<br />If needed, additional payment will be taken at the time of shipment.<br />Additional shipping charges may also apply for the shipment of the balance of a previously split shipped order.</li>
                <li>Our website does NOT reflect what we have in stock.<br />Some parts may be special ordered from BMWNA or sourced from BMWAG (Germany).<br />Special orders take 3-4 business days for us to receive.<br />Parts sourced from BMWAG can take 5-20 business days for us to receive.<br />ETA of backordered parts can change or be indefinite.</li>
                <li>After the order is submitted: if you’d like to add, change, or cancel anything, please contact us by email.<br />Cancelled special orders that cannot be stopped from arriving at MAX BMW, may be subject to a 15% restocking fee.</li>
            </ul>                  
        <%} %>
    </div>
<%} %>
</body>
</html>
