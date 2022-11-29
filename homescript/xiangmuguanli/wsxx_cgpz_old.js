$(function () {
    $("#cgpz_dg").datagrid({
        singleSelect: true,
        async: false,
        collapsible: false,
        method: 'post',
        url: '/xiangmuguanli/getcgpz',
        toolbar: '#cgpz_toolbar',
        rownumbers: true,
        pagination: true,
        nowrap: false,
        //fit: true,
        autoRowheight: false,
        queryParams: {
            xmglid: xmid
        },
        onLoadSuccess: function (data) {

            $.ajax({
                type: 'post',
                async: true,
                url: '/xiangmuguanli/getcgpzjine_hj',
                data: {
                    "xmid": xmid
                },
                success: function (data) {
                    data = JSON.parse(data);
                    if (data.success) {
                        $('#cgpzjine_hj').html("采购金额合计： " + data.Msg + " (元)");
                    }
                    else
                        $('#cgpzjine_hj').html("采购金额合计：出错！ ");
                }
            });

        }
    });




    //文本框有输入后，有删除图标出现
    $('#zichanfenlei').textbox().textbox('addClearBtn', 'icon-clear');

    $('#zichanmingcheng').textbox().textbox('addClearBtn', 'icon-clear');

    $('#guigexinghao').textbox().textbox('addClearBtn', 'icon-clear');

    $('#peizhifagnshi').textbox().textbox('addClearBtn', 'icon-clear');

    $('#peizhishuliang').textbox().textbox('addClearBtn', 'icon-clear');

    $('#danjia').textbox().textbox('addClearBtn', 'icon-clear');

    $('#caigoujine').textbox().textbox('addClearBtn', 'icon-clear');

    $('#zijinxingzhi').textbox().textbox('addClearBtn', 'icon-clear');

    $('#zichancunliang').textbox().textbox('addClearBtn', 'icon-clear');

    $('#caigoushuoming').textbox().textbox('addClearBtn', 'icon-clear');



    var p = $('#cgpz_dg').datagrid('getPager');

    $(p).pagination({

        pageSize: 10,//每页显示的记录条数，默认为10

        pageList: [5, 10, 15, 20, 25, 30, 35, 40, 45, 50, 55, 60, 65, 70, 75, 80, 85, 90, 95, 100],//可以设置每页记录条数的列表

        beforePageText: '第',//页数文本框前显示的汉字

        afterPageText: '页    共 {pages} 页',

        displayMsg: '当前显示 {from} - {to} 条记录   共 {total} 条记录',
    });




    $('#caigoujihua').switchbutton({


        onText: '空表申报',
        offText: '非空表申报',


        onChange: function (checked) {

            if (checked) {   //空表申报

                $.messager.confirm('提示', '您确定要空表申报吗？若您空表申报，配置清单中的记录将被清空。', function (r) {
                    if (r) {
                        $.post('/xiangmuguanli/cgpz_kong_tb', { xmglID: xmid }, function (result) {

                            if (result.success) {
                                $('#cgpz_dg').datagrid('reload');	// reload the user data

                                $('#cgpz_add').linkbutton('disable');

                                $('#cgpz_edit').linkbutton('disable');

                                $('#cgpz_del').linkbutton('disable');

                                $('#cgpz_exl').linkbutton('disable');

                            } else {
                                result = JSON.parse(result);
                                $.messager.show({	// show error message
                                    title: '错误提示',
                                    msg: result.errorMsg
                                });
                            }
                        }, 'json');
                    }
                    else {
                        $('#caigoujihua').switchbutton('uncheck');
                    }
                });

                return;
            } else {

                $.post('/xiangmuguanli/cgpz_feikong_tb', { xmglID: xmid }, function (result) {

                    if (result.success) {
                        $('#cgpz_dg').datagrid('reload');	// reload the user data

                        $('#cgpz_add').linkbutton('enable');

                        $('#cgpz_edit').linkbutton('enable');

                        $('#cgpz_del').linkbutton('enable');

                        $('#cgpz_exl').linkbutton('enable');

                    } else {
                        result = JSON.parse(result);
                        $.messager.show({	// show error message
                            title: '错误提示',
                            msg: result.errorMsg
                        });
                    }
                }, 'json');

                return;
            }
        }

    });


    //电子表格批量导入
    $('#files').filebox({
        buttonText: ' 浏览 ',
        buttonAlign: 'right',
        prompt: '选择文件...'
    })
});



