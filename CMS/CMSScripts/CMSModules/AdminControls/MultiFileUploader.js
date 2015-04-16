cmsdefine(['CMS/EventHub'], function (EventHub) {
    'use strict';


    var MultiFileUploader = function (data) {
        if (!window.File || !window.FileReader || !window.FileList || !window.Blob) {
            document.getElementById(data.overlayClientID).style.display = 'none';
            return;
        }

        var BYTES_PER_CHUNK = 4194304; // Default chunk size is 4 MB

        var uploader = document.getElementById(data.uploaderClientID),
            maxNumberToUpload = data.maxNumberToUpload,
            maximumTotalUpload = data.maximumTotalUpload,
            maximumUploadSize = data.maximumUploadSize,
            uploadChunkSize = data.uploadChunkSize,
            onUploadBegin = data.onUploadBegin,
            onUploadCompleted = data.onUploadCompleted,
            onUploadProgressChanged = data.onUploadProgressChanged,
            containerId = data.containerID,
            filesCount = 0,
            currentFileIndex = 0,
            totalUploadSize = 0,
            currentUploadedSize = 0,
            filesLeft = 0,
            files,
            canceling = false,
            canceled = false,
            allowedExtensions = data.allowedExtensions.split('|')[0];

        // Set chunk size.
        if (uploadChunkSize === 0) {
            uploadChunkSize = BYTES_PER_CHUNK;
        }

        // Calculates total size of all uploaded files
        var calculateTotalUploadSize = function () {
            var size = 0;
            for (var i = 0, l = files.length; i < l; i++) {
                size += files[i].size;
            }
            return size;
        };

        // Returns size of the largest file
        var getMaxFileSize = function () {
            var size = 0;
            for (var i = 0, l = files.length; i < l; i++) {
                size += files[i].size;
            }
            return size;
        };

        // Check if all files have allowed extension. If no, the function returns first wrong extension
        var checkAllowedExtensions = function () {
            if (data.allowedExtensions) {
                var extensionsString = data.allowedExtensions.split('|');
                var extensions = extensionsString[0].split(';');
                var fileName,
                    fileAllowed,
                    extension;

                for (var i = 0; i < files.length; i++) {
                    fileAllowed = false;
                    fileName = files[i].name.toLowerCase();

                    for (var j = 0; j < extensions.length; j++) {
                        extension = extensions[j].toLowerCase();
                        if (fileName.match(extension + '$')) {
                            fileAllowed = true;
                        }
                    }
                    if (!fileAllowed) {
                        return fileName.substr(fileName.lastIndexOf('.') + 1);
                    }
                }
            }
            return "";
        };

        // Cancel running upload
        var cancelUpload = function () {
            canceling = true;
        };

        EventHub.subscribe('UploadCanceled', function (senderContainerId) {
            if (senderContainerId === containerId) {
                cancelUpload();
            }
        });

        // Checks on server if file can be uploaded and uploads it
        var uploadFile = function (file) {
            var xhr = new XMLHttpRequest();

            xhr.open('post', data.uploadPage +
                '?InstanceGuid=' + data.instanceGuid +
                '&filename=' + encodeURIComponent(file.name) +
                '&FilesCount=' + filesCount +
                '&ResizeArgs=' + data.resizeArgs +
                '&GetBytes=true' +
                '&' + data.modeParameters +
                '&' + data.aditionalParameters +
                '&CurrentFileIndex=' + (currentFileIndex + 1) +
                '&AllowedExtensions=' + data.allowedExtensions, true);

            xhr.onloadend = function (e) {
                var onAfterUpload = xhr.responseText;
                var parts = onAfterUpload.split('|');
                if (parts.length == 2) {
                    // Show error message
                    alert(parts[1]);
                    if (onUploadCompleted) {
                        executeFunctionByName(onUploadCompleted, window, containerId);
                    }
                } else {
                    // Upload file
                    uploadChunks(file);
                }
            };

            // Send request
            xhr.send();
        };

        var uploadChunks = function (file, start) {
            if (!canceled) {
                start = start || 0;

                var chunkSize = 0,
                    nextChunkStart = start + uploadChunkSize,
                    xhr = new XMLHttpRequest();

                xhr.open('post', data.uploadPage +
                    '?InstanceGuid=' + data.instanceGuid +
                    '&filename=' + encodeURIComponent(file.name) +
                    '&FilesCount=' + filesCount +
                    '&ResizeArgs=' + data.resizeArgs +
                    '&complete=' + (nextChunkStart >= file.size).toString() +
                    '&StartByte=' + start +
                    '&' + data.modeParameters +
                    '&' + data.aditionalParameters +
                    '&CurrentFileIndex=' + (currentFileIndex + 1) +
                    '&AllowedExtensions=' + data.allowedExtensions +
                    (canceling ? '&canceled=1' : ''), true);

                xhr.upload.onprogress = function (e) {
                    if (e.lengthComputable && onUploadProgressChanged) {
                        executeFunctionByName(onUploadProgressChanged, window, containerId, totalUploadSize, (currentUploadedSize + e.loaded), 0);
                    }
                };

                xhr.onloadend = function (e) {
                    // Check error
                    var onAfterUpload = xhr.responseText;
                    if (onAfterUpload) {
                        if (onAfterUpload.lastIndexOf("0|", 0) === 0) {
                            // Response contains error message, so display it
                            var parts = onAfterUpload.split('|');
                            alert(parts[1]);
                            cancelUpload();
                            if (onUploadCompleted) {
                                executeFunctionByName(onUploadCompleted, window, containerId);
                            }
                            return;
                        }
                    }

                    currentUploadedSize += chunkSize;
                    if (nextChunkStart < file.size) {
                        // Upload next chunk of file
                        uploadChunks(file, nextChunkStart);
                    } else {
                        filesLeft--;

                        if (filesLeft > 0) {
                            //Upload next file
                            currentFileIndex++;
                            uploadFile(files[currentFileIndex]);
                        } else {
                            // All files uploaded, do after upload actions
                            if (onAfterUpload) {
                                new Function(onAfterUpload)();
                            }
                            if (onUploadCompleted) {
                                executeFunctionByName(onUploadCompleted, window, containerId);
                            }
                        }
                    }
                };

                if (canceling) {
                    canceled = true;
                }

                // Set appropriate headers
                xhr.setRequestHeader('Content-Type', 'application/octet-stream');

                var chunk = file.slice(start, nextChunkStart);
                chunkSize = chunk.size;

                // Send the chunk
                xhr.send(chunk);
            } else {

                if (onUploadCompleted) {
                    executeFunctionByName(onUploadCompleted, window, containerId);
                }
            }
        };

        uploader.onchange = function (e) {
            filesCount = this.files.length;
            if (!filesCount) {
                return;
            }

            currentUploadedSize = 0;
            files = this.files;
            totalUploadSize = calculateTotalUploadSize();
            filesLeft = filesCount;
            currentFileIndex = 0;
            canceling = false;
            canceled = false;

            // Check extensions
            var extension = checkAllowedExtensions();
            if (extension) {
                alert(formatString(window.MFUResources['multifileuploader.extensionnotallowed'], extension, allowedExtensions.replace(/;/g, ', ')));
                return;
            }

            // Check maximum files count
            if (maxNumberToUpload && (filesCount > maxNumberToUpload)) {
                alert(window.MFUResources['multifileuploader.maxnumbertoupload']);
                return;
            }

            // Check max size of single file
            if (maximumUploadSize && (getMaxFileSize() > maximumUploadSize)) {
                alert(formatString(window.MFUResources['multifileuploader.maxuploadsize'], maximumUploadSize));
                return;
            }

            // Check maximum total size of all uploaded files
            if (maximumTotalUpload && (totalUploadSize > maximumTotalUpload)) {
                alert(formatString(window.MFUResources['multifileuploader.maxuploadamount'], maximumTotalUpload));
                return;
            }

            if (onUploadBegin) {
                executeFunctionByName(onUploadBegin, window, containerId);
            }

            uploadFile(files[0]);
        };

        // Executes javascript function with given name
        var executeFunctionByName = function (functionName, context) {
            var args = [].slice.call(arguments).splice(2),
                namespaces = functionName.split("."),
                func = namespaces.pop();

            for (var i = 0; i < namespaces.length; i++) {
                context = context[namespaces[i]];
            }

            return context[func].apply(this, args);
        };

        // Formats string with parameters
        var formatString = function () {
            var theString = arguments[0];

            for (var i = 1; i < arguments.length; i++) {
                var regEx = new RegExp("\\{" + (i - 1) + "\\}", "gm");
                theString = theString.replace(regEx, arguments[i]);
            }

            return theString;
        };
    };

    return MultiFileUploader;
});
