using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace xmkgl.Models
{
    public class bumenshezhi
    {
        [Key]
        [Required]
        public string BmName { get; set; }


        //部门性质
        [Required]
        public string Bmxingzhi { get; set; }


        //部门是否有负责的评委
        //public string pingweifou { get; set; }
        public bool pingweifou { get; set; }

        //评委名
        public string pingweiname { get; set; }
    }
}