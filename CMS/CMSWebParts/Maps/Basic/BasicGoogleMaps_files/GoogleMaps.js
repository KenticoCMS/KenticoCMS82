function addGoogleMarker(map, latitude, longitude, title, content, zoom, iconURL) {
    // Initialize point coordinates
    var point = new google.maps.LatLng(latitude, longitude);
    // Create Marker object
    var marker = new google.maps.Marker({
        map: map,
        position: point,
        title: title,
        icon: iconURL
    });
    // Create info window object
    var wname = new google.maps.InfoWindow({
        content: content
    });
    // Register click event
    google.maps.event.addListener(marker, 'click', function() {
        map.setCenter(point);
        var oZoom = map.getZoom();
        if (oZoom != zoom) { map.setZoom(zoom); }
        wname.open(map, marker);
    });
}   