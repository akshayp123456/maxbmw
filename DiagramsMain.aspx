<%@ Page Language="C#" AutoEventWireup="true" CodeFile="DiagramsMain.aspx.cs" Inherits="DiagramsMain" %>
<%@ Register TagPrefix="uc" TagName="UC_MainMenu" Src="~/UC_MainMenu.ascx" %>

<!DOCTYPE html>
<html>
<head>
    <title>MAX BMW Motorcycles - BMW Parts & Technical Diagrams - <%=m_model %></title>
    <meta http-equiv="Pragma" content="no-cache" />
    <meta http-equiv="CACHE-CONTROL" content="NO-CACHE" />
    <meta http-equiv="Expires" content="0" />
    <meta name="description" content="BMW Parts for your <%=m_model %> <%=m_model2%>" />
    <meta name="rating" content="general" />
    <meta name="author" content="MAX BMW Motorcycles Parts" /> 
    <meta name="apple-mobile-web-app-capable" content="yes" />
    <meta name="apple-mobile-web-app-status-bar-style" content="black" />   
    <meta name="format-detection" content="telephone=no" />
   
    <script type="text/javascript">
        try
        {
            if (top != self) {
                top.location.replace(window.location.href);
            }
        } catch (e) { }
    </script>

    <script type="text/javascript" src="js/ie6no.js" charset="utf-8"></script>
    <script type="text/javascript">
        ie6no({ runonload: true });
    </script>

    <link href="css/Global.css" rel="stylesheet" type="text/css" media="all" />
    <link href="css/MainMenu.css" rel="stylesheet" type="text/css" media="all" />
    <link href="css/ProductsDetails.css" rel="stylesheet" type="text/css" media="all" />
    <link href="css/jquery.jscrollpane.css" rel="stylesheet" type="text/css" media="all" />
    <link href="css/jquery.jscrollpane.smallArrows.css" rel="stylesheet" type="text/css" media="all" />

    <style type="text/css">
		.scroll-pane-left
		{
		    height: 387px;
		    width: 421px;
			overflow: auto;
			-moz-border-radius: 4px;
	        -webkit-border-radius: 4px;
	        border-radius: 4px;
		}
		.scroll-pane-right
		{
		    height: 387px;
		    width: 100%;
			overflow: auto;
			-moz-border-radius: 4px;
	        -webkit-border-radius: 4px;
	        border-radius: 4px;
		}

        A.MainGroupLink:link {color: #000; text-decoration: none;}
        A.MainGroupLink:visited {color: #000; text-decoration: none;}
        A.MainGroupLink:active {color: #000; text-decoration: none;}
        A.MainGroupLink:hover {color: #cc3333; text-decoration: none;}
        
        #MenuMainGroups 
        {
            width: 100%;
            border: 0px;
            font-size: 10px;
            font-weight:bold;
            background-color: #fafafa;
            border: 1px solid #ccc;
            border-top: 0px;
            -ms-filter:"progid:DXImageTransform.Microsoft.Alpha(Opacity=90)"; 
            filter: alpha(opacity=90);
            -moz-opacity: 0.9;
            -khtml-opacity: 0.9;
            opacity: 0.9;
	        -moz-border-radius-bottomleft: 4px;
            -moz-border-radius-bottomright: 4px;
	        -webkit-border-bottom-left-radius: 4px;
	        -webkit-border-bottom-right-radius: 4px;
	        border-bottom-left-radius: 4px;
	        border-bottom-right-radius: 4px;
        }
        
        td.LeftPanel
        {
            background-color: #fff;
            -moz-border-radius: 4px;
	        -webkit-border-radius: 4px;
	        border-radius: 4px;
        }
        td.RightPanel
        {
            background-color: #fff;
            -moz-border-radius: 4px;
	        -webkit-border-radius: 4px;
	        border-radius: 4px;
            width: 100%;
        }
    </style>


    <script src="js/jquery-1.7.1.min.js"></script>    
    <script type="text/javascript" src="js/jquery.mousewheel.js"></script>
    <script type="text/javascript" src="js/jquery.jscrollpane.min.js"></script>


    <script type="text/javascript">
    $(function () {
        $('.scroll-pane-left').jScrollPane({ showArrows: true, reinitialiseOnImageLoad: true });
    });

    var AdjustHeightIntervalId = null;

    function SelectText(spanelement) {
        var span = spanelement;

        var userSelection;
        if (window.getSelection) {
            var range = document.createRange();
            range.setStartBefore(span.firstChild);
            range.setEndAfter(span.lastChild);
            var sel = window.getSelection();
            sel.removeAllRanges();
            sel.addRange(range); 
        }
        else if (document.selection) { // should come last; Opera!
         //   
        }       
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

    function loadRightPanelContainer(docIframe) {
        var pane = $('.scroll-pane-right');
        var api = pane.data('jsp');
        
        if (api != null) {            
            api.destroy();
        }

        $('#RightPanelContainer').html(docIframe.getElementById('RightPanelContainer').innerHTML);
        $('.scroll-pane-right').jScrollPane({ showArrows: true, maintainPosition: false });

        AdjustHeightIntervalId = setInterval(
		    function () {
		        AdjustHeight();
		    },
		    300
	    );

        SetRightPanelEvents();
    }

    function ScrollToParts() {
        var pane = $('.scroll-pane-right');
        var api = pane.data('jsp');
        api.scrollToY(360, 'slow');
    }

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

    function PrintElem(elem) {
        var mywindow = window.open('about:blank', 'mydiv', 'height=400,width=600');
        mywindow.document.write('<html><head><title>Print</title>');
        mywindow.document.write('</head><body style="font-face: Arial;">');
        mywindow.document.write($(elem).html());
        mywindow.document.write('</body></html>');
        mywindow.print();
        mywindow.document.close();
        return true;
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

        w.css({ zIndex: "101", position: "fixed", left: ($(window).width() - wwidth) / 2}).show();

        w.children('.ButtonClose').click(function () {
            w.hide();
            $(".ModalDialog").remove();
        });
    }

    function AnimateToMG(MG) {
        var pane = $('.scroll-pane-left');
        var api = pane.data('jsp');
        var anchor = $('#A_' + MG);
        if (anchor != null && api != null) {
            var pos = anchor.position().top;
            api.scrollToY(pos - 20, 'slow');
        }
    }

    function AdjustHeight() {
        if (AdjustHeightIntervalId != null) {
            clearInterval(AdjustHeightIntervalId);
        }

        var top = $('#MenuMainGroups').position().top + $('#MenuMainGroups').height();
        $('.scroll-pane-left').height($(window).height() - top - 10);
        $('.scroll-pane-right').height($(window).height() - top - 10);

        var paneLeft = $('.scroll-pane-left');
        var apiL = paneLeft.data('jsp');
        if (apiL != null) {
            apiL.reinitialise();
        }

        var paneRight = $('.scroll-pane-right');
        var apiR = paneRight.data('jsp');
        if (apiR != null) {
            apiR.reinitialise();
        }
    }

    // Callback function for the onload of the image
    function ResizeZoomed(ID) {
        var big = $('#' + ID + '>.BigImg');

        $('#' + ID).css({
            position: 'fixed',
            width: big.width(),
            height: big.height(),
            left: ($(window).width() - big.width()) / 2,
            top: 5
        });

        $('.BottonClose').css('right', '5px');
    }

    function SetRightPanelEvents() {
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

        $('.ProductThumbnailImg').click(function () {
            $('.ProductThumbnailImg').removeClass('Selected');
            $('.ProductThumbnailTopBar').removeClass('Selected');
            $(this).addClass('Selected');

            var index = $('.ProductThumbnailImg').index($(this));            
            $('.ProductThumbnailTopBar').eq(index).addClass('Selected');

            var thumbSrc = $(this).attr('src');
            // change from N to B (nano to big)
            // change from T to M (thumbnail to medium)
            var largeSrc = thumbSrc.replace("_N.", "_B.").replace("_T.", "_M.");
            $('#ImgProduct').attr('src', largeSrc);
        });       
        
        // For the Product zoom
        $("#ButtonZoomIn").click(function () {
            var bigSrc = $("#ImgProduct").attr('src').replace("DiagramsMid", "Diagrams");
            $('#DIVZoomedProduct>.BigImg').attr('src', bigSrc);

            ShowModal('DIVZoomedProduct');        
        });

        // For the Parts zoom
        $(".PartThumbnailImg").click(function () {
            var bigSrc = $(this).attr('src').replace("_N.", "_B.");
            $('#DIVZoomedPart>.BigImg').attr('src', bigSrc);
                              
            ShowModal('DIVZoomedPart');
        });


        $(".Slider").click(function () {
            var isNext = $(this).hasClass('Next');

            // Products sliders handler
            var allThumbs = $('.ProductThumbnailImg');
            var Thumb;
            if (allThumbs.size() > 1) {
                allThumbs.each(function (index, value) {
                    if ($(this).hasClass('Selected')) {
                        if (isNext) {
                            Thumb = allThumbs[index + 1];
                            if (Thumb == null)
                                Thumb = allThumbs[0];
                        } else {
                            Thumb = allThumbs[index - 1];
                            if (Thumb == null)
                                Thumb = allThumbs[allThumbs.size() - 1];
                        }
                    }
                });

                Thumb.click();

                // Check if Large button clicked
                // if so, also change the zoomed image (ImgProductBig) and the sizes/positions
                if ($(this).hasClass('Large')) {
                    var bigSrc = $("#ImgProduct").attr('src').replace("_M.", "_B.");
                    $('#DIVZoomedProduct>.BigImg').attr('src', bigSrc);    
                }

                // Readjust the scroll-pane-right
                var pane = $('.scroll-pane-right');
                var api = pane.data('jsp');
                if (api != null)
                    api.destroy();
                                    
                $('.scroll-pane-right').jScrollPane({ showArrows: true });
            }
        });

        // check url for partnumber, if present, then animate to it and make its background red
        var currentParams = getUrlVars(window.location.href);
        var partnumber = currentParams['partnumber'];
                
        if (partnumber != null && partnumber != "") {
            $('.PartNumber').each(function (index, value) {
                var pn = $(this).text();
                if (pn.length > 1 && pn.substr(1, pn.length - 1).replace(/ /gi, '') == partnumber) {
                    $(this).css("background-color", "#f88");
                    var pane = $('.scroll-pane-right');
                    var api = pane.data('jsp');
                    var pos = $(this).position().top;
                    api.scrollToY(pos - 20, 'slow');
                }
            });
        }
    }

    window.onresize = AdjustHeight;
    </script>

    <script type="text/javascript">
        // functions for the shopping cart AddCart page (that runs in the IFRAME)
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
        <table cellpadding="0" cellspacing="0" border="0" class="NavigationBar">
            <tr>
                <td width="220">
                    <span class="BackButton Up" onclick="window.location='fiche.aspx';">
                        Main Menu    
                    </span>
                    <span class="BackButton" onclick="<%=m_BackButtonOnClick %>">
                        <%=m_BackButtonCaption %>     
                    </span>           
                </td>
                <td class="BarSeparator">&nbsp;</td>
                <td align="center">
                    <table cellpadding="0" cellspacing="0" style="margin-left: auto; margin-right: auto;">
                        <tr>
                            <td><img src="images/IconFiche.png" /></td>
                            <td>&nbsp;<%=m_model %>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<%=m_model2.Replace("Production: ","") %></td>
                        </tr>
                    </table>                    
                </td>
            </tr>
            <tr>
                <td colspan="3" style="height: 5px; background-color: #eb3f3c;"><img src="images/1pixel.gif" style="display: block;" /></td>
            </tr>
        </table>        
                       
        <table id="MenuMainGroups" cellpadding="0" cellspacing="3">
            <tr>                                              
                <td align="center" style="width: 180px; height: 100px; vertical-align: middle;">
                    <%if (m_vid != "") {%>
                    <h1 style="font-size: 11px; font-weight: bold; margin: 0px; padding: 0px;"><%=m_model%></h1>
                    <label style="font-size: 10px; font-weight: bold;"><%=m_model2%></label>                                                    
                    <br />                                              
                    <img alt="" src="VehiclesIcons/<%=m_VehicleIcon %>" onerror="this.onerror=null;this.src='images/1pixel.gif';" />                                                                            
                    <%} %>                                                                                       
                </td>
                <td align="center" valign="bottom" style="font-size: 11px; font-weight: bold;">   
                    <table cellpadding="2" cellspacing="0" style="">
                        <tr>                                                                                       
                            <td style="vertical-align: top;" align="left">
                                <%=ShowMenuItem("01", "01-Literature", "")%>
                                <%=ShowMenuItem("02", "02-Service Items", "")%>
                                <%=ShowMenuItem("11", "11-Engine", "")%>
                                <%=ShowMenuItem("12", "12-Engine Electrics", "")%>
                                <%=ShowMenuItem("13", "13-Fuel Preparation", "")%>
                                <%=ShowMenuItem("16", "16-Fuel Supply", "")%>
                                <%=ShowMenuItem("17", "17-Cooling", "")%>
                                <%=ShowMenuItem("18", "18-Exhaust System", "")%>
                                <%=ShowMenuItem("21", "21-Clutch", "")%>
                                <%=ShowMenuItem("22", "22-Engine &amp; Transmission", "")%>                                              
                            </td>
                            <td style="width: 2px; border-right: 1px dotted #ccc;">&nbsp;</td><td style="width: 2px;">&nbsp;</td>
                            <td style="vertical-align: top;" align="left">
                                <%=ShowMenuItem("23", "23-Transmission", "")%>
                                <%=ShowMenuItem("24", "24-Transmission", "")%>
                                <%=ShowMenuItem("25", "25-Gearshift", "")%>
                                <%=ShowMenuItem("26", "26-Drive Shaft", "")%>
                                <%=ShowMenuItem("27", "27-Chain Drive", "")%>
                                <%=ShowMenuItem("31", "31-Front Suspension", "")%>
                                <%=ShowMenuItem("32", "32-Steering", "")%>
                                <%=ShowMenuItem("33", "33-Rear Axle &amp; Suspension", "")%>
                                <%=ShowMenuItem("34", "34-Brakes", "")%>
                                <%=ShowMenuItem("35", "35-Pedals", "")%>                                                                                                
                            </td>
                            <td style="width: 2px; border-right: 1px dotted #ccc;">&nbsp;</td><td style="width: 2px;">&nbsp;</td>
                            <td style="vertical-align: top;" align="left">
                                <%=ShowMenuItem("36", "36-Wheels", "")%>
                                <%=ShowMenuItem("41", "41-Bodywork", "")%>
                                <%=ShowMenuItem("46", "46-Frame, Fairing, Cases", "")%>
                                <%=ShowMenuItem("51", "51-Vehicle Trim", "")%>
                                <%=ShowMenuItem("52", "52-Seat", "")%>
                                <%=ShowMenuItem("54", "54-Roof &amp; Top", "")%>
                                <%=ShowMenuItem("61", "61-Electrical System", "")%>
                                <%=ShowMenuItem("62", "62-Instruments Dash", "")%>
                                <%=ShowMenuItem("63", "63-Lighting", "")%>
                            </td>
                            <td style="width: 2px; border-right: 1px dotted #ccc;">&nbsp;</td><td style="width: 2px;">&nbsp;</td>
                            <td style="vertical-align: top;" align="left">
                                <%=ShowMenuItem("71", "71-Equipment Parts", "")%>
                                <%=ShowMenuItem("72", "72-Riders Equipment", "")%>
                                <%=ShowMenuItem("77", "77-Accessories", "")%>
                                <br />
                                <%=ShowServiceSchedules() %>
                            </td>
                            <td style="width: 2px; border-right: 1px dotted #ccc;">&nbsp;</td><td style="width: 8px;">&nbsp;</td>
                            <td style="vertical-align: middle; text-align: right;">
                                <a class="MainGroupLink" style="font-size: 12px;" href="DiagramsMain.aspx?rnd=<%=m_rnd%>&vid=<%=m_vid %>">Specials</a>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<br />
                                <br />                               
                                <span class="ButtonForward" onclick="window.location='PartsCatalog.aspx<%=(m_vid!=""? "?vid=" + m_vid : "") %>';">Parts Catalog</span><br />
                                <span class="ButtonForward" onclick="window.location='PartsTires.aspx<%=(m_vid!=""? "?vid=" + m_vid : "") %>';">Tires</span><br />
                                <span class="ButtonForward" onclick="window.location='PartsApparel.aspx<%=(m_vid!=""? "?vid=" + m_vid : "") %>';">Apparel</span><br />
                                <br />                                 
                                <span class="LinkNewWindow" onclick="window.open('FAQ.pdf', '_blank', 'menubar=no,toolbar=no,scrollbars=yes');">See our FAQ</span>
                            </td>
                        </tr>
                    </table>                            
                </td>
            </tr> 
        </table>

        <div><img src="images/1pixel.gif" style="display: block" height="5" /></div>

        <table cellpadding="0" cellspacing="0" width="100%" style="background-color: transparent;">
            <tr style="vertical-align: top;">
                <td class="LeftPanel">
                    <div class="scroll-pane-left">
			            <table cellspacing="0" cellpadding="0" width="100%">
                            <%=ShowDiagramsLeft()%>
                        </table>
                    </div>
                </td>
                <td width="7">&nbsp;&nbsp;</td>
                <td class="RightPanel">
                    <div class="scroll-pane-right" id="RightPanelContainer">
                        <!-- content will be dynamically loaded using jQuery -->
                    </div>
                </td>
            </tr>
        </table>
    </div>

    <div id="DIVZoomedProduct" class="DIVZoomed">
        <div class="ButtonClose" title="Close"></div>        
        <img src="" class="BigImg" height="550" onLoad="ResizeZoomed('DIVZoomedProduct');" />
    </div>

    <div id="DIVZoomedPart" class="DIVZoomed">
        <div class="ButtonClose" title="Close"></div>        
        <img src="" class="BigImg" height="550" onLoad="ResizeZoomed('DIVZoomedPart');" />
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
    
    <iframe id="IFRAMERIGHTPANEL" src="<%=(m_Diagram == "" ? "DiagramsSpecials.aspx?rnd=" + m_rnd + "&vid=" + m_vid : "DiagramsRight.aspx?rnd=" + m_rnd + "&vid=" + m_vid + "&diagram=" + m_Diagram) %>" style="display:none;"></iframe>

    <div id="ShoppingCart" class="ShoppingCart">        
        <div class="ButtonClose" title="Close"></div>
        <div id="CartContainer"></div>
    </div>       

    <iframe id="CARTFRAME" src="about:blank" style="display: none;"></iframe>

    <script type="text/javascript">
        $(document).ready(function () {
            var currentParams = getUrlVars(window.location.href);

            var diagram = currentParams['diagram'];

            if (diagram != null && diagram != "") {

                var diagramByID = $('#Diagram' + diagram);
                if (diagramByID != null) {
                    try {
                        var pane = $('.scroll-pane-left');
                        var api = pane.data('jsp');
                        var pos = diagramByID.position().top;

                        api.scrollToY(pos - 20, 'slow');

                        diagramByID.css('border', '2px dotted #cc3333');
                    }
                    catch (err) {
                    }
                }
            }
        });
    </script>  
</body>
</html>
