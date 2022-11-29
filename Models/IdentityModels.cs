using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations.Schema;

namespace xmkgl.Models
{
    // 可以通过向 ApplicationUser 类添加更多属性来为用户添加配置文件数据。若要了解详细信息，请访问 http://go.microsoft.com/fwlink/?LinkID=317594。
    public class ApplicationUser : IdentityUser
    {
        //添加自定义属性
        public string suoshuxueyuan { get; set; }

        public int parentID { get; set; }

        public string zhenshiname { get; set; }

        public string role { get; set; }

        //myuserID 和parentID为父子关系
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int myuserID { get; set; }
        public int usercount { get; set; }

        public string pingshenmulu { get; set;}


        //评委评审任务
        public string pingshenrenwu { get; set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // 请注意，authenticationType 必须与 CookieAuthenticationOptions.AuthenticationType 中定义的相应项匹配
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // 在此处添加自定义用户声明
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("IdentityDb", throwIfV1Schema: false)
        {
        }

        static ApplicationDbContext()
       {
           Database.SetInitializer<ApplicationDbContext>(new IdentityDbInit());
       }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }

    public class IdentityDbInit : DropCreateDatabaseIfModelChanges<ApplicationDbContext>
    {
        protected override void Seed(ApplicationDbContext context)
        {
            PerformInitialSetup(context);
            base.Seed(context);
        }
        public void PerformInitialSetup(ApplicationDbContext context)
        {
            //初始化
            
            ApplicationUserManager userMgr = new ApplicationUserManager(new UserStore<ApplicationUser>(context));
            AppRoleManager roleMgr = new AppRoleManager(new RoleStore<AppRole>(context));

            //int[] numbers = new int[]{1,2,3,4,5,6};
            string[] roleNames = new string[]{ "员工", "部门负责人", "业务专员", "财务主管", "分管领导", "领导" ,"评委","校内评审"};

            for (int i = 0; i <roleNames.Length; i++)
            {
                if (!roleMgr.RoleExists(roleNames[i]))
                {
                    roleMgr.Create(new AppRole(roleNames[i]));
                }
            }            

            string userName = "admin";
            string password = "123456";
            string email = "admin@example.com";

            ApplicationUser user = userMgr.FindByName(userName);
            if (user == null)
            {

                //UserName = model.name, suoshuxueyuan = xueyuan, Email = model.name + "@qq.com", zhenshiname = model.zhenshiname, role = model.role, parentID = -1, usercount = 0
                userMgr.Create(new ApplicationUser { UserName = userName, Email = email, suoshuxueyuan="all",zhenshiname ="admin",role="业务专员",parentID=-1,usercount=0 },
                    password);
                user = userMgr.FindByName(userName);
            }

            if (!userMgr.IsInRole(user.Id, roleNames[2]))//在这里判断admin是不是在数组中的角色，不是就添加到第三下标的角色
            {
                userMgr.AddToRole(user.Id, roleNames[2]);
            }

            //foreach (ApplicationUser dbUser in userMgr.Users)
            //{
            //    dbUser.City = Cities.PARIS;
            //}
            context.SaveChanges();

        }
    }
}