<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Checkout.aspx.cs" Inherits="Checkout" %>
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
        
    <script type="text/javascript" src="js/ie6no.js" charset="utf-8"></script>
    <script type="text/javascript">
        ie6no({ runonload: true });
    </script>

    <script src="js/jquery-1.7.1.min.js"></script>
    <script type="text/javascript" src="js/FormValidations.js"></script>

    <!-- Shopping Cart functions -->
    <script type="text/javascript">
    function AfterLoad()
    {                                                           
        UpdateFields();
        document.getElementById('EMAIL').focus();
        $('.RequiredField').attr('title', 'This field is required');
        UpdateCountry();
    }        
        
    function NumToCurrency(num)
    {
	    num = num.toString().replace(/\$|\,/g,'');
	    if(isNaN(num))
		    num = "0";
	    sign = (num == (num = Math.abs(num)));
	    num = Math.floor(num*100+0.50000000001);
	    cents = num%100;
	    num = Math.floor(num/100).toString();
	    if(cents<10)
		    cents = "0" + cents;

	    for (var i = 0; i < Math.floor((num.length-(1+i))/3); i++)
		    num = num.substring(0,num.length-(4*i+3))+','+num.substring(num.length-(4*i+3));
        	
	    return (((sign)?'':'-') + '$' + num + '.' + cents);
    }

    function CurrencyToNum(num)
    {
	    var x;
	    num = num.replace(/\$|\,|\%/g,'');
	    if(isNaN(num))
		    num = "0";
	    sign = (num == (num = Math.abs(num)));
	    num = Math.floor(num*100+0.50000000001);
	    cents = num%100;
	    if (cents!=0) cents=cents/100;
	    num = Math.floor(num/100);	
	    return (num+cents);
	}

    function UpdateFields()
    { 
	    var SubTotal = 0.0;
	    var AdditionalShipping = 0.0;
	    var TotalWeight = 0.0;
	    var id = "";
	    var INPUTField = document.getElementsByTagName("INPUT");            
        for (i=0; i<INPUTField.length; i++) {
            if ( INPUTField[i].id.indexOf("TP_")==0 ) {
			    SubTotal += CurrencyToNum(INPUTField[i].value);
            } else if ( INPUTField[i].id.indexOf("AS_")==0 ) {
			    id = INPUTField[i].id.substring(3, INPUTField[i].id.length);			        		
			    AdditionalShipping += CurrencyToNum(INPUTField[i].value) * document.getElementById("Q_" + id).value;			        
            } else if ( INPUTField[i].id.indexOf("W_")==0 ) {
                id = INPUTField[i].id.substring(2, INPUTField[i].id.length);
                if (!isNaN(INPUTField[i].value))
                {
                    TotalWeight += INPUTField[i].value * document.getElementById("Q_" + id).value;		        
                }
            }
        }                     
        document.getElementById('AdditionalShipping').value = NumToCurrency(AdditionalShipping);            
	    document.getElementById('SubTotal').value = NumToCurrency(SubTotal);
	    document.getElementById('TotalWeight').value = Math.round(TotalWeight*100)/100;
    }
                         
    function ValidateFields()
    {
        var Problems = "";           
        Problems += ValidateField('FIRSTNAME', 'First Name', 1, nameRegxp);            
        Problems += ValidateField('LASTNAME', 'Last Name', 1, nameRegxp);
        if (document.getElementById('COMPANY'))
            Problems += ValidateField('COMPANY', 'Company', 0, addressRegxp);
        if (document.getElementById('EMAIL2')!=null)
        {
            Problems += ValidateField('EMAIL', 'e-mail', 4, emailRegxp);
              
            if (document.getElementById('EMAIL').value != document.getElementById('EMAIL2').value) {
                Problems += "The e-mail and re-entered e-mail don't match.\n\r";
            }
        }
        Problems += ValidateField('ADDRESS', 'Address', 4, addressRegxp);
        Problems += ValidateField('ADDRESS2', 'Address', 0, addressRegxp);
        Problems += ValidateField('CITY', 'City', 1, nameRegxp);
        if (document.getElementById('STATE_US').value.length == 0 && document.getElementById('STATE_CA').value.length == 0 && document.getElementById('STATE_INTERNATIONAL').value.length == 0 ) {
            Problems += "Invalid State (Province).\n\r";
        }
        if (document.getElementById('COUNTRY').value.length == 0) {
            Problems += "Invalid Country.\n\r";
        }
        Problems += validateZIP(document.getElementById('ZIP').value, document.getElementById('COUNTRY').value);
            
        if (document.getElementById('PHONE').value.length==0 && document.getElementById('PHONE2').value.length==0 && document.getElementById('PHONE3').value.length==0) {
            Problems += "At least one of the three phone numbers must be entered in case we need to contact you.\n\r";
        }

        if (Problems.length>0)
        {
            alert(Problems + "\n\r\n\rPlease correct the required fields and try again.\n\r");
            return false;
        }

        return true;
    }
        
    function UpdateCountry()
    {
        var Country = document.getElementById('COUNTRY').value;
        if (Country == "US") {            
            document.getElementById('STATE_US').style.display = "block";
            document.getElementById('STATE_CA').style.display = "none";
            document.getElementById('STATE_INTERNATIONAL').style.display = "none";
        } else if (Country == "CA") {
            document.getElementById('STATE_US').style.display = "none";
            document.getElementById('STATE_CA').style.display = "block";
            document.getElementById('STATE_INTERNATIONAL').style.display = "none";
        } else {
            document.getElementById('STATE_US').style.display = "none";
            document.getElementById('STATE_CA').style.display = "none";
            document.getElementById('STATE_INTERNATIONAL').style.display = "block";
        }
    }
    </script>
</head>
<body <%=(cartHelper.IsCartEmpty()?"":"onload=\"AfterLoad();\"") %>>
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
                Checkout - Step 1 of 2
            </td>
        </tr>
    </table>                                 
    
    <uc:UC_CheckoutTermsAndConditions runat="server" />            
                                
