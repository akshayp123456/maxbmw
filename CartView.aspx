<%@ Page Language="C#" AutoEventWireup="true" CodeFile="CartView.aspx.cs" Inherits="CartView" %>
<%@ Register TagPrefix="uc" TagName="UC_MainMenu" Src="~/UC_MainMenu.ascx" %>
<%@ Register TagPrefix="uc" TagName="UC_Footer" Src="~/UC_Footer.ascx" %>

<!DOCTYPE html>
<html>
<head>
    <title>MAX BMW Motorcycles - Shopping Cart & Wish Lists</title>
    <meta http-equiv="Pragma" content="no-cache" />
    <meta http-equiv="CACHE-CONTROL" content="NO-CACHE" />
    <meta http-equiv="Expires" content="0" />
    <meta name="rating" content="general" />
    <meta name="author" content="Max BMW Motorcycles" /> 
    <meta name="apple-mobile-web-app-capable" content="yes" />
    <meta name="apple-mobile-web-app-status-bar-style" content="black" />   
    <meta name="format-detection" content="telephone=no" />

    <script type="text/javascript" src="js/ie6no.js" charset="utf-8"></script>
    <script type="text/javascript">
        ie6no({ runonload: true });
    </script>

    <script src="js/jquery-1.7.1.min.js"></script>
    <script type="text/javascript" src="js/FormValidations.js"></script>

    <link href="css/Global.css" rel="stylesheet" type="text/css" media="all" />
    <link href="css/MainMenu.css" rel="stylesheet" type="text/css" media="all" />
    <link href="css/Forms.css" rel="stylesheet" type="text/css" media="all" />
           
    <style type="text/css">
        div.DIVModal
        {
            display: none;
            background-color: #fff;
            border: 4px solid #333;
            text-align: center;
            vertical-align: middle;
            width: 340px;
            height: 150px;
            top: 20%;
        }
    </style>

    <!-- Shopping Cart functions -->
    <script type="text/javascript">
        function NumToCurrency(num) {
            num = num.toString().replace(/\$|\,/g, '');
            if (isNaN(num))
                num = "0";
            sign = (num == (num = Math.abs(num)));
            num = Math.floor(num * 100 + 0.50000000001);
            cents = num % 100;
            num = Math.floor(num / 100).toString();
            if (cents < 10)
                cents = "0" + cents;

            for (var i = 0; i < Math.floor((num.length - (1 + i)) / 3); i++)
                num = num.substring(0, num.length - (4 * i + 3)) + ',' + num.substring(num.length - (4 * i + 3));

            return (((sign) ? '' : '-') + '$' + num + '.' + cents);
        }

        function CurrencyToNum(num) {
            var x;
            num = num.replace(/\$|\,/g, '');
            if (isNaN(num))
                num = "0";
            sign = (num == (num = Math.abs(num)));
            num = Math.floor(num * 100 + 0.50000000001);
            cents = num % 100;
            if (cents != 0) cents = cents / 100;
            num = Math.floor(num / 100);
            return (num + cents);
        }

        var g_IsDirty = false;

        function UpdateFields() {
            var SubTotal = 0.0;
            var AdditionalShipping = 0.0;
            var TotalWeight = 0.0;
            var id = "";
            var INPUTField = document.getElementsByTagName("INPUT");
            for (i = 0; i < INPUTField.length; i++) {
                if (INPUTField[i].id.indexOf("TP_") == 0) {
                    SubTotal += CurrencyToNum(INPUTField[i].value);
                } else if (INPUTField[i].id.indexOf("AS_") == 0) {
                    id = INPUTField[i].id.substring(3, INPUTField[i].id.length);
                    AdditionalShipping += CurrencyToNum(INPUTField[i].value) * document.getElementById("Q_" + id).value;
                } else if (INPUTField[i].id.indexOf("W_") == 0) {
                    id = INPUTField[i].id.substring(2, INPUTField[i].id.length);
                    if (!isNaN(INPUTField[i].value)) {
                        TotalWeight += INPUTField[i].value * document.getElementById("Q_" + id).value;
                    }
                }
            }
            document.getElementById('AdditionalShipping').value = NumToCurrency(AdditionalShipping);
            document.getElementById('SubTotal').value = NumToCurrency(SubTotal);
            document.getElementById('TotalWeight').value = Math.round(TotalWeight * 100) / 100;

            if (AdditionalShipping > 0.0)
                document.getElementById('TR_ADDITIONALSHIPPING').style.display = "block";
            else
                document.getElementById('TR_ADDITIONALSHIPPING').style.display = "none";


            if (g_IsDirty) {
                if (document.getElementById('BTNCHECKOUT') != null) {
                    document.getElementById('BTNCHECKOUT').style.color = "#777";
                    document.getElementById('BTNCHECKOUT').style.borderColor = "#ddd";
                }

                if (document.getElementById('BTNSAVE') != null) {
                    document.getElementById('BTNSAVE').style.color = "#777";
                    document.getElementById('BTNSAVE').style.borderColor = "#ddd";
                }

                if (document.getElementById('BTNCOPYTOWISHLIST') != null) {
                    document.getElementById('BTNCOPYTOWISHLIST').style.color = "#777";
                    document.getElementById('BTNCOPYTOWISHLIST').style.borderColor = "#ddd";
                }

                if (document.getElementById('BTNCOPYTOCART') != null) {
                    document.getElementById('BTNCOPYTOCART').style.color = "#777";
                    document.getElementById('BTNCOPYTOCART').style.borderColor = "#ddd";
                }

                if (document.getElementById('BTNRENAME') != null) {
                    document.getElementById('BTNRENAME').style.color = "#777";
                    document.getElementById('BTNRENAME').style.borderColor = "#ddd";
                }
            }                        
        }

        function RenameWishListPrompt() {
            if (g_IsDirty) return;          
            document.getElementById('INPUT_RENAMEWISHLIST').value = document.getElementById('WISHLISTNAME').value;
            ShowModal('DIV_RENAMEWISHLIST');
            document.getElementById('INPUT_RENAMEWISHLIST').focus();
        }

        function RenameWishList() {
            var NewName = document.getElementById('INPUT_RENAMEWISHLIST').value.replace(/^\s+|\s+$/g, "");
            
            if (NewName == "")
                alert("The name cannot be blank");
            else if (NewName.length > 25)
                alert("The name is too long. A maximum of 25 characters is allowed.");
            else {
                document.getElementById('WISHLISTNAME').value = NewName;
                document.getElementById('CMD').value = "RenameWishList";
                document.getElementById('FORM1').submit();
            }
        }

        function SaveCartAsWishListPrompt() {
            if (g_IsDirty) return;
            document.getElementById('INPUT_SAVECARTASWISHLIST').value = '';
            ShowModal('DIV_SAVECARTASWISHLIST');
            document.getElementById('INPUT_SAVECARTASWISHLIST').focus();
        }

        function SaveCartAsWishList() {
            if (g_IsDirty) return;

            var NewName = document.getElementById('INPUT_SAVECARTASWISHLIST').value.replace(/^\s+|\s+$/g, "");
            if (NewName == "")
                alert("The name cannot be blank");
            else if (NewName.length > 25)
                alert("The name is too long. A maximum of 25 characters is allowed.");
            else {
                document.getElementById('WISHLISTNAME').value = NewName;
                document.getElementById('CMD').value = 'SaveCartAsWishList';
                document.getElementById('FORM1').submit();
            }
        }

        function CopyCartToWishList() {
            if (g_IsDirty) return;

            document.getElementById('CMD').value = 'CopyCartToWishList';
            document.getElementById('FORM1').submit();
        }

        function CopyWishListToCart() {
            if (g_IsDirty) return;

            document.getElementById('CMD').value = 'CopyWishListToCart';
            document.getElementById('FORM1').submit();
        }

        function ClearCart() {
            var conf = confirm("Are you sure you want to clear all your Shopping Cart items?");
            if (conf) {                
                document.getElementById('CMD').value = 'ClearCart';
                document.getElementById('FORM1').submit();
            }
        }

        function UpdateAndSave() {
            document.getElementById('CMD').value = 'UpdateAndSave';
            document.getElementById('FORM1').submit();
        }

        function DeleteWishList() {
            var conf = confirm("Are you sure you want to delete this Wish List?");
            if (conf) {
                document.getElementById('CMD').value = 'DeleteWishList';
                document.getElementById('FORM1').submit();
            }
        }
                
        function Checkout() {
            if (g_IsDirty) return;

            window.location = "<%=Utilities.URL_FICHE(true)%>/Checkout.aspx";
        }

        function ShowModal(contentDIV) {
            $("<iframe id='modalShim' class='ModalDialog' src='about:blank'>").css({
                width: "100%",
                height: "100%",
                position: "fixed",
                left: "0px",
                top: "0px",
                zIndex: "100",
                backgroundColor: "#ddd",
                opacity: "0.8"
            }).appendTo(document.body);

            var w = $('#' + contentDIV);
            var wwidth = $('#' + contentDIV).width();

            w.css({ zIndex: "101", position: "fixed", left: ($(window).width() - wwidth) / 2 }).show();

            w.children('.ButtonClose').click(function () {
                w.hide();
                $(".ModalDialog").remove();
            });
        }       
    </script>
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
                    My Shopping Cart
                </td>
            </tr>
        </table>  

        <form action="CartView.aspx" method="post" id="FORM1" name="FORM1">
        <input type="hidden" id="SELECTEDLIST" name="SELECTEDLIST" value="<%=m_SelectedList %>" />
        <input type="hidden" id="WISHLISTNAME" name="WISHLISTNAME" value="<%=GetWishListName(m_SelectedList) %>" />
        <input type="hidden" id="CMD" name="CMD" />
        
        <table class="Panel RoundedBottom" cellpadding="0" cellspacing="0">
            <tr style="vertical-align: top; height: 200px;">                                              
                <td align="center" style="font-size: 14px; width: 480px;">
                    <%if (m_CartCount > 0) { %>
                        <table cellpadding="0" cellspacing="0" style="border: 0px">
                            <tr style="vertical-align: middle;">
                                <td>
                                    You have <%=m_CartCount%> item<%=(m_CartCount>1?"s":"")%> in your Shopping Cart&nbsp;
                                </td>
                                <td>
                                    <%if (m_SelectedList != 0) { %>
                                        <img src="images/view-list.gif" alt="View Cart" style="cursor: pointer;" onclick="document.getElementById('SELECTEDLIST').value=0; document.getElementById('FORM1').submit();" />
                                    <%} else { %>
                                        &nbsp;
                                    <%} %>      
                                </td>
                            </tr>
                        </table>                                                                               
                        <ul style="font-weight: bold; font-size: 8pt; color: #d00; text-align: left;">
                            <li>The "Each" column depicts the price for an individual part, kit, or set.<br />Fiche defaults to what is required on the vehicle in that particular diagram.<br />Adjust quantities as needed.</li>
                            <li>Overweight or oversized items may be susceptible to additional shipping charges.<br />If needed, additional payment will be taken at the time of shipment.<br />Additional shipping charges may also apply for the shipment of the balance of a previously split shipped order.</li>
                            <li>Our website does NOT reflect what we have in stock.<br />Some parts may be special ordered from BMWNA or sourced from BMWAG (Germany).<br />Special orders take 3-4 business days for us to receive.<br />Parts sourced from BMWAG can take 5-20 business days for us to receive.<br />ETA of backordered parts can change or be indefinite.</li>
                            <li>After the order is submitted: if you’d like to add, change, or cancel anything, please contact us by email.<br />Cancelled special orders that cannot be stopped from arriving at MAX BMW, may be subject to a 15% restocking fee.</li>
                        </ul>
                    <%} else { %>
                        Your Shopping Cart is empty
                    <%} %>                                        
                </td>
                <td style="font-size: 11px; color: #333; border-left: 1px dotted #bbb;" align="center" valign="middle">
                    <label style="font-size: 13px;">Your Wish Lists:</label><br />
                    <%if (m_UserID > 0) { %>
                        <%if (m_WishListsCount>0) { %>
                            <%=ShowWishListsLinks() %>
                        <%} else { %>
                            <br />
                            You don't have any Wish Lists saved.<br /><br />
                            To create one, you can add items to your shopping cart, then from<br />
                            your shopping cart you have the options to save into a Wish List.<br />
                            You can have up to 5 Wish Lists.
                        <%} %>
                    <%} else { %>
                        <br />
                        You need to <a href="<%=Utilities.URL_FICHE(true) %>/UserLogin.aspx" style="text-decoration: none; font-weight: bold; color: #eb3f3c;">login</a> in order to manage your Wish Lists
                    <%} %>
                </td>
            </tr>
        </table>
                                                                                
        <table class="centered">
            <tr>
                <td style="vertical-align: top;" align="center">
                    <%if (m_SelectedList == 0) { %>
                        <%if (m_CartCount > 0) { %>
                            <table border="0" cellspacing="2" cellpadding="2" style="border: 0px; width: 780px; margin-top: 3px;">
                                <tr>
                                    <td align="left" class="Heading">
                                        Your current Shopping Cart Items:
                                    </td>
                                </tr>
                                <tr>
                                    <td align="center" class="Content">
                                        <%=cartHelper.ShowCart("", "", "", "", true, true) %>
                                    </td>
                                </tr>
                            </table>                                                                                        
                            <br />
                            <label style="font-weight: bold">Options for your Shopping Cart items:</label><br />
                            <br />                                            
                            <span class="Button" onclick="ClearCart();">Clear</span>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                            <span class="Button" onclick="UpdateAndSave();">Update and Save</span>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                            <span class="Button" id="BTNCHECKOUT" onclick="Checkout();">Proceed to Checkout&nbsp;&nbsp;<img alt="Checkout" src="images/checkout.gif" /></span>
                            <br />
                            <br />
                            <br />
                            <%if (m_UserID > 0) { %>
                                <%if (m_WishListsCount < 5) { %>
                                    <span class="Button" id="BTNSAVE" onclick="SaveCartAsWishListPrompt();">Save cart as new Wish List</span>
                                <%} %>
                                <%if (m_WishListsCount > 0) { %>
                                    <br />
                                    <table cellpadding="2" cellspacing="2" style="border: 0px;">
                                        <tr style="vertical-align: middle;">
                                            <td>
                                                Copy your cart items into an existing Wish List:
                                            </td>
                                            <td>
                                                <select name="CMBSELECTED">
                                                    <option selected="selected"></option>
                                                    <%=ShowWishListsComboBox()%>                                
                                                </select>
                                            </td>
                                            <td>
                                                <span class="Button" id="BTNCOPYTOWISHLIST" onclick="CopyCartToWishList();">Copy</span>
                                            </td>                                                        
                                        </tr>
                                    </table>
                                <%} %>
                            <%} else { %>
                                <br />
                                You need to <a href="<%=Utilities.URL_FICHE(true) %>/UserLogin.aspx" style="text-decoration: none; font-weight: bold; color: #eb3f3c;">login</a> in order to manage your Wish Lists
                            <%} %>                                        
                            <br />                                            
                            <label style="color: #f33; font-size: 10px; font-weight: normal;"><%=m_Msg %></label>
                            <script type="text/javascript">
                                UpdateFields();       
                            </script>                                        
                        <%} %>
                    <%} else { %>
                        <%if (GetWishListItemsCount(m_SelectedList) > 0) { %>
                            <table border="0" cellspacing="2" cellpadding="2" style="border: 0px; width: 780px; margin-top: 3px;">
                                <tr>
                                    <td align="left" class="Heading">
                                        Wish List Items for "<%=GetWishListName(m_SelectedList) %>":
                                    </td>
                                </tr>
                                <tr>
                                    <td align="center" class="Content">
                                        <%=ShowWishList(m_SelectedList) %>
                                    </td>
                                </tr>
                            </table>
                            <br />
                            <label style="font-weight: bold">Options for "<%=GetWishListName(m_SelectedList) %>" Wish List:</label><br />
                            <br />
                            <span class="Button" onclick="DeleteWishList();">Delete</span>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                            <span class="Button" onclick="UpdateAndSave();">Update and Save</span>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;                                            
                            <br />
                            <br />
                            <span class="Button" id="BTNRENAME" onclick="RenameWishListPrompt();">Rename</span>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                            <span class="Button" id="BTNCOPYTOCART" onclick="CopyWishListToCart();">Copy to Shopping Cart</span>
                            <br />                                   
                            <br />
                            <label style="color: #f33; font-size: 10px; font-weight: normal;"><%=m_Msg %></label>
                            <script type="text/javascript">
                                UpdateFields();       
                            </script>
                        <%} else { %>
                            <center>
                                <br /><br /><br /><br />
                                <span style="color: #000; font-size: 13px; font-weight: bold;">The wish list '<%=GetWishListName(m_SelectedList) %>' is empty.</span>
                                <br /><br /><br /><br /><br /><br />
                            </center>
                        <%} %>
                    <%} %>
                </td>
            </tr>
        </table>
        </form>

        <br />
        <div id="ClearFooter"></div> 
    </div>

    <uc:UC_Footer runat="server" /> 

    <div id="DIV_RENAMEWISHLIST" class="DIVModal">
        <div style="width: 100%; text-align: center; padding: 10px;">            
            <label style="font-weight: bold;">Enter a new name for your Wish List:</label><br />
        </div>
        <table cellpadding="5" cellspacing="5" style="width: 100%;">            
            <tr>
                <td align="center">
                    <input type="text" id="INPUT_RENAMEWISHLIST" style="border: 1px #333 solid" value="" maxlength="25" size="35" />
                </td>
            </tr>
            <tr>
                <td align="center">
                    <span class="Button" onclick="$('#DIV_RENAMEWISHLIST').hide(); $('.ModalDialog').remove(); RenameWishList();">OK</span>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="Button" onclick="$('#DIV_RENAMEWISHLIST').hide(); $('.ModalDialog').remove();">Cancel</span>    
                </td>
            </tr>
        </table>
    </div>
        
    <div id="DIV_SAVECARTASWISHLIST" class="DIVModal">
        <div style="width: 100%; text-align: center; padding: 10px;">
            <label style="font-weight: bold;">Enter a name for your Wish List:</label><br />
            <label style="color: #999; font-size: 11px;">Example: "My 2007 clutch project"</label><br />     
        </div>
        <table cellpadding="5" cellspacing="5" style="width: 100%;">
            <tr>
                <td align="center">
                    <input type="text" id="INPUT_SAVECARTASWISHLIST" style="border: 1px #333 solid" maxlength="25" size="35" />
                </td>
            </tr>
            <tr>
                <td align="center">
                    <span class="Button" onclick="$('#DIV_SAVECARTASWISHLIST').hide(); $('.ModalDialog').remove(); SaveCartAsWishList();">OK</span>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="Button" onclick="$('#DIV_SAVECARTASWISHLIST').hide(); $('.ModalDialog').remove();">Cancel</span>
                </td>
            </tr>
        </table>        
    </div>    
</body>
</html>
