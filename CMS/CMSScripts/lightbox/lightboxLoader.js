var CacheArrayObject = new Array();
var mtmpobj = document.getElementById('aspxoutput');
var currentKey = '';

var lgb_images = 0;
var lgb_imagesProcessed = false;

function loadLightBoxContent(content) {

    // Reset image counter
    lgb_images = 0;
    // Reset image flag
    lgb_imagesProcessed = false;

    mtmpobj = document.getElementById('aspxoutput');
    mtmpobj.innerHTML = content.replace(/[\n]/g, '');
    CacheArrayObject[currentKey] = content;
    currentKey = '';
    mtmpobj.style.visibility = 'hidden';
    mtmpobj.style.display = '';
    mtmpobj.style.opacity = '0';

    windowWidth = 0;
    windowHeight = 0;

    if (loadDelay == 0) {
        setwidhei();
    }

    if ((windowWidth == 0 || windowHeight == 0)) {
        setTimeout("TryShow();", loadDelay);
    }
    else {
        Element.hide('aspxoutput');
        mtmpobj.style.visibility = '';
        myLightbox.resizeImageContainer(windowWidth, windowHeight);
        return false;
    }
}

function beforeLightBoxLoad(mpath, key) {
    currentKey = '';
    if (CacheArrayObject[key] != null) {
        mtmpobj.innerHTML = CacheArrayObject[key];
        mtmpobj.style.visibility = 'hidden';
        mtmpobj.style.display = '';
        mtmpobj.style.opacity = '0';

        windowWidth = 0;
        windowHeight = 0;

        if (loadDelay == 0) {
            setwidhei();
        }

        if ((windowWidth == 0 || windowHeight == 0)) {
            setTimeout("TryShow();", loadDelay);
            return false;
        }
        else {
            Element.hide('aspxoutput');
            mtmpobj.style.visibility = '';
            myLightbox.resizeImageContainer(windowWidth, windowHeight);
            return false;
        }
    }

    Element.hide('aspxoutput');
    currentKey = key;
    return true;
}

function getchildelem(elem) {
    if (elem.hasChildNodes()) {
        for (i = 0; i < elem.childNodes.length; i++) {
            var childNode = elem.childNodes[i];
            // Skip comments etc.
            if ((childNode != null) && childNode.nodeName[0] != '#') {
                return childNode;
            }
        }
    }
    return null;
}

function setwidhei() {
    var childelem = getchildelem(mtmpobj);
    if (childelem != null) {
        windowWidth = childelem.offsetWidth;
        windowHeight = childelem.offsetHeight;
    }
    else {
        windowWidth = mtmpobj.offsetWidth;
        windowHeight = mtmpobj.offsetHeight;
    }

    if ((windowWidth == 0 || windowHeight == 0)) {
        var childelem = getchildelem(mtmpobj);
        if (childelem != null) {
            windowWidth = childelem.clientWidth;
            windowHeight = childelem.clientHeight;
        }
        else {
            windowWidth = mtmpobj.clientWidth;
            windowHeight = mtmpobj.clientHeight;
        }
    }

    if (predefWidth != 0) { windowWidth = predefWidth; }
    if (predefHeight != 0) { windowHeight = predefHeight; }
}


function TryShow() {
    // Try get width and height
    setwidhei();
    // If width and height isn't initialized try it again later
    if ((windowWidth == 0 || windowHeight == 0)) {
        setTimeout("TryShow();", loadDelay);
    }
    else {
        // Check whether inner image processing was called for current item
        if (!lgb_imagesProcessed) {
            // Set current image flag
            lgb_imagesProcessed = true;
            // Get all sub-images
            var imageTags = mtmpobj.getElementsByTagName("IMG");
            // Loop thru all sub images and create loading object
            for (var i = 0; i < imageTags.length; i++) {
                // Get original image url
                var src = imageTags[i].getAttribute("src");
                // Increment number of loaded images
                lgb_images = lgb_images + 1;
                // Create new image element
                var image = document.createElement('img');
                // Setup OnLoad and OnError events
                image.onload = lgb_imageLoader;
                image.onerror = lgb_imageLoaderError;
                // Hide image on output
                image.style.display = 'none';
                // Set the original src to the loading image
                image.setAttribute('src', src);
                // Add image to the document structure
                document.body.appendChild(image);
            }
        }

        // If all images are loaded, display lightbox content
        if (lgb_images <= 0) {
            Element.hide('aspxoutput');
            mtmpobj.style.visibility = '';
            myLightbox.resizeImageContainer(windowWidth, windowHeight);
        }
        // otherwise check loading status again later
        else {
            setTimeout("TryShow();", loadDelay);
        }
    }
}

// OnError - decrement images counter
function lgb_imageLoaderError() {
    lgb_images = lgb_images - 1;
}

// If image is fully loaded, decrement image counter
function lgb_imageLoader() {

    lgb_images = lgb_images - 1;
}

