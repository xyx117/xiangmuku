using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace xmkgl.Models
{
    public class xmnianduyusuan
    {
        [Key]
        public int xiangmuguanliID { get; set; }

        public string Xiangmumingcheng { get; set; }

        //所属部门
        public string suoshuxueyuan { get; set; }

        //后面这些字段需要填写
        //资金性质
        public string zijingxingzhi { get; set; }


        //起始年度
        public string qishiniandu { get; set; }

        //专款类别
        public string zhuankuanleibie { get; set; }


        //支出功能分类
        public string gongnengfenlei { get; set; }


        //第一年申报数
        public decimal diyinian { get; set; }

        //第二年申报数
        public decimal diernian { get; set; }

        //第三年申报数
        public decimal disannian { get; set; }

        //预算说明，项目简介
        public string yusuanshuoming { get; set; }

        public virtual xiangmuguanli xiangmuguanli { get; set; }
    }
}