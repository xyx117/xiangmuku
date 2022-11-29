

$(function () {

    var sf = new Date().getTime();
    //解决url路径传中文值乱码    
    $("#dg_bsbmfzr").datagrid({
        singleSelect: true,
        async: false,
        collapsible: true,
        method: 'post',
        url: '/xiangmuguanli/bmfzr_getXiangmu',
        toolbar: '#bmfzr_toolbar',
        rownumbers: true,
        //pagination: true,
        autoRowheight: false,
        fit: true, //自动撑满
        //fitcolumns: true,
        nowrap: true,
        queryParams: {
            xiangmu: dgtitle,
            loingid: loingid,
            bmfzrname:username,
            dateshijian: sf
        },

        //为datagrid中的操作链接样式 设置一个按钮图形
        onLoadSuccess: function (data) {

            //填表
            $('.tianbiao').linkbutton({ text: '填表', plain: true });
            $('.tianbiao').addClass("c6");
            $('.tianbiao_jin').linkbutton({ text: '填表', disabled: true });
            //$('.tianbiao_jin').addClass("easyui-linkbutton");

            //审核
            $('.shenhe').linkbutton({ text: '审核', plain: true });
            $('.shenhe').addClass("c6");
            $('.shenhe_jin').linkbutton({ text: '审核', disabled: true });


            //送审
            $('.songshen').linkbutton({ text: '送审', plain: true });
            $('.songshen').addClass("c6");
            $('.songshen_jin').linkbutton({ text: '送审', disabled: true });

            //以下两句用于menubutton
            var timelu = $('#dg_bsbmfzr').datagrid('getRows').length;

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


function InputAction(value, row, index) {    //onmouseover

    return '<a href="javascript:void(0)" id="mb' + index + '" style="width:90%" class="easyui-menubutton c6" onclick="ShowMenu(' + index + ')">' + '更多' + '</a>';

};

function rizhi(index) {  //key 改index

    $('#dg_bsbmfzr').datagrid('selectRow', index);  //key 改index

    var row = $('#dg_bsbmfzr').datagrid('getSelected');
    if (row) {
        var xmname = row.XmName;
        var id = row.ID;
        var xmmulu = row.Xiangmumulu;
        //var tabTitle = name + "完善信息";
        var tabTitle = "流程：" + changdu(xmname);

        //var url = "/xiangmuguanli/liucheng?NoticeXmName=" + name + "&xmmulu=" + xmmulu + "&xmid=" + id;

        var url = "/xiangmuguanli/liucheng?xmname=" + xmname + "&loingid=" + loingid + "&xmid=" + id;

        var icon = "icon-edit";
        window.parent.addTab(tabTitle, url, icon);//使用新加的tab打开窗口
    }

};



function yidong(index) {  //key 改index

    //bmfzrtijiao == "未提交" || ywzyshenhe == "未通过"

    $('#dg_bsbmfzr').datagrid('selectRow', index);  //key 改index

    var row = $('#dg_bsbmfzr').datagrid('getSelected');

    if (row.bmfzrtijiao == "未提交" || row.ywzyshenhe == "未通过") {
        //if (row.lenght != 0) {

        //    $('#dlg_mulu').dialog('open').dialog('setTitle', '更改项目目录');

        //    var ID = row.ID;

        //    $('#fm_mulu').form('load', row);

        //    url = '/xiangmuguanli/yidongmulu?ID=' + ID;
        //}
        //else {
                    
        //    $.messager.alert("错误提示", "请选择要更改目录的行！", "warning");
        //}

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

            url = '/xiangmuguanli/yidongmulu?loingid='+loingid+'&ID='+ID;
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


function daochu(index) {  //key 改index
    $('#dg_bsbmfzr').datagrid('selectRow', index);  //key 改index

    var row = $('#dg_bsbmfzr').datagrid('getSelected');

    location.href("excel_out?id=" + row.ID);
};


//导出年度汇总表nianduhuizong
function nianduhuizong() {  //key 改index

    $.ajax({
        type: 'post',
        async: false,
        url: '/xiangmuguanli/checkpaixu',
        data: {
            "mulu": dgtitle,
            "loingid": loingid
        },
        success: function (data) {

            var data = JSON.parse(data);
            if (data.success) {
                window.location.href = "excel_out_huizong?mulu="+dgtitle+"&loingid="+loingid;
            } else {
                $.messager.show({	// show error message
                    title: '错误提示',
                    msg: data.Msg
                })
            }
        }
    });
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


//限定项目名称在tabtitle中的长度
function changdu(xm_name) {
    value1 = xm_name;
    if (value1.length > 15) {
        value1 = value1.substr(0, 15) + "...";
                
    }
    return value1;
};


//员工新增，部门负责人新增
function newxm_yg() {
    $('#dlg').dialog('open').dialog('setTitle', '新增项目');
    $('#fm').form('clear');

    $('#XmName').textbox({ disabled: false });

    url = '/xiangmuguanli/saveXiangmu?xiangmu='+dgtitle+'&loingid='+loingid+'&username='+username;  //这里要把登录人的loingid传过去是为了，通过这个loingid获得登录者的所属学院字段的值，并把这个值保存在项目中，为了在显示时作为过滤条件
};


//部门负责人编辑
function editxm_bmfzr() {

    var row = $('#dg_bsbmfzr').datagrid('getSelected');
    // 将json对象转为字符串进行显示
    // var str = JSON.stringify(row);

    if (row) {
        var xmid = row.ID;

        $('#dlg_edit').dialog('open').dialog('setTitle', '编辑项目');

        $('#fm_edit').form('load', row);

        url = '/xiangmuguanli/updateXiangmu?muluname=' + dgtitle + '&xmid=' + xmid;

    }
    else {
                
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

                $('#dg_bsbmfzr').datagrid('reload');	// reload the user data
                $.messager.show({
                    title: '提示',
                    msg: result.errorMsg
                });
            }
        }
    });
};


//部门负责人删除
function desxm_bmfzr() {
    var row = $('#dg_bsbmfzr').datagrid('getSelected');
    if (row) {
        $.messager.confirm('提示', '您确定要删除这条记录吗？', function (r) {
            if (r) {
                $.post('/xiangmuguanli/delXiangmu', { id: row.ID, xmname: row.XmName, mulu: dgtitle }, function (result) {
                    //这里把主键更改为项目名称后需要变动
                    //$.post('/xiangmuguanli/delXiangmu', { xmname: row.XmName }, function (result) {
                    if (result.success) {
                        $('#dg_bsbmfzr').datagrid('reload');	// reload the user data

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

            if (result.result == false) {

                result = JSON.parse(result);   //IE浏览器在后台操作完成后返回提示信息，转化为json字符串格式,在删除的方法中，加了这个后不能自动reload  dategrid

                $.messager.show({
                    title: '错误提示',
                    msg: result.errorMsg
                });
            } else {

                $('#dlg').dialog('close');		// close the dialog

                //$('#dg_yg').datagrid('reload');	// reload the user data

                $('#dg_bsbmfzr').datagrid('reload');	// reload the user data
            }
        }
    });
};


//移动目录




//部门负责人排序
function paixuXiangmu() {

    $('#dlg_paixu').dialog('open').dialog('setTitle', '项目排序');
    var rows = $('#dg_bsbmfzr').datagrid('getRows');

    $('#sortable').empty();
    for (var i = 0; i < rows.length; i++) {
        $('#sortable').append("<li id='" + rows[i].ID + "' class='drag-item'>" + rows[i].XmName + "</li>");
                
    }

            
    var indicator = $(".indicator");
            
    $('.drag-item').draggable({
        revert: true,
        deltaX: 0,
        deltaY: 0
    }).droppable({
        onDragOver: function (e, source) {

            //纠正显示坐标偏移量
            var l = $('#dlg_paixu').offset().left + 10;
            var r = $('#dlg_paixu').offset().top + 5;
                    
            indicator.css({
                display: 'block',
                left: $(this).offset().left - l,
                top: $(this).offset().top + 2 * $(this).outerHeight() - r,
            });

        },
        onDragLeave: function (e, source) {
                    
            indicator.hide();
        },
        onDrop: function (e, source) {
                    
            $(source).insertAfter(this);
            indicator.hide();
        }
    });
};

//排序保存
function paixuxiansh() {

    var idstr = "";

    $("#sortable li").each(function () {
        // 将选中的字段id拼成字符串
        idstr += $(this).attr("id") + ",";


    });
    if (idstr.length > 0) {
        idstr = idstr.substring(0, idstr.length - 1);

        $.ajax({
            //要用post方式
            type: "Post",
            //方法所在页面和方法名
            url: "/xiangmuguanli/paixu?paixu=" + idstr,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {
                //返回的数据用data.d获取内容

                $('#dlg_paixu').dialog('close');		// close the dialog
                $('#dg_bsbmfzr').datagrid('reload');

                $.messager.show({
                    title: '提示',
                    msg: data.message
                });
            }
        });
    } else {
        return;
    }
};

////当项目名称字数太长，限定只显示前面一部分
function TitleFormatter(value, row, index) {
    var value1 = value;
    if (value1 == null) {
        var ss = '<a href="#" title="' + value + '" onclick="bmfzrchakan(' + index + ')" class="easyui-tooltip"></a>';
        return ss
    }
    else {
        if (value1.length > 15) {
            value1 = value1.substr(0, 15) + "...";
        }
        var ss = '<a href="#" title="' + value + '" onclick="bmfzrchakan(' + index + ')" class="easyui-tooltip">' + value1 + '</a>';
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
        if (value.length > 13) {
            value1 = value.substr(0, 13) + "...";
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

//部门负责人查看
function bmfzrchakan(index) {
    $('#dg_bsbmfzr').datagrid('selectRow', index);
    var row = $('#dg_bsbmfzr').datagrid('getSelected');
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


//部门负责人完善信息
function rowformater_bmfzr(value, row, index) {
    var tjzhuantai = row.tijiaozhuantai;  //员工提交状态
    var bmfzrtj = row.bmfzrtijiao;
    var bmfzrshenhe = row.bmfzrshenhe;


    if (bmfzrtj == "未提交" || bmfzrshenhe == "未审核") {           //部门负责人的完善信息，只有在未审核和未提交时候才能使用
        return "<a href='javascript:void(0)' style='width:95%' class='tianbiao' onclick='wsxx_bmfzr(" + index + ")' target='mainFrame'>填表</a>";
    }
    else {
        return "<a href='javascript:void(0)' style='width:95%' class='tianbiao_jin'></a>";
    }
};

//部门负责人完善信息
function wsxx_bmfzr(index) {
    $('#dg_bsbmfzr').datagrid('selectRow', index);
    var row = $('#dg_bsbmfzr').datagrid('getSelected');
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


//部门负责人审核，审核其实就是一个编辑按钮，先查看后审核
function bmfzr_shenhe(value, row, index) {

    var bmfzrshenhe = row.bmfzrshenhe;
    var fgldshenhe = row.fgldshenhe;
    var ywzyshenhe = row.ywzyshenhe;
    var cjzusername = row.username;
    var tijiaozhuantai = row.tijiaozhuantai;
    var bmfzrtijiao = row.bmfzrtijiao;

    //如果是部门负责人 自己创建的项目，那就不需要经过审核
    if (cjzusername == username) {

        return "<a href='javascript:void(0)' style='width:95%' class='shenhe_jin'>审核</a>";
    }
    else {
        if ((bmfzrshenhe == "未审核" && tijiaozhuantai == "已提交") || (bmfzrtijiao == "未提交") || (bmfzrshenhe == "通过" && fgldshenhe == "未通过")) {

            return "<a href='javascript:void(0)' style='width:95%' class='shenhe' onclick='shenhecaozuo(" + index + ")' target='mainFrame'>审核</a>";
        }
        else {
            return "<a href='javascript:void(0)' style='width:95%' class='shenhe_jin'>审核</a>";
        }

    }
};

//审核操作弹出对话框
function shenhecaozuo(index) {
    $('#fm_shbmfzr').form('clear');
    $('#dg_bsbmfzr').datagrid('selectRow', index);
    var row = $('#dg_bsbmfzr').datagrid('getSelected');
    //此处要参考一下编辑方法
    if (row) {
        $('#shenhe_dlg').dialog('open').dialog('setTitle', '部门负责人审核');
                
        var name = row.XmName;
        var id = row.ID;
        var xmmulu = row.Xiangmumulu;

        url = '/xiangmuguanli/bmfzr_shenhe?name=' + name + '&loingid='+loingid+'&xmid='+id;

    }

};

//审核操作弹出对话框后点击确认保存
function bmfzrshenhe_save() {

    $('#fm_shbmfzr').form('submit', {
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
                $('#shenhe_dlg').dialog('close');		// close the dialog

                $('#dg_bsbmfzr').datagrid('reload');	// reload the user data
                $.messager.show({
                    title: '提示',
                    msg: result.errorMsg
                });
            }
        }
    });
};



//部门负责人提交操作
function bmfzr_tijiao(value, row, index) {
    var zhi = row.tijiaozhuantai;
    var bmfzrtijiao = row.bmfzrtijiao;
    var fgldshenhe = row.fgldshenhe;
    var ywzyshenhe = row.ywzyshenhe;
            
    if (bmfzrtijiao == "未提交" || (bmfzrtijiao == "未提交" && fgldshenhe == "未通过") || (bmfzrtijiao == "未提交" && ywzyshenhe == "未通过")) {
        return "<a href='javascript:void(0)' style='width:95%' class='songshen' onclick='bmfzr_tj(" + index + ")' target='mainFrame'>送审</a>";
    }
    else {
        //已经提交的不能再次提交，已经提交但分管领导和业务专员未审核或通过的，不能再提交
        return "<a href='javascript:void(0)' style='width:95%' class='songshen_jin'>送审</a>";
    }
};

//部门负责人提交操作
function bmfzr_tj(index) {

    //$('#tijiao_fm').form('clear');
    $('#dg_bsbmfzr').datagrid('selectRow', index);
    var row = $('#dg_bsbmfzr').datagrid('getSelected');
    if (row) {
        $('#tj_bmfzr_dlg').dialog('open').dialog('setTitle', '提示');

        var id = row.ID;

        //var url = "/xiangmuguanli/tijiao?NoticeXmName=" + name + "&xmmulu=" + xmmulu + "&xmid=" + id;
        url = "/xiangmuguanli/bmfzrtijiao?loingid="+loingid+"&shijian="+sf+"&xmid="+id;
    }
};
//提交保存
function tj_bmfzr_save() {
    $('#tj_bmfzr_fm').form('submit', {
        url: url,
        onSubmit: function () {
            return $(this).form('validate');
        },
        success: function (result) {
                    
            result = JSON.parse(result);   //IE浏览器在后台操作完成后返回提示信息，转化为json字符串格式,在删除的方法中，加了这个后不能自动reload  dategrid

            if (result.success == 1) {
                $('#tj_bmfzr_dlg').dialog('close');		// close the dialog
                $('#dg_bsbmfzr').datagrid('reload');


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
function rizhichakan(index) {
    $('#dg_bsbmfzr').datagrid('selectRow', index);
    var row = $('#dg_bsbmfzr').datagrid('getSelected');
    if (row) {
        var xmname = row.XmName;
        var id = row.ID;
        var xmmulu = row.Xiangmumulu;
        //var tabTitle = name + "完善信息";
        var tabTitle = "流程：" + changdu(xmname);

        var url = "/xiangmuguanli/liucheng?xmname=" + xmname + "&loingid="+loingid+"&xmid="+id;

        var icon = "icon-edit";
        window.parent.addTab(tabTitle, url, icon);//使用新加的tab打开窗口
    }
};

//项目移动目录,弹出对话框
function shenhe_yidong() {

    var row = $('#dg_bsbmfzr').datagrid('getSelected');

    if (row.lenght != 0) {

        $('#dlg_mulu').dialog('open').dialog('setTitle', '更改项目目录');

        var ID = row.ID;

        $('#fm_mulu').form('load', row);

        url = '/xiangmuguanli/yidongmulu?ID=' + ID;
    }
    else {
                
        $.messager.alert("错误提示", "请选择要更改目录的行！", "warning");
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

                $('#dg_bsbmfzr').datagrid('reload');	// reload the user data
                $.messager.show({
                    title: '提示',
                    msg: result.errorMsg
                });
            }
        }
    });

};