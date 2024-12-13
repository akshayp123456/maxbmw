<%@ Control Language="C#" AutoEventWireup="true" CodeFile="UC_Parts_Body.ascx.cs" Inherits="UC_Parts_Body" %>
<%@ Register TagPrefix="uc" TagName="UC_MainMenu" Src="~/UC_MainMenu.ascx" %>
<%@ Register TagPrefix="uc" TagName="UC_ButtonsMenu" Src="~/UC_ButtonsMenu.ascx" %>
<%@ Register TagPrefix="uc" TagName="UC_Footer" Src="~/UC_Footer.ascx" %>

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
                    <%=ShowBikesList("R nineT")%>
                    <%=ShowBikesList("R1200GS 05%")%>
                    <%=ShowBikesList("R1200GS 08%")%>
                    <%=ShowBikesList("R1200GS 10%")%>
                    <%=ShowBikesList("R1200GSW 13%")%>
                    <%=ShowBikesList("R1200GS ADV%")%>
                    <%=ShowBikesList("R1200GSW ADV%")%>
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
                            <td><img src="images/Icon<%=m_source%>.png" /></td>
                            <td id="ApplicationLabel" style="text-transform: capitalize;"><%=m_source%></td>
                            <td id="ProductsCount">&nbsp;</td>
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
                    <div class="FiltersBox">
                        <table cellspacing="2" cellpadding="0" style="font-size: 12px; font-weight: bold;">
                            <tr style="vertical-align: middle;">
                                <td>View:&nbsp;</td>
                                <td>
                                    <img id="ViewModeThumbs" alt="Thumbnails View" title="Thumbnails View" src="images/ViewThumb.png" style="border: 2px #fff solid; cursor: pointer;" onclick="this.style.borderColor='#eb3f3c'; $('#ViewModeList').css('border-color','#333'); UpdateResults();" />
                                </td>
                                <td>
                                    Thumbnails
                                </td>
                                <td width="8">
                                    &nbsp;
                                </td>
                                <td>
                                    <img id="ViewModeList" alt="List View" title="List View" src="images/ViewDetails.png" style="border: 2px #fff solid; cursor: pointer;" onclick="this.style.borderColor='#eb3f3c'; $('#ViewModeThumbs').css('border-color','#333'); UpdateResults();" />
                                </td>
                                <td>
                                    List
                                </td>
                            </tr>
                        </table>
                        <br />
                        <label style="font-size: 13px; font-weight: bold;">
                            Narrow down by:
                        </label>        
                        <div id="ModelsFilter" class="ComboBox" style="width: 180px;">
                            <label class="ComboBoxText"></label><label class="ComboBoxX">x</label>
                            <input class="ComboBoxDefault" type="hidden" value="Models: All Models" />
                            <input id="comboBoxModels" class="ComboBoxInput" type="hidden" />
                            <div style="position: relative;" class="ComboBoxButton">
                            </div>
                            <div id="ComboBoxDropdownBikeModels" class="ComboBoxDropdown" style="width: 920px; max-height: 340px; height: 340px; padding: 10px;">
                                <!-- Bike Models menu goes in here dynamically -->
                            </div>
                        </div>
                        <br />
                        <div class="ComboBox" style="width: 180px;">
                            <label class="ComboBoxText"></label><label class="ComboBoxX">x</label>
                            <input class="ComboBoxDefault" type="hidden" value="Brands: All Brands" />
                            <input id="comboBoxBrands" class="ComboBoxInput" type="hidden" />
                            <div style="position: relative;" class="ComboBoxButton">
                            </div>
                            <div class="ComboBoxDropdown" style="height: 500px;">
                                <ul style="list-style-type: none; padding: 3px; margin: 0px; cursor: pointer;">
                                    <!-- populated by jquery -->
                                </ul>
                            </div>
                        </div>
                        <br />
                        <div class="ComboBox" style="width: 180px;">
                            <label class="ComboBoxText"></label><label class="ComboBoxX">x</label>
                            <input class="ComboBoxDefault" type="hidden" value="Categories: All Categories" />
                            <input id="comboBoxCategories" class="ComboBoxInput" type="hidden" />
                            <div style="position: relative;" class="ComboBoxButton">
                            </div>
                            <div class="ComboBoxDropdown">
                                <ul style="list-style-type: none; padding: 3px; margin: 0px; cursor: pointer;">
                                    <!-- populated by jquery -->
                                </ul>
                            </div>
                        </div>
                        <br />
                        <label style="color: #333; font-size: 13px; font-weight: bold;">
                            Categories Quick Links:<br />
                            <ul class="QuickLinks" style="font-size: 11px; list-style-type: none; padding: 3px; padding-left: 6px; margin: 0px; cursor: pointer;">
                            </ul>
                        </label>
                        <%if (m_source=="apparel") { %>
                        <br />
                        <span class="LinkNewWindow" onclick="window.open('images/BMWSizing.jpg?v=<%=Utilities.VERSION %>', '_blank', 'menubar=no,toolbar=no,scrollbars=yes');">Apparel Size Chart</span>
                        <%} %>                        
                    </div>
                </td>
                <td valign="top">
                     <%=ShowProductsList() %>
                </td>
            </tr>        
        </table>        
    </div>
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

