$(function () {

   
    var defaultOptions = {
        errorClass: 'has-error',
        validClass: 'has-success',
        validIcon: 'glyphicon glyphicon-ok form-control-feedback',
        invalidIcon: 'glyphicon glyphicon-remove form-control-feedback',
        highlight: function (element, errorClass, validClass, validIcon, invalidIcon) {
            $(element).closest(".form-group")
            .addClass(errorClass)
            .removeClass(validClass);
            //debugger;
            $(element).next()
            .addClass(invalidIcon)
            .removeClass(validIcon);
        },
        unhighlight: function (element, errorClass, validClass, validIcon, invalidIcon) {
            $(element).closest(".form-group")
            .removeClass(errorClass)
            .addClass(validClass);
            //debugger;
            $(element).next()
            .removeClass(invalidIcon)
            .addClass(validIcon);
        },
        ignore: ":hidden:not(select)"
    };

    $.validator.setDefaults(defaultOptions);

    $.validator.unobtrusive.options = {
        errorClass: defaultOptions.errorClass,
        validClass: defaultOptions.validClass,
    };

})

