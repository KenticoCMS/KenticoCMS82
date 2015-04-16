cmsdefine(['Underscore'], function (_) {

    var Controller = function($scope, messageHub, resolveFilter) {
            this.$scope = $scope;
            this.PREVIEW_ROW_COUNT = 5;
            this.messageHub = messageHub;
            this.getString = resolveFilter;

            // Initialize contact field mapping that will be distributed to subdirectives to fill it
            this.$scope.processingContactFieldsMapping = angular.copy(this.$scope.contactFields) || [];

            this.processContactFieldsMapping();

            this.$scope.onMappingFinished = this.onMappingFinished.bind(this);
            this.$scope.message = 'om.contact.importcsv.mapstepmessage';
                
            this.$scope.$watch('parsedLines', this.parsedLinesChanged.bind(this));
        },
        directive = function() {
            return {
                restrict: 'A',
                scope: {
                    // Inputs
                    fileStream: '=',
                    contactFields: '=',
                    // Output, when this gets filled, it means the mapping is finished
                    contactFieldsMapping: '=',
                    contactGroups: '=',
                    selectedContactGroup: '=',
                    parsedLines: '=',
                },
                templateUrl: 'attributeMappingTemplate.html',
                controller: [ '$scope', 'messageHub', 'resolveFilter', Controller ]
            };
        };


    /**
     * Iterates the contact fields collection and set mappedIndex to -1 for every contact field.
     */
    Controller.prototype.processContactFieldsMapping = function() {
        // Initialize contact field mapping that will be distributed to subdirectives to fill it
        this.$scope.processingContactFieldsMapping.forEach(function(category) {
            category.categoryMembers.forEach(function(field) {
                field.mappedIndex = -1;
            });
        });
    };


    /**
     * Removes contact groups from contact fields mapping.
     */
    Controller.prototype.onMappingFinished = function() {
        var contactFieldsWithoutCategories = this.getFieldsWithoutCategories(this.$scope.processingContactFieldsMapping),
            contactFieldsMapping = this.filterContactFieldsMapping(contactFieldsWithoutCategories);

        if (this.checkValidity(contactFieldsMapping)) {
            // Set the final contactFieldsMapping that's handled to the controller
            this.$scope.contactFieldsMapping = contactFieldsMapping;
        }
    };


    /**
     * Sets column names with example to scope from parsed lines.
     */
    Controller.prototype.parsedLinesChanged = function(newValue, oldValue) {
        if (newValue !== oldValue) {
            this.$scope.csvColumnNamesWithExamples = this.getCSVColumnNamesWithExamples(newValue);
        }
    };


    /**
     * From the given array of parsed lines creates new object suitable for displaying column names with their examples.
     * @param  parsedLines  string[]  array of input lines
     * @return object                 object suitable for displaying column names and the examples
     */
    Controller.prototype.getCSVColumnNamesWithExamples = function(parsedLines) {
        return _.first(parsedLines).map(function(data, index) {
            return {
                ColumnIndex: index,
                ColumnName: data,
                ColumnExamples: _.rest(parsedLines).map(function(restData) {
                    return restData[index];
                })
            };
        });
    };


    /**
     * Flatten given contact fields and return them without categories.
     * @param   contactFieldsMappingWithCategories  contactFieldMapping[]  collection of contact field mappings to be flattened.
     * @return  contactFieldMapping[]                                      collection of contact field mappings without categories
     */
    Controller.prototype.getFieldsWithoutCategories = function(contactFieldsMappingWithCategories) {
        return _.flatten(
            _.pluck(contactFieldsMappingWithCategories, 'categoryMembers')
        );
    };


    /**
     * Checks whether given collection of mapped contact fields is valid and can be pass to the importer. If not, binds proper error message.
     * @param   contactFieldsMapping  contactFieldMapping[]  collection of contact field mappings to be checked.
     * @return  boolean                                      true, if collection is valid; otherwise, false.
     */
    Controller.prototype.checkValidity = function(contactFieldsMapping) {
        var isValid = true;

        if (!this.checkEmailIsMapped(contactFieldsMapping)) {
            this.messageHub.publishError(this.getString('om.contact.importcsv.noemailmapping'));
            isValid = false;
        }

        return isValid;
    };


    /**
     * Checks whether given collection of mapped contact fields map also ContactEmail field. 
     * @param   contactFieldsMapping  contactFieldMapping[]  collection of contact field mappings to be checked.
     * @return  boolean                                      true, if collection contains ContactEmail field; otherwise, false.
     */
    Controller.prototype.checkEmailIsMapped = function(contactFieldsMapping) {
        if (contactFieldsMapping && contactFieldsMapping.length > 0) {
            return _.findWhere(contactFieldsMapping, { 'name': 'ContactEmail' });
        }
        return false;
    };


    /**
     * Checks whether each element in given collection has set mappedIndex. If not, the element is filtered out.
     * @param   contactFieldsMapping  contactFieldMapping[]  collection of contact field mappings to be checked.
     * @return  contactFieldMapping[]                        collection of filtered contact field mappings
     */
    Controller.prototype.filterContactFieldsMapping = function(contactFieldsMapping) {
        return contactFieldsMapping.filter(function(elem) {
            return elem.mappedIndex > -1;
        });
    };
    
    return [directive];
});