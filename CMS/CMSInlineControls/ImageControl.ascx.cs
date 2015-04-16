using System;
using System.Text;
using System.Web;
using System.Web.UI;

using CMS.ExtendedControls;
using CMS.Helpers;

public partial class CMSInlineControls_ImageControl : InlineUserControl
{
    #region "Properties"

    /// <summary>
    /// URL of the image media.
    /// </summary>
    public string URL
    {
        get
        {
            return ValidationHelper.GetString(GetValue("URL"), null);
        }
        set
        {
            SetValue("URL", value);
        }
    }


    /// <summary>
    /// Gets or sets the value which determines whether to append size parameters to URL ot not.
    /// </summary>
    public bool SizeToURL
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("SizeToURL"), true);
        }
        set
        {
            SetValue("SizeToURL", value);
        }
    }


    /// <summary>
    /// Image extension.
    /// </summary>
    public string Extension
    {
        get
        {
            return ValidationHelper.GetString(GetValue("Extension"), null);
        }
        set
        {
            SetValue("Extension", value);
        }
    }


    /// <summary>
    /// Image alternative text.
    /// </summary>
    public string Alt
    {
        get
        {
            return ValidationHelper.GetString(GetValue("Alt"), null);
        }
        set
        {
            SetValue("Alt", value);
        }
    }


    /// <summary>
    /// Image width.
    /// </summary>
    public int Width
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("Width"), -1);
        }
        set
        {
            SetValue("Width", value);
        }
    }


    /// <summary>
    /// Image height.
    /// </summary>
    public int Height
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("Height"), -1);
        }
        set
        {
            SetValue("Height", value);
        }
    }


    /// <summary>
    /// Image border width.
    /// </summary>
    public int BorderWidth
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("BorderWidth"), -1);
        }
        set
        {
            SetValue("BorderWidth", value);
        }
    }


    /// <summary>
    /// Image border color.
    /// </summary>
    public string BorderColor
    {
        get
        {
            return ValidationHelper.GetString(GetValue("BorderColor"), null);
        }
        set
        {
            SetValue("BorderColor", value);
        }
    }

    /// <summary>
    /// Image horizontal space.
    /// </summary>
    public int HSpace
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("HSpace"), -1);
        }
        set
        {
            SetValue("HSpace", value);
        }
    }


    /// <summary>
    /// Image vertical space.
    /// </summary>
    public int VSpace
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("VSpace"), -1);
        }
        set
        {
            SetValue("VSpace", value);
        }
    }


    /// <summary>
    /// Image align.
    /// </summary>
    public string Align
    {
        get
        {
            return ValidationHelper.GetString(GetValue("Align"), null);
        }
        set
        {
            SetValue("Align", value);
        }
    }


    /// <summary>
    /// Image ID.
    /// </summary>
    public string ImageID
    {
        get
        {
            return ValidationHelper.GetString(GetValue("Id"), null);
        }
        set
        {
            SetValue("Id", value);
        }
    }


    /// <summary>
    /// Image tooltip text.
    /// </summary>
    public string Tooltip
    {
        get
        {
            return ValidationHelper.GetString(GetValue("Tooltip"), null);
        }
        set
        {
            SetValue("Tooltip", value);
        }
    }


    /// <summary>
    /// Image css class.
    /// </summary>
    public string Class
    {
        get
        {
            return ValidationHelper.GetString(GetValue("Class"), null);
        }
        set
        {
            SetValue("Class", value);
        }
    }


    /// <summary>
    /// Image inline style.
    /// </summary>
    public string Style
    {
        get
        {
            return ValidationHelper.GetString(GetValue("Style"), null);
        }
        set
        {
            SetValue("Style", value);
        }
    }


    /// <summary>
    /// Image link destination.
    /// </summary>
    public string Link
    {
        get
        {
            return ValidationHelper.GetString(GetValue("Link"), null);
        }
        set
        {
            SetValue("Link", value);
        }
    }


    /// <summary>
    /// Image link target (_blank/_self/_parent/_top)
    /// </summary>
    public string Target
    {
        get
        {
            return ValidationHelper.GetString(GetValue("Target"), null);
        }
        set
        {
            SetValue("Target", value);
        }
    }


    /// <summary>
    /// Image behavior.
    /// </summary>
    public string Behavior
    {
        get
        {
            return ValidationHelper.GetString(GetValue("Behavior"), null);
        }
        set
        {
            SetValue("Behavior", value);
        }
    }


    /// <summary>
    /// Width of the thumbnail image which is displayed when mouse is moved over the image.
    /// </summary>
    public int MouseOverWidth
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("MouseOverWidth"), 0);
        }
        set
        {
            SetValue("MouseOverWidth", value);
        }
    }


    /// <summary>
    /// Height of the thumbnail image which is displayed when mouse is moved over the image.
    /// </summary>
    public int MouseOverHeight
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("MouseOverHeight"), 0);
        }
        set
        {
            SetValue("MouseOverHeight", value);
        }
    }


    /// <summary>
    /// Control parameter.
    /// </summary>
    public override string Parameter
    {
        get
        {
            return URL;
        }
        set
        {
            URL = value;
        }
    }

    #endregion


    #region "Page events"

    protected void Page_PreRender(object sender, EventArgs e)
    {
        if (Behavior == "hover")
        {
            StringBuilder sb = new StringBuilder();
            // If jQuery not loaded
            sb.AppendFormat(@"
if (typeof $cmsj == 'undefined') {{ 
    var jQueryCore=document.createElement('script');
    jQueryCore.setAttribute('type','text/javascript'); 
    jQueryCore.setAttribute('src', '{0}');
    setTimeout('document.body.appendChild(jQueryCore)',100); 
    setTimeout('loadTooltip()',200);
}}", ScriptHelper.GetScriptUrl("~/CMSScripts/jquery/jquery-core.js"));

            // If jQuery tooltip plugin not loaded
            sb.AppendFormat(@"
var jQueryTooltips=document.createElement('script'); 
function loadTooltip() {{ 
    if (typeof $cmsj == 'undefined') {{ setTimeout('loadTooltip()',200); return;}} 
    if (typeof $cmsj.fn.tooltip == 'undefined') {{ 
        jQueryTooltips.setAttribute('type','text/javascript'); 
        jQueryTooltips.setAttribute('src', '{0}'); 
        setTimeout('document.body.appendChild(jQueryTooltips)',100); 
    }}
}}", ScriptHelper.GetScriptUrl("~/CMSScripts/jquery/jquery-tooltips.js"));


            string rtlDefinition = null;
            if (((IsLiveSite) && (CultureHelper.IsPreferredCultureRTL())) || (CultureHelper.IsUICultureRTL()))
            {
                rtlDefinition = "positionLeft: true,left: -15,";
            }

            sb.AppendFormat(@"
function hover(imgID, width, height, sizeInUrl) {{ 
    if ((typeof $cmsj == 'undefined')||(typeof $cmsj.fn.tooltip == 'undefined')) {{
        var imgIDForTimeOut = imgID.replace(/\\/gi,""\\\\"").replace(/'/gi,""\\'"");
        setTimeout(""loadTooltip();hover('"" + imgIDForTimeOut + ""',"" + width + "",""  +height + "","" + sizeInUrl + "")"",100); return;
    }}
    $cmsj('img[id=' + imgID + ']').tooltip({{
        delay: 0,
        track: true,
        showBody: "" - "",
        showBody: "" - "", 
        extraClass: ""ImageExtraClass"",
        showURL: false, 
        {0}
        bodyHandler: function() {{
            // Apply additional style to main hover div if needed
            $cmsj('#tooltip').css({{'position' : 'absolute','z-index' : 5000}});
            var hidden = $cmsj(""#"" + imgID + ""_src"");
            var source = this.src;
            if (hidden[0] != null) {{
                source = hidden[0].value;
            }}
            var hoverDiv = $cmsj(""<div/>"");
            var hoverImg = $cmsj(""<img/>"").attr(""class"", ""ImageTooltip"").attr(""src"", source);
            hoverImg.css({{'width' : width, 'height' : height}});
            hoverDiv.append(hoverImg);
            return hoverDiv;
        }}
    }});
}}", rtlDefinition);

            ScriptHelper.RegisterStartupScript(Page, typeof(Page), "JQueryImagePreview", ScriptHelper.GetScript(sb.ToString()));
        }

        ImageParameters imgParams = new ImageParameters();
        if (!String.IsNullOrEmpty(URL))
        {
            imgParams.Url = ResolveUrl(URL);
        }
        imgParams.Align = Align;
        imgParams.Alt = Alt;
        imgParams.Behavior = Behavior;
        imgParams.BorderColor = BorderColor;
        imgParams.BorderWidth = BorderWidth;
        imgParams.Class = Class;
        imgParams.Extension = Extension;
        imgParams.Height = Height;
        imgParams.HSpace = HSpace;
        imgParams.Id = (String.IsNullOrEmpty(ImageID) ? Guid.NewGuid().ToString() : ImageID);
        imgParams.Link = Link;
        imgParams.MouseOverHeight = MouseOverHeight;
        imgParams.MouseOverWidth = MouseOverWidth;
        imgParams.SizeToURL = SizeToURL;
        imgParams.Style = Style;
        imgParams.Target = Target;
        imgParams.Tooltip = Tooltip;
        imgParams.VSpace = VSpace;
        imgParams.Width = Width;

        ltlImage.Text = MediaHelper.GetImage(imgParams);

        // Dynamic JQuery hover effect
        if (Behavior == "hover")
        {
            string imgId = HTMLHelper.HTMLEncode(HttpUtility.UrlDecode(imgParams.Id));
            string url = HttpUtility.HtmlDecode(URL);
            if (SizeToURL)
            {
                if (MouseOverWidth > 0)
                {
                    url = URLHelper.UpdateParameterInUrl(url, "width", MouseOverWidth.ToString());
                }
                if (MouseOverHeight > 0)
                {
                    url = URLHelper.UpdateParameterInUrl(url, "height", MouseOverHeight.ToString());
                }
                url = URLHelper.RemoveParameterFromUrl(url, "maxsidesize");
            }
            ltlImage.Text += "<input type=\"hidden\" id=\"" + imgId + "_src\" value=\"" + ResolveUrl(url) + "\" />";

            ScriptHelper.RegisterStartupScript(Page, typeof(Page), "ImageHover_" + imgId, ScriptHelper.GetScript("hover(" + ScriptHelper.GetString(imgId) + ", " + MouseOverWidth + ", " + MouseOverHeight + ", " + (SizeToURL ? "true" : "false") + ");"));
        }
    }

    #endregion
}