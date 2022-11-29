

$(function () {

    var sf = new Date().getTime();

    //解决url路径传中文值乱码
    $("#dg_yg").datagrid({
        singleSelect: true,
        collapsible: true,
        url: '/xiangmuguanli/yg_getXiangmu',
        method: 'post',
        toolbar: '#yg_toolbar',
        pagination: true,
        rownumbers: true,
        fit: true,
        autoRowheight: false,
        //nowrap: false,
        queryParams: {
            xiangmu: dgtitle,
            loingid: loingid,
            username: username,
            dateshijian: sf
        },

        //为datagrid中的操作链接样式 设置一个按钮图形
        onLoadSuccess: function (data) {

            //填表
            $('.tianbiao').linkbutton({ text: '填表', plain: true });
            $('.tianbiao').addClass("c6");
            $('.tianbiao_jin').linkbutton({ text: '填表', disabled: true });
            //$('.tianbiao_jin').addClass("easyui-linkbutton");

            //送审
            $('.tijiao').linkbutton({ text: '提交', plain: true });
            $('.tijiao').addClass("c6");
            $('.tijiao_jin').linkbutton({ text: '提交', disabled: true });
            //$('.tijiao_jin').addClass("easyui-linkbutton");

            //日志
            $('.rizhi').linkbutton({ text: '日志', plain: true });
            $('.rizhi').addClass("c6");

            //以下两句用于menubutton
            var timelu = $('#dg_yg').datagrid('getRows').length;

            for (i = 0; i < timelu; i++) {

                $('#mb' + i).menubutton({
                    //iconCls: 'icon-gengduo',

                    menu: '#mm3',
                    duration: 2000000  //更多的按钮加一个时间延时,将onmouseover功能改为点击
                });

            };


            //显示简介的详细内容
            $('.jianjie_all').tooltip({
                position: 'left',
                onShow: function () {
                    $(this).tooltip('tip').css({
                        width: '60%'
                    });
                }
            });

        }

    });


    var p = $('#dg_yg').datagrid('getPager');

    $(p).pagination({

        pageSize: 10,//每页显示的记录条数，默认为10

        pageList: [5, 10, 15, 20, 25, 30, 35, 40, 45, 50, 55, 60, 65, 70, 75, 80, 85, 90, 95, 100],//可以设置每页记录条数的列表

        beforePageText: '第',//页数文本框前显示的汉字

        afterPageText: '页    共 {pages} 页',

        displayMsg: '当前显示 {from} - {to} 条记录   共 {total} 条记录',
    });



    $('#XmName').textbox().textbox('addClearBtn', 'icon-clear');

    $('#zongjine').textbox().textbox('addClearBtn', 'icon-clear');

    $('#Beizhu').textbox().textbox('addClearBtn', 'icon-clear');

});


