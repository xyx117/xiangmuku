
$(function () {

    if (jbxx_yt == "1") {

        $('#jbxx_fm').form('load', '/xiangmuguanli/load_jbxx?xmid='+xmid+'&shijian='+sf);
    };
    //Jieshushijian    Kaishishijian


    $('#Zhengceyiju').textbox().textbox('addClearBtn', 'icon-clear');

    $('#Xiangmubeijing').textbox().textbox('addClearBtn', 'icon-clear');

    $('#shishidizhi').textbox().textbox('addClearBtn', 'icon-clear');

    $('#jingbanren').textbox().textbox('addClearBtn', 'icon-clear');

    $('#Lianxidianhua').textbox().textbox('addClearBtn', 'icon-clear');

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



//基本信息保存按钮
function add_Form() {

    //对开始日期和结束日期进行判断
    var sdate = $('#star').datebox('getValue');

    var edate = $('#end').datebox('getValue');
    if (sdate > edate) {
        $.messager.alert("错误提示", "结束日期要大于开始日期！", "warning");
    }

    if (jbxx_yt == "1") {
        url = '/xiangmuguanli/edit_jbxx?name='+xmname+'&xmid='+xmid;
    }
    else {
        url = '/xiangmuguanli/add_jbxx?name='+xmname+'&xmid='+xmid;
    }

    //$.messager.progress();	// 提示进度条
    $('#jbxx_fm').form('submit', {
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
                //$("#addsave").linkbutton("disable");
                //$("#reset").linkbutton("disable");

                $('#addjbxx').linkbutton({ text: '更新' });
                jbxx_yt = "1";
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