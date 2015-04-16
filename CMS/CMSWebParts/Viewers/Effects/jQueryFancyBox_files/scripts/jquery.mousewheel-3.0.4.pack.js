/*! Copyright (c) 2010 Brandon Aaron (http://brandonaaron.net)
* Licensed under the MIT License (LICENSE.txt).
*
* Thanks to: http://adomas.org/javascript-mouse-wheel/ for some pointers.
* Thanks to: Mathias Bank(http://www.mathias-bank.de) for a scope bug fix.
* Thanks to: Seamus Leahy for adding deltaX and deltaY
*
* Version: 3.0.4
*
* Requires: 1.2.2+
*/
(function(e){function t(t){var n=t||window.event,r=[].slice.call(arguments,1),i=0,s=0,o=0;t=e.event.fix(n);t.type="mousewheel";if(t.wheelDelta)i=t.wheelDelta/120;if(t.detail)i=-t.detail/3;o=i;if(n.axis!==undefined&&n.axis===n.HORIZONTAL_AXIS){o=0;s=-1*i}if(n.wheelDeltaY!==undefined)o=n.wheelDeltaY/120;if(n.wheelDeltaX!==undefined)s=-1*n.wheelDeltaX/120;r.unshift(t,i,s,o);return e.event.handle.apply(this,r)}var n=["DOMMouseScroll","mousewheel"];e.event.special.mousewheel={setup:function(){if(this.addEventListener)for(var e=n.length;e;)this.addEventListener(n[--e],t,false);else this.onmousewheel=t},teardown:function(){if(this.removeEventListener)for(var e=n.length;e;)this.removeEventListener(n[--e],t,false);else this.onmousewheel=null}};e.fn.extend({mousewheel:function(e){return e?this.bind("mousewheel",e):this.trigger("mousewheel")},unmousewheel:function(e){return this.unbind("mousewheel",e)}})})(jQuery);