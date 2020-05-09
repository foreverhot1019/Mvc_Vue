/*requireJs 模块配置*/
requirejs.config({
    //By default load any module IDs from /Scripts
    baseUrl: '/Scripts',//根目录
    waitSeconds: 10,//等待资源时间 10秒
    urlArgs: jsVersion,//文件url加上参数-避免緩存
    map: {//插件
        '*': {
            'css': 'requireJs/require_css.min'
        }
    },
    //except, if the module ID starts with "app",
    //load it from the js/app directory. paths
    //config is relative to the baseUrl, and
    //never includes a ".js" extension since
    //the paths config could be for a directory.
    paths: {
        ace: '../AceAssets/js/ace.min',
        aceElement: "../AceAssets/js/ace-elements.min",
        vue: 'vue.min',//写成vue，element-ui 会require('vue'),名字对不上会去取 /Scripts/vue.js，导致冲突 Tooltip弹不出来
        _vue_resource: 'vue-resource.min',
        ELEMENT: 'ElementUI/element-ui.min',//必须是 ELEMENT，define 赋值了已命名对象
        element_ui_css: '../Content/ElementUI/element-ui',
        _dialogdrag: 'Vue/dialogDrag',//el-dialog 弹出框扩展最大化，拖拽
        _formatter: 'Vue/formatter',//全局设置 显示格式化 function
        _validtors: 'Vue/validtors',//全局设置 验证规则
        _VueFilters: 'Vue/VueFilters',//全局设置 显示格式化 注册filter({{A|filterA}})
        LazyLoadingComponent: 'Vue/LazyLoadingComponent',//懒加载Vue组件，带加载中效果
        IndexBaseMixin: 'Vue/IndexBaseMixin',//页面是Index时，CRUD 都已实现只需配置ArrField与CRUD权限
        xlsx: 'xlsx.full.min',//table转excel
        'file-saver': 'FileSaver',//文件保存
        Convert_Pinyin: 'Convert_Pinyin',//中文转拼音
    },
    shim: {//处理模块 依赖和输出
        ace: {
            deps: ['aceElement'],
            exports: 'ace',
        },
        vue: {
            //deps: [],
            exports: 'Vue'
        },
        ELEMENT: {
            deps: ['_formatter', '_validtors', '_VueFilters', 'css!element_ui_css', '_dialogdrag'],//
            exports: 'ELEMENT'
        },
        _formatter: {
            deps: ['vue']
        },
        _validtors: {
            deps: ['vue']
        },
        _VueFilters: {
            deps: ['vue']
        },
        _vue_resource: {
            deps: ['vue']
        },
        _dialogdrag: {
            deps: ['vue']
        },
        LazyLoadingComponent: {
            deps: ['vue', 'ELEMENT']
        },
        xlsx: {
            deps: ['file-saver']
        },
        'file-saver': {
            //deps: [],
            exports: 'FileSaver'
        },
    }
});