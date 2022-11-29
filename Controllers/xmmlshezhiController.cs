using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using xmkgl.DAL;
using xmkgl.Models;
using PagedList;
using xmkgl.common;

namespace xmkgl.Controllers
{
    [Authorize(Roles = "业务专员")]
    public class xmmlshezhiController : Controller
    {
        // GET: xiangmushezhi
        private cwcxmkContent db = new cwcxmkContent();

        //存在就返回false,不存在就返回true
        [HttpPost]
        public String checkNameIsSame(String name)
        {
            string isOk = "False";
            var xm = db.xiangmumulus.Where(x => x.Name == name).Select(x => x.Name);

            if (!xm.Any())
            {
                isOk = "True";
            }
            return isOk + "";
        }

        // GET: users
        public ActionResult xmmlshezhiIndex()
        {         
           return View();
        }       


        [HttpPost]
        public JsonResult getMulu()
        {
            int page =(Request.Form["page"]!=null) ? Int32.Parse(Request.Form["page"]) : 1;

            int rows = (Request.Form["rows"] != null) ? Int32.Parse(Request.Form["rows"]) : 10;

            var xm = from s in db.xiangmumulus
                     orderby s.chuangjian_time descending
                     select s;
           try
            {
                // 返回到前台的值必须按照如下的格式包括 total and rows 
                var easyUIPages = new Dictionary<string, object>();
                easyUIPages.Add("total", xm.Count());
                easyUIPages.Add("rows", xm.ToPagedList(page, rows));
                return Json(easyUIPages);
            }
            catch (Exception)
            {
                throw;
            }
        }


        [HttpPost]
        //[ValidateAntiForgeryToken]   这条语句不能用！！！！！
        //public JsonResult saveuser([Bind(Include = "Lastname,Firstname,Phone,Email")] user user)
        public JsonResult saveMulu([Bind(Include = "Name,Beizhu,fgxzshenhefou,ldshenhefou,Kaishishijian,Jieshushijian")]xiangmumulu xiangmumulu)//在这里的绑定中没有主键ID，因为在数据库中，ID是自动增长得
        {
            xiangmumulu.chuangjian_time = DateTime.Now;    //目录的创建时间

            if (xiangmumulu.fgxzshenhefou == "on")
            {
                xiangmumulu.fgxzshenhefou = "审核";
            }
            else
            {
                xiangmumulu.fgxzshenhefou = "不审";
            }

            if (ModelState.IsValid)
            {
                try
                { 
                    db.xiangmumulus.Add(xiangmumulu);
                    db.SaveChanges();
                    return Json(new {success= true ,errorMsg ="保存目录成功！" }, "text/html");
                }
                catch (Exception ex) 
                {
                    return Json(new {success=false, errorMsg = ex.ToString() }, "text/html");
                }                
            }
            //底下这个方法更简洁
            return Json(new { success= false,errorMsg = "some error occured." });
        }


        [HttpPost]
        //[ValidateAntiForgeryToken]    [Bind(Include = "ID,Name,Beizhu,fgxzshenhefou,Kaishishijian,Jieshushijian,chuangjian_time")] xiangmumulu xiangmumulu,
        public JsonResult updateMulu(string name)
        {
            var xiangmumulu = db.xiangmumulus.Where(s => s.Name == name).FirstOrDefault();

            xiangmumulu.Beizhu = Request.Form["Beizhu"];
            xiangmumulu.Kaishishijian = Convert.ToDateTime(Request.Form["Kaishishijian"]);
            xiangmumulu.Jieshushijian = Convert.ToDateTime(Request.Form["Jieshushijian"]);

            string fgldshenhefou = Request.Form["fgxzshenhefou"];   //switchbutton传过来的到底是什么类型的值

            if (fgldshenhefou == "on")
            {
                xiangmumulu.fgxzshenhefou = "审核";
            }
            else
            {
                xiangmumulu.fgxzshenhefou = "不审";
            }
            if (ModelState.IsValid)
            {
                try
                {
                    db.Entry(xiangmumulu).State = EntityState.Modified;
                    db.SaveChanges();
                    return Json(new { success = true, errorMsg = "编辑目录保存成功！" }, "text/html");
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, errorMsg = ex.ToString() }, "text/html");
                }
            }
            //底下这个方法更简洁
            return Json(new {success=false, errorMsg = "some error occured." });
        }


        [HttpPost]
        // [ValidateAntiForgeryToken]     这种方式获取不到数据
        public JsonResult delMulu(FormCollection form)
        {
            string muluname = form["muluname"];   //删除实际就是按主键类型，查找主键，删除主键记录

            if (mulucustom.muluisempty(muluname))
            { 
                try
                {
                    xiangmumulu niandu = db.xiangmumulus.Find(muluname);
                    db.xiangmumulus.Remove(niandu);
                    db.SaveChanges();
                    return Json(new { success = true, errorMsg="删除目录成功！" }, "text/html");
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, errorMsg = ex.ToString() }, "text/html"); 
                }
            }
            else
            {
                return Json(new { success = false, errorMsg = "该目录下的内容不为空，不能删除" }, "text/html");
              
            }
        }
    }
}