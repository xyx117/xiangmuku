using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Owin;
using xmkgl.Models;
using PagedList;
using xmkgl.DAL;
using System.Transactions;
using System.Data.SqlClient;
using xmkgl.common;
using System.Data.Entity;
using System.Net;
using System.Collections;
using Microsoft.Owin;
using System.Web.SessionState;

namespace xmkgl.Controllers
{
    [Authorize]
    public partial class AccountController : Controller 
    {
        private cwcxmkContent db = new cwcxmkContent();

        private ElmahContent db1 = new ElmahContent();

        //private IdentityDb db2 = new cwcxmkContent();

        private ApplicationUserManager _userManager;

        //这里是添加用户角色时 添加上的
        private ApplicationRoleManager _roleManager;

        //这里是根据登录人的id取出登录人的所属学院，在登录人创建项目时把登录人的所属学院赋值给项目。在xiangmuguanliindex.cshtml中也要根据登录人的所属学院信息显示对应学院信息
        //private ApplicationDbContext identitydb = new ApplicationDbContext();
        //public string user_suoshuxueyuan(string userId)
        //{
        //    ApplicationUser xueyuan = identitydb.Users.Where(s => s.Id == userId).FirstOrDefault();
        //    return (xueyuan.suoshuxueyuan);
        //}


        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager)
        {
            UserManager = userManager;
        }

        public ApplicationUserManager UserManager
        {         //这里定义了一个连接数据库连接字符串的类，有关于人员的操作可以直接使用这个类
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        //添加角色
        public AccountController(ApplicationRoleManager roleManager)
        {
            RoleManager = roleManager;
        }
        public ApplicationRoleManager RoleManager
        {         //这里定义了一个连接数据库连接字符串的类，有关于人员的操作可以直接使用这个类
            get
            {
                return _roleManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationRoleManager>();
            }
            private set
            {
                _roleManager = value;
            }
        }

        public ActionResult role_add()
        {
           
            return View();
        }

        //添加角色,待验证
        public ActionResult AddRole(String name)
        {
            using (var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(new IdentityDbContext())))
            {
                if (!roleManager.RoleExists(name))
                {
                    roleManager.Create(new IdentityRole(name));
                }
            }
            return Json(new { success = false, msg = "用户不存在！" }, "text/html"); 
        }

        //添加角色,待验证
        public class ApplicationRoleManager : RoleManager<IdentityRole>
        {
            public ApplicationRoleManager(IRoleStore<IdentityRole, string> roleStore)
                : base(roleStore)
            {
            }
            public static ApplicationRoleManager Create(IdentityFactoryOptions<ApplicationRoleManager> options, IOwinContext context)
            {
                return new ApplicationRoleManager(new RoleStore<IdentityRole>(context.Get<ApplicationDbContext>()));
            }
        }

        //部门负责人显示用户
        [HttpPost]
        public JsonResult getrole(string userId)
        {
            var xm1 = from c in RoleManager.Roles
                      //where c.parentID == myid
                      //orderby c.UserName
                      select new { Id = c.Id,name = c.Name};
            var xm = xm1;

            return Json(xm1);           
        }

        //分管领导查看分管部门
        public ActionResult fenguangbm(string userid)
        {
            List<fgbmViewModel> fgbmlist = new List<fgbmViewModel>();

            using (ApplicationDbContext identitydb1 = new ApplicationDbContext()) //创建一个新的上下文
            {
                var fgbm = identitydb1.Users.Where(s => s.Id == userid).Select(s => s.suoshuxueyuan).FirstOrDefault();

                if (fgbm != null)
                {
                    string[] bm = fgbm.Split(',');

                    for (var i = 0; i < bm.Length; i++)
                    {
                        fgbmViewModel bm_temp = new fgbmViewModel();
                        bm_temp.bm = bm[i];
                        fgbmlist.Add(bm_temp);

                    }
                }
            }
            return View(fgbmlist);
        }

        //这里相当于一个  ViewModel
        public class userinfo
        {
            public string username { get; set; }
            public string userrole { get; set; }

            public string bumen { get; set; }
        }

        //返回登录人的所属部门
        [HttpPost]
        public JsonResult getusernameandrole(string id)
        {
            var user = UserManager.Users.Where(c => c.Id == id).Select(c => new userinfo{ username=c.UserName,userrole=c.role,bumen=c.suoshuxueyuan }).FirstOrDefault();

            if (user == null)
            {
                return Json(new { success = false,msg="用户不存在！" }, "text/html");
            }

            if (user.userrole == "部门负责人" || user.userrole == "员工")
            {
                return Json(new { success = true, loginrole =user.bumen+"/"+user.userrole, loginname = user.username }, "text/html");
            }
            else
            {
                return Json(new { success = true, loginrole = user.userrole, loginname = user.username }, "text/html"); 
            }
        }               

        public ActionResult bmfzindex()
        {
            //为所辖部门获取部门设置列表
            var GenreLst = new List<string>();
            var GenreQry = from d in db.bumenshezhis
                           orderby d.BmName
                           select d.BmName;
            GenreLst.AddRange(GenreQry.Distinct());
            
            ViewBag.bumen = GenreLst;            
            return View();
        }

        public ActionResult ywzyindex()
        {            
            return View();
        }

        public ActionResult A_class_dep_users()
        {
            //为所辖部门获取部门设置列表
            var GenreLst = new List<string>();
            var GenreQry = from d in db.bumenshezhis
                           orderby d.BmName
                           select d.BmName;
            GenreLst.AddRange(GenreQry.Distinct());            
            ViewBag.bumen = GenreLst;
            return View();
        }

        public ActionResult B_class_dep_users()
        { 
            return View();
        }

        //转到评委页面
        public ActionResult pingweiindex()
        {                       
            var mululist = new List<string>();
            var muluQury = from d in db.xiangmumulus.Where(s=>s.Jieshushijian<DateTime.Now)
                           select d.Name;
            mululist.AddRange(muluQury.Distinct());
            ViewBag.mululist = mululist;
            return View();
        }

        private class bumen
        {
           public string id{ get; set; }
           public string it{ get; set; }
        }

        //评委分配所属学院是加载的列表，yxbm为已经选择的部门
        public JsonResult bumenlist(string yxbm)
        {
            yxbm = Server.UrlDecode(yxbm);
            //为所辖部门获取部门设置列表
            var GenreLst = new List<bumen>();
            var GenreQry = from d in db.bumenshezhis.Where(s => s.pingweifou == false||yxbm.Contains(s.BmName))
                           orderby d.BmName
                           select new bumen {id=d.BmName, it=d.BmName};
            GenreLst.AddRange(GenreQry.Distinct());
            
            return Json(GenreLst.ToList(),JsonRequestBehavior.AllowGet);            
        }

