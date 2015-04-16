function LoadFlash(elementId, flashUrl, width, height, allowFullscreen, quality, scale, autoplay, loop, contentText, additionalParams)
{
    var content = '<object type="application/x-shockwave-flash" width="' + width + '" height="' + height + '" allowFullScreen="true" data="' + flashUrl + '">\n' +
                '<param name="classid" value="clsid:D27CDB6E-AE6D-11cf-96B8-444553540000" />\n' +
                '<param name="codebase" value="http://download.macromedia.com/pub/shockwave/cabs/flash/swflash.cab#version=6,0,40,0" />\n' +
                '<param name="movie" value="' + flashUrl + '" />\n' +
                '<param name="quality" value="' + quality + '" />\n' +
                '<param name="scale" value="' + scale + '" />\n' +
                '<param name="allowFullScreen" value="' + allowFullscreen + '" />\n' +
                '<param name="play" value="' + autoplay + '" />\n' +
                '<param name="loop" value="' + loop + '" />\n' +
                '<param name="pluginurl" value="http://www.adobe.com/go/getflashplayer" />\n' +
                '<param name="wmode" value="transparent"/>\n' +
                additionalParams +
                contentText +
                '</object>';

    var element = document.getElementById(elementId);
    element.innerHTML = content;
}