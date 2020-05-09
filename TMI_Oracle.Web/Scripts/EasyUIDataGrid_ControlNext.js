//datagrid  combogrid键盘事件
var dg_keyHandler_ = function () {
    return {
        keyHandler: {
            up: function (e) {
                combogrid_upKey(this, e);
            },
            down: function (e) {
                combogrid_downKey(this, e);
            },
            enter: function (e) {
                var fieldName = '';
                var $txt = $(this);
                if ($(this).hasClass('textbox-text'))
                    $txt = $(this).parent().siblingd("input:first");
                fieldName = $txt.parents("td[field]:first()").attr("field");
                combogrid_enterKey(this, e, $dg, fieldName, editIndex);
            },
            query: function (q) {
                combogrid_query(this, q, editIndex, {});
            }
        }
    }
}();
//datagrid 编辑时的 combogrid通用参数设置
var combogrid_DefaultSettings = function () {
    return {
        required: false,
        method: 'get',
        mode: 'remote',
        delay: 500,
        multiple: false,
        panelHeight: 'auto',
        panelWidth: 230,
        pageSize: 10,//每页显示的记录条数，默认为10
        pageList: [5, 10, 20],//可以设置每页记录条数的列表
        pagination: true,//是否分页
        idField: 'ID',
        valueField: 'ID',
        textField: 'TEXT',
        columns: [[
            { field: 'ID', title: '代码', width: 53 },
            { field: 'TEXT', title: '名称', width: 150 }
        ]],
        onLoadSuccess: function (data) {
            //写在 beginEdit里，这里会被 覆盖掉
        }
    }
}();

