﻿$(function () { $("#cwkz_dg").datagrid({ singleSelect: false, selectOnCheck: true, checkOnSelect: true, async: false, collapsible: true, method: 'post', url: '/Account/ywzygetuser', toolbar: '#cwkz_toolbar', rownumbers: true, pagination: true, fitcolumns: true, nowrap: true, fit: true, autoRowHeight: false, queryParams: { searchquery: '' }, }); var p = $('#cwkz_dg').datagrid('getPager'); $(p).pagination({ pageSize: 10, pageList: [5, 10, 15, 20, 25, 30, 35, 40, 45, 50], beforePageText: '第', afterPageText: '页    共 {pages} 页', displayMsg: '当前显示 {from} - {to} 条记录   共 {total} 条记录', }); $('#name').textbox().textbox('addClearBtn', 'icon-clear'); $('#zhenshiname').textbox().textbox('addClearBtn', 'icon-clear'); $('#userpassword').textbox().textbox('addClearBtn', 'icon-clear'); $('#confirmPassword').textbox().textbox('addClearBtn', 'icon-clear'); $('#phonenumber').textbox().textbox('addClearBtn', 'icon-clear'); $('#role').combobox({ onSelect: function () { var juese = $('#role').combobox('getValue'); switch (juese) { case "分管领导": { $('#suoshuxueyuan').combobox({ readonly: false }); $('#suoshuxueyuan').combobox({ multiple: true }); $('#suoshuxueyuan').combobox("clear"); break }; case "部门负责人": { $('#suoshuxueyuan').combobox({ readonly: false }); $('#suoshuxueyuan').combobox({ multiple: false }); $('#suoshuxueyuan').combobox("clear"); break }; case "领导": { $('#suoshuxueyuan').combobox({ readonly: false }); $('#suoshuxueyuan').combobox({ multiple: true }); $('#suoshuxueyuan').combobox("clear"); break }; default: { $('#suoshuxueyuan').combobox({ multiple: false }); $('#suoshuxueyuan').combobox("clear"); $('#suoshuxueyuan').combobox({ readonly: true }); $('#suoshuxueyuan').combobox('select', 'all') } } } }) }); $.extend($.fn.combobox.methods, { selectedIndex: function (jq, index) { if (!index) index = 0; var data = $(jq).combobox('options').data; var vf = $(jq).combobox('options').valueField; $(jq).combobox('setValue', eval('data[index].' + vf)) } }); $.extend($.fn.textbox.methods, { addClearBtn: function (jq, iconCls) { return jq.each(function () { var t = $(this); var opts = t.textbox('options'); opts.icons = opts.icons || []; opts.icons.unshift({ iconCls: iconCls, handler: function (e) { $(e.data.target).textbox('clear').textbox('textbox').focus(); $(this).css('visibility', 'hidden') } }); t.textbox(); if (!t.textbox('getText')) { t.textbox('getIcon', 0).css('visibility', 'hidden') } t.textbox('textbox').bind('keyup', function () { var icon = t.textbox('getIcon', 0); if ($(this).val()) { icon.css('visibility', 'visible') } else { icon.css('visibility', 'hidden') } }) }) } }); $.extend($.fn.validatebox.defaults.rules, { checknameissame: { validator: function (value, param) { var name = value.trim(); var result = ""; $.ajax({ type: 'post', async: false, url: '/Account/checkNameIsSame', data: { "name": name }, success: function (data) { result = data } }); return result.indexOf("True") == 0 }, message: '该名字已经被占用' }, equalTo: { validator: function (value, param) { return $(param[0]).val() == value }, message: '确认密码与密码不相同' }, minLength: { validator: function (value, param) { return value.length >= param[0] }, message: '密码至少为6位字符或数字的组合！' }, phoneRex: { validator: function (value) { var rex = /^1[3-8]+\d{9}$/; var rex2 = /^((0\d{2,3})-)(\d{7,8})(-(\d{3,}))?$/; if (rex.test(value) || rex2.test(value)) { return true } else { return false } }, message: '请输入正确电话或手机格式' } }); function doSearch(value) { $('#cwkz_dg').datagrid('load', { "searchquery": value }) }; function newUser() { $('#cwkz_dlg').dialog('open').dialog('setTitle', '新增用户'); $('#name').textbox('enable', 'true'); $('#juese').show(); $('#bumen').show(); $('#pw').show(); $('#pw_cf').show(); $('#imf').show(); $('#cwkz_fm').form('clear'); url = '/Account/ywzysaveuser' }; function editUser() { var row = $('#cwkz_dg').datagrid('getSelected'); if (row) { var id = row.Id; $('#juese').hide(); if (row.role != "分管领导") { $('#bumen').hide() } else { $('#bumen').show(); $('#suoshuxueyuan').combobox({ readonly: false }); $('#suoshuxueyuan').combobox({ multiple: true }) } $('#pw').hide(); $('#pw_cf').hide(); $('#imf').hide(); $('#cwkz_dlg').dialog('open').dialog('setTitle', '编辑用户'); $('#cwkz_fm').form('load', row); $('#name').textbox('disable', true); url = '/Account/edit_user?id=' + id + '' } else { $.messager.alert("错误提示", "请选择要编辑的行！", "warning") } }; function init_pw() { var rows = $('#cwkz_dg').datagrid('getChecked'); var users = []; if (rows) { $('#restpaw_dlg').dialog('open').dialog('setTitle', '重置密码'); $.each(rows, function (index, item) { users.push(item.Id) }); url = '/Account/Ywzy_init_pw?users=' + users } else { $.messager.alert("错误提示", "请选择要重置密码的用户！", "warning") } }; function init_pw_conf() { $('#restpaw_fm').form('submit', { url: url, onSubmit: function () { return $(this).form('validate') }, success: function (result) { result = JSON.parse(result); $('#restpaw_dlg').dialog('close'); $('#cwkz_dg').datagrid('reload'); if (result.success == true) { $.messager.show({ title: '提示', msg: result.errorMsg }) } else { $.messager.show({ title: '错误提示', msg: result.errorMsg }) } } }) }; function saveUser() { $('#cwkz_fm').form('submit', { url: url, onSubmit: function () { return $(this).form('validate') }, success: function (result) { result = JSON.parse(result); $('#cwkz_dlg').dialog('close'); $('#cwkz_dg').datagrid('reload'); if (result.success == false) { $.messager.show({ title: '错误提示', msg: result.errorMsg }) } else { $.messager.show({ title: '提示', msg: result.errorMsg }) } } }) }; function destroyUser() { var row = $('#cwkz_dg').datagrid('getSelected'); if (row) { if (row.UserName == "admin") { $.messager.alert("错误提示", "admin用户不能删除！", "warning"); return } if (row.role == "部门负责人") { $.messager.confirm('提示', '您确定要删除这条记录吗？删除后会清除该部门下所有人员！', function (r) { if (r) { $.post('/Account/ywzydelBumen', { id: row.Id }, function (result) { if (result.Succeeded) { $('#cwkz_dg').datagrid('reload'); $.messager.show({ title: '提示', msg: result.errorMsg }) } else { $.messager.show({ title: '错误提示', msg: result.errorMsg }) } }, 'json') } }) } else { $.messager.confirm('提示', '您确定要删除这条记录吗？', function (r) { if (r) { $.post('/Account/ywzydelqitq', { id: row.Id }, function (result) { if (result.Succeeded) { $('#cwkz_dg').datagrid('reload'); $.messager.show({ title: '提示', msg: result.errorMsg }) } else { $.messager.show({ title: '错误提示', msg: result.errorMsg }) } }, 'json') } }) } } else { $.messager.alert("错误提示", "请选择要删除的行！", "warning") } }; function yuangongliebiao(value, row, index) { if (row.role == "部门负责人") { if (value > 0) { return ss = '<a id="yg' + index + '" href="javascript:;" onmouseover="bmfzr_yuangong(' + row.myuserID + ',' + index + ')">' + value + '</a>' } else { return 0 } } else { return '--' } }; function bmfzr_yuangong(myuserID, index) { $.post('/Account/bumenyuangong', { bmfzr_myuser_id: myuserID }, function (result) { if (result.success) { var ygh = "#yg" + index; $(ygh).tooltip({ content: result.Msg }) } }, 'json') }; function TitleFormatter(value, row, index) { var value1 = value; if (value1 == null) { var ss = '<a href="#" title="' + value + '" class="easyui-tooltip"></a>'; return ss } else { if (value1.length > 10) { value1 = value1.substr(0, 10) + "..." } var ss = '<a href="#" title="' + value + '" class="easyui-tooltip">' + value1 + '</a>'; return ss } };