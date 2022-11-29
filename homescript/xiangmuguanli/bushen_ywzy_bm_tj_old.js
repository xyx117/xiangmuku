$(function () {

    var searchquery = "所有部门";

    $("#dg_shywzy_tg").datagrid({
        singleSelect: true,
        async: false,
        selectOnCheck: true,
        checkOnSelect: true,
        //async: true,
        collapsible: true,
        method: 'post',
        url: '/xiangmuguanli/ywzy_shenhe_bmtj',
        toolbar: '#tb_ywzy_toolbar',
        rownumbers: true,
        pagination: true,
        fit: true,
        autoRowheight: false,
        view: groupview,
        groupField: 'suoshuxueyuan',
        groupFormatter: function (value, rows) { return value + ' - ' + rows.length + ' 项'; },
        queryParams: {
            xiangmu: dgtitle,
            searchquery: searchquery
        },
        //为datagrid中的操作链接样式 设置一个按钮图形
        onLoadSuccess: function (data) {
            //查看
            //$('.chakan').linkbutton({ text: '查看', plain: true });
            //$('.chakan').addClass("easyui-linkbutton c6");

            $('.chehui').linkbutton({ text: '撤回', plain: true });
            $('.chehui').addClass("c6");
            $('.chehui_jin').linkbutton({ text: '撤回', disabled: true });


            //日志
            $('.rizhi').linkbutton({ text: '日志', plain: true });
            $('.rizhi').addClass("c6");

            //导出电子表格
            $('.daochu').linkbutton({ text: '导出excel', plain: true });
            $('.daochu').addClass("c6");

            //显示简介的详细内容
            $('.jianjie_all').tooltip({
                position: 'left',
                onShow: function () {
                    $(this).tooltip('tip').css({
                        width: '60%'
                    });
                }
            });
        }
    });


    var p = $('#dg_shywzy_tg').datagrid('getPager');

    $(p).pagination({

        pageSize: 10,//每页显示的记录条数，默认为10

        pageList: [5, 10, 15, 20, 25, 30, 35, 40, 45, 50, 55, 60, 65, 70, 75, 80, 85, 90, 95, 100],//可以设置每页记录条数的列表

        beforePageText: '第',//页数文本框前显示的汉字

        afterPageText: '页    共 {pages} 页',

        displayMsg: '当前显示 {from} - {to} 条记录   共 {total} 条记录',
    });
    $('#suoshuxueyuan').combobox({
        select: '所有部门',
        //singleSelect: false,
        onChange: function (newValue, oldValue) {

            $('#dg_shywzy_tg').datagrid('load', { "xiangmu": dgtitle, "searchquery": newValue });
        }
    });
});

//限定项目名称在tabtitle中的长度
function changdu(xm_name) {
    value1 = xm_name;
    if (value1.length > 15) {
        value1 = value1.substr(0, 10) + "...";
        //alert("jhjksssssssss");

    }
    return value1;
};


////当项目名称字数太长，限定只显示前面一部分
function TitleFormatter(value, row, index) {
    var value1 = value;
    if (value1 == null) {
        var ss = '<a href="#" title="' + value + '" class="easyui-tooltip"></a>';
        return ss
    }
    else {
        if (value1.length > 15) {
            value1 = value1.substr(0, 15) + "...";
        }
        var ss = '<a href="#" title="' + value + '" onclick="ywzychakan_tg(' + index + ')" class="easyui-tooltip">' + value1 + '</a>';
        return ss
    }
};



//当项目名称字数太长，限定只显示前面一部分
function TitleFormatter_jj(value, row, index) {
    if (value == null) {
        var ss = '';
        return ss
    }
    else {
        var value1 = value;
        if (value.length > 13) {
            value1 = value.substr(0, 13) + "...";
        }

        var ss = '<a href="javascript:;" title="' + value + '" class="jianjie_all">' + value1 + '</a>';
        return ss
    }
};


//员工界面部门负责人图标
function bmfzrtubiao(value, row, index) {
    var bmfzrshenhe = row.bmfzrshenhe;
    switch (bmfzrshenhe) {
        case "通过": return "<Image src='/Scripts/easyui/themes/icons/ok.png' Title='通过'/>";
            break;
        case "未审核": return "<Image src='/Scripts/easyui/themes/icons/question.png' Title='未审核'/>";
            break;
        case "未通过": return "<Image src='/Scripts/easyui/themes/icons/no.png' Title='未通过'/>";
            break;
    }
};

////员工界面分管领导人图标
//function fgldtubiao(value, row, index) {
//    var fgldshenhe = row.fgldshenhe;
//    switch (fgldshenhe) {
//        case "通过": return "<Image src='/Scripts/easyui/themes/icons/ok.png' Title='通过'/>";
//            break;
//        case "未审核": return "<Image src='/Scripts/easyui/themes/icons/question.png' Title='未审核'/>";
//            break;
//        case "未通过": return "<Image src='/Scripts/easyui/themes/icons/no.png' Title='未通过'/>";
//            break;
//    }
//};

