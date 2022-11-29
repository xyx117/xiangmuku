$(function () {
    $("#easytab").tabs({
        tabPosition: 'top',
        fit: true,
        onSelect: function (title, index) {
            open(index);

        }
    });

    function open(index) {

        var tab = $("#easytab").tabs("getTab", index);

        //不重复打开，这里在为  操作设置按钮样式的时候做出修改，要求可以重复打开，因为  财务主管  仲裁后，要在  “未评审” 的tab中显示出该仲裁的记录
        //if (tab.attr("opend")) {
        //    return;
        //}
        var url = tab.panel("options").url;  //这里的URL是tab中的url

        if (url) {

            $(tab[0]).html('<iframe frameborder="0" scrolling="auto" width="100%" height="100%" src=" ' + url + ' "></iframe>');

            tab.attr("opend", true);
        }
    }
    open(0);
});