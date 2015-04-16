cmsdefine([
    'Underscore',
    'CMS.OnlineMarketing/ContactImport/FileStreamer',
    'CMS.OnlineMarketing/ContactImport/CSVParser'], function (
        _,
        FileStreamer,
        CSVParser) {

        var Controller = function ($scope, $element, $timeout, resourceFilter, messageHub) {
            this.$scope = $scope;
            this.$fileInput = $element.children('.js-file-input');
            this.fileReference = null;
            this.$timeout = $timeout;
            this.PREVIEW_ROW_COUNT = 5;
            this.CSVParser = CSVParser;
            this.getString = resourceFilter;
            this.messageHub = messageHub;

            // All mime types that can be considered to be valid CSV source
            this.acceptableCSVtypes = [
                'text/csv',
                'text/plain',
                'application/csv',
                'text/comma-separated-values',
                'application/excel',
                'application/vnd.ms-excel',
                'application/vnd.msexcel',
                'text/anytext',
                'application/octet-stream',
                'application/txt'
            ];

            this.$scope.onClick = this.elementClick.bind(this);
            this.$fileInput.on('change', this.fileInputChanged.bind(this));
        },
            directive = function () {
                return {
                    restrict: 'A',
                    scope: {
                        fileStream: '=',
                    },
                    templateUrl: 'fileUploadTemplate.html',
                    controller: [
                        '$scope',
                        '$element',
                        '$timeout',
                        'resolveFilter',
                        'messageHub',
                        Controller],
                };
            };


        /**
         * Raise click event on the hidden HTML file input.
         */
        Controller.prototype.elementClick = function () {
            this.$fileInput.click();
        };


        /**
         * Apply the change to the current scope.
         */
        Controller.prototype.fileInputChanged = function () {
            var that = this;
            this.$timeout(function () {
                that.fileSelected();
            });
        };


        /**
         * Checks whether the file is in valid format and sends event message to parent controller.
         * Creates new instance of the FileStreamer for the given file.
         */
        Controller.prototype.fileSelected = function () {
            this.fileReference = null;

            if (!this.ensureMimeType()) {
                this.badInputFile(this.getString('om.contact.importcsv.badmimetype'));
                return;
            }

            this.$scope.$emit('fileSelected');
            this.$scope.fileStream = new FileStreamer(this.fileReference);
            this.fileStreamLoaded();
        };


        /**
         * Shows the given message and set value of the file input to null.
         * @param  {string}  message  Message to be displayed on error.
         */
        Controller.prototype.badInputFile = function(message) {
            this.messageHub.publishError(message);

            // Set file input value to null to be able to get notified when the same file is selected again.
            this.$fileInput.val(null);
            this.$scope.$apply();
        }


        /**
         * Ensures given file is in valid format to be considered as CSV file. 
         * Acceptable mime type formats are presented in acceptableCSVtypes field.
         * @return True, if file has valid MIME type; otherwise, false.
         */
        Controller.prototype.ensureMimeType = function () {
            return _.contains(this.acceptableCSVtypes, this.getFileReference().type);
        };


        /**
         * Gets file reference from the HTML file input.
         * @return FileReference usable for further processing with File API.
         */
        Controller.prototype.getFileReference = function () {
            if (this.fileReference) {
                return this.fileReference;
            }

            this.fileReference = this.$fileInput[0].files[0];
            return this.fileReference;
        };


        /**
         * Uses CSV parser, parse first n (PREVIEW_ROW_COUNT) rows of given file streamer and emits CSV columns with example data and file streamer to controller. 
         */
        Controller.prototype.fileStreamLoaded = function () {
            var that = this;

            this.$scope.fileStream.read(function (buffer, handle) {
                var result,
                    lines;

                handle.reset();

                if (buffer === '') {
                    that.badInputFile(that.getString('om.contact.importcsv.emptyfile'));
                    return;
                }

                try
                {
                    result = that.CSVParser.findValidCSVInBuffer(buffer, handle.finished);
                }
                catch (e) {
                    that.badInputFile(that.getString('om.contact.importcsv.badfileformat'));
                    return;
                }

                lines = _.first(result.rows, that.PREVIEW_ROW_COUNT);
                lines = that.CSVParser.filterEmptyRecords(lines);
             
                if (that.CSVParser.checkLineLength(lines)) {
                    // Emitting to the parent controller, can use scope events.
                    that.$scope.$emit('firstNRowsLoaded', {
                        parsedLines: lines,
                        fileStream: that.$scope.fileStream
                    });
                } else {
                    that.badInputFile(that.getString('om.contact.importcsv.firstrowslengthdoesnotmatch'));
                }
            });
        };

        return [directive];
    });