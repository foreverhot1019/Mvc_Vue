/*Vue 全局数据转换 /Date12345685/转datetime 等*/

define(function (require)
{
    var Vue = require('vue');
    //全局 formatter
    var $formatter = {
        dateformatter: function (row, column, value, index)
        {
            if (typeof value === 'undefined')
                return null;
            if (value == null)
                return null;
            if (value == '')//add
                return null;
            else
                return moment(value).format('YYYY-MM-DD');
        },//日期格式化
        datetimeformatter: function (row, column, value, index)
        {
            if (typeof value === 'undefined')
                return null;
            if (value == null)
                return null;
            if (value == '')//add
                return null;
            else
                return moment(value).format('YYYY-MM-DD HH:mm:ss');
        },//时间格式化
        boolformatter: function (row, column, value, index)
        {
            try {
                if (typeof value === 'undefined')
                    return value;
                else if (typeof value === 'boolean') {
                    if (value == true)
                        return '是';
                    if (value == false)
                        return '否';
                }
                else if (value == null)
                    return value;
                else {
                    if (typeof (value) === 'boolean') {
                        if (value == true)
                            return '是';
                        if (value == false)
                            return '否';
                    }
                    else if (isNaN(value)) {
                        if (value.toLowerCase() == 'true')
                            return '是';
                        if (value.toLowerCase() == 'false')
                            return '否';
                    }
                    else {
                        var intval = parseInt(value);
                        if (intval > 0)
                            return '是';
                        if (intval <= 0)
                            return '否';
                    }
                }
            }
            catch (e) {
                return value;
            }
        },//bool格式化
        Usedformatter: function (row, column, value, index)
        {
            //console.log('Usedformatter', value);
            try {
                if (typeof value === 'undefined')
                    return value;
                else if (typeof value === 'boolean') {
                    if (value == true)
                        return '启用';
                    if (value == false)
                        return '停用';
                }
                else if (value == null)
                    return value;
                else {
                    if (isNaN(value)) {
                        if (value == true)
                            return '启用';
                        if (value == false)
                            return '停用';
                    }
                    else {
                        var intval = parseInt(value);
                        if (intval > 0)
                            return '启用';
                        if (intval <= 0)
                            return '停用';
                    }
                }
            }
            catch (e) {
                return value;
            }
        },//启用/停用
        joinformatter: function (row, column, value, index)
        {
            try {
                var name = column.property + 'NAME';
                if (row[name])
                    return row[name];
                else
                    return value;
            }
            catch (e) {
                return value;
            }
        },//Id显示成名称
        Sexformatter: function (row, column, value, index)
        {
            try {
                let val_type = typeof (value);
                if (val_type === 'boolean'){
                    if(value)
                        return '男';
                    else
                        return '女';
                }
                else{
                    let i_val = parseInt(value);
                    if (isNaN(i_val)) {//string
                        let lowerVal = value.toLowerCase();
                        if (lowerVal.indexOf('male') >= 0)
                            return '男';
                        else if (lowerVal.indexOf('female') >= 0)
                            return '女';
                        else
                            return value;
                    } else {
                        if (i_val <= 0) {
                            return value;
                        }
                        else if (i_val == 1)
                            return '男';
                        else
                            return '女';
                    }
                }
            }
            catch (e) {
                return value;
            }
        },//性别显示
        //photoformatter: function (row, column, value, index)
        //{
        //    if (!ObjectIsEmpty(value)) {
        //        var prop = column.property.toLowerCase();
        //        if (prop.indexOf('photo') >= 0) {
        //            //验证guid
        //            var reg = new RegExp(/^[0-9a-z]{8}-[0-9a-z]{4}-[0-9a-z]{4}-[0-9a-z]{4}-[0-9a-z]{12}$/);
        //            let FileGuid = value.toLowerCase();
        //            if (reg.test(FileGuid)) {
        //                return "<img src='/FileUpload/GetImgMinByte?FileGuid=" + value + "' style='width:41px;height:41px' />";
        //            } else
        //                return value;
        //        }
        //    }
        //},//图片显示
    };
    Vue.prototype.$formatter = $formatter;
    //return {
    //    $formmatter: $formmatter
    //}
});