cmsdefine(['angular'], function (angular) {

    return function (resourceStrings) {

        var moduleName = 'CMS/Filters.Resolve',
            module = angular.module(moduleName, []);

        module.filter('resolve', function () {
            return function (resourceStringKey) {
                return resourceStrings[resourceStringKey] || resourceStringKey;
            }
        });

        return moduleName;
    };
})