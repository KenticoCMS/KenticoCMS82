var firstLayoutX = 0;
var firstLayoutY = 0;

var initialWidth = 0;
var initialHeight = 0;

var currentWidth = 0;
var currentHeight = 0;

var currentLayoutElem = null;
var layoutOverlayElem = null;

var layoutInfoElem = null;
var layoutInfoText = null;

var lastLayoutElem = null;
var lastResizedElem = null;

var textBoxEdit = null;
var textAreaEdit = null;
var colorEdit = null;

var propOverlay = null;
var activeEditor = null;

function IsWebKit() {
    var s = navigator.userAgent.toLowerCase() + '';
    if (s.indexOf('applewebkit/') >= 0) {
        return true;
    }
    return false;
}

function Get(elemId) {
    return document.getElementById(elemId);
}

function UpdateLayoutInfo(el) {
    if (el != null) {
        var je = $cmsj(el);
        var pos = je.offset();

        var x = pos.left;
        var y = pos.top;

        var w = el.offsetWidth;
        var h = el.offsetHeight;

        var inS = layoutInfoElem.style;
        inS.top = y + 'px';
        inS.left = x + 'px';

        inS.width = (w - 2) + 'px';
        inS.height = (h - 2) + 'px';

        var inT = layoutInfoText;
        inT.innerHTML = (w == initialWidth ? w : '<strong>' + w + '</strong>') + ' x ' + (h == initialHeight ? h : '<strong>' + h + '</strong>');

        inT.style.top = (y + h / 2 - inT.offsetHeight / 2) + 'px';
        inT.style.left = (x + w / 2 - inT.offsetWidth / 2) + 'px';
    }
}

function IsLeftButton(ev) {
    var bt = ev.button;
    var s = navigator.userAgent.toLowerCase() + '';
    var b = $cmsj.browser;
    if (b.msie) {
        return (bt == 0) || (bt == 1);
    }
    else if (b.mozilla || b.opera || b.webkit) {
        return (bt == 0);
    }
    else {
        return (bt == 1);
    }
}

function InitHorizontalResizer(ev, elem, clientId, elementId, propertyName, inverted, infoId, callback) {
    if (ev == null) ev = window.event;

    if (IsLeftButton(ev)) {
        firstLayoutX = ev.clientX;
        firstLayoutY = ev.clientY;

        currentLayoutElem = elem;

        var elId = elementId;
        if (elId.indexOf('_') < 0) {
            elId = clientId + '_' + elId;
        }
        var el = Get(elId);

        var inId = infoId;
        if (inId == null) {
            inId = elId;
        }
        else if ((inId != '#') && (inId.indexOf('_') < 0)) {
            inId = clientId + '_' + inId;
        }

        var e = elem;
        if (e.horzProperty == null) {
            e.horzProperty = propertyName;
            e.horzClientId = clientId;
            e.horzElementId = elId;
            e.horzInverted = inverted;
            e.horzInfoId = inId;
            e.horzCallback = callback;
        }

        currentWidth = initialWidth = el.offsetWidth;
        currentHeight = initialHeight = el.offsetHeight;

        var infoElem = Get(inId);
        EnsureOverlay('e-resize', infoElem);

        InitReset(elem);

        lastResizedElem = el;
        lastLayoutElem = elem;

        el.vertProperty = null;
        el.horzProperty = propertyName;
        el.horzClientId = clientId;

        el.notResize = true;
    }
}

