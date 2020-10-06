// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

jQuery.fn.extend({
    autoHeight: function () {
        function autoHeight_(element) {
            return jQuery(element).css({
                'height': 'auto',
                'overflow-y': 'hidden'
            }).height(element.scrollHeight);
        }
        return this.each(function () {
            autoHeight_(this).on('input', function () {
                autoHeight_(this);
            });
        });
    }
});
$('#exampleFormControlTextarea11').autoHeight();