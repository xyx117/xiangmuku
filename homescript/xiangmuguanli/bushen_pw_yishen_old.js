
$(function () {
    var searchquery = "所有评审部门";

    $("#dg_shpw_wc").datagrid({
        singleSelect: false,
        selectOnCheck: true,
        checkOnSelect: true,
        async: false,
        //async: true,
        fit: true,
        collapsible: true,
        method: 'post',
        url: '/xiangmuguanli/pw_bushen_xm_wc',
        toolbar: '#pl_pwtj_toolbar',
        rownumbers: true,
        pagination: true,

        autoRowheight: false,
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

            //提交
            $('.tijiao').linkbutton({ text: '提交', plain: true });
            $('.tijiao').addClass("c6");
            $('.tijiao_jin').linkbutton({ text: '提交', disabled: true });

            //再审
            $('.zaishen').linkbutton({ text: '再审', plain: true });
            $('.zaishen').addClass("c6");
            $('.zaishen_jin').linkbutton({ text: '再审', disabled: true });


            //日志
            $('.rizhi').linkbutton({ text: '日志', plain: true });
            $('.rizhi').addClass("c6");

            //显示简介的详细内容
            $('.jianjie_all').tooltip({
                position: 'left',
                onShow: function () {
                    $(this).tooltip('tip').css({
                        width: '600px'
                    });
                }
            });


        }
    });
    var wc = $('#dg_shpw_wc').datagrid('getPager');

    $(wc).pagination({

        pageSize: 10,//每页显示的记录条数，默认为10

        pageList: [5, 10, 15, 20, 25, 30, 35, 40, 45, 50, 55, 60, 65, 70, 75, 80, 85, 90, 95, 100],//可以设置每页记录条数的列表

        beforePageText: '第',//页数文本框前显示的汉字

        afterPageText: '页    共 {pages} 页',

        displayMsg: '当前显示 {from} - {to} 条记录   共 {total} 条记录',
    });

    $('#suoshuxueyuan').combobox({
        select: '所有评审部门',
        //singleSelect: false,
        onChange: function (newValue, oldValue) {

            $('#dg_shpw_wc').datagrid('load', { "xiangmu": dgtitle, "loingid": loingid, "searchquery": newValue });
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
        var ss = '<a href="#" title="' + value + '" onclick="pwchakan_wc(' + index + ')" class="easyui-tooltip">' + value1 + '</a>';
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


//评委审核状态图标
function pwshenhebubiao(value, row, index) {
    var pingweishenhe = row.pingweishenhe;
    //var pingweishenhe = row.pingweishenhe;
    switch (pingweishenhe) {
        case "通过": return "<Image src='/Scripts/easyui/themes/icons/ok.png' Title='通过'/>";
            break;

        case "未通过": return "<Image src='/Scripts/easyui/themes/icons/no.png' Title='未通过'/>";
            break;
        case "撤回": return "<Image src='/Scripts/easyui/themes/icons/chehui.png' Title='撤回'/>";
            break;
        case "未审核": return "<Image src='/Scripts/easyui/themes/icons/question.png' Title='未审核'/>";
            break;

    }
};


//评委提交状态图标
function pwtjbubiao(value, row, index) {
    var pingweitijiao = row.pingweitijiao;
    //var pingweishenhe = row.pingweishenhe;
    switch (pingweitijiao) {
        case "未提交": return "<Image src='/Scripts/easyui/themes/icons/question.png' Title='未提交'/>";
            break;
        case "已提交": return "<Image src='/Scripts/easyui/themes/icons/tijiao.png' Title='已提交'/>";
            break;
    }
};


//业务专员 查看未通过  的脚本
//function pw_wc_chakan(value, row, index) {
//    return "<a href='javascript:void(0)' style='width:60%' class='chakan' onclick='pwchakan_wc(" + index + ")' target='mainFrame'>查看</a>";
//};

//业务专员查看
function pwchakan_wc(index) {
    $('#dg_shpw_wc').datagrid('selectRow', index);
    var row = $('#dg_shpw_wc').datagrid('getSelected');
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
function xmrizhi(value, row, index) {
    //alert("sdf")
    return "<a href='javascript:void(0)' style='width:95%' class='rizhi' onclick='rizhichakan(" + index + ")' target='mainFrame'>日志</a>";

};
//审核流程日志
function rizhichakan(index) {
    $('#dg_shpw_wc').datagrid('selectRow', index);
    var row = $('#dg_shpw_wc').datagrid('getSelected');
    if (row) {
        var xmname = row.XmName;
        var id = row.ID;
        var xmmulu = row.Xiangmumulu;
        //var tabTitle = name + "完善信息";
        var tabTitle = "流程：" + changdu(xmname);

        //var url = "/xiangmuguanli/liucheng?NoticeXmName=" + name + "&xmmulu=" + xmmulu + "&xmid=" + id;

        var url = "/xiangmuguanli/liucheng?xmname=" + xmname + "&loingid"+loingid+"&xmid=" + id;

        var icon = "icon-edit";
        window.parent.parent.addTab(tabTitle, url, icon);//使用新加的tab打开窗口
    }
};

var names;


//评委批量提交
function pl_tijiao() {

    var checkedItems = $('#dg_shpw_wc').datagrid('getChecked');
    //var checkedItems = $('#dg_shywzy_wsh').datagrid('getSelected');

    var names = [];    //取出每一个提交项的id并转为数组

    if (checkedItems.length == 0) {

        $.messager.alert("错误提示", "请选择要提交的项目！", "warning");
    } else {
        if (checkedItems) {
            $('#pl_tijiao_dlg').dialog('open').dialog('setTitle', '项目批量提交');
            //$('#tj_pw_fm').form('clear');
            $.each(checkedItems, function (index, item) {

                names.push(item.ID);         //使用getSelected时不能获得这里的item.ID，使用getChecked时不能弹出警告提示框

            });

            url = '/xiangmuguanli/pltijiao_pw?loingid='+loingid+'&username='+username+'&xmid='+names;
        }
    }

};


//批量评委提交保存
function pl_pwtj_save() {
    //var name = names;
    //alert(url);
    //var url = url;
    //url = '/xiangmuguanli/plshenhe_ywzy?names=' + names;
    $('#tj_pw_fm').form('submit', {

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
                $('#pl_tijiao_dlg').dialog('close');		// close the dialog

                $('#dg_bspw_wsh').datagrid('reload');	// reload the user data

                $('#dg_shpw_wc').datagrid('reload');	// reload the user data

                $.messager.show({
                    title: '提示',
                    msg: result.errorMsg
                });
            }
        }
    });
};



//评委再审
function zaishen(value, row, index) {

    var pwtijiao = row.pingweitijiao;
    if (pwtijiao == "未提交") {
        return "<a href='javascript:void(0)' style='width:95%' class='zaishen' onclick='pw_zaishen(" + index + ")' target='mainFrame'>再审</a>";
    } else {
        return "<a href='javascript:void(0)' style='width:95%' class='zaishen_jin'>再审</a>";
    }
};


//评委再审操作
function pw_zaishen(index) {
    $('#fm_shpw').form('clear');
    $('#dg_shpw_wc').datagrid('selectRow', index);
    var row = $('#dg_shpw_wc').datagrid('getSelected');
    //此处要参考一下编辑方法
    if (row) {
        $('#pwshenhe_dlg').dialog('open').dialog('setTitle', '评委再审');
        //alert(row);
        var name = row.XmName;
        var id = row.ID;
        var xmmulu = row.Xiangmumulu;

        url = '/xiangmuguanli/pw_shenhe?name=' + name + '&loingid=' + loingid + '&xmid=' + id;

    }
};


//审核操作弹出对话框后点击确认保存
function pwshenhe_save() {

    $('#fm_shpw').form('submit', {
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
                $('#pwshenhe_dlg').dialog('close');		// close the dialog

                //$('#dg_shpw_wsh').datagrid('reload');	// reload the user data

                $('#dg_shpw_wc').datagrid('reload');	// reload the user data

                $.messager.show({
                    title: '提示',
                    msg: result.errorMsg
                });
            }
        }
    });
};



//评委单个提交
function pw_tijiao(value, row, index) {

    var pwtijiao = row.pingweitijiao;
    if (pwtijiao == "未提交") {
        return "<a href='javascript:void(0)' style='width:95%' class='tijiao' onclick='pw_tj(" + index + ")' target='mainFrame'>提交</a>";
    } else {
        return "<a href='javascript:void(0)' style='width:95%' class='tijiao_jin'>提交</a>";
    }

};

//评委提交操作
function pw_tj(index) {

    //$('#tijiao_fm').form('clear');
    $('#dg_shpw_wc').datagrid('selectRow', index);
    var row = $('#dg_shpw_wc').datagrid('getSelected');
    if (row) {
        $('#pw_tijiao_dlg').dialog('open').dialog('setTitle', '提示');

        var id = row.ID;

        url = "/xiangmuguanli/pwtijiao?loingid="+loingid+"&username="+username+"&xmid=" + id;
    }
};
//提交保存
function tj_pw_save() {
    $('#tj_pw_fm_dt').form('submit', {
        url: url,
        onSubmit: function () {
            return $(this).form('validate');
        },
        success: function (result) {
            //var result = eval('(' + result + ')');

            result = JSON.parse(result);   //IE浏览器在后台操作完成后返回提示信息，转化为json字符串格式,在删除的方法中，加了这个后不能自动reload  dategrid

            if (result.Succeeded == 1) {
                $('#pw_tijiao_dlg').dialog('close');		// close the dialog
                $('#dg_shpw_wc').datagrid('reload');

                $.messager.show({
                    title: '提示',
                    msg: result.errorMsg
                });
            }
            else {
                $.messager.show({
                    title: '提示',
                    msg: result.errorMsg
                });
            }
        }
    });
};
