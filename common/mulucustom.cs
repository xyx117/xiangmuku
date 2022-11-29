using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using xmkgl.DAL;
using xmkgl.Models;

namespace xmkgl.common
{
    public class mulucustom
    {
        //private static cwcxmkContent db = new cwcxmkContent();

        //判断保存项目时 是否在有限期内
        public static bool muluyouxiaoqi(string muluname)
        {            
            DateTime kaishi;
            DateTime jieshu;

            ////这里为了避免linq缓存影响修改后的结果
            using (cwcxmkContent db1 = new cwcxmkContent()) //创建一个新的上下文
            {
                xiangmumulu mulu = db1.xiangmumulus.Find(muluname);
                kaishi = mulu.Kaishishijian;
                jieshu = mulu.Jieshushijian;
            }
            
            if (DateTime.Now >= kaishi && DateTime.Now <= jieshu)
            {
                return true;
            }
            return false;
        }


        //判断分管校长是否审核
        public static string fgxzshenhefou(string xiangmu)
        {
            xiangmumulu mulu = new xiangmumulu();
            using (cwcxmkContent db1 = new cwcxmkContent()) //创建一个新的上下文
            {
                mulu = db1.xiangmumulus.Find(xiangmu);
            }
            return mulu.fgxzshenhefou;
        }
        
        //在业务专员删除目录时使用，判断所要删除的目录底下是否有项目
        public static bool muluisempty(string muluname)
        {
            using (cwcxmkContent db1 = new cwcxmkContent()) //创建一个新的上下文
            {

                var xiangmu = db1.xiangmuguanlis.Where(s => s.Xiangmumulu == muluname);

                if (xiangmu.Count() > 0)
                {
                    return false;
                }
            }
            return true;
        }

        //判断项目是否已经完善
        public static bool shifouwanshan(int xmid)
        {
            string jpxx = "0";
            string ccjx = "0";
            string xljx = "0";
            string csmx = "0";
            string lxpj = "0";
            string cgpz = "0";
            string cccx = "0";
            string ndys = "0";
            using (cwcxmkContent db1 = new cwcxmkContent()) //创建一个新的上下文
            {
                var xiangmu = db1.xiangmuguanlis.Where(s => s.ID == xmid).FirstOrDefault();
                jpxx = xiangmu.jpxx;
                ccjx = xiangmu.ccjx;
                xljx = xiangmu.xljx;
                csmx = xiangmu.csmx;
                lxpj = xiangmu.lxpj;
                cgpz = xiangmu.cgpz;
                cccx = xiangmu.cccx;
                ndys = xiangmu.nianduyusuan;
            }
            if (jpxx == "1" && ccjx == "1" && xljx == "1" && csmx == "1" && lxpj == "1" && cgpz == "1" && cccx == "1"&&ndys=="1")
            {
                return true;
            }
            return false;
        }


        //判断哪一个表没有完善，提示给用户
        public static int biao_kongbiao(int xmid)
        {
            string jpxx = "0";
            string ccjx = "0";
            string xljx = "0";
            string csmx = "0";
            string lxpj = "0";
            string cgpz = "0";
            string cccx = "0";
            string ndys = "0";
            using (cwcxmkContent db1 = new cwcxmkContent()) //创建一个新的上下文
            {
                var xiangmu = db1.xiangmuguanlis.Where(s => s.ID == xmid).FirstOrDefault();
                jpxx = xiangmu.jpxx;
                ccjx = xiangmu.ccjx;
                xljx = xiangmu.xljx;
                csmx = xiangmu.csmx;
                lxpj = xiangmu.lxpj;
                cgpz = xiangmu.cgpz;
                cccx = xiangmu.cccx;
                ndys = xiangmu.nianduyusuan;
            }
            if (jpxx == "0")
            {
                return 1;
            }
            if (ccjx == "0")
            {
                return 2;
            }
            if ( xljx == "0" )
            {
                return 3;
            }
            if ( csmx == "0")
            {
                return 4;
            }
            if ( lxpj == "0" )
            {
                return 5;
            }
            if ( cgpz == "0")
            {
                return 6;
            }
            if (cccx == "0" )
            {
                return 7;
            }
            return 8;
        }
    }
}