        //评委管理，显示评委列表   pingweigetuser
        [HttpPost]
        public JsonResult pingweigetuser()
        {
            int page = (Request.Form["page"] != null) ? Int32.Parse(Request.Form["page"]) : 1;
            int rows = (Request.Form["rows"] != null) ? Int32.Parse(Request.Form["rows"]) : 10;

            var xm = from c in UserManager.Users
                     where c.role == "评委"
                     orderby c.role
                     select new { Id = c.Id, UserName = c.UserName, zhenshiname = c.zhenshiname,pingshenmulu=c.pingshenmulu,pingshenrenwu=c.pingshenrenwu, suoshuxueyuan = c.suoshuxueyuan, PhoneNumber = c.PhoneNumber };           
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

        //部门负责人显示用户
        [HttpPost]
        public JsonResult bmfergetuser(string userId)
        {
            int page = (Request.Form["page"] != null) ? Int32.Parse(Request.Form["page"]) : 1;
            int rows = (Request.Form["rows"] != null) ? Int32.Parse(Request.Form["rows"]) : 10;
            var user = UserManager.FindById(userId);//通过userid找出部门负责人
            int myid = user.myuserID;//取出部门负责人的myuserID
          
            //var xm = UserManager.Users.Where(c => c.parentID == myid).OrderBy(c=>c.UserName);  //找出parentID等于其部门负责人myuserID的人员
            var xm = from c in UserManager.Users
                     where c.parentID == myid
                     orderby c.UserName
                     select new { Id = c.Id, UserName = c.UserName, zhenshiname = c.zhenshiname, suoshuxueyuan = c.suoshuxueyuan, PhoneNumber = c.PhoneNumber };
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

        //业务专员显示用户   ywzygetuser_yg
        [HttpPost]
        public JsonResult ywzygetuser(string searchquery)
        {  
            int page = (Request.Form["page"] != null) ? Int32.Parse(Request.Form["page"]) : 1;
            int rows = (Request.Form["rows"] != null) ? Int32.Parse(Request.Form["rows"]) : 10;
            
            var xm = from c in UserManager.Users
                 where c.role != "员工" && c.role != "评委" && c.UserName != "admin" && (c.UserName.Contains(searchquery) || c.zhenshiname.Contains(searchquery))
                 orderby c.role
                 select new { Id = c.Id, UserName = c.UserName, myuserID = c.myuserID, zhenshiname = c.zhenshiname, role = c.role, usercount = c.usercount, suoshuxueyuan = c.suoshuxueyuan, PhoneNumber = c.PhoneNumber };

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

        //业务专员获取 二级部门用户
        [HttpPost]
        public JsonResult ywzygetuser_yg(string searchquery) 
        {
            //page: The page number, start with 1.
            // rows: The page rows per page.

            int page = (Request.Form["page"] != null) ? Int32.Parse(Request.Form["page"]) : 1;
            int rows = (Request.Form["rows"] != null) ? Int32.Parse(Request.Form["rows"]) : 10;

            //选取显示不是员工的所有人员
            //var xm = UserManager.Users.Where(c => c.role == "员工");
            //var xm = UserManager.Users.Where(c => c.UserName != "admin").OrderBy(c => c.role);/*&&c.role!="评委"*/
            var xm = from c in UserManager.Users
                     where c.role == "员工"  && c.UserName != "admin" && (c.UserName.Contains(searchquery) || c.zhenshiname.Contains(searchquery))
                     orderby c.role
                     select new { Id = c.Id, UserName = c.UserName, myuserID = c.myuserID, zhenshiname = c.zhenshiname, role = c.role, usercount = c.usercount, suoshuxueyuan = c.suoshuxueyuan, PhoneNumber = c.PhoneNumber };

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


        //显示部门负责人所创建的人员的名单
        [HttpPost]
        public JsonResult bumenyuangong(int bmfzr_myuser_id)
        {
            //选取显示不是员工的所有人员
            //var xm = UserManager.Users.Where(c => c.role != "员工");
            var xm = UserManager.Users.Where(c => c.parentID == bmfzr_myuser_id).OrderBy(c => c.zhenshiname).Select(c => new { name = c.UserName });

            string yuangong="";

            if (xm.Any())
            {
                foreach (var item in xm)
                {
                    yuangong = yuangong + item.name + " " ;
                }                
            }
            return Json(new { success = true, Msg = yuangong }, "text/html");
        }


        [HttpPost]
        //[ValidateAntiForgeryToken]   这条语句不能用！！！！！
        public JsonResult bmfzrsaveuser(RegisterViewModel model, string userId)
        {
            string xueyuan = customidentity.suoshuxueyuan(userId);
            
            bool zhuangtai = true;

            string msg= "";

            if (ModelState.IsValid)
            {
                var bmferuser = UserManager.FindById(userId);  //找出这个部门负责人，通过登录的id，id为加密值

                int myuserID = bmferuser.myuserID;  //取出这个负责人的myuserID赋值给parentID

                int bmferusercount = bmferuser.usercount;//

                bmferuser.usercount = bmferusercount + 1;

                var user = new ApplicationUser() {
                    UserName = model.UserName,
                    suoshuxueyuan = xueyuan,
                    Email = model.UserName + "@qq.com",
                    zhenshiname = model.zhenshiname,
                    role = "员工",
                    parentID = myuserID,
                    usercount = 0,
                    PhoneNumber = model.PhoneNumber
                };

                using (TransactionScope transaction = new TransactionScope())//原子操作，事物错误回滚
                {
                    IdentityResult result1, result2, result3;
                    try
                    {
                        if (model.password == null && model.confirmPassword == null)
                        {
                            string pw = common.Initial_pw.Init_pw();//获取初始化的密码，也可以说是获取默认密码
                            result1 = UserManager.Create(user, pw);//根据对象user和password创建新用户
                        }
                        else
                        {
                            result1 = UserManager.Create(user, model.password);//根据对象user和password创建新用户
                        }                        
                        if (result1.Succeeded)
                        {
                            result2 = UserManager.AddToRole(user.Id, "员工");
                            if (result2.Succeeded)
                            {
                                result3 = UserManager.Update(bmferuser);
                                if (result3.Succeeded)
                                {
                                    transaction.Complete();
                                    msg = "新增员工成功！";
                                }
                                else
                                {
                                    zhuangtai = false;
                                    foreach (var error in result3.Errors)
                                    {
                                        msg =msg + error;
                                    }
                                }
                            }
                            else {

                                zhuangtai = false;
                                foreach (var error in result2.Errors)
                                {
                                    msg =msg + error;
                                }                                                       
                            }                
                        }
                        else
                        {
                            zhuangtai = false;
                            foreach (var error in result1.Errors)
                            {
                              msg = msg + error;
                            }
                        }                                  
                    }
                    catch (Exception ex)
                    {
                        msg =msg+ex.ToString();
                        return Json(new {success=false, errorMsg =msg }, "text/html");
                    }
                    finally
                    {
                        transaction.Dispose();
                    }
                }
                return Json(new { success = zhuangtai, errorMsg =msg }, "text/html");
            }           
            return Json(new {success=false, errorMsg = "您输入的信息有错误！" }, "text/html");
        }


        //部门负责人编辑部门底下人员
        [HttpPost]
        public async Task<JsonResult> edit_renyuan(string id)
        {
            ApplicationUser user = await UserManager.FindByIdAsync(id);
            if (user != null)
            {
                try
                {
                    user.zhenshiname = Request.Form["zhenshiname"];
                    user.PhoneNumber = Request.Form["PhoneNumber"];
                    IdentityResult result = await UserManager.UpdateAsync(user);
                    return Json(new { success = true, errorMsg = "编辑保存成功！" }, "text/html");
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, errorMsg = ex.ToString() }, "text/html");
                }
            }
            else
            {
                return Json(new { success = false, errorMsg = "您编辑的用户不存在！" }, "text/html");
            }
        }


        //判断用户名存在就返回false,不存在就返回true
        [HttpPost]
        public String checkNameIsSame(String name)
        {
            string isOk = "False";
            var xm = UserManager.FindByName(name);                
          if (xm==null)
          {
                isOk = "True";
          }
            return isOk + "";
        }


        [HttpPost]
        //[ValidateAntiForgeryToken]   这条语句不能用！！！！！
        public JsonResult ywzysaveuser(ywzyRegisterViewModel model)
        {
            string xueyuan = Request.Form["suoshuxueyuan"];//模型无法接受到多选值，此处通过表单接收多选值    

            string role = Request.Form["role"];  //新增领导分管学院的时候，根据新增用户时所分配的角色，判断角色属于领导的，在所属学院上加一个all
            if (role == "领导")
            {
                xueyuan = "all,"+xueyuan;
            };

            bool zhuangtai = true;
            string msg = "";
            
            IdentityResult result;

            if (ModelState.IsValid)
            {
                var user = new ApplicationUser() { UserName = model.UserName, suoshuxueyuan = xueyuan, Email = model.UserName + "@qq.com", zhenshiname = model.zhenshiname, role = model.role, parentID = -1, usercount = 0, PhoneNumber = model.PhoneNumber };
                
                using (TransactionScope transaction = new TransactionScope())//原子操作，事物错误回滚
                {
                    try
                    {
                        if (model.password == null&&model.confirmPassword==null)
                        {
                            string pw = common.Initial_pw.Init_pw();  //获取初始化的密码，也可以说是获取默认密码

                            result = UserManager.Create(user, pw);//根据对象user和password创建新用户
                        }
                        else
                        {
                            result = UserManager.Create(user, model.password);//根据对象user和password创建新用户
                        }
                        //IdentityResult result = UserManager.Create(user, model.password);//根据对象user和password创建新用户               
                     
                        if (result.Succeeded)
                        {
                            if (!UserManager.IsInRole(user.Id, model.role))   //在业务专员添加人员工时，判断添加角色是否属于员工，不是则添加到角色
                            {
                                UserManager.AddToRole(user.Id, model.role);
                            };
                            transaction.Complete();
                            msg = "保存成功！";                         
                        }
                        else
                        {
                            zhuangtai = false;
                            foreach (var error in result.Errors)
                            {
                                msg = msg + error;
                            }                                                    
                        };
                    }
                    catch (Exception ex)
                    {
                        //return Json(new { errorMsg = ex.ToString() }, "text/html");
                        return Json(new { success = false, errorMsg = ex.ToString() }, "text/html");
                    }
                    finally
                    {
                        transaction.Dispose();
                    }
                }               
                return Json(new { success = zhuangtai, errorMsg = msg }, "text/html");                
            }
            return Json(new { success = false, errorMsg = "您输入的值有错误！" }, "text/html");
        }



        //编辑用户
        [HttpPost]
        //public async Task<ActionResult> Edit(string id, string email, string password)
        public async Task<JsonResult> edit_user(string id)
        {
            ApplicationUser user = await UserManager.FindByIdAsync(id);

            if (user != null)
            {               
                try 
                {
                    //user.UserName = Request.Form["Username"];
                    user.zhenshiname = Request.Form["zhenshiname"];
                    //user.role = Request.Form["role"];

                    if (user.role == "分管领导"||user.role=="领导")
                    {
                        user.suoshuxueyuan = Request.Form["suoshuxueyuan"];
                    }                    
                    user.PhoneNumber = Request.Form["PhoneNumber"];

                    IdentityResult result = await UserManager.UpdateAsync(user);

                    return Json(new { success = true, errorMsg = "编辑保存成功！" }, "text/html");
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, errorMsg = ex.ToString() }, "text/html");
                }
            }
            else
            {
                return Json(new { success = false, errorMsg = "您编辑的用户不存在！" }, "text/html");
            }
        }

