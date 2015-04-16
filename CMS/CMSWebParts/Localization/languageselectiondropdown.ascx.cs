using System;
using System.Collections.Generic;
using System.Text;

using CMS.Helpers;
using CMS.Localization;
using CMS.PortalControls;
using CMS.SiteProvider;

public partial class CMSWebParts_Localization_languageselectiondropdown : CMSAbstractLanguageWebPart
{
    #region "Public properties"

    /// <summary>
    /// Gets or sets the value than indicates whether culture names are displayed.
    /// </summary>
    public bool ShowCultureNames
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ShowCultureNames"), true);
        }
        set
        {
            SetValue("ShowCultureNames", value);
        }
    }


    /// <summary>
    /// Gets or sets the value than indicates whether the control is shown.
    /// </summary>
    public bool HideIfOneCulture
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("HideIfOneCulture"), true);
        }
        set
        {
            SetValue("HideIfOneCulture", value);
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// OnPreRender event handler.
    /// </summary>
    /// <param name="e">Event argument</param>
    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);
        SetupControl();
    }


    /// <summary>
    /// Initializes the control properties.
    /// </summary>
    protected void SetupControl()
    {
        if (StopProcessing)
        {
            // Do nothing
        }
        else
        {
            // If there is only one culture on site and hiding is enabled hide webpart
            if (HideIfOneCulture && !CultureSiteInfoProvider.IsSiteMultilingual(SiteContext.CurrentSiteName))
            {
                Visible = false;
                return;
            }

            // Get list of cultures
            List<string[]> cultures = GetCultures();

            // Check whether exists more than one culture
            if ((cultures != null) && ((cultures.Count > 1) || (HideCurrentCulture && (cultures.Count > 0))))
            {

                // Add CSS Stylesheet
                CSSHelper.RegisterCSSLink(Page, URLHelper.ResolveUrl("~/CMSWebparts/Localization/languageselectiondropdown_files/langselector.css"));

                string imgFlagIcon = String.Empty;

                StringBuilder result = new StringBuilder();
                result.Append("<ul class=\"langselector\">");

                // Set first item to the current language
                CultureInfo ci = CultureInfoProvider.GetCultureInfo(CultureHelper.GetPreferredCulture());
                if (ci != null)
                {
                    // Drop down imitating icon
                    string dropIcon = ResolveUrl("~/CMSWebparts/Localization/languageselectiondropdown_files/dd_arrow.gif");

                    // Current language
                    imgFlagIcon = GetImageUrl("Flags/16x16/" + HTMLHelper.HTMLEncode(ci.CultureCode) + ".png");

                    string currentCultureShortName = String.Empty;
                    if (ShowCultureNames)
                    {
                        currentCultureShortName = HTMLHelper.HTMLEncode(ci.CultureShortName);
                    }

                    result.AppendFormat("<li class=\"lifirst\" style=\"background-image:url('{0}'); background-repeat: no-repeat\"><a class=\"first\" style=\"background-image:url({1}); background-repeat: no-repeat\" href=\"{2}\">{3}</a>",
                                        dropIcon, imgFlagIcon, "#", currentCultureShortName);
                }

                result.Append("<ul>");

                 // Loop thru all cultures
                foreach (string[] data in cultures)
                {
                    string url = data[0];
                    string code = data[1];
                    string name = HTMLHelper.HTMLEncode(data[2]);

                    // Language icon
                    imgFlagIcon = GetImageUrl("Flags/16x16/" + HTMLHelper.HTMLEncode(code) + ".png");
                    if (!ShowCultureNames)
                    {
                        name = string.Empty;
                    }

                    result.AppendFormat("<li><a style=\"background-image:url({0}); background-repeat: no-repeat\" href=\"{1}\">{2}</a></li>\r\n",
                                                imgFlagIcon, HTMLHelper.EncodeForHtmlAttribute(URLHelper.ResolveUrl(url)), name);
                }

                result.Append("</ul></li></ul>");
                ltlLanguages.Text = result.ToString();

            }
            else if (HideIfOneCulture)
            {
                Visible = false;
            }
        }
    }


    /// <summary>
    /// Reloads data for partial caching.
    /// </summary>
    public override void ReloadData()
    {
        base.ReloadData();
        SetupControl();
    }

    #endregion
}