function InitVerticalResizer(ev, elem, clientId, elementId, propertyName, infoId, callback) {
    if (ev == null) ev = window.event;

    if (IsLeftButton(ev)) {
        firstLayoutX = ev.clientX;
        firstLayoutY = ev.clientY;

        currentLayoutElem = elem;

        var elId = elementId;
        var el = Get(elId);
        if (el == null) {
            elId = clientId + '_' + elId;
            el = Get(elId);
        }

        var inId = infoId;
        if (inId == null) {
            inId = elId;
        }
        else if ((inId != '#') && (inId.indexOf('_') < 0)) {
            inId = clientId + '_' + inId;
        }

        var e = elem;
        if (e.vertProperty == null) {
            e.vertProperty = propertyName;
            e.vertClientId = clientId;
            e.vertElementId = elId;
            e.vertInfoId = inId;
            e.vertCallback = callback;
        }

        currentWidth = initialWidth = el.offsetWidth;
        currentHeight = initialHeight = el.offsetHeight;

        var infoElem = Get(inId);
        EnsureOverlay('n-resize', infoElem);

        InitReset(elem);

        lastResizedElem = el;
        lastLayoutElem = elem;

        el.vertProperty = propertyName;
        el.vertClientId = clientId;
        el.horzProperty = null;

        el.notResize = true;
    }
}

function InitReset(e) {
    if (e.ondblclick != ResetProperties) {
        e.ondblclick = ResetProperties;
    }
}

function ResetProperties(ev) {
    if (lastResizedElem != null) {
        elem = lastResizedElem;
        if (elem.vertProperty != null) {
            SetWebPartProperty(elem.vertClientId, elem.vertProperty, '');
            elem.style.height = 'auto';
        }

        if (elem.horzProperty != null) {
            SetWebPartProperty(elem.horzClientId, elem.horzProperty, '');
            elem.style.width = 'auto';
        }
    }

    return false;
}

function LayoutMouseMove(ev) {
    if (ev == null) ev = window.event;

    var el = currentLayoutElem;
    if (el != null) {
        if (el.horzProperty != null) {
            var x = ev.clientX;
            var dx = firstLayoutX - x;
            if (el.horzInverted) {
                dx = -dx;
            }
            var elem = Get(el.horzElementId);
            currentWidth = initialWidth - dx;
            if (elem.originalWidth != null) elem.originalWidth = currentWidth;
            elem.style.width = currentWidth + 'px';

            if (el.horzInfoId != '#') {
                var infoElem = Get(el.horzInfoId);
                UpdateLayoutInfo(infoElem);
            }
        }

        if (el.vertProperty != null) {
            var y = ev.clientY;
            var dy = firstLayoutY - y;
            var elem = Get(el.vertElementId);
            currentHeight = initialHeight - dy;
            if (elem.originalHeight != null) elem.originalHeight = currentHeight;
            elem.style.height = currentHeight + 'px';

            if (el.vertInfoId != '#') {
                var infoElem = Get(el.vertInfoId);
                UpdateLayoutInfo(infoElem);
            }
        }

        if (el.parentNode.className == "WebPartResizer") {
            el.parentNode.style.width = "1px";
            el.parentNode.style.width = "";
        }

        if (el.resized) {
            el.resized(el);
        }
    }

    ev.returnValue = false;
    return false;
}

function InitBoxedImage(boxId, imgId) {
    var box = Get(boxId);
    if (box.resized == null) {
        box.resized = function (e) {
            var img = Get(imgId);
            var imgObj = new Image();
        };
    }
}

function LayoutMouseUp(ev) {
    if (currentLayoutElem != null) {
        var elem = currentLayoutElem;

        if ((elem.vertProperty != null) && (currentHeight != initialHeight)) {
            var h = currentHeight + 'px';
            SetWebPartProperty(elem.vertClientId, elem.vertProperty, h);
            if (elem.vertCallback) {
                elem.vertCallback(h);
            }
            Get(elem.vertElementId).notResize = null;
        }

        if ((elem.horzProperty != null) && (currentWidth != initialWidth)) {
            var w = currentWidth + 'px';
            SetWebPartProperty(elem.horzClientId, elem.horzProperty, w);
            if (elem.horzCallback) {
                elem.horzCallback(w);
            }
            Get(elem.horzElementId).notResize = null;
        }
    }

    currentLayoutElem = null;
    firstLayoutX = 0;
    firstLayoutY = 0;
    initialWidth = 0;
    initialHeight = 0;

    HideOverlay();

    return true;
}

