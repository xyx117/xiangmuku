using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace xmkgl.Models
{
    public class News
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
       
        [Required(ErrorMessage = "必填")]
        [StringLength(60)]
        [Display(Name = "标题：")]
        public string title { get; set; }

        [Display(Name = "内容：")]
        [DataType(DataType.MultilineText)]
        public string Content { get; set; }

        [Display(Name = "发布时间：")]
        public DateTime publishtime { get; set; }
        
        
        [StringLength(20)]
        [Display(Name = "发布者：")]
        public string author { get; set; }

    }
}