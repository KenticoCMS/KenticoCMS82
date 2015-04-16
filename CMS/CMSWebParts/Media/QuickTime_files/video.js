function LoadQuickTime(elementId, videoUrl, width, height, showControls, autostart, loop, contentText) {
    var content = '<object classid="clsid:02BF25D5-8C17-4B23-BC80-D3488ABDDC6B" codebase="http://www.apple.com/qtactivex/qtplugin.cab" width="' + width + '" height="' + height + '" >' +
                  '<param name="src" value="' + videoUrl + '" />' +
                  '<param name="controller" value="' + showControls + '" />' +
                  '<param name="autoplay" value="' + autostart + '" />' +
                  '<param name="wmode" value="transparent" />' +
                  '<param name="loop" value="' + loop + '" />' +
                  '<param name="scale" value="tofit" />' +
                  '<!--[if !IE]>-->' +
                  '<object type="video/quicktime" data="' + videoUrl + '" width="' + width + '" height="' + height + '" >' +
                  '<param name="autoplay" value="' + autostart + '" />' +
                  '<param name="controller" value="' + showControls + '" />' +
                  '<param name="loop" value="' + loop + '" />' +
                  '<param name="wmode" value="transparent" />' +
                  '<param name="scale" value="tofit" />' +
                  contentText +
                  '</object>' +
                  '<!--<![endif]-->';
    content += '</object>';

    var element = document.getElementById(elementId);
    element.innerHTML = content;
}