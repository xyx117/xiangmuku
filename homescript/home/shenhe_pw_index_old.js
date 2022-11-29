
$(function () {


    $('#pw_aa').accordion({
        onSelect: function (title, index) {

            if (index == 1) {

                $('#xmmlgl').datalist('reload');


            };
            if (index == 0) {

                $('#mainFrame').attr("src", "/home/indexnew");

            };
        }
    });

    //$("#main-tab").tabs({
    //    onContextMenu: function (e, title) {
    //        e.preventDefault();
    //        $("#tab-menu").menu("show", { left: e.pageX, top: e.pageY })
    //            .data("tabTitle", title); //将点击的Tab标题加到菜单数据中
    //    }
    //});

    //$("#tab-menu").menu({
    //    onClick: function (item) {
    //        tabHandle(this, item.id);
    //    }
    //});
});

function addTab(title, url, icon) {

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

//function tabHandle(menu, type) {
//    var title = $(menu).data("tabTitle");
//    var $tab = $("#main-tab");
//    var tabs = $tab.tabs("tabs");
//    var index = $tab.tabs("getTabIndex", $tab.tabs("getTab", title));
//    var closeTitles = [];
//    switch (type) {
//        case "tab-menu-refresh":
//            var iframe = $(".tabs-panels .panel").eq(index).find("iframe");
//            if (iframe) {
//                var url = iframe.attr("src");
//                iframe.attr("src", url);
//            }
//            break;
//        case "tab-menu-openFrame":
//            var iframe = $(".tabs-panels .panel").eq(index).find("iframe");
//            if (iframe) {
//                window.open(iframe.attr("src"));
//            }
//            break;
//        case "tab-menu-close":
//            closeTitles.push(title);
//            break;
//        case "tab-menu-closeleft":
//            if (index == 0) {
//                $.osharp.easyui.msg.tip("左边没有可关闭标签。");
//                return;
//            }
//            for (var i = 0; i < index; i++) {
//                var opt = $(tabs[i]).panel("options");
//                if (opt.closable) {
//                    closeTitles.push(opt.title);
//                }
//            }
//            break;
//        case "tab-menu-closeright":
//            if (index == tabs.length - 1) {
//                $.osharp.easyui.msg.tip("右边没有可关闭标签。");
//                return;
//            }
//            for (var i = index + 1; i < tabs.length; i++) {
//                var opt = $(tabs[i]).panel("options");
//                if (opt.closable) {
//                    closeTitles.push(opt.title);
//                }
//            }
//            break;
//        case "tab-menu-closeother":
//            for (var i = 0; i < tabs.length; i++) {
//                if (i == index) {
//                    continue;
//                }
//                var opt = $(tabs[i]).panel("options");
//                if (opt.closable) {
//                    closeTitles.push(opt.title);
//                }
//            }
//            break;
//        case "tab-menu-closeall":
//            for (var i = 0; i < tabs.length; i++) {
//                var opt = $(tabs[i]).panel("options");
//                if (opt.closable) {
//                    closeTitles.push(opt.title);
//                }
//            }
//            break;
//    }
//    for (var i = 0; i < closeTitles.length; i++) {
//        $tab.tabs("close", closeTitles[i]);
//    }
//};

//function transToTreeData(data) {
//    return $.Enumerable.From(data).Select(function (m) {
//        var obj = {};
//        obj.id = m.Id;
//        obj.text = m.Text;
//        obj.iconCls = m.IconCls;
//        obj.checked = m.Checked;
//        if (m.Url) {
//            obj.attributes = { url: m.Url };
//        }
//        if (m.Children && m.Children.length > 0) {
//            obj.children = transToTreeData(m.Children);
//        }
//        return obj;
//    }).ToArray();
//};

function xm(url) {
    $("#main-tab").tabs("select", "首页");
    $("#mainFrame").attr("src", url);
};

function showniandulist(url) {
    $("#main-tab").tabs("select", "首页");
    $("#mainFrame").attr("src", url);
};
