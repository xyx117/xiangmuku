$(function () {

    var searchquery = "所有部门";

    $("#dg_shywzy_tg").datagrid({
        singleSelect: false,
        async: false,
        selectOnCheck: true,
        checkOnSelect: true,
        //async: true,
        collapsible: true,
        method: 'post',
        url: '/xiangmuguanli/ywzy_shenhe_xm_tg',
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
            searchquery:searchquery
        },
        //为datagrid中的操作链接样式 设置一个按钮图形
        onLoadSuccess: function (data) {
            //查看
            //$('.chakan').linkbutton({ text: '查看', plain: true });
            //$('.chakan').addClass("easyui-linkbutton c6");

                
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
        value1 = value1.substr(0, 15) + "...";
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

//员工界面分管领导人图标
function fgldtubiao(value, row, index) {
    var fgldshenhe = row.fgldshenhe;
    switch (fgldshenhe) {
        case "通过": return "<Image src='/Scripts/easyui/themes/icons/ok.png' Title='通过'/>";
            break;
        case "未审核": return "<Image src='/Scripts/easyui/themes/icons/question.png' Title='未审核'/>";
            break;
        case "未通过": return "<Image src='/Scripts/easyui/themes/icons/no.png' Title='未通过'/>";
            break;
    }
};

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

        case "未通过": return "<Image src='/Scripts/easyui/themes/icons/no.png' Title='未通过'/>";
            break;
        case "撤回": return "<Image src='/Scripts/easyui/themes/icons/chehui.png' Title='撤回'/>";
            break;
    }
};



//业务专员 查看通过  的脚本
//function ywzy_chakan_tg(value, row, index) {
//    return "<a href='javascript:void(0)' style='width:60%' class='chakan' onclick='ywzychakan_tg(" + index + ")' target='mainFrame'>查看</a>";
//};

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

        var url = "/xiangmuguanli/liucheng?xmname=" + xmname + "&loingid="+loingid+"&xmid="+id;

        var icon = "icon-edit";
        window.parent.parent.addTab(tabTitle, url, icon);//使用新加的tab打开窗口
    }
};


//项目导出电子表格
function daochu(value, row, index) {
    return "<a href='/xiangmuguanli/excel_out?id=" + row.ID + "' style='width:90%' class='daochu'>导出execl</a>";
};

//业务专员导出word
//function ywzy_word(index) {

//    var checkedItems = $('#dg_shywzy_tg').datagrid('getChecked');

//    var ID_str = [];
//    //var names = {};
//    if (checkedItems.length == 0) {

//        $.messager.alert("错误提示", "请选择要导出的项目！", "warning");
//    }
//    else {
//        if (checkedItems) {

//            $.each(checkedItems, function (index, item) {

//                ID_str.push(item.ID);         //使用getSelected时不能获得这里的item.ID，使用getChecked时不能弹出警告提示框

//            });

//            return "<a href='/xiangmuguanli/xm_word_out?id=" + ID_str + "'>导出execl</a>";
//        }
//    }
//};

    
//业务专员统计图表新增窗口
function tjtb_ywzy() {
    //alert("d");
    var url = "/xiangmuguanli/BasicBarindex?muluname="+dgtitle;

    var tabTitle = dgtitle + "部门申报项目图表";  //正确写法

    var icon = "icon-tubiao";
    window.parent.parent.addTab(tabTitle, url, icon);//使用新加的tab打开窗口
};

//业务专员之评委评审进度图表
function jdtb_ywzy() {
    //var pingweiname = row.UserName;
    $('#pw_jindu').dialog('open').dialog('setTitle', '评委评审进度');
    $('#tubiao').attr("src", "/xiangmuguanli/ywzy_pwjd_piechart?mulu="+dgtitle+"&fgldshenhefou=1");
};


//导出年度汇总表nianduhuizong
function nianduhuizong() {  
    window.location.href = "ywzy_exl_huizong?mulu="+dgtitle;
};

function tiqufujian() {
    //var pingweiname = row.UserName;
    $('#tiqufujian_dlg').dialog('open').dialog('setTitle', '提取附件');
    $('#ff').form('clear');
};

function tiqufujiansubmit() {
    var desdir = $('#tiqumulu').textbox('getValue');

    $.ajax({
        type: 'post',
        async: false,
        url: '/xiangmuguanli/tiqufujian',
        data: {
            "mulu": '@dgtitle',
            "desDir": desdir
        },
        success: function (data) {

            var data = JSON.parse(data);
            if (data.success) {
                $.messager.show({	// show error message
                    title: '成功提示',
                    msg: data.errorMsg
                })

            } else {
                $.messager.show({	// show error message
                    title: '错误提示',
                    msg: data.errorMsg
                })
            }
            $('#tiqufujian_dlg').dialog('close');
        }
    });
};

function tiqufujianclear() {

    $('#tiqumulu').textbox('clear');
    $('#tiqufujian_dlg').dialog('close');
};