function EnsureOverlay(cursor, el) {
    var inE = layoutInfoElem;
    if (inE == null) {
        inE = document.createElement('div');
        inE.className = 'LayoutInfoOverlay';
        inE.innerHTML = '&nbsp;';
        layoutInfoElem = inE;

        document.body.appendChild(inE);

        var inT = document.createElement('div');
        inT.className = 'LayoutInfoText';
        inT.innerHTML = '&nbsp;';
        layoutInfoText = inT;

        document.body.appendChild(inT);
    }

    var ovE = layoutOverlayElem;
    if (ovE == null) {
        ovE = document.createElement('div');
        ovE.className = 'LayoutOverlay';
        ovE.innerHTML = '&nbsp;';

        ovE.onmousemove = LayoutMouseMove;
        ovE.onmouseup = LayoutMouseUp;
        layoutOverlayElem = ovE;

        document.body.appendChild(ovE);
    }

    ovE.style.display = 'block';
    ovE.style.cursor = cursor;

    if (el != null) {
        inE.style.display = 'block';
        layoutInfoText.style.display = 'block';
    }

    UpdateLayoutInfo(el);
}

function HideOverlay() {
    layoutOverlayElem.style.display = 'none';
    if (layoutInfoElem != null) {
        layoutInfoElem.style.display = 'none';
        layoutInfoText.style.display = 'none';
    }
}

function SetCookie(cookieName, cookieValue, days) {
    var today = new Date();
    var expire = new Date();
    if ((days == null) || (days == 0)) {
        days = 1;
    }

    expire.setTime(today.getTime() + 3600000 * 24 * days);
    document.cookie = cookieName + "=" + escape(cookieValue) + ";expires=" + expire.toGMTString();
}

function ChangeLayoutItem(clientId, offset, instanceGuid, containerId) {
    var id = clientId + "_current";
    var curElem = Get(id);
    var items = parseInt(Get(clientId + "_items").value);

    var cur = parseInt(curElem.value);
    if ((cur >= 1) && (cur <= items) && (offset != 0)) {
        Get(clientId + "_item" + cur).style.display = 'none';
    }
    cur += offset;
    if (cur < 1) { cur = 1; }
    if (cur > items) { cur = items; }
    if ((cur >= 1) && (cur <= items)) {
        var el = Get(clientId + "_item" + cur);
        if (offset != 0) {
            el.style.display = 'block';
        }
    }
    curElem.value = cur;

    if (containerId != null) {
        var cont = Get(containerId);
        if (cont != null) {
            cont.currentStep = cur;
        }
    }

    if (instanceGuid != null) {
        SetCookie("CMSEd" + instanceGuid + "Current", cur, 1);
    }
}

function PreviousLayoutItem(clientId, containerId) {
    ChangeLayoutItem(clientId, -1, null, containerId);
}

function NextLayoutItem(clientId, containerId) {
    ChangeLayoutItem(clientId, 1, null, containerId);
}

function InitAutoResize(clientId, cname, adjust) {
    $cmsj(document).ready(function () {
        AutoResize(clientId, cname, adjust);
        setInterval("AutoResize('" + clientId + "', '" + cname + "', " + adjust + ")", 500);
    });
}

