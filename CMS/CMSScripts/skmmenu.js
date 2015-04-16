//Region Global Variables
var skm_SelectedMenuStyleInfos=new Object();
var skm_UnselectedMenuStyleInfos=new Object();
var skm_MenuFadeDelays=new Object();
var skm_clockValue=0;
var skm_ticker;
var skm_highlightTopMenus=new Object();
var skm_images=new Array();
var skm_OpenMenuItems = new Array();
//var skm_previousMenu;
//EndRegion
//Region Methods to hook up a menu to the global variables
function skm_registerMenu(menuID, selectedStyleInfo, unselectedStyleInfo, menuFadeDelay, highlightTopMenu){
	skm_SelectedMenuStyleInfos[menuID]=selectedStyleInfo;
	skm_UnselectedMenuStyleInfos[menuID]=unselectedStyleInfo;
	skm_MenuFadeDelays[menuID]=menuFadeDelay;
	skm_highlightTopMenus[menuID]=highlightTopMenu;
}
//Region The methods and contructor of the skm_styleInfo object.
function skm_applyStyleInfoToElement(element){
	element.style.backgroundColor=this.backgroundColor;
	element.style.borderColor=this.borderColor;
	element.style.borderStyle=this.borderStyle;
	element.style.borderWidth=this.borderWidth;
	element.style.color=this.color;
	if (this.fontFamily!='')
		element.style.fontFamily=this.fontFamily;
	element.style.fontSize=this.fontSize;
	element.style.fontStyle=this.fontStyle;
	element.style.fontWeight=this.fontWeight;
	if (this.className!='')
		element.style.className=this.className;
}
function skm_styleInfo(backgroundColor,borderColor,borderStyle,borderWidth,color,fontFamily,fontSize,fontStyle,fontWeight,className){
	this.backgroundColor=backgroundColor;
	this.borderColor=borderColor;
	this.borderStyle=borderStyle;
	this.borderWidth=borderWidth;
	this.color=color;
	this.fontFamily=fontFamily;
	this.fontSize=fontSize;
	this.fontStyle=fontStyle;
	this.fontWeight=fontWeight;
	this.className=className;
	this.applyToElement=skm_applyStyleInfoToElement;
}

function getPropertyValueFromCss(className, propertyName)
{
    var toReturn = null;
    
	if (document.styleSheets) {	
    for (var i = 0; i < document.styleSheets.length; i++)
    {
        var cssRules = null;
        
        try {
            // IE
            if (document.styleSheets[i].rules) {
                cssRules = document.styleSheets[i].rules;
            }
            // Mozilla and others
            else if (document.styleSheets[i].cssRules) {
                cssRules = document.styleSheets[i].cssRules;
            }        
        }
        catch(err)
        {
        }
        
        if (cssRules != null) {
            for (var j = 0; j < cssRules.length; j++) {
                if (cssRules[j].selectorText == ("." + className) && cssRules[j].style[propertyName] != null)
                {
                    toReturn = cssRules[j].style[propertyName];
                }
            }
        }
    }
    }
    return toReturn;
}

