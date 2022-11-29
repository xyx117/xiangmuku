

$(function () {


    $("#pw_dg").datagrid({
        singleSelect: true,
        async: false,
        collapsible: true,
        method: 'post',
        url: '/Account/pingweigetuser',
        toolbar: '#pw_toolbar',
        rownumbers: true,
        pagination: true,
        fitcolumns: true,
        fit: true,
        nowrap: false,
    });

    var p = $('#pw_dg').datagrid('getPager');

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

    $("#suoshuxueyuan").combobox({

        onChange: function (newvalue, oldValue) {

            var ss = $("#pingshenmulu").combobox('getValues');
            if (ss != "") {
                $.ajax({

                    //要用post方式
                    type: "Post",
                    contentType: "application/json; charset=utf-8",
                    //方法所在页面和方法名
                    url: encodeURI("/Account/pingshenshuliang?suoshumulu=" + ss + "&suoshuxueyue=" + newvalue),  //汉字乱码时候处理方法，加上encodeURI


                    dataType: "json",
                    success: function (data) {

                        if (data.success == true) {

                            $("#pingshenrenwu").textbox('setValue', data.pingshenshu + "/" + data.zongshu);

                            $("#pingshenrenwu1").html("评审数：" + data.pingshenshu + "/" + "总数:" + data.zongshu);

                        } else {


                            $("#pingshenrenwu").textbox('setValue', "0/0");
                            $("#pingshenrenwu1").html("评审数：0" + "/" + "总数:0");

                        }
                    }
                });
            }
        }
    });


    $("#pingshenmulu").combobox({

        onChange: function (newvalue, oldValue) {

            var ss = $("#suoshuxueyuan").combobox('getValues');
            //alert(ss);
            if (ss != "") {
                $.ajax({

                    //要用post方式
                    type: "Post",
                    contentType: "application/json; charset=utf-8",
                    //方法所在页面和方法名
                    url: encodeURI("/Account/pingshenshuliang?suoshumulu=" + newvalue + "&suoshuxueyue=" + ss),  //汉字乱码时候处理方法，加上encodeURI


                    dataType: "json",
                    success: function (data) {

                        if (data.success == true) {

                            $("#pingshenrenwu").textbox('setValue', data.pingshenshu + "/" + data.zongshu);

                            $("#pingshenrenwu1").html("评审数：" + data.pingshenshu + "/" + "总数:" + data.zongshu);

                        } else {

                            $("#pingshenrenwu").textbox('setValue', "0/0");
                            $("#pingshenrenwu1").html("评审数：0" + "/" + "总数:0");

                        }
                    }
                });

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
        message: '两次输入密码不一致！'
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



function newUser() {
    $('#pw_dlg').dialog('open').dialog('setTitle', '新增用户');

    $('#name').textbox('enable', 'true');
    $('#pw').show();
    $('#pw_cf').show();
    $('#imf').show();
    $('#pw_fm').form('clear');
    //因为在加载form时会进行clear动作，所以在role中设置的value也会被清空

    //这里做了一次过滤，已经有评委的部门不再显示在下拉列表中
    $('#suoshuxueyuan').combobox({
        url: '/Account/bumenlist?yxbm=' + '',
        valueField: 'id',
        textField: 'it',
        multiple: true,
        editable: false
    });

    $('#pingshenrenwu1').html("");

    url = '/Account/ywzy_pw_save';
};

//用户编辑
function editUser() {

    var row = $('#pw_dg').datagrid('getSelected');

    if (row) {



        var id = row.Id;
        //alert(id);

        $('#pw_dlg').dialog('open').dialog('setTitle', '编辑评委');

        var yxbmold = row.suoshuxueyuan;

        $('#suoshuxueyuan').combobox({
            url: '/Account/bumenlist?yxbm=' + encodeURI(yxbmold),
            valueField: 'id',
            textField: 'it',
            multiple: true,
            editable: false
        });
        $('#pw_fm').form('load', row);

        $('#name').textbox('disable', true);

        $('#pw').hide();
        $('#pw_cf').hide();
        $('#imf').hide();

        url = '/Account/edit_pw?id=' + id + '&username=' + encodeURI(row.UserName) + '&yxbmold=' + encodeURI(yxbmold);
    }
    else {

        $.messager.alert("错误提示", "请选择要编辑的行！", "warning");
    }
};


function editpassword() {

    var row = $('#pw_dg').datagrid('getSelected');
    // 将json对象转为字符串进行显示
    // var str = JSON.stringify(row);
    //alert(str);
    // alert(row.Kaishishijian)

    if (row) {
        var id = row.Id;       

        //alert(id)
        $('#restpaw_dlg').dialog('open').dialog('setTitle', '重置密码');


        //$("#bmxingzhi").combobox({ editable: false });//不可编辑，只能选择与editable="false"功能类似

        //$('#restpaw_dlg').form('load', row);


        url = '/Account/resetpassword_csh?users=' + id;
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

            result = JSON.parse(result);   //IE浏览器在后台操作完成后返回提示信息，转化为json字符串格式,在删除的方法中，加了这个后不能自动reload  dategrid
            
            $('#restpaw_dlg').dialog('close');		// close the dialog
            $('#pw_dg').datagrid('reload');	// reload the user data
            if (result.success == true) {
                //result = JSON.parse(result);   //IE浏览器在后台操作完成后返回提示信息，转化为json字符串格式,在删除的方法中，加了这个后不能自动reload  dategrid

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
    //};
}

function saveUser() {
    $('#pw_fm').form('submit', {
        url: url,
        onSubmit: function () {
            return $(this).form('validate');
        },
        success: function (result) {
            //var result = eval('(' + result + ')');
            result = JSON.parse(result);   //IE浏览器在后台操作完成后返回提示信息，转化为json字符串格式,在删除的方法中，加了这个后不能自动reload  dategrid

            $('#pw_dlg').dialog('close');		// close the dialog
            $('#pw_dg').datagrid('reload');	// reload the user data
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
    var row = $('#pw_dg').datagrid('getSelected');
    if (row) {
        if (row.UserName == "admin") {
            $.messager.alert("错误提示", "admin用户不能删除！", "warning");
            return;
        }


        $.messager.confirm('提示', '您确定要删除评委吗？', function (r) {
            if (r) {
                $.post('/Account/ywzydelpingwei', { id: row.Id, bumen: row.suoshuxueyuan }, function (result) {

                    if (result.Succeeded) {

                        $('#pw_dg').datagrid('reload');	// reload the user data
                        //$.messager.show({	// show error message
                        //    title: '提示',
                        //    msg: result.errorMsg
                        //});
                    } else {
                        //result = JSON.parse(result);   //IE浏览器在后台操作完成后返回提示信息，转化为json字符串格式,在删除的方法中，加了这个后不能自动reload  dategrid


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

//当所管学院字数太长，限定只显示前面一部分
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
        var ss = '<a href="#" title="' + value + '" class="easyui-tooltip">' + value1 + '</a>';
        return ss
    }
};

function pingshenjindu(value, row, index) {
    return "<a href='javascript:void(0)' onclick='pingshenjindu_chakan(" + index + ")'>查看</a>";
};

function pingshenjindu_chakan(index) {
    $('#pw_dg').datagrid('selectRow', index);
    var row = $('#pw_dg').datagrid('getSelected');

    if (row) {
        //var pingweiname = row.UserName;

        //alert(row.UserName);
        var pingweiname = escape(row.UserName);    //这里好像没什么区别

        //alert(pingweiname);

        $('#tubiao').attr("src", "/xiangmuguanli/pwjd_piechart?username=" + pingweiname);
        $('#pw_jindu').dialog('open').dialog('setTitle', '评委评审进度');



    }
};
