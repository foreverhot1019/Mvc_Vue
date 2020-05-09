/*
module.exports可用时，按module.exports 规范输出
符合amd规范时，按amd规范define输出
exports 可用时 按照exports输出
window 输出 相当于var
*/
!function (t, e)
{
    "object" == typeof exports && "object" == typeof module ? module.exports = e() :
    "function" == typeof define && define.amd ? define([], e) : "object" == typeof exports ? exports.CostMoney = e() : t.CostMoney = e()
}(this, function ()
{
    var LazyLoadingComponent = require('LazyLoadingComponent');//组件懒加载func
    var CostMoney = {
        props: {
            orderid: {
                type: Number,
                default: 0,
            },
            tb_costmoney_data: {
                type: Array,
                required: true,
            },
            financemoney_data: {
                type: Array,
                required: true,
            },
            actualmoney_data: {
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
            if (!ObjectIsEmpty(this.tb_costmoney_data)) {
                var addNum = parseInt(this.tb_costmoney_data.addNum);
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
                tb_CostMoney_data: this.tb_costmoney_data,//当前列表数据
                formLabelWidth: this.formlabel_width,//Label宽度
                CostMoney: {},//当前编辑行
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
                FinanceMoney_Data: this.financemoney_data,//请款数据-提交请款后push进来
                ActualMoney_Data:this.actualmoney_data,
                TJ_FinanceMoneyVisable: false,//显示请款数据
                TJ_FinanceMoney_data: [],//提交请款数据
            }
            console.log('data', data, this);
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
                let thisVue = this;
                console.log('handleAddRow', e);
                var NewRow = { Id: --thisVue.addNum, OrderId: thisVue.OrderId, Price: 0, Num: 0 };
                thisVue.DialogVisible = true;
                thisVue.CostMoney = NewRow;
                thisVue.dlgLoading = false;//编辑弹出框加载中
                thisVue.tb_CostMoney_data.push(NewRow);
                thisVue.tb_CostMoney_data.addNum = this.addNum;//记录上次添加数-<keep-alive>
            },//增加行数据 弹出框添加
            handledblclick: function (row)
            {
                this.DialogVisible = true;
                //this.curr_rowdata_Original = row;//原始行数据
                //this.CostMoney = Object.assign({}, row);
                this.CostMoney = row;
                let curr_rowdata = this.CostMoney;
                let ArrEnumField = this.ArrEnumField;//所有select/枚举
                if (ObjectIsEmpty(curr_rowdata.Num))
                    this.$set(this.CostMoney, 'Num', 0);//数量设置0,防止读取上次数据
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
                    var FinanceMoney_Data = this.FinanceMoney_Data;
                    if (FinanceMoney_Data.length > 0) {
                        var ArrSeltId = this.selctRows.map(function (item) { return item.Id; });
                        var Any = FinanceMoney_Data.some(function (item) { return ArrSeltId.includes(item.CostMoneyId) });
                        if (Any) {
                            this.$message({
                                duration: 0,//不自动关闭
                                showClose: true,
                                message: '错误:已有请款数据,不能删除',
                                type: 'error'
                            });
                            return;
                        }
                    }
                    var thisVue = this;
                    thisVue.tbLoading = true;//加载中
                    var deltRowIndex = this.tb_CostMoney_data.map(function (item, i)
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
                            let KeyId = thisVue.tb_CostMoney_data[ArrIdx].Id;
                            thisVue.tb_CostMoney_data.splice(ArrIdx, 1);
                            thisVue.$root.CostMoney.deltRows.push(KeyId);
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
                let MyForm = this.$refs['CostMoneyForm'];
                let MyFormModel = MyForm.model;
                //MyForm.resetFields();//重置验证
                MyForm.clearValidate();//清除验证
                MyForm.validate(function (valid)
                {
                    if (valid) {
                        //console.log(MyForm.model.ServiceProjectNAME);
                        var ActualMoney = thisVue.ActualMoney_Data;
                        var AddActualMoneyNum = thisVue.ActualMoney_Data.length;//获取已新增的数据数量
                        let NewRow = {
                            Id: -(AddActualMoneyNum + 1),
                            OrderId: MyFormModel.OrderId,
                            SupplierName: MyFormModel.SupplierName,
                            ServiceProject: MyFormModel.ServiceProject,
                            Price: MyFormModel.Price,
                            Num: MyFormModel.Num,
                            TotalAmount: MyFormModel.TotalAmount,
                            ExcessAmount: 0,
                            RequestAmount: 0,//请款金额 =余额
                            Remark: MyFormModel.Remark,
                        };
                        //console.log(NewRow);
                        ActualMoney.push(NewRow);
                        
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
                                this.$set(this.CostMoney, 'SupplierNameNAME', OOption.TEXT);
                            }
                        }
                    }
                } else {
                    this.$set(this.CostMoney, 'SupplierNameNAME', '');
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
                                this.$set(this.CostMoney, 'ServiceProjectNAME', OOption.TEXT);
                            }
                        }
                    }
                } else {
                    this.$set(this.CostMoney, 'ServiceProjectNAME', '');
                }
            },//服务项目变更
            Copy: function ()
            {

            },//复制
            GetMoney: function ()
            {
                if (!ObjectIsEmpty(this.selctRows)) {
                    var TJ_FinanceMoney_data = this.TJ_FinanceMoney_data = [];
                    var thisVue = this;
                    var ValidTF = true;
                    var AddFinanceMoneyNum = this.FinanceMoney_Data.length;//获取已新增的数据数量
                    this.selctRows.forEach(function (item, i)
                    {
                        if (item.ExcessAmount <= 0) {
                            ValidTF = false;
                            return;
                        }
                        let NewRow = {
                            Id: -(AddFinanceMoneyNum+i+1),
                            OrderId: thisVue.OrderId,
                            CostMoneyId: item.Id,
                            SupplierName: item.SupplierName,
                            SupplierNameNAME: item.SupplierNameNAME,
                            ServiceProject: item.ServiceProject,
                            ServiceProjectNAME: item.ServiceProjectNAME,
                            Price: item.Price,
                            Num: item.Num,
                            TotalAmount: item.TotalAmount,
                            ExcessAmount: 0,
                            RequestAmount: item.ExcessAmount,//请款金额 =余额
                            Remark: item.Remark,
                        };
                        TJ_FinanceMoney_data.push(NewRow);
                        if (ObjectIsEmpty(item.IsRequest)) {
                            item.ExcessAmount = 0;//余额清零
                            item.RequestAmount = item.TotalAmount;//已请款金额 为总价
                            item.IsRequest = true;//已发起请款
                        } else {
                            item.RequestAmount += item.ExcessAmount;//已请款金额 为总价
                            item.ExcessAmount = 0;//余额清零
                        }
                    });//新增提交请款
                    if (!ValidTF) {
                        TJ_FinanceMoney_data = this.TJ_FinanceMoney_data = [];
                        this.$message({
                            duration: 0,//不自动关闭
                            showClose: true,
                            message: '错误:选择的数据，余额不足，无法再次请款',
                            type: 'error'
                        });
                        return false;
                    }
                    if (!this.TJ_FinanceMoneyVisable)
                        this.TJ_FinanceMoneyVisable = true;
                    else {
                        this.$refs.TJ_FinanceMoney.DialogVisible_FM = true;//显示弹出框
                        //设置子组件数据
                        //this.$refs.TJ_FinanceMoney.TJ_FinanceMoney_data = this.TJ_FinanceMoney_data;
                    }
                } else {
                    this.$message({
                        duration: 0,//不自动关闭
                        showClose: true,
                        message: '请款错误，至少选择 一条数据',
                        type: 'error'
                    });
                }
            },//请款
            dlgClose: function ()
            {
                let thisVue = this;
                let MyForm = this.$refs['CostMoneyForm'];
                //MyForm.resetFields();//重置验证
                MyForm.clearValidate();//清除验证
                MyForm.validate(function (valid)
                {
                    if (valid) {
                        thisVue.DialogVisible = false;
                        thisVue.$emit('dlgok_func');
                    }
                    else {
                        thisVue.$confirm('预算成本,验证错误, 强制新增?', '提示', {
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
                            var CostMoney = thisVue.CostMoney;
                            var del_index = null;
                            thisVue.tb_CostMoney_data.forEach(function (item, idx)
                            {
                                if (item.Id == CostMoney.Id) {
                                    del_index = idx;
                                    return false;
                                }
                            });
                            if (del_index != null) {
                                thisVue.tb_CostMoney_data.splice(del_index, 1);
                            }
                        });
                    }
                });
            },//弹出框关闭
        },
        components: {
            'tj_finacemoney': function (resolve, reject)
            {
                return LazyLoadingComponent('/Views/Orders/TJ_FinanceMoney.js');
            },//异步加载 请款信息 注册到局部
        },
        computed: {//计算属性
            PriceMin: function ()
            {
                if (!ObjectIsEmpty(this.CostMoney) && JSON.stringify(this.CostMoney) != "{}") {
                    //var TotalAmount = parseFloat(this.CostMoney.TotalAmount);//金额
                    //var ExcessAmount = parseFloat(this.CostMoney.ExcessAmount);//余额
                    //var RequestAmount = parseFloat(this.CostMoney.RequestAmount);//已请款
                    if (!ObjectIsEmpty(this.CostMoney.IsRequest))
                        return this.CostMoney.Price;
                    else
                        return 0;//-Number.MAX_VALUE;
                }
                else
                    return 0;//-Number.MAX_VALUE;
            },
            NumMin: function ()
            {
                if (!ObjectIsEmpty(this.CostMoney) && JSON.stringify(this.CostMoney) != "{}") {
                    //var TotalAmount = parseFloat(this.CostMoney.TotalAmount);//金额
                    //var ExcessAmount = parseFloat(this.CostMoney.ExcessAmount);//余额
                    //var RequestAmount = parseFloat(this.CostMoney.RequestAmount);//已请款
                    if (!ObjectIsEmpty(this.CostMoney.IsRequest))
                        return this.CostMoney.Num;
                    else
                        return 0;//-Number.MAX_VALUE;
                }
                else
                    return 0;//-Number.MAX_VALUE;
            },
        },//计算属性
        watch: {//监听属性变化
            "CostMoney.Price": {
                handler: function (val, oldVal)
                {
                    var NewTotalAmount = val * this.CostMoney.Num;//单价*数量
                    var ChangeAmonunt = NewTotalAmount - this.CostMoney.TotalAmount;
                    this.CostMoney.TotalAmount = NewTotalAmount;//金额
                    if ((this.CostMoney.ExcessAmount == 0 || ObjectIsEmpty(this.CostMoney.ExcessAmount) || isNaN(this.CostMoney.ExcessAmount)) &&
                        (this.CostMoney.RequestAmount == 0 || ObjectIsEmpty(this.CostMoney.RequestAmount) || isNaN(this.CostMoney.RequestAmount))) {
                        this.CostMoney.ExcessAmount = this.CostMoney.TotalAmount;//余额
                        this.CostMoney.RequestAmount = 0;//已请款
                    } else {
                        this.CostMoney.ExcessAmount += ChangeAmonunt;//余额
                    }
                    //刷新提交数据
                }
            },
            "CostMoney.Num": {
                handler: function (val, oldVal)
                {
                    var NewTotalAmount = this.CostMoney.Price * val;//单价*数量
                    var ChangeAmonunt = NewTotalAmount - this.CostMoney.TotalAmount;
                    this.CostMoney.TotalAmount = NewTotalAmount;//金额
                    if ((this.CostMoney.ExcessAmount == 0 || ObjectIsEmpty(this.CostMoney.ExcessAmount) || isNaN(this.CostMoney.ExcessAmount)) &&
                        (this.CostMoney.RequestAmount == 0 || ObjectIsEmpty(this.CostMoney.RequestAmount) || isNaN(this.CostMoney.RequestAmount))) {
                        this.CostMoney.ExcessAmount = this.CostMoney.TotalAmount;//余额
                        this.CostMoney.RequestAmount = 0;//已请款
                    } else {
                        this.CostMoney.ExcessAmount += ChangeAmonunt;//余额 
                    }
                    //刷新提交数据
                }
            },
            orderid: {
                handler:function(newval, oldval){
                    this.OrderId = newval;
                },
                immediate: true,
                deep: true
            },
            tb_CostMoney_data: {
                handler: function (newval, oldval)
                {
                    this.tb_CostMoney_data = newval;
                },
                immediate: true,
                deep: true
            }
        },//监听属性变化
        template: '' +
        '<templete id="dlg_CostMoney">' +
        '    <el-row style="padding: 3px 0px 3px 0px;"> <!--上右下左-->' +
        '        <el-col>' +
        '            <el-button-group>' +
        '                <el-button type="primary" icon="el-icon-plus" size="small" v-bind:disabled="!UserRoles.Create" v-on:click="handleAddRow">新增</el-button>' +
        '                <el-button icon="el-icon-copy" size="small" v-bind:disabled="!UserRoles.Export" v-on:click="Copy">复制</el-button>' +
        '                <el-button icon="el-icon-money" size="small" v-bind:disabled="!UserRoles.Import" v-on:click="GetMoney">请款</el-button>' +
        '                <el-button type="danger" size="small" v-on:click="handledelSeltRow" v-bind:disabled="(UserRoles.Delete ? selctRows.length===0 : true)">批量删除</el-button>' +
        '            </el-button-group>' +
        '        </el-col>' +
        '    </el-row><!--列表按钮组-->' +
        '    <el-row>' +
        '        <el-col>' +
        '            <el-table ref="tb_CostMoney" size="mini" style="width: 100%" max-height="500" row-key="Id" border stripe' +
        '                      v-bind:default-sort="{prop:\'Id\',order:\'descending\'}"' +
        '                      v-bind:data="tb_CostMoney_data"' +
        '                      v-loading="tbLoading"' +
        '                      v-on:row-dblclick="handledblclick"' +
        '                      v-on:selection-change="handleSelectionChange">' +
        '                <el-table-column fixed type="selection" width="36"></el-table-column>' +
        '                <template>' +
        '                    <el-table-column show-overflow-tooltip prop="SupplierName" label="供应商名称" sortable v-bind:formatter="formatter({Type: \'string\', Name: \'SupplierName\'})" width="120px"></el-table-column>' +
        '                    <el-table-column show-overflow-tooltip prop="ServiceProject" label="服务项目" sortable v-bind:formatter="formatter({Type: \'string\', Name: \'英文名\',ForeignKeyGetListUrl:true})" width="120px"></el-table-column>' +
        '                    <el-table-column show-overflow-tooltip prop="Price" label="单价" sortable v-bind:formatter="formatter({Type: \'number\', Name: \'Price\'})" width="80px"></el-table-column>' +
        '                    <el-table-column show-overflow-tooltip prop="Num" label="数量" sortable v-bind:formatter="formatter({Type: \'number\', Name: \'Num\'})" width="70px"></el-table-column>' +
        '                    <el-table-column show-overflow-tooltip prop="TotalAmount" label="金额" sortable v-bind:formatter="formatter({Type: \'number\', Name: \'TotalAmount\'})" width="90px"></el-table-column>' +
        '                    <el-table-column show-overflow-tooltip prop="ExcessAmount" label="余额" sortable v-bind:formatter="formatter({Type: \'number\', Name: \'ExcessAmount\'})" width="90px"></el-table-column>' +
        '                    <el-table-column show-overflow-tooltip prop="RequestAmount" label="请款" sortable v-bind:formatter="formatter({Type: \'number\', Name: \'RequestAmount\'})" width="90px"></el-table-column>' +
        '                    <el-table-column show-overflow-tooltip prop="Remark" label="备注" sortable v-bind:formatter="formatter({Type: \'string\', Name: \'Remark\'})" min-width="150px" width="*"></el-table-column>' +
        '                </template>' +
        '            </el-table><!--Table列表-->' +
        '        </el-col>' +
        '    </el-row>' +
        '    <el-dialog ref="CostMoneyDialog" width="60%" center append-to-body ' +
        '               v-bind:close-on-click-modal="false"' +
        '               v-bind:show-close="false"' +
        '               v-bind:visible.sync="DialogVisible"' +
        '               v-bind:before-close="(done)=>{dlgClose(done)}"' +
        '               v-loading="dlgLoading">' +
        '        <div slot="title" class="el-dialog__title" style="">' +
        '            <el-row>' +
        '                <el-col v-bind:span="8" style="cursor:move;">&nbsp;</el-col>' +
        '                <el-col v-bind:span="8" style="cursor:move;">预算成本</el-col>' +
        '                <el-col v-bind:span="8" style="text-align:right;">' +
        '                    <el-button type="primary" icon="el-icon-check" size="mini" v-bind:disabled="!UserRoles.Edit" v-on:click="dlgok_func" title="确 定" circle></el-button>' +
        '                    <el-button type="danger" icon="el-icon-close" size="mini" v-on:click="dlgClose" title="取 消" circle></el-button>' +
        '                </el-col>' +
        '            </el-row>' +
        '        </div>' +
        '        <el-form ref="CostMoneyForm" v-bind:model="CostMoney" label-position="right" inline size="small">' +
        '            <el-form-item v-bind:label-width="formLabelWidth" label="供应商名称" prop="SupplierName"' +
        '                          v-bind:rules="el_FormFieldRules({Name:\'SupplierName\',DisplayName:\'供应商名称\',Required:true,Editable:true,MinLength:0,MaxLength:50})">' +
        '                <el-select v-model="CostMoney[\'SupplierName\']" style="width:178px" reserve-keyword clearable' +
        '                           v-bind:loading="el_selt.el_selt_loading" v-on:change="SupplierNameChange">' +
        '                    <template v-if="el_selt.SupplierName_form">' +
        '                        <el-option v-for="item in el_selt.SupplierName_form.ArrOption"' +
        '                                   v-bind:key="item.ID"' +
        '                                   v-bind:label="item.TEXT"' +
        '                                   v-bind:value="item.ID">' +
        '                        </el-option>' +
        '                    </template>' +
        '                </el-select>' +
        '            </el-form-item>' +
        '            <el-form-item v-bind:label-width="formLabelWidth" label="服务项目" prop="ServiceProject"' +
        '                          v-bind:rules="el_FormFieldRules({Name:\'ServiceProject\',DisplayName:\'服务项目\',Required:true,Editable:true,MinLength:0,MaxLength:100})">' +
        '                <el-select v-model="CostMoney[\'ServiceProject\']" style="width:178px" reserve-keyword clearable' +
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
        '                <el-input-number v-model="CostMoney[\'Price\']" placeholder="单价" v-bind:precision="2" v-bind:min="PriceMin" v-bind:style="{width:\'178px\'}" />' +
        '            </el-form-item>' +
        '            <el-form-item v-bind:label-width="formLabelWidth" label="数量" prop="Num"' +
        '                          v-bind:rules="el_FormFieldRules({Name:\'Num\',DisplayName:\'数量\',Required:true,Editable:true,MinLength:0,MaxLength:0})">' +
        '                <el-input-number v-model="CostMoney[\'Num\']" placeholder="数量" v-bind:precision="2" v-bind:min="NumMin" v-bind:style="{width:\'178px\'}" />' +
        '            </el-form-item>' +
        '            <el-form-item v-bind:label-width="formLabelWidth" label="金额" prop="TotalAmount"' +
        '                          v-bind:rules="el_FormFieldRules({Name:\'TotalAmount\',DisplayName:\'金额\',Required:true,Editable:false,MinLength:0,MaxLength:0})">' +
        '                <el-input-number v-model="CostMoney[\'TotalAmount\']" placeholder="金额" disabled="true" v-bind:precision="2" v-bind:style="{width:\'178px\'}" />' +
        '            </el-form-item>' +
        '            <el-form-item v-bind:label-width="formLabelWidth" label="请款" prop="RequestAmount"' +
        '                          v-bind:rules="el_FormFieldRules({Name:\'RequestAmount\',DisplayName:\'请款\',Required:true,Editable:false,MinLength:0,MaxLength:0})">' +
        '                <el-input-number v-model="CostMoney[\'RequestAmount\']" placeholder="请款" disabled="true" v-bind:precision="2" v-bind:style="{width:\'178px\'}" />' +
        '            </el-form-item>' +
        '            <el-form-item v-bind:label-width="formLabelWidth" label="余额" prop="ExcessAmount"' +
        '                          v-bind:rules="el_FormFieldRules({Name:\'ExcessAmount\',DisplayName:\'余额\',Required:true,Editable:false,MinLength:0,MaxLength:0})">' +
        '                <el-input-number v-model="CostMoney[\'ExcessAmount\']" placeholder="余额" disabled="true" v-bind:precision="2" v-bind:style="{width:\'178px\'}" />' +
        '            </el-form-item>' +
        '            <el-form-item v-bind:label-width="formLabelWidth" label="备注" prop="Remark"' +
        '                          v-bind:rules="el_FormFieldRules({Name:\'Remark\',DisplayName:\'备注\',Required:false,Editable:true,MinLength:0,MaxLength:1000})">' +
        '                <el-input type="textarea" v-model="CostMoney[\'Remark\']" v-bind:clearable="true" style="width:100%;" v-bind:style="{width:\'178px\'}" />' +
        '            </el-form-item>' +
        '        </el-form>' +
        '        <span slot="footer" class="dialog-footer">' +
        '            <el-button type="primary" icon="el-icon-check" v-bind:disabled="!UserRoles.Edit" v-on:click="dlgok_func">确 定</el-button>' +
        '            <el-button type="default" icon="el-icon-close" v-on:click="dlgClose">取 消</el-button>' +
        '        </span>' +
        '    </el-dialog><!--预算成本-编辑-->' +
        '    <tj_finacemoney v-if="TJ_FinanceMoneyVisable" ref="TJ_FinanceMoney" ' +
        '       v-bind:tj_financemoney_data="TJ_FinanceMoney_data" ' +
        '       v-bind:financemoney_data="FinanceMoney_Data" ' +
        '       v-bind:tb_costmoney_data="tb_CostMoney_data" ' +
        '       v-bind:user_roles="UserRoles" />' +
        '</templete>',
    };
    return CostMoney;
});