using System;

using CMS.Helpers;
using CMS.MacroEngine;
using CMS.OnlineMarketing;
using CMS.WebAnalytics;
using CMS.Newsletters;

public partial class CMSModules_ContactManagement_Controls_UI_ActivityDetails_NewsletterOpen : ActivityDetail
{
    #region "Methods"

    public override bool LoadData(ActivityInfo ai)
    {
        if ((ai == null) || (ai.ActivityType != PredefinedActivityType.NEWSLETTER_OPEN))
        {
            return false;
        }

        // Get newsletter name
        int newsletterId = ai.ActivityItemID;
        NewsletterInfo newsletterInfo = NewsletterInfoProvider.GetNewsletterInfo(newsletterId);
        if (newsletterInfo != null)
        {
            string subject = ValidationHelper.GetString(newsletterInfo.NewsletterDisplayName, null);
            ucDetails.AddRow("om.activitydetails.newsletter", subject);
        }

        // Get issue subject
        int issueId = ai.ActivityItemDetailID;
        IssueInfo issueInfo = IssueInfoProvider.GetIssueInfo(issueId);
        if (issueInfo != null)
        {
            string subject = ValidationHelper.GetString(issueInfo.IssueSubject, null);
            ucDetails.AddRow("om.activitydetails.newsletterissue", MacroSecurityProcessor.RemoveSecurityParameters(subject, true, null));
        }

        return ucDetails.IsDataLoaded;
    }

    #endregion
}