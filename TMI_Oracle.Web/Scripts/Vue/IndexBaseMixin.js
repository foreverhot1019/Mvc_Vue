;(function (global, factory) {
    typeof exports === 'object' && typeof module !== 'undefined' ? module.exports = factory() :
    typeof define === 'function' && define.amd ? define([],factory) :(global.myMixin = factory());
}(this,function(){
    // The one and only way of getting global scope in all environments
    // https://stackoverflow.com/q/3277182/1008999
    var _global = typeof window === 'object' && window.window === window ? window : typeof self === 'object' && self.self === self ? self : typeof global === 'object' && global.global === global ? global : void 0;
    var myMixin = {
        directives:{},// 注册局部指令
        created: function () {
            //this.UserRoles={
            //    Edit  :Edit,
            //    Create:Create,
            //    Delete:Delete,
            //    Import:Import,
            //    Export:Export,
            //}
            let thisVue = this;
            thisVue.ArrEnumField = BaseArrField.ArrField.filter(function(val){
                return val.IsForeignKey;
            });//所有select/枚举
            thisVue.ArrEnumField.forEach(function(item){//所有select/枚举
                thisVue.el_remoteMethod('',item,'search',true);
                thisVue.el_remoteMethod('',item,'form',true);
            });
        },//数据初始化，还未渲染dom,在此处设置的数据 不受响应
        mounted: function () {
            document.querySelector('#div_Loading').hidden = true;//必须得有，不然一直显示加载中。。。
            this.tb_GetData();//table数据初始化
            this.$set(this.el_selt,'el_selt_loading',false);//选择框loding状态
            //this.$set(this.el_selt,'ArrOption',[]);//设置选择框数据
            /* 设置属性 参考：https://developer.mozilla.org/zh-CN/docs/Web/JavaScript/Reference/Global_Objects/Object/defineProperty
            let Prop_description = {
                configurable:false,//对象的属性是否可以被删除，以及除value和writable特性外的其他特性是否可以被修改。
                writable:false,//是否可写
                enumerable:false,//定义了对象的属性是否可以在 for...in 循环和 Object.keys() 中被枚举
                value: undefined,//要设置的值 不能与 get/set 同时使用
                //get:function(){return Edit; },
                //set : function(newVal) {
                //    return false;
                //}
            };
            //注：如果property已存在，则所有忽略的属性值维持原样不变。*/
            /* 设置属性不能修改 相当于const  {value:{}}等价于 {value : {},writable : false,configurable : false,enumerable : false}*/
            Object.defineProperty(this, 'UserRoles', {value:{}});
            var setterFunc=function(newVal){
                var err ='不允许修改值';
                if(typeof(console)==='undefined')
                    alert(err);
                else 
                    console.log(err);
            };
            Object.defineProperty(this.UserRoles, 'Edit',   {configurable:false,get:function(){ return Edit;},set:setterFunc});
            Object.defineProperty(this.UserRoles, 'Create', {configurable:false,get:function(){ return Create;},set:setterFunc});
            Object.defineProperty(this.UserRoles, 'Delete', {configurable:false,get:function(){ return Delete;},set:setterFunc});
            Object.defineProperty(this.UserRoles, 'Import', {configurable:false,get:function(){ return Import;},set:setterFunc});
            Object.defineProperty(this.UserRoles, 'Export', {configurable:false,get:function(){ return Export;},set:setterFunc});
        },//相当于构造函数，渲染完dom后触发
        filters: {//v-bind可以使用，v-model 无效
        },//数据过滤器
        data: function ()
        {
            var tb = {
                tbUrl: {
                    controller: '/a/',
                    getdata: 'GetData',//获取数据 action
                    batchSave: 'SaveData',//批量操作 action
                    exportExcel: 'ExportExcel',//导出Excel action
                    importExcel: '/FileUpload/Upload?modelType=AccountManage',//导入Excel action
                },
                AddNum:0,//新增序号
                method: 'post',//HTTP请求方法
                tbLoading: true,//加载中
                addRows: [],//新增的行
                selctRows: [],//选中行
                curr_rowdata: {},//当前选择的行
                curr_rowdata_Original:{},//当前行原始数据
                centerDialogVisible: false,//弹出框是否打开
                dlgLoading:false,//弹出框加载状态
                formLabelWidth: '120px',
                tableData: [],
                UserRoles:{},//权限
            };
            var pagiNation = {//翻页控件数据
                pageSizes: [1,10, 20, 50, 100, 200, 300, 500],
                pageSize: 10,
                currentPage: 1,
                layout: "total, sizes, prev, pager, next, jumper",
                total: 0
            };
            var filters = {//搜索数据
                page: pagiNation.currentPage,
                rows: pagiNation.pageSize,
                sort: "Id",
                order: "desc",
                filterRules: {}//查询条件
            };
            tb.pagiNation = pagiNation;
            tb.filters = filters;
            tb.ArrFields = ArrField;//自定义列数据 table-searchForm-editForm 通过此配置渲染
            //tb.valid_rules = valid_rules;
            //el-select 搜索框数据
            tb.el_selt ={
                //el_selt_loading : false,//选择框 搜索状态
                //'CompanyId_form':{ArrOption : []},//选择框 数据
            }
            return tb;
        },//数据集
        computed: {//计算属性
            dgTitle:function(){
                //debugger;
                this.UserRoles.Edit = false;//修改不了 writable 为 false 属性const化
                //Object.defineProperty(this.UserRoles,'Edit',{configurable:true,writable:true});
                //Object.defineProperty(this.UserRoles,'Edit',{value:false});
                var t_curr_rowdata =typeof(this.curr_rowdata);
                if(t_curr_rowdata==='undefined'||this.curr_rowdata==null ||JSON.stringify(this.curr_rowdata)=='{}')
                    return '未知';
                if(this.curr_rowdata.Id<=0)
                {
                    return '新增';
                }else{
                    if(this.UserRoles.Edit)
                        return '编辑/查看';
                    else
                        return '查看';
                }
            }
        },//计算属性
        watch: {//监听属性变化
            //pagiNation:{//深度监听，可监听到对象、数组的变化
            //    handler(val, oldVal){
            //        console.log("b.c: "+val.c, oldVal.c);//但是这两个值打印出来却都是一样的
            //    },
            //    immediate: true,//将立即以表达式的当前值触发回调
            //    deep:true
            //},
            "pagiNation.currentPage": {
                handler :function(newValue, oldValue)
                {
                    this.filters.page = newValue;
                    this.tb_GetData();//重新获取数据
                }
            },
            "pagiNation.pageSize": {
                handler: function (newValue, oldValue)
                {
                    this.filters.rows = newValue;
                    if (this.pagiNation.currentPage == 1)
                        this.tb_GetData();//重新获取数据
                    else
                        this.pagiNation.currentPage = 1;
                }
            }
        },//监听属性变化
        methods:{
            el_FormFieldRules:function(rowConfig,isSearchForm){
                //是否搜索form
                var t_isSearchForm = typeof(isSearchForm)
                if(t_isSearchForm==='undefined'||isSearchForm==null ||t_isSearchForm!=='boolean')
                    isSearchForm=false;
                var ArrRules = [];
                if(!rowConfig.Editable && !isSearchForm)
                    return ArrRules;
                if(rowConfig.Required && !isSearchForm){
                    ArrRules.push({ required: true, message: '请输入'+rowConfig.DisplayName||rowConfig.Name, trigger: ['blur','change'] });
                }
                var name = rowConfig.Name.toLowerCase()
                if(name=='email' || rowConfig.isEmail)
                    ArrRules.push({ type: 'email', message: '请输入正确的邮箱地址', trigger: ['blur', 'change'] });
                if(name.indexOf('password')==0)
                    ArrRules.push({ validator :this.$Validtors.PasswordValidator, trigger: ['blur', 'change'] });
                if(name.indexOf('idcard')==0 && rowConfig.inputType=='text')
                    ArrRules.push({ validator: this.$Validtors.IdCardValidator, trigger: 'blur' });

                if(rowConfig.MinLength || rowConfig.MaxLength ){
                    var rule = { trigger: ['blur','change'] }
                    if(rowConfig.MinLength){
                        rule.min = rowConfig.MinLength;
                        if(rowConfig.MaxLength)
                            rule.message = '字符长度必须介于 '+rowConfig.MinLength+' 到 '+rowConfig.MaxLength+' 之间';
                        else{
                            rule.message = '字符长度 必须大于 '+rowConfig.MinLength;
                        }
                    }
                    if(rowConfig.MaxLength){
                        rule.max = rowConfig.MaxLength;
                        if(rowConfig.MinLength)
                            rule.message = '字符长度 必须介于 '+rowConfig.MinLength+' 到 '+rowConfig.MaxLength+' 之间';
                        else
                            rule.message = '字符长度 必须小于 '+rowConfig.MaxLength;
                    }

                    ArrRules.push(rule);
                }
                return ArrRules;
            },//输出input验证规则
            el_inputType:function(rowConfig){
                var elInputType = 'input';
                if(rowConfig.Type=='number'){
                    elInputType = 'input-number';
                }
                if(rowConfig.Type=='boolean')
                    elInputType = 'checkbox';
                if(rowConfig.Type=='datetime')
                    elInputType = 'date-picker'
                //ES5 模板替换
                return `el-${elInputType}`;// "el-"+elInputType;
            },//判断input输出格式
            el_inputProtoType:function(field,isSearchForm){//el_input-Type属性
                //是否搜索form
                var t_isSearchForm = typeof(isSearchForm)
                if(t_isSearchForm==='undefined'||isSearchForm==null ||t_isSearchForm!=='boolean')
                    isSearchForm=false;
                let filterData = isSearchForm?this.filters.filterRules:this.curr_rowdata;
                //设置零时变量，记录$inputType
                if(filterData['$'+field.Name+'inputType']==undefined||filterData['$'+field.Name+'inputType']==null){
                    if(field.inputType =='datetime' && isSearchForm)
                        return 'daterange';
                    else
                        return field.inputType;
                }
                else
                    return filterData['$'+field.Name+'inputType'];
            },//el_input-Type属性
            el_inputClass:function(field){
                if(field.Name.toLowerCase().indexOf('password')>=0){
                    let curr_rowdata = this.curr_rowdata;
                    let name = '$'+field.Name+'pswView';
                    let inputClass = {'fa-eye-slash':false,'fa-eye':curr_rowdata[name]};
                    if(curr_rowdata[name]==undefined||curr_rowdata[name]==null)
                        inputClass['fa-eye-slash'] = true;
                    else{
                        inputClass['fa-eye-slash'] = !curr_rowdata[name],inputClass['fa-eye']=curr_rowdata[name];
                    }
                    return inputClass;
                }else{
                    return {'el-icon-edit':true};
                }
            },//password 显示/隐藏 class
            pswView:function(field){//密码框 显示隐藏
                var pswView = '$'+field.Name+'pswView';
                var inputType = '$'+field.Name+'inputType';
                if(this.curr_rowdata[pswView]==undefined||this.curr_rowdata[pswView]==null)
                    this.$set(this.curr_rowdata,pswView,true),this.$set(this.curr_rowdata,inputType,'text');
                else if(!this.curr_rowdata[pswView])
                    this.curr_rowdata[pswView] = true,this.curr_rowdata[inputType]='text';
                else
                    this.curr_rowdata[pswView] = false,this.curr_rowdata[inputType]='password';
            },//密码框 显示隐藏
            keydown: function (e)//dom原生控件keydown事件 v-on:keydown.native="keydown"
            {
                return true;
                var event = e || window.event;//事件
                var keycode = event.keycode || event.which;//键码

                //取消事件冒泡(W3C)
                if (event && event.stopPropagation)
                    event.stopPropagation();
                else
                    //IE中取消事件冒泡
                    window.event.cancelBubble = true;
                //阻止默认浏览器动作(W3C)
                if (event && event.preventDefault)
                    event.preventDefault();
                else//IE中阻止函数器默认动作的方式
                    window.event.returnValue = false;

                console.log('keydown', event, this);
                event.returnValue = false;
                //window.event.returnValue = false;
                return false;
            },//dom原生控件keydown事件 v-on:keydown.native="keydown"
            tb_GetData: function (e)//获取数据
            {
                this.tbLoading = true;//加载中
                var filterData = this.filters;//获取搜索条件值
                //console.log('tb_GetData', filterData, e);
                let url = this.tbUrl.controller + this.tbUrl.getdata;
                var paramData = $.extend({}, this.filters);
                var SearchFilter = this.getSearchFilter();//搜索{}转换为[]
                paramData.filterRules = JSON.stringify(SearchFilter);
                this.$http.get(url, {
                    params: paramData,
                    headers: {//指示为 ajax请求
                        "X-Requested-With": "XMLHttpRequest"
                    }
                }).then(function(success){//成功
                    try{
                        this.tableData = success.body.rows;
                        this.pagiNation.total = success.body.total;
                    } catch (e) {
                        this.$message({
                            duration:0,//不自动关闭
                            showClose: true,
                            message: '数据处理，出现错误',
                            type: 'error'
                        });
                    }
                    this.tbLoading = false;//加载完毕
                },function(error){//错误
                    this.tbLoading = false;//加载完毕
                    this.$message({
                        duration:0,//不自动关闭
                        showClose: true,
                        message: '获取数据出现错误',
                        type: 'error'
                    });
                });
            },//获取数据
            getSearchFilter:function(){//搜索条件{}转换为[]
                var ArrFilter=[];
                var filterRules = $.extend({}, this.filters.filterRules);
                if (!(typeof (filterRules) === 'undefined' || filterRules == null || filterRules === JSON.parse('{}')))
                {
                    var ArrKey = Object.keys(filterRules);
                    for (var i in ArrKey) {
                        var key = ArrKey[i];
                        var value = filterRules[key];
                        if (ObjectIsArray(value)) {
                            ArrFilter.push({ field: "_" + key, op: 'equals', value: value[0] });
                            ArrFilter.push({ field: key + "_", op: 'equals', value: value[1] });
                        } else
                            ArrFilter.push({ field: key, op: 'equals', value: value });
                    }
                }
                return ArrFilter;
            },//搜索条件{}转换为[]
            search:function(){
                var thisVue = this;
                this.$refs['tb_search'].validate(function(valid){
                    if (valid) {
                        thisVue.pagiNation.currentPage==1?thisVue.tb_GetData():thisVue.pageCurrentChange(1);
                    }
                });
            },//搜索
            resetFilter:function(searchForm){
                this.$refs[searchForm].resetFields();
                if(this.pagiNation.currentPage==1)
                    this.tb_GetData();
                else
                    this.pageCurrentChange(1);//重新获取数据
            },//重设搜索条件
            handleViewClick:function(row){
                //console.log('row',row);
            },//行查看按钮
            handledblclick:function(row){
                this.centerDialogVisible = true;
                this.curr_rowdata_Original = row;//原始行数据
                this.curr_rowdata = Object.assign({}, row);
                let curr_rowdata = this.curr_rowdata;
                let ArrEnumField = this.ArrEnumField;//所有select/枚举
                let thisVue = this;
                Object.keys(this.curr_rowdata).forEach(function(item,index){
                    let val = curr_rowdata[item]+'';
                    if(!ObjectIsEmpty(val)){
                        if(val.indexOf('/Date(')>=0){
                            var d = new moment(val);
                            if (d.isValid())
                                curr_rowdata[item] = d.toDate();
                        }
                        var ArrFilter = ArrEnumField.filter(function(field){return field.Name === item;});
                        if(ArrFilter.length>0){
                            let OFilter = ArrFilter[0];
                            let url = OFilter.ForeignKeyGetListUrl;//'/MenuItems/GetData';
                            if(!ObjectIsEmpty(url) && url.indexOf('GetPagerEnum')<0)
                                thisVue.el_remoteMethod(val,OFilter,'form',false);
                        }
                    }
                });
                //console.log('row-dblclick',row);
            },//双击行
            handleAddRow:function(e){
                //console.log('handleAddRow',e);
                this.centerDialogVisible = true;
                this.curr_rowdata = { Id: --this.AddNum };
                this.dlgLoading = false;
            },//增加行数据 弹出框添加
            deleteRow: function (index, row)
            {
                //this.tableData.splice(index, 1);
                var batchSaveData = {};//批量操作数据
                var thisVue = this;
                batchSaveData.deleted = [row];
                var url = thisVue.tbUrl.controller + thisVue.tbUrl.batchSave;//批量保存url
                thisVue.tbLoading = true;//加载中
                //提交数据
                thisVue.$http.post(url, batchSaveData, {
                    headers: {//指示为 ajax请求
                        "X-Requested-With": "XMLHttpRequest"
                    }
                }).then(function (success)
                {//成功
                    thisVue.tbLoading = false;//加载完毕
                    try {
                        var retData = success.body;
                        if (retData.Success) {
                            thisVue.tb_GetData();//删除数据后，重新获取数据
                        } else {
                            thisVue.$message({
                                duration:0,//不自动关闭
                                showClose: true,
                                message: '删除错误:' + retData.ErrMsg,
                                type: 'error'
                            });
                        }
                    } catch (e) {
                        thisVue.$message({
                            duration:0,//不自动关闭
                            showClose: true,
                            message: '删除数据处理，出现错误',
                            type: 'error'
                        });
                    }
                }, function (error)
                {//错误
                    thisVue.tbLoading = false;//加载完毕
                    thisVue.$message({
                        duration:0,//不自动关闭
                        showClose: true,
                        message: '删除数据，出现错误',
                        type: 'error'
                    });
                });
            },//行删除按钮-删除行搜索条件
            handledelSeltRow:function(e){
                //console.log('handledelSeltRow', e, this.selctRows);
                if (this.selctRows.length <= 0) {
                    this.$message({
                        duration:0,//不自动关闭
                        showClose: true,
                        message: '错误:未选择需要删除的数据',
                        type: 'error'
                    });
                } else {
                    var batchSaveData = {};//批量操作数据
                    var thisVue = this;
                    batchSaveData.deleted = thisVue.selctRows;
                    var url = thisVue.tbUrl.controller + thisVue.tbUrl.batchSave;//批量保存url
                    thisVue.tbLoading = true;//加载中
                    //提交数据
                    thisVue.$http.post(url, batchSaveData, {
                        headers: {//指示为 ajax请求
                            "X-Requested-With": "XMLHttpRequest"
                        }
                    }).then(function (success){//成功
                        thisVue.tbLoading = false;//加载完毕
                        try {
                            var retData = success.body;
                            if (retData.Success) {
                                thisVue.tb_GetData();//删除数据后，重新获取数据
                            } else {
                                thisVue.$message({
                                    duration:0,//不自动关闭
                                    showClose: true,
                                    message: '删除错误:' + retData.ErrMsg,
                                    type: 'error'
                                });
                            }
                        } catch (e) {
                            thisVue.$message({
                                duration:0,//不自动关闭
                                showClose: true,
                                message: '删除数据处理，出现错误',
                                type: 'error'
                            });
                        }
                    }, function (error){//错误
                        thisVue.tbLoading = false;//加载完毕
                        thisVue.$message({
                            duration:0,//不自动关闭
                            showClose: true,
                            message: '删除数据，出现错误',
                            type: 'error'
                        });
                    });
                }
                //rows.splice(index, 1);
            },//删除选中行数据
            handleSelectionChange:function(selections){
                this.selctRows = selections;
                //console.log('handleSelectionChange',selections);
            },//选择数据变更
            tbSortChange: function (sortObj)//{column:列,prop:字段,sort:排序}
            {
                //console.log('tbSortChange', sortObj);
                var IsReload = false;
                if (!(typeof (sortObj) === 'undefined' || sortObj == null || JSON.stringify(sortObj) === '{}')) {
                    var sort = sortObj.prop;
                    if (!(typeof (sortObj.prop) === 'undefined' || sortObj.prop == null || sortObj.prop == '')) {
                        sort = sortObj.prop;
                    } else
                        sort = 'Id';
                    var order = sortObj.order;
                    if (!(typeof (order) === 'undefined' || order == null || order == '')) {
                        order = sortObj.order.replace('ending', '');
                    } else
                        order = 'desc';
                    if (this.filters.sort != sort || this.filters.order != order) {
                        IsReload = true;
                        this.filters.sort = sort;
                        this.filters.order = order;
                    }
                } else {
                    if (this.filters.sort != "Id" ||this.filters.order != 'descending')
                        IsReload = true;
                    this.filters.sort = "Id";
                    this.filters.order = "desc";
                }
                if (IsReload) {
                    if(this.pagiNation.currentPage==1)
                        this.tb_GetData();
                    else
                        this.pageCurrentChange(1);//重新获取数据
                    //this.tb_GetData();
                }
            },//table排序变更
            dlgClose: function (){//弹出框关闭时触发
                $.extend(this.curr_rowdata, this.curr_rowdata_Original);
            },//弹出框关闭时触发
            dlgSubmit:function(e){
                let thisVue = this;
                let MyForm = this.$refs['MyForm'];
                //MyForm.resetFields();//清除验证
                MyForm.clearValidate();//清除验证
                MyForm.validate(function(valid){
                    if (valid) {
                        thisVue.dlgLoading = true;//弹出框加载中
                        var url = thisVue.tbUrl.controller + thisVue.tbUrl.batchSave;//批量保存url
                        var batchSaveData = {//批量操作数据
                            inserted: [],
                            deleted: [],
                            updated: []
                        };
                        if (thisVue.curr_rowdata.Id <= 0) {
                            batchSaveData.inserted.push(thisVue.curr_rowdata);
                        } else {
                            batchSaveData.updated.push(thisVue.curr_rowdata);
                        }
                        //提交数据
                        thisVue.$http.post(url,batchSaveData, {
                            headers: {//指示为 ajax请求
                                "X-Requested-With": "XMLHttpRequest"
                            }
                        }).then(function (success){//成功
                            thisVue.dlgLoading = false;//弹出框加载完毕
                            try {
                                var retData = success.body;
                                if (retData.Success) {
                                    thisVue.centerDialogVisible = false;//显示/关闭弹出框
                                    if (thisVue.curr_rowdata.Id <= 0){
                                        if(thisVue.pagiNation.currentPage==1)
                                            thisVue.tb_GetData();
                                        else
                                            thisVue.pageCurrentChange(1);//新增数据时，重新获取数据
                                    }
                                    else
                                        $.extend(thisVue.curr_rowdata_Original, thisVue.curr_rowdata);
                                }else{
                                    thisVue.$message({
                                        duration:0,//不自动关闭
                                        showClose: true,
                                        message: '错误:' + retData.ErrMsg,
                                        type: 'error'
                                    });
                                }
                            } catch (e) {
                                thisVue.$message({
                                    duration:0,//不自动关闭
                                    showClose: true,
                                    message: '提交数据处理，出现错误',
                                    type: 'error'
                                });
                            }
                        }, function (error){//错误
                            thisVue.dlgLoading = false;//弹出框加载完毕
                            thisVue.$message({
                                duration:0,//不自动关闭
                                showClose: true,
                                message: '提交数据出现错误',
                                type: 'error'
                            });
                        });
                    } else {
                        console.log('error submit!!');
                        return false;
                    }
                });
            },//弹出框提交数据
            //----翻页控件事件
            pageSizeChange: function (pageSize)
            {
                //this.filters.rows =
                this.pagiNation.pageSize = pageSize;
                //this.pageCurrentChange(1);//重新获取数据
                //console.log('handleSizeChange', this.pagiNation, this.filters);
            },//改变显示条数触发事件
            pageCurrentChange: function (currentPage)
            {
                //this.filters.page =
                this.pagiNation.currentPage = currentPage;
                //this.tb_GetData();//重新获取数据
                //console.log('handleCurrentChange',currentPage);
            },//改变当前页触发事件
            PrevPage:function(currentPage){
                //console.log('handlePrevPage',currentPage);
            },//点击上一页触发事件
            NextPage:function(currentPage){
                //console.log('handleNextPage',currentPage);
            },//点击下一页触发事件
            //翻页控件事件----
            //导出 导入 Excel
            ExportXls:function(JsonData,fileName){
                //console.log('ExportXls');
                require(['xlsx','file-saver'],function(XLSX,FileSaver){
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
                        const s2ab = function(s){//字符串转字符流
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
                        FileSaver.saveAs(new Blob([s2ab(wbout)], { type: 'application/octet-stream' }),fileName+'.xlsx');
                    } catch (e){
                        if (typeof console !== 'undefined')
                            console.log(e, wbout)
                    }
                    return wbout
                });
            },//导出数据
            ImportXls:function(){
                //console.log('ImportXls');
            },//导入数据
            formatter:function(field){//el-table-column 数据显示转换
                var formatter = null;
                switch(field.Type)
                {
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
                if(lower_Name.indexOf('sex')==0){
                    formatter = this.$formatter.Sexformatter;
                }
                if(!ObjectIsEmpty(field.ForeignKeyGetListUrl)){
                    formatter = this.$formatter.joinformatter;
                }
                //if (lower_Name.indexOf('photo') >= 0){
                //    formatter = this.$formatter.photoformatter;
                //}
                return formatter;
            },//el-table-column 数据显示转换
            el_remoteMethod:function(query,field,profx,forceload){
                let ArrOptionName = field.Name+'_'+profx;
                if (!ObjectIsEmpty(query) || !ObjectIsEmpty(forceload)) {
                    this.el_selt.el_selt_loading = true;
                    var paramData = {filterRules:JSON.stringify([{ field: "q", op: 'equals', value: query }])};
                    //let thisVue = this;
                    let url = field.ForeignKeyGetListUrl;//'/MenuItems/GetData';
                    this.$http.get(url, {
                        params: paramData,
                        headers: {//指示为 ajax请求
                            "X-Requested-With": "XMLHttpRequest"
                        }
                    }).then(function(success){//成功
                        try{
                            if(typeof this.el_selt[ArrOptionName] ==='undefined')
                                this.$set(this.el_selt,ArrOptionName,{});
                            if(ObjectIsEmpty(success.body.rows)){
                                this.$set(this.el_selt[ArrOptionName],"ArrOption",success.body);
                            }else
                                this.$set(this.el_selt[ArrOptionName],"ArrOption",success.body.rows);
                        } catch (e) {
                            this.$message({
                                duration:0,//不自动关闭
                                showClose: true,
                                message: '数据处理，出现错误',
                                type: 'error'
                            });
                        }
                        this.el_selt.el_selt_loading = false;//加载完毕
                    },function(error){//错误
                        this.el_selt.el_selt_loading = false;//加载完毕
                        this.$message({
                            duration:0,//不自动关闭
                            showClose: true,
                            message: '获取数据出现错误',
                            type: 'error'
                        });
                    });
                } else {
                    if(typeof this.el_selt[ArrOptionName] ==='undefined')
                        this.el_selt[ArrOptionName]={};
                    this.el_selt[ArrOptionName]["ArrOption"] = [];
                }
            },//外键触发搜索
        }
    };
    if (typeof module !== 'undefined')
        module.exports = myMixin;
    else if (typeof define === "function" && define.amd){
        return myMixin;
    }else
        _global.myMixin = myMixin;//window.myMixin
}));