//当项目名称字数太长，限定只显示前面一部分
function jine(value, row, index) {

    var peizhishuliang = row.peizhishuliang;

    var danjia = row.danjia;

    return danjia * peizhishuliang;

};


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



//采购配置
function cgpz_new() {
    $('#cgpz_dlg').dialog('open').dialog('setTitle', '项目采购和配置计划表设置');
    $('#cgpz_fm').form('clear');

    $('#suoshuniandu').combobox({
        url: '/xiangmuguanli/wsxx_niandu_list',
        method: 'post',
        valueField: 'id',
        textField: 'it',
        editable: false,

    });

    url = '/xiangmuguanli/cgpz_save?xmglID='+xmid;
};

//采购配置表编辑
function cgpz_edit() {
    var row = $('#cgpz_dg').datagrid('getSelected');
    if (row) {
        $('#cgpz_dlg').dialog('open').dialog('setTitle', '编辑项目');
        $('#cgpz_fm').form('load', row);

        $('#suoshuniandu').combobox({
            url: '/xiangmuguanli/wsxx_niandu_list',
            method: 'post',
            valueField: 'id',
            textField: 'it',
            editable: false,

        });

        var xmcgpzID = row.xmcgpzID;
        url = '/xiangmuguanli/cgpz_edit?xmcgpzID=' + xmcgpzID;
    }
    else {
        $.messager.alert("错误提示", "请选择要编辑的行！", "warning");
    }
};

function cgpz_destroy() {
    var row = $('#cgpz_dg').datagrid('getSelected');
    if (row) {
        $.messager.confirm('提示', '您确定要删除这条记录吗？', function (r) {
            if (r) {
                $.post('/xiangmuguanli/cgpz_del', { id: row.xmcgpzID, xmglID: xmid }, function (result) {


                    if (result.success) {
                        $('#cgpz_dg').datagrid('reload');	// reload the user data



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


function cgpz_save() {
    $('#cgpz_fm').form('submit', {
        url: url,
        onSubmit: function () {
            return $(this).form('validate');
        },
        success: function (result) {
            //var result = eval('(' + result + ')');

            result = JSON.parse(result);   //IE浏览器在后台操作完成后返回提示信息，转化为json字符串格式
            $('#cgpz_dlg').dialog('close');		// close the dialog

            $('#cgpz_dg').datagrid('reload');	// reload the user data
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
            } else {

                $('#cgpz_dlg').dialog('close');		// close the dialog

                $('#cgpz_dg').datagrid('reload');	// reload the user data
            }
        }
    });
};

function cgpz_exl_new() {
    $('#cgpz_dlgexl').dialog('open').dialog('setTitle', '采购配置电子表格导入');
    $('#files').filebox('clear');
    url = '/xiangmuguanli/excelImport?xmglID='+xmid;
};

function cgpz_saveexl() {
    $('#cgpz_fmexl').form('submit', {
        url: url,
        onSubmit: function () {
            return $(this).form('validate');
        },
        success: function (result) {
            //var result = eval('(' + result + ')');
            result = JSON.parse(result);   //IE浏览器在后台操作完成后返回提示信息，转化为json字符串格式
            $('#cgpz_dlgexl').dialog('close');		// close the dialog

            $('#cgpz_dg').datagrid('reload');	// reload the user data

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
                $('#cgpz_dlgexl').dialog('close');		// close the dialog

                $('#cgpz_dg').datagrid('reload');	// reload the user data
            }
        }
    });

};