//Region MouseEventHandlers
function skm_mousedOverMenu(menuID,elem,parent,displayedVertically,imageSource, leftImage, rightImage, overStyle, browser, uniqueId, rtl){
	skm_stopTick();
	skm_closeSubMenus(elem);
	

	
	var childID=elem.id+"-subMenu";  // Display child menu if needed
	if (document.getElementById(childID)!=null){  // make the child menu visible and specify that its position is specified in absolute coordinates

		child = document.getElementById(childID);
		if (child.style)
		{
			child = child.style;
		}	
		child.display='block';
		child.position='absolute';

		// get width of CMSMenu from css stylesheet
		var tableWidth = getPropertyValueFromCss(document.getElementById(childID).className, "width");
		if (tableWidth != null)
		{
			child.width = tableWidth;
		}

		skm_OpenMenuItems = skm_OpenMenuItems.concat(childID);
		if (displayedVertically){ // Set the child menu's left and top attributes according to the menu's offsets
			child.left=(skm_getAscendingLefts(parent)+parent.offsetWidth) + 'px';
			
			if (rtl)
			{
			    child.left=(skm_getAscendingLefts(parent)-document.getElementById(childID).offsetWidth) + 'px';
			}
			
			child.top=skm_getAscendingTops(elem) + 'px';
			
			var visibleWidth=parseInt(window.outerWidth?window.outerWidth-9:document.body.clientWidth,10);						
			if ( (parseInt(skm_getElementLeft(document.getElementById(childID)),10) + parseInt(document.getElementById(childID).offsetWidth,10))>visibleWidth) {				
				//document.getElementById(childID).style.backgroundColor='red';				
				var oldpos = document.getElementById(childID).offsetLeft;
				var width = parseInt(document.getElementById(childID).offsetWidth,10);
				var newpos = oldpos - parent.offsetWidth - width;
				child.left = newpos + 'px';
				/* due to incorrect movement when not defined width in css stylesheet, it is important to move new submenu
				(displayed on the left from actual submenu) UNDER actual submenu. Otherwise some items from actual submenu
				may be not visible. */
				child.zIndex = skm_getElemZIndex(parent) - 1;
			} else {
				child.zIndex = skm_getElemZIndex(parent) + 10;
			}
		}else{  // Set the child menu's left and top attributes according to the menu's offsets		
			child.left=skm_getAscendingLefts(elem) + 'px';	
			child.top=(skm_getAscendingTops(parent)+parent.offsetHeight) + 'px';
			if (document.getElementById(childID).offsetWidth<elem.offsetWidth)
			{
				child.width=elem.offsetWidth + 'px';			
		    }
		    else
		    {
		        if (rtl){
		        child.left = skm_getAscendingLefts(elem) - (document.getElementById(childID).offsetWidth - elem.offsetWidth) + 'px';
		        }
		    }
		}
	}
	if (skm_SelectedMenuStyleInfos[menuID] != null) skm_SelectedMenuStyleInfos[menuID].applyToElement(elem);
	if (skm_highlightTopMenus[menuID]){
		var eId=elem.id+'';
		while (eId.indexOf('-subMenu')>=0){
			eId=eId.substring(0, eId.lastIndexOf('-subMenu'));
			skm_SelectedMenuStyleInfos[menuID].applyToElement(document.getElementById(eId));
		}
	}	

	var index = 0;
	
	if (leftImage!='')
	{
	    var i=elem.getElementsByTagName("img")[index];
	    if (i != null)
	    {
	        i.src=leftImage;
	        index++;
	    }
	}
	
	
	if (imageSource!=''){
	    var i=elem.getElementsByTagName("img")[index];
	    if (i != null)
	    {
	        i.src=imageSource;
	        index++;
	        }
	}
	
	if (rightImage!='')
	{
	    var i=elem.getElementsByTagName("img")[index];
	    if (i != null)
	    {
	        i.src=rightImage;
	        index++;
	    }
	}
	
	if (overStyle != '')
	{
	    if (browser == 'IE') 
	    {

	        if (document.styleSheets.length == 0)
	        {
	            document.createStyleSheet('extrastyle.css');  
	        }
    	    document.styleSheets[0].addRule(".OverClass_skm_Menu_CMS" + uniqueId, overStyle);
	    }
	    else
	    {
	        if (elem.attributes['style'] != null)
	        {
	    	    elem.attributes['style'].value =  overStyle + elem.attributes['style'].value;		
    	    }
	        else
	        {
	            elem.attributes['style'].value =  overStyle;
	        }
    	}
	}
}

function skm_mousedOverClickToOpen(menuID,elem,parent,imageSource){
	skm_stopTick();
	if (skm_SelectedMenuStyleInfos[menuID] != null) skm_SelectedMenuStyleInfos[menuID].applyToElement(elem);
	if (skm_highlightTopMenus[menuID]){
		var eId=elem.id+'';
		while (eId.indexOf('-subMenu')>=0){
			eId=eId.substring(0, eId.lastIndexOf('-subMenu'));
			skm_SelectedMenuStyleInfos[menuID].applyToElement(document.getElementById(eId));
		}
	}	
	if (imageSource!=''){
		setimage(elem,imageSource);
	}
}

function skm_getElemZIndex(elem)
{
	if (elem == null)
	{
		return -1;
	} else if (elem.style.zIndex == undefined){
		return 100;
	} else {
		return elem.style.zIndex;
	}		
}

function skm_mousedOverSpacer(menuID,elem,parent){
	skm_stopTick();
}

function skm_mousedOutMenu(menuID,elem,imageSource,cssClass, defaultStyle, browser, uniqueId, leftImage, rightImage){
	if (document.getElementById('lastSelectedItemID_' + menuID) != null)
		{
		if (document.getElementById('lastSelectedItemID_' + menuID).value == elem.id)	
			{
			  return;
			}
		}
		
		
	skm_doTick(menuID);
	if (skm_UnselectedMenuStyleInfos[menuID] != null) skm_UnselectedMenuStyleInfos[menuID].applyToElement(elem);
	if (skm_highlightTopMenus[menuID]){
		var eId=elem.id+'';
		while (eId.indexOf('-subMenu')>=0){
			eId=eId.substring(0, eId.lastIndexOf('-subMenu'));
			skm_UnselectedMenuStyleInfos[menuID].applyToElement(document.getElementById(eId));
		}
	}
	
	settd(elem,cssClass);
    var index = 0;
	
	if (leftImage!='')
	{
	    var i=elem.getElementsByTagName("img")[index];
	    if (i != null)
	    {
	    i.src=leftImage;
	    index++;
	    }
	}
	
	if (imageSource!=''){
	    var i=elem.getElementsByTagName("img")[index];
	    if (i != null)
	    {
	        i.src=imageSource;
	        index++;
	    }
	}
	
	if (rightImage!='')
	{
	    var i=elem.getElementsByTagName("img")[index];
	    if (i != null)
	    {
	        i.src=rightImage;
	     index++;
	     }
	}
	
	
	if (defaultStyle != '')
	{
	    if (browser == 'IE')
	    {
	        if (uniqueId != '')
	        {
	            if (document.styleSheets.length == 0)
	            {
	                document.createStyleSheet('extrastyle.css');  
	            }
   	            document.styleSheets[0].addRule(".OverOut_skm_Menu_CMS_" + uniqueId, defaultStyle);
 	        }
        }
	    else
	    {
	            if (elem.attributes['style'] != null)
	            {
	                elem.attributes['style'].value =  defaultStyle + elem.attributes['style'].value;
	            }
	            else
	            { 
	                elem.attributes['style'].value =  defaultStyle;
	            }
    	}
}
}

