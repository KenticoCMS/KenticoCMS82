using System;
using CMS.Helpers;
using CMS.PortalControls;
using CMS.PortalEngine;
using CMS.Base;
using CMS.UIControls;

[HashValidation(HashValidationSalts.REDIRECT_PAGE)]
public partial class CMSMessages_Redirect : MessagePage
{
    #region "Lifecycle events"

    /// <summary>
    /// OnInit event.
    /// </summary>
    protected override void OnPreInit(EventArgs e)
    {
        string url = QueryHelper.GetText("url", String.Empty);     
        CheckHashValidationAttribute = !(url.StartsWithAny(StringComparison.InvariantCulture, "~", "/"));

        base.OnPreInit(e);
    }


    /// <summary>
    /// Page load event.
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {       
        titleElem.TitleText = GetString("Redirect.Header");
        lblInfo.Text = GetString("Redirect.Info");

        string url = QueryHelper.GetText("url", String.Empty);
        string target = QueryHelper.GetText("target", String.Empty);
        string frame = QueryHelper.GetText("frame", String.Empty);
        
        // Change view mode to live site
        bool liveSite = QueryHelper.GetBoolean("livesite", false);
        if (liveSite)
        {
            PortalContext.ViewMode = ViewModeEnum.LiveSite;
        }

        bool urlIsRelative = url.StartsWithCSafe("~/");

        string script = String.Empty;
        url = ResolveUrl(url);

        // Information about the target page
        lnkTarget.Text = url;
        lnkTarget.NavigateUrl = url;
        lnkTarget.Target = target;

        string redirectUrlString = ScriptHelper.GetString(url);

        // Generate redirect script
        if (urlIsRelative && frame.EqualsCSafe("top", true))
        {
            script += "if (self.location != top.location) { top.location = " + redirectUrlString + "; } else { document.location = " + redirectUrlString + " }";
        }
        else if ((target == String.Empty) && (url != String.Empty))
        {
            script += "if (IsCMSDesk()) { window.open(" + redirectUrlString + "); } else { document.location = " + redirectUrlString + "; }";
        }

        ltlScript.Text += ScriptHelper.GetScript(script);       
    }

    #endregion
}