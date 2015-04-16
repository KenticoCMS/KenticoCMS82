cmsdefine(['CMS/Application'], function (application) {
    return ['$resource', 'cmsContactInterceptor', function ($resource, contactInterceptor) {
        return $resource(application.getData('applicationUrl') + 'cmsapi/ContactImport/', {}, {
            'import': {
                method: 'POST',
                interceptor: contactInterceptor
            }
        });
    }];
});
