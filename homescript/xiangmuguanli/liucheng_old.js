//根据jQuery选择器找到需要加载ystep的容器
//loadStep 方法可以初始化ystep
$(function () {

    //分管领导人参与审核显示流程
    $(".ystep_shenhe").loadStep({
        //ystep的外观大小
        //可选值：small,large
        size: "large",
        //ystep配色方案
        //可选值：green,blue
        color: "green",
        //ystep中包含的步骤
        steps: [{
            title: "开始",
        }, {
            //步骤名称
            title: "录入",
            //步骤内容(鼠标移动到本步骤节点时，会提示该内容)
            content: "创建者"
        }, {
            title: "提交",
            content: "创建者"
        }, {
            title: "审核",
            content: "部门负责人"
        }, {
            title: "送审",
            content: "部门负责人"
        }, {
            title: "审定",
            content: "分管领导"
        },
        {
            title: "评审",
            content: "评委"
        },

        {
            title: "终审",
            content: "业务专员"
        }, {
            title: "结束"
        }]
    });

    $(".ystep_shenhe").setStep(step);


    //分管领导人不参与审核显示流程
    $(".ystep_bushen").loadStep({
        //ystep的外观大小
        //可选值：small,large
        size: "large",
        //ystep配色方案
        //可选值：green,blue
        color: "green",
               
        steps: [{
            title: "开始",
        }, {
            //步骤名称
            title: "录入",
            //步骤内容(鼠标移动到本步骤节点时，会提示该内容)
            content: "创建者"
        }, {
            title: "提交",
            content: "创建者"
        }, {
            title: "审核",
            content: "部门负责人"
        }, {
            title: "送审",
            content: "部门负责人"
        },
        {
            title: "评审",
            content: "评委"
        },
        {
            title: "终审",
            content: "业务专员"
        }, {
            title: "结束"
        }]
    });
    $(".ystep_bushen").setStep(step2);


    //不是评委，业务专员，领导，业务主管，并且目录思分管领导参与审核的
    $(".ystep_shenhe_ybf").loadStep({
        //ystep的外观大小
        //可选值：small,large
        size: "large",
        //ystep配色方案
        //可选值：green,blue
        color: "green",
        //ystep中包含的步骤
        steps: [{
            title: "开始",
        }, {
            //步骤名称
            title: "录入",
            //步骤内容(鼠标移动到本步骤节点时，会提示该内容)
            content: "创建者"
        }, {
            title: "提交",
            content: "创建者"
        }, {
            title: "审核",
            content: "部门负责人"
        }, {
            title: "送审",
            content: "部门负责人"
        }, {
            title: "审定",
            content: "分管领导"
        },
        {
            title: "终审",
            content: "业务专员"
        }, {
            title: "结束"
        }]
    });

    $(".ystep_shenhe_ybf").setStep(step3);


    //不是评委，业务专员，领导，业务主管，并且目录思分管领导参与审核的
    $(".ystep_bushen_ybf").loadStep({
        //ystep的外观大小
        //可选值：small,large
        size: "large",
        //ystep配色方案
        //可选值：green,blue
        color: "green",
        //ystep中包含的步骤
        steps: [{
            title: "开始",
        }, {
            //步骤名称
            title: "录入",
            //步骤内容(鼠标移动到本步骤节点时，会提示该内容)
            content: "创建者"
        }, {
            title: "提交",
            content: "创建者"
        }, {
            title: "审核",
            content: "部门负责人"
        }, {
            title: "送审",
            content: "部门负责人"
        },
        //{
        //    title: "审定",
        //    content: "分管领导"
        //},
        {
            title: "终审",
            content: "业务专员"
        }, {
            title: "结束"
        }]
    });

    $(".ystep_bushen_ybf").setStep(step4);



});

//当项目名称字数太长，限定只显示前面一部分
function TitleFormatter(value, row, index) {
    var value1 = value;
    if (value1 == null) {
        var ss = '<a href="#" title="' + value + '"  class="easyui-tooltip"></a>';
        return ss
    }
    else {
        if (value1.length > 16) {
            value1 = value1.substr(0, 16) + "...";
        }
        var ss = '<a href="#" title="' + value + '" class="easyui-tooltip">' + value1 + '</a>';
        return ss
    }
};