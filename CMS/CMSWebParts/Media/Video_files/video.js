function LoadVideo(elementId, videoUrl, width, height, showControls, autostart, loop, contentText) {
    
    // Create WMV player
    var content = "<div class=\"VideoLikeContent\" style=\"position:relative;z-index:0;\" >" +
    "<object id=\"" + elementId + "_video\" classid=\"CLSID:22D6f312-B0F6-11D0-94AB-0080C74C7E95\" width=\"" + width + "\" height=\"" + height + "\" type=\"video/x-ms-wmv\" standby=\Loading Windows Media Player components...\" codebase=\"http://activex.microsoft.com/activex/controls/mplayer/en/nsmp2inf.cab#Version=6,4,7,1112\" >" +
    "<param name=\"filename\" value=\"" + videoUrl + "\"/>" +
    "<param name=\"src\" value=\"" + videoUrl + "\"/>" +
    "<param name=\"animationatStart\" value=\"1\"/>" +
    "<param name=\"windowlessvideo\" value=\"1\"/>" +
    "<param name=\"wmode\" value=\"transparent\"/>" +
    "<param name=\"transparentatStart\" value=\"1\" />" +
    "<param name=\"autostart\" value=\"" + autostart + "\"/>" +
    "<param name=\"showControls\" value=\"" + showControls + "\"/>" +
    "<param name=\"loop\" value=\"" + loop + "\"/>" +
    "<object type=\"application/x-mplayer2\" src=\"" + videoUrl +
        "\" name=\"" + elementId + "_video\" width=\"" + width + "\" height=\"" + height +
        "\" autostart=\"" + autostart + "\" wmode=\"transparent\">" +
    contentText + "</object>" +
    "</object></div>";

    var element = document.getElementById(elementId);
    element.innerHTML = content;
}