window.cmsResizeIntervalIsSet = false;
var cmsDragEditableItem = {};

function InitializePage() {
    if (IsIExplorer(8)) {
        // Ensure initialize page after load
        AddEvent(window, 'load', function () {
            InitializePageStart();
        });
    }
    else {
        InitializePageStart();
    }
}

function InitializePageStart() {
    window.cmsHeader = null;
    window.cmsHeaderPad = null;
    window.cmsFooter = null;
    window.cmsFooterPad = null;

    InitializeHeader();
    InitializeFooter();
    InitializeToolbarResize();

    // Ensure header width on resize
    AddEvent(window, 'resize', function () {
        if (typeof (EnsureToolbarWidth) === 'function') {
            EnsureToolbarWidth();
        }
        if (typeof (InitializeToolbarResize) === 'function') {
            InitializeToolbarResize();
        }
    });
}

function InitializeToolbarResize() {
    if (window.cmsHeader != null) {
        if (window.CKEDITOR) {
            if (window.CKEDITOR.on) {
                function resizeCKToolbar() {
                    setTimeout(function () {
                        ResizeToolbar();
                    }, 100);
                }

                function initCKResizeToolbar(e) {
                    e.editor.on('focus', resizeCKToolbar);
                    e.editor.on('afterCommandExec', function (e) {
                        if (e.data.name === 'toolbarCollapse') {
                            resizeCKToolbar();
                        }
                    });
                    resizeCKToolbar();
                }

                // Add on load event for editor
                CKEDITOR.on('loaded', function (e) {
                    CKEDITOR.on('instanceReady', initCKResizeToolbar);
                });

                // Add instance ready event
                CKEDITOR.on('instanceReady', initCKResizeToolbar);

                // If there are instances initialize resize
                var editorName = null;
                if (CKEDITOR.instances) {
                    for (editorName in CKEDITOR.instances) {
                        if (editorName) {
                            var editor = CKEDITOR.instances[editorName];
                            initCKResizeToolbar({
                                'editor': editor
                            });
                        }
                    }
                }

                // Resize once if there are no instances
                if (editorName === null) {
                    ResizeToolbar();
                }
            } else {
                setTimeout(function () {
                    InitializeHeader();
                    InitializeFooter();
                    InitializeToolbarResize();
                }, 100);
            }
        } else {
            ResizeToolbar();
        }
    } else {
        setTimeout(function () {
            InitializeHeader();
            InitializeFooter();
            InitializeToolbarResize();
        }, 100);
    }
}

function InitializeHeader() {
    try {
        var docObj = (window.frames['pageview'] ? window.frames['pageview'].document : window.document);
        window.cmsHeader = docObj.getElementById('CMSHeaderDiv');
        if (window.cmsHeader !== null) {
            if (window.cmsHeader.style.position !== 'fixed') {
                window.cmsHeader.style.position = 'fixed';
                window.cmsHeader.style.top = '0';
                window.cmsHeader.style.left = '0';
                window.cmsHeader.style.right = '0';
                window.cmsHeader.style.bottom = 'auto';
                window.cmsHeader.style.zIndex = '10000';
                EnsureToolbarWidth();
            }
            window.cmsHeaderPad = docObj.getElementById('CMSHeaderPad');
            if (window.cmsHeaderPad === null) {
                // Create new padding div for header
                window.cmsHeaderPad = docObj.createElement('div');
                window.cmsHeaderPad.id = 'CMSHeaderPad';
                window.cmsHeaderPad.style.height = '0px';
                window.cmsHeaderPad.style.lineHeight = '0px';
                window.cmsHeader.parentNode.insertBefore(window.cmsHeaderPad, window.cmsHeader);

                CheckBackgroundPosition(docObj.body);

                if (window.CMSRfrLblsPos) {
                    window.CMSRfrLblsPos();
                }
            }
        }
    } catch (err) {
    }
}

function InitializeFooter() {
    try {
        var docObj = (window.frames['pageview'] ? window.frames['pageview'].document : window.document);

        window.cmsFooter = docObj.getElementById('CMSFooterDiv');
        if (window.cmsFooter !== null) {
            if (window.cmsFooter.style.position !== 'fixed') {
                window.cmsFooter.style.position = 'fixed';
                window.cmsFooter.style.bottom = '0';
                window.cmsFooter.style.left = '0';
                window.cmsFooter.style.width = '100%';
                window.cmsFooter.style.overflow = 'hidden';
                window.cmsFooter.style.zIndex = '10000';
            }
            window.cmsFooterPad = docObj.getElementById('CMSFooterPad');
            if (window.cmsFooterPad === null) {
                // Create new padding div for header
                window.cmsFooterPad = docObj.createElement('div');
                window.cmsFooterPad.id = 'CMSFooterPad';
                window.cmsFooterPad.style.height = '0px';
                // Append at the end of page
                docObj.body.appendChild(window.cmsFooterPad);
            }
        }
    } catch (err) {
    }
}

