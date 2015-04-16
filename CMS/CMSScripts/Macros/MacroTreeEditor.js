var documentOnClick = null;
var macroTreeHasFocus = false;
var rtl = (document.body.className.indexOf('RTL') >= 0);

// Inserts the macro to caret position (in syntax highlighter).
function InsertToMacroEditor(macro, extendedAreaElem, txtAreaId) {

    if ((typeof (extendedAreaElem) != 'undefined') && (extendedAreaElem != null)) {
        // Check whether the ExtendedArea is used
        var pos = extendedAreaElem.getCursor();
        if ((pos.line == null) && (window.addEventListener)) {
            var start = { node: extendedAreaElem.getLine(lineStart), offset: caretPositionStart };
            var end = { node: extendedAreaElem.getLine(lineEnd), offset: caretPositionEnd };
            extendedAreaElem.editor.replaceRange(start, end, macro);
        }
        else {
            extendedAreaElem.replaceSelection(macro);
        }
        extendedAreaElem.setSelection(pos, { line: pos.line, ch: pos.character + macro.length });
    }
    else {
        InsertMacroPlain(macro, txtAreaId);
    }
}


// Inserts the macro to caret position (in text area)
function InsertMacroPlain(text, txtAreaId) {
    var obj = document.getElementById(txtAreaId);
    if (obj != null) {
        if (document.selection) {
            // IE
            obj.focus();
            var orig = obj.value.replace(/\r\n/g, '\n');
            var range = document.selection.createRange();
            if (range.parentElement() != obj) {
                return false;
            }
            range.text = text;
            var actual = tmp = obj.value.replace(/\r\n/g, '\n');
            for (var diff = 0; diff < orig.length; diff++) {
                if (orig.charAt(diff) != actual.charAt(diff)) break;
            }
            for (var index = 0, start = 0; tmp.match(text) && (tmp = tmp.replace(text, '')) && index <= diff; index = start + text.length) {
                start = actual.indexOf(text, index);
            }
        } else {
            // Firefox
            var start = obj.selectionStart;
            var end = obj.selectionEnd;
            obj.value = obj.value.substr(0, start) + text + obj.value.substr(end, obj.value.length);
        }
        if (start != null) {
            SetCaretTo(obj, start + text.length);
        } else {
            obj.value += text;
        }
    }
}

// Handles the macro tree node click
function nodeClick(macro, editorElem, pnlMacroTreeId, txtAreaId) {
    if ((macro != null) && (macro != '')) {
        if (InsertToMacroEditor) {
            InsertToMacroEditor(macro, editorElem, txtAreaId);
        } else {
            editorElem.setValue(macro);
        }
    }
    showHideMacroTree(pnlMacroTreeId);
}

function hideMacroTree(pnlMacroTreeId) {
    if (!macroTreeHasFocus) {
        showHideMacroTree(pnlMacroTreeId, null, null, 0, 0, false, true);
    }
}

// Hides / shows div with macro tree
function showHideMacroTree(pnlMacroTreeId, editorElem, autoCompletionObject, leftOffset, topOffset, forceAbove, forceHide) {
    var pnlTree = document.getElementById(pnlMacroTreeId);
    if (pnlTree != null) {
        // Set the visibility
        if ((pnlTree.style.display == 'none') && !forceHide) {
            pnlTree.style.display = 'block';

            // Set the document onclick to hide the the tree help and set the original value
            bodyOnClick = document.onclick;
            setTimeout('document.onclick = new Function("hideMacroTree(\'' + pnlMacroTreeId + '\')");', 500);

        } else {
            pnlTree.style.display = 'none';

            // Set the document onclick event back to original value
            document.onclick = bodyOnClick;
        }

        // Position the div
        if ((editorElem != null) && (autoCompletionObject != null)) {
            autoCompletionObject.forceAbove = forceAbove;
            if (rtl) {
                // RTL positioning
                pnlTree.style.left = (-(Math.abs(editorElem.getWrapperElement().offsetWidth - pnlTree.offsetWidth) - 26) - leftOffset) + 'px';
            } else {
                // Normal LTR positioning
                pnlTree.style.left = (-leftOffset) + 'px';
            }
            if (forceAbove) {
                pnlTree.style.top = (-pnlTree.offsetHeight - topOffset - 33) + 'px';
            } else {
                pnlTree.style.top = (editorElem.getWrapperElement().offsetHeight - topOffset - 28) + 'px';
            }
        }
    }
}