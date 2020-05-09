function initaadd(value, index, FromDataWTXX) {
    $("#Operation_Id", value).textbox("disable");
    $("#Custom_Code", value).textbox("disable");
    $("#Area_Code", value).textbox("disable");

    $("#Pieces_Fact", value).numberbox("disable");
    $("#Weight_Fact", value).numberbox("disable");
    $("#Volume_Fact", value).numberbox("disable");
    $("#Charge_Weight_Fact", value).numberbox("disable");
    $("#Bulk_Weight_Fact", value).numberbox("disable");
    $("#Account_Weight_Fact", value).numberbox("disable");

    $("#Charge_Weight_TS", value).numberbox("disable");
    $("#Charge_Weight_SK", value).numberbox("disable");
    $("#Charge_Weight_DC", value).numberbox("disable");

    $("#Bulk_Weight_TS", value).numberbox("disable");
    $("#Bulk_Weight_SK", value).numberbox("disable");
    $("#Bulk_Weight_DC", value).numberbox("disable");

    $("#Account_Weight_SK", value).numberbox("disable");
    $("#Account_Weight_DC", value).numberbox("disable");

    $("#Warehouse_Code", value).textbox("disable");
    $("#RK_Date", value).datebox("disable");
    $("#CK_Date", value).datebox("disable");
    $("#CH_Name", value).textbox("disable");

    $("#Shipper_M", value).textbox("disable");
    $("#Consignee_M", value).textbox("disable");
    $("#Notify_Part_M", value).textbox("disable");
    $("#Shipper_H", value).textbox("disable");
    $("#Consignee_H", value).textbox("disable");
    $("#Notify_Part_H", value).textbox("disable");
    $("#ADDTS", value).textbox("disable");

    var Bulk_Percent_SK = $("#Bulk_Percent_SK", value).numberbox("getValue");
    if (typeof (Bulk_Percent_SK) === "unddfined" || Bulk_Percent_SK == null || Bulk_Percent_SK == "") {
        $("#Bulk_Percent_SK", value).numberbox("setValue", 100);
    }
    var Bulk_Percent_DC = $("#Bulk_Percent_DC", value).numberbox("getValue");
    if (typeof (Bulk_Percent_DC) === "unddfined" || Bulk_Percent_DC == null || Bulk_Percent_DC == "") {
        $("#Bulk_Percent_DC", value).numberbox("setValue", 0);
    }
    var Bulk_Percent_Fact = $("#Bulk_Percent_Fact", value).numberbox("getValue");
    if (typeof (Bulk_Percent_Fact) === "unddfined" || Bulk_Percent_Fact == null || Bulk_Percent_Fact == "") {
        $("#Bulk_Percent_Fact", value).numberbox("setValue", 0);
    }
    //分担运费 初始加载处理
    loading_AS_Arranged(value);
    //绑定下拉数据源
    initbindcombogriddata(value);
    is_binding = is_bindingdata(value);
    //initPage();//初始化页面
    //initWTLoading(value);//初始化委托和分单信息页面
    initEasyUIControl();//回车下一个控件

    nowtabid = value;
    //设置ComboGrid真实url(为了不页面进入后 不立即加载数据，优化页面载入速度)
    resetCombogridUrl(value, "url1", false, FromDataWTXX);
    TS_NUM(value);//托书，件数、毛重、体积
    IS_MoorLevel(value);
    $("#IS_MoorLevel", value).on('click', function () {
        IS_MoorLevel(value);
    });
    $("#BL", value).on('click', function () {
        is_BL(value);
    });
    $("#Is_Self", value).on("click", function () {
        isSelf(value);
    });
    ///委托数据向下同步绑定 航空公司 启运港 目的港 航班号 
    //ADEA();
    //委托数据向下同步绑定 收货人 发货人 通知人 
    SCN_MH();
}
var nowtabid = "";
var is_binding = "0";//是否绑定进仓0 :否；1：是; 2:已经绑定好了；3：取消绑定
//自营
function isSelf(value) {
    if ($("#Is_Self", value).prop("checked")) {
        var tabs = $("#tt").tabs("tabs");
        if (tabs.length > 4) {
            $.messager.alert("提示", "当前有多个分单号，不能选择自营业务！");
            $("#Is_Self", value).prop("checked", false); return;
        } else {
            var $operationid = $("#Operation_Id", value).textbox("getValue");
            $("#HBL", value).textbox("setValue", $operationid);
            $("#HBL", "#fdxx_1").textbox("setValue", $operationid);
        }
    } else {
        $("#HBL", value).textbox("setValue", "");
        $("#HBL", "#fdxx_1").textbox("setValue", "");
    }
    //分单号 同步修改
    IsSelfAutoSetHBL();
}
//初始化委托和分单信息页面
function initWTLoading(idwt) {
    var value = $("#MBL", "#zdxx").get(0)("getValue");
    $("#MBL", idwt)[item.type]("setValue", value);
    $.each(domejsons, function (index, item) {
        switch (item.num) {
            case 2:
                var value = $(item.domid, "#zdxx")[item.type]("getValue");
                if (idwt.indexOf(item.targetid1) != -1) {
                    $(item.domid, idwt)[item.type]("setValue", value);
                }
                break;
        }
    });
}
//委托-托书，件数、毛重、体积 计算计费重量，分泡比
function TS_NUM(valueid) {
    $("input[id$='_TS'],input[id$='_SK'],input[id$='_DC'],input[id$='_Fact']").each(function () {
        if ($(this).attr("id").indexOf('Account_Weight_') < 0) {
            var data = $(this).data();
            data.valueid = valueid;
            data.numberbox.options.onChange = PJMVonChange;
            data.textbox.options.onChange = PJMVonChange;
        }
    });
}
//计算分泡比
function PJMVonChange() {
    var $this = $(this);
    var data = $this.data();
    var valueid = data.valueid;
    var $this_id = $this.attr('id');
    var lasttxt = $this_id.substr($this_id.length - 3);
    var lasttxt2 = $this_id.substr($this_id.length - 5);
    if (lasttxt == "Account_Weight") {
        return;
    }
    if (lasttxt2 == "_Fact") {
        lasttxt = lasttxt2;
    }
    var $Volume_TS = $("#Volume" + lasttxt, valueid).numberbox("getValue");//体积 
    var $Charge_Weight_TS = 0;//计费重量
    if ($Volume_TS != null && $Volume_TS > 0) {
        $Charge_Weight_TS = parseFloat(($Volume_TS / 0.006).toFixed(2));
    }
    var $Weight_TS = $("#Weight" + lasttxt, valueid).numberbox("getValue");//重量
    if ($Weight_TS != null && $Weight_TS != "") {
        if ($Charge_Weight_TS < $Weight_TS) {
            $Charge_Weight_TS = $Weight_TS;
        }
        if ($Charge_Weight_TS == 0) {
            $Charge_Weight_TS = $Weight_TS;
        }
    }
    var IsDCZD = false;//是否订舱主单
    if (lasttxt == "_DC") {
        IsDCZD = true;
        $("#Charge_Weight_M", "#zdxx").numberbox("setValue", $Charge_Weight_TS);
    }
    $("#Charge_Weight" + lasttxt, valueid).numberbox("setValue", $Charge_Weight_TS);//计费重量
    var $Bulk_Weight = $Charge_Weight_TS - $Weight_TS;
    $("#Bulk_Weight" + lasttxt, valueid).numberbox("setValue", $Bulk_Weight);//泡重
    if (lasttxt == "_SK" || lasttxt == "_DC" || lasttxt == "_Fact") {
        var Is_BAS = $("#Is_BAS").prop("checked");//BSA
        var IS_MoorLevel = $("#IS_MoorLevel").prop("checked");//靠级
        var $Bulk_Percent_SK = $("#Bulk_Percent" + lasttxt, valueid).numberbox("getValue");//分泡比
        if (ObjectIsEmpty($Bulk_Percent_SK))
            $Bulk_Percent_SK = 0;
        else {
            if (isNaN($Bulk_Percent_SK))
                $Bulk_Percent_SK = 0;
            else {
                $Bulk_Percent_SK = parseFloat($Bulk_Percent_SK);
                $Bulk_Percent_SK = $Bulk_Percent_SK * 0.01;//计算出分泡比
            }
        }
        if (lasttxt != "_DC" || (!Is_BAS && !IS_MoorLevel)) {
            if ($Charge_Weight_TS > $Weight_TS) {
                $("#Account_Weight" + lasttxt, valueid).numberbox("setValue", (($Bulk_Weight * $Bulk_Percent_SK) + parseInt($Weight_TS)));
            } else {
                $("#Account_Weight" + lasttxt, valueid).numberbox("setValue", $Charge_Weight_TS);
            }
        } else {
            //订舱主单 计算结算重量
            var Book_Flat_Code = $("#Book_Flat_Code").combogrid("getValue").toUpperCase();//订舱方
            var ArrBook_Flat_Code = ["GTHKG01", "CRHKG01"];
            if (Is_BAS) {
                $("#Account_Weight" + lasttxt, valueid).numberbox("setValue", $Weight_TS);
            } else if (IS_MoorLevel) {
                var MoorLevel = $("#MoorLevel").combogrid("getValue");//靠级 重量
                if (ObjectIsEmpty(MoorLevel)) {
                    MoorLevel = 0;
                } else if (!isNaN(MoorLevel)) {
                    MoorLevel = parseFloat(MoorLevel);
                }
                if ($.inArray(Book_Flat_Code, ArrBook_Flat_Code) >= 0) {
                    $("#Account_Weight" + lasttxt, valueid).numberbox("setValue", MoorLevel);
                } else {
                    //(计费重量-靠级重量) * 分泡比 + 靠级重量
                    var calcweight = $Charge_Weight_TS - MoorLevel;
                    if (calcweight <= 0)
                        calcweight = 0;
                    var calcweight = calcweight * $Bulk_Percent_SK + MoorLevel;
                    $("#Account_Weight" + lasttxt, valueid).numberbox("setValue", calcweight);
                }
            }
        }
    }
}
//委托信息二 
function synchrodata(firstdata, seconddata, thirddata, tabid) {
    var firPiecesVal = $("#Pieces" + firstdata, nowtabid).numberbox("getValue");
    var firWeightVal = $("#Weight" + firstdata, nowtabid).numberbox("getValue");
    var firVolumeVal = $("#Volume" + firstdata, nowtabid).numberbox("getValue");

    var $SecPieces = $("#Pieces" + seconddata, nowtabid);
    var $ThiPieces = $("#Pieces" + thirddata, nowtabid);
    var $SecWeight = $("#Weight" + seconddata, nowtabid);
    var $ThiWeight = $("#Weight" + thirddata, nowtabid);
    var $SecVolume = $("#Volume" + seconddata, nowtabid);
    var $ThiVolume = $("#Volume" + thirddata, nowtabid);

    $SecPieces.numberbox("setValue", firPiecesVal);
    $ThiPieces.numberbox("setValue", firPiecesVal);

    $SecWeight.numberbox("setValue", firWeightVal);
    $ThiWeight.numberbox("setValue", firWeightVal);

    $SecVolume.numberbox("setValue", firVolumeVal);
    $ThiVolume.numberbox("setValue", firVolumeVal);

    var $SecPieces_txt = $SecPieces.data().textbox.textbox.find("input.textbox-text");
    var $ThiPieces_txt = $ThiPieces.data().textbox.textbox.find("input.textbox-text");
    var $SecWeight_txt = $SecWeight.data().textbox.textbox.find("input.textbox-text");
    var $ThiWeight_txt = $ThiWeight.data().textbox.textbox.find("input.textbox-text");
    var $SecVolume_txt = $SecVolume.data().textbox.textbox.find("input.textbox-text");
    var $ThiVolume_txt = $ThiVolume.data().textbox.textbox.find("input.textbox-text");

    $SecPieces_txt.trigger('change');
    $ThiPieces_txt.trigger('change');
    $SecWeight_txt.trigger('change');
    $ThiWeight_txt.trigger('change');
    $SecVolume_txt.trigger('change');
    $ThiVolume_txt.trigger('change');
}
//分担运费 初始加载处理
function loading_AS_Arranged(value) {
    if ($("#Hbl_Feight", value).val() == "As_Arranged") {
        $("#As_Arranged", value).attr("checked", true);
        $("#Infact", value).attr("checked", false);
    } else if ($("#Hbl_Feight", value).val() == "Infact") {
        $("#As_Arranged", value).attr("checked", false);
        $("#Infact", value).attr("checked", true);
    } else {
        $("#As_Arranged", value).attr("checked", false);
        $("#Infact", value).attr("checked", false);
    }
}
//分担运费 单选框选中1 As_Arranged
function radioselect_As_Arranged(value) {
    $("#As_Arranged", value).attr("checked", true);
    $("#Infact", value).attr("checked", false);
    $("#Hbl_Feight", value).val("As_Arranged");
}
//分担运费 单选框选中2 Infact
function radioselect_Infact(value) {
    $("#Infact", value).attr("checked", true);
    $("#As_Arranged", value).attr("checked", false);
    $("#Hbl_Feight", value).val("Infact");
}
//是否绑定委托信息
function Is_Bindingwtxx() {
    return is_binding;
}
//取消绑定仓库进仓
function Is_Bindingcencel() {
    is_binding = "-1"; //取消绑定仓库进仓
}
//判断是否靠级
function IS_MoorLevel(value) {
    var is_moorlevel = $("#IS_MoorLevel", value).prop('checked');
    var moorlevel = $("#MoorLevel", value).combogrid("getValue");
    if (is_moorlevel) {
        $("#IS_MoorLevel", value).prop('checked', true);
        $("#MoorLevel", value).combogrid("enable");
    } else {
        $("#IS_MoorLevel", value).prop('checked', false);
        $("#MoorLevel", value).combogrid("disable");
        $("#MoorLevel", value).combogrid("setValue", "");
    }
}
//判断，是否有 总单号
function is_BL(value) {
    //if ($("#BL", value).prop('checked')) {
    //    $("#MBL", value).textbox("enable");
    //} else {
    //    $("#MBL", value).textbox("disable");
    //}
    //$('#MBL', value).textbox('textbox').addClass('Mask')
}
//加载时绑定combogrid数据源
function initbindcombogriddata(value)
{
    //销售
    var $SallerId = $("#SallerId", value);
    if ($SallerId.length > 0) {
        $SallerId.combogrid($.extend({}, combogrid_DefaultSettings, {
            url1: '/Sallers/GetPagerSallers_FromCache',
            columns: [[
                { field: 'ID', title: 'ID', width: 30 },
                { field: 'Code', title: '代码', width: 60 },
                { field: 'TEXT', title: '名称', width: 120 },
            ]],
            onChange: function (newValue, oldValue)
            {
                var $cbdg = $(this);
                var opts = $cbdg.combogrid('options');
                var textField = opts.textField;
                var Selted = $cbdg.combogrid('grid').datagrid('getSelected');
                if (Selted) {
                    $("#SallerName", value).val(Selted[textField]);
                }
            }
        }));
    }
    //接单类别
    var $Ty_Type = $("#Ty_Type", value);
    if ($Ty_Type.length > 0) {
        $Ty_Type.combogrid($.extend({}, combogrid_DefaultSettings, {
            url1: '/BD_DEFDOC_LISTs/Get_DEFDOC_DICT_FromCache?DOCCODE=Ty_Type',
            columns: [[
                { field: 'TEXT', title: '名称', width: 130 }
            ]]
        }));
        //keyup事件 小写转大写
        var $txt = $Ty_Type.combogrid("textbox");
        inputKeydown2UpperCase($txt);
    }
    //发货方
    var $Consign_Code = $("#Consign_Code", value);
    if ($Consign_Code.length > 0) {
        $Consign_Code.combogrid($.extend({}, combogrid_DefaultSettings, {
            url1: '/OPS_EntrustmentInfors/GetCusBusInfCode',
            onChange: function () {
                var data = $(this).combogrid("grid").datagrid("getSelected");
                if (data != null) {
                    //$("#Shipper_M", value).textbox("enable");
                    $("#Custom_Code", value).textbox("setValue", data.CustomsCode);
                    $("#Area_Code", value).textbox("setValue", data.AreaCode);
                    $("#Entrustment_Name", value).combogrid("setValue", data.ID);
                    $("#Entrustment_Name", value).combogrid("setText", data.TEXT);
                    //$("#Entrustment_Code", value).textbox("setValue", data.ID);
                    $("#Carriage_Account_Code", value).combogrid("setValue", data.ID);
                    $("#Carriage_Account_Code", value).combogrid("setText", data.TEXT);
                    if (!ObjectIsEmpty(data.SallerId))
                        $("#SallerId", value).combogrid('setValue', data.SallerId);
                    if (!ObjectIsEmpty(data.SallerName))
                        $("#SallerId", value).combogrid("setText", data.SallerName);
                } else {
                    //$("#Shipper_M", value).textbox("disable");
                    $("#Shipper_M", value).textbox("setValue", "");
                    $("#Custom_Code", value).textbox("setValue", "");
                    $("#Area_Code", value).textbox("setValue", "");
                    $("#Entrustment_Name", value).combogrid("setValue", "");
                    //$("#Entrustment_Code", value).textbox("setValue", "");
                    $("#Carriage_Account_Code", value).combogrid("setValue", "");
                }
            }
        }));
        //keyup事件 小写转大写
        var $txt = $Consign_Code.combogrid("textbox");
        inputKeydown2UpperCase($txt);
    }
    //委托方
    var $Entrustment_Name = $("#Entrustment_Name", value);
    if ($Entrustment_Name.length > 0) {
        $Entrustment_Name.combogrid($.extend({}, combogrid_DefaultSettings, {
            url1: '/CusBusInfos/GetPagerCusBusInfos_FromCache',
            onChange: function ()
            {
                var Seltdata = $(this).combogrid("grid").datagrid("getSelected");
                if (Seltdata != null) {
                    if (!ObjectIsEmpty(Seltdata.SallerId)) 
                        $("#SallerId", value).combogrid('setValue', Seltdata.SallerId);
                    if (!ObjectIsEmpty(Seltdata.SallerName)) 
                        $("#SallerId", value).combogrid("setText", Seltdata.SallerName);
                }
            }
        }));
        //keyup事件 小写转大写
        var $txt = $Entrustment_Name.combogrid("textbox");
        inputKeydown2UpperCase($txt);
    }
    //收货方
    var $Consignee_Code = $("#Consignee_Code", value);
    if ($Consignee_Code.length > 0) {
        $("#Consignee_Code", value).combogrid($.extend({}, combogrid_DefaultSettings, {
            url1: '/CusBusInfos/GetPagerCusBusInfos_FromCache'
        }));
        //keyup事件 小写转大写
        var $txt = $Consignee_Code.combogrid("textbox");
        inputKeydown2UpperCase($txt);
    }
    //国外代理
    var $FWD_Code = $("#FWD_Code", value);
    if ($FWD_Code.length > 0) {
        $FWD_Code.combogrid($.extend({}, combogrid_DefaultSettings, {
            url1: '/CusBusInfos/GetPagerCusBusInfos_FromCache',
            onChange: function (newValue, oldValue) {
                var data = $(this).combogrid("grid").datagrid("getSelected");
                var $text = "";
                if (data != null) {
                    $text = data.TEXT;
                    $("input[id='Consignee_M']").each(function () {
                        $(this).textbox("setValue", data.AddressEng);
                    });
                } else {
                    $text = "";
                    $("input[id='Consignee_M']").each(function () {
                        $(this).textbox("setValue", "");
                    });
                }
                $("input[textboxname='FWD_Code'].easyui-combogrid").each(function (k, item) {
                    var txt = $(item).combogrid('getText');
                    var val = $(item).combogrid('getValue');
                    if (txt == $text || newValue == val)
                        return;
                    $(item).combogrid('setValue', newValue);
                    $(item).combogrid('setText', $text);
                });
            }
        }));
        //keyup事件 小写转大写
        var $txt = $FWD_Code.combogrid("textbox");
        inputKeydown2UpperCase($txt);
    }
    //运费结算方
    var $Carriage_Account_Code = $("#Carriage_Account_Code", value);
    if ($Carriage_Account_Code.length > 0) {
        $Carriage_Account_Code.combogrid($.extend({}, combogrid_DefaultSettings, {
            url1: '/CusBusInfos/GetPagerCusBusInfos_FromCache'
        }));
        //keyup事件 小写转大写
        var $txt = $Carriage_Account_Code.combogrid("textbox");
        inputKeydown2UpperCase($txt);
    }
    //杂费结算方
    var $Incidental_Account_Code = $("#Incidental_Account_Code", value);
    if ($Incidental_Account_Code.length > 0) {
        $Incidental_Account_Code.combogrid($.extend({}, combogrid_DefaultSettings, {
            url1: '/CusBusInfos/GetPagerCusBusInfos_FromCache'
        }));
        //keydown事件 小写转大写
        var $txt = $Incidental_Account_Code.combogrid("textbox");
        inputKeydown2UpperCase($txt);
    }
    //靠级  
    var $MoorLevel = $("#MoorLevel", value);
    if ($MoorLevel.length > 0) {
        $MoorLevel.combogrid($.extend({}, combogrid_DefaultSettings, {
            url1: '/BD_DEFDOC_LISTs/Get_DEFDOC_DICT_FromCache?DOCCODE=MoorLevel',
            onChange: function (newValue, oldVlue) {
                //$("#Account_Weight_DC", value).numberbox("setValue", newValue);
                PJMVonChange.call($("#Charge_Weight_DC"));//触发重新计算 计费重量/结算重量
            }
        }));
        //keydown事件 小写转大写
        var $txt = $MoorLevel.combogrid("textbox");
        inputKeydown2UpperCase($txt);
    }
    //订舱方
    var $Book_Flat_Code = $("#Book_Flat_Code", value);
    if ($Book_Flat_Code.length > 0) {
        $Book_Flat_Code.combogrid($.extend({}, combogrid_DefaultSettings, {
            url1: '/CusBusInfos/GetPagerCusBusInfos_FromCache',
            onChange: function () {
                var $Pieces_DC = $("#Pieces_DC");
                var data = $Pieces_DC.data();
                if (data) {
                    if (data.numberbox)
                        data.numberbox.options.onChange.call($Pieces_DC);
                }
            }
        }));
        //keydown事件 小写转大写
        var $txt = $Book_Flat_Code.combogrid("textbox");
        inputKeydown2UpperCase($txt);
    }
    //航空公司
    var $Airways_Code = $("#Airways_Code", value);
    if ($Airways_Code.length > 0) {
        $Airways_Code.combogrid($.extend({}, combogrid_DefaultSettings, {
            url1: '/CusBusInfos/GetPagerCusBusInfos_FromCache',
            onChange: function (newValue, oldValue) {
                SetSameValcombogrid_onChange("#Airways_Code", value, newValue);
                var Seltdata = $(this).combogrid("grid").datagrid("getSelected");
                if (Seltdata != null) {
                    if (!ObjectIsEmpty(Seltdata.Delivery_PointCode))
                        $("#Delivery_Point", value).combogrid('setValue', Seltdata.Delivery_PointCode);
                    else
                        $("#Delivery_Point", value).combogrid('setValue', "");
                    if (!ObjectIsEmpty(Seltdata.Delivery_PointName))
                        $("#Delivery_Point", value).combogrid("setText", Seltdata.Delivery_PointName);
                    else
                        $("#Delivery_Point", value).combogrid("setText", "");
                }
            }
        }));
        //keydown事件 小写转大写
        var $txt = $Airways_Code.combogrid("textbox");
        inputKeydown2UpperCase($txt);
    }
    //启运港
    var $Depart_Port = $("#Depart_Port", value);
    if ($Depart_Port.length > 0) {
        $Depart_Port.combogrid($.extend({}, combogrid_DefaultSettings, {
            textField: 'IDTEXT',
            url1: '/PARA_AirPorts/GetPARA_AirPorts_FromCache',
            onChange: function (newValue, oldValue) {
                SetSameValcombogrid_onChange("#Depart_Port", value, newValue);
            }
        }));
        //keydown事件 小写转大写
        var $txt = $Depart_Port.combogrid("textbox");
        inputKeydown2UpperCase($txt);
    }
    //中转港
    var $Transfer_Port = $("#Transfer_Port", value);
    if ($Transfer_Port.length > 0) {
        $Transfer_Port.combogrid($.extend({}, combogrid_DefaultSettings, {
            textField: 'IDTEXT',
            url1: '/PARA_AirPorts/GetPARA_AirPorts_FromCache'
        }));
        //keydown事件 小写转大写
        var $txt = $Transfer_Port.combogrid("textbox");
        inputKeydown2UpperCase($txt);
    }
    //目的港
    var $End_Port = $("#End_Port", value);
    if ($End_Port.length > 0) {
        $End_Port.combogrid($.extend({}, combogrid_DefaultSettings, {
            textField: 'IDTEXT',
            url1: '/PARA_AirPorts/GetPARA_AirPorts_FromCache',
            onChange: function (newValue, oldValue) {
                SetSameValcombogrid_onChange("#End_Port", value, newValue);
            }
        }));
        //keydown事件 小写转大写
        var $txt = $End_Port.combogrid("textbox");
        inputKeydown2UpperCase($txt);
    }
    //航班号
    var $Flight_No = $("#Flight_No", value);
    if ($Flight_No.length > 0) {
        $Flight_No.combogrid($.extend({}, combogrid_DefaultSettings, {
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
        //keydown事件 小写转大写
        var $txt = $Airways_Code.combogrid("textbox");
        inputKeydown2UpperCase($txt);
    }
    //交货地址
    var $Delivery_Point = $("#Delivery_Point", value);
    if ($Delivery_Point.length > 0) {
        $Delivery_Point.combogrid($.extend({}, combogrid_DefaultSettings, {
            url1: '/BD_DEFDOC_LISTs/Get_DEFDOC_DICT_FromCache?DOCCODE=Delivery_Point'
        }));
        //keydown事件 小写转大写
        var $txt = $Delivery_Point.combogrid("textbox");
        inputKeydown2UpperCase($txt);
    }
}
//验证货到齐按钮是否可以点击
//function valadataHDQ(tabsId) {
//    var is_gl = is_bindingdata(tabsId);
//    if (is_gl == "0") {
//        if ($(this).prop("checked"))
//            $(this).prop("checked", false);
//        //$.messager.alert("提示", "有实际数据为空，不能选中货到齐");
//        return false;
//    }
//}
//判断 实际进仓 是否有数据
function is_bindingdata(tabsId) {
    if (tabsId.indexOf("#") < 0)
        tabsId = "#" + tabsId;
    var is_gl = "2";
    var $Pieces_Fact = $("#Pieces_Fact", tabsId).numberbox("getValue");
    if ($Pieces_Fact == null || $Pieces_Fact == "" || $Pieces_Fact == "0") {
        is_gl = "0";
        return is_gl;
    }
    var $Weight_Fact = $("#Weight_Fact", tabsId).numberbox("getValue")
    if ($Weight_Fact == null || $Weight_Fact == "" || $Weight_Fact == "0") {
        is_gl = "0";
        return is_gl;
    }
    var $Volume_Fact = $("#Volume_Fact", tabsId).numberbox("getValue");
    if ($Volume_Fact == null || $Volume_Fact == "" || $Volume_Fact == "0.000") {
        is_gl = "0";
        return is_gl;
    }
    return is_gl;
}
//获取仓库接单明细表中的长宽高件数，导入到备注中Pieces_Fact
function remarkemport(tabsId) {
    var is_gl = is_bindingdata(tabsId);
    if (is_gl == "0") {
        $.messager.alert("提示", "该委托没有绑定仓库接单，请先绑定仓库接单！");
        return;
    }
    $.ajax({
        url: '/OPS_EntrustmentInfors/GetLWGP?ID=' + $("#Id", "#zdxx").val() + '&MBL=' + $("#MBL", tabsId).val() + '&HBL=' + $("#HBL", tabsId).textbox("getValue"),
        type: "POST",
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        success: function (result) {
            if (result.Success) {
                var oldRemark = $("#Remark").textbox("getValue");
                var Nidx = oldRemark.indexOf('\n');
                var FirstStr = oldRemark;
                var LastStr = "";
                if (Nidx >= 0) {
                    FirstStr = oldRemark.substr(0, Nidx);
                    LastStr = oldRemark.substr(Nidx);
                }
                var len = FirstStr.length;
                var Num = 0;
                for (var i = 0; i < len; i++) {
                    var char = FirstStr.charAt(i);
                    if (char == "*") Num++;
                    if (Num >= 3)
                        break;
                }
                if (Num >= 3)
                    oldRemark = result.ReturnRemark + LastStr;
                else
                    oldRemark = result.ReturnRemark + "\n" + (Nidx >= 0 ? FirstStr + LastStr : oldRemark);

                $("#Remark", tabsId).textbox("setValue", oldRemark);
                $.messager.alert("提示", "导入成功！");
            } else {
                $.messager.alert("提示", result.ErrMsg);
            }
        },
        error: function (result) {

        }
    });
}
//计算泡重、计费重量、毛重
function calculateWeight() {
    $("#Bulk_Weight_CK").numberbox("setValue", $("#CHARGE_WEIGHT_CK").numberbox("getValue") - $("#Weight_CK").numberbox("getValue"));
    if ($("#Bulk_Weight_CK").numberbox("getValue") < 0) {
        $("#Bulk_Weight_CK").numberbox("setValue", 0);
        $("#CHARGE_WEIGHT_CK").numberbox("setValue", $("#Weight_CK").numberbox("getValue"));
    }
}
//体积计算泡重
function volumecalculateWeight() {
    var weight = $("#Volume_CK").numberbox("getValue") / 0.006
    if (weight > $("#Weight_CK").numberbox("getValue")) {//计算值大于实际毛重，计费重量等于计算值
        $("#CHARGE_WEIGHT_CK").numberbox("setValue", weight);
    } else {//计算值大于实际毛重，计费重量等于实际毛重
        $("#CHARGE_WEIGHT_CK").numberbox("setValue", $("#Weight_CK").numberbox("getValue"));
    }
    calculateWeight();
}
var CurTab = "";
//委托点击绑定 弹出仓库接单数据选择框
function bindingwarehouse(is_binding, pageX, pageY) {
    var tabsId = "#wtxx_1";
    var index = 1;
    var is_gl = is_bindingdata(tabsId);
    var $Entrustment_Code = $("#Entrustment_Code", tabsId).textbox("getValue");

    var $win = "#win_" + index;
    if (is_binding) {
        //$($win).window("options").title = "绑定仓库接单信息";
        $($win).panel({ title: "绑定仓库接单信息" })
    } else {
        //$($win).window("options").title = "取消绑定仓库接单信息";
        $($win).panel({ title: "取消绑定仓库接单信息" })
    }
    $($win).window("panel").left = pageX;
    $($win).window("panel").top = pageY;
    searchf_M(tabsId, $win, is_binding, $Entrustment_Code);
    $($win).window('open');
    $($win).window("center");
}
//获取委托信息的所有栏位的值
function getEntruValue(tabsid) {
    //委托表信息
    var newEntrustmentInfor = {
        Id: $("#Id", tabsid).val(),
        Operation_Id: $("#Operation_Id", tabsid).textbox("getValue"),//委托编号
        Consign_Code: $("#Consign_Code", tabsid).combogrid("getValue"),
        Custom_Code: $("#Custom_Code", tabsid).textbox("getValue"),
        Area_Code: $("#Area_Code", tabsid).textbox("getValue"),
        Entrustment_Name: $("#Entrustment_Name", tabsid).combogrid("getValue"),
        Entrustment_Code: $("#Entrustment_Code", tabsid).textbox("getValue"),
        FWD_Code: $("#FWD_Code", tabsid).combogrid("getValue"),
        Consignee_Code: $("#Consignee_Code", tabsid).combogrid("getValue"),
        Carriage_Account_Code: $("#Carriage_Account_Code", tabsid).combogrid("getValue"),
        Incidental_Account_Code: $("#Incidental_Account_Code", tabsid).combogrid("getValue"),
        Depart_Port: $("#Depart_Port", tabsid).combogrid("getValue"),
        Transfer_Port: $("#Transfer_Port", tabsid).combogrid("getValue"),
        End_Port: $("#End_Port", tabsid).combogrid("getValue"),
        Shipper_H: $("#Shipper_H", tabsid).textbox("getValue"),
        Consignee_H: $("#Consignee_H", tabsid).textbox("getValue"),
        Notify_Part_H: $("#Notify_Part_H", tabsid).textbox("getValue"),
        Shipper_M: $("#Shipper_M", tabsid).textbox("getValue"),
        Consignee_M: $("#Consignee_M", tabsid).textbox("getValue"),
        Notify_Part_M: $("#Notify_Part_M", tabsid).textbox("getValue"),
        Pieces_TS: $("#Pieces_TS", tabsid).numberbox("getValue"),
        Weight_TS: $("#Weight_TS", tabsid).numberbox("getValue"),
        Pieces_SK: $("#Pieces_SK", tabsid).numberbox("getValue"),
        Slac_SK: $("#Slac_SK", tabsid).numberbox("getValue"),
        Weight_SK: $("#Weight_SK", tabsid).numberbox("getValue"),
        Pieces_DC: $("#Pieces_DC", tabsid).numberbox("getValue"),
        Slac_DC: $("#Slac_DC", tabsid).numberbox("getValue"),
        Weight_DC: $("#Weight_DC", tabsid).numberbox("getValue"),
        Pieces_Fact: $("#Pieces_Fact", tabsid).numberbox("getValue"),
        Weight_Fact: $("#Weight_Fact", tabsid).numberbox("getValue"),
        IS_MoorLevel: $("#IS_MoorLevel", tabsid).get(0).checked,
        MoorLevel: $("#MoorLevel", tabsid).combogrid("getValue"),
        Volume_TS: $("#Volume_TS", tabsid).numberbox("getValue"),
        Charge_Weight_TS: $("#Charge_Weight_TS", tabsid).numberbox("getValue"),
        Bulk_Weight_TS: $("#Bulk_Weight_TS", tabsid).numberbox("getValue"),
        Volume_SK: $("#Volume_SK", tabsid).numberbox("getValue"),
        Charge_Weight_SK: $("#Charge_Weight_SK", tabsid).numberbox("getValue"),
        Bulk_Weight_SK: $("#Bulk_Weight_SK", tabsid).numberbox("getValue"),
        Bulk_Percent_SK: $("#Bulk_Percent_SK", tabsid).numberbox("getValue"),
        Account_Weight_SK: $("#Account_Weight_SK", tabsid).numberbox("getValue"),
        Volume_DC: $("#Volume_DC", tabsid).numberbox("getValue"),
        Charge_Weight_DC: $("#Charge_Weight_DC", tabsid).numberbox("getValue"),
        Bulk_Weight_DC: $("#Bulk_Weight_DC", tabsid).numberbox("getValue"),
        Bulk_Percent_DC: $("#Bulk_Percent_DC", tabsid).numberbox("getValue"),
        Account_Weight_DC: $("#Account_Weight_DC", tabsid).numberbox("getValue"),
        Volume_Fact: $("#Volume_Fact", tabsid).numberbox("getValue"),
        Charge_Weight_Fact: $("#Charge_Weight_Fact", tabsid).numberbox("getValue"),
        Bulk_Weight_Fact: $("#Bulk_Weight_Fact", tabsid).numberbox("getValue"),
        Bulk_Percent_Fact: $("#Bulk_Percent_Fact", tabsid).numberbox("getValue"),
        Account_Weight_Fact: $("#Account_Weight_Fact", tabsid).numberbox("getValue"),
        Marks_H: $("#Marks_H", tabsid).val(),
        //EN_Name_H: $("#EN_Name_H",tabsid).textbox("getValue"),
        Is_Book_Flat: $("#Is_Book_Flat", tabsid).get(0).checked,
        Book_Flat_Code: $("#Book_Flat_Code", tabsid).combogrid("getValue"),
        Airways_Code: $("#Airways_Code", tabsid).combogrid("getValue"),
        Flight_No: $("#Flight_No", tabsid).combogrid("getValue"),
        MBL: $("#MBL", tabsid).val(),
        HBL: $("#HBL", tabsid).textbox("getValue"),
        Flight_Date_Want: $("#Flight_Date_Want", tabsid).datebox("getValue"),
        Book_Remark: $("#Book_Remark", tabsid).textbox("getValue"),
        Remark: $("#Remark", tabsid).textbox("getValue"),
        RK_Date: $("#RK_Date", tabsid).datebox("getValue"),
        CK_Date: $("#CK_Date", tabsid).datebox("getValue"),
        CH_Name: $("#CH_Name", tabsid).textbox("getValue"),
        AMS: $("#AMS", tabsid).numberbox("getValue"),
        Lot_No: $("#Lot_No", tabsid).val(),
        Is_Self: $("#Is_Self", tabsid).get(0).checked,
        Ty_Type: $("#Ty_Type", tabsid).combogrid("getValue"),
        Hbl_Feight: $("#Hbl_Feight", tabsid).val(),
        Is_XC: $("#Is_XC", tabsid).get(0).checked,
        Is_BAS: $("#Is_BAS", tabsid).get(0).checked,
        Is_DCZ: $("#Is_DCZ", tabsid).get(0).checked,
        Is_ZB: $("#Is_ZB", tabsid).get(0).checked,
        Is_HDQ: $("#Is_HDQ", tabsid).prop("checked"),
        Is_TG: $("#Is_TG", tabsid).prop("checked"),
        Delivery_Point: $("#Delivery_Point", tabsid).combogrid("getValue"),
        Batch_Num: $("#Batch_Num", tabsid).val(),
        SallerId: $("#SallerId", tabsid).combogrid("getValue"),
        SallerName: $("#SallerName", tabsid).val(),
    };
    //console.log("Is_HDQ:", $("#Is_HDQ", tabsid).prop("checked"));

    return newEntrustmentInfor;
}