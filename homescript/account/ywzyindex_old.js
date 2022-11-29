$(function () {

    $("#cwkz_dg").datagrid({
        singleSelect: true,
        async: false,
        collapsible: true,
        method: 'post',
        url: '/Account/ywzygetuser',
        toolbar: '#cwkz_toolbar',
        rownumbers: true,
        pagination: true,
        fitcolumns: true,
        nowrap: true,
        fit: true,
        autoRowHeight: false,
        queryParams: {
            searchquery: ''
        },
    });

    //设置分页控件
    var p = $('#cwkz_dg').datagrid('getPager');

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


    //控制当角色变化时，下拉菜单是多选还是单选
    $('#role').combobox({
        onSelect: function () {
            var juese = $('#role').combobox('getValue');

            switch (juese) {
                case "分管领导":
                    {
                        $('#suoshuxueyuan').combobox({ readonly: false });
                        $('#suoshuxueyuan').combobox({ multiple: true });
                        $('#suoshuxueyuan').combobox("clear");
                        //alert("fgld clear");
                        break;
                    };

                case "部门负责人":
                    {
                        $('#suoshuxueyuan').combobox({ readonly: false });
                        $('#suoshuxueyuan').combobox({ multiple: false });
                        $('#suoshuxueyuan').combobox("clear");
                        //alert("bmfzr clear");
                        break;
                    };

                    //这里新增领导分管的所属学院
                case "领导":
                    {
                        $('#suoshuxueyuan').combobox({ readonly: false });
                        $('#suoshuxueyuan').combobox({ multiple: true });
                        $('#suoshuxueyuan').combobox("clear");
                        //alert("ld clear");
                        break;
                    };

                default:
                    {
                        $('#suoshuxueyuan').combobox({ multiple: false });
                        $('#suoshuxueyuan').combobox("clear");
                        $('#suoshuxueyuan').combobox({ readonly: true });
                        $('#suoshuxueyuan').combobox('select', 'all');

                    };
            }
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

//验证两次密码输入是否相同
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
        message: '确认密码与密码不相同'
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

function doSearch(value) {
    //alert('You input: ' + value);
    $('#cwkz_dg').datagrid('load', { "searchquery": value });
};

function newUser() {
    $('#cwkz_dlg').dialog('open').dialog('setTitle', '新增用户');
    $('#name').textbox('enable', 'true');
    $('#juese').show();
    $('#bumen').show();
    $('#pw').show();
    $('#pw_cf').show();
    $('#imf').show();
    $('#cwkz_fm').form('clear');

    url = '/Account/ywzysaveuser';
};


//用户编辑
function editUser() {

    var row = $('#cwkz_dg').datagrid('getSelected');

    if (row) {

        var id = row.Id;
        $('#juese').hide();
        if (row.role == "分管领导" || row.role == "领导") {
            $('#bumen').show();
            $('#suoshuxueyuan').combobox({ readonly: false });
            $('#suoshuxueyuan').combobox({ multiple: true });

            
        } else {
            $('#bumen').hide();
        }

        $('#pw').hide();
        $('#pw_cf').hide();
        $('#imf').hide();

        $('#cwkz_dlg').dialog('open').dialog('setTitle', '编辑用户');

        $('#cwkz_fm').form('load', row);
        $('#name').textbox('disable', true);

        url = '/Account/edit_user?id=' + id + '';
    }
    else {

        $.messager.alert("错误提示", "请选择要编辑的行！", "warning");
    }
};

function editpassword() {

    var row = $('#cwkz_dg').datagrid('getSelected');

    if (row) {
        var name = row.UserName;

        //alert(name)
        $('#restpaw_dlg').dialog('open').dialog('setTitle', '重置密码');

        url = '/Account/resetpassword_csh?name=' + name;
    }
    else {
        // alert("b");
        $.messager.alert("错误提示", "请选择要重置密码的用户！", "warning");
    }
};

function restpwd() {
    $('#restpaw_fm').form('submit', {
        url: url,
        onSubmit: function () {
            return $(this).form('validate');
        },
        success: function (result) {
            //var result = eval('(' + result + ')');
            result = JSON.parse(result);   //IE浏览器在后台操作完成后返回提示信息，转化为json字符串格式,在删除的方法中，加了这个后不能自动reload  dategrid

            $('#restpaw_dlg').dialog('close');		// close the dialog
            $('#cwkz_dg').datagrid('reload');	// reload the user data
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

    //对开始日期和结束日期进行判断

    $('#cwkz_fm').form('submit', {
        url: url,
        onSubmit: function () {
            return $(this).form('validate');
        },
        success: function (result) {
            //var result = eval('(' + result + ')');
            result = JSON.parse(result);   //IE浏览器在后台操作完成后返回提示信息，转化为json字符串格式,在删除的方法中，加了这个后不能自动reload  dategrid

            $('#cwkz_dlg').dialog('close');		// close the dialog
            $('#cwkz_dg').datagrid('reload');	// reload the user data
            if (result.success == false) {

                $.messager.show({
                    title: '错误提示',
                    msg: result.errorMsg
                });
            } else {

                $.messager.show({
                    title: '提示',
                    msg: result.errorMsg
                });
            }

        }
    });

};


function destroyUser() {
    var row = $('#cwkz_dg').datagrid('getSelected');
    if (row) {
        if (row.UserName == "admin") {
            $.messager.alert("错误提示", "admin用户不能删除！", "warning");
            return;
        }

        if (row.role == "部门负责人") {                     //如果删除的记录，这条记录人的角色是部门负责人
            $.messager.confirm('提示', '您确定要删除这条记录吗？删除后会清除该部门下所有人员！', function (r) {
                if (r) {
                    $.post('/Account/ywzydelBumen', { id: row.Id }, function (result) {


                        if (result.Succeeded) {

                            $('#cwkz_dg').datagrid('reload');	// reload the user data
                            $.messager.show({	// show error message
                                title: '提示',
                                msg: result.errorMsg
                            });

                        } else {
                            //  result = JSON.parse(result);   IE浏览器在后台操作完成后返回提示信息，转化为json字符串格式,在删除的方法中，加了这个后不能自动reload  dategrid


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
            $.messager.confirm('提示', '您确定要删除这条记录吗？', function (r) {
                if (r) {
                    $.post('/Account/ywzydelqitq', { id: row.Id }, function (result) {
                        if (result.Succeeded) {

                            $('#cwkz_dg').datagrid('reload');	// reload the user data
                            $.messager.show({	// show error message
                                title: '提示',
                                msg: result.errorMsg
                            });
                        } else {

                            // result = JSON.parse(result);   //IE浏览器在后台操作完成后返回提示信息，转化为json字符串格式,在删除的方法中，加了这个后不能自动reload  dategrid

                            $.messager.show({	// show error message
                                title: '错误提示',
                                msg: result.errorMsg
                            });
                        }
                    }, 'json');
                }
            });
        };

    }
    else {
        $.messager.alert("错误提示", "请选择要删除的行！", "warning");

    }
};

function yuangongliebiao(value, row, index) {


    if (row.role == "部门负责人") {
        if (value > 0) {

            return ss = '<a id="yg' + index + '" href="javascript:;" onmouseover="bmfzr_yuangong(' + row.myuserID + ',' + index + ')">' + value + '</a>';
        }
        else {
            return 0;
        };
    }
    else {
        return '--'
    }
};


function bmfzr_yuangong(myuserID, index) {

    $.post('/Account/bumenyuangong', { bmfzr_myuser_id: myuserID }, function (result) {

        if (result.success) {
            var ygh = "#yg" + index;

            //alert(ygh);
            $(ygh).tooltip({ content: result.Msg });
        }
    }, 'json');

};



//当所管学院字数太长，限定只显示前面一部分
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

