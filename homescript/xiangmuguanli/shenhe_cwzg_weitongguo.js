﻿$(function () { var searchquery = "所有部门"; $("#dg_shcwzg_btg").datagrid({ singleSelect: true, async: false, collapsible: true, method: 'post', url: '/xiangmuguanli/ywzy_shenhe_xm_btg', toolbar: '#cwzg_toolbar', rownumbers: true, pagination: true, autoRowheight: false, fit: true, view: groupview, groupField: 'suoshuxueyuan', groupFormatter: function (value, rows) { return value + ' - ' + rows.length + ' 项' }, queryParams: { xiangmu: dgtitle, searchquery: searchquery }, onLoadSuccess: function (data) { $('.cwzgzhongcai').linkbutton({ text: '仲裁', plain: true }); $('.cwzgzhongcai').addClass("c6"); $('.rizhi').linkbutton({ text: '日志', plain: true }); $('.rizhi').addClass("c6"); $('.jianjie_all').tooltip({ position: 'left', onShow: function () { $(this).tooltip('tip').css({ width: '60%' }) } }) } }); var p_btg = $('#dg_shcwzg_btg').datagrid('getPager'); $(p_btg).pagination({ pageSize: 10, pageList: [5, 10, 15, 20, 25, 30, 35, 40, 45, 50, 55, 60, 65, 70, 75, 80, 85, 90, 95, 100], beforePageText: '第', afterPageText: '页    共 {pages} 页', displayMsg: '当前显示 {from} - {to} 条记录   共 {total} 条记录', }); $('#suoshuxueyuan').combobox({ select: '所有部门', onChange: function (newValue, oldValue) { $('#dg_shcwzg_btg').datagrid('load', { "xiangmu": dgtitle, "searchquery": newValue }) } }) }); $.extend($.fn.combobox.methods, { selectedIndex: function (jq, index) { if (!index) index = 0; var data = $(jq).combobox('options').data; var vf = $(jq).combobox('options').valueField; $(jq).combobox('setValue', eval('data[index].' + vf)) } }); function changdu(xm_name) { value1 = xm_name; if (value1.length > 15) { value1 = value1.substr(0, 15) + "..."; } return value1 }; function TitleFormatter(value, row, index) { var value1 = value; if (value1 == null) { var ss = '<a href="#" title="' + value + '" class="easyui-tooltip"></a>'; return ss } else { if (value1.length > 15) { value1 = value1.substr(0, 15) + "..." } var ss = '<a href="#" title="' + value + '" onclick="cwzgchakan_btg(' + index + ')" class="easyui-tooltip">' + value1 + '</a>'; return ss } }; function TitleFormatter_jj(value, row, index) { if (value == null) { var ss = ''; return ss } else { var value1 = value; if (value.length > 13) { value1 = value.substr(0, 13) + "..." } var ss = '<a href="javascript:;" title="' + value + '" class="jianjie_all">' + value1 + '</a>'; return ss } }; function bmfzrtubiao(value, row, index) { var bmfzrshenhe = row.bmfzrshenhe; switch (bmfzrshenhe) { case "通过": return "<Image src='/Scripts/easyui/themes/icons/ok.png' Title='通过'/>"; break; case "未审核": return "<Image src='/Scripts/easyui/themes/icons/question.png' Title='未审核'/>"; break; case "未通过": return "<Image src='/Scripts/easyui/themes/icons/no.png' Title='未通过'/>"; break } }; function fgldtubiao(value, row, index) { var fgldshenhe = row.fgldshenhe; switch (fgldshenhe) { case "通过": return "<Image src='/Scripts/easyui/themes/icons/ok.png' Title='通过'/>"; break; case "未审核": return "<Image src='/Scripts/easyui/themes/icons/question.png' Title='未审核'/>"; break; case "未通过": return "<Image src='/Scripts/easyui/themes/icons/no.png' Title='未通过'/>"; break } }; function ywzybubiao(value, row, index) { var ywzyshenhe = row.ywzyshenhe; switch (ywzyshenhe) { case "通过": return "<Image src='/Scripts/easyui/themes/icons/ok.png' Title='通过'/>"; break; case "未审核": return "<Image src='/Scripts/easyui/themes/icons/question.png' Title='未审核'/>"; break; case "未通过": return "<Image src='/Scripts/easyui/themes/icons/no.png' Title='未通过'/>"; break } }; function pwshenhetubiao(value, row, index) { var pingweishenhe = row.pingweishenhe; switch (pingweishenhe) { case "通过": return "<Image src='/Scripts/easyui/themes/icons/ok.png' Title='通过'/>"; break; case "未通过": return "<Image src='/Scripts/easyui/themes/icons/no.png' Title='未通过'/>"; break; case "撤回": return "<Image src='/Scripts/easyui/themes/icons/chehui.png' Title='撤回'/>"; break } }; function cwzgchakan_btg(index) { $('#dg_shcwzg_btg').datagrid('selectRow', index); var row = $('#dg_shcwzg_btg').datagrid('getSelected'); if (row) { var name = row.XmName; var id = row.ID; var xmmulu = row.Xiangmumulu; var tabTitle = "查看：" + changdu(name); var url = "/xiangmuguanli/chakan?NoticeXmName=" + name + "&xmmulu=" + xmmulu + "&xmid=" + id; var icon = "icon-shenhe"; window.parent.parent.addTab(tabTitle, url, icon); } }; function cwzg_xmrizhi_btg(value, row, index) { return "<a href='javascript:void(0)' style='width:80%' class='rizhi' onclick='rizhichakan_btg(" + index + ")' target='mainFrame'>日志</a>" }; function rizhichakan_btg(index) { $('#dg_shcwzg_btg').datagrid('selectRow', index); var row = $('#dg_shcwzg_btg').datagrid('getSelected'); if (row) { var xmname = row.XmName; var id = row.ID; var xmmulu = row.Xiangmumulu; var tabTitle = "流程：" + changdu(xmname); var url = "/xiangmuguanli/liucheng?xmname=" + xmname + "&loingid=" + loingid + "&xmid=" + id; var icon = "icon-edit"; window.parent.parent.addTab(tabTitle, url, icon); } }; function cwzg_zhongcai(value, row, index) { return "<a href='javascript:void(0)' style='width:80%' class='cwzgzhongcai' onclick='zhongcai(" + index + ")' target='mainFrame'>仲裁</a>" }; function zhongcai(index) { $('#dg_shcwzg_btg').datagrid('selectRow', index); var row = $('#dg_shcwzg_btg').datagrid('getSelected'); if (row) { $('#cwzg_zc_dlg').dialog('open').dialog('setTitle', '提示'); var id = row.ID; url = "/xiangmuguanli/cwzg_zc?loingid=" + loingid + "&id=" + id } }; function cwzg_zc_save() { $('#cwzg_zc_fm').form('submit', { url: url, onSubmit: function () { return $(this).form('validate') }, success: function (result) { result = JSON.parse(result); if (result.success == true) { $('#cwzg_zc_dlg').dialog('close'); $('#dg_shcwzg_btg').datagrid('reload'); $.messager.show({ title: '提示', msg: result.errorMsg }) } else { $.messager.show({ title: '提示', msg: result.errorMsg }) } } }) };