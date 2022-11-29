using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using xmkgl.Models;

namespace xmkgl.DAL
{
    public class cwcxmkContent : DbContext
    {
        public cwcxmkContent(): base("cwcxmkContent")
        {
            this.Configuration.ProxyCreationEnabled = false;     //关闭关联的子表
        }

        public DbSet<News> news { get; set; }
        public DbSet<xiangmumulu> xiangmumulus { get; set; }

        public DbSet<bumenshezhi> bumenshezhis { get; set; }

        public DbSet<bumenxingzhi> bumenxingzhis { get; set; }

        public DbSet<xiangmuguanli> xiangmuguanlis { get; set; }

        public DbSet<xmjibenxinxi> xmjibenxinxis { get; set; }

        public DbSet<xmjixiao_cc> xmjixiao_ccs { get; set; }

        public DbSet<xmjixiao_xl> xmjixiao_xls { get; set; }

        public DbSet<xmcsmx> xmcsmxs { get; set; }

        public DbSet<xmlxpj> xmlxpjs { get; set; }


        public DbSet<xmcgpz> xmcgpzs { get; set; }

        public DbSet<xmnianduyusuan> xmnianduyusuans { get; set; }

        public DbSet<fileupload> fileuploads { get; set; }

        public DbSet<xmrizhi> xmrizhis { get; set; }

        //日志较多，存放地址改为elmah的数据库
        //public DbSet<denglurizhi> denglurizhis { get; set; }

        public DbSet<xmchanchuchengxiao> xmchanchuchengxiao { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            ////项目基本信息表WillCascadeOnDelete(false)
            modelBuilder.Entity<xmjibenxinxi>().HasRequired(p => p.xiangmuguanli).WithOptional(p => p.xmjibenxinxi).WillCascadeOnDelete(true);//.WillCascadeOnDelete(true)启动级联删除
           

            //modelBuilder.Entity<xiangmuguanli>().HasRequired(p => p.xmjibenxinxi).WithOptional(p => p.xiangmuguanli);

            modelBuilder.Entity<xiangmuguanli>().Property(p => p.zongjine).HasPrecision(18, 3);

            ////项目绩效产出指标
            //modelBuilder.Entity<xmjixiao_cc>().HasRequired(p => p.xiangmuguanli).WithOptional(p => p.xmjixiao_cc);

            ////项目绩效效率指标
            //modelBuilder.Entity<xmjixiao_xl>().HasRequired(p => p.xiangmuguanli).WithOptional(p => p.xmjixiao_xl);


            ////项目立项评级表
            modelBuilder.Entity<xmlxpj>().HasRequired(p => p.xiangmuguanli).WithRequiredDependent(p => p.xmlxpj).WillCascadeOnDelete(true);//.WillCascadeOnDelete(true)启动级联删除


            ////项目产出成效启动级联删除
            modelBuilder.Entity<xmchanchuchengxiao>().HasRequired(p => p.xiangmuguanli).WithOptional(p => p.xmchanchuchengxiao).WillCascadeOnDelete(true);//.WillCascadeOnDelete(true)启动级联删除

            //项目分年度预算启动级联删除
            modelBuilder.Entity<xmnianduyusuan>().HasRequired(p => p.xiangmuguanli).WithOptional(p => p.xmnianduyusuan).WillCascadeOnDelete(true);

           
            //项目测算明细表：一对多的关系


            //建立树的父子节点关系
            //modelBuilder.Entity<TreeNode>()
            //        .HasMany(t => t.Childs)
            //        .WithOptional(t => t.Father)
            //        .HasForeignKey(t => t.ParenetID);

        }

    }
}