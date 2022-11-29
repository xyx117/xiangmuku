using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace xmkgl.Models
{
    public class xiangmumulu

    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        // public int? ParenetID { get; set; }
        [Key]
        public string Name { get; set; }


        //分管领导是否审核
        public string fgxzshenhefou { get; set; }

        ////领导是否审核
        //public string ldshenhefou { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Kaishishijian { get; set; }


        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Jieshushijian { get; set; }


        //创建时间
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime chuangjian_time { get; set; }



        //public bool shifouxianzhishijian { get; set; }

        //public string Url { get; set; } 
        public string Beizhu { get; set; }
    }
}