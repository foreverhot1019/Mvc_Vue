;(function ($) {
    var methods = {
        Init: function (options) {
            //console.log('Init', this);
            return this.each(function () {
                // Create a reference to the jQuery DOM object
                var $this = $(this);
                $this.data('NavResize', {
                    inputs: {}, // The object that contains all the file inputs
                    FirstnavClick: false,
                    navCookieName: 'min_navbar',//cookie 名称
                    navCookieVal: null//cookie值
                });

                var $data = $this.data('NavResize');
                var settings = $data.settings = $.extend({
                    Auto: true,                               //自动找datagrid并自适应width
                    navbarlinkid: 'navbar-minimalize',        //收缩菜单 按钮Id
                    delay: 300                                //延迟执行自适应width方法时间
                    /*
                    // Events
                    'OnNavResizefunc': function () { }                        // 点击收缩菜单 按钮 后的回调方法 自适应width方法
                    */
                }, options);

                $data.navresizeauto = function () {
                    var options = $data.settings;
                    $("#" + options.navbarlinkid).bind("click", function () {
                        if (!$data.FirstnavClick) {
                            if ($data.CookieVal == null || typeof ($data.CookieVal) === undefined || $data.CookieVal == '') {
                                $.cookie($data.navCookieName, '1', { path: '/' });
                                $data.CookieVal = '1';
                                //console.log('CookieVal-1', $.cookie(navCookieName));
                            }
                            else {
                                $.cookie($data.navCookieName, '', { path: '/' });
                                $data.CookieVal = '';
                                //console.log('CookieVal-0', $.cookie(navCookieName));
                            }
                        }
                        else {
                            $data.FirstnavClick = false;
                            $("div[class='panel datagrid']").each(function (i, item) {
                                $(item).find("table.easyui-datagrid").each(function (x, elem) {
                                    setTimeout(function () {
                                        var $head = $("div.row.wrapper.border-bottom.white-bg");
                                        var width = $head.width() - 60;//$(item).parent().width() - 10 
                                        //console.log(width);
                                        $(elem).datagrid("resize", { width: width });
                                        if (typeof options.OnNavResizefunc === 'function') {
                                            options.OnNavResizefunc.call();
                                        }
                                    }, options.delay);
                                });
                            });
                            return;
                        }

                        //console.log(options.Auto, typeof options.OnNavResizefunc);
                        if (options.Auto) {
                            $("div[class='panel datagrid']").each(function (i, item) {
                                $(item).find("table.easyui-datagrid").each(function (x, elem) {
                                    setTimeout(function () {
                                        var $head = $("div.row.wrapper.border-bottom.white-bg");
                                        var width = $head.width() - 60;//$(item).parent().width() - 10 
                                        $(elem).datagrid("resize", { width: width });
                                        if (typeof options.OnNavResizefunc === 'function') {
                                            options.OnNavResizefunc.call();
                                        }
                                    }, options.delay);
                                });
                            });
                        }
                        else {
                            if (typeof options.OnNavResizefunc === 'function') {
                                setTimeout(function () {
                                    options.OnNavResizefunc.call();//Array.prototype.slice.call(arguments, 1)
                                }, options.delay);
                            }
                            else {
                                $("div[class='panel datagrid']").each(function (i, item) {
                                    $(item).find("table.easyui-datagrid").each(function (x, elem) {
                                        setTimeout(function () {
                                            var $head = $("div.row.wrapper.border-bottom.white-bg");
                                            var width = $head.width() - 60;//$(item).parent().width() - 10 
                                            $(elem).datagrid("resize", { width: width });
                                        }, options.delay);
                                    });
                                });
                            }
                        }
                    });
                    if (navigator.cookieEnabled) {
                        $data.CookieVal = $.cookie($data.navCookieName);
                    }

                    if (!($data.CookieVal == null || typeof ($data.CookieVal) === undefined || $data.CookieVal == '')) {
                        $data.FirstnavClick = true;
                        $("#" + options.navbarlinkid).click();
                    }
                }
                $data.navresizeauto.call();
            });
        },
        //对所有的操作
        getOptions: function () {
            var retoptions = {};
            this.each(function () {
                var $this = $(this),
                    $data = $this.data('NavResize'),
                    settings = $data.settings;
                retoptions = settings;
                return;

                //// Trigger the onClearQueue event
                //if (typeof settings.onClearQueue === 'function') {
                //    settings.onClearQueue.call($this, $('#' + $data.settings.queueID));
                //}
            });
            return retoptions;
        }
    };
    $.fn.NavResize = function (method) {
        if (methods[method]) {
            return methods[method].apply(this, Array.prototype.slice.call(arguments, 1));
        } else if (typeof method === 'object' || !method) {
            return methods.Init.apply(this, arguments);
        } else {
            $.error('The method ' + method + ' does not exist in $.NavResize');
        }
    }
})(jQuery);


