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
        //console.log(tab);
        //console.log(tab[0]);

        //不重复打开 ,之前这里是保留的，但是在评委审核通过后，我们需要刷新“已审核”tab标签，所以这里我们允许重复打开
        //if (tab.attr("opend")) {
        //    return;
        //}

        var url = tab.panel("options").url;  //这里的URL是tab中的url
        //var op=tab.panel("options");
        //console.log(op);


        if (url) {
            //$(tab[0]).html('<iframe frameborder="0" scrolling="auto" width="100%" height="100%" src="${path}' + url + '"></iframe>');  // /xiangmuguanli/${path}xiangmuguanli/wsxx_jbxx

            $(tab[0]).html('<iframe frameborder="0" scrolling="auto" width="100%" height="100%" src=" ' + url + ' "></iframe>');

            tab.attr("opend", true);
        }
    }
    open(0);
});