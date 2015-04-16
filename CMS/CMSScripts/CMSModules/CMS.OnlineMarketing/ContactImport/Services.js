cmsdefine([
    'angular',
    'CMS.OnlineMarketing/ContactImport/Services/ContactResource'],
    function (
        angular,
        contactResource) {

        var moduleName = 'cms.onlinemarketing.contactimport.services';
        
        angular.module(moduleName, [])
            .factory('cmsContactResource', contactResource);

        return moduleName;
    });
