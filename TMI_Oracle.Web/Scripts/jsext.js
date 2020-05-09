//要查找 回车 下一个元素的 PaneId
var $divViewPanel;
//页面加载完毕为控件 添加 回车下一个 等功能
$(document).ready(function () {
    initEasyUIControl();
});

//本地combogrid-locale textbox 按钮弹起 设置空值（Bug）
//设置空值时，text清除但值未清除
function SetkeyUpVal(e) {
    var $this = $(this);
    var val = $this.val();
    if (ObjectIsEmpty(val)) {
        var vOpts = $this.validatebox('options');
        if(vOpts){
            var id = vOpts.id;
            $("#"+id).combo('setText', val);
            $("#"+id).combo('setValue', val);
        }
    }
}
//datagrid  combogrid键盘事件
var dg_keyHandler = function () {
    return {
        keyHandler: {
            up: function (e) {
                dg_combogrid_upKey(this, e);
            },
            down: function (e) {
                dg_combogrid_downKey(this, e);
            },
            enter: function (e) {
                var fieldName = '';
                var $txt = $(this);
                if ($(this).hasClass('textbox-text'))
                    $txt = $(this).parent().siblingd("input:first");
                fieldName = $txt.parents("td[field]:first()").attr("field");
                dg_combogrid_enterKey(this, e, $dg, fieldName, editIndex);
            },
            query: function (q) {
                dg_combogrid_query(this, q, editIndex, {});
            }
        }
    };
}();
//form combogrid键盘事件
var form_keyHandler = function () {
    return {
        up: function (e) {
            combogrid_upKey(this, e);
        },
        down: function (e) {
            combogrid_downKey(this, e);
        },
        enter: function (e) {
            combogrid_enterKey(this, e);
        },
        query: function (q, e) {
            $(this).data("LoadSuccess", false);
            var opts = $(this).combogrid('options');
            //console.log(q, opts);
            if (opts.mode == 'local' || (opts.url == null || opts.url == '')) {
                combogrid_local_query(this, q, e);
            }
            else {
                //if (opts.method == 'get') {
                //    //服务端获取数据 需 手写(参考如下)
                //    var opts = $(this).combogrid('options');
                //    var url = opts.url;
                //    if (url.indexOf('?') < 0) {
                //        url += "?";
                //    }
                //    else
                //        url += "&";
                //    $(this).combogrid('grid').datagrid('reload', url + 'q=' + q);
                //}
                //动态搜索
                var opts = $(this).combogrid('options')
                //设置查询参数
                var queryParams = {
                    page: 1,
                    rows: opts.pageSize,
                    q: q
                };
                $(this).combogrid("grid").datagrid('reload', queryParams);
                if (q == '')
                    $(this).combogrid('setValue', '');
                else {
                    $(this).combogrid('setValue', q);
                }
            }
        }
    };
}();
//datagrid 编辑时的 combogrid通用参数设置
var combogrid_DefaultSettings = function () {
    return {
        required: false,
        method: 'get',
        mode: 'remote',
        delay: 300,
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
            { field: 'ID', title: '代码', width: 70 },
            { field: 'TEXT', title: '名称', width: 150 }
        ]],
        onLoadSuccess: function (data) {
            $(this).data("LoadSuccess", true);
            //写在 beginEdit里，这里会被 覆盖掉,
            var onLS$dg = $(this);//datagrid
            var dgopts = onLS$dg.datagrid('options');
            var onLS$dgc = $('#' + dgopts.id);//combogrid
            //console.log(onLS$dg.data(), onLS$dgc.data());
            if (onLS$dgc.length > 1) {
                onLS$dgc.each(function () {
                    if ($(this).combogrid('grid') == onLS$dg) {
                        onLS$dgc = $(this);
                        return;
                    }
                });
            }
            var opts = onLS$dgc.combogrid('options');
            if (typeof opts.onChange === 'function') {
                setTimeout(function () {
                    var Selted = onLS$dg.datagrid('getSelected');
                    if (Selted) {
                        var valStr = '';
                        if (!(typeof (opts.valueField) === 'undefined' || opts.valueField == null)) {
                            valStr = opts.valueField;
                        } else if (!(typeof (opts.valuefield) === 'undefined' || opts.valuefield == null))
                            valStr = opts.valuefield;
                        else if (!(typeof (opts.Valuefield) === 'undefined' || opts.Valuefield == null))
                            valStr = opts.Valuefield;
                        opts.onChange.call(onLS$dgc, Selted[valStr]);
                    }
                }, 100);
            }
        }
    };
}();
//reload分页Combogrid
function reloadPageCombogrid(Comname) {
    var val = $("#" + Comname + "").combogrid('getValue');
    var opts = $("#" + Comname + "").combogrid('options');
    //设置查询参数
    var queryParams = {
        page: 1,
        rows: opts.pageSize,
        q: val
    };
    //opts.queryParams = queryParams;
    $("#" + Comname + "").combogrid("grid").datagrid('reload', queryParams);
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
//设置 targetEditor值
function setTargetVal(targetEditor, val) {
    //console.log(targetEditor);
    switch (targetEditor.type.toLowerCase()) {
    //switch ("combogrid") {
        case "combogrid":
            $(targetEditor.target).combogrid("setValue", val);
            var opts = $(targetEditor.target).combogrid('options')
            //设置查询参数
            var queryParams = {
                page: 1,
                rows: opts.pageSize,
                q: val
            };
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
                    if (OldVal == '是' || OldVal.toLowerCase() == 'true')
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
                    if (OldVal == '是')
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
//设置TextBox控件必填
function setTextBoxRequired($target) {
    var Required_TF = null;
    if (arguments.length > 1) {
        Required_TF = false;
        if (arguments[1])
            Required_TF = true;
    }
    var V$target = $target.parent().find('.validatebox-text:eq(0)');
    var v_opts = V$target.validatebox('options');
    if (typeof (Required_TF) === 'undefined' || Required_TF == null) {
        if (typeof (v_opts.required) === 'undefined' || v_opts.required == null || !v_opts.required) {
            v_opts.required = true;
            v_opts.novalidate = false;
        }
        else {
            v_opts.required = false;
            v_opts.novalidate = true;
        }
    } else {
        v_opts.required = Required_TF;
        v_opts.novalidate = !Required_TF;
    }
    V$target.validatebox('enableValidation').validatebox('validate');

    //V$target.bind('blur', function () {
    //    $(this).validatebox('enableValidation').validatebox('validate');
    //});
}
//设置扩展自Combo控件必填
function setComboRequired($target) {
    var Required_TF = null;
    if (arguments.length > 1) {
        Required_TF = false;
        if (arguments[1])
            Required_TF = true;
    }
    var cmb_opts = $target.combo('options');
    var v_opts = $target.combo('textbox').validatebox('options');
    //console.log('setComboRequired',cmb_opts, v_opts);
    if (typeof (Required_TF) === 'undefined' || Required_TF == null) {
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
    } else {
        v_opts.required = Required_TF;
        v_opts.novalidate = !Required_TF;
        v_opts.required = Required_TF;
        v_opts.novalidate = !Required_TF;
    }
    $target.combo('textbox').validatebox('enableValidation').validatebox('validate');
}
//设置 targetEditor必填
function setTargetRequired(target) {
    var Required_TF = null;
    if (arguments.length > 1) {
        Required_TF = false;
        if (arguments[1])
            Required_TF = true;
    }
    var typeofStr = typeof (target);
    var typeStr = '';
    var T_data = {};
    var $target;
    //console.log('setTargetRequired', target, typeofStr);
    if (typeofStr === 'undefined' || target == null || target == '') {
        T_data = null;
        $target = null;
    }
    else if (typeofStr === 'string') {
        if (target.indexOf("#")==0)
            $target = $(target);
        else
            $target = $("#"+target);
    }
    else {
        $target = $(target);
    }
    if ($target == null) {
        typeStr = '';
    } else {
        T_data = $target.data();
        if (!(typeof (T_data) === 'undefined' || target == null || target == '')) {
            for (var J_field in T_data) {
                if (!(typeof (J_field) === 'undefined' || J_field == null || J_field == '')) {
                    typeStr = J_field;
                    break;
                }
            }
        }
    }
    //console.log('setTargetRequired', $target, typeStr);
    switch (typeStr.toLowerCase()) {
        case "combogrid":
            setComboRequired($target, Required_TF);
            break;
        case "combobox":
            setComboRequired($target, Required_TF);
            break;
        case "textbox":
            setTextBoxRequired($target, Required_TF);
            break;
        case "numberbox":
            setTextBoxRequired($target, Required_TF);
            break;
        case "datebox":
            setTextBoxRequired($target, Required_TF);
            break;
        case "datetimebox":
            setTextBoxRequired($target, Required_TF);
            break;
    }
}
//获取参数
function getOptions(p$next) {
    var nextopts = {};
    if (typeof (p$next) === 'undefined' || p$next == null)
        return nextopts;
    var $next = null;
    if (typeof (p$next.target) === 'undefined' || p$next.target == null)
        $next = p$next;
    else
        $next = p$next.target;
    if ($next.data('textbox')) {
        nextopts = $next.textbox('options');
    }
    if ($next.data('datebox')) {
        nextopts = $next.datebox('options');
    }
    if ($next.data('datetimebox')) {
        nextopts = $next.datetimebox('options');
    }
    if ($next.data('numberbox')) {
        nextopts = $next.numberbox('options');
    }
    if ($next.data('combobox')) {
        nextopts = $next.combobox('options');
    }
    if ($next.data('combogrid')) {
        nextopts = $next.combogrid('options');
    }
    return nextopts;
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
    var dataRows = $(obj).combogrid('grid').datagrid('getRows');
    if (dataRows.length > 0) {
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
    } else {
        $(obj).siblings("span.textbox.combo").find("a.textbox-icon.combo-arrow").click();
    }
}
//combogrid keyhandler 事件 【回车键】押下处理
function combogrid_enterKey(obj, e) {
    var $combogridPanel = $(obj).combogrid('panel');
    var $trs = $combogridPanel.find('table.datagrid-btable:eq(0)').find('tr.datagrid-row');

    var Selted = $(obj).combogrid('grid').datagrid('getSelected');
    var val = $(obj).combogrid('getValue');
    var firstrow = { index: 0, row: {} };
    var datarows = $(obj).combogrid('grid').datagrid('getData').rows;
    if (datarows.length > 0) {
        var num = 0;
        while ($($trs[num]).hasClass('hide')) {
            num++;
        }
        if (num < datarows.length)
            firstrow = datarows[num];
    }
    //console.log('enter', obj, Selted, $divViewPanel);
    if (!Selted) {
        var LoadSuccess = $(obj).data('LoadSuccess');
        if (!(typeof val === 'undefined' || val == null || val == '') && (typeof LoadSuccess === 'undefined' || LoadSuccess == null || LoadSuccess)) {
            var q = $(obj).combogrid('getText');
            var val = $(obj).combogrid('getValue');
            if (datarows.length > 0) {
                Selted = firstrow;
                var rowindex = $(obj).combogrid('grid').datagrid('getRowIndex', Selted);
                $(obj).combogrid('grid').datagrid('selectRow', rowindex);
            }
        } else {
            //$(obj).combogrid('setValue', val || q);
            return false;
        }
    }
    else {
        //if (Selted != firstrow) {
        //    //console.log('含有值');
        //    Selted = firstrow;
        //    var rowindex = $(obj).combogrid('grid').datagrid('getRowIndex', Selted);
        //    $(obj).combogrid('grid').datagrid('selectRow', rowindex);
        //}
    }
    if (Selted) {
    	var ValField = $(obj).combogrid('options').valueField;
        var val = Selted[ValField];
        if (!(typeof (val) === 'undefined' || val == null || val == '')) {
            //设置【性别】文本框的内容为选中行的的性别字段内容
            $(obj).combogrid('setValue',val);
        }
    }
    //else
    //    console.log('combogrid_enterKey');
    //选中后让下拉表格消失
    $(obj).combogrid('hidePanel');

    var _tabIndex = $(obj).attr('tabindex');
    var tabIndex = parseInt(_tabIndex);
    var $divPanel = $divViewPanel;
    if (e && e.preventDefault) {
        e.preventDefault();
        var nextNum = tabIndex + 1;
        var $next = $('[tabindex=' + nextNum + ']:eq(0)');
        //console.log($divPanel, $next);
        //var $next = $divPanel.find('[tabindex=' + nextNum + ']:eq(0)');
        //if (typeof ($divPanel) === 'undefined' || $divPanel == null || $divPanel == "") {
        //    $next = $('[tabindex=' + nextNum + ']:eq(0)');
        //}
        if ($next) {
            var nextopts = getOptions($next);
            var times = 0;
            while ($next.attr('disabled') || nextopts.readonly) {
                nextNum++;
                times++;
                $next = $('[tabindex=' + nextNum + ']:eq(0)');
                if ($next) {
                    nextopts = getOptions($next);
                }
                else if (times > 20)
                    break;
            }
        }
        if ($next) {
            //console.log('combogrid_enterKey', $next);
            //setTimeout(function () {
            if ($next.data("textbox")) {
                var $dom = $($next).siblings('span.textbox,.easyui-fluid:eq(0)').children('input,textarea').get(0);
                $dom.focus();
            } else {
                $next.focus();
            }
            if (($($next).attr('type') == 'radio' || $($next).attr('type') == 'checkbox')) {
                $($next).prop("checked", true);
                $($next).focus();
            }
            //}, 100);
        }
    }
    else {
        window.event.returnValue = false;
    }

    return false;
}
//combogrid keyhandler 事件 按任意键 搜索（本地）
function combogrid_local_query(obj, q, e) {
    var opts = $(obj).combogrid('options');
    var data = $(obj).combogrid('grid').datagrid('getData');
    var $combogridPanel = $(obj).combogrid('panel');
    var $trs = $combogridPanel.find('table.datagrid-btable:eq(0)').find('tr.datagrid-row');
    $trs.removeClass('hide datagrid-row-checked datagrid-row-selected');
    if (typeof (q) === 'undefined' || q == null || q == "") {
        if (opts.pagination)
            $(":hidden[name=" + $(obj).attr('id') + "]").val("");
        $(obj).combogrid('setValue', '');
    }
    //console.log($trs);
    if (data) {
        if (opts.pagination)
            $(":hidden[name=" + $(obj).attr('id') + "]").val("");
        $.each(data.rows, function (i, item) {
            if ((item[opts.valueField] + '').indexOf(q) < 0 && (item[opts.textField] + '').indexOf(q) < 0) {
                if (opts.pagination) {
                    $($trs[i]).addClass('hide');
                }
                $($trs[i]).removeClass('datagrid-row-checked datagrid-row-selected');
            }
            else
            {
                if (item[opts.valueField] == q) {
                    $(obj).combogrid('grid').datagrid('selectRecord', q);
                    //$($trs[i]).addClass('datagrid-row-checked datagrid-row-selected');
                    //$(":hidden[name=" + $(obj).attr('id') + "]").val(q);
                    //var text = item[opts.textField];
                    //$(obj).combogrid('setText', text);
                    //$(obj).sblings('span:eq(0)').find('input:eq(0)').val(text);
                    //var data = $(obj).data();
                }
                //if (item[opts.textField] == q) {
                //    $($trs[i]).addClass('datagrid-row-checked datagrid-row-selected');
                //    $(":hidden[name=" + $(obj).attr('id') + "]").val($trs[i].val());
                //}
            }
        });
    }
}
//combogrid 打开面板
function combogrid_onShowPanel() {
    var obj = this;
    var datarows = $(obj).combogrid('grid').datagrid('getData').rows;
    var selected = $(obj).combogrid('grid').datagrid('getSelected');
    var opts = $(obj).combogrid('options');
    //console.log($(obj), selected);
    var q = "";
    var qtxt = $(obj).combogrid('getText');
    if (typeof (qtxt) === 'undefined' || qtxt == null || $.trim(qtxt) == '') {
        q = "";
        $(obj).combogrid('setValue', "");
    } else {
        q = $(obj).combogrid('getValue');
    }
    if (typeof (q) === 'undefined' || q == null || $.trim(q) == '')
        q = qtxt;
    if (typeof (q) === 'undefined' || q == null || $.trim(q) == '')
        q = '';

    if ((!selected && q != '') || datarows.length <= 0) {
        //console.log(opts);
        //设置查询参数
        var queryParams = {
            page: 1,
            rows: opts.pageSize,
            q: q
        };
        //动态搜索
        $(obj).combogrid('grid').datagrid('reload', queryParams);
    }
}
//combogrid 关闭面板
function combogrid_onHidePanel(){
	var $cdg = $(this);
	var Selted = $cdg.combogrid('grid').datagrid('getSelected');
	var opts = $cdg.combogrid('options');
	if(!Selted){
		var datarows = $cdg.combogrid('grid').datagrid('getData').rows;
		if (datarows.length <= 0) {
		    if (typeof ($cdg.data("LoadSuccess")) !== 'undefined')
		        $cdg.combogrid("clear");
		}
	} else {
	    //if (typeof opts.onChange === 'function') {
	    //    var valStr = '';
	    //    if (!(typeof (opts.valueField) === 'undefined' || opts.valueField == null)) {
	    //        valStr = opts.valueField;
	    //    } else if (!(typeof (opts.valuefield) === 'undefined' || opts.valuefield == null))
	    //        valStr = opts.valuefield;
	    //    else if (!(typeof (opts.Valuefield) === 'undefined' || opts.Valuefield == null))
	    //        valStr = opts.Valuefield;
	    //    opts.onChange.call($cdg, Selted[valStr]);
	    //}
	}
}
//继承自textbox的keydown事件 enter 下一个
function textbox_keydown(obj, event) {
    //debugger;
    var $target = $(obj).parent().siblings('input[class*=easyui]:eq(0)');
    var _tabIndex = $target.attr('tabindex');
    var tabIndex = parseInt(_tabIndex);
    //console.log('textbox',$(this), $target);
    if (!event) {
        event = window.event;
    }
    var keyCode = (event.keyCode || event.which);
    //回车事件
    if (keyCode == 13) {
        //debugger;
        var $divPanel = $divViewPanel;
        if (event && event.preventDefault) {
            var nextNum = tabIndex + 1;
            var $next = $('[tabindex=' + nextNum + ']:eq(0)');
            var txtdata = $target.data();
            var validtxtdata = $(obj).data();
            var multiline = null;
            if (!validtxtdata.combo)
                multiline = validtxtdata.validatebox.options.multiline;
            if (!(typeof (multiline) === 'undefined' || multiline == null || multiline == "")) {
                if (multiline) {
                    if (!event.ctrlKey)
                        return true;
                }
            }
            event.preventDefault();//不能注释，不然 datagrid 会自动刷新
            if (!ObjectIsEmpty(txtdata.datebox)) {
                var val = $target.datebox('getValue');
                if (typeof (val) === 'undefined' || val == null | val == '') {
                    $target.datebox('setValue', moment().format('YYYY-MM-DD'));
                }
                $tdata = $target.textbox("textbox");
                $tdata.trigger('change.Michael', { obj: $target });
            }
            if (!ObjectIsEmpty(txtdata.datetimebox)) {
                var val = $target.datetimebox('getValue');
                if (typeof (val) === 'undefined' || val == null | val == '') {
                    $target.datetimebox('setValue', moment().format('YYYY-MM-DD HH:mm:ss'));
                }
                $tdata = $target.textbox("textbox");
                $tdata.trigger('change.Michael', { obj: $target });
            }
            if ($next) {
                var nextopts = getOptions($next);
                var times = 0;
                //console.log(nextopts, nextopts.readonly == true);
                while ($next.attr('disabled') || nextopts.readonly) {
                    nextNum++;
                    times++;
                    $next = $('[tabindex=' + nextNum + ']:eq(0)');
                    //console.log($next);
                    if ($next) {
                        nextopts = getOptions($next);
                    }
                    else if (times > 20)
                        break;
                }
                if ($next) {
                    //console.log($next, $($next).siblings('span'), $($next).siblings('span:eq(0)').children('input'),$($next).siblings('span:eq(0)').children('input:eq(0)'));
                    if ($next.data("textbox")) {
                        var $dom = $($next).siblings('span.textbox,.easyui-fluid:eq(0)').children('input,textarea').get(0);
                        $dom.focus();
                    } else {
                        $next.focus();
                    }
                    if ($($next).attr('type') == 'radio' || $($next).attr('type') == 'checkbox') {
                        $($next).prop("checked", true);
                        $($next).focus();
                    }
                }
            }
        }
        else {
            window.event.returnValue = false;
        }
        return false;
        //debugger;
    }
}
//input 输入小写转大写
function inputKeydown2UpperCase(obj, parentObj) {
    if (!(typeof (parentObj) === "string")) {
        parentObj = "";
    }
    if (typeof (obj) === "string") {
        if(!ObjectIsEmpty(obj))
        {
            if (obj.substr(0, 1) != "#")
                obj = "#" + obj;
        }
    }
    $(obj, parentObj).unbind('compositionstart.Michael').bind('compositionstart.Michael', function (event) {
        console.log('compositionstart', event);
        $(this).data('ime', true);
    });
    $(obj, parentObj).unbind('compositionend.Michael').bind('compositionend.Michael', function (event) {
        console.log('compositionend', event);
        $(this).data('ime', null);
    });
    $(obj, parentObj).unbind('blur.Michael').bind('blur.Michael', function () { $(this).data('ime', null); });
    $(obj, parentObj).unbind('keydown.Michael').bind('keydown.Michael', function (event) {
        if (!event) {
            event = window.event;
        }
        var keyCode = (event.keyCode || event.which);
        var ime = $(this).data('ime');
        if (keyCode != 229 && keyCode >= 48 && keyCode <= 90) {
            if (ObjectIsEmpty(ime)) {
                var cursor_pos = getInputSelection(this);//光标区间位置
                //console.log(cursor_pos);
                var realkey = String.fromCharCode(keyCode);
                var isShift = event.shiftKey || (keyCode == 16) || false; // shift键是否按住
                if (isShift) {
                    if (!ObjectIsEmpty(cursor_pos)) {
                        this.value = this.value.substr(0, cursor_pos.start) + realkey.toLowerCase() + this.value.substr(cursor_pos.end);
                        setCaretPosition(this, cursor_pos.end + 1);
                    } else {
                        this.value += realkey.toLowerCase();
                    }
                }
                else {
                    if (!ObjectIsEmpty(cursor_pos)) {
                        this.value = this.value.substr(0, cursor_pos.start) + realkey.toUpperCase() + this.value.substr(cursor_pos.end);
                        setCaretPosition(this, cursor_pos.end + 1);
                    } else {
                        this.value += realkey.toUpperCase();
                    }
                }
            }
            event.preventDefault();//取消 冒泡
            event.returnValue = false;
            return false;
        }
    });
}
//keyup输入后转大写
function keyupToUpper(event) {
    if (!event) {
        event = window.event;
    }
    var keyCode = (event.keyCode || event.which);
    //console.log('keyupToUpper',keyCode);
    var ime = $(this).data('ime');
    if (keyCode != 229)//输入法占位符
    {
        if (ObjectIsEmpty(ime)) {
            var realkey = String.fromCharCode(keyCode);
            var isShift = event.shiftKey || (keyCode == 16) || false; // shift键是否按住
            if (isShift) {
                event.key = realkey.toLowerCase();
                this.value = this.value.toLowerCase();
            }
            else {
                event.key = realkey.toUpperCase();
                this.value = this.value.toUpperCase();
            }
        }
    }
    else {
        event.preventDefault();//取消 冒泡
        $(this).data('ime',null);
    }
}
//总单号加面罩
function MBLsetMask(obj) {
    //日期输入框点击事件
    if (!(typeof (obj) === 'undefined' || obj == null || obj == ''))
        var ArrEasyUITxt = $("input.easyui-textbox[id^='MBL']", "#" + obj);
    else
        var ArrEasyUITxt = $("input.easyui-textbox[id^='MBL']");
    if (ArrEasyUITxt.length > 0) {
        ArrEasyUITxt.each(function (i, item) {
            var $MBLTxt = $(item);
            var $RealMblTxt = $MBLTxt.textbox('textbox');
            $RealMblTxt.addClass('mask_MBL');
            //$MBLTxt.off('Michael.keydown').on('Michael.keydown', function (event) {
            //    if (!event) {
            //        event = window.event;
            //    }
            //    var $this = $(this);
            //    //var KeyCode = (event.keyCode || event.which);
            //    textbox_keydown($this, event);
            //});
        });
        $("input.mask_MBL").mask('XXX-XXXX XXXX', {
            'translation': {
                X: { pattern: /[0-9]/ },
                optional: true
            }
        }).off("blur.Michael").on("blur.Michael", function (event) {
            var $txt = $(this);
            var showTFMsg = $txt.data('showTFMsg');
            var txtmbl = $txt.val();
            if (validataMBL(txtmbl, showTFMsg)) {

            }
        });
    }
}
//验证总单号，是否符合规则
function validataMBL(txtmbl,showTFMsg) {
    var mbl = txtmbl;
    if (mbl != null && mbl != "") {
        var $strmbl = mbl.replace(/-/, "");
        var strmbl = $strmbl.replace(/ /, "");
        if (strmbl.length != 11) {
            if (!ObjectIsEmpty(showTFMsg)) {
                $.messager.alert("提示", "总单号输入不符合规则！");
            }
            return false;
        } else {
            var mblnum = strmbl.substr(3, 7);
            var remainder = mblnum % 7;
            var last = $strmbl.substr($strmbl.length - 1, 1);
            if (last != remainder.toString()) {
                if (!ObjectIsEmpty(showTFMsg)) {
                    $.messager.alert("提示", "总单号输入不符合规则！");
                }
                return false;
            }
        }
    }
    return true;
}
//EasyUI控件 回车下一个
function initEasyUIControl(obj) {
    //要查找 回车 下一个元素的 PaneId
    $divViewPanel = $(".easyui-panel:eq(0)");
    if ($divViewPanel.length <= 0) {
        $divViewPanel = null;
    }
    //获取各种控件
    if (!(typeof (obj) === 'undefined' || obj == null || obj == '')) {
        var EasyUITextbox = $('.easyui-textbox', "#" + obj);
        var EasyUIDatebox = $('.easyui-datebox', "#" + obj);
        var EasyUIDateTimebox = $('.easyui-datetimebox', "#" + obj);
        var EasyUINumberbox = $('.easyui-numberbox', "#" + obj);
        var EasyUIPasswordbox = $('.easyui-passwordbox', "#" + obj);
        var EasyUIFilebox = $('.easyui-filebox', "#" + obj);
        var EasyUICombobox = $('.easyui-combobox', "#" + obj);
        var EasyUICombogrid = $('.easyui-combogrid', "#" + obj);
        var EasyUIRadio = $('input[type=radio]', "#" + obj);
        var EasyUICheckbox = $('input[type=checkbox]', "#" + obj);
        //console.log('-----');
    } else {
        var EasyUITextbox = $('.easyui-textbox');
        var EasyUIDatebox = $('.easyui-datebox');
        var EasyUIDateTimebox = $('.easyui-datetimebox');
        var EasyUINumberbox = $('.easyui-numberbox');
        var EasyUIPasswordbox = $('.easyui-passwordbox');
        var EasyUIFilebox = $('.easyui-filebox');
        var EasyUICombobox = $('.easyui-combobox');
        var EasyUICombogrid = $('.easyui-combogrid');
        var EasyUIRadio = $('input[type=radio]');
        var EasyUICheckbox = $('input[type=checkbox]');
    }
    //总单号加面罩
    MBLsetMask(obj);
    var keydown_Name = "keydown.Michael";
    EasyUITextbox.each(function (i, item) {
        var data = $(item).data();
        data.textbox.options.inputEvents.keydown = function (event) {
            textbox_keydown(this, event);
        };
        var $tb = data.textbox.textbox.find(".textbox-text:eq(0)");
        $tb.off(keydown_Name).on(keydown_Name, function (event) {
            textbox_keydown(this, event);
        });
        //if($vdbox.length>0){
        //    $vdbox.data().validatebox.options.inputEvents.keydown = function (event) {
        //        textbox_keydown(this, event);
        //    };
        //}
        //$(item).textbox({
        //    inputEvents: $.extend({}, $.fn.textbox.defaults.inputEvents, {
        //        keydown: function (event) {
        //            textbox_keydown(this, event);
        //        }
        //    })
        //});
    });
    //日期控件
    EasyUIDatebox.each(function (i, item) {
        var data = $(item).data();
        var Datebox_Opts = data.datebox.options;//$(item).datebox('options');
        var Datebox_ComboOpts = data.combo.options; //$(item).combo("options");
        var $dateComboTxtbox = $($(item).combo("textbox"));
        var Datebox_ComboTxtOpts = $dateComboTxtbox.validatebox('options');
        Datebox_Opts.buttons = Datebox_buttons;

        Datebox_ComboOpts.onHidePanel = Datebox_onHidePanel;
        $dateComboTxtbox.off(keydown_Name).on(keydown_Name, function (event) {
            textbox_keydown(this, event);
        });
        var dateVal =$dateComboTxtbox.val();
        if (!(typeof (dateVal) === "undefined" || dateVal == null || dateVal == '')) {
            var MDate = new moment(dateVal);
            if (!MDate.isValid())
                MDate = new moment();
            data.datebox.calendar.calendar("options").current = MDate.toDate();
        }
        $dateComboTxtbox.off("keyup.Michael").on("keyup.Michael", Datebox_keyup);
        //$dateComboTxtbox.bind('keydown', function (event) {
        //    textbox_keydown(this, event);
        //});
        //$dateComboTxtbox.bind('keyup', Datebox_keyup);
        ////$dateComboTxtbox.on('blur', Datebox_blur);
    });
    //日期输入框 点击事件 默认yyyy- 以及加面罩yyyy-mm-dd
    NullSetDefaultDateBox(obj);
    //日期+时间控件
    EasyUIDateTimebox.each(function (i, item) {
        var data = $(item).data();
        var Datebox_Opts = $(item).datebox('options');
        var Datebox_ComboOpts = $(item).combo("options");
        var $dateComboTxtbox = $($(item).combo("textbox"));
        var Datebox_ComboTxtOpts = $dateComboTxtbox.validatebox('options');
        Datebox_Opts.buttons = Datebox_buttons;

        Datebox_ComboOpts.onHidePanel = Datebox_onHidePanel;
        $dateComboTxtbox.off(keydown_Name).on(keydown_Name, function (event) {
            textbox_keydown(this, event);
        });
        var dateVal = $dateComboTxtbox.val();
        if (!(typeof (dateVal) === "undefined" || dateVal == null || dateVal == '')) {
            var MDate = new moment(dateVal);
            if(!MDate.isValid())
                MDate = new moment();
            data.datebox.calendar.calendar("options").current = MDate.toDate();
        }
        $dateComboTxtbox.off("keyup.Michael").on("keyup.Michael", Datebox_keyup);
        //$dateComboTxtbox.bind('keydown', function (event) {
        //    textbox_keydown(this, event);
        //});
        //$dateComboTxtbox.bind('keyup', Datebox_keyup);
        ////$dateComboTxtbox.bind('blur', Datebox_blur);
    });
    //数字控件
    EasyUINumberbox.each(function (i, item) {
        var data = $(item).data();
        data.numberbox.options.inputEvents.keydown = function (event) {
            textbox_keydown(this, event);
        };
        data.textbox.options.inputEvents.keydown = function (event) {
            textbox_keydown(this, event);
        };
        var $tb = data.textbox.textbox.find(".textbox-text:eq(0)");
        $tb.off(keydown_Name).on(keydown_Name, function (event) {
            textbox_keydown(this, event);
        });
        
        //$(item).numberbox({
        //    inputEvents: $.extend({}, $.fn.numberbox.defaults.inputEvents, {
        //        keydown: function (event) {
        //            textbox_keydown(this, event);
        //        }
        //    })
        //});
    });
    //密码控件
    EasyUIPasswordbox.each(function (i, item) {
        var data = $(item).data();
        data.Passwordbox.options.inputEvents.keydown = function (event) {
            textbox_keydown(this, event);
        };
        data.textbox.options.inputEvents.keydown = function (event) {
            textbox_keydown(this, event);
        };
        var $tb = data.textbox.textbox.find(".textbox-text:eq(0)");
        $tb.off(keydown_Name).on(keydown_Name, function (event) {
            textbox_keydown(this, event);
        });
        //$(item).passwordbox({
        //    inputEvents: $.extend({}, $.fn.passwordbox.defaults.inputEvents, {
        //        keydown: function (event) {
        //            textbox_keydown(this, event);
        //        }
        //    })
        //});
    });
    //文件上传控件
    EasyUIFilebox.each(function (i, item) {
        var data = $(item).data();
        data.Filebox.options.inputEvents.keydown = function (event) {
            textbox_keydown(this, event);
        };
        data.textbox.options.inputEvents.keydown = function (event) {
            textbox_keydown(this, event);
        };
        var $tb = data.textbox.textbox.find(".textbox-text:eq(0)");
        $tb.off(keydown_Name).on(keydown_Name, function (event) {
            textbox_keydown(this, event);
        });
        //$(item).filebox({
        //    inputEvents: $.extend({}, $.fn.filebox.defaults.inputEvents, {
        //        keydown: function (event) {
        //            textbox_keydown(this, event);
        //        }
        //    })
        //});
    });
    //下拉控件
    EasyUICombobox.each(function (i, item) {
        var combobox_keydown = function (event) {
            //debugger;
            var $target = $(this).parent().siblings('.easyui-combobox:eq(0)');
            var _tabIndex = $target.attr('tabindex');
            var tabIndex = parseInt(_tabIndex);
            //console.log('combobox',$(this), $target);
            $target.combobox('showPanel');
            //debugger;
            //取得选中行
            var opts = $target.combobox('options');
            if (!opts) {
                $target.combobox('showPanel');
            }
            if (!event) {
                event = window.event;
            }
            //回车事件
            if ((event.keyCode || event.which) == 13) {
                //debugger;
                //console.log('enter', $target.combobox('panel'));
                $target.combobox('hidePanel');
                var $divPanel = $divViewPanel;//$('#' + divViewPanelId);
                if (event && event.preventDefault) {
                    event.preventDefault();
                    var $next = $('[tabindex=' + (tabIndex + 1) + ']:eq(0)');
                    if ($next) {
                        var nextopts = getOptions($next);
                        var times = 0;
                        while ($next.attr('disabled') || nextopts.readonly) {
                            nextNum++;
                            times++;
                            $next = $('[tabindex=' + nextNum + ']:eq(0)');
                            if ($next) {
                                nextopts = getOptions($next);
                            }
                            else if (times > 10)
                                break;
                        }
                    }
                    if ($next) {
                        //console.log('combogrid_enterKey', $next);
                        //setTimeout(function () {
                        $($next).siblings('span:eq(0)').children('input:eq(0)').focus();
                        if (($($next).attr('type') == 'radio' || $($next).attr('type') == 'checkbox')) {
                            $($next).prop("checked", true);
                        }
                        //}, 100);
                    }
                }
                else {
                    window.event.returnValue = false;
                }
                return false;
                //debugger;
            }
            //up
            if ((event.keyCode || event.which) == 38) {
                //console.log('up');
                var data = opts.data;
                if (data.length > 0) {
                    //取得选中值
                    var selected = $target.combobox('getValue');
                    if (selected.length > 0) {
                        for (var i = 0; i < data.length; i++) {
                            if (selected == data[i][opts.valueField]) {
                                if (i > 0) {
                                    $target.combobox('setValue', data[i - 1][opts.valueField]);
                                }
                            }
                        }
                    }
                }
            }
            //dowm
            if ((event.keyCode || event.which) == 40) {
                //console.log('down');
                var data = opts.data;
                if (data.length > 0) {
                    //取得选中值
                    var selected = $target.combobox('getValue');
                    if (selected.length > 0) {
                        for (var i = 0; i < data.length; i++) {
                            if (selected == data[i][opts.valueField]) {
                                if (i != data.length - 1) {
                                    $target.combobox('setValue', data[i + 1][opts.valueField]);
                                }
                            }
                        }
                    }
                    else {
                        $target.combobox('setValue', data[0][opts.valueField]);
                    }
                }
            }
        };
        //$(item).combobox({
        //    inputEvents: $.extend({}, $.fn.combobox.defaults.inputEvents, {
        //        keydown: combobox_keydown
        //    })
        //    //,onShowPanel: function () {
        //    //    var obj = this;
        //    //    var ComBoData = $(obj).combobox('getData');
        //    //    var combotxt = $(obj).combobox('getText');
        //    //    var comboval = $(obj).combobox('getValue');
        //    //    //console.log(this, $(obj), ComBoData, selected);
        //    //    if (!comboval) {
        //    //        var opts = $(obj).combobox('options');
        //    //        //console.log(opts);
        //    //        //设置查询参数
        //    //        var queryParams = {
        //    //            page: 1,
        //    //            rows: opts.pageSize,
        //    //            q: $(obj).combobox('getText')
        //    //        };
        //    //        //动态搜索
        //    //        $(obj).combobox('grid').datagrid('load', queryParams);
        //    //    }
        //    //}
        //});
        var data = $(item).data();
        data.combobox.options.inputEvents.keydown = function (event) {
            combobox_keydown(this, event);
        };
        data.textbox.options.inputEvents.keydown = function (event) {
            combobox_keydown(this, event);
        };
        var $tb = data.textbox.textbox.find(".textbox-text:eq(0)");
        $tb.off(keydown_Name).on(keydown_Name, function (event) {
            textbox_keydown(this, event);
        });
    });
    //下拉面板控件
    EasyUICombogrid.each(function (i, item) {
        var $item = $(item);
        var opts = $item.combogrid('options');
        if (opts) {
            opts.keyHandler = form_keyHandler;
            opts.onShowPanel = combogrid_onShowPanel;
            opts.onHidePanel = combogrid_onHidePanel;
            //设置Combogrid 分页控件 简单模式
            SetCombogridPager(item);
            if (opts.mode == 'local') {
                //opts.onChange = function () {
                //    console.log('-----',this);
                //};
                //var $txt = $item.combogrid('textbox');
                //$txt.bind('keydown', function (e) {
                //    var val = $(this).val();
                //    if (ObjectIsEmpty(val)) {
                //        $(this).combo('setText', val);
                //    }
                //});
            }
        }
    });
    //单选控件
    EasyUIRadio.each(function (i, item) {
        $(item).off(keydown_Name).on(keydown_Name,function (event) {
            var $target = $(this);
            var _tabIndex = $target.attr('tabindex');
            var tabIndex = parseInt(_tabIndex); if (!event) {
                event = window.event;
            }
            //回车事件
            if ((event.keyCode || event.which) == 13) {
                //debugger;
                $target.prop("checked", true);
                var $divPanel = $divViewPanel;
                if (event && event.preventDefault) {
                    event.preventDefault();
                    var nextNum = tabIndex + 1;
                    var $next = $('[tabindex=' + (tabIndex + 1) + ']:eq(0)');
                    if ($next) {
                        var nextopts = getOptions($next);
                        var times = 0;
                        while ($next.attr('disabled') || nextopts.readonly) {
                            nextNum++;
                            times++;
                            $next = $('[tabindex=' + nextNum + ']:eq(0)');
                            if ($next) {
                                nextopts = getOptions($next);
                                break;
                            }
                            else if (times > 20)
                                break;
                        }
                        if ($next) {
                            if ($next.data("textbox")) {
                                var $dom = $($next).siblings('span.textbox,.easyui-fluid:eq(0)').children('input,textarea').get(0);
                                $dom.focus();
                            } else {
                                $next.focus();
                            }
                            if (($($next).attr('type') == 'radio' || $($next).attr('type') == 'checkbox')) {
                                $($next).focus();
                            }
                        }
                    }
                }
                else {
                    window.event.returnValue = false;
                }
                return false;
                //debugger;
            }
        });
    });
    //多选控件
    EasyUICheckbox.each(function (i, item) {
        $(item).off(keydown_Name).on(keydown_Name,function (event) {
            var $target = $(this);
            var _tabIndex = $target.attr('tabindex');
            var tabIndex = parseInt(_tabIndex); if (!event) {
                event = window.event;
            }
            //回车事件
            if ((event.keyCode || event.which) == 13) {
                //debugger;
                $target.prop("checked", true);
                var $divPanel = $divViewPanel;
                if (event && event.preventDefault) {
                    event.preventDefault();
                    var nextNum = tabIndex + 1;
                    var $next = $('[tabindex=' + (tabIndex + 1) + ']:eq(0)');
                    if ($next) {
                        var nextopts = getOptions($next);
                        var times = 0;
                        while ($next.attr('disabled') || nextopts.readonly) {
                            nextopts = getOptions($next);
                            nextNum++;
                            times++;
                            $next = $('[tabindex=' + nextNum + ']:eq(0)');
                            if ($next) {
                                break;
                            }
                            else if (times > 20)
                                break;
                        }
                        if ($next) {
                            if ($next.data("textbox")) {
                                var $dom = $($next).siblings('span.textbox,.easyui-fluid:eq(0)').children('input,textarea').get(0);
                                $dom.focus();
                            } else {
                                $next.focus();
                            }
                            if (($($next).attr('type') == 'radio' || $($next).attr('type') == 'checkbox')) {
                                $($next).focus();
                            }
                        }
                    }
                }
                else {
                    window.event.returnValue = false;
                }
                return false;
                //debugger;
            }
        });
    });
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
//设置ComboGrid真实url(为了不页面进入后 不立即加载数据，优化页面载入速度)
function resetCombogridUrl(obj, VirtualUrlName, IsReLoad, data, isSimplePager) {
    var reload = false;
    var ArrComboGrid = [];
    if (typeof (VirtualUrlName) === 'undefined' || VirtualUrlName == null || VirtualUrlName == "") {
        VirtualUrlName = "url1";
    }
    if (typeof (IsReLoad) === 'undefined' || IsReLoad == null || IsReLoad == "") {
        reload = false;
    }
    else {
        if (IsReLoad)
            reload = true;
        else
            reload = false;
    }

    if (!(typeof (obj) === 'undefined' || obj == null || obj == "")) {
        if (typeof (obj) == "object" || obj.indexOf('#') == 0)
            ArrComboGrid = $(".easyui-combogrid", obj);
        else
            ArrComboGrid = $(".easyui-combogrid", "#" + obj);
    }
    else {
        ArrComboGrid = $(".easyui-combogrid");
    }
    $.each(ArrComboGrid, function (i, elem) {
        var $elem = $(elem);
        var value = $elem.combogrid('getValue');
        var opts = $elem.combogrid('options');
        var gridopts = $elem.combogrid('grid').datagrid('options');
        opts.url = gridopts.url = opts[VirtualUrlName];
        //if(reload && (typeof(value)==='undefined' || value==null || value=='')){
        if (reload) {
            reloadPageCombogrid($(elem).attr("Id"));
        } else {
            if (!(typeof (data) === 'undefined' || data == null)) {
                var Id_attr = $elem.attr('id');
                var Name_attr = $elem.attr('name');
                var elemdata = data[Id_attr];
                var elemNAMEdata = data[Id_attr + 'NAME'];
                if (typeof (elemdata) === 'undefined' || elemdata == null || elemdata == '') {
                    if ((typeof (Name_attr) === 'undefined' || Name_attr == null || Name_attr == '') || $elem.css("display") == 'none') {
                        if ($elem.attr("class").indexOf('easyui') >= 0)
                            Name_attr = $elem.attr("textboxname");
                    }
                    elemdata = data[Name_attr];
                    elemNAMEdata = data[Name_attr + 'NAME'];
                }
                if (typeof (elemdata) != undefined && elemdata != null && elemdata != '')
                    if (!(typeof (elemNAMEdata) === 'undefined' || elemNAMEdata == null || elemNAMEdata == ''))
                        $(elem).combogrid('setText', elemNAMEdata);
            }
        }
        if (typeof (isSimplePager) === 'undefined' || isSimplePager == null || isSimplePager) {
            //设置Combogrid 分页控件 简单模式
            SetCombogridPager(elem);
        }
    });
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
                    var valHSCODE = getTargetVal(targetHSCODE);
                    if (typeof (targetHSCODE) === 'undefined' || targetHSCODE == null) {
                        try {
                            valHSCODE = $dg_.datagrid('getData').row[rowIndex][fieldHSCODE];
                        } catch (e) {
                            valHSCODE = "";
                        }
                    }
                    $.ajax({
                        type: 'Get',
                        url: '/PARA_HSCODES/GetCurrG_ModelType',//获取数据的函数
                        async: true,//true 异步，false 同步
                        dataType: "json",
                        contentType: "application/json; charset=utf-8",
                        data: { G_Model: valStr, HSCODE: valHSCODE },//查询条件JSON.stringify()
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
                                        $dialog.dialog('center');
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
        $.messager.alert("错误", '规格型号类型，必须填写完整');
        return;
    }
    else {
        for (var x in G_ModelTypeVals) {
            var item = G_ModelTypeVals[x];
            if (item.G_ModelType != "其他" && (typeof item.Value === 'undefined' || item.Value == null || item.Value == '')) {
                $.messager.alert("错误", '规格型号类型，必须填写完整(除 其他 类型以外)');
                return;
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
                $(TargetEditor.target).textbox("setValue", TypeValStr);
            } else {
                var Str = TypeValStr.substr(0, pmStr.length);
                if (Str.indexOf(pmStr) >= 0) {
                    $.messager.alert("错误", '第一个规格型号类型，不能是品名');
                    return;
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
//combogrid keyhandler 事件【向上键】押下处理
function dg_combogrid_upKey(obj, e) {
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
function dg_combogrid_downKey(obj, e) {
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
//combogrid query 事件 按任意键 搜索（本地）
function dg_combogrid_local_query(obj, q, e) {
    var opts = $(obj).combogrid('options');
    var data = $(obj).combogrid('grid').datagrid('getData');
    var $combogridPanel = $(obj).combogrid('panel');
    var $trs = $combogridPanel.find('table.datagrid-btable:eq(0)').find('tr.datagrid-row');
    $trs.removeClass('hide');
    //console.log($trs,data);
    if (data) {
        $.each(data.rows, function (i, item) {
            if (item[opts.valueField].indexOf(q) < 0 && item[opts.textField].indexOf(q) < 0) {
                $($trs[i]).addClass('hide');
            }
        });
    }
}
//combogrid query 事件 按任意键 搜索（本地）
function dg_combogrid_query(obj, q, rowIndex, queryparam) {
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
function dg_combogrid_enterKey(obj, e, $dg_, fieldName, editIndex) {
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
        else {
            return false;
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
//combobox keydown事件
function dg_combobox_keydown(obj, e, $dg_, rowindex) {
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
//继承自textbox的keydown事件 enter 下一个
function dg_Listtextbox_keydown(obj, event, $dg_, rowindex) {
    //return;
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
        //获取所有列名
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
                    while ($next == null || $next.target.attr('disabled') || nextopts.readonly ) {
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
            //48-57&96-105：数字0-9；
            //8：BackSpace；9：Tab；46：Delete
            //110：数字键盘./Delete；190：.>
            if (!(KeyCode == 8 || KeyCode == 9 || KeyCode == 46 || KeyCode == 110 || KeyCode == 190 || (KeyCode >= 48 && KeyCode <= 57) || (KeyCode >= 96 && KeyCode <= 105))) {
                //console.log('KeyCode', KeyCode);
                //37-40：左上右下，112-123：F1-F12
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
                //110：数字键盘./Delete；190：.>
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
                }
                //8：BackSpace；46：Delete
                else if (!(KeyCode == 8 || KeyCode == 46)) {
                    numberboxOpts = $target.target.numberbox('options');
                    if (numberboxOpts.precision) {
                        if (_pointNum >= 0) {
                            var objprecision = val.length - _pointNum - 1;
                            if (objprecision > numberboxOpts.precision) {
                                if (event && event.preventDefault)
                                    event.preventDefault();
                                else
                                    window.event.returnValue = false;
                            } else if (objprecision == numberboxOpts.precision) {
                                var pos = getCursortPosition($(obj)[0]);//光标所在位置
                                if (pos > _pointNum) {
                                    if (event && event.preventDefault)
                                        event.preventDefault();
                                    else
                                        window.event.returnValue = false;
                                }
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
//combobox keydown事件
function dg_combobox_keydown(obj, e, $dg_, rowindex) {
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
//checkbox或radio keydown事件
function dg_chk_rdo_keydown(obj, event, $dg_, rowindex) {
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
//grid中时间控件的处理。
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
/*DateBox 处理 Start*/
//空时 默认 yyyy- 格式 yyyy-mm-dd
function NullSetDefaultDateBox(obj) {
    //日期输入框点击事件
    if (!(typeof (obj) === 'undefined' || obj == null || obj == ''))
        var ArrDate_Txt = $("input.easyui-datebox", "#" + obj);
    else
        var ArrDate_Txt = $("input.easyui-datebox");
    var IsMasked = false;
    if (ArrDate_Txt.length > 0) {
        ArrDate_Txt.each(function (i, item) {
            $Date_box = $(item);
            var $txt = $Date_box.textbox("textbox");
            //var combo = $Date_box.data('combo');
            //var comboOpts = combo.options;
            if ($txt.data('mask'))
                IsMasked = true;
            else {
                $txt.addClass('mask_rq');
                $txt.unbind('blur').bind("blur", Datebox_blur);
                $txt.off("focus.Michael").on("focus.Michael", { $DateBox: $Date_box, $txt: $txt }, Date_Onfocus);
                //var date_txt = $txt.data();
                //console.log(date_txt.onchange);
                //var Changefunc = comboOpts.onChange;
                //$txt.off("change.Michael").on("change.Michael", function () {
                //    Changefunc.call(this);
                //});
            }
        });
        if (!IsMasked) {
            $("input.mask_rq").mask('XXXX-FX-CX', {
                'translation': {
                    X: { pattern: /[0-9]/ },
                    F: { pattern: /[0-2]/ },
                    C: { pattern: /[0-3]/ },
                    optional: true
                }
            });
        }
    }
}
//日期 默认 获取焦点时 设置当天日期
function Date_Onfocus(event) {
    if (event && event.data) {
        if (!ObjectIsEmpty(event.data.$DateBox)) {
            var $DateBox = event.data.$DateBox;
            var $txt = event.data.$txt;
            var DateStr = $DateBox.datebox("getValue");
            var MDate = new moment(DateStr);
            if (ObjectIsEmpty(DateStr) || !MDate.isValid()) {
                if ($txt) {
                    var myDate = new Date();
                    var str = myDate.getFullYear() + "-";
                    //$DateBox.datebox("setValue", str);
                    $txt.val(str);
                }
            }
        }
    }
}
//清除DateBox值
var ClearDateboxValue = function ($this, event) {
    if (event && event.preventDefault) {
        //event.preventDefault();//阻止触发事件的默认行为。
        //setTimeout(function () {
        var StrVal = $this.val();
        if (StrVal == '') {
            var $dateDiv = $this.parent().parent();
            var $dateInput = $dateDiv.find("input.easyui-datebox,.easyui-datetimebox");
            //console.log($this, $dateDiv, $dateInput);
            if ($dateInput.length > 0) {
                //var classNameStr = $dateInput.prop("className");
                if ($dateInput.hasClass('easyui-datebox')) {
                    //console.log($dateInput.datebox('getValue'));
                    //console.log('inputEvents-keyup-datebox', event, StrVal);
                    $dateInput.datebox('setValue', '');
                    //console.log($dateInput.datebox('getValue'));
                }
                if ($dateInput.hasClass('easyui-datetimebox')) {
                    //console.log($dateInput.datebox('getValue'));
                    //console.log('inputEvents-keyup-datetimebox', event, StrVal);
                    $dateInput.datetimebox('setValue', '');
                    //console.log($dateInput.datebox('getValue'));
                }
            }
        }
        //}, 100);
    }
    else {
        window.event.returnValue = false;
    }
}
//DateBox 键盘抬起事件
var Datebox_keyup = function (event) {
    if (!event) {
        event = window.event;
    }
    var $this = $(this);
    var KeyCode = (event.keyCode || event.which);
    ClearDateboxValue($this, event);
};
//验证Datebox值是否正确
var Datebox_Valid = function ($this) {
    //console.log('Datebox_Valid', $this);
    var StrTxt = '';
    var momentFormate = ["YYYY-MM-DD", "YYYY-MM-DD HH:mm", "YYYY-MM-DD HH:mm:ss", "YYYY-MM-DD HH:mm:sss"];
    var ObjData = $this.data();
    var valid = true;
    try {
        if (ObjData.combo) {
            var OCalendarNavHover = $(ObjData.combo.panel).find('td.calendar-nav-hover');
            if (OCalendarNavHover.length > 0) {
                var rgx = new RegExp(/,/g);
                var dataStr = OCalendarNavHover.attr('abbr').replace(rgx, '-');
                $this.datebox('setValue', dataStr);
                $this.datebox('hidePanel');
                valid = false;
            }
        }
    }
    catch (e) {

    }
    if (valid) {
        if (ObjData.datebox) {
            //var calddata = ObjData.datebox.calendar.data();
            var $txt = ObjData.textbox.textbox.find('input:first');
            var txtval = $txt.val();
            StrTxt = $this.datebox('getValue');
            if (StrTxt != '' || txtval != '') {
                if (StrTxt != txtval) {//值和显示不一致时，按显示赋值
                    StrTxt = txtval;
                } 
                //console.log('datebox', StrTxt, moment(StrTxt, momentFormate), moment(StrTxt, momentFormate, true).isValid());
                var momentDate = moment(StrTxt, momentFormate, true);
                if (!momentDate.isValid()) {
                    $this.datebox('setValue', '');
                    $txt.val('');
                }
                else 
                    $this.datebox('setValue', StrTxt);
            }
        }
        if (ObjData.datetimebox) {
            var $txt = ObjData.textbox.textbox.find('input:first');
            var txtval = $txt.val();
            StrTxt = $this.datetimebox('getValue');
            if (StrTxt != '' || txtval != '') {
                if (StrTxt != txtval) {//值和显示不一致时，按显示赋值
                    StrTxt = txtval;
                }
                //console.log('datetimebox', StrTxt, moment(StrTxt, momentFormate), moment(StrTxt, momentFormate, true).isValid());
                if (!moment(StrTxt, momentFormate, true).isValid()) {
                    $this.datetimebox('setValue', '');
                    $txt.val('');
                }
                else
                    $this.datetimebox('setValue', StrTxt);
            }
        }
    }
};
//Datebox 失去焦点事件
var Datebox_blur = function (event) {
    var $this = $(this);
    //if (!event) {
    //    event = window.event;
    //}
    //var KeyCode = (event.keyCode || event.which);
    //console.log('Datebox_blur', event, $this);
    var $dateDiv = $this.parent().parent();
    var $dateInput = $dateDiv.find("input.easyui-datebox,.easyui-datetimebox");
    //$dateInput.datebox('hidePanel');
    Datebox_Valid($dateInput);
};
//Datebox 关闭Panel事件
var Datebox_onHidePanel = function () {
    var $this = $(this);
    Datebox_Valid($this);
    //console.log('onHidePanel', StrTxt, this);
};
//Datebox 自定义清空日期事件
var Datebox_buttons = $.extend([], $.fn.datebox.defaults.buttons);
Datebox_buttons.splice(1, 0, {
    text: '清空',
    handler: function (target) {
        var $target = $(target);
        var nextopts = {};
        if ($target.data('datebox')) {
            $target.datebox('setValue', '');
        }
        if ($target.data('datetimebox')) {
            $target.datetimebox('setValue', '');
        }
    }
});
/*DateBox 处理 End*/

/**
* From扩展
* getData 获取数据接口
* 
* @param {Object} jq
* @param {Object} params 设置为true的话，会把string型"true"和"false"字符串值转化为boolean型。
*/
$.extend($.fn.form.methods, {
    getData: function (jq, params) {
        var formArray = jq.serializeArray();
        var oRet = {};
        for (var i in formArray) {
            if (typeof (oRet[formArray[i].name]) == 'undefined') {
                if (params) {
                    oRet[formArray[i].name] = (formArray[i].value == "true" || formArray[i].value == "false") ? formArray[i].value == "true" : formArray[i].value;
                }
                else {
                    oRet[formArray[i].name] = formArray[i].value;
                }
            }
            else {
                if (params) {
                    oRet[formArray[i].name] = (formArray[i].value == "true" || formArray[i].value == "false") ? formArray[i].value == "true" : formArray[i].value;
                }
                else {
                    oRet[formArray[i].name] += "," + formArray[i].value;
                }
            }
        }
        return oRet;
    }
});
//扩展easyui方法
$.extend($.fn.datagrid.methods, {
    //行号自适应宽度 行数据超过9999时，第一列的行号rownumber将会因为表格内容过长而导致无法显示全部数字
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
    },
    //列移动
    columnMoving: function (jq) {
        return jq.each(function () {
            var target = this;
            var dg_id = $(target).attr('id');
            var cells = $(this).datagrid('getPanel').find('div.datagrid-header td[field]');
            var opts = $(this).datagrid('options');//datagrid属性
            var columns = $(target).datagrid('options').columns;//datagrid 列
            var cc = columns[0];//datagrid 列顺序 列宽 编辑格式等
            opts.onResizeColumn = onResizeColumn;//修改列宽时触发
            //html5属性draggable-droppable
            cells.draggable({
                revert: true,
                cursor: 'pointer',
                edge: 5,
                proxy: function (source) {
                    var p = $('<div class="tree-node-proxy tree-dnd-no" style="position:absolute;border:1px solid #ff0000"/>').appendTo('body');
                    p.html($(source).text());
                    p.hide();
                    return p;
                },
                onBeforeDrag: function (e) {
                    e.data.startLeft = $(this).offset().left;
                    e.data.startTop = $(this).offset().top;
                },
                onStartDrag: function () {
                    $(this).draggable('proxy').css({
                        left: -10000,
                        top: -10000
                    });
                },
                onDrag: function (e) {
                    $(this).draggable('proxy').show().css({
                        left: e.pageX + 15,
                        top: e.pageY + 15
                    });
                    return false;
                }
            }).droppable({
                accept: 'td[field]',
                onDragOver: function (e, source) {
                    $(source).draggable('proxy').removeClass('tree-dnd-no').addClass('tree-dnd-yes');
                    $(this).css('border-left', '1px solid #ff0000');
                },
                onDragLeave: function (e, source) {
                    $(source).draggable('proxy').removeClass('tree-dnd-yes').addClass('tree-dnd-no');
                    $(this).css('border-left', 0);
                },
                onDrop: function (e, source) {
                    $(this).css('border-left', 0);
                    var fromField = $(source).attr('field');
                    var toField = $(this).attr('field');
                    setTimeout(function () {
                        swapField(fromField, toField);
                        $(target).datagrid();
                        $(target).datagrid('columnMoving');
                    }, 0);
                }
            });

            // swap Field to another location
            function swapField(from, to) {
                _swap(from, to);
                function _swap(fromfiled, tofiled) {
                    var fromtemp;
                    var totemp;
                    var fromindex = 0;
                    var toindex = 0;
                    for (var i = 0; i < cc.length; i++) {
                        if (cc[i].field == fromfiled) {
                            fromindex = i;
                            fromtemp = cc[i];
                        }
                        if (cc[i].field == tofiled) {
                            toindex = i;
                            totemp = cc[i];
                        }
                    }
                    cc.splice(fromindex, 1, totemp);
                    cc.splice(toindex, 1, fromtemp);
                    //产生一个 新的数组
                    var ArrFieldNoWidth = cc.filter(function (item) {
                        return item;
                    });
                    //设置cookie和本地存储
                    setdg_FieldNoWidth(dg_id, ArrFieldNoWidth);
                }
            }
            //修改列宽时触发（记录到cookie）
            function onResizeColumn(field, width) {
                var $dg = $(this);
                var dg_id = $dg.attr('id');
                var columns = $(target).datagrid('options').columns;
                var cc = columns[0];
                var ArrFieldNoWidth = getdg_FieldNoWidth(dg_id);
                if (ObjectIsEmpty(ArrFieldNoWidth)) {
                    //产生一个 新的数组
                    ArrFieldNoWidth = cc.filter(function (item) {
                        if (item.field == field)
                            item.width = width;
                        return item;
                    });
                } else {
                    $.each(ArrFieldNoWidth,function (i,item) {
                        if (item.field == field) {
                            item.width = width;
                        }
                    });
                }
                //设置cookie和本地存储
                setdg_FieldNoWidth(dg_id, ArrFieldNoWidth);
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
//获取光标位置函数
function getCursortPosition(ctrl) {
    var CaretPos = 0;	// IE Support
    if (document.selection) {
        ctrl.focus();
        var Sel = document.selection.createRange();
        Sel.moveStart('character', -ctrl.value.length);
        CaretPos = Sel.text.length;
    }
    // Firefox support
    else if (ctrl.selectionStart || ctrl.selectionStart == '0')
        CaretPos = ctrl.selectionStart;
    return (CaretPos);
}
//设置光标位置函数
function setCaretPosition(ctrl, pos) {
    if (ctrl.setSelectionRange) {
        ctrl.focus();
        ctrl.setSelectionRange(pos, pos);
    }
    else if (ctrl.createTextRange) {
        var range = ctrl.createTextRange();
        range.collapse(true);
        range.moveEnd('character', pos);
        range.moveStart('character', pos);
        range.select();
    }
}
//获取控件选中值
function getInputSelection(el) {
    var start = 0, end = 0, normalizedValue, range, textInputRange, len, endRange;
    //判断是否可直接取出选择索引
    if (typeof el.selectionStart == "number" && typeof el.selectionEnd == "number") {
        start = el.selectionStart;
        end = el.selectionEnd;
    } else {
        range = document.selection.createRange();
        if (range && range.parentElement() == el) {
            len = el.value.length;
            normalizedValue = el.value.replace(/\r\n/g, "\n");
            // Create a working TextRange that lives only in the input
            textInputRange = el.createTextRange();
            textInputRange.moveToBookmark(range.getBookmark());
            // Check if the start and end of the selection are at the very end
            // of the input, since moveStart/moveEnd doesn't return what we want
            // in those cases
            endRange = el.createTextRange();
            endRange.collapse(false);

            if (textInputRange.compareEndPoints("StartToEnd", endRange) > -1) {
                start = end = len;
            } else {
                start = -textInputRange.moveStart("character", -len);
                start += normalizedValue.slice(0, start).split("\n").length - 1;

                if (textInputRange.compareEndPoints("EndToEnd", endRange) > -1) {
                    end = len;
                } else {
                    end = -textInputRange.moveEnd("character", -len);
                    end += normalizedValue.slice(0, end).split("\n").length - 1;
                }
            }
        }
    }
    //返回
    return {
        start: start,
        end: end
    };
}
//获取form-Json数据
//function getFormSerializeJson(fromid) {
//    var retdata = {};
//    var data = {};
//    try{
//        if (typeof fromid === 'object') {
//            data = fromid.serializeArray();
//        } else {
//            if (!(fromid == null || fromid == '')) {
//                if (fromid.indexOf("#") >= 0)
//                    data = $(fromid).serializeArray();
//                else
//                    data = $("#" + fromid).serializeArray();
//            } else
//                return retdata;
//        }
//        for (var i in data) {
//            if (ObjectIsEmpty(retdata[data[i].name]))
//                retdata[data[i].name] = data[i].value;
//            else
//                retdata[data[i].name] += "," + data[i].value;
//        }
//        return retdata;
//    } catch (e) {
//        console.log(e);
//        return {};
//    }
//}
//异步获取 提取 借用账册 强行提取 选择框
var ArrIsLoadWin = {
    IsLoadWin : false
};
function AsyncGetMyWin(url, IsLoadMyWin, InitMyWinFucName, Async_Load, postObj) {
    var AsyncLoad = false;
    if (!(typeof (Async_Load) === 'undefined' || Async_Load == null || Async_Load == "")) {
        if (Async_Load)
            AsyncLoad = true;
    }
    
    if ((typeof (postObj) === 'undefined' || postObj == null || !ObjectIsJson(postObj))) {
        postObj = {};
    }
    if (!ArrIsLoadWin[IsLoadMyWin]) {
        $.messager.progress({
            text: '正在加载数据......'
        });
        var TabIndexMax = $("input[tabindex]:not([id^='_easyui']):last");
        if (TabIndexMax.length > 0) {
            postObj.Num = TabIndexMax.attr('tabindex');
        }
        var retTF = true;
        $.ajax({
            type: "POST",
            datatype: "html",
            async: Async_Load,
            data: postObj,
            url: url,
            success: function (data) {
                ArrIsLoadWin[IsLoadMyWin] = true;
                var docm = $("#EasyUIDom");
                if (data) {
                    setTimeout(function(){
                        docm.append(data);
                        $.parser.onComplete = function (contxt) {
                            try{
                                //InitMyWinFuc();
                                eval(InitMyWinFucName);//js 方式
                                //$(this)[InitMyWinFucName](); //jquery方式$(this).trigger(InitMyWinFucName)
                            } catch (e) {
                                $.messager.progress('close');
                                console.log('jsext-AsyncGetMyWin-eval', e);
                            }
                        };
                        try{
                            $.parser.parse(docm);
                            $.messager.progress('close');
                        } catch (e) {
                            $.messager.progress('close');
                            $.messager.alert('警告',"获取数据，出现错误！");
                            console.log(e);
                        }
                    },10);
                }
                else{
                    retTF = false;
                    $.messager.progress('close');
                }
            },
            error: function () {
                retTF = false;
                $.messager.progress('close');
            }
        });
        return retTF;
    }
    else {
        return true;
    }
}

//扩展easyui-datagrid默认属性
$.extend($.fn.datagrid.defaults, {
    onBeforeLoad: function (param) {
        try {
            var $dg = $(this);
            var dg_id = $dg.attr('id');
            var pageSize = getdgDefaultPageSize(dg_id);
            if (param.rows != pageSize) {
                param.rows = pageSize;
                var data = $dg.data();
                if (data) {
                    data.datagrid.options.pageSize = pageSize;
                    var $pagination = $dg.datagrid('getPager');
                    var pagedata = $pagination.data();
                    if (pagedata) {
                        pagedata.pagination.options.pageSize = pageSize;
                        pagedata.pagination.options.total = data.datagrid.data.total;
                        var ps = $pagination.find("select.pagination-page-list");
                        if (ps.length) {
                            ps.val(pageSize + "");
                        }
                        if (ObjectIsEmpty(ps.val())) {
                            var $TopOption = $pagination.find("select.pagination-page-list option:first");
                            var txt = $TopOption.text();
                            if (!isNaN(txt)) {
                                pageSize = parseInt(txt);
                                data.datagrid.options.pageSize = pageSize;
                                pagedata.pagination.options.pageSize = pageSize;
                                $TopOption.prop("selected", true);
                                setdgDefaultPageSize(dg_id, pageSize);
                            }
                        }
                    }
                }
            }
        } catch (e) {
            console.log('jsext-datagrid.defaults-onBeforeLoad', e);
        }
    },
    pageList: [10, 100, 200, 500, 1000],
    pageSize: 10
});
//扩展easyui-pagination默认属性
$.extend($.fn.pagination.defaults, {
    onChangePageSize: function (pageSize) {
        try{
            var $dg = $(this).parent().find("table.easyui-datagrid:eq(0)");
            var dg_id ="";
            if ($dg.length > 0) {
                dg_id = $dg.attr('id');
            }
            setdgDefaultPageSize(dg_id, pageSize);
        } catch (e) {
            console.log('jsext-pagination-onChangePageSize', e);
        }
    }
});
////localStorage 操作
//localStorage.setItem("key","value");                   // 存储变量名为key，值为value的变量
//localStorage.key = "value"                             // 同setItem方法，存储数据
//var valueLocal = localStorage.getItem("key");          // 读取存储变量名为key的值
//var valueLocal = localStorage.key;                     // 同getItem，读取数据
//localStorage.removeItem('key');                        // removeItem方法，删除变量名为key的存储变量
//localStorage.clear();                                  // clear方法，清除所有保存的数据
//// 利用length属性和key方法，遍历所有的数据
//for(var i = 0; i < localStorage.length; i++)
//{
//    console.log(localStorage.key(i));
//}
//// 存储 localStorage 数据为 Json 格式
//value = JSON.stringify(jsonValue);                     // 将 JSON 对象 jsonValue 转化成字符串
//localStorage.setItem("key", value);                    // 用 localStorage 保存转化好的的字符串
//// 读取 localStorage 中 Json 格式数据
//var value = localStorage.getItem("key");              // 取回 value 变量
//jsonValue = JSON.parse(value);                        // 把字符串转换成 JSON 对象

//测试Js 函数重载
var TestFuc = { a: 'a', b: 'b', c: 'c' };
function addMethod(object, name, fn) {
    var old = object[name]; //把前一次添加的方法存在一个临时变量old里面
    //参数被另一个函数引用，不会被回收
    object[name] = function () { // 重写了object[name]的方法
        // 如果调用object[name]方法时，传入的参数个数跟预期的一致，则直接调用
        if (fn.length === arguments.length) {
            return fn.apply(this, arguments);
            // 否则，判断old是否是函数，如果是，就调用old
        } else if (typeof old === "function") {
            return old.apply(this, arguments);
        }
    }
}
addMethod(TestFuc, 'testfunc', function () {
    console.log('0-arguments');
});
addMethod(TestFuc, 'testfunc', function (a) {
    console.log(arguments.length + '-arguments', a);
});
addMethod(TestFuc, 'testfunc', function (a, b) {
    console.log(arguments.length + '-arguments', a, b);
});
addMethod(TestFuc, 'testfunc', function (a, b, c) {
    console.log(arguments.length + '-arguments', a, b, c);
});
//TestFuc.testfunc();
/*---------------------------------require.js 声明输出---------------------------------*/
//define(function ()
//{
//    function fun1()
//    {
//        alert("it works");
//    }

//    fun1();
//});
//define(function (require, exports, modules)
//{
//    // do something
//    var m = {
//        'color': 'red',
//        'size': '13px'
//    };
//    exports.default({ m: m });
//});