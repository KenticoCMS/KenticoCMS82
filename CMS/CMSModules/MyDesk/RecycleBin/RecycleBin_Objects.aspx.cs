using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.LicenseProvider;
using CMS.Membership;
using CMS.Base;
using CMS.SiteProvider;
using CMS.UIControls;

public partial class CMSModules_MyDesk_RecycleBin_RecycleBin_Objects : CMSContentManagementPage
{
    protected void Page_Init(object sender, EventArgs e)
    {
        // Check the license
        if (DataHelper.GetNotEmpty(RequestContext.CurrentDomain, "") != "")
        {
            LicenseHelper.CheckFeatureAndRedirect(RequestContext.CurrentDomain, FeatureEnum.ObjectVersioning);
        }
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        // Check UIProfile
        if (!MembershipContext.AuthenticatedUser.IsAuthorizedPerUIElement("CMS.Content", new string[] { "MyRecycleBin", "MyRecycleBin.Objects" }, SiteContext.CurrentSiteName))
        {
            RedirectToUIElementAccessDenied("CMS.Content", "MyRecycleBin;MyRecycleBin.Objects");
        }
        recycleBin.SiteName = SiteContext.CurrentSiteName;
    }
}