//获取 targetEditor的options
function getOptions($next) {
    var nextopts = {};
    if ($next) {
        if ($next.target.data('textbox')) {
            nextopts = $next.target.textbox('options');
        }
        if ($next.target.data('datebox')) {
            nextopts = $next.target.datebox('options');
        }
        if ($next.target.data('datetimebox')) {
            nextopts = $next.target.datetimebox('options');
        }
        if ($next.target.data('numberbox')) {
            nextopts = $next.target.numberbox('options');
        }
        if ($next.target.data('combobox')) {
            nextopts = $next.target.combobox('options');
        }
        if ($next.target.data('combogrid')) {
            nextopts = $next.target.combogrid('options');
        }
    }
    return nextopts;
}
//设置 targetEditor值
function setTargetVal(targetEditor, val) {
    switch (targetEditor.type.toLowerCase()) {
        case "combogrid":
            $(targetEditor.target).combogrid("setValue", val);
            var opts = $(targetEditor.target).combogrid('options');
            if (opts.queryParams && opts.queryParams.DOCCODE) {
                //设置查询参数
                var queryParams = {
                    page: 1,
                    rows: opts.pageSize,
                    q: val,
                    DOCCODE: opts.queryParams.DOCCODE  //枚举代码
                };
            }
            else {
                //设置查询参数
                var queryParams = {
                    page: 1,
                    rows: opts.pageSize,
                    q: val
                };
            }

            opts.queryParams = queryParams;
            $(targetEditor.target).combogrid("grid").datagrid('load', queryParams);
            break;
        case "combobox":
            $(targetEditor.target).combobox("setValue", val);
            break;
        case "textbox":
            $(targetEditor.target).textbox("setValue", val);
            break;
        case "numberbox":
            $(targetEditor.target).numberbox("setValue", val);
            break;
        case "datebox":
            $(targetEditor.target).datebox("setValue", val);
            break;
        case "datetimebox":
            $(targetEditor.target).datetimebox("setValue", val);
            break;
        case "checkbox":
            var OldVal = val;
            var slet = false;
            if (!(OldVal == null || typeof OldVal === 'undefined' || OldVal == '')) {
                if (isNaN(OldVal)) {
                    if (OldVal == '是' || OldVal == '启用' || OldVal.toLowerCase() == 'true')
                        slet = true;
                }
                else {
                    if (OldVal)
                        slet = true;
                }
            }
            $(targetEditor.target).prop("checked", slet);
            break;
        case "radio":
            var OldVal = val;
            var slet = false;
            if (!(OldVal == null || typeof OldVal === 'undefined' || OldVal == '')) {
                if (isNaN(OldVal)) {
                    if (OldVal == '是' || OldVal == '启用' || OldVal.toLowerCase() == 'true')
                        slet = true;
                }
                else {
                    if (OldVal)
                        slet = true;
                }
            }
            $(targetEditor.target).prop("setValue", slet);
            break;
    }
}
//获取 targetEditor值
function getTargetVal(targetEditor) {
    var val = '';
    if (targetEditor != null) {
        switch (targetEditor.type.toLowerCase()) {
            case "combogrid":
                val = $(targetEditor.target).combogrid("getValue");
                break;
            case "combobox":
                val = $(targetEditor.target).combobox("getValue");
                break;
            case "textbox":
                val = $(targetEditor.target).textbox("getValue");
                break;
            case "numberbox":
                val = $(targetEditor.target).numberbox("getValue");
                break;
            case "datebox":
                val = $(targetEditor.target).datebox("getValue");
                break;
            case "datetimebox":
                val = $(targetEditor.target).datetimebox("getValue");
                break;
            case "checkbox":
                val = $(targetEditor.target).prop("checked");
                break;
            case "radio":
                val = $(targetEditor.target).prop("checked");
                break;
        }
    }
    return val;
}
//设置TextBox控件必填
function setTextBoxRequired($target) {
    //$target.textbox({ required: true });
    var V$target = $target.parent().find('.validatebox-text:eq(0)');
    var v_opts = V$target.validatebox('options');
    if (typeof (v_opts.required) === 'undefined' || v_opts.required == null || !v_opts.required) {
        v_opts.required = true;
        v_opts.novalidate = false;
    }
    else {
        v_opts.required = false;
        v_opts.novalidate = true;
    }

    //V$target.bind('blur', function () {
    //    $(this).validatebox('enableValidation').validatebox('validate');
    //});
}
//设置扩展自Combo控件必填
function setComboRequired($target) {
    var cmb_opts = $target.combo('options');
    var v_opts = $target.combo('textbox').validatebox('options');
    //console.log('setComboRequired',cmb_opts, v_opts);
    if ((typeof (cmb_opts.required) === 'undefined' || cmb_opts.required == null || !cmb_opts.required) ||
        (typeof (v_opts.required) === 'undefined' || v_opts.required == null || !v_opts.required)) {
        cmb_opts.required = true;
        cmb_opts.novalidate = false;
        v_opts.required = true;
        v_opts.novalidate = false;
    }
    else {
        cmb_opts.required = false;
        cmb_opts.novalidate = true;
        v_opts.required = false;
        v_opts.novalidate = true;
    }
}
//设置 targetEditor必填
function setTargetRequired(targetEditor) {
    var $target;
    if (typeof (targetEditor.target) === 'undefined' || targetEditor.target == null)
        $target = $(targetEditor);
    else
        $target = $(targetEditor.target);
    //console.log('setTargetRequired', targetEditor.type);
    switch (targetEditor.type.toLowerCase()) {
        case "combogrid":
            setComboRequired($target);
            break;
        case "combobox":
            setComboRequired($target);
            break;
        case "textbox":
            setTextBoxRequired($target);
            break;
        case "numberbox":
            setTextBoxRequired($target);
            break;
        case "datebox":
            setTextBoxRequired($target);
            break;
        case "datetimebox":
            setTextBoxRequired($target);
            break;
    }
}
//combogrid keyhandler 事件【向上键】押下处理
function combogrid_upKey(obj, e) {
    //console.log('up', obj);
    //取得选中行
    var selected = $(obj).combogrid('grid').datagrid('getSelected');
    if (selected) {
        //取得选中行的rowIndex
        var index = $(obj).combogrid('grid').datagrid('getRowIndex', selected);
        //向上移动到第一行为止
        if (index > 0) {
            $(obj).combogrid('grid').datagrid('selectRow', index - 1);
        }
    } else {
        var rows = $(obj).combogrid('grid').datagrid('getRows');
        $(obj).combogrid('grid').datagrid('selectRow', rows.length - 1);
    }
}
//combogrid keyhandler 事件 【向下键】押下处理
function combogrid_downKey(obj, e) {
    //console.log('down', obj);
    //取得选中行
    var selected = $(obj).combogrid('grid').datagrid('getSelected');
    if (selected) {
        //取得选中行的rowIndex
        var index = $(obj).combogrid('grid').datagrid('getRowIndex', selected);
        //向下移动到当页最后一行为止
        if (index < $(obj).combogrid('grid').datagrid('getData').rows.length - 1) {
            $(obj).combogrid('grid').datagrid('selectRow', index + 1);
        }
    } else {
        $(obj).combogrid('grid').datagrid('selectRow', 0);
    }
}
//combobox keydown事件
function combobox_keydown(obj, e, $dg_, rowindex) {
    //debugger;
    //var $thisElem = $(obj).parent().siblings('.easyui-combobox:eq(0)');
    var $thisElem = $(obj).parent().parent().children('input:eq(0)');
    var $target;
    $.each($dg_.datagrid('getEditors', rowindex), function (y, elem) {
        if (elem) {
            //console.log('keyup', $thisElem[0], $(elem.target)[0]);
            if ($(elem.target)[0] == $thisElem[0]) {
                $target = elem;
            }
        }
    });
    $target.target.combobox('showPanel');
    //debugger;
    //取得选中行
    var opts = $target.target.combobox('options');
    if (!opts) {
        $target.target.combobox('showPanel');
    }
    var datarows = $target.target.combobox('getData');
    if (!event) {
        event = window.event;
    }
    //console.log('combobox', $(obj), $target, $target.target.combobox('options'),datarows);
    //回车事件
    if ((event.keyCode || event.which) == 13) {
        //debugger;
        //console.log('enter', $target.combobox('panel'));
        var Selted = $target.target.combobox('getValue');
        if (Selted.length <= 0) {
            //if (datarows.length > 0) {
            //    Selted = datarows[0];
            //    $target.target.combobox('setValue', datarows[0][opts.valueField]);
            //}
        }

        $target.target.combobox('hidePanel');
        var rowColumn = $dg_.datagrid('getColumnFields', false);
        for (var x = 0; x < rowColumn.length; x++) {
            if (rowColumn[x] == $target.field) {
                if (x != rowColumn.length - 1) {
                    var nextNum = x + 1;
                    var $next = $dg_.datagrid('getEditor', {
                        field: rowColumn[nextNum],
                        index: rowindex
                    });

                    var nextopts = getOptions($next);
                    while ($next == null || $next.target.attr('disabled') || $next.readonly) {
                        nextNum++;
                        $next = $dg_.datagrid('getEditor', {
                            field: rowColumn[nextNum],
                            index: rowindex
                        });
                        if (nextNum >= rowColumn.length) {
                            break;
                        }
                        else if ($next) {
                            nextopts = getOptions($next);
                        }
                    }
                    //console.log('keydown回车事件', $next, nextNum, rowColumn.length);
                    if ($next) {
                        if (event && event.preventDefault) {
                            event.preventDefault();
                            if ($next.type.toLowerCase() == "checkbox" || $next.type.toLowerCase() == "radio")
                                $($next.target).focus();
                            else
                                $($next.target).parent().children('span:eq(0)').children('input:eq(0)').focus();
                        }
                        else {
                            window.event.returnValue = false;
                        }
                    }
                    else {
                        if (nextNum >= rowColumn.length)
                            accept();//保存
                    }
                }
                else
                    accept();//保存
                break;
            }
        }
        //debugger;
    }
    //up
    if ((event.keyCode || event.which) == 38) {
        //console.log('up');
        var data = datarows;//$target.target.combobox('options').data;
        if (data.length > 0) {
            //取得选中值
            var selected = $target.target.combobox('getValue');
            if (selected.length > 0) {
                for (var i = 0; i < data.length; i++) {
                    if (selected == data[i][opts.valueField]) {
                        if (i > 0) {
                            $target.target.combobox('setValue', data[i - 1][opts.valueField]);
                        }
                    }
                }
            }
        }
    }
    //dowm
    if ((event.keyCode || event.which) == 40) {
        //console.log('down', obj, datarows);
        var data = datarows;//opts.data;
        if (data.length > 0) {
            //取得选中值
            var selected = $target.target.combobox('getValue');
            if (selected.length > 0) {
                for (var i = 0; i < data.length; i++) {
                    if (selected == data[i][opts.valueField]) {
                        if (i != data.length - 1) {
                            $target.target.combobox('setValue', data[i + 1][opts.valueField]);
                        }
                    }
                }
            }
            else {
                $target.target.combobox('setValue', data[0][opts.valueField]);
            }
        }
    }
}
//combogrid query 事件 按任意键 搜索（本地）
function combogrid_local_query(obj, q, e) {
    var opts = $(obj).combogrid('options');
    var data = $(obj).combogrid('grid').datagrid('getData');
    //$(obj).combogrid('setValue','');
    var $combogridPanel = $(obj).combogrid('panel');
    var $trs = $combogridPanel.find('table.datagrid-btable:eq(0)').find('tr.datagrid-row');
    $trs.removeClass('hide datagrid-row-checked datagrid-row-selected');
    //console.log($trs,data);
    if (data) {
        $.each(data.rows, function (i, item) {
            if ((item[opts.valueField] + '').indexOf(q) < 0 && (item[opts.textField] + '').indexOf(q) < 0) {
                $($trs[i]).addClass('hide');
                $($trs[i]).removeClass('datagrid-row-checked datagrid-row-selected');
            }
            else {
                if (item[opts.valueField] == q) {
                    $($trs[i]).addClass('datagrid-row-checked datagrid-row-selected');
                    $(obj).parent().find(":hidden.textbox-value").val(q);
                }
            }
        });
    }
}
//combogrid query 事件 按任意键 搜索（本地）
function combogrid_query(obj, q, rowIndex, queryparam) {
    $(obj).data("LoadSuccess", false);
    var opts = $(obj).combogrid('options');
    var url = opts.url;
    //设置查询参数
    var queryParams = {
        page: 1,
        rows: opts.pageSize,
        q: q
    };
    queryParams = $.extend(true, queryparam, queryParams);
    //设置值,不加搜完后 就清空了
    if (q == '')
        $(obj).combogrid('setValue', '');
    else {
        $(obj).combogrid('setValue', q);
    }
    //动态搜索
    $(obj).combogrid('grid').datagrid('load', queryParams);
    setTimeout(function () {
        var Selted = $(obj).combogrid('grid').datagrid('getSelected');
        if (Selted) {
            if (!(typeof opts.onChange === 'undefined' || opts.onChange == null)) {
                if (typeof opts.onChange === 'function') {
                    var valStr = '';
                    if (!(typeof (opts.valueField) === 'undefined' || opts.valueField == null)) {
                        valStr = opts.valueField;
                    } else if (!(typeof (opts.valuefield) === 'undefined' || opts.valuefield == null))
                        valStr = opts.valuefield;
                    else if (!(typeof (opts.Valuefield) === 'undefined' || opts.Valuefield == null))
                        valStr = opts.Valuefield;

                    opts.onChange(Selted[valStr]);
                }
            }
        }
    }, 300);
}
//加载数据成功
function ondgLoadSuccess($dg_, fieldName, rowIndex) {
    var targetEditor = $dg_.datagrid('getEditor', {
        field: fieldName,
        index: rowIndex
    });
    $(targetEditor.target).data("LoadSuccess", true);
    //setTimeout(function () {
    //    var Selted = $(targetEditor.target).combogrid('grid').datagrid('getSelected');
    //    if (Selted) {
    //        console.log('onLoadSuccess', $(targetEditor.target).combogrid('grid'), Selted);
    //        var opts = $(targetEditor.target).combogrid('options');
    //        if (!(typeof opts.onChange === 'undegined' || opts.onChange == null)) {
    //            if (typeof opts.onChange === 'function')
    //                opts.onChange(Selted[opts.valuefield]);
    //        }
    //        //$(targetEditor.target).combogrid('grid').trigger("onChange", Selted);
    //        //console.log('onLoadSuccess',fieldName, rowIndex);
    //    }
    //}, 300);
}
//combogrid keyhandler 事件 【回车键】押下处理
function combogrid_enterKey(obj, e, $dg_, fieldName, editIndex) {
    var $target = $dg_.datagrid('getEditor', {
        field: fieldName,
        index: editIndex
    });
    var Selted = $(obj).combogrid('grid').datagrid('getSelected');
    var val = $(obj).combogrid('getValue');
    if (!Selted) {
        var LoadSuccess = $($target.target).data('LoadSuccess');
        if (!(typeof val === 'undefined' || val == null || val == '') && (typeof LoadSuccess === 'undefined' || LoadSuccess == null || LoadSuccess)) {
            var datarows = $(obj).combogrid('grid').datagrid('getData').rows;
            if (datarows.length > 0) {
                Selted = datarows[0];
                var rowindex = $(obj).combogrid('grid').datagrid('getRowIndex', datarows[0]);
                $(obj).combogrid('grid').datagrid('selectRow', rowindex);
            }
        }
    }
    if (Selted) {
        //设置文本框的内容为选中行的的性别字段内容
        //$(obj).val(Selted[$(obj).combogrid('options').valueField]);
    }
    //选中后让下拉表格消失
    $(obj).combogrid('hidePanel');

    if (typeof (fieldName) == undefined || typeof (editIndex) == undefined) {
        return false;
    }
    if (fieldName == '' || isNaN(editIndex)) {
        return false;
    }

    if (Selted) {
        var datarow = $dg_.datagrid('getData').rows[editIndex];
        if (!(typeof datarow === 'undefined' || datarow == null)) {
            datarow[fieldName + 'NAME'] = Selted[$(obj).combogrid('options').textField];
        }
    }

    var dataColumn = $dg_.datagrid('getColumnFields', false);
    //console.log(dataColumn,$(this));
    for (var x = 0; x < dataColumn.length; x++) {
        if (dataColumn[x] == fieldName) {
            if (x != dataColumn.length - 1) {
                var nextNum = x + 1;
                var $next = $dg_.datagrid('getEditor', {
                    field: dataColumn[nextNum],
                    index: editIndex
                });
                //if ($next) {
                var nextopts = getOptions($next);
                while ($next == null || $next.target.attr('disabled') || nextopts.readonly) {
                    nextNum++;
                    $next = $dg_.datagrid('getEditor', {
                        field: dataColumn[nextNum],
                        index: editIndex
                    });
                    if (nextNum >= dataColumn.length) {
                        break;
                    }
                    nextopts = getOptions($next);
                }
                //}
                //console.log($dg_, editIndex, $next,x, dataColumn[x + 1]);
                if ($next) {
                    //setTimeout(function () {
                    //    $($next.target).parent().children('span:eq(0)').children('input:eq(0)').focus();
                    //}, 100);
                    if (e && e.preventDefault) {
                        e.preventDefault();
                        if ($next.type.toLowerCase() == "checkbox" || $next.type.toLowerCase() == "radio")
                            $($next.target).focus();
                        else
                            $($next.target).parent().children('span:eq(0)').children('input:eq(0)').focus();
                    }
                    else {
                        window.event.returnValue = false;
                    }
                }
                else {
                    if (nextNum >= rowColumn.length)
                        dgaccept($dg_);//保存
                }
            }
            else {
                dgaccept($dg_);//保存
            }
            break;
        }
    }
    return false;
}
//继承自textbox的keydown事件 enter 下一个
function Listtextbox_keydown(obj, event, $dg_, rowindex) {
    //debugger;
    var $thisElem = $(obj).parent().parent().children('input:eq(0)');
    var $target;
    $.each($dg_.datagrid('getEditors', rowindex), function (y, elem) {
        if (elem) {
            //console.log('keyup', $thisElem[0], $(elem.target)[0]);
            if ($(elem.target)[0] == $thisElem[0]) {
                $target = elem;
            }
        }
    });
    //console.log('keydown', $target);

    if (!event) {
        event = window.event;
    }
    //回车事件
    if ((event.keyCode || event.which) == 13) {
        if ($target.type == "datebox") {
            if ($($thisElem).datebox('getValue') == '') {
                //$($thisElem).datebox('setValue', moment().format('YYYY-MM-DD'));
            }
        }
        if ($target.type == "datetimebox") {
            if ($($thisElem).datebox('getValue') == '') {
                //$($thisElem).datebox('setValue', moment().format('YYYY-MM-DD HH:mm:ss'));
            }
        }
        //console.log('keydown回车事件', $target);
        var rowColumn = $dg_.datagrid('getColumnFields', false);
        for (var x = 0; x < rowColumn.length; x++) {
            if (rowColumn[x] == $target.field) {
                if (x != rowColumn.length - 1) {
                    var nextNum = x + 1;
                    var $next = $dg_.datagrid('getEditor', {
                        field: rowColumn[nextNum],
                        index: rowindex
                    });
                    var nextopts = getOptions($next);
                    while ($next == null || $next.target.attr('disabled') || nextopts.readonly) {
                        nextNum++;
                        $next = $dg_.datagrid('getEditor', {
                            field: rowColumn[nextNum],
                            index: rowindex
                        });
                        if (nextNum >= rowColumn.length) {
                            break;
                        }
                        nextopts = getOptions($next);
                    }
                    //console.log('keydown回车事件', $next, nextNum, rowColumn.length);
                    if ($next) {
                        if (event && event.preventDefault) {
                            event.preventDefault();
                            if ($next.type.toLowerCase() == "checkbox" || $next.type.toLowerCase() == "radio")
                                $($next.target).focus();
                            else
                                $($next.target).parent().children('span:eq(0)').children('input:eq(0)').focus();
                        }
                        else {
                            window.event.returnValue = false;
                        }
                    }
                    else {
                        if (nextNum >= rowColumn.length)
                            dgaccept($dg_);
                    }
                }
                else
                    dgaccept($dg_);//保存
                break;
            }
        }
    }
    else {
        var KeyCode = (event.keyCode || event.which);
        var numberboxOpts = {};//precision
        if ($target.type == 'numberbox') {
            if (!(KeyCode == 8 || KeyCode == 46 || KeyCode == 110 || KeyCode == 190 || (KeyCode >= 48 && KeyCode <= 57) || (KeyCode >= 96 && KeyCode <= 105))) {
                //console.log('KeyCode', KeyCode);
                if (!((KeyCode >= 37 && KeyCode <= 40) || (KeyCode >= 112 && KeyCode <= 123))) {
                    if (event && event.preventDefault)
                        event.preventDefault();
                    else
                        window.event.returnValue = false;
                }
            }
            else {
                //console.log('KeyCode-', KeyCode);
                var val = $(obj).val();
                var _pointNum = val.indexOf('.');
                if (KeyCode == 110 || KeyCode == 190) {
                    if (_pointNum > 0) {
                        if (event && event.preventDefault)
                            event.preventDefault();
                        else
                            window.event.returnValue = false;
                    } else {
                        if (val == "") {
                            if (event && event.preventDefault)
                                event.preventDefault();
                            else
                                window.event.returnValue = false;
                        }
                    }
                } else if (!(KeyCode == 8 || KeyCode == 46)) {
                    numberboxOpts = $target.target.numberbox('options');
                    if (numberboxOpts.precision) {
                        if (_pointNum >= 0) {
                            if (val.length - _pointNum > numberboxOpts.precision) {
                                if (event && event.preventDefault)
                                    event.preventDefault();
                                else
                                    window.event.returnValue = false;
                            }
                        }
                    }
                }
                else {

                }
            }
        }
    }
    //debugger;
}
//checkbox或radio keydown事件
function chk_rdo_keydown(obj, event, $dg_, rowindex) {
    //debugger;
    var $thisElem = $(obj);
    var $target;
    $.each($dg_.datagrid('getEditors', rowindex), function (y, elem) {
        if (elem) {
            //console.log('chk_rdo', $thisElem, $(elem.target)[0]);
            if ($(elem.target)[0] == $thisElem[0]) {
                $target = elem;
            }
        }
    });
    //console.log('chk_rdo', $target);
    //debugger;
    if (!event) {
        event = window.event;
    }
    //回车事件
    if ((event.keyCode || event.which) == 13) {
        //console.log('chk_rdo回车事件', $target);
        //debugger;
        $($target.target).prop("checked", true);
        var rowColumn = $dg_.datagrid('getColumnFields', false);
        for (var x = 0; x < rowColumn.length; x++) {
            if (rowColumn[x] == $target.field) {
                if (x != rowColumn.length - 1) {
                    var nextNum = x + 1;
                    var $next = $dg_.datagrid('getEditor', {
                        field: rowColumn[nextNum],
                        index: rowindex
                    });
                    var nextopts = getOptions($next);
                    while ($next == null || $next.target.attr('disabled') || nextopts.readonly) {
                        nextNum++;
                        $next = $dg_.datagrid('getEditor', {
                            field: rowColumn[nextNum],
                            index: rowindex
                        });
                        if (nextNum >= rowColumn.length) {
                            break;
                        }
                        nextopts = getOptions($next);
                    }
                    //console.log('keydown回车事件', $next, nextNum, rowColumn.length);
                    if ($next) {
                        if (event && event.preventDefault) {
                            event.preventDefault();
                            if ($next.type.toLowerCase() == "checkbox" || $next.type.toLowerCase() == "radio")
                                $($next.target).focus();
                            else
                                $($next.target).parent().children('span:eq(0)').children('input:eq(0)').focus();
                        }
                        else {
                            window.event.returnValue = false;
                        }
                    }
                    else {
                        if (nextNum >= rowColumn.length)
                            dgaccept($dg_);//保存
                    }
                }
                else
                    dgaccept($dg_);//保存
                break;
            }
        }
        //debugger;
    }
}
//执行Accept方法
function dgaccept($dg_) {
    var funcaccept = $dg_.attr('id').replace('_datagrid', '_') + "accept";
    //根据函数名得到函数类型
    var func;
    try {
        func = eval(funcaccept);
        if (typeof func === 'function') {
            //创建函数对象，并调用
            func.call();
        }
    }
    catch (ex) {
        accept();//保存
    }
}
//添加HSCODE关联规格型号选择控件
function AddHSCODESeltControl($dg_, targetEditor, fieldName, fieldHSCODE, dialogId, rowIndex) {
    if (!(typeof targetEditor === 'undefined' || targetEditor == null)) {
        if (targetEditor.field == fieldName) {
            var $dialog = $('#' + dialogId);
            var targetHSCODE = $dg_.datagrid('getEditor', {
                field: fieldHSCODE,
                index: rowIndex
            });
            var $span = $(targetEditor.target).siblings("span:eq(0)");
            if ($span) {
                $span.on("dblclick", function () {
                    var valStr = $(this).siblings("input[type=text]:eq(0)").textbox("getValue");
                    var $layout = $dialog.children("div.easyui-layout:eq(0)");
                    $layout.html('');
                    $.ajax({
                        type: 'Get',
                        url: '/PARA_HSCODES/GetCurrG_ModelType',//获取数据的函数
                        async: true,//true 异步，false 同步
                        dataType: "json",
                        contentType: "application/json; charset=utf-8",
                        data: { G_Model: valStr, HSCODE: getTargetVal(targetHSCODE) },//查询条件JSON.stringify()
                        beforeSend: function (XHR) {
                            $.messager.progress({
                                title: '设置规格型号',
                                msg: '正在获取数据，请等待...'
                            });
                        },
                        success: function (data) {//查询成功,data为返回的数据
                            $.messager.progress('close');
                            if (data.Success) {
                                if (data.ArrG_ModelType) {
                                    //console.log(data.ArrG_ModelType);
                                    if (data.ArrG_ModelType.length > 0) {
                                        var ArrTemplete = [
                                            '<div style="margin-bottom:1px; max-height:38px; overflow:hidden;"> \r\n',
                                            //'    <label class="textbox-label textbox-label-before" for="" style="width:200px; text-align: right; height: 24px; line-height: 24px; overflow:hidden;"></label>',
                                            '    <div style="text-align:right; width:200px; height:48px; float:left;">',
                                            '        <div style=" height:38px; display:block; vertical-align:middle;">',
                                            '            <div name="content" style="border:0px solid red; width:200px; height:48px; margin:0 auto;">',
                                            '                ',
                                            '            </div>',
                                            '        </div>',
                                            '    </div>',
                                            //'    <input class="easyui-textbox" name="" style="width:200px; height: 24px; line-height: 24px;">',
                                            '    <div style="text-align:right; width:200px; height:38px;float:left;">',
                                            '        <div style=" height:38px; display:block; vertical-align:middle;">',
                                            '            <div name="inputcontent" style="border:0px solid red; width:200px; height:48px; vertical-align:middle; margin:auto;">',
                                            '                <input class="easyui-textbox" name="" style="width:200px; height: 24px; line-height: 24px;">',
                                            '            </div>',
                                            '        </div>',
                                            '    </div>',
                                            '</div> \r\n'
                                        ];
                                        $.each(data.ArrG_ModelType, function (i, item) {
                                            ArrTemplete[1] = ArrTemplete[1].replace('label:\'\',', 'label:\'' + item.Key + '\',');
                                            var $templete = $(ArrTemplete.join(''));
                                            //var $label = $templete.children('label:eq(0)');
                                            var $label = $templete.children('div:eq(0)').find("div[name=content]");
                                            //var $input = $templete.children('input:eq(0)');
                                            var $input = $templete.children('div:eq(1)').find("div[name=inputcontent]").children('input:eq(0)');
                                            var inputName = "G_ModelType" + i;
                                            $label.attr({ "for": inputName, "title": item.Key });
                                            $label.html(item.Key + ":");
                                            $input.attr("name", inputName);
                                            $input.val(item.Value);
                                            $layout.append($templete);
                                        });
                                        $dialog.dialog('open');
                                    }
                                    else
                                        $.messager.alert("错误", "无法解析数据", 'error');
                                }
                                else
                                    $.messager.alert("错误", "解析数据错误", 'error');
                            }
                            else {
                                $.messager.alert("错误", data.ErrMsg, 'error');
                            }
                        },
                        error: function (response) {
                            $.messager.progress('close');
                            $.messager.alert("错误", "处理数据时发生错误了！", 'error');
                        }
                    });
                });
            }
        }
    }
}
//保存选择的规格型号类别
function SaveTypeValue($dg_, dialogId, fieldName, editIndex) {
    //对话框
    var $dialog = $("#" + dialogId);
    var $layout = $dialog.children("div.easyui-layout:eq(0)");
    var G_ModelTypeInputs = $layout.find('input.easyui-textbox');
    var G_ModelTypeVals = [];//保存键值对
    G_ModelTypeInputs.each(function (i, item) {
        //var $label = $(item).siblings("label:eq(0)");
        var $label = $(item).parent().parent().parent().parent().find("div[name=content]");
        var labelStr = $label.html();
        var val = $(item).val();
        if (labelStr.length > 1) {
            var index = labelStr.length - 1;
            var lastval = labelStr.substr(index);
            if (lastval == ':' || lastval == '：') {
                labelStr = $.trim(labelStr.substr(0, index));
            }
        }
        G_ModelTypeVals.push({ G_ModelType: labelStr, Value: val });
    });
    if (typeof G_ModelTypeVals === 'undefined' || G_ModelTypeVals == null) {
        //$.messager.alert("错误", '规格型号类型，必须填写完整');
        //return;
    }
    else {
        for (var x in G_ModelTypeVals) {
            var item = G_ModelTypeVals[x];
            if (item.G_ModelType != "其他" && (typeof item.Value === 'undefined' || item.Value == null || item.Value == '')) {
                //$.messager.alert("错误", '规格型号类型，必须填写完整(除 其他 类型以外)');
                //return;
            }
        }
    }
    $dialog.dialog('close');
    var TargetEditor = $dg_.datagrid('getEditor', {
        field: fieldName,
        index: editIndex
    });
    //console.log($dg_, fieldName, editIndex,TargetEditor);
    if (!(typeof TargetEditor === 'undefined' || TargetEditor == null)) {
        if (TargetEditor.target) {
            var TypeValStr = "";
            for (var i in G_ModelTypeVals) {
                //TypeValStr += (parseInt(i) + 1) + ":" + G_ModelTypeVals[i].G_ModelType + "-" + G_ModelTypeVals[i].Value + ";";
                TypeValStr += G_ModelTypeVals[i].Value + "|";
            }
            if (TypeValStr.substr(TypeValStr.length - 1) == "|")
                TypeValStr = TypeValStr.substr(0, TypeValStr.length - 1);
            var pmStr = "1:品名";
            if (TypeValStr.length < pmStr.length) {
                if (TypeValStr.replace(/[|]/g, '') == "") {
                    TypeValStr = "";
                }
                $(TargetEditor.target).textbox("setValue", TypeValStr);
            } else {
                var Str = TypeValStr.substr(0, pmStr.length);
                if (Str.indexOf(pmStr) >= 0) {
                    $.messager.alert("错误", '第一个规格型号类型，不能是品名');
                    return;
                }
                if (TypeValStr.replace(/[|]/g, '') == "") {
                    TypeValStr = "";
                }
                $(TargetEditor.target).textbox("setValue", TypeValStr);
            }
        }
    }
    else {
        $.messager.alert("错误", '规格型号编辑控件不存在');
        return;
    }
}
//设置datebox值
function setdateboxvalue($dg_, index, fields) {
    var seletRow = $dg_.datagrid('getSelected');
    for (var i = 0; i < fields.length; i++) {
        if (seletRow[fields[i]] != null && seletRow[fields[i]] != '') {
            var TargetEditor = $dg_.datagrid('getEditor', { index: index, field: fields[i] });
            if (TargetEditor != null && TargetEditor.type.toLowerCase() == 'datebox') {
                $(TargetEditor.target).datebox('setValue', moment(seletRow[fields[i]]).format('YYYY-MM-DD'));
            }
        }
    }
}
//设置datagrid编辑时，Combogrid 等编辑器的回车下一个等功能
function Set_dg_onBeginEditCombogrid($dg_, combogrid_Settings, rowIndex, rowData) {
    if (typeof (combogrid_Settings) === 'undefined' || combogrid_Settings == null)
        combogrid_Settings = [];
    for (var item in combogrid_Settings) {
        if (combogrid_Settings[item].onLoadSuccess == undefined || combogrid_Settings[item].onLoadSuccess == null) {
            combogrid_Settings[item].onLoadSuccess = function () {
                ondgLoadSuccess($dg_, item, rowIndex);
            }
        }
        if (combogrid_Settings[item].onChange == undefined || combogrid_Settings[item].onChange == null) {
            combogrid_Settings[item].onChange = function (value) {
                var $thisCombog = $(this);
                //加个0.1获取选择项，不然有时EasyUI还未选中
                setTimeout(function () { 
                    var Selted = $thisCombog.combogrid('grid').datagrid('getSelected');
                    if (Selted) {
                        var fiedName = "";
                        $thisCombog.parents("td").each(function (i, obj) {
                            var attrib = $(obj).attr('field');
                            if (!(typeof (attrib) === 'undefined' || attrib == null || attrib == "")) {
                                fiedName = attrib;
                                return;
                            }
                        });
                        if (fiedName != "")
                            rowData[fiedName + 'NAME'] = Selted[$thisCombog.combogrid('options').textField];
                    }
                }, 100);
            }
        }
        if (combogrid_Settings[item].keyHandler == undefined || combogrid_Settings[item].keyHandler == null) {
            combogrid_Settings[item].keyHandler = {
                up: function (e) {
                    combogrid_upKey(this, e);
                },
                down: function (e) {
                    combogrid_downKey(this, e);
                },
                enter: function (e) {
                    combogrid_enterKey(this, e, $dg_, item, rowIndex);
                },
                query: function (q) {
                    var opts = $(this).combogrid('options');
                    if (opts.mode == 'local' || (opts.url == null || opts.url == '')) {
                        combogrid_local_query(this, q, e);
                    } else {
                        var url = opts.url;
                        //设置查询参数
                        var queryParams = {
                            page: 1,
                            rows: opts.pageSize,
                            q: q
                        };
                        //设置值,不加搜完后 就清空了
                        if (q == '')
                            $(this).combogrid('setValue', '');
                        else {
                            $(this).combogrid('setValue', q);
                        }
                        //动态搜索
                        $(this).combogrid('grid').datagrid('load', queryParams);
                    }
                }
            }
        }
    }
    var num = 0;
    for (var i in rowData) {
        var targetEditor = $dg_.datagrid('getEditor', {
            field: i,
            index: rowIndex
        });
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
                    if (OldVal != null) {
                        if (typeof (OldVal) != 'undefined') {
                            if (OldVal != '') {
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
                                $(targetEditor.target).data("LoadSuccess", false);
                                $(targetEditor.target).combogrid('grid').datagrid('load', queryParams);
                            }
                        }
                    }
                    break;
                case "textbox":
                    $(targetEditor.target).textbox({
                        inputEvents: $.extend({}, $.fn.textbox.defaults.inputEvents, {
                            keydown: function (event) {
                                Listtextbox_keydown(this, event, $dg_, rowIndex);
                            }
                        })
                    });
                    break;
                case "combobox":
                    var opts = $(targetEditor.target).combobox({
                        inputEvents: $.extend({}, $.fn.combobox.defaults.inputEvents, {
                            keydown: function (event) {
                                combobox_keydown(this, event, $dg_, rowIndex);
                            }
                        })
                    });
                    break;
                case "numberbox":
                    $(targetEditor.target).numberbox({
                        inputEvents: $.extend({}, $.fn.textbox.defaults.inputEvents, {
                            keydown: function (event) {
                                Listtextbox_keydown(this, event, $dg_, rowIndex);
                            }
                        })
                    });
                    break;
                case "datebox":
                    $(targetEditor.target).datebox({
                        inputEvents: $.extend({}, $.fn.textbox.defaults.inputEvents, {
                            keydown: function (event) {
                                Listtextbox_keydown(this, event, $dg_, rowIndex);
                            }
                        })
                    });
                    setTargetVal(targetEditor, OldVal);
                    break;
                case "datetimebox":
                    $(targetEditor.target).datetimebox({
                        inputEvents: $.extend({}, $.fn.textbox.defaults.inputEvents, {
                            keydown: function (event) {
                                Listtextbox_keydown(this, event, $dg_, rowIndex);
                            }
                        })
                    });
                    setTargetVal(targetEditor, OldVal);
                    break;
                case "checkbox":
                    $(targetEditor.target).keydown(function (e) {
                        chk_rdo_keydown(this, e, $dg_, rowIndex);
                    });
                    setTargetVal(targetEditor, OldVal);
                    break;
                case "radio":
                    $(targetEditor.target).keydown(function (e) {
                        chk_rdo_keydown(this, e, $dg_, rowIndex);
                    });
                    setTargetVal(targetEditor, OldVal);
                    break;
            }
        }
    }
}
//行号自适应宽度 行数据超过9999时，第一列的行号rownumber将会因为表格内容过长而导致无法显示全部数字
$.extend($.fn.datagrid.methods, {
    fixRownumber: function (jq) {
        return jq.each(function () {
            var panel = $(this).datagrid("getPanel");
            //获取最后一行的number容器,并拷贝一份
            var clone = $(".datagrid-cell-rownumber", panel).last().clone();
            //由于在某些浏览器里面,是不支持获取隐藏元素的宽度,所以取巧一下
            clone.css({
                "position": "absolute",
                left: -1000
            }).appendTo("body");
            var width = clone.width("auto").width();
            //默认宽度是25,所以只有大于25的时候才进行fix
            if (width > 25) {
                //多加5个像素,保持一点边距
                $(".datagrid-header-rownumber,.datagrid-cell-rownumber", panel).width(width + 5);
                //修改了宽度之后,需要对容器进行重新计算,所以调用resize
                $(this).datagrid("resize");
                //一些清理工作
                clone.remove();
                clone = null;
            } else {
                //还原成默认状态空调维修
                $(".datagrid-header-rownumber,.datagrid-cell-rownumber", panel).removeAttr("style");
            }
        });
    }
});
//获取DataGrid 数据
function GetDataGrid_data($dg) {
    var opts = $dg.datagrid('options');
    if (opts.remoteFilter) {
        return $dg.datagrid('getData');
    } else {
        var inserted_detail = $dg.data('datagrid').filterSource;
        //console.log('GetDataGrid_data', inserted_detail, $dg.datagrid().data('datagrid'));
        if (!(typeof (inserted_detail) === 'undefined' || inserted_detail == null)) {
            if (ObjectIsArray(inserted_detail)) {
                if (inserted_detail.length > 0) {
                    return inserted_detail;
                }
            } else {
                if (!(typeof (inserted_detail.rows) === 'undefined' || inserted_detail.rows == null)) {
                    if (ObjectIsArray(inserted_detail.rows)) {
                        if (inserted_detail.rows.length > 0)
                            return inserted_detail.rows;
                    }
                }
            }
        } else {
            inserted_detail = opts.data;
            //console.log('GetDataGrid_data', inserted_detail, $dg.datagrid('options'), $dg.datagrid().data);
            if (!(typeof (inserted_detail) === 'undefined' || inserted_detail == null)) {
                if (ObjectIsArray(inserted_detail)) {
                    if (inserted_detail.length > 0) {
                        return inserted_detail;
                    }
                    else {
                        inserted_detail = $dg.datagrid('getData');
                        if (!(typeof (inserted_detail) === 'undefined' || inserted_detail == null)) {
                            if (ObjectIsArray(inserted_detail)) {
                                if (inserted_detail.length > 0) {
                                    return inserted_detail;
                                }
                            } else {
                                if (!(typeof (inserted_detail.rows) === 'undefined' || inserted_detail.rows == null)) {
                                    if (ObjectIsArray(inserted_detail.rows)) {
                                        if (inserted_detail.rows.length > 0)
                                            return inserted_detail.rows;
                                    }
                                }
                            }
                        }
                    }
                }
                else {
                    if (!(typeof (inserted_detail.rows) === 'undefined' || inserted_detail.rows == null)) {
                        if (ObjectIsArray(inserted_detail.rows)) {
                            if (inserted_detail.rows.length > 0)
                                return inserted_detail.rows;
                        }
                    }
                }
            }
        }
    }
    return [];
}
//设置DataGrid 数据
function SetDataGrid_data($dg, data) {
    var opts = $dg.datagrid('options');
    if (opts.remoteFilter) {
        opts.data = data;
        return true;
    } else {
        var inserted_detail = $dg.data('datagrid').filterSource;
        if (!(typeof (inserted_detail) === 'undefined' || inserted_detail == null)) {
            $dg.data('datagrid').filterSource = data;
        } else {
            inserted_detail = opts.data;
            if (!(typeof (inserted_detail) === 'undefined' || inserted_detail == null)) {
                if (ObjectIsArray(inserted_detail)) {
                    if (inserted_detail.length > 0) {
                        opts.data = data;
                        return true;
                    }
                    else {
                        inserted_detail = $dg.datagrid('getData');
                        if (!(typeof (inserted_detail) === 'undefined' || inserted_detail == null)) {
                            opts.data = data;
                            return true;
                        }
                    }
                }
                else {
                    if (!(typeof (inserted_detail.rows) === 'undefined' || inserted_detail.rows == null)) {
                        opts.data = data;
                        return true;
                    }
                }
            }
            return true;
        }
    }
}
//设置Combogrid 分页控件 简单模式
function SetCombogridPager(item) {
    var opts = $(item).combogrid('options');
    if (opts.pagination) {
        var cdgPager = $(item).combogrid('grid').datagrid('getPager');
        if (cdgPager) {
            $(cdgPager).pagination({
                layout: ['list', 'first', 'prev', 'next', 'last'],
                displayMsg: "{from}-{to} 共:{total}条"
            });
        }
    }
}