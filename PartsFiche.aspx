<%@ Page Language="C#" AutoEventWireup="true" CodeFile="PartsFiche.aspx.cs" Inherits="PartsFiche" %>
<%@ Register TagPrefix="uc" TagName="UC_MainMenu" Src="~/UC_MainMenu.ascx" %>
<%@ Register TagPrefix="uc" TagName="UC_ButtonsMenu" Src="~/UC_ButtonsMenu.ascx" %>
<%@ Register TagPrefix="uc" TagName="UC_Footer" Src="~/UC_Footer.ascx" %>

<!DOCTYPE html>
<html>
<head>
    <title>BMW Motorcycle Parts | Catalog Parts Online | MAX BMW Motorcycles</title>
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
    
    <script src="js/jquery-1.7.1.min.js"></script>

    <link href="css/Global.css" rel="stylesheet" type="text/css" media="all" />
    <link href="css/MainMenu.css" rel="stylesheet" type="text/css" media="all" />
    <link href="css/ButtonsMenu.css" rel="stylesheet" type="text/css" media="all" /> 

    <style>
    div.FicheMenu
    {
        padding: 10px;
        background-color: #fff;
        border-top: 0px;
        border-left: 1px solid #ccc;
        border-right: 1px solid #ccc;
        border-bottom: 1px solid #ccc;
        -moz-border-radius-bottomleft: 4px;
	    -webkit-border-bottom-left-radius: 4px;
	    border-bottom-left-radius: 4px;
        -moz-border-radius-bottomright: 4px;
	    -webkit-border-bottom-right-radius: 4px;
	    border-bottom-right-radius: 4px;        
    } 
    div.DottedDivider
    {
        background-image: url(images/3pixel.gif);
        background-repeat: repeat-x;
        width: 100%;
        height: 1px;
        margin-top: 4px;
        margin-bottom: 6px;
        margin-left: 0px;
        margin-right: 10px;
        -ms-filter:"progid:DXImageTransform.Microsoft.Alpha(Opacity=60)"; 
        filter: alpha(opacity=60);
        -moz-opacity: 0.6;
        -khtml-opacity: 0.6;
        opacity: 0.6;
    }   
    </style>

    <script type="text/javascript">
        function SetVehicleIcon(text1, text2, icon) {
            document.getElementById('VEHICLETEXT1').innerText = text1;
            document.getElementById('VEHICLETEXT2').innerText = 'Production: ' + text2;
            document.getElementById('VEHICLEICON').src = "VehiclesIcons/" + icon + ".jpg?v=<%=Utilities.VERSION %>";
        }
    
        function BikeModelClicked(model, vid) {
            // fiche menu is selected, so go to the DiagramsMain.aspx page with the vid
            $('#Loading').show();
            window.location = "DiagramsMain.aspx?vid=" + vid + "&rnd=" + "<%=Utilities.VERSION %>";
        }

        function ValidateVIN(vin) {
            if (vin.length == 0) {
                $('#VINRESULT').css('color', '#888');
                $('#VINRESULT').text('recommended for non-US models');
            } else if (vin.length != 7) {
                $('#VINRESULT').css('color', '#333');
                $('#VINRESULT').text('enter the last 7 digits of the VIN');
            } else if (vin.length == 7) {
                $('#VINRESULT').css('color', '#333');

                vin = vin.toUpperCase();
                $('#VIN').val(vin);

                $('#VINRESULT').text('Searching VIN...');

                // JSON call to inquery the VIN, test: ZP61627
                $.ajax({
                    type: "POST",
                    url: "ws/vin.asmx/GetBikeInfoFromVin",
                    data: "{'vin':'" + vin + "'}",
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (msg) {
                        if (msg.d.vid == null) {
                            $('#VINRESULT').css('color', '#d00');
                            $('#VINRESULT').text('no bike information was found for this VIN');
                        } else {
                            $('#VINRESULT').text(msg.d.info + '.  Loading fiche...');
                            var t = setTimeout("window.location='DiagramsMain.aspx?vid=" + msg.d.vid + "&vin=" + vin + "';", 2000);
                        }
                    }
                });
            }
            return true;
        }
    </script>
</head>
<body>

