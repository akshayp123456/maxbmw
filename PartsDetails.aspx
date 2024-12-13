<%@ Page Language="C#" AutoEventWireup="true" CodeFile="PartsDetails.aspx.cs" Inherits="PartsDetails" %>
<%@ Register TagPrefix="uc" TagName="UC_MainMenu" Src="~/UC_MainMenu.ascx" %>
<%@ Register TagPrefix="uc" TagName="UC_Footer" Src="~/UC_Footer.ascx" %>

<!DOCTYPE html>
<html>
<head>
    <title>MAX BMW Motorcycles - <%=m_imagetext %></title>
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
    <link href="css/ProductsDetails.css" rel="stylesheet" type="text/css" media="all" />

    <style>
        label.ModelX
        {
            cursor: pointer;
            padding-left: 5px;
            padding-right: 3px;
            color: #666;
        }    
        label.ModelX:hover
        {
            color: #d33;
        } 
        label.ModelsFits
        {
            background-color: #bbb;
            line-height: 16px;
            -moz-border-radius: 2px;
	        -webkit-border-radius: 2px;
	        border-radius: 2px;	               
        }      
    </style>

    <script type="text/javascript">
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

        function SetThisPageVIDFilter(vid) {
            var currentParams = getUrlVars(window.location.href);
            var source = currentParams['source'];
            if (source == null) source = '';

            var category = currentParams['category'];
            if (category == null) category = '';
            
            var brand = currentParams['brand'];
            if (brand == null) brand = '';

            var diagram = currentParams['diagram'];
            if (diagram == null) diagram = '';

            var viewmode = currentParams['viewmode'];
            if (viewmode == null) viewmode = '';

            // build the url with the parameters
            var params = '';
            params += 'source=' + source; // source always has to be present     
            params += (vid != '' ? '&vid=' + vid : '');
            params += (category != '' ? '&category=' + escape(unescape(category)) : '');
            params += (brand != '' ? '&brand=' + escape(unescape(brand)) : '');
            params += (diagram != '' ? '&diagram=' + escape(unescape(diagram)) : '');
            params += (viewmode != '' ? '&viewmode=' + viewmode : '');

            window.location = 'PartsDetails.aspx' + (params != '' ? '?' + params : '');
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
            <td title="<%=m_Diagram %>">
                <span class="BreadCrumb" onclick="window.location='<%=m_BackButtonURL %>';">
                    <%=m_BackButtonCaption %>              
                </span>
                <span class="BreadCrumbSeparator">
                    &#9679;
                </span>
                <span class="BreadCrumbLast">
                    <%=m_DiagramCaption %>              
                </span>  
            </td>
        </tr>
        <tr>
            <td colspan="4" style="height: 5px; background-color: #eb3f3c;"><img src="images/1pixel.gif" style="display: block;" /></td>
        </tr>
    </table>
    <table cellpadding="0" cellspacing="0" border="0" class="ProductDetails">
        <tr>
            <td width="447" valign="top">
                <div style="border: 2px solid #ddd; position: relative; text-align: center;">
                    <img width="443" id="ImgProduct" src="<%=m_image %>" />
                    <span id="ButtonZoomIn"></span>
                    <%if (m_ProductImages.Count > 0) { %>
                    <span class="Slider Prev"></span><span class="Slider Next"></span>
                    <%} %>
                </div>
                <%if (m_ProductImages.Count > 0) { %>
                <table cellspacing="0" cellpadding="0" class="Thumbnails">
                    <tr>
                        <td class="ProductThumbnailTopBar Selected"><img src="images/1pixel.gif" alt="" /></td><td><img src="images/1pixel.gif" alt="" /></td>
                        <%foreach (string imgPath in m_ProductImages) { %>
                            <td class="ProductThumbnailTopBar"><img src="images/1pixel.gif" alt="" /></td><td><img src="images/1pixel.gif" alt="" /></td>
                        <%} %>                      
                        <td><img src="images/1pixel.gif" alt="" /></td>
                    </tr>
                    <tr>
                        <td width="40"><img class="ProductThumbnailImg Selected" src="<%=m_image %>" alt="" /></td><td width="3"><img src="images/1pixel.gif" alt="" /></td>
                        <%foreach (string imgPath in m_ProductImages) { %>
                        <td width="40"><img class="ProductThumbnailImg" src="<%=imgPath %>" alt="" /></td><td width="3"><img src="images/1pixel.gif" alt="" /></td>
                        <%} %>                      
                        <td>&nbsp;</td>
                    </tr>
                </table>
                <%} %>

                <%if (m_ProductVideos.Count > 0) { %>
                <table cellspacing="0" cellpadding="0" class="Thumbnails">
                    <tr valign="middle">                        
                        <%foreach (string videoPath in m_ProductVideos) { %>
                        <td width="82"><img class="ThumbnailVideo" src="images/YouTubeVideos.jpg" alt="<%=videoPath %>" /></td>
                        <%} %>                      
                        <td>&nbsp;</td>
                    </tr>
                </table>
                <%} %>

                <%if (m_ProductPDFs.Count > 0) { %>
                <table cellspacing="0" cellpadding="0" class="Thumbnails">
                    <tr valign="middle">                        
                        <%foreach (string pdfPath in m_ProductPDFs) { %>
                        <td width="82"><img class="ThumbnailPDF" src="images/PDF.jpg" alt="<%=pdfPath %>" /></td>
                        <%} %>                      
                        <td>&nbsp;</td>
                    </tr>
                </table>
                <%} %>
                <div style="padding: 5px; font-size: 13px; color: #333;">             
			        <%=m_DiagramNoteSpecs %><br />
                    <%if (m_source == "apparel") {%>
                    <div style="border: 1px solid #666; background-color: #ffc; padding: 8px; width: 150px; text-align: center; vertical-align: middle; color: #333; font-size: 11px; font-weight: bold; cursor: pointer;" onclick="window.open('images/BMWSizing.jpg?v=<%=Utilities.VERSION %>', '_blank', 'menubar=no,toolbar=no,scrollbars=yes,width=1024');">Click here for Size Chart</div>
                    <%} %>               
                </div>                                                                                     
            </td>
            <td width="6">
                &nbsp;
            </td>
            <td valign="top" style="font-size: 11px;">
                <div style="padding: 5px; padding-top: 10px; font-size: 18px; font-weight: bold; color: #000;">
			        <%=m_DiagramCaption %>
                </div>
                <div style="padding: 5px; font-size: 14px; color: #333;">
			        <%=m_DiagramNoteComments %>
                </div>
                <br />
                <%=ShowParts() %>
            </td>
            <td width="6">
                &nbsp;
            </td>
        </tr>
    </table>
    <br />

    <div id="ClearFooter"></div>

</div>

<uc:UC_Footer runat="server" />

<div id="DIVZoomedProduct" class="DIVZoomed">
    <div class="ButtonClose" title="Close"></div>        
    <img src="" class="BigImg" height="550" onLoad="ResizeZoomed('DIVZoomedProduct');" />
    <%if (m_ProductImages.Count > 0) { %>
    <span class="Slider Prev Large"></span>
    <span class="Slider Next Large"></span>
    <%} %>
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

<div id="ShoppingCart" class="ShoppingCart">        
    <div class="ButtonClose" title="Close"></div>
    <div id="CartContainer"></div>
</div>       

<iframe id="CARTFRAME" src="about:blank" style="display: none;"></iframe>

<script type="text/javascript">
    $(document).ready(function () {
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
            var bigSrc = $("#ImgProduct").attr('src').replace("_M.", "_B.");
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

                if (Thumb!=null)
                    Thumb.click();

                // Check if Large button clicked
                // if so, also change the zoomed image (ImgProductBig) and the sizes/positions
                if ($(this).hasClass('Large')) {
                    var bigSrc = $("#ImgProduct").attr('src').replace("_M.", "_B.");
                    $('#DIVZoomedProduct>.BigImg').attr('src', bigSrc);                    
                }
            }
        });

        // check url for partnumber, if present, then animate to it and make its background red
        var currentParams = getUrlVars(window.location.href);
        var partnumber = currentParams['partnumber'];

        if (partnumber != null && partnumber != "") {
            $('.PartNumber').each(function (index, value) {
                var pn = $(this).text();
                if (pn.length > 1 && pn.substr(1, pn.length - 1) == partnumber) {
                    $(this).css("background-color", "#f88");
                    $('html,body').animate({ scrollTop: $(this).offset().top - 45 }, 'slow');
                }
            });
        }
    });
</script>
</body>
</html>
