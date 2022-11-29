using System;
using System.Linq;
using System.Web.Mvc;
using xmkgl.DAL;
using PagedList;

namespace xmkgl.Controllers
{
    public class loginController : Controller
    {
        //日志较多，存放地址改为elmah的数据库
        //private cwcxmkContent db = new cwcxmkContent();
        private ElmahContent db = new ElmahContent();
        //
        // GET: /login/
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult loginindex(int? page, string searchstring, string currentfilter)
        {
            if (searchstring != null)
            {
                page = 1;
            }
            else
            {
                searchstring = currentfilter;
            }
            ViewBag.currentfilter = searchstring;

            var logintemp = from s in db.denglurizhis
                           select s;

            if (!String.IsNullOrEmpty(searchstring))
            {
                logintemp = logintemp.Where(s => s.username.ToUpper().Contains(searchstring.ToUpper()) || s.loginIP.ToUpper().Contains(searchstring.ToUpper()));
            }
            logintemp = logintemp.OrderByDescending(s => s.login_time);
            int pagesize = 10;
            int pagenumber = (page ?? 1);

            return View(logintemp.ToPagedList(pagenumber, pagesize));
        }
	}
}