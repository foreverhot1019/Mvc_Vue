/**
 * @author zhixin wen <wenzhixin2010@gmail.com>
 * extensions: https://github.com/vitalets/x-editable
 */
//$.ready(function () {
//    console.log($.fn.bootstrapTable);
//});
!function ($) {
    'use strict';
    //$.extend 后面的参数和前面的相同则覆盖
    $.extend($.fn.bootstrapTable.groupdefaults, {
        group: true
        //onGroupInit: function () {
        //    return false;
        //},
        //onGroupSave: function (field, row, oldValue, $el) {
        //    return false;
        //}
    });

    $.extend($.fn.bootstrapTable.Constructor.EVENTS, {
        //'editable-init.bs.table': 'onGroupInit',
        //'editable-save.bs.table': 'onGroupSave'
    });

    var BootstrapTable = $.fn.bootstrapTable.Constructor,
        _initTable = BootstrapTable.prototype.initTable,
        _initBody = BootstrapTable.prototype.initBody;

    BootstrapTable.prototype.initTable = function () {
        var that = this;
        _initTable.apply(this, Array.prototype.slice.apply(arguments));

        if (!this.options.group) {
            return;
        }
        $.each(this.options.columns, function (i, column) {
            if (!column.group) {
                return;
            }

            var _formatter = column.formatter;
            column.formatter = function (value, row, index) {
                var result = _formatter ? _formatter(value, row, index) : value;
                
                var htmlstr = new Array();
                if (Object.prototype.toString.call(result).indexOf("Array") > 0) {
                    if (result.length > 0) {
                        htmlstr.push('<table id="group" data-name="' + column.field + '" class="" style="border:1px #ddd solid; width:100%"> ');
                        for (var i = 0; i < result.length; i++) {
                            htmlstr.push('<tr style="border-top:1px #ddd solid;">');
                            for (var key in result[i]) {
                                var _width ='*';
                                if (key == 'Id' || key == 'VehLoadWeight')
                                    _width = '20%';
                                htmlstr.push('<td style="width:' + _width + ';">');
                                htmlstr.push(result[i][key])
                                htmlstr.push('</td>');
                            }
                            htmlstr.push('</tr>');
                        }
                        htmlstr.push('</table>');
                    }
                }

                return htmlstr.join('');
            };
        });
    };

    //BootstrapTable.prototype.initBody = function () {
    //    var that = this;
    //    _initBody.apply(this, Array.prototype.slice.apply(arguments));

    //    if (!this.options.group) {
    //        return;
    //    }

    //    $.each(this.options.columns, function (i, column) {
    //        if (!column.group) {
    //            return;
    //        }

    //        //that.$body.find('table[data-name="' + column.field + '"]').group(column.group)
    //        //    .off('save').on('save', function (e, params) {
    //        //        var data = that.getData(),
    //        //            index = $(this).parents('tr[data-index]').data('index'),
    //        //            row = data[index],
    //        //            oldValue = row[column.field];

    //        //        row[column.field] = params.submitValue;
    //        //        that.trigger('group-save', column.field, row, oldValue, $(this));
    //        //    });
    //    });
    //    this.trigger('group-init');
    //};

}(jQuery);
