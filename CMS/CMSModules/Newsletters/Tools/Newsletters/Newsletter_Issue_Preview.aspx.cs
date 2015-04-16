using System;

using CMS.Core;
using CMS.Helpers;
using CMS.LicenseProvider;
using CMS.Newsletters;
using CMS.SiteProvider;
using CMS.Membership;
using CMS.UIControls;
using CMS.DataEngine;
using CMS.Modules;
using CMS.ExtendedControls;

// Set edited object
[UIElement(ModuleName.NEWSLETTER, "Newsletter")]
[EditedObject(IssueInfo.OBJECT_TYPE, "objectid")]
[Title("general.preview")]
public partial class CMSModules_Newsletters_Tools_Newsletters_Newsletter_Issue_Preview : CMSDeskPage
{
    protected void Page_Init(object sender, EventArgs e)
    {
        var newsletterIssue = EditedObject as IssueInfo;
        if (newsletterIssue == null)
        {
            RedirectToAccessDenied(GetString("general.invalidparameters"));
        }

        if (!newsletterIssue.CheckPermissions(PermissionsEnum.Read, CurrentSiteName, CurrentUser))
        {
            RedirectToAccessDenied(newsletterIssue.TypeInfo.ModuleName, "AuthorIssues");
        }
        
        // Check the license
        if (!string.IsNullOrEmpty(DataHelper.GetNotEmpty(RequestContext.CurrentDomain, string.Empty)))
        {
            LicenseHelper.CheckFeatureAndRedirect(RequestContext.CurrentDomain, FeatureEnum.Newsletters);
        }

        // Check site availability
        if (!ResourceSiteInfoProvider.IsResourceOnSite("CMS.Newsletter", SiteContext.CurrentSiteName))
        {
            RedirectToResourceNotAvailableOnSite("CMS.Newsletter");
        }

        CurrentMaster.HeaderContainer.RemoveCssClass("header-container");
        CurrentMaster.PanelContent.RemoveCssClass("dialog-content");
    }
}