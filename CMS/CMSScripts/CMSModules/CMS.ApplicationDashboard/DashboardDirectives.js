cmsdefine(["require", "exports", 'angular', 'CMS.ApplicationDashboard/Directives/TileDirective', 'CMS.ApplicationDashboard/Directives/WelcomeTileDirective', 'CMS.ApplicationDashboard/Directives/TileIconDirective'], function(cmsrequire, exports, angular, tileDirective, welcomeTileDirective, tileIconDirective) {
    var ModuleName = 'cms.dashboard.directives';

    angular.module(ModuleName, []).directive('tile', tileDirective.Directive).directive('welcometile', welcomeTileDirective.Directive).directive('tileIcon', tileIconDirective);

    exports.Module = ModuleName;
});
