﻿$(function () { $("#xingzhi_dg").datagrid({ singleSelect: true, async: false, collapsible: true, method: 'post', url: '/bumenxingzhi/getxingzhi', toolbar: '#toolbar', rownumbers: true, pagination: true, fitcolumns: true, fit: true, autoRowheight: false, }); var p = $('#xingzhi_dg').datagrid('getPager'); $(p).pagination({ pageSize: 10, pageList: [5, 10, 15, 20, 25, 30, 35, 40, 45, 50, 55, 60, 65, 70, 75, 80, 85, 90, 95, 100], beforePageText: '第', afterPageText: '页    共 {pages} 页', displayMsg: '当前显示 {from} - {to} 条记录   共 {total} 条记录', }); $('#xingzhi').textbox().textbox('addClearBtn', 'icon-clear') }); $.extend($.fn.textbox.methods, { addClearBtn: function (jq, iconCls) { return jq.each(function () { var t = $(this); var opts = t.textbox('options'); opts.icons = opts.icons || []; opts.icons.unshift({ iconCls: iconCls, handler: function (e) { $(e.data.target).textbox('clear').textbox('textbox').focus(); $(this).css('visibility', 'hidden') } }); t.textbox(); if (!t.textbox('getText')) { t.textbox('getIcon', 0).css('visibility', 'hidden') } t.textbox('textbox').bind('keyup', function () { var icon = t.textbox('getIcon', 0); if ($(this).val()) { icon.css('visibility', 'visible') } else { icon.css('visibility', 'hidden') } }) }) } }); $.extend($.fn.validatebox.defaults.rules, { checknameissame: { validator: function (value, param) { var name = value.trim(); var result = ""; $.ajax({ type: 'post', async: false, url: '/bumenxingzhi/checkNameIsSame', data: { "name": name }, success: function (data) { result = data } }); return result.indexOf("True") == 0 }, message: '该名称已经被占用' } }); function newxingzhi() { $('#dlg').dialog('open').dialog('setTitle', '新增部门性质'); $('#fm').form('clear'); $('#xingzhi').textbox({ disabled: false }); url = '/bumenxingzhi/savexingzhi' }; function savexingzhi() { $('#fm').form('submit', { url: url, onSubmit: function () { return $(this).form('validate') }, success: function (result) { result = JSON.parse(result); if (result.success) { $('#dlg').dialog('close'); $('#xingzhi_dg').datagrid('reload'); $.messager.show({ title: '提示', msg: result.errorMsg }) } else { $.messager.show({ title: '提示', msg: result.errorMsg }) } } }) }; function destroyxingzhi() { var row = $('#xingzhi_dg').datagrid('getSelected'); if (row) { $.messager.confirm('提示', '您确定要删除这条记录吗？', function (r) { if (r) { $.post('/bumenxingzhi/delxingzhi', { ID: row.ID }, function (result) { if (result.success) { $('#xingzhi_dg').datagrid('reload'); $.messager.show({ title: '提示', msg: result.errorMsg }) } else { $.messager.show({ title: '错误提示', msg: result.errorMsg }) } }, 'json') } }) } else { $.messager.alert("错误提示", "请选择要删除的行！", "warning") } };