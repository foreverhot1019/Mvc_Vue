/*
module.exports可用时，按module.exports 规范输出
符合amd规范时，按amd规范define输出
exports 可用时 按照exports输出
window 输出 相当于var
*/
!function (t, e) {
    "object" == typeof exports && "object" == typeof module ? module.exports = e() :
    "function" == typeof define && define.amd ? define([], e) : "object" == typeof exports ? exports.PlanePerson = e() : t.PlanePerson = e()
}(this, function () {
    var LazyLoadingComponent = require('LazyLoadingComponent');
    var PlanePerson = {
        props: {
            airticketorderid: {
                type: Number,
                default: 0,
            },
            tb_planeperson_data: {
                type: Array,
                required: true,
            },
            formlabel_width: {
                type: String,
                default: '120px'
            },
            plane_person: Object,
            user_roles: {
                type: Object,
                required: true,
                validator: function (value) {
                    var IsError = false;
                    if (ArrKey.length > 0) {
                        var ArrKey = Object.keys(value);
                        var Roles = ['Edit', 'Create', 'Delete', 'Import', 'Export'];
                        Roles.forEach(function (item, i) {
                            if (!ArrKey.includes(item)) {
                                IsError = true;
                                return false;
                            }
                        });
                    }
                    return IsError;
                }//验证
            }
        },
        mounted: function () {
            if (!ObjectIsEmpty(this.tb_planeperson_data)) {
                var addNum = parseInt(this.tb_planeperson_data.addNum);
                this.addNum = isNaN(addNum) ? 0 : addNum;
            }//记录上次渲染时，新增数据Num
            var thisVue = this;
            //this.ArrEnumField.forEach(function (item) {
            //    thisVue.el_remoteMethod('', item, 'form', true);
            //});//外键触发搜索初始化
            console.log('mounted', this);
        },//相当于构造函数，渲染完dom后触发
        data: function () {
            var data = {
                addNum: 0,//新增序号
                AirTicketOrderId: this.airticketorderid,//订单Id
                tb_PlanePerson_data: this.tb_planeperson_data,//当前列表数据
                formLabelWidth: this.formlabel_width,//Label宽度
                //PlanePerson_Data:this.actualmoney_data,
                PlanePerson: {},//this.plane_person,//当前编辑行
                UserRoles: this.user_roles,//权限
                DialogVisible: false,//弹出框显示
                dlgLoading: false,//编辑弹出框加载中
                tbLoading: false,//数据列表加载中
                selctRows: [],//选择的数据
                InputExportInfo:"",
                el_selt: {
                    el_selt_loading: false,//选择框 搜索状态
                },//select数据
                //ArrEnumField: [
				//	{ Name: 'IdCardType', ForeignKeyGetListUrl: '/Home/GetPagerEnum?EnumName=IdCardType' },
                //],//所有Select
            }
            console.log('data', data, this);
            return data;
        },
        methods: {
            el_FormFieldRules: function (rowConfig, isSearchForm) {
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
            handleAddRow: function (e) {
                console.log('handleAddRow', e);
                var NewRow = { Id: --this.addNum, AirTicketOrderId: this.AirTicketOrderId };
                this.DialogVisible = true;
                this.PlanePerson = NewRow;
                this.dlgLoading = false;//编辑弹出框加载中
                this.tb_PlanePerson_data.push(NewRow);
                this.tb_PlanePerson_data.addNum = this.addNum;//记录上次添加数-<keep-alive>
            },//增加行数据 弹出框添加
            handledblclick: function (row) {
                this.DialogVisible = true;
                //this.curr_rowdata_Original = row;//原始行数据
                //this.OrderCuntomer = Object.assign({}, row);
                this.PlanePerson = row;
                let curr_rowdata = this.PlanePerson;
                //let ArrEnumField = this.ArrEnumField;//所有select/枚举
                let thisVue = this;
                Object.keys(curr_rowdata).forEach(function (item, index) {
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
            handleSelectionChange: function (selections) {
                this.selctRows = selections;
                console.log('handleSelectionChange', selections);
            },//选择数据变更
            handledelSeltRow: function (e) {
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
                    var deltRowIndex = this.tb_PlanePerson_data.map(function (item, i) {
                        var has_el = thisVue.selctRows.some(function (el, x) { return el.Id === item.Id; })
                        if (has_el)
                            return i;
                        else return null;
                    });
                    deltRowIndex = deltRowIndex.filter(function (item) { return item != null });
                    deltRowIndex = deltRowIndex.reverse();
                    if (deltRowIndex.length >= 0) {
                        deltRowIndex.forEach(function (ArrIdx) {
                            let KeyId = thisVue.tb_PlanePerson_data[ArrIdx].Id;
                            thisVue.tb_PlanePerson_data.splice(ArrIdx, 1);
                            thisVue.$root.PlanePerson.deltRows.push(KeyId);
                        });
                    }
                    thisVue.tbLoading = false;//加载中
                }
                //rows.splice(index, 1);
            },//批量删除选中行数据
            dlgok_func: function () {
                let thisVue = this;
                let MyForm = this.$refs['PlanePersonForm'];
                //MyForm.resetFields();//重置验证
                MyForm.clearValidate();//清除验证
                MyForm.validate(function (valid) {
                    if (valid) {
                        thisVue.DialogVisible = false;
                        thisVue.$emit('dlgok_func');
                    }
                    else {
                    }
                });
            },
            formatter: function (field) {//el-table-column 数据显示转换
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
            ExportXls: function (JsonData, fileName) {
                //console.log('ExportXls');
                require(['xlsx', 'file-saver'], function (XLSX, FileSaver) {
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
                        const s2ab = function (s) {//字符串转字符流
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
            ImportXls: function () {

            },//导入数据
            el_remoteMethod: function (query, field, profx, forceload) {
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
                    }).then(function (success) {//成功
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
                    }, function (error) {//错误
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
            dlgClose: function (doneFunc) {
                let thisVue = this;
                let MyForm = this.$refs['PlanePersonForm'];
                //MyForm.resetFields();//重置验证
                MyForm.clearValidate();//清除验证
                MyForm.validate(function (valid) {
                    if (valid) {
                        thisVue.DialogVisible = false;
                        thisVue.$emit('dlgok_func');
                    }
                    else {
                        thisVue.$confirm('乘机人信息,验证错误, 强制新增?', '提示', {
                            confirmButtonText: '确定',
                            cancelButtonText: '取消',
                            type: 'warning'
                        }).then(function () {
                            if (typeof (doneFunc) === 'function') {
                                doneFunc();
                            } else {
                                thisVue.DialogVisible = false;
                                thisVue.$emit('dlgok_func');
                            }
                        }).catch(function () {
                            if (typeof (doneFunc) === 'function')
                                doneFunc();
                            else
                                thisVue.DialogVisible = false;

                            var PlanePerson = thisVue.PlanePerson;
                            var del_index = null;
                            thisVue.tb_PlanePerson_data.forEach(function (item, idx) {
                                if (item.Id == PlanePerson.Id) {
                                    del_index = idx;
                                    return false;
                                }
                            });
                            if (del_index != null) {
                                thisVue.tb_PlanePerson_data.splice(del_index, 1);
                            }
                        });
                    }
                });
            },//弹出框关闭
            ExportInfo: function () {
                let EXPinfo = this.$refs['exp_info'].value;
                if (EXPinfo == 'undefined' || EXPinfo == "" | EXPinfo == null)
                    return false;
                let paramData = {
                    EXPinfo: EXPinfo,
                }
                let url = '/AirTicketOrders/ExportPPInfo'
                this.$http.get(url, {
                    params: paramData,
                    headers: {//指示为 ajax请求
                        "X-Requested-With": "XMLHttpRequest"
                    }
                }).then(function (success){//成功
                    try {
                        var retData = success.body;
                        if (retData.Success) {
                            for (var i = 0; i < retData.rows.length; i++)
                            {
                                this.tb_PlanePerson_data.push(retData.rows[i]);
                            }
                            if (retData.PNR !== "undefined" && retData.PNR !== "" && retData.PNR !== null)
                            {
                                this.$set(this.$root.curr_rowdata, "PNR", retData.PNR)
                            }                              
                            console.log(retData);

                        }else{
                            this.$message({
                                duration:0,//不自动关闭
                                showClose: true,
                                message: '错误:' + retData.ErrMsg,
                                type: 'error'
                            });
                        }
                    } catch (e) {
                        this.$message({
                            duration:0,//不自动关闭
                            showClose: true,
                            message: '提交数据处理，出现错误',
                            type: 'error'
                        });
                    }
                }, function (error){//错误
                    this.$message({
                        duration:0,//不自动关闭
                        showClose: true,
                        message: '提交数据出现错误',
                        type: 'error'
                    });
                });
            }
        },
        watch: {
            airticketorderid: {
                handler: function (val, oldVal) {
                    this.AirTicketOrderId = val;
                },
                immediate: true,
                deep: true
            },
            tb_planeperson_data: {
                handler: function (newval, oldval) {
                    this.tb_PlanePerson_data = newval;
                },
                immediate: true,
                deep: true
            }
        },//监听属性变化
        template: '' +
        '<templete id=\'tb_PlanePerson\'>' +
        '    <el-row style="padding: 3px 0px 3px 0px;"> <!--上右下左-->' +
        '      <el-input ref="exp_info" v-model="InputExportInfo" type="textarea" v-bind:clearable="true" style="width:700px" />' +
        '        <el-col>' +
        '            <el-button-group>' +
        '                <el-button type="primary" icon="el-icon-plus" size="small" v-bind:disabled="!UserRoles.Create" v-on:click="handleAddRow">新增</el-button>' +
        '                <el-button icon="el-icon-download" size="small" v-bind:disabled="!UserRoles.Export" v-on:click="ExportXls(tb_PlanePerson_data,\'导出PlanePerson\')">导出</el-button>' +
        '                <el-button icon="el-icon-upload" size="small" v-bind:disabled="!UserRoles.Import" v-on:click="ExportInfo">导入</el-button>' +
        '                <el-button type="danger" size="small" v-on:click="handledelSeltRow" v-bind:disabled="(UserRoles.Delete ? selctRows.length===0 : true)">批量删除</el-button>' +
        '            </el-button-group>' +
        '        </el-col>' +
        '    </el-row><!--Table按钮组-->' +
        '    <el-row>' +
        '        <el-col>' +
        '            <el-table ref="tb_PlanePerson" size="mini" style="width: 100%" max-height="300" row-key="Id" border stripe' +
        '                      v-bind:default-sort="{prop:\'Id\',order:\'descending\'}"' +
        '                      v-bind:data="tb_PlanePerson_data"' +
        '                      v-loading="tbLoading"' +
        '                      v-on:row-dblclick="handledblclick"' +
        '                      v-on:selection-change="handleSelectionChange">' +
        '                <el-table-column fixed type="selection" width="36"></el-table-column>' +
        '                <template>' +
        '                    <el-table-column show-overflow-tooltip prop="NameChs" label="中文名" sortable v-bind:formatter="formatter({Type: \'string\', Name: \'NameChs\'})" width="120px"></el-table-column>' +
        '                    <el-table-column show-overflow-tooltip prop="LastNameEng" label="英文姓" sortable v-bind:formatter="formatter({Type: \'string\', Name: \'LastNameEng\'})" width="120px"></el-table-column>' +
        '                    <el-table-column show-overflow-tooltip prop="FirstNameEng" label="英文名" sortable v-bind:formatter="formatter({Type: \'string\', Name: \'FirstNameEng\'})" width="120px"></el-table-column>' +
        '                </template>' +
        '            </el-table><!--Table列表-->' +
        '        </el-col>' +
        '    </el-row><!--Table列表-->' +
        '    <el-dialog ref="PlanePersonDialog" width="60%" center append-to-body' +
		'               v-bind:close-on-click-modal="false"' +
        '               v-bind:show-close="false"' +
        '               v-bind:visible.sync="DialogVisible"' +
        '               v-bind:before-close="(done)=>{dlgClose(done)}"' +
        '               v-loading="dlgLoading">' +
        '        <div slot="title" class="el-dialog__title" style="">' +
        '            <el-row>' +
        '                <el-col v-bind:span="8" style="cursor:move;">&nbsp;</el-col>' +
        '                <el-col v-bind:span="8" style="cursor:move;">乘机人信息</el-col>' +
        '                <el-col v-bind:span="8" style="text-align:right;">' +
        '                    <el-button type="primary" icon="el-icon-check" size="mini" v-bind:disabled="!UserRoles.Edit" v-on:click="dlgok_func" title="确 定" circle></el-button>' +
        '                    <el-button type="danger" icon="el-icon-close" size="mini" v-on:click="dlgClose" title="取 消" circle></el-button>' +
        '                </el-col>' +
        '            </el-row>' +
        '        </div>' +
        '        <el-form ref="PlanePersonForm" v-bind:model="PlanePerson" label-position="right" inline size="small">' +
        '            <el-form-item v-bind:label-width="formLabelWidth" label="中文名" prop="NameChs"' +
        '                          v-bind:rules="el_FormFieldRules({Name:\'NameChs\',DisplayName:\'中文名\',Required:true,Editable:true,MinLength:0,MaxLength:100})">' +
        '                <el-input v-model="PlanePerson[\'NameChs\']" v-bind:clearable="true" style="width:178px" />' +
        '            </el-form-item>' +
        '            <el-form-item v-bind:label-width="formLabelWidth" label="英文姓" prop="LastNameEng"' +
        '                          v-bind:rules="el_FormFieldRules({Name:\'LastNameEng\',DisplayName:\'英文姓\',Required:true,Editable:true,MinLength:0,MaxLength:100})">' +
        '                <el-input v-model="PlanePerson[\'LastNameEng\']" v-bind:clearable="true" style="width:178px" />' +
        '            </el-form-item>' +
        '            <el-form-item v-bind:label-width="formLabelWidth" label="英文名" prop="FirstNameEng"' +
        '                          v-bind:rules="el_FormFieldRules({Name:\'FirstNameEng\',DisplayName:\'英文名\',Required:true,Editable:true,MinLength:0,MaxLength:100})">' +
        '                <el-input v-model="PlanePerson[\'FirstNameEng\']" v-bind:clearable="true" style="width:178px" />' +
        '            </el-form-item>' +
        '        </el-form>' +
        '        <span slot="footer" class="dialog-footer">' +
        '            <el-button type="primary" icon="el-icon-check" v-bind:disabled="!UserRoles.Edit" v-on:click="dlgok_func">确 定</el-button>' +
        '            <el-button type="default" icon="el-icon-close" v-on:click="dlgClose">取 消</el-button>' +
        '        </span>' +
        '    </el-dialog>' +
        '</templete>',
    };
    return PlanePerson;
});