function CollectLocations(par, loc, cname, level) {
    var found = false;

    level -= 1;
    if (level == 0) {
        return false;
    }

    var child = par.firstChild;
    if (child == null) {
        return false;
    }

    while (child != null) {
        var c = child.className;
        var cmatch = ((c != null) && ((c == cname) || (c.startsWith(cname + ' '))));

        if (((cname == null) || cmatch) && !child.notUseForResize) {
            CollectLocations(child, loc, null, level);

            var jc = $cmsj(child);
            var ploc = jc.offset();

            if (ploc.top != null) {
                ploc.top += child.offsetHeight;
                if (ploc.top > loc.top) loc.top = ploc.top;
            }

            if ((ploc.left != null) && cmatch && (child.firstChild != null)) {
                var w = jc.children("div").width();
                ploc.left += w;
                if (ploc.left > loc.left) {
                    loc.left = ploc.left;
                }
            }

            found = true;
        }

        child = child.nextSibling;
    }

    return found;
}

function EqualHeight(groupClass) {
    if (currentLayoutElem == null) {
        $cmsj('.' + groupClass).equalHeight();
    }
}

function InitEqualHeight(groupClass) {
    setInterval('EqualHeight("' + groupClass + '")', 500);
}

function SetAllHeight(groupClass, h) {
    $cmsj('.' + groupClass).css('height', h);
}

function IsHidden(e) {
    var n = e;
    var abs = false;
    while (n.style != null) {
        var ns = n.style;
        if ((!abs && (ns.height == "0px")) || (ns.display == "none") || (ns.visibility == "hidden")) {
            return false;
        }
        if (ns.position == "absolute") {
            abs = true;
        }
        n = n.parentNode;
    }
}

function AutoResize(clientId, cname, adjust) {
    var par = Get(clientId);
    if ((par == null) || par.notResize) {
        return;
    }

    if (IsHidden(par)) {
        if (!par.wasHidden) {
            par.wasHidden = true;
            par.origHTML = par.innerHTML;
        }
        return;
    }
    // Check if the code had changed, if not do not resize
    if ((par.origHTML != null) && (par.origHTML == par.innerHTML)) {
        if (par.wasHidden) {
            par.wasHidden = false;
        }
        else {
            return;
        }
    }

    par.isResizing = true;

    try {
        var ploc = $cmsj(par).offset();
        var mloc = { left: ploc.left, top: ploc.top };

        if (!CollectLocations(par, mloc, cname, 3) && !adjust) {
            return;
        }

        var s = par.style;
        var marg = 5;

        var newwidth = (mloc.left - ploc.left) + marg;
        var newheight = (mloc.top - ploc.top) + marg;

        var cont = true;

        if (adjust) {
            if ((par.adjustedHeight == null) && (par.adjustedWidth == null)) {
                var p = par.parentNode;
                while (p != null) {
                    var ps = p.style;
                    var jp = $cmsj(p);
                    if (p.tagName == "FORM") {
                        if (p.offsetHeight < document.documentElement.offsetHeight) {
                            newheight = par.offsetHeight + (document.documentElement.offsetHeight - p.offsetHeight) - 1;
                        }
                        par.adjustedHeight = newheight;
                        break;
                    }

                    // Do not take in account the Web part toolbar wrapper tags
                    var isWPTWrapper = (p.className.match(/WPT/) != null);
                    if (isWPTWrapper) {
                        p = p.parentNode;
                        continue;
                    }

                    jp.addClass("default_width_check");

                    var nw = jp.width();
                    var nh = jp.height();

                    if ((nw != 1) || (nh != 1)) {
                        jp.removeClass("default_width_check");


                        if (p.offsetHeight > newheight) {
                            par.adjustedHeight = newheight = p.offsetHeight;
                        }
                        if (p.offsetWidth > newwidth) {
                            par.adjustedWidth = newwidth = p.offsetWidth;
                        }
                        break;
                    }

                    jp.removeClass("default_width_check");

                    p = p.parentNode;
                }
            }
            else {
                if ((par.adjustedWidth != null) && (newwidth < par.adjustedWidth)) {
                    newwidth = par.adjustedWidth;
                }
                if ((par.adjustedHeight != null) && (newheight < par.adjustedHeight)) {
                    newheight = par.adjustedHeight;
                }
            }
        }

        if (s.width != (newwidth + 'px')) {
            if (par.originalWidth == null) par.originalWidth = par.offsetWidth;
            if (newwidth < par.originalWidth) newwidth = par.originalWidth - 2;
            if (s.width != (newwidth + 'px')) {
                s.width = newwidth + 'px';
            }
        }

        if (s.height != (newheight + 'px')) {
            if (par.originalHeight == null) par.originalHeight = par.offsetHeight;
            if (newheight < par.originalHeight) newheight = par.originalHeight - 2;
            if (s.height != (newheight + 'px')) {
                s.height = newheight + 'px';
            }
        }
        s.overflow = 'hidden';
    }
    finally {
        par.isResizing = null;
    }
}

