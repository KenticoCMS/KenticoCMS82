var enableSearchLogo = false;
var infoHeight = 126;
var infoWidth = 256;
function addBingMarker(map, latitude, longtitude, title, content, iconURL) {
    var offset = new MM.Point(0, 5);
    if (iconURL) {
        var pushpinOptions = { icon: iconURL, width: 30, height: 50, textOffset: offset };
    }
    else {
        var pushpinOptions = { textOffset: offset };
    }
    var pushpin = new MM.Pushpin(new Microsoft.Maps.Location(latitude, longtitude), pushpinOptions);
    pushpin.description = content;
    map.entities.push(pushpin);
    pushpin.title = title;
    return pushpin;
}
function showInfo(map, pushpin, zoom) {
    var location = pushpin.getLocation();
    var infoboxOptions = { width: infoWidth, height: infoHeight, title: pushpin.title, description: pushpin.description, offset: new MM.Point(0, pushpin.getHeight()) };
    var infobox = new MM.Infobox(location, infoboxOptions);
    map.setView({ zoom: zoom, center: new MM.Location(location.latitude, location.longitude) });
    map.entities.push(infobox);
}
function customKeyDown(e) {
    e.handled = true;
}
function callBingService(url) {
    var script = document.createElement("script");
    script.setAttribute("type", "text/javascript");
    script.setAttribute("src", url);
    document.body.appendChild(script);
}
function replaceContent(className, expression, replacement) {
    var selHTMLTags = new Array();
    var selHTMLTags = document.getElementsByClassName(className);
    for (i = 0; i < selHTMLTags.length; i++) {
        selHTMLTags[i].innerHTML = selHTMLTags[i].innerHTML.replace(expression, replacement);
    }
}