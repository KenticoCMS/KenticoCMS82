cmsdefine(['jQuery'], function ($) {
    'use strict';

    var Module = function () {

        function displayRedirectionMessage() {
            if ($('#CouponCheckBox input[type=checkbox]').is(':checked')) {
                $('#CouponsInfoLabel').show();
            }
            else {
                $('#CouponsInfoLabel').hide();
            }
        }

        var init = function () {
            $('#CouponCheckBox input[type=checkbox]').change(function () {
                if ($('.discountUsesCouponsValue input[type=hidden]').val() === 'false') {
                    displayRedirectionMessage();
                }
            });
        };

        init();
    };

    return Module;
});
