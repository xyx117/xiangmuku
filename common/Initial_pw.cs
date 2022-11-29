using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace xmkgl.common
{
    public class Initial_pw
    {
        //批量初始化密码为  这里的字符串，可以定期 修改密码，或者统一修改弱密码
        public static string Init_pw() 
        {
            string pw = "pjmng@321cba"; 
            //string pw = "654321";

            return pw;
        }
    }
}