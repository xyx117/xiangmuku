using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using xmkgl.DAL;
using xmkgl.Models;
using PagedList;
using Microsoft.AspNet.Identity;
//路径设置时引用

namespace xmkgl.Controllers
{
    public class HomeController : Controller
    {
        private cwcxmkContent db = new cwcxmkContent();
        [Authorize]
        public ActionResult Index()
        {
            string loingid = User.Identity.GetUserId();          
            var role = common.customidentity.userrole(loingid);
            if (role == "评委")
            {
                return View("shenhe_pw_index");
            }
            return View();
        }

        //测试登录页面 的验证码
        public ActionResult Test()
        {            
            return View();
        }

        //[Authorize(Roles = "部门负责人,员工")]
        // 下载 上传模板
        public FileResult GetFile()
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + "Content/uploads/moban/";
            string fileName = "bmfzr_help.docx";
            return File(path + fileName, "text/plain", fileName);
        }



        //附件路径设置fujianlujing
        //private void btnUser_Click(object sender, EventArgs e)
        //{
        //    //选择文件
        //    OpenFileDialog openFileDialog = new OpenFileDialog();
        //    openFileDialog.Multiselect = true;
        //    //文件格式
        //    openFileDialog.Filter = "所有文件|*.*";
        //    //还原当前目录
        //    openFileDialog.RestoreDirectory = true;
        //    //默认的文件格式
        //    openFileDialog.FilterIndex = 1;
        //    if (openFileDialog.ShowDialog() == DialogResult.OK)
        //    {
        //        string path = openFileDialog.FileName;
        //    }

        //    //选择文件夹
        //    FolderBrowserDialog dialog = new FolderBrowserDialog();
        //    dialog.Description = "请选择文件路径";
        //    if (dialog.ShowDialog() == DialogResult.OK)
        //    {
        //        string foldPath = dialog.SelectedPath;
        //        MessageBox.Show("已选择文件夹:" + foldPath, "选择文件夹提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
        //    }
        //}



        //消息发布开始，消息发布页面
        [Authorize]
        public ActionResult Indexnew(int? page, string searchstring, string currentfilter)
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

            var newstemp = from s in db.news
                           select s;
            if (!String.IsNullOrEmpty(searchstring))
            {
                newstemp = newstemp.Where(s => s.Content.ToUpper().Contains(searchstring.ToUpper())||s.title.ToUpper().Contains(searchstring.ToUpper()));
            }
            newstemp = newstemp.OrderByDescending(s => s.publishtime);
            int pagesize = 10;
            int pagenumber = (page ?? 1);