function SaveImage() {
    var ed = this;

    SetWebPartProperty(ed.wpid, ed.propName, ed.value);
}

function EditImageProperty(ev, e, wpid, propName, selUrl) {
    if (ev == null) ev = window.event;

    var je = $cmsj(e);
    je.e = e;

    if (selUrl != null) {
        var url = je.attr('src');
        if (url == null) {
            url = je.css('background-image');
        }
        e.onchange = SaveImage;
        e.origVal = e.value = url;
        ed = e;

        modalDialog(selUrl, "SelectImage", '95%', '86%');
    }

    ed.wpid = wpid;
    ed.propName = propName;

    var ev = $cmsj.event.fix(ev);
    ev.stopPropagation();
    ev.preventDefault();
}

function SaveElement() {
    var newVal = this.value;
    if ((newVal != null) && (newVal != "") && (newVal != this.origVal)) {
        var ed = this;
        this.innerHTML = this.value;

        SetWebPartProperty(ed.wpid, ed.propName, ed.value);
    }
}

function EditFormControlProperty(ev, e, wpid, propName, url) {
    if (ev == null) ev = window.event;

    var je = $cmsj(e);
    je.e = e;

    if (url != null) {
        e.origVal = e.value = e.innerHTML;
        e.save = SaveElement;
        ed = e;

        modalDialog(url, "EditProperty", 900, 600);
    }

    ed.wpid = wpid;
    ed.propName = propName;

    var ev = $cmsj.event.fix(ev);
    ev.stopPropagation();
    ev.preventDefault();
}

function EditColorProperty(ev, e, wpid, propName, type, targetId) {
    EditProperty(ev, e, wpid, propName, type, false, null, null, null, null, targetId, -1);
}

function EditTextProperty(ev, e, wpid, propName, type, line, img, val) {
    EditProperty(ev, e, wpid, propName, type, img, null, val, null, null, null, null, null, line);
}

function EditProperty(ev, e, wpid, propName, type, img, frm, val, selItems, selText, targetId, subFormat, sep, line) {
    if (ev == null) ev = window.event;

    var je = $cmsj(e);
    je.e = e;

    var ed = activeEditor = EnsureEditControls(type);
    ed.line = line;
    ed.target = je;
    ed.etarget = e;
    ed.format = frm;
    ed.subFormat = subFormat;
    ed.sep = sep;
    ed.selText = selText;
    ed.targetId = targetId;

    var ta = (type.toLowerCase() == "textarea");
    ed.handleLines = ta;

    var decode = true;

    if (val) {
        if (e.val == null) {
            e.val = val;
        }
        else {
            val = e.val;
        }
        decode = !ta;
    }
    if (selItems) {
        if (e.selItems == null) {
            e.selItems = selItems;
        }
        else {
            selItems = e.selItems;
        }
    }

    img = (img == true);
    ed.isImg = img;

    var c = val;
    if (c == null) {
        if (img) {
            c = je.attr('tooltip');
            if (c == null) {
                c = je.attr('onmouseover');
                c = c.substr(5, c.length - 7);
            }
            c = c.replace(/\\</g, '<').replace(/\\>/g, '>');
        }
        else {
            c = je.html();
        }
        if (ta) {
            c = c.replace(/[\r]?[\n]/g, '').replace(/<br \/>/g, '\n').replace(/<br>/g, '\n');
        }
    }

    var eVal = c;
    if (decode) {
        eVal = htmlDecode(eVal);
    }
    ed.val(eVal);
    ed.origVal = c;

    var pos = je.offset();
    ed.css({ top: pos.top, left: pos.left });

    if (ta) {
        var h = je.innerHeight();
        if (h < 200) h = 200;

        ed.css({ height: h });
    }

    if (type.toLowerCase() != "color") {
        var w = je.innerWidth();
        if (w < 300) w = 300;

        ed.css({ width: w });
    }

    propOverlay.show();
    ed.show();
    ed.focus();

    ed.wpid = wpid;
    ed.propName = propName;

    var ev = $cmsj.event.fix(ev);
    ev.stopPropagation();
    ev.preventDefault();
}

