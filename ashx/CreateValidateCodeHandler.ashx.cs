using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Web;
using System.Web.SessionState;


namespace xmkgl.ashx
{
    public class CreateValidateCodeHandler : IHttpHandler, IRequiresSessionState
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "image/jpeg";
            string code = GetRandomNumber();
            context.Session["code"] = code;
            using (Bitmap bitmap = CreateImage(code))
            {
                bitmap.Save(context.Response.OutputStream, ImageFormat.Bmp);
            }
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// 随机数字
        /// </summary>
        /// <returns></returns>
        private string GetRandomNumber()
        {
            string code = string.Empty;
            Random random = new Random();
            for (int i = 0; i < 4; i++)
            {
                code += random.Next(10);
            }
            return code;
        }

        /// <summary>
        ///  随机字符
        /// </summary>
        /// <returns></returns>
        private string GetRandomLetter()
        {
            string code = string.Empty;
            Random random = new Random();
            for (int i = 0; i < 4; i++)
            {
                char letter = (char)random.Next(65, 90);
                code += letter.ToString();
            }
            return code;
        }

        /// <summary>
        /// 画图片的背景图+干扰线 
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        private Bitmap CreateImage(string code)
        {
            Bitmap bitmap = new Bitmap(code.Length * 18, 33);
            Graphics g = Graphics.FromImage(bitmap);
            g.Clear(Color.White);

            // 随机颜色、字体
            Color[] colors = { Color.Black, Color.Red, Color.DarkBlue, Color.Green, Color.Brown, Color.DarkCyan, Color.Purple };
            string[] fonts = { "Verdana", "Microsoft Sans Serif", "Comic Sans MS", "Arial", "宋体" };

            // 绘制背景线
            Random random = new Random();
            Pen pen = new Pen(Color.LightGray, 1);
            for (int i = 0; i < 20; i++)
            {
                int x1 = random.Next(bitmap.Width);
                int x2 = random.Next(bitmap.Width);
                int y1 = random.Next(bitmap.Height);
                int y2 = random.Next(bitmap.Height);
                g.DrawLine(pen, x1, y1, x2, y2);
            }

            // 绘制字符
            for (int i = 0; i < code.Length; i++)
            {
                Font font = new Font(fonts[random.Next(5)], 15, FontStyle.Bold);
                Brush brush = new SolidBrush(colors[random.Next(7)]);
                g.DrawString(code[i].ToString(), font, brush, 3 + (i * 15), 2);
            }
            return bitmap;
        }
    }



    /// <summary>
    /// CreateValidateCodeHandler 的摘要说明
    /// </summary>
    //public class CreateValidateCodeHandler : IHttpHandler
    //{

    //    public void ProcessRequest(HttpContext context)
    //    {
    //        context.Response.ContentType = "text/plain";
    //        context.Response.Write("Hello World");
    //    }

    //    public bool IsReusable
    //    {
    //        get
    //        {
    //            return false;
    //        }
    //    }
    //}

}