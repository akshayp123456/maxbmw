<%@ Page Language="C#" AutoEventWireup="true" CodeFile="PartsSpecials.aspx.cs" Inherits="PartsSpecials" %>
<%@ Register TagPrefix="uc" TagName="UC_MainMenu" Src="~/UC_MainMenu.ascx" %>
<%@ Register TagPrefix="uc" TagName="UC_Footer" Src="~/UC_Footer.ascx" %>

<!DOCTYPE html>
<html>
<head>
    <title>BMW Motorcycle Parts | Specials and Clearance parts</title>
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
    <script src="js/jquery-contained-sticky-scroll.js" type="text/javascript"></script>

    <link href="css/Global.css" rel="stylesheet" type="text/css" media="all" />
    <link href="css/MainMenu.css" rel="stylesheet" type="text/css" media="all"  />
    <link href="css/ListProducts.css" rel="stylesheet" type="text/css" media="all"  />
    <link href="css/ProductsDetails.css" rel="stylesheet" type="text/css" media="all"  />
    <link href="css/ComboBox.css" rel="stylesheet" type="text/css" />

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

            var vid = $('#comboBoxModels').val();
            var mg = $('#comboBoxMG').val();
            var parts = $('#PARTS').val();

            // blank them in case of "...: All ..."
            if (vid.indexOf(':') >= 0) vid = '';
            if (mg.indexOf(':') >= 0) mg = '';

            // build the url with the parameters
            var params = '';
            params += (vid != '' ? (params != "" ? "&" : "") + 'vid=' + vid : '');
            params += (mg != '' ? (params != "" ? "&" : "") + 'mg=' + mg : '');
            params += (parts != '' ? (params != "" ? "&" : "") + 'parts=' + escape(parts) : '');

            window.location = 'PartsSpecials.aspx' + (params != '' ? '?' + params : '');
        }

        function BikeModelClicked(model, vid) {
            $('#comboBoxModels').parent().children('.ComboBoxText').text(model);
            $('#comboBoxModels').parent().children('.ComboBoxText').css('background-color', '#ccc');
            $('#comboBoxModels').parent().children('.ComboBoxX').show();
            $('#comboBoxModels').val(vid);
        }

        function MainGroupClicked(description, mg) {
            $('#comboBoxMG').parent().children('.ComboBoxText').text(description);
            $('#comboBoxMG').parent().children('.ComboBoxText').css('background-color', '#ccc');
            $('#comboBoxMG').parent().children('.ComboBoxX').show();
            $('#comboBoxMG').val(mg);
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
                    Oilheads&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                </td>
                <td align="center" width="315">
                    Airheads&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                </td>
            </tr>
        </table>
        <table id="TableBikeModels" cellspacing="0" cellpadding="0" border="0" style="font-family: Verdana; font-weight: bold; font-size: 11px;">
            <tr valign="top">
                <td align="left" width="155">
                    <%=ShowBikesList("G%") %>
                    <br />
                    <%=ShowBikesList("F%")%>
                </td>
                <td width="8">&nbsp;</td>
                <td width="8" style="border-left: 1px #999 dotted;">&nbsp;</td>
                <td align="left" width="105">                                 
                    <%=ShowBikesList("K1100%,K1200%,K1300%,K1600%")%>
                    <br />
                    <%=ShowBikesList("S%")%>
                </td>
                <td width="8">&nbsp;</td>
                <td align="left" width="95">                                                                                  
                    <%=ShowBikesList("K1,K100%,K75%")%>
                </td>
                <td width="8">&nbsp;</td>
                <td width="8" style="border-left: 1px #999 dotted;">&nbsp;</td>
                <td align="left" width="135">
                    <%=ShowBikesList("HP%")%>
                    <br />
                    <%=ShowBikesList("R1200G%,R1200R%,R1200S%,R900%")%>
                </td>
                <td width="8">&nbsp;</td>
                <td align="left" width="115">
                    <%=ShowBikesList("R1200C%,R1200 Montauk,R1150%,R1100%,R850%")%>
                </td>
                <td width="8">&nbsp;</td>
                <td align="left" width="55">
                    <%=ShowBikesList("R24%,R25%,R26%,R27%,R45%,R50%,R51%")%>
                </td>
                <td width="8">&nbsp;</td>
                <td valign="top" align="left" width="88">
                    <%=ShowBikesList("R60%,R65%,R67%,R68%,R69%,R75%,R80%")%>
                </td>
                <td width="8">&nbsp;</td>
                <td align="left" width="120">
                    <%=ShowBikesList("R90/6%,R90S%,R100%")%>
                </td>
            </tr>                                    
        </table>
    </div>

    <div id="MainGroupsMenu" style="position: relative; display: none;">
        <table cellpadding="2" cellspacing="0" style="">
            <tr>
                <td align="center" style="font-size: 15px;" colspan="5">
                    <label style="cursor: pointer;" onclick="MainGroupClicked('Main Group: All','');">All Groups</label><br />
                </td>
            </tr>
            <tr>
                <td align="center" style="font-size: 15px; color: #888;">Fiche</td>
                <td style="width: 2px; border-right: 1px dotted #888;">&nbsp;</td>
                <td align="center" style="font-size: 15px; color: #888;">Catalog</td>
                <td style="width: 2px; border-right: 1px dotted #888;">&nbsp;</td>
                <td align="center" style="font-size: 15px;">&nbsp;</td>
            </tr>                
            <tr valign="top">
                <td style="padding-left: 20px; padding-right: 20px;">
                    <table cellpadding="2" cellspacing="0" style="">
                        <tr>                                                                                       
                            <td style="vertical-align: top;" align="left">
                                <label style="cursor: pointer;" onclick="MainGroupClicked('01-Literature','01');">01-Literature</label><br />
                                <label style="cursor: pointer;" onclick="MainGroupClicked('11-Engine','11');">11-Engine</label><br />
                                <label style="cursor: pointer;" onclick="MainGroupClicked('12-Engine Electrics','12');">12-Engine Electrics</label><br />
                                <label style="cursor: pointer;" onclick="MainGroupClicked('13-Fuel Preparation','13');">13-Fuel Preparation</label><br />
                                <label style="cursor: pointer;" onclick="MainGroupClicked('16-Fuel Supply','16');">16-Fuel Supply</label><br />
                                <label style="cursor: pointer;" onclick="MainGroupClicked('17-Cooling','17');">17-Cooling</label><br />
                                <label style="cursor: pointer;" onclick="MainGroupClicked('18-Exhaust System','18');">18-Exhaust System</label><br />
                                <label style="cursor: pointer;" onclick="MainGroupClicked('21-Clutch','21');">21-Clutch</label><br />
                                <label style="cursor: pointer;" onclick="MainGroupClicked('22-Engine &amp; Transmission','22');">22-Engine &amp; Transmission</label>                                    
                            </td>
                            <td style="width: 2px; border-right: 1px dotted #888;">&nbsp;</td><td style="width: 2px;">&nbsp;</td>
                            <td style="vertical-align: top;" align="left">
                                <label style="cursor: pointer;" onclick="MainGroupClicked('23-Transmission','23');">23-Transmission</label><br />
                                <label style="cursor: pointer;" onclick="MainGroupClicked('26-Drive Shaft','26');">26-Drive Shaft</label><br />
                                <label style="cursor: pointer;" onclick="MainGroupClicked('27-Chain Drive','27');">27-Chain Drive</label><br />
                                <label style="cursor: pointer;" onclick="MainGroupClicked('31-Front Suspension','31');">31-Front Suspension</label><br />
                                <label style="cursor: pointer;" onclick="MainGroupClicked('32-Steering','32');">32-Steering</label><br />
                                <label style="cursor: pointer;" onclick="MainGroupClicked('33-Rear Axle &amp; Suspension','33');">33-Rear Axle &amp; Suspension</label><br />
                                <label style="cursor: pointer;" onclick="MainGroupClicked('34-Brakes','34');">34-Brakes</label><br />
                                <label style="cursor: pointer;" onclick="MainGroupClicked('35-Pedals','35');">35-Pedals</label>
                            </td>
                            <td style="width: 2px; border-right: 1px dotted #888;">&nbsp;</td><td style="width: 2px;">&nbsp;</td>
                            <td style="vertical-align: top;" align="left">
                                <label style="cursor: pointer;" onclick="MainGroupClicked('36-Wheels','36');">36-Wheels</label><br />
                                <label style="cursor: pointer;" onclick="MainGroupClicked('46-Frame, Fairing, Cases','46');">46-Frame, Fairing, Cases</label><br />
                                <label style="cursor: pointer;" onclick="MainGroupClicked('51-Vehicle Trim','51');">51-Vehicle Trim</label><br />
                                <label style="cursor: pointer;" onclick="MainGroupClicked('52-Seat','52');">52-Seat</label><br />
                                <label style="cursor: pointer;" onclick="MainGroupClicked('61-Electrical System','61');">61-Electrical System</label><br />
                                <label style="cursor: pointer;" onclick="MainGroupClicked('62-Instruments Dash','62');">62-Instruments Dash</label><br />
                                <label style="cursor: pointer;" onclick="MainGroupClicked('63-Lighting','63');">63-Lighting</label>
                            </td>
                            <td style="width: 2px; border-right: 1px dotted #888;">&nbsp;</td><td style="width: 2px;">&nbsp;</td>
                            <td style="vertical-align: top;" align="left">
                                <label style="cursor: pointer;" onclick="MainGroupClicked('65-GPS, Alarm &amp; Radios','65');">65-GPS, Alarm &amp; Radios</label><br />
                                <label style="cursor: pointer;" onclick="MainGroupClicked('71-Equipment Parts','71');">71-Equipment Parts</label><br />
                                <label style="cursor: pointer;" onclick="MainGroupClicked('72-Riders Equipment','72');">72-Riders Equipment</label><br />
                                <label style="cursor: pointer;" onclick="MainGroupClicked('77-Accessories','77');">77-Accessories</label><br />
                            </td>                
                        </tr>
                    </table>
                </td>
                <td style="width: 2px; border-right: 1px dotted #888;">&nbsp;</td>
                <td style="padding-left: 15px; padding-right: 15px;">
                    <label style="cursor: pointer;" onclick="MainGroupClicked('Performance','PE');">Performance</label><br />
                    <label style="cursor: pointer;" onclick="MainGroupClicked('Exhaust','EX');">Exhaust</label><br />
                    <label style="cursor: pointer;" onclick="MainGroupClicked('Suspension','SU');">Suspension</label><br />
                    <label style="cursor: pointer;" onclick="MainGroupClicked('Electrical','EL');">Electrical</label><br />
                    <label style="cursor: pointer;" onclick="MainGroupClicked('Lighting','LI');">Lighting</label><br />
                    <label style="cursor: pointer;" onclick="MainGroupClicked('Fairing','FA');">Fairing</label><br />
                    <label style="cursor: pointer;" onclick="MainGroupClicked('Service','ST');">Service &amp; Tools</label><br />
                    <label style="cursor: pointer;" onclick="MainGroupClicked('Other','OT');">Other</label>
                </td>
                <td style="width: 2px; border-right: 1px dotted #888;">&nbsp;</td>
                <td style="padding-left: 35px; font-size: 15px;">
                    <br />
                    <label style="cursor: pointer;" onclick="MainGroupClicked('Apparel','76');">Apparel</label><br />
                    <br /><br />
                    <label style="cursor: pointer;" onclick="MainGroupClicked('Tires','WT');">Tires</label>
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
                            <td id="ApplicationLabel">Specials &amp; Clearance Parts</td>
                            <td align="left" id="ProductsCount">&nbsp;</td>
                        </tr>
                    </table>                    
                </td>
            </tr>
        </table>
        <div style="width: 100%; height: 5px; background-color: #eb3f3c;"></div>
    </div>

    <div id="Loading" style="display: block; position: fixed; top: 30%; left: 40%; background-color: #fff; border: 3px solid #999; width: 220px; height: 130px; text-align: center; vertical-align: middle;">
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

    <div id="ProductsResults" class="ProductsResults">
        <table cellpadding="0" cellspacing="0" border="0">
            <tr>
                <td valign="top">
                    <div class="FiltersBox" style="width: 250px;margin-top: 0px;"> 
                        <label style="color: #333; font-size: 13px; font-weight: bold;">
                            Part Numbers:
                        </label>                            
                        <br />
                        <textarea id="PARTS" cols="20" rows="5" style="border: 1px solid #666; width: 200px;"><%=m_parts %></textarea>
                        <br />    
                        <br />        
                        <div id="ModelsFilter" class="ComboBox" style="width: 200px;">
                            <label class="ComboBoxText"></label><label class="ComboBoxX">x</label>
                            <input class="ComboBoxDefault" type="hidden" />
                            <input id="comboBoxModels" class="ComboBoxInput" type="hidden" />
                            <div style="position: relative;" class="ComboBoxButton"></div>
                            <div id="ComboBoxDropdownBikeModels" class="ComboBoxDropdown" style="width: 920px; max-height: 320px; height: 320px; padding: 10px;">
                                <!-- Bike Models menu goes in here dynamically -->
                            </div>
                        </div>
                        <br />
                        <div id="MainGroupsFilter" class="ComboBox" style="width: 200px;">
                            <label class="ComboBoxText"></label><label class="ComboBoxX">x</label>
                            <input class="ComboBoxDefault" type="hidden" />
                            <input id="comboBoxMG" class="ComboBoxInput" type="hidden" />
                            <div style="position: relative;" class="ComboBoxButton"></div>
                            <div id="ComboBoxDropdownMainGroups" class="ComboBoxDropdown" style="width: 950px; max-height: 180px; height: 180px; padding: 8px;">
                                <!-- Main Groups menu goes in here dynamically -->
                            </div>
                        </div>
                        <br />                        
                        <span class="Button" style="background-image: url('images/GoRedSmall.png'); background-repeat: no-repeat; background-position: 90px; padding-right: 24px; margin-right: 5px; position: absolute; left: 109px;" onclick="UpdateResults();">&nbsp;&nbsp;Apply Filter&nbsp;</span>
                        <br />
                    </div>
                </td>
                <td valign="top" align="center" style="color: #333;">
                    <%//=ShowParts() %>                                                   
                </td>
            </tr>        
        </table>
    </div>
    <br /> 
    <div id="ClearFooter"></div>
</div>   
 
<uc:UC_Footer runat="server" />   

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
            $(this).parent().children('.ComboBoxText').hide();
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

        $("#ComboBoxDropdownBikeModels").append($("#BikeModelsMenu")); // dynamically place the Bike Models Menu inside of the DIV ComboBoxDropdownBikeModels
        $("#ModelsFilter").css("display", "block");
        $("#BikeModelsMenu").css("display", "block");

        $("#ComboBoxDropdownMainGroups").append($("#MainGroupsMenu")); // dynamically place the Main Groups Menu inside of the DIV ComboBoxDropdownMainGroups
        $("#MainGroupsFilter").css("display", "block");
        $("#MainGroupsMenu").css("display", "block");

        $("#HeaderBarTopArrow").show();

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
            combobox.children(".ComboBoxText").text('Bike Models: All Models');
        }

        combobox = $("#comboBoxMG").parent();
        var mg = currentParams['mg'];
        var mgdescription = "";
        if (mg != null && mg != "") {
            mgdescription = "<%=GetMGDescription(m_MG) %>";
            combobox.children('.ComboBoxText').text(mgdescription);
            combobox.children('.ComboBoxText').css('background-color', '#ccc');
            combobox.children('.ComboBoxX').show();
            combobox.children('.ComboBoxInput').val(mg);
        } else {
            combobox.children(".ComboBoxText").text('Main Group: All');
        }

        $('#ProductsCount').text($('#ProductsCountInvisible').text() + (model != "" ? "- filtered by " + model : ""));

        $('.ComboBoxText').show();

        // For the Parts zoom
        $(".PartThumbnailImg").click(function () {
            var bigSrc = $(this).attr('src');
            bigSrc = bigSrc.replace("_N.", "_B.");
            bigSrc = bigSrc.replace("_T.", "_B.");
            bigSrc = bigSrc.replace("DiagramsThumb", "Diagrams");

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

        $('#Loading').hide();
    });
</script>
</body>
</html>