using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace xmkgl.Models
{
    public class xmjixiao_cc
    {
        [Key]
        //[ForeignKey("xiangmuguanli")]
        //[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int xmjixiao_ccID { get; set; }

        public int xiangmuguanliID { get; set; }


        //项目编码
        public string Xiangmubiama { get; set; }

        //项目名称
        public string Xiangmumingcheng { get; set; }

        //绩效指标
        public string jixiaozhibiao { get; set; }

        //绩效目标
        public string jixiaomubiao { get; set; }

        //绩效标准优
        public string jixiaoyou { get; set; }
        //绩效标准良
        public string jixiaoliang { get; set; }
        //绩效标准中
        public string jixiaozhong { get; set; }
        //绩效标准差
        public string jixiaocha { get; set; }


        //关联到项目管理表
        public virtual xiangmuguanli xiangmuguanli { get; set; }//这个延时加载暂时保留
        
    }
}