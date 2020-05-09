/* FileSaver.js
 * A saveAs() FileSaver implementation.
 * 1.3.0
 *
 * By Eli Grey, http://eligrey.com
 * License: MIT
 *   See https://github.com/eligrey/FileSaver.js/blob/master/LICENSE.md
 */

/*global self */
/*jslint bitwise: true, indent: 4, laxbreak: true, laxcomma: true, smarttabs: true, plusplus: true */

/*! @source http://purl.eligrey.com/github/FileSaver.js/blob/master/FileSaver.js */

var saveAs = saveAs || (function (view) {
    "use strict";
    // IE <10 is explicitly unsupported
    if (typeof view === "undefined" || typeof navigator !== "undefined" && /MSIE [1-9]\./.test(navigator.userAgent)) {
        return;
    }
    var
		  doc = view.document
		  // only get URL when necessary in case Blob.js hasn't overridden it yet
		, get_URL = function () {
		    return view.URL || view.webkitURL || view;
		}
		, save_link = doc.createElementNS("http://www.w3.org/1999/xhtml", "a")
		, can_use_save_link = "download" in save_link
		, click = function (node) {
		    var event = new MouseEvent("click");
		    node.dispatchEvent(event);
		}
		, is_safari = /constructor/i.test(view.HTMLElement)
		, throw_outside = function (ex) {
		    (view.setImmediate || view.setTimeout)(function () {
		        throw ex;
		    }, 0);
		}
		, force_saveable_type = "application/octet-stream"
		// the Blob API is fundamentally broken as there is no "downloadfinished" event to subscribe to
		, arbitrary_revoke_timeout = 1000 * 40 // in ms
		, revoke = function (file) {
		    var revoker = function () {
		        if (typeof file === "string") { // file is an object URL
		            get_URL().revokeObjectURL(file);
		        } else { // file is a File
		            file.remove();
		        }
		    };
		    setTimeout(revoker, arbitrary_revoke_timeout);
		}
		, dispatch = function (filesaver, event_types, event) {
		    event_types = [].concat(event_types);
		    var i = event_types.length;
		    while (i--) {
		        var listener = filesaver["on" + event_types[i]];
		        if (typeof listener === "function") {
		            try {
		                listener.call(filesaver, event || filesaver);
		            } catch (ex) {
		                throw_outside(ex);
		            }
		        }
		    }
		}
		, auto_bom = function (blob) {
		    // prepend BOM for UTF-8 XML and text/* types (including HTML)
		    // note: your browser will automatically convert UTF-16 U+FEFF to EF BB BF
		    if (/^\s*(?:text\/\S*|application\/xml|\S*\/\S*\+xml)\s*;.*charset\s*=\s*utf-8/i.test(blob.type)) {
		        return new Blob([String.fromCharCode(0xFEFF), blob], { type: blob.type });
		    }
		    return blob;
		}
		, FileSaver = function (blob, name, no_auto_bom) {
		    if (!no_auto_bom) {
		        blob = auto_bom(blob);
		    }
		    // First try a.download, then web filesystem, then object URLs
		    var
				  filesaver = this
				, type = blob.type
				, force = type === force_saveable_type
				, object_url
				, dispatch_all = function () {
				    dispatch(filesaver, "writestart progress write writeend".split(" "));
				}
				// on any filesys errors revert to saving with object URLs
				, fs_error = function () {
				    if (force && is_safari && view.FileReader) {
				        // Safari doesn't allow downloading of blob urls
				        var reader = new FileReader();
				        reader.onloadend = function () {
				            var base64Data = reader.result;
				            view.location.href = "data:attachment/file" + base64Data.slice(base64Data.search(/[,;]/));
				            filesaver.readyState = filesaver.DONE;
				            dispatch_all();
				        };
				        reader.readAsDataURL(blob);
				        filesaver.readyState = filesaver.INIT;
				        return;
				    }
				    // don't create more object URLs than needed
				    if (!object_url) {
				        object_url = get_URL().createObjectURL(blob);
				    }
				    if (force) {
				        view.location.href = object_url;
				    } else {
				        var opened = view.open(object_url, "_blank");
				        if (!opened) {
				            // Apple does not allow window.open, see https://developer.apple.com/library/safari/documentation/Tools/Conceptual/SafariExtensionGuide/WorkingwithWindowsandTabs/WorkingwithWindowsandTabs.html
				            view.location.href = object_url;
				        }
				    }
				    filesaver.readyState = filesaver.DONE;
				    dispatch_all();
				    revoke(object_url);
				}
		    ;
		    filesaver.readyState = filesaver.INIT;

		    if (can_use_save_link) {
		        object_url = get_URL().createObjectURL(blob);
		        setTimeout(function () {
		            save_link.href = object_url;
		            save_link.download = name;
		            click(save_link);
		            dispatch_all();
		            revoke(object_url);
		            filesaver.readyState = filesaver.DONE;
		        });
		        return;
		    }

		    fs_error();
		}
		, FS_proto = FileSaver.prototype
		, saveAs = function (blob, name, no_auto_bom) {
		    return new FileSaver(blob, name || blob.name || "download", no_auto_bom);
		}
    ;
    // IE 10+ (native saveAs)
    if (typeof navigator !== "undefined" && navigator.msSaveOrOpenBlob) {
        return function (blob, name, no_auto_bom) {
            name = name || blob.name || "download";

            if (!no_auto_bom) {
                blob = auto_bom(blob);
            }
            return navigator.msSaveOrOpenBlob(blob, name);
        };
    }

    FS_proto.abort = function () { };
    FS_proto.readyState = FS_proto.INIT = 0;
    FS_proto.WRITING = 1;
    FS_proto.DONE = 2;

    FS_proto.error =
	FS_proto.onwritestart =
	FS_proto.onprogress =
	FS_proto.onwrite =
	FS_proto.onabort =
	FS_proto.onerror =
	FS_proto.onwriteend =
		null;

    return saveAs;
}(
	   typeof self !== "undefined" && self
	|| typeof window !== "undefined" && window
	|| this.content
));
// `self` is undefined in Firefox for Android content script context
// while `this` is nsIContentFrameMessageManager
// with an attribute `content` that corresponds to the window

