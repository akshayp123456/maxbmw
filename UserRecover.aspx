<%@ Page Language="C#" AutoEventWireup="true" CodeFile="UserRecover.aspx.cs" Inherits="UserRecovery" %>

<%@ Register TagPrefix="uc" TagName="UC_MainMenu" Src="~/UC_MainMenu.ascx" %>
<%@ Register TagPrefix="uc" TagName="UC_Footer" Src="~/UC_Footer.ascx" %>

<!DOCTYPE html>
<html>
<head>
    <title>MAX BMW Motorcycles - Forgot Password?</title>
    <meta http-equiv="Pragma" content="no-cache" />
    <meta http-equiv="CACHE-CONTROL" content="NO-CACHE" />
    <meta http-equiv="Expires" content="0" />
    <meta name="rating" content="general" />
    <meta name="author" content="MAX BMW Motorcycles Parts" />
    <meta name="apple-mobile-web-app-capable" content="yes" />
    <meta name="apple-mobile-web-app-status-bar-style" content="black" />
    <meta name="format-detection" content="telephone=no" />

    <link href="css/Global.css" rel="stylesheet" type="text/css" media="all" />
    <link href="css/MainMenu.css" rel="stylesheet" type="text/css" media="all" />
    <link href="css/Forms.css" rel="stylesheet" type="text/css" media="all" />

    <script type="text/javascript" src="js/FormValidations.js"></script>

    <!--script src="https://www.google.com/recaptcha/api.js" async defer></!--script-->

    <script type="text/javascript">
        function ValidateFields() {
            var Problems = "";
            var captchaCode = document.getElementById('captcha').value;

            Problems += ValidateField('FIRSTNAME', 'First Name', 1, nameRegxp);
            Problems += ValidateField('LASTNAME', 'Last Name', 1, nameRegxp);
            Problems += ValidateField('EMAIL', 'e-mail', 4, emailRegxp);
            //if (document.getElementById('recaptcha_response_field').value.length==0)
            //    Problems += "You have to type the text shown in the image.";            
            if (captchaCode == "")
                Problems += "Please enter captcha code";

            if (Problems.length > 0) {
                alert(Problems + "\n\r\n\rPlease correct the required fields and try again.\n\r");
                return false;
            }
            alert("You will receive an e-mail soon containing your new password.\n\rOnce you receive it you can try to log in again.");
            return true;
        }
    </script>
</head>
<body onload="document.getElementById('EMAIL').focus();">
    <div id="centered">
        <uc:UC_MainMenu runat="server" />

        <table cellpadding="0" cellspacing="0" border="0" class="NavigationBar">
            <tr>
                <td width="70">
                    <span class="BackButton" onclick="window.location='<%=Utilities.GetLastCheckoutURL() %>';">Back   
                    </span>
                </td>
                <td class="BarSeparator">&nbsp;</td>
                <td style="text-align: center;">
                    <%=m_Msg %>
                </td>
            </tr>
        </table>

        <table cellpadding="0" cellspacing="0" border="0" class="Panel RoundedBottom">
            <tr>
                <td style="vertical-align: top;" align="center">
                    <form id="USERFORM" action="UserRecover.aspx" method="post">
                        <table cellpadding="3" cellspacing="0" style="border: 0px; font-size: 13px; font-weight: bold;">
                            <tr>
                                <td align="left" class="Heading">Enter your information:
                                </td>
                            </tr>
                            <tr>
                                <td align="left" class="Content">
                                    <table cellspacing="0" cellpadding="1" border="0">
                                        <%=m_MsgWrongInfo %>
                                        <tr>
                                            <td>e-mail:</td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <input name="EMAIL" id="EMAIL" value="<%=m_eMail %>" type="text" maxlength="60" style="width: 230px;" /><span class="RequiredField">*</span></td>
                                        </tr>
                                        <tr>
                                            <td>&nbsp;</td>
                                        </tr>
                                    </table>
                                    <table cellspacing="0" cellpadding="1" border="0">
                                        <tr>
                                            <td>First Name (Given Name):</td>
                                            <td>&nbsp;&nbsp;Last Name (Surname):</td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <input name="FIRSTNAME" id="FIRSTNAME" type="text" value="<%=m_FirstName %>" maxlength="30" style="width: 230px;" /><span class="RequiredField">*</span></td>
                                            <td>&nbsp;&nbsp;<input name="LASTNAME" id="LASTNAME" type="text" value="<%=m_LastName %>" maxlength="30" style="width: 230px;" /><span class="RequiredField">*</span></td>
                                        </tr>
                                        <tr>
                                            <td colspan="2">&nbsp;<br />
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            <tr>
                                <td align="left">&nbsp;</td>
                            </tr>
                        </table>
                        <%=m_MsgCaptcha %>
                        <!--script type="text/javascript">
                        var RecaptchaOptions = {
                            theme : 'white',
                            tabindex : 2
                        };
                    </!--script-->

                        <!--
                    <div class="g­recaptcha" data­sitekey="6LeX0VUUAAAAAKr1G6ZVEXX4J5KbS2o0nnYqGGmI"></div>
                    -->
                        <br />
                        <table cellspacing="0" cellpadding="1" border="0">
                            <tr>
                                <td></td>
                                <td>
                                    <img src="CaptchaCode.aspx" alt="CAPTCHA" /></td>
                            </tr>
                            <tr>
                                <td>
                                    <label for="captcha">Enter CAPTCHA :</label></td>
                                <td>
                                    <input type="text" runat="server" id="captcha" name="captcha" /><span class="RequiredField">*</span></td></td>
                            </tr>

                        </table>
                        <br />
                        <span style="font-size: 11px;">We will e-mail you a new password if your information matches our records.</span><br />
                        <br />
                        <span class="Button" onclick="if (ValidateFields()) document.getElementById('USERFORM').submit();">&nbsp;&nbsp;Submit&nbsp;&nbsp;</span>
                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                    <span class="Button" onclick="window.location='<%=Utilities.GetLastLoginURL() %>';">&nbsp;&nbsp;Cancel&nbsp;&nbsp;</span>
                    </form>
                </td>
            </tr>
            <tr>
                <td style="color: #B00; font-size: 10px; font-weight: normal;" align="right">Your IP Address is: <%=Request.ServerVariables["REMOTE_ADDR"] %>&nbsp;&nbsp;&nbsp;
                </td>
            </tr>
        </table>

        <div id="ClearFooter"></div>

    </div>

    <uc:UC_Footer runat="server" />

</body>
</html>
