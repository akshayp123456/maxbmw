<%@ Page Language="C#" AutoEventWireup="true" CodeFile="UserLogin.aspx.cs" Inherits="UserLogin" %>
<%@ Register TagPrefix="uc" TagName="UC_MainMenu" Src="~/UC_MainMenu.ascx" %>
<%@ Register TagPrefix="uc" TagName="UC_Footer" Src="~/UC_Footer.ascx" %>

<!DOCTYPE html>
<html>
<head>
<title>MAX BMW Motorcycles - Login</title>
    <meta http-equiv="Pragma" content="no-cache"/>
    <meta http-equiv="CACHE-CONTROL" content="NO-CACHE"/>
    <meta http-equiv="Expires" content="0" />
    <meta name="rating" content="general" />
    <meta name="author" content="MAX BMW Motorcycles Parts" />
    <meta name="apple-mobile-web-app-capable" content="yes" />
    <meta name="apple-mobile-web-app-status-bar-style" content="black" />   
    <meta name="format-detection" content="telephone=no" />

    <script type="text/javascript" src="js/ie6no.js" charset="utf-8"></script>
    <script type="text/javascript">
        ie6no({ runonload: true });
    </script>

    <link href="css/Global.css" rel="stylesheet" type="text/css" media="all" />
    <link href="css/MainMenu.css" rel="stylesheet" type="text/css" media="all" />
    <link href="css/Forms.css" rel="stylesheet" type="text/css" media="all" />

    <script type="text/javascript">
    function ValidateFields()
    {        
        var Problems = "";       
                
        if (document.getElementById('EMAIL').value.indexOf("@")==-1 || document.getElementById('EMAIL').value.length<4) {
            Problems += "Invalid e-mail.\n\r";            
        }              
        if (document.getElementById('PASSWORD').value.length<6) {
            Problems += "The Password needs to be at least 6 characters.\n\r";            
        }           
        
        if (Problems.length>0)
        {
            alert(Problems + "\n\r\n\rPlease correct the required fields and try again.\n\r");
            return false;
        }
            
        return true;
    }
    
    function checkEnterKey(e, FormName)
    { 
        var characterCode;
        if(e && e.which){ //if which property of event object is supported (NN4)
            e = e;
            characterCode = e.which; //character code is contained in NN4's which property
        } else {
            e = event
            characterCode = e.keyCode; //character code is contained in IE's keyCode property
        }

        if(characterCode == 13){ //if generated character code is equal to ascii 13 (if enter key)
            document.getElementById(FormName).submit(); //submit the form
            return false;
        } else {
            return true;
        }
    }

    function AfterLoad()
    {
        if (document.getElementById('EMAIL').value.length>3)
            document.getElementById('PASSWORD').focus();
        else
            document.getElementById('EMAIL').focus();            
    }
    </script>
</head>
<body onload="AfterLoad();">
<div id="centered">
    <uc:UC_MainMenu runat="server" />
                       
    <table cellpadding="0" cellspacing="0" border="0" class="NavigationBar">
        <tr>
            <td width="70">
                <span class="BackButton" onclick="window.location='<%=Utilities.GetLastCheckoutURL() %>';">
                    Cancel
                </span>
            </td>
            <td class="BarSeparator">&nbsp;</td>
            <td style="text-align: center;">
                Login
            </td>
        </tr>
    </table>

    <table class="Panel RoundedBottom" cellpadding="0" cellspacing="0">
        <tr>
            <td style="vertical-align: top;" align="center">                            
                <form id="LOGINFORM" action="UserLogin.aspx" method="post">
                    <table cellpadding="3" cellspacing="0" style="border: 0px; font-size: 13px; font-weight:bold;">
                        <tr>
                            <td colspan="2">
                                <br />
                                <br />
                                <%=m_Msg %>                                            
                                <br />
                                <br />
                            </td>    
                        </tr>
                        <tr>
                            <td align="right">
                                e-mail:
                            </td>
                            <td align="left">
                                <input type="text" id="EMAIL" name="EMAIL" maxlength="60" style="width: 200px;" value="<%=m_eMail %>" />                                       
                            </td>
                        </tr>
                        <tr>
                            <td align="right">
                                Password:
                            </td>
                            <td align="left">
                                <input type="password" id="PASSWORD" name="PASSWORD" maxlength="30" style="width: 200px;" onkeypress="checkEnterKey(event, 'LOGINFORM');" />                                        
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2" align="right">                                            
                                <a href="UserRecover.aspx" style="font-size: 11px; font-weight: bold; color: #33d; text-decoration: none;">Forgot your password?</a><br /><br />
                                <a href="UserRegister.aspx" style="font-size: 11px; font-weight: bold; color: #33d; text-decoration: none;">New user? Register here...</a>
                            </td>                                        
                        </tr>
                        <tr>
                            <td colspan="2" align="center">
                                <br />
                                <br />
                                <span class="Button" onclick="if (ValidateFields()) document.getElementById('LOGINFORM').submit();">&nbsp;&nbsp;Submit&nbsp;&nbsp;</span>
                                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                <span class="Button" onclick="window.location='<%=Utilities.GetLastLoginURL() %>';">&nbsp;&nbsp;Cancel&nbsp;&nbsp;</span>
                                <br />
                                <br />                                                                                        
                            </td>    
                        </tr>
                    </table>
                    <ul style="font-size: 12px; font-weight: bold; color: #e00; text-align: left; padding-left: 200px;">
                        <li>for your protection we do not keep your credit card information in our system</li>
                        <li>we do not share your email nor information with anyone</li>
                    </ul>
                </form> 
            </td>
        </tr>
        <tr>
            <td style="color: #B00; font-size: 10px; font-weight: normal;" align="right">
                Your IP Address is: <%=Request.ServerVariables["REMOTE_ADDR"] %>&nbsp;&nbsp;&nbsp;
            </td>
        </tr>
    </table>                
    <div id="ClearFooter"></div>    
</div>
 
<uc:UC_Footer runat="server" />

</body>
</html>

