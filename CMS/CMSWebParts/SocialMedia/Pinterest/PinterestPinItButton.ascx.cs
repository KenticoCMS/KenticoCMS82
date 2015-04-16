using System;

using CMS.Helpers;
using CMS.SocialMedia;


public partial class CMSWebParts_SocialMedia_Pinterest_PinterestPinItButton : SocialMediaAbstractWebPart
{
    #region "Private fiels"

    private bool mHide;

    #endregion


    #region "Public properties"

    /// <summary>
    /// Indicates whether to hide content of the WebPart
    /// </summary>
    public override bool HideContent
    {
        get
        {
            return mHide;
        }
        set
        {
            mHide = value;
            ltlPluginCode.Visible = !value;
        }
    }


    /// <summary>
    /// URL.
    /// </summary>
    public string Url
    {
        get
        {
            return ValidationHelper.GetString(GetValue("Url"), string.Empty);
        }
        set
        {
            SetValue("Url", value);
        }
    }


    /// <summary>
    /// Image URL.
    /// </summary>
    public string ImageUrl
    {
        get
        {
            return ValidationHelper.GetString(GetValue("ImageUrl"), string.Empty);
        }
        set
        {
            SetValue("ImageUrl", value);
        }
    }


    /// <summary>
    /// Description.
    /// </summary>
    public string Description
    {
        get
        {
            return ValidationHelper.GetString(GetValue("Description"), string.Empty);
        }
        set
        {
            SetValue("Description", value);
        }
    }


    /// <summary>
    /// Defines if pin count should bwe displayed and where.
    /// </summary>
    public string PinCount
    {
        get
        {
            return ValidationHelper.GetString(GetValue("PinCount"), string.Empty);
        }
        set
        {
            SetValue("PinCount", value);
        }
    }

    #endregion


    #region "Methods"


    /// <summary>
    /// Initializes the control properties
    /// </summary>
    protected override void SetupControl()
    {
        if (this.StopProcessing)
        {
            // Do not process
        }
        else
        {
            // Register plugin javascript
            string script = @"
                (function() {
                    window.PinIt = window.PinIt || { loaded:false };
                    if (window.PinIt.loaded) return;
                    window.PinIt.loaded = true;
                    function async_load(){
                        var s = document.createElement(""script"");
                        s.type = ""text/javascript"";
                        s.async = true;
                        s.src = ""http://assets.pinterest.com/js/pinit.js"";
                        var x = document.getElementsByTagName(""script"")[0];
                        x.parentNode.insertBefore(s, x);
                    }
                    if (window.attachEvent)
                        window.attachEvent(""onload"", async_load);
                    else
                        window.addEventListener(""load"", async_load, false);
                })();";

            ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "Pinterest", ScriptHelper.GetScript(script));

            // If URL not set -> get current URL
            if (String.IsNullOrEmpty(Url))
            {
                Url = URLHelper.GetAbsoluteUrl(RequestContext.CurrentURL);
            }

            // Build plugin code
            string query = null;
            string src = "http://pinterest.com/pin/create/button/";

            // Set webpart parameters
            query = URLHelper.AddUrlParameter(query, "url", Url);
            query = URLHelper.AddUrlParameter(query, "media", URLHelper.GetAbsoluteUrl(ImageUrl));
            query = URLHelper.AddUrlParameter(query, "description", Description);

            src = HTMLHelper.EncodeForHtmlAttribute(URLHelper.AppendQuery(src, query));

            // Output HTML code
            string output = "<a href=\"{0}\" class=\"pin-it-button\" count-layout=\"{1}\">Pin It</a>";
            ltlPluginCode.Text = String.Format(output, src, HTMLHelper.EncodeForHtmlAttribute(PinCount));
        }
    }

    #endregion
}



