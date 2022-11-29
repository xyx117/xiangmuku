$(function () {
    $('#cc').combobox({
        onSelect: function (item) {

            var value = $('#cc').combobox('getValue');
            //alert(value);
            if (value == "shuliang") {
                $('#tubiao').attr("src", "/xiangmuguanli/BasicBar?leibie=shuliang&muluname="+muluname);
            }
            if (value == "jine") {
                $('#tubiao').attr("src", "/xiangmuguanli/BasicBar?leibie=jine&muluname="+muluname);
            }
            if (value == "shuliang_pie") {
                $('#tubiao').attr("src", "/xiangmuguanli/piechart?leibie=shuliang&muluname="+muluname);
            }
            if (value == "jine_pie") {
                $('#tubiao').attr("src", "/xiangmuguanli/piechart?leibie=jine&muluname="+muluname);
            }
        }
    });

    //此方法可行
    //$('#tubiao').panel({
    //    href: '/xiangmuguanli/BasicBar?leibie=jine',
    //        onLoad:function(){
    //            alert('loaded successfully');
    //       }
    //});

});