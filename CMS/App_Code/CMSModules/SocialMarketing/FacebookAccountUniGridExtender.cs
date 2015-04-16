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

[assembly: RegisterCustomClass("FacebookAccountUniGridExtender", typeof(FacebookAccountUniGridExtender))]

/// <summary>
/// Extends Facebook account Unigrid with additional abilities.
/// </summary>
public class FacebookAccountUniGridExtender : ControlExtender<UniGrid>
{
    #region "Life-cycle methods"

    /// <summary>
    /// Initializes the extender.
    /// </summary>
    public override void OnInit()
    {
        Control.OnExternalDataBound += Control_OnExternalDataBound;
        
        DisplayWarningIfNoDefaultAccount();
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
            case "FacebookAccountPageAccessTokenExpiration":
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
        if (FacebookAccountInfoProvider.GetDefaultFacebookAccount(SiteContext.CurrentSiteID) == null)
        {
            Control.ShowWarning(ResHelper.GetString("sm.facebook.nodefaultpage"));
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
            return ResHelper.GetString("sm.facebook.account.msg.tokenneverexpire");
        }
        expiration = new DateTime(expiration.Ticks, DateTimeKind.Utc);
        TimeSpan delta = expiration - DateTime.UtcNow;
        if (delta < TimeSpan.FromMinutes(5))
        {
            return GetWarning(ResHelper.GetString("sm.facebook.account.msg.accesstokenexpired"));
        }
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
        return TimeZoneHelper.ConvertToUserTimeZone(value.ToLocalTime(), true, MembershipContext.AuthenticatedUser, SiteContext.CurrentSite);
    }

    #endregion
}