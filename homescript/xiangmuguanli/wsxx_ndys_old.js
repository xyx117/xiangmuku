
$(function () {

    if (ndys_yt == "1") {

        $('#ndys_fm').form('load', '/xiangmuguanli/load_ndys?xmid='+xmid+'&shijian='+sf);

    };

    $('#diyinian').textbox().textbox('addClearBtn', 'icon-clear');

    $('#diernian').textbox().textbox('addClearBtn', 'icon-clear');

    $('#disannian').textbox().textbox('addClearBtn', 'icon-clear');

    $('#yusuanshuoming').textbox().textbox('addClearBtn', 'icon-clear');


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

$.extend($.fn.textbox.defaults.rules, {
    numeric: {
        validator: function (value, param) {
            if (value) {
                return /^[0-9]*(\.[0-9]+)?$/.test(value);
            } else {
                return true;
            }
        },
        message: '只能输入数字.'
    }
});





//基本信息保存按钮
function add_ndys() {

    //对开始日期和结束日期进行判断


    if (ndys_yt == "1") {
        url = '/xiangmuguanli/nianduyusuan_edit?name='+xmname+'&xmid='+xmid;
    }
    else {
        url = '/xiangmuguanli/nianduyusuan_save?name='+xmname+'&xmid='+xmid;
    }


    $('#ndys_fm').form('submit', {
        url: url,

        //onSubmit: function () {
        //    //
        //    var diyi = $('#diyinian').textbox('getValue');

        //    var dier = $('#diernian').textbox('getValue');

        //    var disan = $('#disannian').textbox('getValue');

        //    var zhi=(parseFloat(diyi) + parseFloat(dier) + parseFloat(disan));

        //    if (zhi!=(jine*10).toFixed(3)) {

        //        $.messager.show({
        //            title: '错误提示',
        //            msg: "年度预算之和与项目总金额不符！"
        //        });
        //        return false;
        //    }
        //},

        success: function (result) {

            result = JSON.parse(result);   //IE浏览器在后台操作完成后返回提示信息，转化为json字符串格式

            if (result.success) {
                $.messager.show({
                    title: '提示',
                    msg: result.message
                });

                $('#ndyssave').linkbutton({ text: '更新' });
                ndys_yt = "1";

                //用于设置是否刷新首页的状态
                window.parent.parent.zongerchanged = 1;
            }
            else {

                $.messager.show({
                    title: '错误提示',
                    msg: result.message
                });
            }

        }
    })
};