cmsdefine(['Underscore', 'CMS/EventHub', 'CMS/NavigationBlocker', 'CMS/Messages/MessageHub'], function (_, EventHub, NavigationBlocker, MessageHub) {

    return function (dataFromServer) {
        var controller = function ($scope, $timeout, resolveFilter) {
            var navigationBlocker = new NavigationBlocker();
        
            $scope.model = {};

            // Visibility
            $scope.model.attributeMapperIsVisible = false;
            $scope.model.importProcessIsVisible = false;
            $scope.model.fileUploaderIsVisible = true;

            if (!window.FileReader) {
                var messageHub = new MessageHub();
                messageHub.publishError(resolveFilter('om.contact.importcsv.notsupportedbrowser'));
                return;
            }

            $scope.model.contactFields = dataFromServer.contactFields;
            $scope.model.contactGroups = dataFromServer.contactGroups;
            $scope.model.siteGuid = dataFromServer.siteGuid;

            // When this model property is ready, mapping process starts
            $scope.model.fileStream = null;

            // When this model property is filled, the import process starts
            $scope.model.contactFieldsMapping = null;

            // Selected contact group
            $scope.model.selectedContactGroup = null;


            // First rows of CSV were successfully loaded, we can step on to the mapping
            $scope.$on('firstNRowsLoaded', function (e, data) {
                $timeout(function () {
                    $scope.model.fileUploaderIsVisible = false;
                    $scope.model.attributeMapperIsVisible = true;

                    if (data) {
                        $scope.model.fileStream = data.fileStream;
                        $scope.model.parsedLines = data.parsedLines;
                    }

                    // Ensure header shadow registration and message clearance
                    EventHub.publish('cms.angularViewLoaded');
                });
            });

            // Event fired by import directive
            $scope.$on('importStarted', function () {
                $scope.model.fileUploaderIsVisible = false;
                $scope.model.attributeMapperIsVisible = false;
                $scope.model.importProcessIsVisible = true;
                navigationBlocker.block(resolveFilter("om.contact.importcsv.confirmleave"));
                // Ensure header shadow registration and message clearance
                EventHub.publish('cms.angularViewLoaded');
            });

            $scope.$on('importFinished', function() {
                navigationBlocker.unblock();
            });
        };

        return ['$scope', '$timeout', 'resolveFilter', controller];
    };
});