using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using xmkgl.DAL;
using PagedList;
using xmkgl.Models;
using System.Data.Entity;
using System.IO;
using System.Data.OleDb;
using System.Data;
using System.Data.SqlClient;
using System.Transactions;
using System.Text;
using System.Net;
using xmkgl.common;
using DotNet.Highcharts;
using DotNet.Highcharts.Options;
using DotNet.Highcharts.Enums;
using DotNet.Highcharts.Helpers;
using System.Drawing;
using Aspose.Words.Saving;
using Aspose.Words;
using Aspose.Cells;
using System.Configuration;
using System.Xml;

namespace xmkgl.Controllers
{

    [Authorize]    //这里全部方法都需要身份验证
    public class xiangmuguanliController : Controller
    {
        // GET: xiangmuguanli
        private cwcxmkContent db = new cwcxmkContent();

        //存在就返回false,不存在就返回true
        [HttpPost]
        public string checkNameIsSame(string name, string mulu)
        {
            string isOk = "False";
            var xm = db.xiangmuguanlis.Where(x => x.XmName == name && x.Xiangmumulu == mulu).Select(x => x.XmName);
            if (!xm.Any())
            {
                isOk = "True";
            }
            return isOk + "";
        }


        [HttpPost]
        public ActionResult UpLoadProcessfile(string id, string name, string type, string lastModifiedDate, int size, HttpPostedFileBase file, int xmid, string xmname, string xmmulu)
        {
            if (Request.Files.Count == 0)
            {
                return Json(new { jsonrpc = 2.0, success = false, message = "请选择要上传的文件。", id = id }, "text/html");
            }

            string fileName;

            fileupload upload = new fileupload();

            if (file != null)
            {
                string filePath1 = string.Format("~/Uploads/{0}/{1}/", xmmulu, xmname);//通过参数组建一个路径格式的字符串

                // 文件上传后的保存路径
                string filePath = Server.MapPath(filePath1);//将路径格式的字符串通过函数转化为服务器路径
                if (!Directory.Exists(filePath))            //判断路径是否存在，如果不存在，则根据路径建立路径文件夹，这里只需要检验到文件夹
                {
                    Directory.CreateDirectory(filePath);
                }
                string fileseze = (file.ContentLength / 1024).ToString();

                fileName = Path.GetFileName(file.FileName);// 原始文件名称

                string fileExtension = Path.GetExtension(fileName); // 文件扩展名

                //string saveName = Guid.NewGuid().ToString() + fileExtension; // 保存文件名称

                string webPath = string.Format("~/Uploads/{0}/{1}/{2}", xmmulu, xmname, fileName);//这里只是字符串，判断参数，用于判断上传的文件是否存在，这里需要检验到四层目录

                string generateFilePath = Server.MapPath(webPath);//这里是真实的路径，由字符串转化为路径

                if (System.IO.File.Exists(generateFilePath))//判断文件是否存在，如果存在返回提示json字符串
                {
                    return Json(new { jsonrpc = 2.0, success = false, message = fileName + "这个文件已经存在！保存失败", id = id }, "text/html");
                }

                try
                {
                    file.SaveAs(generateFilePath);//saveas保存的参数是服务器根路径，webpath不是路径   通过路径和源文件名做参数，保存上传文件                    

                    upload.filepath = string.Format("Uploads/{0}/{1}/", xmmulu, xmname);
                    upload.filename = fileName;
                    upload.fileextension = fileExtension;
                    //upload.savename = saveName;
                    upload.xiangmuguanliID = xmid;          //这里要给xiangmuguanliID赋值，存在一对多关系

                    upload.uploadtime = DateTime.Now.ToString("yyyy-MM-dd HH：mm：ss");

                    upload.filesize = fileseze + "kb";

                    db.fileuploads.Add(upload);
                    db.SaveChanges();

                    //return Json(new { Success = true, Message = fileName + "上传成功！" }, JsonRequestBehavior.AllowGet);  //, FileName = fileName 
                    return Json(new { jsonrpc = "2.0", success = true, id = id, filePath = webPath }, "text/html");
                }
                catch (Exception ex)
                {
                    if (System.IO.File.Exists(generateFilePath))//判断文件是否存在，如果存在返回提示json字符串
                    {
                        System.IO.File.Delete(generateFilePath);   //报错的时候，删除已经上传的文件
                    }
                    return Json(new { jsonrpc = 2.0, success = false, message = ex.Message, id = id }, "text/html");
                }
            }
            else
            {
                return Json(new { jsonrpc = 2.0, success = false, message = "请选择要上传的文件！", id = id }, "text/html");
            }
        }


