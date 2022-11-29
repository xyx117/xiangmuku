using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace xmkgl.Models
{
    public class denglurizhi
    {
        [Key]
       
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        //登录IP地址
        public string loginIP { get; set; }

        //登录用户名
        public string username { get; set; }

        //登录时间
        public DateTime login_time { get; set; }
    }
}