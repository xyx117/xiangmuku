﻿$(function () { $("#upload_dg").datagrid({ singleSelect: true, async: false, collapsible: true, method: 'post', url: '/xiangmuguanli/getupload', rownumbers: true, pagination: true, fit: true, autoRowheight: false, nowrap: false, queryParams: { xmglid: xmid } }); var p = $('#upload_dg').datagrid('getPager'); $(p).pagination({ pageSize: 10, pageList: [5, 10, 15, 20, 25, 30, 35, 40, 45, 50, 55, 60, 65, 70, 75, 80, 85, 90, 95, 100], beforePageText: '第', afterPageText: '页    共 {pages} 页', displayMsg: '当前显示 {from} - {to} 条记录   共 {total} 条记录', }) }); function rowformater(value, row, index) { if (row) { var filename = row.filename; var filepath = row.filepath; return "<a href='GetFile?filename=" + filename + "&filepath=" + filepath + "'>下载</a>"; } };