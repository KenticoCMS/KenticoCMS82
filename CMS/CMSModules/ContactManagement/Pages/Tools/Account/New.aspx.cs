using System;

using CMS.Helpers;
using CMS.OnlineMarketing;
using CMS.SiteProvider;
using CMS.UIControls;

[Help("onlinemarketing_account_new", "helptopic")]
[Breadcrumbs]
[Breadcrumb(0, "om.account.list", "~/CMSModules/ContactManagement/Pages/Tools/Account/List.aspx?{%QueryString|(encode)false%}", null)]
[Breadcrumb(1, "om.account.new")]
public partial class CMSModules_ContactManagement_Pages_Tools_Account_New : CMSContactManagementAccountsPage
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
            PageTitle.TitleText = GetString("om.account.new");
        }

        editElem.SiteID = siteId;
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        CheckReadPermission(siteId);
    }

    #endregion
}