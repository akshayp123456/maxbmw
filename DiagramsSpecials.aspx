<%@ Page Language="C#" AutoEventWireup="true" CodeFile="DiagramsSpecials.aspx.cs" Inherits="DiagramsSpecials" %>

<!DOCTYPE html>
<html>
<head>
    <title></title>
</head>
<body style="margin: 0px; font-family: Verdana;" onload="top.loadRightPanelContainer(document);">
<div style="text-align: center;" id="RightPanelContainer">
    <div style="margin-left: 3px;">
        <p style="text-align: center; font-size: 13px; font-weight: bold;">
        Scroll through the thumbnails on the left and select a diagram<br />
        or jump to a main group from the list above.
        </p>
        <%=ShowSpecials() %>
    </div>
    <div style="display: none;"><%=DateTime.Now.ToString() %>&nbsp;</div>
</div>
</body>
</html>
