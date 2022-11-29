$(function () {
    $('#userpassword').textbox().textbox('addClearBtn', 'icon-clear');
    $('#confirmPassword').textbox().textbox('addClearBtn', 'icon-clear');

    //验证两次密码输入是否相同
    $.extend($.fn.validatebox.defaults.rules, {
        /*必须和某个字段相等*/
        style_input: {
            validator: function (value, param) {
                if (value) {
                    return /^(?=.*[0-9])(?=.*[a-zA-Z])(?=.*[\x21-\x2F\x3A-\x40\x5b-\x60\x7b-\x7e])[0-9a-zA-Z\x21-\x2F\x3A-\x40\x5b-\x60\x7b-\x7e]{8,}$/.test(value);
                } else {
                    return true;
                }
            },
            message: '请输入包含数字、字母和特殊符号，长度至少八位的密码'
        },
        equalTo: {
            validator: function (value, param) {
                return $(param[0]).val() == value;
            },
            message: '两次输入密码不一致！'
        }   
    });
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


function restpwd() {

    $('#setpaw_fm').form('submit', {

        url: '/Account/setpwd?userid='+loingid,
        onSubmit: function () {
            return $(this).form('validate');
        },
        success: function (result) {
            //var result = eval('(' + result + ')');
            result = JSON.parse(result);
            if (result.success == true) {
                //result = JSON.parse(result);   //IE浏览器在后台操作完成后返回提示信息，转化为json字符串格式,在删除的方法中，加了这个后不能自动reload  dategrid

                $.messager.show({
                    title: '提示！',
                    msg: result.Msg
                });

                $('#setpaw_fm').form('clear');

            } if (result.success == false) {
                $.messager.show({
                    title: '错误提示！',
                    msg: result.Msg
                });
                $('#setpaw_dlg').dialog('close');		// close the dialog

            }
        }
    });
    //alert("sdf")
};