        //业务专员登录，获取评委所审核项目总数
        [HttpPost]
        public JsonResult pingshenshuliang(string suoshumulu, string suoshuxueyue)
        {
            if (suoshuxueyue == "" || suoshumulu == "")
            {
                //return Json(new { success = true, errorMsg = "" }, "text/html");

                return Json(new { success = true, pingshenshu = "0", zongshu = "0" }, "text/html");
            }
            else 
            {
                //这里的总数我们做出了后期的修改，总数的基数改为部门负责人已经提交的
                int allcount = db.xiangmuguanlis.Where(s => suoshumulu.Contains(s.Xiangmumulu)&&s.bmfzrtijiao=="已提交").Count();

                if (allcount == 0)
                {
                    return Json(new { success = true, pingshenshu = "0", zongshu = "0" }, "text/html");
                }
                int mycount = db.xiangmuguanlis.Where(s =>suoshumulu.Contains(s.Xiangmumulu) && suoshuxueyue.Contains(s.suoshuxueyuan)).Count();

                string ss = mycount + "/" + allcount;
                return Json(new { success = true, pingshenshu = mycount, zongshu = allcount }, "text/html");
            }            
        }


        //业务专员增加评委ywzy_pw_save
        [HttpPost]
        //[ValidateAntiForgeryToken]   这条语句不能用！！！！！
        public JsonResult ywzy_pw_save(ywzyRegister_pw_ViewModel model)//这里如果用模型来取值，那在视图上，如果pingshenmulu和suoshuxueyue是多选的，那在model上，只会获取到一个值
        {
            string xueyuan = Request.Form["suoshuxueyuan"];//模型无法接受到多选值，此处通过表单接收多选值

            string pingweiname = Request.Form["UserName"];

            string pingshenmulu = Request.Form["pingshenmulu"];

            string[] xueyuan_fenli = xueyuan.Split(',');   //这里考虑到一种情况就是，如果评委所属学院是多个的话，那么在选择学院时就要给多个学院一起改变学院的标志，其他评委不可选
            var xueyuan_fenli_conut = xueyuan_fenli.Count();

            bumenshezhi bumenshezhi = new bumenshezhi();              //因为在业务专员增加评委时，要给部门做一个标记，这里取出部门

            bool zhuangtai = true;
            string msg = "";

            IdentityResult result;

            if (ModelState.IsValid)
            {
                var user = new ApplicationUser() { UserName = model.UserName, suoshuxueyuan = xueyuan, pingshenmulu = pingshenmulu, Email = model.UserName + "@qq.com", zhenshiname = model.zhenshiname, role = "评委", parentID = -1, usercount = 0, pingshenrenwu = model.pingshenrenwu, PhoneNumber = model.PhoneNumber };

                using (TransactionScope transaction = new TransactionScope())//原子操作，事物错误回滚
                {
                    try
                    {
                        if (model.password == null && model.confirmPassword == null)
                        {
                            string pw = common.Initial_pw.Init_pw(); //获取初始化的密码，也可以说是获取默认密码
                            result = UserManager.Create(user, pw);//根据对象user和password创建新用户
                        }
                        else
                        {
                            result = UserManager.Create(user, model.password);//根据对象user和password创建新用户
                        }

                        //IdentityResult result = UserManager.Create(user, model.password);//根据对象user和password创建新用户

                        if (!UserManager.IsInRole(user.Id, "评委"))   //在业务专员添加人员工时，判断添加角色是否属于员工，不是则添加到角色
                        {
                            UserManager.AddToRole(user.Id, "评委");
                        };

                        if (result.Succeeded)
                        {
                          for (int i = 0; i < xueyuan_fenli_conut; i++)   //这里考虑到一种情况就是，如果评委所属学院是多个的话，那么在选择学院时就要给多个学院一起改变学院的标志
                            {
                            string xueyue_fen = xueyuan_fenli[i];

                            bumenshezhi = db.bumenshezhis.Where(s => s.BmName == xueyue_fen).FirstOrDefault();

                            bumenshezhi.pingweifou = true;

                            bumenshezhi.pingweiname = pingweiname;

                            db.Entry(bumenshezhi).State = EntityState.Modified;   //保存对部门表的修改
                          }

                          db.SaveChanges();

                          transaction.Complete();

                          msg = "新增评委成功！";
                        }
                        else
                        {
                            zhuangtai = false;
                            foreach (var error in result.Errors)
                            {
                                msg = msg + error;
                            }                           
                        };
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
                return Json(new { success = zhuangtai, errorMsg = msg }, "text/html");
            }
            return Json(new { success =false , errorMsg = "您输入的值有错误！" }, "text/html");
        }


        //编辑评委
        [HttpPost]
        //public async Task<ActionResult> Edit(string id, string email, string password)
        public JsonResult edit_pw(string id, string username, string yxbmold)
        {
            //AppUser user = await UserManager.FindByIdAsync(id);        //这里改为了 applicationuser

            ApplicationUser user = UserManager.FindById(id);
            if (user != null)
            {
                //user.UserName = Request.Form["UserName"];  
                user.zhenshiname = Request.Form["zhenshiname"];
                //user.role = Request.Form["role"];
                var  xueyuan = Request.Form["suoshuxueyuan"];
                user.suoshuxueyuan = xueyuan;
                user.pingshenmulu = Request.Form["pingshenmulu"];
                user.PhoneNumber = Request.Form["PhoneNumber"];

                string[] xueyuan_old = yxbmold.Split(',');   //这里考虑到一种情况就是，如果评委所属学院是多个的话，那么在选择学院时就要给多个学院一起改变学院的标志，其他评委不可选
                var xueyuan_old_conut = xueyuan_old.Count();
                string[] xueyuan_fenli = xueyuan.Split(',');   //这里考虑到一种情况就是，如果评委所属学院是多个的话，那么在选择学院时就要给多个学院一起改变学院的标志，其他评委不可选
                var xueyuan_fenli_conut = xueyuan_fenli.Count();


                bool zhuangtai = true;
                string Msg = "";

                using (TransactionScope transaction = new TransactionScope())//原子操作，事物错误回滚
                {
                    try
                    {   IdentityResult result =  UserManager.Update(user);

                        if (result.Succeeded)
                        {
                            bumenshezhi bumenshezhi = new bumenshezhi();

                            for (int i = 0; i < xueyuan_old_conut; i++)   //这里考虑到一种情况就是，如果评委所属学院是多个的话，那么在选择学院时就要给多个学院一起改变学院的标志
                            {
                                string xueyue = xueyuan_old[i];
                                bumenshezhi = db.bumenshezhis.Where(s => s.BmName == xueyue).FirstOrDefault();
                                bumenshezhi.pingweifou = false;
                                bumenshezhi.pingweiname = null;
                                db.Entry(bumenshezhi).State = EntityState.Modified;   //保存对部门表的修改
                            }

                            for (int i = 0; i < xueyuan_fenli_conut; i++)   //这里考虑到一种情况就是，如果评委所属学院是多个的话，那么在选择学院时就要给多个学院一起改变学院的标志
                            {
                                string xueyue_fen = xueyuan_fenli[i];
                                bumenshezhi = db.bumenshezhis.Where(s => s.BmName == xueyue_fen).FirstOrDefault();
                                bumenshezhi.pingweifou = true;
                                bumenshezhi.pingweiname =Server.UrlDecode(username);
                                db.Entry(bumenshezhi).State = EntityState.Modified;   //保存对部门表的修改
                            }

                           db.SaveChanges();
                           transaction.Complete();
                           Msg = "编辑保存成功！";
                        }
                        else
                        {
                            zhuangtai = false;
                            foreach (var error in result.Errors) { Msg =Msg+error; }                            
                        }
                    }
                    catch (Exception ex)
                    {
                        //return Json(new { errorMsg = ex.ToString() }, "text/html");
                        return Json(new { success = false, errorMsg = ex.ToString() }, "text/html");
                    }
                    finally
                    {
                        transaction.Dispose();
                    }
                 }
                return Json(new { success = zhuangtai, errorMsg = Msg }, "text/html");
            }
            else
            {             
                return Json(new { success = false, errorMsg = "您编辑的用户不存在！" }, "text/html");
            }          
        }     


        //部门负责人删除员工
        [HttpPost]
        public JsonResult bmfzrdeluser(string yuangongid,string bmferid)
        {
            string errormsg0 = "";
            string errormsg1 = "";
            string errormsg2 = "";
            bool zhuantai = true;

           var yuangong = UserManager.FindById(yuangongid);  //找出这个员工，通过登录的id，id为加密值

           var bmfer = UserManager.FindById(bmferid);//通过登录的id找出部门负责人

           IdentityResult resultyg;//定义一个表示identity操作的值
           IdentityResult resultfzr;

           int usercount = bmfer.usercount; //取出这个部门负责人的usercount值

           bmfer.usercount = usercount - 1;//因为删除一个部门负责人下的员工，部门负责人的usercount减1，并重新赋值给部门负责人的usercount

           using (TransactionScope transaction = new TransactionScope())//原子操作，事物错误回滚
           {
               try
               {
                    resultfzr = UserManager.Update(bmfer);//操作更新部门负责人

                    resultyg = UserManager.Delete(yuangong);//删除选出的员工
                  
                    if (resultfzr.Succeeded && resultyg.Succeeded)
                    {
                        errormsg0 = "删除用户成功！";
                        transaction.Complete();                      
                    }
                    else
                    {
                        foreach (var error in resultfzr.Errors)
                        {
                            errormsg1 = errormsg1 + error;
                        }
                        foreach (var error in resultyg.Errors)
                        {
                            errormsg2 = errormsg2 + error;
                        }
                        zhuantai = false;
                        errormsg0 = errormsg1 + errormsg2;
                    };                 
               }
               catch (Exception ex)
               {
                   return Json(new {Succeeded=false, Msg = ex.ToString() }, "text/html");
               }
               finally
               {
                   transaction.Dispose();
               }
           }
           return Json(new {Succeeded=zhuantai, Msg = errormsg0 }, "text/html");
        }



        //业务专员删除部门管理员
        [HttpPost]
        public JsonResult ywzydelBumen(string id)
        {
            string errormsg1 = "";

           // IdentityResult resultyg;//定义一个表示identity操作的值
            IdentityResult resultfzr;
          
            //var yuangong = UserManager.FindById(yuangongid);  //找出这个员工，通过登录的id，id为加密值

            var bmfer = UserManager.FindById(id);//通过登录的id找出部门负责人

            //bmfer.parentID = bmfer.myuserID;

            int usercount = bmfer.usercount;//取出这个部门部门负责人的usercount，并且判读是否为0，为0直接删除，不为0，先删除底下员工后删除负责人
           
            int myuserid = bmfer.myuserID;//取出这个部门负责人的myuserID,并根据myuserID找出底下员工

            string ConString = System.Configuration.ConfigurationManager.ConnectionStrings["cwcxmkContent"].ConnectionString;
            SqlConnection con = new SqlConnection(ConString);
            string sqldel = "delete from AspNetUsers where parentID=" + myuserid.ToString();  //这个筛选方式写法是否正确待检验   

            if(usercount==0)
            {
                resultfzr = UserManager.Delete(bmfer);//操作更新部门负责人

                if (resultfzr.Succeeded)
                {
                    return Json(new { Succeeded = true, errorMsg = "删除成功！" }, "text/html");
                    //return Json(new { success = true, errorMsg = "保存成功！" }, "text/html");
                }
                else
                {
                    foreach (var error in resultfzr.Errors)
                    {
                        errormsg1 = errormsg1 + error;
                    }
                    return Json(new { Succeeded = false, errorMsg = errormsg1.ToString() }, "text/html");
                };
            }
            else 
            {
                bool zhuangtai = true;
                string msg = "";                
                using (TransactionScope transaction = new TransactionScope())//原子操作，事物错误回滚
                {
                    resultfzr = UserManager.Delete(bmfer);//操作更新部门负责人

                    if (!resultfzr.Succeeded)
                        {
                            zhuangtai = false;
                            foreach (var error in resultfzr.Errors)
                            {
                               msg = msg + error;
                            }
                         }
                    else
                    {
                        try
                        {
                            con.Open();

                            SqlCommand sqlcmddel = new SqlCommand(sqldel, con);

                            sqlcmddel.ExecuteNonQuery();   //删除了现有的名单
                            
                            transaction.Complete();

                             msg = "用户删除成功！" ;
                        }
                        catch (Exception ex)
                        {                            
                            return Json(new { Succeeded = false, errorMsg = ex.ToString() }, "text/html");
                        }
                        finally
                        {
                            con.Close(); //无论如何都要执行的语句。
                            transaction.Dispose();
                        }
                    }
                 }
                return Json(new { Succeeded =zhuangtai, errorMsg = msg }, "text/html");
            }                       
        }


        //业务专员删除用户
        [HttpPost]
        public JsonResult ywzydelqitq(string id)
        {
            string errormsg2 = "";
          
            var yuangong = UserManager.FindById(id);  //找出这个员工，通过登录的id，id为加密值
            try
            {
                IdentityResult resultyg = UserManager.Delete(yuangong);//删除选出的员工

                if (resultyg.Succeeded)
                {
                    return Json(new { Succeeded = true, errorMsg = "删除用户成功！" }, "text/html");
                }
                else
                {
                    foreach (var error in resultyg.Errors)
                    {
                        errormsg2 = errormsg2 + error;
                    }
                    return Json(new { Succeeded = false, errorMsg = errormsg2.ToString() }, "text/html");
                };
            }
            catch (Exception ex)
            {
                return Json(new { Succeeded = false, errorMsg = ex.ToString() }, "text/html");
            }                      
        }


        //业务专员删除评委ywzydelpingwei
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="bumen">前端页面选取多选时，由多个  BmName 拼接成的 字符串</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult ywzydelpingwei(string id,string bumen)
        {
            string[] bumen_fenli = bumen.Split(',');   //拆分字符串 ，这里考虑到一种情况就是，如果评委所属学院是多个的话，那么在选择学院时就要给多个学院一起改变学院的标志
            var bumen_fenli_conut = bumen_fenli.Count();

            bumenshezhi bumenshezhi = new bumenshezhi();              //因为在业务专员增加评委时，要给部门做一个标记，这里取出部门

            bool zhuangtai = true;
            string msg = "";

            var yuangong = UserManager.FindById(id);  //找出这个员工，通过登录的id，id为加密值

            IdentityResult resultyg;//定义一个表示identity操作的值

            using (TransactionScope transaction = new TransactionScope())//原子操作，事物错误回滚
            {
                try
                {
                    resultyg = UserManager.Delete(yuangong);//删除选出的员工

                    if (resultyg.Succeeded)
                    {
                        for (int i = 0; i < bumen_fenli_conut; i++)   //这里考虑到一种情况就是，如果评委所属学院是多个的话，那么在选择学院时就要给多个学院一起改变学院的标志
                        {
                            string bumen_fen = bumen_fenli[i];
                            bumenshezhi = db.bumenshezhis.Where(s => s.BmName == bumen_fen).FirstOrDefault();

                            bumenshezhi.pingweifou = false;   //评委状态改为bool值

                            bumenshezhi.pingweiname = null;

                            db.Entry(bumenshezhi).State = EntityState.Modified;   //保存对部门表的修改
                        }                  
                        db.SaveChanges();
                        transaction.Complete();
                        msg = "删除评委成功！";
                    }
                    else
                    {
                        zhuangtai = false;
                        foreach (var error in resultyg.Errors)
                        {
                            msg = msg + error;
                        }                       
                    };
                }
                catch (Exception ex)
                {
                    return Json(new { Succeeded = false, errorMsg = ex.ToString() }, "text/html");
                }
                finally
                {
                    transaction.Dispose();
                }
            }
            return Json(new { Succeeded = zhuangtai, errorMsg = msg }, "text/html");
        }


        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }
        
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken] 
        public async Task<ActionResult> Login_new(LoginViewModel model, string returnUrl)/*, */
        {     
            //获取验证码
            var validatecode = Session["code"].ToString();           
            //获取输入框验证码
            string code = Request.Form["userCode"];
            if (code.Length==0)
            {
                ModelState.AddModelError("", "请输入验证码。");
            }
            else
            {
                if (validatecode!= code)
                {
                    ModelState.AddModelError("", "验证码无效。");
                }
            }            
            denglurizhi denglu = new denglurizhi();
            if (ModelState.IsValid )   //先判断用户名然后判断密码
            {
                string username = model.name;

                var user_lgin = common.customidentity.User_Lock(username);  //判断用户名的 用户是否存在
                if (user_lgin != null)
                {
                    //就算密码正确，再登录之前还要判断用户是否处于再锁住状态,并且锁止时间是否在当前时间外
                    DateTime lock_time = Convert.ToDateTime(user_lgin.LockoutEndDateUtc);
                    if (lock_time!=null && lock_time>DateTime.Now )
                    {
                       ModelState.AddModelError("", "用户登录错误超过5次，已经被锁定，请"+lock_time+"后再试。");  //
                    }
                    else
                    {
                        var user = await UserManager.FindAsync(model.name, model.Password);   //用存在，根据用户名和密码匹配用户是否正确
                        DateTime logintime = DateTime.Now;   //获取登录时间

                        //这里直接得到正在使用的ipv4地址，简单粗暴
                        string ip2 = Dns.GetHostEntry(Dns.GetHostName()).AddressList.FirstOrDefault<IPAddress>(a => a.AddressFamily.ToString().Equals("InterNetwork")).ToString();

                        //用户存在，登录正确，初始化用户锁定信息  
                        if (user != null)
                        {
                            //保存登录日志
                            denglu.login_time = logintime;
                            denglu.username = username;
                            denglu.loginIP = ip2;
                            db1.denglurizhis.Add(denglu);
                            db1.SaveChanges();

                            await SignInAsync(user, model.RememberMe);

                            //正确登录的用户 初始化 控制登录错误的 三个 字段                        
                            user.AccessFailedCount = 0;
                            user.LockoutEnabled = false;
                            user.LockoutEndDateUtc = null;
                            IdentityResult result = await UserManager.UpdateAsync(user);

                            return RedirectToLocal(returnUrl);
                        }
                        else
                        {
                            //判断 密码登录错误次数，大于5次时锁定 用户
                            int failecount = user.AccessFailedCount;
                            if (true)
                            {
                                user.AccessFailedCount = +1;
                                user.LockoutEnabled = true;
                                user.LockoutEndDateUtc = DateTime.Now.AddMinutes(3);
                                IdentityResult result = await UserManager.UpdateAsync(user);
                                ModelState.AddModelError("", "");
                            }                          
                            ModelState.AddModelError("", "用户密码无效。");
                        }
                    }                    
                }
                else
                {
                    ModelState.AddModelError("", "用户名无效。");
                }                  
            }
            // 如果我们进行到这一步时某个地方出错，则重新显示表单
            return View(model);
        }



        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            //获取验证码
            var validatecode = Session["code"].ToString();
            //获取输入框验证码
            string code = Request.Form["userCode"];
            if (code.Length == 0)
            {
                ModelState.AddModelError("", "请输入验证码。");
            }
            else
            {
                if (validatecode != code)
                {
                    ModelState.AddModelError("", "验证码无效。");
                }
            }
            denglurizhi denglu = new denglurizhi();
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindAsync(model.name, model.Password);

