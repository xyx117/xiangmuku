using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using xmkgl.DAL;
using PagedList;
using xmkgl.Models;
using System.Data.Entity;

namespace xmkgl.Controllers
{
    [Authorize(Roles = "业务专员")]
    public class bumenxingzhiController : Controller
    {
        private cwcxmkContent db = new cwcxmkContent();
        
        // GET: /bumenxingzhi/
        public ActionResult xingzhiindex()
        {
            return View();
        }


        [HttpPost]
        public JsonResult getxingzhi()
        {            
            int page = (Request.Form["page"] != null) ? Int32.Parse(Request.Form["page"]) : 1;
            int rows = (Request.Form["rows"] != null) ? Int32.Parse(Request.Form["rows"]) : 10;

            var xm = from s in db.bumenxingzhis
                     orderby s.ID
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
        public JsonResult savexingzhi([Bind(Include = "xingzhi")]bumenxingzhi bumenxingzhi)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    db.bumenxingzhis.Add(bumenxingzhi);
                    db.SaveChanges();                    
                    return Json(new { success = true, errorMsg = "保存部门性质成功！" }, "text/html");
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, errorMsg = ex.ToString() }, "text/html");
                }
            }
            //底下这个方法更简洁
            return Json(new { success = false, errorMsg = "保存部门行政失败！" }, "text/html");
        }
        

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public JsonResult updatexingzhi([Bind(Include = "ID,xingzhi")]bumenxingzhi bumenxingzhi)
        {            
            if (ModelState.IsValid)
            {
                try
                {
                    db.Entry(bumenxingzhi).State = EntityState.Modified;
                    db.SaveChanges();
                    return Json(new { success = true, errorMsg = "更改部门性质成功！" }, "text/html");                    
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, errorMsg = ex.ToString() }, "text/html");
                }
            }
            
            //底下这个方法更简洁
            return Json(new { success = false, errorMsg = "更改部门性质失败！" }, "text/html");
        }


        [HttpPost]
        // [ValidateAntiForgeryToken]     这种方式获取不到数据
        public JsonResult delxingzhi(int ID)
        {
           try
           {
                bumenxingzhi xingzhi = db.bumenxingzhis.Find(ID);
                db.bumenxingzhis.Remove(xingzhi);
                db.SaveChanges();
                return Json(new { success = true, errorMsg = "删除部门性质成功！" }, "text/html");                
           }
           catch (Exception ex)
           {
                return Json(new { success = false, errorMsg = ex.ToString() }, "text/html"); ;
           }
        }


        //存在就返回false,不存在就返回true
        [HttpPost]
        public String checkNameIsSame(String name)
        {
            string isOk = "False";
            var xm = db.bumenxingzhis.Where(x => x.xingzhi == name).Select(x=>x.xingzhi);         
            if(!xm.Any()){
               isOk = "True";
            }    
            return isOk+"";
        }
	}
}