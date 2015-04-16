cmsdefine(['CMS/WebFormCaller'], function (webFormCaller) {
    'use strict';

    var Module = function(data) {
        var logText = '',
            received = 0,
            callBackParam,
            asyncProcessFinished = false,
            timeout = null,
            asyncBusy = false,
            config = data,

            setNextTimeout = function() {
                timeout = setTimeout(getAsyncStatus, 200);
            },

            getAsyncStatus = function() {
                if (!asyncBusy) {
                    asyncBusy = true;
                    setTimeout('asyncBusy = false;', 2000);
                    callBackParam = received + '|' + config.machineName;
                    var cbOptions = {
                        targetControlUniqueId: config.uniqueId,
                        args: callBackParam,
                        successCallback: receiveAsyncStatus
                    };
                    webFormCaller.doCallback(cbOptions);
                }
                if (asyncProcessFinished) {
                    cancel(false);
                    return;
                } else {
                    setNextTimeout();
                }
            },

            setClose = function() {
                var cancelElem = document.getElementById(config.id + '_btnCancel');
                if (cancelElem != null) {
                    cancelElem.value = config.closeText;
                }
            },

            receiveAsyncStatus = function(rvalue) {
                var totalReceived, i, resultValue, values, code;

                asyncBusy = false;
                if (asyncProcessFinished) {
                    return;
                }

                values = rvalue.split('|');

                code = values[0];
                totalReceived = parseInt(values[1]);

                resultValue = '';

                for (i = 2; i < values.length; i++) {
                    resultValue += values[i];
                }

                if (resultValue != '') {
                    setLog(resultValue, totalReceived);
                }

                if (code == 'running') {
                } else if (code == 'finished') {
                    asyncProcessFinished = true;

                    webFormCaller.doPostback({
                        targetControlUniqueId: config.uniqueId,
                        args: 'finished|' + config.machineName
                    });
                } else if ((code == 'threadlost') || (code == 'stopped')) {
                    asyncProcessFinished = true;
                    setClose();
                } else if (code == 'error') {
                    asyncProcessFinished = true;

                    if (config.postbackOnError) {
                        webFormCaller.doPostback({
                            targetControlUniqueId: config.uniqueId,
                            args: 'error|' + config.machineName
                        });
                    } else {
                        setClose();
                    }
                }
            },

            setLog = function(text, totalReceived) {
                var elem = document.getElementById(config.logId),
                    lines,
                    log;

                if (config.maxLogLines == 0) {
                    logText += text;
                } else {
                    logText = text;
                }
                received = totalReceived;

                lines = logText.split('\n');

                log = '';
                if (config.reversed) {
                    for (var i = lines.length - 1; i >= 0; i--) {
                        if (i != 0) {
                            log += lines[i] + '<br />';
                        } else {
                            log += lines[i];
                        }
                    }
                } else {
                    for (var i = 0; i < lines.length; i++) {
                        if (i != lines.length) {
                            log += lines[i] + '<br />';
                        } else {
                            log += lines[i];
                        }
                    }
                }

                elem.innerHTML = log;
            },

            cancel = function(withPostback) {
                var t;

                asyncProcessFinished = true;
                if (withPostback) {
                    webFormCaller.doPostback({
                        targetControlUniqueId: config.cancelButtonUniqueId
                    });
                } else {
                    t = timeout;
                    if ((t != 'undefined') && (t != null)) {
                        clearTimeout(timeout);
                    }
                }
            }

        setNextTimeout();

        window.CMS = window.CMS || {};

        return window.CMS['AC_' + config.id] = {
            cancel: cancel
        };
    }

    return Module;
});