            return View(newstemp.ToPagedList(pagenumber, pagesize));
        }        

        //编辑 发布消息页面
        public ActionResult Createnew()
        {
            return View();
        }


        [HttpPost]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Createnew([Bind(Include = "title,Content")]News news, string author)
        {
            if (ModelState.IsValid)
            {
                news.publishtime = DateTime.Now;
                news.author = author;
                try
                {
                    db.news.Add(news);
                    db.SaveChanges();
                    return RedirectToAction("Indexnew");
                }
                catch (DataException)
                {
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
                }
            }
            return View(news);
        }


        public ActionResult Editnew(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            News new1 = db.news.Find(id);
            if (new1 == null)
            {
                return HttpNotFound();
            }
            return View(new1);
        }


        [HttpPost]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Editnew([Bind(Include = "ID,title, Content")]News news, string author)//
        {
            try
            {
                if (ModelState.IsValid)
                {
                    news.publishtime = DateTime.Now;
                    news.author = author;
                    db.Entry(news).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Indexnew");
                }
            }
            catch (DataException /* dex */)
            {
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
            }
            return View(news);
        }


        public ActionResult Detailnew(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            News new1 = db.news.Find(id);
            if (new1 == null)
            {
                return HttpNotFound();
            }
            return View(new1);
        }


        public ActionResult Delnew(int? id, bool? saveChangesError = false)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (saveChangesError.GetValueOrDefault())
            {
                ViewBag.ErrorMessage = "Delete failed. Try again, and if the problem persists see your system administrator.";
            }
            News news = db.news.Find(id);
            if (news == null)
            {
                return HttpNotFound();
            }
            return View(news);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delnew(int id)
        {
            try
            {
                News student = db.news.Find(id);
                db.news.Remove(student);
                db.SaveChanges();
            }
            catch (DataException/* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                return RedirectToAction("Delete", new { id = id, saveChangesError = true });
            }
            return RedirectToAction("Indexnew");
        }

        //消息发布结束
       

        public ActionResult Welcome()
        {
            return View();
        }
 
        public class xmname
        {
            public string text{ get; set; }
        }


        //将数据库中的项目目录以json返回页面
        [HttpPost]
        public JsonResult GetXmmuluData(string loingid)
        {
            //根据ID取出该用户的所属目录
            string suoshumulu = common.customidentity.pingshenmulu(loingid);

            //判断所登录的userid是否是评委角色，是则根据所属目录来显示相关目录
            string shifoupingwei = common.customidentity.userrole(loingid);

            string temp;
            List<xmname> xmmuluList = new List<xmname>();            

            var query = from s in db.xiangmumulus
                        orderby s.chuangjian_time descending     //按照目录创建时间降序排序
                        select new { xmname = s.Name, fgldshenhefou = s.fgxzshenhefou, ksshijian = s.Kaishishijian, jsshijian = s.Jieshushijian };

            foreach (var item in query)
            {
                temp = "<a style=\"margin-left:20px;text-decoration:none\" title=\"申报项目时间" + item.ksshijian.ToShortDateString() + "至" + item.jsshijian.ToShortDateString() + "\" href=\"javascript:void(0)\" onclick=\"showniandulist('/xiangmuguanli/xiangmugualiIndex?muluName=" + item.xmname + "&fgldshenhefou=" + item.fgldshenhefou + "&loingid=" + loingid + "&jsshijian=" + item.jsshijian + "')\" target=\"mainFrame\" ><Image src='/Scripts/easyui/themes/icons/zoom_in.png' Title='分管领导参与审核'/>&nbsp;&nbsp;" + item.xmname + "</a>";//glyphicon glyphicon-eye-open
                
                if (item.fgldshenhefou=="不审")
                {
                    temp = "<a style=\"margin-left:20px;text-decoration:none\" title=\"申报项目时间" + item.ksshijian.ToShortDateString() + "至" + item.jsshijian.ToShortDateString() + "\"  href=\"javascript:void(0)\" onclick=\"showniandulist('/xiangmuguanli/xiangmugualiIndex?muluName=" + item.xmname + "&fgldshenhefou=" + item.fgldshenhefou + "&loingid=" + loingid + "&jsshijian=" + item.jsshijian + "')\" target=\"mainFrame\"><Image src='/Scripts/easyui/themes/icons/zoom.png' Title='分管领导不参与审核'/>&nbsp;&nbsp;" + item.xmname + "</span></a>";                      
                }
                xmname student = new xmname { text = temp };
                xmmuluList.Add(student);                    
            }            
            return Json(xmmuluList.ToList());
        }


        //评委获取目录
        [HttpPost]
        public JsonResult pw_GetXmmuluData(string loingid)
        {
            //根据ID取出该用户的所属目录
            string suoshumulu = common.customidentity.pingshenmulu(loingid);

            //判断所登录的userid是否是评委角色，是则根据所属目录来显示相关目录
            string shifoupingwei = common.customidentity.userrole(loingid);

            string temp;
            List<xmname> xmmuluList = new List<xmname>();

            var query1 = from s in db.xiangmumulus.Where(s => suoshumulu.Contains(s.Name))
                            orderby s.chuangjian_time descending     //按照目录创建时间降序排序
                            select new { xmname = s.Name, fgldshenhefou = s.fgxzshenhefou, ksshijian = s.Kaishishijian, jsshijian = s.Jieshushijian };
            foreach (var item in query1)
            {
                temp = "<a style=\"margin-left:20px;text-decoration:none\" title=\"申报项目时间" + item.ksshijian.ToShortDateString() + "至" + item.jsshijian.ToShortDateString() + "\" href=\"javascript:void(0)\" onclick=\"showniandulist('/xiangmuguanli/xiangmugualiIndex?muluName=" + item.xmname + "&fgldshenhefou=" + item.fgldshenhefou + "&loingid=" + loingid + "&jsshijian=" + item.jsshijian + "')\" target=\"mainFrame\"><Image src='/Scripts/easyui/themes/icons/zoom_in.png' Title='分管领导参与审核'/>&nbsp;&nbsp;" + item.xmname + "</span></a>";//这里的userid有什么作用

                if (item.fgldshenhefou == "不审")
                {
                    temp = "<a style=\"margin-left:20px;text-decoration:none\" title=\"申报项目时间" + item.ksshijian.ToShortDateString() + "至" + item.jsshijian.ToShortDateString() + "\" href=\"javascript:void(0)\" onclick=\"showniandulist('/xiangmuguanli/xiangmugualiIndex?muluName=" + item.xmname + "&fgldshenhefou=" + item.fgldshenhefou + "&loingid=" + loingid + "&jsshijian=" + item.jsshijian + "')\" target=\"mainFrame\"><Image src='/Scripts/easyui/themes/icons/zoom.png' Title='分管领导不参与审核'/>&nbsp;&nbsp;" + item.xmname + "</span></a>";//这里的userid有什么作用
                }                    
                xmname student = new xmname { text = temp };
                xmmuluList.Add(student);
            }            
            return Json(xmmuluList.ToList());
        }       


        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult help()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}