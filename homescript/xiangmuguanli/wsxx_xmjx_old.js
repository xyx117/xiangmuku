

$(function () {

    if (cccx_yt == "1") {
        $('#xmchengxiao_fm').form('load', '/xiangmuguanli/load_cccx?xmid='+xmid+'&shijian='+sf);
    };
    //解决url路径传中文值乱码
    $("#jxcc_dg").datagrid({
        singleSelect: true,
        async: false,
        collapsible: true,
        method: 'post',
        url: '/xiangmuguanli/getxmjx_cc',
        toolbar: '#jxcc_toolbar',
        rownumbers: true,
        pagination: true,
        //fitcolumns: true,
        nowrap: false,
        autoRowheight: false,
        queryParams: {
            xmglid: xmid
        }

    });
    //jxxl_dg
    $("#jxxl_dg").datagrid({
        singleSelect: true,
        async: false,
        collapsible: true,
        method: 'post',
        url: '/xiangmuguanli/getxmjx_xl',
        toolbar: '#jxxl_toolbar',
        rownumbers: true,
        pagination: true,
        //fitcolumns: true,
        autoRowheight: false,
        nowrap: false,
        queryParams: {
            xmglid: xmid
        }
    });


    var p = $('#jxxl_dg').datagrid('getPager');

    $(p).pagination({

        pageSize: 10,//每页显示的记录条数，默认为10

        pageList: [5, 10, 15, 20, 25, 30, 35, 40, 45, 50, 55, 60, 65, 70, 75, 80, 85, 90, 95, 100],//可以设置每页记录条数的列表

        beforePageText: '第',//页数文本框前显示的汉字

        afterPageText: '页    共 {pages} 页',

        displayMsg: '当前显示 {from} - {to} 条记录   共 {total} 条记录',
    });


    var p1 = $('#jxcc_dg').datagrid('getPager');

    $(p1).pagination({

        pageSize: 10,//每页显示的记录条数，默认为10

        pageList: [5, 10, 15, 20, 25, 30, 35, 40, 45, 50, 55, 60, 65, 70, 75, 80, 85, 90, 95, 100],//可以设置每页记录条数的列表

        beforePageText: '第',//页数文本框前显示的汉字

        afterPageText: '页    共 {pages} 页',

        displayMsg: '当前显示 {from} - {to} 条记录   共 {total} 条记录',
    });

    //文本框有输入后，有删除图标出现
    $('#xmchanchu').textbox().textbox('addClearBtn', 'icon-clear');

    $('#xmchengxiao').textbox().textbox('addClearBtn', 'icon-clear');

    //绩效产出指标，文本框输入后，有删除图标按钮出现
    $('#cc_zhibiao').textbox().textbox('addClearBtn', 'icon-clear');
    $('#cc_mubiao').textbox().textbox('addClearBtn', 'icon-clear');
    $('#cc_you').textbox().textbox('addClearBtn', 'icon-clear');
    $('#cc_liang').textbox().textbox('addClearBtn', 'icon-clear');
    $('#cc_zhong').textbox().textbox('addClearBtn', 'icon-clear');
    $('#cc_cha').textbox().textbox('addClearBtn', 'icon-clear');

    //绩效效率指标，文本框输入后，有删除图标按钮出现
    $('#xl_zhibiao').textbox().textbox('addClearBtn', 'icon-clear');
    $('#xl_mubiao').textbox().textbox('addClearBtn', 'icon-clear');
    $('#xl_you').textbox().textbox('addClearBtn', 'icon-clear');
    $('#xl_liang').textbox().textbox('addClearBtn', 'icon-clear');
    $('#xl_zhong').textbox().textbox('addClearBtn', 'icon-clear');
    $('#xl_cha').textbox().textbox('addClearBtn', 'icon-clear');
});



//文本框有输入之后文本框末尾有删除符号出现
$.extend($.fn.textbox.methods, {
    addClearBtn: function (jq, iconCls) {
        return jq.each(function () {
            var t = $(this);
            var opts = t.textbox('options');
            opts.icons = opts.icons || [];
            opts.icons.unshift({
                iconCls: iconCls,
                handler: function (e) {
                    $(e.data.target).textbox('clear').textbox('textbox').focus();
                    $(this).css('visibility', 'hidden');
                }
            });
            t.textbox();
            if (!t.textbox('getText')) {
                t.textbox('getIcon', 0).css('visibility', 'hidden');
            }
            t.textbox('textbox').bind('keyup', function () {
                var icon = t.textbox('getIcon', 0);
                if ($(this).val()) {
                    icon.css('visibility', 'visible');
                } else {
                    icon.css('visibility', 'hidden');
                }
            });
        });
    }
});

var url;                         //此处的URL具体什么作用不是很清楚


//产出成效保存
function add_cccx() {

    if (cccx_yt == "1") {
        url = '/xiangmuguanli/edit_cccx?name='+xmname+'&xmid='+xmid;
    }
    else {
        url = '/xiangmuguanli/add_cccx?name='+xmname+'&xmid='+xmid;
    }

    //$.messager.progress();	// 提示进度条
    $('#xmchengxiao_fm').form('submit', {
        url: url,
        success: function (result) {

            result = JSON.parse(result);   //IE浏览器在后台操作完成后返回提示信息，转化为json字符串格式
            if (result.success) {
                //$.messager.progress('close');	// hide progress bar while submit successfully
                $.messager.show({
                    title: '提示',
                    msg: result.message
                });
                //保存成功后按钮禁用
                $('#addcccx').linkbutton({ text: '更新' });
                cccx_yt = "1";
            }
            else {
                $.messager.show({
                    title: '错误提示',
                    msg: result.errorMsg
                });
            }
        }
    })
};

