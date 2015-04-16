cmsdefine(['Underscore', 'CMS.OnlineMarketing/ContactImport/CSVParser', 'CMS/Messages/MessageHub'], function (_, CSVParser, MessageHub) {

    var Controller = function($scope, $timeout, $q, cmsContactResource, messageHub, resolveFilter) {
            var that = this;

            this.messageHub = new MessageHub();
            this.$scope = $scope;
            this.$timeout = $timeout;
            this.$q = $q;
            this.parser = CSVParser;
            this.messageHub = messageHub;
            this.getString = resolveFilter;

            this.$scope.result = {
                imported: 0,
                duplicities: 0,
                failures: 0,
                processed: 0,
            };

            this.$scope.isImporting = false;
            this.$scope.numberOfAllRecords = 0;
            this.$scope.finished = false;
            this.contactResource = cmsContactResource;

            this.$scope.importedTotalCount = 0;

            // If contact mapping has changed, run the importing process.
            this.$scope.$watch('contactFieldsMapping', function(contactFieldsMapping) {
                if (contactFieldsMapping) {
                    that.$scope.$emit('importStarted');
                    that.$scope.isImporting = true;
                    that.$scope.unfinishedRequests = 0;
                    that.importContacts(contactFieldsMapping, that.$scope.fileStream);
                }
            });

            messageHub.subscribeToError(function() {
                that._importFinished();
            });
        },
        directive = function() {
            return {
                restrict: 'A',
                scope: {
                    // Inputs
                    contactFieldsMapping: '=',
                    fileStream: '=',
                    contactGroup: '=',
                    siteGuid: '='
                },
                controller: ['$scope', '$timeout', '$q', 'cmsContactResource', 'messageHub', 'resolveFilter', Controller],
                templateUrl: 'importProcessTemplate.html'
            };
        };


    /**
     * Parse contacts according to the given mappings and send them to the server.
     * @param  contactFieldsMapping  object[]      collection of contact fields mapping set by the user
     * @param  fileStream            FileStreamer  instance of FileStreamer
     */
    Controller.prototype.importContacts = function (contactFieldsMapping, fileStream) {
        var that = this,
            names = contactFieldsMapping.map(function (elem) { return elem.name; }),
            selectedIndexes = contactFieldsMapping.map(function (elem) { return elem.mappedIndex; }),
            skipFirstRow = true,
            columnsCount = 0;

        fileStream.read(function (buffer, handle) {
            that.handle = handle;
            try {
                var result = that.parser.findValidCSVInBuffer(buffer, handle.finished),
                    filtered;

                handle.move(-result.remainder);

                if (skipFirstRow) {
                    columnsCount = result.rows[0].length;
                    result.rows.shift();
                }
                skipFirstRow = false;

                result.rows = that.parser.filterEmptyRecords(result.rows);
                that.parser.fixLineColumnsCount(result.rows, columnsCount);
                filtered = that.filterColumns(result.rows, selectedIndexes);

                that.sendBulkToServer(names, filtered);
                that.$scope.numberOfAllRecords += filtered.length;
            }
            catch (e) {
                that.$scope.finished = true;
                that.messageHub.publishError(that.getString('om.contact.importcsv.importerror') + ' ' + that.getString('om.contact.importcsv.invalidcsv'));
                that.$scope.$apply();
            }
        });
    };

    /**
     * Filters out all columns from the given input which index is not present in the given included columns array.
     * @param  input            string[[]]  input to be modified
     * @param  includedColumns  int[]    array of indexes which should be included from the parsed results
     * @return                  input without filtered columns
     */
    Controller.prototype.filterColumns = function (input, includedColumns) {
        return input.map(function (item) {
            var resultItem = [];
            includedColumns.forEach(function (index) {
                resultItem.push(item[index]);
            });
            return resultItem;
        });
    };


    /**
     * Called when the request is finished.
     */
    Controller.prototype.onRequestFinished = function () {
        if (this.handle) {
            if (this.handle.finished) {
                this._importFinished();
            }
            this.handle.continue();
        }
    };


    Controller.prototype._importFinished = function () {
        this.$scope.finished = true;
        this.$scope.$emit('importFinished');
    }


    /**
     * Performs sending of the contacts to the server.
     * @param  fieldNames    string[]         array of field names obtained from the CSV header
     * @param  contactLines  array[string[]]  array of arrays containing the data itself
     */
    Controller.prototype.sendBulkToServer = function (fieldNames, contactLines) {
        var that = this,
            importPromise = this.getImportPromise(fieldNames, contactLines);

        importPromise.then(function (data) {
            that.onImportBulkSuccess(data, contactLines.length);
        });
    };


    /**
     * Creates new import request to the server and returns the resource promise.
     * @param  fieldNames    string[]         array of field names obtained from the CSV header
     * @param  contactLines  array[string[]]  array of arrays containing the data itself
     * @return                                promise of the import request
     */
    Controller.prototype.getImportPromise = function (fieldNames, contactLines) {
        return this.contactResource.import({
            contactGroupGuid: this.$scope.contactGroup,
            siteGuid: this.$scope.siteGuid,
        }, {
            fieldsOrder: fieldNames,
            fieldValues: contactLines,
        }).$promise;
    };


    /**
     * Import request success handler. Sets number of results to the UI.
     * @param  data   object  result object returned from the server
     */
    Controller.prototype.onImportBulkSuccess = function (data, processed) {
        var that = this;
        if (data) {
            this.$timeout(function () {
                that.$scope.result.processed += processed;
                that.$scope.result.imported += data.imported;
                that.$scope.result.duplicities += data.duplicities;
                that.$scope.result.failures += data.failures;
                that.onRequestFinished();
            });
        }
    };

    return [directive];
});