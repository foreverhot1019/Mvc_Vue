//明细
var $dgd = $('#warehouse_cargo_size_datagrid');
var editIndex = 1000;
//var $dgd = $('#warehouse_cargo_size_datagrid');
//var editIndex = undefined;
//setTimeout(searchfdetail,
//    2000);
//searchfdetail();

//统计总体积和总件数
function totalCount() {
    //if (endEditingDetail()) {
    //console.log("editIndex", editIndex);
    if (!(typeof editIndex === 'undefined' || editIndex == null || isNaN(editIndex))) {
        var CM_Length = 0;
        var CM_Width = 0;
        var CM_Height = 0;
        var CM_Piece = 0;
        var ed1 = $dgd.datagrid('getEditor', { index: editIndex, field: "CM_Length" });
        if (ed1 != null) {
            CM_Length = $(ed1.target).numberbox('getValue');
        }
        var ed2 = $dgd.datagrid('getEditor', { index: editIndex, field: "CM_Width" });
        if (ed2 != null) {
            CM_Width = $(ed2.target).numberbox('getValue');
        }
        var ed3 = $dgd.datagrid('getEditor', { index: editIndex, field: "CM_Height" });
        if (ed3 != null) {
            CM_Height = $(ed3.target).numberbox('getValue');
        }
        var ed4 = $dgd.datagrid('getEditor', { index: editIndex, field: "CM_Piece" });
        if (ed4 != null) {
            CM_Piece = $(ed4.target).numberbox('getValue');
        }

        var data = $dgd.datagrid("getRows");
        var totalvolume = 0;
        var totalnum = 0;
        $.each(data, function (index, item) {
            if (index == editIndex) {
                totalvolume += (CM_Length * CM_Width * CM_Height * CM_Piece);
                totalnum += (CM_Piece * 1);
            } else {
                if (!(typeof (item.CM_Length) === 'undefined' || item.CM_Length == null || item.CM_Length == "")) {
                    totalvolume += (item.CM_Length * item.CM_Width * item.CM_Height * item.CM_Piece);
                }
                if (!(typeof (item.CM_Piece) === 'undefined' || item.CM_Piece == null || item.CM_Piece == "")) {
                    
                    totalnum += (item.CM_Piece * 1);
                }
            }
        });
        totalvolume = totalvolume / 1000000;
        $("#Volume_CK", "#div_Warehouse_receipts").numberbox("setValue", totalvolume);
        $("#totalCount").text("总体积： " + totalvolume + "   件数:" + totalnum);
    }
    //}
}

//点击查询按钮时，触发
function searchfdetail() {
    setfilteroptsdetail();//设置查询条件
    var opts = $dgd.datagrid('options');
    opts.url = '/Warehouse_Cargo_Sizes/GetData';
    opts.pageNumber = 1;
    var pager = $dgd.datagrid('getPager');
    pager.pagination('refresh', {
        pageNumber: 1
    });
    $dgd.datagrid('reload');
}

var Entry_Id = "";

