﻿$(function () { var searchquery = "所有部门"; $("#dg_shywzy_wsh").datagrid({ singleSelect: false, selectOnCheck: true, checkOnSelect: true, async: false, collapsible: true, method: 'post', url: '/xiangmuguanli/ywzy_shenhe_xm_wsh', toolbar: '#pl_ywzy_toolbar', rownumbers: true, pagination: true, fit: true, autoRowheight: false, view: groupview, groupField: 'suoshuxueyuan', groupFormatter: function (value, rows) { return value + ' - ' + rows.length + ' 项' }, queryParams: { xiangmu: dgtitle, searchquery: searchquery }, onLoadSuccess: function (data) { $('.tianbiao').linkbutton({ text: '填表', plain: true }); $('.tianbiao').addClass("c6"); $('.tianbiao_jin').linkbutton({ text: '填表', disabled: true }); $('.zhongshen').linkbutton({ text: '终审', plain: true }); $('.zhongshen').addClass("c6"); $('.rizhi').linkbutton({ text: '日志', plain: true }); $('.rizhi').addClass("c6"); $('.jianjie_all').tooltip({ position: 'left', onShow: function () { $(this).tooltip('tip').css({ width: '60%' }) } }) } }); var p = $('#dg_shywzy_wsh').datagrid('getPager'); $(p).pagination({ pageSize: 10, pageList: [5, 10, 15, 20, 25, 30, 35, 40, 45, 50, 55, 60, 65, 70, 75, 80, 85, 90, 95, 100], beforePageText: '第', afterPageText: '页    共 {pages} 页', displayMsg: '当前显示 {from} - {to} 条记录   共 {total} 条记录', }); $('#suoshuxueyuan').combobox({ select: '所有部门', onChange: function (newValue, oldValue) { $('#dg_shywzy_wsh').datagrid('load', { "xiangmu": dgtitle, "searchquery": newValue }) } }) }); function changdu(xm_name) { value1 = xm_name; if (value1.length > 15) { value1 = value1.substr(0, 15) + "..."; } return value1 }; function TitleFormatter(value, row, index) { var value1 = value; if (value1 == null) { var ss = '<a href="#" title="' + value + '" class="easyui-tooltip"></a>'; return ss } else { if (value1.length > 15) { value1 = value1.substr(0, 15) + "..." } var ss = '<a href="#" title="' + value + '" onclick="ywzychakan_wsh(' + index + ')" class="easyui-tooltip">' + value1 + '</a>'; return ss } }; function TitleFormatter_jj(value, row, index) { if (value == null) { var ss = ''; return ss } else { var value1 = value; if (value.length > 11) { value1 = value.substr(0, 11) + "..." } var ss = '<a href="javascript:;" title="' + value + '" class="jianjie_all">' + value1 + '</a>'; return ss } }; function bmfzrtubiao(value, row, index) { var bmfzrshenhe = row.bmfzrshenhe; switch (bmfzrshenhe) { case "通过": return "<Image src='/Scripts/easyui/themes/icons/ok.png' Title='通过'/>"; break; case "未审核": return "<Image src='/Scripts/easyui/themes/icons/question.png' Title='未审核'/>"; break; case "未通过": return "<Image src='/Scripts/easyui/themes/icons/no.png' Title='未通过'/>"; break } }; function fgldtubiao(value, row, index) { var fgldshenhe = row.fgldshenhe; switch (fgldshenhe) { case "通过": return "<Image src='/Scripts/easyui/themes/icons/ok.png' Title='通过'/>"; break; case "未审核": return "<Image src='/Scripts/easyui/themes/icons/question.png' Title='未审核'/>"; break; case "未通过": return "<Image src='/Scripts/easyui/themes/icons/no.png' Title='未通过'/>"; break } }; function ywzybubiao(value, row, index) { var ywzyshenhe = row.ywzyshenhe; switch (ywzyshenhe) { case "通过": return "<Image src='/Scripts/easyui/themes/icons/ok.png' Title='通过'/>"; break; case "未审核": return "<Image src='/Scripts/easyui/themes/icons/question.png' Title='未审核'/>"; break; case "未通过": return "<Image src='/Scripts/easyui/themes/icons/no.png' Title='未通过'/>"; break } }; function pwshenhetubiao(value, row, index) { var pingweishenhe = row.pingweishenhe; switch (pingweishenhe) { case "通过": return "<Image src='/Scripts/easyui/themes/icons/ok.png' Title='通过'/>"; break; case "未通过": return "<Image src='/Scripts/easyui/themes/icons/no.png' Title='未通过'/>"; break; case "撤回": return "<Image src='/Scripts/easyui/themes/icons/chehui.png' Title='撤回'/>"; break } }; function ywzychakan_wsh(index) { $('#dg_shywzy_wsh').datagrid('selectRow', index); var row = $('#dg_shywzy_wsh').datagrid('getSelected'); if (row) { var name = row.XmName; var id = row.ID; var xmmulu = row.Xiangmumulu; var tabTitle = "查看：" + changdu(name); var url = "/xiangmuguanli/chakan?NoticeXmName=" + name + "&xmmulu=" + xmmulu + "&xmid=" + id; var icon = "icon-shenhe"; window.parent.parent.addTab(tabTitle, url, icon); } }; function tianbiao_ywzy_wsh(value, row, index) { var ywzyshenhe = row.ywzyshenhe; if (ywzyshenhe == "未审核") { return "<a href='javascript:void(0)' style='width:95%' class='tianbiao' onclick='wsxx_ywzy_wsh(" + index + ")' target='mainFrame'>填表</a>" } else { return "<a href='javascript:void(0)' style='width:95%' class='tianbiao_jin'></a>" } }; function wsxx_ywzy_wsh(index) { $('#dg_shywzy_wsh').datagrid('selectRow', index); var row = $('#dg_shywzy_wsh').datagrid('getSelected'); if (row) { var name = row.XmName; var id = row.ID; var xmmulu = row.Xiangmumulu; var tabTitle = "填表：" + changdu(name); var url = "/xiangmuguanli/wanshanxinxi?NoticeXmName=" + name + "&xmmulu=" + xmmulu + "&xmid=" + id; var icon = "icon-edit"; window.parent.parent.addTab(tabTitle, url, icon); } }; function ygxmrizhi_wsh(value, row, index) { return "<a href='javascript:void(0)' style='width:95%' class='rizhi' onclick='rizhichakan_wsh(" + index + ")' target='mainFrame'>日志</a>" }; function rizhichakan_wsh(index) { $('#dg_shywzy_wsh').datagrid('selectRow', index); var row = $('#dg_shywzy_wsh').datagrid('getSelected'); if (row) { var xmname = row.XmName; var id = row.ID; var xmmulu = row.Xiangmumulu; var tabTitle = "流程：" + changdu(xmname); var url = "/xiangmuguanli/liucheng?xmname=" + xmname + "&loingid=" + loingid + "&xmid=" + id; var icon = "icon-edit"; window.parent.parent.addTab(tabTitle, url, icon); } }; function ywzy_shenhe_wsh(value, row, index) { var ywzyshenhe = row.ywzyshenhe; var fgldshenhe = row.fgldshenhe; var pingweitijiao = row.pingweitijiao; if (ywzyshenhe == "未审核" && fgldshenhe == "通过" && pingweitijiao == "已提交") { return "<a href='javascript:void(0)' style='width:95%' class='zhongshen' onclick='ywzy_caozuo_wsh(" + index + ")' target='mainFrame'>终审</a>" } }; function ywzy_caozuo_wsh(index) { $('#fm_shywzy').form('clear'); $('#dg_shywzy_wsh').datagrid('selectRow', index); var row = $('#dg_shywzy_wsh').datagrid('getSelected'); if (row) { $('#shenhe_dlg').dialog('open').dialog('setTitle', '业务部门终审'); var name = row.XmName; var id = row.ID; var xmmulu = row.Xiangmumulu; url = '/xiangmuguanli/ywzy_zhongshen?name=' + name + '&loingid=' + loingid + '&xmid=' + id } }; function ywzyshenhe_save() { $('#fm_shywzy').form('submit', { url: url, onSubmit: function () { return $(this).form('validate') }, success: function (result) { result = JSON.parse(result); if (result.success == false) { $.messager.show({ title: '错误提示', msg: result.errorMsg }) } if (result.success == true) { $('#shenhe_dlg').dialog('close'); $('#dg_shywzy_wsh').datagrid('reload'); $.messager.show({ title: '提示', msg: result.errorMsg }) } } }) }; function pl_shenhe() { var checkedItems = $('#dg_shywzy_wsh').datagrid('getChecked'); var names = []; if (checkedItems.length == 0) { $.messager.alert("错误提示", "请选择要审核项目！", "warning") } else { if (checkedItems) { $('#pl_shenhe_dlg').dialog('open').dialog('setTitle', '项目批量审核'); $('#shenhe_fm').form('clear'); $.each(checkedItems, function (index, item) { names.push(item.ID); }); url = '/xiangmuguanli/plshenhe_ywzy?loingid=' + loingid + '&username=' + username + '&xmid=' + names } } }; function pl_ywzy_save() { $('#shenhe_fm').form('submit', { url: url, onSubmit: function () { return $(this).form('validate') }, success: function (result) { result = JSON.parse(result); if (result.success == "2") { $.messager.show({ title: '错误提示', msg: result.errorMsg }) } if (result.success == "1") { $('#pl_shenhe_dlg').dialog('close'); $('#dg_shywzy_wsh').datagrid('reload'); $.messager.show({ title: '提示', msg: result.errorMsg }) } } }) };