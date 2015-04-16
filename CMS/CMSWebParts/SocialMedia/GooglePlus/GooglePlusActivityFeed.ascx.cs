using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CMS.SiteProvider;
using CMS.SocialMedia;
using CMS.Helpers;
using CMS.Base;
using CMS.UIControls;
using CMS.PortalEngine;
using CMS.DataEngine;
using CMS.Membership;


public partial class CMSWebParts_SocialMedia_GooglePlus_GooglePlusActivityFeed : SocialMediaAbstractWebPart
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
    /// ID of the user or page (can be profile URL).
    /// </summary>
    public string FeedID
    {
        get
        {
            return HTMLHelper.StripTags(ValidationHelper.GetString(GetValue("FeedID"), string.Empty));
        }
        set
        {
            SetValue("FeedID", value);
        }
    }


    /// <summary>
    /// Width of the web part in pixels.
    /// </summary>
    public int Width
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("Width"), 0);
        }
        set
        {
            SetValue("Width", value);
        }
    }


    /// <summary>
    /// Height of the web part in pixels.
    /// </summary>
    public int Height
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("Height"), 0);
        }
        set
        {
            SetValue("Height", value);
        }
    }


    /// <summary>
    /// Maximal number of activities to display.
    /// </summary>
    public int NumberOfActivities
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("NumberOfActivities"), 5);
        }
        set
        {
            SetValue("NumberOfActivities", value);
        }
    }


    /// <summary>
    /// Whether to show scrollbar.
    /// </summary>
    public bool Scrollbar
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("Scrollbar"), false);
        }
        set
        {
            SetValue("Scrollbar", value);
        }
    }


    /// <summary>
    /// Shell background color.
    /// </summary>
    public string ShellBackgroundColor
    {
        get
        {
            return ValidationHelper.GetString(GetValue("ShellBackgroundColor"), string.Empty);
        }
        set
        {
            SetValue("ShellBackgroundColor", value);
        }
    }


    /// <summary>
    /// Shell text color.
    /// </summary>
    public string ShellTextColor
    {
        get
        {
            return ValidationHelper.GetString(GetValue("ShellTextColor"), string.Empty);
        }
        set
        {
            SetValue("ShellTextColor", value);
        }
    }


    /// <summary>
    /// Activity text color.
    /// </summary>
    public string ActivityTextColor
    {
        get
        {
            return ValidationHelper.GetString(GetValue("ActivityTextColor"), string.Empty);
        }
        set
        {
            SetValue("ActivityTextColor", value);
        }
    }


    /// <summary>
    /// Activity footer text color.
    /// </summary>
    public string ActivityFooterTextColor
    {
        get
        {
            return ValidationHelper.GetString(GetValue("ActivityFooterTextColor"), string.Empty);
        }
        set
        {
            SetValue("ActivityFooterTextColor", value);
        }
    }


    /// <summary>
    /// Border color.
    /// </summary>
    public string BorderColor
    {
        get
        {
            return ValidationHelper.GetString(GetValue("BorderColor"), string.Empty);
        }
        set
        {
            SetValue("BorderColor", value);
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
            string apiKey = SettingsKeyInfoProvider.GetValue(SiteContext.CurrentSiteName + ".CMSGooglePlusClientID");
            string apiSecret = SettingsKeyInfoProvider.GetValue(SiteContext.CurrentSiteName + ".CMSGooglePlusClientSecret");
            string accessToken = SettingsKeyInfoProvider.GetValue(SiteContext.CurrentSiteName + ".CMSGooglePlusAccessToken");

            // Check Social networking DLL and settings
            if (!SystemContext.IsFullTrustLevel)
            {
                // Error label is displayed in Design mode when Dll is renamed
                ShowDesignErrorOrHide(GetString("socialnetworking.fulltrustrequired"));
            }
            else if (String.IsNullOrEmpty(apiKey) || String.IsNullOrEmpty(apiSecret) || String.IsNullOrEmpty(accessToken))
            {
                // Error label is displayed in Design mode when missing settings
                string pathToSettings = SocialMediaHelper.GetPathToGooglePlusSettings();
                ShowDesignErrorOrHide(String.Format(GetString("socialnetworking.googleplus.allsettingsmissing"), pathToSettings));
            }
            else
            {
                // Process ID - it can be URL
                string showID = ExtractUserID(FeedID);

                GooglePlusPerson profile = null;

                // Fetch data from Google+
                try
                {
                    // Try to get person directly
                    profile = GooglePlusProvider.GetProfileInfo(showID);
                }
                catch
                {
                    try
                    {
                        // Try alternative approach (search for person)
                        profile = GooglePlusProvider.FindPerson(showID);
                    }
                    catch
                    {
                        ShowDesignErrorOrHide(String.Format(GetString("sm.googleplus.nopeoplefound"), showID));
                    }
                }


                if (profile != null)
                {
                    showID = profile.UserID;
                    List<GooglePlusActivity> activities = GooglePlusProvider.GetActivities(showID, NumberOfActivities);
                    // Build code for plugin
                    StringBuilder sb = new StringBuilder();
                    sb.Append("<div class=\"gpaf-plugin\"><div class=\"gpaf-doc\" style=\"width: ");
                    sb.Append(Width);
                    sb.Append("px; height: ");
                    sb.Append(Height);
                    sb.Append("px; border: 1px solid ");
                    sb.Append(BorderColor);
                    sb.Append("; position: relative; background-color: ");
                    sb.Append(ShellBackgroundColor);
                    sb.Append("\"><div style=\"margin: 15px;\">");

                    // Build plugin header
                    sb.Append("<div class=\"gpaf-header\">");

                    sb.Append("<table><tr><td style=\"vertical-align: center;\">");
                    sb.Append("<a href=\"");
                    sb.Append(profile.URL);
                    sb.Append("\">");
                    sb.Append("<img src=\"");
                    string imageURL = profile.ImageURL.Remove(profile.ImageURL.Length - 2) + "40";
                    sb.Append(imageURL);
                    sb.Append("\" alt=\"");
                    sb.Append(profile.DisplayName);
                    sb.Append("\" style=\"float: left; border: none;\" />");
                    sb.Append("</a>");
                    sb.Append("</td>");

                    sb.Append("<td style=\"vertical-align: center; padding-left: 10px;\">");
                    sb.Append("<a href=\"");
                    sb.Append(profile.URL);
                    sb.Append("\" style=\"text-decoration: none; font-size: 25px; color: ");
                    sb.Append(ShellTextColor);
                    sb.Append(";\">");
                    sb.Append(profile.DisplayName);
                    sb.Append("</a>");
                    sb.Append("</td></tr></table>");

                    sb.Append("</div>");

                    // Insert section separator
                    sb.Append("<div class=\"gpaf-doc-separator\" style=\"height: 15px; margin-top: 15px; width: 100%; border-top: 1px solid ");
                    sb.Append(BorderColor);
                    sb.Append(";\"></div>");

                    // Build plugin body
                    sb.Append("<div class=\"gpaf-body\" style=\"height: ");
                    sb.Append(Height - 164);
                    sb.Append("px; overflow: ");
                    sb.Append((Scrollbar) ? "auto" : "hidden");
                    sb.Append(";\">");

                    int counting = 1;
                    foreach (GooglePlusActivity s in activities)
                    {
                        sb.Append("<div class=\"gpaf-activity\" style=\"color: ");
                        sb.Append(ActivityTextColor);
                        sb.Append("\">");
                        sb.Append("<div class=\"gpaf-activity-text\">");
                        sb.Append(s.Content);
                        sb.Append("</div>");
                        sb.Append("<div class=\"gpaf-activity-footer\" style=\"color: ");
                        sb.Append(ActivityFooterTextColor);
                        sb.Append("; font-size: 13px; margin-top: 3px;\">");
                        sb.Append(s.Published);
                        sb.Append("</div>");
                        sb.Append("</div>");

                        // Insert activity separator
                        if (counting < activities.Count)
                        {
                            sb.Append("<div class=\"gpaf-body-separator\" style=\"height: 10px; margin-top: 5px; width: 100%; border-top: 1px dashed ");
                            sb.Append(BorderColor);
                            sb.Append(";\"></div>");
                            counting++;
                        }
                    }
                    sb.Append("</div></div>");

                    // Insert section separator
                    sb.Append("<div style=\"margin: 15px; position: absolute; bottom: 0px; left: 0px; width: ");
                    sb.Append(Width - 30);
                    sb.Append("px;\">");
                    sb.Append("<div class=\"gpaf-doc-separator\" style=\"height: 15px; margin-top: 15px; width: 100%; border-top: 1px solid ");
                    sb.Append(BorderColor);
                    sb.Append(";\"></div>");

                    // Build plugin footer
                    sb.Append("<div class=\"gpaf-footer\">");
                    sb.Append("<a href=\"https://plus.google.com\">");
                    sb.Append("<img src=\"https://ssl.gstatic.com/images/icons/gplus-32.png\" alt=\"\" style=\"float: left; border: none;\" />");
                    sb.Append("</a>");

                    sb.Append("<div class=\"gpaf-footer-text\" style=\"text-align: left; margin-left: 50px;\">");
                    sb.Append("<table style=\"height: 32px;\"><tr><td style=\"vertical-align: center;\">");
                    sb.Append("<a href=\"");
                    sb.Append(profile.URL);
                    sb.Append("\" style=\"text-decoration: none; font-size: 12px; color: ");
                    sb.Append(ShellTextColor);
                    sb.Append(";\">");
                    sb.Append(ResHelper.GetString("socialnetworking.viewfullprofile"));
                    sb.Append("</a>");
                    sb.Append("</td></tr></table>");
                    sb.Append("</div>");

                    sb.Append("<div style=\"clear: both;\"></div>");
                    sb.Append("</div>");

                    // Close open divs and finish plugin
                    sb.Append("</div></div></div>");
                    ltlPluginCode.Text = sb.ToString();
                }
                else
                {
                    ShowDesignErrorOrHide(String.Format(GetString("sm.googleplus.nopeoplefound"), showID));
                }
            }
        }
    }


    /// <summary>
    /// Displays the provided error message if viewmode is non-LiveSite or hides the webpart if on LiveSite
    /// </summary>
    /// <param name="message">The error message.</param>
    private void ShowDesignErrorOrHide(string message)
    {
        if (!ViewMode.IsLiveSite())
        {
            // Append a linebreak because of WP menu.
            lblError.Text = "<br />" + message;
            lblError.Visible = true;
        }
        else
        {
            Visible = false;
        }
    }


    /// <summary>
    /// Extracts user ID (or page ID) if input string is valid Google+ profile URL.
    /// </summary>
    /// <param name="input">Input - user ID or URL that contains user ID.</param>
    private string ExtractUserID(string input)
    {
        string output = input;
        int indexEnd = input.LastIndexOfCSafe("/posts");
        int indexStart = 0;

        // If input string is valid Google+ profile URL
        if (indexEnd > 0)
        {
            output = output.Remove(indexEnd);
            indexStart = output.LastIndexOfCSafe("/");
            output = output.Substring(indexStart + 1);
        }

        return output;
    }

    #endregion
}