using System;

using CMS.Helpers;
using CMS.OnlineMarketing;
using CMS.SiteProvider;
using CMS.UIControls;

[Help("onlinemarketing_contact_new", "helptopic")]
[Breadcrumbs]
[Breadcrumb(0, "om.contact.list", "~/CMSModules/ContactManagement/Pages/Tools/Contact/List.aspx?{%QueryString|(encode)false%}", null)]
[Breadcrumb(1, "om.contact.new")]
public partial class CMSModules_ContactManagement_Pages_Tools_Contact_New : CMSContactManagementContactsPage
{
    #region "Variables"

    private int siteId;

    #endregion


    #region "Page events"

    protected void Page_Init(object sender, EventArgs e)
    {
        siteId = QueryHelper.GetInteger("siteid", SiteContext.CurrentSiteID);

        if (IsSiteManager)
        {
            PageTitle.TitleText = GetString("om.contact.new");
        }

        editElem.SiteID = siteId;
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        CheckReadPermission(siteId);
    }

    #endregion
}