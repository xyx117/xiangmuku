$(function () {
    $('#cc').combobox({
        onSelect: function (item) {

            var value = $('#cc').combobox('getValue');
            //alert(value);
            if (value == "tongguo") {
                $('#pw_tubiao').attr("src", "/xiangmuguanli/pw_BasicBar?leibie=tongguo&muluname="+muluname+"&loingid="+loingid);
            }
            if (value == "weitonggguo") {
                $('#pw_tubiao').attr("src", "/xiangmuguanli/pw_BasicBar?leibie=weitonggguo&muluname="+muluname+"&loingid="+loingid);
            }


            if (value == "tongguo_pie") {
                $('#pw_tubiao').attr("src", "/xiangmuguanli/pw_piechart?leibie=tongguo_pie&muluname="+muluname+"&loingid="+loingid);
            }
            if (value == "weitongguo_pie") {
                $('#pw_tubiao').attr("src", "/xiangmuguanli/pw_piechart?leibie=weitongguo_pie&muluname="+muluname+"&loingid="+loingid);
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
