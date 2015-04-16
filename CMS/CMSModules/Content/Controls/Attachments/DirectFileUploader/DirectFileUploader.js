DFU = {
    init: function (uploader, containerId) {
        if (uploader != null) {
            var db = uploader.ownerDocument.body, de = uploader.ownerDocument.documentElement, height = Math.max(db.scrollWidth, de.scrollWidth, db.scrollHeight, de.scrollHeight);
            uploader.style.position = 'absolute';
            uploader.style.cursor = 'pointer';
            uploader.style.outline = 'none';
            uploader.style.padding = uploader.style.margin = uploader.style.top = uploader.style.right = '0';
            uploader.style.fontSize = (height > 0 ? height : 500) + 'px';
            // IE opacity filter
            if (typeof uploader.style.opacity != 'string' && typeof (uploader.filters) != 'undefined') {
                uploader.style.filter = 'alpha(opacity=0)';
            } else {
                uploader.style.opacity = '0';
            }
            if (containerId) {
                DFU.toogleLoading(containerId, false);
            }
        }
    },
    initializeDesign: function (containerId) {
        var container = $cmsj('#' + containerId), innerDiv = container.find('div.InnerDiv'), uploaderDiv = container.find('div.UploaderDiv'), divLoaded = true;

        innerDiv.find('img').each(function () {
            if (!DFU.isImageLoaded(this)) {
                divLoaded = false;
            }
        });
        if (divLoaded) {
            var uplWidth = innerDiv.actual('outerWidth', {
                clone: true
            }), uplHeight = innerDiv.actual('outerHeight', {
                clone: true
            });
            if ((uplWidth > 0) && (uplHeight > 0)) {
                uploaderDiv.width(uplWidth).height(uplHeight);
                container.find('iframe').width(uplWidth).height(uplHeight).trigger('resize');
                container.find('object').width(uplWidth).height(uplHeight);
                container.width(uplWidth).height(uplHeight).mouseenter(function () {
                    $cmsj(this).addClass('MouseOver');
                }).mouseleave(function () {
                    $cmsj(this).removeClass('MouseOver');
                });
                // Initialize loading div after postback
                DFU.toogleLoading(containerId, false);

                // Ensure display for inner div (IE bug after async postback)
                innerDiv.css('display', 'block');
            }
        } else {
            setTimeout(function () {
                DFU.initializeDesign(containerId);
            }, 100);
        }
    },
    toogleLoading: function (containerId, show) {
        var container = $cmsj('#' + containerId), loadingDiv = container.find('.LoadingDiv'), innerLoadingDiv = container.find('.innerLoadingDiv'), innerDiv = container.find('.InnerDiv'), uploaderDiv = container.find('.UploaderDiv');
        if (show) {
            // Hide inner loading div if not fits into container
            if (Math.abs(innerLoadingDiv.width() - container.width()) < 30) {
                innerLoadingDiv.hide();
            }
            loadingDiv.show();
            // Move uploader up because IE bug in hidden silverlight
            uploaderDiv.css({ 'top': -uploaderDiv.outerHeight(), 'z-index': -100 });
            innerDiv.hide();
        } else {
            loadingDiv.hide();
            uploaderDiv.css({ 'top': 0, 'z-index': '' });
            innerDiv.show();
        }
    },
    isImageLoaded: function (img) {
        if (!img.complete) {
            return false;
        }
        if (typeof img.naturalWidth != 'undefined' && img.naturalWidth == 0) {
            return false;
        }
        return true;
    },
    OnUploadBegin: function (containerId) {
        DFU.maxProgresReached = false;
        DFU.toogleLoading(containerId, true);
    },
    OnUploadCompleted: function (containerId) {
        DFU.toogleLoading(containerId, false);
    },
    maxProgresReached: false,
    OnUploadProgressChanged: function (containerId, totalUploadSize, totalUploaded, timeLeft) {
        if (totalUploadSize === 0) {
            totalUploadSize = 1;
        }
        var container = $cmsj('#' + containerId), progressSpan = container.find('*[id$=Progress]'), cancelSpan = container.find('.UploadCancel'), percent = parseInt(totalUploaded / totalUploadSize * 100, 10);
        if (container.width() > 50) {

            // Ensure cancel icon
            if (cancelSpan.length === 0) {

                // Create cancel immage
                cancelSpan = $cmsj('<span></span>');
                cancelSpan.html('<i class="icon-bin cms-icon-80" aria-hidden="true"></i>');
                cancelSpan.addClass('UploadCancel');
                progressSpan.after(cancelSpan);
                cancelSpan.click(function () {
                    cmsrequire(['CMS/EventHub'], function (EventHub) {
                        EventHub.publish('UploadCanceled', containerId);
                    });
                });
            }
            if ((percent > 100) || (DFU.maxProgresReached)) {
                DFU.maxProgresReached = true;
                percent = 100;
            }
            progressSpan.html(percent + '%');
        }
        else {
            // Ensure cancel icon
            if (cancelSpan.length === 0) {

                // Create cancel immage
                cancelSpan = $cmsj('<span></span>');
                cancelSpan.html('&nbsp;');
                cancelSpan.addClass('UploadCancel');
                cancelSpan.css({ 'display': 'block', 'background': '#fff', 'position': 'absolute', 'top': 0, 'left': 0, 'width': container.width(), 'height': container.height() });
                cancelSpan.hide();
                progressSpan.after(cancelSpan);
                // IE bug to detect click on empty span
                cancelSpan.fadeTo(100, 0.01);
                cancelSpan.click(function () {
                    cmsrequire(['CMS/EventHub'], function (EventHub) {
                        EventHub.publish('UploadCanceled', containerId);
                    });
                });
            }
        }
    }
};

if (typeof ($cmsj) !== 'undefined') {
    (function ($) {
        $.fn.extend({
            actual: function (method, options) {
                var $hidden, $target, configs, css, tmp, actual, fix, restore;
                if (!this[method])
                    throw '$.actual => The jQuery method "' + method + '" you called does not exist';
                configs = $.extend({
                    absolute: false,
                    clone: false
                }, options);
                $target = this;
                fix = configs.clone === true ? function () {
                    $target = $target.filter(':first').clone().css({
                        position: 'absolute',
                        top: -1000
                    }).appendTo('body');
                } : function () {
                    $hidden = $target.parents().andSelf().filter(':hidden');
                    css = configs.absolute === true ? { position: 'absolute', visibility: 'hidden', display: 'block' } : { visibility: 'hidden', display: 'block' };
                    tmp = [];
                    $hidden.each(function () {
                        var _tmp = {}, name;
                        for (name in css) {
                            _tmp[name] = this.style[name];
                            this.style[name] = css[name];
                        }
                        tmp.push(_tmp);
                    });
                };
                restore = configs.clone === true ? function () {
                    $target.remove();
                } : function () {
                    $hidden.each(function (i) {
                        var _tmp = tmp[i], name;
                        for (name in css) {
                            this.style[name] = _tmp[name];
                        }
                    });
                };
                fix();
                actual = $target[method]();
                restore();
                return actual;
            }
        });
    }($cmsj));
}