function htmlEncode(value) {
    return $cmsj('<div/>').text(value).html();
}

function htmlDecode(value) {
    return $cmsj('<div/>').html(value).text();
}

function SaveActiveProperty() {
    var ed = activeEditor;
    if (ed != null) {
        if (!ed.notSave) {
            var edVal = ed.val();
            var val = htmlEncode(edVal);
            var t = ed.target;

            if (ed.format) {
                var items = edVal.split('\n');
                var c = '';

                var selItems = ed.etarget.selItems;
                if (selItems == null) {
                    selItems = '';
                }

                var sep = ed.sep;
                if (sep == null) sep = '';

                for (var i = 0; i < items.length; i++) {
                    var item = items[i];
                    if ((item != null) && (item != '')) {
                        var itemCode = item;

                        if ((ed.subFormat != null) && (ed.subFormat != '')) {
                            var subItems = item.split(',');
                            itemCode = '';

                            for (var j = 0; j < subItems.length; j++) {
                                var subItem = htmlEncode(subItems[j]);
                                var subClass = "SubItem" + j + " " + (j % 2 == 0 ? "SubItemEven" : "SubItemOdd");
                                itemCode += GetItemCode(ed, ed.subFormat, selItems, subItem, subClass);
                            }
                        }
                        else {
                            itemCode = htmlEncode(itemCode);
                        }

                        var cls = "Item" + i + " " + (i % 2 == 0 ? "ItemEven" : "ItemOdd");
                        if (i > 0) {
                            c += ed.sep;
                        }
                        c += GetItemCode(ed, ed.format, selItems, itemCode, cls);
                    }
                }

                t.e.val = edVal;
                t.html(c);
            }
            else {
                var c = val;
                if (ed.handleLines) {
                    c = c.replace(/[\r]?[\n]/g, '<br />');
                }
                if (ed.isImg) {
                    if (t.attr("tooltip") == null) {
                        t.attr("onmouseover", "Tip('" + c + "')");
                    }
                    else {
                        t.attr("tooltip", c);
                    }
                }
                else {
                    c = ApplyReplacements(c);
                    t.html(c);
                }
            }

            t.e.val = edVal;

            if (ed.origVal != edVal) {
                SetWebPartProperty(ed.wpid, ed.propName, edVal, ed.line);
            }
        }

        ed.hide();
        propOverlay.hide();
    }

    activeEditor = null;
}

function GenerateSpaces(m) {
    var res = "";
    var count = m.length - 2;
    for (var i = 0; i < count; i++) {
        res += "<span class=\"WireframeSpace\">&nbsp;</span>";
    }

    return res;
}

function GetIconRe(ch) {
    return "\\[" + ch + "\\]";
}

function GetIconCode(cls) {
    return "<span class=\"" + cls + "\">&nbsp;</span>";
}

function GetBracketRe(ch) {
    return "\\[" + ch + "\\]((?:(?!\\[/" + ch + "\\]).)+)(?:\\[/" + ch + "\\]|$)";
}

