cmsdefine(['angular', "CMS/StringFormatter"], function (angular, stringFormatter) {

    return function () {

        var moduleName = 'CMS/Filters.StringFormat',
            module = angular.module(moduleName, []);

        module.filter('stringFormat', function() {
            return stringFormatter.format;
        });

        return moduleName;
    };
})