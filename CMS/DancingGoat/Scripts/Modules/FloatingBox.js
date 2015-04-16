// Handles floating of element within a page. Floating style of element must be defined in 'fixed' class.
//

define(['domReady!', 'jQuery'], function (document, $) {

    // Remove floating styles for mobile device
    var checkMobile = function($box) {
        if (window.matchMedia('(max-width: 768px)').matches) {
            $box.removeClass('fixed');
            $box.width("");
            return true;
        }
        return false;
    };

    // Add floating functionality 
    $('.floating-box').each(function (elementPosition, box) {

        var $box = $(box);
        var boxMargin = parseFloat($box.css('marginTop').replace(/auto/, 0));
        var initialTop = $box.offset().top - boxMargin;

        // Update style according scroll position
        $(window).scroll(function () {

            // Skip mobile device
            if (checkMobile($box)) {
                return;
            }

            var scrollTop = $(this).scrollTop();
            var $parent = $($box.parent());

            if (scrollTop >= initialTop) {
                // Add class with fixed position and ensure right width
                $box.addClass('fixed');
                $box.outerWidth($parent.outerWidth());
                
            } else {
                // Remove fixed class
                $box.removeClass('fixed');
                $box.width("");
            }

            var boxHeight = $box.outerHeight();
            var footerTop = $('.footer-container').offset().top;
            var bottomOffset = parseFloat($('html').css('font-size'));

            // Stop floating over footer
            if (footerTop < (scrollTop + boxHeight + bottomOffset)) {
                $box.css("top", (footerTop - boxHeight - scrollTop - bottomOffset) + "px");
            } else {
                $box.css("top","");
            }
        });

        // Ensure right styles when window is resized
        $(window).resize(function () {

            // Ensure width if box is floating
            if ($box.hasClass('fixed')) {
                $box.outerWidth($box.parent().outerWidth());
            }

            if (!checkMobile($box)) {
                // Ensure right position
                if (!$box.hasClass('fixed')) {
                    initialTop = $box.offset().top - boxMargin;
                }
                $(this).scroll();
            }
        });
    });
    
    // Ensure right position after postback with scrolled page
    $(this).scroll();
});