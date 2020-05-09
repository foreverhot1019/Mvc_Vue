/*
module.exports可用时，按module.exports 规范输出
符合amd规范时，按amd规范define输出
exports 可用时 按照exports输出
window 输出 相当于var
*/
!function (t, e)
{
    "object" == typeof exports && "object" == typeof module ? module.exports = e() :
    "function" == typeof define && define.amd ? define([], e) : "object" == typeof exports ? exports.FinanceMoney = e() : t.FinanceMoney = e()
}(this, function ()
{
    var FinanceMoney = {
        props: {
            orderid: {
                type: Number,
                default: 0,
            },
            tb_financemoney_data: {
                type: Array,
                required: true,
            },
            formlabel_width: {
                type: String,
                default: '120px'
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
            }
        },
        mounted: function ()
        {
            console.log('mounted', this);
        },//相当于构造函数，渲染完dom后触发
        data: function ()
        {
            var data = {
                OrderId: this.orderid,//订单Id
                tb_FinanceMoney_Data: this.tb_financemoney_data,//请款数据
                formLabelWidth: this.formlabel_width,//Label宽度
                UserRoles: this.user_roles,//权限
                tbLoading: false,//数据列表加载中
            }
            //console.log('data', data, this);
            return data;
        },
        methods: {
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
        },
        watch: {//监听属性变化
            tb_financemoney_data: {
                handler: function (newval, oldval)
                {
                    this.tb_FinanceMoney_Data = newval;
                },
                immediate: true,
                deep: true
            }
        },//监听属性变化
        template: '' +
        '<templete id="tb_TJ_FinanceMoney">' +
        '   <el-table ref="tb_FinanceMoney" size="mini" style="width: 100%;" max-height="500" row-key="Id" border stripe show-summary' +
        '             v-bind:default-sort="{prop:\'Id\',order:\'descending\'}"' +
        '             v-bind:data="tb_FinanceMoney_Data"' +
        '             v-loading="tbLoading">' +
        '       <template>' +
        '           <el-table-column show-overflow-tooltip prop="SupplierName" label="供应商名称" sortable v-bind:formatter="formatter({Type: \'string\', Name: \'SupplierName\'})" width="120px"></el-table-column>' +
        '           <el-table-column show-overflow-tooltip prop="ServiceProject" label="服务项目" sortable v-bind:formatter="formatter({Type: \'string\', Name: \'ServiceProject\',ForeignKeyGetListUrl:true})" width="120px"></el-table-column>' +
        '           <el-table-column show-overflow-tooltip prop="Price" label="单价" sortable v-bind:formatter="formatter({Type: \'number\', Name: \'Price\'})" width="80px"></el-table-column>' +
        '           <el-table-column show-overflow-tooltip prop="Num" label="数量" sortable v-bind:formatter="formatter({Type: \'number\', Name: \'Num\'})" width="70px"></el-table-column>' +
        '           <el-table-column show-overflow-tooltip prop="TotalAmount" label="金额" sortable v-bind:formatter="formatter({Type: \'number\', Name: \'TotalAmount\'})" width="90px"></el-table-column>' +
        '           <el-table-column show-overflow-tooltip prop="ExcessAmount" label="余额" sortable v-bind:formatter="formatter({Type: \'number\', Name: \'ExcessAmount\'})" width="90px"></el-table-column>' +
        '           <el-table-column show-overflow-tooltip prop="RequestAmount" label="请款" sortable v-bind:formatter="formatter({Type: \'number\', Name: \'RequestAmount\'})" width="90px"></el-table-column>' +
        '           <el-table-column show-overflow-tooltip prop="Remark" label="备注" sortable v-bind:formatter="formatter({Type: \'string\', Name: \'Remark\'})" min-width="150px" width="*"></el-table-column>' +
        '       </template>' +
        '   </el-table><!--Table列表-->' +
        '</templete>',
    };
    return FinanceMoney;
});