//设置查询条件
function setfilteroptsdetail() {
    var params = [];
    var opts = $dgd.datagrid('options');
    //Entry_Id = $("#Entry_Id").textbox("getValue");
    //if (!(typeof (Entry_Id) === 'undefined' || Entry_Id == null || Entry_Id == "")) {
    //    params.push({ "field": "Entry_Id", "op": "equal", "value": Entry_Id });
    //} else {
    //    params.push({ "field": "Entry_Id", "op": "equal", "value": "-1" });
    //}

    var Status = $("#Status").val();
    var warid = $("#Id").val();

    if (!(typeof (Status) === 'undefined' || Status == null || Status == "")) {
        params.push({ "field": "Status", "op": "equal", "value": Status });
    }

    if (!(typeof (warid) === 'undefined' || warid == null || warid == "")) {
        params.push({ "field": "Warehouse_Receipt_Id", "op": "equal", "value": warid });
    }

    opts.queryParams = {
        'filterRules': JSON.stringify(params)
    };
}
//
function valadatapiece() {
    var rows = $dgd.datagrid("getRows");
    var $totalPieces = 0;
    $.each(rows, function (index, row) {
        if (typeof (row.CM_Piece) !== 'undefined' && row.CM_Piece != null && row.CM_Piece != '' && !isNaN(row.CM_Piece)) {
            $totalPieces += parseInt(row.CM_Piece);
        }
    });
    var $piece = $("#Pieces_CK").numberbox("getValue");
    if (!isNaN($piece))
        $piece = parseInt($piece);
    if ($piece == $totalPieces) {
        return true;
    } else {
        return false;
    }
}
//
function getwarehousesize() {
    if ($dgd.datagrid('getChanges').length) {
        var inserted = $dgd.datagrid('getChanges', "inserted");
        var deleted = $dgd.datagrid('getChanges', "deleted");
        var updated = $dgd.datagrid('getChanges', "updated");
        var effectRow = new Object();

        if (inserted.length) {
            effectRow.inserted = inserted;
        }
        if (deleted.length) {
            effectRow.deleted = deleted; 
        }
        if (updated.length) {
            effectRow.updated = updated;
        }
        return effectRow;
    }
    return null;
    //var list = $dgd.datagrid("getChanges");
    //return list;
}
//
function removeitDetail() {
    var Seltdata = $dgd.datagrid('getSelections');
    if (!(typeof editIndex === 'undefined' || editIndex == null || isNaN(editIndex))) {
        $dgd.datagrid('cancelEdit', editIndex);
        editIndex = undefined;
    }
    for (var i = Seltdata.length; i > 0; i--) {
        var rowindex = $dgd.datagrid('getRowIndex', Seltdata[i - 1]);
        $dgd.datagrid('deleteRow', rowindex);
    }
}
//保存/下一行
function warehouse_cargo_size_accept() {
    var ArrED = $dgd.datagrid('getEditors', editIndex);
    var field = ArrED[0].field;
    var index = editIndex + 1;
    onClickCellDetail(index, field);
}
//开始编辑行时，赋值 联动数据
function onBeginEditDetail(rowIndex, rowData) {
    var num = 0;
    var regx = /^\/Date\([0-9]+(\)\/)$/g;
    for (var i in rowData) {
        if (regx.test(rowData[i])) {
            rowData[i] = datetimeformatter(rowData[i]);
        }
        var targetEditor = $dgd.datagrid('getEditor', {
            field: i,
            index: rowIndex
        });
        //console.log("targetEditor", i, targetEditor);
        if (targetEditor) {
            num++;
            if (num == 1 && $(targetEditor.target).val() == '') {
                $(targetEditor.target).parent().children("span:eq(0)").children("input:eq(0)").focus();
            }

            var OldVal = targetEditor.oldHtml;
            switch (targetEditor.type.toLowerCase()) {
                case "combogrid":
                    //console.log(targetEditor);
                    var combogrid_Setting = combogrid_Settings[targetEditor.field];
                    if (combogrid_Setting) {
                        $(targetEditor.target).combogrid(combogrid_Setting);
                    }
                    if (!(typeof (OldVal) === 'undefined' || OldVal == null || OldVal == '')) {
                        if (rowData[targetEditor.field]) {
                            OldVal = rowData[targetEditor.field];
                        }
                        $(targetEditor.target).combogrid('setValue', OldVal);
                        var opts = $(targetEditor.target).combogrid('options');
                        var url = opts.url;
                        //设置查询参数
                        var queryParams = {
                            page: 1,
                            rows: opts.pageSize,
                            q: OldVal
                        };
                        $(targetEditor.target).combogrid('grid').datagrid('load', queryParams);
                    }
                    break;
                case "combobox":
                    var opts = $(targetEditor.target).combobox({
                        inputEvents: $.extend({}, $.fn.combobox.defaults.inputEvents, {
                            keydown: function (event) {
                                dg_combobox_keydown(this, event, $dgd, rowIndex);
                            }
                        })
                    });
                    setTargetVal(targetEditor, OldVal);
                    break;
                case "textbox":
                case "numberbox":
                case "datebox":
                case "datetimebox":
                    var data = $(targetEditor.target).data();
                    var options = {
                        inputEvents: $.extend({}, $.fn.textbox.defaults.inputEvents, {
                            keydown: function (event) {
                                dg_Listtextbox_keydown(this, event, $dgd, rowIndex);
                            }
                        })
                    };
                    var $target = $(targetEditor.target);
                    $target.textbox(options);
                    setTargetVal(targetEditor, OldVal);
                    break;
                case "checkbox":
                    $(targetEditor.target).keydown(function (e) {
                        dg_chk_rdo_keydown(this, e, $dgd, rowIndex);
                    });
                    setTargetVal(targetEditor, OldVal);
                    break;
            }
        }
    }
}
//
function endEditingDetail() {
    if (editIndex == undefined)
        return true;
    if ($dgd.datagrid('validateRow', editIndex)) {
        //totalCount();
        $dgd.datagrid('endEdit', editIndex);
        editIndex = undefined;
        return true;
    } else {
        return false;
    }
}
//
function appendDetail() {
    if (endEditingDetail()) {
        var NewRow = {};
        var Columns = $dgd.datagrid('getColumnFields');
        for (var column in Columns) {
            NewRow[Columns[column]] = null;
        }
        ////动态设置 编辑样式
        //var byteColumn = $dgd.datagrid('getColumnOption', 'ADDWHO');
        //if (!(typeof (byteColumn) === 'undefined' || byteColumn == null || byteColumn == '')) {
        //    byteColumn.editor = {
        //        type: 'textbox',
        //        options: {
        //            required: false,
        //            validType: 'length[0,20]'
        //        }
        //    };
        //    byteColumn = $dgd.datagrid('getColumnOption', 'ADDTS');
        //    byteColumn.editor = {
        //        type: 'datebox',
        //        options: {
        //            required: false
        //        }
        //    };
        //    byteColumn = $dgd.datagrid('getColumnOption', 'EDITWHO');
        //    byteColumn.editor = {};
        //    byteColumn = $dgd.datagrid('getColumnOption', 'EDITTS');
        //    byteColumn.editor = {};
        //}
        $dgd.datagrid('insertRow', { index: 0, row: NewRow });
        editIndex = 0;
        $dgd.datagrid('selectRow', editIndex).datagrid('beginEdit', editIndex);
    }
}
//
function onClickCellDetail(index, field) {
    var _operates = ["_operate1", "_operate2", "_operate3", "ck"]
    if ($.inArray(field, _operates) >= 0) {
        return;
    }
    if (editIndex != index) {
        if (endEditingDetail()) {
            ////动态设置 编辑样式WarehouseSize
            //var byteColumn = $dgd.datagrid('getColumnOption', 'ADDWHO');
            //if (!(typeof (byteColumn) === 'undefined' || byteColumn == null || byteColumn == '')) {
            //    byteColumn.editor = {};
            //    byteColumn = $dgd.datagrid('getColumnOption', 'ADDTS');
            //    byteColumn.editor = {};
            //    var byteColumn = $dgd.datagrid('getColumnOption', 'EDITWHO');
            //    byteColumn.editor = {
            //        type: 'textbox',
            //        options: {
            //            required: false,
            //            validType: 'length[0,20]'
            //        }
            //    };
            //    byteColumn = $dgd.datagrid('getColumnOption', 'EDITTS');
            //    byteColumn.editor = {
            //        type: 'datebox',
            //        options: {
            //            required: false
            //        }
            //    };
            //}
            $dgd.datagrid('selectRow', index).datagrid('beginEdit', index);
            var ed = $dgd.datagrid('getEditor', { index: index, field: field });
            if (ed) {
                ($(ed.target).data('textbox') ? $(ed.target).textbox('textbox') : $(ed.target)).focus();
            }
            editIndex = index;
        } else {
            $dgd.datagrid('selectRow', editIndex);
        }
    }
}