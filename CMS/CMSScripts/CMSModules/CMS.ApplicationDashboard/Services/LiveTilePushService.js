cmsdefine(["require", "exports", 'angular', 'CMS/Application'], function(cmsrequire, exports, angular, application) {
    var Service = (function () {
        function Service($interval, $http) {
            var _this = this;
            this.$interval = $interval;
            this.$http = $http;
            /**
            * Polling interval in seconds.
            */
            this.POLLING_INTERVAL = 20;
            this.start = function (applicationGuid, newDataCallback, noDataCallback) {
                var oldData = null, getData = function () {
                    _this.$http.get(application.getData('applicationUrl') + 'cmsapi/LiveTile/', { params: { guid: applicationGuid } }).success(function (newLiveTileData) {
                        if (!newLiveTileData.HasData) {
                            oldData = null;
                            noDataCallback();
                            return;
                        }

                        var data = newLiveTileData.Data;

                        if (!angular.equals(oldData, data)) {
                            oldData = data;
                            newDataCallback(data);
                        }
                    }).error(function (data, status) {
                        if (status == 403) {
                            window.top.location.href = data.LogonPageUrl;
                        }
                        oldData = null;
                        noDataCallback();
                    });
                };

                getData();

                return _this.$interval(getData, _this.POLLING_INTERVAL * 1000);
            };
            this.stop = function (interval) {
                _this.$interval.cancel(interval);
            };
        }
        Service.$inject = [
            '$interval',
            '$http'
        ];
        return Service;
    })();
    exports.Service = Service;
});
