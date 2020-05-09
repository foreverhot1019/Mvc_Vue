//判断，是否有 总单号
function is_BL_H(tabsId) {
    var ss = $("#MBL", tabsId);
    if ($("#BL", tabsId).prop('checked')) {
        ss.textbox("enable");
    } else {
        ss.textbox("disable");
    }
}
//加载时绑定combogrid数据源
function initbindcombogriddata_H(tabsId, FromDataFDXX) {
    //is_BL_H(tabsId);
    $("#BL", tabsId).on('click', function () {
        is_BL_H(tabsId);
    });
    //航空公司
    $("#Airways_Code", tabsId).combogrid($.extend({}, combogrid_DefaultSettings, {
        url1: '/CusBusInfos/GetPagerCusBusInfos_FromCache',
        columns: [[
            { field: 'ID', title: '代码', width: 97 },
            { field: 'TEXT', title: '名称', width: 157 }
        ]],
        onChange: function (newValue, oldValue) {
            //SetSameValcombogrid_onChange("#Airways_Code", tabsId, newValue);
        }
    }));
    //国外代理
    $("#FWD_Code", tabsId).combogrid($.extend({}, combogrid_DefaultSettings, {
        url1: '/CusBusInfos/GetPagerCusBusInfos_FromCache',
        columns: [[
            { field: 'ID', title: '代码', width: 97 },
            { field: 'TEXT', title: '名称', width: 157 }
        ]],
        onChange: function (newValue, oldValue) {
            //SetSameValcombogrid_onChange("#FWD_Code", tabsId, newValue);
        }
    }));
    //启运港
    $("#Depart_Port", tabsId).combogrid($.extend({}, combogrid_DefaultSettings, {
        textField: 'IDTEXT',
        url1: '/PARA_AirPorts/GetPARA_AirPorts_FromCache',
        columns: [[
            { field: 'ID', title: '代码', width: 97 },
            { field: 'TEXT', title: '名称', width: 157 }
        ]],
        onChange: function (newValue, oldValue) {
            //SetSameValcombogrid_onChange("#Depart_Port", tabsId, newValue);
        }
    }));
    //目的港
    $("#End_Port", tabsId).combogrid($.extend({}, combogrid_DefaultSettings, {
        textField: 'IDTEXT',
        url1: '/PARA_AirPorts/GetPARA_AirPorts_FromCache',
        columns: [[
            { field: 'ID', title: '代码', width: 97 },
            { field: 'TEXT', title: '名称', width: 157 }
        ]],
        onChange: function (newValue, oldValue) {
            //SetSameValcombogrid_onChange("#End_Port", tabsId, newValue);
        }
    }));
    //币种
    $("#Currency_H", tabsId).combogrid($.extend({}, combogrid_DefaultSettings, {
        url1: '/PARA_CURRs/GetPagerPARA_CURR_FromCache',
    }));
    //航班号
    $("#Flight_No", tabsId).combogrid($.extend({}, combogrid_DefaultSettings, {
        url1: '/PARA_AirLines/GetPagerAirLine_FromCache',
        columns: [[
            { field: 'ID', title: '代码', width: 220 }
        ]],
        onChange: function (newValue, oldValue) {
            var data = $(this).combogrid("grid").datagrid("getSelected");
            var $text = "";
            var $time = "";
            if (data != null) {//费用代码存在时，自动填写费用名称
                $text = data.TEXT;
                $time = data.Time;
            }
            $("input[id='Flight_No']").each(function (k, item) {
                var val = $(item).combogrid('getValue');
                if (newValue == val)
                    return;
                $(this).combogrid('setValue', newValue);
            });
            if ($time != null) {//费用代码存在时，自动填写费用名称
                $("input[id='Flight_No1']").each(function () {
                    $(this).textbox("setValue", $time);
                });
            } else {//清空费用名称
                $("input[id='Flight_No1']").each(function () {
                    $(this).textbox("setValue", "");
                });
            }
        }
    }));
    //成交条款
    $("#Bragainon_Article_H", tabsId).combogrid($.extend({}, combogrid_DefaultSettings, {
        textField: 'ID',
        url1: '/DealArticles/GetPagerDealArticle_FromCache',
        columns: [[
            { field: 'ID', title: '代码', width: 97 },
            { field: 'TEXT', title: '名称', width: 157 }
        ]],
        onChange: function () {
            var row = $(this).combogrid("grid").datagrid("getSelected");
            if (row != null) {
                $("#Pay_Mode_H", tabsId).textbox("setValue", row.Pay_ModeCode);
                if (row.Carriage != null && row.Carriage != "") {
                    $("#Carriage_H", tabsId).textbox("setValue", row.Carriage);
                } else {
                    $("#Carriage_H", tabsId).textbox("setValue", "");
                }
                if (row.Incidental_Expenses != null && row.Incidental_Expenses != "") {
                    $("#Incidental_Expenses_H", tabsId).textbox("setValue", row.Incidental_Expenses);
                } else {
                    $("#Incidental_Expenses_H", tabsId).textbox("setValue", "");
                }
            } else {
                $("#Pay_Mode_H").combogrid("setValue", "");
                $("#Carriage_H").textbox("setValue", "");
                $("#Incidental_Expenses_H").textbox("setValue", "");
            }
        }
    }));
    //付款方式
    //$("#Pay_Mode_H", tabsId).combogrid($.extend({}, combogrid_DefaultSettings, {
    //    url1: '/BD_DEFDOC_LISTs/Get_DEFDOC_DICT_FromCache?DOCCODE=Pay_Mode',
    //    columns: [[
    //        { field: 'TEXT', title: '名称' }
    //    ]]
    //}));
    initEasyUIControl();//回车下一个控件

    BmsBill($("#Carriage_H").textbox("getValue"), "#Carriage_H2", tabsId)
    BmsBill($("#Incidental_Expenses_H").textbox("getValue"), "#Incidental_Expenses_H2", tabsId)
    //设置ComboGrid真实url(为了不页面进入后 不立即加载数据，优化页面载入速度)
    resetCombogridUrl(tabsId, "url1", false, FromDataFDXX);
    
    totalCharge_Weight(tabsId);
}
//计算结算重量
function totalCharge_Weight(tabsId) {
    $("input[id='Pieces_H'],input[id='Weight_H'],input[id='Volume_H'],input[id='Pieces_M'],input[id='Weight_M'],input[id='Volume_M']", tabsId).each(function () {
        var $this = $(this);
        var data = $this.data();
        data.tabsId = tabsId;
        data.numberbox.options.onChange = PWVonChange;
        data.textbox.options.onChange = PWVonChange;
    });
}
//修改 件 毛 体 触发 重新计算 计费重量
function PWVonChange() {
    var $this = $(this);
    var type = $this.attr('id').substr(-2);
    var data = $this.data();
    var tabsId = data.tabsId;
    var $Volume_TS = $("#Volume" + type, tabsId).numberbox("getValue");//体积 
    var $Charge_Weight_TS = 0;//计费重量
    if ($Volume_TS != null && $Volume_TS > 0) {
        $Charge_Weight_TS = parseFloat(($Volume_TS / 0.006).toFixed(2));
    }
    var $Weight_TS = $("#Weight" + type, tabsId).numberbox("getValue");//重量
    if ($Weight_TS != null && $Weight_TS != "") {
        if ($Charge_Weight_TS < $Weight_TS) {
            $Charge_Weight_TS = $Weight_TS;
        }
        if ($Charge_Weight_TS == 0) {
            $Charge_Weight_TS = $Weight_TS;
        }
    }
    $("#Charge_Weight" + type, tabsId).numberbox("setValue", $Charge_Weight_TS);
}
//分单向上绑定
function initFDLoading(idfd) {
    $.each(domejsons, function (index, item) {
        switch (item.num) {
            case 1:
                var value = $(item.domid, "#zdxx")[item.type]("getValue");
                $(item.domid, idfd)[item.type]("setValue", value);
                break;
            case 2:
                var value = $(item.domid, "#zdxx")[item.type]("getValue");
                if (idfd.indexOf(item.targetid1) != -1) {
                    $(item.domid, idfd)[item.type]("setValue", value);
                }
                break;
        }
    });
}
//获取分单 
function gethorderValue(tabsId,wttabid) {
    //分单表信息
    var newH_Order = {
        Id: $("#Id", tabsId).val(),
        Operation_Id: $("#Operation_Id", wttabid).textbox("getValue"),
        Shipper_H: $("#Shipper_H", tabsId).textbox("getValue"),
        Consignee_H: $("#Consignee_H", tabsId).textbox("getValue"),
        Notify_Part_H: $("#Notify_Part_H", tabsId).textbox("getValue"),
        Currency_H: $("#Currency_H", tabsId).combogrid("getValue"),
        Bragainon_Article_H: $("#Bragainon_Article_H", tabsId).combogrid("getValue"),
        Pay_Mode_H: $("#Pay_Mode_H", tabsId).combogrid("getValue"),
        Carriage_H: $("#Carriage_H", tabsId).textbox("getValue"),
        Incidental_Expenses_H: $("#Incidental_Expenses_H", tabsId).textbox("getValue"),
        Declare_Value_Trans_H: $("#Declare_Value_Trans_H", tabsId).textbox("getValue"),
        Declare_Value_Ciq_H: $("#Declare_Value_Ciq_H", tabsId).textbox("getValue"),
        Risk_H: $("#Risk_H", tabsId).textbox("getValue"),
        Marks_H: $("#Marks_H", tabsId).textbox("getValue"),
        EN_Name_H: $("#EN_Name_H", tabsId).textbox("getValue"),
        Pieces_H: $("#Pieces_H", tabsId).numberbox("getValue"),
        Weight_H: $("#Weight_H", tabsId).numberbox("getValue"),
        Volume_H: $("#Volume_H", tabsId).numberbox("getValue"),
        Charge_Weight_H: $("#Charge_Weight_H", tabsId).numberbox("getValue"),
        MBL: $("#MBL", tabsId).val(),
        HBL: $("#HBL", tabsId).textbox("getValue"),

        Is_Self: $("#Is_Self", wttabid).get(0).checked,
        Ty_Type: $("#Ty_Type", wttabid).combogrid("getValue"),
        //Lot_No: $("#Lot_No").textbox("getValue"),
        //Hbl_Feight: $("#Hbl_Feight").val(),
        //Is_XC: $("#Is_XC").get(0).checked,
        //Is_BAS: $("#Is_BAS").get(0).checked,
        //Is_DCZ: $("#Is_DCZ").get(0).checked,
        //Is_ZB: $("#Is_ZB").get(0).checked,
        //Is_TG: $("#Is_TG").get(0).checked,//退关
    };
    return newH_Order;
}
//鼠标移动离开超链接时，隐藏删除按钮
function displeyImg(img, value) {
    var $id = $("#" + img, value);
    $id.attr("style", "visibility:hidden");
    // document.all(img).style.visibility='hidden';
}
//鼠标移动到超链接时，显示删除按钮
function imgblur(img, value) {
    var $id = $("#" + img, value);
    $id.attr("style", "visibility:visible");
    //document.all(img).style.visibility="visible";        
}
//点击删除按钮，删除该按钮对应的上传的附档图片
function deletepicture(nowId, value) {//Delete
    console.log("nowId", nowId);
    var url = '/Pictures/Delete?id=' + nowId;
    $.ajax({
        type: 'POST',
        url: url,//获取数据的函数
        async: true,//true 异步，false 同步
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        beforeSend: function (xhr) {//发送请求前运行的函数
            //console.log('beforeSend');
            $.messager.progress({
                title: '数据处理中',
                msg: '数据处理中，请等待...'
            });
            AddAjaxActionGuidName(this);
        },
        success: function (data) {//查询成功,data为返回的数据
            if (data.Success) {
                //$.messager.alert("提示", "删除成功！", "info");
                window.location.reload();//刷新当前页面
            }
            else {
                var ErrMsgStr = data.ErrMsg;
                if (!(typeof (data.ErrMsg) === 'undefined' || data.ErrMsg == null || data.ErrMsg == ''))
                    ErrMsgStr = data.ErrMsg;
                $.messager.alert("错误", ErrMsgStr, 'error');
            }
        },
        error: function () {//查询失败
            $.messager.alert("错误", "提交错误了！", 'error');
        },
        complete: function (xhr, status) {//请求完成时运行的函数（在请求成功或失败之后均调用，即在 success 和 error 函数之后）。
            $.messager.progress('close');
        }
    });
}