if (typeof module !== "undefined" && module.exports) {
    module.exports.saveAs = saveAs;
} else if ((typeof define !== "undefined" && define !== null) && (define.amd !== null)) {
    define([], function () {
        return saveAs;
    });
}





jQuery.extend({
    postDownload: function (url, formData, onCompleted) {
        var xhr = new XMLHttpRequest();
        xhr.open('POST', url, true);
        xhr.responseType = 'blob';
        xhr.overrideMimeType("application/vnd.ms-excel");
        xhr.onreadystatechange = function () {
            if (xhr.readyState == 4 && xhr.status == 200) {
                //console.log(xhr.getResponseHeader('Content-Disposition'));
                var header = xhr.getResponseHeader('Content-Disposition');
                var fileName = '';
                if (header) {
                    var regx = /filename[^;=\n]*=((['\"]).*?\2|[^;\n]*)/g;
                    fileName = regx.exec(header)[1];
                    //console.log(regx.exec(header));
                }
                var blob = xhr.response;
                saveAs(blob, fileName);
                onCompleted(fileName);
            }
        };
        xhr.onload = function (e) {
            //console.log(e);
        };
        //var formData = new FormData();
        //formData.append('filterRules', filterRules);
        //formData.append('sort', 'Id');
        //formData.append('order', 'asc');
        xhr.send(formData);
    }
});
jQuery.extend({
    postJSONDownload: function (url, formData, onCompleted) {
        var xhr = new XMLHttpRequest();
        xhr.open('POST', url, true);
        xhr.responseType = 'blob';
        content_type = 'application/json';
        if (arguments.length > 3) {
            content_type = arguments[3];
        }
        if (typeof (content_type) === "undefined")
            content_type = 'application/json';
        //console.log(content_type, formData,arguments);
        //xhr.timeout = 0;
        //设置发送数据的请求格式
        xhr.setRequestHeader('content-type', content_type);//application/x-www-form-urlencoded
        xhr.overrideMimeType(content_type);
        xhr.onreadystatechange = function () {
            if (xhr.readyState == 4 && xhr.status == 200) {
                //console.log(xhr.getResponseHeader('Content-Disposition'));
                var header = xhr.getResponseHeader('Content-Disposition');
                var fileName = '';
                if (header) {
                    var regx = /filename[^;=\n]*=((['\"]).*?\2|[^;\n]*)/g;
                    fileName = regx.exec(header)[1];
                    //console.log(regx.exec(header));
                }
                var blob = xhr.response;
                saveAs(blob, fileName);
                onCompleted(fileName);
            }
        };
        xhr.onload = function (e) {
            //console.log(e);
        };
        xhr.onerror = function (e) {
            console.log('postJSONDownload',e);
        };
        //var formData = new FormData();
        //formData.append('filterRules', filterRules);
        //formData.append('sort', 'Id');
        //formData.append('order', 'asc');
        try {
            if (content_type.toLowerCase().indexOf('json') >= 0) {
                var Jsondata = JSON.stringify(formData);
                //console.log('postJSONDownload', Jsondata);
                xhr.send(Jsondata);
            } else {
                if (typeof (formData) === 'object') {
                    var sendData = '';
                    for (var item in formData) {
                        if (sendData == '') {
                            sendData = item + "=" + formData[item];
                        } else {
                            sendData = "&"+item + "=" + formData[item];
                        }
                    }
                    xhr.send(sendData);
                } else {
                    xhr.send(formData);
                }
            }
        } catch (ex) {
            console.log('postJSONDownload',ex);
        }
    }
});

//+function ($) {

//    // BUTTON PUBLIC CLASS DEFINITION
//    // ==============================

//    var postDownloadFile = function (element, options) {
//        this.$element = $(element)
//        this.options = $.extend({}, Button.DEFAULTS, options)
//        this.isLoading = false
//    }

//    postDownloadFile.VERSION = '1.0.0'

//    Button.DEFAULTS = {
//        loadingText: 'loading...'
//    }
//    postDownloadFile.prototype.Start

//    postDownloadFile.prototype.Start = function (state) {
//    }
//}(jQuery);


//downloadFile(FileUrl, function (xhr) {
//    var Descrption = xhr.getResponseHeader('Content-Disposition');
//    var FileName = '';
//    if (Descrption != null && Descrption != "undefind" && Descrption != "") {
//        var _filenameStartindex = Descrption.lastIndexOf('=');
//        if (_filenameStartindex >= 0)
//            FileName = Descrption.substr(_filenameStartindex + 1);
//    }
//    else
//        FileName = '未知.data';
//    var blob = xhr.response;
//    //debugger;//js 调试状态
//    saveAs(blob, FileName);
//    setTimeout(function () {
//        $("#DownLoadDrdDtlProgrsformModal").modal('toggle');
//    }, 3000);
//});

//function downloadFile(url, success) {
//    var xhr = new XMLHttpRequest();
//    xhr.open('GET', url, true);
//    xhr.responseType = "blob";
//    xhr.onreadystatechange = function () {
//        if (xhr.readyState == 4) {
//            $("#div_time").html("下载耗时：" + (+new Date - startTime) / 1000 + "s");
//            $("#div_time").show();
//            if (success)
//                success(xhr);
//        }
//    };

//    ////下载进度条界面 取消时的操作 停止下载
//    //$("button[id=DownLoadDrdDtlProgrsClose],button[id=DownLoadDrdDtlProgrsCancel],button[id=DownLoadDrdDtlProgrsOK]","#DownLoadDrdDtlProgrsformModal").on("click", function () {
//    //    try
//    //    {
//    //        if (xhr.readyState == 4) {
//    //            $("#DownLoadDrdDtlProgrsformModal").modal('toggle');
//    //        }
//    //    }
//    //    catch(e)
//    //    {
//    //        $("#DownLoadDrdDtlProgrsformModal").modal('toggle');
//    //    }
//    //});

//    var lastLoaded = 0, speed = 0, lastTime = +new Date, startTime = lastTime;
//    var speedText = $("#div_speed");
//    var loadedInfo = $("#div_progressinfo");
//    ////文件读取开始时触发。
//    //xhr.addEventListener("Onloadstart",function(e){
//    //},false);
//    //进行中时定时触发
//    xhr.addEventListener("progress", function (e) {
//        var currTime = +new Date;
//        var currLoaded = e.loaded;
//        var dT = currTime - lastTime;
//        var dL = currLoaded - lastLoaded;

//        lastTime = currTime;
//        lastLoaded = currLoaded;

//        speed = parseInt(dL / dT);
//        speedText.html("下载速度 " + speed + " kb/s");

//        var percent = (currLoaded / e.total);
//        loadedInfo.html("文件大小： " + (e.total / 1024 / 1024).toFixed(2) + "M，已下载：" + (currLoaded / 1024 / 1024).toFixed(2) + "M  <br />进度：" + (percent * 100).toFixed(2) + "%");

//        $("#div_UploadSuccess", '#div_progressbar').css({ width: (percent * 100).toFixed(2).toString() + '%' });
//        $("#div_UploadDanger", '#div_progressbar').css({ width: (100 - (percent * 100)).toFixed(2).toString() + '%' });
//    });
//    //被中止时触发
//    xhr.addEventListener("abort", function (e) {
//        alert('下载被终止');
//        $("#div_time").html("");
//        $("#div_time").hide();
//        $("#div_speed").html("");
//        $("#div_progressinfo").html("");
//        $("#div_UploadSuccess", '#div_progressbar').css({ width: '0%' });
//        $("#div_UploadDanger", '#div_progressbar').css({ width: '100%' });
//        $("#DownLoadDrdDtlProgrsformModal").modal('toggle');
//    }, false);
//    //出错时触发
//    xhr.addEventListener("error", function (e) {
//        alert('下载时发生错误');
//        $("#div_time").html("");
//        $("#div_time").hide();
//        $("#div_speed").html("");
//        $("#div_progressinfo").html("");
//        $("#div_UploadSuccess", '#div_progressbar').css({ width: '0%' });
//        $("#div_UploadDanger", '#div_progressbar').css({ width: '100%' });
//        $("#DownLoadDrdDtlProgrsformModal").modal('toggle');
//    }, false);
//    //成功完成时触发
//    xhr.addEventListener("load", function (e) {
//    }, false);
//    //完成时，无论成功或者失败都会触发
//    xhr.addEventListener("loadend", function (e) {
//    }, false);
//    xhr.send(null);
//}
