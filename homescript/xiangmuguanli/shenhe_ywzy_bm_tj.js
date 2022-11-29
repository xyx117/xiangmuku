﻿$(function () { var searchquery = "所有部门"; $("#dg_shywzy_tg").datagrid({ singleSelect: true, async: false, selectOnCheck: true, checkOnSelect: true, collapsible: true, method: 'post', url: '/xiangmuguanli/ywzy_shenhe_bmtj', toolbar: '#tb_ywzy_toolbar', rownumbers: true, pagination: true, fit: true, autoRowheight: false, view: groupview, groupField: 'suoshuxueyuan', groupFormatter: function (value, rows) { return value + ' - ' + rows.length + ' 项' }, queryParams: { xiangmu: dgtitle, searchquery: searchquery }, onLoadSuccess: function (data) { $('.chehui').linkbutton({ text: '撤回', plain: true }); $('.chehui').addClass("c6"); $('.chehui_jin').linkbutton({ text: '撤回', disabled: true }); $('.rizhi').linkbutton({ text: '日志', plain: true }); $('.rizhi').addClass("c6"); $('.daochu').linkbutton({ text: '导出excel', plain: true }); $('.daochu').addClass("c6"); $('.jianjie_all').tooltip({ position: 'left', onShow: function () { $(this).tooltip('tip').css({ width: '60%' }) } }) } }); var p = $('#dg_shywzy_tg').datagrid('getPager'); $(p).pagination({ pageSize: 10, pageList: [5, 10, 15, 20, 25, 30, 35, 40, 45, 50, 55, 60, 65, 70, 75, 80, 85, 90, 95, 100], beforePageText: '第', afterPageText: '页    共 {pages} 页', displayMsg: '当前显示 {from} - {to} 条记录   共 {total} 条记录', }); $('#suoshuxueyuan').combobox({ select: '所有部门', onChange: function (newValue, oldValue) { $('#dg_shywzy_tg').datagrid('load', { "xiangmu": dgtitle, "searchquery": newValue }) } }) }); function changdu(xm_name) { value1 = xm_name; if (value1.length > 15) { value1 = value1.substr(0, 10) + "..."; } return value1 }; function TitleFormatter(value, row, index) { var value1 = value; if (value1 == null) { var ss = '<a href="#" title="' + value + '" class="easyui-tooltip"></a>'; return ss } else { if (value1.length > 15) { value1 = value1.substr(0, 15) + "..." } var ss = '<a href="#" title="' + value + '" onclick="ywzychakan_tg(' + index + ')" class="easyui-tooltip">' + value1 + '</a>'; return ss } }; function TitleFormatter_jj(value, row, index) { if (value == null) { var ss = ''; return ss } else { var value1 = value; if (value.length > 13) { value1 = value.substr(0, 13) + "..." } var ss = '<a href="javascript:;" title="' + value + '" class="jianjie_all">' + value1 + '</a>'; return ss } }; function bmfzrtubiao(value, row, index) { var bmfzrshenhe = row.bmfzrshenhe; switch (bmfzrshenhe) { case "通过": return "<Image src='/Scripts/easyui/themes/icons/ok.png' Title='通过'/>"; break; case "未审核": return "<Image src='/Scripts/easyui/themes/icons/question.png' Title='未审核'/>"; break; case "未通过": return "<Image src='/Scripts/easyui/themes/icons/no.png' Title='未通过'/>"; break } }; function fgldtubiao(value, row, index) { var fgldshenhe = row.fgldshenhe; switch (fgldshenhe) { case "通过": return "<Image src='/Scripts/easyui/themes/icons/ok.png' Title='通过'/>"; break; case "未审核": return "<Image src='/Scripts/easyui/themes/icons/question.png' Title='未审核'/>"; break; case "未通过": return "<Image src='/Scripts/easyui/themes/icons/no.png' Title='未通过'/>"; break } }; function ywzybubiao(value, row, index) { var ywzyshenhe = row.ywzyshenhe; switch (ywzyshenhe) { case "通过": return "<Image src='/Scripts/easyui/themes/icons/ok.png' Title='通过'/>"; break; case "未审核": return "<Image src='/Scripts/easyui/themes/icons/question.png' Title='未审核'/>"; break; case "未通过": return "<Image src='/Scripts/easyui/themes/icons/no.png' Title='未通过'/>"; break } }; function pwshenhetubiao(value, row, index) { var pingweishenhe = row.pingweishenhe; switch (pingweishenhe) { case "通过": return "<Image src='/Scripts/easyui/themes/icons/ok.png' Title='通过'/>"; break; case "未审核": return "<Image src='/Scripts/easyui/themes/icons/question.png' Title='通过'/>"; break; case "未通过": return "<Image src='/Scripts/easyui/themes/icons/no.png' Title='未通过'/>"; break; case "撤回": return "<Image src='/Scripts/easyui/themes/icons/chehui.png' Title='撤回'/>"; break } }; function ywzychakan_tg(index) { $('#dg_shywzy_tg').datagrid('selectRow', index); var row = $('#dg_shywzy_tg').datagrid('getSelected'); if (row) { var name = row.XmName; var id = row.ID; var xmmulu = row.Xiangmumulu; var tabTitle = "查看：" + changdu(name); var url = "/xiangmuguanli/chakan?NoticeXmName=" + name + "&xmmulu=" + xmmulu + "&xmid=" + id; var icon = "icon-shenhe"; window.parent.parent.addTab(tabTitle, url, icon); } }; function xm_bm_tj_chehui(value, row, index) { var bmfzrtj = row.bmfzrtijiao; var fgldshenhe = row.fgldshenhe; if (bmfzrtj == "已提交" && fgldshenhe == "未审核") { return "<a href='javascript:void(0)' style='width:95%' class='chehui' onclick='ywzy_chehui(" + index + ")' target='mainFrame'>撤回</a>" } else { return "<a href='javascript:void(0)' style='width:95%' class='chehui_jin' >撤回</a>" } }; function ywzy_chehui(index) { $('#dg_shywzy_tg').datagrid('selectRow', index); var row = $('#dg_shywzy_tg').datagrid('getSelected'); if (row) { $('#ywzy_chehui').dialog('open').dialog('setTitle', '提示'); var id = row.ID; url = "/xiangmuguanli/shenhe_ywzy_bmtj_chehui?xmid=" + id } }; function ywzy_chehui_save() { $('#ywzy_chehui_fm').form('submit', { url: url, onSubmit: function () { return $(this).form('validate') }, success: function (result) { result = JSON.parse(result); if (result.success) { $('#ywzy_chehui').dialog('close'); $('#dg_shywzy_tg').datagrid('reload'); $.messager.show({ title: '提示', msg: result.Msg }) } else { $.messager.show({ title: '提示', msg: result.Msg }) } } }) }; function ygxmrizhi_tg(value, row, index) { return "<a href='javascript:void(0)' style='width:95%' class='rizhi' onclick='rizhichakan_tg(" + index + ")' target='mainFrame'>日志</a>" }; function rizhichakan_tg(index) { $('#dg_shywzy_tg').datagrid('selectRow', index); var row = $('#dg_shywzy_tg').datagrid('getSelected'); if (row) { var xmname = row.XmName; var id = row.ID; var xmmulu = row.Xiangmumulu; var tabTitle = "流程：" + changdu(xmname); var url = "/xiangmuguanli/liucheng?xmname=" + xmname + "&loingid=" + loingid + "&xmid=" + id; var icon = "icon-edit"; window.parent.parent.addTab(tabTitle, url, icon); } }; function daochu(value, row, index) { return "<a href='/xiangmuguanli/excel_out?id=" + row.ID + "' style='width:95%' class='daochu'>导出execl</a>" }; function nianduhuizong() { window.location.href = "ywzy_exl_huizong_bmtj?mulu=" + dgtitle };