using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace xmkgl.Models
{
    public class xmcgpz
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int xmcgpzID { get; set; }

        public int xiangmuguanliID { get; set; }

        //资产名称
        public string zichanmingcheng { get; set; }

        //规格型号
        public string guigexinghao { get; set; }

        ////配置方式
        //public string peizhifangshi { get; set; }

        //配置数量
        public decimal peizhishuliang { get; set; }

        //单价
        public decimal danjia { get; set; }

        
        ////资金性质
        //public string zijinxingzhi { get; set; }

        //资产存量
        public string zichancunliang { get; set; }

        //采购说明和配置资产申请理由
        public string caigoushuoming { get; set; }


        //增加分年度预算表时增加的 年度字段
        public string suoshuniandu { get; set; }

        //关联到项目管理表
        public virtual xiangmuguanli xiangmuguanli { get; set; }
    }
}