//员工界面业务专员图标
function ywzybubiao(value, row, index) {
    var ywzyshenhe = row.ywzyshenhe;
    //var pingweishenhe = row.pingweishenhe;
    switch (ywzyshenhe) {
        case "通过": return "<Image src='/Scripts/easyui/themes/icons/ok.png' Title='通过'/>";
            break;
        case "未审核": return "<Image src='/Scripts/easyui/themes/icons/question.png' Title='未审核'/>";
            break;
        case "未通过": return "<Image src='/Scripts/easyui/themes/icons/no.png' Title='未通过'/>";
            break;
    }
};

//评委审核图标
function pwshenhetubiao(value, row, index) {
    var pingweishenhe = row.pingweishenhe;
    //var pingweishenhe = row.pingweishenhe;
    switch (pingweishenhe) {
        case "通过": return "<Image src='/Scripts/easyui/themes/icons/ok.png' Title='通过'/>";
            break;

        case "未审核": return "<Image src='/Scripts/easyui/themes/icons/question.png' Title='通过'/>";
            break;

        case "未通过": return "<Image src='/Scripts/easyui/themes/icons/no.png' Title='未通过'/>";
            break;
        case "撤回": return "<Image src='/Scripts/easyui/themes/icons/chehui.png' Title='撤回'/>";
            break;
    }
};



//业务专员查看
function ywzychakan_tg(index) {
    $('#dg_shywzy_tg').datagrid('selectRow', index);
    var row = $('#dg_shywzy_tg').datagrid('getSelected');
    if (row) {
        var name = row.XmName;
        var id = row.ID;
        var xmmulu = row.Xiangmumulu;
        //var tabTitle = name + "查看";
        var tabTitle = "查看：" + changdu(name);
        var url = "/xiangmuguanli/chakan?NoticeXmName=" + name + "&xmmulu=" + xmmulu + "&xmid=" + id;
        var icon = "icon-shenhe";
        window.parent.parent.addTab(tabTitle, url, icon);//使用新加的tab打开窗口
    }
};


//撤回部门负责人提交的项目
function xm_bm_tj_chehui(value, row, index) {

    var bmfzrtj = row.bmfzrtijiao;
    var pingweishenhe = row.pingweishenhe;


    if (bmfzrtj == "已提交" && pingweishenhe == "未审核") {           //部门负责人的完善信息，只有在未审核和未提交时候才能使用  chehui_jin
        return "<a href='javascript:void(0)' style='width:95%' class='chehui' onclick='ywzy_chehui(" + index + ")' target='mainFrame'>撤回</a>";
    }
    else {

        return "<a href='javascript:void(0)' style='width:95%' class='chehui_jin' >撤回</a>";
    }
};

//撤回部门负责人提交的项目，弹出对话框
function ywzy_chehui(index) {

    $('#dg_shywzy_tg').datagrid('selectRow', index);
    var row = $('#dg_shywzy_tg').datagrid('getSelected');
    if (row) {
        $('#ywzy_chehui').dialog('open').dialog('setTitle', '提示');

        var id = row.ID;
        url = "/xiangmuguanli/shenhe_ywzy_bmtj_chehui?xmid=" + id;
    }
};



//撤回点击确定保存
function ywzy_chehui_save() {
    $('#ywzy_chehui_fm').form('submit', {
        url: url,
        onSubmit: function () {
            return $(this).form('validate');
        },
        success: function (result) {

            result = JSON.parse(result);   //IE浏览器在后台操作完成后返回提示信息，转化为json字符串格式,在删除的方法中，加了这个后不能自动reload  dategrid

            if (result.success) {
                $('#ywzy_chehui').dialog('close');		// close the dialog
                $('#dg_shywzy_tg').datagrid('reload');

                $.messager.show({
                    title: '提示',
                    msg: result.Msg
                });
            }
            else {
                $.messager.show({
                    title: '提示',
                    msg: result.Msg
                });
            }
        }
    });
};



//审核流程日志
function ygxmrizhi_tg(value, row, index) {
    //alert("sdf")
    return "<a href='javascript:void(0)' style='width:95%' class='rizhi' onclick='rizhichakan_tg(" + index + ")' target='mainFrame'>日志</a>";

};
//审核流程日志
function rizhichakan_tg(index) {
    $('#dg_shywzy_tg').datagrid('selectRow', index);
    var row = $('#dg_shywzy_tg').datagrid('getSelected');
    if (row) {
        var xmname = row.XmName;
        var id = row.ID;
        var xmmulu = row.Xiangmumulu;
        //var tabTitle = name + "完善信息";
        var tabTitle = "流程：" + changdu(xmname);

        var url = "/xiangmuguanli/liucheng?xmname=" + xmname + "&loingid=" + loingid + "&xmid=" + id;

        var icon = "icon-edit";
        window.parent.parent.addTab(tabTitle, url, icon);//使用新加的tab打开窗口
    }
};


//项目导出电子表格
function daochu(value, row, index) {
    return "<a href='/xiangmuguanli/excel_out?id=" + row.ID + "' style='width:95%' class='daochu'>导出execl</a>";
};


//导出年度汇总表nianduhuizong
function nianduhuizong() {
    window.location.href = "ywzy_exl_huizong_bmtj?mulu=" + dgtitle;
};