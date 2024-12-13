using System;
using System.Diagnostics;
using System.Text;
using System.Web;
using System.Net;
using System.Net.Sockets;
using System.IO;

public class RecaptchaResponse
{
    public static RecaptchaResponse Valid = new RecaptchaResponse(true, "");
    public static RecaptchaResponse InvalidSolution = new RecaptchaResponse(false, "incorrect-captcha-sol");
    public static RecaptchaResponse RecaptchaNotReachable = new RecaptchaResponse(false, "recaptcha-not-reachable");

    bool isValid;
    string errorCode;

    internal RecaptchaResponse(bool isValid, string errorCode)
    {
        this.isValid = isValid;
        this.errorCode = errorCode;
    }

    public bool IsValid
    {
        get { return isValid; }
    }

    public string ErrorCode
    {
        get { return errorCode; }
    }

    public override bool Equals(object obj)
    {
        RecaptchaResponse other = (RecaptchaResponse)obj;
        if (other == null)
        {
            return false;
        }

        return other.IsValid == IsValid && other.ErrorCode == ErrorCode;
    }

    public override int GetHashCode()
    {
        return IsValid.GetHashCode() ^ ErrorCode.GetHashCode();
    }
}


public class RecaptchaValidator
{

    public RecaptchaValidator(){}

    private string privateKey;  
    private string remoteIp;
    
    private string challenge;
    private string response;

    const string VerifyUrl = "http://api-verify.recaptcha.net/verify";

    public string PrivateKey
    {
	    get { return privateKey; }
	    set { privateKey = value; }
    }

    public string RemoteIP
    {
	    get { return remoteIp;}
	    set { 
            IPAddress ip = IPAddress.Parse(value);

            if (ip.AddressFamily != AddressFamily.InterNetwork &&
                ip.AddressFamily != AddressFamily.InterNetworkV6) {
                throw new ArgumentException("Expecting an IP address, got " + ip);
            }

            remoteIp = ip.ToString();
        }
    }

    public string Challenge
    {
	    get { return challenge;}
	    set { challenge = value;}
    }

    public string Response
    {
	    get { return response;}
	    set { response = value;}
    }

    void CheckNotNull(object obj, string name)
    {
        if (obj == null)
        {
            throw new ArgumentNullException(name);
        }
    }

    public RecaptchaResponse Validate()
    {
        CheckNotNull(PrivateKey, "PrivateKey");
        CheckNotNull(RemoteIP, "RemoteIp");
        CheckNotNull(Challenge, "Challenge");
        CheckNotNull(Response, "Response");

        if (challenge == "" || response == "") {
            return RecaptchaResponse.InvalidSolution;
        }

        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(VerifyUrl);
        // to avoid issues with Expect headers
        request.ProtocolVersion = HttpVersion.Version10;
        request.Timeout = 30 * 1000 /* 30 seconds */;
        request.Method = "POST";
        request.UserAgent = "reCAPTCHA/ASP.NET";

        request.ContentType = "application/x-www-form-urlencoded";

        string formdata = string.Format("privatekey={0}&remoteip={1}&challenge={2}&response={3}",
                                HttpUtility.UrlEncode(PrivateKey),
                                HttpUtility.UrlEncode(RemoteIP),
                                HttpUtility.UrlEncode(Challenge),
                                HttpUtility.UrlEncode(Response));

        byte[] formbytes = Encoding.ASCII.GetBytes(formdata);

        using (Stream requestStream = request.GetRequestStream())
            requestStream.Write(formbytes, 0, formbytes.Length);

        string[] results;

        try {
            using (WebResponse httpResponse = request.GetResponse())
            {
                using (TextReader readStream = new StreamReader(httpResponse.GetResponseStream(), Encoding.UTF8))
                {
                    results = readStream.ReadToEnd().Split();
                }
            }
        } catch (WebException ex) {
            EventLog.WriteEntry("Application", ex.Message, EventLogEntryType.Error);
            return RecaptchaResponse.RecaptchaNotReachable;
        }

        switch (results[0])
        {
            case "true":
                return RecaptchaResponse.Valid;
            case "false":
                return new RecaptchaResponse(false, results[1]);
            default:
                throw new InvalidProgramException("Unknown status response.");
        }
    }
}