function CheckBackgroundPosition(element) {
    var bgTopPos = '', bgLeftPos = '';
    if (document.defaultView && document.defaultView.getComputedStyle) {
        var bgPos = document.defaultView.getComputedStyle(element, '').getPropertyValue('background-position');
        bgLeftPos = bgPos.match(/^[\w-%]+/)[0];
        bgTopPos = bgPos.match(/[\w-%]+$/)[0];
    } else if (element.currentStyle) { // IE
        bgTopPos = element.currentStyle['backgroundPositionY'];
        bgLeftPos = element.currentStyle['backgroundPositionX'];
    }

    var pixelsPosition = /px$/.test(bgTopPos);
    if ((bgTopPos === '0%') || (pixelsPosition) || (bgTopPos === 'top')) {
        window.moveBodyBg = true;
        window.bodyBgLeftPos = bgLeftPos;

        if (pixelsPosition) {
            window.bodyBgTopPos = parseInt(bgTopPos, 10);
        }
        else {
            window.bodyBgTopPos = 0;
        }
    }
}

function GetHeight() {
    var headerHeight = -1;
    if (window.cmsHeader) {
        // If header contains html tags get offset height
        if ((window.cmsHeader.innerHTML.indexOf("<") !== -1) && (window.cmsHeader.innerHTML.indexOf(">") !== -1)) {
            headerHeight = window.cmsHeader.offsetHeight;
        } else {
            headerHeight = 0;
        }
    }
    var footerHeight = -1;
    if (window.cmsFooter) {
        // If footer contains html tags get offset height        
        if ((window.cmsFooter.innerHTML.indexOf("<") !== -1) && (window.cmsFooter.innerHTML.indexOf(">") !== -1)) {
            footerHeight = window.cmsFooter.offsetHeight;
        } else {
            footerHeight = 0;
        }
    }
    // if height was not set try to initialize
    if ((headerHeight === -1) && (footerHeight === -1)) {
        InitializePage();
        headerHeight = 0;
        footerHeight = 0;
    }
    return { 'header': headerHeight, 'footer': footerHeight };
}

function ResizeToolbar() {
    try {
        var height = GetHeight();
        if (window.cmsHeaderPad) {
            window.cmsHeaderPad.style.height = height.header + 'px';
            if (window.moveBodyBg) {
                var pos = window.bodyBgLeftPos + ' ' + (window.bodyBgTopPos + height.header) + 'px';
                window.document.body.style.backgroundPosition = pos;
            }
            if (window.CMSRfrLblsPos) {
                window.CMSRfrLblsPos();
            }
            if (window.cmsfixpanelheight) {
                window.cmsHeader.style.top = cmsfixpanelheight + 'px';
            }
        }
        if (window.cmsFooterPad) {
            window.cmsFooterPad.style.height = height.footer + 'px';
        }
    } catch (err) {
        InitializeHeader();
        InitializeFooter();
    }
}

function ShowToolbar() {
    InitializeHeader();
    InitializeFooter();
    ResizeToolbar();
}

function EnsureToolbarWidth() {
    if (window.cmsHeader) {
        if (IsIExplorer()) {
            // Set the width manually. This will speed up web part toolbar in IE
            var paddingLeft = window.cmsHeader.style.paddingLeft.replace(/px/, "");
            var paddingRight = window.cmsHeader.style.paddingRight.replace(/px/, "");
            window.cmsHeader.style.width = document.documentElement.clientWidth - paddingLeft - paddingRight + 'px';
        } else {
            window.cmsHeader.style.width = 'auto';
        }
    }
}

function Approve(nodeId, stepId) {
    try {
        if (window.frames['pageview'].Approve) {
            window.frames['pageview'].Approve(nodeId, stepId);
        } else {
            alert(notAllowedAction);
        }
    } catch (err) {
        alert(notAllowedAction);
    }
}

function Reject(nodeId) {
    if (window.frames['pageview'].Reject) {
        window.frames['pageview'].Reject(nodeId);
    }
}

function CheckIn(nodeId) {
    try {
        if (window.frames['pageview'].CheckIn) {
            window.frames['pageview'].CheckIn(nodeId);
        } else {
            alert(notAllowedAction);
        }
    } catch (err) {
        alert(notAllowedAction);
    }
}

function FramesRefresh(refreshTree, selectNodeId) {
    if ((parent !== this) && parent.FramesRefresh) {
        parent.FramesRefresh(refreshTree, selectNodeId);
    }
}

function RefreshTree(nodeId, selectNodeId) {
    if ((parent !== this) && parent.RefreshTree) {
        return parent.RefreshTree(nodeId, selectNodeId);
    }
    return false;
}

function SelectNode(nodeId, nodeElem, tab) {
    if ((parent !== this) && parent.SelectNode) {
        return parent.SelectNode(nodeId, nodeElem, tab);
    }
}

function SetMode(mode, passive) {
    if (parent != this) {
        parent.SetMode(mode, passive);
    }
}

function CreateAnother() {
    if ((parent !== this) && parent.CreateAnother) {
        parent.CreateAnother();
    }
    else {
        window.location.replace(window.location.href);
    }
}

