$(function () {

    var searchquery = "所有分管部门";

    //解决url路径传中文值乱码
    $("#dg_shfgld").datagrid({
        singleSelect: false,
        selectOnCheck: true,
        checkOnSelect: true,
        async: false,
        pagination: true,

        collapsible: true,
        url: '/xiangmuguanli/fgld_shenhe_xm',
        toolbar: '#pl_fgld_toolbar',
        method: 'post',
        autoRowheight: false,
        rownumbers: true,
        fit: true,
        view: groupview,
        groupField: 'suoshuxueyuan',
        groupFormatter: function (value, rows) { return value + ' - ' + rows.length + ' 项'; },
        queryParams: {
            xiangmu: dgtitle,
            loingid: loingid,
            searchquery: searchquery
        },

        //为datagrid中的操作链接样式 设置一个按钮图形
        onLoadSuccess: function (data) {
            
            //审定
            $('.shending').linkbutton({ text: '审定', plain: true });
            $('.shending').addClass("c6");
            $('.shending_jin').linkbutton({ text: '审定', disabled: true });


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

    });



    var p = $('#dg_shfgld').datagrid('getPager');

    $(p).pagination({

        pageSize: 10,//每页显示的记录条数，默认为10

        pageList: [5, 10, 15, 20, 25, 30, 35, 40, 45, 50, 55, 60, 65, 70, 75, 80, 85, 90, 95, 100],//可以设置每页记录条数的列表

        beforePageText: '第',//页数文本框前显示的汉字

        afterPageText: '页    共 {pages} 页',

        displayMsg: '当前显示 {from} - {to} 条记录   共 {total} 条记录',
    });


    $('#suoshuxueyuan').combobox({
        select: '所有分管部门',
        //singleSelect: false,
        onChange: function (newValue, oldValue) {

            $('#dg_shfgld').datagrid('load', { "xiangmu": dgtitle, "loingid": loingid, "searchquery": newValue });
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
    if (value1.length > 10) {
        value1 = value1.substr(0, 10) + "...";
        
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
        var ss = '<a href="#" title="' + value + '" onclick="fgldchakan(' + index + ')" class="easyui-tooltip">' + value1 + '</a>';
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


//业务专员查看
//function fgld_chakan(value, row, index) {
//    return "<a href='javascript:void(0)' style='width:60%' class='chakan' onclick='fgldchakan(" + index + ")' target='mainFrame'>查看</a>";
//};

function fgld_tubiao() {

    var tabTitle = dgtitle + "--分管部门申报项目图表";

    var url = "/xiangmuguanli/fgld_BasicBarindex?loingid="+loingid+"&muluname=" + dgtitle;

    var icon = "icon-tubiao";
    window.parent.addTab(tabTitle, url, icon);//使用新加的tab打开窗口
};




//业务专员查看
function fgldchakan(index) {
    $('#dg_shfgld').datagrid('selectRow', index);
    var row = $('#dg_shfgld').datagrid('getSelected');
    if (row) {
        var name = row.XmName;
        var id = row.ID;
        var xmmulu = row.Xiangmumulu;
        //var tabTitle = name + "查看";
        var tabTitle = "查看：" + changdu(name);
        var url = "/xiangmuguanli/chakan?NoticeXmName=" + name + "&xmmulu=" + xmmulu + "&xmid=" + id;
        var icon = "icon-shenhe";
        window.parent.addTab(tabTitle, url, icon);//使用新加的tab打开窗口
    }
};


//分管领导审核
function fgld_shenhe(value, row, index) {
    var fgldshenhe = row.fgldshenhe;
    if (fgldshenhe == "未审核") {
        return "<a href='javascript:void(0)' style='width:95%' class='shending' onclick='fgld_caozuo(" + index + ")' target='mainFrame'>审定</a>";
    }
    else {
        return "<a href='javascript:void(0)' style='width:95%' class='shending_jin'>审定</a>";
    }

};


//审核操作弹出对话框
function fgld_caozuo(index) {
    $('#fm_shbmfzr').form('clear');
    $('#dg_shfgld').datagrid('selectRow', index);
    var row = $('#dg_shfgld').datagrid('getSelected');
    //此处要参考一下编辑方法
    if (row) {
        $('#shenhe_dlg').dialog('open').dialog('setTitle', '分管领导审核');
        //alert(row);
        var name = row.XmName;
        var id = row.ID;
        var xmmulu = row.Xiangmumulu;

        url = '/xiangmuguanli/fgld_shenhe?name=' + name + '&loingid='+loingid+'&xmid='+id;

    }

};

//审核操作弹出对话框后点击确认保存
function fgldshenhe_save() {

    $('#fm_shbmfzr').form('submit', {
        url: url,
        onSubmit: function () {
            return $(this).form('validate');
        },
        success: function (result) {

            result = JSON.parse(result);   //IE浏览器在后台操作完成后返回提示信息，转化为json字符串格式,在删除的方法中，加了这个后不能自动reload  dategrid
            if (result.success == false) {

                $.messager.show({
                    title: '错误提示',
                    msg: result.errorMsg
                });
            }
            if (result.success == true) {
                $('#shenhe_dlg').dialog('close');		// close the dialog

                $('#dg_shfgld').datagrid('reload');	// reload the user data
                $.messager.show({
                    title: '提示',
                    msg: result.errorMsg
                });
            }
        }
    });
};

//审核流程日志
function ygxmrizhi(value, row, index) {
    //alert("sdf")
    return "<a href='javascript:void(0)' style='width:95%' class='rizhi' onclick='rizhichakan(" + index + ")' target='mainFrame'>日志</a>";
};

//审核流程日志
function rizhichakan(index) {
    $('#dg_shfgld').datagrid('selectRow', index);
    var row = $('#dg_shfgld').datagrid('getSelected');
    if (row) {
        var xmname = row.XmName;
        var id = row.ID;
        var xmmulu = row.Xiangmumulu;
        //var tabTitle = name + "完善信息";
        var tabTitle = "流程：" + changdu(xmname);

        //var url = "/xiangmuguanli/liucheng?NoticeXmName=" + name + "&xmmulu=" + xmmulu + "&xmid=" + id;

        var url = "/xiangmuguanli/liucheng?xmname=" + xmname + "&loingid="+loingid+"&xmid="+id;

        var icon = "icon-edit";
        window.parent.addTab(tabTitle, url, icon);//使用新加的tab打开窗口
    }
};

var names;

//批量审核
function pl_shenhe() {

    
    //$('#pl_shenhe_dlg').dialog('open').dialog('setTitle', '项目批量审核');
    var checkedItems = $('#dg_shfgld').datagrid('getChecked');
    //var checkedItems = $('#dg_shywzy_wsh').datagrid('getSelected');

    var names = [];
    //var names = {};

    if (checkedItems.length == 0) {

        $.messager.alert("错误提示", "请选择要审核的项目！", "warning");
    }
    else {
        if (checkedItems) {
            $('#pl_shenhe_dlg').dialog('open').dialog('setTitle', '项目批量审核');
            $('#shenhe_fm').form('clear');
            $.each(checkedItems, function (index, item) {

                names.push(item.ID);

            });

            url = '/xiangmuguanli/plshenhe_fgld?loingid='+loingid+'&username='+username+'&xmid='+names;
        }
    }
};


//批量审核保存
function pl_fgld_save() {

    $('#shenhe_fm').form('submit', {

        url: url,
        onSubmit: function () {
            return $(this).form('validate');
        },
        success: function (result) {
            result = JSON.parse(result);   //IE浏览器在后台操作完成后返回提示信息，转化为json字符串格式,在删除的方法中，加了这个后不能自动reload  dategrid

            if (result.success == "2") {

                $.messager.show({
                    title: '错误提示',
                    msg: result.errorMsg
                });
            }
            //else {
            if (result.success == "1") {
                $('#pl_shenhe_dlg').dialog('close');		// close the dialog

                $('#dg_shfgld').datagrid('reload');	// reload the user data

                $.messager.show({
                    title: '提示',
                    msg: result.errorMsg
                });
            }
        }
    });
};



function doSearch(value) {
    alert('You input: ' + value);
};
