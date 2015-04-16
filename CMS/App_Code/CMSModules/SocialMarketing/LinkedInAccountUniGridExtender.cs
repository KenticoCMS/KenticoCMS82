using System;

using CMS;
using CMS.Base;
using CMS.ExtendedControls;
using CMS.Globalization;
using CMS.Helpers;
using CMS.Membership;
using CMS.SiteProvider;
using CMS.SocialMarketing;
using CMS.UIControls;

[assembly: RegisterCustomClass("LinkedInAccountUniGridExtender", typeof(LinkedInAccountUniGridExtender))]

/// <summary>
/// Extends LinkedIn accounts Unigrid with additional abilities.
/// </summary>
public class LinkedInAccountUniGridExtender : ControlExtender<UniGrid>
{
    #region "Life-cycle methods"

    /// <summary>
    /// Initializes the extender.
    /// </summary>
    public override void OnInit()
    {
        DisplayWarningIfNoDefaultAccount();
        Control.OnExternalDataBound += Control_OnExternalDataBound;
    }


    /// <summary>
    /// External data binding event handler.
    /// </summary>
    /// <param name="sender">Sender object</param>
    /// <param name="sourceName">External source name</param>
    /// <param name="parameter">Source parameter</param>
    private object Control_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        switch (sourceName)
        {
            case "LinkedInAccountAccessTokenExpiration":
                return GetTokenExpiration(parameter);
        }

        return parameter;
    }

    #endregion


    #region "Private methods"

    /// <summary>
    /// Displays warning if there is no default account set on the site.
    /// </summary>
    private void DisplayWarningIfNoDefaultAccount()
    {
        if (LinkedInAccountInfoProvider.GetDefaultLinkedInAccount(SiteContext.CurrentSiteID) == null)
        {
            Control.ShowWarning(ResHelper.GetString("sm.linkedin.nodefaultprofile"));
        }
    }


    /// <summary>
    /// Gets information about token expiration.
    /// </summary>
    /// <param name="parameter">DateTime with token expiration.</param>
    /// <returns>String with information about token expiration.</returns>
    private string GetTokenExpiration(object parameter)
    {
        DateTime expiration = ValidationHelper.GetDateTime(parameter, DateTimeHelper.ZERO_TIME);
        if (expiration == DateTimeHelper.ZERO_TIME)
        {
            return String.Empty;
        }
        TimeSpan delta = expiration - DateTime.Now;
        if (delta < TimeSpan.FromDays(7))
        {
            return GetWarning(FormatDateTime(expiration));
        }
        return FormatDateTime(expiration);
    }


    /// <summary>
    /// Gets warning from given text.
    /// </summary>
    /// <param name="text">Warning content.</param>
    /// <returns>Given text formated as warning.</returns>
    private string GetWarning(string text)
    {
        return String.Format("<span class='Red'>{0}</span>", HTMLHelper.HTMLEncode(text));
    }


    /// <summary>
    /// Gets string with formated date time.
    /// </summary>
    /// <param name="value">Date time to be formated.</param>
    /// <returns>String with formated date time.</returns>
    private string FormatDateTime(DateTime value)
    {
        return TimeZoneHelper.ConvertToUserTimeZone(value, true, MembershipContext.AuthenticatedUser, SiteContext.CurrentSite);
    }

    #endregion
}