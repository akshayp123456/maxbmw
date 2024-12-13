<%@ Page Language="C#" AutoEventWireup="true" CodeFile="fiche.aspx.cs" Inherits="Fiche" %>
<%@ Register TagPrefix="uc" TagName="UC_MainMenu" Src="~/UC_MainMenu.ascx" %>
<%@ Register TagPrefix="uc" TagName="UC_ButtonsMenu" Src="~/UC_ButtonsMenu.ascx" %>
<%@ Register TagPrefix="uc" TagName="UC_Footer" Src="~/UC_Footer.ascx" %>

<!DOCTYPE html>
<html>
<head>
    <title>MAX BMW Motorcycles - Parts Online</title>
    <meta name="description" content="MAX BMW Motorcycle offers an extensive BMW Motorcycle Parts fiche. OEM BMW Motorcycle Parts and Accessores can be ordered online for fast delivery. View our detailed BMW Motorcycle Parts diagrams and locate the part that you need efficiently." />
    <meta name="rating" content="general" />
    <meta name="author" content="MAX BMW Motorcycles Parts" />
    <meta http-equiv="Pragma" content="no-cache" />
    <meta http-equiv="CACHE-CONTROL" content="NO-CACHE" />
    <meta http-equiv="Expires" content="0" />
  
    <meta name="apple-mobile-web-app-capable" content="yes" />
    <meta name="apple-mobile-web-app-status-bar-style" content="black" />   
    <meta name="format-detection" content="telephone=no" />
    
    <link rel="apple-touch-startup-image" href="images/Splash1024x748.png" media="screen and (min-device-width: 481px) and (max-device-width: 1024px) and (orientation:landscape)" />
    <link rel="apple-touch-startup-image" href="images/Splash768x1004.png" media="screen and (min-device-width: 481px) and (max-device-width: 1024px) and (orientation:portrait)" />
    <link rel="apple-touch-startup-image" href="images/Splash320x460.png" media="screen and (min-device-width: 200px) and (max-device-width: 320px) and (orientation:portrait)" />

    <link rel="apple-touch-icon" href="images/apple-touch-icon-precomposed.png" />    

    <script type="text/javascript">
        try {
            if (top != self) {
                top.location.replace(window.location.href);
            }
        } catch (e) { }

        //if (window.location.href.indexOf('//shop.maxbmw.com/') < 0 && window.location.href.indexOf('//localhost/') < 0)
        //    window.location.href = 'https://shop.maxbmw.com/fiche/fiche.aspx';
        if (window.location.href.indexOf('//localhost/') < 0)
            window.location.href = 'http://localhost:50368/fiche.aspx'; 

    </script>
    
    <script type="text/javascript" src="js/ie6no.js" charset="utf-8"></script>
    <script type="text/javascript">
        ie6no({ runonload: true });
    </script>

    <script src="js/jquery-1.7.1.min.js" type="text/javascript"></script>
        
    <link href="css/Global.css" rel="stylesheet" type="text/css" />
    <link href="css/MainMenu.css" rel="stylesheet" type="text/css" />
    <link href="css/ButtonsMenu.css" rel="stylesheet" type="text/css" />       
        
    <script src="js/unoslider.js" type="text/javascript"></script> 
    <link href="css/unoslider.css" rel="stylesheet" type="text/css" /> 
    <link href="css/unoslider_theme_modern.css" rel="stylesheet" type="text/css" />
    
    <style type="text/css">
    img.img-slider
    {
      -moz-border-radius: 3px;
      -webkit-border-radius: 3px;
      border-radius: 3px;
    }
    div.panel-left-header
    {
        position: absolute;
        z-index: 3;
        width: 510px;
        height: 22px;
        padding-left: 10px;
        padding-top: 4px;
        font-size: 17px;
        font-weight: bold;
        color: #fff;
        background-color: #262626;
        border-bottom: solid 3px #eb3f3c;
        -ms-filter:"progid:DXImageTransform.Microsoft.Alpha(Opacity=65)"; 
        filter: alpha(opacity=65);
        -moz-opacity: 0.65;
        -khtml-opacity: 0.65;
        opacity: 0.65;
        -moz-border-radius-topleft: 4px;
	    -webkit-border-top-left-radius: 4px;
	    border-top-left-radius: 4px;
        -moz-border-radius-topright: 4px;
	    -webkit-border-top-right-radius: 4px;
	    border-top-right-radius: 4px;
    }   
    div.panel-right-header
    {
        position: absolute;
        z-index: 3;
        width: 430px;
        height: 40px;
        padding-left: 10px;
        padding-top: 4px;
        font-size: 16px;
        font-weight: bold;
        color: #fff;
        background-color: #262626;
        border-bottom: solid 2px #eb3f3c;
        -ms-filter:"progid:DXImageTransform.Microsoft.Alpha(Opacity=85)"; 
        filter: alpha(opacity=85);
        -moz-opacity: 0.85;
        -khtml-opacity: 0.85;
        opacity: 0.85;
        -moz-border-radius-topleft: 4px;
	    -webkit-border-top-left-radius: 4px;
	    border-top-left-radius: 4px;
        -moz-border-radius-topright: 4px;
	    -webkit-border-top-right-radius: 4px;
	    border-top-right-radius: 4px;
    }
    div.panel-right-description
    {
        position: absolute;
        z-index: 3;
        width: 100px;
        top: 46px;
        height: 310px;
        right: 0px;
        padding-left: 10px;
        padding-top: 11px;
        padding-right: 8px;
        font-weight: bold;
        font-size: 14px;
        color: #fff;
        text-align: right;
        background-color: #262626;
        -ms-filter:"progid:DXImageTransform.Microsoft.Alpha(Opacity=60)"; 
        filter: alpha(opacity=60);
        -moz-opacity: 0.60;
        -khtml-opacity: 0.60;
        opacity: 0.60;
        -moz-border-radius-bottomright: 4px;
	    -webkit-border-bottom-right-radius: 4px;
	    border-bottom-right-radius: 4px;
    }
    div.BannerBrands
    {
        background-color: #eee;
        height: 68px;
        width: 970px;
        -moz-border-radius: 4px;
        -webkit-border-radius: 4px;
        border-radius: 4px;
        -ms-filter:"progid:DXImageTransform.Microsoft.Alpha(Opacity=80)"; 
        filter: alpha(opacity=80);
        -moz-opacity: 0.8;
        -khtml-opacity: 0.8;
        opacity: 0.8;
	    margin-left: auto;
	    margin-right: auto;
	    cursor: pointer;  
	    padding-top: 7px;
	    margin-top: 6px;
    }
    a.BannerLink
    {
        text-decoration: none;
    }
    </style>
</head>
<body>
<div id="centered">
    <uc:UC_MainMenu runat="server" />
    <uc:UC_ButtonsMenu runat="server" />

    <!--div style="font-size: 22px; font-weight: bold; text-align: center; margin-top: 120px; margin-bottom: 180px; padding: 3px;">
        <label style="color: #c11;">Free shipping on all orders in the continental US</label>    
    </div-->
    <br />
    <div id="ClearFooter"></div> 
</div>

<uc:UC_Footer runat="server" />

<script type="text/javascript">
    $(document).ready(function () {
        $("td.ButtonMenu").hover(function () {
            $(this).addClass("hoverlook").siblings().removeClass("hoverlook");
        }, function () {
            $(this).removeClass("hoverlook");
        });

        $('#slider-left').unoslider({
            width: 520,
            height: 367,
            navigation: {
                autohide: true
            },
            slideshow: {
                speed: 10
            }
        });

        $('#slider-right').unoslider({
            width: 440,
            height: 367,
            navigation: {
                autohide: true
            },
            slideshow: {
                speed: 16
            }
        });
    });
</script>

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
</body>
</html>
