$(function () {
    function open(index) {

        var tab = $("#easytab").tabs("getTab", index);

        //不重复打开
        if (tab.attr("opend")) {

            return;
        }
        var url = tab.panel("options").url;  //这里的URL是tab中的url

        if (url) {

            var content = '<iframe scrolling="auto" frameborder="0"  src="' + url + '" style="width:100%;height:100%;"></iframe>';

            $("#easytab").tabs('update', {
                tab: tab,
                options: {
                    content: content,
                }

            });
            tab.attr("opend", true);


        }
    }

    $("#easytab").tabs({
        tabPosition: 'top',
        fit: true,
        onSelect: function (title, index) {
            open(index);
        }
    });


    open(0);
});