using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using xmkgl.DAL;
using PagedList;
using xmkgl.Models;
using System.Data.Entity;


namespace xmkgl.Controllers
{
    [Authorize(Roles = "业务专员")]
    public class bmshezhiController : Controller
    {
        private cwcxmkContent db = new cwcxmkContent();
        // GET: bmshezhi
        
        public ActionResult bmshezhiIndex()
        {
            //为所辖部门获取部门设置列表
            var GenreLst = new List<string>();

            var GenreQry = from d in db.bumenxingzhis
                           orderby d.ID
                           select d.xingzhi;
            GenreLst.AddRange(GenreQry.Distinct());
            //ViewBag.zxdanwei = new SelectList(GenreLst);
            ViewBag.xingzhi = GenreLst;

            return View();
        }        

        //在注册用户时，判断是否存在同名用户，存在就返回false,不存在就返回true
        [HttpPost]
        public string checkNameIsSame(string name)
        {
            string isOk = "False";
            var xm = db.bumenshezhis.Where(x => x.BmName == name).Select(x => x.BmName);

            if (!xm.Any())   // 不存在同名用户 ，就返回 true
            {
                isOk = "True";
            }
            return isOk + "";
        }

        [HttpPost]
        public JsonResult getBumen(string searchquery)
        {
            int page = (Request.Form["page"] != null) ? Int32.Parse(Request.Form["page"]) : 1;
            int rows = (Request.Form["rows"] != null) ? Int32.Parse(Request.Form["rows"]) : 10;
            var xm = from s in db.bumenshezhis
                        where s.BmName.Contains(searchquery)
                        orderby s.BmName
                        select new { BmName = s.BmName, Bmxingzhi = s.Bmxingzhi };
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
        public JsonResult saveBumen([Bind(Include = "BmName,Bmxingzhi")]bumenshezhi bumenshezhi)
        {          
            if (ModelState.IsValid)
            {
                bumenshezhi.pingweifou = false;             
                try
                {           
                    db.bumenshezhis.Add(bumenshezhi);
                    db.SaveChanges();
                    return Json(new { success = true, errorMsg = "新增部门成功！" }, "text/html");
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, errorMsg = ex.ToString() }, "text/html");
                }
            }
            //底下这个方法更简洁
            return Json(new { success = false, errorMsg = "新增部门失败，请再试一试！" }, "text/html");
        }


        [HttpPost]
        public JsonResult updateBumen(string bmname)  //string bmname 好像多余
        {
            var bumen = db.bumenshezhis.Where(s => s.BmName == bmname).FirstOrDefault();
            bumen.Bmxingzhi = Request.Form["Bmxingzhi"];

            if (ModelState.IsValid)
            {
                try
                {
                    db.Entry(bumen).State = EntityState.Modified;
                    db.SaveChanges();

                    return Json(new { success = true, errorMsg = "更改部门信息成功！" }, "text/html");
                    //return Json(bumen1);                 
                }
                catch (Exception ex)
                {
                    return Json(new { success = false,errorMsg = ex.ToString() }, "text/html");
                }
            }
            //底下这个方法更简洁
            return Json(new { success = false, errorMsg = "更改部门信息失败，请再试一试！" }, "text/html");
        }


        [HttpPost]
        // [ValidateAntiForgeryToken]     这种方式获取不到数据
        public JsonResult delBumen(FormCollection form,string bmname)
        {
            bumenshezhi bumen = db.bumenshezhis.Find(bmname);
            try
            {               
                db.bumenshezhis.Remove(bumen);
                db.SaveChanges();
                return Json(new { success = true });
                //return Json(bumen);
            }
            catch (Exception ex)
            {
                return Json(new { errorMsg = ex.ToString() }, "text/html"); ;
            }
        }
    }
}