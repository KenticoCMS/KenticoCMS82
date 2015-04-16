using System;
using System.Text.RegularExpressions;

using CMS.Helpers;
using CMS.PortalControls;
using CMS.PortalEngine;

public partial class CMSWebParts_General_SearchEngineResultsHighlighter : CMSAbstractWebPart
{
    #region "Public properties"

    /// <summary>
    /// Gets or sets the class name of span which wrap the highlighted words.
    /// </summary>
    public string CSSClassName
    {
        get
        {
            return ValidationHelper.GetString(GetValue("CSSClassName"), string.Empty);
        }
        set
        {
            SetValue("CSSClassName", value);
        }
    }

    #endregion


    #region "Page events"

    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        if (StopProcessing)
        {
            // Do nothing
        }
        else
        {
            if (PortalContext.ViewMode.IsOneOf(ViewModeEnum.LiveSite, ViewModeEnum.Preview))
            {
                Uri referrer = Request.UrlReferrer;

                // Regular expression for match and extraxt host name
                Regex domainRegex = RegexHelper.GetRegex("(www.)?(google|yahoo|bing).[a-zA-Z]{2,3}");

                if (referrer != null)
                {
                    if (domainRegex.Match(referrer.Host).Success)
                    {
                        bool isRegularExpression = false;

                        // Host name without www or domain
                        string host = domainRegex.Match(referrer.Host).Groups[2].Value;

                        // If host name is yahoo - uses other query parameter name
                        string highlightText = URLHelper.GetUrlParameter(referrer.Query, (host == "yahoo") ? "p" : "q");

                        if (highlightText != null)
                        {
                            // Replace + with blank space, if text has more than one word
                            if (highlightText.Contains("+"))
                            {
                                // Search concrete phrase
                                if (highlightText.Contains("%22"))
                                {
                                    highlightText = highlightText.Replace("+", " ");
                                }
                                else
                                {
                                    isRegularExpression = true;
                                    highlightText = highlightText.Replace("+", "|");
                                }
                            }

                            // In case that somebody will search exact phrase
                            if (highlightText.Contains("%22"))
                            {
                                highlightText = highlightText.Replace("%22", string.Empty);
                            }
                        }

                        // Register highlighter script
                        ScriptHelper.RegisterJQueryHighLighter(Page);
                        ScriptHelper.RegisterStartupScript(Page, typeof(String), "highlighter" + ClientID,
                                                           ScriptHelper.GetScript(
                                                               "$cmsj(function(){$cmsj('body').highlight(" +
                                                               ScriptHelper.GetString(highlightText) + ", " +
                                                               ScriptHelper.GetString(CSSClassName) + ", " +
                                                               ScriptHelper.GetString(isRegularExpression.ToString()) +
                                                               ")})"));
                    }
                }
            }
        }
    }

    #endregion
}