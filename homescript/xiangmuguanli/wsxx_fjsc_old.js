var applicationPath = window.applicationPath === "" ? "" : window.applicationPath || "../../", uploader;
$(function () {

    $("#upload_dg").datagrid({
        singleSelect: true,
        async: false,
        collapsible: true,
        method: 'post',
        url: '/xiangmuguanli/getupload',
        toolbar: '#upload_tba',
        rownumbers: true,
        //fit: true,
        pagination: true,
        //fitcolumns: true,
        nowrap: false,
        queryParams: {
            xmglid: xmid
        }
    });


    var p = $('#upload_dg').datagrid('getPager'),

        $list = $('#fileList');



    uploader = WebUploader.create({
        // 选完文件后，是否自动上传。
        auto: false,

        fileNumLimit: 5, //限制选择5个文件
        fileSizeLimit: 100 * 1024 * 1024,    // 100 M
        fileSingleSizeLimit: 20 * 1024 * 1024,   // 20 M

        disableGlobalDnd: true,
        // swf文件路径
        swf: applicationPath + '/Scripts/Uploader.swf',
        //swf: '~/Script/webuploader/Uploader.swf',
        // 文件接收服务端。
        server: '/xiangmuguanli/UpLoadProcessfile',


        // 选择文件的按钮。可选。
        // 内部根据当前运行是创建，可能是input元素，也可能是flash.
        pick: '#filePicker',

        //chunked: true,  //分片处理
        //chunkSize: 100 * 1024 * 1024, //每片100M

        formData: {
            xmid: xmid,
            xmname: xmname,
            xmmulu: dgtitle
        },
        //只允许选择图片
        accept: {
            title: '文件上传',
            extensions: 'rar,zip,doc,docx,xls,xlsx,pdf',
            mimeTypes: '.rar,.zip,.doc,.docx,.xls,.xlsx,.pdf'
        }
    });

    // 当有文件添加进来的时候
    uploader.on('fileQueued', function (file) {
        var $li = $('<div id="' + file.id + '" class="item">' + '<h4 class="info">' + file.name + '</h4>' + '<p class="state">等待上传...</p>' + '<div class="cp_img_jian"></div></div>');

        // $list为容器jQuery实例
        $list.append($li);
    });

    // 文件上传过程中创建进度条实时显示。
    uploader.on('uploadProgress', function (file, percentage) {
        var $li = $('#' + file.id),
            $percent = $li.find('.progress span');

        // 避免重复创建
        if (!$percent.length) {
            $percent = $('<p class="progress"><span></span></p>')
                    .appendTo($li)
                    .find('span');
        }

        $percent.css('width', percentage * 100 + '%');
    });

    // 文件上传成功，给item添加成功class, 用样式标记上传成功。
    uploader.on('uploadSuccess', function (file, result) {

        //$('#' + file.id).addClass('upload-state-done');
        if (result.success) {
            $("#" + file.id).find("p.state").text("已上传");

        } else {

            $("#" + file.id).find("p.state").text("上传失败，原因：" + result.message);
            //$("#" + file.id).find("p.state").attr("color", "red");
            $("#" + file.id).find("p.state").attr("style", "color:red");
            return false;
        }


    });


    // 文件上传失败，显示上传出错。
    uploader.on('uploadError', function (file, result) {

        $("#" + file.id).find("p.state").text("上传失败，原因：IIS设置有误，如限制文件上传大小！");
        //$("#" + file.id).find("p.state").attr("color", "red");

        $("#" + file.id).find("p.state").attr("style", "color:red");
    });

    // 完成上传完了，成功或者失败，先删除进度条。
    uploader.on('uploadComplete', function (file) {
        $('#' + file.id).find('.progress').remove();


    });

    //所有文件上传完毕
    uploader.on("uploadFinished", function () {
        //提交表单

        // reload the user data
        $('#upload_dg').datagrid('reload');

    });

    uploader.on("error", function (type, hanlder) {

        if (type == "Q_TYPE_DENIED") {

            $.messager.show({	// show error message
                title: '错误提示',
                msg: "文件类型超出了允许范围"
            })

        } else if (type == "Q_EXCEED_SIZE_LIMIT") {
            $.messager.show({	// show error message
                title: '错误提示',
                msg: "文件大小不能超过20M"
            })
        } else {
            $.messager.show({
                title: '错误提示',
                msg: "上传出错！错误代码：" + type
            })
        }
    });




    //开始上传
    $("#ctlBtn").click(function () {
        uploader.upload();

    });



    // 显示删除按钮

    $(".item").live("mouseover", function () {
        $(this).children(".cp_img_jian").css('display', 'block');

    });
    //隐藏删除按钮
    $(".item").live("mouseout", function () {
        $(this).children(".cp_img_jian").css('display', 'none');

    });
    //执行删除方法
    $list.on("click", ".cp_img_jian", function () {
        var Id = $(this).parent().attr("id");

        uploader.removeFile(uploader.getFile(Id, true));
        $(this).parent().remove();


        //var fileArray = uploader.getFiles();
        //for (var i = 0 ; i < fileArray.length; i++) {
        //    //取消文件上传
        //    uploader.cancelFile(fileArray[i]);
        //    //从队列中移除掉
        //    uploader.removeFile(fileArray[i], true);
        //}
        ////发生错误重置webupload,初始化变量
        //uploader.reset();
        //fileSize = 0;
        //fileName = [];
        //fileSizeOneByOne = [];
    });






    $(p).pagination({

        pageSize: 10,//每页显示的记录条数，默认为10

        pageList: [5, 10, 15, 20, 25, 30, 35, 40, 45, 50, 55, 60, 65, 70, 75, 80, 85, 90, 95, 100],//可以设置每页记录条数的列表

        beforePageText: '第',//页数文本框前显示的汉字

        afterPageText: '页    共 {pages} 页',

        displayMsg: '当前显示 {from} - {to} 条记录   共 {total} 条记录',
    });

});



function emptyqueen() {


    //$list.empty() ;


    var fileArray = uploader.getFiles();

    for (var i = 0 ; i < fileArray.length; i++) {
        //取消文件上传
        uploader.cancelFile(fileArray[i]);
        //从队列中移除掉

        uploader.removeFile(fileArray[i], true);
    };
    $('#fileList').empty();


}

function feupld_del() {
    var row = $('#upload_dg').datagrid('getSelected');
    if (row) {
        $.messager.confirm('提示', '您确定要删除这条记录吗？', function (r) {
            if (r) {
                $.post('/xiangmuguanli/feupld_del', { id: row.fileuploadID, path: row.filepath, name: row.filename }, function (result) {
                    //这里把主键更改为项目名称后需要变动
                    //$.post('/xiangmuguanli/delXiangmu', { xmname: row.XmName }, function (result) {
                    if (result.success) {
                        $('#upload_dg').datagrid('reload');	// reload the user data
                        $.messager.show({	// show error message
                            title: '提示',
                            msg: result.errorMsg
                        });
                    } else {
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

//附件下载
function rowformater(value, row, index) {
    if (row) {

        var filename = row.filename;
        var filepath = row.filepath;

        return "<a href='GetFile?filename=" + filename + "&filepath=" + filepath + "'>下载</a>";    //直接指向control，不需要经过filexiazai()
    }

};