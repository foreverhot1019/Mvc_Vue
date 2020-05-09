/*
module.exports可用时，按module.exports 规范输出
符合amd规范时，按amd规范define输出
exports 可用时 按照exports输出
window 输出 相当于var
*/
!function (t, e)
{
    "object" == typeof exports && "object" == typeof module ? module.exports = e() :
    "function" == typeof define && define.amd ? define([], e) : "object" == typeof exports ? exports.TJ_FinanceMoney = e() : t.TJ_FinanceMoney = e()
}(this, function ()
{
    var TJ_FinanceMoney = {
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
            tj_financemoney_data: {
                type:Array,
                required:true
            },
            formlabel_width: {
                type: String,
                default: '120px'
            },
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
            console.log('mounted', this);
        },//相当于构造函数，渲染完dom后触发
        data: function () {
            var data = {
                OrderId: this.orderid,//订单Id
                FinanceMoney_Data: this.financemoney_data,//请款数据-提交请款后push进来
                TJ_FinanceMoney_data: this.tj_financemoney_data,//提交情况数据
                formLabelWidth: this.formlabel_width,//Label宽度
                tb_CostMoney_data: this.tb_costmoney_data,//预算成本数据
                UserRoles: this.user_roles,//权限
                tbLoading_FM: false,//数据列表加载中
                DialogVisible_FM: true,//弹出框显示
                dlgLoading_FM: false,//弹出框加载中
                Is_dlgClose: false,
            }
            console.log('data', data, this);
            return data;
        },
        methods: {
            dlgOK_Func_FM: function () {
                this.DialogVisible_FM = false;
                var thisVue = this;
                var FinanceMoney_Data = this.FinanceMoney_Data || thisVue.FinanceMoney_Data;
                this.TJ_FinanceMoney_data.forEach(function (item) {
                    FinanceMoney_Data.push(item);
                });
                this.TJ_FinanceMoney_data = [];
            },
            dlgClose: function (done) {
                let thisVue = this;
                if (this.Is_dlgClose) {
                    thisVue.DialogVisible_FM = false;
                    thisVue.$emit('dlgOK_Func_FM');
                } else {
                    for (var i = 0; i < this.TJ_FinanceMoney_data.length; i++) {
                        var RequestAmountTotal = 0;//已请款数据
                        var FinanceMoney_Data = this.FinanceMoney_Data;
                        var row = this.TJ_FinanceMoney_data[i];
                        var tb_CostMoney_data = this.tb_CostMoney_data
                        var QFinanceMoney_Data = FinanceMoney_Data.filter(function (item) { return row.CostMoneyId == item.CostMoneyId });
                        if (QFinanceMoney_Data.length > 0) {
                            var ArrRequestAmount = QFinanceMoney_Data.map(function (item) { return item.RequestAmount });
                            RequestAmountTotal = ArrRequestAmount.reduce(function (a, b) {
                                var _a = parseFloat(a);
                                var _b = parseFloat(b);
                                _a = isNaN(_a) ? 0 : _a;
                                _b = isNaN(_b) ? 0 : _b;
                                return _a + _b;
                            });
                        }
                        var CostMoneyId = row.CostMoneyId;
                        this.tb_CostMoney_data.forEach(function (item, i) {
                            if (item.Id == CostMoneyId) {
                                item.ExcessAmount = item.TotalAmount - RequestAmountTotal;
                                item.RequestAmount = item.TotalAmount - item.ExcessAmount;
                                thisVue.$set(thisVue.tb_CostMoney_data, i, item);
                                ////触发父组件重绘
                                //thisVue.$parent.$refs.tb_CostMoney.toggleRowSelection(item);
                                //thisVue.$parent.$refs.tb_CostMoney.toggleRowSelection(item);
                                return;
                            }
                        });//修改预算成本-余额，未请款
                    }
                    thisVue.DialogVisible_FM = false;
                }

            },//弹出框关闭
            formatter: function (field){//el-table-column 数据显示转换
                var formatter = null;
                switch (field.Type){
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
            ChangeRowRequestAmount: function (val, row) {
                var RequestAmountTotal = 0;//已请款数据
                var FinanceMoney_Data = this.FinanceMoney_Data;
                if (FinanceMoney_Data.length > 0) {
                    var QFinanceMoney_Data = FinanceMoney_Data.filter(function (item) { return row.CostMoneyId == item.CostMoneyId });
                    if (QFinanceMoney_Data.length > 0) {
                        var ArrRequestAmount = QFinanceMoney_Data.map(function (item) { return item.RequestAmount });
                        RequestAmountTotal = ArrRequestAmount.reduce(function (a, b) {
                            var _a = parseFloat(a);
                            var _b = parseFloat(b);
                            _a = isNaN(_a) ? 0 : _a;
                            _b = isNaN(_b) ? 0 : _b;
                            return _a + _b;
                        });
                    }
                }//计算实际已请款数据
                var ExcessAmount = row.TotalAmount - RequestAmountTotal - val;
                var CostMoneyId = row.CostMoneyId;
                row.ExcessAmount = ExcessAmount;
                var thisVue = this;
                this.tb_CostMoney_data.forEach(function (item, i) {
                    if (item.Id == CostMoneyId) {
                        item.ExcessAmount = ExcessAmount;
                        item.RequestAmount = item.TotalAmount - ExcessAmount;
                        thisVue.$set(thisVue.tb_CostMoney_data, i, item);
                        ////触发父组件重绘
                        //thisVue.$parent.$refs.tb_CostMoney.toggleRowSelection(item);
                        //thisVue.$parent.$refs.tb_CostMoney.toggleRowSelection(item);
                        return;
                    }
                });//修改预算成本-余额，已请款
            },//行请款金额修改编辑
        },
        watch: {//监听属性变化
            tb_CostMoney_data: {//深度监听，可监听到对象、数组的变化
                handler: function (val, oldVal) {
                    this.tb_costmoney_data = val;
                },
                deep: true,
                immediate: true, // 很重要！！！
            },
            tj_financemoney_data: {
                immediate: true, // 很重要！！！
                handler: function (val, oldVal) {
                    this.TJ_FinanceMoney_data = val;
                },
                deep: true,
            }
        },//监听属性变化
        template: '' +
        '<templete id="dlg_TJ_FinanceMoney">' +
        '    <el-dialog ref="TJ_FinanceMoneyDialog" width="60%" center append-to-body ' +
        '               v-bind:close-on-click-modal="false"' +
        '               v-bind:show-close="false"' +
        '               v-bind:visible.sync="DialogVisible_FM"' +
        '               v-bind:before-close="(done)=>{dlgClose(done)}"' +
        '               v-loading="dlgLoading_FM">' +
        '        <div slot="title" class="el-dialog__title" style="">' +
        '            <el-row>' +
        '                <el-col v-bind:span="8" style="cursor:move;">&nbsp;</el-col>' +
        '                <el-col v-bind:span="8" style="cursor:move;">请款数据</el-col>' +
        '                <el-col v-bind:span="8" style="text-align:right;">' +
        '                    <el-button type="primary" icon="el-icon-check" size="mini" v-bind:disabled="!UserRoles.Edit" v-on:click="dlgOK_Func_FM" title="确 定" circle></el-button>' +
        '                    <el-button type="danger" icon="el-icon-close" size="mini" v-on:click="dlgClose" title="取 消" circle></el-button>' +
        '                </el-col>' +
        '            </el-row>' +
        '        </div>' +
        '        <el-row>' +
        '            <el-col>' +
        '                <el-table ref="tb_FinanceMoney" size="mini" style="width: 100%" max-height="500" row-key="Id" border stripe show-summary' +
        '                          v-bind:default-sort="{prop:\'Id\',order:\'descending\'}"' +
        '                          v-bind:data="TJ_FinanceMoney_data"' +
        '                          v-loading="tbLoading_FM">' +
        '                    <template>' +
        '                        <el-table-column show-overflow-tooltip prop="SupplierName" label="供应商名称" sortable v-bind:formatter="formatter({Type: \'string\', Name: \'SupplierName\'})" width="120px"></el-table-column>' +
        '                        <el-table-column show-overflow-tooltip prop="ServiceProject" label="服务项目" sortable v-bind:formatter="formatter({Type: \'string\', Name: \'ServiceProject\',ForeignKeyGetListUrl:true})" width="120px"></el-table-column>' +
        '                        <el-table-column show-overflow-tooltip prop="Price" label="单价" sortable v-bind:formatter="formatter({Type: \'number\', Name: \'Price\'})" width="80px"></el-table-column>' +
        '                        <el-table-column show-overflow-tooltip prop="Num" label="数量" sortable v-bind:formatter="formatter({Type: \'number\', Name: \'Num\'})" width="70px"></el-table-column>' +
        '                        <el-table-column show-overflow-tooltip prop="TotalAmount" label="金额" sortable v-bind:formatter="formatter({Type: \'number\', Name: \'TotalAmount\'})" width="90px"></el-table-column>' +
        '                        <el-table-column show-overflow-tooltip prop="ExcessAmount" label="余额" sortable v-bind:formatter="formatter({Type: \'number\', Name: \'ExcessAmount\'})" width="90px"></el-table-column>' +
        '                        <el-table-column prop="RequestAmount" label="请款" sortable v-bind:formatter="formatter({Type: \'number\', Name: \'RequestAmount\'})" width="178px">' +
        '                            <templete slot-scope="sp">' +
        '                                <el-input-number v-model="sp.row.RequestAmount" placeholder="请款" size="mini" v-bind:precision="2" v-bind:min="0.1" style="width:160px;" v-bind:max="sp.row.RequestAmount+sp.row.ExcessAmount" v-on:change="(val)=>ChangeRowRequestAmount(val,sp.row)" />' +
        '                            </templete>' +
        '                        </el-table-column>' +
        '                        <el-table-column show-overflow-tooltip prop="Remark" label="备注" sortable v-bind:formatter="formatter({Type: \'string\', Name: \'Remark\'})" width="150px"></el-table-column>' +
        '                    </template>' +
        '                </el-table><!--Table列表-->' +
        '            </el-col>' +
        '        </el-row>' +
        '        <span slot="footer" class="dialog-footer">' +
        '            <el-button type="default" icon="el-icon-close" v-on:click="dlgClose">取 消</el-button>' +
        '            <el-button type="primary" icon="el-icon-check" v-bind:disabled="!UserRoles.Edit" v-on:click="dlgOK_Func_FM">确 定</el-button>' +
        '        </span>' +
        '    </el-dialog>' +
        '</templete>',
    };
    return TJ_FinanceMoney;
});