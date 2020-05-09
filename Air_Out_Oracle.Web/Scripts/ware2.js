//setTimeout(initbindcombogriddata_H,
//    2000);
//initbindcombogriddata_H(); 
function initbindcombogriddata_H() {
    $("#Warehouse_Id").textbox("disable");
    $("#Damaged").textbox("disable");
    $("#Dampness").textbox("disable");
    $("#Deformation2").textbox("disable");
    $("#Closure_Remark").textbox("disable");
    $("#Warehouse_Remark").textbox("disable");
    $("#Volume_CK").textbox("disable");
    //仓库
    $("#Warehouse_Code").combogrid($.extend({}, combogrid_DefaultSettings, {
        value: "JGJW9",
        url: '/BD_DEFDOC_LISTs/GetPager_DEFDOC_DICT_FromCache?DOCCODE=WAREHOUSE',
    }));
    //包装
    $("#Packing").combogrid($.extend({}, combogrid_DefaultSettings, {
        url: '/Warehouse_receipts/GetPARA_Package',
    }));
    //委托方
    $("#Consign_Code_CK").combogrid($.extend({}, combogrid_DefaultSettings, {
        url1: '/CusBusInfos/GetPagerCusBusInfos_FromCache'
    }));
    //启运港
    $("#End_Port").combogrid($.extend({}, combogrid_DefaultSettings, {
        url1: '/PARA_AirPorts/GetPARA_AirPorts_FromCache',
    }));
    //航班号
    $("#FLIGHT_No").combogrid($.extend({}, combogrid_DefaultSettings, {
        url: '/PARA_AirLines/GetPagerAirLine_FromCache',
    }));

    is_Damaged_CK();
    $("#Damaged_CK").on('click', function () {
        is_Damaged_CK();
    });

    is_Dampness_CK();
    $("#Dampness_CK").on('click', function () {
        is_Dampness_CK();
    });
    is_Deformation();
    $("#Deformation").on('click', function () {
        is_Deformation();
    });
    is_GF();
    $("#Is_GF").on('click', function () {
        is_GF();
    });
    is_QG();
    $("#Is_QG").on('click', function () {
        is_QG();
    });
    //绑定onChange方法
    var domjsons = [
         { domid: '#Entry_Id', type: 'textbox', num: 4 },//进仓编号
         {domid: '#Damaged_Num',type: 'numberbox',num: 1},//破损
         {domid: '#Dampness_Num',type: 'numberbox',num: 1},//受潮
         {domid: '#Deformation_Num',type: 'numberbox',num: 1},//变形
         {domid: '#Closure_Remark',type: 'textbox',num: 1},//随货文件
         {domid: '#Warehouse_Remark',type: 'textbox',num: 1},//一体化
         {domid: '#Weight_CK',type: 'numberbox',num: 3},//毛重
         {domid: '#CHARGE_WEIGHT_CK',type: 'numberbox',num: 2},//计费重量
         {domid: '#Volume_CK',type: 'numberbox',num: 3},//体积
    ]
    binddome(domjsons);
    //设置ComboGrid真实url(为了不页面进入后 不立即加载数据，优化页面载入速度)
    resetCombogridUrl("#div_Warehouse_receipts", "url1", false, FromDataWTXX);
}

