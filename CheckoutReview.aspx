<%@ Page Language="C#" AutoEventWireup="true" CodeFile="CheckoutReview.aspx.cs" Inherits="CheckoutReview" %>
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
        
    <script type="text/javascript" src="js/FormValidations.js"></script>    

    <script src="https://code.jquery.com/jquery-1.12.0.min.js"></script>
    <script src="https://code.jquery.com/jquery-migrate-1.2.1.min.js"></script>
    <style>
        .input[type=radio] {
          vertical-align: -3px;
        }
    </style>
    
    <script type="text/javascript">
        $(document).ready(function () {
            $("#CONTINUE").click(function () {
                if (ValidateFields() == true) {
                    $("#CHECKOUTFORM").submit();
                }
            });

            $("#COPYADDRESS").click(function () {
                CopyAddress();
            });
        });

        function ChangePaymentType()
        {
            if (document.getElementById('PAYMENTTYPE_CC').checked) {
                // Credit Card
                document.getElementById('DIV_CC').style.display = 'block';
                document.getElementById('DIV_PP').style.display = 'none';
                $("#CHECKOUTFORM").attr("action", "CheckoutCard.aspx");
            } else {
                // PayPal
                document.getElementById('DIV_CC').style.display = 'none';
                document.getElementById('DIV_PP').style.display = 'block';                
                $("#CHECKOUTFORM").attr("action", "CheckoutPayPal.aspx");
            }
        }

        function AfterLoad()
        {
            UpdateCountry();
            UpdateFields();                        
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
            var Total = 0.0; 
	        var SubTotal = 0.0;
	        var AdditionalShipping = 0.0;
	        var TotalWeight = 0.0;
	        var Shipping = 0.0;	        
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
                } else if (INPUTField[i].id.indexOf("SHIPPINGMETHOD_") == 0 && INPUTField[i].checked) {
                    Shipping = CurrencyToNum(INPUTField[i].value.substring(INPUTField[i].value.indexOf("$") + 1, INPUTField[i].value.length));
			   }
            }

            if (document.getElementById('SHIPPINGMETHOD_FREE_0') != null && document.getElementById('SHIPPINGMETHOD_FREE_0').checked) {
                AdditionalShipping = 0.0;
            }

            if (document.getElementById('SHIPPINGMETHOD_PICKUP_0') != null && document.getElementById('SHIPPINGMETHOD_PICKUP_0').checked) {
                AdditionalShipping = 0.0;
            }
            if (document.getElementById('SHIPPINGMETHOD_PICKUP_1') != null && document.getElementById('SHIPPINGMETHOD_PICKUP_1').checked) {
                AdditionalShipping = 0.0;
            }
            if (document.getElementById('SHIPPINGMETHOD_PICKUP_2') != null && document.getElementById('SHIPPINGMETHOD_PICKUP_2').checked) {
                AdditionalShipping = 0.0;
            }
            if (document.getElementById('SHIPPINGMETHOD_PICKUP_3') != null && document.getElementById('SHIPPINGMETHOD_PICKUP_3').checked) {
                AdditionalShipping = 0.0;
            }

            document.getElementById('AdditionalShipping').value = NumToCurrency(AdditionalShipping);            
	        document.getElementById('SubTotal').value = NumToCurrency(SubTotal);
	        document.getElementById('TotalWeight').value = Math.round(TotalWeight*100)/100;
        
	        if (AdditionalShipping>0.0)
	        {
	            document.getElementById('TR_ADDITIONALSHIPPING').style.display = "";
	            document.getElementById('TR_SUMMARY_ADDITIONALSHIPPING').style.display = "";
	        } else {
	            document.getElementById('TR_ADDITIONALSHIPPING').style.display = "none";
	            document.getElementById('TR_SUMMARY_ADDITIONALSHIPPING').style.display = "none";
            }
	        
	        // Summary
	        document.getElementById('SummarySubTotal').value = document.getElementById('SubTotal').value;
	        document.getElementById('SummaryAdditionalShipping').value = document.getElementById('AdditionalShipping').value;
	        document.getElementById('SummaryShipping').value = NumToCurrency(Shipping);
	                
	        // Coupons Discounts
            var CouponsDiscount = 0.0;
            document.getElementById('TR_SUMMARY_COUPONS_DISCOUNTS').style.display = "none";
	        if (document.getElementById('CouponsDiscount')!=null)
	        {
	            CouponsDiscount = CurrencyToNum(document.getElementById('CouponsDiscount').value);
	            document.getElementById('SummaryCouponsDiscount').value = document.getElementById('CouponsDiscount').value;
	            document.getElementById('TR_SUMMARY_COUPONS_DISCOUNTS').style.display = "";
            }

	        
	        var CouponsPercentage = 0;
            var CouponsPercentageMoney = 0.0;
            document.getElementById('TR_SUMMARY_COUPONS_PERCENTAGE').style.display = "none";
	        if (document.getElementById('CouponsPercentage')!=null)
	        {
	            CouponsPercentage = CurrencyToNum(document.getElementById('CouponsPercentage').value);	            
	            CouponsPercentageMoney = -(SubTotal * CouponsPercentage/100.0);
	            document.getElementById('CouponsPercentageMoney').value = NumToCurrency(CouponsPercentageMoney);
	            document.getElementById('SummaryCouponsPercentageMoney').value = document.getElementById('CouponsPercentageMoney').value;
                document.getElementById('SummaryCouponsPercentage').value = document.getElementById('CouponsPercentage').value;
	            document.getElementById('TR_SUMMARY_COUPONS_PERCENTAGE').style.display = "";            
	        }
	        
	        var WholesaleDiscount = 0;
	        var WholesaleDiscountMoney = 0.0;
	        var MinimumPurchase = 0.0;
            document.getElementById('TR_SUMMARY_WHOLESALE_DISCOUNT').style.display = "none";
	        if (document.getElementById('WholesaleDiscount')!=null)
	        {
	            MinimumPurchase = document.getElementById('MinimumPurchase').value;
	            if (SubTotal>=MinimumPurchase)
	            {
	                WholesaleDiscount = document.getElementById('WholesaleDiscount').value;
	                WholesaleDiscountMoney = -(SubTotal * WholesaleDiscount/100.0);
	                document.getElementById('SummaryWholesaleDiscount').value = document.getElementById('WholesaleDiscount').value;	            
	                document.getElementById('SummaryWholesaleDiscountMoney').value = NumToCurrency(WholesaleDiscountMoney);
                    document.getElementById('TR_SUMMARY_WHOLESALE_DISCOUNT').style.display = "";
                }
	        }
	        	        
	        if (NeedsForcedVOR())
                document.getElementById('VORCHK').checked = true;
	        
	        var VOR = SubTotal*0.05;
	        var VORTotal = 0.0;
	        document.getElementById('VOR').value = NumToCurrency(VOR);
	        if (document.getElementById('VORCHK').checked==true)
	        {	
	            VORTotal = VOR;                      
	            document.getElementById('TR_SUMMARY_VOR').style.display = "";
	        } else {
	            VORTotal = 0.0;	            
	            document.getElementById('TR_SUMMARY_VOR').style.display = "none";
	        }
	        document.getElementById('SummaryVOR').value = NumToCurrency(VORTotal);
	        
	        var Taxes = 0.0;
	        Taxes = document.getElementById('SummaryTaxes').value;
	        
	        var TaxesMoney = 0.0;
	        TaxesMoney = (SubTotal * Taxes)/100.0;
	        document.getElementById('SummaryTaxesMoney').value = NumToCurrency(TaxesMoney);
	        
	        document.getElementById('TR_TAXID').style.display = "none";
	        if (document.getElementById('SummaryTaxID').value!="")
	            document.getElementById('TR_TAXID').style.display = "";	            
	            
	        document.getElementById('TR_TAXES').style.display = "none";    
	        if (document.getElementById('SummaryTaxes').value>0.0)
	            document.getElementById('TR_TAXES').style.display = "";	            	                
	        
	        Total = SubTotal + Shipping + AdditionalShipping + VORTotal + TaxesMoney - CouponsDiscount + CouponsPercentageMoney + WholesaleDiscountMoney;	        
            document.getElementById('SummaryTotal').value = NumToCurrency(Total);
        }
        
        function CopyAddress()
        {
            document.getElementById('CARDFIRSTNAME').value = document.getElementById('FIRSTNAME').value;
            document.getElementById('CARDMIDDLENAME').value = document.getElementById('MIDDLENAME').value;
            document.getElementById('CARDLASTNAME').value = document.getElementById('LASTNAME').value;
            document.getElementById('CARDPHONE').value = document.getElementById('PHONE').value;

            document.getElementById('CARDZIP').value = document.getElementById('ZIP').value;
            document.getElementById('CARDPHONE').value = document.getElementById('PHONE').value;
            document.getElementById('CARDCOUNTRY').value = document.getElementById('COUNTRY').value;
            document.getElementById('CARDADDRESS').value = document.getElementById('ADDRESS').value;
            document.getElementById('CARDADDRESS2').value = document.getElementById('ADDRESS2').value;
            document.getElementById('CARDSTATE_US').value = document.getElementById('STATE').value;
            document.getElementById('CARDSTATE_CA').value = document.getElementById('STATE').value;
            document.getElementById('CARDCITY').value = document.getElementById('CITY').value;
            UpdateCountry();
        }
        
        function NeedsForcedVOR() {
            // keywords: FedEx, USPS Express, UPS 2nd Day Air, UPS Next Day Air
            if (document.getElementById('SHIPPINGMETHOD_USPS_1') != null && document.getElementById('SHIPPINGMETHOD_USPS_1').checked) {
                if (document.getElementById('SHIPPINGMETHOD_USPS_1').value.indexOf('USPS Express') == 0)
                    return true;
            } else if (document.getElementById('SHIPPINGMETHOD_FedEx_0') != null && document.getElementById('SHIPPINGMETHOD_FedEx_0').checked) {
                return true;
            } else if (document.getElementById('SHIPPINGMETHOD_FedEx_1') != null && document.getElementById('SHIPPINGMETHOD_FedEx_1').checked) {
                return true;
            } else if (document.getElementById('SHIPPINGMETHOD_UPS_0') != null && document.getElementById('SHIPPINGMETHOD_UPS_0').checked) {
                if (document.getElementById('SHIPPINGMETHOD_UPS_0').value.indexOf('UPS 2nd Day Air') == 0)
                    return true;
                if (document.getElementById('SHIPPINGMETHOD_UPS_0').value.indexOf('UPS Next Day Air') == 0)
                    return true;
            } else if (document.getElementById('SHIPPINGMETHOD_UPS_1') != null && document.getElementById('SHIPPINGMETHOD_UPS_1').checked) {
                if (document.getElementById('SHIPPINGMETHOD_UPS_1').value.indexOf('UPS 2nd Day Air') == 0)
                    return true;
                if (document.getElementById('SHIPPINGMETHOD_UPS_1').value.indexOf('UPS Next Day Air') == 0)
                    return true;
            } else if (document.getElementById('SHIPPINGMETHOD_UPS_2') != null && document.getElementById('SHIPPINGMETHOD_UPS_2').checked) {
                if (document.getElementById('SHIPPINGMETHOD_UPS_2').value.indexOf('UPS 2nd Day Air') == 0)
                    return true;
                if (document.getElementById('SHIPPINGMETHOD_UPS_2').value.indexOf('UPS Next Day Air') == 0)
                    return true;
            }
            return false;         
        }                
        
        function ValidateFields()
        {
            var Problems = "";                      
            
            if (document.getElementById('PAYMENTTYPE_CC').checked == true) {
                Problems += ValidateField('CARDFIRSTNAME', 'Credit Card First Name', 1, nameRegxp);
                Problems += ValidateField('CARDLASTNAME', 'Credit Card Last Name', 1, nameRegxp);
                Problems += ValidateField('CARDADDRESS', 'Address', 4, addressRegxp);
                Problems += ValidateField('CARDADDRESS2', 'Address', 0, addressRegxp);
                Problems += ValidateField('CARDCITY', 'City', 1, nameRegxp);
                if (document.getElementById('CARDSTATE_US').value.length == 0 && document.getElementById('CARDSTATE_CA').value.length == 0) {
                    Problems += "Invalid State (Province).\n\r";
                }
                if (document.getElementById('CARDCOUNTRY').value.length == 0) {
                    Problems += "Invalid Country.\n\r";
                }
                Problems += validateZIP(document.getElementById('CARDZIP').value, document.getElementById('CARDCOUNTRY').value);
                Problems += ValidateField('CARDPHONE', 'Credit Card Phone Number', 10, null);
                switch(document.getElementById('CARDNUMBER').value.substring(0,1))
                {
                    case "3":
                        document.getElementById('CARDTYPE').value = "A";
                        break;
                    case "4":
                        document.getElementById('CARDTYPE').value = "V";
                        break;
                    case "5":
                        document.getElementById('CARDTYPE').value = "M";
                        break;
                    case "6":
                        document.getElementById('CARDTYPE').value = "D";
                        break;
                    default:
                        Problems += "Only VISA, MasterCard, Amex and Discover accepted.\n\r";
                        break;    
                }
                if (CheckCCNumber(document.getElementById('CARDNUMBER').value)==false) {
                    Problems += "Invalid credit card Number.\n\r";
                }
                
                if (document.getElementById('CARDEXPMONTH').value.length==0) {
                    Problems += "Invalid credit card Expiration Month.\n\r";                    
                }
                if (document.getElementById('CARDEXPYEAR').value.length==0) {
                    Problems += "Invalid credit card Expiration Year\n\r";                    
                }
                if (document.getElementById('CARDCVV2').value.length!=3) {
                    Problems += "Invalid credit card Security Check Number (CVV).\n\r";
                }
            }

            if (Problems.length>0)
            {
                alert(Problems + "\n\r\n\rPlease correct the required fields and try again.\n\r");
                return false;
            }
            return true;
        }   

        function UpdateCountry() {
            var Country = document.getElementById('CARDCOUNTRY').value;
            
            if (Country == "US" || Country == "CA") {
                
                // for US and CA, allow Credit Cards and PayPal
                document.getElementById('DIV_PAYMENTTYPE_CC').style.display = 'block';
                document.getElementById('DIV_PAYMENTTYPE_PAYPAL').style.display = 'block';

                ChangePaymentType();

                if (Country == "US") {
                    document.getElementById('CARDSTATE_US').style.display = "block";
                    document.getElementById('CARDSTATE_CA').style.display = "none";
                } else {
                    document.getElementById('CARDSTATE_US').style.display = "none";
                    document.getElementById('CARDSTATE_CA').style.display = "block";
                }
            } else {
                // for international billing, allow only PayPal
                document.getElementById('DIV_PAYMENTTYPE_CC').style.display = 'none';
                document.getElementById('DIV_PAYMENTTYPE_PAYPAL').style.display = 'block';
                document.getElementById('PAYMENTTYPE_CC').checked = false;
                document.getElementById('PAYMENTTYPE_PAYPAL').checked = true;

                ChangePaymentType();

                alert("For international billing, you will be redirected to the PayPal system after completing this form.\r\n\r\nIn the PayPal page you will be able to choose to pay with a Credit Card if desired (select \"I don't have a PayPal account\" in the PayPal page).");
            }
        }
    </script>
