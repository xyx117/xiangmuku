using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace xmkgl.Models
{
    public class xmchanchuchengxiao
    {
        [Key]
        public int xiangmuguanliID { get; set; }


        //项目名称
        public string xmchanchu { get; set; }

        public string xmchengxiao { get; set; }

        public virtual xiangmuguanli xiangmuguanli { get; set; }
    }
}