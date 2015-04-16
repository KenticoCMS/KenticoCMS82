using System;

using CMS.Helpers;
using CMS.Membership;
using CMS.Newsletters;
using CMS.SiteProvider;
using CMS.UIControls;

public partial class CMSModules_Newsletters_CMSPages_GetNewsletterIssue : LivePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // Check read permission for newsletters
        if (MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("CMS.Newsletter", "Read"))
        {
            IssueInfo iss = null;
            int issueId = QueryHelper.GetInteger("IssueId", 0);
            if (issueId > 0)
            {
                // Get newsletter issue by ID
                iss = IssueInfoProvider.GetIssueInfo(issueId);
            }
            else
            {
                // Get newsletter issue by GUID and site ID
                Guid issueGuid = QueryHelper.GetGuid("IssueGUID", Guid.Empty);
                iss = IssueInfoProvider.GetIssueInfo(issueGuid, SiteContext.CurrentSiteID);
            }

            if ((iss != null) && (iss.IssueSiteID == SiteContext.CurrentSiteID))
            {
                // Get newsletter
                NewsletterInfo news = NewsletterInfoProvider.GetNewsletterInfo(iss.IssueNewsletterID);

                Response.Clear();
                Response.Write(IssueInfoProvider.GetEmailBody(iss, news, null, null, false, SiteContext.CurrentSiteName, null, null, null));
                Response.Flush();

                RequestHelper.EndResponse();
            }
        }
    }
}