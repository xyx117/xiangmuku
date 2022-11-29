$(function () {
    var searchquery = "所有部门";
    $("#dg_bscwzg_wsh").datagrid({
        singleSelect: false,
        selectOnCheck: true,
        checkOnSelect: true,
        async: false,
        //async: true,
        collapsible: true,
        method: 'post',
        url: '/xiangmuguanli/ywzy_bushen_xm_wsh',
        toolbar: '#cwzg_toolbar',
        rownumbers: true,
        pagination: true,
        //fitcolumns: true,
        autoRowheight: false,
        //nowrap: false,
        view: groupview,
        fit: true,
        groupField: 'suoshuxueyuan',
        groupFormatter: function (value, rows) { return value + ' - ' + rows.length + ' 项'; },
        queryParams: {
            xiangmu: dgtitle,
            searchquery: searchquery
        },
        //为datagrid中的操作链接样式 设置一个按钮图形
        onLoadSuccess: function (data) {
            //查看
            $('.chakan').linkbutton({ text: '查看', plain: true });
            $('.chakan').addClass("c6");

            //日志
            $('.rizhi').linkbutton({ text: '日志', plain: true });
            $('.rizhi').addClass("c6");


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
        //data-options=" "
    });
    var p_wsh = $('#dg_bscwzg_wsh').datagrid('getPager');

    $(p_wsh).pagination({

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

            $('#dg_bscwzg_wsh').datagrid('load', { "xiangmu": dgtitle, "searchquery": newValue });
        }
    });


});


$.extend($.fn.combobox.methods, {
    selectedIndex: function (jq, index) {
        if (!index)
            index = 0;
        var data = $(jq).combobox('options').data;
        var vf = $(jq).combobox('options').valueField;
        $(jq).combobox('setValue', eval('data[index].' + vf));
    }
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
        var ss = '<a href="#" title="' + value + '" onclick="cwzgchakan_wsh(' + index + ')" class="easyui-tooltip">' + value1 + '</a>';
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
        if (value.length > 15) {
            value1 = value.substr(0, 15) + "...";
        }

        var ss = '<a href="javascript:;" title="' + value + '" class="jianjie_all">' + value1 + '</a>';
        return ss
    }
};


//部门负责人审核图标
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

//分管领导人审核图标
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


//业务专员审核图标
function ywzybubiao(value, row, index) {
    var ywzyshenhe = row.ywzyshenhe;
    switch (ywzyshenhe) {
        case "通过": return "<Image src='/Scripts/easyui/themes/icons/ok.png' Title='通过'/>";
            break;
        case "未审核": return "<Image src='/Scripts/easyui/themes/icons/question.png' Title='未审核'/>";
            break;
        case "未通过": return "<Image src='/Scripts/easyui/themes/icons/no.png' Title='未通过'/>";
            break;
    }
};


//财务主管查看未审核项目
//function cwzg_chakan_wsh(value, row, index) {
//    return "<a href='javascript:void(0)' style='width:60%' class='chakan' onclick='cwzgchakan_wsh(" + index + ")' target='mainFrame'>查看</a>";
//};
//财务主管查看财务主管查看未审核项目
function cwzgchakan_wsh(index) {
    $('#dg_bscwzg_wsh').datagrid('selectRow', index);
    var row = $('#dg_bscwzg_wsh').datagrid('getSelected');
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

//审核流程日志，财务主管查看未审核项目
function cwzg_xmrizhi_wsh(value, row, index) {
    //alert("sdf")
    return "<a href='javascript:void(0)' style='width:60%' class='rizhi' onclick='rizhichakan_wsh(" + index + ")' target='mainFrame'>日志</a>";

};
//审核流程日志，财务主管查看未审核项目
function rizhichakan_wsh(index) {
    $('#dg_bscwzg_wsh').datagrid('selectRow', index);
    var row = $('#dg_bscwzg_wsh').datagrid('getSelected');
    if (row) {
        var xmname = row.XmName;
        var id = row.ID;
        var xmmulu = row.Xiangmumulu;
        //var tabTitle = name + "完善信息";
        var tabTitle = "流程：" + changdu(xmname);

        //var url = "/xiangmuguanli/liucheng?NoticeXmName=" + name + "&xmmulu=" + xmmulu + "&xmid=" + id;

        var url = "/xiangmuguanli/liucheng?xmname=" + xmname + "&loingid="+loingid+"&xmid="+id;

        var icon = "icon-edit";
        window.parent.parent.addTab(tabTitle, url, icon);//使用新加的tab打开窗口
    }
};