﻿$(function () { var searchquery = "所有评审部门"; $("#dg_shpw_wc").datagrid({ singleSelect: false, selectOnCheck: true, checkOnSelect: true, async: false, collapsible: true, method: 'post', url: '/xiangmuguanli/pw_shenhe_xm_wc', toolbar: '#pl_pwtj_toolbar', rownumbers: true, pagination: true, autoRowheight: false, fit: true, view: groupview, groupField: 'suoshuxueyuan', groupFormatter: function (value, rows) { return value + ' - ' + rows.length + ' 项' }, queryParams: { xiangmu: dgtitle, loingid: loingid, searchquery: searchquery }, onLoadSuccess: function (data) { $('.pwtijiao').linkbutton({ text: '提交', plain: true }); $('.pwtijiao').addClass("c6"); $('.pwtijiao_jin').linkbutton({ text: '提交', disabled: true }); $('.zaishen').linkbutton({ text: '再审', plain: true }); $('.zaishen').addClass("c6"); $('.zaishen_jin').linkbutton({ text: '再审', disabled: true }); $('.rizhi').linkbutton({ text: '日志', plain: true }); $('.rizhi').addClass("c6"); $('.jianjie_all').tooltip({ position: 'left', onShow: function () { $(this).tooltip('tip').css({ width: '60%' }) } }) } }); var wc = $('#dg_shpw_wc').datagrid('getPager'); $(wc).pagination({ pageSize: 10, pageList: [5, 10, 15, 20, 25, 30, 35, 40, 45, 50, 55, 60, 65, 70, 75, 80, 85, 90, 95, 100], beforePageText: '第', afterPageText: '页    共 {pages} 页', displayMsg: '当前显示 {from} - {to} 条记录   共 {total} 条记录', }); $('#suoshuxueyuan').combobox({ select: '所有评审部门', onChange: function (newValue, oldValue) { $('#dg_shpw_wc').datagrid('load', { "xiangmu": dgtitle, "loingid": loingid, "searchquery": newValue }) } }) }); $.extend($.fn.combobox.methods, { selectedIndex: function (jq, index) { if (!index) index = 0; var data = $(jq).combobox('options').data; var vf = $(jq).combobox('options').valueField; $(jq).combobox('setValue', eval('data[index].' + vf)) } }); function changdu(xm_name) { value1 = xm_name; if (value1.length > 15) { value1 = value1.substr(0, 15) + "..."; } return value1 }; function TitleFormatter(value, row, index) { var value1 = value; if (value1 == null) { var ss = '<a href="#" title="' + value + '" class="easyui-tooltip"></a>'; return ss } else { if (value1.length > 15) { value1 = value1.substr(0, 15) + "..." } var ss = '<a href="#" title="' + value + '" onclick="pwchakan_wc(' + index + ')" class="easyui-tooltip">' + value1 + '</a>'; return ss } }; function TitleFormatter_jj(value, row, index) { if (value == null) { var ss = ''; return ss } else { var value1 = value; if (value.length > 15) { value1 = value.substr(0, 15) + "..." } var ss = '<a href="javascript:;" title="' + value + '" class="jianjie_all">' + value1 + '</a>'; return ss } }; function bmfzrtubiao(value, row, index) { var bmfzrshenhe = row.bmfzrshenhe; switch (bmfzrshenhe) { case "通过": return "<Image src='/Scripts/easyui/themes/icons/ok.png' Title='通过'/>"; break; case "未审核": return "<Image src='/Scripts/easyui/themes/icons/question.png' Title='未审核'/>"; break; case "未通过": return "<Image src='/Scripts/easyui/themes/icons/no.png' Title='未通过'/>"; break } }; function fgldtubiao(value, row, index) { var fgldshenhe = row.fgldshenhe; switch (fgldshenhe) { case "通过": return "<Image src='/Scripts/easyui/themes/icons/ok.png' Title='通过'/>"; break; case "未审核": return "<Image src='/Scripts/easyui/themes/icons/question.png' Title='未审核'/>"; break; case "未通过": return "<Image src='/Scripts/easyui/themes/icons/no.png' Title='未通过'/>"; break } }; function pwshenhebubiao(value, row, index) { var pingweishenhe = row.pingweishenhe; switch (pingweishenhe) { case "通过": return "<Image src='/Scripts/easyui/themes/icons/ok.png' Title='通过'/>"; break; case "未通过": return "<Image src='/Scripts/easyui/themes/icons/no.png' Title='未通过'/>"; break; case "撤回": return "<Image src='/Scripts/easyui/themes/icons/chehui.png' Title='撤回'/>"; break; case "未审核": return "<Image src='/Scripts/easyui/themes/icons/question.png' Title='未审核'/>"; break } }; function pwtjbubiao(value, row, index) { var pingweitijiao = row.pingweitijiao; switch (pingweitijiao) { case "未提交": return "<Image src='/Scripts/easyui/themes/icons/question.png' Title='未提交'/>"; break; case "已提交": return "<Image src='/Scripts/easyui/themes/icons/tijiao.png' Title='已提交'/>"; break } }; function pwchakan_wc(index) { $('#dg_shpw_wc').datagrid('selectRow', index); var row = $('#dg_shpw_wc').datagrid('getSelected'); if (row) { var name = row.XmName; var id = row.ID; var xmmulu = row.Xiangmumulu; var tabTitle = "查看：" + changdu(name); var url = "/xiangmuguanli/chakan?NoticeXmName=" + name + "&xmmulu=" + xmmulu + "&xmid=" + id; var icon = "icon-shenhe"; window.parent.parent.addTab(tabTitle, url, icon); } }; function xmrizhi(value, row, index) { return "<a href='javascript:void(0)' style='width:95%' class='rizhi' onclick='rizhichakan(" + index + ")' target='mainFrame'>日志</a>" }; function rizhichakan(index) { $('#dg_shpw_wc').datagrid('selectRow', index); var row = $('#dg_shpw_wc').datagrid('getSelected'); if (row) { var xmname = row.XmName; var id = row.ID; var xmmulu = row.Xiangmumulu; var tabTitle = "流程：" + changdu(xmname); var url = "/xiangmuguanli/liucheng?xmname=" + xmname + "&loingid=" + loingid + "&xmid=" + id; var icon = "icon-edit"; window.parent.parent.addTab(tabTitle, url, icon); } }; var names; function pl_tijiao() { var checkedItems = $('#dg_shpw_wc').datagrid('getChecked'); var names = []; if (checkedItems.length == 0) { $.messager.alert("错误提示", "请选择要提交的项目！", "warning") } else { if (checkedItems) { $('#pl_tijiao_dlg').dialog('open').dialog('setTitle', '项目批量提交'); $.each(checkedItems, function (index, item) { names.push(item.ID); }); url = '/xiangmuguanli/pltijiao_pw?loingid=' + loingid + '&username=' + username + '&xmid=' + names } } }; function pl_pwtj_save() { $('#tj_pw_fm').form('submit', { url: url, onSubmit: function () { return $(this).form('validate') }, success: function (result) { result = JSON.parse(result); if (result.success == "2") { $.messager.show({ title: '错误提示', msg: result.errorMsg }) } if (result.success == "1") { $('#pl_tijiao_dlg').dialog('close'); $('#dg_shpw_wsh').datagrid('reload'); $('#dg_shpw_wc').datagrid('reload'); $.messager.show({ title: '提示', msg: result.errorMsg }) } } }) }; function zaishen(value, row, index) { var pwtijiao = row.pingweitijiao; if (pwtijiao == "未提交") { return "<a href='javascript:void(0)' style='width:95%' class='zaishen' onclick='pw_zaishen(" + index + ")' target='mainFrame'>再审</a>" } else { return "<a href='javascript:void(0)' style='width:95%' class='zaishen_jin'>再审</a>" } }; function pw_tijiao(value, row, index) { var pwtijiao = row.pingweitijiao; if (pwtijiao == "未提交") { return "<a href='javascript:void(0)' style='width:95%' class='pwtijiao' onclick='pw_tj(" + index + ")' target='mainFrame'>提交</a>" } else { return "<a href='javascript:void(0)' style='width:95%' class='pwtijiao_jin'>提交</a>" } }; function pw_zaishen(index) { $('#fm_shpw').form('clear'); $('#dg_shpw_wc').datagrid('selectRow', index); var row = $('#dg_shpw_wc').datagrid('getSelected'); if (row) { $('#pwshenhe_dlg').dialog('open').dialog('setTitle', '评委再审'); var name = row.XmName; var id = row.ID; var xmmulu = row.Xiangmumulu; url = '/xiangmuguanli/pw_shenhe?name=' + name + '&loingid=' + loingid + '&xmid=' + id } }; function pwshenhe_save() { $('#fm_shpw').form('submit', { url: url, onSubmit: function () { return $(this).form('validate') }, success: function (result) { result = JSON.parse(result); if (result.success == false) { $.messager.show({ title: '错误提示', msg: result.errorMsg }) } if (result.success == true) { $('#pwshenhe_dlg').dialog('close'); $('#dg_shpw_wc').datagrid('reload'); $.messager.show({ title: '提示', msg: result.errorMsg }) } } }) }; function pw_tj(index) { $('#dg_shpw_wc').datagrid('selectRow', index); var row = $('#dg_shpw_wc').datagrid('getSelected'); if (row) { $('#pw_tijiao_dlg').dialog('open').dialog('setTitle', '提示'); var id = row.ID; url = "/xiangmuguanli/pwtijiao?loingid=" + loingid + "&username=" + username + "&xmid=" + id } }; function tj_pw_save() { $('#tj_pw_fm_dt').form('submit', { url: url, onSubmit: function () { return $(this).form('validate') }, success: function (result) { result = JSON.parse(result); if (result.Succeeded == 1) { $('#pw_tijiao_dlg').dialog('close'); $('#dg_shpw_wc').datagrid('reload'); $.messager.show({ title: '提示', msg: result.errorMsg }) } else { $.messager.show({ title: '提示', msg: result.errorMsg }) } } }) };