//var NavResize = (function () {
//    var options = {};
//    var FirstnavClick = false;
//    //var navResizefuncName = '';//改变宽度后的func方法
//    var navCookieName = 'min_navbar';//cookie 名称
//    var navCookieVal = null;//cookie值

//    function navresizeauto(options) {
//        if (navigator.cookieEnabled) {
//            CookieVal = $.cookie(navCookieName);
//        }
//        $("#" + options.navbarlinkid).bind("click", function () {
//            if (!FirstnavClick) {
//                if (CookieVal == null || typeof (CookieVal) === undefined || CookieVal == '') {
//                    $.cookie(navCookieName, '1', { path: '/' });
//                    CookieVal = '1';
//                    //console.log('CookieVal-1', $.cookie(navCookieName));
//                }
//                else {
//                    $.cookie(navCookieName, '', { path: '/' });
//                    CookieVal = '';
//                    //console.log('CookieVal-0', $.cookie(navCookieName));
//                }
//            }
//            else {
//                FirstnavClick = false;
//                return;
//            }

//            console.log(options.Auto, typeof options.OnNavResizefunc);
//            if (options.Auto) {
//                $("div[class='panel datagrid']").each(function (i, item) {
//                    $(item).find("table.easyui-datagrid").each(function (x, elem) {
//                        setTimeout(function () {
//                            $(elem).datagrid("resize", { width: $(item).parent().width() - 10 });
//                            if (typeof options.OnNavResizefunc === 'function') {
//                                options.OnNavResizefunc.call();
//                            }
//                        }, options.delay);
//                    });
//                });
//            }
//            else {
//                if (typeof options.OnNavResizefunc === 'function') {
//                    setTimeout(function () {
//                        options.OnNavResizefunc.call();
//                    }, options.delay);
//                }
//                else {
//                    $("div[class='panel datagrid']").each(function (i, item) {
//                        $(item).find("table.easyui-datagrid").each(function (x, elem) {
//                            setTimeout(function () {
//                                $(elem).datagrid("resize", { width: $(item).parent().width() - 10 });
//                            }, options.delay);
//                        });
//                    });
//                }
//            }
//        });
//        if (!(CookieVal == null || typeof (CookieVal) === undefined || CookieVal == '')) {
//            FirstnavClick = true;
//            $("#" + options.navbarlinkid).click();
//        }
//    }

//    return function (arg) {
//        options = $.extend({
//            Auto: true,//自动找datagrid并自适应width
//            navbarlinkid: 'navbar-minimalize',//收缩菜单 按钮Id
//            delay: 300//延迟执行自适应width方法时间
//            //OnNavResizefunc: function () { }//点击收缩菜单 按钮 后的回调方法 自适应width方法
//        }, arg);
//        navresizeauto(options);
//    };
//})();
//if (typeof jQuery !== 'undefined') {
//    jQuery.extend({
//        NavResize: function (arg) {
//            //console.log(arg);
//            return NavResize(arg);
//        }
//    });
//}