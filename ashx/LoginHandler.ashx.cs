using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;
using Newtonsoft.Json;

namespace xmkgl.ashx
{
    public class LoginHandler : IHttpHandler, IRequiresSessionState
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";

            string userName = context.Request["userName"].ToString();
            string userPass = context.Request["userPass"].ToString();
            string userCode = context.Request["userCode"].ToString();

            if (userName == "admin" && userPass == "123456" && userCode == context.Session["code"].ToString())
            {
                context.Response.Write(JsonConvert.SerializeObject("Yes"));
            }
            else
            {
                context.Response.Write(JsonConvert.SerializeObject("No"));
            }
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }


    /// <summary>
    /// LoginHandler 的摘要说明
    /// </summary>
    //public class LoginHandler : IHttpHandler
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