//用来验证文本框中的内容是否已经存在（即重名）
$.extend($.fn.validatebox.defaults.rules, {
    checknameissame: {
        validator: function (value, param) {
            var name = value.trim();
            var mulu = dgtitle;

            var result = "";
            $.ajax({
                type: 'post',
                async: false,
                url: '/xiangmuguanli/checkNameIsSame',
                data: {
                    "name": name,
                    "mulu": mulu
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

$.extend($.fn.combobox.methods, {
    selectedIndex: function (jq, index) {
        if (!index)
            index = 0;
        var data = $(jq).combobox('options').data;
        var vf = $(jq).combobox('options').valueField;
        $(jq).combobox('setValue', eval('data[index].' + vf));
    }
});

$.extend($.fn.validatebox.defaults.rules, {
    isBlank: {
        validator: function (value, param) { return $.trim(value) != '' },
        message: '不能为空，全空格也不行'
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


//限定项目名称在tabtitle中的长度
function changdu(xm_name) {
    value1 = xm_name;
    if (value1.length > 15) {
        value1 = value1.substr(0, 15) + "...";

    }
    return value1;
};

//更多
function InputAction(value, row, index) {    //onmouseover

    return '<a href="javascript:void(0)" id="mb' + index + '" style="width:90%" class="easyui-menubutton c6" onclick="ShowMenu(' + index + ')">' + '更多' + '</a>';

};

function ShowMenu(index) {  //key 改index

    $('#mm3').menu({

        onClick: function (item) {

            if (item.id != undefined && eval(item.id) != undefined && $.isFunction(eval(item.id))) {

                item.onclick = eval(item.id + "(" + index + ")");  //key 改index
            }
        }
    });
};



//员工新增，部门负责人新增
function newxm_yg() {
    $('#dlg').dialog('open').dialog('setTitle', '新增项目');
    $('#fm').form('clear');

    $('#XmName').textbox({ disabled: false });

    url = '/xiangmuguanli/saveXiangmu?xiangmu='+dgtitle+'&loingid='+loingid+'&username='+username;  //这里要把登录人的loingid传过去是为了，通过这个loingid获得登录者的所属学院字段的值，并把这个值保存在项目中，为了在显示时作为过滤条件
};


//员工编辑
function editxm_yg() {

    var row = $('#dg_yg').datagrid('getSelected');
    // 将json对象转为字符串进行显示
    // var str = JSON.stringify(row);

    if (row) {

        var xmid = row.ID;

        $('#dlg_edit').dialog('open').dialog('setTitle', '编辑项目');

        $('#fm_edit').form('load', row);

        url = '/xiangmuguanli/updateXiangmu?muluname=' + dgtitle + '&xmid=' + xmid;
    }
    else {
        // alert("b");
        $.messager.alert("错误提示", "请选择要编辑的行！", "warning");
    }
};



//员工保存，部门负责人编辑保存
function saveXiangmu_edit() {
    //alert(url);
    $('#fm_edit').form('submit', {
        url: url,
        onSubmit: function () {
            return $(this).form('validate');
        },
        success: function (result) {
            //var result = eval('(' + result + ')');

            result = JSON.parse(result);   //IE浏览器在后台操作完成后返回提示信息，转化为json字符串格式,在删除的方法中，加了这个后不能自动reload  dategrid

            if (result.success == false) {

                $.messager.show({
                    title: '错误提示',
                    msg: result.errorMsg
                });
            }
            //else {
            if (result.success == true) {
                $('#dlg_edit').dialog('close');		// close the dialog

                $('#dg_yg').datagrid('reload');	// reload the user data
                $.messager.show({
                    title: '提示',
                    msg: result.errorMsg
                });
            }
        }
    });
};


//员工删除
function desxm_yg() {
    var row = $('#dg_yg').datagrid('getSelected');
    if (row) {
        $.messager.confirm('提示', '您确定要删除这条记录吗？', function (r) {
            if (r) {
                $.post('/xiangmuguanli/delXiangmu', { id: row.ID, xmname: row.XmName, mulu: dgtitle }, function (result) {
                    //这里把主键更改为项目名称后需要变动
                    //$.post('/xiangmuguanli/delXiangmu', { xmname: row.XmName }, function (result) {
                    if (result.success) {
                        $('#dg_yg').datagrid('reload');	// reload the user data

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




//员工保存，部门负责人保存
function saveXiangmu() {

    $('#fm').form('submit', {
        url: url,
        onSubmit: function () {
            return $(this).form('validate');
        },
        success: function (result) {
            //var result = eval('(' + result + ')');
            result = JSON.parse(result);   //IE浏览器在后台操作完成后返回提示信息，转化为json字符串格式,在删除的方法中，加了这个后不能自动reload  dategrid
            $('#dlg').dialog('close');		// close the dialog

            $('#dg_yg').datagrid('reload');	// reload the user data
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




////当项目名称字数太长，限定只显示前面一部分
function TitleFormatter(value, row, index) {
    var value1 = value;
    if (value1 == null) {
        var ss = '<a href="#" title="' + value + '" onclick="ygchakan(' + index + ')" class="easyui-tooltip"></a>';
        return ss
    }
    else {
        if (value1.length > 15) {
            value1 = value1.substr(0, 15) + "...";
        }
        var ss = '<a href="#" title="' + value + '" onclick="ygchakan(' + index + ')" class="easyui-tooltip">' + value1 + '</a>';
        return ss
    }
};


//当项目名称字数太长，限定只显示前面一部分
function TitleFormatter_jj(value, row, index) {
    if (value == null) {
        var ss = '';
        return ss
    }
    else {
        var value1 = value;
        if (value.length > 15) {
            value1 = value.substr(0, 15) + "...";
        }

        var ss = '<a href="javascript:;" title="' + value + '" class="jianjie_all">' + value1 + '</a>';
        return ss
    }
};


//员工界面部门负责人图标
function bmfzrtubiao(value, row, index) {
    var bmfzrshenhe = row.bmfzrshenhe;
    switch (bmfzrshenhe) {
        case "通过": return "<Image src='/Scripts/easyui/themes/icons/ok.png' Title='通过'/>";
            break;
        case "未审核": return "<Image src='/Scripts/easyui/themes/icons/question.png' Title='未审核'/>";
            break;
        case "未通过": return "<Image src='/Scripts/easyui/themes/icons/no.png' Title='未通过'/>";
            break;
    }
};

//员工界面分管领导人图标
function fgldtubiao(value, row, index) {
    var fgldshenhe = row.fgldshenhe;
    switch (fgldshenhe) {
        case "通过": return "<Image src='/Scripts/easyui/themes/icons/ok.png' Title='通过'/>";
            break;
        case "未审核": return "<Image src='/Scripts/easyui/themes/icons/question.png' Title='未审核'/>";
            break;
        case "未通过": return "<Image src='/Scripts/easyui/themes/icons/no.png' Title='未通过'/>";
            break;
    }
};

//员工界面业务专员图标
function ywzybubiao(value, row, index) {
    var ywzyshenhe = row.ywzyshenhe;
    //var pingweishenhe = row.pingweishenhe;
    switch (ywzyshenhe) {
        case "通过": return "<Image src='/Scripts/easyui/themes/icons/ok.png' Title='通过'/>";
            break;
        case "未审核": return "<Image src='/Scripts/easyui/themes/icons/question.png' Title='未审核'/>";
            break;
        case "未通过": return "<Image src='/Scripts/easyui/themes/icons/no.png' Title='未通过'/>";
            break;
    }
};


//员工查看
//function yg_chakan(value, row, index) {
//    //alert("sdf")
//    return "<a href='javascript:void(0)' style='width:60%' class='chakan' onclick='ygchakan(" + index + ")' target='mainFrame'>查看</a>";

//};

//员工查看
function ygchakan(index) {
    $('#dg_yg').datagrid('selectRow', index);
    var row = $('#dg_yg').datagrid('getSelected');
    if (row) {
        var name = row.XmName;
        var id = row.ID;
        var xmmulu = row.Xiangmumulu;
        //var tabTitle = name + "查看";
        var tabTitle = "查看：" + changdu(name);
        var url = "/xiangmuguanli/chakan?NoticeXmName=" + name + "&xmmulu=" + xmmulu + "&xmid=" + id;
        var icon = "icon-shenhe";
        window.parent.addTab(tabTitle, url, icon);//使用新加的tab打开窗口
    }
};


//在下面的函数中参数（value，row，index）是必须要有的
//员工完善信息
function rowformater(value, row, index) {

    var tjzhuangtai = row.tijiaozhuantai;
    if (tjzhuangtai == "未提交") {
        return "<a href='javascript:void(0)' style='width:80%' class='tianbiao' onclick='wanshanxinxi(" + index + ")' target='mainFrame'>填表</a>";

    }
    else {
        return "<a href='javascript:void(0)' style='width:80%' class='tianbiao_jin'>填表</a>";
    }
};


//员工完善信息
function wanshanxinxi(index) {
    $('#dg_yg').datagrid('selectRow', index);
    var row = $('#dg_yg').datagrid('getSelected');
    if (row) {
        var name = row.XmName;
        var id = row.ID;
        var xmmulu = row.Xiangmumulu;
        //var tabTitle = name + "完善信息";
        var tabTitle = "填表：" + changdu(name);

        var url = "/xiangmuguanli/wanshanxinxi?NoticeXmName=" + name + "&xmmulu=" + xmmulu + "&xmid=" + id;
        var icon = "icon-edit";
        window.parent.addTab(tabTitle, url, icon);//使用新加的tab打开窗口
    }
};


//员工已完善信息
function yiwanshanxinxi() {
    $.messager.show({	// show error message
        title: '提示信息',
        msg: "记录已经完善并且提交，不能更改！"
    });
};


//员工提交操作
function yg_tijiao(value, row, index) {
    var zhi = row.tijiaozhuantai;

    if (zhi == "未提交") {
        return "<a href='javascript:void(0)' style='width:80%' class='tijiao' onclick='tijiao(" + index + ")' target='mainFrame'>提交</a>";
    }
    else {
        return "<a href='javascript:void(0)' style='width:80%' class='tijiao_jin'>提交</a>";
    }
};

//员工提交操作
function tijiao(index) {

    //$('#tijiao_fm').form('clear');
    $('#dg_yg').datagrid('selectRow', index);
    var row = $('#dg_yg').datagrid('getSelected');
    if (row) {
        $('#tijiao_dlg').dialog('open').dialog('setTitle', '提示');
        var name = row.XmName;
        var id = row.ID;
        var xmmulu = row.Xiangmumulu;

        //var url = "/xiangmuguanli/tijiao?NoticeXmName=" + name + "&xmmulu=" + xmmulu + "&xmid=" + id;
        url = "/xiangmuguanli/tijiao?loingid="+loingid+"&shijian="+sf+"&xmid=" + id;
    }
};


function tijiao_save() {
    $('#tijiao_fm').form('submit', {
        url: url,
        onSubmit: function () {
            return $(this).form('validate');
        },
        success: function (result) {
            //var result = eval('(' + result + ')');

            result = JSON.parse(result);   //IE浏览器在后台操作完成后返回提示信息，转化为json字符串格式,在删除的方法中，加了这个后不能自动reload  dategrid


            if (result.Succeeded) {
                $('#tijiao_dlg').dialog('close');		// close the dialog
                $('#dg_yg').datagrid('reload');
                $.messager.show({
                    title: '提示',
                    msg: result.Succeeded
                });
            }
            else {
                $.messager.show({
                    title: '提示',
                    msg: result.errorMsg
                });
            }
        }
    });
};

//审核流程日志
function ygxmrizhi(value, row, index) {
    //alert("sdf")
    return "<a href='javascript:void(0)' style='width:80%' class='rizhi' onclick='rizhichakan(" + index + ")' target='mainFrame'>日志</a>";

};
//审核流程日志
function rizhichakan(index) {
    $('#dg_yg').datagrid('selectRow', index);
    var row = $('#dg_yg').datagrid('getSelected');
    if (row) {
        var xmname = row.XmName;
        var id = row.ID;
        var xmmulu = row.Xiangmumulu;
        //var tabTitle = name + "完善信息";
        var tabTitle = "流程：" + changdu(xmname);

        //var url = "/xiangmuguanli/liucheng?NoticeXmName=" + name + "&xmmulu=" + xmmulu + "&xmid=" + id;

        var url = "/xiangmuguanli/liucheng?xmname=" + xmname + "&loingid="+loingid+"&xmid="+id;

        var icon = "icon-edit";
        window.parent.addTab(tabTitle, url, icon);//使用新加的tab打开窗口
    }
};


function rizhi(index) {  //key 改index

    $('#dg_yg').datagrid('selectRow', index);  //key 改index

    var row = $('#dg_yg').datagrid('getSelected');
    if (row) {
        var xmname = row.XmName;
        var id = row.ID;
        var xmmulu = row.Xiangmumulu;
        //var tabTitle = name + "完善信息";
        var tabTitle = "流程：" + changdu(xmname);

        var url = "/xiangmuguanli/liucheng?xmname=" + xmname + "&loingid="+loingid+"&xmid=" + id;

        var icon = "icon-edit";
        window.parent.addTab(tabTitle, url, icon);//使用新加的tab打开窗口
    }
};

function yidong(index) {  //key 改index

    $('#dg_yg').datagrid('selectRow', index);  //key 改index

    var row = $('#dg_yg').datagrid('getSelected');

    //if (row.tijiaozhuantai == "未提交" || row.ywzyshenhe == "未通过") {
    //    if (row.lenght != 0) {

    //        $('#dlg_mulu').dialog('open').dialog('setTitle', '更改项目目录');

    //        var ID = row.ID;

    //        $('#fm_mulu').form('load', row);

    //        url = '/xiangmuguanli/yidongmulu?ID=' + ID;
    //    }
    //    else {

    //        $.messager.alert("错误提示", "请选择要更改目录的行！", "warning");
    //    }
    //} else {
    //    $.messager.show({
    //        title: '提示',
    //        msg: "当前项目还在审核流程中，不能移动！"
    //    });
    //}

    if (row.tijiaozhuantai == "未提交" || row.ywzyshenhe == "未通过") {

        $('#ydmulu').textbox({ disabled: true });

        if (row.lenght != 0) {

            var ID = row.ID;
            var xmmulu = row.Xiangmumulu;

            $('#dlg_mulu').dialog('open').dialog('setTitle', '更改项目目录');


            $.ajax({
                type: "POST",
                url: "/xiangmuguanli/mululist",
                dataType: "json",
                data: {
                    xmmulu: xmmulu
                },
                success: function (json) {

                    if (json.length > 0) {
                        $('#ydmulu').combobox({
                            //data: json.rows,这是错的写法
                            data: json,
                            valueField: 'id',
                            textField: 'text',
                            editable: false
                        });
                        $('#ydmulu').combobox('selectedIndex', 0);
                    }
                }

            });


            url = '/xiangmuguanli/yidongmulu?loingid='+loingid+'&ID=' + ID;
        }
        else {
            //alert("b");
            $.messager.alert("错误提示", "请选择要更改目录的行！", "warning");
        }
    } else {
        $.messager.show({
            title: '提示',
            msg: "当前项目还在审核流程中，不能移动！"
        });
    }
};

////保存移动目录
function mulu_save() {

    $('#fm_mulu').form('submit', {
        url: url,
        onSubmit: function () {
            return $(this).form('validate');
        },
        success: function (result) {

            result = JSON.parse(result);   //IE浏览器在后台操作完成后返回提示信息，转化为json字符串格式,在删除的方法中，加了这个后不能自动reload  dategrid

            if (result.success == false) {

                $.messager.show({
                    title: '错误提示',
                    msg: result.errorMsg
                });
            }

            if (result.success == true) {
                $('#dlg_mulu').dialog('close');		// close the dialog

                $('#dg_yg').datagrid('reload');	// reload the user data
                $.messager.show({
                    title: '提示',
                    msg: result.errorMsg
                });
            }
        }
    });
};

function daochu(index) {  //key 改index
    $('#dg_yg').datagrid('selectRow', index);  //key 改index

    var row = $('#dg_yg').datagrid('getSelected');

    //window.href("excel_out?id=" + row.ID);

    window.location.href = "excel_out?id=" + row.ID;

    //return "<a href='/xiangmuguanli/excel_out?id=" + row.ID + "' style='width:90%' class='daochu'>11导出execl</a>";

};