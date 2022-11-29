using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using xmkgl.Models;

namespace xmkgl.DAL
{
    public class ElmahContent : DbContext
    {
        public ElmahContent()
            : base("cwcxmkContent_elmah")
        {
            //this.Configuration.ProxyCreationEnabled = false;     //关闭关联的子表
        }
        public DbSet<denglurizhi> denglurizhis { get; set; }
    }
}