        [Authorize(Roles = "部门负责人,员工,评委,业务专员")]
        public JsonResult Upload1(HttpPostedFileBase fileData)
        {
            string xmmulu = Request.Form["xmmulu"];
            int xmID = Convert.ToInt32(Request.Form["xmID"]);
            string xmname = Request.Form["xmname"];

            fileupload upload = new fileupload();

            if (fileData != null)
            {
                string filePath1 = string.Format("~/Uploads/{0}/{1}/", xmmulu, xmname);//通过参数组建一个路径格式的字符串

                // 文件上传后的保存路径
                string filePath = Server.MapPath(filePath1);//将路径格式的字符串通过函数转化为服务器路径
                if (!Directory.Exists(filePath))            //判断路径是否存在，如果不存在，则根据路径建立路径文件夹，这里只需要检验到文件夹
                {
                    Directory.CreateDirectory(filePath);
                }
                string fileseze = (fileData.ContentLength / 1024).ToString();

                string fileName = Path.GetFileName(fileData.FileName);// 原始文件名称

                string fileExtension = Path.GetExtension(fileName); // 文件扩展名

                string webPath = string.Format("~/Uploads/{0}/{1}/{2}", xmmulu, xmname, fileName);//这里只是字符串，判断参数，用于判断上传的文件是否存在，这里需要检验到四层目录

                string generateFilePath = Server.MapPath(webPath);//这里是真实的路径，由字符串转化为路径

                if (System.IO.File.Exists(generateFilePath))//判断文件是否存在，如果存在返回提示json字符串
                {
                    return Json(new { Success = false, Message = fileName + "这个文件已经存在！" }, JsonRequestBehavior.AllowGet);
                }

                try
                {
                    fileData.SaveAs(generateFilePath);//saveas保存的参数是服务器根路径，webpath不是路径   通过路径和源文件名做参数，保存上传文件

                    upload.filepath = string.Format("Uploads/{0}/{1}/", xmmulu, xmname);
                    upload.filename = fileName;
                    upload.fileextension = fileExtension;
                    //upload.savename = saveName;
                    upload.xiangmuguanliID = xmID;          //这里要给xiangmuguanliID赋值，存在一对多关系

                    upload.uploadtime = DateTime.Now.ToString("yyyy-MM-dd HH：mm：ss");

                    upload.filesize = fileseze + "kb";

                    db.fileuploads.Add(upload);
                    db.SaveChanges();

                    //using (TransactionScope transaction = new TransactionScope())//原子操作，事物错误回滚
                    //{

                    //    transaction.Complete();
                    //}

                    return Json(new { Success = true, Message = fileName + "上传成功！" }, JsonRequestBehavior.AllowGet);  //, FileName = fileName 
                }
                catch (Exception ex)
                {
                    if (System.IO.File.Exists(generateFilePath))//判断文件是否存在，如果存在返回提示json字符串
                    {
                        System.IO.File.Delete(generateFilePath);
                    }
                    return Json(new { Success = false, Message = ex.Message }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(new { Success = false, Message = "请选择要上传的文件！" }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 业务专员 提取 项目中的所有文件 转存到 自定义目录
        /// </summary>
        /// <param name="mulu"></param>
        /// <param name="desDir">自定义目录</param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "业务专员")]
        public JsonResult tiqufujian(string mulu, string desDir)
        {
            var xmname = db.xiangmuguanlis.Where(s => s.Xiangmumulu == mulu && s.ywzyshenhe == "通过").Select(s => s.XmName);
            string jieguo;
            foreach (var item in xmname)
            {
                jieguo = CopyDir(mulu, item.ToString(), desDir);   // 调用后面的方法，保存到自定义路径

                if (jieguo != "ok!")
                {
                    desDir = Server.MapPath(string.Format("~/tiqufujian/{0}/", desDir));

                    if (Directory.Exists(desDir))
                    {
                        Directory.Delete(desDir, true);
                    }
                    return Json(new { success = false, errorMsg = jieguo }, "text/html");
                }
            }
            return Json(new { success = true, errorMsg = "附件提取成功！" }, "text/html");
        }

        /// <summary>
        /// 拷贝oldlab的文件到newlab下面
        /// </summary>
        /// <param name="sourcePath">lab文件所在目录(@"~\labs\oldlab")</param>
        /// <param name="savePath">保存的目标目录(@"~\labs\newlab")</param>
        /// <returns>返回:true-拷贝成功;false:拷贝失败</returns>
        public string CopyDir(string mulu, string sourcePath, string savePath)
        {
            sourcePath = Server.MapPath(string.Format("~/Uploads/{0}/{1}", mulu, sourcePath));  // 服务器路径

            savePath = Server.MapPath(string.Format("~/tiqufujian/{0}/", savePath));  // 服务器路径
            try
            {
                if (!Directory.Exists(savePath))   // 判断路径是否存在，否则创建
                {
                    Directory.CreateDirectory(savePath);
                }

                if (Directory.Exists(sourcePath))
                {
                    string[] labFiles = Directory.GetFiles(sourcePath);//文件

                    if (labFiles.Length > 0)
                    {
                        for (int i = 0; i < labFiles.Length; i++)
                        {
                            System.IO.File.Copy(sourcePath + "\\" + Path.GetFileName(labFiles[i]), savePath + "\\" + Path.GetFileName(labFiles[i]), true);

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
            return "ok!";
        }


        //这里是由home/getmuludate跳转过来的
        public ActionResult xiangmugualiIndex(string muluName, string fgldshenhefou, string loingid, DateTime jsshijian)
        {
            ViewBag.muluName = muluName;
            ViewBag.shenhefou = fgldshenhefou;//判断分管领导是否审核
            //ViewBag.ksshijian = ksshijian;
            ViewBag.jsshijian = jsshijian;

            string loingrole = customidentity.userrole(loingid);

            //导出分年度预算表需要登录人  所属部门，作为条件筛选
            //string suoshuxueyuan = customidentity.suoshuxueyuan(loingid);

            if (fgldshenhefou == "审核")
            {
                switch (loingrole)
                {
                    case "业务专员": return View("shenhe_ywzy");

                    case "分管领导":
                        {
                            //筛选分管领导所属部门
                            string suoshuxueyuan = customidentity.suoshuxueyuan(loingid);

                            List<string> fgbmlist = new List<string>();

                            string[] bm = suoshuxueyuan.Split(',');
                            fgbmlist.Add("所有分管部门");

                            for (var i = 0; i < bm.Length; i++)
                            {
                                fgbmlist.Add(bm[i]);
                            }
                            ViewBag.fgld_sx_bumen = fgbmlist;
                            return View("shenhe_fgld");
                        };
                    case "财务主管": return View("shenhe_cwzg");

                    case "领导": return View("shenhe_ld");

                    case "部门负责人": //return View("shenhe_bmfzr");
                        {
                            //这里是为部门负责人更改项目目录时，提供目录列表数据
                            var mululist = new List<string>();
                            var muluQury = from d in db.xiangmumulus.Where(s => s.Jieshushijian >= DateTime.Now)
                                           select d.Name;
                            mululist.AddRange(muluQury.Distinct());
                            ViewBag.mululist = mululist;
                            return View("shenhe_bmfzr");
                        };
                    case "员工":     //return View("shenhe_yg");
                        {
                            return View("shenhe_yg");
                        };
                    case "评委":
                        return View("shenhe_pw");
                }
            }
            switch (loingrole)
            {
                case "业务专员": return View("bushen_ywzy");

                case "分管领导":     //return View("bushen_fgld");
                    {

                        //筛选分管领导所属部门
                        string suoshuxueyuan = customidentity.suoshuxueyuan(loingid);

                        List<string> fgbmlist = new List<string>();

                        string[] bm = suoshuxueyuan.Split(',');
                        fgbmlist.Add("所有分管部门");

                        for (var i = 0; i < bm.Length; i++)
                        {
                            fgbmlist.Add(bm[i]);
                        }
                        ViewBag.fgld_sx_bumen = fgbmlist;
                        return View("bushen_fgld");
                    };
                case "财务主管": return View("bushen_cwzg");

                case "领导": return View("bushen_ld");

                case "部门负责人": //return View("bushen_bmfzr");
                    {
                        //这里是为部门负责人更改项目目录时，提供目录列表数据
                        var mululist = new List<string>();
                        var muluQury = from d in db.xiangmumulus.Where(s => s.Jieshushijian >= DateTime.Now)
                                       select d.Name;
                        mululist.AddRange(muluQury.Distinct());

                        //导出分年度预算表需要登录人  所属部门，作为条件筛选
                        //ViewBag.suoshuxueyuan = customidentity.suoshuxueyuan(loingid);

                        ViewBag.mululist = mululist;
                        return View("bushen_bmfzr");
                    };

                case "评委": return View("bushen_pw");
            }
            //这里是为部门负责人更改项目目录时，提供目录列表数据
            var mululist1 = new List<string>();
            var muluQury1 = from d in db.xiangmumulus.Where(s => s.Jieshushijian >= DateTime.Now)
                            select d.Name;
            mululist1.AddRange(muluQury1.Distinct());

            ViewBag.mululist = mululist1;
            return View("bushen_yg");
        }


        //不审核页面，评委分离，待审页面
        [Authorize(Roles = "评委")]
        public ActionResult bushen_pw_daishen(string muluName, string time, string loingid)
        {
            //筛选分管领导所属部门
            string suoshuxueyuan = customidentity.suoshuxueyuan(loingid);

            List<string> fgbmlist = new List<string>();

            string[] bm = suoshuxueyuan.Split(',');
            fgbmlist.Add("所有评审部门");

            for (var i = 0; i < bm.Length; i++)
            {
                fgbmlist.Add(bm[i]);
            }
            ViewBag.fgld_sx_bumen = fgbmlist;

            ViewBag.muluName = muluName;
            return View();
        }


        //不审核页面，评委分离，已审核页面
        [Authorize(Roles = "评委")]
        public ActionResult bushen_pw_yishen(string muluName, string time, string loingid)
        {
            //筛选分管领导所属部门
            string suoshuxueyuan = customidentity.suoshuxueyuan(loingid);

            List<string> fgbmlist = new List<string>();
            //这里应该还需要判断一下，取出来的  suoshubumen  是否只有一个
            if (suoshuxueyuan.Contains(","))
            {
                string[] bm = suoshuxueyuan.Split(',');
                fgbmlist.Add("所有评审部门");

                for (var i = 0; i < bm.Length; i++)
                {
                    fgbmlist.Add(bm[i]);
                }
                ViewBag.fgld_sx_bumen = fgbmlist;
                ViewBag.muluName = muluName;
                return View();
            }
            else
            {
                ViewBag.fgld_sx_bumen = suoshuxueyuan;
                ViewBag.muluName = muluName;
                return View();
            }
        }


        //审核页面，评委分离，待审页面
        [Authorize(Roles = "评委")]
        public ActionResult shenhe_pw_daishen(string muluName, string time, string loingid)//
        {
            //筛选分管领导所属部门
            string suoshuxueyuan = customidentity.suoshuxueyuan(loingid);

            List<string> fgbmlist = new List<string>();

            string[] bm = suoshuxueyuan.Split(',');
            fgbmlist.Add("所有评审部门");

            for (var i = 0; i < bm.Length; i++)
            {
                fgbmlist.Add(bm[i]);
            }
            ViewBag.fgld_sx_bumen = fgbmlist;


            ViewBag.muluName = muluName;
            return View();

        }

        //审核页面，评委分离，已审核页面
        [Authorize(Roles = "评委")]
        public ActionResult shenhe_pw_yishen(string muluName, string time, string loingid)//
        {
            //筛选分管领导所属部门
            string suoshuxueyuan = customidentity.suoshuxueyuan(loingid);

            List<string> fgbmlist = new List<string>();

            string[] bm = suoshuxueyuan.Split(',');
            fgbmlist.Add("所有评审部门");

            for (var i = 0; i < bm.Length; i++)
            {
                fgbmlist.Add(bm[i]);
            }
            ViewBag.fgld_sx_bumen = fgbmlist;

            ViewBag.muluName = muluName;
            return View();
        }


        //审核页面，财务主管分离，通过页面
        [Authorize(Roles = "财务主管")]
        public ActionResult shenhe_cwzg_tongguo(string muluName)
        {
            //筛选分管领导所属部门
            var GenreLst = new List<string>();
            var GenreQry = from d in db.bumenshezhis
                           orderby d.BmName
                           select d.BmName;
            GenreLst.Add("所有部门");
            GenreLst.AddRange(GenreQry.Distinct());
            ViewBag.fgld_sx_bumen = GenreLst;

            ViewBag.muluName = muluName;
            return View();
        }

        //审核页面，财务主管分离，不通过页面
        [Authorize(Roles = "财务主管")]
        public ActionResult shenhe_cwzg_weitongguo(string muluName)
        {
            //筛选分管领导所属部门
            var GenreLst = new List<string>();
            var GenreQry = from d in db.bumenshezhis
                           orderby d.BmName
                           select d.BmName;
            GenreLst.Add("所有部门");
            GenreLst.AddRange(GenreQry.Distinct());
            ViewBag.fgld_sx_bumen = GenreLst;

            ViewBag.muluName = muluName;
            return View();
        }

        //分管领导不审核页面，财务主管分离，未审核页面
        [Authorize(Roles = "财务主管")]
        public ActionResult shenhe_cwzg_weishenhe(string muluName)
        {
            //筛选分管领导所属部门
            var GenreLst = new List<string>();
            var GenreQry = from d in db.bumenshezhis
                           orderby d.BmName
                           select d.BmName;
            GenreLst.Add("所有部门");
            GenreLst.AddRange(GenreQry.Distinct());
            ViewBag.fgld_sx_bumen = GenreLst;

            ViewBag.muluName = muluName;
            return View();
        }


        //分管领导不审核页面，财务主管分离，通过页面
        [Authorize(Roles = "财务主管")]
        public ActionResult bushen_cwzg_tongguo(string muluName)
        {
            //筛选分管领导所属部门
            var GenreLst = new List<string>();
            var GenreQry = from d in db.bumenshezhis
                           orderby d.BmName
                           select d.BmName;
            GenreLst.Add("所有部门");
            GenreLst.AddRange(GenreQry.Distinct());
            ViewBag.fgld_sx_bumen = GenreLst;

            ViewBag.muluName = muluName;
            return View();
        }

        //分管领导不审核页面，财务主管分离，不通过页面
        [Authorize(Roles = "财务主管")]
        public ActionResult bushen_cwzg_weitongguo(string muluName)
        {
            //筛选分管领导所属部门
            var GenreLst = new List<string>();
            var GenreQry = from d in db.bumenshezhis
                           orderby d.BmName
                           select d.BmName;
            GenreLst.Add("所有部门");
            GenreLst.AddRange(GenreQry.Distinct());
            ViewBag.fgld_sx_bumen = GenreLst;

            ViewBag.muluName = muluName;
            return View();
        }


        //审核页面，财务主管分离，未审核页面
        [Authorize(Roles = "财务主管")]
        public ActionResult bushen_cwzg_weishenhe(string muluName)
        {
            //筛选分管领导所属部门
            var GenreLst = new List<string>();
            var GenreQry = from d in db.bumenshezhis
                           orderby d.BmName
                           select d.BmName;
            GenreLst.Add("所有部门");
            GenreLst.AddRange(GenreQry.Distinct());
            ViewBag.fgld_sx_bumen = GenreLst;

            ViewBag.muluName = muluName;
            return View();

        }


        //分管领导审核，业务专员审核通过
        [Authorize(Roles = "业务专员")]
        public ActionResult shenhe_ywzy_tongguo(string muluName, string time)
        {
            //筛选分管领导所属部门
            var GenreLst = new List<string>();
            var GenreQry = from d in db.bumenshezhis
                           orderby d.BmName
                           select d.BmName;
            GenreLst.Add("所有部门");
            GenreLst.AddRange(GenreQry.Distinct());
            ViewBag.fgld_sx_bumen = GenreLst;

            ViewBag.muluName = muluName;
            return View();

        }

        //分管领导审核，业务专员审核不通过
        [Authorize(Roles = "业务专员")]
        public ActionResult shenhe_ywzy_weitongguo(string muluName, string time)
        {
            //筛选分管领导所属部门
            var GenreLst = new List<string>();
            var GenreQry = from d in db.bumenshezhis
                           orderby d.BmName
                           select d.BmName;
            GenreLst.Add("所有部门");
            GenreLst.AddRange(GenreQry.Distinct());
            ViewBag.fgld_sx_bumen = GenreLst;

            ViewBag.muluName = muluName;
            return View();

        }

        //分管领导审核，业务专员未审核
        [Authorize(Roles = "业务专员")]
        public ActionResult shenhe_ywzy_weishenhe(string muluName, string time)
        {
            //筛选分管领导所属部门
            var GenreLst = new List<string>();
            var GenreQry = from d in db.bumenshezhis
                           orderby d.BmName
                           select d.BmName;
            GenreLst.Add("所有部门");
            GenreLst.AddRange(GenreQry.Distinct());
            ViewBag.fgld_sx_bumen = GenreLst;

            ViewBag.muluName = muluName;
            return View();

        }


        //分管领导审核，业务专员导出部门负责人送审项目
        [Authorize(Roles = "业务专员")]
        public ActionResult shenhe_ywzy_bm_tj(string muluName, string time)
        {
            //筛选分管领导所属部门
            var GenreLst = new List<string>();
            var GenreQry = from d in db.bumenshezhis
                           orderby d.BmName
                           select d.BmName;
            GenreLst.Add("所有部门");
            GenreLst.AddRange(GenreQry.Distinct());
            ViewBag.fgld_sx_bumen = GenreLst;

            ViewBag.muluName = muluName;
            return View();

        }



        //分管领导审核，业务专员待审核
        [Authorize(Roles = "业务专员")]
        public ActionResult shenhe_ywzy_daifenpei(string muluName, string time)
        {
            //筛选分管领导所属部门

            var GenreLst = new List<string>();
            var GenreQry = from d in db.bumenshezhis
                           orderby d.BmName
                           select d.BmName;
            GenreLst.Add("所有部门");
            GenreLst.AddRange(GenreQry.Distinct());
            ViewBag.fgld_sx_bumen = GenreLst;
            ViewBag.muluName = muluName;
            return View();
        }


        //业务专员查看部门负责人提交的  项目 进度  柱形图
        [Authorize(Roles = "业务专员,财务主管")]
        public ActionResult shenhe_ywzy_tjjd(string muluName, string time)
        {
            var bmname = db.bumenshezhis.OrderByDescending(a => a.Bmxingzhi).Select(a => a.BmName);

            if (bmname.Any())                                  //判断是否有部门存在，没有则转到数据错误页面
            {
                int bmshuliang = bmname.Count();
                string[] y_categories = new string[bmshuliang];  //声明一个部门总数位数的数组

                y_categories = bmname.ToArray();              //由部门名称数组化

                //object[] chartvalues = new object[bmshuliang];   //声明一个部门总数的对象数组

                object[] yjtianbao = new object[bmshuliang];
                object[] bmfzryjtijiao = new object[bmshuliang];
                object[] fgldyjshenhe = new object[bmshuliang];


                for (int i = 0; i < bmshuliang; i++)
                {
                    string xueyuan = y_categories[i];
                    //各部门已经填报的项目的个数
                    yjtianbao[i] = db.xiangmuguanlis.Where(s => s.suoshuxueyuan == xueyuan && s.Xiangmumulu == muluName && s.jpxx == "1" && s.lxpj == "1" && s.cccx == "1" && s.ccjx == "1" && s.xljx == "1" && s.csmx == "1" && s.cgpz == "1" && s.nianduyusuan == "1").Count();

                    //各部门负责人已经提交的项目的个数
                    bmfzryjtijiao[i] = db.xiangmuguanlis.Where(s => s.suoshuxueyuan == xueyuan && s.Xiangmumulu == muluName && s.bmfzrtijiao == "已提交").Count();

                    //各分管领导人已经审核的项目的个数
                    fgldyjshenhe[i] = db.xiangmuguanlis.Where(s => s.suoshuxueyuan == xueyuan && s.Xiangmumulu == muluName && s.fgldshenhe == "通过").Count();
                }
                Highcharts chart = new Highcharts("chart")
                   .InitChart(new DotNet.Highcharts.Options.Chart { DefaultSeriesType = ChartTypes.Column, Width = 2000 })
                   .SetTitle(new DotNet.Highcharts.Options.Title { Text = "申报进度表" })
                   //.SetSubtitle(new Subtitle { Text = "Source: Wikipedia.org" })
                   .SetXAxis(new XAxis
                   {
                       Categories = y_categories,
                       Title = new XAxisTitle { Text = string.Empty },
                       Labels = new XAxisLabels { Rotation = 90 }
                   })
                   .SetYAxis(new YAxis
                   {
                       Title = new YAxisTitle
                       {
                           Text = "数量",
                           Align = AxisTitleAligns.High
                       }
                   })
                   .SetTooltip(new Tooltip
                   {
                       HeaderFormat = "<span style=\"font-size:10px\">{point.key}</span><table>",
                       PointFormat = "<tr><td style=\"color:{series.color}\"} >{series.name}:</td><td style=\"padding:0px\"><b>{point.y}个</b></td></tr>",
                       FooterFormat = "</table>",
                       Shared = true,
                       UseHTML = true
                   })

                   .SetCredits(new Credits { Enabled = false })
                   .SetSeries(new[]
                   {
                        new Series { Name = "已填表项目", Data =new Data(yjtianbao) },

                        new Series { Name = "已送审项目", Data = new Data(bmfzryjtijiao) },

                        new Series { Name = "分管领导已审核项目", Data = new Data(fgldyjshenhe) }
                   });
                return View(chart);
            }
            else
            {
                return View("basicbar_error");
            }
        }

        //项目评审进度
        [Authorize(Roles = "业务专员,财务主管")]
        public ActionResult bushen_ywzy_tjjd(string muluName, string time)
        {
            var bmname = db.bumenshezhis.OrderByDescending(a => a.Bmxingzhi).Select(a => a.BmName);

            if (bmname.Any())                                  //判断是否有部门存在，没有则转到数据错误页面
            {
                int bmshuliang = bmname.Count();
                string[] y_categories = new string[bmshuliang];  //声明一个部门总数位数的数组

                y_categories = bmname.ToArray();              //由部门名称数组化

                //object[] chartvalues = new object[bmshuliang];   //声明一个部门总数的对象数组

                object[] yjtianbao = new object[bmshuliang];
                object[] bmfzryjtijiao = new object[bmshuliang];
                //object[] fgldyjshenhe = new object[bmshuliang];


                for (int i = 0; i < bmshuliang; i++)
                {
                    string xueyuan = y_categories[i];
                    //各部门已经填报的项目的个数
                    yjtianbao[i] = db.xiangmuguanlis.Where(s => s.suoshuxueyuan == xueyuan && s.Xiangmumulu == muluName && s.jpxx == "1" && s.lxpj == "1" && s.cccx == "1" && s.ccjx == "1" && s.xljx == "1" && s.csmx == "1" && s.cgpz == "1" && s.nianduyusuan == "1").Count();


                    //各部门负责人已经提交的项目的个数
                    bmfzryjtijiao[i] = db.xiangmuguanlis.Where(s => s.suoshuxueyuan == xueyuan && s.Xiangmumulu == muluName && s.bmfzrtijiao == "已提交").Count();

                    //各分管领导人已经审核的项目的个数
                    //fgldyjshenhe[i] = db.xiangmuguanlis.Where(s => s.suoshuxueyuan == xueyuan && s.Xiangmumulu == muluName && s.fgldshenhe == "通过").Count();
                }
                Highcharts chart = new Highcharts("chart")
                    .InitChart(new DotNet.Highcharts.Options.Chart { DefaultSeriesType = ChartTypes.Column, Width = 2000 })
                    .SetTitle(new DotNet.Highcharts.Options.Title { Text = "申报进度表" })
                    //.SetSubtitle(new Subtitle { Text = "Source: Wikipedia.org" })
                    .SetXAxis(new XAxis
                    {
                        //Categories = new [] { "Africa", "America", "Asia", "Europe", "Oceania" },
                        Categories = y_categories,
                        Title = new XAxisTitle { Text = string.Empty },
                        Labels = new XAxisLabels { Rotation = 90 }
                    })
                    .SetYAxis(new YAxis
                    {
                        Title = new YAxisTitle
                        {
                            Text = "数量",
                            Align = AxisTitleAligns.High
                        }
                    })
                    .SetTooltip(new Tooltip
                    {
                        HeaderFormat = "<span style=\"font-size:10px\">{point.key}</span><table>",
                        PointFormat = "<tr><td style=\"color:{series.color}\"} >{series.name}:</td><td style=\"padding:0px\"><b>{point.y}个</b></td></tr>",
                        FooterFormat = "</table>",
                        Shared = true,
                        UseHTML = true
                    })
                    .SetCredits(new Credits { Enabled = false })
                    .SetSeries(new[]
                {                    
                    new Series { Name = "已填表项目", Data =new Data( yjtianbao) },
                    new Series { Name = "已送审项目", Data = new Data(bmfzryjtijiao) }                    
                });
                //ViewBag.ChartModel = chart;
                return View(chart);
            }
            else
            {
                return View("basicbar_error");
            }
        }


        //分管领导不审核，业务专员审核通过
        [Authorize(Roles = "业务专员")]
        public ActionResult bushen_ywzy_tongguo(string muluName, string time)
        {
            //筛选分管领导所属部门
            var GenreLst = new List<string>();
            var GenreQry = from d in db.bumenshezhis
                           orderby d.BmName
                           select d.BmName;
            GenreLst.Add("所有部门");
            GenreLst.AddRange(GenreQry.Distinct());
            ViewBag.fgld_sx_bumen = GenreLst;

            ViewBag.muluName = muluName;
            return View();
        }

        //分管领导不审核，业务专员审核不通过
        [Authorize(Roles = "业务专员")]
        public ActionResult bushen_ywzy_weitongguo(string muluName, string time)
        {
            //筛选分管领导所属部门
            var GenreLst = new List<string>();
            var GenreQry = from d in db.bumenshezhis
                           orderby d.BmName
                           select d.BmName;
            GenreLst.Add("所有部门");
            GenreLst.AddRange(GenreQry.Distinct());
            ViewBag.fgld_sx_bumen = GenreLst;

            ViewBag.muluName = muluName;
            return View();
        }

        //分管领导不审核，业务专员未审核
        [Authorize(Roles = "业务专员")]
        public ActionResult bushen_ywzy_weishenhe(string muluName, string time)
        {
            //筛选分管领导所属部门
            var GenreLst = new List<string>();
            var GenreQry = from d in db.bumenshezhis
                           orderby d.BmName
                           select d.BmName;
            GenreLst.Add("所有部门");
            GenreLst.AddRange(GenreQry.Distinct());
            ViewBag.fgld_sx_bumen = GenreLst;

            ViewBag.muluName = muluName;
            return View();
        }


        //分管领导不审核，业务专员未审核
        [Authorize(Roles = "业务专员")]
        public ActionResult bushen_ywzy_bm_tj(string muluName, string time)
        {
            //筛选分管领导所属部门
            var GenreLst = new List<string>();
            var GenreQry = from d in db.bumenshezhis
                           orderby d.BmName
                           select d.BmName;
            GenreLst.Add("所有部门");
            GenreLst.AddRange(GenreQry.Distinct());
            ViewBag.fgld_sx_bumen = GenreLst;

            ViewBag.muluName = muluName;
            return View();
        }

        //分管领导不审核，业务专员待审核
        [Authorize(Roles = "业务专员")]
        public ActionResult bushen_ywzy_daifenpei(string muluName, string time)
        {
            //筛选分管领导所属部门
            var GenreLst = new List<string>();
            var GenreQry = from d in db.bumenshezhis
                           orderby d.BmName
                           select d.BmName;
            GenreLst.Add("所有部门");
            GenreLst.AddRange(GenreQry.Distinct());
            ViewBag.fgld_sx_bumen = GenreLst;

            ViewBag.muluName = muluName;
            return View();
        }


        private class mulu_yd
        {
            public string id { get; set; }
            public string text { get; set; }
        }

        [HttpPost]
        //移动项目时加载的目录列表，yxbm为已经选择的部门
        public JsonResult mululist(string xmmulu)
        {
            xmmulu = Server.UrlDecode(xmmulu);

            //为所辖部门获取部门设置列表
            var GenreLst = new List<mulu_yd>();
            var GenreQry = from d in db.xiangmumulus.Where(s => s.Jieshushijian >= DateTime.Now && s.Name != xmmulu)
                           orderby d.Jieshushijian
                           select new mulu_yd { id = d.Name, text = d.Name };
            GenreLst.AddRange(GenreQry.Distinct());
            return Json(GenreLst.ToList()); // JsonRequestBehavior.AllowGet

        }



        //部门负责人移动目录yidongmulu，这里只需要更改 ：创建时间，序号，及各种初始状态
        [HttpPost]
        [Authorize(Roles = "部门负责人,员工")]
        public JsonResult yidongmulu(int ID, string loingid)
        {
            //创建项目时添加项目日志内容
            string mulu = Request.Form["ydmulu"];

            string baoliufou = Request.Form["yidon_baoliufou"];

            var xiangmuguanli = db.xiangmuguanlis.Where(s => s.ID == ID).FirstOrDefault();

            var fennianduyusuan = db.xmnianduyusuans.Where(s => s.xiangmuguanliID == ID).FirstOrDefault();

            string mulu_lod = xiangmuguanli.Xiangmumulu;
            if (xiangmuguanli == null)
            {
                return Json(new { success = false, errorMsg = "该项目不存在，请再试一试！" }, "text/html");
            }

            var xm = db.xiangmuguanlis.Where(s => s.Xiangmumulu == mulu && s.XmName == xiangmuguanli.XmName);

            if (xm.Any())
            {
                return Json(new { success = false, errorMsg = "该目录下已经存在同名的项目，不能进行移动！" }, "text/html");
            }
            if (xiangmuguanli.ywzyshenhe == "未审核")  //这里是针对前端老是判断不出来  部门负责人 是否可以移动项目 后 另外添加的 后台判断，前端也有判断，后端双保险 
            {
                return Json(new { success = false, errorMsg = "该项目业务专员未审核，不能进行移动！" }, "text/html");
            }

            xiangmuguanli.Chuangjianshijian = DateTime.Now.ToString("yyyy-MM-dd");

            fennianduyusuan.qishiniandu = DateTime.Now.ToString("yyyy");   //移动目录时，项目管理表中的创建时间要更改为当前时间，分年度预算表中的  时间值  也要做出更改

            xiangmuguanli.Xiangmumulu = mulu;   //更改目录

            xiangmuguanli.Xuhao = 1;    //更改项目序号为  创建新项目时的序号

            //创建项目时赋值审核状态  Enum.GetValues(typeof (GenderType))
            xiangmuguanli.shenhezhuangtai = "未审核";

            xiangmuguanli.bmfzrshenhe = "未审核";
            xiangmuguanli.fgldshenhe = "未审核";
            xiangmuguanli.ywzyshenhe = "未审核";
            //xiangmuguanli.ldshenhe = "未审核";
            xiangmuguanli.pingweishenhe = "未审核";
            xiangmuguanli.pingweitijiao = "未提交";

            xiangmuguanli.tijiaozhuantai = "未提交";
            xiangmuguanli.bmfzrtijiao = "未提交";
            xiangmuguanli.fgldtijiao = "未提交";

            string rizhiname = customidentity.rizhiusername(loingid);
            string rizhirole = customidentity.rizhiuserrole(loingid);

            xmrizhi xmrizhi = new xmrizhi();
            xmrizhi.xiangmuguanliID = ID;
            xmrizhi.shenheshijian = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
            xmrizhi.shenhejiedian = "项目移动";
            xmrizhi.shenheren = rizhiname;
            xmrizhi.shenhejuese = rizhirole;
            xmrizhi.shenhejieguo = "由“" + mulu_lod + "”移动到“" + mulu + "”";
            xmrizhi.shenheyijian = "移动成功！";
            using (TransactionScope transaction = new TransactionScope())//原子操作，事物错误回滚
            {
                try
                {
                    db.Entry(xiangmuguanli).State = EntityState.Modified;
                    db.Entry(fennianduyusuan).State = EntityState.Modified;
                    db.xmrizhis.Add(xmrizhi);
                    db.SaveChanges();
                    transaction.Complete();
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, errorMsg = ex.ToString() }, "text/html");
                }
                finally
                {
                    transaction.Dispose();
                }
            }
            return Json(new { success = true, errorMsg = "更改目录已保存！" }, "text/html");
        }        


        //导出已经提交的项目，但是在导出项目前，检查项目是否已经排序，要按照项目的排序顺序拨钱
        [HttpPost]
        public JsonResult checkpaixu(string mulu, string loingid)
        {
            int[] all_xuhao;

            string xueyue = customidentity.suoshuxueyuan(loingid);

            ////这里为了避免linq缓存影响修改后的结果
            using (cwcxmkContent db1 = new cwcxmkContent()) //创建一个新的上下文
            {
                //这里在筛选时 加了一个，员工的提交状态为“已提交”，否则的话，这里会把员工创建的项目，也作为筛选范围
                all_xuhao = db1.xiangmuguanlis.Where(s => s.Xiangmumulu == mulu && s.suoshuxueyuan == xueyue && s.tijiaozhuantai == "已提交").Select(s => s.Xuhao).ToArray();
            }

            //去重数组的维数
            int all_xuhao_dis = all_xuhao.Distinct().Count();

            ////数组的维数
            int all_xuhao_cou = all_xuhao.Length;

            if (all_xuhao == null)
            {
                return Json(new { success = false, Msg = "目前还没有符合汇总的项目，请再试一试！" }, "text/html");
            }
            else
            {
                if (all_xuhao_dis != all_xuhao_cou)      //比较两个数组维数是否相同，就可判断是否有相同序号，要两个数组维数相等的情况下，才可能不存在有相同序号
                {
                    return Json(new { success = false, Msg = "在汇总导出前，请先对您单位的项目进行排序！" }, "text/html");
                }
            }
            return Json(new { success = true, Msg = "项目已排序！" }, "text/html");
        }

        /// <summary>  
        /// 表查询,取得datatable  
        /// </summary>  
        /// <param name="sql">由变量组成的  sql 查询字符串</param>  
        /// <returns>数据表</returns>  
        public static DataTable GetDataTable(string sql)
        {
            string connStr = ConfigurationManager.ConnectionStrings["cwcxmkContent"].ToString();//程序链接数据库字符串

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                SqlDataAdapter sqldataadapter = null;
                DataSet dataset = null;                
                try
                {
                    sqldataadapter = new SqlDataAdapter(sql, connStr);  //SqlDataAdapter是 DataSet和 SQL Server之间的桥接器。
                    dataset = new DataSet();    // DataSet当成内存中的数据库                    
                    sqldataadapter.Fill(dataset);                    
                }
                catch (System.Data.SqlClient.SqlException ex)
                {
                    throw new Exception(ex.Message);
                }
                return dataset.Tables[0];
            }
        }


        //导出分年度汇总表
        [Authorize(Roles = "业务专员,部门负责人,员工")]
        public ActionResult excel_out_huizong(string mulu, string loingid)  //这里的id为xiangmuguanli.cs表中的ID
        {         
            //取出登录人的所属部门
            string suoshuxueyuan = customidentity.suoshuxueyuan(loingid);

            //项目分年度预算
            var sql_ndys = @"select x.Beizhu,x.Xuhao,x.XmName,x.zongjine,y.diyinian,y.diernian,y.disannian from [xiangmuguanli] x,[xmnianduyusuan] y where x.ID=y.xiangmuguanliID and x.bmfzrtijiao='已提交' and x.Xiangmumulu='" + mulu + "' and x.suoshuxueyuan='" + suoshuxueyuan + "' order by x.Xuhao";

            var dt_ndys = GetDataTable(sql_ndys);

            dt_ndys.TableName = "huizongbiao";

            WorkbookDesigner designer = new WorkbookDesigner();

            //designer
            designer.Open(Server.MapPath("~/huizongbiao.xls"));

            designer.SetDataSource(dt_ndys);

            designer.Process();

            var byti = designer.Workbook.SaveToStream().GetBuffer();

            designer = null;

            var filename = "bmfzrhuizongbiao" + DateTime.Now.ToString("yyyyMMddhhmmss") + ".xls";
            //转换成流字节，输出浏览器下载

            ////通知浏览器保存文件，其实也就是输出到浏览器
            Response.Clear();
            Response.ContentType = "application/octet-stream";
            Response.AppendHeader("Content-Disposition", "attachment;filename=" + filename);
            Response.BinaryWrite(byti);
            Response.Flush();
            Response.Close();
            return new EmptyResult();
        }


        private class ywzy_huizong
        {
            public int xuhao { get; set; }
            public string xmname { get; set; }

            public string bumen { get; set; }

            public string beizhu { get; set; }

            public decimal diyinian { get; set; }

            public decimal diernian { get; set; }

            public decimal disannian { get; set; }

            //public string pwyijian { get; set; }
        }



        //导出分年度汇总表
        [Authorize(Roles = "业务专员,部门负责人,员工")]
        public ActionResult ywzy_exl_huizong(string mulu)  //这里的id为xiangmuguanli.cs表中的ID
        {
            var templatePath = Server.MapPath("~/ywzytongguohz.xlsx");

            Workbook workbook = new Workbook();
            workbook.Open(templatePath);
            Cells cells = workbook.Worksheets[0].Cells;

            //cells.SetRowHeight(0, 38); 

            Aspose.Cells.Style style1 = workbook.Styles[workbook.Styles.Add()];//新增样式 
            style1.Font.IsBold = true;//粗体 

            //样式2 
            Aspose.Cells.Style style2 = workbook.Styles[workbook.Styles.Add()];//新增样式 
            style2.HorizontalAlignment = TextAlignmentType.Center;//文字居中 
            style2.Font.IsBold = true;//粗体 
            style2.Font.Size = 12;//文字大小

            //样式3
            Aspose.Cells.Style style3 = workbook.Styles[workbook.Styles.Add()];//新增样式 
            style3.HorizontalAlignment = TextAlignmentType.Center;//文字居中 


            //样式4
            Aspose.Cells.Style style4 = workbook.Styles[workbook.Styles.Add()];//新增样式 
            style4.Font.Size = 12;//文字大小
            style4.IsTextWrapped = true;//单元格内容自动换行 

            //designer.Open(Server.MapPath("~/ywzy_huizongbiao.xls"));  //打开的模板


            //为所辖部门获取部门设置列表
            var GenreLst = new List<string>();
            var GenreQry = from d in db.bumenshezhis
                           orderby d.BmName
                           select d.BmName;
            GenreLst.AddRange(GenreQry.Distinct());

            decimal zongji_yi = 0;
            decimal zongji_er = 0;
            decimal zongji_san = 0;

            //ViewBag.bumen = GenreLst;
            if (GenreLst.Any())
            {

                int hang = 5;
                int daxuhao = 1;
                foreach (var item_bm in GenreLst)
                {
                    //提取出每个部门，以这个部门为条件，检索出与该部门相关的项目信息                

                    var GenreLst_bmxx = new List<ywzy_huizong>();

                    var bmxinxi = (from x in db.xiangmuguanlis
                                   join y in db.xmnianduyusuans
                                       on x.ID equals y.xiangmuguanliID
                                   where x.suoshuxueyuan == item_bm && x.Xiangmumulu == mulu && x.ywzyshenhe == "通过"
                                   orderby x.Xuhao
                                   select new ywzy_huizong
                                   {
                                       xuhao = x.Xuhao,
                                       xmname = x.XmName,
                                       bumen = x.suoshuxueyuan,
                                       beizhu = x.Beizhu,
                                       diyinian = y.diyinian / 10,
                                       diernian = y.diernian / 10,
                                       disannian = y.disannian / 10,
                                       //pwyijian = x.pingweiyijian
                                   });
                    GenreLst_bmxx.AddRange(bmxinxi);

                    if (GenreLst_bmxx.Any())
                    {
                        //合并单元格并写入部门名称 信息
                        //int sl = bmxinxi.Count(),1 );

                        //cells[hang ,1 ].PutValue(item_bm);

                        cells[hang, 0].PutValue(daxuhao);

                        cells[hang, 0].SetStyle(style2);

                        cells[hang, 1].PutValue(item_bm + "小计：");
                        cells[hang, 1].SetStyle(style1); //设置样式居中，粗体

                        int shuliangsta = hang + 2;
                        int shuliangend = hang + 1 + bmxinxi.Count();

                        cells[hang, 2].Formula = "=SUM(C" + shuliangsta + ":C" + shuliangend + ")";
                        cells[hang, 2].SetStyle(style2); //设置样式居中，粗体

                        cells[hang, 3].Formula = "=SUM(D" + shuliangsta + ":D" + shuliangend + ")";
                        cells[hang, 3].SetStyle(style2); //设置样式居中，粗体

                        cells[hang, 4].Formula = "=SUM(E" + shuliangsta + ":E" + shuliangend + ")";
                        cells[hang, 4].SetStyle(style2); //设置样式居中，粗体

                        int xiaoxuhao = 1;

                        foreach (var item_bmxx in GenreLst_bmxx)
                        {
                            hang = hang + 1;
                            //这样就可以提取出相应的信息了

                            cells[hang, 0].PutValue(daxuhao + "." + xiaoxuhao);
                            cells[hang, 0].SetStyle(style3); //设置样式居中

                            //cells[hang, 1].PutValue(item_bm);

                            cells[hang, 1].PutValue(item_bmxx.xmname);

                            cells[hang, 2].PutValue(item_bmxx.diyinian);
                            cells[hang, 2].SetStyle(style3); //设置样式居中

                            cells[hang, 3].PutValue(item_bmxx.diernian);
                            cells[hang, 3].SetStyle(style3); //设置样式居中

                            cells[hang, 4].PutValue(item_bmxx.disannian);
                            cells[hang, 4].SetStyle(style3); //设置样式居中


                            cells[hang, 5].PutValue(item_bmxx.beizhu);
                            cells[hang, 5].SetStyle(style4);

                            //cells[hang, 6].PutValue(item_bmxx.pwyijian);

                            zongji_yi = zongji_yi + item_bmxx.diyinian;
                            zongji_er = zongji_er + item_bmxx.diernian;
                            zongji_san = zongji_san + item_bmxx.disannian;

                            //hang = hang + 1;
                            xiaoxuhao = xiaoxuhao + 1;
                        }
                        hang = hang + 1;
                        daxuhao = daxuhao + 1;
                    }
                }
                cells[4, 2].PutValue(zongji_yi);
                cells[4, 3].PutValue(zongji_er);
                cells[4, 4].PutValue(zongji_san);
            }

            var filename = "ywzyhuizongbiao" + DateTime.Now.ToString("yyyyMMddhhmmss") + ".xls";
            //转换成流字节，输出浏览器下载
            var byti = workbook.SaveToStream().GetBuffer();
            ////通知浏览器保存文件，其实也就是输出到浏览器
            Response.Clear();
            Response.ContentType = "application/octet-stream";
            Response.AppendHeader("Content-Disposition", "attachment;filename=" + filename);
            Response.BinaryWrite(byti);
            Response.Flush();
            Response.Close();
            return new EmptyResult();
        }


        //业务专员撤回部门负责人提交的项目
        [HttpPost]
        [Authorize(Roles = "业务专员")]
        public JsonResult shenhe_ywzy_bmtj_chehui(int xmid) //FormCollection form,   , string loingid, string username
        {
            var xm = db.xiangmuguanlis.Where(s => s.ID == xmid).FirstOrDefault();

            xm.bmfzrtijiao = "未提交";
            using (TransactionScope transaction = new TransactionScope())//原子操作，事物错误回滚
            {
                try
                {
                    //db.xmrizhis.Add(xmrizhi);
                    db.Entry(xm).State = EntityState.Modified;
                    db.SaveChanges();
                    transaction.Complete();
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, Msg = ex.ToString() }, "text/html");
                }
                finally
                {
                    transaction.Dispose();
                }
            }
            return Json(new { success = true, Msg = "保存成功！" }, "text/html");
        }


        //业务专员导出部门提交的分年度汇总表
        [Authorize(Roles = "业务专员,部门负责人,员工")]
        public ActionResult ywzy_exl_huizong_bmtj(string mulu)  //这里的id为xiangmuguanli.cs表中的ID
        {
            var templatePath = Server.MapPath("~/ywzytongguohz.xlsx");  //打开服务器路径中的模板

            Workbook workbook = new Workbook();
            workbook.Open(templatePath);  //工作本打开模板

            Cells cells = workbook.Worksheets[0].Cells;

            Aspose.Cells.Style style1 = workbook.Styles[workbook.Styles.Add()];//新增样式 
            style1.Font.IsBold = true;//粗体 

            //样式2 
            Aspose.Cells.Style style2 = workbook.Styles[workbook.Styles.Add()];//新增样式 
            style2.HorizontalAlignment = TextAlignmentType.Center;//文字居中 
            style2.Font.IsBold = true;//粗体 
            style2.Font.Size = 12;//文字大小

            //样式3
            Aspose.Cells.Style style3 = workbook.Styles[workbook.Styles.Add()];//新增样式 
            style3.HorizontalAlignment = TextAlignmentType.Center;//文字居中 

            //样式4
            Aspose.Cells.Style style4 = workbook.Styles[workbook.Styles.Add()];//新增样式 
            style4.Font.Size = 12;//文字大小
            style4.IsTextWrapped = true;//单元格内容自动换行 

            //为所辖部门获取部门设置列表
            var GenreLst = new List<string>();
            var GenreQry = from d in db.bumenshezhis
                           orderby d.BmName
                           select d.BmName;
            GenreLst.AddRange(GenreQry.Distinct());

            decimal zongji_yi = 0;
            decimal zongji_er = 0;
            decimal zongji_san = 0;

            if (GenreLst.Any())
            {
                int hang = 5;
                int daxuhao = 1;
                foreach (var item_bm in GenreLst)
                {
                    //提取出每个部门，以这个部门为条件，检索出与该部门相关的项目信息               
                    var GenreLst_bmxx = new List<ywzy_huizong>();

                    var bmxinxi = (from x in db.xiangmuguanlis
                                   join y in db.xmnianduyusuans
                                       on x.ID equals y.xiangmuguanliID
                                   where x.suoshuxueyuan == item_bm && x.Xiangmumulu == mulu && x.bmfzrtijiao == "已提交"
                                   orderby x.Xuhao
                                   select new ywzy_huizong
                                   {
                                       xuhao = x.Xuhao,
                                       xmname = x.XmName,
                                       bumen = x.suoshuxueyuan,
                                       beizhu = x.Beizhu,
                                       diyinian = y.diyinian / 10,
                                       diernian = y.diernian / 10,
                                       disannian = y.disannian / 10,
                                       //pwyijian = x.pingweiyijian
                                   });
                    GenreLst_bmxx.AddRange(bmxinxi);

                    if (GenreLst_bmxx.Any())
                    {
                        //合并单元格并写入部门名称 信息
                        cells[hang, 0].PutValue(daxuhao);

                        cells[hang, 0].SetStyle(style2);

                        cells[hang, 1].PutValue(item_bm + "小计：");
                        cells[hang, 1].SetStyle(style1); //设置样式居中，粗体

                        int shuliangsta = hang + 2;
                        int shuliangend = hang + 1 + bmxinxi.Count();

                        cells[hang, 2].Formula = "=SUM(C" + shuliangsta + ":C" + shuliangend + ")";
                        cells[hang, 2].SetStyle(style2); //设置样式居中，粗体

                        cells[hang, 3].Formula = "=SUM(D" + shuliangsta + ":D" + shuliangend + ")";
                        cells[hang, 3].SetStyle(style2); //设置样式居中，粗体

                        cells[hang, 4].Formula = "=SUM(E" + shuliangsta + ":E" + shuliangend + ")";
                        cells[hang, 4].SetStyle(style2); //设置样式居中，粗体

                        int xiaoxuhao = 1;

                        foreach (var item_bmxx in GenreLst_bmxx)
                        {
                            hang = hang + 1;
                            //这样就可以提取出相应的信息了

                            cells[hang, 0].PutValue(daxuhao + "." + xiaoxuhao);
                            cells[hang, 0].SetStyle(style3); //设置样式居中

                            //cells[hang, 1].PutValue(item_bm);
                            cells[hang, 1].PutValue(item_bmxx.xmname);

                            cells[hang, 2].PutValue(item_bmxx.diyinian);
                            cells[hang, 2].SetStyle(style3); //设置样式居中

                            cells[hang, 3].PutValue(item_bmxx.diernian);
                            cells[hang, 3].SetStyle(style3); //设置样式居中

                            cells[hang, 4].PutValue(item_bmxx.disannian);
                            cells[hang, 4].SetStyle(style3); //设置样式居中

                            cells[hang, 5].PutValue(item_bmxx.beizhu);
                            cells[hang, 5].SetStyle(style4);

                            //cells[hang, 6].PutValue(item_bmxx.pwyijian);
                            zongji_yi = zongji_yi + item_bmxx.diyinian;
                            zongji_er = zongji_er + item_bmxx.diernian;
                            zongji_san = zongji_san + item_bmxx.disannian;

                            //hang = hang + 1;
                            xiaoxuhao = xiaoxuhao + 1;
                        }
                        hang = hang + 1;
                        daxuhao = daxuhao + 1;
                    }
                }
                cells[4, 2].PutValue(zongji_yi);
                cells[4, 3].PutValue(zongji_er);
                cells[4, 4].PutValue(zongji_san);
            }
            var filename = "ywzyhuizongbiao" + DateTime.Now.ToString("yyyyMMddhhmmss") + ".xls";
            //转换成流字节，输出浏览器下载
            var byti = workbook.SaveToStream().GetBuffer();
            ////通知浏览器保存文件，其实也就是输出到浏览器
            Response.Clear();
            Response.ContentType = "application/octet-stream";
            Response.AppendHeader("Content-Disposition", "attachment;filename=" + filename);
            Response.BinaryWrite(byti);
            Response.Flush();
            Response.Close();
            return new EmptyResult();
        }


        //导出exccel  
        [Authorize(Roles = "业务专员,部门负责人,员工")]
        public ActionResult excel_out(int id)  //这里的id为xiangmuguanli.cs表中的ID
        {
            //这里给  基本信息表中的  项目类别  取值
            //这里的ID是  xiangmuguanli.cs 表中的ID，先取出表中的  suoshuxueyuan
            string suoshuxueyuan = db.xiangmuguanlis.Where(s => s.ID == id).FirstOrDefault().suoshuxueyuan;

            //再从部门表中取出部门性质
            string leibie = db.bumenshezhis.Where(s => s.BmName == suoshuxueyuan).FirstOrDefault().Bmxingzhi;

            //这里要取出部门负责人，由项目所在的  suoshubumen和 角色role==“部门负责人”  为条件，从menbership中取出用户
            string fuzeren = common.customidentity.fuzeren(suoshuxueyuan);

            //项目采购
            var sql = @"select * from [xmcgpz] where xiangmuguanliID=" + id;

            var dt = GetDataTable(sql);

            dt.TableName = "xmcgpz";

            //项目成效
            var sql_cccx = @"select * from [xmchanchuchengxiao] where xiangmuguanliID=" + id;

            var dt_cccx = GetDataTable(sql_cccx);

            dt_cccx.TableName = "xmchanchuchengxiao";


            //项目产出绩效
            var sql_jxcc = @"select * from [xmjixiao_cc] where xiangmuguanliID=" + id;

            var dt_jxcc = GetDataTable(sql_jxcc);

            dt_jxcc.TableName = "xmjixiao_cc";


            //项目分年度预算
            var sql_ndys = @"select * from [xmnianduyusuan] where xiangmuguanliID=" + id;

            var dt_ndys = GetDataTable(sql_ndys);

            dt_ndys.TableName = "xmnianduyusuan";


            //绩效产出效率
            var sql_jxxl = @"select * from [xmjixiao_xl] where xiangmuguanliID=" + id;

            var dt_jxxl = GetDataTable(sql_jxxl);

            dt_jxxl.TableName = "xmjixiao_xl";

            //项目立项评级
            var sql_lxpj = @"select * from [xmlxpj] where xiangmuguanliID=" + id;

            var dt_lxpj = GetDataTable(sql_lxpj);

            dt_lxpj.TableName = "xmlxpj";

            //测算明细
            var sql_csmx = @"select * from [xmcsmx] where xiangmuguanliID=" + id;

            var dt_csmx = GetDataTable(sql_csmx);

            dt_csmx.TableName = "xmcsmx";

            //var order = GetDataTable(@"select * from [xmjibenxinxi] 
            //where [xmjibenxinxi].xiangmuguanliID=id");

            //order.TableName = "Order";

            //string sqlstr = "select * from [xmjibenxinxi] where xiangmuguanliID=" + id;

            var xm = db.xmjibenxinxis.Where(c => c.xiangmuguanliID == id).FirstOrDefault();

            WorkbookDesigner designer = new WorkbookDesigner();

            designer.Open(Server.MapPath("~/excel.xls"));

            //数据源 
            designer.SetDataSource(dt);
            designer.SetDataSource(dt_cccx);
            designer.SetDataSource(dt_jxcc);
            designer.SetDataSource(dt_ndys);
            designer.SetDataSource(dt_jxxl);
            designer.SetDataSource(dt_lxpj);

            designer.SetDataSource(dt_csmx);

            var jilushu = dt_csmx.Rows.Count + 4;

            if (xm != null)
            {
                //报表单位 
                designer.SetDataSource("Xiangmumingcheng", xm.Xiangmumingcheng);

                designer.SetDataSource("Jieshushijian", xm.Jieshushijian.ToString("yyyy年MM月dd日"));

                designer.SetDataSource("Kaishishijian", xm.Kaishishijian.ToString("yyyy年MM月dd日"));

                designer.SetDataSource("Zhengceyiju", xm.Zhengceyiju);

                designer.SetDataSource("Xiangmubeijing", xm.Xiangmubeijing);

                designer.SetDataSource("shishidizhi", xm.shishidizhi);

                designer.SetDataSource("Lianxidianhua", xm.Lianxidianhua);

                designer.SetDataSource("jingbanren", xm.jingbanren);
            }
            designer.SetDataSource("leibie", leibie + "类项目");

            designer.SetDataSource("fuzeren", fuzeren);    //这里的负责人已经改为 部门负责人的 真实名

            designer.Process();

            //string fileToSave = Server.MapPath("~/createxcel.xls");

            //if (System.IO.File.Exists(fileToSave) == true)
            //{
            //    System.IO.File.Delete(fileToSave);
            //}
            //designer.Workbook.Save(fileToSave);

            var byti = designer.Workbook.SaveToStream().GetBuffer();

            designer = null;

            //var fileName = Server.MapPath("~/createxcel.xls");

            //return File(fileName, "application/ms-excel", "createxcel.xls");            

            var filename = "biaogedaochu" + DateTime.Now.ToString("yyyyMMddhhmmss") + ".xls";
            //转换成流字节，输出浏览器下载

            ////通知浏览器保存文件，其实也就是输出到浏览器
            Response.Clear();
            Response.ContentType = "application/octet-stream";
            Response.AppendHeader("Content-Disposition", "attachment;filename=" + filename);
            Response.BinaryWrite(byti);
            Response.Flush();
            Response.Close();
            return new EmptyResult();
        }


        //批量导出电子表格，只有 基本信息表  的数据来源使用  Linq方式获取，其他表格的数据使用  Sqldataset 方式获取
        //导出exccel  
        [Authorize(Roles = "业务专员,部门负责人,员工")]
        public ActionResult excel_out_piliang(string xmid)  //这里的id为xiangmuguanli.cs表中的ID
        {
            string[] xmid_fenli = xmid.Split(',');//评委负责的评审部门是一个字符串，要改成数组

            var xmid_count = xmid_fenli.Length;

            //WorkbookDesigner byti = new WorkbookDesigner();

            for (int i = 0; i < xmid_count; i++)
            {
                int id = Convert.ToInt32(xmid_fenli[i]);

                //这里给  基本信息表中的  项目类别  取值
                //这里的ID是  xiangmuguanli.cs 表中的ID，先取出表中的  suoshuxueyuan
                string suoshuxueyuan = db.xiangmuguanlis.Where(s => s.ID == id).FirstOrDefault().suoshuxueyuan;

                //再从部门表中取出部门性质
                string leibie = db.bumenshezhis.Where(s => s.BmName == suoshuxueyuan).FirstOrDefault().Bmxingzhi;

                //这里要取出部门负责人，由项目所在的  suoshubumen和 角色role==“部门负责人”  为条件，从menbership中取出用户
                string fuzeren = common.customidentity.fuzeren(suoshuxueyuan);

                //var s = Aspose.Cells.CellsHelper.GetVersion();

                //项目采购
                var sql = @"select * from [xmcgpz] where xiangmuguanliID=" + id;

                var dt = GetDataTable(sql);

                dt.TableName = "xmcgpz";

                //项目成效
                var sql_cccx = @"select * from [xmchanchuchengxiao] where xiangmuguanliID=" + id;

                var dt_cccx = GetDataTable(sql_cccx);

                dt_cccx.TableName = "xmchanchuchengxiao";


                //项目产出绩效
                var sql_jxcc = @"select * from [xmjixiao_cc] where xiangmuguanliID=" + id;

                var dt_jxcc = GetDataTable(sql_jxcc);

                dt_jxcc.TableName = "xmjixiao_cc";


                //项目分年度预算
                var sql_ndys = @"select * from [xmnianduyusuan] where xiangmuguanliID=" + id;

                var dt_ndys = GetDataTable(sql_ndys);

                dt_ndys.TableName = "xmnianduyusuan";


                //绩效产出效率
                var sql_jxxl = @"select * from [xmjixiao_xl] where xiangmuguanliID=" + id;

                var dt_jxxl = GetDataTable(sql_jxxl);

                dt_jxxl.TableName = "xmjixiao_xl";


                //项目立项评级
                var sql_lxpj = @"select * from [xmlxpj] where xiangmuguanliID=" + id;

                var dt_lxpj = GetDataTable(sql_lxpj);

                dt_lxpj.TableName = "xmlxpj";


                //测算明细
                var sql_csmx = @"select * from [xmcsmx] where xiangmuguanliID=" + id;

                var dt_csmx = GetDataTable(sql_csmx);

                dt_csmx.TableName = "xmcsmx";

                var xm = db.xmjibenxinxis.Where(c => c.xiangmuguanliID == id).FirstOrDefault();

                WorkbookDesigner designer = new WorkbookDesigner();

                //designer

                designer.Open(Server.MapPath("~/excel.xls"));

                //数据源 ，designer 设置数据源加载内存数据库  dataSet
                designer.SetDataSource(dt);
                designer.SetDataSource(dt_cccx);
                designer.SetDataSource(dt_jxcc);
                designer.SetDataSource(dt_ndys);
                designer.SetDataSource(dt_jxxl);
                designer.SetDataSource(dt_lxpj);

                designer.SetDataSource(dt_csmx);

                var jilushu = dt_csmx.Rows.Count + 4;                

                // 基本信息表的 数据 使用 Linq 查询方式获取
                if (xm != null)    // 按道理说，其他表也可以使用  Linq 查询数据，而不用  sqldataadapter  和 dataset
                {
                    //报表单位 
                    designer.SetDataSource("Xiangmumingcheng", xm.Xiangmumingcheng);

                    designer.SetDataSource("Jieshushijian", xm.Jieshushijian.ToString("yyyy年MM月dd日"));

                    designer.SetDataSource("Kaishishijian", xm.Kaishishijian.ToString("yyyy年MM月dd日"));

                    designer.SetDataSource("Zhengceyiju", xm.Zhengceyiju);

                    designer.SetDataSource("Xiangmubeijing", xm.Xiangmubeijing);

                    designer.SetDataSource("shishidizhi", xm.shishidizhi);

                    designer.SetDataSource("Lianxidianhua", xm.Lianxidianhua);

                    designer.SetDataSource("jingbanren", xm.jingbanren);
                }
                designer.SetDataSource("leibie", leibie + "类项目");

                designer.SetDataSource("fuzeren", fuzeren);    //这里的负责人已经改为 部门负责人的 真实名

                designer.Process();

                var byti = designer.Workbook.SaveToStream().GetBuffer();

                designer = null;

                var filename = "biaogedaochu" + DateTime.Now.ToString("yyyyMMddhhmmss") + id + ".xls";
                //转换成流字节，输出浏览器下载

                ////通知浏览器保存文件，其实也就是输出到浏览器
                Response.Clear();
                Response.ContentType = "application/octet-stream";
                Response.AppendHeader("Content-Disposition", "attachment;filename=" + filename);
                Response.BinaryWrite(byti);
                Response.Flush();
            }
            Response.Close();

            return new EmptyResult();
        }

        // 使用Aspose 导出 word 文档
        //业务专员页面导出项目总体信息，
        //这里导出的是，当前目录下，所有 业务专员通过审核的 项目
        [Authorize(Roles = "业务专员")]
        public FileResult xm_word_out(string muluname)
        {
            string sqlstr = "select ROW_NUMBER() OVER (ORDER BY XmName ASC) as 序号,XmName,suoshuxueyuan,zongjine,Beizhu from xiangmuguanli where ywzyshenhe='通过'and Xiangmumulu='" + muluname + "'";   //Xiangmumulu=" + muluname + "&&

            string temPath = Server.MapPath("~/xm_word.docx");

            string outputPath = Server.MapPath("~/项目信息表.docx");

            //载入模板
            var doc = new Document(temPath);

            //提供数据源
            var datatable = GetDataTable(sqlstr);

            datatable.TableName = "userlist";

            //合并模版，相当于页面的渲染
            doc.MailMerge.ExecuteWithRegions(datatable);
            
            var docStream = new MemoryStream();

            doc.Save(docStream, Aspose.Words.Saving.SaveOptions.CreateSaveOptions(Aspose.Words.SaveFormat.Docx));          //在这里引用了using Aspose.Words.Saving;

            return base.File(docStream.ToArray(), "application/msword", "项目信息表.docx");
        }


        //查看基本信息，导出word文档 
        //这里的 导出，是点击查看的时候，在基本信息中，可以导出基本信息的word文件
        public FileResult word_out(int id)
        {
            //获取webconfig连接字符串，和下面语句等效，但灵活
            string ConString = System.Configuration.ConfigurationManager.ConnectionStrings["cwcxmkContent"].ConnectionString;

            SqlConnection conn = new SqlConnection(ConString);

            string sqlstr = "select * from [xmjibenxinxi] where xiangmuguanliID=" + id;

            SqlDataAdapter da = new SqlDataAdapter(sqlstr, conn);

            DataSet ds = new DataSet();

            da.Fill(ds);

            string temPath = Server.MapPath("~/Template.docx");

            string outputPath = Server.MapPath("~/项目基本信息表.docx");

            //载入模板
            var doc = new Aspose.Words.Document(temPath);

            conn.Open();

            //提供数据源
            String[] fieldNames = new String[] { "Xiangmumingcheng", "Kaishishijian", "Jieshushijian", "Zhengceyiju", "Xiangmubeijing", "Lianxidianhua", "shishidizhi", "jingbanren" };

            Object[] fieldValues = new Object[] { ds.Tables[0].Rows[0]["Xiangmumingcheng"].ToString(), ds.Tables[0].Rows[0]["Kaishishijian"], ds.Tables[0].Rows[0]["Jieshushijian"], ds.Tables[0].Rows[0]["Zhengceyiju"].ToString(), ds.Tables[0].Rows[0]["Xiangmubeijing"].ToString(), ds.Tables[0].Rows[0]["Lianxidianhua"].ToString(), ds.Tables[0].Rows[0]["shishidizhi"].ToString(), ds.Tables[0].Rows[0]["jingbanren"].ToString() };

            //合并模板
            doc.MailMerge.Execute(fieldNames, fieldValues);

            //保存合并后的文档
            doc.Save(outputPath);

            conn.Close();

            var docStream = new MemoryStream();

            doc.Save(docStream, Aspose.Words.Saving.SaveOptions.CreateSaveOptions(Aspose.Words.SaveFormat.Doc));          //在这里引用了using Aspose.Words.Saving;

            return base.File(docStream.ToArray(), "application/msword", "项目基本信息表.doc");
        }


        //评委点击图表执行的方法BasicBarindex
        [Authorize(Roles = "评委")]
        public ActionResult pw_BasicBarindex(string muluname)
        {
            ViewBag.muluname = muluname;
            return View();
        }


        //业务专员审核页面显示的饼图
        [Authorize(Roles = "业务专员,财务主管,领导")]
        public ActionResult BasicBarindex(string muluname)
        {
            ViewBag.muluname = muluname;
            return View();
        }


        //领导查看评审表图，与分管领导查看表图类似
        [Authorize(Roles = "领导")]
        public ActionResult ld_BasicBarindex(string loingid, string muluname)
        {
            //var bmname = db.bumenshezhis.OrderByDescending(a => a.Bmxingzhi).Select(a => a.BmName);

            //这里加上substring的原因是，领导取出  suoshuxueyuan带有 “all,”，这里要把“all,”排除
            string suoshuxueyuan = customidentity.suoshuxueyuan(loingid).Substring(4);

            string[] bumen_fenli = suoshuxueyuan.Split(',');//评委负责的评审部门是一个字符串，要改成数组

            var bmcount = bumen_fenli.Length;

            if (suoshuxueyuan.Any())                                  //判断是否有部门存在，没有则转到数据错误页面
            {
                string[] y_categories = bumen_fenli;  //声明一个部门总数位数的数组

                object[] fgld_zongshu = new object[bmcount];
                object[] fgld_yishen = new object[bmcount];

                for (int i = 0; i < bmcount; i++)
                {
                    string xueyuan = y_categories[i];
                    //各部门已经填报的项目的个数
                    fgld_zongshu[i] = db.xiangmuguanlis.Where(s => s.suoshuxueyuan == xueyuan && s.Xiangmumulu == muluname && s.bmfzrtijiao == "已提交").Count();

                    //各部门负责人已经提交的项目的个数
                    fgld_yishen[i] = db.xiangmuguanlis.Where(s => s.suoshuxueyuan == xueyuan && s.Xiangmumulu == muluname && s.bmfzrtijiao == "已提交" && s.fgldshenhe != "未审核").Count();

                    //各分管领导人已经审核的项目的个数
                    //fgldyjshenhe[i] = db.xiangmuguanlis.Where(s => s.suoshuxueyuan == xueyuan && s.Xiangmumulu == muluname && s.fgldshenhe == "通过").Count();
                }

                Highcharts chart = new Highcharts("chart")
                   .InitChart(new DotNet.Highcharts.Options.Chart { DefaultSeriesType = ChartTypes.Column, Width = 1000 })
                   .SetTitle(new DotNet.Highcharts.Options.Title { Text = "申报进度表" })
                   //.SetSubtitle(new Subtitle { Text = "Source: Wikipedia.org" })
                   .SetXAxis(new XAxis
                   {
                       //Categories = new [] { "Africa", "America", "Asia", "Europe", "Oceania" },
                       Categories = y_categories,
                       Title = new XAxisTitle { Text = string.Empty },
                       Labels = new XAxisLabels { Rotation = 90 }
                   })

                   .SetYAxis(new YAxis
                   {
                       Title = new YAxisTitle
                       {
                           Text = "数量",
                           Align = AxisTitleAligns.High
                       }
                   })
                   .SetTooltip(new Tooltip
                   {
                       HeaderFormat = "<span style=\"font-size:10px\">{point.key}</span><table>",
                       PointFormat = "<tr><td style=\"color:{series.color}\"} >{series.name}:</td><td style=\"padding:0px\"><b>{point.y}个</b></td></tr>",
                       FooterFormat = "</table>",
                       Shared = true,
                       UseHTML = true
                   })
                   .SetCredits(new Credits { Enabled = false })
                   .SetSeries(new[]
                  {
                    new Series { Name = "拟申报项目", Data =new Data(fgld_zongshu) },

                    new Series { Name = "已审核项目", Data = new Data(fgld_yishen) }
                });
                return View(chart);
            }
            else
            {
                return View("basicbar_error");
            }
        }


        //分管领导查看评审表图
        [Authorize(Roles = "分管领导")]
        public ActionResult fgld_BasicBarindex(string loingid, string muluname)
        {
            string suoshuxueyuan = customidentity.suoshuxueyuan(loingid);

            string[] bumen_fenli = suoshuxueyuan.Split(',');//评委负责的评审部门是一个字符串，要改成数组

            var bmcount = bumen_fenli.Length;

            if (suoshuxueyuan.Any())                                  //判断是否有部门存在，没有则转到数据错误页面
            {
                string[] y_categories = bumen_fenli;  //声明一个部门总数位数的数组
                object[] fgld_zongshu = new object[bmcount];
                object[] fgld_yishen = new object[bmcount];
                for (int i = 0; i < bmcount; i++)
                {
                    string xueyuan = y_categories[i];
                    //各部门已经填报的项目的个数
                    fgld_zongshu[i] = db.xiangmuguanlis.Where(s => s.suoshuxueyuan == xueyuan && s.Xiangmumulu == muluname && s.bmfzrtijiao == "已提交").Count();

                    //各部门负责人已经提交的项目的个数
                    fgld_yishen[i] = db.xiangmuguanlis.Where(s => s.suoshuxueyuan == xueyuan && s.Xiangmumulu == muluname && s.bmfzrtijiao == "已提交" && s.fgldshenhe != "未审核").Count();

                    //各分管领导人已经审核的项目的个数
                    //fgldyjshenhe[i] = db.xiangmuguanlis.Where(s => s.suoshuxueyuan == xueyuan && s.Xiangmumulu == muluname && s.fgldshenhe == "通过").Count();
                }
                Highcharts chart = new Highcharts("chart")
                   .InitChart(new DotNet.Highcharts.Options.Chart { DefaultSeriesType = ChartTypes.Column, Width = 1000 })
                   .SetTitle(new DotNet.Highcharts.Options.Title { Text = "申报进度表" })
                   //.SetSubtitle(new Subtitle { Text = "Source: Wikipedia.org" })
                   .SetXAxis(new XAxis
                   {
                       Categories = y_categories,
                       Title = new XAxisTitle { Text = string.Empty },
                       Labels = new XAxisLabels { Rotation = 90 }
                   })
                   .SetYAxis(new YAxis
                   {
                       Title = new YAxisTitle
                       {
                           Text = "数量",
                           Align = AxisTitleAligns.High
                       }
                   })
                   .SetTooltip(new Tooltip
                   {
                       HeaderFormat = "<span style=\"font-size:10px\">{point.key}</span><table>",
                       PointFormat = "<tr><td style=\"color:{series.color}\"} >{series.name}:</td><td style=\"padding:0px\"><b>{point.y}个</b></td></tr>",
                       FooterFormat = "</table>",
                       Shared = true,
                       UseHTML = true
                   })
                   .SetCredits(new Credits { Enabled = false })
                   .SetSeries(new[]
                  {
                    new Series { Name = "拟申报项目", Data =new Data(fgld_zongshu) },

                    new Series { Name = "已审核项目", Data = new Data(fgld_yishen) },                 
                });
                return View(chart);
            }
            else
            {
                return View("basicbar_error");
            }
        }


        //业务专员登录页面，项目管理，每个目录的评委评审进度图表，饼图
        [Authorize(Roles = "业务专员")]
        public ActionResult ywzy_pwjd_piechart(string mulu, int fgldshenhefou)
        {
            int weishen_count = 0;

            int chehui_count = 0;

            int yishenhe_count = 0;

            if (fgldshenhefou == 1)
            {
                weishen_count = db.xiangmuguanlis.Where(s => s.Xiangmumulu == mulu && s.bmfzrshenhe == "通过" && s.fgldshenhe == "通过" && s.pingweishenhe == "未审核").Count();  //赛选符合条件的记录
            }
            else
            {
                weishen_count = db.xiangmuguanlis.Where(s => s.Xiangmumulu == mulu && s.bmfzrshenhe == "通过" && s.pingweishenhe == "未审核").Count();  //赛选符合条件的记录
            }
            chehui_count = db.xiangmuguanlis.Where(s => s.Xiangmumulu == mulu && s.pingweishenhe == "撤回").Count();

            yishenhe_count = db.xiangmuguanlis.Where(s => s.Xiangmumulu == mulu && s.pingweishenhe == "通过" && s.pingweishenhe == "未通过").Count();

            if (weishen_count + chehui_count + yishenhe_count == 0)
            {
                return View("basicbar_error");
            }
            object[,] chartvalues = new object[3, 2] { { "未审核", weishen_count }, { "撤回", chehui_count }, { "已审核", yishenhe_count } };

            Highcharts chart = new Highcharts("chart")
              .InitChart(new DotNet.Highcharts.Options.Chart { PlotShadow = false, Height = 350 })
              .SetTitle(new DotNet.Highcharts.Options.Title { Text = "评委审核进度" })
              .SetTooltip(new Tooltip { Formatter = "function() { return '<b>'+ this.point.name +'</b>: '+ this.percentage +' %'; }" })
              .SetPlotOptions(new PlotOptions
              {
                  Pie = new PlotOptionsPie
                  {
                      AllowPointSelect = true,
                      Cursor = Cursors.Pointer,
                      DataLabels = new PlotOptionsPieDataLabels
                      {
                          Color = ColorTranslator.FromHtml("#000000"),
                          ConnectorColor = ColorTranslator.FromHtml("#000000"),
                          Formatter = "function() { return '<b>'+ this.point.name +'</b>: '+ this.percentage +' %'; }"
                      }
                  }
              })
              .SetSeries(new Series
              {
                  Type = ChartTypes.Pie,
                  Name = "Browser share",
                  Data = new Data(chartvalues),
              });
            return View(chart);
        }


        //业务专员登录，评委管理，每个评委的评委审核显示的饼图  
        [Authorize(Roles = "业务专员")]
        public ActionResult pwjd_piechart(string username)
        {
            username = Server.UrlDecode(username);

            bumenshezhi bmshezhi = new bumenshezhi();

            ApplicationUser pw = common.customidentity.pingwei(username);//取出评委

            string[] pwbumen_fenli = pw.suoshuxueyuan.Split(',');//评委负责的评审部门是一个字符串，要改成数组

            string[] pwmulu_fenli = pw.pingshenmulu.Split(',');//评委负责的目录是一个字符串，要改成数组，

            var mulucount = pwmulu_fenli.Length;

            var bmcount = pwbumen_fenli.Length;

            if (bmcount > 0 && mulucount > 0)
            {
                int weishen_count = 0;

                int chehui_count = 0;

                int yishenhe_count = 0;

                for (int k = 0; k < mulucount; k++)
                {
                    var mulu = pwmulu_fenli[k];

                    for (int i = 0; i < bmcount; i++)
                    {
                        var bm = pwbumen_fenli[i];      //这里直接取出评委参审目录所组成数组的，坐标值

                        weishen_count = weishen_count + db.xiangmuguanlis.Where(s => s.Xiangmumulu == mulu && s.suoshuxueyuan == bm && s.bmfzrshenhe == "通过" && s.pingweishenhe == "未审核").Count();  //赛选符合条件的记录

                        chehui_count = chehui_count + db.xiangmuguanlis.Where(s => s.Xiangmumulu == mulu && s.suoshuxueyuan == bm && s.pingweishenhe == "撤回").Count();

                        yishenhe_count = yishenhe_count + db.xiangmuguanlis.Where(s => s.Xiangmumulu == mulu && s.suoshuxueyuan == bm && s.pingweishenhe == "通过" && s.pingweishenhe == "未通过").Count();
                    }
                }
                if (weishen_count + chehui_count + yishenhe_count == 0)
                {
                    return View("basicbar_error");
                }
                object[,] chartvalues = new object[3, 2] { { "未审核", weishen_count }, { "撤回", chehui_count }, { "已审核", yishenhe_count } };

                Highcharts chart = new Highcharts("chart")
                  .InitChart(new DotNet.Highcharts.Options.Chart { PlotShadow = false, Height = 350 })
                  .SetTitle(new DotNet.Highcharts.Options.Title { Text = "评委审核进度" })
                  .SetTooltip(new Tooltip { Formatter = "function() { return '<b>'+ this.point.name +'</b>: '+ this.percentage +' %'; }" })
                  .SetPlotOptions(new PlotOptions
                  {
                      Pie = new PlotOptionsPie
                      {
                          AllowPointSelect = true,
                          Cursor = Cursors.Pointer,
                          DataLabels = new PlotOptionsPieDataLabels
                          {
                              Color = ColorTranslator.FromHtml("#000000"),
                              ConnectorColor = ColorTranslator.FromHtml("#000000"),
                              Formatter = "function() { return '<b>'+ this.point.name +'</b>: '+ this.percentage +' %'; }"
                          }
                      }
                  })
                  .SetSeries(new Series
                  {
                      Type = ChartTypes.Pie,
                      Name = "Browser share",
                      Data = new Data(chartvalues),
                  });
                return View(chart);
            }
            else
            {
                return View("basicbar_error");
            }
        }


        //业务专员审核页面显示的饼图
        [Authorize(Roles = "业务专员,财务主管,领导")]
        public ActionResult piechart(string leibie, string muluname)
        {

            bumenshezhi bmshezhi = new bumenshezhi();
            var bmcount = db.bumenshezhis.Count();
            if (bmcount > 0)
            {
                string[] y_categories = new string[bmcount];
                var bmname = from r in db.bumenshezhis
                             select r.BmName;
                y_categories = bmname.ToArray();

                object[] chartvalues = new object[bmcount];

                if (leibie == "jine")
                {
                    decimal xm_jiner = 0;

                    for (int i = 0; i < bmcount; i++)
                    {
                        var dd = y_categories[i];  //这里取出数组中的部门，作为临时值条件在过滤中使用
                        var db1 = db.xiangmuguanlis.Where(s => s.suoshuxueyuan == dd && s.Xiangmumulu == muluname && s.ywzyshenhe == "通过");  //赛选符合条件的记录
                        if (db1.Count() == 0)
                        {
                            chartvalues[i] = 0;
                        }
                        else
                        {
                            var jiner = (from r in db1
                                         select r.zongjine).Sum();
                            chartvalues[i] = new object[] { dd, jiner };
                            xm_jiner = xm_jiner + jiner;
                        }
                    }
                    if (xm_jiner == 0)
                    {
                        return View("basicbar_error");
                    }
                }
                else
                {
                    //tubiaoming = "数量";
                    //danweizhi = "个";
                    //biaoti = "各部门所报项目数量";
                    var jilu_count = 0;
                    for (int i = 0; i < bmcount; i++)
                    {
                        var dd = y_categories[i];
                        var db1 = db.xiangmuguanlis.Where(s => s.suoshuxueyuan == dd && s.Xiangmumulu == muluname && s.ywzyshenhe == "通过");

                        var shuliang = db1.Count();

                        chartvalues[i] = new object[] { dd, shuliang };

                        jilu_count = jilu_count + db1.Count();
                    }
                    if (jilu_count == 0)
                    {
                        return View("basicbar_error");
                    }
                }
                Highcharts chart = new Highcharts("chart")
                  .InitChart(new DotNet.Highcharts.Options.Chart { PlotShadow = false })
                  .SetTitle(new DotNet.Highcharts.Options.Title { Text = "各部门通过评审项目分布" })
                  .SetTooltip(new Tooltip { Formatter = "function() { return '<b>'+ this.point.name +'</b>: '+ this.percentage +' %'; }" })
                  .SetPlotOptions(new PlotOptions
                  {
                      Pie = new PlotOptionsPie
                      {
                          AllowPointSelect = true,
                          Cursor = Cursors.Pointer,
                          DataLabels = new PlotOptionsPieDataLabels
                          {
                              Color = ColorTranslator.FromHtml("#000000"),
                              ConnectorColor = ColorTranslator.FromHtml("#000000"),
                              Formatter = "function() { return '<b>'+ this.point.name +'</b>: '+ this.percentage +' %'; }"
                          }
                      }
                  })
                  .SetSeries(new Series
                  {
                      Type = ChartTypes.Pie,
                      Name = "Browser share",
                      Data = new Data(chartvalues),
                  });
                return View(chart);
            }
            else
            {
                return View("basicbar_error");
            }
        }


        //业务专员显示图表BasicBar,参数leibie可取值项目数，金额等
        [Authorize(Roles = "业务专员,财务主管,领导")]
        public ActionResult BasicBar(string leibie, string muluname)
        {
            bumenshezhi bmshezhi = new bumenshezhi();
            var bmcount = db.bumenshezhis.Count();            //读取部门总数
            if (bmcount > 0)                                  //判断是否有部门存在，没有则转到数据错误页面
            {
                string[] y_categories = new string[bmcount];  //声明一个部门总数位数的数组
                var bmname = from r in db.bumenshezhis        //取出部门的名称
                             select r.BmName;
                y_categories = bmname.ToArray();              //由部门名称数组化

                object[] chartvalues = new object[bmcount];   //声明一个部门总数的对象数组

                string tubiaoming = "金额";
                string danweizhi = "金额（万元）";
                string biaoti = "各部门所报项目终审通过合计金额";

                if (leibie == "jine")
                {
                    decimal xm_jine = 0;

                    for (int i = 0; i < bmcount; i++)
                    {
                        var dd = y_categories[i];  //这里取出数组中的部门，作为临时值条件在过滤中使用
                        var db1 = db.xiangmuguanlis.Where(s => s.suoshuxueyuan == dd && s.Xiangmumulu == muluname && s.ywzyshenhe == "通过");  //赛选符合条件的记录
                        if (db1.Count() == 0)
                        {
                            chartvalues[i] = 0;
                        }
                        else
                        {
                            var jiner = (from r in db1
                                         select r.zongjine).Sum();
                            chartvalues[i] = jiner;
                            xm_jine = xm_jine + jiner;
                        }
                    }
                    if (xm_jine == 0)
                    {
                        return View("basicbar_error");
                    }
                }
                else
                {
                    tubiaoming = "数量";
                    danweizhi = "个";
                    biaoti = "各部门所报项目终审通过数量";
                    var jilu_count = 0;

                    for (int i = 0; i < bmcount; i++)
                    {

                        var dd = y_categories[i];
                        var db1 = db.xiangmuguanlis.Where(s => s.suoshuxueyuan == dd && s.Xiangmumulu == muluname && s.ywzyshenhe == "通过");

                        var shuliang = db1.Count();

                        chartvalues[i] = shuliang;

                        jilu_count = jilu_count + db1.Count();
                    }
                    if (jilu_count == 0)
                    {
                        return View("basicbar_error");
                    }
                }

                Highcharts chart = new Highcharts("chart")
                    //.InitChart(new DotNet.Highcharts.Options.Chart { DefaultSeriesType = ChartTypes.Bar })
                    .InitChart(new DotNet.Highcharts.Options.Chart { DefaultSeriesType = ChartTypes.Column, Width = 2000 }) //把柱形图的形状加粗
                    .SetTitle(new DotNet.Highcharts.Options.Title { Text = biaoti })

                    .SetXAxis(new XAxis
                    {

                        Categories = y_categories,
                        Title = new XAxisTitle { Text = string.Empty },
                        Labels = new XAxisLabels { Rotation = 90 }
                    })
                    .SetYAxis(new YAxis
                    {
                        Min = 0,
                        Title = new YAxisTitle
                        {
                            Text = danweizhi,
                            Align = AxisTitleAligns.High
                        }
                    })
                    .SetTooltip(new Tooltip
                    {
                        HeaderFormat = "<span style=\"font-size:10px\">{point.key}</span><table>",
                        PointFormat = "<tr><td style=\"color:{series.color}\"} >{series.name}:</td><td style=\"padding:0px\"><b>{point.y}个</b></td></tr>",
                        FooterFormat = "</table>",
                        Shared = true,
                        UseHTML = true
                    })
                   .SetCredits(new Credits { Enabled = false })
                    .SetSeries(new[]
                    {                 
                      //以下为我改的
                        new Series { Name = tubiaoming, Data =new Data(chartvalues) }
                    });
                //ViewBag.ChartModel = chart;
                return View(chart);
            }
            else
            {
                return View("basicbar_error");
            }
        }


        [HttpPost]
        [Authorize(Roles = "部门负责人,员工")]
        public JsonResult saveXiangmu(string xiangmu, string loingid, string username)
        {
            bool shijianqi = mulucustom.muluyouxiaoqi(xiangmu);      //调用公共方法判断当前时间是否在有效期内

            xiangmuguanli xiangmuguanli = new xiangmuguanli();

            if (shijianqi == true)
            {
                xiangmuguanli.XmName = Request.Form["XmName"];

                xiangmuguanli.Beizhu = Request.Form["Beizhu"];

                string suoshuxueyuan = customidentity.suoshuxueyuan(loingid);
                xiangmuguanli.suoshuxueyuan = suoshuxueyuan;               //把登录人的所属学院值赋值给所创建的项目
                xiangmuguanli.username = username;                         //创建项目时把创建人的值赋值给项目
                xiangmuguanli.userid = loingid;

                xiangmuguanli.Chuangjianshijian = DateTime.Now.ToString("yyyy-MM-dd");

                xiangmuguanli.Xiangmumulu = xiangmu;
                xiangmuguanli.Xuhao = 1;

                //编辑表申请表 初始状态
                xiangmuguanli.jpxx = "0";
                xiangmuguanli.ccjx = "0";
                xiangmuguanli.xljx = "0";
                xiangmuguanli.csmx = "0";
                xiangmuguanli.lxpj = "0";
                xiangmuguanli.cgpz = "0";
                xiangmuguanli.cccx = "0";
                xiangmuguanli.nianduyusuan = "0";

                //创建项目时赋值审核状态  Enum.GetValues(typeof (GenderType))
                xiangmuguanli.shenhezhuangtai = "未审核";
                //xiangmuguanli.bmfzrshenhe = shenhejieguo.未审核;
                //xiangmuguanli.fgldshenhe = shenhejieguo.未审核;
                //xiangmuguanli.ywzyshenhe = shenhejieguo.未审核;

                xiangmuguanli.bmfzrshenhe = "未审核";
                xiangmuguanli.fgldshenhe = "未审核";
                xiangmuguanli.ywzyshenhe = "未审核";
                //xiangmuguanli.ldshenhe = "未审核";
                xiangmuguanli.pingweishenhe = "未审核";
                xiangmuguanli.pingweitijiao = "未提交";

                xiangmuguanli.tijiaozhuantai = "未提交";
                xiangmuguanli.bmfzrtijiao = "未提交";
                xiangmuguanli.fgldtijiao = "未提交";

                //这里的修改是在，部门负责人送审的时候要检查“当前项目（部门负责人看到的项目）”是否排序，而我们做的方法是  从数据库中筛选（仅将目录名和所属学院作为条件）做比较
                //这里
                if (customidentity.userrole(loingid) == "部门负责人")
                {
                    xiangmuguanli.tijiaozhuantai = "已提交";
                    //xiangmuguanli.bmfzrtijiao = "未提交";
                    //xiangmuguanli.fgldtijiao = "未提交";
                }
                if (ModelState.IsValid)
                {
                    try
                    {
                        db.xiangmuguanlis.Add(xiangmuguanli);
                        db.SaveChanges();
                        return Json(new { success = true, errorMsg = "项目保存成功！" }, "text/html");
                    }
                    catch (Exception ex)
                    {
                        return Json(new { success = false, errorMsg = ex.ToString() }, "text/html");
                    }
                }
                var error_msg = new         //这里是什么意思
                {
                    errorMsg = "Some errors occured.",
                };
                //return Json(error_msg);
                //底下这个方法更简洁
                return Json(new { success = false, errorMsg = "some error occured." }, "text/html");
            }
            else
            {
                var error_Msg = new          //这里是什么意思
                {
                    errorMsg = "项目添加不在有效期内！"
                };
                //return Json(error_msg);
                return Json(new { success = false, errorMsg = "项目添加不在有效期内！" }, "text/html");
            }
        }


        //[ValidateAntiForgeryToken]
        //把项目名称改为主键的时候，在编辑时，主键是不能修改的，即在视图中XmName中考虑是否加入不开编辑
        [HttpPost]
        [Authorize(Roles = "部门负责人,员工")]
        public JsonResult updateXiangmu(string muluname, int xmid)   //在form中要加入一个隐藏的ID，编辑时需要有ID          FormCollection form, string xiangmu
        {
            string xmname = Request.Form["XmName"];

            xiangmuguanli xmgl = new xiangmuguanli();

            xmgl = db.xiangmuguanlis.Where(s => s.ID == xmid).FirstOrDefault();

            //更改基本信息表中的  项目名称
            var jbxx = db.xmjibenxinxis.Where(s => s.xiangmuguanliID == xmid).FirstOrDefault();

            string xmname_old = xmgl.XmName;

            if (xmname_old == xmname)
            {
                xmgl.XmName = xmname;
            }
            else
            {
                string issamename = checkNameIsSame(xmname, muluname);

                if (issamename == "True")
                {
                    xmgl.XmName = xmname;

                    //更改基本信息表中的  项目名称
                    jbxx.Xiangmumingcheng = xmname;

                    //在编辑项目名称时要判断，如果该项目由附件的，要更改附件的文件路径，以及数据库中路径字段的值
                    string srcFolderPath = Server.MapPath(string.Format("~/Uploads/{0}/{1}/", muluname, xmname_old));    //将字符串路径转化为服务器路径

                    string destFolderPath = Server.MapPath(string.Format("~/Uploads/{0}/{1}/", muluname, xmname));

                    if (System.IO.Directory.Exists(srcFolderPath))
                    {
                        System.IO.Directory.Move(srcFolderPath, destFolderPath);
                    }

                    //修改fileup表中的路径值
                    fileupload fujian_gai = new fileupload();

                    var fujian = db.fileuploads.Where(s => s.xiangmuguanliID == xmid).Select(s => s.fileuploadID).ToArray();    //取出表中所有对应关系的主键

                    if (fujian.Any())
                    {
                        for (int i = 0; i < fujian.Count(); i++)
                        {
                            var fileupid = fujian[i];

                            fujian_gai = db.fileuploads.Where(c => c.fileuploadID == fileupid).FirstOrDefault();

                            fujian_gai.filepath = string.Format("Uploads/{0}/{1}/", muluname, xmname);
                        }

                        db.Entry(fujian_gai).State = EntityState.Modified;
                    }
                }
                else
                {
                    return Json(new { success = false, errorMsg = "项目名已经存在，请重新命名" }, "text/html");
                }
            }
            xmgl.Xiugaishijian = DateTime.Now.ToString("yyyy-MM-dd");

            xmgl.Beizhu = Request.Form["Beizhu"];
            try
            {
                db.Entry(jbxx).State = EntityState.Modified;
                db.Entry(xmgl).State = EntityState.Modified;
                db.SaveChanges();
                return Json(new { success = true, errorMsg = "项目编辑保存成功！" }, "text/html");
            }
            catch (Exception ex)
            {
                return Json(new { success = false, errorMsg = ex.ToString() }, "text/html");
            }
        }


        // [ValidateAntiForgeryToken]     这种方式获取不到数据
        //删除时时要根据主键进行选择看那一条记录被选中，视图需要更改选中的主键
        [HttpPost]
        [Authorize(Roles = "部门负责人,员工")]
        public JsonResult delXiangmu()
        {
            int ID = Convert.ToInt32(Request.Form["id"]);  //form["id"]为页面post方法传递的参数,选中row然后 id=row.ID。destyxiangmu()方法中都没有form，这里为什么使用了from

            string xmname = Request.Form["XmName"];

            string mulu = Request.Form["mulu"];

            string filePath1 = string.Format("~/Uploads/{0}/{1}/", mulu, xmname);//通过参数组建一个路径格式的字符串

            try
            {
                xiangmuguanli xiamgmu = db.xiangmuguanlis.Find(ID);
                db.xiangmuguanlis.Remove(xiamgmu);
                db.SaveChanges();
                // 文件上传后的保存路径

                string filePath = Server.MapPath(filePath1);//将路径格式的字符串通过函数转化为服务器路径
                DirectoryInfo dir = new DirectoryInfo(filePath);
                if (dir.Exists)
                {
                    dir.Delete(true);
                }               

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { errorMsg = ex.ToString() }, "text/html");
            }
        }


        //完善信息
        //[HttpPost]
        public ActionResult wanshanxinxi(string NoticeXmName, int xmid, string xmmulu)  //xmid为xiangmuguanli中的ID
        {
            ViewBag.xmmulu = xmmulu;

            ViewBag.name = NoticeXmName;
            ViewBag.jibenid = xmid;

            //编辑时放回的的参数
            //xmjibenxinxi jbxx = new xmjibenxinxi();
            xiangmuguanli xmgl = new xiangmuguanli();
            xmgl = db.xiangmuguanlis.Where(s => s.ID == xmid).FirstOrDefault();   //选择第一项或者符合条件的项
            ViewBag.xmgl_jbxx = xmgl.jpxx;                                        //把基本信息的值传给前台，再由前台判断其值

            ViewBag.xmgl_lxpj = xmgl.lxpj;

            ViewBag.xmgl_cccx = xmgl.cccx;

            ViewBag.xmgl_cgpz_null = xmgl.cgpz_nll;

            ViewBag.xmgl_ndys = xmgl.nianduyusuan;  //分年度预算

            return View();
        }


        //员工登录后看到的页面
        [HttpPost]
        [Authorize(Roles = "员工")]
        public JsonResult yg_getXiangmu(string xiangmu, string loingid, string username, string dateshijian)
        {
            int page = (Request.Form["page"] != null) ? Int32.Parse(Request.Form["page"]) : 1;

            int rows = (Request.Form["rows"] != null) ? Int32.Parse(Request.Form["rows"]) : 10;
            string a = dateshijian;

            string suoshuxueyuan = customidentity.suoshuxueyuan(loingid);  //由登录人的id取出登录人的所属学院值，根据所属学院值过滤显示的信息。登录人员只能显示他所在部门的信息

            var xm = from s in db.xiangmuguanlis
                     where s.Xiangmumulu == xiangmu && s.suoshuxueyuan == suoshuxueyuan && s.userid == loingid
                     orderby s.ID
                     select new
                     {
                         ID = s.ID,
                         XmName = s.XmName,
                         Chuangjianshijian = s.Chuangjianshijian,
                         Beizhu = s.Beizhu,
                         //suoshuxueyuan=s.suoshuxueyuan,
                         Xiangmumulu = s.Xiangmumulu,
                         tijiaozhuantai = s.tijiaozhuantai,
                         //bmfzrtijiao = s.bmfzrtijiao,
                         bmfzrshenhe = s.bmfzrshenhe,
                         fgldshenhe = s.fgldshenhe,
                         ywzyshenhe = s.ywzyshenhe,
                         //username = s.username,
                         //bmfzryijian = s.bmfzryijian,
                         pingweishenhe = s.pingweishenhe,
                         zongjine = s.zongjine
                     };
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


        //分管领导参与审核，部门负责人登录加载的datagrid数据
        [HttpPost]
        [Authorize(Roles = "部门负责人")]
        public JsonResult shenhe_bmfzr_xm(string xiangmu, string loingid, string bmfzrname)
        {
            int page = (Request.Form["page"] != null) ? Int32.Parse(Request.Form["page"]) : 1;

            int rows = (Request.Form["rows"] != null) ? Int32.Parse(Request.Form["rows"]) : 10;

            string suoshuxueyuan = customidentity.suoshuxueyuan(loingid);  //由登录人的id取出登录人的所属学院值，根据所属学院值过滤显示的信息。登录人员只能显示他所在部门的信息


            //为了区分读取出来的是哪一个年度的项目,  注意：在indentity中username是唯一的，这里可以使用username作为过滤条件取出部门负责人自己穿件的项目
            var mulu = db.xiangmuguanlis.Where(s => (s.Xiangmumulu == xiangmu && s.suoshuxueyuan == suoshuxueyuan) && (s.tijiaozhuantai == "已提交" || s.username == bmfzrname));

            var xm = from s in mulu  //只能看到部门负责人未审核或分管领导或业务专员未通过的项
                     orderby s.Xuhao
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


        //部门负责人登录后再页面看到的  datagrid数据
        [HttpPost]
        [Authorize(Roles = "部门负责人")]
        public JsonResult bmfzr_getXiangmu(string xiangmu, string loingid, string bmfzrname)       
        {
            string suoshuxueyuan = customidentity.suoshuxueyuan(loingid);  //由登录人的id取出登录人的所属学院值，根据所属学院值过滤显示的信息。登录人员只能显示他所在部门的信息

            var xm = from s in db.xiangmuguanlis
                     where (s.Xiangmumulu == xiangmu && s.suoshuxueyuan == suoshuxueyuan) && (s.tijiaozhuantai == "已提交" || s.userid == loingid)
                     orderby s.Xuhao
                     select new
                     {
                         ID = s.ID,
                         XmName = s.XmName,
                         Xuhao = s.Xuhao,
                         Chuangjianshijian = s.Chuangjianshijian,
                         Beizhu = s.Beizhu,
                         //suoshuxueyuan=s.suoshuxueyuan,
                         Xiangmumulu = s.Xiangmumulu,
                         tijiaozhuantai = s.tijiaozhuantai,
                         bmfzrtijiao = s.bmfzrtijiao,
                         bmfzrshenhe = s.bmfzrshenhe,
                         fgldshenhe = s.fgldshenhe,
                         ywzyshenhe = s.ywzyshenhe,
                         username = s.username,
                         //bmfzryijian = s.bmfzryijian,
                         pingweishenhe = s.pingweishenhe,
                         zongjine = s.zongjine
                     };
            try
            {
                // 返回到前台的值必须按照如下的格式包括 total and rows 
                //var easyUIPages = new Dictionary<string, object>();
                //easyUIPages.Add("total", xm.Count());
                ////easyUIPages.Add("rows", xm.ToPagedList(page, rows)); 忘了，要变成json
                //return Json(easyUIPages);
                return Json(xm.ToList());
            }
            catch (Exception)
            {
                throw;
            }
        }


        [HttpPost]
        [Authorize(Roles = "业务专员")]
        public JsonResult ywzy_shenhe_bmtj(string xiangmu, string searchquery)
        {
            int page = (Request.Form["page"] != null) ? Int32.Parse(Request.Form["page"]) : 1;

            int rows = (Request.Form["rows"] != null) ? Int32.Parse(Request.Form["rows"]) : 10;

            if (searchquery == "所有部门")
            {
                var xm = from s in db.xiangmuguanlis
                         where (s.Xiangmumulu == xiangmu && s.bmfzrshenhe == "通过" && s.bmfzrtijiao == "已提交")
                         orderby s.Xuhao
                         select new
                         {
                             ID = s.ID,
                             XmName = s.XmName,
                             Xuhao = s.Xuhao,
                             Chuangjianshijian = s.Chuangjianshijian,
                             Beizhu = s.Beizhu,
                             suoshuxueyuan = s.suoshuxueyuan,
                             Xiangmumulu = s.Xiangmumulu,
                             //tijiaozhuantai = s.tijiaozhuantai,
                             //bmfzrtijiao = s.bmfzrtijiao,
                             bmfzrshenhe = s.bmfzrshenhe,
                             fgldshenhe = s.fgldshenhe,
                             ywzyshenhe = s.ywzyshenhe,
                             //username = s.username,
                             //bmfzryijian = s.bmfzryijian,
                             pingweishenhe = s.pingweishenhe,
                             zongjine = s.zongjine,
                             bmfzrtijiao = s.bmfzrtijiao
                         };
                try
                {
                    // 返回到前台的值必须按照如下的格式包括 total and rows 
                    var easyUIPages = new Dictionary<string, object>();
                    easyUIPages.Add("total", xm.Count());
                    easyUIPages.Add("rows", xm.ToPagedList(page, rows));
                    return Json(easyUIPages, JsonRequestBehavior.AllowGet);
                }
                catch (Exception)
                {
                    throw;
                }
            }
            else
            {
                var xm = from s in db.xiangmuguanlis
                         where (s.Xiangmumulu == xiangmu && s.suoshuxueyuan == searchquery && s.bmfzrshenhe == "通过" && s.bmfzrtijiao == "已提交")
                         orderby s.Xuhao
                         select new
                         {
                             ID = s.ID,
                             XmName = s.XmName,
                             Xuhao = s.Xuhao,
                             Chuangjianshijian = s.Chuangjianshijian,
                             Beizhu = s.Beizhu,
                             suoshuxueyuan = s.suoshuxueyuan,
                             Xiangmumulu = s.Xiangmumulu,
                             //tijiaozhuantai = s.tijiaozhuantai,
                             //bmfzrtijiao = s.bmfzrtijiao,
                             bmfzrshenhe = s.bmfzrshenhe,
                             fgldshenhe = s.fgldshenhe,
                             ywzyshenhe = s.ywzyshenhe,
                             //username = s.username,
                             //bmfzryijian = s.bmfzryijian,
                             pingweishenhe = s.pingweishenhe,
                             zongjine = s.zongjine,
                             bmfzrtijiao = s.bmfzrtijiao
                         };
                try
                {
                    // 返回到前台的值必须按照如下的格式包括 total and rows 
                    var easyUIPages = new Dictionary<string, object>();
                    easyUIPages.Add("total", xm.Count());
                    easyUIPages.Add("rows", xm.ToPagedList(page, rows));
                    return Json(easyUIPages, JsonRequestBehavior.AllowGet);
                    //return Json(xm.ToList());
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
                                    

        //分管领导参与审核，业务专员登录，可以看到通过的项目
        [HttpPost]
        [Authorize(Roles = "业务专员")]
        public JsonResult ywzy_shenhe_xm_tg(string xiangmu, string searchquery)
        {
            int page = (Request.Form["page"] != null) ? Int32.Parse(Request.Form["page"]) : 1;
            int rows = (Request.Form["rows"] != null) ? Int32.Parse(Request.Form["rows"]) : 10;

            if (searchquery == "所有部门")
            {
                var xm = from s in db.xiangmuguanlis
                         where (s.Xiangmumulu == xiangmu && s.ywzyshenhe == "通过" && s.fgldshenhe == "通过")
                         orderby s.Xuhao
                         select new
                         {
                             ID = s.ID,
                             XmName = s.XmName,
                             Xuhao = s.Xuhao,
                             Chuangjianshijian = s.Chuangjianshijian,
                             Beizhu = s.Beizhu,
                             suoshuxueyuan = s.suoshuxueyuan,
                             Xiangmumulu = s.Xiangmumulu,
                             //tijiaozhuantai = s.tijiaozhuantai,
                             //bmfzrtijiao = s.bmfzrtijiao,
                             bmfzrshenhe = s.bmfzrshenhe,
                             fgldshenhe = s.fgldshenhe,
                             ywzyshenhe = s.ywzyshenhe,
                             //username = s.username,
                             //bmfzryijian = s.bmfzryijian,
                             pingweishenhe = s.pingweishenhe,
                             zongjine = s.zongjine
                         };
                try
                {
                    // 返回到前台的值必须按照如下的格式包括 total and rows 
                    var easyUIPages = new Dictionary<string, object>();
                    easyUIPages.Add("total", xm.Count());
                    easyUIPages.Add("rows", xm.ToPagedList(page, rows));
                    return Json(easyUIPages, JsonRequestBehavior.AllowGet);
                }
                catch (Exception)
                {
                    throw;
                }
            }
            else
            {
                var xm = from s in db.xiangmuguanlis
                         where (s.Xiangmumulu == xiangmu && s.suoshuxueyuan == searchquery && s.ywzyshenhe == "通过" && s.fgldshenhe == "通过")
                         orderby s.Xuhao
                         select new
                         {
                             ID = s.ID,
                             XmName = s.XmName,
                             Xuhao = s.Xuhao,
                             Chuangjianshijian = s.Chuangjianshijian,
                             Beizhu = s.Beizhu,
                             suoshuxueyuan = s.suoshuxueyuan,
                             Xiangmumulu = s.Xiangmumulu,
                             //tijiaozhuantai = s.tijiaozhuantai,
                             //bmfzrtijiao = s.bmfzrtijiao,
                             bmfzrshenhe = s.bmfzrshenhe,
                             fgldshenhe = s.fgldshenhe,
                             ywzyshenhe = s.ywzyshenhe,
                             //username = s.username,
                             //bmfzryijian = s.bmfzryijian,
                             pingweishenhe = s.pingweishenhe,
                             zongjine = s.zongjine
                         };
                try
                {
                    // 返回到前台的值必须按照如下的格式包括 total and rows 
                    var easyUIPages = new Dictionary<string, object>();
                    easyUIPages.Add("total", xm.Count());
                    easyUIPages.Add("rows", xm.ToPagedList(page, rows));
                    return Json(easyUIPages, JsonRequestBehavior.AllowGet);
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }


        //分管领导参与审核，财务主管登录页面显示的datagrid数据
        [HttpPost]
        [Authorize(Roles = "财务主管")]
        public JsonResult cwzg_shenhe_xm(string xiangmu, string searchquery)
        {
            int page = (Request.Form["page"] != null) ? Int32.Parse(Request.Form["page"]) : 1;
            int rows = (Request.Form["rows"] != null) ? Int32.Parse(Request.Form["rows"]) : 10;

            if (searchquery == "所有部门")
            {
                var xm = from s in db.xiangmuguanlis
                         where (s.Xiangmumulu == xiangmu && s.ywzyshenhe == "通过")
                         orderby s.Xuhao
                         select new
                         {
                             ID = s.ID,
                             XmName = s.XmName,
                             Xuhao = s.Xuhao,
                             Chuangjianshijian = s.Chuangjianshijian,
                             Beizhu = s.Beizhu,
                             suoshuxueyuan = s.suoshuxueyuan,
                             Xiangmumulu = s.Xiangmumulu,
                             //tijiaozhuantai = s.tijiaozhuantai,
                             //bmfzrtijiao = s.bmfzrtijiao,
                             bmfzrshenhe = s.bmfzrshenhe,
                             fgldshenhe = s.fgldshenhe,
                             ywzyshenhe = s.ywzyshenhe,
                             //username = s.username,
                             //bmfzryijian = s.bmfzryijian,
                             pingweishenhe = s.pingweishenhe,
                             zongjine = s.zongjine
                         };
                try
                {
                    // 返回到前台的值必须按照如下的格式包括 total and rows 
                    var easyUIPages = new Dictionary<string, object>();
                    easyUIPages.Add("total", xm.Count());
                    easyUIPages.Add("rows", xm.ToPagedList(page, rows));
                    return Json(easyUIPages, JsonRequestBehavior.AllowGet);
                    //return Json(xm.ToList());
                }
                catch (Exception)
                {
                    throw;
                }
            }
            else
            {
                var xm = from s in db.xiangmuguanlis
                         where (s.Xiangmumulu == xiangmu && s.ywzyshenhe == "通过" && s.suoshuxueyuan == searchquery)
                         orderby s.Xuhao
                         select new
                         {
                             ID = s.ID,
                             XmName = s.XmName,
                             Xuhao = s.Xuhao,
                             Chuangjianshijian = s.Chuangjianshijian,
                             Beizhu = s.Beizhu,
                             suoshuxueyuan = s.suoshuxueyuan,
                             Xiangmumulu = s.Xiangmumulu,
                             //tijiaozhuantai = s.tijiaozhuantai,
                             //bmfzrtijiao = s.bmfzrtijiao,
                             bmfzrshenhe = s.bmfzrshenhe,
                             fgldshenhe = s.fgldshenhe,
                             ywzyshenhe = s.ywzyshenhe,
                             //username = s.username,
                             //bmfzryijian = s.bmfzryijian,
                             pingweishenhe = s.pingweishenhe,
                             zongjine = s.zongjine
                         };
                try
                {
                    // 返回到前台的值必须按照如下的格式包括 total and rows 
                    var easyUIPages = new Dictionary<string, object>();
                    easyUIPages.Add("total", xm.Count());
                    easyUIPages.Add("rows", xm.ToPagedList(page, rows));
                    return Json(easyUIPages, JsonRequestBehavior.AllowGet);
                    //return Json(xm.ToList());
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }


        //分管领导参与审核，业务专员登录，可以看到不通过的项目，  这里业务专员和财务主管公用一个方法
        [HttpPost]
        [Authorize(Roles = "业务专员,财务主管")]
        public JsonResult ywzy_shenhe_xm_btg(string xiangmu, string searchquery)
        {
            int page = (Request.Form["page"] != null) ? Int32.Parse(Request.Form["page"]) : 1;

            int rows = (Request.Form["rows"] != null) ? Int32.Parse(Request.Form["rows"]) : 10;

            if (searchquery == "所有部门")
            {
                var xm = from s in db.xiangmuguanlis
                         where (s.Xiangmumulu == xiangmu && s.ywzyshenhe == "未通过" && s.fgldshenhe == "通过")
                         orderby s.Xuhao
                         select new
                         {
                             ID = s.ID,
                             XmName = s.XmName,
                             Xuhao = s.Xuhao,
                             Chuangjianshijian = s.Chuangjianshijian,
                             Beizhu = s.Beizhu,
                             suoshuxueyuan = s.suoshuxueyuan,
                             Xiangmumulu = s.Xiangmumulu,
                             //tijiaozhuantai = s.tijiaozhuantai,
                             //bmfzrtijiao = s.bmfzrtijiao,
                             bmfzrshenhe = s.bmfzrshenhe,
                             fgldshenhe = s.fgldshenhe,
                             ywzyshenhe = s.ywzyshenhe,
                             //username = s.username,
                             //bmfzryijian = s.bmfzryijian,
                             pingweishenhe = s.pingweishenhe,
                             zongjine = s.zongjine,
                             pingweitijiao = s.pingweitijiao
                         };
                try
                {
                    // 返回到前台的值必须按照如下的格式包括 total and rows 
                    var easyUIPages = new Dictionary<string, object>();
                    easyUIPages.Add("total", xm.Count());
                    easyUIPages.Add("rows", xm.ToPagedList(page, rows));
                    return Json(easyUIPages, JsonRequestBehavior.AllowGet);
                    //return Json(xm.ToList());
                }
                catch (Exception)
                {
                    throw;
                }
            }
            else
            {
                var xm = from s in db.xiangmuguanlis
                         where (s.Xiangmumulu == xiangmu && s.suoshuxueyuan == searchquery && s.ywzyshenhe == "未通过" && s.fgldshenhe == "通过")
                         orderby s.Xuhao
                         select new
                         {
                             ID = s.ID,
                             XmName = s.XmName,
                             Xuhao = s.Xuhao,
                             Chuangjianshijian = s.Chuangjianshijian,
                             Beizhu = s.Beizhu,
                             suoshuxueyuan = s.suoshuxueyuan,
                             Xiangmumulu = s.Xiangmumulu,
                             //tijiaozhuantai = s.tijiaozhuantai,
                             //bmfzrtijiao = s.bmfzrtijiao,
                             bmfzrshenhe = s.bmfzrshenhe,
                             fgldshenhe = s.fgldshenhe,
                             ywzyshenhe = s.ywzyshenhe,
                             //username = s.username,
                             //bmfzryijian = s.bmfzryijian,
                             pingweishenhe = s.pingweishenhe,
                             zongjine = s.zongjine
                         };
                try
                {
                    // 返回到前台的值必须按照如下的格式包括 total and rows 
                    var easyUIPages = new Dictionary<string, object>();
                    easyUIPages.Add("total", xm.Count());
                    easyUIPages.Add("rows", xm.ToPagedList(page, rows));
                    return Json(easyUIPages, JsonRequestBehavior.AllowGet);
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }


        //分管领导参与审核，业务专员登录，可以看到不通过的项目 ，  这里业务专员和财务主管公用一个方法
        [HttpPost]
        [Authorize(Roles = "业务专员,财务主管")]
        public JsonResult ywzy_shenhe_xm_wsh(string xiangmu, string searchquery)
        {
            int page = (Request.Form["page"] != null) ? Int32.Parse(Request.Form["page"]) : 1;
            int rows = (Request.Form["rows"] != null) ? Int32.Parse(Request.Form["rows"]) : 10;

            if (searchquery == "所有部门")
            {
                var xm = from s in db.xiangmuguanlis
                         where (s.Xiangmumulu == xiangmu && s.ywzyshenhe == "未审核" && s.fgldshenhe == "通过" && s.pingweitijiao == "已提交")
                         orderby s.Xuhao
                         select new
                         {
                             ID = s.ID,
                             XmName = s.XmName,
                             Xuhao = s.Xuhao,
                             Chuangjianshijian = s.Chuangjianshijian,
                             Beizhu = s.Beizhu,
                             suoshuxueyuan = s.suoshuxueyuan,
                             Xiangmumulu = s.Xiangmumulu,
                             //tijiaozhuantai = s.tijiaozhuantai,
                             //bmfzrtijiao = s.bmfzrtijiao,
                             bmfzrshenhe = s.bmfzrshenhe,
                             fgldshenhe = s.fgldshenhe,
                             ywzyshenhe = s.ywzyshenhe,
                             //username = s.username,
                             //bmfzryijian = s.bmfzryijian,
                             pingweishenhe = s.pingweishenhe,
                             zongjine = s.zongjine,
                             pingweitijiao = s.pingweitijiao
                         };
                try
                {
                    // 返回到前台的值必须按照如下的格式包括 total and rows 
                    var easyUIPages = new Dictionary<string, object>();
                    easyUIPages.Add("total", xm.Count());
                    easyUIPages.Add("rows", xm.ToPagedList(page, rows));
                    return Json(easyUIPages, JsonRequestBehavior.AllowGet);
                    //return Json(xm.ToList());
                }
                catch (Exception)
                {
                    throw;
                }
            }
            else
            {
                var xm = from s in db.xiangmuguanlis
                         where (s.Xiangmumulu == xiangmu && s.suoshuxueyuan == searchquery && s.ywzyshenhe == "未审核" && s.fgldshenhe == "通过" && s.pingweitijiao == "已提交")
                         orderby s.Xuhao
                         select new
                         {
                             ID = s.ID,
                             XmName = s.XmName,
                             Xuhao = s.Xuhao,
                             Chuangjianshijian = s.Chuangjianshijian,
                             Beizhu = s.Beizhu,
                             suoshuxueyuan = s.suoshuxueyuan,
                             Xiangmumulu = s.Xiangmumulu,
                             //tijiaozhuantai = s.tijiaozhuantai,
                             //bmfzrtijiao = s.bmfzrtijiao,
                             bmfzrshenhe = s.bmfzrshenhe,
                             fgldshenhe = s.fgldshenhe,
                             ywzyshenhe = s.ywzyshenhe,
                             //username = s.username,
                             //bmfzryijian = s.bmfzryijian,
                             pingweishenhe = s.pingweishenhe,
                             zongjine = s.zongjine,
                             pingweitijiao = s.pingweitijiao
                         };
                try
                {
                    // 返回到前台的值必须按照如下的格式包括 total and rows 
                    var easyUIPages = new Dictionary<string, object>();
                    easyUIPages.Add("total", xm.Count());
                    easyUIPages.Add("rows", xm.ToPagedList(page, rows));
                    return Json(easyUIPages, JsonRequestBehavior.AllowGet);
                    //return Json(xm.ToList());
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }


        //分管领导参与审核，业务专员登录，可以看到分管领导通过的项目，根据这个参考分配评委
        [HttpPost]
        [Authorize(Roles = "业务专员")]
        public JsonResult ywzy_shenhe_xm_dfp(string xiangmu, string searchquery)
        {
            int page = (Request.Form["page"] != null) ? Int32.Parse(Request.Form["page"]) : 1;

            int rows = (Request.Form["rows"] != null) ? Int32.Parse(Request.Form["rows"]) : 10;

            if (searchquery == "所有部门")
            {
                //pwsuoshuxueyuan = searchquery;
                var xm = from s in db.xiangmuguanlis
                         where s.Xiangmumulu == xiangmu && s.ywzyshenhe == "未审核" && s.fgldshenhe == "通过" && s.pingweishenhe == "未审核"
                         orderby s.suoshuxueyuan, s.Xuhao
                         select new
                         {
                             ID = s.ID,
                             XmName = s.XmName,
                             Xuhao = s.Xuhao,
                             Chuangjianshijian = s.Chuangjianshijian,
                             Beizhu = s.Beizhu,
                             suoshuxueyuan = s.suoshuxueyuan,
                             Xiangmumulu = s.Xiangmumulu,
                             //tijiaozhuantai = s.tijiaozhuantai,
                             //bmfzrtijiao = s.bmfzrtijiao,
                             bmfzrshenhe = s.bmfzrshenhe,
                             fgldshenhe = s.fgldshenhe,
                             ywzyshenhe = s.ywzyshenhe,
                             //username = s.username,
                             //bmfzryijian = s.bmfzryijian,
                             pingweishenhe = s.pingweishenhe,
                             zongjine = s.zongjine,
                             pingweitijiao = s.pingweitijiao
                         };
                try
                {
                    // 返回到前台的值必须按照如下的格式包括 total and rows 
                    var easyUIPages = new Dictionary<string, object>();
                    easyUIPages.Add("total", xm.Count());
                    easyUIPages.Add("rows", xm.ToPagedList(page, rows));
                    return Json(easyUIPages, JsonRequestBehavior.AllowGet);
                    //return Json(xm.ToList());
                }
                catch (Exception)
                {
                    throw;
                }
            }
            else
            {
                var xm = from s in db.xiangmuguanlis
                         where s.Xiangmumulu == xiangmu && s.suoshuxueyuan == searchquery && s.ywzyshenhe == "未审核" && s.fgldshenhe == "通过" && s.pingweishenhe == "未审核"
                         orderby s.suoshuxueyuan, s.Xuhao
                         select new
                         {
                             ID = s.ID,
                             XmName = s.XmName,
                             Xuhao = s.Xuhao,
                             Chuangjianshijian = s.Chuangjianshijian,
                             Beizhu = s.Beizhu,
                             suoshuxueyuan = s.suoshuxueyuan,
                             Xiangmumulu = s.Xiangmumulu,
                             //tijiaozhuantai = s.tijiaozhuantai,
                             //bmfzrtijiao = s.bmfzrtijiao,
                             bmfzrshenhe = s.bmfzrshenhe,
                             fgldshenhe = s.fgldshenhe,
                             ywzyshenhe = s.ywzyshenhe,
                             //username = s.username,
                             //bmfzryijian = s.bmfzryijian,
                             pingweishenhe = s.pingweishenhe,
                             zongjine = s.zongjine
                         };
                try
                {
                    // 返回到前台的值必须按照如下的格式包括 total and rows 
                    var easyUIPages = new Dictionary<string, object>();
                    easyUIPages.Add("total", xm.Count());
                    easyUIPages.Add("rows", xm.ToPagedList(page, rows));
                    return Json(easyUIPages, JsonRequestBehavior.AllowGet);
                    //return Json(xm.ToList());
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }


        //分管领导不参与审核，评委待审核项目datagrid显示列表pw_bushen_xm_wsh
        [HttpPost]
        [Authorize(Roles = "评委")]
        public JsonResult pw_bushen_xm_wsh(string xiangmu, string loingid, string searchquery)
        {
            int page = (Request.Form["page"] != null) ? Int32.Parse(Request.Form["page"]) : 1;

            int rows = (Request.Form["rows"] != null) ? Int32.Parse(Request.Form["rows"]) : 10;
            string pwsuoshuxueyuan = customidentity.suoshuxueyuan(loingid);  //由登录人的id取出登录人的所属学院值，根据所属学院值过滤显示的信息。登录人员只能显示他所在部门的信息

            if (searchquery != "所有评审部门")
            {
                pwsuoshuxueyuan = searchquery;
            }
            var xm = from s in db.xiangmuguanlis
                     where (s.Xiangmumulu == xiangmu && pwsuoshuxueyuan.Contains(s.suoshuxueyuan)) && (s.pingweishenhe == "未审核" || s.pingweishenhe == "撤回") && s.bmfzrtijiao == "已提交"
                     orderby s.suoshuxueyuan, s.Xuhao
                     select new
                     {
                         ID = s.ID,
                         XmName = s.XmName,
                         Xuhao = s.Xuhao,
                         Chuangjianshijian = s.Chuangjianshijian,
                         Beizhu = s.Beizhu,
                         suoshuxueyuan = s.suoshuxueyuan,
                         Xiangmumulu = s.Xiangmumulu,
                         //tijiaozhuantai = s.tijiaozhuantai,
                         bmfzrtijiao = s.bmfzrtijiao,
                         bmfzrshenhe = s.bmfzrshenhe,
                         fgldshenhe = s.fgldshenhe,
                         ywzyshenhe = s.ywzyshenhe,
                         //username = s.username,
                         //bmfzryijian = s.bmfzryijian,
                         pingweishenhe = s.pingweishenhe,
                         zongjine = s.zongjine,
                         pingweitijiao = s.pingweitijiao
                     };
            try
            {
                // 返回到前台的值必须按照如下的格式包括 total and rows 
                var easyUIPages = new Dictionary<string, object>();
                easyUIPages.Add("total", xm.Count());
                easyUIPages.Add("rows", xm.ToPagedList(page, rows));
                return Json(easyUIPages, JsonRequestBehavior.AllowGet);
                //return Json(xm.ToList());
            }
            catch (Exception)
            {
                throw;
            }
        }


        //分管领导不参与审核，评委审核完成datagrid显示项目列表
        [HttpPost]
        [Authorize(Roles = "评委")]
        public JsonResult pw_bushen_xm_wc(string xiangmu, string loingid, string searchquery)
        {
            int page = (Request.Form["page"] != null) ? Int32.Parse(Request.Form["page"]) : 1;

            int rows = (Request.Form["rows"] != null) ? Int32.Parse(Request.Form["rows"]) : 10;

            //string fgldshenhefou = mulucustom.fgxzshenhefou(xiangmu);
            string pwsuoshuxueyuan = customidentity.suoshuxueyuan(loingid);  //由登录人的id取出登录人的所属学院值，根据所属学院值过滤显示的信息。登录人员只能显示他所在部门的信息
                                 
            if (searchquery != "所有评审部门")
            {
                pwsuoshuxueyuan = searchquery;
            }

            var xm = from s in db.xiangmuguanlis
                     where (s.Xiangmumulu == xiangmu && pwsuoshuxueyuan.Contains(s.suoshuxueyuan)) && s.pingweishenhe != "未审核" && s.pingweishenhe != "撤回" && s.bmfzrshenhe == "通过"
                     orderby s.suoshuxueyuan, s.Xuhao
                     select new
                     {
                         ID = s.ID,
                         XmName = s.XmName,
                         Xuhao = s.Xuhao,
                         Chuangjianshijian = s.Chuangjianshijian,
                         Beizhu = s.Beizhu,
                         suoshuxueyuan = s.suoshuxueyuan,
                         Xiangmumulu = s.Xiangmumulu,
                         //tijiaozhuantai = s.tijiaozhuantai,
                         bmfzrtijiao = s.bmfzrtijiao,
                         bmfzrshenhe = s.bmfzrshenhe,
                         fgldshenhe = s.fgldshenhe,
                         ywzyshenhe = s.ywzyshenhe,
                         //username = s.username,
                         //bmfzryijian = s.bmfzryijian,
                         pingweishenhe = s.pingweishenhe,
                         zongjine = s.zongjine,
                         pingweitijiao = s.pingweitijiao
                     };
            try
            {
                // 返回到前台的值必须按照如下的格式包括 total and rows 
                var easyUIPages = new Dictionary<string, object>();
                easyUIPages.Add("total", xm.Count());
                easyUIPages.Add("rows", xm.ToPagedList(page, rows));
                return Json(easyUIPages, JsonRequestBehavior.AllowGet);
                //return Json(xm.ToList());
            }
            catch (Exception)
            {
                throw;
            }
        }

        //分管领导参与审核，评委待审核项目datagrid显示列表
        [HttpPost]
        [Authorize(Roles = "评委")]
        public JsonResult pw_shenhe_xm_wsh(string xiangmu, string loingid, string searchquery)
        {
            int page = (Request.Form["page"] != null) ? Int32.Parse(Request.Form["page"]) : 1;

            int rows = (Request.Form["rows"] != null) ? Int32.Parse(Request.Form["rows"]) : 10;

            //string fgldshenhefou = mulucustom.fgxzshenhefou(xiangmu);
            string pwsuoshuxueyuan = customidentity.suoshuxueyuan(loingid);  //由登录人的id取出登录人的所属学院值，根据所属学院值过滤显示的信息。登录人员只能显示他所在部门的信息
                       
            if (searchquery != "所有评审部门")
            {
                pwsuoshuxueyuan = searchquery;
            }

            var xm = from s in db.xiangmuguanlis
                     where (s.Xiangmumulu == xiangmu && pwsuoshuxueyuan.Contains(s.suoshuxueyuan)) && (s.pingweishenhe == "未审核" || s.pingweishenhe == "撤回") && s.fgldshenhe == "通过" && s.bmfzrtijiao == "已提交"
                     orderby s.suoshuxueyuan, s.Xuhao
                     select new
                     {
                         ID = s.ID,
                         XmName = s.XmName,
                         Xuhao = s.Xuhao,
                         Chuangjianshijian = s.Chuangjianshijian,
                         Beizhu = s.Beizhu,
                         suoshuxueyuan = s.suoshuxueyuan,
                         Xiangmumulu = s.Xiangmumulu,
                         //tijiaozhuantai = s.tijiaozhuantai,
                         bmfzrtijiao = s.bmfzrtijiao,
                         bmfzrshenhe = s.bmfzrshenhe,
                         fgldshenhe = s.fgldshenhe,
                         ywzyshenhe = s.ywzyshenhe,
                         //username = s.username,
                         //bmfzryijian = s.bmfzryijian,
                         pingweishenhe = s.pingweishenhe,
                         zongjine = s.zongjine,
                         pingweitijiao = s.pingweitijiao
                     };
            try
            {
                // 返回到前台的值必须按照如下的格式包括 total and rows 
                var easyUIPages = new Dictionary<string, object>();
                easyUIPages.Add("total", xm.Count());
                easyUIPages.Add("rows", xm.ToPagedList(page, rows));
                return Json(easyUIPages, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                throw;
            }
        }


        //分管领导参与审核，评委审核完成datagrid显示项目列表
        [HttpPost]
        [Authorize(Roles = "评委")]
        public JsonResult pw_shenhe_xm_wc(string xiangmu, string loingid, string searchquery)
        {
            int page = (Request.Form["page"] != null) ? Int32.Parse(Request.Form["page"]) : 1;

            int rows = (Request.Form["rows"] != null) ? Int32.Parse(Request.Form["rows"]) : 10;
            string pwsuoshuxueyuan = customidentity.suoshuxueyuan(loingid);  //由登录人的id取出登录人的所属学院值，根据所属学院值过滤显示的信息。登录人员只能显示他所在部门的信息

            if (searchquery != "所有评审部门")
            {
                pwsuoshuxueyuan = searchquery;
            }

            var xm = from s in db.xiangmuguanlis
                     where (s.Xiangmumulu == xiangmu && pwsuoshuxueyuan.Contains(s.suoshuxueyuan)) && s.fgldshenhe == "通过" && s.bmfzrtijiao == "已提交" && s.pingweishenhe != "未审核" && s.pingweishenhe != "撤销"
                     orderby s.suoshuxueyuan, s.Xuhao
                     select new
                     {
                         ID = s.ID,
                         XmName = s.XmName,
                         Xuhao = s.Xuhao,
                         Chuangjianshijian = s.Chuangjianshijian,
                         Beizhu = s.Beizhu,
                         suoshuxueyuan = s.suoshuxueyuan,
                         Xiangmumulu = s.Xiangmumulu,
                         //tijiaozhuantai = s.tijiaozhuantai,
                         bmfzrtijiao = s.bmfzrtijiao,
                         bmfzrshenhe = s.bmfzrshenhe,
                         fgldshenhe = s.fgldshenhe,
                         ywzyshenhe = s.ywzyshenhe,
                         //username = s.username,
                         //bmfzryijian = s.bmfzryijian,
                         pingweishenhe = s.pingweishenhe,
                         zongjine = s.zongjine,
                         pingweitijiao = s.pingweitijiao
                     };
            try
            {
                // 返回到前台的值必须按照如下的格式包括 total and rows 
                var easyUIPages = new Dictionary<string, object>();
                easyUIPages.Add("total", xm.Count());
                easyUIPages.Add("rows", xm.ToPagedList(page, rows));
                return Json(easyUIPages, JsonRequestBehavior.AllowGet);
                //return Json(xm.ToList());
            }
            catch (Exception)
            {
                throw;
            }
        }


        //分管领导参与审核，业务专员登录，可以看到通过的项目
        [HttpPost]
        [Authorize(Roles = "业务专员")]
        public JsonResult ywzy_bushen_xm_tg(string xiangmu, string searchquery)
        {
            int page = (Request.Form["page"] != null) ? Int32.Parse(Request.Form["page"]) : 1;
            int rows = (Request.Form["rows"] != null) ? Int32.Parse(Request.Form["rows"]) : 10;

            if (searchquery == "所有部门")
            {
                var xm = from s in db.xiangmuguanlis
                         where (s.Xiangmumulu == xiangmu && s.ywzyshenhe == "通过" && s.bmfzrshenhe == "通过" && s.pingweitijiao == "已提交")
                         orderby s.Xuhao
                         select new
                         {
                             ID = s.ID,
                             XmName = s.XmName,
                             Xuhao = s.Xuhao,
                             Chuangjianshijian = s.Chuangjianshijian,
                             Beizhu = s.Beizhu,
                             suoshuxueyuan = s.suoshuxueyuan,
                             Xiangmumulu = s.Xiangmumulu,
                             //tijiaozhuantai = s.tijiaozhuantai,
                             //bmfzrtijiao = s.bmfzrtijiao,
                             bmfzrshenhe = s.bmfzrshenhe,
                             //fgldshenhe = s.fgldshenhe,
                             ywzyshenhe = s.ywzyshenhe,
                             //username = s.username,
                             //bmfzryijian = s.bmfzryijian,
                             pingweishenhe = s.pingweishenhe,
                             zongjine = s.zongjine,
                             pingweitijiao = s.pingweitijiao
                         };
                try
                {
                    // 返回到前台的值必须按照如下的格式包括 total and rows 
                    var easyUIPages = new Dictionary<string, object>();
                    easyUIPages.Add("total", xm.Count());
                    easyUIPages.Add("rows", xm.ToPagedList(page, rows));
                    return Json(easyUIPages, JsonRequestBehavior.AllowGet);
                    //return Json(xm.ToList());
                }
                catch (Exception)
                {
                    throw;
                }
            }
            else
            {
                var xm = from s in db.xiangmuguanlis
                         where (s.Xiangmumulu == xiangmu && s.suoshuxueyuan == searchquery && s.ywzyshenhe == "通过" && s.bmfzrshenhe == "通过" && s.pingweitijiao == "已提交")
                         orderby s.Xuhao
                         select new
                         {
                             ID = s.ID,
                             XmName = s.XmName,
                             Xuhao = s.Xuhao,
                             Chuangjianshijian = s.Chuangjianshijian,
                             Beizhu = s.Beizhu,
                             suoshuxueyuan = s.suoshuxueyuan,
                             Xiangmumulu = s.Xiangmumulu,
                             //tijiaozhuantai = s.tijiaozhuantai,
                             //bmfzrtijiao = s.bmfzrtijiao,
                             bmfzrshenhe = s.bmfzrshenhe,
                             //fgldshenhe = s.fgldshenhe,
                             ywzyshenhe = s.ywzyshenhe,
                             //username = s.username,
                             //bmfzryijian = s.bmfzryijian,
                             pingweishenhe = s.pingweishenhe,
                             zongjine = s.zongjine
                         };
                try
                {
                    // 返回到前台的值必须按照如下的格式包括 total and rows 
                    var easyUIPages = new Dictionary<string, object>();
                    easyUIPages.Add("total", xm.Count());
                    easyUIPages.Add("rows", xm.ToPagedList(page, rows));
                    return Json(easyUIPages, JsonRequestBehavior.AllowGet);
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }


        //分管领导参与审核，业务专员登录，可以看到不通过的项目
        [HttpPost]
        [Authorize(Roles = "业务专员,财务主管")]
        public JsonResult ywzy_bushen_xm_btg(string xiangmu, string searchquery)
        {
            int page = (Request.Form["page"] != null) ? Int32.Parse(Request.Form["page"]) : 1;

            int rows = (Request.Form["rows"] != null) ? Int32.Parse(Request.Form["rows"]) : 10;

            if (searchquery == "所有部门")
            {
                var xm = from s in db.xiangmuguanlis
                         where (s.Xiangmumulu == xiangmu && s.ywzyshenhe == "未通过" && s.bmfzrshenhe == "通过" && s.pingweitijiao == "已提交")
                         orderby s.Xuhao
                         select new
                         {
                             ID = s.ID,
                             XmName = s.XmName,
                             Xuhao = s.Xuhao,
                             Chuangjianshijian = s.Chuangjianshijian,
                             Beizhu = s.Beizhu,
                             suoshuxueyuan = s.suoshuxueyuan,
                             Xiangmumulu = s.Xiangmumulu,
                             //tijiaozhuantai = s.tijiaozhuantai,
                             //bmfzrtijiao = s.bmfzrtijiao,
                             bmfzrshenhe = s.bmfzrshenhe,
                             //fgldshenhe = s.fgldshenhe,
                             ywzyshenhe = s.ywzyshenhe,
                             //username = s.username,
                             //bmfzryijian = s.bmfzryijian,
                             pingweishenhe = s.pingweishenhe,
                             zongjine = s.zongjine,
                             pingweitijiao = s.pingweitijiao
                         };
                try
                {
                    // 返回到前台的值必须按照如下的格式包括 total and rows 
                    var easyUIPages = new Dictionary<string, object>();
                    easyUIPages.Add("total", xm.Count());
                    easyUIPages.Add("rows", xm.ToPagedList(page, rows));
                    return Json(easyUIPages, JsonRequestBehavior.AllowGet);
                }
                catch (Exception)
                {
                    throw;
                }
            }
            else
            {
                var xm = from s in db.xiangmuguanlis
                         where (s.Xiangmumulu == xiangmu && s.suoshuxueyuan == searchquery && s.ywzyshenhe == "未通过" && s.bmfzrshenhe == "通过" && s.pingweitijiao == "已提交")
                         orderby s.Xuhao
                         select new
                         {
                             ID = s.ID,
                             XmName = s.XmName,
                             Xuhao = s.Xuhao,
                             Chuangjianshijian = s.Chuangjianshijian,
                             Beizhu = s.Beizhu,
                             suoshuxueyuan = s.suoshuxueyuan,
                             Xiangmumulu = s.Xiangmumulu,
                             //tijiaozhuantai = s.tijiaozhuantai,
                             //bmfzrtijiao = s.bmfzrtijiao,
                             bmfzrshenhe = s.bmfzrshenhe,
                             //fgldshenhe = s.fgldshenhe,
                             ywzyshenhe = s.ywzyshenhe,
                             //username = s.username,
                             //bmfzryijian = s.bmfzryijian,
                             pingweishenhe = s.pingweishenhe,
                             zongjine = s.zongjine
                         };
                try
                {
                    // 返回到前台的值必须按照如下的格式包括 total and rows 
                    var easyUIPages = new Dictionary<string, object>();
                    easyUIPages.Add("total", xm.Count());
                    easyUIPages.Add("rows", xm.ToPagedList(page, rows));
                    return Json(easyUIPages, JsonRequestBehavior.AllowGet);
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }


        //分管领导参与审核，业务专员登录，可以看到不通过的项目
        [HttpPost]
        [Authorize(Roles = "业务专员,财务主管")]
        public JsonResult ywzy_bushen_xm_wsh(string xiangmu, string searchquery)
        {
            int page = (Request.Form["page"] != null) ? Int32.Parse(Request.Form["page"]) : 1;

            int rows = (Request.Form["rows"] != null) ? Int32.Parse(Request.Form["rows"]) : 10;

            if (searchquery == "所有部门")
            {
                var xm = from s in db.xiangmuguanlis
                         where (s.Xiangmumulu == xiangmu && s.ywzyshenhe == "未审核" && s.bmfzrshenhe == "通过" && s.pingweitijiao == "已提交")
                         orderby s.Xuhao
                         select new
                         {
                             ID = s.ID,
                             XmName = s.XmName,
                             Xuhao = s.Xuhao,
                             Chuangjianshijian = s.Chuangjianshijian,
                             Beizhu = s.Beizhu,
                             suoshuxueyuan = s.suoshuxueyuan,
                             Xiangmumulu = s.Xiangmumulu,
                             //tijiaozhuantai = s.tijiaozhuantai,
                             bmfzrtijiao = s.bmfzrtijiao,
                             bmfzrshenhe = s.bmfzrshenhe,
                             //fgldshenhe = s.fgldshenhe,
                             ywzyshenhe = s.ywzyshenhe,
                             //username = s.username,
                             //bmfzryijian = s.bmfzryijian,
                             pingweishenhe = s.pingweishenhe,
                             pingweitijiao = s.pingweitijiao,
                             zongjine = s.zongjine
                         };
                try
                {
                    // 返回到前台的值必须按照如下的格式包括 total and rows 
                    var easyUIPages = new Dictionary<string, object>();
                    easyUIPages.Add("total", xm.Count());
                    easyUIPages.Add("rows", xm.ToPagedList(page, rows));
                    return Json(easyUIPages, JsonRequestBehavior.AllowGet);
                }
                catch (Exception)
                {
                    throw;
                }
            }
            else
            {
                var xm = from s in db.xiangmuguanlis
                         where (s.Xiangmumulu == xiangmu && s.suoshuxueyuan == searchquery && s.ywzyshenhe == "未审核" && s.bmfzrshenhe == "通过" && s.pingweitijiao == "已提交")
                         orderby s.Xuhao
                         select new
                         {
                             ID = s.ID,
                             XmName = s.XmName,
                             Xuhao = s.Xuhao,
                             Chuangjianshijian = s.Chuangjianshijian,
                             Beizhu = s.Beizhu,
                             suoshuxueyuan = s.suoshuxueyuan,
                             Xiangmumulu = s.Xiangmumulu,
                             //tijiaozhuantai = s.tijiaozhuantai,
                             bmfzrtijiao = s.bmfzrtijiao,
                             bmfzrshenhe = s.bmfzrshenhe,
                             //fgldshenhe = s.fgldshenhe,
                             ywzyshenhe = s.ywzyshenhe,
                             //username = s.username,
                             //bmfzryijian = s.bmfzryijian,
                             pingweishenhe = s.pingweishenhe,
                             pingweitijiao = s.pingweitijiao,
                             zongjine = s.zongjine
                         };
                try
                {
                    // 返回到前台的值必须按照如下的格式包括 total and rows 
                    var easyUIPages = new Dictionary<string, object>();
                    easyUIPages.Add("total", xm.Count());
                    easyUIPages.Add("rows", xm.ToPagedList(page, rows));
                    return Json(easyUIPages, JsonRequestBehavior.AllowGet);
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }


        //分管领导不参与审核，业务专员登录，可以看到部门负责人通过提交的项目,通过这个分配评委
        [HttpPost]
        [Authorize(Roles = "业务专员")]
        public JsonResult ywzy_bushen_xm_dfp(string xiangmu, string searchquery)
        {
            int page = (Request.Form["page"] != null) ? Int32.Parse(Request.Form["page"]) : 1;

            int rows = (Request.Form["rows"] != null) ? Int32.Parse(Request.Form["rows"]) : 10;

            if (searchquery == "所有部门")
            {
                var xm = from s in db.xiangmuguanlis
                         where (s.Xiangmumulu == xiangmu && s.ywzyshenhe == "未审核" && s.bmfzrtijiao == "已提交" && s.pingweishenhe == "未审核")
                         orderby s.Xuhao
                         select new
                         {
                             ID = s.ID,
                             XmName = s.XmName,
                             Xuhao = s.Xuhao,
                             Chuangjianshijian = s.Chuangjianshijian,
                             Beizhu = s.Beizhu,
                             suoshuxueyuan = s.suoshuxueyuan,
                             Xiangmumulu = s.Xiangmumulu,
                             //tijiaozhuantai = s.tijiaozhuantai,
                             bmfzrtijiao = s.bmfzrtijiao,
                             bmfzrshenhe = s.bmfzrshenhe,
                             //fgldshenhe = s.fgldshenhe,
                             ywzyshenhe = s.ywzyshenhe,
                             //username = s.username,
                             //bmfzryijian = s.bmfzryijian,
                             pingweishenhe = s.pingweishenhe,
                             pingweitijiao = s.pingweitijiao,
                             zongjine = s.zongjine
                         };
                try
                {
                    // 返回到前台的值必须按照如下的格式包括 total and rows 
                    var easyUIPages = new Dictionary<string, object>();
                    easyUIPages.Add("total", xm.Count());
                    easyUIPages.Add("rows", xm.ToPagedList(page, rows));
                    return Json(easyUIPages, JsonRequestBehavior.AllowGet);
                }
                catch (Exception)
                {
                    throw;
                }
            }
            else
            {
                var xm = from s in db.xiangmuguanlis
                         where (s.Xiangmumulu == xiangmu && s.suoshuxueyuan == searchquery && s.ywzyshenhe == "未审核" && s.bmfzrtijiao == "已提交" && s.pingweishenhe == "未审核")
                         orderby s.Xuhao
                         select new
                         {
                             ID = s.ID,
                             XmName = s.XmName,
                             Xuhao = s.Xuhao,
                             Chuangjianshijian = s.Chuangjianshijian,
                             Beizhu = s.Beizhu,
                             suoshuxueyuan = s.suoshuxueyuan,
                             Xiangmumulu = s.Xiangmumulu,
                             //tijiaozhuantai = s.tijiaozhuantai,
                             bmfzrtijiao = s.bmfzrtijiao,
                             bmfzrshenhe = s.bmfzrshenhe,
                             //fgldshenhe = s.fgldshenhe,
                             ywzyshenhe = s.ywzyshenhe,
                             //username = s.username,
                             //bmfzryijian = s.bmfzryijian,
                             pingweishenhe = s.pingweishenhe,
                             pingweitijiao = s.pingweitijiao,
                             zongjine = s.zongjine
                         };
                try
                {
                    // 返回到前台的值必须按照如下的格式包括 total and rows 
                    var easyUIPages = new Dictionary<string, object>();
                    easyUIPages.Add("total", xm.Count());
                    easyUIPages.Add("rows", xm.ToPagedList(page, rows));
                    return Json(easyUIPages, JsonRequestBehavior.AllowGet);
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }


        //分管领导审核，并且是有分管领导登录后再页面看到的  datagrid数据
        [HttpPost]
        [Authorize(Roles = "分管领导")]
        public JsonResult fgld_shenhe_xm(string xiangmu, string loingid, string searchquery)
        {
            int page = (Request.Form["page"] != null) ? Int32.Parse(Request.Form["page"]) : 1;

            int rows = (Request.Form["rows"] != null) ? Int32.Parse(Request.Form["rows"]) : 10;

            string fgldsuoshuxueyuan = customidentity.suoshuxueyuan(loingid);  //由登录人的id取出登录人的所属学院值，根据所属学院值过滤显示的信息。登录人员只能显示他所在部门的信息
            
            if (searchquery != "所有分管部门")
            {
                fgldsuoshuxueyuan = searchquery;
            }

            var xm = from s in db.xiangmuguanlis
                     where (s.Xiangmumulu == xiangmu && fgldsuoshuxueyuan.Contains(s.suoshuxueyuan) && ((s.bmfzrtijiao == "已提交" && s.fgldshenhe != "未通过") || (s.bmfzrtijiao == "未提交" && s.pingweishenhe == "撤回" && s.fgldshenhe == "通过") || (s.bmfzrtijiao == "未提交" && s.pingweishenhe == "未通过" && s.fgldshenhe == "通过")))
                     orderby s.suoshuxueyuan, s.Xuhao
                     select new
                     {
                         ID = s.ID,
                         XmName = s.XmName,
                         Xuhao = s.Xuhao,
                         Chuangjianshijian = s.Chuangjianshijian,
                         Beizhu = s.Beizhu,
                         suoshuxueyuan = s.suoshuxueyuan,
                         Xiangmumulu = s.Xiangmumulu,
                         //tijiaozhuantai = s.tijiaozhuantai,
                         bmfzrtijiao = s.bmfzrtijiao,
                         bmfzrshenhe = s.bmfzrshenhe,
                         fgldshenhe = s.fgldshenhe,
                         ywzyshenhe = s.ywzyshenhe,
                         //username = s.username,
                         //bmfzryijian = s.bmfzryijian,
                         //pingweishenhe = s.pingweishenhe,
                         zongjine = s.zongjine
                     };
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


        //分管领导审核，并且是有分管领导登录后再页面看到的  datagrid数据
        [HttpPost]
        [Authorize(Roles = "分管领导")]
        public JsonResult fgld_bushen_xm(string xiangmu, string loingid, string searchquery)
        {
            int page = (Request.Form["page"] != null) ? Int32.Parse(Request.Form["page"]) : 1;

            int rows = (Request.Form["rows"] != null) ? Int32.Parse(Request.Form["rows"]) : 10;

            string ldsuoshuxueyuan = customidentity.suoshuxueyuan(loingid);  //由登录人的id取出登录人的所属学院值，根据所属学院值过滤显示的信息。登录人员只能显示他所在部门的信息
            
            if (searchquery != "所有分管部门")
            {
                ldsuoshuxueyuan = searchquery;
            }

            var xm = from s in db.xiangmuguanlis
                     where (s.Xiangmumulu == xiangmu && ldsuoshuxueyuan.Contains(s.suoshuxueyuan) && ((s.bmfzrtijiao == "已提交") || (s.bmfzrtijiao == "未提交" && s.pingweishenhe == "撤回") || (s.bmfzrtijiao == "未提交" && s.pingweishenhe == "未通过")))
                     orderby s.suoshuxueyuan, s.Xuhao
                     select new
                     {
                         ID = s.ID,
                         XmName = s.XmName,
                         Xuhao = s.Xuhao,
                         Chuangjianshijian = s.Chuangjianshijian,
                         Beizhu = s.Beizhu,
                         suoshuxueyuan = s.suoshuxueyuan,
                         Xiangmumulu = s.Xiangmumulu,
                         //tijiaozhuantai = s.tijiaozhuantai,
                         bmfzrtijiao = s.bmfzrtijiao,
                         bmfzrshenhe = s.bmfzrshenhe,
                         fgldshenhe = s.fgldshenhe,
                         ywzyshenhe = s.ywzyshenhe,
                         //username = s.username,
                         //bmfzryijian = s.bmfzryijian,
                         //pingweishenhe = s.pingweishenhe,
                         zongjine = s.zongjine
                     };
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


        //领导审看到全部
        [Authorize(Roles = "领导")]
        public ActionResult bushen_ld_all(string muluname)  //ie下这个目录name可以正常传值
        {
            //筛选分管领导所属部门
            var GenreLst = new List<string>();
            var GenreQry = from d in db.bumenshezhis
                           orderby d.BmName
                           select d.BmName;
            GenreLst.Add("所有部门");
            GenreLst.AddRange(GenreQry.Distinct());
            ViewBag.fgld_sx_bumen = GenreLst;

            ViewBag.muluname = muluname;
            return View();
        }

        //领导审看到全部
        [Authorize(Roles = "领导")]
        public ActionResult bushen_ld_shen(string muluname, string loingid)  //ie下这个目录name可以正常传值
        {

            //筛选分管领导所属部门
            string suoshuxueyuan = customidentity.suoshuxueyuan(loingid);

            List<string> fgbmlist = new List<string>();

            string[] bm = suoshuxueyuan.Split(',');

            fgbmlist.Add("所有分管部门");
            for (var i = 0; i < bm.Length; i++)
            {
                fgbmlist.Add(bm[i]);
            }
            fgbmlist.Remove("all");
            ViewBag.fgld_sx_bumen = fgbmlist;
            ViewBag.muluname = muluname;
            return View();
        }


        //领导审看到全部
        [Authorize(Roles = "领导")]
        public ActionResult shenhe_ld_all(string muluname)  //ie下这个目录name可以正常传值
        {
            //筛选分管领导所属部门
            var GenreLst = new List<string>();
            var GenreQry = from d in db.bumenshezhis
                           orderby d.BmName
                           select d.BmName;
            GenreLst.Add("所有部门");
            GenreLst.AddRange(GenreQry.Distinct());
            ViewBag.fgld_sx_bumen = GenreLst;

            ViewBag.muluname = muluname;
            return View();
        }

        //领导审核看到审核项目
        [Authorize(Roles = "领导")]
        public ActionResult shenhe_ld_shen(string muluname, string loingid)  //ie下这个目录name可以正常传值
        {
            //筛选分管领导所属部门
            string suoshuxueyuan = customidentity.suoshuxueyuan(loingid);

            List<string> fgbmlist = new List<string>();

            fgbmlist.Add("所有分管部门");
            string[] bm = suoshuxueyuan.Split(',');

            for (var i = 0; i < bm.Length; i++)
            {
                fgbmlist.Add(bm[i]);
            }

            fgbmlist.Remove("all");
            ViewBag.fgld_sx_bumen = fgbmlist;

            ViewBag.muluname = muluname;
            return View();
        }


        //校领导登录后再页面看到的  datagrid数据  cwzg_getXiangmu
        [HttpPost]
        [Authorize(Roles = "领导")]
        public JsonResult ld_shenhe_xm_shen(string dgtitle, string loingid, string searchquery)
        {
            int page = (Request.Form["page"] != null) ? Int32.Parse(Request.Form["page"]) : 1;

            int rows = (Request.Form["rows"] != null) ? Int32.Parse(Request.Form["rows"]) : 10;

            string ldsuoshuxueyuan = customidentity.suoshuxueyuan(loingid);  //由登录人的id取出登录人的所属学院值，根据所属学院值过滤显示的信息。登录人员只能显示他所在部门的信息

            if (searchquery != "所有分管部门")
            {
                ldsuoshuxueyuan = searchquery;
            }
            var xm = from s in db.xiangmuguanlis
                     where (s.Xiangmumulu == dgtitle && ldsuoshuxueyuan.Contains(s.suoshuxueyuan) && ((s.bmfzrtijiao == "已提交" && s.fgldshenhe != "未通过") || (s.bmfzrtijiao == "未提交" && s.pingweishenhe == "撤回" && s.fgldshenhe == "通过")))
                     orderby s.Xuhao
                     select new
                     {
                         ID = s.ID,
                         XmName = s.XmName,
                         Xuhao = s.Xuhao,
                         Chuangjianshijian = s.Chuangjianshijian,
                         Beizhu = s.Beizhu,
                         suoshuxueyuan = s.suoshuxueyuan,
                         Xiangmumulu = s.Xiangmumulu,
                         //tijiaozhuantai = s.tijiaozhuantai,
                         bmfzrtijiao = s.bmfzrtijiao,
                         bmfzrshenhe = s.bmfzrshenhe,
                         fgldshenhe = s.fgldshenhe,
                         ywzyshenhe = s.ywzyshenhe,
                         //username = s.username,
                         //bmfzryijian = s.bmfzryijian,
                         //pingweishenhe = s.pingweishenhe,
                         zongjine = s.zongjine
                     };
            try
            {
                // 返回到前台的值必须按照如下的格式包括 total and rows 
                var easyUIPages = new Dictionary<string, object>();
                easyUIPages.Add("total", xm.Count());
                easyUIPages.Add("rows", xm.ToPagedList(page, rows));
                return Json(easyUIPages, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                throw;
            }
        }


        //不审领导登录，看到分管的数据
        [HttpPost]
        [Authorize(Roles = "领导")]
        public JsonResult ld_bushen_shen_xm(string xiangmu, string loingid, string searchquery)//
        {
            int page = (Request.Form["page"] != null) ? Int32.Parse(Request.Form["page"]) : 1;

            int rows = (Request.Form["rows"] != null) ? Int32.Parse(Request.Form["rows"]) : 10;

            string ldsuoshuxueyuan = customidentity.suoshuxueyuan(loingid);  //由登录人的id取出登录人的所属学院值，根据所属学院值过滤显示的信息。登录人员只能显示他所在部门的信息

            if (searchquery != "所有分管部门")
            {
                ldsuoshuxueyuan = searchquery;
            }
            var xm = from s in db.xiangmuguanlis
                     where (s.Xiangmumulu == xiangmu && ldsuoshuxueyuan.Contains(s.suoshuxueyuan) && ((s.bmfzrtijiao == "已提交" && s.fgldshenhe != "未通过") || (s.bmfzrtijiao == "未提交" && s.pingweishenhe == "撤回")))  //&& s.fgldshenhe == "通过"
                     orderby s.Xuhao
                     select new
                     {
                         ID = s.ID,
                         XmName = s.XmName,
                         Xuhao = s.Xuhao,
                         Chuangjianshijian = s.Chuangjianshijian,
                         Beizhu = s.Beizhu,
                         suoshuxueyuan = s.suoshuxueyuan,
                         Xiangmumulu = s.Xiangmumulu,
                         //tijiaozhuantai = s.tijiaozhuantai,
                         //bmfzrtijiao = s.bmfzrtijiao,
                         bmfzrshenhe = s.bmfzrshenhe,
                         //fgldshenhe = s.fgldshenhe,
                         ywzyshenhe = s.ywzyshenhe,
                         //username = s.username,
                         //bmfzryijian = s.bmfzryijian,
                         //pingweishenhe = s.pingweishenhe,
                         zongjine = s.zongjine
                     };
            try
            {
                // 返回到前台的值必须按照如下的格式包括 total and rows 
                var easyUIPages = new Dictionary<string, object>();
                easyUIPages.Add("total", xm.Count());
                easyUIPages.Add("rows", xm.ToPagedList(page, rows));
                return Json(easyUIPages, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                throw;
            }
        }


        //不审领导登录，看到全面的数据
        [HttpPost]
        [Authorize(Roles = "领导")]
        public JsonResult ld_bushen_xm(string xiangmu, string searchquery)//, string loingid
        {
            int page = (Request.Form["page"] != null) ? Int32.Parse(Request.Form["page"]) : 1;

            int rows = (Request.Form["rows"] != null) ? Int32.Parse(Request.Form["rows"]) : 10;

            if (searchquery == "所有部门")
            {
                var xm = from s in db.xiangmuguanlis
                         where (s.Xiangmumulu == xiangmu && s.ywzyshenhe == "通过")
                         orderby s.Xuhao
                         select new
                         {
                             ID = s.ID,
                             XmName = s.XmName,
                             Xuhao = s.Xuhao,
                             Chuangjianshijian = s.Chuangjianshijian,
                             Beizhu = s.Beizhu,
                             suoshuxueyuan = s.suoshuxueyuan,
                             Xiangmumulu = s.Xiangmumulu,
                             //tijiaozhuantai = s.tijiaozhuantai,
                             //bmfzrtijiao = s.bmfzrtijiao,
                             bmfzrshenhe = s.bmfzrshenhe,
                             //fgldshenhe = s.fgldshenhe,
                             ywzyshenhe = s.ywzyshenhe,
                             //username = s.username,
                             //bmfzryijian = s.bmfzryijian,
                             //pingweishenhe = s.pingweishenhe,
                             zongjine = s.zongjine
                         };
                try
                {
                    // 返回到前台的值必须按照如下的格式包括 total and rows 
                    var easyUIPages = new Dictionary<string, object>();
                    easyUIPages.Add("total", xm.Count());
                    easyUIPages.Add("rows", xm.ToPagedList(page, rows));
                    return Json(easyUIPages, JsonRequestBehavior.AllowGet);
                    //return Json(xm.ToList());
                }
                catch (Exception)
                {
                    throw;
                }
            }
            else
            {
                var xm = from s in db.xiangmuguanlis
                         where (s.Xiangmumulu == xiangmu && s.suoshuxueyuan == searchquery && s.ywzyshenhe == "通过")
                         orderby s.Xuhao
                         select new
                         {
                             ID = s.ID,
                             XmName = s.XmName,
                             Xuhao = s.Xuhao,
                             Chuangjianshijian = s.Chuangjianshijian,
                             Beizhu = s.Beizhu,
                             suoshuxueyuan = s.suoshuxueyuan,
                             Xiangmumulu = s.Xiangmumulu,
                             //tijiaozhuantai = s.tijiaozhuantai,
                             //bmfzrtijiao = s.bmfzrtijiao,
                             bmfzrshenhe = s.bmfzrshenhe,
                             //fgldshenhe = s.fgldshenhe,
                             ywzyshenhe = s.ywzyshenhe,
                             //username = s.username,
                             //bmfzryijian = s.bmfzryijian,
                             //pingweishenhe = s.pingweishenhe,
                             //zongjine = s.zongjine
                         };
                try
                {
                    // 返回到前台的值必须按照如下的格式包括 total and rows 
                    var easyUIPages = new Dictionary<string, object>();
                    easyUIPages.Add("total", xm.Count());
                    easyUIPages.Add("rows", xm.ToPagedList(page, rows));
                    return Json(easyUIPages, JsonRequestBehavior.AllowGet);
                    //return Json(xm.ToList());
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }


        //校领导登录后再页面看到的  datagrid数据  cwzg_getXiangmu
        [HttpPost]
        [Authorize(Roles = "领导")]
        public JsonResult ld_shenhe_xm_all(string dgtitle, string searchquery)
        {
            int page = (Request.Form["page"] != null) ? Int32.Parse(Request.Form["page"]) : 1;

            int rows = (Request.Form["rows"] != null) ? Int32.Parse(Request.Form["rows"]) : 10;

            if (searchquery == "所有部门")
            {
                var xm = from s in db.xiangmuguanlis
                         where (s.Xiangmumulu == dgtitle && s.ywzyshenhe == "通过")
                         orderby s.Xuhao
                         select new
                         {
                             ID = s.ID,
                             XmName = s.XmName,
                             Xuhao = s.Xuhao,
                             Chuangjianshijian = s.Chuangjianshijian,
                             Beizhu = s.Beizhu,
                             suoshuxueyuan = s.suoshuxueyuan,
                             Xiangmumulu = s.Xiangmumulu,
                             zongjine = s.zongjine
                         };
                try
                {
                    // 返回到前台的值必须按照如下的格式包括 total and rows 
                    var easyUIPages = new Dictionary<string, object>();
                    easyUIPages.Add("total", xm.Count());
                    easyUIPages.Add("rows", xm.ToPagedList(page, rows));
                    return Json(easyUIPages, JsonRequestBehavior.AllowGet);
                }
                catch (Exception)
                {
                    throw;
                }
            }
            else
            {
                var xm = from s in db.xiangmuguanlis
                         where (s.Xiangmumulu == dgtitle && s.suoshuxueyuan == searchquery && s.ywzyshenhe == "通过")
                         orderby s.Xuhao
                         select new
                         {
                             ID = s.ID,
                             XmName = s.XmName,
                             Xuhao = s.Xuhao,
                             Chuangjianshijian = s.Chuangjianshijian,
                             Beizhu = s.Beizhu,
                             suoshuxueyuan = s.suoshuxueyuan,
                             Xiangmumulu = s.Xiangmumulu
                         };
                try
                {
                    // 返回到前台的值必须按照如下的格式包括 total and rows 
                    var easyUIPages = new Dictionary<string, object>();
                    easyUIPages.Add("total", xm.Count());
                    easyUIPages.Add("rows", xm.ToPagedList(page, rows));
                    return Json(easyUIPages, JsonRequestBehavior.AllowGet);
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }


        //校领导登录后再页面看到的  datagrid数据  cwzg_getXiangmu
        [HttpPost]
        [Authorize(Roles = "领导")]
        public JsonResult ld_getXiangmu(string xiangmu)
        {
            int page = (Request.Form["page"] != null) ? Int32.Parse(Request.Form["page"]) : 1;

            int rows = (Request.Form["rows"] != null) ? Int32.Parse(Request.Form["rows"]) : 10;

            //为了区分读取出来的是哪一个年度的项目
            var mulu = db.xiangmuguanlis.Where(s => s.Xiangmumulu == xiangmu && s.ywzyshenhe == "通过");

            var xm = from s in mulu
                     orderby s.Xuhao
                     select s;
            try
            {
                // 返回到前台的值必须按照如下的格式包括 total and rows 
                //var easyUIPages = new Dictionary<string, object>();
                //easyUIPages.Add("total", xm.Count());
                //easyUIPages.Add("rows", xm.ToPagedList(page, rows));
                //return Json(easyUIPages,JsonRequestBehavior.AllowGet);
                return Json(xm.ToList());
            }
            catch (Exception)
            {
                throw;
            }
        }



        //排序，只有部门负责人才有排序权限
        /// <summary>
        /// 
        /// </summary>
        /// <param name="paixu">在前端批量勾选的项目中，由项目ID和分隔符，组成的字符串</param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "部门负责人")]
        public JsonResult paixu(string paixu)
        {
            string[] paixu1 = paixu.Split(',');
            var sdf = paixu1.Count();

            using (TransactionScope transaction = new TransactionScope())//原子操作，事物错误回滚
            {
                try
                {
                    for (int i = 0; i < sdf; i++)
                    {
                        int df = Convert.ToInt32(paixu1[i]);
                        var xm = db.xiangmuguanlis.Where(s => s.ID == df).FirstOrDefault();
                        xm.Xuhao = i;

                        db.Entry(xm).State = EntityState.Modified;
                    }
                    db.SaveChanges();
                    transaction.Complete();
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, errorMsg = ex.ToString() }, "text/html");
                }
                finally
                {
                    transaction.Dispose();
                }
            }
            return Json(new { success = true, message = "排序保存成功！" }, "text/html");
        }


        //员工查看
        public ActionResult chakan(string NoticeXmName, int xmid, string xmmulu)  //xmid为xiangmuguanli中的ID
        {
            ViewBag.xmmulu = xmmulu;

            ViewBag.name = NoticeXmName;
            ViewBag.jibenid = xmid;

            //编辑时放回的的参数
            //xmjibenxinxi jbxx = new xmjibenxinxi();
            xiangmuguanli xmgl = new xiangmuguanli();
            xmgl = db.xiangmuguanlis.Where(s => s.ID == xmid).FirstOrDefault();   //选择第一项或者符合条件的项
            ViewBag.xmgl_jbxx = xmgl.jpxx;                                       //把基本信息的值传给前台，再由前台判断其值

            ViewBag.xmgl_lxpj = xmgl.lxpj;
            ViewBag.xmgl_cccx = xmgl.cccx;
            return View();
        }


        //查看，基本信息tab
        public ActionResult chakan_jbxx(int xmid, string xmgl_xmname)
        {
            xmjibenxinxi xmjibenxinxi = db.xmjibenxinxis.Find(xmid);
            ViewBag.xmid = xmid;

            if (xmjibenxinxi == null)
            {
                ViewBag.xmname = xmgl_xmname;
                return View("chakan_jbxx_null");
            }
            return View(xmjibenxinxi);
        }


        //查看，项目绩效表tab
        public ActionResult chakan_xmjx(int xmid, string xmgl_cccx)
        {
            ViewBag.xmgl_cccx = xmgl_cccx;
            ViewBag.xmid = xmid;
            return View();
        }


        //查看，项目测算明细
        public ActionResult chakan_csmx(int xmid)
        {
            var xmcsmx = db.xmcsmxs.Where(s => s.xiangmuguanliID == xmid).OrderBy(s => s.suoshuniandu);
            return View(xmcsmx);
        }


        //查看，分年度预算表
        public ActionResult chakan_ndys(int xmid)
        {
            var xmgl = db.xiangmuguanlis.Where(s => s.ID == xmid).FirstOrDefault();

            ViewBag.qishi_year = Convert.ToInt32(xmgl.Chuangjianshijian.Substring(0, 4));

            var xmndys = db.xmnianduyusuans.Where(s => s.xiangmuguanliID == xmid).FirstOrDefault();
            if (xmndys == null)
            {
                return View("chakan_ndys_empty");
            }
            return View(xmndys);
        }


        //查看，项目立项评价
        public ActionResult chakan_lxpj(int xmid)
        {
            xmlxpj xmlxpj = db.xmlxpjs.Find(xmid);
            ViewBag.xmid = xmid;
            return View(xmlxpj);
        }

        //查看，项目采购配置
        public ActionResult chakan_cgpz(int xmid)
        {
            var xmcgpz = db.xmcgpzs.Where(s => s.xiangmuguanliID == xmid).OrderBy(s => s.suoshuniandu);

            return View(xmcgpz);
        }

        //查看，项目附件上传
        public ActionResult chakan_fjsc(int xmid)
        {
            ViewBag.xmid = xmid;
            return View();
        }


        //查看日志审核流程
        //这里缺少一个， 如果是评委是撤回的  状态  判断
        public ActionResult liucheng(int xmid, string loingid, string xmname)  //xmid为xiangmuguanli中的ID
        {

            ViewBag.xmid = xmid;

            ViewBag.xmname = xmname;
            xiangmuguanli xmgl = new xiangmuguanli();
            xmgl = db.xiangmuguanlis.Where(s => s.ID == xmid).FirstOrDefault();   //选择第一项或者符合条件的项

            //xmgl = db.xiangmuguanlis.Where(s => s.ID == xmid).OrderBy();   //选择第一项或者符合条件的项

            string xiangmu = xmgl.Xiangmumulu;
            string ywzyshenhe = xmgl.ywzyshenhe;
            string fgldshenhe = xmgl.fgldshenhe;
            string bmfzrshenhe = xmgl.bmfzrshenhe;
            string bmfzrtijiao = xmgl.bmfzrtijiao;
            string ygtijiao = xmgl.tijiaozhuantai;
            string pingweishenhe = xmgl.pingweishenhe;

            string fgldshenhefou = common.mulucustom.fgxzshenhefou(xiangmu);            //判断分管领导是否审核
            //string ldshenhefou = common.mulucustom.ldshenhefou(xiangmu);            //判断领导是否审核

            //因为评委，业务专员，领导和处长的进度条显示不一样，这里取出由loingid所判定的角色
            string role = common.customidentity.userrole(loingid);

            ViewBag.role = role;

            if (role == "评委" || role == "业务专员" || role == "财务主管" || role == "领导")
            {
                if (fgldshenhefou == "审核")
                {
                    if (ywzyshenhe == "通过" || ywzyshenhe == "未通过")
                    {
                        ViewBag.step = 9;   //9是结束
                    }
                    else
                    {
                        if (pingweishenhe == "通过" || pingweishenhe == "未通过")
                        {
                            ViewBag.step = 8;   //8是终审
                        }
                        else
                        {
                            if (pingweishenhe == "撤回")
                            {
                                ViewBag.step = 4;  //7是评审
                            }
                            else
                            {
                                if (fgldshenhe == "通过" && bmfzrtijiao == "已提交")
                                {
                                    ViewBag.step = 7;  //7是评审
                                }
                                else
                                {
                                    if (bmfzrtijiao == "已提交")
                                    {
                                        ViewBag.step = 6; //6是审定
                                    }
                                    else
                                    {
                                        if (bmfzrshenhe == "通过")
                                        {
                                            ViewBag.step = 5;//5是送审
                                        }
                                        else
                                        {
                                            if (ygtijiao == "已提交")
                                            {
                                                ViewBag.step = 4;
                                            }
                                            else
                                            {
                                                ViewBag.step = 2;
                                            }
                                        }
                                    }

                                }
                            }
                        }
                    }
                }
                else
                {
                    if (ywzyshenhe == "通过" || ywzyshenhe == "未通过")
                    {
                        ViewBag.step2 = 8;//8是结束
                    }
                    else
                    {
                        if (pingweishenhe == "通过" || pingweishenhe == "未通过")
                        {
                            ViewBag.step2 = 7;  //7是终审
                        }
                        else
                        {
                            if (pingweishenhe == "撤回")
                            {
                                ViewBag.step2 = 4;
                            }
                            else
                            {
                                if (bmfzrtijiao == "已提交")
                                {
                                    ViewBag.step2 = 6;   //6是评审
                                }
                                else
                                {
                                    if (bmfzrshenhe == "通过")
                                    {
                                        ViewBag.step2 = 5;
                                    }
                                    else
                                    {
                                        if (ygtijiao == "已提交")
                                        {
                                            ViewBag.step2 = 4;
                                        }
                                        else
                                        {
                                            ViewBag.step2 = 2;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else           //如果角色不是评委，业务专员，领导，财务主管，  这里看不到  评委审核的 节点
            {
                if (fgldshenhefou == "审核")
                {
                    if (ywzyshenhe == "通过" || ywzyshenhe == "未通过")
                    {
                        ViewBag.step3 = 8;    //8是结束
                    }
                    else
                    {

                        if (fgldshenhe == "通过" && bmfzrtijiao == "已提交")
                        {
                            ViewBag.step3 = 7;  // 7是终审
                        }
                        else
                        {
                            if (bmfzrtijiao == "已提交")
                            {
                                ViewBag.step3 = 6;   //6是审定
                            }
                            else
                            {
                                if (bmfzrshenhe == "通过")   //
                                {
                                    ViewBag.step3 = 4;
                                }
                                else
                                {
                                    if (ygtijiao == "已提交")
                                    {
                                        ViewBag.step3 = 3;
                                    }
                                    else
                                    {
                                        ViewBag.step3 = 2;
                                    }
                                }
                            }
                        }
                    }
                }
                else   //如果角色不是评委，业务专员，领导，财务主管，并且目录不是领导角色参与审核的
                {
                    if (ywzyshenhe == "通过" || ywzyshenhe == "未通过")
                    {
                        ViewBag.step4 = 7;
                    }
                    else
                    {
                        if (bmfzrtijiao == "已提交")
                        {
                            ViewBag.step4 = 6;
                        }
                        else
                        {
                            if (bmfzrshenhe == "通过")
                            {
                                ViewBag.step4 = 5;
                            }
                            else
                            {
                                if (ygtijiao == "已提交")
                                {
                                    ViewBag.step4 = 4;
                                }
                                else
                                {
                                    ViewBag.step4 = 2;
                                }
                            }
                        }
                    }
                }
            }
            ViewBag.fgldshenhefou = fgldshenhefou;
            return View();
        }


        //get_xmliucheng
        [HttpPost]
        public JsonResult get_xmliucheng(int xmid)
        {
            int page = (Request.Form["page"] != null) ? Int32.Parse(Request.Form["page"]) : 1;

            int rows = (Request.Form["rows"] != null) ? Int32.Parse(Request.Form["rows"]) : 10;

            //为了区分读取出来的是哪一个年度的项目
            //var mulu = db.xiangmuguanlis.Where(s => s.Xiangmumulu == xiangmu);
            var mulu = db.xmrizhis.Where(s => s.xiangmuguanliID == xmid).OrderByDescending(s => s.shenheshijian);

            var xm = from s in mulu
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


        //部门负责人项目审核到页面               ，，，，，，，这个好像是无效的方法
        //[HttpPost]
        public ActionResult shenhe(string NoticeXmName, int xmid, string xmmulu)  //xmid为xiangmuguanli中的ID
        {
            ViewBag.xmmulu = xmmulu;

            ViewBag.name = NoticeXmName;
            ViewBag.jibenid = xmid;

            //编辑时放回的的参数
            //xmjibenxinxi jbxx = new xmjibenxinxi();
            xiangmuguanli xmgl = new xiangmuguanli();
            xmgl = db.xiangmuguanlis.Where(s => s.ID == xmid).FirstOrDefault();   //选择第一项或者符合条件的项
            ViewBag.xmgl_jbxx = xmgl.jpxx;                                        //把基本信息的值传给前台，再由前台判断其值

            ViewBag.xmgl_lxpj = xmgl.lxpj;

            return View();
        }



        //部门负责人审核提交按钮执行方法
        [HttpPost]
        [Authorize(Roles = "部门负责人")]
        public JsonResult bmfzr_shenhe(string name, int xmid, string loingid)//FormCollection form,
        {
            xiangmuguanli xmgl = new xiangmuguanli();
            xmgl = db.xiangmuguanlis.Where(s => s.ID == xmid).FirstOrDefault();

            //部门负责人审核意见
            var bmfzryijian = Request.Form["bmfzryijian"];
            xmgl.bmfzryijian = bmfzryijian;

            var bmfzrshenhe = Request.Form["shenhe"];
            xmgl.bmfzrshenhe = bmfzrshenhe;                    //判断部门负责人的审核结果，若是通过，则直接赋值并保存


            //创建项目时添加项目日志内容
            string rizhiname = customidentity.rizhiusername(loingid);
            string rizhirole = customidentity.rizhiuserrole(loingid);

            xmrizhi xmrizhi = new xmrizhi();
            xmrizhi.xiangmuguanliID = xmid;
            xmrizhi.shenheshijian = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
            xmrizhi.shenhejiedian = "部门负责人审核";
            xmrizhi.shenheren = rizhiname;
            xmrizhi.shenhejuese = rizhirole;
            xmrizhi.shenhejieguo = bmfzrshenhe;
            xmrizhi.shenheyijian = bmfzryijian;

            var fgldshenhe = xmgl.fgldshenhe;
            var ywzyshenhe = xmgl.ywzyshenhe;
            var pingweishenhe = xmgl.pingweishenhe;
            var bmfzrtj = xmgl.bmfzrtijiao;

            string username = xmgl.username;   //取到创建这条记录者的username，并判断是部门负责人还是人员
            string shifoubmfzr = customidentity.userrole_name(username);//根据创建项目时的username判断是否是部门负责人创建

            if (bmfzrshenhe == null)
            {
                return Json(new { success = false, errorMsg = "请选择审核状态后在确认！" }, "text/html");
            }
            if (shifoubmfzr == "部门负责人")//如果创建项目的部门负责人，则审核为通过
            {
                xmgl.bmfzrshenhe = "通过";
            }
            else           //如果该条记录是由员工创建的就执行下面语句
            {
                xmgl.bmfzrshenhe = bmfzrshenhe;

                if (pingweishenhe == "撤回" && bmfzrshenhe == "未通过")  //这里有点想不明白为什么这样子
                {
                    xmgl.tijiaozhuantai = "未提交";

                    xmgl.ywzyshenhe = "未审核";
                    xmgl.pingweishenhe = "未审核";
                }

                if (bmfzrshenhe == "未通过" && fgldshenhe == "未审核" && ywzyshenhe == "未审核") //部门负责人第一次审核
                {
                    xmgl.tijiaozhuantai = "未提交";
                }

                //这里是第一次bmfzr通过审核，fgld也通过审核，到ywzy不通过，保留fgld审核结果，bmfzr再审不通过
                if (bmfzrshenhe == "未通过" && fgldshenhe == "通过" && pingweishenhe == "撤回")
                {
                    xmgl.tijiaozhuantai = "未提交";
                }

                //这里是分管领导不参与审核,由员工建立项目，部门负责人审核先通过，业务专员审核不通过转交部门负责人，部门负责人发现问题审核不通过交友员工
                if (fgldshenhe == "未审核" && pingweishenhe == "撤回" && bmfzrshenhe == "未通过")
                {
                    xmgl.tijiaozhuantai = "未提交";
                }
            }
            using (TransactionScope transaction = new TransactionScope())//原子操作，事物错误回滚
            {
                try
                {
                    db.Entry(xmgl).State = EntityState.Modified;                   //更新项目管理表中表示基本信息表的字段，这种更新方式不知道对不对
                    db.xmrizhis.Add(xmrizhi);
                    db.SaveChanges();
                    transaction.Complete();
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, errorMsg = ex.ToString() }, "text/html");
                }
                finally
                {
                    transaction.Dispose();
                }
            }
            return Json(new { success = true, errorMsg = "部门负责人审核成功！" }, "text/html");
        }


        //分管领导审核
        [HttpPost]
        [Authorize(Roles = "分管领导")]
        public JsonResult fgld_shenhe(string name, int xmid, string loingid)//FormCollection form,
        {
            xiangmuguanli xmgl = new xiangmuguanli();
            xmgl = db.xiangmuguanlis.Where(s => s.ID == xmid).FirstOrDefault();

            var fgldshenhe = Request.Form["shenhe"];
            xmgl.fgldshenhe = fgldshenhe;                    //分管领导工作只是审核

            //部门负责人审核意见
            var fgldyijian = Request.Form["fgldyijian"];
            xmgl.fgldyijian = fgldyijian;

            //创建项目时添加项目日志内容
            string rizhiname = customidentity.rizhiusername(loingid);
            string rizhirole = customidentity.rizhiuserrole(loingid);

            xmrizhi xmrizhi = new xmrizhi();
            xmrizhi.xiangmuguanliID = xmid;
            xmrizhi.shenheshijian = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
            xmrizhi.shenhejiedian = "分管领导审核";
            xmrizhi.shenheren = rizhiname;
            xmrizhi.shenhejuese = rizhirole;
            xmrizhi.shenhejieguo = fgldshenhe;
            xmrizhi.shenheyijian = fgldyijian;

            if (fgldshenhe == null)
            {
                return Json(new { success = false, errorMsg = "请选择审核状态后在提交！" }, "text/html");
            }
            if (fgldshenhe == "未通过")//如果分管领导审核结果是未通过的，要把部门负责人的提交状态改为未提交，部门负责人的审核状态不改
            {
                xmgl.bmfzrtijiao = "未提交";
            }
            using (TransactionScope transaction = new TransactionScope())//原子操作，事物错误回滚
            {
                try
                {
                    db.Entry(xmgl).State = EntityState.Modified;                   //更新项目管理表中表示基本信息表的字段，这种更新方式不知道对不对

                    db.xmrizhis.Add(xmrizhi);
                    db.SaveChanges();
                    transaction.Complete();
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, errorMsg = ex.ToString() }, "text/html");
                }
                finally
                {
                    transaction.Dispose();
                }
            }
            return Json(new { success = true, errorMsg = "分管领导审核提交保存成功！" }, "text/html");
        }


        //领导审核
        [HttpPost]
        [Authorize(Roles = "领导")]
        public JsonResult ld_shenhe(string name, int xmid, string loingid)//FormCollection form,
        {
            xiangmuguanli xmgl = new xiangmuguanli();
            xmgl = db.xiangmuguanlis.Where(s => s.ID == xmid).FirstOrDefault();

            var ldshenhe = Request.Form["shenhe"];
            xmgl.fgldshenhe = ldshenhe;

            //部门负责人审核意见
            var ldyijian = Request.Form["ldyijian"];
            //xmgl.ldyijian = ldyijian;
            xmgl.fgldyijian = ldyijian;

            //创建项目时添加项目日志内容
            string rizhiname = customidentity.rizhiusername(loingid);
            string rizhirole = customidentity.rizhiuserrole(loingid);

            xmrizhi xmrizhi = new xmrizhi();
            xmrizhi.xiangmuguanliID = xmid;
            xmrizhi.shenheshijian = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
            xmrizhi.shenhejiedian = "分管领导审核";
            xmrizhi.shenheren = rizhiname;
            xmrizhi.shenhejuese = rizhirole;
            xmrizhi.shenhejieguo = ldshenhe;
            xmrizhi.shenheyijian = ldyijian;

            if (ldshenhe == null)
            {
                return Json(new { success = false, errorMsg = "请选择审核状态后在提交！" }, "text/html");
            }
            if (ldshenhe == "未通过")//如果分管领导审核结果是未通过的，要把部门负责人的提交状态改为未提交，部门负责人的审核状态不改
            {
                xmgl.bmfzrtijiao = "未提交";
            }
            using (TransactionScope transaction = new TransactionScope())//原子操作，事物错误回滚
            {
                try
                {
                    db.Entry(xmgl).State = EntityState.Modified;                   //更新项目管理表中表示基本信息表的字段，这种更新方式不知道对不对

                    db.xmrizhis.Add(xmrizhi);
                    db.SaveChanges();
                    transaction.Complete();
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, errorMsg = ex.ToString() }, "text/html");
                }
                finally
                {
                    transaction.Dispose();
                }
            }
            return Json(new { success = true, errorMsg = "领导审核提交保存成功！" }, "text/html");
        }


        //评委审核
        [HttpPost]
        [Authorize(Roles = "评委")]
        public JsonResult pw_shenhe(string name, int xmid, string loingid)//FormCollection form,
        {
            xiangmuguanli xmgl = new xiangmuguanli();
            xmgl = db.xiangmuguanlis.Where(s => s.ID == xmid).FirstOrDefault();

            var pwshenhe = Request.Form["shenhe"];
            //xmgl.ywzyshenhe = ywzyshenhe;                    //分管领导工作只是审核
            xmgl.pingweishenhe = pwshenhe;

            //部门负责人审核意见
            var pwyijian = Request.Form["pwyijian"];
            //xmgl.ywzyyijian = ywzyyijian;
            xmgl.pingweiyijian = pwyijian;

            //创建项目时添加项目日志内容
            string rizhiname = customidentity.rizhiusername(loingid);
            string rizhirole = customidentity.rizhiuserrole(loingid);

            if (pwshenhe == null)
            {
                return Json(new { success = false, errorMsg = "请选择审核状态后在提交！" }, "text/html");
            }

            using (TransactionScope transaction = new TransactionScope())//原子操作，事物错误回滚
            {
                try
                {
                    db.Entry(xmgl).State = EntityState.Modified;                   //更新项目管理表中表示基本信息表的字段，这种更新方式不知道对不对
                    //db.xmrizhis.Add(xmrizhi);
                    db.SaveChanges();

                    transaction.Complete();
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, errorMsg = ex.ToString() }, "text/html");
                }
                finally
                {
                    transaction.Dispose();
                }
            }
            return Json(new { success = true, errorMsg = "审核保存成功！" }, "text/html");
        }


        //评委批量审核
        [HttpPost]
        [Authorize(Roles = "评委")]
        public JsonResult plshenhe_pw(string xmid, string loingid, string username)//FormCollection form,
        {
            string[] xmid_fenli = xmid.Split(',');
            var xmid_fenli_conut = xmid_fenli.Count();

            var pwshenhe = Request.Form["shenhe"];

            //业务专员批量审核意见
            var pwyijian = Request.Form["pingweiyijian"];

            string rizhirole = customidentity.rizhiuserrole(loingid);
            if (pwshenhe != null)
            {
                using (TransactionScope transaction = new TransactionScope())//原子操作，事物错误回滚
                {
                    try
                    {
                        for (int i = 0; i < xmid_fenli_conut; i++)
                        {
                            int id = Convert.ToInt32(xmid_fenli[i]);
                            var xm = db.xiangmuguanlis.Where(s => s.ID == id).FirstOrDefault();

                            xm.pingweishenhe = pwshenhe;

                            xm.ywzyyijian = pwyijian;    //业务专员批量审核 意见

                            db.Entry(xm).State = EntityState.Modified;
                        }
                        db.SaveChanges();                     
                        transaction.Complete();
                    }
                    catch (Exception ex)
                    {
                        //return Json(new { errorMsg = ex.ToString() }, "text/html");
                        return Json(new { success = 2, errorMsg = ex.ToString() }, "text/html");
                    }
                    finally
                    {
                        transaction.Dispose();
                    }
                }
                return Json(new { success = 1, errorMsg = "审核保存成功！" }, "text/html");
            }
            else
            {
                return Json(new { success = 2, errorMsg = "请选择审核状态后在提交！" }, "text/html");
            }
        }


        //评委批量提交
        [HttpPost]
        [Authorize(Roles = "评委")]
        public JsonResult pltijiao_pw(string xmid, string loingid, string username)//FormCollection form,
        {
            string[] xmid_fenli = xmid.Split(',');
            var xmid_fenli_conut = xmid_fenli.Count();

            string rizhirole = customidentity.rizhiuserrole(loingid);

            using (TransactionScope transaction = new TransactionScope())//原子操作，事物错误回滚
            {
                try
                {
                    for (int i = 0; i < xmid_fenli_conut; i++)
                    {
                        int id = Convert.ToInt32(xmid_fenli[i]);
                        var xm = db.xiangmuguanlis.Where(s => s.ID == id).FirstOrDefault();
                        var pingweishenhe = xm.pingweishenhe;
                        var pingweiyijian = xm.pingweiyijian;
                        xm.pingweitijiao = "已提交";

                        xmrizhi xmrizhi = new xmrizhi();

                        xmrizhi.xiangmuguanliID = id;
                        xmrizhi.shenheshijian = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
                        xmrizhi.shenhejiedian = "评委提交";
                        xmrizhi.shenheren = username;
                        xmrizhi.shenhejuese = rizhirole;
                        xmrizhi.shenhejieguo = pingweishenhe;
                        xmrizhi.shenheyijian = pingweiyijian;   //评委的重复评审不记入日志，但是评委的提交记入日志

                        db.xmrizhis.Add(xmrizhi);
                        db.Entry(xm).State = EntityState.Modified;

                    }
                    db.SaveChanges();

                    transaction.Complete();
                }
                catch (Exception ex)
                {
                    //return Json(new { errorMsg = ex.ToString() }, "text/html");
                    return Json(new { success = 2, errorMsg = ex.ToString() }, "text/html");
                }
                finally
                {
                    transaction.Dispose();
                }
            }
            return Json(new { success = 1, errorMsg = "审核保存成功！" }, "text/html");            
        }


        //评委单个提交 pwtijiao
        [HttpPost]
        [Authorize(Roles = "评委")]
        public JsonResult pwtijiao(int xmid, string loingid, string username)//FormCollection form,
        {
            xmrizhi xmrizhi = new xmrizhi();

            string rizhirole = customidentity.rizhiuserrole(loingid);

            var xm = db.xiangmuguanlis.Where(s => s.ID == xmid).FirstOrDefault();

            var pingweishenhe = xm.pingweishenhe;
            var pingweiyijian = xm.pingweiyijian;

            //评委的审核 和提交 只记录 提交日志，而且在提交日志中，不在体现评委的审核结果
            xm.pingweitijiao = "已提交";
            xmrizhi.xiangmuguanliID = xmid;
            xmrizhi.shenheshijian = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
            xmrizhi.shenhejiedian = "评委提交";
            xmrizhi.shenheren = username;
            xmrizhi.shenhejuese = rizhirole;
            xmrizhi.shenheyijian = pingweiyijian;   //评委的重复评审不记入日志，但是评委的提交记入日志

            using (TransactionScope transaction = new TransactionScope())//原子操作，事物错误回滚
            {
                try
                {
                    db.xmrizhis.Add(xmrizhi);
                    db.Entry(xm).State = EntityState.Modified;

                    db.SaveChanges();
                    transaction.Complete();
                }
                catch (Exception ex)
                {
                    return Json(new { Succeeded = 2, errorMsg = ex.ToString() }, "text/html");
                }
                finally
                {
                    transaction.Dispose();
                }
            }
            return Json(new { Succeeded = 1, errorMsg = "评委提交保存成功！" }, "text/html");
        }


        //评委撤回
        [HttpPost]
        [Authorize(Roles = "评委")]
        public JsonResult pw_chehui(string name, int xmid, string loingid)//FormCollection form, 
        {
            xiangmuguanli xmgl = new xiangmuguanli();
            xmgl = db.xiangmuguanlis.Where(s => s.ID == xmid).FirstOrDefault();

            //再审处理
            xmgl.bmfzrtijiao = "未提交";
            xmgl.pingweishenhe = "撤回";

            //部门负责人撤回意见
            var pingweiyijian = Request.Form["pingweiyijian"];
            xmgl.pingweiyijian = pingweiyijian;

            //创建项目时添加项目日志内容
            string rizhiname = customidentity.rizhiusername(loingid);
            string rizhirole = customidentity.rizhiuserrole(loingid);

            xmrizhi xmrizhi = new xmrizhi();
            xmrizhi.xiangmuguanliID = xmid;
            xmrizhi.shenheshijian = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
            xmrizhi.shenhejiedian = "评委撤回";
            xmrizhi.shenheren = rizhiname;
            xmrizhi.shenhejuese = rizhirole;
            xmrizhi.shenhejieguo = "评委撤回";
            xmrizhi.shenheyijian = pingweiyijian;

            using (TransactionScope transaction = new TransactionScope())//原子操作，事物错误回滚
            {
                try
                {
                    db.Entry(xmgl).State = EntityState.Modified;                   //更新项目管理表中表示基本信息表的字段，这种更新方式不知道对不对
                    db.xmrizhis.Add(xmrizhi);
                    db.SaveChanges();
                    transaction.Complete();

                }
                catch (Exception ex)
                {
                    return Json(new { success = false, errorMsg = ex.ToString() }, "text/html");
                }
                finally
                {
                    transaction.Dispose();
                }
            }
            return Json(new { success = true, errorMsg = "评委撤回保存成功！" }, "text/html");
        }


        //财务主管仲裁cwzg_zc
        [HttpPost]
        [Authorize(Roles = "财务主管")]
        public JsonResult cwzg_zc(string loingid, int id)//FormCollection form, 
        {
            xiangmuguanli xmgl = new xiangmuguanli();
            xmgl = db.xiangmuguanlis.Where(s => s.ID == id).FirstOrDefault();

            xmgl.ywzyshenhe = "未审核";

            //创建项目时添加项目日志内容
            string rizhiname = customidentity.rizhiusername(loingid);
            string rizhirole = customidentity.rizhiuserrole(loingid);
            using (TransactionScope transaction = new TransactionScope())//原子操作，事物错误回滚
            {
                try
                {
                    db.Entry(xmgl).State = EntityState.Modified;                   //更新项目管理表中表示基本信息表的字段，这种更新方式不知道对不对
                    db.SaveChanges();

                    transaction.Complete();
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, errorMsg = ex.ToString() }, "text/html");
                }
                finally
                {
                    transaction.Dispose();
                }
            }
            return Json(new { success = true, errorMsg = "财务主管仲裁保存成功！" }, "text/html");
        }



        //业务部门终审
        [HttpPost]
        [Authorize(Roles = "业务专员")]
        public JsonResult ywzy_zhongshen(string name, int xmid, string loingid)//FormCollection form, 
        {
            xiangmuguanli xmgl = new xiangmuguanli();
            xmgl = db.xiangmuguanlis.Where(s => s.ID == xmid).FirstOrDefault();

            var ywzyshenhe = Request.Form["shenhe"];
            xmgl.ywzyshenhe = ywzyshenhe;                    //分管领导工作只是审核

            //部门负责人撤回意见
            var ywzyyijian = Request.Form["ywzyyijian"];
            xmgl.ywzyyijian = ywzyyijian;

            //创建项目时添加项目日志内容
            string rizhiname = customidentity.rizhiusername(loingid);
            string rizhirole = customidentity.rizhiuserrole(loingid);

            xmrizhi xmrizhi = new xmrizhi();
            xmrizhi.xiangmuguanliID = xmid;
            xmrizhi.shenheshijian = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
            xmrizhi.shenhejiedian = "终审";
            xmrizhi.shenheren = rizhiname;
            xmrizhi.shenhejuese = rizhirole;
            xmrizhi.shenhejieguo = ywzyshenhe;
            xmrizhi.shenheyijian = ywzyyijian;

            if (ywzyshenhe == null)
            {
                return Json(new { success = false, errorMsg = "请选择审核状态后在提交！" }, "text/html");
            }

            using (TransactionScope transaction = new TransactionScope())//原子操作，事物错误回滚
            {
                try
                {
                    db.Entry(xmgl).State = EntityState.Modified;                   //更新项目管理表中表示基本信息表的字段，这种更新方式不知道对不对
                    db.xmrizhis.Add(xmrizhi);
                    db.SaveChanges();
                    transaction.Complete();
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, errorMsg = ex.ToString() }, "text/html");
                }
                finally
                {
                    transaction.Dispose();
                }
            }
            return Json(new { success = true, errorMsg = "业务部门审核保存成功！" }, "text/html");
        }


        //业务专员批量审核
        [HttpPost]
        [Authorize(Roles = "业务专员")]
        public JsonResult plshenhe_ywzy(string xmid, string loingid, string username)//FormCollection form,
        {
            string[] xmid_fenli = xmid.Split(',');
            var xmid_fenli_conut = xmid_fenli.Count();
            var ywzy_shenhe = Request.Form["shenhe"];

            //业务专员批量审核意见
            var ywzy_yijian = Request.Form["ywzyyijian"];

            string rizhirole = customidentity.rizhiuserrole(loingid);
            if (ywzy_shenhe != null)
            {
                using (TransactionScope transaction = new TransactionScope())//原子操作，事物错误回滚
                {
                    try
                    {
                        for (int i = 0; i < xmid_fenli_conut; i++)
                        {
                            int id = Convert.ToInt32(xmid_fenli[i]);
                            var xm = db.xiangmuguanlis.Where(s => s.ID == id).FirstOrDefault();
                            //xm.Xuhao = i;
                            xm.ywzyshenhe = ywzy_shenhe;
                            xm.ywzyyijian = ywzy_yijian;    //业务专员批量审核 意见

                            xmrizhi xmrizhi = new xmrizhi();
                            xmrizhi.xiangmuguanliID = id;
                            xmrizhi.shenheshijian = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
                            xmrizhi.shenhejiedian = "业务专员审核";
                            xmrizhi.shenheren = username;
                            xmrizhi.shenhejuese = rizhirole;
                            xmrizhi.shenhejieguo = ywzy_shenhe;
                            xmrizhi.shenheyijian = ywzy_yijian;

                            db.xmrizhis.Add(xmrizhi);
                            db.Entry(xm).State = EntityState.Modified;

                        }
                        db.SaveChanges();
                        transaction.Complete();
                    }
                    catch (Exception ex)
                    {
                        //return Json(new { errorMsg = ex.ToString() }, "text/html");
                        return Json(new { success = 2, errorMsg = ex.ToString() }, "text/html");
                    }
                    finally
                    {
                        transaction.Dispose();
                    }
                }
                return Json(new { success = 1, errorMsg = "审核保存成功！" }, "text/html");
            }
            else
            {
                return Json(new { success = 2, errorMsg = "请选择审核状态后在提交！" }, "text/html");
            }
        }


        //分管领导批量审核    
        [HttpPost]
        [Authorize(Roles = "分管领导")]
        public JsonResult plshenhe_fgld(string xmid, string loingid, string username)//FormCollection form,
        {
            string[] xmid_fenli = xmid.Split(',');
            var xmid_fenli_conut = xmid_fenli.Count();
            var fgld_shenhe = Request.Form["shenhe"];

            //分管领导批量审核意见
            var fgld_yijina = Request.Form["fgldyijian"];

            if (fgld_shenhe == null)
            {
                return Json(new { success = false, errorMsg = "请选择审核状态后在提交！" }, "text/html");
            }

            string rizhirole = customidentity.rizhiuserrole(loingid);
            if (fgld_shenhe != null)
            {
                using (TransactionScope transaction = new TransactionScope())//原子操作，事物错误回滚
                {
                    try
                    {
                        for (int i = 0; i < xmid_fenli_conut; i++)
                        {
                            int id = Convert.ToInt32(xmid_fenli[i]);
                            var xm = db.xiangmuguanlis.Where(s => s.ID == id).FirstOrDefault();
                            xm.fgldshenhe = fgld_shenhe;
                            xm.fgldyijian = fgld_yijina;

                            if (fgld_shenhe == "未通过")
                            {
                                xm.bmfzrtijiao = "未提交";
                            }

                            xmrizhi xmrizhi = new xmrizhi();
                            xmrizhi.xiangmuguanliID = id;
                            xmrizhi.shenheshijian = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
                            xmrizhi.shenhejiedian = "分管领导审核";
                            xmrizhi.shenheren = username;
                            xmrizhi.shenhejuese = rizhirole;
                            xmrizhi.shenhejieguo = fgld_shenhe;
                            xmrizhi.shenheyijian = fgld_yijina;

                            db.xmrizhis.Add(xmrizhi);
                            db.Entry(xm).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                        transaction.Complete();
                    }
                    catch (Exception ex)
                    {
                        return Json(new { success = 2, errorMsg = ex.ToString() }, "text/html");
                    }
                    finally
                    {
                        transaction.Dispose();
                    }
                }
                return Json(new { success = 1, errorMsg = "审核保存成功！" }, "text/html");
            }
            else
            {
                return Json(new { success = 2, errorMsg = "请选择审核状态后在提交！" }, "text/html");
            }
        }


        //领导批量审核
        [HttpPost]
        [Authorize(Roles = "领导")]      // 这里的意思是，领导在实际中也会分管几个学院，所以领导实际上也会作为一个分管领导 进行审核
        public JsonResult plshenhe_ld(string xmid, string loingid, string username)//FormCollection form,
        {
            string[] xmid_fenli = xmid.Split(',');
            var xmid_fenli_conut = xmid_fenli.Count();

            var ld_shenhe = Request.Form["shenhe"];

            //业务专员批量审核意见
            var ld_yijian = Request.Form["ldyijian"];

            if (ld_shenhe == null)
            {
                return Json(new { success = false, errorMsg = "请选择审核状态后在提交！" }, "text/html");
            }

            string rizhirole = customidentity.rizhiuserrole(loingid);
            if (ld_shenhe != null)
            {
                using (TransactionScope transaction = new TransactionScope())//原子操作，事物错误回滚
                {
                    try
                    {
                        for (int i = 0; i < xmid_fenli_conut; i++)
                        {
                            int id = Convert.ToInt32(xmid_fenli[i]);
                            var xm = db.xiangmuguanlis.Where(s => s.ID == id).FirstOrDefault();
                            xm.fgldshenhe = ld_shenhe;

                            if (ld_shenhe == "未通过")
                            {
                                xm.bmfzrtijiao = "未提交";
                            }
                            xm.fgldyijian = ld_yijian;

                            xmrizhi xmrizhi = new xmrizhi();
                            xmrizhi.xiangmuguanliID = id;
                            xmrizhi.shenheshijian = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
                            xmrizhi.shenhejiedian = "业务专员审核";
                            xmrizhi.shenheren = username;
                            xmrizhi.shenhejuese = rizhirole;
                            xmrizhi.shenhejieguo = ld_shenhe;
                            xmrizhi.shenheyijian = ld_yijian;

                            db.xmrizhis.Add(xmrizhi);
                            db.Entry(xm).State = EntityState.Modified;
                        }
                        db.SaveChanges();
                        transaction.Complete();
                    }
                    catch (Exception ex)
                    {
                        //return Json(new { errorMsg = ex.ToString() }, "text/html");
                        return Json(new { success = 2, errorMsg = ex.ToString() }, "text/html");
                    }
                    finally
                    {
                        transaction.Dispose();
                    }
                }
                return Json(new { success = 1, errorMsg = "审核保存成功！" }, "text/html");
            }
            else
            {
                return Json(new { success = 2, errorMsg = "请选择审核状态后在提交！" }, "text/html");
            }
        }


        //员工提交项目
        [HttpPost]
        [Authorize(Roles = "员工")]
        public JsonResult tijiao(string loingid, string shijian, int xmid)//FormCollection form ,
        {
            xiangmuguanli xmgl = new xiangmuguanli();
            xmgl = db.xiangmuguanlis.Where(s => s.ID == xmid).FirstOrDefault();

            string rizhiname = customidentity.rizhiusername(loingid);
            string rizhirole = customidentity.rizhiuserrole(loingid);

            //创建项目时添加项目日志内容
            xmrizhi xmrizhi = new xmrizhi();
            xmrizhi.xiangmuguanliID = xmid;
            xmrizhi.shenheshijian = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
            xmrizhi.shenhejiedian = "送审";
            xmrizhi.shenheren = rizhiname;
            xmrizhi.shenhejuese = rizhirole;
            xmrizhi.shenhejieguo = "送审";

            //在提交之前判断其他的几个附属表是否已经完善
            bool shifoutwanshan = mulucustom.shifouwanshan(xmid);

            var bmfzrshenhe = xmgl.bmfzrshenhe;    //取出该记录的部门负责人审核状态，若为未通过则在重新提交时，需要将未通过改为未审核状态，以便重新审核
            var bmfzrtj = xmgl.bmfzrtijiao;
            var fgld = xmgl.fgldshenhe;
            var ywzyshenhe = xmgl.ywzyshenhe;
            string username = xmgl.username;//记录创建者名字
            var pingweishenhe = xmgl.pingweishenhe;

            if (shifoutwanshan == true)
            {
                string shifoubmfzr = customidentity.userrole_name(username);//判断该记录是否由 角色是 部门负责人 的成员创建，这里还未判断 登录者 提交 的人是哪一个角色
                //if (customidentity.userrole(loingid) == "部门负责人")  //如果是由部门负责人创建并提交的，则在提交时，部门负责人的审核状态为通过

                //这里感觉多余 if的判断
                if (shifoubmfzr == "部门负责人")  //如果是由部门负责人创建并提交的，则在提交时，部门负责人的审核状态为通过
                {
                    //xmgl.tijiaozhuantai = "已提交";
                    xmgl.bmfzrtijiao = "已提交";
                    xmgl.bmfzrshenhe = "通过";
                }
                else
                {
                    xmgl.tijiaozhuantai = "已提交";
                    if (bmfzrshenhe == "未通过")  //这里的未通过包括部门负责人和分管领导未通过
                    {
                        //这里的判断  感觉也是多余
                        if (xmgl.fgldshenhe == "通过")
                        {
                            xmgl.bmfzrshenhe = "未审核";
                            xmgl.fgldshenhe = "通过";
                            xmgl.ywzyshenhe = "未审核";
                            xmgl.bmfzrtijiao = "未提交";
                            //xmgl.fgldtijiao = "未提交";
                            xmgl.pingweishenhe = "未审核";
                            xmgl.pingweitijiao = "未提交";
                        }
                        else
                        {
                            xmgl.bmfzrshenhe = "未审核";
                            xmgl.fgldshenhe = "未审核";
                            xmgl.ywzyshenhe = "未审核";
                            xmgl.bmfzrtijiao = "未提交";
                            //xmgl.fgldtijiao = "未提交";
                            xmgl.pingweishenhe = "未审核";
                            xmgl.pingweitijiao = "未提交";
                        }
                    }
                }
                using (TransactionScope transaction = new TransactionScope())//原子操作，事物错误回滚
                {
                    try
                    {
                        db.Entry(xmgl).State = EntityState.Modified;                   //更新项目管理表中表示基本信息表的字段，这种更新方式不知道对不对
                        db.xmrizhis.Add(xmrizhi);
                        db.SaveChanges();
                        transaction.Complete();
                    }
                    catch (Exception ex)
                    {
                        return Json(new { errorMsg = ex.ToString() }, "text/html");
                    }
                    finally
                    {
                        transaction.Dispose();
                    }
                }
                return Json(new { Succeeded = "提交成功，OK！" }, "text/html");
            }
            else
            {
                string jpxx = "0";
                string ccjx = "0";
                string xljx = "0";
                string csmx = "0";
                string lxpj = "0";
                string cgpz = "0";
                string cccx = "0";
                string ndys = "0";
                using (cwcxmkContent db1 = new cwcxmkContent()) //创建一个新的上下文
                {
                    var xiangmu = db1.xiangmuguanlis.Where(s => s.ID == xmid).FirstOrDefault();
                    jpxx = xiangmu.jpxx;
                    ccjx = xiangmu.ccjx;
                    xljx = xiangmu.xljx;
                    csmx = xiangmu.csmx;
                    lxpj = xiangmu.lxpj;
                    cgpz = xiangmu.cgpz;
                    cccx = xiangmu.cccx;
                    ndys = xiangmu.nianduyusuan;
                }

                if (jpxx == "0")
                {
                    return Json(new { errorMsg = "《基本信息表》 没有完善，请完善后再次提交！" }, "text/html");
                }
                if (cccx == "0")
                {
                    return Json(new { errorMsg = "《项目绩效表/产出及成效》 没有完善，请完善后再次提交！" }, "text/html");
                }
                if (ccjx == "0")
                {
                    return Json(new { errorMsg = "《项目绩效表/产出指标》 没有完善，请完善后再次提交！" }, "text/html");
                }
                if (xljx == "0")
                {
                    return Json(new { errorMsg = "《项目绩效表/成效指标》 没有完善，请完善后再次提交！" }, "text/html");
                }
                if (csmx == "0")
                {
                    return Json(new { errorMsg = "《项目测算明细表》 没有完善，请完善后再次提交！" }, "text/html");
                }
                if (ndys == "0")
                {
                    return Json(new { errorMsg = "《分年度预算表》 没有完善，请完善后再次提交！" }, "text/html");
                }
                if (lxpj == "0")
                {
                    return Json(new { errorMsg = "《立项评级表》 没有完善，请完善后再次提交！" }, "text/html");
                }
                return Json(new { errorMsg = "《采购和配置计划表》 没有完善，请完善后再次提交！" }, "text/html");
            }
        }


        //部门负责人提交项目
        [HttpPost]
        [Authorize(Roles = "部门负责人")]
        public JsonResult bmfzrtijiao(string loingid, string shijian, int xmid)//FormCollection form,
        {
            xiangmuguanli xmgl = new xiangmuguanli();
            xmgl = db.xiangmuguanlis.Where(s => s.ID == xmid).FirstOrDefault();

            //这里的限制条件是，送审前要检查是否排过序
            //取出项目的序号
            //int xuhao = xmgl.Xuhao;
            string mulu = xmgl.Xiangmumulu;
            string xueyue = xmgl.suoshuxueyuan;

            int[] all_xuhao;

            ////这里为了避免linq缓存影响修改后的结果
            using (cwcxmkContent db1 = new cwcxmkContent()) //创建一个新的上下文
            {
                //这里在筛选时 加了一个，员工的提交状态为“已提交”，否则的话，这里会把员工创建的项目，也作为筛选范围
                all_xuhao = db1.xiangmuguanlis.Where(s => s.Xiangmumulu == mulu && s.suoshuxueyuan == xueyue && s.tijiaozhuantai == "已提交").Select(s => s.Xuhao).ToArray();
            }

            //去重数组的维数
            //没有去重的数组，会有相同的 一些默认序号，所以  没有排序过的项目，是会有相同的默认序号的
            int all_xuhao_dis = all_xuhao.Distinct().Count();

            //数组的维数
            int all_xuhao_cou = all_xuhao.Length;

            if (all_xuhao == null)
            {
                return Json(new { errorMsg = "您还未选中要送审的项目，请再试一试！" }, "text/html");
            }
            else
            {
                if (all_xuhao_dis != all_xuhao_cou)      //比较两个数组维数是否相同，就可判断是否有相同序号，要两个数组维数相等的情况下，才可能不存在有相同序号
                {
                    return Json(new { errorMsg = "在送审前，请对您单位的项目进行排序！" }, "text/html");
                }
            }

            //创建项目时添加项目日志内容
            string rizhiname = customidentity.rizhiusername(loingid);
            string rizhirole = customidentity.rizhiuserrole(loingid);

            xmrizhi xmrizhi = new xmrizhi();
            xmrizhi.xiangmuguanliID = xmid;
            xmrizhi.shenheshijian = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
            xmrizhi.shenhejiedian = "部门负责人提交";
            xmrizhi.shenheren = rizhiname;
            xmrizhi.shenhejuese = rizhirole;
            //xmrizhi.shenhejieguo = bmfzryijian;

            //在提交之前判断其他的几个附属表是否已经完善
            bool shifoutwanshan = mulucustom.shifouwanshan(xmid);

            var bmfzrshenhe = xmgl.bmfzrshenhe;    //取出该记录的部门负责人审核状态，若为未通过则在重新提交时，需要将未通过改为未审核状态，以便重新审核
            var bmfzrtj = xmgl.bmfzrtijiao;
            var fgld = xmgl.fgldshenhe;
            var ywzyshenhe = xmgl.ywzyshenhe;
            var fgldshenhe = xmgl.fgldshenhe;

            var pingweishenhe = xmgl.pingweishenhe;
            string username = xmgl.username;//记录创建者名字

            string shifoubmfzr = customidentity.userrole_name(username);//判断该记录是否由 角色是 部门负责人 的成员创建，这里还未判断 登录者 提交 的人是哪一个角色

            if (shifoutwanshan == true)
            {
                //if (customidentity.userrole(loingid) == "部门负责人")  //如果是由部门负责人创建并提交的，则在提交时，部门负责人的审核状态为通过
                if (shifoubmfzr == "部门负责人")  //如果是由部门负责人创建并提交的，则在提交时，部门负责人的审核状态为通过
                {
                    xmgl.tijiaozhuantai = "已提交";
                    xmgl.bmfzrtijiao = "已提交";

                    //部门负责人自己创建的项目，他自己审核时不可能为不通过
                    //如果部门负责人提交的是从上面返回来再次提及的记录
                    if (pingweishenhe == "撤回" && fgldshenhe == "通过")
                    {
                        xmgl.bmfzrshenhe = "通过";
                        xmgl.ywzyshenhe = "未审核";
                        xmgl.pingweishenhe = "未审核";
                    }
                    //这里是分管领导不参与审核时部门负责人建立，部门负责人提交后被打回来，再由部门负责人再次提交情况
                    if (bmfzrshenhe == "通过" && pingweishenhe == "撤回" && fgldshenhe == "未审核")
                    {
                        xmgl.bmfzrshenhe = "通过";
                        xmgl.pingweishenhe = "未审核";
                        xmgl.ywzyshenhe = "未审核";
                    }

                    //这里感觉有点怪
                    if (fgldshenhe == "未通过" && pingweishenhe != "撤回")
                    {
                        //bmfzrshenhe = "通过";
                        //fgldshenhe = "未审核";
                        //ywzyshenhe = "未审核";
                        //pingweishenhe = "未审核";
                        xmgl.bmfzrshenhe = "通过";
                        xmgl.fgldshenhe = "未审核";
                        xmgl.ywzyshenhe = "未审核";
                        xmgl.pingweishenhe = "未审核";
                    }
                    //这里是初次提交
                    if (ywzyshenhe == "未审核" && fgldshenhe == "未审核" && pingweishenhe == "未审核")
                    {
                        xmgl.bmfzrshenhe = "通过";
                    }
                }
                else                              //这里不是部门负责人自己创建的项目还需要由部门负责人判断，项目是否已经经过了审核，否则不能提交
                {
                    xmgl.bmfzrtijiao = "已提交";
                    if (bmfzrshenhe == "未审核")   //这里是初始时datagrid出现的提交按钮，如果部门负责人是未审核的，则按了提交后出现下面提示
                    {
                        return Json(new { errorMsg = "部门负责人未审核，请审核后再次提交！" }, "text/html");
                    }
                    //部门负责人自己创建的项目，他自己审核时不可能为不通过
                    if (bmfzrshenhe == "通过" && pingweishenhe == "撤回" && fgldshenhe == "通过")//如果部门负责人提交的是从上面返回来再次提及的记录
                    {
                        xmgl.bmfzrshenhe = "通过";
                        xmgl.ywzyshenhe = "未审核";
                        xmgl.pingweishenhe = "未审核";
                    }

                    //这里是分管领导不参与审核时员工建立，部门负责人提交后被打回来，再由部门负责人再次提交情况
                    if (bmfzrshenhe == "通过" && pingweishenhe == "撤回" && fgldshenhe == "未审核")
                    {
                        xmgl.bmfzrshenhe = "通过";
                        xmgl.ywzyshenhe = "未审核";
                        xmgl.pingweishenhe = "未审核";
                    }

                    //这里是分管领导审核时就不通过，再由部门负责人提交一次
                    if (fgldshenhe == "未通过" && ywzyshenhe == "未审核")
                    {
                        xmgl.bmfzrshenhe = "通过";
                        xmgl.fgldshenhe = "未审核";
                        xmgl.ywzyshenhe = "未审核";
                        xmgl.pingweishenhe = "未审核";
                    }
                    //这里是初次提交
                    if (ywzyshenhe == "未审核" && fgldshenhe == "未审核" && pingweishenhe == "未审核")
                    {
                        xmgl.bmfzrshenhe = "通过";
                    }
                }
                using (TransactionScope transaction = new TransactionScope())//原子操作，事物错误回滚
                {
                    try
                    {
                        db.Entry(xmgl).State = EntityState.Modified;                   //更新项目管理表中表示基本信息表的字段，这种更新方式不知道对不对
                        db.xmrizhis.Add(xmrizhi);
                        db.SaveChanges();
                        transaction.Complete();
                    }
                    catch (Exception ex)
                    {
                        return Json(new { errorMsg = ex.ToString() }, "text/html");
                    }
                    finally
                    {
                        transaction.Dispose();
                    }
                }
                return Json(new { success = 1, Succeeded = "提交成功，OK！" }, "text/html");
            }
            else          //这里可以写一个判断，提示给用户是哪一个表没有完善
            {
                string jpxx = "0";
                string ccjx = "0";
                string xljx = "0";
                string csmx = "0";
                string lxpj = "0";
                string cgpz = "0";
                string cccx = "0";
                string ndys = "0";
                using (cwcxmkContent db1 = new cwcxmkContent()) //创建一个新的上下文
                {

                    var xiangmu = db1.xiangmuguanlis.Where(s => s.ID == xmid).FirstOrDefault();
                    jpxx = xiangmu.jpxx;
                    ccjx = xiangmu.ccjx;
                    xljx = xiangmu.xljx;
                    csmx = xiangmu.csmx;
                    lxpj = xiangmu.lxpj;
                    cgpz = xiangmu.cgpz;
                    cccx = xiangmu.cccx;
                    ndys = xiangmu.nianduyusuan;
                }
                if (jpxx == "0")
                {
                    return Json(new { errorMsg = "《基本信息表》 没有完善，请完善后再次提交！" }, "text/html");
                }
                if (cccx == "0")
                {
                    return Json(new { errorMsg = "《项目绩效表/产出及成效》 没有完善，请完善后再次提交！" }, "text/html");
                }

                if (ccjx == "0")
                {
                    return Json(new { errorMsg = "《项目绩效表/产出指标》 没有完善，请完善后再次提交！" }, "text/html");
                }
                if (xljx == "0")
                {
                    return Json(new { errorMsg = "《项目绩效表/成效指标》 没有完善，请完善后再次提交！" }, "text/html");
                }
                if (csmx == "0")
                {
                    return Json(new { errorMsg = "《项目测算明细表》 没有完善，请完善后再次提交！" }, "text/html");
                }

                if (ndys == "0")
                {
                    return Json(new { errorMsg = "《分年度预算表》 没有完善，请完善后再次提交！" }, "text/html");
                }
                if (lxpj == "0")
                {
                    return Json(new { errorMsg = "《立项评级表》 没有完善，请完善后再次提交！" }, "text/html");
                }

                return Json(new { errorMsg = "《采购和配置计划表》 没有完善，请完善后再次提交！" }, "text/html");
            }
        }



        //分管领导人提交操作
        [HttpPost]
        [Authorize(Roles = "分管领导")]
        public JsonResult fgldtijiao(string name, int xmid, string loingid)//FormCollection form,
        {
            xiangmuguanli xmgl = new xiangmuguanli();
            xmgl = db.xiangmuguanlis.Where(s => s.ID == xmid).FirstOrDefault();
            var fgldshenh = xmgl.fgldshenhe;

            try
            {
                if (fgldshenh == "未审核")
                {
                    return Json(new { errorMsg = "未审核或审核未通过，不能提交！" }, "text/html");
                }
                xmgl.fgldtijiao = "已提交";
                db.Entry(xmgl).State = EntityState.Modified;                   //更新项目管理表中表示基本信息表的字段，这种更新方式不知道对不对

                db.SaveChanges();

                return Json(new { Succeeded = "提交成功，OK！" }, "text/html");
            }
            catch (Exception ex)
            {
                return Json(new { errorMsg = ex.ToString() }, "text/html");
            }
        }


        //完善信息，基本信息表更改框架后tab执行方法，加载基本信息表
        public ActionResult wsxx_jbxx(int xmid, string xmname, string xmgl_jbxx)
        {
            ViewBag.xmname = xmname;
            ViewBag.xmgl_jbxx = xmgl_jbxx;
            ViewBag.xmid = xmid;
            return View();
        }

        //完善信息，项目绩效表，更改框架后tab执行方法，加载基本信息表
        public ActionResult wsxx_xmjx(int xmid, string xmname, string xmgl_cccx)
        {
            ViewBag.xmname = xmname;
            ViewBag.xmgl_cccx = xmgl_cccx;
            ViewBag.xmid = xmid;
            return View();
        }


        //完善信息，测算明细表，更改框架后tab执行方法，加载基本信息表
        public ActionResult wsxx_csmx(int xmid)
        {
            //ViewBag.xmname = xmname;

            ViewBag.xmid = xmid;
            return View();
        }

        //完善信息，分年度预算
        public ActionResult wsxx_nianduyusuan(int xmid, string xmname, string xmgl_ndys)
        {
            ViewBag.xmname = xmname;

            ViewBag.xmid = xmid;

            var xmgl = db.xiangmuguanlis.Where(s => s.ID == xmid).FirstOrDefault();

            ViewBag.xmgl_ndys = xmgl_ndys;

            ViewBag.zongjine = xmgl.zongjine;

            ViewBag.cj_year = Convert.ToInt32(xmgl.Chuangjianshijian.Substring(0, 4));

            return View();
        }


        //完善信息，立项评级表，更改框架后tab执行方法，加载立项评级表
        public ActionResult wsxx_lxpj(int xmid, string xmname, string xmgl_lxpj)
        {
            ViewBag.xmid = xmid;
            ViewBag.xmname = xmname;
            //ViewBag.xmgl_lxpj = xmgl_lxpj;
            if (xmgl_lxpj == "1")
            {
                return View("wsxx_lxpj_yt");
            }
            else
            {
                return View("wsxx_lxpj_wt");
            }
        }


        //完善信息，采购配置，更改框架后tab执行方法，加载立项评级表
        public ActionResult wsxx_cgpz(int xmid, bool xmgl_cgpz_null)
        {
            ViewBag.cgpz_null = xmgl_cgpz_null;
            ViewBag.xmid = xmid;
            return View();
        }


        //完善信息，附件上传，更改框架后tab执行方法，加载立项评级表
        public ActionResult wsxx_fjsc(int xmid, string xmname, string xmmulu)
        {
            ViewBag.xmname = xmname;
            ViewBag.xmmulu = xmmulu;
            ViewBag.xmid = xmid;
            return View();
        }


        //加载编辑基本信息表，因为这个表和基本信息表是一对一关系
        public JsonResult load_jbxx(int xmid, string shijian)
        {
            var jbxx = db.xmjibenxinxis.Where(s => s.xiangmuguanliID == xmid).FirstOrDefault();
            if (jbxx != null)
            {
                //本来应该可以直接 return Json(jbxx),但是jbxx中的时间格式（2017/03/30）不能够被前端页面接受，所有必须拿出来处理一下再传。
                return Json(new { Xiangmumingcheng = jbxx.Xiangmumingcheng, Kaishishijian = jbxx.Kaishishijian.ToString("yyyy-MM-dd"), Jieshushijian = jbxx.Jieshushijian.ToString("yyyy-MM-dd"), Zhengceyiju = jbxx.Zhengceyiju, Xiangmubeijing = jbxx.Xiangmubeijing, shishidizhi = jbxx.shishidizhi, jingbanren = jbxx.jingbanren, Lianxidianhua = jbxx.Lianxidianhua }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { xiangmuguanliID = xmid, Kaishishijian = "", Jieshushijian = "", Zhengceyiju = "", Xiangmubeijing = "", shishidizhi = "", jingbanren = "", Lianxidianhua = "" }, JsonRequestBehavior.AllowGet);
            }
        }

        //加载编辑基本信息表，
        public JsonResult load_lxpj(int xmid, string shijian)
        {
            xmlxpj lxpj = new xmlxpj();

            lxpj = db.xmlxpjs.Where(s => s.xiangmuguanliID == xmid).FirstOrDefault(); //选择要加载到表中的记录
            return Json(lxpj, JsonRequestBehavior.AllowGet);   //返回要加载到表中的json记录
        }

        //加载项目产出成效
        public JsonResult load_cccx(int xmid, string shijian)
        {
            xmchanchuchengxiao cccx = new xmchanchuchengxiao();
            cccx = db.xmchanchuchengxiao.Where(s => s.xiangmuguanliID == xmid).FirstOrDefault();
            return Json(cccx, JsonRequestBehavior.AllowGet);
        }

        //加载分年度预算表
        public JsonResult load_ndys(int xmid, string shijian)
        {
            var ndys = db.xmnianduyusuans.Where(s => s.xiangmuguanliID == xmid).FirstOrDefault();

            if (ndys != null)
            {
                return Json(new { diyinian = ndys.diyinian, diernian = ndys.diernian, disannian = ndys.disannian, yusuanshuoming = ndys.yusuanshuoming }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { xiangmuguanliID = "", diyinian = "", diernian = "", disannian = "", yusuanshuoming = "" }, JsonRequestBehavior.AllowGet);
            }
        }


        //保存年度预算
        [HttpPost]
        public JsonResult nianduyusuan_save(string name, int xmid)//FormCollection form,
        {
            xmnianduyusuan ndys = new xmnianduyusuan();

            //设置项目管理对象，编辑  基本信息表  时更改  项目管理表  中基本信息表的状态，过滤的方式可以是name项目名称
            //更改项目管理表中的  jpxx 为1

            ndys.xiangmuguanliID = xmid;
            ndys.Xiangmumingcheng = name;

            ndys.zijingxingzhi = "财政拨款";

            ndys.zhuankuanleibie = "";

            ndys.gongnengfenlei = "高等教育";

            ndys.diyinian = Convert.ToDecimal(Request.Form["diyinian"]);
            ndys.diernian = Convert.ToDecimal(Request.Form["diernian"]);
            ndys.disannian = Convert.ToDecimal(Request.Form["disannian"]);
            ndys.yusuanshuoming = Request.Form["yusuanshuoming"];
            ndys.qishiniandu = DateTime.Now.ToString("yyyy");

            xiangmuguanli xmgl = new xiangmuguanli();

            xmgl = db.xiangmuguanlis.Where(s => s.ID == xmid).FirstOrDefault();   //选择第一项或者符合条件的项

            xmgl.nianduyusuan = "1";
            xmgl.zongjine = (ndys.diyinian + ndys.diernian + ndys.disannian) / 10;

            using (TransactionScope transaction = new TransactionScope())//原子操作，事物错误回滚
            {
                try
                {
                    db.Entry(xmgl).State = EntityState.Modified;                   //更新项目管理表中表示基本信息表的字段，这种更新方式不知道对不对
                    db.xmnianduyusuans.Add(ndys);
                    db.SaveChanges();
                    transaction.Complete();
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = ex.ToString() }, "text/html");
                }
                finally
                {
                    transaction.Dispose();
                }
            }
            return Json(new { success = true, message = "保存成功，请完善其他信息！" }, "text/html");
        }


        [HttpPost]
        public JsonResult nianduyusuan_edit(string name, int xmid)//FormCollection form,
        {
            xmnianduyusuan ndys = new xmnianduyusuan();

            ndys = db.xmnianduyusuans.Where(s => s.xiangmuguanliID == xmid).FirstOrDefault();   //选择第一项或者符合条件的项

            ndys.diyinian = Convert.ToDecimal(Request.Form["diyinian"]);

            ndys.diernian = Convert.ToDecimal(Request.Form["diernian"]);
            ndys.disannian = Convert.ToDecimal(Request.Form["disannian"]);
            ndys.yusuanshuoming = Request.Form["yusuanshuoming"];

            xiangmuguanli xmgl = new xiangmuguanli();

            xmgl = db.xiangmuguanlis.Where(s => s.ID == xmid).FirstOrDefault();   //选择第一项或者符合条件的项

            xmgl.zongjine = (ndys.diyinian + ndys.diernian + ndys.disannian) / 10;

            using (TransactionScope transaction = new TransactionScope())//原子操作，事物错误回滚
            {
                try
                {
                    db.Entry(ndys).State = EntityState.Modified;                   //更新项目管理表中表示基本信息表的字段，这种更新方式不知道对不对

                    db.Entry(xmgl).State = EntityState.Modified;
                    db.SaveChanges();
                    transaction.Complete();
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = ex.ToString() }, "text/html");
                }
                finally
                {
                    transaction.Dispose();
                }
            }
            return Json(new { success = true, message = "更新成功，请完善其他信息！" }, "text/html");
        }


        //保存基本信息
        [HttpPost]
        public JsonResult add_jbxx(string name, int xmid)//FormCollection form,
        {
            xmjibenxinxi jbxx = new xmjibenxinxi();

            //设置项目管理对象，编辑  基本信息表  时更改  项目管理表  中基本信息表的状态，过滤的方式可以是name项目名称
            xiangmuguanli xmgl = new xiangmuguanli();

            xmgl = db.xiangmuguanlis.Where(s => s.ID == xmid).FirstOrDefault();   //选择第一项或者符合条件的项

            xmgl.jpxx = "1";                                                //更改项目管理表中的  jpxx 为1
            //db.Entry(xmgl).State = EntityState.Modified;                   //更新项目管理表中表示基本信息表的字段，这种更新方式不知道对不对

            //jbxx.xiangmuguanliID = Convert.ToInt32(Request.Form["xiangmuguanliID"]);
            jbxx.xiangmuguanliID = xmid;
            jbxx.Xiangmumingcheng = name;
            jbxx.Kaishishijian = Convert.ToDateTime(Request.Form["Kaishishijian"]).Date;

            jbxx.Jieshushijian = Convert.ToDateTime(Request.Form["Jieshushijian"]).Date;

            jbxx.Zhengceyiju = Request.Form["Zhengceyiju"];

            jbxx.Xiangmubeijing = Request.Form["Xiangmubeijing"];

            jbxx.shishidizhi = Request.Form["shishidizhi"];
            jbxx.jingbanren = Request.Form["jingbanren"];
            jbxx.Lianxidianhua = Request.Form["Lianxidianhua"];

            using (TransactionScope transaction = new TransactionScope())//原子操作，事物错误回滚
            {
                try
                {
                    db.Entry(xmgl).State = EntityState.Modified;                   //更新项目管理表中表示基本信息表的字段，这种更新方式不知道对不对
                    db.xmjibenxinxis.Add(jbxx);
                    db.SaveChanges();
                    transaction.Complete();
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = ex.ToString() }, "text/html");
                }
                finally
                {
                    transaction.Dispose();
                }
            }
            return Json(new { success = true, message = "保存成功，请完善其他信息！" }, "text/html");
        }


        //加载显示 之前的备注
        public JsonResult load_xiaoneibeizhu(int xmid, string shijian, string username, string userid)
        {
            xiangmuguanli xm = new xiangmuguanli();

            var beizhu = db.xiangmuguanlis.Where(s => s.ID == xmid).FirstOrDefault().xiaoneibeizhu;

            string beizhulist_str = "";

            //要判断是否有备注
            if (beizhu == null)
            {
                return Json(new { beizhu = beizhulist_str }, JsonRequestBehavior.AllowGet);
            }
            else   //没有备注的情况下
            {
                string[] beizhufenzu = beizhu.Split('&');

                int fenzucount = beizhufenzu.Length;

                string role = customidentity.userrole_name(username);  //判断登录用户角色

                if (username == "admin" || role == "评委" || role == "领导" || role == "分管领导")   //这里的意思是，如果登陆的人是这几个，那么久可以看到全部校内评审留下的备注，否则值看到个人自己的留下的备注
                {
                    for (var i = 0; i < fenzucount; i++)
                    {
                        string str = beizhufenzu[i].Replace("&", "") + "\r\n";   //这里是因为，添加校内备注的时候，都是默认在结尾处添加一个分隔符和和换行符
                        beizhulist_str = beizhulist_str + str;
                    }
                }
                else
                {
                    for (var i = 0; i < fenzucount; i++)
                    {
                        if (beizhufenzu[i].Contains(userid))        //使用userid作为筛选的条件比username更有精度
                        {
                            string str = beizhufenzu[i].Replace("&", "") + "\r\n";

                            beizhulist_str = beizhulist_str + str;
                        }
                    }
                }
                return Json(new { beizhu = beizhulist_str }, JsonRequestBehavior.AllowGet);   //返回要加载到表中的json记录
            }
        }


        [HttpPost]
        public JsonResult add_xiaoneibeizhu(int xmid, string userid)//FormCollection form,
        {
            //1、判断该字段是否为空，不为空的话要在该值后面添加上新值

            xiangmuguanli xm = new xiangmuguanli();

            xm = db.xiangmuguanlis.Where(s => s.ID == xmid).FirstOrDefault();   //选择第一项或者符合条件的项

            var beizhuzhi = xm.xiaoneibeizhu;    //如果当前用户已经有评审记录，则取出当前值，做分组，取出该用户的评审字符串

            //这里的备注是给还没有值的  字段赋值用的
            string beizhu = userid + ": " + Request.Form["xiaoneibeizhu"] + " " + DateTime.Now.ToString("yyyy-MM-dd") + "；" + "&";  //这里的“\r\n”没有保存在数据库中也能在页面上实现换行，什么原因

            //这里的备注是给需要修改的  字段  赋值用的
            //string beizhu_yiyou = username + ": " + Request.Form["xiaoneibeizhu"] + " " + DateTime.Now.ToString("yyyy-MM-dd") + "；" ;

            if (xm.xiaoneibeizhu == null || xm.xiaoneibeizhu == "")
            {
                xm.xiaoneibeizhu = beizhu;
            }
            else
            {
                //可以看到评审的过程
                //xm.xiaoneibeizhu = xm.xiaoneibeizhu + beizhu;

                //只需要看到评审的最终结果
                if (beizhuzhi.Contains(userid))
                {
                    string[] beizhuzhifenzu = beizhuzhi.Split('&');

                    int fenzu_count = beizhuzhifenzu.Count();

                    string str = "";

                    for (var i = 0; i < fenzu_count - 1; i++)
                    {
                        if (beizhuzhifenzu[i].Contains(userid))
                        {
                            beizhuzhifenzu[i] = beizhu;

                            str = str + beizhuzhifenzu[i];
                        }
                        else
                        {
                            str = str + beizhuzhifenzu[i] + "&";
                        }
                    }
                    xm.xiaoneibeizhu = str;
                }
                else
                {
                    xm.xiaoneibeizhu = beizhuzhi + beizhu;
                }
            }
            try
            {
                db.Entry(xm).State = EntityState.Modified;                   //更新项目管理表中表示基本信息表的字段，这种更新方式不知道对不对

                db.SaveChanges();

                return Json(new { success = true, message = "保存成功，请完善其他信息！" }, "text/html");
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.ToString() }, "text/html");
            }

        }

        //编辑基本信息表
        [HttpPost]
        public JsonResult edit_jbxx(string name, int xmid)//FormCollection form,
        {
            xmjibenxinxi jbxx = new xmjibenxinxi();

            jbxx = db.xmjibenxinxis.Where(s => s.xiangmuguanliID == xmid).FirstOrDefault();   //选择第一项或者符合条件的项

            jbxx.Kaishishijian = Convert.ToDateTime(Request.Form["Kaishishijian"]);

            jbxx.Jieshushijian = Convert.ToDateTime(Request.Form["Jieshushijian"]);


            jbxx.Zhengceyiju = Request.Form["Zhengceyiju"];

            jbxx.Xiangmubeijing = Request.Form["Xiangmubeijing"];

            jbxx.shishidizhi = Request.Form["shishidizhi"];
            jbxx.jingbanren = Request.Form["jingbanren"];
            jbxx.Lianxidianhua = Request.Form["Lianxidianhua"];
            try
            {
                db.Entry(jbxx).State = EntityState.Modified;                   //更新项目管理表中表示基本信息表的字段，这种更新方式不知道对不对
                db.SaveChanges();
                return Json(new { success = true, message = "保存成功，请完善其他信息！" }, "text/html");
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.ToString() }, "text/html");
            }
        }


        //产出成效保存
        [HttpPost]
        public JsonResult add_cccx(string name, int xmid)//FormCollection form,
        {
            xmchanchuchengxiao cccx = new xmchanchuchengxiao();

            //设置项目管理对象，编辑  基本信息表  时更改  项目管理表  中基本信息表的状态，过滤的方式可以是name项目名称
            xiangmuguanli xmgl = new xiangmuguanli();

            xmgl = db.xiangmuguanlis.Where(s => s.ID == xmid).FirstOrDefault();   //选择第一项或者符合条件的项

            xmgl.cccx = "1";

            cccx.xiangmuguanliID = xmid;
            cccx.xmchanchu = Request.Form["xmchanchu"];
            cccx.xmchengxiao = Request.Form["xmchengxiao"];

            using (TransactionScope transaction = new TransactionScope())//原子操作，事物错误回滚
            {
                try
                {
                    db.Entry(xmgl).State = EntityState.Modified;                   //更新项目管理表中表示基本信息表的字段，这种更新方式不知道对不对
                    db.xmchanchuchengxiao.Add(cccx);
                    db.SaveChanges();
                    transaction.Complete();
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, errorMsg = ex.ToString() }, "text/html");
                }
                finally
                {
                    transaction.Dispose();
                }
            }
            return Json(new { success = true, message = "保存成功，请完善其他信息！" }, "text/html");
        }


        //编辑项目产出、项目成效
        [HttpPost]
        public JsonResult edit_cccx(string name, int xmid)//FormCollection form,
        {
            xmchanchuchengxiao cccx = new xmchanchuchengxiao();

            cccx = db.xmchanchuchengxiao.Where(s => s.xiangmuguanliID == xmid).FirstOrDefault();

            cccx.xmchanchu = Request.Form["xmchanchu"];
            cccx.xmchengxiao = Request.Form["xmchengxiao"];
            try
            {
                db.Entry(cccx).State = EntityState.Modified;                   //更新项目管理表中表示基本信息表的字段，这种更新方式不知道对不对
                db.SaveChanges();

                return Json(new { success = true, message = "保存成功，请完善其他信息！" }, "text/html");
            }
            catch (Exception ex)
            {
                return Json(new { success = false, errorMsg = ex.ToString() }, "text/html");
            }
        }



        //立项评级保存
        [HttpPost]
        public JsonResult lxpj_add(string name, int xmid)//FormCollection form, 
        {
            xmjibenxinxi xiamgmu = new xmjibenxinxi();

            //设置项目管理对象，编辑  基本信息表  时更改  项目管理表  中基本信息表的状态，过滤的方式可以是name项目名称
            xiangmuguanli xmgl = new xiangmuguanli();
            xmgl = db.xiangmuguanlis.Where(s => s.ID == xmid).FirstOrDefault();   //选择第一项或者符合条件的项
            xmgl.lxpj = "1";

            xmlxpj lxpj = new xmlxpj();

            lxpj.xiangmuguanliID = xmid;//因为是关联表，所以主键不能是自动增长，要给主键赋值

            //一部分
            lxpj.one_1_1 = Request.Form["one_1_1"];
            lxpj.one_1_1_f = Convert.ToInt32(Request.Form["one_1_1_f"]);

            lxpj.one_1_2 = Request.Form["one_1_2"];
            lxpj.one_1_2_f = Convert.ToInt32(Request.Form["one_1_2_f"]);

            lxpj.one_1_3 = Request.Form["one_1_3"];
            lxpj.one_1_3_f = Convert.ToInt32(Request.Form["one_1_3_f"]);

            lxpj.one_2_1 = Request.Form["one_2_1"];
            lxpj.one_2_1_f = Convert.ToInt32(Request.Form["one_2_1_f"]);

            //lxpj.one_2_2 = Request.Form["one_2_2"];
            //lxpj.one_2_2_f = Convert.ToInt32(Request.Form["one_2_2_f"]);

            lxpj.one_3_1 = Request.Form["one_3_1"];
            lxpj.one_3_1_f = Convert.ToInt32(Request.Form["one_3_1_f"]);

            lxpj.one_3_2 = Request.Form["one_3_2"];
            lxpj.one_3_2_f = Convert.ToInt32(Request.Form["one_3_2_f"]);

            //lxpj.one_3_3 = Request.Form["one_3_3"];
            //lxpj.one_3_3_f = Convert.ToInt32(Request.Form["one_3_3_f"]);

            lxpj.one_4_1 = Request.Form["one_4_1"];
            lxpj.one_4_1_f = Convert.ToInt32(Request.Form["one_4_1_f"]);

            lxpj.one_4_2 = Request.Form["one_4_2"];
            lxpj.one_4_2_f = Convert.ToInt32(Request.Form["one_4_2_f"]);

            //二部分
            lxpj.two_1_1 = Request.Form["two_1_1"];
            lxpj.two_1_1_f = Convert.ToInt32(Request.Form["two_1_1_f"]);

            lxpj.two_1_2 = Request.Form["two_1_2"];
            lxpj.two_1_2_f = Convert.ToInt32(Request.Form["two_1_2_f"]);

            lxpj.two_2_1 = Request.Form["two_2_1"];
            lxpj.two_2_1_f = Convert.ToInt32(Request.Form["two_2_1_f"]);

            lxpj.two_2_2 = Request.Form["two_2_2"];
            lxpj.two_2_2_f = Convert.ToInt32(Request.Form["two_2_2_f"]);

            lxpj.two_2_3 = Request.Form["two_2_3"];
            lxpj.two_2_3_f = Convert.ToInt32(Request.Form["two_2_3_f"]);

            lxpj.two_3_1 = Request.Form["two_3_1"];
            lxpj.two_3_1_f = Convert.ToInt32(Request.Form["two_3_1_f"]);

            lxpj.two_3_2 = Request.Form["two_3_2"];
            lxpj.two_3_2_f = Convert.ToInt32(Request.Form["two_3_2_f"]);

            lxpj.two_3_3 = Request.Form["two_3_3"];
            lxpj.two_3_3_f = Convert.ToInt32(Request.Form["two_3_3_f"]);

            lxpj.two_4_1 = Request.Form["two_4_1"];
            lxpj.two_4_1_f = Convert.ToInt32(Request.Form["two_4_1_f"]);

            lxpj.two_4_2 = Request.Form["two_4_2"];
            lxpj.two_4_2_f = Convert.ToInt32(Request.Form["two_4_2_f"]);

            //三部分
            lxpj.three_1_1 = Request.Form["three_1_1"];
            lxpj.three_1_1_f = Convert.ToInt32(Request.Form["three_1_1_f"]);

            lxpj.three_1_2 = Request.Form["three_1_2"];
            lxpj.three_1_2_f = Convert.ToInt32(Request.Form["three_1_2_f"]);

            lxpj.three_2_1 = Request.Form["three_2_1"];
            lxpj.three_2_1_f = Convert.ToInt32(Request.Form["three_2_1_f"]);

            //lxpj.three_2_2 = Request.Form["three_2_2"];
            //lxpj.three_2_2_f = Convert.ToInt32(Request.Form["three_2_2_f"]);

            lxpj.three_2_3 = Request.Form["three_2_3"];
            lxpj.three_2_3_f = Convert.ToInt32(Request.Form["three_2_3_f"]);

            //第四部分
            lxpj.four_1_1 = Request.Form["four_1_1"];
            lxpj.four_1_1_f = Convert.ToInt32(Request.Form["four_1_1_f"]);

            lxpj.four_1_2 = Request.Form["four_1_2"];
            lxpj.four_1_2_f = Convert.ToInt32(Request.Form["four_1_2_f"]);

            //项目等级
            lxpj.xmdengji = Request.Form["xmdengji"];

            //项目总分
            lxpj.zongfen = Convert.ToInt32(Request.Form["zongfen"]);
            using (TransactionScope transaction = new TransactionScope())//原子操作，事物错误回滚
            {
                try
                {
                    db.Entry(xmgl).State = EntityState.Modified;                   //更新项目管理表中表示基本信息表的字段，这种更新方式不知道对不对
                    db.xmlxpjs.Add(lxpj);
                    db.SaveChanges();
                    transaction.Complete();
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, errorMsg = ex.ToString() }, "text/html");
                }
                finally
                {
                    transaction.Dispose();
                }
            }
            return Json(new { success = true, message = "保存成功，请完善其他信息！" }, "text/html");
        }


        [HttpPost]
        public JsonResult lxpj_edit(string name, int xmid)//FormCollection form, 
        {
            xmjibenxinxi xiamgmu = new xmjibenxinxi();

            xiamgmu = db.xmjibenxinxis.Where(s => s.xiangmuguanliID == xmid).FirstOrDefault();   //选择第一项或者符合条件的项

            xmlxpj lxpj = new xmlxpj();

            lxpj = db.xmlxpjs.Where(s => s.xiangmuguanliID == xmid).FirstOrDefault();

            //lxpj.xiangmuguanliID = Convert.ToInt32(Request.Form["xiangmuguanliID"]);//因为是关联表，所以主键不能是自动增长，要给主键赋值

            //一部分
            lxpj.one_1_1 = Request.Form["one_1_1"];
            lxpj.one_1_1_f = Convert.ToInt32(Request.Form["one_1_1_f"]);

            lxpj.one_1_2 = Request.Form["one_1_2"];
            lxpj.one_1_2_f = Convert.ToInt32(Request.Form["one_1_2_f"]);

            lxpj.one_1_3 = Request.Form["one_1_3"];
            lxpj.one_1_3_f = Convert.ToInt32(Request.Form["one_1_3_f"]);

            lxpj.one_2_1 = Request.Form["one_2_1"];
            lxpj.one_2_1_f = Convert.ToInt32(Request.Form["one_2_1_f"]);

            //lxpj.one_2_2 = Request.Form["one_2_2"];
            //lxpj.one_2_2_f = Convert.ToInt32(Request.Form["one_2_2_f"]);

            lxpj.one_3_1 = Request.Form["one_3_1"];
            lxpj.one_3_1_f = Convert.ToInt32(Request.Form["one_3_1_f"]);

            lxpj.one_3_2 = Request.Form["one_3_2"];
            lxpj.one_3_2_f = Convert.ToInt32(Request.Form["one_3_2_f"]);

            //lxpj.one_3_3 = Request.Form["one_3_3"];
            //lxpj.one_3_3_f = Convert.ToInt32(Request.Form["one_3_3_f"]);

            lxpj.one_4_1 = Request.Form["one_4_1"];
            lxpj.one_4_1_f = Convert.ToInt32(Request.Form["one_4_1_f"]);

            lxpj.one_4_2 = Request.Form["one_4_2"];
            lxpj.one_4_2_f = Convert.ToInt32(Request.Form["one_4_2_f"]);


            //二部分
            lxpj.two_1_1 = Request.Form["two_1_1"];
            lxpj.two_1_1_f = Convert.ToInt32(Request.Form["two_1_1_f"]);

            lxpj.two_1_2 = Request.Form["two_1_2"];
            lxpj.two_1_2_f = Convert.ToInt32(Request.Form["two_1_2_f"]);

            lxpj.two_2_1 = Request.Form["two_2_1"];
            lxpj.two_2_1_f = Convert.ToInt32(Request.Form["two_2_1_f"]);

            lxpj.two_2_2 = Request.Form["two_2_2"];
            lxpj.two_2_2_f = Convert.ToInt32(Request.Form["two_2_2_f"]);

            lxpj.two_2_3 = Request.Form["two_2_3"];
            lxpj.two_2_3_f = Convert.ToInt32(Request.Form["two_2_3_f"]);

            lxpj.two_3_1 = Request.Form["two_3_1"];
            lxpj.two_3_1_f = Convert.ToInt32(Request.Form["two_3_1_f"]);

            lxpj.two_3_2 = Request.Form["two_3_2"];
            lxpj.two_3_2_f = Convert.ToInt32(Request.Form["two_3_2_f"]);

            lxpj.two_3_3 = Request.Form["two_3_3"];
            lxpj.two_3_3_f = Convert.ToInt32(Request.Form["two_3_3_f"]);

            lxpj.two_4_1 = Request.Form["two_4_1"];
            lxpj.two_4_1_f = Convert.ToInt32(Request.Form["two_4_1_f"]);

            lxpj.two_4_2 = Request.Form["two_4_2"];
            lxpj.two_4_2_f = Convert.ToInt32(Request.Form["two_4_2_f"]);


            //三部分
            lxpj.three_1_1 = Request.Form["three_1_1"];
            lxpj.three_1_1_f = Convert.ToInt32(Request.Form["three_1_1_f"]);

            lxpj.three_1_2 = Request.Form["three_1_2"];
            lxpj.three_1_2_f = Convert.ToInt32(Request.Form["three_1_2_f"]);

            lxpj.three_2_1 = Request.Form["three_2_1"];
            lxpj.three_2_1_f = Convert.ToInt32(Request.Form["three_2_1_f"]);

            lxpj.three_2_2 = Request.Form["three_2_2"];
            lxpj.three_2_2_f = Convert.ToInt32(Request.Form["three_2_2_f"]);

            //lxpj.three_2_3 = Request.Form["three_2_3"];
            //lxpj.three_2_3_f = Convert.ToInt32(Request.Form["three_2_3_f"]);


            //第四部分
            lxpj.four_1_1 = Request.Form["four_1_1"];
            lxpj.four_1_1_f = Convert.ToInt32(Request.Form["four_1_1_f"]);

            lxpj.four_1_2 = Request.Form["four_1_2"];
            lxpj.four_1_2_f = Convert.ToInt32(Request.Form["four_1_2_f"]);

            //项目等级
            lxpj.xmdengji = Request.Form["xmdengji"];

            //项目总分
            lxpj.zongfen = Convert.ToInt32(Request.Form["zongfen"]);
            try
            {
                db.Entry(lxpj).State = EntityState.Modified;                   //更新立项评级表中选定的记录
                db.SaveChanges();
                return Json(new { success = true, message = "保存成功，请完善其他信息！" }, "text/html");
            }
            catch (Exception ex)
            {
                return Json(new { success = false, errorMsg = ex.ToString() }, "text/html");
            }
        }


        //获得项目绩效指标产出列表         这个方法在视图上好像还没有做分页处理，
        [HttpPost]
        public JsonResult getxmjx_cc(int xmglid)
        {
            int page = (Request.Form["page"] != null) ? Int32.Parse(Request.Form["page"]) : 1;

            int rows = (Request.Form["rows"] != null) ? Int32.Parse(Request.Form["rows"]) : 10;

            var jxccmulu = db.xmjixiao_ccs.Where(s => s.xiangmuguanliID == xmglid);
            var xm = from s in jxccmulu
                     orderby s.xmjixiao_ccID
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


        public JsonResult jxcc_save(int xmglID)//FormCollection form,
        {
            xmjixiao_cc jxcc = new xmjixiao_cc();

            //设置项目管理对象，编辑  基本信息表  时更改  项目管理表  中基本信息表的状态，过滤的方式可以是name项目名称
            xiangmuguanli xmgl = new xiangmuguanli();
            xmgl = db.xiangmuguanlis.Where(s => s.ID == xmglID).FirstOrDefault();   //选择第一项或者符合条件的项
            xmgl.ccjx = "1";                                                //更改项目管理表中的  jpxx 为1

            jxcc.xiangmuguanliID = xmglID;
            jxcc.jixiaozhibiao = Request.Form["jixiaozhibiao"];
            jxcc.jixiaomubiao = Request.Form["jixiaomubiao"];
            jxcc.jixiaoyou = Request.Form["jixiaoyou"];
            jxcc.jixiaoliang = Request.Form["jixiaoliang"];
            jxcc.jixiaozhong = Request.Form["jixiaozhong"];
            jxcc.jixiaocha = Request.Form["jixiaocha"];
            using (TransactionScope transaction = new TransactionScope())//原子操作，事物错误回滚
            {
                try
                {
                    db.Entry(xmgl).State = EntityState.Modified;                   //更新项目管理表中表示基本信息表的字段，这种更新方式不知道对不对
                    db.xmjixiao_ccs.Add(jxcc);
                    db.SaveChanges();
                    transaction.Complete();
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, errorMsg = ex.ToString() }, "text/html");
                }
                finally
                {
                    transaction.Dispose();
                }
            }
            return Json(new { success = true, message = "保存成功，请完善其他信息！" }, "text/html");
        }


        //产出绩效表编辑
        [HttpPost]
        public JsonResult jxcc_edit(int xmjxccID)//FormCollection form,
        {
            xmjixiao_cc jxcc = new xmjixiao_cc();

            jxcc = db.xmjixiao_ccs.Where(s => s.xmjixiao_ccID == xmjxccID).FirstOrDefault();   //选择第一项或者符合条件的项
            //jxcc.xiangmuguanliID = xmglID;
            jxcc.jixiaozhibiao = Request.Form["jixiaozhibiao"];
            jxcc.jixiaomubiao = Request.Form["jixiaomubiao"];
            jxcc.jixiaoyou = Request.Form["jixiaoyou"];
            jxcc.jixiaoliang = Request.Form["jixiaoliang"];
            jxcc.jixiaozhong = Request.Form["jixiaozhong"];
            jxcc.jixiaocha = Request.Form["jixiaocha"];
            try
            {
                db.Entry(jxcc).State = EntityState.Modified;                   //更新项目管理表中表示基本信息表的字段，这种更新方式不知道对不对
                db.SaveChanges();
                return Json(new { success = true, message = "保存成功，请完善其他信息！" }, "text/html");
            }
            catch (Exception ex)
            {
                return Json(new { success = false, errorMsg = ex.ToString() }, "text/html");
            }
        }


        //项目绩效产出 删除jxcc_del
        [HttpPost]
        public JsonResult jxcc_del(int id, int xmglID)  //如果这里的int id可以直接去到，为什么还要用form   FormCollection form, 
        {
            //这里判断对应于项目总表中的项目的 绩效产出表 的记录是多条还是1条，如果为1
            xmjixiao_cc cc = new xmjixiao_cc();

            var cc1 = db.xmjixiao_ccs.Where(s => s.xiangmuguanliID == xmglID);

            int cc_count = cc1.Count();

            xiangmuguanli xmgl = new xiangmuguanli();
            xmgl = db.xiangmuguanlis.Where(s => s.ID == xmglID).FirstOrDefault();   //选择第一项或者符合条件的项

            using (TransactionScope transaction = new TransactionScope())//原子操作，事物错误回滚
            {
                try
                {
                    if (cc_count > 1)
                    {
                        xmjixiao_cc jxcc = db.xmjixiao_ccs.Find(id);
                        db.xmjixiao_ccs.Remove(jxcc);
                        db.SaveChanges();
                    }
                    else
                    {
                        xmjixiao_cc jxcc = db.xmjixiao_ccs.Find(id);
                        db.xmjixiao_ccs.Remove(jxcc);

                        //设置项目管理对象，编辑  基本信息表  时更改  项目管理表  中基本信息表的状态，过滤的方式可以是name项目名称
                        xmgl.ccjx = "0";                                                //更改项目管理表中的  jpxx 为1
                        db.Entry(xmgl).State = EntityState.Modified;                   //更新项目管理表中表示基本信息表的字段，这种更新方式不知道对不对
                        db.SaveChanges();
                    }
                    transaction.Complete();
                }
                catch (Exception ex)
                {
                    return Json(new { errorMsg = ex.ToString() }, "text/html");
                }
                finally
                {
                    transaction.Dispose();
                }
            }
            return Json(new { success = true });
        }


        //获得项目绩效效率指标列表         这个方法在视图上好像还没有做分页处理，
        [HttpPost]
        public JsonResult getxmjx_xl(int xmglid)
        {
            int page = (Request.Form["page"] != null) ? Int32.Parse(Request.Form["page"]) : 1;

            int rows = (Request.Form["rows"] != null) ? Int32.Parse(Request.Form["rows"]) : 10;

            var jxxlmulu = db.xmjixiao_xls.Where(s => s.xiangmuguanliID == xmglid);
            var xm = from s in jxxlmulu
                     orderby s.xmjixiao_xlID
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


        //绩效效率表新增
        [HttpPost]
        public JsonResult jxxl_save(int xmglID)   //FormCollection form,
        {
            xmjixiao_xl jxxl = new xmjixiao_xl();

            //设置项目管理对象，编辑  基本信息表  时更改  项目管理表  中基本信息表的状态，过滤的方式可以是name项目名称
            xiangmuguanli xmgl = new xiangmuguanli();
            xmgl = db.xiangmuguanlis.Where(s => s.ID == xmglID).FirstOrDefault();   //选择第一项或者符合条件的项
            xmgl.xljx = "1";                                                //更改项目管理表中的  xljx 为1

            jxxl.xiangmuguanliID = xmglID;
            jxxl.jixiaozhibiao = Request.Form["jixiaozhibiao"];
            jxxl.jixiaomubiao = Request.Form["jixiaomubiao"];
            jxxl.jixiaoyou = Request.Form["jixiaoyou"];
            jxxl.jixiaoliang = Request.Form["jixiaoliang"];
            jxxl.jixiaozhong = Request.Form["jixiaozhong"];
            jxxl.jixiaocha = Request.Form["jixiaocha"];

            using (TransactionScope transaction = new TransactionScope())//原子操作，事物错误回滚
            {
                try
                {
                    db.Entry(xmgl).State = EntityState.Modified;                   //更新项目管理表中表示基本信息表的字段，这种更新方式不知道对不对
                    db.xmjixiao_xls.Add(jxxl);
                    db.SaveChanges();
                    transaction.Complete();
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, errorMsg = ex.ToString() }, "text/html");
                }
                finally
                {
                    transaction.Dispose();
                }
            }
            return Json(new { success = true, message = "保存成功，请完善其他信息！" }, "text/html");
        }



        //绩效效率表编辑
        [HttpPost]
        public JsonResult jxxl_edit(int xmjixiao_xlID)//FormCollection form,
        {
            xmjixiao_xl jxxl = new xmjixiao_xl();
            jxxl = db.xmjixiao_xls.Where(s => s.xmjixiao_xlID == xmjixiao_xlID).FirstOrDefault();   //选择第一项或者符合条件的项

            jxxl.jixiaozhibiao = Request.Form["jixiaozhibiao"];
            jxxl.jixiaomubiao = Request.Form["jixiaomubiao"];
            jxxl.jixiaoyou = Request.Form["jixiaoyou"];
            jxxl.jixiaoliang = Request.Form["jixiaoliang"];
            jxxl.jixiaozhong = Request.Form["jixiaozhong"];
            jxxl.jixiaocha = Request.Form["jixiaocha"];

            try
            {
                db.Entry(jxxl).State = EntityState.Modified;                   //更新项目管理表中表示基本信息表的字段，这种更新方式不知道对不对
                db.SaveChanges();
                return Json(new { success = true, message = "保存成功，请完善其他信息！" }, "text/html");
            }
            catch (Exception ex)
            {
                return Json(new { success = false, errorMsg = ex.ToString() }, "text/html");
            }
        }


        //项目绩效效率 删除
        [HttpPost]
        public JsonResult jxxl_del(int id, int xmglID)  //如果这里的int id可以直接去到，为什么还要用form  FormCollection form,
        {
            //这里判断对应于项目总表中的项目的 绩效产出表 的记录是多条还是1条，如果为1
            xmjixiao_xl cc = new xmjixiao_xl();
            var xl1 = db.xmjixiao_xls.Where(s => s.xiangmuguanliID == xmglID);
            int xl_count = xl1.Count();

            xiangmuguanli xmgl = new xiangmuguanli();
            xmgl = db.xiangmuguanlis.Where(s => s.ID == xmglID).FirstOrDefault();   //选择第一项或者符合条件的项

            using (TransactionScope transaction = new TransactionScope())//原子操作，事物错误回滚
            {
                try
                {
                    if (xl_count > 1)
                    {
                        xmjixiao_xl jxxl = db.xmjixiao_xls.Find(id);
                        db.xmjixiao_xls.Remove(jxxl);
                        db.SaveChanges();
                    }
                    else
                    {
                        xmjixiao_xl jxxl = db.xmjixiao_xls.Find(id);

                        db.xmjixiao_xls.Remove(jxxl);

                        //设置项目管理对象，编辑  基本信息表  时更改  项目管理表  中基本信息表的状态，过滤的方式可以是name项目名称

                        xmgl.xljx = "0";                                                //更改项目管理表中的  jpxx 为1
                        db.Entry(xmgl).State = EntityState.Modified;                   //更新项目管理表中表示基本信息表的字段，这种更新方式不知道对不对
                        db.SaveChanges();
                    }
                    transaction.Complete();
                }
                catch (Exception ex)
                {
                    return Json(new { errorMsg = ex.ToString() }, "text/html");
                }
                finally
                {
                    transaction.Dispose();
                }
            }
            return Json(new { success = true });
        }


        //获得测算明细信息记录getcsmx
        [HttpPost]
        public JsonResult getcsmx(int xmglid)
        {
            int page = (Request.Form["page"] != null) ? Int32.Parse(Request.Form["page"]) : 1;

            int rows = (Request.Form["rows"] != null) ? Int32.Parse(Request.Form["rows"]) : 10;

            var csmx = db.xmcsmxs.Where(s => s.xiangmuguanliID == xmglid);

            var xm = from s in csmx
                     orderby s.suoshuniandu
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


        //测算明细保存
        [HttpPost]
        public JsonResult csmx_save(int xmglID)//FormCollection form, 
        {
            xmcsmx csmx = new xmcsmx();

            csmx.xiangmuguanliID = xmglID;

            csmx.suoshuniandu = Request.Form["suoshuniandu"];
            csmx.xiangmumingxi = Request.Form["xiangmumingxi"];

            csmx.chengbenbiaozhun = Convert.ToDecimal(Request.Form["chengbenbiaozhun"]);

            csmx.gongzuoliang = Convert.ToDecimal(Request.Form["gongzuoliang"]);

            csmx.shenbaoshu = Convert.ToDecimal(csmx.chengbenbiaozhun * csmx.gongzuoliang);

            csmx.yijushuoming = Request.Form["yijushuoming"];

            csmx.gzl_danwei = Request.Form["gzl_danwei"];

            //设置项目管理对象，编辑  基本信息表  时更改  项目管理表  中基本信息表的状态，过滤的方式可以是name项目名称
            xiangmuguanli xmgl = new xiangmuguanli();
            xmgl = db.xiangmuguanlis.Where(s => s.ID == xmglID).FirstOrDefault();   //选择第一项或者符合条件的项
            xmgl.csmx = "1";                                                //更改项目管理表中的  xljx 为1
            using (TransactionScope transaction = new TransactionScope())//原子操作，事物错误回滚
            {
                try
                {
                    db.Entry(xmgl).State = EntityState.Modified;                   //更新项目管理表中表示基本信息表的字段，这种更新方式不知道对不对
                    db.xmcsmxs.Add(csmx);
                    db.SaveChanges();
                    transaction.Complete();
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, errorMsg = ex.ToString() }, "text/html");
                }
                finally
                {
                    transaction.Dispose();
                }
            }
            return Json(new { success = true, message = "保存成功，请完善其他信息！" }, "text/html");
        }


        //测算明细编辑
        [HttpPost]
        public JsonResult csmx_edit(int csmxID)
        {
            xmcsmx csmx = new xmcsmx();

            csmx = db.xmcsmxs.Where(s => s.xmcsmxID == csmxID).FirstOrDefault();   //选择第一项或者符合条件的项
            csmx.suoshuniandu = Request.Form["suoshuniandu"];
            csmx.xiangmumingxi = Request.Form["xiangmumingxi"];
            //csmx.zhichufenlei = Request.Form["zhichufenlei"];

            csmx.chengbenbiaozhun = Convert.ToDecimal(Request.Form["chengbenbiaozhun"]);

            csmx.gongzuoliang = Convert.ToDecimal(Request.Form["gongzuoliang"]);

            csmx.shenbaoshu = Convert.ToDecimal(csmx.chengbenbiaozhun * csmx.gongzuoliang);

            csmx.yijushuoming = Request.Form["yijushuoming"];
            csmx.gzl_danwei = Request.Form["gzl_danwei"];
            try
            {
                db.Entry(csmx).State = EntityState.Modified;                   //更新项目管理表中表示基本信息表的字段，这种更新方式不知道对不对
                db.SaveChanges();
                return Json(new { success = true, message = "保存成功，请完善其他信息！" }, "text/html");
            }
            catch (Exception ex)
            {
                return Json(new { success = false, errorMsg = ex.ToString() }, "text/html");
            }
        }

        //测算明细删除csmx_del
        [HttpPost]
        public JsonResult csmx_del(int id, int xmglID)  //如果这里的int id可以直接去到，为什么还要用form  FormCollection form,
        {        
            //这里判断测算明细表对应于总表 是多条记录还是一条记录
            xmcsmx csmx = new xmcsmx();
            var cs = db.xmcsmxs.Where(s => s.xiangmuguanliID == xmglID);
            int cs_count = cs.Count();

            xmcsmx csmx_sel = db.xmcsmxs.Find(id);

            //设置项目管理对象，编辑  基本信息表  时更改  项目管理表  中基本信息表的状态，过滤的方式可以是name项目名称
            xiangmuguanli xmgl = new xiangmuguanli();
            xmgl = db.xiangmuguanlis.Where(s => s.ID == xmglID).FirstOrDefault();   //选择第一项或者符合条件的项

            using (TransactionScope transaction = new TransactionScope())//原子操作，事物错误回滚
            {
                try
                {
                    if (cs_count > 1)
                    {
                        db.xmcsmxs.Remove(csmx_sel);
                        db.SaveChanges();
                    }
                    else
                    {
                        db.xmcsmxs.Remove(csmx_sel);
                        xmgl.csmx = "0";                                               //更改项目管理表中的  jpxx 为1
                        db.Entry(xmgl).State = EntityState.Modified;                   //更新项目管理表中表示基本信息表的字段，这种更新方式不知道对不对
                        db.SaveChanges();
                    }
                    transaction.Complete();
                }
                catch (Exception ex)
                {
                    return Json(new { errorMsg = ex.ToString() }, "text/html");
                }
                finally
                {
                    transaction.Dispose();
                }
            }
            return Json(new { success = true });
        }


        //获取csmx的申报数合计：int xmglid
        [HttpPost]
        public JsonResult getshenbaoshu_hj(int xmid)
        {
            try
            {
                var heji = db.xmcsmxs.Where(s => s.xiangmuguanliID == xmid).Select(x => x.shenbaoshu);
                if (heji.Any())
                {
                    return Json(new { success = true, Msg = heji.Sum() }, "text/html");
                }
                else
                {
                    return Json(new { success = true, Msg = "0" }, "text/html");
                }
            }
            catch (Exception)
            {
                return Json(new { success = false }, "text/html");
            }
        }

        private class niandu
        {
            public string id { get; set; }
            public string it { get; set; }

            public bool selected { get; set; }
        }


        [HttpPost]
        public JsonResult wsxx_niandu_list()
        {
            var GenreLst = new List<niandu>();
            int niandu = Convert.ToInt32(DateTime.Now.ToString("yyyy"));
            for (var i = 1; i <= 3; i++)
            {
                var nd = new niandu();
                nd.id = (niandu + i).ToString();
                nd.it = (niandu + i).ToString();
                if (i == 1)
                {
                    nd.selected = true;
                }
                GenreLst.Add(nd);
            }
            return Json(GenreLst.ToList(), "text/html");
        }


        //采购配置
        [HttpPost]
        public JsonResult getcgpz(int xmglid)
        {
            int page = (Request.Form["page"] != null) ? Int32.Parse(Request.Form["page"]) : 1;
            int rows = (Request.Form["rows"] != null) ? Int32.Parse(Request.Form["rows"]) : 10;

            var xm = db.xmcgpzs.Where(s => s.xiangmuguanliID == xmglid).OrderBy(s => s.suoshuniandu);
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
        public JsonResult cgpz_save(int xmglID)//FormCollection form,
        {
            xmcgpz cgpz = new xmcgpz();

            //设置项目管理对象，编辑  基本信息表  时更改  项目管理表  中基本信息表的状态，过滤的方式可以是name项目名称
            xiangmuguanli xmgl = new xiangmuguanli();
            xmgl = db.xiangmuguanlis.Where(s => s.ID == xmglID).FirstOrDefault();   //选择第一项或者符合条件的项
            xmgl.cgpz = "1";                                                //更改项目管理表中的  xljx 为1

            cgpz.xiangmuguanliID = xmglID;

            //cgpz.zichanfenlei = Request.Form["zichanfenlei"];

            cgpz.suoshuniandu = Request.Form["suoshuniandu"];

            cgpz.zichanmingcheng = Request.Form["zichanmingcheng"];

            cgpz.guigexinghao = Request.Form["guigexinghao"];

            cgpz.peizhishuliang = Convert.ToDecimal(Request.Form["peizhishuliang"]);

            cgpz.danjia = Convert.ToDecimal(Request.Form["danjia"]);

            cgpz.zichancunliang = Request.Form["zichancunliang"];
            cgpz.caigoushuoming = Request.Form["caigoushuoming"];

            using (TransactionScope transaction = new TransactionScope())//原子操作，事物错误回滚
            {
                try
                {
                    db.Entry(xmgl).State = EntityState.Modified;                   //更新项目管理表中表示基本信息表的字段，这种更新方式不知道对不对
                    db.xmcgpzs.Add(cgpz);
                    db.SaveChanges();
                    transaction.Complete();
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, errorMsg = ex.ToString() }, "text/html");
                }
                finally
                {
                    transaction.Dispose();
                }
            }
            return Json(new { success = true, message = "保存成功，请完善其他信息！" }, "text/html");
        }


        //采购配置编辑
        [HttpPost]
        public JsonResult cgpz_edit(int xmcgpzID)//FormCollection form, 
        {
            xmcgpz cgpz = new xmcgpz();

            //设置项目管理对象，编辑  基本信息表  时更改  项目管理表  中基本信息表的状态，过滤的方式可以是name项目名称
            //xiangmuguanli xmgl = new xiangmuguanli();
            cgpz = db.xmcgpzs.Where(s => s.xmcgpzID == xmcgpzID).FirstOrDefault();   //选择第一项或者符合条件的项

            cgpz.suoshuniandu = Request.Form["suoshuniandu"];

            cgpz.zichanmingcheng = Request.Form["zichanmingcheng"];

            cgpz.guigexinghao = Request.Form["guigexinghao"];

            cgpz.peizhishuliang = Convert.ToDecimal(Request.Form["peizhishuliang"]);
            cgpz.danjia = Convert.ToDecimal(Request.Form["danjia"]);

            cgpz.zichancunliang = Request.Form["zichancunliang"];
            cgpz.caigoushuoming = Request.Form["caigoushuoming"];

            try
            {
                //db.xmcgpzs.Add(cgpz);
                db.Entry(cgpz).State = EntityState.Modified;                   //更新项目管理表中表示基本信息表的字段，这种更新方式不知道对不对
                db.SaveChanges();
                return Json(new { success = true, message = "保存成功，请完善其他信息！" }, "text/html");
            }
            catch (Exception ex)
            {
                return Json(new { success = false, errorMsg = ex.ToString() }, "text/html");
            }
        }

       
        //采购配置删除
        [HttpPost]
        public JsonResult cgpz_del(int id, int xmglID)  //如果这里的int id可以直接去到，为什么还要用form   FormCollection form,
        {
            xmcgpz cgpz = new xmcgpz();
            var cgpz1 = db.xmcgpzs.Where(s => s.xiangmuguanliID == xmglID);
            int cgpz_count = cgpz1.Count();

            xmcgpz cgpz_sel = db.xmcgpzs.Find(id);

            //设置项目管理对象，编辑  基本信息表  时更改  项目管理表  中基本信息表的状态，过滤的方式可以是name项目名称
            xiangmuguanli xmgl = new xiangmuguanli();
            xmgl = db.xiangmuguanlis.Where(s => s.ID == xmglID).FirstOrDefault();   //选择第一项或者符合条件的项

            using (TransactionScope transaction = new TransactionScope())//原子操作，事物错误回滚
            {
                try
                {
                    if (cgpz_count > 1)
                    {
                        db.xmcgpzs.Remove(cgpz_sel);
                        db.SaveChanges();
                    }
                    else
                    {
                        db.xmcgpzs.Remove(cgpz_sel);
                        xmgl.cgpz = "0";                                               //更改项目管理表中的  jpxx 为1
                        db.Entry(xmgl).State = EntityState.Modified;                   //更新项目管理表中表示基本信息表的字段，这种更新方式不知道对不对
                        db.SaveChanges();
                    }
                    transaction.Complete();
                }
                catch (Exception ex)
                {
                    return Json(new { errorMsg = ex.ToString() }, "text/html");
                }
                finally
                {
                    transaction.Dispose();
                }
            }
            return Json(new { success = true });
        }


        //获取采购配置cgpz的金额合计：int xmglid
        [HttpPost]
        public JsonResult getcgpzjine_hj(int xmid)
        {
            try
            {
                var heji = from a in db.xmcgpzs
                           where a.xiangmuguanliID == xmid
                           select new
                           {
                               jine = a.peizhishuliang * a.danjia,
                           };
                if (heji.Any())
                {
                    return Json(new { success = true, Msg = heji.Sum(x => x.jine) }, "text/html");
                }
                else
                {
                    return Json(new { success = true, Msg = "0" }, "text/html");
                }
            }
            catch (Exception)
            {
                return Json(new { success = false }, "text/html");
            }
        }


        //采购配置表 空表填报 按钮的处理
        [HttpPost]
        public JsonResult cgpz_kong_tb(int xmglID)  //如果这里的int id可以直接去到，为什么还要用form   FormCollection form,
        {
            xmcgpz cgpz = new xmcgpz();
            var cgpz1 = db.xmcgpzs.Where(s => s.xiangmuguanliID == xmglID);

            //设置项目管理对象，编辑  基本信息表  时更改  项目管理表  中基本信息表的状态，过滤的方式可以是name项目名称
            xiangmuguanli xmgl = new xiangmuguanli();

            xmgl = db.xiangmuguanlis.Where(s => s.ID == xmglID).FirstOrDefault();   //选择第一项或者符合条件的项
            
            using (TransactionScope transaction = new TransactionScope())//原子操作，事物错误回滚
            {
                try
                {
                    if (cgpz1.Any())
                    {
                        //db.xmcgpzs.Remove(cgpz1);
                        db.xmcgpzs.RemoveRange(cgpz1);

                        xmgl.cgpz = "1";
                        xmgl.cgpz_nll = true;
                        db.Entry(xmgl).State = EntityState.Modified;

                        db.SaveChanges();
                    }
                    else
                    {

                        xmgl.cgpz = "1";                                               //更改项目管理表中的  jpxx 为1
                        xmgl.cgpz_nll = true;
                        db.Entry(xmgl).State = EntityState.Modified;                //更新项目管理表中表示基本信息表的字段，这种更新方式不知道对不对

                        db.SaveChanges();
                    }
                    transaction.Complete();

                }
                catch (Exception ex)
                {
                    return Json(new { errorMsg = ex.ToString() }, "text/html");
                }
                finally
                {
                    transaction.Dispose();
                }
            }
            return Json(new { success = true });
        }


        //采购配置表  非空表填报 按钮的处理
        [HttpPost]
        public JsonResult cgpz_feikong_tb(int xmglID)  //如果这里的int id可以直接去到，为什么还要用form   FormCollection form,
        {
            //设置项目管理对象，编辑  基本信息表  时更改  项目管理表  中基本信息表的状态，过滤的方式可以是name项目名称
            xiangmuguanli xmgl = new xiangmuguanli();

            xmgl = db.xiangmuguanlis.Where(s => s.ID == xmglID).FirstOrDefault();   //选择第一项或者符合条件的项

            using (TransactionScope transaction = new TransactionScope())//原子操作，事物错误回滚
            {
                try
                {
                    xmgl.cgpz = "0";                                               //更改项目管理表中的  jpxx 为1
                    xmgl.cgpz_nll = false;
                    db.Entry(xmgl).State = EntityState.Modified;                //更新项目管理表中表示基本信息表的字段，这种更新方式不知道对不对

                    db.SaveChanges();
                    transaction.Complete();
                }
                catch (Exception ex)
                {
                    return Json(new { errorMsg = ex.ToString() }, "text/html");
                }
                finally
                {
                    transaction.Dispose();
                }
            }
            return Json(new { success = true });
        }


        //在完善采购配置计划表时，点击按钮  执行无采购配置计划命令
        [HttpPost]
        public JsonResult cgpz_null(int id)  //如果这里的int id可以直接去到，为什么还要用form   FormCollection form,
        {

            //设置项目管理对象，编辑  基本信息表  时更改  项目管理表  中基本信息表的状态，过滤的方式可以是name项目名称
            xiangmuguanli xmgl = new xiangmuguanli();
            xmgl = db.xiangmuguanlis.Where(s => s.ID == id).FirstOrDefault();   //选择第一项或者符合条件的项
            xmgl.cgpz = "1";

            using (TransactionScope transaction = new TransactionScope())//原子操作，事物错误回滚
            {
                try
                {
                    db.Entry(xmgl).State = EntityState.Modified;                   //更新项目管理表中表示基本信息表的字段，这种更新方式不知道对不对
                    db.SaveChanges();
                    transaction.Complete();
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, errorMsg = ex.ToString() }, "text/html");
                }
                finally
                {
                    transaction.Dispose();
                }
            }
            return Json(new { success = true, errorMsg = "保存成功！" });
        }


        //采购配置电子表格导入
        [HttpPost]
        [Authorize(Roles = "部门负责人,员工")]
        public JsonResult excelImport(FormCollection form, int xmglID, HttpPostedFileBase upFileBase)     // 
        {
            ViewBag.error = "";
            HttpPostedFileBase fileBase = Request.Files["files"];

            if (fileBase == null || fileBase.ContentLength <= 0)
            {
                ViewBag.error = "文件不能为空";

                return Json(new { success = false, errorMsg = ViewBag.error }, "text/html");
            }

            string filename = Path.GetFileName(fileBase.FileName);    //获得文件全名
            //int filesize = file.ContentLength;//获取上传文件的大小单位为字节byte
            string fileEx = System.IO.Path.GetExtension(filename);     //获取上传文件的扩展名
            string NoFileName = System.IO.Path.GetFileNameWithoutExtension(filename);   //获取无扩展名的文件名

            string FileName = NoFileName + DateTime.Now.ToString("yyyyMMddhhmmss") + fileEx;   //这里是一个 文件名+时间+扩展名  的新文件名，作为保存在服务器中的  文件名

            string path = AppDomain.CurrentDomain.BaseDirectory + "content/uploads/excel/";  // 这里是文件的造服务器上保存的路径;
            string savePath = Path.Combine(path, FileName);
            fileBase.SaveAs(savePath);

            Workbook BuildReport_WorkBook = new Workbook();
            BuildReport_WorkBook.Open(savePath);//fileFullName

            Worksheets sheets = BuildReport_WorkBook.Worksheets;

            //试题表
            Worksheet workSheetQuestion = BuildReport_WorkBook.Worksheets["Sheet1"];   //  sheet1
            Cells cellsQuestion = workSheetQuestion.Cells;    //单元格

            //引用事务机制，出错时，事物回滚
            using (TransactionScope transaction = new TransactionScope())
            {
                //设置项目管理对象，编辑  基本信息表  时更改  项目管理表  中基本信息表的状态，过滤的方式可以是name项目名称
                xiangmuguanli xmgl = new xiangmuguanli();
                xmgl = db.xiangmuguanlis.Where(s => s.ID == xmglID).FirstOrDefault();   //选择第一项或者符合条件的项
                xmgl.cgpz = "1";                                                //更改项目管理表中的  jpxx 为1
                db.Entry(xmgl).State = EntityState.Modified;                   //更新项目管理表中表示基本信息表的字段，这种更新方式不知道对不对
                db.SaveChanges();

                if (form["gengxinmoshi"].ToString() == "fugai")
                {
                    string ConString = System.Configuration.ConfigurationManager.ConnectionStrings["cwcxmkContent"].ConnectionString;
                    SqlConnection con = new SqlConnection(ConString);

                    string sqldel = "delete from xmcgpz where xiangmuguanliID=" + xmglID.ToString();  //这个筛选方式写法是否正确待检验
                    int j;    //提示报错的行
                    try
                    {
                        con.Open();
                        SqlCommand sqlcmddel = new SqlCommand(sqldel, con);
                        sqlcmddel.ExecuteNonQuery();   //删除了现有的名单

                        //试题表 
                        for (int i = 1; i < cellsQuestion.MaxDataRow + 1; i++)
                        {
                            j = i + 1;
                            try
                            {
                                xmcgpz cgpz = new xmcgpz();

                                if (cellsQuestion[i, 0].StringValue == null || cellsQuestion[i, 0].StringValue == "")
                                {
                                    string error = "第" + j + "行记录插入有误，请认真检查格式后再导入！“年度”列为不能为空，建议使用模板上传！";
                                    return Json(new { success = false, errorMsg = error }, "text/html");
                                }
                                else
                                {
                                    int niandu = Convert.ToInt32(DateTime.Now.ToString("yyyy"));
                                    int[] nds = { niandu + 1, niandu + 2, niandu + 3 };
                                    int id = Array.IndexOf(nds, Convert.ToInt32(cellsQuestion[i, 0].StringValue));

                                    if (id == -1)
                                    {
                                        string error = "第" + j + "行记录插入有误，请参照模板认真检查格式后再导入！“年度”值不在范围之内";
                                        return Json(new { success = false, errorMsg = error }, "text/html");
                                    }
                                    else
                                    {
                                        cgpz.suoshuniandu = cellsQuestion[i, 0].StringValue;
                                    }
                                }

                                if (cellsQuestion[i, 1].StringValue == null || cellsQuestion[i, 1].StringValue == "")
                                {
                                    //cgpz.zichanmingcheng = "";
                                    string error = "第" + j + "行记录插入有误，请认真检查格式后再导入！“资产名称”列为不能为空，建议使用模板上传！";

                                    return Json(new { success = false, errorMsg = error }, "text/html");
                                }
                                else
                                {
                                    cgpz.zichanmingcheng = cellsQuestion[i, 1].StringValue;
                                }

                                if (cellsQuestion[i, 2].StringValue == null || cellsQuestion[i, 2].StringValue == "")
                                {
                                    cgpz.guigexinghao = "";
                                }
                                else
                                {
                                    cgpz.guigexinghao = cellsQuestion[i, 2].StringValue;
                                }

                                if (cellsQuestion[i, 3].StringValue == "")    //cellsQuestion[i, 2].IntValue == null ||  ,int的类型永不等于null
                                {
                                    string error = "第" + j + "行记录插入有误，请认真检查格式后再导入！“配置数量”列为不能为空，建议使用模板上传！";

                                    return Json(new { success = false, errorMsg = error }, "text/html");

                                    //cgpz.peizhishuliang = Convert.ToInt32("");
                                }
                                else
                                {
                                    cgpz.peizhishuliang = Convert.ToDecimal(cellsQuestion[i, 3].StringValue);
                                }

                                if (cellsQuestion[i, 4].StringValue == "")//cellsQuestion[i, 3].IntValue == null ||   ，int的类型永不等于null
                                {
                                    string error = "第" + j + "行记录插入有误，请认真检查格式后再导入！“单价”列为不能为空，建议使用模板上传！";

                                    return Json(new { success = false, errorMsg = error }, "text/html");
                                }
                                else
                                {
                                    cgpz.danjia = Convert.ToDecimal(cellsQuestion[i, 4].StringValue);
                                }

                                if (cellsQuestion[i, 5].StringValue == null || cellsQuestion[i, 5].StringValue == "")
                                {
                                    cgpz.zichancunliang = "";
                                }
                                else
                                {
                                    cgpz.zichancunliang = cellsQuestion[i, 5].StringValue;
                                }

                                if (cellsQuestion[i, 6].StringValue == null || cellsQuestion[i, 6].StringValue == "")
                                {
                                    cgpz.caigoushuoming = "";
                                }
                                else
                                {
                                    cgpz.caigoushuoming = cellsQuestion[i, 6].StringValue;
                                }
                                
                                cgpz.xiangmuguanliID = xmglID;

                                //数据库操作
                                db.xmcgpzs.Add(cgpz);
                            }

                            catch (Exception ex)
                            {
                                string error = "第" + j + "行记录插入有误，请认真检查格式后再导入！“配置数量”、“单价”列为数值格式，建议使用模板上传！";

                                string ex_str = ex.ToString();

                                return Json(new { success = false, errorMsg = error }, "text/html");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        string error = ex.Message;
                        return Json(new { success = false, errorMsg = error }, "text/html");
                    }
                    finally
                    {
                        con.Close(); //无论如何都要执行的语句。
                    }
                    db.SaveChanges();
                }
                else  //追加模式
                {
                    int j;    //提示报错的行
                    try
                    {
                        //试题表 
                        for (int i = 1; i < cellsQuestion.MaxRow + 1; i++)
                        {
                            j = i + 1;
                            try
                            {
                                xmcgpz cgpz = new xmcgpz();
                                if (cellsQuestion[i, 0].StringValue == null || cellsQuestion[i, 0].StringValue == "")
                                {
                                    string error = "第" + j + "行记录插入有误，请认真检查格式后再导入！“年度”列为不能为空，建议使用模板上传！";
                                    return Json(new { success = false, errorMsg = error }, "text/html");
                                }
                                else
                                {
                                    int niandu = Convert.ToInt32(DateTime.Now.ToString("yyyy"));
                                    int[] nds = { niandu + 1, niandu + 2, niandu + 3 };
                                    int id = Array.IndexOf(nds, Convert.ToInt32(cellsQuestion[i, 0].StringValue));
                                    if (id == -1)
                                    {
                                        string error = "第" + j + "行记录插入有误，请参照模板认真检查格式后再导入！“年度”值不在范围之内";
                                        return Json(new { success = false, errorMsg = error }, "text/html");
                                    }
                                    else
                                    {
                                        cgpz.suoshuniandu = cellsQuestion[i, 0].StringValue;
                                    }
                                }
                                if (cellsQuestion[i, 1].StringValue == null || cellsQuestion[i, 1].StringValue == "")
                                {
                                    string error = "第" + j + "行记录插入有误，请认真检查格式后再导入！“资产名称”列为不能为空，建议使用模板上传！";
                                    return Json(new { success = false, errorMsg = error }, "text/html");
                                }
                                else
                                {
                                    cgpz.zichanmingcheng = cellsQuestion[i, 1].StringValue;
                                }
                                if (cellsQuestion[i, 2].StringValue == null || cellsQuestion[i, 2].StringValue == "")
                                {
                                    cgpz.guigexinghao = "";
                                }
                                else
                                {
                                    cgpz.guigexinghao = cellsQuestion[i, 2].StringValue;
                                }
                                if (cellsQuestion[i, 3].StringValue == "")    //cellsQuestion[i, 2].IntValue == null ||  ,int的类型永不等于null
                                {
                                    string error = "第" + j + "行记录插入有误，请认真检查格式后再导入！“配置数量”列为不能为空，建议使用模板上传！";
                                    return Json(new { success = false, errorMsg = error }, "text/html");
                                }
                                else
                                {
                                    cgpz.peizhishuliang = Convert.ToDecimal(cellsQuestion[i, 3].StringValue);
                                }
                                if (cellsQuestion[i, 4].StringValue == "")//cellsQuestion[i, 3].IntValue == null ||   ，int的类型永不等于null
                                {
                                    string error = "第" + j + "行记录插入有误，请认真检查格式后再导入！“单价”列为不能为空，建议使用模板上传！";
                                    return Json(new { success = false, errorMsg = error }, "text/html");
                                }
                                else
                                {
                                    cgpz.danjia = Convert.ToDecimal(cellsQuestion[i, 4].StringValue);
                                }
                                if (cellsQuestion[i, 5].StringValue == null || cellsQuestion[i, 5].StringValue == "")
                                {
                                    cgpz.zichancunliang = "";
                                }
                                else
                                {
                                    cgpz.zichancunliang = cellsQuestion[i, 5].StringValue;
                                }
                                if (cellsQuestion[i, 6].StringValue == null || cellsQuestion[i, 6].StringValue == "")
                                {
                                    cgpz.caigoushuoming = "";
                                }
                                else
                                {
                                    cgpz.caigoushuoming = cellsQuestion[i, 6].StringValue;
                                }
                                cgpz.xiangmuguanliID = xmglID;
                                //数据库操作
                                db.xmcgpzs.Add(cgpz);
                            }
                            catch (Exception ex)
                            {
                                string error = "第" + j + "行记录插入有误，请认真检查格式后再导入！“配置数量”、“单价”列为数值格式，建议使用模板上传！";
                                string ex_str = ex.ToString();
                                return Json(new { success = false, errorMsg = error }, "text/html");
                            }
                        }
                        db.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        string error = ex.Message;
                        return Json(new { success = false, errorMsg = error }, "text/html");
                    }
                }
                transaction.Complete();
            }
            //string error1 = "祝贺您，本次信息导入成功！";
            //System.Threading.Thread.Sleep(1000);
            return Json(new { success = true, errorMsg = "祝贺您，本次信息导入成功！" }, "text/html");
        }


        //采购配置电子表格导入，使用 Microsoft.Ace.OleDb.12.0;  的电子表格导入
        [HttpPost]
        [Authorize(Roles = "部门负责人,员工")]
        public JsonResult excelImport_old(FormCollection form, int xmglID)
        {
            ViewBag.error = "";
            HttpPostedFileBase file = Request.Files["files"];
            string FileName;
            string savePath;
            if (file == null || file.ContentLength <= 0)
            {
                ViewBag.error = "文件不能为空";
                return Json(new { success = false, errorMsg = ViewBag.error }, "text/html");
            }
            else
            {
                string filename = Path.GetFileName(file.FileName);
                int filesize = file.ContentLength;//获取上传文件的大小单位为字节byte
                string fileEx = System.IO.Path.GetExtension(filename);//获取上传文件的扩展名
                string NoFileName = System.IO.Path.GetFileNameWithoutExtension(filename);//获取无扩展名的文件名

                //int Maxsize = 4000 * 1024;//定义上传文件的最大空间大小为4M

                //string FileType = ".xls,.xlsx";   //定义上传文件的类型字符串

                FileName = NoFileName + DateTime.Now.ToString("yyyyMMddhhmmss") + fileEx;
                //if (!FileType.Contains(fileEx))
                //{
                //    ViewBag.error = "文件类型不对，只能导入xls和xlsx格式的文件";
                //    //return View();
                //    //return Json(new { errorMsg = ViewBag.error }, "text/html");
                //    return Json(new {success=false, errorMsg = ViewBag.error }, "text/html");
                //}
                string path = AppDomain.CurrentDomain.BaseDirectory + "content/uploads/excel/";
                savePath = Path.Combine(path, FileName);
                file.SaveAs(savePath);
            }

            //string result = string.Empty;
            string strConn;
            //office 2007 可用 导入xls 不用于.xlsx
            //strConn = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + savePath + ";" + "Extended Properties=Excel 8.0";

            strConn = "Provider=Microsoft.Ace.OleDb.12.0;Data Source=" + savePath + ";" + "Extended Properties='Excel 12.0; HDR=Yes; IMEX=1'";
            OleDbConnection conn = new OleDbConnection(strConn);
            OleDbDataAdapter myCommand = new OleDbDataAdapter("select * from [Sheet1$]", strConn);
            DataSet myDataSet = new DataSet();
            try
            {
                conn.Open();
                myCommand.Fill(myDataSet, "ExcelInfo");
            }
            catch (Exception ex)
            {
                ViewBag.error = ex.Message;
                return Json(new { success = false, errorMsg = ViewBag.error }, "text/html");
            }
            finally
            {
                conn.Close(); //无论如何都要执行的语句。
            }
            DataTable ds = myDataSet.Tables["ExcelInfo"].DefaultView.ToTable();
            DataRow[] dr = ds.Select();

            //引用事务机制，出错时，事物回滚
            using (TransactionScope transaction = new TransactionScope())
            {
                //设置项目管理对象，编辑  基本信息表  时更改  项目管理表  中基本信息表的状态，过滤的方式可以是name项目名称
                xiangmuguanli xmgl = new xiangmuguanli();
                xmgl = db.xiangmuguanlis.Where(s => s.ID == xmglID).FirstOrDefault();   //选择第一项或者符合条件的项
                xmgl.cgpz = "1";                                                //更改项目管理表中的  jpxx 为1
                db.Entry(xmgl).State = EntityState.Modified;                   //更新项目管理表中表示基本信息表的字段，这种更新方式不知道对不对
                db.SaveChanges();
                if (form["gengxinmoshi"].ToString() == "fugai")
                {
                    string ConString = System.Configuration.ConfigurationManager.ConnectionStrings["cwcxmkContent"].ConnectionString;
                    SqlConnection con = new SqlConnection(ConString);
                    string sqldel = "delete from xmcgpz where xiangmuguanliID=" + xmglID.ToString();   //这个筛选方式写法是否正确待检验
                    try
                    {
                        con.Open();
                        SqlCommand sqlcmddel = new SqlCommand(sqldel, con);
                        sqlcmddel.ExecuteNonQuery();   //删除了现有的名单
                        int j;
                        for (int i = 0; i < dr.Length; i++)
                        {
                            j = i + 1;
                            int xiangmuguanliID = xmglID;
                            string zichanmingcheng = dr[i]["资产名称"].ToString();
                            string guigexinghao = dr[i]["规格型号"].ToString();
                            int peizhishuliang = Convert.ToInt32(dr[i]["配置数量"].ToString());         //这里可能需要额外注意一下
                            Decimal danjia;
                            if (!Decimal.TryParse(dr[i]["单价"].ToString(), out danjia))
                            {
                                ViewBag.error = "第" + j + "条记录的单价格式可能有误，请认真检查后再导入！";
                                return Json(new { success = false, errorMsg = ViewBag.error }, "text/html");
                            }
                            string zichancunliang = dr[i]["资产存量情况"].ToString();
                            string caigoushuoming = dr[i]["采购说明和配置资产申请理由"].ToString();
                            string insertstr = "insert into xmcgpz (xiangmuguanliID, zichanmingcheng, guigexinghao, peizhishuliang ,danjia, zichancunliang, caigoushuoming) values ('" + xiangmuguanliID + "','" + zichanmingcheng + "','" + guigexinghao + "','" + peizhishuliang + "','" + danjia + "','" + zichancunliang + "','" + caigoushuoming + "')";
                            SqlCommand cmd = new SqlCommand(insertstr, con);
                            try
                            {
                                cmd.ExecuteNonQuery();
                            }

                            catch (Exception ex)
                            {
                                ViewBag.error = "第" + j + "条记录插入有误，请认真检查格式后再导入！覆盖";
                                string ex_str = ex.ToString();
                                return Json(new { success = false, errorMsg = ViewBag.error }, "text/html");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        ViewBag.error = ex.Message;
                        return Json(new { success = false, errorMsg = ViewBag.error }, "text/html");
                    }
                    finally
                    {
                        con.Close(); //无论如何都要执行的语句。
                    }
                }
                else  //追加模式
                {
                    string ConString = System.Configuration.ConfigurationManager.ConnectionStrings["cwcxmkContent"].ConnectionString;
                    SqlConnection con = new SqlConnection(ConString);
                    try
                    {
                        con.Open();
                        int j;
                        for (int i = 0; i < dr.Length; i++)
                        {
                            j = i + 1;
                            int xiangmuguanliID = xmglID;
                            string zichanmingcheng = dr[i]["资产名称"].ToString();
                            string guigexinghao = dr[i]["规格型号"].ToString();
                            int peizhishuliang = Convert.ToInt32(dr[i]["配置数量"].ToString());         //这里可能需要额外注意一下
                            Decimal danjia;
                            if (!Decimal.TryParse(dr[i]["单价"].ToString(), out danjia))
                            {
                                ViewBag.error = "第" + j + "条记录的单价格式可能有误，请认真检查后再导入！";
                                return Json(new { success = false, errorMsg = ViewBag.error }, "text/html");
                            }
                            string zichancunliang = dr[i]["资产存量情况"].ToString();

                            string caigoushuoming = dr[i]["采购说明和配置资产申请理由"].ToString();
                            string insertstr = "insert into xmcgpz (xiangmuguanliID, zichanmingcheng, guigexinghao, peizhishuliang ,danjia, zichancunliang, caigoushuoming) values ('" + xiangmuguanliID + "','" + zichanmingcheng + "','" + guigexinghao + "','" + peizhishuliang + "','" + danjia + "','" + zichancunliang + "','" + caigoushuoming + "')";
                            SqlCommand cmd = new SqlCommand(insertstr, con);
                            try
                            {
                                cmd.ExecuteNonQuery();
                            }

                            catch (Exception ex)
                            {
                                ViewBag.error = "第" + j + "条记录插入有误，请认真检查格式后再导入！追加";
                                string ex_str = ex.ToString();
                                return Json(new { success = false, errorMsg = ViewBag.error }, "text/html");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        ViewBag.error = ex.Message;
                        return Json(new { success = false, errorMsg = ViewBag.error }, "text/html");
                    }
                    finally
                    {
                        con.Close(); //无论如何都要执行的语句。
                    }
                }
                transaction.Complete();
            }
            ViewBag.error = "祝贺您，本次信息导入成功！";
            System.Threading.Thread.Sleep(2000);
            return Json(new { success = true, errorMsg = ViewBag.error }, "text/html");
        }


        //测算明细电子表格导入  
        [HttpPost]
        [Authorize(Roles = "部门负责人,员工")]
        public JsonResult csmx_excelImport(FormCollection form, int xmglID, HttpPostedFileBase upFileBase)     // 
        {
            string error = "";

            HttpPostedFileBase fileBase = Request.Files["files"];

            if (fileBase == null || fileBase.ContentLength <= 0)
            {
                return Json(new { success = false, errorMsg = "文件不能为空" }, "text/html");
            }
            string filename = Path.GetFileName(fileBase.FileName);    //获得文件全名
            //int filesize = file.ContentLength;//获取上传文件的大小单位为字节byte
            string fileEx = System.IO.Path.GetExtension(filename);     //获取上传文件的扩展名
            string NoFileName = System.IO.Path.GetFileNameWithoutExtension(filename);   //获取无扩展名的文件名

            string FileName = NoFileName + DateTime.Now.ToString("yyyyMMddhhmmss") + fileEx;   //这里是一个 文件名+时间+扩展名  的新文件名，作为保存在服务器中的  文件名

            string path = AppDomain.CurrentDomain.BaseDirectory + "content/uploads/excel/";  // 这里获得的是文件的物理路径;
            string savePath = Path.Combine(path, FileName);
            fileBase.SaveAs(savePath);

            Workbook BuildReport_WorkBook = new Workbook();
            BuildReport_WorkBook.Open(savePath);//fileFullName

            Worksheets sheets = BuildReport_WorkBook.Worksheets;

            //试题表
            Worksheet workSheetQuestion = BuildReport_WorkBook.Worksheets["Sheet1"];   //  sheet1
            Cells cellsQuestion = workSheetQuestion.Cells;    //单元格

            //引用事务机制，出错时，事物回滚
            using (TransactionScope transaction = new TransactionScope())
            {
                //设置项目管理对象，编辑  基本信息表  时更改  项目管理表  中基本信息表的状态，过滤的方式可以是name项目名称
                xiangmuguanli xmgl = new xiangmuguanli();
                xmgl = db.xiangmuguanlis.Where(s => s.ID == xmglID).FirstOrDefault();   //选择第一项或者符合条件的项
                xmgl.csmx = "1";                                                //更改项目管理表中的  jpxx 为1
                db.Entry(xmgl).State = EntityState.Modified;                   //更新项目管理表中表示基本信息表的字段，这种更新方式不知道对不对
                db.SaveChanges();
                if (form["gengxinmoshi"].ToString() == "fugai")
                {
                    string ConString = System.Configuration.ConfigurationManager.ConnectionStrings["cwcxmkContent"].ConnectionString;
                    SqlConnection con = new SqlConnection(ConString);

                    string sqldel = "delete from xmcsmx where xiangmuguanliID=" + xmglID.ToString();   //这个筛选方式写法是否正确待检验
                    int j;    //提示报错的行
                    try
                    {
                        con.Open();
                        SqlCommand sqlcmddel = new SqlCommand(sqldel, con);
                        sqlcmddel.ExecuteNonQuery();   //删除了现有的名单

                        //试题表 
                        for (int i = 1; i < cellsQuestion.MaxDataRow + 1; i++)
                        {
                            j = i + 1;
                            try
                            {
                                xmcsmx csmx = new xmcsmx();
                                if (cellsQuestion[i, 0].StringValue == null || cellsQuestion[i, 0].StringValue == "")
                                {
                                    error = "第" + j + "行记录插入有误，请认真检查格式后再导入！“年度”列为不能为空，建议使用模板上传！";
                                    return Json(new { success = false, errorMsg = error }, "text/html");
                                }
                                else
                                {
                                    int niandu = Convert.ToInt32(DateTime.Now.ToString("yyyy"));
                                    int[] nds = { niandu + 1, niandu + 2, niandu + 3 };
                                    int id = Array.IndexOf(nds, Convert.ToInt32(cellsQuestion[i, 0].StringValue));

                                    if (id == -1)
                                    {
                                        error = "第" + j + "行记录插入有误，请参照模板认真检查格式后再导入！“年度”值不在范围之内";
                                        return Json(new { success = false, errorMsg = error }, "text/html");
                                    }
                                    else
                                    {
                                        csmx.suoshuniandu = cellsQuestion[i, 0].StringValue;
                                    }
                                }
                                if (cellsQuestion[i, 1].StringValue == null || cellsQuestion[i, 1].StringValue == "")
                                {
                                    error = "第" + j + "行记录插入有误，请认真检查格式后再导入！“项目任务明细”列不能为空。";
                                    return Json(new { success = false, errorMsg = error }, "text/html");
                                }
                                else
                                {
                                    csmx.xiangmumingxi = cellsQuestion[i, 1].StringValue;
                                }

                                if (cellsQuestion[i, 2].StringValue == null || cellsQuestion[i, 2].StringValue == "")
                                {
                                    error = "第" + j + "行记录插入有误，请认真检查格式后再导入！“单位成本”列不能为空。";
                                    return Json(new { success = false, errorMsg = error }, "text/html");
                                }
                                else
                                {
                                    csmx.chengbenbiaozhun = Convert.ToDecimal(cellsQuestion[i, 2].StringValue);
                                }
                                if (cellsQuestion[i, 3].StringValue == "")    //cellsQuestion[i, 2].IntValue == null ||  ,int的类型永不等于null
                                {
                                    error = "第" + j + "行记录插入有误，请认真检查格式后再导入！“工作量”列不能为空。";
                                    return Json(new { success = false, errorMsg = error }, "text/html");
                                }
                                else
                                {
                                    csmx.gongzuoliang = Convert.ToDecimal(cellsQuestion[i, 3].StringValue);
                                }

                                if (cellsQuestion[i, 4].StringValue == "")//cellsQuestion[i, 3].IntValue == null ||   ，int的类型永不等于null
                                {
                                    error = "第" + j + "行记录插入有误，请认真检查格式后再导入！“单位”列不能为空。";
                                    return Json(new { success = false, errorMsg = error }, "text/html");
                                }
                                else
                                {
                                    csmx.gzl_danwei = cellsQuestion[i, 4].StringValue;
                                }
                                if (cellsQuestion[i, 5].StringValue == null || cellsQuestion[i, 5].StringValue == "")
                                {
                                    csmx.yijushuoming = "";
                                }
                                else
                                {
                                    csmx.yijushuoming = cellsQuestion[i, 5].StringValue;
                                }
                                csmx.shenbaoshu = csmx.gongzuoliang * csmx.chengbenbiaozhun;
                                csmx.xiangmuguanliID = xmglID;
                                //数据库操作
                                db.xmcsmxs.Add(csmx);
                            }
                            //db.SaveChanges();放在这里有问题
                            catch (Exception ex)
                            {
                                error = "第" + j + "行记录插入有误，请认真检查格式后再导入！建议使用模板上传！追加";
                                string ex_str = ex.ToString();
                                return Json(new { success = false, errorMsg = error }, "text/html");
                            }
                        }
                        //db.SaveChanges();      //db.SaveChanges();放在这里有问题
                    }
                    catch (Exception ex)
                    {
                        error = ex.Message;
                        return Json(new { success = false, errorMsg = error }, "text/html");
                    }
                    finally
                    {
                        con.Close(); //无论如何都要执行的语句。
                    }

                    db.SaveChanges();
                }
                else  //追加模式
                {
                    int j;    //提示报错的行
                    try
                    {
                        //试题表 
                        for (int i = 1; i < cellsQuestion.MaxRow + 1; i++)
                        {
                            j = i + 1;
                            try
                            {
                                xmcsmx csmx = new xmcsmx();
                                if (cellsQuestion[i, 0].StringValue == null || cellsQuestion[i, 0].StringValue == "")
                                {
                                    error = "第" + j + "行记录插入有误，请认真检查格式后再导入！“年度”列为不能为空，建议使用模板上传！";
                                    return Json(new { success = false, errorMsg = error }, "text/html");
                                }
                                else
                                {
                                    int niandu = Convert.ToInt32(DateTime.Now.ToString("yyyy"));
                                    int[] nds = { niandu + 1, niandu + 2, niandu + 3 };
                                    int id = Array.IndexOf(nds, Convert.ToInt32(cellsQuestion[i, 0].StringValue));
                                    if (id == -1)
                                    {
                                        error = "第" + j + "行记录插入有误，请参照模板认真检查格式后再导入！“年度”值不在范围之内";
                                        return Json(new { success = false, errorMsg = error }, "text/html");
                                    }
                                    else
                                    {
                                        csmx.suoshuniandu = cellsQuestion[i, 0].StringValue;
                                    }
                                }

                                if (cellsQuestion[i, 1].StringValue == null || cellsQuestion[i, 1].StringValue == "")
                                {
                                    error = "第" + j + "行记录插入有误，请认真检查格式后再导入！“项目任务明细”列不能为空。";
                                    return Json(new { success = false, errorMsg = error }, "text/html");
                                }
                                else
                                {
                                    csmx.xiangmumingxi = cellsQuestion[i, 1].StringValue;
                                }

                                if (cellsQuestion[i, 2].StringValue == null || cellsQuestion[i, 2].StringValue == "")
                                {
                                    error = "第" + j + "行记录插入有误，请认真检查格式后再导入！“单位成本”列不能为空。";
                                    return Json(new { success = false, errorMsg = error }, "text/html");
                                }
                                else
                                {
                                    csmx.chengbenbiaozhun = Convert.ToDecimal(cellsQuestion[i, 2].StringValue);
                                }

                                if (cellsQuestion[i, 3].StringValue == "")    //cellsQuestion[i, 2].IntValue == null ||  ,int的类型永不等于null
                                {
                                    error = "第" + j + "行记录插入有误，请认真检查格式后再导入！“工作量”列不能为空。";
                                    return Json(new { success = false, errorMsg = error }, "text/html");
                                }
                                else
                                {
                                    csmx.gongzuoliang = Convert.ToDecimal(cellsQuestion[i, 3].StringValue);
                                }

                                if (cellsQuestion[i, 4].StringValue == "")//cellsQuestion[i, 3].IntValue == null ||   ，int的类型永不等于null
                                {
                                    error = "第" + j + "行记录插入有误，请认真检查格式后再导入！“单位”列不能为空。";
                                    return Json(new { success = false, errorMsg = error }, "text/html");
                                }
                                else
                                {
                                    csmx.gzl_danwei = cellsQuestion[i, 4].StringValue;
                                }
                                if (cellsQuestion[i, 5].StringValue == null || cellsQuestion[i, 5].StringValue == "")
                                {
                                    csmx.yijushuoming = "";
                                }
                                else
                                {
                                    csmx.yijushuoming = cellsQuestion[i, 5].StringValue;
                                }
                                csmx.shenbaoshu = csmx.gongzuoliang * csmx.chengbenbiaozhun;
                                csmx.xiangmuguanliID = xmglID;
                                //数据库操作
                                db.xmcsmxs.Add(csmx);
                            }
                            catch (Exception ex)
                            {
                                error = "第" + j + "行记录插入有误，请认真检查格式后再导入！建议使用模板上传！追加";
                                string ex_str = ex.ToString();
                                return Json(new { success = false, errorMsg = error }, "text/html");
                            }
                        }
                        db.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        error = ex.Message;
                        return Json(new { success = false, errorMsg = error }, "text/html");
                    }
                }
                transaction.Complete();
            }
            error = "祝贺您，本次信息导入成功！";
            return Json(new { success = true, errorMsg = error }, "text/html");
        }


        [Authorize(Roles = "部门负责人,员工,评委,业务专员")]
        public JsonResult Upload(HttpPostedFileBase fileData)
        {
            string xmmulu = Request.Form["xmmulu"];
            int xmID = Convert.ToInt32(Request.Form["xmID"]);
            string xmname = Request.Form["xmname"];
            //xmcgpz cgpz = new xmcgpz();
            fileupload upload = new fileupload();
            if (fileData != null)
            {
                string filePath1 = string.Format("~/Uploads/{0}/{1}/", xmmulu, xmname);//通过参数组建一个路径格式的字符串

                // 文件上传后的保存路径
                string filePath = Server.MapPath(filePath1);//将路径格式的字符串通过函数转化为服务器路径
                if (!Directory.Exists(filePath))            //判断路径是否存在，如果不存在，则根据路径建立路径文件夹，这里只需要检验到文件夹
                {
                    Directory.CreateDirectory(filePath);
                }
                string fileseze = (fileData.ContentLength / 1024).ToString();

                string fileName = Path.GetFileName(fileData.FileName);// 原始文件名称

                string fileExtension = Path.GetExtension(fileName); // 文件扩展名

                //string saveName = Guid.NewGuid().ToString() + fileExtension; // 保存文件名称

                string webPath = string.Format("~/Uploads/{0}/{1}/{2}", xmmulu, xmname, fileName);//这里只是字符串，判断参数，用于判断上传的文件是否存在，这里需要检验到四层目录

                string generateFilePath = Server.MapPath(webPath);//这里是真实的路径，由字符串转化为路径

                if (System.IO.File.Exists(generateFilePath))//判断文件是否存在，如果存在返回提示json字符串
                {
                    return Json(new { Success = false, Message = fileName + "这个文件已经存在！" }, JsonRequestBehavior.AllowGet);
                }

                //if (fileExtension!=".jpg"){
                //    return Json(new { Success = false, Message = fileName + "文件上传类型不对！" }, JsonRequestBehavior.AllowGet);
                //}

                ////fileData.SaveAs(filePath +fileName);
                try
                {
                    fileData.SaveAs(generateFilePath);//saveas保存的参数是服务器根路径，webpath不是路径   通过路径和源文件名做参数，保存上传文件
                    //fileData.SaveAs(webPath);

                    upload.filepath = string.Format("Uploads/{0}/{1}/", xmmulu, xmname);
                    upload.filename = fileName;
                    upload.fileextension = fileExtension;
                    //upload.savename = saveName;
                    upload.xiangmuguanliID = xmID;          //这里要给xiangmuguanliID赋值，存在一对多关系

                    upload.uploadtime = DateTime.Now.ToString("yyyy-MM-dd HH：mm：ss");

                    upload.filesize = fileseze + "kb";

                    db.fileuploads.Add(upload);
                    db.SaveChanges();

                    return Json(new { Success = true, Message = fileName + "上传成功！" }, JsonRequestBehavior.AllowGet);  //, FileName = fileName 
                }
                catch (Exception ex)
                {
                    if (System.IO.File.Exists(generateFilePath))//判断文件是否存在，如果存在返回提示json字符串
                    {
                        System.IO.File.Delete(generateFilePath);
                    }
                    return Json(new { Success = false, Message = ex.Message }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(new { Success = false, Message = "请选择要上传的文件！" }, JsonRequestBehavior.AllowGet);
            }
        }
        

        //下载电子导入模板
        //GetexcelFile
        [Authorize(Roles = "部门负责人,员工")]
        public FileResult GetexcelFile(string mobanname)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + "Content/uploads/moban/";
            string fileName = mobanname + ".xlsx";
            return File(path + fileName, "text/plain", fileName);
        }



        //下载文件
        //[Authorize(Roles = "部门负责人,员工,业务专员,评委,领导,分管领导,财务主管")]
        public FileResult GetFile(string filename, string filepath)
        {
            filepath = filepath.Replace(@"/", @"\");
            string path = AppDomain.CurrentDomain.BaseDirectory + filepath;

            return File(path + filename, "text/plain", filename);//fileName应该是下载出来后的名字
        }


        //datagrid加载记录
        [HttpPost]
        public JsonResult getupload(int xmglid)
        {
            int page = (Request.Form["page"] != null) ? Int32.Parse(Request.Form["page"]) : 1;

            int rows = (Request.Form["rows"] != null) ? Int32.Parse(Request.Form["rows"]) : 10;

            //var csmx = db.xmcsmxs.Where(s => s.xiangmuguanliID == xmglid);
            var upload = db.fileuploads.Where(s => s.xiangmuguanliID == xmglid);
            var upld = from s in upload
                       orderby s.filename
                       select s;
            try
            {
                // 返回到前台的值必须按照如下的格式包括 total and rows 
                var easyUIPages = new Dictionary<string, object>();
                easyUIPages.Add("total", upld.Count());
                easyUIPages.Add("rows", upld.ToPagedList(page, rows));
                return Json(easyUIPages);
            }
            catch (Exception)
            {
                throw;
            }
        }


        //feupld_del
        [HttpPost]
        [Authorize(Roles = "部门负责人,员工,评委,业务专员")]
        public JsonResult feupld_del(int id, string path, string name)
        {

            path = path.Replace(@"/", @"\");//D:\\32位激活工具\\xmkgl\\xmkgl\\Uploads/2015ll/a1/也能正常删除，不过改为反向双斜杠

            string filepath = AppDomain.CurrentDomain.BaseDirectory + path;//D:\\32位激活工具\\xmkgl\\xmkgl\\Uploads/2015ll/a1/

            //int ID = Convert.ToInt32(form["id"]);  //form["id"]为页面post方法传递的参数,选中row然后 id=row.ID
            try
            {
                if (System.IO.File.Exists(filepath + name))
                {
                    System.IO.File.Delete(filepath + name);

                }
                fileupload fileupload = db.fileuploads.Find(id);

                db.fileuploads.Remove(fileupload);

                db.SaveChanges();

                return Json(new { success = true, errorMsg = "文件删除成功！" });

            }
            catch (Exception ex)
            {
                return Json(new { success = false, errorMsg = ex.ToString() }, "text/html");
            }
        }
    }
}



