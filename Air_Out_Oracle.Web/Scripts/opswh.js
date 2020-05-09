var UpperLevelId = "";//上一级，选项卡的ID
var binging = false;//是否已经绑定
var $winindex = "#win_1";
var $entrustment_Code = "";
var $dgw;
var editIndex = undefined;
//点击重置按钮时，触发，清空查询条件
function clearfrom() {
    $('#searchformwarehouse', $winindex).form('clear');
}
//点击查询按钮
function searchquery() {
    searchf_M(UpperLevelId, $winindex, binging, $entrustment_Code);
}
//点击查询按钮时，触发,
//当前按钮所属的上一级域的id , 
function searchf_M(tabsId, winindex, is_binding, entrustment_Code) {
    UpperLevelId = tabsId;
    binging = is_binding;
    $winindex = winindex;
    $entrustment_Code = entrustment_Code;
    $("#Entrustment_Code_warehouse").text($entrustment_Code);
    if (!binging) {
        $("#Relationbinding").hide();
        $("#Relationsave:last").show();
    } else {
        $("#Relationbinding").show();
        $("#Relationsave:last").hide();
    }
    $dgw = $('#Warehouse_receipt_datagrid', winindex);
    setfilteropts(is_binding);//设置查询条件
    var opts = $dgw.datagrid('options');
    opts.url = '/Warehouse_receipts/GetData';
    opts.pageNumber = 1;
    var pager = $dgw.datagrid('getPager');
    pager.pagination('refresh', {
        pageNumber: 1
    });
    $dgw.datagrid('reload');
    //removeit();
}
//设置查询条件
function setfilteropts(is_binding) {
    var _In_Date = $("#_In_Date").datebox("getValue");
    var In_Date_ = $("#In_Date_").datebox("getValue");
    var Entry_Id = $("#Entry_Id_Query").textbox("getValue");
    var Warehouse_Id = $("#Warehouse_Id_Query").textbox("getValue");

    var params = [];
    var opts = $dgw.datagrid('options');

    if (is_binding) {
        params.push({ "field": "Is_Binding", "op": "equal", "value": false });
    } else {
        params.push({ "field": "Is_Binding", "op": "equal", "value": true });
        params.push({ "field": "MBLId", "op": "equal", "value": $("#Id","#zdxx").val() });
    }
    params.push({ "field": "Status", "op": "equal", "value": "0" });
    params.push({ "field": "Is_CustomerReturn", "op": "equal", "value": false });

    if (!(typeof (_In_Date) === 'undefined' || _In_Date == null || _In_Date == "")) {
        params.push({ "field": "_In_Date", "op": "equal", "value": _In_Date });
    }
    if (!(typeof (In_Date_) === 'undefined' || In_Date_ == null || In_Date_ == "")) {
        params.push({ "field": "In_Date_", "op": "equal", "value": In_Date_ });
    }
    if (!(typeof (Entry_Id) === 'undefined' || Entry_Id == null || Entry_Id == "")) {
        params.push({ "field": "Entry_Id", "op": "equal", "value": Entry_Id });
    }
    if (!(typeof (Warehouse_Id) === 'undefined' || Warehouse_Id == null || Warehouse_Id == "")) {
        params.push({ "field": "Warehouse_Id", "op": "equal", "value": Warehouse_Id });
    }

    opts.queryParams = {
        'filterRules': JSON.stringify(params)
    }; 
}
//datagrid加载完数据后，触发
function onLoadSuccess_binding(data) {
    if (binging) {
        $dgw.datagrid("uncheckAll");
    } else {
        $dgw.datagrid("checkAll");
    }
}
//全部解绑
function AllRelation(value) {
    $("#win_" + value).window('close');
    is_bindingwarehouse = "3";
    $("#Pieces_Fact", UpperLevelId).numberbox("setValue", 0);
    $("#Weight_Fact", UpperLevelId).numberbox("setValue", 0);
    $("#Volume_Fact", UpperLevelId).numberbox("setValue", 0);
    //is_binding = item.Id;
    btnbindingshow(UpperLevelId);
}
//关联选择的仓库接单信息
function Relation(value) {
    var selectionitems = $dgw.datagrid("getSelections");
    if (selectionitems.length == 0) {
        $.messager.alert("提示", "请选择您需要关联的仓库信息！");
    }
    var Pieces_CK = 0;//存储仓库接单明细的件数
    var Weight_CK = 0;//存储仓库接单明细的毛重
    var Volume_CK = 0;//存储仓库接单明细的体积
    var arrid = $("#Marks_H", UpperLevelId).val();//获取当前选中的关联仓库ID
    var Pieces_Fact = $("#Pieces_Fact", UpperLevelId).numberbox("getValue");//获取当前承揽接单中 进仓实际件数
    var Weight_Fact = $("#Weight_Fact", UpperLevelId).numberbox("getValue");//获取当前承揽接单中 进仓实际毛重
    var Volume_Fact = $("#Volume_Fact", UpperLevelId).numberbox("getValue");//获取当前承揽接单中 进仓实际体积
    //获取当前选中的所有要绑定的仓库接单的件毛体数量总和
    $.each(selectionitems, function (index, item) {
        var arr = arrid.split(',');
        //var ss = $.inArray(item.Id, arr);  //返回 3,
        if ($.inArray(item.Id.toString(), arr) == -1) {
            if (arrid != "") {
                arrid = arrid + ",";
            }
            Pieces_CK = Pieces_CK + item.Pieces_CK;
            Weight_CK = Weight_CK + item.Weight_CK;
            Volume_CK = Volume_CK + item.Volume_CK;
            arrid = arrid + item.Id;
        }
    });

    if (binging) {
        if (!(typeof (Pieces_Fact) === 'undefined' || Pieces_Fact == null || Pieces_Fact == "")) {
            Pieces_Fact = parseFloat(Pieces_Fact) + parseFloat(Pieces_CK);
        } else {
            Pieces_Fact = parseFloat(Pieces_CK);
        }
        if (!(typeof (Weight_Fact) === 'undefined' || Weight_Fact == null || Weight_Fact == "")) {
            Weight_Fact = parseFloat(Weight_Fact) + parseFloat(Weight_CK);
        } else {
            Weight_Fact = parseFloat(Weight_CK);
        }
        if (!(typeof (Volume_Fact) === 'undefined' || Volume_Fact == null || Volume_Fact == "")) {
            Volume_Fact = parseFloat(Volume_Fact) + parseFloat(Volume_CK);
        } else {
            Volume_Fact = parseFloat(Volume_CK);
        }
    } else {
        Pieces_Fact = parseFloat(Pieces_CK);
        Weight_Fact = parseFloat(Weight_CK);
        Volume_Fact = parseFloat(Volume_CK);
    }
    $("#Pieces_Fact", UpperLevelId).numberbox("setValue", Pieces_Fact);
    $("#Weight_Fact", UpperLevelId).numberbox("setValue", Weight_Fact);
    $("#Volume_Fact", UpperLevelId).numberbox("setValue", Volume_Fact);
    $("#Marks_H", UpperLevelId).val(arrid);
    //$("#Lot_No", UpperLevelId).val(item.Id);
    if (arrid == null || arrid == "") {
        is_bindingwarehouse = "3";
    }
    //console.log($("#Lot_No", UpperLevelId).val(),"item.Id:",item.Id);
    $("#win_" + value).window('close');
    //is_binding = item.Id;
    btnbindingshow(UpperLevelId);
}
//
function getwarehousesize() {
    if ($dgw.datagrid('getChanges').length) {
        var inserted = $dgw.datagrid('getChanges', "inserted");
        var deleted = $dgw.datagrid('getChanges', "deleted");
        var updated = $dgw.datagrid('getChanges', "updated");
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
    //var list = $dgw.datagrid("getChanges");
    //return list;
}
//
function removeit() {
    if (arrid != "") {
        arrid = arrid + ",";
        var arr = arrid.split(',');
        var Seltdata = $dgw.datagrid('getRows');
        for (var i = Seltdata.length; i > 0; i--) {
            if ($.inArray(Seltdata[i].Id.toString(), arr) != -1) {
                var rowindex = $dgw.datagrid('getRowIndex', Seltdata[i - 1]);
                $dgw.datagrid('deleteRow', rowindex);
            }
        }
    }
}
//
function endEditing() {
    if (editIndex == undefined)
        return true;
    if ($dgw.datagrid('validateRow', editIndex)) {
        $dgw.datagrid('endEdit', editIndex);
        editIndex = undefined;
        return true;
    } else {
        return false;
    }
}
//
function append() {
    if (endEditing()) {
        var NewRow = {};
        var Columns = $dgw.datagrid('getColumnFields');
        for (var column in Columns) {
            NewRow[Columns[column]] = null;
        }
        //动态设置 编辑样式
        var byteColumn = $dgw.datagrid('getColumnOption', 'ADDWHO');
        if (!(typeof (byteColumn) === 'undefined' || byteColumn == null || byteColumn == '')) {
            byteColumn.editor = {
                type: 'textbox',
                options: {
                    required: false,
                    validType: 'length[0,20]'
                }
            };
            byteColumn = $dgw.datagrid('getColumnOption', 'ADDTS');
            byteColumn.editor = {
                type: 'datebox',
                options: {
                    required: false
                }
            };
            byteColumn = $dgw.datagrid('getColumnOption', 'EDITWHO');
            byteColumn.editor = {};
            byteColumn = $dgw.datagrid('getColumnOption', 'EDITTS');
            byteColumn.editor = {};
        }
        $dgw.datagrid('insertRow', { index: 0, row: NewRow });
        editIndex = 0;
        //$dgw.datagrid('selectRow', editIndex).datagrid('beginEdit', editIndex);
    }
}
//
function onClickCell(index, field) {
    var _operates = ["_operate1", "_operate2", "_operate3", "ck"]
    if ($.inArray(field, _operates) >= 0) {
        return;
    }
    if (editIndex != index) {
        if (endEditing()) {
            //动态设置 编辑样式
            var byteColumn = $dgw.datagrid('getColumnOption', 'ADDWHO');
            if (!(typeof (byteColumn) === 'undefined' || byteColumn == null || byteColumn == '')) {
                byteColumn.editor = {};
                byteColumn = $dgw.datagrid('getColumnOption', 'ADDTS');
                byteColumn.editor = {};
                var byteColumn = $dgw.datagrid('getColumnOption', 'EDITWHO');
                byteColumn.editor = {
                    type: 'textbox',
                    options: {
                        required: false,
                        validType: 'length[0,20]'
                    }
                };
                byteColumn = $dgw.datagrid('getColumnOption', 'EDITTS');
                byteColumn.editor = {
                    type: 'datebox',
                    options: {
                        required: false
                    }
                };
            }

            //$dgw.datagrid('selectRow', index).datagrid('beginEdit', index);
            var ed = $dgw.datagrid('getEditor', { index: index, field: field });
            if (ed) {
                ($(ed.target).data('textbox') ? $(ed.target).textbox('textbox') : $(ed.target)).focus();
            }
            editIndex = index;
        } else {
            $dgw.datagrid('selectRow', editIndex);
        }
    }
}