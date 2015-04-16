using System;
using System.Collections.Generic;
using System.Data;

using CMS.PortalControls;
using CMS.Helpers;
using CMS.Base;

public partial class CMSWebParts_Localization_languageselectionwithflags : CMSAbstractLanguageWebPart
{
    #region "Variables"
    
    private string mLayoutSeparator = " ";
    private string imgFlagIcon = String.Empty;
    public string selectionClass = String.Empty;

    #endregion


    #region "Public properties"

    /// <summary>
    /// Gets or sets the display layout.
    /// </summary>
    public string DisplayLayout
    {
        get
        {
            return ValidationHelper.GetString(GetValue("DisplayLayout"), "");
        }
        set
        {
            SetValue("DisplayLayout", value);
            mLayoutSeparator = value.ToLowerCSafe() == "vertical" ? "<br />" : " ";
        }
    }


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
    /// Gets or sets the separator between items.
    /// </summary>
    public string Separator
    {
        get
        {
            return ValidationHelper.GetString(GetValue("Separator"), "");
        }
        set
        {
            SetValue("Separator", value);
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
    /// Reloads data for partial caching.
    /// </summary>
    public override void ReloadData()
    {
        base.ReloadData();
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
            List<string[]> cultures = GetCultures();
            if ((cultures != null) && ((cultures.Count > 1) || (HideCurrentCulture && (cultures.Count > 0))))
            {
                // Render the cultures
                ltlHyperlinks.Text = String.Empty;
                mLayoutSeparator = DisplayLayout.ToLowerCSafe() == "vertical" ? "<br />" : " ";
                bool addSeparator = false;

                // Loop thru all cultures
                foreach (string[] data in cultures)
                {
                    string url = data[0];
                    string code = data[1];
                    string name = data[2];

                    // Get flag icon URL
                    imgFlagIcon = UIHelper.GetFlagIconUrl(Page, code, "16x16");

                    if (addSeparator)
                    {
                        ltlHyperlinks.Text += Separator + mLayoutSeparator;
                    }

                    if (ShowCultureNames)
                    {
                        // Add flag icon before the link text
                        ltlHyperlinks.Text += "<img src=\"" + imgFlagIcon + "\" alt=\"" + HTMLHelper.HTMLEncode(name) + "\" />";
                        ltlHyperlinks.Text += "<a href=\"" + HTMLHelper.EncodeForHtmlAttribute(URLHelper.ResolveUrl(url)) + "\">";
                        ltlHyperlinks.Text += HTMLHelper.HTMLEncode(name);

                        // Set surrounding div css class
                        selectionClass = "languageSelectionWithCultures";
                    }
                    else
                    {
                        ltlHyperlinks.Text += "<a href=\"" + url + "\">" + "<img src=\"" + imgFlagIcon + "\" alt=\"" + HTMLHelper.HTMLEncode(name) + "\" />";

                        // Set surrounding div css class
                        selectionClass = "languageSelection";
                    }


                    ltlHyperlinks.Text += "</a>";
                    addSeparator = true;

                }
            }
            // Hide webpart if there isn't more than one culture
            else
            {
                Visible = false;
            }

            if (string.IsNullOrEmpty(selectionClass))
            {
                ltrDivOpen.Text = "<div>";
            }
            else
            {
                ltrDivOpen.Text = "<div class=\"" + selectionClass + "\">";
            }

            ltrDivClose.Text = "</div>";

            // Check if RTL hack must be applied
            if (CultureHelper.IsPreferredCultureRTL())
            {
                ltrDivOpen.Text += "<span style=\"visibility:hidden;\">a</span>";
            }
        }
    }

    #endregion
}