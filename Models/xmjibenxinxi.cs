using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace xmkgl.Models
{
    public class xmjibenxinxi
    {


        [Key]
        //[DatabaseGenerated(DatabaseGeneratedOption.Identity)]//蓝色是属性，红灰色是方法
        //public int ID { get; set; }

        //[DatabaseGenerated(DatabaseGeneratedOption.Identity)]//蓝色是属性，红灰色是方法
                                //在一对一关系是foreignKey可以建立在    关系主体和依赖类
        public int xiangmuguanliID { get; set; }


        //项目名称
        [Display(Name = "项目名称")]
        public string Xiangmumingcheng { get; set; }


        //起始时间
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "开始日期")]
        public DateTime Kaishishijian { get; set; }


        //结束时间
        [DataType(DataType.Date)]
        [Display(Name = "结束日期")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Jieshushijian { get; set; }


        //政策依据
        [Display(Name = "政策依据")]
        public string Zhengceyiju { get; set; }


        //项目背景
        [Display(Name = "项目背景")]
        public string Xiangmubeijing { get; set; }       


        //实施地址
        [Display(Name = "实施地址")]
        public string shishidizhi { get; set; }

        //联系电话
        [Display(Name = "联系电话")]
        public string Lianxidianhua { get; set; }

        //经办人
        [Display(Name = "经办人")]
        public string jingbanren { get; set; }       
      

        //[ForeignKey("xiangmuguanli")]
        public virtual xiangmuguanli xiangmuguanli { get; set; }
    }
}