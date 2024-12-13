<%@ Page Language="C#" AutoEventWireup="true" CodeFile="PartsSearch.aspx.cs" Inherits="PartsSearch" %>
<%@ Register TagPrefix="uc" TagName="UC_MainMenu" Src="~/UC_MainMenu.ascx" %>
<%@ Register TagPrefix="uc" TagName="UC_ButtonsMenu" Src="~/UC_ButtonsMenu.ascx" %>
<%@ Register TagPrefix="uc" TagName="UC_Footer" Src="~/UC_Footer.ascx" %>

<!DOCTYPE html>
<html>
<head>
    <title>BMW Motorcycle Parts | Search for parts</title>
    <meta name="description" content="MAX BMW Motorcycles offers an extensive BMW Motorcycle Parts fiche. OEM BMW Motorcycle Parts and Accessores can be ordered online for fast delivery. View our detailed BMW Motorcycle Parts diagrams and locate the part that you need efficiently." />
    <meta name="rating" content="general" />
    <meta name="author" content="MAX BMW Motorcycles Parts" /> 
    <meta http-equiv="Pragma" content="no-cache" />
    <meta http-equiv="CACHE-CONTROL" content="NO-CACHE" />
    <meta http-equiv="Expires" content="0" />
    <meta name="apple-mobile-web-app-capable" content="yes" />
    <meta name="apple-mobile-web-app-status-bar-style" content="black" />   
    <meta name="format-detection" content="telephone=no" />
       
    <script type="text/javascript">
        try {
            if (top != self) {
                top.location.replace(window.location.href);
            }
        } catch (e) { }
    </script> 

    <script type="text/javascript" src="js/ie6no.js" charset="utf-8"></script>
    <script type="text/javascript">
        ie6no({ runonload: true });
    </script>

    <script type="text/javascript" src="js/jquery-1.7.1.min.js"></script>
        
    <link href="css/Global.css" rel="stylesheet" type="text/css" media="all" />
    <link href="css/MainMenu.css" rel="stylesheet" type="text/css" media="all"  />
    <link href="css/ButtonsMenu.css" rel="stylesheet" type="text/css" media="all"  /> 
    <link href="css/ProductsDetails.css" rel="stylesheet" type="text/css" media="all" />
    <link href="css/ListProducts.css" rel="stylesheet" type="text/css" media="all"  />

    <style type="text/css">        
    td.SearchResultDiagram
    {
        cursor: pointer;
        color: #eee;
        font-weight: bold;
        border: 1px solid #262626;
        background-color: #262626;
    }
    td.SearchResultDiagram.FirstRow
    {
        height: 25px;
        font-size: 11px;
    }
    td.SearchResultDiagram.FirstRow.Left
    {  
        padding-left: 2px;
        width: 24px;
        -moz-border-radius-topleft: 4px;
	    -webkit-border-top-left-radius: 4px;
	    border-top-left-radius: 4px;
    }
    td.SearchResultDiagram.FirstRow.Right
    {
        text-align: left;
        width: 176px;
        padding-right: 14px;        
        background-image: url('images/GoRedSmall.png');
        background-repeat: no-repeat;
        background-position: right;
        -moz-border-radius-topright: 4px;
	    -webkit-border-top-right-radius: 4px;
	    border-top-right-radius: 4px;
    }
    td.SearchResultDiagram.SecondRow
    {
        font-size: 10px;        
    }
    td.SearchResultDiagram.ThirdRow
    {
        font-size: 10px;        
    }
    </style>

    <script type="text/javascript" src="js/jquery-contained-sticky-scroll.js"></script>
    <link href="css/ComboBox.css" rel="stylesheet" type="text/css" />
    <link href="css/ListProducts.css" rel="stylesheet" type="text/css" />

    <script type="text/javascript" src="js/jquery.tokeninput.js"></script>
    <link href="css/token-input.css" rel="stylesheet" type="text/css" />
    <link href="css/token-input-facebook.css" rel="stylesheet" type="text/css" />


    <script type="text/javascript">
        function SetVehicleIcon(text1, text2, icon) {
            document.getElementById('VEHICLETEXT1').innerText = text1;
            document.getElementById('VEHICLETEXT2').innerText = 'Production: ' + text2;
            document.getElementById('VEHICLEICON').src = "VehiclesIcons/" + icon + ".jpg?v=<%=Utilities.VERSION %>";
        }

        function getUrlVars(url) {
            var vars = [], hash;
            var hashes = url.slice(url.indexOf('?') + 1).split('&');
            for (var i = 0; i < hashes.length; i++) {
                hash = hashes[i].split('=');
                vars.push(hash[0].toLowerCase());
                vars[hash[0].toLowerCase()] = hash[1];
            }
            return vars;
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
                
        // Callback function for the load() of the image
        function ResizeZoomed(ID_DIVZoomed) {
            var big = $('#' + ID_DIVZoomed + '>.BigImg');

            big.off('load');

            $('#' + ID_DIVZoomed).css({
                position: 'fixed',
                width: big.width(),
                height: big.height(),
                left: ($(window).width() - big.width()) / 2,
                top: ($(window).height() - big.height()) / 2
            });

            $('.BottonClose').css('right', '5px');
        }

        function UpdateResults() {
            $('#Loading').show();

            var searchtype = $('input:radio[name=SEARCHTYPE]:checked').val();
            var vid = $('#comboBoxModels').val();
            var category = $('#comboBoxCategories').val();
            var brand = $('#comboBoxBrands').val();
            var parts = $('#PARTS').val();
            var product = $('#PRODUCT').val();            

            if (parts.length > 0)
                product = "";

            // blank them in case of "...: All ..."
            if (vid.indexOf(':') >= 0) vid = '';
            if (brand.indexOf(':') >= 0) brand = '';
            if (category.indexOf(':') >= 0) category = '';
            if (category.indexOf('Front &amp; Rear') >= 0) category = '';            

            // build the url with the parameters
            var params = '';
            params += (searchtype != '' ? (searchtype != "" ? "&" : "") + 'searchtype=' + searchtype : '');
            params += (vid != '' ? (params != "" ? "&" : "") + 'vid=' + escape(vid) : '');
            params += (category != '' ? (params != "" ? "&" : "") + 'category=' + escape(category) : '');
            params += (brand != '' ? (params != "" ? "&" : "") + 'brand=' + escape(brand) : '');
            params += (parts != '' ? (params != "" ? "&" : "") + 'parts=' + escape(parts) : '');
            params += (product != '' ? (params != "" ? "&" : "") + 'product=' + escape(product) : '');

            
            if ($('#ViewModeList').css('border-left-color') == 'rgb(235, 63, 60)') // rgb(204, 51, 51) is the same as #cc3333  /   eb3f3c=235,63,60
                params += '&viewmode=list';

            window.location = 'PartsSearch.aspx' + (params != '' ? '?' + params : '');
        }


        function BikeModelClicked(model, vid) {
            // catalog or tires was selected (apparel has no models), so update the iframe only
            $('#comboBoxModels').parent().children('.ComboBoxText').text(model);
            $('#comboBoxModels').parent().children('.ComboBoxText').css('background-color', '#ccc');
            $('#comboBoxModels').parent().children('.ComboBoxX').show();
            $('#comboBoxModels').val(vid);
        }       
    </script>  

    <script type="text/javascript">
        // functions for the shopping cart AddCart page (that runs in the IFRAME)
        function loadCartContainer(docIframe) {
            document.getElementById('CartContainer').innerHTML = docIframe.getElementById('CartContainer').innerHTML;
            UpdateFields();
        }

        function AddToCart(PartNumber, Qty, Description, Weight) {
            document.getElementById('CARTFRAME').src = "CartAdd.aspx?partnumber=" + PartNumber + "&qty=" + Qty + "&description=" + Description + "&weight=" + Weight;

            $('#ShoppingCart').css('height', '560');
            $('#CARTFRAME').css('height', '500');

            ShowModal('ShoppingCart');
        }

        function UpdateAndSaveCart() {
            var partnumber = $('#PARTNUMBER_ADDED').val();
            var qty = $('#Q_ADDED').val();
            var description = $('#DESCRIPTION_ADDED').val();
            var weight = $('#W_ADDED').val();

            description = escape(description);
            description = description.replace('%A0', '%20'); // this special char comes up some times

            $('#CARTFRAME').attr('src', 'CartAdd.aspx?updatecart=1&partnumber=' + partnumber + '&qty=' + qty + '&description=' + description + '&weight=' + weight);
            $("#ShoppingCart").hide();
            $(".ModalDialog").remove();
        }

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
        }
    </script>