function GetBracketsCode(cls) {
    return "<span class=\"" + cls + "\">$1</span>";
}

var replacements = [
            [GetIconRe("x"), GetIconCode("WireframeCheckBoxChecked"), ""],
            [GetIconRe("\\s?"), GetIconCode("WireframeCheckBox"), ""],

            [GetIconRe("-"), GetIconCode("WireframeMinus"), ""],
            [GetIconRe("+"), GetIconCode("WireframePlus"), ""],

            [GetIconRe("\\^"), GetIconCode("WireframeUp"), ""],
            [GetIconRe("v"), GetIconCode("WireframeDown"), ""],
            [GetIconRe("\\^v"), GetIconCode("WireframeUpDown"), ""],
            [GetIconRe("&lt;"), GetIconCode("WireframeLeft"), ""],
            [GetIconRe("&gt;"), GetIconCode("WireframeRight"), ""],

            ["\\(o\\)", GetIconCode("WireframeRadioChecked"), ""],
            ["\\(\\s?\\)", GetIconCode("WireframeRadio"), ""],

            [GetIconRe("dir"), GetIconCode("WireframeDir"), ""],
            [GetIconRe("file"), GetIconCode("WireframeFile"), ""],
            [GetIconRe("doc"), GetIconCode("WireframeDoc"), ""],
            [GetIconRe("(_+)"), GenerateSpaces, ""],

            [GetBracketRe("a"), GetBracketsCode("WireframeLink"), "$1"],
            [GetBracketRe("i"), GetBracketsCode("WireframeItalics"), "$1"],
            [GetBracketRe("b"), GetBracketsCode("WireframeBold"), "$1"],
            [GetBracketRe("u"), GetBracketsCode("WireframeUnderlined"), "$1"],
            [GetBracketRe("d"), GetBracketsCode("WireframeDisabled"), "$1"],

            ["{([a-z]+):(#?[0-9a-z]+)}((?:(?!{[a-z]+}).)+)(?:{[a-z]+}|$)", "<span style=\"$1: $2\">$3</span>", "$3"],

            ["\\\\n", "<br />", ""]
];

function ApplyReplacements(text, remove) {
    for (var i = 0; i < replacements.length; i++) {
        var repl = replacements[i];
        var re = new RegExp(repl[0], "gi");
        var rep = (remove ? repl[2] : repl[1]);
        text = text.replace(re, rep);
    }

    return text;
}

function GetItemCode(ed, format, sItems, item, itemClass) {
    var sel = '';
    var re = new RegExp("\\b" + item + "\\b", "gi");
    if (re.test(sItems)) {
        sel = ed.selText;
    }

    var itemValue = htmlEncode(item);

    var res = format.replace(/\{([0-9]+)-([0-9]+)\}/g, function (m, from, to) {
        from = parseInt(from);
        to = parseInt(to);

        return from + Math.round(Math.random() * (to - from));
    });

    var itemNoFormat = ApplyReplacements(item, true);

    item = ApplyReplacements(item, false);

    return res.replace(/\{0\}/g, item).replace(/\{1\}/g, sel).replace(/\{2\}/g, itemClass).replace(/\{3\}/g, itemValue).replace(/\{4\}/g, itemNoFormat);
}

function CheckListClick(e, wpid, propName) {
    e.checked = !e.checked;

    var res = new Array();

    $cmsj('.' + wpid + '_check').each(function (i, item) {
        if (item.checked) {
            res.push(item.value);
        }
    });

    res = res.join('\n');
    e.parentNode.selItems = res;

    SetWebPartProperty(wpid, propName, res);
}

function RadioListClick(e, wpid, propName) {
    e.parentNode.selItems = '{0}';
    e.checked = true;

    SetWebPartProperty(wpid, propName, e.value);
}

