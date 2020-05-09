/*自定义 el-dialog  全屏-拖拽指令*/
define(function (require)
{
    var Vue = require('vue');
    Vue.component('ComboGrid', {
        props:['prop','url','ViewField'],
        template: "",
        created: function ()
        {
            this.ArrOption = [];
        },//数据初始化，还未渲染dom
        mounted: function ()
        {
            this.tb_GetData();//table数据初始化
        },//相当于构造函数，渲染完dom后触发
        data: function ()
        {
            return {};
        },
        Methods: {
            LoadData: function ()
            {

            }
        },
    });
    //全局组件 弹出框
    Vue.component('M-el-dialog', {
        props: {
            templeteId: {
                type: String,
                required: true
            },//模板Id
            Original_data: Object,//原始数据
            Current_data:{
                type: Object,
                required: true
            },//当前编剧数据
            GetUrl: {
                type: String
            },//获取数据Url
            OK_Func: {
                type: Function
            },//确定事件
            Cancle_Func:{
                type: Function
            },//取消事件
        },
        mounted: function ()
        {
        },//相当于构造函数，渲染完dom后触发
        data: function ()
        {
            return {
                isLoading: false,//加载中

            }
        },
        methods: {

        },
        templete:""
    });
});