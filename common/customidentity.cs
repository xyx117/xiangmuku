using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using xmkgl.Models;

namespace xmkgl.common
{
    public  class customidentity
    {       
       //这里是根据登录人的id取出登录人的所属学院，在登录人创建项目时把登录人的所属学院赋值给项目。在xiangmuguanliindex.cshtml中也要根据登录人的所属学院信息显示对应学院信息
        public static string suoshuxueyuan(string loingid)
        {
            using (ApplicationDbContext identitydb1 = new ApplicationDbContext()) //创建一个新的上下文
            {
                ApplicationUser xueyuan = identitydb1.Users.Where(s => s.Id == loingid).FirstOrDefault();
                return (xueyuan.suoshuxueyuan);
            }
        }

        //部门负责人初始化密码时使用 userid 找到用户，
        //业务专员要是使用 userid 找用户存在拼接 url 参数过长的问题，所以这里使用username 寻找用户
        public static string user_id(string user_name)
        {
            using (ApplicationDbContext identitydb1 = new ApplicationDbContext()) //创建一个新的上下文
            {

                ApplicationUser user = identitydb1.Users.Where(s => s.UserName == user_name).FirstOrDefault();
                return (user.Id);
            }
        }

        //这里是在 导出电子表格 的时候，为  基本信息表  的  负责人单元格取值，具体是根据项目所在的 部门  ，从  用户表 中取出
        //负责 该项目的 部门负责人， 条件是  角色为部门负责人且学院是该学院
        public static string fuzeren(string suoshuxueyuan)
        {
            using (ApplicationDbContext identitydb1 = new ApplicationDbContext()) //创建一个新的上下文
            {
                ApplicationUser renyuan = identitydb1.Users.Where(s => s.suoshuxueyuan == suoshuxueyuan && s.role == "部门负责人").FirstOrDefault();
                return (renyuan.zhenshiname);
            }            
        }


        //评委登录后，评委只看到他所评审的目录
        public static string pingshenmulu(string loingid)
        {
            using (ApplicationDbContext db1 = new ApplicationDbContext()) //创建一个新的上下文
            {
                ApplicationUser pingwei = db1.Users.Where(s => s.Id == loingid).FirstOrDefault();
                return pingwei.pingshenmulu;
            }
        }

        //在提取评委评审进度的时候使用到 pwjd_piechart
        public static ApplicationUser pingwei(string username)
        {
            using (ApplicationDbContext identitydb1 = new ApplicationDbContext()) //创建一个新的上下文
            {

                ApplicationUser pingwei = identitydb1.Users.Where(s => s.UserName == username).FirstOrDefault();
                return (pingwei);
            }
        }

        //根据登录的loingid，取出用户的名称,这里在天蝎日志表时用到
        public static string rizhiusername(string loingid)
        {
            using (ApplicationDbContext identitydb1 = new ApplicationDbContext()) //创建一个新的上下文
            {

                ApplicationUser name = identitydb1.Users.Where(s => s.Id == loingid).FirstOrDefault();
                return (name.UserName);
            }
        }

        //根据登录的loingid，取出用户的角色,这里在天蝎日志表时用到
        public static string rizhiuserrole(string loingid)
        {
            using (ApplicationDbContext identitydb1 = new ApplicationDbContext()) //创建一个新的上下文
            {

                ApplicationUser role = identitydb1.Users.Where(s => s.Id == loingid).FirstOrDefault();
                return (role.role);
            }
        }


        //由用户登录id判断用户所属的角色，在部门负责人保存所创建项目时使用
        public static string userrole(string loingid)
        {

            using (ApplicationDbContext identitydb1 = new ApplicationDbContext()) //创建一个新的上下文
            {

                ApplicationUser user = identitydb1.Users.Where(s => s.Id == loingid).FirstOrDefault();
                return (user.role);
            }
        }


        //由创建项目时的username，判读username的角色
        public static string userrole_name(string name)
        {
            using (ApplicationDbContext identitydb1 = new ApplicationDbContext()) //创建一个新的上下文
            {

                ApplicationUser user = identitydb1.Users.Where(s => s.UserName == name).FirstOrDefault();
                return (user.role);
            }
        }

        
        //登录错误的时候，根据登录者的用户名 ，为锁定用户，查找用户
        public static ApplicationUser User_Lock(string username)
        {
            using (ApplicationDbContext identitydb1 = new ApplicationDbContext()) //创建一个新的上下文
            {
                //这里需要判断用户是否存在，存在就返回一个用户，不存在就返回提示信息
                ApplicationUser user = identitydb1.Users.Where(s => s.UserName == username).FirstOrDefault();
                return user;                             
            }
        }

        public static string Sha256(string plainText)
        {
            SHA256Managed _sha256 = new SHA256Managed();
            byte[] _cipherText = _sha256.ComputeHash(Encoding.Default.GetBytes(plainText));
            return Convert.ToBase64String(_cipherText);
        }
    }
}