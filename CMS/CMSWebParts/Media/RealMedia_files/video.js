function LoadRealMedia(elementId, videoUrl, width, height, showControls, autostart, loop, controlsHeight, contentText)
{
    var content = '<div class="Video"><object classid="clsid:CFCDAA03-8BE4-11cf-B84B-0020AFBBCCFA" width="' + width + '" height="' + height + '" >' +
                  '<param name="src" value="' + videoUrl + '" />' +
                  '<param name="autostart" value="' + autostart + '" />' +
                  '<param name="controls" value="ImageWindow">' +
                  '<param name="logo" value="false" />' +
                  '<param name="console" value="one">' +
                  '<param name="wmode" value="transparent" />' +
                  '<param name="loop" value="' + loop + '" />'+
                  '<!--[if !IE]>-->' +
                  '<embed height="' + height + '" loop="' + loop + '" wmode="transparent" src="' + videoUrl + '" type="audio/x-pn-realaudio-plugin" width="' + width + '" controls="ImageWindow" autostart="' + autostart + '" console="one" logo="false" />' +
                  '<!--<![endif]-->'
                  + contentText;
                  
       content += '</object></div>';
    
    if(showControls=='true')
    {
        content += '<div class="Controls"><object classid="clsid:CFCDAA03-8BE4-11CF-B84B-0020AFBBCCFA" width="' + width + '" height="' + controlsHeight + '" >' +
                      '<param name="src" value="' + videoUrl + '" />' +
                      '<param name="autostart" value="' + autostart + '" />' +
                      '<param name="controls" value="ControlPanel">' +
                      '<param name="logo" value="false" />' +
                      '<param name="console" value="one">' +
                      '<param name="wmode" value="transparent" />' +
                      '<param name="loop" value="' + loop + '" />'+
                      '<!--[if !IE]>-->' +
                      '<embed height="' + controlsHeight + '" loop="' + loop + '" wmode="transparent" src="' + videoUrl + '" type="audio/x-pn-realaudio-plugin" width="' + width + '" controls="ControlPanel" autostart="' + autostart + '" console="one" logo="false" />' +
                      '<!--<![endif]-->' +
                      contentText;
                      
         content += '</object></div>';
    }

    var element = document.getElementById(elementId);
    element.innerHTML = content;
}