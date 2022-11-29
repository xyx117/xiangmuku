$(function () {



    $("#csmx_dg").datagrid({
        singleSelect: true,
        async: false,
        collapsible: false,
        method: 'post',
        url: '/xiangmuguanli/getcsmx',
        toolbar: '#csmx_toolbar',
        rownumbers: true,
        autoRowheight: false,
        pagination: true,

        nowrap: false,
        queryParams: {
            xmglid: xmid
        },
        onLoadSuccess: function (data) {

            $.ajax({
                type: 'post',
                async: true,
                url: '/xiangmuguanli/getshenbaoshu_hj',
                data: {
                    "xmid": xmid
                },
                success: function (data) {
                    data = JSON.parse(data);
                    if (data.success) {
                        $('#shenbaoshu_hj').html("申报数合计： " + data.Msg + " (千元)");
                    }
                    else
                        $('#shenbaoshu_hj').html("申报数合计：出错！ ");
                }
            });

        }


    });

    var p = $('#csmx_dg').datagrid('getPager');

    $(p).pagination({

        pageSize: 10,//每页显示的记录条数，默认为10

        pageList: [5, 10, 15, 20, 25, 30, 35, 40, 45, 50, 55, 60, 65, 70, 75, 80, 85, 90, 95, 100],//可以设置每页记录条数的列表

        beforePageText: '第',//页数文本框前显示的汉字

        afterPageText: '页    共 {pages} 页',

        displayMsg: '当前显示 {from} - {to} 条记录   共 {total} 条记录',
    });


    //文本框有输入后，有删除图标出现
    $('#xmmingxi').textbox().textbox('addClearBtn', 'icon-clear');

    $('#zhichufenlei').textbox().textbox('addClearBtn', 'icon-clear');

    $('#shenbaoshu').textbox().textbox('addClearBtn', 'icon-clear');

    $('#chengbenbiaozhun').textbox().textbox('addClearBtn', 'icon-clear');

    $('#gongzuoliang').textbox().textbox('addClearBtn', 'icon-clear');

    $('#yijushuoming').textbox().textbox('addClearBtn', 'icon-clear');

    $('#gzl_danwei').textbox().textbox('addClearBtn', 'icon-clear');
    //电子表格批量导入
    $('#files').filebox({
        buttonText: ' 浏览 ',
        buttonAlign: 'right',
        prompt: '选择文件...'
    })

});

//对textbox进行验证
$.extend($.fn.textbox.defaults.rules, {


    positive_int: {
        validator: function (value, param) {
            if (value) {
                return /^[0-9]*[1-9][0-9]*$/.test(value);
            } else {
                return true;
            }
        },
        message: '只能输入正整数.'
    },
    numeric: {
        validator: function (value, param) {
            if (value) {
                return /^[0-9]*(\.[0-9]+)?$/.test(value);
            } else {
                return true;
            }
        },
        message: '只能输入数值.'
    }

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





//项目活动测算明细
function csmx_new() {
    $('#csmx_dlg').dialog('open').dialog('setTitle', '新增绩效指标');
    $('#csmx_fm').form('clear');
    $('#suoshuniandu').combobox({
        url: '/xiangmuguanli/wsxx_niandu_list',
        method: 'post',
        valueField: 'id',
        textField: 'it',
        editable: false,

    });

    url = '/xiangmuguanli/csmx_save?xmglID='+xmid;

};
//测算明细编辑
function csmx_edit() {
    var row = $('#csmx_dg').datagrid('getSelected');
    if (row) {
        $('#csmx_dlg').dialog('open').dialog('setTitle', '编辑项目');
        $('#csmx_fm').form('load', row);

        $('#suoshuniandu').combobox({
            url: '/xiangmuguanli/wsxx_niandu_list',
            method: 'post',
            valueField: 'id',
            textField: 'it',
            editable: false,
        });

        var csmxID = row.xmcsmxID;
        url = '/xiangmuguanli/csmx_edit?csmxID=' + csmxID;
    }
    else {
        $.messager.alert("错误提示", "请选择要编辑的行！", "warning");
    }
};

//测算明细删除
function csmx_destroy() {
    var row = $('#csmx_dg').datagrid('getSelected');
    if (row) {
        $.messager.confirm('提示', '您确定要删除这条记录吗？', function (r) {
            if (r) {
                $.post('/xiangmuguanli/csmx_del', { id: row.xmcsmxID, xmglID: xmid }, function (result) {
                    //这里把主键更改为项目名称后需要变动
                    //$.post('/xiangmuguanli/delXiangmu', { xmname: row.XmName }, function (result) { 

                    //result = JSON.parse(result);   //IE浏览器在后台操作完成后返回提示信息，转化为json字符串格式，在删除的方法中，加了这个后不能自动reload  dategrid

                    if (result.success) {
                        $('#csmx_dg').datagrid('reload');	// reload the user data

                        //var delid = "#" + row.ID;
                        //$(delid, window.parent.document).remove();

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

//电子表格导入
function csmx_exl_new() {
    $('#csmx_dlgexl').dialog('open').dialog('setTitle', '测算明细电子表格导入');


    $('#files').filebox('clear');

    url = '/xiangmuguanli/csmx_excelImport?xmglID='+xmid;
};


function csmx_saveexl() {
    $('#csmx_fmexl').form('submit', {
        url: url,
        onSubmit: function () {
            return $(this).form('validate');
        },
        success: function (result) {
            //var result = eval('(' + result + ')');
            result = JSON.parse(result);   //IE浏览器在后台操作完成后返回提示信息，转化为json字符串格式
            $('#csmx_dlgexl').dialog('close');		// close the dialog

            $('#csmx_dg').datagrid('reload');	// reload the user data

            if (result.success == false) {
                $.messager.show({
                    title: '错误提示',
                    msg: result.errorMsg
                });

            }

            if (result.success == true) {
                $.messager.show({	// show error message
                    title: '提示',
                    msg: result.errorMsg
                });
                $('#csmx_dlgexl').dialog('close');		// close the dialog

                $('#csmx_dg').datagrid('reload');	// reload the user data
            }
        }
    });

};

//测算明细保存
function csmx_save() {
    $('#csmx_fm').form('submit', {
        url: url,
        onSubmit: function () {
            return $(this).form('validate');
        },
        success: function (result) {
            //var result = eval('(' + result + ')');

            result = JSON.parse(result);   //IE浏览器在后台操作完成后返回提示信息，转化为json字符串格式
            $('#csmx_dlg').dialog('close');		// close the dialog

            $('#csmx_dg').datagrid('reload');	// reload the user data
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
