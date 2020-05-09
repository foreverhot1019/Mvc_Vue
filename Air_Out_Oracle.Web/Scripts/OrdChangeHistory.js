/*
查看操作日志1.0.0
Michael
20180117创建
*/
(function ($) {
    var IsCreateUI = false;
    var OrdChangeHistorier = function (options) {
        var $data = $.extend({
            EasyUIWindowId: 'Win_OrdChangeHistory',     //EasyUI-window的ID
            EasyUIDataGridId: 'dg_OrdChangeHistory',    //EasyUI-DataGrid的ID
            IsCreateUI: false,
            AsyncGetUIUrl: '/ChangeOrderHistories/GetPopupOrdChangHis',
            parserDivId: "parserDiv",                    //EasyUI渲染使用的DIV
            DataGridUrl: '/ChangeOrderHistories/GetData',//DataGrid-Url
            Key_Id: 0, //操作表主键
            TabName: '',//操作表名称
            AsyncFuc: function () {

            }
        }, options);

        $data.IsCreateUI = false;
        $data.parserDivTempleate = $("<div id='" + $data.parserDivId + "' style='display:none'></div>");
        //创建UI
        $data.CreateUI = function () {
            var $this = this;
            var $parserDiv = $('#' + $this.parserDivId);
            if (typeof ($parserDiv) === 'undefined' || $parserDiv == null || $parserDiv.length <= 0) {
                $parserDiv = $this.parserDivTempleate;
                $("body").append($parserDiv);
            }
            $this.GetWinUI($this.AsyncGetUIUrl, function (resdata) {
                //console.log(this, resdata)
                var docm = $("#" + this.parserDivId);
                if (resdata) {
                    this.IsCreateUI = IsCreateUI = true;
                    var $resdata = $(resdata);
                    $("div.easyui-window", $resdata).attr("id", this.EasyUIWindowId);
                    $("div.easyui-datagrid", $resdata).attr("id", this.EasyUIDataGridId);
                    docm.append($resdata);
                    //渲染EasyUI样式
                    $.parser.parse(docm);
                }
                else
                    this.IsCreateUI = IsCreateUI = false;
                if (typeof (this.AsyncFuc) === 'function')
                    this.AsyncFuc(this.EasyUIDataGridId);
            });
        };
        //异步获取弹出框UI
        $data.GetWinUI = function (url, MyWinFuc) {
            var $this = this;
            //console.log(this,url, typeof (MyWinFuc), MyWinFuc);
            $.ajax({
                type: "POST",
                datatype: "html",
                async: true,
                data: JSON.stringify({ "CheckRepeat": false }),
                url: url,
                success: function (data) {
                    //console.log(data);
                    if (typeof(MyWinFuc)==='function')
                        MyWinFuc.call($this, data);
                    //字符串方法调用
                    //eval(InitMyWinFucName);//js 方式
                    //$(this)[InitMyWinFucName](); //jquery方式$(this).trigger(InitMyWinFucName)
                },
                error: function () {
                }
            });
        };
        //获取数据
        $data.GetChangeHisByTableName = function () {
            var $this = this;
            //创建UI结束执行方法
            $this.AsyncFuc = function (dgId) {
                $("#" + $this.EasyUIWindowId).window('open');
                var opts = $("#" + dgId).datagrid('options');
                if (typeof (opts.url) === 'undefind' || opts.url == null || opts.url != $this.DataGridUrl) {
                    opts.url = $this.DataGridUrl;
                }
                //opts.sortName = "Key_Id";
                opts.queryParams = {
                    filterRules: JSON.stringify([
                        { field: "Key_Id", op: 'equal', value: $this.Key_Id },
                        { field: "tableName", op: 'equal', value: $this.TabName }
                    ]),
                    sort: opts.sortName,
                    order: opts.sortOrder
                };
                console.log(opts.queryParam);
                if ($this.Key_Id > 0) {
                    $("#" + dgId).datagrid('reload');
                }
                else {
                    console.log('操作表主键不能为空');
                }
            };
            //console.log($this.IsCreateUI);
            if (!IsCreateUI)
                $this.CreateUI();
            else
                $this.AsyncFuc($this.EasyUIDataGridId);
        }
        //执行
        $data.GetChangeHisByTableName();
    };

    if (typeof jQuery !== 'undefined') {
        jQuery.extend({
            OrdChangeHistorier: function () {
                return OrdChangeHistorier(Array.prototype.slice.call(arguments, 0)[0]);
            }
        });
    }
})(jQuery);