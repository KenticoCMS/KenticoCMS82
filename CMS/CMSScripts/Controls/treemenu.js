// 
var lastClassName = new String('');
var lastClassNameItem = new String('');

// Last class in use for CSS classes
var lastClassStyled = '';

// Set style
function TreeMenuOver(id, mStyle, browser, type, imgId, leftImgUrl, mainImgUrl, rightImgUrl)
{
    if (browser == 'ie')
    {
        // Create virtual stylesheet if nothing exists
        if (document.styleSheets.length == 0)
        {
            document.createStyleSheet('extrastyleTree.css'); 
        }
     
       // addRule show error if style is ''
       if (mStyle == '' )
       {
          mStyle= ';';
       }
    
        // Add rule to stylesheet 
        document.styleSheets[0].addRule(".CmsTreeMenuIeHackClass_"+id+'_'+type, mStyle);
        
        // Get objects elements
        var elemTr = document.getElementById(id); 
        var elemTrItem = document.getElementById(id+'_item'); 
        
        // Remember original Classes
        if (type != 1)
        {
          lastClassName = elemTr.className;
          lastClassNameItem = elemTrItem.className;
          
          if (lastClassName.indexOf('CmsTreeMenuIeHackClass_') > 0)
          {
            lastClassName = lastClassName.substring(0,lastClassName.indexOf('CmsTreeMenuIeHackClass_')-1);
          }
          
          if (lastClassNameItem.indexOf('CmsTreeMenuIeHackClass_') > 0)
          {
            lastClassNameItem = lastClassNameItem.substring(0,lastClassNameItem.indexOf('CmsTreeMenuIeHackClass_')-1);
          }
          
        }
    
        // Set '<td>' object style
        if (elemTr != null)
        {
            if (type != 1)
            {
                elemTr.removeAttribute("style");
                lastClassStyled = '';
                elemTr.className  = lastClassName +' CmsTreeMenuIeHackClass_'+id+'_'+type;
            }
            else
            {
                elemTr.className = lastClassName + ' CmsTreeMenuIeHackClass_'+ id+'_'+type;
                lastClassStyled = ' CmsTreeMenuIeHackClass_'+ id+'_'+type+';'+id;
            }
        }
    
        // Set '<a>' object style
        if (elemTrItem != null)
        {
            if (type != 1)
            {
                elemTrItem.removeAttribute("style");
                lastClassStyled = '';
                elemTrItem.className  =  ' CmsTreeMenuIeHackClass_'+id+'_'+type;
            }
            else    
            {
                elemTrItem.className = lastClassNameItem + ' CmsTreeMenuIeHackClass_'+id+'_'+type;
                lastClassStyled = ' CmsTreeMenuIeHackClass_'+ id+'_'+type+';'+id;;
            }
        }
        
    }
    else // Non IE browser (easy...)
    {
        var elemTr = document.getElementById(id); 
        var elemTrItem = document.getElementById(id+'_item'); 
        if (elemTr != null)
        {
            elemTr.attributes['style'].value = mStyle;
        }
    
        if (elemTrItem != null)
        {
            elemTrItem.attributes['style'].value = mStyle;
        }
        
    }
    
    // Images
    TreeMenuOverImages(imgId, leftImgUrl, mainImgUrl, rightImgUrl);
    
}

// Classes
function TreeMenuOverClass(id, mClass, browser, type, imgId, leftImgUrl, mainImgUrl, rightImgUrl)
{
    if (browser == 'ie')
    {
        var elemTr = document.getElementById(id); 
        var elemTrItem = document.getElementById(id+'_item'); 
        if (elemTr != null)
        {
            elemTr.className = mClass;
            if (type == 1)
            {
                elemTr.className = elemTr.className + lastClassStyled;
                
            }
        }
    
        if (elemTrItem != null)
        {
            var ar = lastClassStyled.split(';');
            if ((ar != null)&&(ar.length == 2))
            {
                if ((type == 1)&&(ar[1] == id))
                {
                    elemTrItem.className = mClass;
                    elemTrItem.className = elemTrItem.className + ar[0];
                    alert(elemTrItem.className);
                }
            }
        }
        
    }
    else // Non IE browser (easy...)
    {
        var elemTr = document.getElementById(id); 
        var elemTrItem = document.getElementById(id+'_item'); 
        if (elemTr != null)
        {
            elemTr.attributes['class'].value = mClass;
        }
    
        if (elemTrItem != null)
        {
            //elemTrItem.attributes['class'].value = mClass;
        }
        
    }
    
    // Images
    TreeMenuOverImages(imgId, leftImgUrl, mainImgUrl, rightImgUrl);
    
}

// Images
function TreeMenuOverImages(imgId, leftImgUrl, mainImgUrl, rightImgUrl)
{

    var elemLeftImg = document.getElementById(imgId+'_IMGLEFT'); 
    if (elemLeftImg != null)
    {
        if (leftImgUrl != '') {
            elemLeftImg.attributes['src'].value = leftImgUrl;
        }
    }

    var elemMainImg = document.getElementById(imgId+'_IMGMAIN'); 
    if (elemMainImg != null)
    {
        if (mainImgUrl != '') {
            elemMainImg.attributes['src'].value = mainImgUrl;
        }
    }
    
    var elemRightImg = document.getElementById(imgId+'_IMGRIGHT'); 
    if (elemRightImg != null)
    {
        if (rightImgUrl != '') {
            elemRightImg.attributes['src'].value = rightImgUrl;
        }
    }
}

// on click
function TreeMenuOnClick(tabId,imgId,imgUrl, openUrl)
{
	var obj = document.getElementById(tabId);
	var img = document.getElementById(imgId);
	if (obj != null)
	{
		if (obj.style.display == 'none')
		{
			obj.style.display = 'block';
			if (img != null)
			{
			    img.src = openUrl;
			}
		}
		else
		{
			obj.style.display = 'none';
			if (img != null)
			{
			    img.src = imgUrl;
			}
		}
	}
}