$(function () {

    $("#bumen_dg").datagrid({
        singleSelect: true,
        async: false,
        collapsible: true,
        method: 'post',
        url: '/bmshezhi/getBumen',
        toolbar: '#toolbar',
        rownumbers: true,
        pagination: true,
        fitcolumns: true,
        //nowrap: false,
        fit: true,
        autoRowheight: false,
        queryParams: {
            searchquery: ''
        },
    });

    //设置分页控件

    var p = $('#bumen_dg').datagrid('getPager');

    $(p).pagination({

        pageSize: 10,//每页显示的记录条数，默认为10

        pageList: [5, 10, 15, 20, 25, 30, 35, 40, 45, 50, 55, 60, 65, 70, 75, 80, 85, 90, 95, 100],//可以设置每页记录条数的列表

        beforePageText: '第',//页数文本框前显示的汉字

        afterPageText: '页    共 {pages} 页',

        displayMsg: '当前显示 {from} - {to} 条记录   共 {total} 条记录',
    });



    $('#BmName').textbox().textbox('addClearBtn', 'icon-clear');

});

//用来验证文本框中的内容是否已经存在（即重名）
$.extend($.fn.validatebox.defaults.rules, {
    checknameissame: {
        validator: function (value, param) {
            var name = value.trim();

            var result = "";
            $.ajax({
                type: 'post',
                async: false,
                url: '/bmshezhi/checkNameIsSame',
                data: {
                    "name": name
                },
                success: function (data) {
                    result = data;

                }
            });

            return result.indexOf("True") == 0;
        },
        message: '该名称已经被占用'
    }
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

function doSearch(value) {
    //alert('You input: ' + value);
    $('#bumen_dg').datagrid('load', { "searchquery": value });
};


function newBumen() {
    $('#dlg').dialog('open').dialog('setTitle', '新增部门');
    $('#BmName').textbox('setValue', '');
    $("#bmxingzhi").combobox({ editable: false });       //不可编辑，只能选择与editable="false"功能类似
    $('#BmName').textbox({ disabled: false });

    url = '/bmshezhi/saveBumen';
};

function editBumen() {
    var row = $('#bumen_dg').datagrid('getSelected');
    if (row) {
        var bmname = row.BmName;
        //需要对日期的格式进行一下转化
        //row.Kaishishijian = formatDatebox(row.Kaishishijian);
        //row.Jieshushijian = formatDatebox(row.Jieshushijian);
        $('#dlg').dialog('open').dialog('setTitle', '编辑部门');
        $('#BmName').textbox({
            value: row.BmName,
            disabled: true
        });
        $('#bmxingzhi').combobox('setValue', row.Bmxingzhi);
        url = '/bmshezhi/updateBumen?bmname=' + bmname + '';
    }
    else {
        $.messager.alert("错误提示", "请选择要编辑的行！", "warning");
    }
};


function saveBumen() {
    $('#fm').form('submit', {
        url: url,
        onSubmit: function () {
            return $(this).form('validate');
        },
        success: function (result) {


            result = JSON.parse(result);   //IE浏览器在后台操作完成后返回提示信息，转化为json字符串格式,在删除的方法中，加了这个后不能自动reload  dategrid


            $('#dlg').dialog('close');		// close the dialog
            $('#bumen_dg').datagrid('reload');	// reload the user data
            if (result.success == false) {
                $.messager.show({
                    title: '提示',
                    msg: result.errorMsg
                });
            }

            else {

                $.messager.show({
                    title: '提示',
                    msg: result.errorMsg
                })
            }
        },
    })

};




function destroyBumen() {
    var row = $('#bumen_dg').datagrid('getSelected');
    if (row) {
        $.messager.confirm('提示', '您确定要删除这条记录吗？', function (r) {
            if (r) {
                $.post('/bmshezhi/delBumen', { bmname: row.BmName }, function (result) {
                    if (result.success) {

                        $('#bumen_dg').datagrid('reload');	// reload the user data


                    } else {
                        result = JSON.parse(result);   //IE浏览器在后台操作完成后返回提示信息，转化为json字符串格式,在删除的方法中，加了这个后不能自动reload  dategrid

                        $.messager.show({	// show error message
                            title: 'Error',
                            msg: result.errorMsg
                        });
                    }
                }, 'json');
            };
        });
    }
    else {
        $.messager.alert("错误提示", "请选择要删除的行！", "warning");

    }
};