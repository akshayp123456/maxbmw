using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Net;

public class FileParameter
{
    public byte[] File { get; set; }
    public string FileName { get; set; }
    public string ContentType { get; set; }
    public FileParameter(byte[] file) : this(file, null) { }
    public FileParameter(byte[] file, string filename) : this(file, filename, null) { }
    public FileParameter(byte[] file, string filename, string contenttype)
    {
        File = file;
        FileName = filename;
        ContentType = contenttype;
    }
} 

public class WebInterface
{        
    CookieContainer m_cookies;

    public WebInterface()
    {
        m_cookies = new CookieContainer();
    }                  

    public string GetPostPage(string URL, string FormData)
    {
        string page = "";

        ServicePointManager.Expect100Continue = false;
            
        byte[] data = Encoding.Default.GetBytes(FormData);

        HttpWebRequest wr = (HttpWebRequest)WebRequest.Create(URL.ToString());

        wr.Accept = "*/*";
        wr.ContentLength = data.Length;
        wr.ContentType = "application/x-www-form-urlencoded";
        wr.Method = "POST";
        wr.CookieContainer = m_cookies;
        wr.KeepAlive = true;
        wr.UserAgent = "Mozilla/5.0 (Windows NT 6.3; Trident/7.0; rv:11.0) like Gecko";

        Stream newStream;
        newStream = wr.GetRequestStream();
        newStream.Write(data, 0, data.Length);
        newStream.Close();

        try
        {
            HttpWebResponse wresponse = (HttpWebResponse)wr.GetResponse();
            CookieCollection ck = m_cookies.GetCookies(new Uri(URL.ToString()));
            foreach (System.Net.Cookie co in ck)
            {
                co.Path = "/";
                m_cookies.Add(co);
            }

            StreamReader reader = new StreamReader(wresponse.GetResponseStream());
            page = reader.ReadToEnd();
        }
        catch (WebException e)
        {
            using (WebResponse response = e.Response)
            {
                HttpWebResponse httpResponse = (HttpWebResponse)response;
                Console.WriteLine("Error code: {0}", httpResponse.StatusCode);
                using (Stream d = response.GetResponseStream())
                using (var reader = new StreamReader(d))
                {
                    string text = reader.ReadToEnd();
                    page = text;
                    Console.WriteLine(text);
                }
            }
        }

        return page;
    }
    
    public string GetPage(string URL)
    {
        ServicePointManager.Expect100Continue = false;

        HttpWebRequest wr = (HttpWebRequest)WebRequest.Create(URL.ToString());

        wr.Accept = "*/*";
        wr.CookieContainer = m_cookies;
        wr.KeepAlive = true;
        wr.ContentType = "application/x-www-form-urlencoded";
        wr.UserAgent = "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; WOW64; Trident/5.0)";
        
        string page = "";
        try
        {
            HttpWebResponse wresponse = (HttpWebResponse)wr.GetResponse();

            StreamReader reader = new StreamReader(wresponse.GetResponseStream());
            page = reader.ReadToEnd();
        }
        catch
        {}

        return page;
    }
    
    private readonly Encoding encoding = Encoding.UTF8;

    public string MultipartFormDataPost(string postUrl, Dictionary<string, object> postParameters)
    {
        string formDataBoundary = String.Format("----------{0:N}", Guid.NewGuid());
        string contentType = "multipart/form-data; boundary=" + formDataBoundary;

        byte[] formData = GetMultipartFormData(postParameters, formDataBoundary);

        HttpWebResponse webResponse =  PostForm(postUrl, contentType, formData);
        StreamReader responseReader = new StreamReader(webResponse.GetResponseStream());
        string fullResponse = responseReader.ReadToEnd();
        webResponse.Close();
        
        return fullResponse;
    }

    private HttpWebResponse PostForm(string postUrl, string contentType, byte[] formData)
    {
        ServicePointManager.Expect100Continue = false;

        HttpWebRequest request = WebRequest.Create(postUrl) as HttpWebRequest;

        if (request == null)
        {
            throw new NullReferenceException("request is not a http request");
        }

        // Set up the request properties. 
        request.Method = "POST";
        request.ContentType = contentType;
        request.UserAgent = "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; WOW64; Trident/5.0)";
        request.CookieContainer = m_cookies;
        request.ContentLength = formData.Length;
  
        // Send the form data to the request. 
        using (Stream requestStream = request.GetRequestStream())
        {
            requestStream.Write(formData, 0, formData.Length);
            requestStream.Close();
        }

        return request.GetResponse() as HttpWebResponse;
    }

    private byte[] GetMultipartFormData(Dictionary<string, object> postParameters, string boundary)
    {
        Stream formDataStream = new System.IO.MemoryStream();
        bool needsCLRF = false;

        foreach (var param in postParameters)
        {
            // Thanks to feedback from commenters, add a CRLF to allow multiple parameters to be added. 
            // Skip it on the first parameter, add it to subsequent parameters. 
            if (needsCLRF)
                formDataStream.Write(encoding.GetBytes("\r\n"), 0, encoding.GetByteCount("\r\n"));

            needsCLRF = true;

            if (param.Value is FileParameter)
            {
                FileParameter fileToUpload = (FileParameter)param.Value;

                // Add just the first part of this param, since we will write the file data directly to the Stream 
                string header = string.Format("--{0}\r\nContent-Disposition: form-data; name=\"{1}\"; filename=\"{2}\";\r\nContent-Type: {3}\r\n\r\n",
                    boundary,
                    param.Key,
                    fileToUpload.FileName ?? param.Key,
                    fileToUpload.ContentType ?? "application/octet-stream");

                formDataStream.Write(encoding.GetBytes(header), 0, encoding.GetByteCount(header));

                // Write the file data directly to the Stream, rather than serializing it to a string. 
                formDataStream.Write(fileToUpload.File, 0, fileToUpload.File.Length);
            }
            else
            {
                string postData = string.Format("--{0}\r\nContent-Disposition: form-data; name=\"{1}\"\r\n\r\n{2}",
                    boundary,
                    param.Key,
                    param.Value);
                formDataStream.Write(encoding.GetBytes(postData), 0, encoding.GetByteCount(postData));
            }
        }

        // Add the end of the request.  Start with a newline 
        string footer = "\r\n--" + boundary + "--\r\n";
        formDataStream.Write(encoding.GetBytes(footer), 0, encoding.GetByteCount(footer));

        // Dump the Stream into a byte[] 
        formDataStream.Position = 0;
        byte[] formData = new byte[formDataStream.Length];
        formDataStream.Read(formData, 0, formData.Length);
        formDataStream.Close();

        return formData;
    }

    public static string HttpPost(string url, string data, string charSet)
    {
        string value = "";

        // Initialize Connection
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
        request.Method = "POST";
        request.ContentType = "application/x-www-form-urlencoded; charset=" + charSet;
        request.UserAgent = "Mozilla/5.0 (Windows NT 6.3; Trident/7.0; rv:11.0) like Gecko";
        request.ContentLength = data.Length;

        // Write Data
        StreamWriter writer = new StreamWriter(request.GetRequestStream());
        writer.Write(data);
        writer.Close();

        // Read Response
        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        StreamReader reader = new StreamReader(response.GetResponseStream());
        value = reader.ReadToEnd();
        reader.Close();

        return value;
    }
}