</head>
<body>
<div id="centered">    
    <uc:UC_MainMenu runat="server" />
    <uc:UC_ButtonsMenu runat="server" />

    <div id="BikeModelsMenu" style="position: relative; display: none;">
        <div id="VEHICLETOOLTIP" style="padding-left: 15px; display: none; position: absolute; width: 250px; height: 124px; background-image: url(images/VehiclesTooltipLeft.png); background-repeat: no-repeat; z-index: 9999;">                        
            <table style="width: 190px; width-min: 190px;" cellpadding="0" cellspacing="0">
                <tr style="vertical-align: middle;">
                    <td style="color: #fff; font-weight: bold; font-family: Verdana, Arial; text-align: center; line-height: 12px; padding-top: 4px;">
                        <label id="VEHICLETEXT1" style="font-size: 11px;">&nbsp;</label><br />
                        <label id="VEHICLETEXT2" style="font-size: 9px;">&nbsp;</label><br />
                        <img id="VEHICLEICON" src="images/1pixel.gif" style="padding-top: 8px; height: 75px; width: 150px;"/>                                    
                    </td>
                </tr>
            </table>
        </div>
        <table cellspacing="0" cellpadding="0" border="0" style="font-family: Verdana; font-weight: bold; color: #888;">
            <tr style="font-size: 15px;">
                <td align="center" width="155">
                    F &amp; G Bikes
                </td>                            
                <td align="center" width="240">
                    K &amp; S Bikes
                </td>          
                <td align="center" width="565">
                    R Bikes              
                </td>
            </tr>
        </table>
        <table cellspacing="0" cellpadding="0" border="0" style="font-family: Verdana; font-weight: bold; color: #888;">
            <tr style="font-size: 13px;">
                <td width="395">
                    &nbsp;
                </td>
                <td align="center" width="145">
                    Hexheads&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                </td>
                <td align="center" width="115">
                    &nbsp;&nbsp;&nbsp;&nbsp;Oilheads&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                </td>
                <td align="left" width="315">
                    &nbsp;&nbsp;&nbsp;Pre-1970&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;1970-1995
                </td>
            </tr>
        </table>
        <table id="TableBikeModels" cellspacing="0" cellpadding="0" border="0" style="font-family: Verdana; font-weight: bold; font-size: 10px;">
            <tr valign="top">
                <td align="left" width="155">
                    <%=ShowBikesList("G%") %>
                    <br />
                    <%=ShowBikesList("F650 Funduro")%>
                    <%=ShowBikesList("F650ST%")%>
                    <%=ShowBikesList("F650CS%")%>
                    <%=ShowBikesList("F650GS%")%>
                    <%=ShowBikesList("F700%")%>
                    <%=ShowBikesList("F800GS%")%>
                    <%=ShowBikesList("F800R")%>
                    <%=ShowBikesList("F800S")%>
                    <%=ShowBikesList("F800ST")%>
                    <%=ShowBikesList("F800GT")%>
                </td>
                <td width="8">&nbsp;</td>
                <td width="8" style="border-left: 1px #999 dotted;">&nbsp;</td>
                <td align="left" width="105">                                 
                    <%=ShowBikesList("K1100%")%>
                    <%=ShowBikesList("K1200RS 98%")%>
                    <%=ShowBikesList("K1200RS 02%")%>
                    <%=ShowBikesList("K1200GT 03%")%>
                    <%=ShowBikesList("K1200LT 99%")%>
                    <%=ShowBikesList("K1200LT 05%")%>
                    <%=ShowBikesList("K1200R")%>
                    <%=ShowBikesList("K1200R Sport")%>
                    <%=ShowBikesList("K1200GT 06%")%>
                    <%=ShowBikesList("K1200S%")%>
                    <%=ShowBikesList("K1300GT%")%>
                    <%=ShowBikesList("K1300R%")%>
                    <%=ShowBikesList("K1300S%")%>
                    <%=ShowBikesList("K1600%")%>                                      
                    <br />
                    <%=ShowBikesList("S%")%>
                    <%=ShowBikesList("HP4%")%>
                    <br />
                    <label style="font-family: Verdana; font-weight: bold; color: #888; font-size: 15px;">C Scooters</label><br />
                    <%=ShowBikesList("C6%")%>
                </td>
                <td width="8">&nbsp;</td>
                <td align="left" width="95">                                                                                  
                    <%=ShowBikesList("K1,K100%,K75%")%>
                </td>
                <td width="8">&nbsp;</td>
                <td width="8" style="border-left: 1px #999 dotted;">&nbsp;</td>
                <td align="left" width="135">
                    <%=ShowBikesList("HP2%")%>
                    <br />
                    <%=ShowBikesList("R1200GS 05%")%>
                    <%=ShowBikesList("R1200GS 08%")%>
                    <%=ShowBikesList("R1200GS 10%")%>
                    <%=ShowBikesList("R1200GSW%")%>
                    <%=ShowBikesList("R1200GS ADV%")%>
                    <%=ShowBikesList("R1200R %")%>
                    <%=ShowBikesList("R1200RT%")%>
                    <%=ShowBikesList("R1200S")%>
                    <%=ShowBikesList("R1200ST")%>
                    <%=ShowBikesList("R900RT%")%>
                </td>
                <td width="8">&nbsp;</td>
                <td align="left" width="115">
                    <%=ShowBikesList("R1100GS")%>
                    <%=ShowBikesList("R1100R")%>
                    <%=ShowBikesList("R1100RS")%>
                    <%=ShowBikesList("R1100RT")%>
                    <%=ShowBikesList("R1200C %")%>
                    <%=ShowBikesList("R1200CL")%>
                    <%=ShowBikesList("R1200 M%")%>
                    <%=ShowBikesList("R1100S")%>
                    <%=ShowBikesList("R1150GS%")%>
                    <%=ShowBikesList("R850R")%>
                    <%=ShowBikesList("R1150R")%>
                    <%=ShowBikesList("R1150R %")%>
                    <%=ShowBikesList("R1150RS")%>
                    <%=ShowBikesList("R1150RT")%>
                </td>
                <td width="8">&nbsp;</td>
                <td align="left" width="70">
                    <%=ShowBikesList("R24%,R25%,R26%,R27%,R50,R50US,R50/2,R50S,R51%,R60,R60US,R60/2,R67%,R68%,R69%")%>
                </td>
                <td width="8">&nbsp;</td>
                <td valign="top" align="left" width="83">
                    <%=ShowBikesList("R45%,R50/5,R60/5,R60/6,R60/7,R65%,R75%,R80%")%>
                </td>
                <td width="5">&nbsp;</td>
                <td align="left" width="112">
                    <%=ShowBikesList("R90/6%,R90S%,R100%")%>
                </td>
            </tr>                                    
        </table>
    </div>

    <div class="HeaderBar">
        <div class="ButtonClose" title="Close and go to the Main Menu" onclick="$('#Loading').show(); window.location='fiche.aspx';"></div>        
        <table width="100%" height="30" cellpadding="0" cellspacing="0">
            <tr>
                <td width="110">
                    <span class="BackButton Up" onclick="window.location='fiche.aspx';">
                        Main Menu
                    </span>                               
                </td>
                <td class="BarSeparator">&nbsp;</td>
                <td align="center">
                    <table cellpadding="0" cellspacing="0" style="margin-left: auto; margin-right: auto;">
                        <tr>
                            <td><img src="images/IconSearch.png" /></td>
                            <td id="ApplicationLabel">Advanced Parts Search</td>
                            <td align="left" id="ProductsCount">&nbsp;</td>
                        </tr>
                    </table>                    
                </td>
            </tr>
        </table>
        <div style="width: 100%; height: 5px; background-color: #eb3f3c;"></div>
    </div>

    <div id="ProductsResults" class="ProductsResults">
        <table cellpadding="0" cellspacing="0" border="0">
            <tr>
                <td valign="top">
                    <div class="FiltersBox" style="width: 250px;margin-top: 0px; font-size: 13px; font-weight: bold;"> 
                        <!--
                        Search by:<br />
                        <input type="radio" name="SEARCHTYPE" value="partnumber" <%=(m_SearchType=="partnumber"?"checked=\"checked\"":"") %>>Part numbers<br />
                        <input type="radio" name="SEARCHTYPE" value="product" <%=(m_SearchType=="product"?"checked=\"checked\"":"") %>>Name or description<br />
                        <br />
                        <br />
                        -->
                        <label style="font-size: 13px; font-weight: bold;">
                            Narrow down by:
                        </label>
                        <br />        
                        <div id="ModelsFilter" class="ComboBox" style="width: 200px;">
                            <label class="ComboBoxText"></label><label class="ComboBoxX">x</label>
                            <input class="ComboBoxDefault" type="hidden" value="Models: All Models" />
                            <input id="comboBoxModels" class="ComboBoxInput" type="hidden" />
                            <div style="position: relative;" class="ComboBoxButton"></div>
                            <div id="ComboBoxDropdownBikeModels" class="ComboBoxDropdown" style="width: 920px; max-height: 340px; height: 340px; padding: 10px;">
                                <!-- Bike Models menu goes in here dynamically -->
                            </div>
                        </div>
                        <br />
                        <div class="ComboBox" style="width: 200px;">
                            <label class="ComboBoxText"></label><label class="ComboBoxX">x</label>
                            <input class="ComboBoxDefault" type="hidden" value="Sources: All Sources" />
                            <input id="comboBoxCategories" class="ComboBoxInput" type="hidden" />
                            <div style="position: relative;" class="ComboBoxButton"></div>
                            <div class="ComboBoxDropdown">
                                <ul style="list-style-type: none; padding: 3px; margin: 0px; cursor: pointer;">
                                    <!-- populated by jquery -->
                                </ul>
                            </div>
                        </div>
                        <br />
                        <div class="ComboBox" style="width: 200px;">
                            <label class="ComboBoxText"></label><label class="ComboBoxX">x</label>
                            <input class="ComboBoxDefault" type="hidden" value="Brands: All Brands" />
                            <input id="comboBoxBrands" class="ComboBoxInput" type="hidden" />
                            <div style="position: relative;" class="ComboBoxButton">
                            </div>
                            <div class="ComboBoxDropdown">
                                <ul style="list-style-type: none; padding: 3px; margin: 0px; cursor: pointer;">
                                    <!-- populated by jquery -->
                                </ul>
                            </div>
                        </div>
                        <br />
                            <div id="PARTSFRAME" style="display: <%=(m_SearchType=="partnumber"?"block":"none") %>">
                            <label style="color: #333; font-size: 13px; font-weight: bold;">
                                Part numbers:
                            </label>                            
                            <br />
                            <textarea id="PARTS" cols="20" rows="5" style="border: 1px solid #666; width: 200px;"><%=m_parts %></textarea>
                        </div>
                        <!--
                        <div id="PRODUCTFRAME" style="display: <%=(m_SearchType=="product"?"block":"none") %>">
                            <label style="color: #333; font-size: 13px; font-weight: bold;">
                                Product name or description:
                            </label>                            
                            <br />
                            <input type="text" id="PRODUCT" style="width: 203px;" />
                        </div> 
                        -->                                                     
                        <br />
                        <br />                        
                        <span class="Button" style="background-image: url('images/GoRedSmall.png'); background-repeat: no-repeat; background-position: 60px; padding-right: 24px; margin-right: 5px; position: absolute; left: 135px;" onclick="UpdateResults();">&nbsp;&nbsp;Search&nbsp;</span>
                        <br />
                    </div>
                </td>
                <td valign="top" align="center" style="color: #333;">
                    <%=ShowParts() %>
                    <br />
                    <%=ShowDiagramsByParts()%>
                    <br />
                    <%=ShowDiagramsByDiagrams()%>                        
                </td>
            </tr>        
        </table>
    </div>
    <br /> 
    <div id="ClearFooter"></div>
