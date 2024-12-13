<%@ Page Language="C#" AutoEventWireup="true" CodeFile="PartsTires.aspx.cs" Inherits="PartsTires" %>
<%@ Register TagPrefix="uc" TagName="UC_Parts_Head" Src="~/UC_Parts_Head.ascx" %>
<%@ Register TagPrefix="uc" TagName="UC_Parts_Body" Src="~/UC_Parts_Body.ascx" %>

<!DOCTYPE html>
<html>
<head>
    <title>BMW Motorcycle Parts | Tires Online | MAX BMW Motorcycles</title>
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
    <link href="css/MainMenu.css" rel="stylesheet" type="text/css" media="all"  />
    <link href="css/ButtonsMenu.css" rel="stylesheet" type="text/css" media="all"  /> 

    <uc:UC_Parts_Head runat="server" />         
</head>
<body>
    <uc:UC_Parts_Body runat="server" />
</body>
</html>