using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace xmkgl.Models
{
    //项目测算明细表，这是一对多
    public class xmcsmx
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int xmcsmxID { get; set; }

        //关联字段
        public int xiangmuguanliID { get; set; }

        //项目编码
        //public string Xiangmubiama { get; set; }

        //序号
        public string xuhao { get; set; }


        //项目明细活动
        public string xiangmumingxi { get; set; }

        //支出经济分类
        //public string zhichufenlei { get; set; }

        //申报数
        public decimal shenbaoshu { get; set; }

        //成本标准
        public decimal chengbenbiaozhun { get; set; }

        //工作量
        public decimal gongzuoliang { get; set; }


        //工作量单位
        public string gzl_danwei { get; set; }



        //测算依据依据或说明
        public string yijushuoming { get; set; }


        //增加分年度预算表时增加的 年度字段
        public string suoshuniandu { get; set; }


        //关联到项目管理表
        public virtual xiangmuguanli xiangmuguanli { get; set; }
    }
}