</div>   
 
<uc:UC_Footer runat="server" />   

<div id="Loading" style="display: none; position: fixed; top: 30%; left: 40%; background-color: #fff; border: 3px solid #999; width: 220px; height: 130px; text-align: center; vertical-align: middle;">
    <table cellpadding="0" cellspacing="20" width="100%">
        <tr>
            <td align="center">
                <img src="images/loading.gif" />
            </td>
        </tr>
        <tr>
            <td align="center" style="font-size: 13px; font-weight: bold; color: #333;">
                Loading...
            </td>
        </tr>
    </table>   
</div>

<p id="back-top">
    <a href="#top"><span></span>Back to Top</a>
</p>
   
<div id="DIVZoomedPart" class="DIVZoomed">
    <div class="ButtonClose" title="Close"></div>        
    <img src="" id='bigpart' class="BigImg" height="550" />
</div>

<div id="DIVZoomedVideo" class="DIVZoomed">
    <div class="ButtonClose" title="Close"></div>
    <div style="height: 26px;"></div>
    <div id="ytapiplayer" style="display: none;">
        You need Flash player 8+ and JavaScript enabled to view this video.
    </div>        
    <object width="520" height="417" id="YouTubeVideo">                
    </object>    
</div>

<div id="ShoppingCart" class="ShoppingCart">        
    <div class="ButtonClose" title="Close"></div>
    <div id="CartContainer"></div>