<p id="back-top">
    <a href="#top"><span></span>Back to Top</a>
</p>


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
            //$(this).parent().children('.ComboBoxText').hide();
            $(this).prev().css('background-color', '#eee');
            $(this).parent().children('.ComboBoxDropdown').hide();
            $(this).hide();
            UpdateResults();
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

        var source = '<%=this.Page.ToString().ToLower().Replace("asp.parts","").Replace("_aspx","") %>';

        switch (source) {
            case 'catalog':
                $("#HeaderBarTopArrow").css("left", "282px");
                $("#ComboBoxDropdownBikeModels").append($("#BikeModelsMenu")); // dynamically place the Bike Models Menu inside of the DIV ComboBoxDropdownBikeModels
                $("#ModelsFilter").css("display", "block");
                $("#BikeModelsMenu").css("display", "block");

                // update Catalog Categories 
                var ul = $("#comboBoxCategories").parent().children(".ComboBoxDropdown").children("ul");
                ul.children().remove();
                ul.append("<li>Performance</li>");
                ul.append("<li>Exhaust</li>");
                ul.append("<li>Suspension</li>");
                ul.append("<li>Electrical</li>");
                ul.append("<li>Lighting</li>");
                ul.append("<li>Fairing</li>");
                ul.append("<li>Service &amp; Tools</li>");
                ul.append("<li>Other</li>");

                // copy to QuickLinks class
                $('.QuickLinks').children().remove();
                $('.QuickLinks').append(ul.children().clone());

                // update Catalog Brands
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
                //ul.append("<li>Wunderlich</li>");
                ul.append("<li>Wurth</li>");
                ul.append("<li>Yuasa</li>");
                break;

            case 'tires': // Tires
                $("#HeaderBarTopArrow").css("left", "475px");
                $("#ComboBoxDropdownBikeModels").append($("#BikeModelsMenu")); // dynamically place the Bike Models Menu inside of the DIV ComboBoxDropdownBikeModels
                $("#ModelsFilter").css("display", "block");
                $("#BikeModelsMenu").css("display", "block");

                // update tires Categories
                var ul = $("#comboBoxCategories").parent().children(".ComboBoxDropdown").children("ul");
                ul.children().remove();
                ul.append("<li>Front Tires</li>");
                ul.append("<li>Rear Tires</li>");

                // copy to QuickLinks class
                $('.QuickLinks').children().remove();
                $('.QuickLinks').append(ul.children().clone());

                // update tires Brands
                ul = $("#comboBoxBrands").parent().children(".ComboBoxDropdown").children("ul");
                ul.children().remove();
                ul.append("<li>Continental</li>");
                ul.append("<li>Metzeler</li>");
                ul.append("<li>Michelin</li>");
                break;

            case 'apparel': // Apparel
                $("#HeaderBarTopArrow").css("left", "670px");
                $("#ModelsFilter").css("display", "none");
                $("#BikeModelsMenu").css("display", "none");

                // update apparel Categories                    
                var ul = $("#comboBoxCategories").parent().children(".ComboBoxDropdown").children("ul");
                ul.children().remove();
                ul.append("<li>Suits, Jackets &amp; Pants</li>");
                ul.append("<li>Gloves</li>");
                ul.append("<li>Boots</li>");
                ul.append("<li>Clothing</li>");
                ul.append("<li>Thermal</li>");
                ul.append("<li>Heated</li>");
                ul.append("<li>Caps</li>");
                ul.append("<li>Storage</li>");
                ul.append("<li>Helmets</li>");
                ul.append("<li>Other</li>");

                // copy to QuickLinks class
                $('.QuickLinks').children().remove();
                $('.QuickLinks').append(ul.children().clone());

                // update apparel Brands
                ul = $("#comboBoxBrands").parent().children(".ComboBoxDropdown").children("ul");
                ul.children().remove();
                ul.append("<li>BMW</li>");
                ul.append("<li>MAX BMW</li>");
                ul.append("<li>Gerbing</li>");
                break;
        }

        $("#HeaderBarTopArrow").show();

        // update the li click function
        $('.ComboBoxDropdown li').click(function () {
            var newValue = $(this).text();
            $(this).parent().parent().parent().children('.ComboBoxText').text(newValue);
            $(this).parent().parent().parent().children('.ComboBoxText').css('background-color', '#ccc');
            $(this).parent().parent().parent().children('.ComboBoxX').show();
            $(this).parent().parent().parent().children('.ComboBoxInput').val(newValue);
            UpdateResults();
        });

        $('.QuickLinks li').click(function () {
            var newValue = $(this).text().replace(/\s+/g, '').replace('&', '');
            if (newValue.indexOf("Suits") == 0)
                newValue = "Suits";

            try {
                $('html,body').animate({ scrollTop: $("#QUICKLINKS_" + newValue).offset().top - 20 }, 'slow');
            }
            catch (e) {
            }

        });

        $("#TableBikeModels").css("font-size", "10px");

        // Get url parameters to set the current filters and position within the page         
        var currentParams = getUrlVars(window.location.href);

        // If a diagram is present, then scroll to it
        var diagram = currentParams['diagram'];

        if (diagram != null && diagram != "") {
            // for animating to an ID, the ID cannot contain special chars and starts with "Diagram_..."
            diagram = unescape(diagram); // removes the %20, etc from the URL encoding
            diagram = diagram.replace(/\s+/g, ''); // remove ' '
            diagram = diagram.replace(/\./g, ''); // remove '.'
            diagram = diagram.replace(/\+/g, ''); // remove '+'

            if ($("#Diagram_" + diagram) != null) {
                try {
                    $('html,body').animate({ scrollTop: $("#Diagram_" + diagram).offset().top - 45 }, 'slow');
                }
                catch (err) {
                    alert("#Diagram_" + diagram);
                }
            }

        }

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
            combobox.children(".ComboBoxText").text('Categories: All Categories');
        }

        var viewmode = currentParams['viewmode'];
        if (viewmode != null && viewmode == "list") {
            $("#ViewModeThumbs").css("border", "2px solid #333");
            $("#ViewModeList").css("border", "2px solid #eb3f3c");
        } else {
            $("#ViewModeThumbs").css("border", "2px solid #eb3f3c");
            $("#ViewModeList").css("border", "2px solid #333");
        }

        $('#ProductsCount').text($('#ProductsCountInvisible').text() + (model != "" ? " - filtered by " + model : ""));
        $('.ComboBoxText').show();

        // fade in #back-top
        $(function () {
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
        });


        $('#Loading').hide();
    });
</script>