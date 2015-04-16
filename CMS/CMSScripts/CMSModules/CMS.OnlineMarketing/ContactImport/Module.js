cmsdefine([
    'angular',
    'angular-resource',
    'CMS.OnlineMarketing/ContactImport/Controller',
    'CMS.OnlineMarketing/ContactImport/Directives',
    'CMS.OnlineMarketing/ContactImport/Services',
    'CMS.OnlineMarketing/ContactImport/Interceptors',
    'CMS/Filters/Resolve',
    'CMS/Filters/StringFormat',
    'CMS/Messages/Module'
], function (
    angular,
    ngResource,
    controller,
    directives,
    services,
    interceptors,
    resolveFilter,
    stringFormatFilter,
    messageModule
) {
    
    return function(dataFromServer) {

        // Create ...
        var moduleName = 'cms.OnlineMarketing.ContactImport',
            module = angular.module(moduleName, [
                directives,
                services,
                interceptors,
                resolveFilter(dataFromServer.resourceStrings),
                stringFormatFilter(),
                'ngResource',
                messageModule
            ]);

        // Inject controllers
        module.controller('CMS.OnlineMarketing.ContactImport.Controller', controller(dataFromServer));

        return moduleName;
    };
})

