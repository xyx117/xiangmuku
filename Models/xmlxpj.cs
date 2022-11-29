using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace xmkgl.Models
{
    //项目立项评级表
    public class xmlxpj
    {
        [Key]
        //[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        //[ForeignKey("xiangmuguanli")]
        public int xiangmuguanliID { get; set; }

        public string one_1_1 { get; set; }

        public int one_1_1_f { get; set; }

        public string one_1_2 { get; set; }

        public int one_1_2_f { get; set; }

        public string one_1_3 { get; set; }

        public int one_1_3_f { get; set; }


        public string one_2_1 { get; set; }
        public int one_2_1_f { get; set; }

        public string one_2_2 { get; set; }

        public int one_2_2_f { get; set; }


        public string one_3_1 { get; set; }
        public int one_3_1_f { get; set; }
        public string one_3_2 { get; set; }
        public int one_3_2_f { get; set; }
        public string one_3_3 { get; set; }
        public int one_3_3_f { get; set; }


        public string one_4_1 { get; set; }
        public int one_4_1_f { get; set; }
        public string one_4_2 { get; set; }
        public int one_4_2_f { get; set; }


        public string two_1_1 { get; set; }

        public int two_1_1_f { get; set; }
        public string two_1_2 { get; set; }
        public int two_1_2_f { get; set; }



        public string two_2_1 { get; set; }
        public int two_2_1_f { get; set; }
        public string two_2_2 { get; set; }
        public int two_2_2_f { get; set; }
        public string two_2_3 { get; set; }
        public int two_2_3_f { get; set; }


        public string two_3_1 { get; set; }
        public int two_3_1_f { get; set; }
        public string two_3_2 { get; set; }
        public int two_3_2_f { get; set; }
        public string two_3_3 { get; set; }
        public int two_3_3_f { get; set; }


        public string two_4_1 { get; set; }
        public int two_4_1_f { get; set; }
        public string two_4_2 { get; set; }
        public int two_4_2_f { get; set; }


        public string three_1_1 { get; set; }
        public int three_1_1_f { get; set; }
        public string three_1_2 { get; set; }
        public int three_1_2_f { get; set; }


        public string three_2_1 { get; set; }
        public int three_2_1_f { get; set; }
        public string three_2_2 { get; set; }
        public int three_2_2_f { get; set; }
        public string three_2_3 { get; set; }
        public int three_2_3_f { get; set; }


        public string four_1_1 { get; set; }
        public int four_1_1_f { get; set; }
        public string four_1_2 { get; set; }
        public int four_1_2_f { get; set; }

        public int zongfen { get; set; }

        public string xmdengji { get; set; }        

        //关联到项目管理表,
        //在测试时，基本信息表示没有加virtual的，所以这里先暂时不加virtual
        //[ForeignKey("xiangmuguanli")]
        public xiangmuguanli xiangmuguanli { get; set; }
    }
}