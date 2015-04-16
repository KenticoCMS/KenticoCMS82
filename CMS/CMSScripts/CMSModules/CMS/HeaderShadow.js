cmsdefine(['jQuery', 'Underscore', 'CMS/EventHub'], function ($, _, eventHub) {

    var $uiHeader,
        $headerPanel,
        headerPanelFound,
        headerShadowClass = "header-shadow",

        doc = $(document),

        // Run the header shadow job
        toggleShadow = _.throttle(function () {

            // Check if scroll area exists
            if ($(this).length > 0) {

                ensureUiHeader();

                if ($uiHeader.length > 0) {
                    // Set or remove shadow according to scrolled element position
                    if ($(this).scrollTop() > 0) {
                        toggleShadowInternal(this, true);
                    }
                    else {
                        toggleShadowInternal(this, false);
                    }
                }
            }
        }, 1000 / 60), // 30 FPS is good enough

        // Prepare all the shadow holder panels the shadow should be applied to
        ensureUiHeader = function () {
            if ($uiHeader == null || $uiHeader.length <= 0) {
                if ($('div.shadow-holder').height() > 0) {
                    $uiHeader.push($('div.shadow-holder'));
                }
                if ($('div.PreviewMenu').height() > 0) {
                    $uiHeader.push($('div.PreviewMenu'));
                }
                if ($('div#CMSHeaderDiv').height() > 0) {
                    $uiHeader.push($('div#CMSHeaderDiv'));
                }
                if ($('div.header-container').height() > 0) {
                    $uiHeader.push($('div.header-container'));
                }
                if ($('div#CKToolbar').height() > 0) {
                    $uiHeader.push($('div#CKToolbar'));
                }
                if ($('div.preview-edit-panel').height() > 0) {
                    $uiHeader.push($('div.preview-edit-panel'));
                }
                if ($('div.cms-edit-menu').height() > 0) {
                    $uiHeader.push($('div.cms-edit-menu'));
                }
            }
        },

        // Set or remove shadow
        toggleShadowInternal = function (scrollElem, scrolled) {

            headerPanelFound = false;

            _.each($uiHeader, function ($header) {
                // Only one shadow header panel should be selected per scroll
                if (headerPanelFound === false) {
                    // Find previous siblings. E.g. data driven UI
                    if (($headerPanel = $(scrollElem).prevAll($header.selector).first()).length > 0) { // .first() is used in order to match only one wrapper
                        scrolled ? $headerPanel.addClass(headerShadowClass) : $headerPanel.removeClass(headerShadowClass);
                        headerPanelFound = true; // We know now the closest shadow holder panel
                    }
                        // Find all previous siblings and then traverse deep down. E.g. Media Library - bottom
                    else if (($headerPanel = $(scrollElem).prevAll('div').find($header.selector).first()).length > 0) {
                        scrolled ? $headerPanel.addClass(headerShadowClass) : $headerPanel.removeClass(headerShadowClass);
                        headerPanelFound = true;
                    }
                        // Go up and find all previous siblings. Traverse deep down then. E.g. Media Library - top
                    else if (($headerPanel = $(scrollElem).parents('div').prevAll('div').find($header.selector).first()).length > 0) {
                        scrolled ? $headerPanel.addClass(headerShadowClass) : $headerPanel.removeClass(headerShadowClass);
                        headerPanelFound = true;
                    }
                        // You are on the root of document so traverse down directly. E.g. Page, Design or Form tab
                    else if (($headerPanel = $(scrollElem).find($header).first()).length > 0) { // .selector is missing because this works with an iframe
                        scrolled ? $headerPanel.addClass(headerShadowClass) : $headerPanel.removeClass(headerShadowClass);
                        headerPanelFound = true;
                    }
                        // Use the shadow holder directly as a fallback. E.g. Preview mode
                    else {
                        $headerPanel = $header.first();
                        scrolled ? $headerPanel.addClass(headerShadowClass) : $headerPanel.removeClass(headerShadowClass);
                        headerPanelFound = true;
                    }
                }
            });
        },

        // Shadow initialization on selected elements
        initializeShadow = function () {
            doc.on('scroll', toggleShadow);
            $('.scroll-area,.PageContent,.PreviewBody,.DeviceFrame,.dialog-content').on('scroll', toggleShadow);

            // Iframe scroll area - e.g. in Newsletter preview dialog
            var $iframeScrollArea = $('iframe.scroll-area');
            $iframeScrollArea.ready(function () {
                $iframeScrollArea.contents().on('scroll', toggleShadow);
            });
            // e.g. for Newsletter preview Prev and Next buttons
            $iframeScrollArea.load(function () {
                $iframeScrollArea.contents().on('scroll', toggleShadow);
                toggleShadow.call($iframeScrollArea.contents());
            });

            $uiHeader = [];

            // Async postback
            if (typeof (Sys) != 'undefined') {
                var prm = Sys.WebForms.PageRequestManager.getInstance();

                function EndRequestHandler(sender, args) {
                    // Call the function directly with particular context - e.g. in Media libraries, Form controls
                    toggleShadow.call($('.DialogViewContent.scroll-area, .UIContent.scroll-area'));

                    // Register the scroll event again
                    doc.on('scroll', toggleShadow);
                    $('.scroll-area,.PageContent,.PreviewBody,.DeviceFrame,.dialog-content').on('scroll', toggleShadow);
                }

                prm.add_endRequest(EndRequestHandler);
            }
        };

    doc.ready(initializeShadow);
    eventHub.subscribe('cms.angularViewLoaded', initializeShadow);

    return function () { }
});