cmsdefine(['CMS/EventHub'], function (EventHub) {

    return function () {
        var ERROR_EVENT_KEY = "MessageHubErrorEvent",

            /**
             * Publishes given error
             * @param  message  string  message to be published.
             * @param description string description for the error message. Might be long text.
             */
            publishError = function (message, description) {
                EventHub.publish(ERROR_EVENT_KEY, message, description);
            },

            /**
             * Subscribes to all errors
             * @param  callback  function callback with a parameter of an error that was published.
             */
            subscribeToError = function (callback) {
                EventHub.subscribe(ERROR_EVENT_KEY, callback);
            };

        return {
            publishError: publishError,
            subscribeToError: subscribeToError
        };
    };
})
