    $("#file_upload1").uploadifive({//uploadifive
        'height': 30,
        'fileTypeDesc': 'Image Files',
        'fileTypeExts': '*.gif; *.jpg; *.png; *.bmp;*.tif;*.doc;*.xls;*.zip',
        'uploadScript': urlpic + "&Talbe=Warehouse_receipt&type=DamageUpload",
        'buttonText': '上传',
        'multi': true,
        'displayData': 'speed',
        'formData': { 'modelType': 'Picture' },
        'width': 120,
        'successTimeout': 100000,
        'onFallback': function () {
            $.messager.alert('上传错误', '上传错误，请重试！<br>', 'error');
            $('#file_upload').uploadifive('clearQueue')
        },
        'onSelect': function (queue) {
            $.messager.progress({
                title: '执行上传',
                msg: '请等待...'
            });
        },
        'onUpload': function (file) {
            console.log("file:", file);
        },
        'onUploadComplete': function (file, data) {
            $.messager.progress('close');
            var data = $.parseJSON(data);
            if (data.Success === false) {
                $.messager.alert('上传错误', '上传错误，请修改后再上传！<br>' + data.ErrMsg, 'error');
                $("#file_upload").uploadifive('clearQueue')
            } else {
                //$.messager.alert('上传完成', '上传完成！<br>', 'info');
                //$(location).attr('href', "/Warehouse_receipts/Edit?id=" + $("#Id").val());
                var url = "/Warehouse_receipts/EditWindow?id=" + $("#Id").val();
                $("#receiptywindow").window('refresh', url); 
            }
            return true;
        }
    });

    $("#file_upload2").uploadifive({//uploadifive
        'height': 30,
        'fileTypeDesc': 'Image Files',
        'fileTypeExts': '*.gif; *.jpg; *.png; *.bmp;*.tif;*.doc;*.xls;*.zip',
        'uploadScript': urlpic + "&Talbe=Warehouse_receipt&type=DeliveryUpload",
        'buttonText': '上传',
        'multi': true,
        'displayData': 'speed',
        'formData': { 'modelType': 'Picture' },
        'width': 120,
        'successTimeout': 100000,
        'onFallback': function () {
            $.messager.alert('上传错误', '上传错误，请重试！<br>', 'error');
            $('#file_upload').uploadifive('clearQueue')
        },
        'onSelect': function (queue) {
            $.messager.progress({
                title: '执行上传',
                msg: '请等待...'
            });
        },
        'onUpload': function (file) {
            console.log("file:", file);
        },
        'onUploadComplete': function (file, data) {
            $.messager.progress('close');
            var data = $.parseJSON(data);
            if (data.Success === false) {
                $.messager.alert('上传错误', '上传错误，请修改后再上传！<br>' + data.ErrMsg, 'error');
                $("#file_upload").uploadifive('clearQueue')
            } else {
                //$.messager.alert('上传完成', '上传完成！<br>', 'info');
                //$(location).attr('href', "/Warehouse_receipts/Edit?id=" + $("#Id").val());
                var url = "/Warehouse_receipts/EditWindow?id=" + $("#Id").val();
                $("#receiptywindow").window('refresh', url); 
            }
            return true;
        }
    });
    $("#file_upload3").uploadifive({//uploadifive
        'height': 30,
        'fileTypeDesc': 'Image Files',
        'fileTypeExts': '*.gif; *.jpg; *.png; *.bmp;*.tif;*.doc;*.xls;*.zip',
        'uploadScript': urlpic + "&Talbe=Warehouse_receipt&type=EntryUpload",
        'buttonText': '上传',
        'multi': true,
        'displayData': 'speed',
        'formData': { 'modelType': 'Picture' },
        'width': 120,
        'successTimeout': 100000,
        'onFallback': function () {
            $.messager.alert('上传错误', '上传错误，请重试！<br>', 'error');
            $('#file_upload').uploadifive('clearQueue')
        },
        'onSelect': function (queue) {
            return;
            $.messager.progress({
                title: '执行上传',
                msg: '请等待...'
            });
        },
        'onUpload': function (file) {
            console.log("file:", file);
        },
        'onUploadComplete': function (file, data) {
            $.messager.progress('close');
            var data = $.parseJSON(data);
            if (data.Success === false) {
                $.messager.alert('上传错误', '上传错误，请修改后再上传！<br>' + data.ErrMsg, 'error');
                $("#file_upload").uploadifive('clearQueue')
            } else {
                //$.messager.alert('上传完成', '上传完成！<br>', 'info');
                //$(location).attr('href', "/Warehouse_receipts/Edit?id=" + $("#Id").val());
                var url = "/Warehouse_receipts/EditWindow?id=" + $("#Id").val();
                $("#receiptywindow").window('refresh', url); 
            }
            return true;
        }
    });

function initloading() {
    var intervalwt = setInterval(function () {
        var $ArrTxtDriver = $("input[name='Remark'].textbox-value", "#receiptywindow");
        if ($ArrTxtDriver.length == 1) {
            //initEasyUIControl();
            searchfdetail();
            initbindcombogriddata_H();
            //退出
            clearInterval(intervalwt);
        };
    }, 50);
}

//上传按钮点击事件
//$("#file_upload").click(function () {
//    var row = $dg.datagrid('getSelected');
//    console.log("row:", row);
//    if (row == null) {
//        $.messager.alert("提示对话框", "上传图片必须选择一条订单！", 'info'); return false;
//    }
//    id = row.id;
//});

//变形删除
function deleteDamagePicture() {
    deletePicture("&Talbe=Warehouse_receipt&type=DamageUpload");
}
//变形清除
function deleteEmptyDamagePicture() {
    deleteEmptyPicture("&Talbe=Warehouse_receipt&type=DamageUpload");
}