</div>       

<iframe id="CARTFRAME" src="about:blank" style="display: none;"></iframe>

<script type="text/javascript">
    $(document).ready(function () {
        /*
        $('[name="SEARCHTYPE"]').change(function () {
            if ($(this).val() == "product" && $(this).attr("checked")) {
                $("#PARTSFRAME").hide();
                $("#PRODUCTFRAME").show();
            } else {
                $("#PRODUCTFRAME").hide();
                $("#PARTSFRAME").show();
            }

            if ($('input:radio[name=SEARCHTYPE]:checked').val() == "partnumber")
                $('#PARTS').focus();
            else
                $('#PRODUCT').focus();
        });
        */
        $('#PARTS').focus();

        $('.ComboBoxButton').click(function () {
            var thisDropdown = $(this).next('.ComboBoxDropdown');
            thisDropdown.toggle(300);
        });

        $('.ComboBoxDropdown').click(function () {
            $(this).hide();
        });

        $('.ComboBoxText').click(function () {
            $(this).parent().children('.ComboBoxDropdown').toggle(300);
        });

        $('.ComboBoxX').click(function () {
            var defaultValue = $(this).parent().children('.ComboBoxDefault').val();
            $(this).parent().children('.ComboBoxInput').val(defaultValue);
            $(this).parent().children('.ComboBoxText').text(defaultValue);
            //$(this).parent().children('.ComboBoxText').hide();
            $(this).prev().css('background-color', '#eee');
            $(this).parent().children('.ComboBoxDropdown').hide();
            $(this).hide();
        });

        $(document).click(function () {
            $('.ComboBoxDropdown').hide();
        });

        $('.ComboBox').click(function (e) {
            e.stopPropagation();
        });

        $("td.ButtonMenu").hover(function () {
            $(this).addClass("hoverlook").siblings().removeClass("hoverlook");
        }, function () {
            $(this).removeClass("hoverlook");
        });

        $("#BikeModelsMenu").bind('mousemove', function (e) {
            var x = e.pageX - $("#BikeModelsMenu").offset().left + 30;
            var y = e.pageY - $("#BikeModelsMenu").offset().top - 18;

            if (x + $("#VEHICLETOOLTIP").width() > $("#BikeModelsMenu").width()) {
                $("#VEHICLETOOLTIP").css("background-image", "url('images/VehiclesTooltipRight.png')");
                $("#VEHICLETOOLTIP").css("padding-left", "5px");
                x = x - ($("#VEHICLETOOLTIP").width() + 45);
            } else {
                $("#VEHICLETOOLTIP").css("background-image", "url('images/VehiclesTooltipLeft.png')");
                $("#VEHICLETOOLTIP").css("padding-left", "15px");
            }

            $("#VEHICLETOOLTIP").css("left", x);
            $("#VEHICLETOOLTIP").css("top", y);
        });

        $("a.TooltipLink").hover(function (evt) {
            $("#VEHICLETOOLTIP").css("display", "block");
        }, function () {
            $("#VEHICLETOOLTIP").css("display", "none");
        });

        $('.FiltersBox').containedStickyScroll({
            duration: 400,
            closeChar: ''
        });

        // case Search
        $("#HeaderBarTopArrow").css("left", "868px");
        $("#ComboBoxDropdownBikeModels").append($("#BikeModelsMenu")); // dynamically place the Bike Models Menu inside of the DIV ComboBoxDropdownBikeModels
        $("#ModelsFilter").css("display", "block");
        $("#BikeModelsMenu").css("display", "block");

        // update Catalog Categories 
        var ul = $("#comboBoxCategories").parent().children(".ComboBoxDropdown").children("ul");
        ul.children().remove();
        ul.append("<li>Fiche</li>");
        ul.append("<li>Catalog</li>");
        ul.append("<li>Tires</li>");
        ul.append("<li>Apparel</li>");

        // update Brands
        ul = $("#comboBoxBrands").parent().children(".ComboBoxDropdown").children("ul");
        ul.children().remove();
        ul.append("<li>MAX BMW</li>");
        ul.append("<li>BMW</li>");
        ul.append("<li>Albert</li>");
        ul.append("<li>Akrapovic</li>");
        ul.append("<li>Best Rest</li>");
        ul.append("<li>Bing</li>");
        ul.append("<li>Centech</li>");
        ul.append("<li>Clymer</li>");
        ul.append("<li>CPC</li>");
        ul.append("<li>Dimple</li>");
        ul.append("<li>EME</li>");
        ul.append("<li>FAT CAT</li>");
        ul.append("<li>Gerbing</li>");
        ul.append("<li>GPR</li>");
        ul.append("<li>Grimeca</li>");
        ul.append("<li>Hornig</li>");
        ul.append("<li>HPN</li>");
        ul.append("<li>Iwis</li>");
        ul.append("<li>K&amp;N</li>");
        ul.append("<li>Kaoko</li>");
        ul.append("<li>Meier</li>");
        ul.append("<li>Motion Pro</li>");
        ul.append("<li>MotoEquip</li>");
        ul.append("<li>Motoren Israel</li>");
        ul.append("<li>Motorrad Elektrik</li>");
        ul.append("<li>Nippondenso</li>");
        ul.append("<li>Ohlins</li>");
        ul.append("<li>Permatex</li>");
        ul.append("<li>Progressive</li>");
        ul.append("<li>Remus</li>");
        ul.append("<li>R&amp;G</li>");
        ul.append("<li>SpeedBleeder</li>");
        ul.append("<li>ThreeBond</li>");
        ul.append("<li>Touratech</li>");
        ul.append("<li>TwinMax</li>");
        ul.append("<li>Whitehorse Press</li>");
        ul.append("<li>Wunderlich</li>");
        ul.append("<li>Wurth</li>");
        ul.append("<li>Yuasa</li>");

        $("#HeaderBarTopArrow").show();

        // update the li click function
        $('.ComboBoxDropdown li').click(function () {
            var newValue = $(this).text();
            $(this).parent().parent().parent().children('.ComboBoxText').text(newValue);
            $(this).parent().parent().parent().children('.ComboBoxText').css('background-color', '#ccc');
            $(this).parent().parent().parent().children('.ComboBoxX').show();
            $(this).parent().parent().parent().children('.ComboBoxInput').val(newValue);
        });

        $("#TableBikeModels").css("font-size", "10px");

        var currentParams = getUrlVars(window.location.href);
        // Populate the combo boxes with the params from the url (if any)
        var combobox;

        combobox = $("#comboBoxModels").parent();
        var vid = currentParams['vid'];
        var model = "";
        if (vid != null && vid != "") {
            model = "<%=GetModelFromVID(m_vid) %>";
            combobox.children('.ComboBoxText').text(model);
            combobox.children('.ComboBoxText').css('background-color', '#ccc');
            combobox.children('.ComboBoxX').show();
            combobox.children('.ComboBoxInput').val(vid); // carefull
        } else {
            combobox.children(".ComboBoxText").text('Models: All Models');
        }

        combobox = $("#comboBoxBrands").parent();
        var brand = currentParams['brand'];
        if (brand != null && brand != "") {
            brand = unescape(brand);
            brand = brand.replace(/\+/g, ' '); // replace '+' with ' '
            combobox.children('.ComboBoxText').text(brand);
            combobox.children('.ComboBoxText').css('background-color', '#ccc');
            combobox.children('.ComboBoxX').show();
            combobox.children('.ComboBoxInput').val(brand);
        } else {
            combobox.children(".ComboBoxText").text('Brands: All Brands');
        }

        combobox = $("#comboBoxCategories").parent();
        var category = currentParams['category'];
        if (category != null && category != "") {
            category = unescape(category);
            category = category.replace(/\+/g, ' '); // replace '+' with ' '
            combobox.children('.ComboBoxText').text(category);
            combobox.children('.ComboBoxText').css('background-color', '#ccc');
            combobox.children('.ComboBoxX').show();
            combobox.children('.ComboBoxInput').val(category);
        } else {
            combobox.children(".ComboBoxText").text('Sources: All Sources');
        }

        $('#ProductsCount').text($('#ProductsCountInvisible').text() + (model != "" ? " - filtered by " + model : ""));

        $('.ComboBoxText').show();

        // fade in #back-top
        $(window).scroll(function () {
            if ($(this).scrollTop() > 100) {
                $('#back-top').fadeIn();
            } else {
                $('#back-top').fadeOut();
            }
        });

        // scroll body to 0px on click
        $('#back-top a').click(function () {
            $('body,html').animate({
                scrollTop: 0
            }, 800);
            return false;
        });

        // For the Parts zoom
        $(".PartThumbnailImg").click(function () {
            var bigSrc = $(this).attr('src').replace("_N.", "_B.");
            $('#DIVZoomedPart>.BigImg').attr('src', bigSrc);

            ShowModal('DIVZoomedPart');

            $('#DIVZoomedPart>.BigImg').load(function () {
                ResizeZoomed('DIVZoomedPart');
            });
            ResizeZoomed('DIVZoomedPart');
        });

        $('.ThumbnailVideo').click(function () {
            var videoID = $(this).attr('alt'); // get the youtube video id stored in the alt attrib
            $('#YouTubeVideo').children().remove(); // clear it first in case there was something old there
            $('#YouTubeVideo').append('<param name="allowFullScreen" value="true"></param>');
            $('#YouTubeVideo').append('<param name="allowscriptaccess" value="always"></param>');
            $('#YouTubeVideo').append('<param name="movie" value="http://www.youtube.com/v/' + videoID + '&hl=en&fs=1&rel=0&color1=0x3a3a3a&color2=0x999999"></param>');
            $('#YouTubeVideo').append('<embed src="http://www.youtube.com/v/' + videoID + '&hl=en&fs=1&rel=0&color1=0x3a3a3a&color2=0x999999" type="application/x-shockwave-flash" allowscriptaccess="always" allowfullscreen="true" width="520" height="417"></embed>');

            $('#DIVZoomedVideo').css({
                position: 'fixed',
                width: 520,
                height: 442,
                left: ($(window).width() - 520) / 2,
                top: ($(window).height() - 442) / 2
            });

            $('.BottonClose').css('right', '5px');

            ShowModal('DIVZoomedVideo');

            $('.ButtonClose').click(function () {
                $('#YouTubeVideo').children().remove();
            });
        });

        $('.ThumbnailPDF').click(function () {
            var PDFLink = $(this).attr('alt'); // get the PDF url stored in the alt attrib
            window.open(PDFLink, "PDF");
        });

        /*
        $('#PRODUCT').tokenInput(function () {
            var params = '';

            var vid = $('#comboBoxModels').val();
            var category = $('#comboBoxCategories').val();
            var brand = $('#comboBoxBrands').val();

            // blank them in case of "...: All ..."
            if (vid.indexOf(':') >= 0) vid = '';
            if (brand.indexOf(':') >= 0) brand = '';
            if (category.indexOf(':') >= 0) category = '';
            if (category.indexOf('Front &amp; Rear') >= 0) category = '';

            // build the url with the parameters
            params += (vid != '' ? (params != "" ? "&" : "") + 'vid=' + escape(vid) : '');
            params += (category != '' ? (params != "" ? "&" : "") + 'category=' + escape(category) : '');
            params += (brand != '' ? (params != "" ? "&" : "") + 'brand=' + escape(brand) : '');
            if (params == "")
                return 'ws/PartDescriptions.aspx';
            else
                return 'ws/PartDescriptions.aspx?' + params;
        }, {
            theme: "facebook",
            propertyToSearch: "firstline",
            searchDelay: 500,
            minChars: 3,
            tokenLimit: 1,
            hintText: "Type name or description",
            resultsFormatter: function (item) { return "<li><table cellspacing='0' cellpadding='0'><tr valign='middle'><td align='left' width='25'><img src='images/Icon" + item.icon + ".png' alt='' />&nbsp;</td><td align='left' width='380'>" + item.firstline + (item.secondline != "" ? "<br /><label style='font-size: 9px;'>" + item.secondline + "</label>" : "") + "</td></tr></table></li>" },
            tokenFormatter: function (item) { var t = item.token; t = (t.length > 21 ? t.substring(0, 21) + '...' : t); return "<li>" + t + "</li>" }
        });

        $('#Loading').hide();

        if ($('input:radio[name=SEARCHTYPE]:checked').val() == "partnumber")
            $('#PARTS').focus();   
        */
        $('#PARTS').focus();
    });
</script>
</body>
</html>