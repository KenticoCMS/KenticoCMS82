define(['domReady!', 'jQuery'], function (document, $) {

    var google = window.google,
        mapObject,

        // There is no simple way to access map object directly,
        // because the object is a global object and its name
        // is the map div wrapper's IDentifier        
        loadMapObject = function () {
            var mapObjectIdentifier = $('.js-map-wrapper')
                .children('div')
                .not('div[class^="Webpart"]')
                .attr('id');

            mapObject = window[mapObjectIdentifier];
        },

        scrollToLocation = function (location) {
            $("html, body").animate({ scrollTop: $(".map-title").offset().top }, 400);
            var geocoder = new google.maps.Geocoder();
            geocoder.geocode({ 'address': location }, scrollToMapPosition);
        },

        scrollToMapPosition = function (results, status) {
            if (status == google.maps.GeocoderStatus.OK) {
                mapObject.setCenter(new google.maps.LatLng(results[0].geometry.location.lat(), results[0].geometry.location.lng()));
                mapObject.setZoom(16);
            }
        };       


    $('.js-scroll-to-map').each(function (elementPosition, element) {
        var $element = $(element);

        $element.on('click', (function ($el) {
            return function () {
                if (!mapObject) {
                    loadMapObject();
                }
                var elAddress = $el.data().address;
                scrollToLocation(elAddress);
            }
        }($element)));
    });
});