//交货删除
function deleteDeliveryPicture() {
    deletePicture("&Talbe=Warehouse_receipt&type=DeliveryUpload");
}
//交货清除
function deleteEmptyDeliveryPicture() {
    deleteEmptyPicture("&Talbe=Warehouse_receipt&type=DeliveryUpload");
}

//进仓删除
function deleteEntryPicture() {
    deletePicture("&Talbe=Warehouse_receipt&type=EntryUpload");
}
//进仓清除
function deleteEmptyEntryPicture() {
    deleteEmptyPicture("&Talbe=Warehouse_receipt&type=EntryUpload");
}

//删除该单已经上传的图片
function deletePicture(type) {
    $.ajax({
        url: '/Pictures/DeleteUpload?ID=' + $("#Id").val() + type,
        type: "POST",
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        success: function (result) {
            if (result.Success) {
                $.messager.alert("提示", "删除成功！");
                var url = "/Warehouse_receipts/EditWindow?id=" + $("#Id").val();
                $("#receiptywindow").window('refresh', url);
                //$(location).attr('href', "/Warehouse_receipts/Edit?id=" + $("#Id").val());
                //$(location).attr("href", "/Warehouse_receipts/Index")
            } else {
                $.messager.alert("提示", result.ErrMsg);
            }
        },
        error: function (result) {

        }
    });
}
//清空该单已经上传的图片
function deleteEmptyPicture(type) {
    $.ajax({
        url: '/Pictures/DeleteEmptyUpload?ID=' + $("#Id").val() + type,
        type: "POST",
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        success: function (result) {
            if (result.Success) {
                $.messager.alert("提示", "清空成功！");
                var url = "/Warehouse_receipts/EditWindow?id=" + $("#Id").val() + "&Warehouse_Id=" + $("#Warehouse_Id").textbox("getValue");
                $("#receiptywindow").window('refresh', url);
                //$(location).attr('href', "/Warehouse_receipts/Edit?id=" + $("#Id").val());
                //$(location).attr("href", "/Warehouse_receipts/Index")
                //var url = "/Warehouse_receipts/EditWindow?id=0&Warehouse_Id=" + $("#Warehouse_Id").textbox("getValue");
                //$("#receiptywindow").window('refresh', url);
            } else {
                $.messager.alert("提示", result.ErrMsg);
            }
        },
        error: function (result) {
            //
        }
    });
}
//返回
function cencelbutton() {
    $(location).attr("href", "/Warehouse_receipts/Index");
}
//
function saveandaddwarehouse(){
    if (endEditingDetail()) {
        var warehousereceipt = getwarehousereceipt();
        var warehousesize = getwarehousesize();
        $.ajax({
            url: '/Warehouse_receipts/SaveWarehouse',
            type: "POST",
            dataType: "json",
            contentType: "application/json; charset=utf-8",
            data: JSON.stringify({ WarehouseReceipt: warehousereceipt, warehouse_cargo_size: warehousesize }),//JSON.stringify()
            success: function (result) {
                if (result.success) {
                    //$.messager.alert("提示","新增成功！");
                    //$(location).attr("href", "/Warehouse_receipts/Index")
                    append();
                } else {
                    alert(result.err);
                }
            },
            error: function () {
                $.messager.alert("错误", "提交错误了！", 'error');
            },
            complete: function (xhr, status) {//请求完成时运行的函数（在请求成功或失败之后均调用，即在 success 和 error 函数之后）。
                $.messager.progress('close');
            }
        });
    }
}
//
function declareInvalidCurrent() {
    if ($("#Is_Binding").val() == "True") {
        $.messager.alert("提示", "仓库接单信息已经绑定，不能删除！");
        return;
    }
    var ids = [];
    ids.push($("#Id", "#receiptywindow").val());
    SaveDeclaredelete(ids, "delete");
    $("#receiptywindow").window("close");
}
//批量作废
function SaveDeclaredelete(ids, type) {
    var IDS = JSON.stringify(ids);
    var url = '/Warehouse_receipts/SaveDeclareInvalid?ids=' + ids + '&type=' + type;
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
                $.messager.alert("提示", data.ErrMsg, "info");
                $dg.datagrid('reload');
                $dg.datagrid('clearSelections');
            }
            else {
                var ErrMsgStr = '提交错误了！';
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
//
function savewarehouse() {
    if (endEditingDetail()) {
        if (!valadatapiece()) {
            $.messager.alert("提示", "仓库接单主表中的实际件数不等于明细表中的件数的总和！");
            return false;
        }
        var warehousereceipt = getwarehousereceipt();
        var warehousesize = getwarehousesize();
        $.ajax({
            url: '/Warehouse_receipts/SaveWarehouse',
            type: "POST",
            dataType: "json",
            contentType: "application/json; charset=utf-8",
            data: JSON.stringify({ WarehouseReceipt: warehousereceipt, warehouse_cargo_size: warehousesize }),//JSON.stringify()
            success: function (result) {
                if (result.success) {
                    //$.messager.alert("提示","新增成功！");
                    //$(location).attr("href", "/Warehouse_receipts/Index")
                    var url = "/Warehouse_receipts/EditWindow?id=" + result.Id + "&Warehouse_Id=" + result.Warehouse_Id;
                    $("#receiptywindow").window('refresh', url);
                    initloading();
                } else {
                    alert(result.err);
                }
            },
            error: function () {
                $.messager.alert("错误", "提交错误了！", 'error');
            },
            complete: function (xhr, status) {//请求完成时运行的函数（在请求成功或失败之后均调用，即在 success 和 error 函数之后）。
                $.messager.progress('close');
            }
        });
    }
}
