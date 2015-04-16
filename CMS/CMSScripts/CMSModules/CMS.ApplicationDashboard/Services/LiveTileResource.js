cmsdefine(["require", "exports", 'CMS/Application'], function(cmsrequire, exports, application) {
    exports.Resource = [
        '$resource',
        function ($resource) {
            return $resource(application.getData('applicationUrl') + 'cmsapi/LiveTile/', {}, {});
        }
    ];
});
