using System;

using CMS;
using CMS.Base;
using CMS.ExtendedControls;
using CMS.Helpers;
using CMS.SiteProvider;
using CMS.SocialMarketing;
using CMS.UIControls;

[assembly: RegisterCustomClass("TwitterAccountUniGridExtender", typeof(TwitterAccountUniGridExtender))]

/// <summary>
/// Extends Twitter accounts Unigrid with additional abilities.
/// </summary>
public class TwitterAccountUniGridExtender : ControlExtender<UniGrid>
{
    #region "Life-cycle methods"

    /// <summary>
    /// Initializes the extender.
    /// </summary>
    public override void OnInit()
    {
        DisplayWarningIfNoDefaultAccount();
    }

    #endregion


    #region "Private methods"

    /// <summary>
    /// Displays warning if there is no default account set on the site.
    /// </summary>
    private void DisplayWarningIfNoDefaultAccount()
    {
        if (TwitterAccountInfoProvider.GetDefaultTwitterAccount(SiteContext.CurrentSiteID) == null)
        {
            Control.ShowWarning(ResHelper.GetString("sm.twitter.nodefaultchannel"));
        }
    }

    #endregion
}