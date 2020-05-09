/*
module.exports可用时，按module.exports 规范输出
符合amd规范时，按amd规范define输出
exports 可用时 按照exports输出
window 输出 相当于var
*/
!function (t, e)
{
    "object" == typeof exports && "object" == typeof module ? module.exports = e() :
    "function" == typeof define && define.amd ? define([], e) : "object" == typeof exports ? exports.ActualMoney = e() : t.ActualMoney = e()
}(this, function ()
{
    var LazyLoadingComponent = require('LazyLoadingComponent');//组件懒加载func
    var ActualMoney = {
        props: {
            orderid: {
                type: Number,
                default: 0,
            },
            tb_actualmoney_data: {
                type: Array,
                required: true,
            },
            user_roles: {
                type: Object,
                required: true,
                validator: function (value)
                {
                    var IsError = false;
                    if (ArrKey.length > 0) {
                        var ArrKey = Object.keys(value);
                        var Roles = ['Edit', 'Create', 'Delete', 'Import', 'Export'];
                        Roles.forEach(function (item, i)
                        {
                            if (!ArrKey.includes(item)) {
                                IsError = true;
                                return false;
                            }
                        });
                    }
                    return IsError;
                }//验证
            },
            formlabel_width: {
                type: String,
                default: '120px'
            },
        },
        mounted: function ()
        {
            if (!ObjectIsEmpty(this.tb_actualmoney_data)) {
                var addNum = parseInt(this.tb_actualmoney_data.addNum);
                this.addNum = isNaN(addNum) ? 0 : addNum;
            }//记录上次渲染时，新增数据Num
            var thisVue = this;
            this.ArrEnumField.forEach(function (item)
            {
                thisVue.el_remoteMethod('', item, 'form', true);
            });//外键触发搜索初始化
            console.log('mounted', this);
        },//相当于构造函数，渲染完dom后触发
        data: function ()
        {
            var data = {
                addNum: 0,//新增序号
                OrderId: this.orderid,//订单Id
                tb_ActualMoney_data: this.tb_actualmoney_data,//当前列表数据
                formLabelWidth: this.formlabel_width,//Label宽度
                ActualMoney: {},//当前编辑行
                UserRoles: this.user_roles,//权限
                DialogVisible: false,//弹出框显示
                dlgLoading: false,//编辑弹出框加载中
                tbLoading: false,//数据列表加载中
                selctRows: [],//选择的数据
                el_selt: {
                    el_selt_loading: false,//选择框 搜索状态
                },//select数据
                ArrEnumField: [
					{ Name: 'SupplierName', ForeignKeyGetListUrl: '/SupplierS/GetPagerSupplier_FromCache' },
					{ Name: 'ServiceProject', ForeignKeyGetListUrl: '/BD_DEFDOC_LISTS/GetPager_DEFDOC_DICT_FromCache?DOCCODE=ServiceProject' },
                ],//所有Select
            }
            return data;
        },
        methods: {
            el_FormFieldRules: function (rowConfig, isSearchForm)
            {
                //是否搜索form
                var t_isSearchForm = typeof (isSearchForm)
                if (t_isSearchForm === 'undefined' || isSearchForm == null || t_isSearchForm !== 'boolean')
                    isSearchForm = false;
                var ArrRules = [];
                if (!rowConfig.Editable)
                    return ArrRules;
                if (rowConfig.Required && !isSearchForm) {
                    ArrRules.push({ required: true, message: '请输入' + rowConfig.DisplayName || rowConfig.Name, trigger: ['blur', 'change'] });
                }
                var name = rowConfig.Name.toLowerCase()
                if (name == 'email' || rowConfig.isEmail)
                    ArrRules.push({ type: 'email', message: '请输入正确的邮箱地址', trigger: ['blur', 'change'] });
                if (name.indexOf('password') == 0)
                    ArrRules.push({ validator: this.$Validtors.PasswordValidator, trigger: ['blur', 'change'] });
                if (name.indexOf('idcard') == 0 && rowConfig.inputType == 'text')
                    ArrRules.push({ validator: this.$Validtors.IdCardValidator, trigger: 'blur' });

                if (rowConfig.MinLength || rowConfig.MaxLength) {
                    var rule = { trigger: ['blur', 'change'] }
                    if (rowConfig.MinLength) {
                        rule.min = rowConfig.MinLength;
                        if (rowConfig.MaxLength)
                            rule.message = '字符长度必须介于 ' + rowConfig.MinLength + ' 到 ' + rowConfig.MaxLength + ' 之间';
                        else {
                            rule.message = '字符长度 必须大于 ' + rowConfig.MinLength;
                        }
                    }
                    if (rowConfig.MaxLength) {
                        rule.max = rowConfig.MaxLength;
                        if (rowConfig.MinLength)
                            rule.message = '字符长度 必须介于 ' + rowConfig.MinLength + ' 到 ' + rowConfig.MaxLength + ' 之间';
                        else
                            rule.message = '字符长度 必须小于 ' + rowConfig.MaxLength;
                    }

                    ArrRules.push(rule);
                }
                return ArrRules;
            },//输出input验证规则
            handleAddRow: function (e)
            {
                console.log('handleAddRow', e);
                var NewRow = { Id: --this.addNum, OrderId: this.OrderId };
                this.DialogVisible = true;
                this.ActualMoney = NewRow;
                this.dlgLoading = false;//编辑弹出框加载中
                this.tb_ActualMoney_data.push(NewRow);
                this.tb_ActualMoney_data.addNum = this.addNum;//记录上次添加数-<keep-alive>
            },//增加行数据 弹出框添加
            handledblclick: function (row)
            {
                this.DialogVisible = true;
                //this.curr_rowdata_Original = row;//原始行数据
                //this.ActualMoney = Object.assign({}, row);
                this.ActualMoney = row;
                let curr_rowdata = this.ActualMoney;
                //let ArrEnumField = this.ArrEnumField;//所有select/枚举
                let thisVue = this;
                Object.keys(curr_rowdata).forEach(function (item, index)
                {
                    let val = curr_rowdata[item] + '';
                    if (!ObjectIsEmpty(val)) {
                        if (val.indexOf('/Date(') >= 0) {
                            var d = new moment(val);
                            if (d.isValid())
                                curr_rowdata[item] = d.toDate();
                        }
                        var ArrFilter = thisVue.ArrEnumField.filter(function (field) { return field.Name === item; });
                        if (ArrFilter.length > 0) {
                            let OFilter = ArrFilter[0];
                            let url = OFilter.ForeignKeyGetListUrl;//'/MenuItems/GetData';
                            if (!ObjectIsEmpty(url) && url.indexOf('GetPagerEnum') < 0)
                                thisVue.el_remoteMethod('', OFilter, 'form', true);
                        }
                    }
                });
                console.log('row-dblclick', row);
            },//双击行
            handledelSeltRow: function (e)
            {
                //console.log('handledelSeltRow', e, this.selctRows);
                if (this.selctRows.length <= 0) {
                    this.$message({
                        duration: 0,//不自动关闭
                        showClose: true,
                        message: '错误:未选择需要删除的数据',
                        type: 'error'
                    });
                } else {
                    var thisVue = this;
                    thisVue.tbLoading = true;//加载中
                    var deltRowIndex = this.tb_ActualMoney_data.map(function (item, i)
                    {
                        var has_el = thisVue.selctRows.some(function (el, x) { return el.Id === item.Id; })
                        if (has_el)
                            return i;
                        else return null;
                    });
                    deltRowIndex = deltRowIndex.filter(function (item) { return item != null });
                    deltRowIndex = deltRowIndex.reverse();
                    if (deltRowIndex.length >= 0) {
                        deltRowIndex.forEach(function (ArrIdx)
                        {
                            let KeyId = thisVue.tb_ActualMoney_data[ArrIdx].Id;
                            thisVue.tb_ActualMoney_data.splice(ArrIdx, 1);
                            thisVue.$root.ActualMoney.deltRows.push(KeyId);
                        });
                    }
                    thisVue.tbLoading = false;//加载中
                }
                //rows.splice(index, 1);
            },//批量删除选中行数据
            handleSelectionChange: function (selections)
            {
                this.selctRows = selections;
                console.log('handleSelectionChange', selections);
            },//选择数据变更
            dlgok_func: function ()
            {
                let thisVue = this;
                let MyForm = this.$refs['ActualMoneyForm'];
                //MyForm.resetFields();//清除验证
                MyForm.clearValidate();//清除验证
                MyForm.validate(function (valid)
                {
                    if (valid) {
                        thisVue.DialogVisible = false;
                        thisVue.$emit('dlgok_func');
                    }
                    else {
                    }
                });
            },
            formatter: function (field)
            {//el-table-column 数据显示转换
                var formatter = null;
                switch (field.Type) {
                    case 'boolean':
                        formatter = this.$formatter.boolformatter;
                        break;
                    case 'date':
                        formatter = this.$formatter.dateformatter;
                        break;
                    case 'datetime':
                        formatter = this.$formatter.datetimeformatter;
                        break;
                    default:
                        formatter = null;
                        break;
                }
                var lower_Name = field.Name.toLowerCase();
                if (lower_Name.indexOf('sex') == 0) {
                    formatter = this.$formatter.Sexformatter;
                }
                if (!ObjectIsEmpty(field.ForeignKeyGetListUrl)) {
                    formatter = this.$formatter.joinformatter;
                }
                //if (lower_Name.indexOf('photo') >= 0){
                //    formatter = this.$formatter.photoformatter;
                //}
                return formatter;
            },//el-table-column 数据显示转换
            ExportXls: function (JsonData, fileName)
            {
                //console.log('ExportXls');
                require(['xlsx', 'file-saver'], function (XLSX, FileSaver)
                {
                    var sheetName = "帐户";
                    let wb = XLSX.utils.book_new();  //工作簿对象包含一SheetNames数组，以及一个表对象映射表名称到表对象。XLSX.utils.book_new实用函数创建一个新的工作簿对象。
                    let ws = XLSX.utils.json_to_sheet(JsonData);
                    wb.SheetNames.push(sheetName)
                    wb.Sheets[sheetName] = ws
                    const defaultCellStyle = { font: { name: "Verdana", sz: 13, color: "FF00FF88" }, fill: { fgColor: { rgb: "FFFFAA00" } } };//设置表格的样式
                    let wopts = { bookType: 'xlsx', bookSST: false, type: 'binary', cellStyles: true, defaultCellStyle: defaultCellStyle, showGridLines: false }  //配置参数和样式

                    //let wb = XLSX.utils.table_to_book(thisVue.$refs['Mytb']);
                    /* get binary string as output */
                    let wbout = XLSX.write(wb, wopts);
                    try {
                        const s2ab = function (s)
                        {//字符串转字符流
                            if (typeof ArrayBuffer !== 'undefined') {
                                var buf = new ArrayBuffer(s.length)
                                var view = new Uint8Array(buf)
                                for (var i = 0; i != s.length; ++i) view[i] = s.charCodeAt(i) & 0xff
                                return buf
                            } else {
                                var buf = new Array(s.length);
                                for (var i = 0; i != s.length; ++i) buf[i] = s.charCodeAt(i) & 0xFF;
                                return buf;
                            }
                        }
                        FileSaver.saveAs(new Blob([s2ab(wbout)], { type: 'application/octet-stream' }), fileName + '.xlsx');
                    } catch (e) {
                        if (typeof console !== 'undefined')
                            console.log(e, wbout)
                    }
                    return wbout;
                });
            },//导出数据
            ImportXls: function ()
            {

            },//导入数据
            el_remoteMethod: function (query, field, profx, forceload)
            {
                let ArrOptionName = field.Name + '_' + profx;
                if (!ObjectIsEmpty(query) || !ObjectIsEmpty(forceload)) {
                    this.el_selt.el_selt_loading = true;
                    var paramData = { filterRules: JSON.stringify([{ field: "q", op: 'equals', value: query }]) };
                    //let thisVue = this;
                    let url = field.ForeignKeyGetListUrl;//'/MenuItems/GetData';
                    this.$http.get(url, {
                        params: paramData,
                        headers: {//指示为 ajax请求
                            "X-Requested-With": "XMLHttpRequest"
                        }
                    }).then(function (success)
                    {//成功
                        try {
                            if (typeof this.el_selt[ArrOptionName] === 'undefined')
                                this.$set(this.el_selt, ArrOptionName, {});
                            if (ObjectIsEmpty(success.body.rows)) {
                                this.$set(this.el_selt[ArrOptionName], "ArrOption", success.body);
                            } else
                                this.$set(this.el_selt[ArrOptionName], "ArrOption", success.body.rows);
                        } catch (e) {
                            this.$message({
                                duration: 0,//不自动关闭
                                showClose: true,
                                message: '数据处理，出现错误',
                                type: 'error'
                            });
                        }
                        this.el_selt.el_selt_loading = false;//加载完毕
                    }, function (error)
                    {//错误
                        this.el_selt.el_selt_loading = false;//加载完毕
                        this.$message({
                            duration: 0,//不自动关闭
                            showClose: true,
                            message: '获取数据出现错误',
                            type: 'error'
                        });
                    });
                } else {
                    if (typeof this.el_selt[ArrOptionName] === 'undefined')
                        this.el_selt[ArrOptionName] = {};
                    this.el_selt[ArrOptionName]["ArrOption"] = [];
                }
            },//外键触发搜索
            SupplierNameChange: function (value)
            {
                if (!ObjectIsEmpty(value)) {
                    var ArrOptionData = this.el_selt["SupplierName_form"];
                    if (!(ObjectIsEmpty(ArrOptionData) || JSON.stringify(ArrOptionData) == "{}")) {
                        var ArrOption = ArrOptionData.ArrOption;
                        if (!ObjectIsEmpty(ArrOption)) {
                            var QOption = ArrOption.filter(function (item) { return item.ID == value });
                            if (QOption.length > 0) {
                                var OOption = QOption[0];
                                this.$set(this.ActualMoney, 'SupplierNameNAME', OOption.TEXT);
                            }
                        }
                    }
                } else {
                    this.$set(this.ActualMoney, 'SupplierNameNAME', '');
                }
            },//供应商变更
            ServiceProjectChange: function (value)
            {
                if (!ObjectIsEmpty(value)) {
                    var ArrOptionData = this.el_selt["ServiceProject_form"];
                    if (!(ObjectIsEmpty(ArrOptionData) || JSON.stringify(ArrOptionData) == "{}")) {
                        var ArrOption = ArrOptionData.ArrOption;
                        if (!ObjectIsEmpty(ArrOption)) {
                            var QOption = ArrOption.filter(function (item) { return item.ID == value });
                            if (QOption.length > 0) {
                                var OOption = QOption[0];
                                this.$set(this.ActualMoney, 'ServiceProjectNAME', OOption.TEXT);
                            }
                        }
                    }
                } else {
                    this.$set(this.ActualMoney, 'ServiceProjectNAME', '');
                }
            },//服务项目变更
            dlgClose: function ()
            {
                let thisVue = this;
                let MyForm = this.$refs['ActualMoneyForm'];
                //MyForm.resetFields();//重置字段验证
                MyForm.clearValidate();//清除验证
                MyForm.validate(function (valid)
                {
                    if (valid) {
                        thisVue.DialogVisible = false;
                        thisVue.$emit('dlgok_func');
                    }
                    else {
                        thisVue.$confirm('实际成本,验证错误, 强制新增?', '提示', {
                            confirmButtonText: '确定',
                            cancelButtonText: '取消',
                            type: 'warning'
                        }).then(function ()
                        {
                            thisVue.DialogVisible = false;
                            thisVue.$emit('dlgok_func');
                        }).catch(function ()
                        {
                            thisVue.DialogVisible = false;
                            var ActualMoney = thisVue.ActualMoney;
                            var del_index = null;
                            thisVue.tb_ActualMoney_data.forEach(function (item, idx)
                            {
                                if (item.Id == ActualMoney.Id) {
                                    del_index = idx;
                                    return false;
                                }
                            });
                            if (del_index != null) {
                                thisVue.tb_ActualMoney_data.splice(del_index, 1);
                            }
                        });
                    }
                });
            },//弹出框关闭
        },
        watch: {//监听属性变化
            "ActualMoney.Price": {
                handler: function (val, oldVal)
                {
                    var NewTotalAmount = val * this.ActualMoney.Num;//单价*数量
                    //var ChangeAmonunt = NewTotalAmount - this.ActualMoney.TotalAmount;//变化值
                    this.ActualMoney.TotalAmount = NewTotalAmount;//金额
                }
            },
            "ActualMoney.Num": {
                handler: function (val, oldVal)
                {
                    var NewTotalAmount = this.ActualMoney.Price * val;//单价*数量
                    //var ChangeAmonunt = NewTotalAmount - this.ActualMoney.TotalAmount;//变化值
                    this.ActualMoney.TotalAmount = NewTotalAmount;//金额
                }
            },
            orderid: {
                handler: function (newval, oldval) {
                    this.OrderId = newval;
                },
                immediate: true,
                deep: true
            },
            tb_actualmoney_data: {
                handler: function (newval, oldval)
                {
                    this.tb_ActualMoney_data = newval;
                },
                immediate: true,
                deep: true
            }
        },//监听属性变化
        template: '' +
        '<templete id="dlg_ActualMoney">' +
        '    <el-row style="padding: 3px 0px 3px 0px;"> <!--上右下左-->' +
        '        <el-col>' +
        '            <el-button-group>' +
        '                <el-button type="primary" icon="el-icon-plus" size="small" v-bind:disabled="!UserRoles.Create" v-on:click="handleAddRow">新增</el-button>' +
        '                <el-button type="danger" size="small" v-on:click="handledelSeltRow" v-bind:disabled="(UserRoles.Delete ? selctRows.length===0 : true)">批量删除</el-button>' +
        '            </el-button-group>' +
        '        </el-col>' +
        '    </el-row><!--列表按钮组-->' +
        '    <el-row>' +
        '        <el-col>' +
        '            <el-table ref="tb_ActualMoney" size="mini" style="width: 100%" max-height="500" row-key="Id" border stripe' +
        '                      v-bind:default-sort="{prop:\'Id\',order:\'descending\'}"' +
        '                      v-bind:data="tb_ActualMoney_data"' +
        '                      v-loading="tbLoading"' +
        '                      v-on:row-dblclick="handledblclick"' +
        '                      v-on:selection-change="handleSelectionChange">' +
        '                <el-table-column fixed type="selection" width="36"></el-table-column>' +
        '                <template>' +
        '                    <el-table-column show-overflow-tooltip prop="SupplierName" label="供应商名称" sortablev-bind:formatter="formatter({Type: \'string\', Name: \'SupplierName\'})" width="120px"></el-table-column>' +
        '                    <el-table-column show-overflow-tooltip prop="ServiceProject" label="服务项目" sortablev-bind:formatter="formatter({Type: \'string\', Name: \'ServiceProject\',ForeignKeyGetListUrl:true})" width="120px"></el-table-column>' +
        '                    <el-table-column show-overflow-tooltip prop="Price" label="单价" sortablev-bind:formatter="formatter({Type: \'number\', Name: \'Price\'})" width="80px"></el-table-column>' +
        '                    <el-table-column show-overflow-tooltip prop="Num" label="数量" sortablev-bind:formatter="formatter({Type: \'number\', Name: \'Num\'})" width="70px"></el-table-column>' +
        '                    <el-table-column show-overflow-tooltip prop="TotalAmount" label="金额" sortablev-bind:formatter="formatter({Type: \'number\', Name: \'TotalAmount\'})" width="90px"></el-table-column>' +
        '                    <el-table-column show-overflow-tooltip prop="Remark" label="备注" sortablev-bind:formatter="formatter({Type: \'string\', Name: \'Remark\'})" width="*"></el-table-column>' +
        '                </template>' +
        '            </el-table><!--Table列表-->' +
        '        </el-col>' +
        '    </el-row>' +
        '    <el-dialog ref="ActualMoneyDialog" width="60%" center append-to-body ' +
        '               v-bind:close-on-click-modal="false"' +
        '               v-bind:show-close="false"' +
        '               v-bind:visible.sync="DialogVisible"' +
        '               v-bind:before-close="(done)=>{dlgClose(done)}"' +
        '               v-loading="dlgLoading">' +
        '        <div slot="title" class="el-dialog__title" style="">' +
        '            <el-row>' +
        '                <el-col v-bind:span="8" style="cursor:move;">&nbsp;</el-col>' +
        '                <el-col v-bind:span="8" style="cursor:move;">实际成本</el-col>' +
        '                <el-col v-bind:span="8" style="text-align:right;">' +
        '                    <el-button type="primary" icon="el-icon-check" size="mini" v-bind:disabled="!UserRoles.Edit" v-on:click="dlgok_func" title="确 定" circle></el-button>' +
        '                    <el-button type="danger" icon="el-icon-close" size="mini" v-on:click="dlgClose" title="取 消" circle></el-button>' +
        '                </el-col>' +
        '            </el-row>' +
        '        </div>' +
        '        <el-form ref="ActualMoneyForm" v-bind:model="ActualMoney" label-position="right" inline size="small">' +
        '            <el-form-item v-bind:label-width="formLabelWidth" label="供应商名称" prop="SupplierName"' +
        '                          v-bind:rules="el_FormFieldRules({Name:\'SupplierName\',DisplayName:\'供应商名称\',Required:true,Editable:true,MinLength:0,MaxLength:50})">' +
        '                <el-select v-model="ActualMoney[\'SupplierName\']" style="width:178px" reserve-keyword clearable' +
        '                           v-bind:loading="el_selt.el_selt_loading" v-on:change="SupplierNameChange">' +
        '                    <template v-if="el_selt.SupplierName_form">' +
        '                        <el-option v-for="item in el_selt.SupplierName_form.ArrOption"' +
        '                                   v-bind:key="item.ID|filterInt"' +
        '                                   v-bind:label="item.TEXT"' +
        '                                   v-bind:value="item.ID|filterInt">' +
        '                        </el-option>' +
        '                    </template>' +
        '                </el-select>' +
        '            </el-form-item>' +
        '            <el-form-item v-bind:label-width="formLabelWidth" label="服务项目" prop="ServiceProject"' +
        '                          v-bind:rules="el_FormFieldRules({Name:\'ServiceProject\',DisplayName:\'服务项目\',Required:true,Editable:true,MinLength:0,MaxLength:100})">' +
        '                <el-select v-model="ActualMoney[\'ServiceProject\']" style="width:178px" reserve-keyword clearable' +
        '                           v-bind:loading="el_selt.el_selt_loading" v-on:change="ServiceProjectChange">' +
        '                    <template v-if="el_selt.ServiceProject_form">' +
        '                        <el-option v-for="item in el_selt.ServiceProject_form.ArrOption"' +
        '                                   v-bind:key="item.ID|filterInt"' +
        '                                   v-bind:label="item.TEXT"' +
        '                                   v-bind:value="item.ID|filterInt">' +
        '                        </el-option>' +
        '                    </template>' +
        '                </el-select>' +
        '            </el-form-item>' +
        '            <el-form-item v-bind:label-width="formLabelWidth" label="单价" prop="Price"' +
        '                          v-bind:rules="el_FormFieldRules({Name:\'Price\',DisplayName:\'单价\',Required:true,Editable:true,MinLength:0,MaxLength:0})">' +
        '                <el-input-number v-model="ActualMoney[\'Price\']" placeholder="单价" v-bind:precision="2" v-bind:style="{width:\'178px\'}" />' +
        '            </el-form-item>' +
        '            <el-form-item v-bind:label-width="formLabelWidth" label="数量" prop="Num"' +
        '                          v-bind:rules="el_FormFieldRules({Name:\'Num\',DisplayName:\'数量\',Required:true,Editable:true,MinLength:0,MaxLength:0})">' +
        '                <el-input-number v-model="ActualMoney[\'Num\']" placeholder="数量" v-bind:precision="2" v-bind:style="{width:\'178px\'}" />' +
        '            </el-form-item>' +
        '            <el-form-item v-bind:label-width="formLabelWidth" label="金额" prop="TotalAmount"' +
        '                          v-bind:rules="el_FormFieldRules({Name:\'TotalAmount\',DisplayName:\'金额\',Required:true,Editable:false,MinLength:0,MaxLength:0})">' +
        '                <el-input-number v-model="ActualMoney[\'TotalAmount\']" placeholder="金额" disabled="true" v-bind:precision="2" v-bind:style="{width:\'178px\'}" />' +
        '            </el-form-item>' +
        '            <el-form-item v-bind:label-width="formLabelWidth" label="备注" prop="Remark"' +
        '                          v-bind:rules="el_FormFieldRules({Name:\'Remark\',DisplayName:\'备注\',Required:false,Editable:true,MinLength:0,MaxLength:1000})">' +
        '                <el-input type="textarea" v-model="ActualMoney[\'Remark\']" v-bind:clearable="true" style="width:100%;" v-bind:style="{width:\'178px\'}" />' +
        '            </el-form-item>' +
        '        </el-form>' +
        '        <span slot="footer" class="dialog-footer">' +
        '            <el-button type="primary" icon="el-icon-check" v-bind:disabled="!UserRoles.Edit" v-on:click="dlgok_func">确 定</el-button>' +
        '            <el-button type="default" icon="el-icon-close" v-on:click="dlgClose">取 消</el-button>' +
        '        </span>' +
        '    </el-dialog><!--实际成本-编辑-->' +
        '</templete>',
    };
    return ActualMoney;
});