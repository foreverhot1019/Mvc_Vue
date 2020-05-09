/*
module.exports可用时，按module.exports 规范输出
符合amd规范时，按amd规范define输出
exports 可用时 按照exports输出
window 输出 相当于var
*/
!function (t, e)
{
    "object" == typeof exports && "object" == typeof module ? module.exports = e() :
    "function" == typeof define && define.amd ? define([], e) : "object" == typeof exports ? exports.LazyLoadingComponent = e() : t.LazyLoadingComponent = e()
}(this, function ()
{
    const LazyLoadingComponent = function (Vue_Component)
    {
        return {
            // 需要加载的组件 (应该是一个 `Promise` 对象)
            component: new Promise(function (resolve, reject)
            {
                // 这个特殊的 `require` 语法将会告诉 webpack
                // 自动将你的构建代码切割成多个包，这些包
                // 会通过 Ajax 请求加载
                require([Vue_Component], function (CostMoney)
                {
                    resolve(CostMoney);
                });
            }),
            // 异步组件加载时使用的组件
            loading: {
                template: '<div ref="LazyLoading" id="LazyLoading" class="el-row" style="height:100px;">&nbsp;</div>',
                mounted: function ()
                {
                    //console.log('LazyLoading-mounted', this);
                    const loading = this.$loading({
                        lock: true,
                        text: '加载中...',
                        target: this.$refs.LazyLoading,//document.querySelector("#LazyLoading"),
                        spinner: 'el-icon-loading',
                        background: 'rgba(0, 0, 0, 0.7)'
                    });
                },//相当于构造函数，渲染完dom后触发
            },
            // 加载失败时使用的组件
            error: {
                template: '<div class="el-row" style="padding-top:3px;margin-top:5px;height:50px; border:1px solid #dcdfe6;"><div style="width:100px; margin:auto;color:red;">加载失败...</div></div>'
            },
            // 展示加载时组件的延时时间。默认值是 200 (毫秒)
            delay: 10,
            // 如果提供了超时时间且组件加载也超时了，
            // 则使用加载失败时使用的组件。默认值是：`Infinity`
            timeout: 60000
        }
    };
    return LazyLoadingComponent;
});