function CreateAnotherWithParam(param) {
    if ((parent !== this) && parent.CreateAnother) {
        parent.CreateAnother();
    }
    else {
        window.location.replace(window.location.href + param);
    }
}

// Edit mode actions
function NotAllowed(baseUrl, action) {
    if ((parent !== this) && parent.NotAllowed) {
        parent.NotAllowed(baseUrl, action);
    }
}

function NewDocument(parentNodeId, className) {
    if ((parent !== this) && parent.NewDocument) {
        parent.NewDocument(parentNodeId, className);
    }
}

function ConvertDocument(parentNodeId, convertDocumentId) {
    if ((parent !== this) && parent.ConvertDocument) {
        parent.ConvertDocument(parentNodeId, convertDocumentId);
    }
}

function DeleteDocument(nodeId) {
    if ((parent !== this) && parent.DeleteDocument) {
        parent.DeleteDocument(nodeId);
    }
}

function EditDocument(nodeId, tab) {
    if ((parent !== this) && parent.EditDocument) {
        parent.EditDocument(nodeId, tab);
    }
}

function SpellCheck(spellURL) {
    try {
        if (window.frames['pageview'].SpellCheck) {
            window.frames['pageview'].SpellCheck(spellURL);
        } else {
            alert(notAllowedAction);
        }
    } catch (err) {
        alert(notAllowedAction);
    }
}

// File created
function FileCreated(nodeId, parentNodeId, closeWindow) {
    if ((parent !== this) && parent.FileCreated) {
        parent.FileCreated(nodeId, parentNodeId, closeWindow);
    }
}

function FocusFrame() {
    var fr = document.getElementById("pageview");
    try {
        if (document.all)
            //IE
        {
            fr.document.body.focus();
        } else
            //Firefox
        {
            fr.contentDocument.body.focus();
        }
    } catch (err) {
    }
}

function AddEvent(elem, type, eventHandle) {
    if (elem === null || elem === undefined)
        return;
    if (elem.addEventListener) {
        elem.addEventListener(type, eventHandle, false);
    } else if (elem.attachEvent) {
        elem.attachEvent("on" + type, eventHandle);
    }
};

function IsIExplorer(versionNum) {
    if (/MSIE (\d+\.\d+);/.test(navigator.userAgent)) {
        if (versionNum) {
            var ieversion = new Number(RegExp.$1)
            return (ieversion == versionNum)
        }
        return true;
    }
    return false;
}

// Refresh frame in split mode
function SplitModeRefreshFrame() {
    if ((parent !== this) && parent.SplitModeRefreshFrame) {
        parent.SplitModeRefreshFrame();
    }
}

// Refresh selected device
function ChangeDevice(device) {
    if ((parent !== this) && parent.ChangeDevice) {
        parent.ChangeDevice(device);
    }
}

// Rename the text area element (ID and NAME parameter) when moving widgets between zones
function CKRenameWidgetTextareas(zoneName, targetZoneName) {
    if ((cmsDragEditableItem !== null)
        && (typeof (CKEDITOR) !== "undefined")
        && (CKEDITOR !== null)
        && (CKEDITOR.instances !== null)) {
        var items = cmsDragEditableItem;

        for (var key in cmsDragEditableItem) {
            var instanceId = key;
            var config = cmsDragEditableItem[key];
            var el = document.getElementById(instanceId);

            if (el !== null) {
                // Generate correct element IDs to ensure correct postback behavior
                var zoneId = zoneName.replace(/\$/g, "_");;
                var targetZoneId = targetZoneName.replace(/\$/g, "_");;
                instanceId = el.id.replace(zoneId, targetZoneId);

                // Update htmlEditor identifiers to reflect the new target zone
                el.id = instanceId;
                el.name = el.name.replace(zoneName, targetZoneName);

                // Restore editable content
                var jObj = $cmsj(el);
                jObj.val(jObj.data('value'));
                jObj.html(jObj.val());
                el.defaultValue = jObj.html();

                // Reload the CKEditor
                if (typeof (CKReplace) === 'function') {
                    CKReplace(instanceId, config);
                }
            }
        }
    }
    cmsDragEditableItem = {};
}

function BeforeDropWebPart(container, item, position) {
    // Build a list of text areas (and store their value) which are being dragged because text areas loose their value when removed and inserted into DOM.
    cmsDragEditableItem = {};
    if ((typeof (CKEDITOR) !== "undefined")
        && (CKEDITOR !== null)
        && (CKEDITOR.instances !== null)) {

        $cmsj('textarea', item).each(function () {
            var jObj = $cmsj(this);
            var instanceId = jObj.attr('id');
            var inst = CKEDITOR.instances[instanceId];

            if (typeof inst !== 'undefined') {
                // Store editable content
                jObj.val(inst.getData());
                jObj.data('value', jObj.val());
                cmsDragEditableItem[instanceId] = inst.config;

                // Destroy editor instance
                inst.destroy();
            }
        });
    }
}