<%@ Control Language="C#" AutoEventWireup="true" CodeFile="UC_MainMenu.ascx.cs" Inherits="UC_MainMenu" %>
<table id="MainMenuTable" cellpadding="0" cellspacing="0" border="0">
    <tr>
        <td>
            <table cellpadding="0" cellspacing="0" border="0">
                <tr valign="middle">
                    <td width="5">&nbsp;</td>
                    <td><img src="images/MAXLogo.png" alt="MAX BMW Motorcycles" height="38" /></td>
                    <td width="160">&nbsp;</td>
                    <td>&nbsp;</td>
                </tr>
            </table>
            <table cellpadding="0" cellspacing="0" border="0">
                <tr>
                    <td width="8">&nbsp;</td>
                    <td class="MainMenu"><a href="https://www.maxbmw.com">Home</a></td>
                    <td width="15">&nbsp;</td>
                    <td class="MainMenu"><a href="https://shop.maxbmw.com/machine/inventory.aspx">Motorcycles</a></td>
                    <td width="15">&nbsp;</td>
                    <td class="MainMenuSelected"><a href="javascript:window.location='<%=Utilities.URL_FICHE(false) %>/fiche.aspx';">Parts</a></td>
                    <td width="15">&nbsp;</td>
                    <td class="MainMenu"><a href="https://www.maxbmw.com/service-repair-motorcycles-dealership--service">Service</a></td>
                    <td width="15">&nbsp;</td> 
                    <td class="MainMenu"><a href="https://www.maxbmw.com/events-calendar-motorcycles-dealership--xcalendar">Events</a></td>
                    <td width="15">&nbsp;</td>
                    <td class="MainMenu"><a href="https://www.maxbmw.com/about-us-motorcycles-dealership--info">About</a></td>
                    <td width="15">&nbsp;</td>
                    <td class="MainMenu"><a href="https://www.maxbmw.com/contact-email-motorcycles-dealership--xcontact">Contact</a></td>
                    <td width="70">&nbsp;</td>
                    <% if (Page.Request.Url.ToString().IndexOf("User")>-1 || Page.Request.Url.ToString().IndexOf("CheckoutReview")>-1) { %>
					<td width="100" class="MainMenu">&nbsp;</td>
					<% } else if (Session["UserID"]!=null) { %>
                    <td width="100" align="right" class="MainMenu"><a href="javascript:window.location='<%=Utilities.URL_FICHE(true) %>/UserEdit.aspx';">My Account</a></td>
				    <td width="70" align="right" class="MainMenu"><a href="javascript:window.location='<%=Utilities.URL_FICHE(true) %>/UserLogout.aspx';">Logout</a></td>					
					<% } else { %>
			        <td width="70" align="right" class="MainMenu"><a href="javascript:window.location='<%=Utilities.URL_FICHE(true) %>/UserLogin.aspx';">Login</a></td>
					<% } %>
                                       
                    <td width="30">&nbsp;</td>
                    <% if (Page.Request.Url.ToString().IndexOf("Checkout")>-1 || Page.Request.Url.ToString().IndexOf("User")>-1) { %>
					<td width="110" class="MainMenu">&nbsp;</td>
					<% } else { %>
                        <%if (Page.Request.Url.ToString().IndexOf("CartView") > -1) { %>
                    <td width="70">&nbsp;</td>
					    <%} else { %>
                    <td class="MainMenu"><a href="javascript:window.location='<%=Utilities.URL_FICHE(true) %>/CartView.aspx';">View Cart</a></td>
                        <%} %>
                    <td width="30">&nbsp;</td>
					<td class="MainMenu"><a href="javascript:window.location='<%=Utilities.URL_FICHE(true) %>/Checkout2.aspx';">Checkout</a>&nbsp;</td>              						
					<% } %>
                </tr>
            </table>
        </td>
        <td align="right">
            <img src="images/DealerLogo.png" height="61" alt="Delear Logo" />
        </td>
    </tr>
</table>
    