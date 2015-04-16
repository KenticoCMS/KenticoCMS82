cmsdefine(["require", "exports", 'angular', 'CMS.ApplicationDashboard/Services/TileResource', 'CMS.ApplicationDashboard/Services/WelcomeTileResource', 'CMS.ApplicationDashboard/Services/LiveTileResource', 'CMS.ApplicationDashboard/Services/LiveTilePushService'], function(cmsrequire, exports, angular, tileResource, welcomeTileResource, liveTileResource, liveTilePushService) {
    var ModuleName = 'cms.dashboard.services';

    angular.module(ModuleName, []).factory('cms.resource.tile', tileResource.Resource).factory('cms.resource.welcomeTile', welcomeTileResource.Resource).factory('cms.resource.liveTile', liveTileResource.Resource).service('cms.service.liveTilePushService', liveTilePushService.Service);

    exports.Module = ModuleName;
});
