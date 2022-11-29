$(function () {

    $("#upload_dg").datagrid({
        singleSelect: true,
        async: false,
        collapsible: true,
        method: 'post',
        url: '/xiangmuguanli/getupload',
        //toolbar: '#upload_tba',
        rownumbers: true,
        pagination: true,
        fit: true,
        autoRowheight: false,
        nowrap: false,
        queryParams: {
            xmglid: xmid
        }
    });


    var p = $('#upload_dg').datagrid('getPager');

    $(p).pagination({

        pageSize: 10,//每页显示的记录条数，默认为10

        pageList: [5, 10, 15, 20, 25, 30, 35, 40, 45, 50, 55, 60, 65, 70, 75, 80, 85, 90, 95, 100],//可以设置每页记录条数的列表

        beforePageText: '第',//页数文本框前显示的汉字

        afterPageText: '页    共 {pages} 页',

        displayMsg: '当前显示 {from} - {to} 条记录   共 {total} 条记录',
    });

});


//附件下载
function rowformater(value, row, index) {
    if (row) {

        var filename = row.filename;
        //var savename = row.savename;
        var filepath = row.filepath;
        //var tabTitle = name + "完善信息";
        //var url ="/xiangmuguanli/GetFile?filename="+filename+"";

        //var icon = "icon-edit";
        //window.parent.addTab(tabTitle, url, icon);

        return "<a href='GetFile?filename=" + filename + "&filepath=" + filepath + "'>下载</a>";    //直接指向control，不需要经过filexiazai()
    }

};