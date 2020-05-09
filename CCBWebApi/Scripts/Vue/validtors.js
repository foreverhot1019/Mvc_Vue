/*Vue 全局验证方法 Form-rules 验证规则*/
define(function (require)
{
    var Vue = require('vue');
    //全局 验证规则
    var $Validtors = {
        PasswordValidator: function (rule, value, callback)
        {
            if (ObjectIsEmpty(value))
                callback();//验证通过也必须调用callback
            else {
                if (value.length < 6 || value.length > 20)
                    return callback(new Error('密码必须介于6-20位之间'));
                if (!/[A-Z]/.test(value) || !/[a-z]/.test(value)) {
                    return callback(new Error('密码至少含有一个大写&小写字母'));
                }
                //var AllCHN_reg = /^[\u4E00-\u9FA5]+$/;//全为中文
                var HasCHN_reg = /[\u4e00-\u9fa5]+/;//含有中文
                if (HasCHN_reg.test(value)) {
                    return callback(new Error('密码不能含有中文'));
                }
                callback();//验证通过也必须调用callback
            }
        },
        IdCardValidator: function (rule, value, callback)
        {
            if (ObjectIsEmpty(value))
                callback();//验证通过也必须调用callback
            else {
                //身份证正则
                let reg = /(^\d{15}$)|(^\d{18}$)|(^\d{17}(\d|X|x)$)/;
                if (value === '') {
                    callback(new Error('身份证号不能为空'));
                } else if (!reg.test(value)) {
                    callback(new Error('请输入正确的身份证号'));
                }else
                    callback();//验证通过也必须调用callback
            }
        },
        IdCardHKValidator: function (rule, value, callback)
        {
            if (ObjectIsEmpty(value))
                callback();//验证通过也必须调用callback
            else {
                var strValidChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ"
                // basic check length
                if (str.length < 8)
                    callback(new Error('请输入正确的香港身份证号'));
                // handling bracket
                if (str.charAt(str.length - 3) == '(' && str.charAt(str.length - 1) == ')')
                    str = str.substring(0, str.length - 3) + str.charAt(str.length - 2);
                // convert to upper case
                str = str.toUpperCase();
                // regular expression to check pattern and split
                var hkidPat = /^([A-Z]{1,2})([0-9]{6})([A0-9])$/;
                var matchArray = str.match(hkidPat);
                // not match, return false
                if (matchArray == null)
                    callback(new Error('请输入正确的香港身份证号'));
                callback();//验证通过也必须调用callback
            }
        },
    };

    Vue.prototype.$Validtors = $Validtors;
    //return {
    //    $Validtors : $Validtors
    //}
});