function skm_mousedOutSpacer(menuID, elem){
	skm_doTick(menuID);
}

//Region Utility Functions
function skm_closeSubMenus(parent){
	if (skm_OpenMenuItems == "undefined") return;
	for (var i=skm_OpenMenuItems.length-1; i>-1;i--) {
		if (parent.id.indexOf(skm_OpenMenuItems[i]) != 0) {
			if (document.getElementById(skm_OpenMenuItems[i]) != null) {
				document.getElementById(skm_OpenMenuItems[i]).style.display = 'none';
				skm_shimSetVisibility(false, skm_OpenMenuItems[i]);			
				skm_OpenMenuItems = new Array().concat(skm_OpenMenuItems.slice(0,i), skm_OpenMenuItems.slice(i+1));
			}
  		} 
	}
}
function skm_shimSetVisibility(makevisible, tableid){
	var tblRef=document.getElementById(tableid);
	var IfrRef=document.getElementById('shim'+tableid);
	if (tblRef!=null && IfrRef!=null){
		if(makevisible){
			IfrRef.style.width=tblRef.offsetWidth;
			IfrRef.style.height=tblRef.offsetHeight;
			IfrRef.style.top=tblRef.style.top;
			IfrRef.style.left=tblRef.style.left;
			IfrRef.style.zIndex=tblRef.style.zIndex-1;
			IfrRef.style.display="block";
		}else{
			IfrRef.style.display="none";
		}
	}
}
function skm_IsSubMenu(id){
	if (skm_subMenuIDs == "undefined") return false;
	for (var i=0;i<skm_subMenuIDs.length;i++)
	  if (id==skm_subMenuIDs[i]) return true;
	return false;
}
function skm_getAscendingLefts(elem){
	if (elem==null)
		return 0;
	else
	{
		var elemPosition = getPropertyValueFromCss(elem.className,'position');
		if (!elemPosition) {
			elemPosition = elem.style.position;
		}		
		if ((elemPosition=='absolute' || elemPosition=='relative') && !skm_IsSubMenu(elem.id)) return 0;
		var x = elem.offsetLeft+skm_getAscendingLefts(elem.offsetParent);
		return x;
	}
}
function skm_getElementLeft(elem)
{
	if (elem==null)
	{
		return 0;
	} else {
		return elem.offsetLeft + skm_getElementLeft(elem.offsetParent);
	}	
}
function skm_getAscendingTops(elem){
	if (elem==null)
		return 0;
	else {	
		var elemPosition = getPropertyValueFromCss(elem.className,'position');
		if (!elemPosition) {
			elemPosition = elem.style.position;
		}
		var parent = skm_getAscendingTops(elem.offsetParent);		
		if ((elemPosition=='absolute' || elemPosition=='relative') && !skm_IsSubMenu(elem.id)) return 0;
		return elem.offsetTop + parent;
	}
}
//Region Fade Functions
function skm_doTick(menuID){
	if (skm_clockValue>=skm_MenuFadeDelays[menuID]){
		skm_stopTick();
		skm_closeSubMenus(document.getElementById(menuID));
	} else {
		skm_clockValue++;
		skm_ticker=setTimeout("skm_doTick('"+menuID+"');", 500);
	}
}
function skm_stopTick(){
	skm_clockValue=0;
	clearTimeout(skm_ticker);
}
function preloadimages(){
	for (i=0;i<preloadimages.arguments.length;i++){
		skm_images[i]=new Image();
		skm_images[i].src=preloadimages.arguments[i];
	}
}
function setimage(elem,imageSource){
	var i=elem.getElementsByTagName("img")[0];
	i.src=imageSource;
}
function settd(elem,styleName){
	elem.className=styleName;
		
}
function skm_selectNewItem(menuID, elem, imageSource, normalImageSource, className, normalClassName){
	if (document.getElementById('lastSelectedItemID_' + menuID).value != elem.id)
	{	  
	  //set original values
	  setimage(document.getElementById(document.getElementById('lastSelectedItemID_' + menuID).value), document.getElementById('lastSelectedItemNormalImageSource_' + menuID).value);			
	  settd(document.getElementById(document.getElementById('lastSelectedItemID_' + menuID).value), document.getElementById('lastSelectedItemNormalClassName_' + menuID).value);
	  //store values
	  document.getElementById('lastSelectedItemID_' + menuID).value = elem.id;
	  document.getElementById('lastSelectedItemNormalImageSource_' + menuID).value = normalImageSource;
	  document.getElementById('lastSelectedItemNormalClassName_' + menuID).value = normalClassName;
	  //set new values
	  setimage(elem, imageSource);				  
	  settd(elem, className);
	}
}
//-->
