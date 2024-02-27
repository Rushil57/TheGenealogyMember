$(function () {
    jQuery.validator.setDefaults({
        errorElement: 'span',
        errorClass: 'help-block help-block-error',
        focusInvalid: true,
        highlight: function (element) {
            $(element).closest('.form-group').addClass('has-error');
        },
        unhighlight: function (element) {
            $(element).closest('.form-group').removeClass('has-error');
        },
        success: function (label) {
            label.closest('.form-group').removeClass('has-error');
        }
    });     

})

