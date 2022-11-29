$(function () {

    $("#loginmsg").fadeOut(10000);

    choose_bg();   
    
});




function choose_bg() {
    var bg = Math.floor(Math.random() * 9 + 1);
    $('#loginbg').css('background-image', 'url(../../Content/image/loginbg_0' + bg + '.jpg)');

}
function closezoh() {
    if (confirm("我知道了，下次不再提示?")) {
        writeCookie("checkbrower", "1", 24);
    }
    document.getElementById("zmain").style.display = "none";

}
function donwload() {
    var form = $("<form>");//定义一个form表单 );
    form.attr("style", "display:none");
    form.attr("target", "");
    form.attr("method", "post");
    form.attr("action", "http://www.firefox.com.cn/download/");
    $("body").append(form);//将表单放置在web中
    var input1 = $("<input>");
    input1.attr("type", "hidden");
    input1.attr("name", "exportDate");
    input1.attr("value", (new Date()).getMilliseconds());
    form.append(input1);
    form.submit();//表单提交
    form.remove();
    document.getElementById("zmain").style.display = "none";
    //document.getElementById("topLayer").style.display = "none";
}
var c = getCookie("checkbrower");
if (c != "1") {
    document.getElementById("zmain").style.display = "block";
    //document.getElementById("topLayer").style.display = "block";
}

function writeCookie(name, value, hours) {
    var expire = "";
    if (hours != null) {
        expire = new Date((new Date()).getTime() + hours * 3600000);
        expire = "; expires=" + expire.toGMTString();
        if (hours == "0") {
            expire = "";
        }
        path = ";path=/;";
    }
    document.cookie = name + "=" + escape(value) + path + expire;
}

function getCookie(Name) {
    var search = Name + "=";
    if (document.cookie.length > 0) {
        offset = document.cookie.indexOf(search)
        if (offset != -1) {
            offset += search.length
            end = document.cookie.indexOf(";", offset)
            if (end == -1) end = document.cookie.length
            return unescape(document.cookie.substring(offset, end))
        }
        else return "";
    }
}

var c = getCookie("ptrmooc");
if (c == "") {
    writeCookie("ptrmooc", "t", 0);
}