$(function () {
    $('#swth_btn').switchbutton({
        checked: false,
        onChange: function (checked) {

            if (checked) {
                $('#tubiao').attr("src", "/xiangmuguanli/shenhe_ld_all?muluname="+dgtitle);
            }
            else {
                $('#tubiao').attr("src", "/xiangmuguanli/shenhe_ld_shen?muluname="+dgtitle+"&loingid="+loingid);
            }
        }
    })

    //此方法可行
    //$('#tubiao').panel({
    //    href: '/xiangmuguanli/BasicBar?leibie=jine',
    //        onLoad:function(){
    //            alert('loaded successfully');
    //       }
    //});
});