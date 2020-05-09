(function () {
    var DownLoadProgress = function (options) {
        var HtmlDownLoadProgress = '' +
        '<div class="modal fade" id="DownLoadProgrsformModal" tabindex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true" data-backdrop="static">                                 ' +
        '    <div class="modal-dialog">                                                                                                                                                                   ' +
        '        <div class="modal-content">                                                                                                                                                              ' +
        '            <div class="modal-header">                                                                                                                                                           ' +
        '                <button type="button" class="close" id="DownLoadDrdDtlProgrsClose" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>                      ' +
        '                <h4 class="modal-title" id="myModalLabel">订单明细下载</h4>                                                                                                                      ' +
        '            </div>                                                                                                                                                                               ' +
        '            <div class="modal-body" id="DownLoadDrdDtlProgrsformModal-body">                                                                                                                     ' +
        '                <div class="row">                                                                                                                                                                ' +
        '                    <div class="col-lg-12 col-md-12 col-sm-12">                                                                                                                                  ' +
        '                        <div id="div_progressbar" class="progress progress-striped active">                                                                                                      ' +
        '                            <div id="div_UploadSuccess" class="progress-bar progress-bar-success" role="progressbar" aria-valuenow="60" aria-valuemin="0" aria-valuemax="100" style="width: 0%;">' +
        '                                <span class="sr-only">0% 已上传</span>                                                                                                                           ' +
        '                            </div>                                                                                                                                                               ' +
        '                            <div id="div_UploadDanger" class="progress-bar progress-bar-danger" role="progressbar" aria-valuenow="60" aria-valuemin="0" aria-valuemax="100" style="width: 100%;">' +
        '                                <span class="sr-only">100% 未上传</span>                                                                                                                         ' +
        '                            </div>                                                                                                                                                               ' +
        '                        </div>                                                                                                                                                                   ' +
        '                        <div id="div_progressinfo"></div>                                                                                                                                        ' +
        '                        <div id="div_speed"></div>                                                                                                                                               ' +
        '                        <div id="div_time"></div>                                                                                                                                                ' +
        '                    </div>                                                                                                                                                                       ' +
        '                </div>                                                                                                                                                                           ' +
        '            </div>                                                                                                                                                                               ' +
        '            <div class="modal-footer">                                                                                                                                                           ' +
        '                <button type="button" id="DownLoadDrdDtlProgrsCancel" class="btn btn-default" data-dismiss="modal">取消</button>                                                                 ' +
        '                <button type="button" id="DownLoadDrdDtlProgrsOK" class="btn btn-primary" data-dismiss="modal">确定</button>                                                                                          ' +
        '            </div>                                                                                                                                                                               ' +
        '        </div>                                                                                                                                                                                   ' +
        '    </div>                                                                                                                                                                                       ' +
        '</div>';

        var NewId = 'DownLoadProgrsformModal';
        if ($("div[id='" + NewId + "']").length==0) {
            $("body").append(HtmlDownLoadProgress);
        }

        var settings = $.extend({
            auto: true,      //自动开始
            url: '',         //下载文件的url
            method: 'Get',   //链接打开方式
            data: {},         //要发送的数据   
            downLoadInfo: '下载文件', //下载界面文字
            closeDelay: 2000, //自动关闭时间(毫秒)
            callbackFuc: null //回掉函数
            /*
            // Events
            'downloadFilefunc': function () { }                        // 点击收缩菜单 按钮 后的回调方法 自适应width方法
            */
        }, options);
        //html5下载
        function downloadFile(method, url, data, success) {
            var xhr = new XMLHttpRequest();
            //open(method, url, async, username, password)
            xhr.open(method, url, true);
            xhr.responseType = "blob";
            xhr.onreadystatechange = function () {
                if (xhr.readyState == 4) {
                    $("#div_time", "#" + NewId).html("下载耗时：" + (+new Date - startTime) / 1000 + "s");
                    $("#div_time", "#" + NewId).show();
                    if (success)
                        success(xhr);
                }
            };

            ////下载进度条界面 取消时的操作 停止下载
            //$("button[id=DownLoadDrdDtlProgrsClose],button[id=DownLoadDrdDtlProgrsCancel],button[id=DownLoadDrdDtlProgrsOK]","#DownLoadDrdDtlProgrsformModal").on("click", function () {
            //    try
            //    {
            //        if (xhr.readyState == 4) {
            //            $("#DownLoadDrdDtlProgrsformModal").modal('toggle');
            //        }
            //    }
            //    catch(e)
            //    {
            //        $("#DownLoadDrdDtlProgrsformModal").modal('toggle');
            //    }
            //});

            var lastLoaded = 0, speed = 0, lastTime = +new Date, startTime = lastTime;
            var speedText = $("#div_speed", "#" + NewId);
            var loadedInfo = $("#div_progressinfo", "#" + NewId);
            ////文件读取开始时触发。
            //xhr.addEventListener("Onloadstart",function(e){
            //},false);
            //进行中时定时触发
            xhr.addEventListener("progress", function (e) {
                var currTime = +new Date;
                var currLoaded = e.loaded;
                var dT = currTime - lastTime;
                var dL = currLoaded - lastLoaded;

                lastTime = currTime;
                lastLoaded = currLoaded;

                speed = parseInt(dL / dT);
                speedText.html("下载速度 " + speed + " kb/s");

                var percent = (currLoaded / e.total);
                loadedInfo.html("文件大小： " + (e.total / 1024 / 1024).toFixed(2) + "M，已下载：" + (currLoaded / 1024 / 1024).toFixed(2) + "M  <br />进度：" + (percent * 100).toFixed(2) + "%");

                $("#div_UploadSuccess", "#" + NewId).css({ width: (percent * 100).toFixed(2).toString() + '%' });
                $("#div_UploadDanger", "#" + NewId).css({ width: (100 - (percent * 100)).toFixed(2).toString() + '%' });
            });
            //被中止时触发
            xhr.addEventListener("abort", function (e) {
                alert('下载被终止');
                $("#div_time", "#" + NewId).html("");
                $("#div_time", "#" + NewId).hide();
                $("#div_speed", "#" + NewId).html("");
                $("#div_progressinfo", "#" + NewId).html("");
                $("#div_UploadSuccess", "#" + NewId).css({ width: '0%' });
                $("#div_UploadDanger", "#" + NewId).css({ width: '100%' });
                $("#" + NewId).modal('toggle');
            }, false);
            //出错时触发
            xhr.addEventListener("error", function (e) {
                alert('下载时发生错误');
                $("#div_time", "#" + NewId).html("");
                $("#div_time", "#" + NewId).hide();
                $("#div_speed", "#" + NewId).html("");
                $("#div_progressinfo", "#" + NewId).html("");
                $("#div_UploadSuccess", "#" + NewId).css({ width: '0%' });
                $("#div_UploadDanger", "#" + NewId).css({ width: '100%' });
                $("#" + NewId).modal('toggle');
            }, false);
            //成功完成时触发
            xhr.addEventListener("load", function (e) {
            }, false);
            //完成时，无论成功或者失败都会触发
            xhr.addEventListener("loadend", function (e) {
            }, false);
            // 不支持FormData的浏览器的处理 
            if (typeof FormData == "undefined") {
                xhr.setRequestHeader("Content-Type", "application/x-www-form-urlencoded");
            }
            //发送请求
            xhr.send(postDataFormat(data));
        }
        function postDataFormat(obj) {
            if (typeof obj != "object") {
                alert("输入的参数必须是对象");
                return;
            }

            // 支持有FormData的浏览器（Firefox 4+ , Safari 5+, Chrome和Android 3+版的Webkit）
            if (typeof FormData == "function") {
                var data = new FormData();
                for (var attr in obj) {
                    data.append(attr, obj[attr]);
                }
                return data;
            } else {
                // 不支持FormData的浏览器的处理 
                var arr = new Array();
                var i = 0;
                for (var attr in obj) {
                    arr[i] = encodeURIComponent(attr) + "=" + encodeURIComponent(obj[attr]);
                    i++;
                }
                return arr.join("&");
            }
        }
        //分析Url头 需要用时 解开顶部 javascript when.js 注释
        function UriAnalyser(url) {
            var deferred = when.defer();
            var xhr = new XMLHttpRequest();
            xhr.open('HEAD', url, true);
            xhr.onreadystatechange = function () {
                if (2 == this.readyState) {
                    var ret = {
                        mimeType: xhr.getResponseHeader('Content-Type'),
                        size: xhr.getResponseHeader('Content-Length') | 0,
                        filename: xhr.getResponseHeader('Content-Disposition'),
                    };
                    debugger;
                    deferred.resolve(ret);
                }
            };
            xhr.send();
            return deferred.promise;
        }
        //开始下载
        function StartDownLoad() {
            if (typeof (settings.url) === 'undefined' || settings.url == null) {
                return;
            }
            $("#" + NewId).modal('toggle');
            $("#div_time", "#" + NewId).html("");
            $("#div_time", "#" + NewId).hide();
            $("#div_speed", "#" + NewId).html("");
            $("#div_progressinfo", "#" + NewId).html("");
            $("#div_UploadSuccess", "#" + NewId).css({ width: '0%' });
            $("#div_UploadDanger", "#" + NewId).css({ width: '100%' });
            downloadFile(settings.method, settings.url, settings.data, function (xhr) {
                //.net直接输出 attachment; filename*=UTF-8''%e6%b5%8b%e8%af%95.xlsx 测试.xlsx
                //指定编码格式后输出 attachment; filename=%e6%b5%8b%e8%af%95.xlsx   测试.xlsx
                var Descrption = xhr.getResponseHeader('Content-Disposition');
                //返回内容格式 application/json text/plain text/html application/javascript application/octet-stream... 等
                var contentType = xhr.getResponseHeader("Content-Type");
                var retObj;
                if (contentType.toLowerCase().indexOf('json') >= 0) {
                    var blob_res = xhr.response;
                    try {
                        var reader = new FileReader(); //通过 FileReader 读取blob类型
                        reader.onload = function () {
                            var base64Str = this.result.substr(this.result.indexOf('base64,') + 7);
                            retObj = b64DecodeUnicode(base64Str)//base64编码
                        }
                        reader.readAsDataURL(blob_res);
                        retObj = JSON.parse(retObj);
                    }
                    catch (ex) {
                        retObj = { Success: false, ErrMsg: '转换json出错：' +ex};
                    }
                } else {
                    var FileName = '';
                    if (Descrption != null && Descrption != "undefind" && Descrption != "") {
                        var keysp = 'filename=';
                        var _filenameStartindex = Descrption.toLowerCase().lastIndexOf(keysp);
                        if (_filenameStartindex <= 0) {
                            keysp = 'filename*=';
                            var _filenameStartindex = Descrption.toLowerCase().lastIndexOf(keysp);
                        }
                        if (_filenameStartindex >= 0) {
                            var _filename = Descrption.substr(_filenameStartindex + keysp.length);
                            if (keysp == 'filename*=') {
                                var skip = _filename.indexOf("''");
                                if (skip >= 0)
                                    _filename = _filename.substr(skip + 2);
                            }
                            //实际上，escape()不能直接用于URL编码，它的真正作用是返回一个字符的Unicode编码值。
                            //它的具体规则是，除了ASCII字母、数字、标点符号"@ * _ + - . /"以外，对其他所有字符进行编码。
                            //无论网页的原始编码是什么，一旦被Javascript编码，就都变为unicode字符。也就是说，Javascipt函数的输入和输出，默认都是Unicode字符。
                            //其次，escape()不对"+"编码。但是我们知道，网页在提交表单的时候，如果有空格，则会被转化为+字符。服务器处理数据的时候，会把+号处理成空格。所以，使用的时候要小心。
                            //ecodeURI()它着眼于对整个URL进行编码，因此除了常见的符号以外，对其他一些在网址中有特殊含义的符号"; / ? : @ & = + $ , #"，也不进行编码。编码后，它输出符号的utf-8形式，并且在每个字节前加上%。
                            try {
                                if (_filename.indexOf('%u') >= 0)
                                    FileName = unescape(_filename);
                                else
                                    FileName = decodeURI(_filename);
                            } catch (e) {
                                alert('解析文件名失败：' + e);
                                FileName = "未知.data";
                            }
                        }
                    }
                    else
                        FileName = '未知.data';
                    var blob = xhr.response;
                    //debugger;//js 调试状态
                    saveAs(blob, FileName);
                    retObj = { Success: true};
                }
                var closeDelay = settings.closeDelay;
                if (isNaN(closeDelay)) {
                    closeDelay = 2000;
                } else {
                    if (closeDelay < 0) {
                        closeDelay = 0;
                    }
                }
                setTimeout(function () {
                    $("#" + NewId).modal('toggle');
                    if (settings.callbackFuc) {
                        settings.callbackFuc.call(null,retObj);
                    }
                }, closeDelay);
            });
            return false;
        }
        if (settings.auto) {
            StartDownLoad();
        }
        // 解码base64格式的数据
        function b64DecodeUnicode(str) {
            return decodeURIComponent(atob(str).split('').map(function (c) {
                return '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2);
            }).join(''));
        }
        function decodeBase64(str) {
            var base64Chars = [
            'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H',
            'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P',
            'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X',
            'Y', 'Z', 'a', 'b', 'c', 'd', 'e', 'f',
            'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n',
            'o', 'p', 'q', 'r', 's', 't', 'u', 'v',
            'w', 'x', 'y', 'z', '0', '1', '2', '3',
            '4', '5', '6', '7', '8', '9', '+', '/'
            ];
            var i,j,k,len,t = 0,curbinary,start = 0,binary = '',newStr = '';
            var flag = [
                {
                    str: '0',
                    len: 8
                },
                {
                    str: '110',
                    len: 11
                },
                {
                    str: '1110',
                    len: 16
                },
                {
                    str: '11110',
                    len: 21
                }];
                
            for (i = 0, len = str.length ; i < len ; i++) {
                var curbinary = base64Chars.indexOf(str.charAt(i)).toString(2);
                if (curbinary != '-1') {
                    for (j = 0 ; curbinary.length < 6 ; j++) {
                        curbinary = 0 + curbinary;
                    }
                    binary = binary + curbinary;
                }
                if (i >= len - 2 && str.charAt(i) == '=') {
                    ++t;
                }
            }
            if (t == 0) {
                len = binary.length;
            }
            else {
                len = binary.length - (6 - 2 * t)
            }
            for (; start < len ;) {
                for (j = 0 ; j < 4 ; j++) {
                    if (binary.indexOf(flag[j].str, start) == start) {
                        if (flag[j].len == 8) {
                            newStr = newStr + String.fromCharCode(parseInt(binary.substr(start, 8), 2));
                        }
                        else if (flag[j].len == 11) {
                            newStr = newStr +
                                     String.fromCharCode(parsetInt(binary.substr(start + 3, 5) +
                                     binary.substr(start + 10, 6), 2));
                        }
                        else if (flag[j].len == 16) {
                            newStr = newStr +
                                     String.fromCharCode(parsetInt(binary.substr(start + 4, 4) +
                                     binary.substr(start + 10, 6) +
                                     binary.substr(start + 18, 6), 2));
                        }
                        else if (flag[j].len == 21) {
                            newStr = newStr +
                                     String.fromCharCode(parseInt(binary.substr(start + 5, 3) +
                                     binary.substr(start + 10, 6) + binary.substr(start + 18, 6) +
                                     binary.substr(start + 26, 6), 2));
                        }
                        start = start + flag[j].len;
                        break;
                    }
                }
            }
            binary = null;
            return newStr;
        }
    };
    if (typeof jQuery !== 'undefined') {
        jQuery.extend({
            DownLoadProgress: function () {
                return DownLoadProgress(Array.prototype.slice.call(arguments, 0)[0]);
            }
        });
    }
})();