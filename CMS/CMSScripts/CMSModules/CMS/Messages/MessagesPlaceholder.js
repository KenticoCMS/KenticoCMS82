cmsdefine(['CMS/EventHub', 'CMS/Application'], function (eventHub, app) {

    var Controller = function ($scope, messageHub) {

        $scope.model = $scope.model || {};

        messageHub.subscribeToError(function (message, description) {
            $scope.model.message = message;
            $scope.model.description = description;
        });

        eventHub.subscribe('cms.angularViewLoaded', function() {
            $scope.model.message = null;
        });

        $scope.toggleDescription = function () {
            $scope.model.descriptionVisible = !$scope.model.descriptionVisible;
        };
    };

    var directive = function () {
        return {
            scope: {},
            templateUrl: app.getData('applicationUrl') + 'CMSScripts/CMSModules/CMS/Templates/MessagesPlaceholderTemplate.html',
            controller: ['$scope', 'messageHub', Controller]
        };
    };

    return [directive];
})