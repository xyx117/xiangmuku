$(function () {

    if (cccx_yt == "1") {
        $('#xmchengxiao_fm').form('load', '/xiangmuguanli/load_cccx?xmid=' + xmid);
    };


    //解决url路径传中文值乱码
    $("#jxcc_dg").datagrid({
        singleSelect: true,
        async: false,
        collapsible: true,
        method: 'post',
        url: '/xiangmuguanli/getxmjx_cc',
        toolbar: '#jxcc_toolbar',
        rownumbers: true,
        pagination: true,
        nowrap: false,
        autoRowheight: false,
        queryParams: {
            xmglid: xmid
        }

    });
    //jxxl_dg
    $("#jxxl_dg").datagrid({
        singleSelect: true,
        async: false,
        collapsible: true,
        method: 'post',
        url: '/xiangmuguanli/getxmjx_xl',
        toolbar: '#jxxl_toolbar',
        rownumbers: true,
        pagination: true,
        autoRowheight: false,
        nowrap: false,
        queryParams: {
            xmglid: xmid
        }
    });


    var p = $('#jxxl_dg').datagrid('getPager');

    $(p).pagination({

        pageSize: 10,//每页显示的记录条数，默认为10

        pageList: [5, 10, 15, 20, 25, 30, 35, 40, 45, 50, 55, 60, 65, 70, 75, 80, 85, 90, 95, 100],//可以设置每页记录条数的列表

        beforePageText: '第',//页数文本框前显示的汉字

        afterPageText: '页    共 {pages} 页',

        displayMsg: '当前显示 {from} - {to} 条记录   共 {total} 条记录',
    });


    var p1 = $('#jxcc_dg').datagrid('getPager');

    $(p1).pagination({

        pageSize: 10,//每页显示的记录条数，默认为10

        pageList: [5, 10, 15, 20, 25, 30, 35, 40, 45, 50, 55, 60, 65, 70, 75, 80, 85, 90, 95, 100],//可以设置每页记录条数的列表

        beforePageText: '第',//页数文本框前显示的汉字

        afterPageText: '页    共 {pages} 页',

        displayMsg: '当前显示 {from} - {to} 条记录   共 {total} 条记录',
    });
});