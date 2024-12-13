<%@ WebHandler Language="C#" Class="CaptchaHandler" %>


using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Web;

public class CaptchaHandler : IHttpHandler
{
    public void ProcessRequest(HttpContext context)
    {
        //if (context.Session == null)
        //{
        //    context.Response.StatusCode = 500;
        //    context.Response.Write("Session is not available.");
        //    return;
        //}


        context.Response.ContentType = "image/png";

        // Generate CAPTCHA text
        string captchaText = GenerateCaptchaText();
        // Save CAPTCHA text in session (to verify later)
        //HttpContext.Current.Session["Captcha"] = captchaText;
        //context.Session["Captcha"] = captchaText;
        // Create CAPTCHA image
        Bitmap captchaImage = CreateCaptchaImage(captchaText);

        // Output the image to the response stream
        captchaImage.Save(context.Response.OutputStream, ImageFormat.Png);

    }

    private string GenerateCaptchaText()
    {
        string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        Random rand = new Random();
        char[] captchaText = new char[6];

        for (int i = 0; i < captchaText.Length; i++)
        {
            captchaText[i] = chars[rand.Next(chars.Length)];
        }

        return new string(captchaText);
    }

    private Bitmap CreateCaptchaImage(string text)
    {
        Bitmap bmp = new Bitmap(200, 80);
        Graphics g = Graphics.FromImage(bmp);
        g.Clear(Color.White);

        // Set up font and brush
        Font font = new Font("Arial", 24, FontStyle.Bold);
        Brush brush = new SolidBrush(Color.Black);

        // Draw the text
        g.DrawString(text, font, brush, 10, 20);

        // Add noise (optional)
        AddNoise(g, bmp.Width, bmp.Height);

        return bmp;
    }

    private void AddNoise(Graphics g, int width, int height)
    {
        Random rand = new Random();
        for (int i = 0; i < 100; i++)
        {
            int x = rand.Next(width);
            int y = rand.Next(height);
            g.FillRectangle(Brushes.Gray, x, y, 1, 1);
        }
    }

    public bool IsReusable
    {
        get { return false; }
    }
}
