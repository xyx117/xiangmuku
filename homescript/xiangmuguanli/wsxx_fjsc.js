﻿var applicationPath=window.applicationPath===""?"":window.applicationPath||"../../",uploader;$(function(){$("#upload_dg").datagrid({singleSelect:true,async:false,collapsible:true,method:'post',url:'/xiangmuguanli/getupload',toolbar:'#upload_tba',rownumbers:true,pagination:true,nowrap:false,queryParams:{xmglid:xmid}});var p=$('#upload_dg').datagrid('getPager'),$list=$('#fileList');uploader=WebUploader.create({auto:false,fileNumLimit:5,fileSizeLimit:100*1024*1024,fileSingleSizeLimit:20*1024*1024,disableGlobalDnd:true,swf:applicationPath+'/Scripts/Uploader.swf',server:'/xiangmuguanli/UpLoadProcessfile',pick:'#filePicker',formData:{xmid:xmid,xmname:xmname,xmmulu:dgtitle},accept:{title:'文件上传',extensions:'rar,zip,doc,docx,xls,xlsx,pdf',mimeTypes:'.rar,.zip,.doc,.docx,.xls,.xlsx,.pdf'}});uploader.on('fileQueued',function(file){var $li=$('<div id="'+file.id+'" class="item">'+'<h4 class="info">'+file.name+'</h4>'+'<p class="state">等待上传...</p>'+'<div class="cp_img_jian"></div></div>');$list.append($li)});uploader.on('uploadProgress',function(file,percentage){var $li=$('#'+file.id),$percent=$li.find('.progress span');if(!$percent.length){$percent=$('<p class="progress"><span></span></p>').appendTo($li).find('span')}$percent.css('width',percentage*100+'%')});uploader.on('uploadSuccess',function(file,result){if(result.success){$("#"+file.id).find("p.state").text("已上传")}else{$("#"+file.id).find("p.state").text("上传失败，原因："+result.message);$("#"+file.id).find("p.state").attr("style","color:red");return false}});uploader.on('uploadError',function(file,result){$("#"+file.id).find("p.state").text("上传失败，原因：IIS设置有误，如限制文件上传大小！");$("#"+file.id).find("p.state").attr("style","color:red")});uploader.on('uploadComplete',function(file){$('#'+file.id).find('.progress').remove()});uploader.on("uploadFinished",function(){$('#upload_dg').datagrid('reload')});uploader.on("error",function(type,hanlder){if(type=="Q_TYPE_DENIED"){$.messager.show({title:'错误提示',msg:"文件类型超出了允许范围"})}else if(type=="Q_EXCEED_SIZE_LIMIT"){$.messager.show({title:'错误提示',msg:"文件大小不能超过20M"})}else{$.messager.show({title:'错误提示',msg:"上传出错！错误代码："+type})}});$("#ctlBtn").click(function(){uploader.upload()});$(".item").live("mouseover",function(){$(this).children(".cp_img_jian").css('display','block')});$(".item").live("mouseout",function(){$(this).children(".cp_img_jian").css('display','none')});$list.on("click",".cp_img_jian",function(){var Id=$(this).parent().attr("id");uploader.removeFile(uploader.getFile(Id,true));$(this).parent().remove();});$(p).pagination({pageSize:10,pageList:[5,10,15,20,25,30,35,40,45,50,55,60,65,70,75,80,85,90,95,100],beforePageText:'第',afterPageText:'页    共 {pages} 页',displayMsg:'当前显示 {from} - {to} 条记录   共 {total} 条记录',})});function emptyqueen(){var fileArray=uploader.getFiles();for(var i=0;i<fileArray.length;i++){uploader.cancelFile(fileArray[i]);uploader.removeFile(fileArray[i],true)};$('#fileList').empty()}function feupld_del(){var row=$('#upload_dg').datagrid('getSelected');if(row){$.messager.confirm('提示','您确定要删除这条记录吗？',function(r){if(r){$.post('/xiangmuguanli/feupld_del',{id:row.fileuploadID,path:row.filepath,name:row.filename},function(result){if(result.success){$('#upload_dg').datagrid('reload');$.messager.show({title:'提示',msg:result.errorMsg})}else{$.messager.show({title:'错误提示',msg:result.errorMsg})}},'json')}})}else{$.messager.alert("错误提示","请选择要删除的行！","warning")}};function rowformater(value,row,index){if(row){var filename=row.filename;var filepath=row.filepath;return"<a href='GetFile?filename="+filename+"&filepath="+filepath+"'>下载</a>";}};