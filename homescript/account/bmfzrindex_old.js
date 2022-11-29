
$(function () {

    $("#bmfzr_dg").datagrid({
        singleSelect: true,
        async: false,
        collapsible: true,
        method: 'post',
        url: '/Account/bmfergetuser',
        toolbar: '#bmfzr_toolbar',
        rownumbers: true,
        pagination: true,
        fitcolumns: true,
        nowrap: true,
        fit: true,
        autoRowHeight: false,
        queryParams: {
            userid: userid
        }
    });


    //设置分页控件

    var p = $('#bmfzr_dg').datagrid('getPager');

    $(p).pagination({

        pageSize: 10,//每页显示的记录条数，默认为10

        pageList: [5, 10, 15, 20, 25, 30, 35, 40, 45, 50, 55, 60, 65, 70, 75, 80, 85, 90, 95, 100],//可以设置每页记录条数的列表

        beforePageText: '第',//页数文本框前显示的汉字

        afterPageText: '页    共 {pages} 页',

        displayMsg: '当前显示 {from} - {to} 条记录   共 {total} 条记录',
    });


    $('#name').textbox().textbox('addClearBtn', 'icon-clear');

    $('#zhenshiname').textbox().textbox('addClearBtn', 'icon-clear');

    $('#userpassword').textbox().textbox('addClearBtn', 'icon-clear');

    $('#confirmPassword').textbox().textbox('addClearBtn', 'icon-clear');

    $('#phonenumber').textbox().textbox('addClearBtn', 'icon-clear');



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

$.extend($.fn.validatebox.defaults.rules, {
    //检查名字是否雷同
    checknameissame: {
        validator: function (value, param) {
            var name = value.trim();

            var result = "";
            $.ajax({
                type: 'post',
                async: false,
                url: '/Account/checkNameIsSame',
                data: {
                    "name": name
                },
                success: function (data) {
                    result = data;

                }
            });

            return result.indexOf("True") == 0;
        },
        message: '该名字已经被占用'
    },



    /*必须和某个字段相等*/
    equalTo: {

        validator: function (value, param) {
            return $(param[0]).val() == value;
        },
        message: '两次输入的密码不一致喔！'
    },

    //验证密码输入至少为6位字符
    minLength: {
        validator: function (value, param) {
            return value.length >= param[0];
        },
        message: '密码至少为6位字符或数字的组合！'
    },

    //验证手机或电话号码
    phoneRex: {
        validator: function (value) {
            var rex = /^1[3-8]+\d{9}$/;
            //var rex=/^(([0\+]\d{2,3}-)?(0\d{2,3})-)(\d{7,8})(-(\d{3,}))?$/;
            //区号：前面一个0，后面跟2-3位数字 ： 0\d{2,3}
            //电话号码：7-8位数字： \d{7,8
            //分机号：一般都是3位数字： \d{3,}
            //这样连接起来就是验证电话的正则表达式了：/^((0\d{2,3})-)(\d{7,8})(-(\d{3,}))?$/		 
            var rex2 = /^((0\d{2,3})-)(\d{7,8})(-(\d{3,}))?$/;
            if (rex.test(value) || rex2.test(value)) {
                // alert('t'+value);
                return true;
            } else {
                //alert('false '+value);
                return false;
            }

        },
        message: '请输入正确电话或手机格式'
    }




});


function newUser() {
    $('#bmfzr_dlg').dialog('open').dialog('setTitle', '新增项目');
    $('#name').textbox('enable', 'true');
    $('#pw').show();
    $('#pw_cf').show();
    $('#imf').show();
    $('#bmfzr_fm').form('clear');

    url = '/Account/bmfzrsaveuser?userId='+userid;
};

//用户编辑
function editUser() {

    var row = $('#bmfzr_dg').datagrid('getSelected');

    if (row) {

        var id = row.Id;

        $('#bmfzr_dlg').dialog('open').dialog('setTitle', '编辑用户');

        $('#bmfzr_fm').form('load', row);
        $('#name').textbox('disable', 'true');
        $('#pw').hide();
        $('#pw_cf').hide();
        $('#imf').hide();

        url = '/Account/edit_renyuan?id=' + id + '';
    }
    else {

        $.messager.alert("错误提示", "请选择要编辑的行！", "warning");
    }
};

function editpassword() {

    var row = $('#bmfzr_dg').datagrid('getSelected');

    if (row) {
        var name = row.UserName;

        $('#bmrestpaw_dlg').dialog('open').dialog('setTitle', '重置密码');

        url = '/Account/resetpassword_csh?name=' + name;
    }
    else {
        // alert("b");
        $.messager.alert("错误提示", "请选择要重置密码的用户！", "warning");
    }
};
function restpwd() {
    $('#bmrestpaw_fm').form('submit', {
        url: url,
        onSubmit: function () {
            return $(this).form('validate');
        },
        success: function (result) {

            $('#bmrestpaw_dlg').dialog('close');		// close the dialog
            result = JSON.parse(result);
            if (result.success == true) {
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

function saveUser() {

    $('#bmfzr_fm').form('submit', {
        url: url,
        onSubmit: function () {
            return $(this).form('validate');
        },
        success: function (result) {
            //var result = eval('(' + result + ')');
            result = JSON.parse(result);

            $('#bmfzr_dlg').dialog('close');		// close the dialog
            $('#bmfzr_dg').datagrid('reload');	// reload the user data
            if (result.success == true) {
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


function destroyUser() {
    var row = $('#bmfzr_dg').datagrid('getSelected');
    if (row) {
        $.messager.confirm('提示', '您确定要删除该员工吗？', function (r) {
            if (r) {
                $.post('/Account/bmfzrdeluser', { yuangongid: row.Id, bmferid: userid }, function (result) {

                    if (result.Succeeded) {
                        $('#bmfzr_dg').datagrid('reload');	// reload the user data
                        $.messager.show({
                            title: '提示',
                            msg: result.Msg
                        });
                    } else {

                        $.messager.show({
                            title: '错误提示',
                            msg: result.Msg
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

//当字数太长，限定只显示前面一部分
function TitleFormatter(value, row, index) {
    var value1 = value;
    if (value1 == null) {
        var ss = '<a href="#" title="' + value + '" class="easyui-tooltip"></a>';
        return ss
    }
    else {
        if (value1.length > 10) {
            value1 = value1.substr(0, 10) + "...";
        }
        var ss = '<a href="#" title="' + value + '" class="easyui-tooltip">' + value1 + '</a>';
        return ss
    }
};