</head>
<body onload="AfterLoad();">
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
                Checkout - Step 2 of 2
            </td>
        </tr>
    </table>
     
    <uc:UC_CheckoutTermsAndConditions runat="server" />           
                                                                                       
    <form id="CHECKOUTFORM" method="post" action="CheckoutPayPal.aspx">                            
        <table class="Panel RoundedBottom" cellpadding="0" cellspacing="0" style="padding-left: 25px;">
            <tr>
                <td align="left" class="Heading">
                    Your Shopping Cart Items:
                </td>
            </tr>
            <tr>
                <td align="right" class="Content">
                    <%=cartHelper.ShowCart("","","","",false, !m_IsWholesale) %>
                    <br />                                               
                    <br />
                    <%if (m_Coupons!="") { %>
                    Coupons:&nbsp;
                    <%} %>
                    <input type="<%= (m_Coupons!=""?"text":"hidden")%>" name="COUPONS" id="COUPONS" readonly="readonly" style="border: 0px;" value="<%=m_Coupons %>" />
                    <%=ShowCouponsDiscounts() %>
                    <%=ShowWholesaleDiscounts() %>
                    </td>
            </tr>
                                                         
            <tr>
                <td align="left">&nbsp;</td>
            </tr>                                                                            
            <tr>
                <td align="left" class="Heading">
                    Shipping Information:
                </td>
            </tr>
            <tr>
                <td align="left" class="Content">
                    <table cellspacing="0" cellpadding="1" border="0">   			  									  
                        <tr>
                            <td style="width: 420px;">
                                <%=m_eMail %><br />
                                <%=m_FirstName %>&nbsp;<%=m_MiddleName %>&nbsp;<%=m_LastName %><br />
                                <%=(m_Company.Length > 0 ? m_Company + "<br /><br />" : "")%>
                                <%=m_Address%><br />
                                <%=(m_Address2.Length>0?m_Address2+"<br />":"")%>                                                            
                                <%=m_City %>,&nbsp;<%=m_State %>&nbsp;<%=m_ZIP %><br />
                                <%=Utilities.GetFullCountryName(m_Country) %><br />
                                <%=m_Phone %><br />
                                <%=(m_Phone2.Length > 0 ? m_Phone2 + "<br />" : "")%>
                                <%=(m_Phone3.Length > 0 ? m_Phone3 + "<br />" : "")%>
                                                            
                                <input name="EMAIL" id="EMAIL" type="hidden" value="<%=m_eMail %>" />
                                <input name="FIRSTNAME" id="FIRSTNAME" type="hidden" value="<%=m_FirstName %>" />
                                <input name="MIDDLENAME" id="MIDDLENAME" type="hidden" value="<%=m_MiddleName %>" />
                                <input name="LASTNAME" id="LASTNAME" type="hidden" value="<%=m_LastName %>" />
                                <input name="COMPANY" id="COMPANY" type="hidden" value="<%=m_Company %>" />
                                <input name="ADDRESS" id="ADDRESS" type="hidden" value="<%=m_Address %>" />
                                <input name="ADDRESS2" id="ADDRESS2" type="hidden" value="<%=m_Address2 %>" />
                                <input name="CITY" id="CITY" type="hidden" value="<%=m_City %>" />
                                <input name="STATE" id="STATE" type="hidden" value="<%=m_State %>" />
                                <input name="COUNTRY" id="COUNTRY" type="hidden" value="<%=m_Country %>" />            
                                <input name="ZIP" id="ZIP" type="hidden" value="<%=m_ZIP %>" />
                                <input name="PHONE" id="PHONE" type="hidden" value="<%=m_Phone %>" />
                                <input name="PHONE2" id="PHONE2" type="hidden" value="<%=m_Phone2 %>" />
                                <input name="PHONE3" id="PHONE3" type="hidden" value="<%=m_Phone3 %>" />
                            </td>
                            <td valign="middle" align="center">
                                <span class="Button" onclick="history.go(-1)">&nbsp;&nbsp;< Change Shipping&nbsp;&nbsp;</span>
                            </td>                                                        
                        </tr>
					</table>                   
                </td>
            </tr>
            <tr>
                <td align="left">&nbsp;</td>
            </tr>
            <tr style="<%=(m_FreeShipping?"display: none;":"") %>">
                <td align="left" class="Heading">
                    Select Shipment Method:
                </td>
            </tr>
            <tr style="<%=(m_FreeShipping?"display: none;":"") %>">
                <td align="left" class="Content">                                              
                    <br />                    
                    <%=m_ShippingOptions %>
                    <br />
                    <br />
                    Express BMW ordering: <input type="checkbox" id="VORCHK" name="VORCHK" onclick="if(this.checked==false && NeedsForcedVOR()) alert('Express BMW ordering is not optional for the selected shipping method.'); UpdateFields();" style="border: 0px; background-color: #fff; vertical-align: -3px;" />
                    Additional 5% of Subtotal&nbsp;(+<input type="text" id="VOR" name="VOR" readonly="readonly" class="Summary" style="width: 60px;" />)<br />
                    <span style="font-size: 10px; color: #c71;">
                    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Order should ship complete the following business day.<br />
                    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;BMW ONLY. Parts sourced from Germany are received in<br />
                    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;5-10 business days instead of 10-20.<br />
                    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Orders must be placed Mon-Fri before 2:30PM EST.
                    </span><br />                                                
                </td>
            </tr>
            <tr style="display: <%=(m_FreeShipping?"none;":"block;") %>">
                <td align="left">&nbsp;</td>
            </tr>
            <tr>
                <td align="left" class="Heading">
                    Review Totals:
                </td>
            </tr>
            <tr>
                <td align="left" class="Content">
                        <table cellspacing="0" cellpadding="2" border="0" style="width: 400px;">                                                    
                            <tr>
                                <td align="right" style="width: 200px;">
                                    Sub Total:
                                </td>
                                <td style="width: 200px;"> 
                                    <input type="text" id="SummarySubTotal" name="SummarySubTotal" readonly="readonly" class="Summary" />
                                </td>
                            </tr>
                            <tr>
                                <td align="right">
                                    Shipping:
                                </td>
                                <td>
                                    <input type="text" id="SummaryShipping" name="SummaryShipping" readonly="readonly" class="Summary" />
                                </td>
                            </tr>
                            <tr id="TR_SUMMARY_VOR">
                                <td align="right">
                                    Express BMW ordering:
                                </td>
                                <td>
                                    <input type="text" id="SummaryVOR" name="SummaryVOR" readonly="readonly" class="Summary" />
                                </td>
                            </tr>
                            <tr id="TR_SUMMARY_ADDITIONALSHIPPING">
                                <td align="right">
                                    Additional Shipping Charges:
                                </td>
                                <td>
                                    <input type="text" id="SummaryAdditionalShipping" name="SummaryAdditionalShipping" readonly="readonly" class="Summary" />
                                </td>
                            </tr>
                            <tr id="TR_SUMMARY_COUPONS_DISCOUNTS">
                                <td align="right">
                                    Coupons Discounts:                                                        
                                </td>
                                <td>
                                    <input type="text" id="SummaryCouponsDiscount" name="SummaryCouponsDiscount" readonly="readonly" class="Summary" />
                                </td>
                            </tr>
                            <tr id="TR_SUMMARY_COUPONS_PERCENTAGE">
                                <td align="right">
                                    Coupons Discounts <input type="text" id="SummaryCouponsPercentage" name="SummaryCouponsPercentage" readonly="readonly" class="Summary" style="width: 35px;" />:
                                </td>
                                <td>
                                    <input type="text" id="SummaryCouponsPercentageMoney" name="SummaryCouponsPercentageMoney" readonly="readonly" class="Summary" />
                                </td>
                            </tr>
                            <tr id="TR_SUMMARY_WHOLESALE_DISCOUNT">
                                <td align="right">
                                    Wholesale Discount <input type="text" id="SummaryWholesaleDiscount" name="SummaryWholesaleDiscount" readonly="readonly" class="Summary" style="width: 20px;" />%:
                                </td>
                                <td style="width: 300px;">
                                    <input type="text" id="SummaryWholesaleDiscountMoney" name="SummaryWholesaleDiscountMoney" readonly="readonly" class="Summary" />
                                </td>
                            </tr>                                                     
                            <tr id="TR_TAXES">
                                <td align="right">
                                    Taxes <input type="text" id="SummaryTaxes" name="SummaryTaxes" readonly="readonly" class="Summary" style="width: 25px;" value="<%=m_Taxes %>" />%:                                                            
                                </td>
                                <td>
                                    <input type="text" id="SummaryTaxesMoney" name="SummaryTaxesMoney" readonly="readonly" class="Summary" />
                                </td>
                            </tr>
                            <tr id="TR_TAXID">
                                <td align="right">
                                    Tax ID: <input type="text" id="SummaryTaxID" name="SummaryTaxID" readonly="readonly" class="Summary" style="width: 50px;" value="<%=m_TaxID %>" />                                                            
                                </td>
                                <td>
                                    &nbsp;
                                </td>
                            </tr>
                            <tr>
                                <td align="right" style="border-top: 1px solid #999;">
                                    TOTAL:
                                </td>
                                <td style="border-top: 1px solid #999;">
                                    <input type="text" id="SummaryTotal" name="SummaryTotal" readonly="readonly" class="Summary" />
                                </td>
                        </tr>
                    </table>                                                 
                </td>
            </tr>
            <tr>
                <td align="left">&nbsp;</td>
            </tr>
            <tr>
                <td align="left" valign="middle" class="Heading">
                     Payment Type and Billing Information:
                </td>
            </tr>
            <tr>
                <td align="left" class="Content">                  
                    
                        Payment Type:&nbsp;&nbsp;
                    <table>
                        <tr>
                            <td><div id="DIV_PAYMENTTYPE_CC"><input type="radio" value="C" name="PAYMENTTYPE" id="PAYMENTTYPE_CC" checked="checked" style="border: 0px; background-color: #fff;" onclick="ChangePaymentType();" /><img alt="Credit Card" src="images/PaymentCC.png" style="vertical-align: middle;" /></div></td>
                            <td><div id="DIV_PAYMENTTYPE_PAYPAL"><input type="radio" value="P" name="PAYMENTTYPE" id="PAYMENTTYPE_PAYPAL" style="border: 0px; background-color: #fff;" onclick="ChangePaymentType();" /><img alt="PayPal" src="images/PaymentPayPal.jpg" style="vertical-align: middle;" /></div></td>
                        </tr>
                    </table>
                    <div id="DIV_CC" style="display: block;">
                        <br />
                        <label style="font-weight: bold;"><input type="checkbox" id="COPYADDRESS" style="vertical-align: -3px;" /> Copy info from the shipping address</label>
                        <br /><br />
                        <input type="hidden" id="CARDTYPE" name="CARDTYPE" />
                        <table cellspacing="0" cellpadding="1" border="0">
                            <tr>
                                <td>First Name (Given Name):</td>
                                <td>&nbsp;&nbsp;MI:</td>
                                <td>&nbsp;&nbsp;Last Name (Surname):</td>
                            </tr>
                            <tr>
                                <td><input name="CARDFIRSTNAME" id="CARDFIRSTNAME" value="" type="text" maxlength="30" style="width:230px;" /></td>
                                <td>&nbsp;&nbsp;<input name="CARDMIDDLENAME" id="CARDMIDDLENAME" value="" type="text" maxlength="1" style="width:20px;" /></td>
                                <td>&nbsp;&nbsp;<input name="CARDLASTNAME" id="CARDLASTNAME" value="" type="text" maxlength="30" style="width:230px;" /></td>
                            </tr>
                        </table>
                        <table cellspacing="0" cellpadding="1" border="0">
                            <tr>
                                <td>
                                    <table cellspacing="0" cellpadding="1" border="0">
                                        <tr>                                                                        
                                            <td>Credit Card Number:</td>
                                            <td>&nbsp;&nbsp;&nbsp;&nbsp;Expiration:</td>
                                            <td>&nbsp;&nbsp;&nbsp;&nbsp;CVV: (3 digits on the back of the card)</td>                                            
                                        </tr>
                                        <tr valign="top">                                                                        
                                            <td>
                                                <input type="text" id="CARDNUMBER" name="CARDNUMBER" maxlength="16" size="17" style="font-family: Courier New, Verdana; font-size: 13px;" />
                                            </td>
                                            <td>
                                                &nbsp;&nbsp;&nbsp;
                                                <select id="CARDEXPMONTH" name="CARDEXPMONTH">
                                                    <option value="" selected="selected">MM</option>
                                                    <option value="01">01</option>
                                                    <option value="02">02</option>
                                                    <option value="03">03</option>
                                                    <option value="04">04</option>
                                                    <option value="05">05</option>
                                                    <option value="06">06</option>
                                                    <option value="07">07</option>
                                                    <option value="08">08</option>
                                                    <option value="09">09</option>
                                                    <option value="10">10</option>
                                                    <option value="11">11</option>
                                                    <option value="12">12</option>
                                                </select>
                                                /
                                                <select id="CARDEXPYEAR" name="CARDEXPYEAR">
                                                    <option value="" selected="selected">YYYY</option>                                                    
                                                    <option value="14">2014</option>
                                                    <option value="15">2015</option>
                                                    <option value="16">2016</option>
                                                    <option value="17">2017</option>                                                                                                                       
                                                    <option value="18">2018</option>
                                                    <option value="19">2019</option>
                                                    <option value="20">2020</option>
                                                    <option value="21">2021</option>
                                                    <option value="22">2022</option>
                                                    <option value="23">2023</option>
                                                    <option value="24">2024</option>
                                                    <option value="25">2025</option>
                                                    <option value="26">2026</option>
                                                    <option value="27">2027</option>
                                                    <option value="28">2028</option>
                                                    <option value="29">2029</option>
                                                    <option value="30">2030</option>
                                                    <option value="31">2031</option>
                                                    <option value="32">2032</option>
                                                    <option value="33">2033</option>
                                                    <option value="34">2034</option>
                                                    <option value="35">2035</option>
                                                    <option value="36">2036</option>
                                                    <option value="37">2037</option>
                                                    <option value="38">2038</option>
                                                    <option value="39">2039</option>
                                                    <option value="40">2040</option>
                                                    <option value="41">2041</option>
                                                    <option value="42">2042</option>
                                                    <option value="43">2043</option>
                                                    <option value="44">2044</option>
                                                    <option value="45">2045</option>
                                                </select>
                                            </td>
                                            <td>
                                                &nbsp;&nbsp;&nbsp;&nbsp;<input type="text" id="CARDCVV2" name="CARDCVV2" maxlength="4" size="4" />
                                            </td>
                                            
                                        </tr>
                                    </table> 
                                </td>
                            </tr>
                        </table>
                            
                        <table cellspacing="0" cellpadding="1" border="0">
                            <tr>
                                <td>Billing Country:</td>
                            </tr>
                            <tr>
                                <td>
                                    <select id="CARDCOUNTRY" name="CARDCOUNTRY" style="width: 240px;" onchange="UpdateCountry();">
                                        <option value="US"<%=(m_CardCountry=="" || m_CardCountry=="US" ? " selected=\"selected\"":"") %>>United States of America</option>
                                        <option value="US">------------------------</option>
                                        <option value="AF"<%=(m_CardCountry=="AF" ? " selected=\"selected\"" : "") %>>Afghanistan</option>
                                        <option value="AL"<%=(m_CardCountry=="AL" ? " selected=\"selected\"" : "") %>>Albania</option>
                                        <option value="DZ"<%=(m_CardCountry=="DZ" ? " selected=\"selected\"" : "") %>>Algeria</option>
                                        <option value="AD"<%=(m_CardCountry=="AD" ? " selected=\"selected\"" : "") %>>Andorra</option>
                                        <option value="AO"<%=(m_CardCountry=="AO" ? " selected=\"selected\"" : "") %>>Angola</option>
                                        <option value="AI"<%=(m_CardCountry=="AI" ? " selected=\"selected\"" : "") %>>Anguilla</option>
                                        <option value="AG"<%=(m_CardCountry=="AG" ? " selected=\"selected\"" : "") %>>Antigua and Barbuda</option>
                                        <option value="AR"<%=(m_CardCountry=="AR" ? " selected=\"selected\"" : "") %>>Argentina</option>
                                        <option value="AM"<%=(m_CardCountry=="AM" ? " selected=\"selected\"" : "") %>>Armenia</option>
                                        <option value="AW"<%=(m_CardCountry=="AW" ? " selected=\"selected\"" : "") %>>Aruba</option>
                                        <option value="AU"<%=(m_CardCountry=="AU" ? " selected=\"selected\"" : "") %>>Australia</option>
                                        <option value="AT"<%=(m_CardCountry=="AT" ? " selected=\"selected\"" : "") %>>Austria</option>
                                        <option value="AZ"<%=(m_CardCountry=="AZ" ? " selected=\"selected\"" : "") %>>Azerbaijan</option>
                                        <option value="BS"<%=(m_CardCountry=="BS" ? " selected=\"selected\"" : "") %>>Bahamas</option>
                                        <option value="BH"<%=(m_CardCountry=="BH" ? " selected=\"selected\"" : "") %>>Bahrain</option>
                                        <option value="BD"<%=(m_CardCountry=="BD" ? " selected=\"selected\"" : "") %>>Bangladesh</option>
                                        <option value="BB"<%=(m_CardCountry=="BB" ? " selected=\"selected\"" : "") %>>Barbados</option>
                                        <option value="BY"<%=(m_CardCountry=="BY" ? " selected=\"selected\"" : "") %>>Belarus</option>
                                        <option value="BE"<%=(m_CardCountry=="BE" ? " selected=\"selected\"" : "") %>>Belgium</option>
                                        <option value="BZ"<%=(m_CardCountry=="BZ" ? " selected=\"selected\"" : "") %>>Belize</option>
                                        <option value="BJ"<%=(m_CardCountry=="BJ" ? " selected=\"selected\"" : "") %>>Benin</option>
                                        <option value="BM"<%=(m_CardCountry=="BM" ? " selected=\"selected\"" : "") %>>Bermuda</option>
                                        <option value="BT"<%=(m_CardCountry=="BT" ? " selected=\"selected\"" : "") %>>Bhutan</option>
                                        <option value="BO"<%=(m_CardCountry=="BO" ? " selected=\"selected\"" : "") %>>Bolivia</option>
                                        <option value="BA"<%=(m_CardCountry=="BA" ? " selected=\"selected\"" : "") %>>Bosnia-Herzegovina</option>
                                        <option value="BW"<%=(m_CardCountry=="BW" ? " selected=\"selected\"" : "") %>>Botswana</option>
                                        <option value="BR"<%=(m_CardCountry=="BR" ? " selected=\"selected\"" : "") %>>Brazil</option>
                                        <option value="IO"<%=(m_CardCountry=="IO" ? " selected=\"selected\"" : "") %>>British Virgin Islands</option>
                                        <option value="BN"<%=(m_CardCountry=="BN" ? " selected=\"selected\"" : "") %>>Brunei Darussalam</option>
                                        <option value="BG"<%=(m_CardCountry=="BG" ? " selected=\"selected\"" : "") %>>Bulgaria</option>
                                        <option value="BF"<%=(m_CardCountry=="BF" ? " selected=\"selected\"" : "") %>>Burma</option>
                                        <option value="BI"<%=(m_CardCountry=="BI" ? " selected=\"selected\"" : "") %>>Burundi</option>
                                        <option value="KH"<%=(m_CardCountry=="KH" ? " selected=\"selected\"" : "") %>>Cambodia</option>
                                        <option value="CM"<%=(m_CardCountry=="CM" ? " selected=\"selected\"" : "") %>>Cameroon</option>
                                        <option value="CA"<%=(m_CardCountry=="CA" ? " selected=\"selected\"" : "") %>>Canada</option>
                                        <option value="CV"<%=(m_CardCountry=="CV" ? " selected=\"selected\"" : "") %>>Cape Verde</option>
                                        <option value="KY"<%=(m_CardCountry=="KY" ? " selected=\"selected\"" : "") %>>Cayman Islands</option>
                                        <option value="CF"<%=(m_CardCountry=="CF" ? " selected=\"selected\"" : "") %>>Central African Republic</option>
                                        <option value="TD"<%=(m_CardCountry=="TD" ? " selected=\"selected\"" : "") %>>Chad</option>
                                        <option value="CL"<%=(m_CardCountry=="CL" ? " selected=\"selected\"" : "") %>>Chile</option>
                                        <option value="CN"<%=(m_CardCountry=="CN" ? " selected=\"selected\"" : "") %>>China</option>
                                        <option value="CO"<%=(m_CardCountry=="CO" ? " selected=\"selected\"" : "") %>>Colombia</option>
                                        <option value="KM"<%=(m_CardCountry=="KM" ? " selected=\"selected\"" : "") %>>Comoros</option>
                                        <option value="CD"<%=(m_CardCountry=="CD" ? " selected=\"selected\"" : "") %>>Congo, Democratic Republic of the</option>
                                        <option value="CG"<%=(m_CardCountry=="CG" ? " selected=\"selected\"" : "") %>>Congo, Republic of the</option>
                                        <option value="CR"<%=(m_CardCountry=="CR" ? " selected=\"selected\"" : "") %>>Costa Rica</option>
                                        <option value="CI"<%=(m_CardCountry=="CI" ? " selected=\"selected\"" : "") %>>Cote d’Ivoire</option>
                                        <option value="HR"<%=(m_CardCountry=="HR" ? " selected=\"selected\"" : "") %>>Croatia</option>
                                        <option value="CU"<%=(m_CardCountry=="CU" ? " selected=\"selected\"" : "") %>>Cuba</option>
                                        <option value="CY"<%=(m_CardCountry=="CY" ? " selected=\"selected\"" : "") %>>Cyprus</option>
                                        <option value="CZ"<%=(m_CardCountry=="CZ" ? " selected=\"selected\"" : "") %>>Czech Republic</option>
                                        <option value="DK"<%=(m_CardCountry=="DK" ? " selected=\"selected\"" : "") %>>Denmark</option>
                                        <option value="DJ"<%=(m_CardCountry=="DJ" ? " selected=\"selected\"" : "") %>>Djibouti</option>
                                        <option value="DM"<%=(m_CardCountry=="DM" ? " selected=\"selected\"" : "") %>>Dominica</option>
                                        <option value="DO"<%=(m_CardCountry=="DO" ? " selected=\"selected\"" : "") %>>Dominican Republic</option>
                                        <option value="EC"<%=(m_CardCountry=="EC" ? " selected=\"selected\"" : "") %>>Ecuador</option>
                                        <option value="EG"<%=(m_CardCountry=="EG" ? " selected=\"selected\"" : "") %>>Egypt</option>
                                        <option value="SV"<%=(m_CardCountry=="SV" ? " selected=\"selected\"" : "") %>>El Salvador</option>
                                        <option value="GQ"<%=(m_CardCountry=="GQ" ? " selected=\"selected\"" : "") %>>Equatorial Guinea</option>
                                        <option value="ER"<%=(m_CardCountry=="ER" ? " selected=\"selected\"" : "") %>>Eritrea</option>
                                        <option value="EE"<%=(m_CardCountry=="EE" ? " selected=\"selected\"" : "") %>>Estonia</option>
                                        <option value="ET"<%=(m_CardCountry=="ET" ? " selected=\"selected\"" : "") %>>Ethiopia</option>
                                        <option value="FO"<%=(m_CardCountry=="FO" ? " selected=\"selected\"" : "") %>>Faroe Islands</option>
                                        <option value="FJ"<%=(m_CardCountry=="FJ" ? " selected=\"selected\"" : "") %>>Fiji</option>
                                        <option value="FI"<%=(m_CardCountry=="FI" ? " selected=\"selected\"" : "") %>>Finland</option>
                                        <option value="FR"<%=(m_CardCountry=="FR" ? " selected=\"selected\"" : "") %>>France</option>
                                        <option value="GF"<%=(m_CardCountry=="GF" ? " selected=\"selected\"" : "") %>>French Guiana</option>
                                        <option value="PF"<%=(m_CardCountry=="PF" ? " selected=\"selected\"" : "") %>>French Polynesia</option>
                                        <option value="GA"<%=(m_CardCountry=="GA" ? " selected=\"selected\"" : "") %>>Gabon</option>
                                        <option value="GM"<%=(m_CardCountry=="GM" ? " selected=\"selected\"" : "") %>>Gambia</option>
                                        <option value="GE"<%=(m_CardCountry=="GE" ? " selected=\"selected\"" : "") %>>Georgia, Republic of</option>
                                        <option value="DE"<%=(m_CardCountry=="DE" ? " selected=\"selected\"" : "") %>>Germany</option>
                                        <option value="GH"<%=(m_CardCountry=="GH" ? " selected=\"selected\"" : "") %>>Ghana</option>
                                        <option value="GI"<%=(m_CardCountry=="GI" ? " selected=\"selected\"" : "") %>>Gibraltar</option>
                                        <option value="GB"<%=(m_CardCountry=="GB" ? " selected=\"selected\"" : "") %>>Great Britain and Northern Ireland</option>
                                        <option value="GR"<%=(m_CardCountry=="GR" ? " selected=\"selected\"" : "") %>>Greece</option>
                                        <option value="GL"<%=(m_CardCountry=="GL" ? " selected=\"selected\"" : "") %>>Greenland</option>
                                        <option value="GD"<%=(m_CardCountry=="GD" ? " selected=\"selected\"" : "") %>>Grenada</option>
                                        <option value="GP"<%=(m_CardCountry=="GP" ? " selected=\"selected\"" : "") %>>Guadeloupe</option>
                                        <option value="GT"<%=(m_CardCountry=="GT" ? " selected=\"selected\"" : "") %>>Guatemala</option>
                                        <option value="GN"<%=(m_CardCountry=="GN" ? " selected=\"selected\"" : "") %>>Guinea</option>
                                        <option value="GW"<%=(m_CardCountry=="GW" ? " selected=\"selected\"" : "") %>>Guinea–Bissau</option>
                                        <option value="GY"<%=(m_CardCountry=="GY" ? " selected=\"selected\"" : "") %>>Guyana</option>
                                        <option value="HT"<%=(m_CardCountry=="HT" ? " selected=\"selected\"" : "") %>>Haiti</option>
                                        <option value="HN"<%=(m_CardCountry=="HN" ? " selected=\"selected\"" : "") %>>Honduras</option>
                                        <option value="HK"<%=(m_CardCountry=="HK" ? " selected=\"selected\"" : "") %>>Hong Kong</option>
                                        <option value="HU"<%=(m_CardCountry=="HU" ? " selected=\"selected\"" : "") %>>Hungary</option>
                                        <option value="IS"<%=(m_CardCountry=="IS" ? " selected=\"selected\"" : "") %>>Iceland</option>
                                        <option value="IN"<%=(m_CardCountry=="IN" ? " selected=\"selected\"" : "") %>>India</option>
                                        <option value="ID"<%=(m_CardCountry=="ID" ? " selected=\"selected\"" : "") %>>Indonesia</option>
                                        <option value="IR"<%=(m_CardCountry=="IR" ? " selected=\"selected\"" : "") %>>Iran</option>
                                        <option value="IQ"<%=(m_CardCountry=="IQ" ? " selected=\"selected\"" : "") %>>Iraq</option>
                                        <option value="IE"<%=(m_CardCountry=="IE" ? " selected=\"selected\"" : "") %>>Ireland</option>
                                        <option value="IL"<%=(m_CardCountry=="IL" ? " selected=\"selected\"" : "") %>>Israel</option>
                                        <option value="IT"<%=(m_CardCountry=="IT" ? " selected=\"selected\"" : "") %>>Italy</option>
                                        <option value="JM"<%=(m_CardCountry=="JM" ? " selected=\"selected\"" : "") %>>Jamaica</option>
                                        <option value="JP"<%=(m_CardCountry=="JP" ? " selected=\"selected\"" : "") %>>Japan</option>
                                        <option value="JO"<%=(m_CardCountry=="JO" ? " selected=\"selected\"" : "") %>>Jordan</option>
                                        <option value="KZ"<%=(m_CardCountry=="KZ" ? " selected=\"selected\"" : "") %>>Kazakhstan</option>
                                        <option value="KE"<%=(m_CardCountry=="KE" ? " selected=\"selected\"" : "") %>>Kenya</option>
                                        <option value="KI"<%=(m_CardCountry=="KI" ? " selected=\"selected\"" : "") %>>Kiribati</option>
                                        <option value="KP"<%=(m_CardCountry=="KP" ? " selected=\"selected\"" : "") %>>Korea, Democratic People’s Republic</option>
                                        <option value="KR"<%=(m_CardCountry=="KR" ? " selected=\"selected\"" : "") %>>Korea, Republic of (South Korea)</option>
                                        <option value="KW"<%=(m_CardCountry=="KW" ? " selected=\"selected\"" : "") %>>Kuwait</option>
                                        <option value="KG"<%=(m_CardCountry=="KG" ? " selected=\"selected\"" : "") %>>Kyrgyzstan</option>
                                        <option value="LA"<%=(m_CardCountry=="LA" ? " selected=\"selected\"" : "") %>>Laos</option>
                                        <option value="LV"<%=(m_CardCountry=="LV" ? " selected=\"selected\"" : "") %>>Latvia</option>
                                        <option value="LB"<%=(m_CardCountry=="LB" ? " selected=\"selected\"" : "") %>>Lebanon</option>
                                        <option value="LS"<%=(m_CardCountry=="LS" ? " selected=\"selected\"" : "") %>>Lesotho</option>
                                        <option value="LR"<%=(m_CardCountry=="LR" ? " selected=\"selected\"" : "") %>>Liberia</option>
                                        <option value="LY"<%=(m_CardCountry=="LY" ? " selected=\"selected\"" : "") %>>Libya</option>
                                        <option value="LI"<%=(m_CardCountry=="LI" ? " selected=\"selected\"" : "") %>>Liechtenstein</option>
                                        <option value="LT"<%=(m_CardCountry=="LT" ? " selected=\"selected\"" : "") %>>Lithuania</option>
                                        <option value="LU"<%=(m_CardCountry=="LU" ? " selected=\"selected\"" : "") %>>Luxembourg</option>
                                        <option value="MO"<%=(m_CardCountry=="MO" ? " selected=\"selected\"" : "") %>>Macao</option>
                                        <option value="MK"<%=(m_CardCountry=="MK" ? " selected=\"selected\"" : "") %>>Macedonia, Republic of</option>
                                        <option value="MG"<%=(m_CardCountry=="MG" ? " selected=\"selected\"" : "") %>>Madagascar</option>
                                        <option value="MW"<%=(m_CardCountry=="MW" ? " selected=\"selected\"" : "") %>>Malawi</option>
                                        <option value="MY"<%=(m_CardCountry=="MY" ? " selected=\"selected\"" : "") %>>Malaysia</option>
                                        <option value="MV"<%=(m_CardCountry=="MV" ? " selected=\"selected\"" : "") %>>Maldives</option>
                                        <option value="ML"<%=(m_CardCountry=="ML" ? " selected=\"selected\"" : "") %>>Mali</option>
                                        <option value="MT"<%=(m_CardCountry=="MT" ? " selected=\"selected\"" : "") %>>Malta</option>
                                        <option value="MQ"<%=(m_CardCountry=="MQ" ? " selected=\"selected\"" : "") %>>Martinique</option>
                                        <option value="MR"<%=(m_CardCountry=="MR" ? " selected=\"selected\"" : "") %>>Mauritania</option>
                                        <option value="MU"<%=(m_CardCountry=="MU" ? " selected=\"selected\"" : "") %>>Mauritius</option>
                                        <option value="MX"<%=(m_CardCountry=="MX" ? " selected=\"selected\"" : "") %>>Mexico</option>
                                        <option value="FM"<%=(m_CardCountry=="FM" ? " selected=\"selected\"" : "") %>>Moldova</option>
                                        <option value="MN"<%=(m_CardCountry=="MN" ? " selected=\"selected\"" : "") %>>Mongolia</option>
                                        <option value="ME"<%=(m_CardCountry=="ME" ? " selected=\"selected\"" : "") %>>Montenegro</option>
                                        <option value="MS"<%=(m_CardCountry=="MS" ? " selected=\"selected\"" : "") %>>Montserrat</option>
                                        <option value="MA"<%=(m_CardCountry=="MA" ? " selected=\"selected\"" : "") %>>Morocco</option>
                                        <option value="MZ"<%=(m_CardCountry=="MZ" ? " selected=\"selected\"" : "") %>>Mozambique</option>
                                        <option value="NA"<%=(m_CardCountry=="NA" ? " selected=\"selected\"" : "") %>>Namibia</option>
                                        <option value="NR"<%=(m_CardCountry=="NR" ? " selected=\"selected\"" : "") %>>Nauru</option>
                                        <option value="NP"<%=(m_CardCountry=="NP" ? " selected=\"selected\"" : "") %>>Nepal</option>
                                        <option value="NL"<%=(m_CardCountry=="NL" ? " selected=\"selected\"" : "") %>>Netherlands</option>
                                        <option value="AN"<%=(m_CardCountry=="AN" ? " selected=\"selected\"" : "") %>>Netherlands Antilles</option>
                                        <option value="NC"<%=(m_CardCountry=="NC" ? " selected=\"selected\"" : "") %>>New Caledonia</option>
                                        <option value="NZ"<%=(m_CardCountry=="NZ" ? " selected=\"selected\"" : "") %>>New Zealand</option>
                                        <option value="NI"<%=(m_CardCountry=="NI" ? " selected=\"selected\"" : "") %>>Nicaragua</option>
                                        <option value="NE"<%=(m_CardCountry=="NE" ? " selected=\"selected\"" : "") %>>Niger</option>
                                        <option value="NG"<%=(m_CardCountry=="NG" ? " selected=\"selected\"" : "") %>>Nigeria</option>                                    
                                        <option value="NO"<%=(m_CardCountry=="NO" ? " selected=\"selected\"" : "") %>>Norway</option>
                                        <option value="OM"<%=(m_CardCountry=="OM" ? " selected=\"selected\"" : "") %>>Oman</option>
                                        <option value="PK"<%=(m_CardCountry=="PK" ? " selected=\"selected\"" : "") %>>Pakistan</option>
                                        <option value="PA"<%=(m_CardCountry=="PA" ? " selected=\"selected\"" : "") %>>Panama</option>
                                        <option value="PG"<%=(m_CardCountry=="PG" ? " selected=\"selected\"" : "") %>>Papua New Guinea</option>
                                        <option value="PY"<%=(m_CardCountry=="PY" ? " selected=\"selected\"" : "") %>>Paraguay</option>
                                        <option value="PE"<%=(m_CardCountry=="PE" ? " selected=\"selected\"" : "") %>>Peru</option>
                                        <option value="PH"<%=(m_CardCountry=="PH" ? " selected=\"selected\"" : "") %>>Philippines</option>
                                        <option value="PN"<%=(m_CardCountry=="PN" ? " selected=\"selected\"" : "") %>>Pitcairn Island</option>
                                        <option value="PL"<%=(m_CardCountry=="PL" ? " selected=\"selected\"" : "") %>>Poland</option>
                                        <option value="PT"<%=(m_CardCountry=="PT" ? " selected=\"selected\"" : "") %>>Portugal</option>
                                        <option value="QA"<%=(m_CardCountry=="QA" ? " selected=\"selected\"" : "") %>>Qatar</option>
                                        <option value="RE"<%=(m_CardCountry=="RE" ? " selected=\"selected\"" : "") %>>Reunion</option>
                                        <option value="RO"<%=(m_CardCountry=="RO" ? " selected=\"selected\"" : "") %>>Romania</option>
                                        <option value="RU"<%=(m_CardCountry=="RU" ? " selected=\"selected\"" : "") %>>Russia</option>
                                        <option value="RW"<%=(m_CardCountry=="RW" ? " selected=\"selected\"" : "") %>>Rwanda</option>
                                        <option value="SH"<%=(m_CardCountry=="SH" ? " selected=\"selected\"" : "") %>>Saint Helena</option>
                                        <option value="LC"<%=(m_CardCountry=="LC" ? " selected=\"selected\"" : "") %>>Saint Lucia</option>
                                        <option value="PM"<%=(m_CardCountry=="PM" ? " selected=\"selected\"" : "") %>>Saint Pierre and Miquelon</option>
                                        <option value="VC"<%=(m_CardCountry=="VC" ? " selected=\"selected\"" : "") %>>Saint Vincent and the Grenadines</option>
                                        <option value="SM"<%=(m_CardCountry=="SM" ? " selected=\"selected\"" : "") %>>San Marino</option>
                                        <option value="ST"<%=(m_CardCountry=="ST" ? " selected=\"selected\"" : "") %>>Sao Tome and Principe</option>
                                        <option value="SA"<%=(m_CardCountry=="SA" ? " selected=\"selected\"" : "") %>>Saudi Arabia</option>
                                        <option value="SN"<%=(m_CardCountry=="SN" ? " selected=\"selected\"" : "") %>>Senegal</option>
                                        <option value="CS"<%=(m_CardCountry=="CS" ? " selected=\"selected\"" : "") %>>Serbia, Republic of</option>
                                        <option value="SC"<%=(m_CardCountry=="SC" ? " selected=\"selected\"" : "") %>>Seychelles</option>
                                        <option value="SL"<%=(m_CardCountry=="SL" ? " selected=\"selected\"" : "") %>>Sierra Leone</option>
                                        <option value="SG"<%=(m_CardCountry=="SG" ? " selected=\"selected\"" : "") %>>Singapore</option>
                                        <option value="SI"<%=(m_CardCountry=="SI" ? " selected=\"selected\"" : "") %>>Slovenia</option>
                                        <option value="SB"<%=(m_CardCountry=="SB" ? " selected=\"selected\"" : "") %>>Solomon Islands</option>
                                        <option value="SO"<%=(m_CardCountry=="SO" ? " selected=\"selected\"" : "") %>>Somalia</option>
                                        <option value="ZA"<%=(m_CardCountry=="ZA" ? " selected=\"selected\"" : "") %>>South Africa</option>
                                        <option value="ES"<%=(m_CardCountry=="ES" ? " selected=\"selected\"" : "") %>>Spain</option>
                                        <option value="LK"<%=(m_CardCountry=="LK" ? " selected=\"selected\"" : "") %>>Sri Lanka</option>
                                        <option value="SD"<%=(m_CardCountry=="SD" ? " selected=\"selected\"" : "") %>>Sudan</option>
                                        <option value="SR"<%=(m_CardCountry=="SR" ? " selected=\"selected\"" : "") %>>Suriname</option>
                                        <option value="SZ"<%=(m_CardCountry=="SZ" ? " selected=\"selected\"" : "") %>>Swaziland</option>
                                        <option value="SE"<%=(m_CardCountry=="SE" ? " selected=\"selected\"" : "") %>>Sweden</option>
                                        <option value="CH"<%=(m_CardCountry=="CH" ? " selected=\"selected\"" : "") %>>Switzerland</option>
                                        <option value="TW"<%=(m_CardCountry=="TW" ? " selected=\"selected\"" : "") %>>Taiwan</option>
                                        <option value="TJ"<%=(m_CardCountry=="TJ" ? " selected=\"selected\"" : "") %>>Tajikistan</option>
                                        <option value="TZ"<%=(m_CardCountry=="TZ" ? " selected=\"selected\"" : "") %>>Tanzania</option>
                                        <option value="TH"<%=(m_CardCountry=="TH" ? " selected=\"selected\"" : "") %>>Thailand</option>
                                        <option value="TG"<%=(m_CardCountry=="TG" ? " selected=\"selected\"" : "") %>>Togo</option>
                                        <option value="TO"<%=(m_CardCountry=="TO" ? " selected=\"selected\"" : "") %>>Tonga</option>
                                        <option value="TT"<%=(m_CardCountry=="TT" ? " selected=\"selected\"" : "") %>>Trinidad and Tobago</option>
                                        <option value="TN"<%=(m_CardCountry=="TN" ? " selected=\"selected\"" : "") %>>Tunisia</option>
                                        <option value="TR"<%=(m_CardCountry=="TR" ? " selected=\"selected\"" : "") %>>Turkey</option>
                                        <option value="TM"<%=(m_CardCountry=="TM" ? " selected=\"selected\"" : "") %>>Turkmenistan</option>
                                        <option value="TC"<%=(m_CardCountry=="TC" ? " selected=\"selected\"" : "") %>>Turks and Caicos Islands</option>
                                        <option value="TV"<%=(m_CardCountry=="TV" ? " selected=\"selected\"" : "") %>>Tuvalu</option>
                                        <option value="UG"<%=(m_CardCountry=="UG" ? " selected=\"selected\"" : "") %>>Uganda</option>
                                        <option value="UA"<%=(m_CardCountry=="UA" ? " selected=\"selected\"" : "") %>>Ukraine</option>
                                        <option value="AE"<%=(m_CardCountry=="AE" ? " selected=\"selected\"" : "") %>>United Arab Emirates</option>
                                        <option value="UY"<%=(m_CardCountry=="UY" ? " selected=\"selected\"" : "") %>>Uruguay</option>
                                        <option value="UZ"<%=(m_CardCountry=="UZ" ? " selected=\"selected\"" : "") %>>Uzbekistan</option>
                                        <option value="VU"<%=(m_CardCountry=="VU" ? " selected=\"selected\"" : "") %>>Vanuatu</option>
                                        <option value="VE"<%=(m_CardCountry=="VE" ? " selected=\"selected\"" : "") %>>Venezuela</option>
                                        <option value="VN"<%=(m_CardCountry=="VN" ? " selected=\"selected\"" : "") %>>Vietnam</option>
                                        <option value="WF"<%=(m_CardCountry=="WF" ? " selected=\"selected\"" : "") %>>Wallis and Futuna Islands</option>
                                        <option value="EH"<%=(m_CardCountry=="EH" ? " selected=\"selected\"" : "") %>>Western Samoa</option>
                                        <option value="YE"<%=(m_CardCountry=="YE" ? " selected=\"selected\"" : "") %>>Yemen</option>
                                        <option value="ZM"<%=(m_CardCountry=="ZM" ? " selected=\"selected\"" : "") %>>Zambia</option>
                                        <option value="ZW"<%=(m_CardCountry=="ZW" ? " selected=\"selected\"" : "") %>>Zimbabwe</option>
                                    </select>
                                </td>
                            </tr>
                        </table>
                        
                                
                        <table cellspacing="0" cellpadding="1" border="0">
                            <tr>
                                <td>                                  
                                    <table cellspacing="0" cellpadding="1" border="0">
                                        <tr>
                                            <td>Billing Address:</td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <input name="CARDADDRESS" id="CARDADDRESS" value="" maxlength="30" type="text" style="width:300px;" /><br />
                                                <input name="CARDADDRESS2" id="CARDADDRESS2" value="" maxlength="30" type="text" style="width:300px;" />                                                        
                                            </td>
                                        </tr>
                                    </table>
                                    <table cellspacing="0" cellpadding="1" border="0">
                                        <tr>
                                            <td>City:</td>
                                            <td>&nbsp;&nbsp;State (Province):</td>
                                            <td>&nbsp;&nbsp;ZIP (Pol
                                        <tr>
                                            <td>
                                                <input name="CARDCITY" id="CARDCITY" type="text" value="" maxlength="30" style="width:160px;" />
                                            </td>
                                            <td>                                
                                                <select id="CARDSTATE_US" name="CARDSTATE_US" style="width: 200px; display: block;">
                                                    <option value="" selected="selected"></option>
                                                    <option value="AL">AL-Alabama</option>
                                                    <option value="AK">AK-Alaska</option>
                                                    <option value="AZ">AZ-Arizona</option>
                                                    <option value="AR">AR-Arkansas</option>
                                                    <option value="CA">CA-California</option>
                                                    <option value="CO">CO-Colorado</option>
                                                    <option value="CT">CT-Connecticut</option>
                                                    <option value="DE">DE-Delaware</option>
                                                    <option value="DC">DC-District of Columbia</option>
                                                    <option value="FL">FL-Florida</option>
                                                    <option value="GA">GA-Georgia</option>
                                                    <option value="HI">HI-Hawaii</option>
                                                    <option value="ID">ID-Idaho</option>
                                                    <option value="IL">IL-Illinois</option>
                                                    <option value="IN">IN-Indiana</option>
                                                    <option value="IA">IA-Iowa</option>
                                                    <option value="KS">KS-Kansas</option>
                                                    <option value="KY">KY-Kentucky</option>
                                                    <option value="LA">LA-Louisiana</option>
                                                    <option value="ME">ME-Maine</option>
                                                    <option value="MD">MD-Maryland</option>
                                                    <option value="MA">MA-Massachusetts</option>
                                                    <option value="MI">MI-Michigan</option>
                                                    <option value="MN">MN-Minnesota</option>
                                                    <option value="MS">MS-Mississippi</option>
                                                    <option value="MO">MO-Missouri</option>
                                                    <option value="MT">MT-Montana</option>
                                                    <option value="NE">NE-Nebraska</option>
                                                    <option value="NV">NV-Nevada</option>
                                                    <option value="NH">NH-New Hampshire</option>
                                                    <option value="NJ">NJ-New Jersey</option>
                                                    <option value="NM">NM-New Mexico</option>
                                                    <option value="NY">NY-New York</option>
                                                    <option value="NC">NC-North Carolina</option>
                                                    <option value="ND">ND-North Dakota</option>
                                                    <option value="OH">OH-Ohio</option>
                                                    <option value="OK">OK-Oklahoma</option>
                                                    <option value="OR">OR-Oregon</option>
                                                    <option value="PA">PA-Pennsylvania</option>                                                                
                                                    <option value="RI">RI-Rhode Island</option>
                                                    <option value="SC">SC-South Carolina</option>
                                                    <option value="SD">SD-South Dakota</option>
                                                    <option value="TN">TN-Tennessee</option>
                                                    <option value="TX">TX-Texas</option>
                                                    <option value="UT">UT-Utah</option>
                                                    <option value="VT">VT-Vermont</option>
                                                    <option value="VA">VA-Virginia</option>
                                                    <option value="WA">WA-Washington</option>
                                                    <option value="WV">WV-West Virginia</option>
                                                    <option value="WI">WI-Wisconsin</option>
                                                    <option value="WY">WY-Wyoming</option>                                                                
                                                    <option value="">----------------------------------</option>
                                                    <option value="PR">PR-Puerto Rico</option>
                                                    <option value="AS">AS-American Samoa</option>
                                                    <option value="FM">FM-Federated States of Micronesia</option>
                                                    <option value="GU">GU-Guam</option>
                                                    <option value="MH">MH-Marshall Islands</option>
                                                    <option value="MP">MP-Northern Mariana Islands</option>
                                                    <option value="PW">PW-Palau</option>
                                                    <option value="VI">VI-US Virgin Islands</option>                                                                
                                                    <option value="">----------------------------------</option>
                                                    <option value="AA">AA-Armed Forces Americas</option>
                                                    <option value="AP">AP-Armed Forces Pacific</option>
                                                    <option value="AE">AE-Armed Forces (Other)</option>                                                               
                                                </select>
                                                <select id="CARDSTATE_CA" name="CARDSTATE_CA" style="width: 200px; display: none;">
                                                    <option value="" selected="selected"></option>
                                                    <option value="AB">AB-Alberta</option>
                                                    <option value="BC">BC-British Columbia</option>
                                                    <option value="MB">MB-Manitoba</option>
                                                    <option value="NB">NB-New Brunswick</option>
                                                    <option value="NL">NL-Newfound. and Labrador</option>
                                                    <option value="NS">NS-Nova Scotia</option>
                                                    <option value="NT">NT-Northwest Territories</option>
                                                    <option value="NU">NU-Nunavut</option>
                                                    <option value="ON">ON-Ontario</option>
                                                    <option value="PE">PE-Prince Edward Island</option>
                                                    <option value="QC">QC-Quebec</option>
                                                    <option value="SK">SK-Saskatchewan</option>
                                                    <option value="YT">YT-Yukon</option>
                                                </select>                                                                                                            
                                            </td>
                                            <td>
                                                &nbsp;<input name="CARDZIP" id="CARDZIP" type="text" value="" style="width:105px;" maxlength="10" />
                                            </td>
                                        </tr>
                                    </table>
                                    <table cellspacing="0" cellpadding="1" border="0">
                                        <tr>
                                            <td>Phone associated with the card:</td>
                                        </tr>
                                        <tr>
                                            <td><input name="CARDPHONE" id="CARDPHONE" type="text" value="" maxlength="20" style="width:200px;" /></td>
                                        </tr>
									</table>
                                </td>
                            </tr>
                        </table>                                                    
                    </div>
                    
                    
                    <div id="DIV_PP" style="display: none;">
                        
                    <div id="DIV_PP" style="color: #DD0000;">
                        You will be directed to the PayPal page after completing this form.<br />
                        <b>In the PayPal page, you can also choose to pay with a credit card if desired.</b><br />
                        <br />
                        After we process your order, you will receive an e-mail from <%=ConfigurationManager.AppSettings["EmailOrders"] %>, providing details<br />
                        on the parts you ordered. Please accept this e-mail (do not block it).<br /><br />
                    </div>
        
                </td>
            </tr>
            <tr>
                <td align="left">&nbsp;</td>
            </tr>
            <tr>
                <td align="left" class="Heading">
                    Additional Information:
                </td>
            </tr>
            <tr>
                <td align="left" class="Content">                                                   
                    <table cellspacing="0" cellpadding="2" border="0">                                       
						<tr>
							<td>Vehicle(s) information: (Make Model Year, last 7 digits of VIN)</td>                                                        
						</tr>
						<tr>										                
                            <td><input name="VEHICLEINFO" id="VEHICLEINFO" value="<%=m_VehicleInfo %>" type="text" style="width:300px;" maxlength="250" /><span class="RequiredField">*</span></td>                                                        
						</tr>
						<tr>
							<td style="font-size: 9px; color: #666;">
								i.e.: BMW K75S 1991  VIN W453695  (you can use commas to separate multiple vehicles)
							</td>
						</tr>
					</table>
					<br />										        
					<br />
					<table cellspacing="0" cellpadding="2" border="0">										
                        <tr>
                            <td>Questions or Comments?</td>
                        </tr>										
                        <tr>
                            <td><textarea name="COMMENTS" id="COMMENTS" cols="78" rows="3"></textarea></td>
                        </tr>										
					</table>                                              
                </td>
            </tr>
            <tr>
                <td align="left">&nbsp;</td>
            </tr>
            <tr>
                <td align="center">
                    <span class="Button" id="CONTINUE">&nbsp;&nbsp;&nbsp;Continue with Checkout&nbsp;&nbsp;&nbsp;</span>
                    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                    <span class="Button" onclick="window.location='<%=Utilities.GetLastCheckoutURL() %>';">&nbsp;&nbsp;&nbsp;Cancel&nbsp;&nbsp;&nbsp;</span><br />
                    <br />                                               
                </td>
            </tr>                                                       
        </table>                                    
    </form>
    <br />
    <div id="ClearFooter"></div> 
</div>

<uc:UC_Footer runat="server" /> 

</body>
</html>
