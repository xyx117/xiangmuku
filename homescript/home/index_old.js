﻿
var zongerchanged = 0;
$(function () {
    $('#aa').accordion({
        onSelect: function (title, index) {
              
            if (index == 1) {
                   
                $('#xmmlgl').datalist('reload');

                 
            };
            if (index == 0) {
                   
                $('#mainFrame').attr("src", "/home/indexnew");

            };
        }
    });
    $("#main-tab").tabs({
        onSelect: function (title, index) {
            if (title == "首页" && zongerchanged == 1) {
                zongerchanged = 0;
                var iframe = $(".tabs-panels .panel").eq(index).find("iframe");
                if (iframe) {
                    var url = iframe.attr("src");
                    iframe.attr("src", url);
                }
            }
        }
    });
        
});

function addTab(title, url, icon) {
    //alert("tet");
    var $mainTabs = $("#main-tab");
    if ($mainTabs.tabs("exists", title)) {
        $mainTabs.tabs("select", title);
    } else {
        $mainTabs.tabs("add", {
            title: title,
            closable: true,
            icon: icon,
            content: createFrame(url)
        });
    }
};

function createFrame(url) {
    var html = '<iframe scrolling="auto" frameborder="0"  src="' + url + '" style="width:100%;height:99%;"></iframe>';
    return html;
};

function transToTreeData(data) {
    return $.Enumerable.From(data).Select(function (m) {
        var obj = {};
        obj.id = m.Id;
        obj.text = m.Text;
        obj.iconCls = m.IconCls;
        obj.checked = m.Checked;
        if (m.Url) {
            obj.attributes = { url: m.Url };
        }
        if (m.Children && m.Children.length > 0) {
            obj.children = transToTreeData(m.Children);
        }
        return obj;
    }).ToArray();
};


   
                
//在点击列表后，我的页面中datagrid出现变形
function xm(url) {
    $("#main-tab").tabs("select", "首页");
    $("#mainFrame").attr("src", url);
};


function showniandulist(url) {
    $("#main-tab").tabs("select", "首页");
    $("#mainFrame").attr("src", url);
};


  
          
//文本框有输入后，出现清空图标
$.extend($.fn.textbox.methods, {
    addClearBtn: function (jq, iconCls) {
        return jq.each(function () {
            var t = $(this);
            var opts = t.textbox('options');
            opts.icons = opts.icons || [];
            opts.icons.unshift({
                iconCls: iconCls,
                handler: function (e) {
                    $(e.data.target).textbox('clear').textbox('textbox').focus();
                    $(this).css('visibility', 'hidden');
                }
            });
            t.textbox();
            if (!t.textbox('getText')) {
                t.textbox('getIcon', 0).css('visibility', 'hidden');
            }
            t.textbox('textbox').bind('keyup', function () {
                var icon = t.textbox('getIcon', 0);
                if ($(this).val()) {
                    icon.css('visibility', 'visible');
                } else {
                    icon.css('visibility', 'hidden');
                }
            });
        });
    }
});

   