function HSVToRGB(h, s, v) {
    var s = s / 100,
    v = v / 100;

    var hi = Math.floor((h / 60) % 6);
    var f = (h / 60) - hi;
    var p = v * (1 - s);
    var q = v * (1 - f * s);
    var t = v * (1 - (1 - f) * s);

    var rgb = [];

    switch (hi) {
        case 0: rgb = [v, t, p]; break;
        case 1: rgb = [q, v, p]; break;
        case 2: rgb = [p, v, t]; break;
        case 3: rgb = [p, q, v]; break;
        case 4: rgb = [t, p, v]; break;
        case 5: rgb = [v, p, q]; break;
    }

    var r = Math.min(255, Math.round(rgb[0] * 256)),
        g = Math.min(255, Math.round(rgb[1] * 256)),
        b = Math.min(255, Math.round(rgb[2] * 256));

    return [r, g, b];
}

function ToHex(i) {
    var hex = parseInt(i).toString(16);
    return (hex.length < 2) ? "0" + hex : hex;
}

function EnsureColorEditor() {
    var bd = $cmsj('body');

    if (colorEdit == null) {
        colorEdit = $cmsj('<div class="ColorEditor"></div>');
        colorEdit.notSave = true;

        var colors = new Array('#000', '#666', '#999', '#ccc', '#fff', '');

        for (var h = 0; h <= 256;) {
            s = 50;
            for (var v = 60; v <= 100;) {
                var rgb = HSVToRGB(h, s, v);
                colors.push('#' + ToHex(rgb[0]) + ToHex(rgb[1]) + ToHex(rgb[2]));

                v += 10;
            }
            h += 32;

            colors.push('');
        }

        $cmsj.each(colors, function (i, col) {
            if (col == '') {
                colorEdit.append('<div class="ClearBoth"></div>');
                return;
            }
            var c = $cmsj('<div class="ColorEditorColor" style="background-color: ' + col + ';">&nbsp;</div>');
            c.click(function () {
                var ed = colorEdit;
                ed.target.css('background-color', col);

                var tid = ed.targetId;
                if ((tid != null) && (tid != '')) {
                    document.getElementById(tid).style.backgroundColor = col;
                }

                SetWebPartProperty(ed.wpid, ed.propName, col);

                propOverlay.hide();
                colorEdit.hide();
            });

            colorEdit.append(c);
        });

        colorEdit.append('<div class="ClearBoth"></div>');

        bd.append(colorEdit);
    }

    return colorEdit;
}

function EnsureEditControls(type) {
    var b = $cmsj('body');

    if (propOverlay == null) {
        propOverlay = $cmsj('<div class="EditOverlay">&nbsp</div>');
        propOverlay.click(SaveActiveProperty);
        b.append(propOverlay);
    }

    switch (type.toLowerCase()) {
        case "color":
            return EnsureColorEditor();

        case "textbox":
            if (textBoxEdit == null) {
                textBoxEdit = $cmsj('<input class="EditProperty form-control" type="text" />');
                textBoxEdit.keypress(function (event) {
                    if (event.which == 13) {
                        SaveActiveProperty();
                        event.preventDefault();
                    }
                });

                b.append(textBoxEdit);
            }
            return textBoxEdit;

        default:
            if (textAreaEdit == null) {
                textAreaEdit = $cmsj('<textarea wrap="off" class="EditProperty TextAreaMedium form-control"></textarea>');
                b.append(textAreaEdit);
            }
            return textAreaEdit;
    }
}

function ToggleProperty(ev, e, wpid, propName, trueUrl, falseUrl) {
    var src = e.src;
    var val = e.src.indexOf(trueUrl, src.length - trueUrl.length) < 0;
    if (val) {
        e.src = trueUrl;
    }
    else {
        e.src = falseUrl;
    }

    SetWebPartProperty(wpid, propName, val + '');
}

function SetHandleClass(id) {
    var e = Get(id);
    e.className = 'WebPartHandle ' + e.className;
}