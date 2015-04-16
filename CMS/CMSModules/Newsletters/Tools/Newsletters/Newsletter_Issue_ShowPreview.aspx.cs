using System;

using CMS.Base;
using CMS.Core;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.Membership;
using CMS.Newsletters;
using CMS.SiteProvider;
using CMS.UIControls;

[EditedObject(IssueInfo.OBJECT_TYPE, "objectid")]
[UIElement(ModuleName.NEWSLETTER, "Newsletter.Issue.Content")]
public partial class CMSModules_Newsletters_Tools_Newsletters_Newsletter_Issue_ShowPreview : CMSNewsletterPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        IssueInfo issue = EditedObject as IssueInfo;
        if (issue == null)
        {
            RedirectToAccessDenied(GetString("general.invalidparameters"));
        }

        if(!issue.CheckPermissions(PermissionsEnum.Read, CurrentSiteName, CurrentUser))
        {
            RedirectToAccessDenied(issue.TypeInfo.ModuleName, "AuthorIssues");   
        }
       
        Guid subscriberGuid = QueryHelper.GetGuid("subscriberguid", Guid.Empty);
        SubscriberInfo subscriber = SubscriberInfoProvider.GetSubscriberInfo(subscriberGuid, SiteContext.CurrentSiteID);

        using (var context = new CMSActionContext())
        {
            // Switch culture to the site culture, so the e-mail isn't rendered in the editor's culture
            string culture = CultureHelper.GetDefaultCultureCode(SiteContext.CurrentSiteName);
            context.Culture = CultureHelper.GetCultureInfo(culture);

            string htmlPage = NewsletterHelper.GetPreviewHTML(issue, subscriber);
            Response.Clear();
            Response.Write(htmlPage);
        }

        RequestHelper.EndResponse();
    }
}