<div id="centered">    
    <uc:UC_MainMenu runat="server" />
    <uc:UC_ButtonsMenu runat="server" />
            
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
                            <td><img src="images/IconFiche.png" /></td>
                            <td id="ApplicationLabel">Fiche - Select a Bike Model</td>
                        </tr>
                    </table>                    
                </td>
            </tr>            
        </table>
        <div style="width: 100%; height: 5px; background-color: #eb3f3c;"></div>
    </div>

    <div class="FicheMenu">
        <div id="BikeModelsMenu" style="position: relative;">
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
            <table cellspacing="3" cellpadding="0" border="0" style="font-family: Verdana; font-size: 12px; font-weight: bold; color: #333;">
                <tr valign="middle">
                    <td align="right" width="510">
                        To search by the last 7 digits of your VIN, enter it here:&nbsp;
                    </td>
                    <td>
                        <input id="VIN" name="VIN" type="text" maxlength="7" style="border: 1px solid #333; width: 65px;" onkeyup="ValidateVIN(this.value);" />
                    </td>
                    <td>
                        <img src="images/ArrowLeft.gif" alt="" />
                    </td>
                    <td>
                        <span id="VINRESULT" style="color: #e33; font-weight: bold;">recommended for non-US models</span>
                    </td>
                </tr>
            </table>
            <div class="DottedDivider"></div>
            <table cellspacing="0" cellpadding="0" border="0" style="font-family: Verdana; font-weight: bold; color: #888;">
                <tr style="font-size: 15px;">
                    <td align="center" width="150">
                        F, G &amp; C Models
                    </td>                            
                    <td align="center" width="240">
                        K, S &amp; M Models
                    </td>          
                    <td align="center" width="570">
                        R Models              
                    </td>
                </tr>
            </table>
            <table cellspacing="0" cellpadding="0" border="0" style="font-family: Verdana; font-weight: bold; color: #888;">
                <tr style="font-size: 13px;">
                    <td width="390">
                        &nbsp;
                    </td>
                    <td align="center" width="150">
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
            <table id="TableBikeModels" cellspacing="0" cellpadding="0" border="0" style="font-family: Tahoma; font-weight: bold; font-size: 11px;">
                <tr valign="top">
                    <td align="left" width="150">
                        <%=ShowBikesList("F650 Funduro")%>
                        <%=ShowBikesList("F650ST%")%>
                        <%=ShowBikesList("F650CS%")%>
                        <%=ShowBikesList("F650GS%")%>
                        <%=ShowBikesList("F700%")%>
                        <%=ShowBikesList("F750%")%>
                        <%=ShowBikesList("F800%")%>
                        <%=ShowBikesList("F850%")%>
                        <%=ShowBikesList("F900%")%>
                        <br />
                        <%=ShowBikesList("G650%") %>
                        <%=ShowBikesList("G310%") %>
                        <br />
                        <%=ShowBikesList("CE%")%>
                        <%=ShowBikesList("C6%")%>
                        <%=ShowBikesList("C4%")%>
                        

                    </td>
                    <td width="6">&nbsp;</td>
                    <td width="6" style="border-left: 1px #999 dotted;">&nbsp;</td>
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
                        <%=ShowBikesList("S1000%")%>
                        <br />
                        <%=ShowBikesList("M1000%")%>
                        <br />
                        <%=ShowBikesList("HP4%")%>
                    </td>
                    <td width="6">&nbsp;</td>
                    <td align="left" width="95">                                                                                  
                        <%=ShowBikesList("K1,K100%,K75%")%>
                    </td>
                    <td width="6">&nbsp;</td>
                    <td width="6" style="border-left: 1px #999 dotted;">&nbsp;</td>
                    <td align="left" width="140">
                        <%=ShowBikesList("HP2%")%>
                        <%=ShowBikesList("R18%")%>
                        <%=ShowBikesList("R nineT%")%>
                        <%=ShowBikesList("R12 %")%>
                        <%=ShowBikesList("R1300%")%>
                        <%=ShowBikesList("R1250%")%>
                        <%=ShowBikesList("R1200GSW %")%>
                        <%=ShowBikesList("R1200GS %")%>
                        <%=ShowBikesList("R1200R %")%>
                        <%=ShowBikesList("R1200RT%")%>
                        <%=ShowBikesList("R1200RS")%>
                        <%=ShowBikesList("R1200S")%>
                        <%=ShowBikesList("R1200ST")%>
                        <%=ShowBikesList("R900RT%")%>
                    </td>
                    <td width="6">&nbsp;</td>
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
                    <td width="6">&nbsp;</td>
                    <td align="left" width="70">
                        <%=ShowBikesList("R24%,R25%,R26%,R27%,R50,R50US,R50/2,R50S,R51%,R60,R60US,R60/2,R67%,R68%,R69%")%>
                    </td>
                    <td width="6">&nbsp;</td>
                    <td valign="top" align="left" width="83">
                        <%=ShowBikesList("R45%,R50/5,R60/5,R60/6,R60/7,R65%,R75%,R80%")%>
                    </td>
                    <td width="4">&nbsp;</td>
                    <td align="left" width="112">
                        <%=ShowBikesList("R90/6%,R90S%,R100%")%>
                    </td>
                </tr>                                    
            </table>
        </div>
    </div>
    <br />
    <br />
    <br />
    <div id="ClearFooter"></div>
</div>   
<uc:UC_Footer runat="server" />

<div id="Loading" style="display: none; position: fixed; top: 30%; left: 40%; background-color: #fff; border: 3px solid #999; width: 220px; height: 130px; text-align: center; vertical-align: middle; z-index: 99999;">
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

<script type="text/javascript">
    $(document).ready(function () {
        $("td.ButtonMenu").hover(function () {
            $(this).addClass("hoverlook").siblings().removeClass("hoverlook");
        }, function () {
            $(this).removeClass("hoverlook");
        });

        $("#BikeModelsMenu").bind('mousemove', function (e) {
            var x = e.pageX - $("#BikeModelsMenu").offset().left + 30;
            var y = e.pageY - $("#BikeModelsMenu").offset().top - 18;

            if (x + $("#VEHICLETOOLTIP").width() > $("#BikeModelsMenu").width()) {
                $("#VEHICLETOOLTIP").css("background-image", "url(images/VehiclesTooltipRight.png)");
                $("#VEHICLETOOLTIP").css("padding-left", "5px");
                x = x - ($("#VEHICLETOOLTIP").width() + 45);
            } else {
                $("#VEHICLETOOLTIP").css("background-image", "url(images/VehiclesTooltipLeft.png)");
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

        $("#HeaderBarTopArrow").css("left", "92px");
        $("#HeaderBarTopArrow").show();

        $("#TableBikeModels").css("font-size", "11px");
    });
</script>
</body>
</html>
