define(function (require)
{
    const Vue_dateformatter = function (value)
    {
        try {
            var m_date = new moment(value);
            if (m_date.isValid())
                return m_date.format('YYYY-MM-DD')
            else
                return value;
        } catch (e) {
            return value;
        }
    }, Vue_datetimeformatter = function (value)
    {
        try {
            var m_date = new moment(value);
            if (m_date.isValid())
                return m_date.format('YYYY-MM-DD HH:mm:ss')
            else
                return value;
        } catch (e) {
            return value;
        }
    }, Vue_Sexformatter = function (value)
    {
        try {
            let val_type = typeof (value);
            if (val_type === 'boolean') {
                if (value)
                    return '男';
                else
                    return '女';
            }
            else {
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
    };//性别显示
    const commonFiltes = { Vue_dateformatter: Vue_dateformatter, Vue_datetimeformatter: Vue_datetimeformatter, Vue_Sexformatter: Vue_Sexformatter };
    var Vue = require('vue');
    //显示格式化 注册到vue全局filter v-bind有效，v-Model 无效
    Object.keys(commonFiltes).forEach(function (key, index, arr)
    {
        Vue.filter(key, commonFiltes[key]);
    });
    //export default {dateformatter:dateformatter,datetimeformatter:datetimeformatter}
    //return commonFiltes;
});