//项目绩效目标，产出指标
function jxcc_new() {
    $('#jxcc_dlg').dialog('open').dialog('setTitle', '新增绩效产出指标');
    $('#jxcc_fm').form('clear');

    url = '/xiangmuguanli/jxcc_save?xmglID='+xmid;
};

//项目绩效目标，产出指标,编辑
function jxcc_edit() {
    var row = $('#jxcc_dg').datagrid('getSelected');
    if (row) {
        $('#jxcc_dlg').dialog('open').dialog('setTitle', '编辑项目');
        $('#jxcc_fm').form('load', row);
        var xmjxccID = row.xmjixiao_ccID;
        url = '/xiangmuguanli/jxcc_edit?xmjxccID=' + xmjxccID;
    }
    else {
        $.messager.alert("错误提示", "请选择要编辑的行！", "warning");
    }
};

//绩效产出指标  删除按钮
function jxcc_destroy() {
    var row = $('#jxcc_dg').datagrid('getSelected');
    if (row) {
        $.messager.confirm('提示', '您确定要删除这条记录吗？', function (r) {
            if (r) {
                $.post('/xiangmuguanli/jxcc_del', { id: row.xmjixiao_ccID, xmglID: xmid }, function (result) {

                    //result = JSON.parse(result);   //IE浏览器在后台操作完成后返回提示信息，转化为json字符串格式，在删除的方法中，加了这个后不能自动reload  dategrid
                    if (result.success) {
                        $('#jxcc_dg').datagrid('reload');	// reload the user data

                    } else {
                        result = JSON.parse(result);
                        $.messager.show({	// show error message
                            title: '错误提示',
                            msg: result.errorMsg
                        });
                    }
                }, 'json');
            }
        });
    }
    else {
        $.messager.alert("错误提示", "请选择要删除的行！", "warning");
    }
};


//这里或许可以重构，就是，绩效产出指标和绩效效率指标共同用这一个保存方法
function jxcc_save() {
    $('#jxcc_fm').form('submit', {
        url: url,
        onSubmit: function () {
            return $(this).form('validate');
        },
        success: function (result) {

            //contentType: "application/json; charset=utf-8";
            result = JSON.parse(result);   //IE浏览器在后台操作完成后返回提示信息，转化为json字符串格式
            $('#jxcc_dlg').dialog('close');		// close the dialog

            $('#jxcc_dg').datagrid('reload');	// reload the user data

            if (result.success == true) {
                // $.messager.progress('close');	// 关闭进度条
                $.messager.show({
                    title: '提示',
                    msg: result.message
                });

            }
            if (result.success == false) {
                //alter("dfdf");
                $.messager.show({
                    title: '错误提示',
                    msg: result.errorMsg
                });
            }
        }
    });
};


//绩效效率表新增
function jxxl_new() {
    $('#jxxl_dlg').dialog('open').dialog('setTitle', '新增绩效效率指标');
    $('#jxxl_fm').form('clear');

    url = '/xiangmuguanli/jxxl_save?xmglID='+xmid;

};
//绩效效率表编辑
function jxxl_edit() {
    var row = $('#jxxl_dg').datagrid('getSelected');
    if (row) {
        $('#jxxl_dlg').dialog('open').dialog('setTitle', '编辑项目');
        $('#jxxl_fm').form('load', row);
        var xmjixiao_xlID = row.xmjixiao_xlID;
        url = '/xiangmuguanli/jxxl_edit?xmjixiao_xlID=' + xmjixiao_xlID;
    }
    else {
        $.messager.alert("错误提示", "请选择要编辑的行！", "warning");
    }
};

//绩效效率表保存
function jxxl_save() {
    $('#jxxl_fm').form('submit', {
        url: url,
        onSubmit: function () {
            return $(this).form('validate');
        },
        success: function (result) {
            //var result = eval('(' + result + ')');
            result = JSON.parse(result);   //IE浏览器在后台操作完成后返回提示信息，转化为json字符串格式

            $('#jxxl_dlg').dialog('close');		// close the dialog

            $('#jxxl_dg').datagrid('reload');	// reload the user data

            if (result.success == true) {
                // $.messager.progress('close');	// 关闭进度条
                $.messager.show({
                    title: '提示',
                    msg: result.message
                });

            }
            if (result.success == false) {

                $.messager.show({
                    title: '错误提示',
                    msg: result.errorMsg
                });
            }
        }
    });
};

//绩效效率删除按钮
function jxxl_destroy() {
    var row = $('#jxxl_dg').datagrid('getSelected');
    if (row) {
        $.messager.confirm('提示', '您确定要删除这条记录吗？', function (r) {
            if (r) {
                $.post('/xiangmuguanli/jxxl_del', { id: row.xmjixiao_xlID, xmglID: xmid }, function (result) {

                    //result = JSON.parse(result);   //IE浏览器在后台操作完成后返回提示信息，转化为json字符串格式,在删除的方法中，加了这个后不能自动reload  dategrid

                    if (result.success) {
                        $('#jxxl_dg').datagrid('reload');	// reload the user data

                    } else {
                        result = JSON.parse(result);
                        $.messager.show({	// show error message
                            title: '错误提示',
                            msg: result.errorMsg
                        });
                    }
                }, 'json');
            }
        });
    }
    else {
        $.messager.alert("错误提示", "请选择要删除的行！", "warning");
    }
};