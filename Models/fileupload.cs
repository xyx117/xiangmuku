using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace xmkgl.Models
{
    public class fileupload
    {
        [Key]
        //[ForeignKey("xiangmuguanli")]
        //[DatabaseGenerated(DatabaseGeneratedOption.Identity)]            //因为与项目总表是一对多的关系，所以不考虑使用主键自动加一
        public int fileuploadID { get; set; }

        public int xiangmuguanliID { get; set; }


        //文件大小
        public string filesize { get; set; }

        //上传时间
        //[DataType(DataType.Date)]
        //[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        //public DateTime uploadtime { get; set; }
        public string uploadtime { get; set; }


        //文件保存路径
        public string filepath { get; set; }
        // 原始文件名称
        public string filename { get; set; }
        // 文件扩展名
        public string fileextension { get; set; }

        // 保存文件名称
        public string savename { get; set; }


        //关联到项目管理表
        public virtual xiangmuguanli xiangmuguanli { get; set; }//这个延时加载暂时保留
    }
}