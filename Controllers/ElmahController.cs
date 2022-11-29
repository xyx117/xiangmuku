using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using xmkgl.Models;

namespace xmkgl.Controllers
{
    
    [Authorize(Roles = "业务专员")]
    public class ElmahController : Controller
    {
        // GET: Elmah
        public ActionResult Index(string type)
        {
            return new ElmahResult(type);
        }
        public ActionResult Detail(string type)
        {
            return new ElmahResult(type);
        }
    }
}