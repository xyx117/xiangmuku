using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace xmkgl.Models
{
    public class xiangmuguanli
    {   
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        //[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        //public int keyID { get; set; }

        //项目目录ID
        public int XiangmumuluID { get; set; }

        
        //项目名称   
        //[StringLength(100,ErrorMessage="最大不可以超过100个字符")]
        public string XmName { get; set; }


        //序号
        public int Xuhao {get; set;}



        [UIHint("HiddenInput")]
        // [ScaffoldColumn(false)] 隐藏此项，ScaffoldColumn  表示的是是否采用MVC框架来处理 设置为true表示采用MVC框架来处理，如果设置为false，则该字段不会在View层显示，里面定义的验证也不会生效。
        [StringLength(10, ErrorMessage = "不能超过5个汉字。")]
        [Display(Name = "录入日期")]
        public string Chuangjianshijian { get; set; }


        ////修改时间    
        [UIHint("HiddenInput")]
        // [ScaffoldColumn(false)] 隐藏此项，ScaffoldColumn  表示的是是否采用MVC框架来处理 设置为true表示采用MVC框架来处理，如果设置为false，则该字段不会在View层显示，里面定义的验证也不会生效。
        [StringLength(10, ErrorMessage = "不能超过5个汉字。")]
        public string Xiugaishijian { get; set; }


        //备注
        public string Beizhu { get; set; }


        //所属学院
        public string suoshuxueyuan { get; set; }


        //创建人人姓名
        public string username { get; set; }


        public string userid { get; set; }

        //项目目录
        public string Xiangmumulu { get; set; }


        //创建人项目提交状态
        public string tijiaozhuantai { get; set; }


        //部门负责人项目提交状态
        public string bmfzrtijiao { get; set; }

        //分管领导提交状态
        public string fgldtijiao { get; set; }


        //部门负责人审核
        public string bmfzrshenhe { get; set; }

        //分管临到审核
        public string fgldshenhe { get; set; }

        //业务专员审核
        public string ywzyshenhe { get; set; }


        //部门负责人审核意见
        public string bmfzryijian { get; set; }

        //分管领导审核意见
        public string fgldyijian { get; set; }

        //业务专员审核意见
        public string ywzyyijian { get; set; }

        //评委审核
        public string pingweishenhe { get; set; }

        //评委意见
        public string pingweiyijian { get; set; }

        //评委提交状态
        public string pingweitijiao { get; set; }


        ////领导审核意见
        //public string ldshenhe { get; set; }
        
        ////领导意见
        //public string ldyijian { get; set; }



        ////分管领导是否参与审核
        //public string fgldshifoushenhe { get; set; }
        
        //项目总金额
        public decimal zongjine { get; set; }

        //审核状态
        public string shenhezhuangtai { get; set; }

        ////部门负责人审核
        //public shenhejieguo bmfzrshenhe { get; set; }
        
        ////分管领导审核
        //public shenhejieguo fgldshenhe { get; set; }

        ////业务专员审核
        //public shenhejieguo ywzyshenhe { get; set; }

       


        //标识基本信息表
        public string jpxx { get; set; }

        //标识产出绩效表
        public string ccjx { get; set; }

        //标识效率绩效表
        public string xljx { get; set; }    
        
        //标识测算明细表
        public string csmx { get; set; }
        
        //标识立项评级表
        public string lxpj { get; set; }

        //标识采购配置表
        public string cgpz { get; set; }

        //产出成效
        public string cccx { get; set; }

        //分年度预算
        public string nianduyusuan { get; set; }

        //采购配置空或非空，是否空表申报
        public bool cgpz_nll { get; set; }


        //校内多部门评审意见备注
        public string xiaoneibeizhu { get; set; }


        //项目基本表
        public virtual xmjibenxinxi xmjibenxinxi { get; set; }

        //项目绩效产出表,这是一个多对一的表， 一条项目记录要对应着绩效表中的多条记录指标
        public virtual ICollection<xmjixiao_cc> xmjixiao_cc { get; set; }

        //项目绩效指标表
        public virtual ICollection<xmjixiao_xl> xmjixiao_xl { get; set; }


        //项目测算明细表 --1对多关系
        public virtual ICollection<xmcsmx> xmcsmxs { get; set; }

        ////项目立项评级表
        //[ForeignKey("xiangmuguanli")]
        public virtual xmlxpj xmlxpj { get; set; }        //测试的时候，这个没有使用还是没有报错，具体原因以后再研究

        
        //项目采购配置表 --1对多关系
        public virtual ICollection<xmcgpz> xmcgpzs { get; set; }


        //项目文件导入表 --1对多关系
        public virtual ICollection<fileupload> fileuploads { get; set; }

        //项目日志表，---一对多关系
        public virtual ICollection<xmrizhi> xmrizhi { get; set; }


        //项目产出成效，----一对一关系
        public virtual xmchanchuchengxiao xmchanchuchengxiao { get; set; }

        //项目分年度预算 -----一对一关系
        public virtual xmnianduyusuan xmnianduyusuan { get; set; }
    }


    //public enum shenhejieguo
    //{
    //    未审核,
    //    未通过,
    //    通过      
    //}

}