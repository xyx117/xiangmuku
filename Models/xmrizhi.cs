using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace xmkgl.Models
{
    public class xmrizhi
    {

        [Key]
        //[ForeignKey("xiangmuguanli")]
        //[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int xmrizhiID { get; set; }


        public int xiangmuguanliID { get; set; }

        //审核节点
        public string shenhejiedian { get; set; }
        
        //审核结果
        public string shenhejieguo { get; set; }
        
        //审核人
        public string shenheren { get; set; }
        
        //审核角色
        public string shenhejuese { get; set; }
        
        //审核意见
        public string shenheyijian { get; set; }
        
        //审核时间
        public string shenheshijian { get; set; }


        //关联到项目管理表
        public virtual xiangmuguanli xiangmuguanli { get; set; }//这个延时加载暂时保留
    }
}