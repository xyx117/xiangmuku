$(function () {


    $('#swth_btn').switchbutton({
        checked: false,
        onChange: function (checked) {

            if (checked) {
                $('#tubiao').attr("src", "/xiangmuguanli/bushen_ld_all?muluname="+dgtitle);

            }
            else {
                $('#tubiao').attr("src", "/xiangmuguanli/bushen_ld_shen?muluname="+dgtitle+"&loingid="+loingid);

            }
        }
    })


});