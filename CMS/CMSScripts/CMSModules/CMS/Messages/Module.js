cmsdefine([
    'angular',
    'CMS/Messages/MessagesPlaceholder',
    'CMS/Messages/MessageHub'
], function (
    angular,
    message,
    messageHub
    ) {

    var moduleName = 'cms.messages';
    angular.module(moduleName, [])
           .directive('cmsMessagesPlaceholder', message)
           .service('messageHub', messageHub);

    return moduleName;
})