//判断，是否有 破损
function is_Damaged_CK() {
    if ($("#Damaged_CK").get(0).checked) {
        $("#Damaged_Num").numberbox("enable");
    } else {
        $("#Damaged_Num").numberbox("disable");
    }
}
//判断，是否有 受潮
function is_Dampness_CK() {
    if ($("#Dampness_CK").get(0).checked) {
        $("#Dampness_Num").numberbox("enable");
    } else {
        $("#Dampness_Num").numberbox("disable");
    }
}
//判断，是否有 变形
function is_Deformation() {
    if ($("#Deformation").get(0).checked) {
        $("#Deformation_Num").numberbox("enable");
    } else {
        $("#Deformation_Num").numberbox("disable");
    }
}
//判断，是否有 随货文件
function is_GF() {
    if ($("#Is_GF").get(0).checked) {
        $("#Closure_Remark").numberbox("enable");
    } else {
        $("#Closure_Remark").numberbox("disable");
    }
}
//判断，是否有 一体化
function is_QG() {
    if ($("#Is_QG").get(0).checked) {
        $("#Warehouse_Remark").numberbox("enable");
    } else {
        $("#Warehouse_Remark").numberbox("disable");
    }
}
//破损、受潮、变形、随货文件、一体化等备注文本框, 毛重、计费重量、体积等numberbox，绑定onchange方法
function binddome(domjsons) {
    $.each(domjsons, function (index, item) {
        switch (item.num) {
            case 1:
                $(item.domid)[item.type]({
                    onChange: function (newValue, oldValue) {
                        writeRemark();
                    }
                });
                break;
            case 2:
                $(item.domid)[item.type]({
                    onChange: function (newValue, oldValue) {
                        calculateWeight();
                    }
                });
                break;
            case 3:
                $(item.domid)[item.type]({
                    onChange: function (newValue, oldValue) {
                        volumecalculateWeight();
                    }
                });
                break;
            case 4:
                break;
        }
    });
}
//新增仓库接单时，通过进仓编号，绑定承揽接单信息中的委托的总单号或者分单号
function bindingops(e) {
    //if (e.keyCode == 13) {
    var entry_id = $("#Entry_Id").textbox("getValue");
        var id = $("#Id").val();
        //if (id > 0) {//id大于0（该仓库接单已经存在）时，退出
        //    return;
    //}
        //console.log("e", e);
        //console.log("entry_id", entry_id);
        if(entry_id == null || entry_id == "")
        {
            $.messager.alert("提示", "请输入进仓编号！"); return;
        }
        var is_binding = $("#Is_Binding").val();
        if (is_binding == "True" && id != 0) {
            //$.messager.alert("提示", "该进仓编号已经绑定承揽接单信息！");
            return;
        }
        var warehousereceipt = getwarehousereceipt();
        $.ajax({
            url: "/Warehouse_receipts/BindingOPS",
            type: "POST",
            datatype: "json",
            contentType: "application/json; charset=utf-8",
            data: JSON.stringify({ warehousereceipt: warehousereceipt }),
            success: function (result) {
                if (result.success) {
                    $("#Is_Binding").val("True");
                    $("#MBLId").val(result.data.MBLId);
                    $("#MBL").textbox("setValue", result.data.MBL);
                    $("#HBL").textbox("setValue", result.data.HBL);
                    $("#Consign_Code_CK").textbox("setValue", result.data.Consign_Code_CK);
                    $("#Flight_Date_Want").datebox("setValue", result.data.Flight_Date_Want);
                    $("#FLIGHT_No").textbox("setValue", result.data.FLIGHT_No);
                    $("#End_Port").textbox("setValue", result.data.End_Port);
                    //$.messager.alert("提示", "新增成功！");
                    //$(location).attr("href", "/Warehouse_receipts/Index")
                } else {
                    if (result.isempty) {
                        $.messager.alert("提示", result.ErrMsg);
                    }
                }
            },
            error: function (result) {

            }
        });
    //}
}
//填写备注文本框
function writeRemark() {
    var remark = "";
    if ($("#Damaged_Num").numberbox("getValue") != null && $("#Damaged_Num").numberbox("getValue") != "") {
        remark += "破损" + $("#Damaged_Num").numberbox("getValue") + "件;";
    }
    if ($("#Dampness_Num").numberbox("getValue") != null && $("#Dampness_Num").numberbox("getValue") != "") {
        remark += "受潮" + $("#Dampness_Num").numberbox("getValue") + "件;";
    }
    if ($("#Deformation_Num").numberbox("getValue") != null && $("#Deformation_Num").numberbox("getValue") != "") {
        remark += "变形" + $("#Deformation_Num").numberbox("getValue") + "件;";
    }
    if ($("#Closure_Remark").numberbox("getValue") != null && $("#Closure_Remark").numberbox("getValue") != "") {
        remark += "随货文件" + $("#Closure_Remark").textbox("getValue") + ";";
    }
    if ($("#Warehouse_Remark").numberbox("getValue") != null && $("#Warehouse_Remark").numberbox("getValue") != "") {
        remark += "一体化" + $("#Warehouse_Remark").textbox("getValue") + ";";
    }
    $("#Remark").textbox("setValue", remark);
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
//获取编辑的仓库接单主信息
function getwarehousereceipt() {
    return recipt = {
        Id: $("#Id").val(),
        MBLId: $("#MBLId", "#receiptywindow").val(),
        Warehouse_Id: $("#Warehouse_Id").textbox("getValue"),
        Entry_Id: $("#Entry_Id").textbox("getValue"),
        Warehouse_Code: $("#Warehouse_Code").textbox("getValue"),
        Pieces_CK: $("#Pieces_CK").numberbox("getValue"),
        Weight_CK: $("#Weight_CK").numberbox("getValue"),
        Volume_CK: $("#Volume_CK").numberbox("getValue"),
        Bulk_Weight_CK: $("#Bulk_Weight_CK").numberbox("getValue"),
        Packing: $("#Packing").combogrid("getValue"),
        CHARGE_WEIGHT_CK: $("#CHARGE_WEIGHT_CK").numberbox("getValue"),//计费重量
        Damaged_CK: $("#Damaged_CK").get(0).checked,
        Damaged_Num: $("#Damaged_Num").numberbox("getValue"),
        Dampness_CK: $("#Dampness_CK").get(0).checked,
        Dampness_Num: $("#Dampness_Num").numberbox("getValue"),
        Deformation: $("#Deformation").get(0).checked,
        Deformation_Num: $("#Deformation_Num").numberbox("getValue"),
        Is_GF: $("#Is_GF").get(0).checked,
        Closure_Remark: $("#Closure_Remark").textbox("getValue"),
        Is_QG: $("#Is_QG").get(0).checked,
        Warehouse_Remark: $("#Warehouse_Remark").textbox("getValue"),
        Consign_Code_CK: $("#Consign_Code_CK").combogrid("getValue"),
        MBL: $("#MBL").textbox("getValue"),
        HBL: $("#HBL").textbox("getValue"),
        Flight_Date_Want: $("#Flight_Date_Want").datebox("getValue"),
        FLIGHT_No: $("#FLIGHT_No").combogrid("getValue"),
        End_Port: $("#End_Port").combogrid("getValue"),
        In_Date: $("#In_Date").datebox("getValue"),
        In_Time: $("#In_Time").textbox("getValue"),//入库时间
        Out_Date: $("#Out_Date").datebox("getValue"),
        Out_Time: $("#Out_Time").textbox("getValue"),//出口时间
        CH_Name_CK: $("#CH_Name_CK").textbox("getValue"),
        Is_CustomerReturn: $("#Is_CustomerReturn").get(0).checked,
        Is_MyReturn: $("#Is_MyReturn").get(0).checked,
        Truck_Id: $("#Truck_Id").textbox("getValue"),
        Driver: $("#Driver").textbox("getValue"),
        Remark: $("#Remark").textbox("getValue"),
        Is_Binding: $("#Is_Binding").val(),
    };
}