                //string IP = Request.UserHostAddress;
                //string ip1;
                //string strHostName = Dns.GetHostName(); //得到本机的主机名
                //ArrayList alAllLocalIp = new ArrayList();
                ////IPHostEntry ipEntry = Dns.GetHostByName(strHostName); //取得本机IP  ，提示已过时
                //IPHostEntry ipEntry = Dns.GetHostEntry(strHostName);
                //for (int i = 0; i < ipEntry.AddressList.Length; i++)
                //{
                //    alAllLocalIp.Add(ipEntry.AddressList[i].ToString());
                //}
                ////string.Concat(al.ToArray()    
                //ip1 = string.Concat(alAllLocalIp.ToArray());   //这里把arraylist转化为字符串，但是链接出没有分隔符，同样可以在循环里 拼凑，这里得到的是 ipv6和ipv4

                string username = model.name;

                DateTime logintime = DateTime.Now;

                //这里直接得到正在使用的ipv4地址，简单粗暴
                string ip2 = Dns.GetHostEntry(Dns.GetHostName()).AddressList.FirstOrDefault<IPAddress>(a => a.AddressFamily.ToString().Equals("InterNetwork")).ToString();
                if (user != null)
                {
                    //保存登录日志
                    denglu.login_time = logintime;
                    denglu.username = username;
                    denglu.loginIP = ip2;
                    db1.denglurizhis.Add(denglu);
                    db1.SaveChanges();

                    await SignInAsync(user, model.RememberMe);
                    return RedirectToLocal(returnUrl);
                }
                else
                {
                    ModelState.AddModelError("", "用户名或密码无效。");
                }
            }
            // 如果我们进行到这一步时某个地方出错，则重新显示表单
            return View(model);
        }

        //
        // GET: /Account/Register
        //[AllowAnonymous]
        //public ActionResult Register()
        //{
        //    return View();
        //}

        //
        // POST: /Account/Register
        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> Register(RegisterViewModel model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var user = new ApplicationUser() { UserName = model.Email, Email = model.Email };
        //        IdentityResult result = await UserManager.CreateAsync(user, model.Password);
        //        if (result.Succeeded)
        //        {
        //            await SignInAsync(user, isPersistent: false);

        //            // 有关如何启用帐户确认和密码重置的详细信息，请访问 http://go.microsoft.com/fwlink/?LinkID=320771
        //            // 发送包含此链接的电子邮件
        //            // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
        //            // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
        //            // await UserManager.SendEmailAsync(user.Id, "确认你的帐户", "请通过单击 <a href=\"" + callbackUrl + "\">這裏</a>来确认你的帐户");

        //            return RedirectToAction("Index", "Home");
        //        }
        //        else
        //        {
        //            AddErrors(result);
        //        }
        //    }

        //    // 如果我们进行到这一步时某个地方出错，则重新显示表单
        //    return View(model);
        //}

        //
        // GET: /Account/ConfirmEmail
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null) 
            {
                return View("Error");
            }
            IdentityResult result = await UserManager.ConfirmEmailAsync(userId, code);
            if (result.Succeeded)
            {
                return View("ConfirmEmail");
            }
            else
            {
                AddErrors(result);
                return View();
            }
        }

        //
        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByNameAsync(model.Email);
                if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
                {
                    ModelState.AddModelError("", "用户不存在或未确认。");
                    return View();
                }

                // 有关如何启用帐户确认和密码重置的详细信息，请访问 http://go.microsoft.com/fwlink/?LinkID=320771
                // 发送包含此链接的电子邮件
                // string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                // var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);		
                // await UserManager.SendEmailAsync(user.Id, "重置密码", "请通过单击 <a href=\"" + callbackUrl + "\">此处</a>来重置你的密码");
                // return RedirectToAction("ForgotPasswordConfirmation", "Account");
            }
            // 如果我们进行到这一步时某个地方出错，则重新显示表单
            return View(model);
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }        

        //在左侧菜单栏添加修改用户自己的密码
        [AllowAnonymous]
        public ActionResult rsetpwdindex()
        {
            //ViewBag.userid = userid;            
            return View();
        }

        //
        // POST: /Account/ResetPassword
        // 重设密码
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByNameAsync(model.name);
                string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);//根据user.id产生code
                if (user == null)
                {
                    ModelState.AddModelError("", "找不到用户。");
                    return View();
                }
                IdentityResult result = await UserManager.ResetPasswordAsync(user.Id, code, model.Password);//ResetPasswordAsync方法需要有三个参数
                if (result.Succeeded)
                {
                    return RedirectToAction("ResetPasswordConfirmation", "Account");
                }
                else
                {
                    AddErrors(result);
                    return View();
                }
            }
            // 如果我们进行到这一步时某个地方出错，则重新显示表单
            return View(model);
        }


        //业务专员  初始化化密码
        //你可能不知道这一点，在.NET Framework 4.5.0  
        //版本中包含有一个关于 System.Transactions.TransactionScope 在与 async/await 一起工作时会产生的一个严重的 bug 。
        //由于这个错误，TransactionScope 不能在异步代码中正常操作，它可能更改事务的线程上下文，导致在处理事务作用域时抛出异常。
        //部门负责人初始化密码时使用 userid 找到用户，
        //业务专员要是使用 userid 找用户存在拼接 url 参数过长的问题，所以这里使用username 寻找用户
        [HttpPost]
        public JsonResult Ywzy_init_pw(string users)    //这里users传递过来的时 username 组成的数组
        {          
            if (ModelState.IsValid)
            {
                if (users.Contains(","))
                {
                    string[] users_fenli = users.Split(',');     //这里是由用户id 组成的 字符串数组
                    var users_fenli_count = users_fenli.Count();
                    //string pw = "pjmng@321cba";
                    string pw = common.Initial_pw.Init_pw();
                    using (TransactionScope transaction = new TransactionScope())//原子操作，事物错误回滚
                    {
                        try
                        {
                            for (int i = 0; i < users_fenli_count; i++)
                            {
                                string user_id = users_fenli[i];
                                //string user_id = common.customidentity.user_id(user_name);
                                string code = UserManager.GeneratePasswordResetToken(user_id);//根据user.id产生code,在forgotpassword中有
                                var user = UserManager.FindByIdAsync(user_id);//这里多一步，可以直接通过登录人信息取得用户id（主键）
                                if (user == null)
                                {
                                    return Json(new { success = false, errorMsg = "找不到用户。" }, "text/html");
                                }

                                IdentityResult result = UserManager.ResetPassword(user_id, code, pw);//ResetPasswordAsync方法需要有三个参数
                                                                                                     //IdentityResult re = UserManager.ResetPassword(user_id, code, "654321");
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
                    return Json(new { success = true, errorMsg = "OK！初始化密码成功！" }, "text/html");
                }
                else
                {
                    string code = UserManager.GeneratePasswordResetToken(users);//根据user.id产生code,在forgotpassword中有
                    var user = UserManager.FindByIdAsync(users);//这里多一步，可以直接通过登录人信息取得用户id（主键）
                    if (user == null)
                    {
                        return Json(new { success = false, errorMsg = "找不到用户。" }, "text/html");
                    }
                    string pw = common.Initial_pw.Init_pw();
                    IdentityResult result = UserManager.ResetPassword(users, code, pw);//ResetPasswordAsync方法需要有三个参数
                    //IdentityResult re = UserManager.ResetPassword(user_id, code, "654321");
                    
                    return Json(new { success = true, errorMsg = "OK！初始化密码成功！" }, "text/html");
                }                
            }
            else
            {
                return Json(new { success = false, errorMsg = "请选择要初始化密码的用户！" }, "text/html");
            }            
        }


        //部门负责人初始化化密码，初始化为123456
        //你可能不知道这一点，在.NET Framework 4.5.0  
        //版本中包含有一个关于 System.Transactions.TransactionScope 在与 async/await 一起工作时会产生的一个严重的 bug 。
        //由于这个错误，TransactionScope 不能在异步代码中正常操作，它可能更改事务的线程上下文，导致在处理事务作用域时抛出异常。
        [HttpPost]
        public JsonResult resetpassword_csh(string users)   //这里users传递过来的时 userid 组成的数组
        {
            if (ModelState.IsValid)
            {
                if (users.Contains(","))   //判断选中的为多位用户，前端用 ， 符号分隔两个用户名
                {
                    string[] users_fenli = users.Split(',');     //这里是由用户id 组成的 字符串数组
                    var users_fenli_count = users_fenli.Count();
                    //string pw = "pjmng@321cba";
                    string pw = common.Initial_pw.Init_pw();    //批量初始化密码为  这里 返回 的字符串，可以定期 修改密码，或者统一修改弱密码
                    using (TransactionScope transaction = new TransactionScope())//原子操作，事物错误回滚
                    {
                        try
                        {
                            for (int i = 0; i < users_fenli_count; i++)
                            {
                                string user_id = users_fenli[i];
                                string code = UserManager.GeneratePasswordResetToken(user_id);//根据user.id产生code,在forgotpassword中有
                                var user = UserManager.FindByIdAsync(user_id);//这里多一步，可以直接通过登录人信息取得用户id（主键）
                                if (user == null)
                                {
                                    return Json(new { success = false, errorMsg = "找不到用户。" }, "text/html");
                                }

                                IdentityResult result = UserManager.ResetPassword(user_id, code, pw);//ResetPasswordAsync方法需要有三个参数
                                //IdentityResult re = UserManager.ResetPassword(user_id, code, "654321");
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
                    return Json(new { success = true, errorMsg = "OK！初始化密码成功！" }, "text/html");
                }
                else   //判断选中的是只有一位用户
                {                    
                    //string pw = "pjmng@321cba";
                    string pw = common.Initial_pw.Init_pw();

                    string code = UserManager.GeneratePasswordResetToken(users);//根据user.id产生code,在forgotpassword中有
                    var user = UserManager.FindByIdAsync(users);//这里多一步，可以直接通过登录人信息取得用户id（主键）
                    if (user == null)
                    {
                        return Json(new { success = false, errorMsg = "找不到用户。" }, "text/html");
                    }

                    IdentityResult result = UserManager.ResetPassword(users, code, pw);//ResetPasswordAsync方法需要有三个参数
                    //IdentityResult re = UserManager.ResetPassword(user_id, code, "654321");
                    return Json(new { success = true, errorMsg = "OK！初始化密码成功！" }, "text/html");
                }                
            }
            else
            {
                return Json(new { success = false, errorMsg = "请选择要初始化密码的用户！" }, "text/html");
            }                               
        }


        //初始化化密码，初始化为123456
        //这里在刘冲要求  重置初始化密码为 pjmng@321cba  后就不再使用，这里实际上是一个 单一用户的修改密码
        [HttpPost]
        public async Task<JsonResult> resetpassword_csh_dange(string users)
        {
            if (ModelState.IsValid)
            {
                string errormsg = "";
                var user = await UserManager.FindByNameAsync(users);//这里多一步，可以直接通过登录人信息取得用户id（主键）

                string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);//根据user.id产生code,在forgotpassword中有

                if (user == null)
                {
                    return Json(new { success = false, errorMsg = "找不到用户。" }, "text/html");
                }
                string pw = common.Initial_pw.Init_pw();
                IdentityResult result = await UserManager.ResetPasswordAsync(user.Id, code, pw);//ResetPasswordAsync方法需要有三个参数
                if (result.Succeeded)
                {
                    return Json(new { success = true, errorMsg = "OK！初始化密码成功！" }, "text/html");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        errormsg = errormsg + error;
                    }
                    return Json(new { success = false, errorMsg = errormsg }, "text/html");
                }
            }
            return Json(new { success = false, errorMsg = "重置密码有误！" }, "text/html");
        }


        //登录人员修改密码
        [HttpPost]
        //[AllowAnonymous]
        public async Task<JsonResult> setpwd(string userid)   
        {
            if (ModelState.IsValid)
            {
                string errormsg = "";
                var user = await UserManager.FindByIdAsync(userid);//这里多一步，可以直接通过登录人信息取得用户id（主键）

                string password = Request.Form["password"];

                string code = await UserManager.GeneratePasswordResetTokenAsync(userid);//根据user.id产生code,在forgotpassword中有

                if (user == null)
                {
                    return Json(new { success=false,Msg = "找不到用户。" }, "text/html");
                }
                
                IdentityResult result = await UserManager.ResetPasswordAsync(user.Id, code, password);//ResetPasswordAsync方法需要有三个参数
                if (result.Succeeded)
                {

                    return Json(new { success = true, Msg = "修改密码成功！" }, "text/html");
                }
                else
                {
                    //AddErrors(result);
                    foreach (var error in result.Errors)
                    {
                        errormsg = errormsg + error;
                    }
                    return Json(new {success=false, errorMsg = errormsg });
                }
            }

            // 如果我们进行到这一步时某个地方出错，则重新显示表单
            //return View(model);
            return Json(new { success = false,Msg = "重置密码有误！" }, "text/html");
        }
       
        
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        //
        // POST: /Account/Disassociate
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Disassociate(string loginProvider, string providerKey)
        {
            ManageMessageId? message = null;
            IdentityResult result = await UserManager.RemoveLoginAsync(User.Identity.GetUserId(), new UserLoginInfo(loginProvider, providerKey));
            if (result.Succeeded)
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                await SignInAsync(user, isPersistent: false);
                message = ManageMessageId.RemoveLoginSuccess;
            }
            else
            {
                message = ManageMessageId.Error;
            }
            return RedirectToAction("Manage", new { Message = message });
        }

        //
        // GET: /Account/Manage
        public ActionResult Manage(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess ? "你的密码已更改。"    // ManageMessageId 是一个枚举类，判断枚举类型，返回文字提示
                : message == ManageMessageId.SetPasswordSuccess ? "已设置你的密码。"
                : message == ManageMessageId.RemoveLoginSuccess ? "已删除外部登录名。"
                : message == ManageMessageId.Error ? "出现错误。"
                : "";
            ViewBag.HasLocalPassword = HasPassword();  //这里返回一个bool值
            ViewBag.ReturnUrl = Url.Action("Manage");
            return View();
        }

        //
        // POST: /Account/Manage
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Manage(ManageUserViewModel model)
        {
            bool hasPassword = HasPassword();  // 如果用户存在，返回密码哈希值不为空，这里即使不传参数，也可以在后台 调用 登录用户的 Identity.GetUserId() 方法
            ViewBag.HasLocalPassword = hasPassword;
            ViewBag.ReturnUrl = Url.Action("Manage");  // 可以返回一个方法 链接
            if (hasPassword)
            {
                if (ModelState.IsValid)
                {
                    IdentityResult result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
                    if (result.Succeeded)
                    {
                        var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                        await SignInAsync(user, isPersistent: false);  //是否记录用户
                        return RedirectToAction("Manage", new { Message = ManageMessageId.ChangePasswordSuccess });
                    }
                    else
                    {
                        AddErrors(result);
                    }
                }
            }
            else
            {
                // 用户没有密码，因此将删除由于缺少 OldPassword 字段而导致的所有验证错误
                ModelState state = ModelState["OldPassword"];
                if (state != null)
                {
                    state.Errors.Clear();
                }

                if (ModelState.IsValid)
                {
                    IdentityResult result = await UserManager.AddPasswordAsync(User.Identity.GetUserId(), model.NewPassword);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Manage", new { Message = ManageMessageId.SetPasswordSuccess });
                    }
                    else
                    {
                        AddErrors(result);
                    }
                }
            }

            // 如果我们进行到这一步时某个地方出错，则重新显示表单
            return View(model);
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // 请求重定向到外部登录提供程序
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // 如果用户已具有登录名，则使用此外部登录提供程序将该用户登录
            var user = await UserManager.FindAsync(loginInfo.Login);
            if (user != null)
            {
                await SignInAsync(user, isPersistent: false);
                return RedirectToLocal(returnUrl);
            }
            else
            {
                // 如果用户没有帐户，则提示该用户创建帐户
                ViewBag.ReturnUrl = returnUrl;
                ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
            }
        }

        //
        // POST: /Account/LinkLogin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LinkLogin(string provider)
        {
            // 请求重定向到外部登录提供程序，以链接当前用户的登录名
            return new ChallengeResult(provider, Url.Action("LinkLoginCallback", "Account"), User.Identity.GetUserId());
        }

        //
        // GET: /Account/LinkLoginCallback
        public async Task<ActionResult> LinkLoginCallback()
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync(XsrfKey, User.Identity.GetUserId());
            if (loginInfo == null)
            {
                return RedirectToAction("Manage", new { Message = ManageMessageId.Error });
            }
            IdentityResult result = await UserManager.AddLoginAsync(User.Identity.GetUserId(), loginInfo.Login);
            if (result.Succeeded)
            {
                return RedirectToAction("Manage");
            }
            return RedirectToAction("Manage", new { Message = ManageMessageId.Error });
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Manage");
            }

            if (ModelState.IsValid)
            {
                // 从外部登录提供程序获取有关用户的信息
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new ApplicationUser() { UserName = model.Email, Email = model.Email };
                IdentityResult result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await SignInAsync(user, isPersistent: false);
                        
                        // 有关如何启用帐户确认和密码重置的详细信息，请访问 http://go.microsoft.com/fwlink/?LinkID=320771
                        // 发送包含此链接的电子邮件
                        // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                        // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                        // SendEmail(user.Email, callbackUrl, "确认你的帐户", "请单击此链接确认你的帐户");
                        
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut();
            return RedirectToAction("Index", "Home");
        }

        public ActionResult LogOff1()
        {
            AuthenticationManager.SignOut();
            return RedirectToAction("Index", "Home");
        }

        

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        [ChildActionOnly]
        public ActionResult RemoveAccountList()
        {
            var linkedAccounts = UserManager.GetLogins(User.Identity.GetUserId());
            ViewBag.ShowRemoveButton = HasPassword() || linkedAccounts.Count > 1;
            return (ActionResult)PartialView("_RemoveAccountPartial", linkedAccounts);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && UserManager != null)
            {
                UserManager.Dispose();
                UserManager = null;
            }
            base.Dispose(disposing);
        }

        #region 帮助程序
        // 用于在添加外部登录名时提供 XSRF 保护
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private async Task SignInAsync(ApplicationUser user, bool isPersistent)
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);
            AuthenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = isPersistent }, await user.GenerateUserIdentityAsync(UserManager));
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private bool HasPassword()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());  // 这里的登录人员 Identity参数，不用从前端传也能在后台获取，简化了
            if (user != null)
            {
                return user.PasswordHash != null;// 如果用户存在，返回密码哈希值不为空
            }
            return false;
        }

        private void SendEmail(string email, string callbackUrl, string subject, string message)
        {
            // 有关发送邮件的信息，请访问 http://go.microsoft.com/fwlink/?LinkID=320771
        }

        public enum ManageMessageId
        {
            ChangePasswordSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
            Error
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        private class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri) : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties() { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
    }
}