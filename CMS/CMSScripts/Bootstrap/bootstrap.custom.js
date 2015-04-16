$cmsj(function () {
    (function ($) {
        // When clicking the dropdown menu, it will NOT disappear, unless a link was clicked.
        $(".dropdown-menu").click(function (event) {
            if (!$(event.target).closest('a').length) {
                event.stopPropagation();
            }
        });

        // Prevent hiding anchor dropup menu after clicking a link inside of dropup.
        $(".anchor-dropup .dropdown-menu a").click(function (event) {
            event.stopPropagation();
        });

        // Disable hiding anchor dropup menu when clicking outside of it.
        var allowClose = false;
        $('.anchor-dropup').on({
            "shown.bs.dropdown": function () { allowClose = false; },
            "click": function () { allowClose = true; },
            "hide.bs.dropdown": function () { if (!allowClose) return false; }
        });

        // This prevents IE9 bug when caching styles causes bad resolving media queries when opening iframe with lesser width.
        $('iframe').load(function () {
            if ($('body').hasClass('IE9')) {
                var iframeContent = $(this).contents();
                iframeContent.find('link[rel="stylesheet"]').each(function () {

                    // Add a 'nocache' random num query string to stylesheet's url for disabling the caching.
                    var cssURL = $(this).attr('href');
                    cssURL += (cssURL.indexOf('?') != -1) ? '&' : '?';
                    cssURL += 'nocache=' + (Math.random());
                    $(this).attr('href', cssURL);
                });
            }
        });

        // On/off switcher
        $('.has-switch').click(function () {
            $(this).find('.switch').toggle();
            var $switch = $(this).find('input[type=checkbox]');
            if ($switch.prop('checked')) {
                $switch.prop('checked', false);
            } else {
                $switch.prop('checked', true);
            }
        });

        // Fix position of localization flag icon for scrolling textareas   
        $('.cms-input-group').each(function () {
            $(this).on('checkScrollbar', function () {
                var $textarea = $(this).find('textarea');
                if ($textarea.length > 0) {
                    if ($textarea[0].clientHeight < $textarea[0].scrollHeight) {
                        $(this).addClass("has-scroller");
                    } else {
                        $(this).removeClass("has-scroller");
                    }
                }
            });
        });
        $('textarea').bind('keyup mouseup mouseout', function () {
            $(this).parent('.input-localized').trigger('checkScrollbar');
        });
        $('.input-localized').each(function () {
            $(this).trigger('checkScrollbar');
        });
    }((function ($) {

        var scopedjQuery = function (selector) {
            return $(selector, '.cms-bootstrap, .cms-bootstrap-js');
        };

        // Copy all jQuery properties into
        // scopedjQuery so they can be used later
        for (var k in $) {
            if ($.hasOwnProperty(k)) {
                scopedjQuery[k] = $[k];
            }
        }

        return scopedjQuery;

    }($cmsj))));
});