<%if (!cartHelper.IsCartEmpty())
    { %>                                            
    <form id="CHECKOUTFORM" method="post" action="CheckoutReview.aspx">                            
        <table class="Panel RoundedBottom" cellpadding="0" cellspacing="0" style="padding-left: 25px;">
            <tr>
                <td align="left" class="Heading">
                    Review Shopping Cart Items:
                </td>
            </tr>
            <tr>
                <td align="center" class="Content">
                    <%=cartHelper.ShowCart("", "", "", "", true, !m_IsWholesale)%>
                    <br /><br />                                                
                    <label>Coupon, offer code, sale code, enter it here: </label><input id="COUPONS" name="COUPONS" type="text" maxlength="20" value="<%=m_Coupons %>" />                                                
                </td>
            </tr>
            <tr>
                <td align="left">&nbsp;</td>
            </tr>

            <%if (Session["UserID"] == null) { %>
            <tr>
                <td align="center" style="font-size: 13px; font-weight: bold; color: #cc0000;">
                    <br />  
                    If you have an account already, <a href="UserLogin.aspx">click here to login</a> to expedite the checkout.<br />
                    <br />
                    <br />
                    If you do not have an account yet, you can <a href="UserRegister.aspx">create an account by clicking here</a>.
                    <br />
                    <br />
                </td>
            </tr>
            <%} %>

            <tr>
                <td align="left" class="Heading">
                    Your e-mail:
                </td>
            </tr>
            <tr>
                <td align="left" class="Content">
                        e-mail: <input name="EMAIL" id="EMAIL" value="<%=m_eMail %>" type="text" <%=((Session["UserID"] == null)?"":"readonly=\"readonly\"")%> style="width:230px; border-color: #bbb;" /><br />
                        <br />
                    <%if (m_IsWholesale)
                        {%>                                                
                        <span style="color: #d50; font-weight: bold; font-size: 10px;">
                            You are registered as a wholesale user<%=(m_Company.Length > 0 ? " for '" + m_Company + "'" : "")%>.<br />
                            <%if (m_WholesaleDiscount.Length > 0)
                                {%>
                            On the next page you will see your discounts of <%=m_WholesaleDiscount%>.<br />
                            <%} %>
                            <%if (m_WholesaleDiscount.Length > 0 && m_MinimumPurchase.Length > 0)
                                {%>
                            You need to place an order of a minimum of <%=m_MinimumPurchase%> for your discount to be applied.<br />
                            <%} %>
                            Your shipping address needs to match our records for this discount to be valid.
                        </span>
                    <%} %>                                                
                </td>
            </tr>
       
            <tr>
                <td align="left">&nbsp;</td>
            </tr>
            <tr>
                <td align="left" class="Heading">
                    Enter Shipping Information<br /><br />
                    New: In-Store Pickup is also available at the 4 stores!<br />
                    You can pick this option on the next page, but we need your address anyways.
                </td>
            </tr>
            <tr>
                <td align="left" class="Content">
                    <table cellspacing="0" cellpadding="1" border="0">   			  									  
                        <tr>
                            <td>First Name (Given Name):</td>
                            <td style="font-weight: normal;">&nbsp;&nbsp;MI:</td>
                            <td>&nbsp;&nbsp;Last Name (Surname):</td>
                        </tr>
                        <tr>
                            <td><input name="FIRSTNAME" id="FIRSTNAME" value="<%=m_FirstName %>" type="text" maxlength="30" style="width:230px;" /></td>
                            <td>&nbsp;&nbsp;<input name="MIDDLENAME" id="MIDDLENAME" value="<%=m_MiddleName %>" type="text" maxlength="1" style="width:20px;" /></td>
                            <td>&nbsp;&nbsp;<input name="LASTNAME" id="LASTNAME" value="<%=m_LastName %>" type="text" maxlength="30" style="width:230px;" /></td>
                        </tr>
                        <%if (m_IsWholesale)
                            { %>
                        <tr>
                            <td colspan="3">Company:</td>
                        </tr>
                        <tr>
                            <td colspan="3"><input name="COMPANY" id="COMPANY" value="<%=m_Company %>" type="text" maxlength="30" style="width:230px;" /></td>
                        </tr>
                        <%} %>
                    </table>
                    <table cellspacing="0" cellpadding="1" border="0">
                        <tr>
                            <td>Country:</td>
                        </tr>
                        <tr>
                            <td>                                
                                <select id="COUNTRY" name="COUNTRY" style="width: 240px;" onchange="UpdateCountry();">
                                    <option value="US"<%=(m_Country == "" || m_Country == "US" ? " selected=\"selected\"" : "") %>>United States of America</option>
                                    <!--
                                    <option value="AF"<%=(m_Country == "AF" ? " selected=\"selected\"" : "") %>>Afghanistan</option>
                                    <option value="AL"<%=(m_Country == "AL" ? " selected=\"selected\"" : "") %>>Albania</option>
                                    <option value="DZ"<%=(m_Country == "DZ" ? " selected=\"selected\"" : "") %>>Algeria</option>
                                    <option value="AD"<%=(m_Country == "AD" ? " selected=\"selected\"" : "") %>>Andorra</option>
                                    <option value="AO"<%=(m_Country == "AO" ? " selected=\"selected\"" : "") %>>Angola</option>
                                    <option value="AI"<%=(m_Country == "AI" ? " selected=\"selected\"" : "") %>>Anguilla</option>
                                    <option value="AG"<%=(m_Country == "AG" ? " selected=\"selected\"" : "") %>>Antigua and Barbuda</option>
                                    <option value="AR"<%=(m_Country == "AR" ? " selected=\"selected\"" : "") %>>Argentina</option>
                                    <option value="AM"<%=(m_Country == "AM" ? " selected=\"selected\"" : "") %>>Armenia</option>
                                    <option value="AW"<%=(m_Country == "AW" ? " selected=\"selected\"" : "") %>>Aruba</option>
                                    <option value="AU"<%=(m_Country == "AU" ? " selected=\"selected\"" : "") %>>Australia</option>
                                    <option value="AT"<%=(m_Country == "AT" ? " selected=\"selected\"" : "") %>>Austria</option>
                                    <option value="AZ"<%=(m_Country == "AZ" ? " selected=\"selected\"" : "") %>>Azerbaijan</option>
                                    <option value="BS"<%=(m_Country == "BS" ? " selected=\"selected\"" : "") %>>Bahamas</option>
                                    <option value="BH"<%=(m_Country == "BH" ? " selected=\"selected\"" : "") %>>Bahrain</option>
                                    <option value="BD"<%=(m_Country == "BD" ? " selected=\"selected\"" : "") %>>Bangladesh</option>
                                    <option value="BB"<%=(m_Country == "BB" ? " selected=\"selected\"" : "") %>>Barbados</option>
                                    <option value="BY"<%=(m_Country == "BY" ? " selected=\"selected\"" : "") %>>Belarus</option>
                                    <option value="BE"<%=(m_Country == "BE" ? " selected=\"selected\"" : "") %>>Belgium</option>
                                    <option value="BZ"<%=(m_Country == "BZ" ? " selected=\"selected\"" : "") %>>Belize</option>
                                    <option value="BJ"<%=(m_Country == "BJ" ? " selected=\"selected\"" : "") %>>Benin</option>
                                    <option value="BM"<%=(m_Country == "BM" ? " selected=\"selected\"" : "") %>>Bermuda</option>
                                    <option value="BT"<%=(m_Country == "BT" ? " selected=\"selected\"" : "") %>>Bhutan</option>
                                    <option value="BO"<%=(m_Country == "BO" ? " selected=\"selected\"" : "") %>>Bolivia</option>
                                    <option value="BA"<%=(m_Country == "BA" ? " selected=\"selected\"" : "") %>>Bosnia-Herzegovina</option>
                                    <option value="BW"<%=(m_Country == "BW" ? " selected=\"selected\"" : "") %>>Botswana</option>
                                    <option value="BR"<%=(m_Country == "BR" ? " selected=\"selected\"" : "") %>>Brazil</option>
                                    <option value="IO"<%=(m_Country == "IO" ? " selected=\"selected\"" : "") %>>British Virgin Islands</option>
                                    <option value="BN"<%=(m_Country == "BN" ? " selected=\"selected\"" : "") %>>Brunei Darussalam</option>
                                    <option value="BG"<%=(m_Country == "BG" ? " selected=\"selected\"" : "") %>>Bulgaria</option>
                                    <option value="BF"<%=(m_Country == "BF" ? " selected=\"selected\"" : "") %>>Burma</option>
                                    <option value="BI"<%=(m_Country == "BI" ? " selected=\"selected\"" : "") %>>Burundi</option>
                                    <option value="KH"<%=(m_Country == "KH" ? " selected=\"selected\"" : "") %>>Cambodia</option>
                                    <option value="CM"<%=(m_Country == "CM" ? " selected=\"selected\"" : "") %>>Cameroon</option>
                                    -->
                                    <option value="CA"<%=(m_Country == "CA" ? " selected=\"selected\"" : "") %>>Canada</option>
                                    <!--
                                    <option value="CV"<%=(m_Country == "CV" ? " selected=\"selected\"" : "") %>>Cape Verde</option>
                                    <option value="KY"<%=(m_Country == "KY" ? " selected=\"selected\"" : "") %>>Cayman Islands</option>
                                    <option value="CF"<%=(m_Country == "CF" ? " selected=\"selected\"" : "") %>>Central African Republic</option>
                                    <option value="TD"<%=(m_Country == "TD" ? " selected=\"selected\"" : "") %>>Chad</option>
                                    <option value="CL"<%=(m_Country == "CL" ? " selected=\"selected\"" : "") %>>Chile</option>
                                    <option value="CN"<%=(m_Country == "CN" ? " selected=\"selected\"" : "") %>>China</option>
                                    <option value="CO"<%=(m_Country == "CO" ? " selected=\"selected\"" : "") %>>Colombia</option>
                                    <option value="KM"<%=(m_Country == "KM" ? " selected=\"selected\"" : "") %>>Comoros</option>
                                    <option value="CD"<%=(m_Country == "CD" ? " selected=\"selected\"" : "") %>>Congo, Democratic Republic of the</option>
                                    <option value="CG"<%=(m_Country == "CG" ? " selected=\"selected\"" : "") %>>Congo, Republic of the</option>
                                    <option value="CR"<%=(m_Country == "CR" ? " selected=\"selected\"" : "") %>>Costa Rica</option>
                                    <option value="CI"<%=(m_Country == "CI" ? " selected=\"selected\"" : "") %>>Cote d’Ivoire</option>
                                    <option value="HR"<%=(m_Country == "HR" ? " selected=\"selected\"" : "") %>>Croatia</option>
                                    <option value="CU"<%=(m_Country == "CU" ? " selected=\"selected\"" : "") %>>Cuba</option>
                                    <option value="CY"<%=(m_Country == "CY" ? " selected=\"selected\"" : "") %>>Cyprus</option>
                                    <option value="CZ"<%=(m_Country == "CZ" ? " selected=\"selected\"" : "") %>>Czech Republic</option>
                                    <option value="DK"<%=(m_Country == "DK" ? " selected=\"selected\"" : "") %>>Denmark</option>
                                    <option value="DJ"<%=(m_Country == "DJ" ? " selected=\"selected\"" : "") %>>Djibouti</option>
                                    <option value="DM"<%=(m_Country == "DM" ? " selected=\"selected\"" : "") %>>Dominica</option>
                                    <option value="DO"<%=(m_Country == "DO" ? " selected=\"selected\"" : "") %>>Dominican Republic</option>
                                    <option value="EC"<%=(m_Country == "EC" ? " selected=\"selected\"" : "") %>>Ecuador</option>
                                    <option value="EG"<%=(m_Country == "EG" ? " selected=\"selected\"" : "") %>>Egypt</option>
                                    <option value="SV"<%=(m_Country == "SV" ? " selected=\"selected\"" : "") %>>El Salvador</option>
                                    <option value="GQ"<%=(m_Country == "GQ" ? " selected=\"selected\"" : "") %>>Equatorial Guinea</option>
                                    <option value="ER"<%=(m_Country == "ER" ? " selected=\"selected\"" : "") %>>Eritrea</option>
                                    <option value="EE"<%=(m_Country == "EE" ? " selected=\"selected\"" : "") %>>Estonia</option>
                                    <option value="ET"<%=(m_Country == "ET" ? " selected=\"selected\"" : "") %>>Ethiopia</option>
                                    <option value="FO"<%=(m_Country == "FO" ? " selected=\"selected\"" : "") %>>Faroe Islands</option>
                                    <option value="FJ"<%=(m_Country == "FJ" ? " selected=\"selected\"" : "") %>>Fiji</option>
                                    <option value="FI"<%=(m_Country == "FI" ? " selected=\"selected\"" : "") %>>Finland</option>
                                    <option value="FR"<%=(m_Country == "FR" ? " selected=\"selected\"" : "") %>>France</option>
                                    <option value="GF"<%=(m_Country == "GF" ? " selected=\"selected\"" : "") %>>French Guiana</option>
                                    <option value="PF"<%=(m_Country == "PF" ? " selected=\"selected\"" : "") %>>French Polynesia</option>
                                    <option value="GA"<%=(m_Country == "GA" ? " selected=\"selected\"" : "") %>>Gabon</option>
                                    <option value="GM"<%=(m_Country == "GM" ? " selected=\"selected\"" : "") %>>Gambia</option>
                                    <option value="GE"<%=(m_Country == "GE" ? " selected=\"selected\"" : "") %>>Georgia, Republic of</option>
                                    <option value="DE"<%=(m_Country == "DE" ? " selected=\"selected\"" : "") %>>Germany</option>
                                    <option value="GH"<%=(m_Country == "GH" ? " selected=\"selected\"" : "") %>>Ghana</option>
                                    <option value="GI"<%=(m_Country == "GI" ? " selected=\"selected\"" : "") %>>Gibraltar</option>
                                    <option value="GB"<%=(m_Country == "GB" ? " selected=\"selected\"" : "") %>>Great Britain and Northern Ireland</option>
                                    <option value="GR"<%=(m_Country == "GR" ? " selected=\"selected\"" : "") %>>Greece</option>
                                    <option value="GL"<%=(m_Country == "GL" ? " selected=\"selected\"" : "") %>>Greenland</option>
                                    <option value="GD"<%=(m_Country == "GD" ? " selected=\"selected\"" : "") %>>Grenada</option>
                                    <option value="GP"<%=(m_Country == "GP" ? " selected=\"selected\"" : "") %>>Guadeloupe</option>
                                    <option value="GT"<%=(m_Country == "GT" ? " selected=\"selected\"" : "") %>>Guatemala</option>
                                    <option value="GN"<%=(m_Country == "GN" ? " selected=\"selected\"" : "") %>>Guinea</option>
                                    <option value="GW"<%=(m_Country == "GW" ? " selected=\"selected\"" : "") %>>Guinea–Bissau</option>
                                    <option value="GY"<%=(m_Country == "GY" ? " selected=\"selected\"" : "") %>>Guyana</option>
                                    <option value="HT"<%=(m_Country == "HT" ? " selected=\"selected\"" : "") %>>Haiti</option>
                                    <option value="HN"<%=(m_Country == "HN" ? " selected=\"selected\"" : "") %>>Honduras</option>
                                    <option value="HK"<%=(m_Country == "HK" ? " selected=\"selected\"" : "") %>>Hong Kong</option>
                                    <option value="HU"<%=(m_Country == "HU" ? " selected=\"selected\"" : "") %>>Hungary</option>
                                    <option value="IS"<%=(m_Country == "IS" ? " selected=\"selected\"" : "") %>>Iceland</option>
                                    <option value="IN"<%=(m_Country == "IN" ? " selected=\"selected\"" : "") %>>India</option>
                                    <option value="ID"<%=(m_Country == "ID" ? " selected=\"selected\"" : "") %>>Indonesia</option>
                                    <option value="IR"<%=(m_Country == "IR" ? " selected=\"selected\"" : "") %>>Iran</option>
                                    <option value="IQ"<%=(m_Country == "IQ" ? " selected=\"selected\"" : "") %>>Iraq</option>
                                    <option value="IE"<%=(m_Country == "IE" ? " selected=\"selected\"" : "") %>>Ireland</option>
                                    <option value="IL"<%=(m_Country == "IL" ? " selected=\"selected\"" : "") %>>Israel</option>
                                    <option value="IT"<%=(m_Country == "IT" ? " selected=\"selected\"" : "") %>>Italy</option>
                                    <option value="JM"<%=(m_Country == "JM" ? " selected=\"selected\"" : "") %>>Jamaica</option>
                                    <option value="JP"<%=(m_Country == "JP" ? " selected=\"selected\"" : "") %>>Japan</option>
                                    <option value="JO"<%=(m_Country == "JO" ? " selected=\"selected\"" : "") %>>Jordan</option>
                                    <option value="KZ"<%=(m_Country == "KZ" ? " selected=\"selected\"" : "") %>>Kazakhstan</option>
                                    <option value="KE"<%=(m_Country == "KE" ? " selected=\"selected\"" : "") %>>Kenya</option>
                                    <option value="KI"<%=(m_Country == "KI" ? " selected=\"selected\"" : "") %>>Kiribati</option>
                                    <option value="KP"<%=(m_Country == "KP" ? " selected=\"selected\"" : "") %>>Korea, Democratic People’s Republic</option>
                                    <option value="KR"<%=(m_Country == "KR" ? " selected=\"selected\"" : "") %>>Korea, Republic of (South Korea)</option>
                                    <option value="KW"<%=(m_Country == "KW" ? " selected=\"selected\"" : "") %>>Kuwait</option>
                                    <option value="KG"<%=(m_Country == "KG" ? " selected=\"selected\"" : "") %>>Kyrgyzstan</option>
                                    <option value="LA"<%=(m_Country == "LA" ? " selected=\"selected\"" : "") %>>Laos</option>
                                    <option value="LV"<%=(m_Country == "LV" ? " selected=\"selected\"" : "") %>>Latvia</option>
                                    <option value="LB"<%=(m_Country == "LB" ? " selected=\"selected\"" : "") %>>Lebanon</option>
                                    <option value="LS"<%=(m_Country == "LS" ? " selected=\"selected\"" : "") %>>Lesotho</option>
                                    <option value="LR"<%=(m_Country == "LR" ? " selected=\"selected\"" : "") %>>Liberia</option>
                                    <option value="LY"<%=(m_Country == "LY" ? " selected=\"selected\"" : "") %>>Libya</option>
                                    <option value="LI"<%=(m_Country == "LI" ? " selected=\"selected\"" : "") %>>Liechtenstein</option>
                                    <option value="LT"<%=(m_Country == "LT" ? " selected=\"selected\"" : "") %>>Lithuania</option>
                                    <option value="LU"<%=(m_Country == "LU" ? " selected=\"selected\"" : "") %>>Luxembourg</option>
                                    <option value="MO"<%=(m_Country == "MO" ? " selected=\"selected\"" : "") %>>Macao</option>
                                    <option value="MK"<%=(m_Country == "MK" ? " selected=\"selected\"" : "") %>>Macedonia, Republic of</option>
                                    <option value="MG"<%=(m_Country == "MG" ? " selected=\"selected\"" : "") %>>Madagascar</option>
                                    <option value="MW"<%=(m_Country == "MW" ? " selected=\"selected\"" : "") %>>Malawi</option>
                                    <option value="MY"<%=(m_Country == "MY" ? " selected=\"selected\"" : "") %>>Malaysia</option>
                                    <option value="MV"<%=(m_Country == "MV" ? " selected=\"selected\"" : "") %>>Maldives</option>
                                    <option value="ML"<%=(m_Country == "ML" ? " selected=\"selected\"" : "") %>>Mali</option>
                                    <option value="MT"<%=(m_Country == "MT" ? " selected=\"selected\"" : "") %>>Malta</option>
                                    <option value="MQ"<%=(m_Country == "MQ" ? " selected=\"selected\"" : "") %>>Martinique</option>
                                    <option value="MR"<%=(m_Country == "MR" ? " selected=\"selected\"" : "") %>>Mauritania</option>
                                    <option value="MU"<%=(m_Country == "MU" ? " selected=\"selected\"" : "") %>>Mauritius</option>
                                    <option value="MX"<%=(m_Country == "MX" ? " selected=\"selected\"" : "") %>>Mexico</option>
                                    <option value="FM"<%=(m_Country == "FM" ? " selected=\"selected\"" : "") %>>Moldova</option>
                                    <option value="MN"<%=(m_Country == "MN" ? " selected=\"selected\"" : "") %>>Mongolia</option>
                                    <option value="ME"<%=(m_Country == "ME" ? " selected=\"selected\"" : "") %>>Montenegro</option>
                                    <option value="MS"<%=(m_Country == "MS" ? " selected=\"selected\"" : "") %>>Montserrat</option>
                                    <option value="MA"<%=(m_Country == "MA" ? " selected=\"selected\"" : "") %>>Morocco</option>
                                    <option value="MZ"<%=(m_Country == "MZ" ? " selected=\"selected\"" : "") %>>Mozambique</option>
                                    <option value="NA"<%=(m_Country == "NA" ? " selected=\"selected\"" : "") %>>Namibia</option>
                                    <option value="NR"<%=(m_Country == "NR" ? " selected=\"selected\"" : "") %>>Nauru</option>
                                    <option value="NP"<%=(m_Country == "NP" ? " selected=\"selected\"" : "") %>>Nepal</option>
                                    <option value="NL"<%=(m_Country == "NL" ? " selected=\"selected\"" : "") %>>Netherlands</option>
                                    <option value="AN"<%=(m_Country == "AN" ? " selected=\"selected\"" : "") %>>Netherlands Antilles</option>
                                    <option value="NC"<%=(m_Country == "NC" ? " selected=\"selected\"" : "") %>>New Caledonia</option>
                                    <option value="NZ"<%=(m_Country == "NZ" ? " selected=\"selected\"" : "") %>>New Zealand</option>
                                    <option value="NI"<%=(m_Country == "NI" ? " selected=\"selected\"" : "") %>>Nicaragua</option>
                                    <option value="NE"<%=(m_Country == "NE" ? " selected=\"selected\"" : "") %>>Niger</option>
                                    <option value="NG"<%=(m_Country == "NG" ? " selected=\"selected\"" : "") %>>Nigeria</option>                                    
                                    <option value="NO"<%=(m_Country == "NO" ? " selected=\"selected\"" : "") %>>Norway</option>
                                    <option value="OM"<%=(m_Country == "OM" ? " selected=\"selected\"" : "") %>>Oman</option>
                                    <option value="PK"<%=(m_Country == "PK" ? " selected=\"selected\"" : "") %>>Pakistan</option>
                                    <option value="PA"<%=(m_Country == "PA" ? " selected=\"selected\"" : "") %>>Panama</option>
                                    <option value="PG"<%=(m_Country == "PG" ? " selected=\"selected\"" : "") %>>Papua New Guinea</option>
                                    <option value="PY"<%=(m_Country == "PY" ? " selected=\"selected\"" : "") %>>Paraguay</option>
                                    <option value="PE"<%=(m_Country == "PE" ? " selected=\"selected\"" : "") %>>Peru</option>
                                    <option value="PH"<%=(m_Country == "PH" ? " selected=\"selected\"" : "") %>>Philippines</option>
                                    <option value="PN"<%=(m_Country == "PN" ? " selected=\"selected\"" : "") %>>Pitcairn Island</option>
                                    <option value="PL"<%=(m_Country == "PL" ? " selected=\"selected\"" : "") %>>Poland</option>
                                    <option value="PT"<%=(m_Country == "PT" ? " selected=\"selected\"" : "") %>>Portugal</option>
                                    <option value="QA"<%=(m_Country == "QA" ? " selected=\"selected\"" : "") %>>Qatar</option>
                                    <option value="RE"<%=(m_Country == "RE" ? " selected=\"selected\"" : "") %>>Reunion</option>
                                    <option value="RO"<%=(m_Country == "RO" ? " selected=\"selected\"" : "") %>>Romania</option>
                                    <option value="RU"<%=(m_Country == "RU" ? " selected=\"selected\"" : "") %>>Russia</option>
                                    <option value="RW"<%=(m_Country == "RW" ? " selected=\"selected\"" : "") %>>Rwanda</option>
                                    <option value="SH"<%=(m_Country == "SH" ? " selected=\"selected\"" : "") %>>Saint Helena</option>
                                    <option value="LC"<%=(m_Country == "LC" ? " selected=\"selected\"" : "") %>>Saint Lucia</option>
                                    <option value="PM"<%=(m_Country == "PM" ? " selected=\"selected\"" : "") %>>Saint Pierre and Miquelon</option>
                                    <option value="VC"<%=(m_Country == "VC" ? " selected=\"selected\"" : "") %>>Saint Vincent and the Grenadines</option>
                                    <option value="SM"<%=(m_Country == "SM" ? " selected=\"selected\"" : "") %>>San Marino</option>
                                    <option value="ST"<%=(m_Country == "ST" ? " selected=\"selected\"" : "") %>>Sao Tome and Principe</option>
                                    <option value="SA"<%=(m_Country == "SA" ? " selected=\"selected\"" : "") %>>Saudi Arabia</option>
                                    <option value="SN"<%=(m_Country == "SN" ? " selected=\"selected\"" : "") %>>Senegal</option>
                                    <option value="CS"<%=(m_Country == "CS" ? " selected=\"selected\"" : "") %>>Serbia, Republic of</option>
                                    <option value="SC"<%=(m_Country == "SC" ? " selected=\"selected\"" : "") %>>Seychelles</option>
                                    <option value="SL"<%=(m_Country == "SL" ? " selected=\"selected\"" : "") %>>Sierra Leone</option>
                                    <option value="SG"<%=(m_Country == "SG" ? " selected=\"selected\"" : "") %>>Singapore</option>
                                    <option value="SI"<%=(m_Country == "SI" ? " selected=\"selected\"" : "") %>>Slovenia</option>
                                    <option value="SB"<%=(m_Country == "SB" ? " selected=\"selected\"" : "") %>>Solomon Islands</option>
                                    <option value="SO"<%=(m_Country == "SO" ? " selected=\"selected\"" : "") %>>Somalia</option>
                                    <option value="ZA"<%=(m_Country == "ZA" ? " selected=\"selected\"" : "") %>>South Africa</option>
                                    <option value="ES"<%=(m_Country == "ES" ? " selected=\"selected\"" : "") %>>Spain</option>
                                    <option value="LK"<%=(m_Country == "LK" ? " selected=\"selected\"" : "") %>>Sri Lanka</option>
                                    <option value="SD"<%=(m_Country == "SD" ? " selected=\"selected\"" : "") %>>Sudan</option>
                                    <option value="SR"<%=(m_Country == "SR" ? " selected=\"selected\"" : "") %>>Suriname</option>
                                    <option value="SZ"<%=(m_Country == "SZ" ? " selected=\"selected\"" : "") %>>Swaziland</option>
                                    <option value="SE"<%=(m_Country == "SE" ? " selected=\"selected\"" : "") %>>Sweden</option>
                                    <option value="CH"<%=(m_Country == "CH" ? " selected=\"selected\"" : "") %>>Switzerland</option>
                                    <option value="TW"<%=(m_Country == "TW" ? " selected=\"selected\"" : "") %>>Taiwan</option>
                                    <option value="TJ"<%=(m_Country == "TJ" ? " selected=\"selected\"" : "") %>>Tajikistan</option>
                                    <option value="TZ"<%=(m_Country == "TZ" ? " selected=\"selected\"" : "") %>>Tanzania</option>
                                    <option value="TH"<%=(m_Country == "TH" ? " selected=\"selected\"" : "") %>>Thailand</option>
                                    <option value="TG"<%=(m_Country == "TG" ? " selected=\"selected\"" : "") %>>Togo</option>
                                    <option value="TO"<%=(m_Country == "TO" ? " selected=\"selected\"" : "") %>>Tonga</option>
                                    <option value="TT"<%=(m_Country == "TT" ? " selected=\"selected\"" : "") %>>Trinidad and Tobago</option>
                                    <option value="TN"<%=(m_Country == "TN" ? " selected=\"selected\"" : "") %>>Tunisia</option>
                                    <option value="TR"<%=(m_Country == "TR" ? " selected=\"selected\"" : "") %>>Turkey</option>
                                    <option value="TM"<%=(m_Country == "TM" ? " selected=\"selected\"" : "") %>>Turkmenistan</option>
                                    <option value="TC"<%=(m_Country == "TC" ? " selected=\"selected\"" : "") %>>Turks and Caicos Islands</option>
                                    <option value="TV"<%=(m_Country == "TV" ? " selected=\"selected\"" : "") %>>Tuvalu</option>
                                    <option value="UG"<%=(m_Country == "UG" ? " selected=\"selected\"" : "") %>>Uganda</option>
                                    <option value="UA"<%=(m_Country == "UA" ? " selected=\"selected\"" : "") %>>Ukraine</option>
                                    <option value="AE"<%=(m_Country == "AE" ? " selected=\"selected\"" : "") %>>United Arab Emirates</option>
                                    <option value="UY"<%=(m_Country == "UY" ? " selected=\"selected\"" : "") %>>Uruguay</option>
                                    <option value="UZ"<%=(m_Country == "UZ" ? " selected=\"selected\"" : "") %>>Uzbekistan</option>
                                    <option value="VU"<%=(m_Country == "VU" ? " selected=\"selected\"" : "") %>>Vanuatu</option>
                                    <option value="VE"<%=(m_Country == "VE" ? " selected=\"selected\"" : "") %>>Venezuela</option>
                                    <option value="VN"<%=(m_Country == "VN" ? " selected=\"selected\"" : "") %>>Vietnam</option>
                                    <option value="WF"<%=(m_Country == "WF" ? " selected=\"selected\"" : "") %>>Wallis and Futuna Islands</option>
                                    <option value="EH"<%=(m_Country == "EH" ? " selected=\"selected\"" : "") %>>Western Samoa</option>
                                    <option value="YE"<%=(m_Country == "YE" ? " selected=\"selected\"" : "") %>>Yemen</option>
                                    <option value="ZM"<%=(m_Country == "ZM" ? " selected=\"selected\"" : "") %>>Zambia</option>
                                    <option value="ZW"<%=(m_Country == "ZW" ? " selected=\"selected\"" : "") %>>Zimbabwe</option>
                                    -->
                                </select>
                                &nbsp;&nbsp;&nbsp;<span style="color: #e00">Due to COVID-19 we are only shipping to the United States and Canada.</span>
                            </td>
                        </tr>
                    </table>
                    <table cellspacing="0" cellpadding="1" border="0">
                        <tr>
                            <td>Address:</td>
                        </tr>
                        <tr>
                            <td>
                                <input name="ADDRESS" id="ADDRESS" value="<%=m_Address %>" maxlength="30" type="text" style="width:300px;" /><br />
                                <input name="ADDRESS2" id="ADDRESS2" value="<%=m_Address2 %>" maxlength="30" type="text" style="width:300px;" />                                                        
                            </td>
                        </tr>
                    </table>
                    <table cellspacing="0" cellpadding="1" border="0">
                        <tr>
                            <td>City:</td>
                            <td>&nbsp;&nbsp;State (Province):</td>
                            <td>&nbsp;&nbsp;ZIP (Postal Code):</td>
                        </tr>
                        <tr>
                            <td>
                                <input name="CITY" id="CITY" type="text" value="<%=m_City %>" maxlength="30" style="width:160px;" />
                            </td>
                            <td>                                
                                <select id="STATE_US" name="STATE_US" style="width: 200px; display: block;">
                                    <option value=""<%=(m_State == "" ? " selected=\"selected\"" : "") %>></option>
                                    <option value="AL"<%=(m_State == "AL" ? " selected=\"selected\"" : "") %>>AL-Alabama</option>
                                    <option value="AK"<%=(m_State == "AK" ? " selected=\"selected\"" : "") %>>AK-Alaska</option>
                                    <option value="AZ"<%=(m_State == "AZ" ? " selected=\"selected\"" : "") %>>AZ-Arizona</option>
                                    <option value="AR"<%=(m_State == "AR" ? " selected=\"selected\"" : "") %>>AR-Arkansas</option>
                                    <option value="CA"<%=(m_State == "CA" ? " selected=\"selected\"" : "") %>>CA-California</option>
                                    <option value="CO"<%=(m_State == "CO" ? " selected=\"selected\"" : "") %>>CO-Colorado</option>
                                    <option value="CT"<%=(m_State == "CT" ? " selected=\"selected\"" : "") %>>CT-Connecticut</option>
                                    <option value="DE"<%=(m_State == "DE" ? " selected=\"selected\"" : "") %>>DE-Delaware</option>
                                    <option value="DC"<%=(m_State == "DC" ? " selected=\"selected\"" : "") %>>DC-District of Columbia</option>
                                    <option value="FL"<%=(m_State == "FL" ? " selected=\"selected\"" : "") %>>FL-Florida</option>
                                    <option value="GA"<%=(m_State == "GA" ? " selected=\"selected\"" : "") %>>GA-Georgia</option>
                                    <option value="HI"<%=(m_State == "HI" ? " selected=\"selected\"" : "") %>>HI-Hawaii</option>
                                    <option value="ID"<%=(m_State == "ID" ? " selected=\"selected\"" : "") %>>ID-Idaho</option>
                                    <option value="IL"<%=(m_State == "IL" ? " selected=\"selected\"" : "") %>>IL-Illinois</option>
                                    <option value="IN"<%=(m_State == "IN" ? " selected=\"selected\"" : "") %>>IN-Indiana</option>
                                    <option value="IA"<%=(m_State == "IA" ? " selected=\"selected\"" : "") %>>IA-Iowa</option>
                                    <option value="KS"<%=(m_State == "KS" ? " selected=\"selected\"" : "") %>>KS-Kansas</option>
                                    <option value="KY"<%=(m_State == "KY" ? " selected=\"selected\"" : "") %>>KY-Kentucky</option>
                                    <option value="LA"<%=(m_State == "LA" ? " selected=\"selected\"" : "") %>>LA-Louisiana</option>
                                    <option value="ME"<%=(m_State == "ME" ? " selected=\"selected\"" : "") %>>ME-Maine</option>
                                    <option value="MD"<%=(m_State == "MD" ? " selected=\"selected\"" : "") %>>MD-Maryland</option>
                                    <option value="MA"<%=(m_State == "MA" ? " selected=\"selected\"" : "") %>>MA-Massachusetts</option>
                                    <option value="MI"<%=(m_State == "MI" ? " selected=\"selected\"" : "") %>>MI-Michigan</option>
                                    <option value="MN"<%=(m_State == "MN" ? " selected=\"selected\"" : "") %>>MN-Minnesota</option>
                                    <option value="MS"<%=(m_State == "MS" ? " selected=\"selected\"" : "") %>>MS-Mississippi</option>
                                    <option value="MO"<%=(m_State == "MO" ? " selected=\"selected\"" : "") %>>MO-Missouri</option>
                                    <option value="MT"<%=(m_State == "MT" ? " selected=\"selected\"" : "") %>>MT-Montana</option>
                                    <option value="NE"<%=(m_State == "NE" ? " selected=\"selected\"" : "") %>>NE-Nebraska</option>
                                    <option value="NV"<%=(m_State == "NV" ? " selected=\"selected\"" : "") %>>NV-Nevada</option>
                                    <option value="NH"<%=(m_State == "NH" ? " selected=\"selected\"" : "") %>>NH-New Hampshire</option>
                                    <option value="NJ"<%=(m_State == "NJ" ? " selected=\"selected\"" : "") %>>NJ-New Jersey</option>
                                    <option value="NM"<%=(m_State == "NM" ? " selected=\"selected\"" : "") %>>NM-New Mexico</option>
                                    <option value="NY"<%=(m_State == "NY" ? " selected=\"selected\"" : "") %>>NY-New York</option>
                                    <option value="NC"<%=(m_State == "NC" ? " selected=\"selected\"" : "") %>>NC-North Carolina</option>
                                    <option value="ND"<%=(m_State == "ND" ? " selected=\"selected\"" : "") %>>ND-North Dakota</option>
                                    <option value="OH"<%=(m_State == "OH" ? " selected=\"selected\"" : "") %>>OH-Ohio</option>
                                    <option value="OK"<%=(m_State == "OK" ? " selected=\"selected\"" : "") %>>OK-Oklahoma</option>
                                    <option value="OR"<%=(m_State == "OR" ? " selected=\"selected\"" : "") %>>OR-Oregon</option>
                                    <option value="PA"<%=(m_State == "PA" ? " selected=\"selected\"" : "") %>>PA-Pennsylvania</option>                                                                
                                    <option value="RI"<%=(m_State == "RI" ? " selected=\"selected\"" : "") %>>RI-Rhode Island</option>
                                    <option value="SC"<%=(m_State == "SC" ? " selected=\"selected\"" : "") %>>SC-South Carolina</option>
                                    <option value="SD"<%=(m_State == "SD" ? " selected=\"selected\"" : "") %>>SD-South Dakota</option>
                                    <option value="TN"<%=(m_State == "TN" ? " selected=\"selected\"" : "") %>>TN-Tennessee</option>
                                    <option value="TX"<%=(m_State == "TX" ? " selected=\"selected\"" : "") %>>TX-Texas</option>
                                    <option value="UT"<%=(m_State == "UT" ? " selected=\"selected\"" : "") %>>UT-Utah</option>
                                    <option value="VT"<%=(m_State == "VT" ? " selected=\"selected\"" : "") %>>VT-Vermont</option>
                                    <option value="VA"<%=(m_State == "VA" ? " selected=\"selected\"" : "") %>>VA-Virginia</option>
                                    <option value="WA"<%=(m_State == "WA" ? " selected=\"selected\"" : "") %>>WA-Washington</option>
                                    <option value="WV"<%=(m_State == "WV" ? " selected=\"selected\"" : "") %>>WV-West Virginia</option>
                                    <option value="WI"<%=(m_State == "WI" ? " selected=\"selected\"" : "") %>>WI-Wisconsin</option>
                                    <option value="WY"<%=(m_State == "WY" ? " selected=\"selected\"" : "") %>>WY-Wyoming</option>                                                                
                                    <option value="">----------------------------------</option>
                                    <option value="PR"<%=(m_State == "PR" ? " selected=\"selected\"" : "") %>>PR-Puerto Rico</option>
                                    <option value="AS"<%=(m_State == "AS" ? " selected=\"selected\"" : "") %>>AS-American Samoa</option>
                                    <option value="FM"<%=(m_State == "FM" ? " selected=\"selected\"" : "") %>>FM-Federated States of Micronesia</option>
                                    <option value="GU"<%=(m_State == "GU" ? " selected=\"selected\"" : "") %>>GU-Guam</option>
                                    <option value="MH"<%=(m_State == "MH" ? " selected=\"selected\"" : "") %>>MH-Marshall Islands</option>
                                    <option value="MP"<%=(m_State == "MP" ? " selected=\"selected\"" : "") %>>MP-Northern Mariana Islands</option>
                                    <option value="PW"<%=(m_State == "PW" ? " selected=\"selected\"" : "") %>>PW-Palau</option>
                                    <option value="VI"<%=(m_State == "VI" ? " selected=\"selected\"" : "") %>>VI-US Virgin Islands</option>                                                                
                                    <option value="">----------------------------------</option>
                                    <option value="AA"<%=(m_State == "AA" ? " selected=\"selected\"" : "") %>>AA-Armed Forces Americas</option>
                                    <option value="AP"<%=(m_State == "AP" ? " selected=\"selected\"" : "") %>>AP-Armed Forces Pacific</option>
                                    <option value="AE"<%=(m_State == "AE" ? " selected=\"selected\"" : "") %>>AE-Armed Forces (Other)</option>                                                               
                                </select>
                                <select id="STATE_CA" name="STATE_CA" style="width: 200px; display: none;">
                                    <option value=""<%=(m_State == "" ? " selected=\"selected\"" : "") %>></option>
                                    <option value="AB"<%=(m_State == "AB" ? " selected=\"selected\"" : "") %>>AB-Alberta</option>
                                    <option value="BC"<%=(m_State == "BC" ? " selected=\"selected\"" : "") %>>BC-British Columbia</option>
                                    <option value="MB"<%=(m_State == "MB" ? " selected=\"selected\"" : "") %>>MB-Manitoba</option>
                                    <option value="NB"<%=(m_State == "NB" ? " selected=\"selected\"" : "") %>>NB-New Brunswick</option>
                                    <option value="NL"<%=(m_State == "NL" ? " selected=\"selected\"" : "") %>>NL-Newfound. and Labrador</option>
                                    <option value="NS"<%=(m_State == "NS" ? " selected=\"selected\"" : "") %>>NS-Nova Scotia</option>
                                    <option value="NT"<%=(m_State == "NT" ? " selected=\"selected\"" : "") %>>NT-Northwest Territories</option>
                                    <option value="NU"<%=(m_State == "NU" ? " selected=\"selected\"" : "") %>>NU-Nunavut</option>
                                    <option value="ON"<%=(m_State == "ON" ? " selected=\"selected\"" : "") %>>ON-Ontario</option>
                                    <option value="PE"<%=(m_State == "PE" ? " selected=\"selected\"" : "") %>>PE-Prince Edward Island</option>
                                    <option value="QC"<%=(m_State == "QC" ? " selected=\"selected\"" : "") %>>QC-Quebec</option>
                                    <option value="SK"<%=(m_State == "SK" ? " selected=\"selected\"" : "") %>>SK-Saskatchewan</option>
                                    <option value="YT"<%=(m_State == "YT" ? " selected=\"selected\"" : "") %>>YT-Yukon</option>
                                </select>
                                <input name="STATE_INTERNATIONAL" id="STATE_INTERNATIONAL" type="text" value="<%=m_State %>" style="width:200px; display: none;" maxlength="30" />                                                               
                            </td>
                            <td>
                                &nbsp;<input name="ZIP" id="ZIP" type="text" value="<%=m_ZIP %>" style="width:105px;" maxlength="10" />
                            </td>
                        </tr>
                    </table>
                    <%if (m_ShippingError != "")
                        { %>
                    <table cellspacing="0" cellpadding="10" style="border: 2px solid #f33">
                        <tr>
                            <td><%=m_ShippingError%></td>
                        </tr>                        
					</table>
                    <%} %>
                    <table cellspacing="0" cellpadding="1" border="0">
                        <tr>
                            <td>Home Phone:</td>
                            <td>&nbsp;&nbsp;Work Phone:</td>
                            <td>&nbsp;&nbsp;Cell Phone:</td>
                        </tr>
                        <tr>
                            <td><input name="PHONE" id="PHONE" type="text" value="<%=m_Phone %>" style="width:150px;" maxlength="20" /></td>
                            <td>&nbsp;&nbsp;<input name="PHONE2" id="PHONE2" type="text" value="<%=m_Phone2 %>" style="width:150px;" maxlength="20" /></td>
                            <td>&nbsp;&nbsp;<input name="PHONE3" id="PHONE3" type="text" value="<%=m_Phone3 %>" style="width:150px;" maxlength="20" /></td>
                        </tr>
					</table>
                </td>
            </tr>                                        
            <tr>
                <td align="left">&nbsp;</td>
            </tr>
            
            <tr>
                <td align="center">
                    <br />
                    
                        <span class="Button" onclick="if (ValidateFields()) document.getElementById('CHECKOUTFORM').submit();">&nbsp;&nbsp;Continue&nbsp;&nbsp;</span>
                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                    
                    <span class="Button" onclick="window.location='<%=Utilities.GetLastCheckoutURL() %>';">&nbsp;&nbsp;Cancel and go back&nbsp;&nbsp;</span>
                    <br />
                    <br />
                </td>
            </tr>                                        
        </table>                    
    </form>
<%} else {%>
    <table class="Panel RoundedBottom" cellpadding="0" cellspacing="0">
        <tr style="height: 200px;">
            <td align="center" style="color: #000; font-size: 14px; font-weight: bold;">
                Your shopping cart is empty.
            </td>
        </tr>
    </table>
<%} %>
    <br />
    <div id="ClearFooter"></div> 
</div>

<uc:UC_Footer runat="server" /> 

</body>
</html>
