$(function () {


    $("#dg").datagrid({
        singleSelect: true,
        async: false,
        collapsible: true,
        method: 'post',
        url: '/xmmlshezhi/getMulu',
        toolbar: '#toolbar',
        rownumbers: true,
        pagination: true,
        fitcolumns: true,
        //nowrap: false,
        fit: true,
        autoRowheight: false,
    });


    //设置分页控件

    var p = $('#dg').datagrid('getPager');

    $(p).pagination({

        pageSize: 10,//每页显示的记录条数，默认为10

        pageList: [5, 10, 15, 20, 25, 30, 35, 40, 45, 50, 55, 60, 65, 70, 75, 80, 85, 90, 95, 100],//可以设置每页记录条数的列表

        beforePageText: '第',//页数文本框前显示的汉字

        afterPageText: '页    共 {pages} 页',

        displayMsg: '当前显示 {from} - {to} 条记录   共 {total} 条记录',

    });

    //给文本框的内容里增加删除即×按钮

    $('#xmName').textbox().textbox('addClearBtn', 'icon-clear');
    $('#Beizhu').textbox().textbox('addClearBtn', 'icon-clear');

});

//当字数太长，限定只显示前面一部分
function TitleFormatter(value, row, index) {
    var value1 = value;
    if (value1 == null) {
        var ss = '<a href="#" title="' + value + '" class="easyui-tooltip"></a>';
        return ss
    }
    else {
        if (value1.length > 24) {
            value1 = value1.substr(0, 24) + "...";
        }
        var ss = '<a href="#" title="' + value + '" class="easyui-tooltip">' + value1 + '</a>';
        return ss
    }
};

//用来验证文本框中的内容是否已经存在（即重名）
$.extend($.fn.validatebox.defaults.rules, {
    checknameissame: {
        validator: function (value, param) {
            var name = value.trim();

            var result = "";
            $.ajax({
                type: 'post',
                async: false,
                url: '/xmmlshezhi/checkNameIsSame',
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
    },
    stringCheckSub: {
        validator: function (value) {
            return /^[\w\u4E00-\u9FA5（）《》【】\-(){}\[\]]+$/.test(value);
        },
        message: "只能包括中文、英文字母、数字及（）《》[]【】(){}-等符号。"
    }
});

// 验证开始时间小于结束时间
$.extend($.fn.validatebox.defaults.rules, {
    md: {
        validator: function (value, param) {
            var temp = "#" + param[0];
            var sdate = $(temp).datebox('getValue');

            var edate = value;
            return sdate < edate;

        },
        message: '结束日期要大于开始日期！'
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


function newMulu() {
    $('#dlg').dialog('open').dialog('setTitle', '新增目录');
    $('#fm').form('clear');
    $("#fgxzshenhefou").switchbutton({ checked: true });
    $('#xmName').textbox({ disabled: false });
    url = '/xmmlshezhi/saveMulu';
};


function editMulu() {

    var row = $('#dg').datagrid('getSelected');
    if (row) {

        //需要对日期的格式进行一下转化
        row.Kaishishijian = formatDatebox(row.Kaishishijian);

        row.Jieshushijian = formatDatebox(row.Jieshushijian);

        row.chuangjian_time = formatDatebox(row.chuangjian_time);

        var name = row.Name;

        //var chuangjian_time = row.chuangjian_time;

        $('#dlg').dialog('open').dialog('setTitle', '编辑目录');
        $('#fm').form('load', row);
        if (row.fgxzshenhefou == "审核") {
            $("#fgxzshenhefou").switchbutton({ checked: true });
        }
        else {
            $("#fgxzshenhefou").switchbutton({ checked: false });
        }


        $('#xmName').textbox({ disabled: true });  //这样子，后台无法获取  xmName的值。  editable  disabled  

        url = '/xmmlshezhi/updateMulu?name=' + name;
    }
    else {

        $.messager.alert("错误提示", "请选择要编辑的行！", "warning");
    }
};
function saveMulu() {

    //对开始日期和结束日期进行判断
    var sdate = $('#startdate').datebox('getValue');

    var edate = $('#enddate').datebox('getValue');
    if (sdate > edate) {
        $.messager.alert("错误提示", "结束日期要大于开始日期！", "warning");
    }
    else {
        $('#fm').form('submit', {
            url: url,
            onSubmit: function () {
                return $(this).form('validate');
            },
            success: function (result) {
                //var result = eval('(' + result + ')');      //只保留这条语句，ie中就会出现中断，保险起见加上下面result语句
                result = JSON.parse(result);   //IE浏览器在后台操作完成后返回提示信息，转化为json字符串格式,在删除的方法中，加了这个后不能自动reload  dategrid

                $('#dlg').dialog('close');		// close the dialog
                $('#dg').datagrid('reload');	// reload the user data
                //if (result.errorMsg) {
                if (result.success) {

                    $.messager.show({
                        title: '提示',
                        msg: result.errorMsg
                    });
                } else {
                    $.messager.show({
                        title: '错误提示',
                        msg: result.errorMsg
                    });

                }
            }
        });
    };
};

function destroyMulu() {
    var row = $('#dg').datagrid('getSelected');
    if (row) {
        $.messager.confirm('提示', '您确定要删除这个项目目录吗？', function (r) {
            if (r) {
                $.post('/xmmlshezhi/delMulu', { muluname: row.Name }, function (result) {
                    if (result.success) {
                        $('#dg').datagrid('reload');	// reload the user data
                        $.messager.show({
                            title: '提示',
                            msg: result.errorMsg
                        });
                        var delid = "#" + row.ID;
                        $(delid, window.parent.document).remove();

                    } else {


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
