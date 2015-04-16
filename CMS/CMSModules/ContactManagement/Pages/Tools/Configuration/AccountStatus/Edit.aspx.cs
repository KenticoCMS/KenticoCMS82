using System;

using CMS.Helpers;
using CMS.OnlineMarketing;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.Membership;

// Edited object
[EditedObject(AccountStatusInfo.OBJECT_TYPE, "statusid")]
// Breadcrumbs
[Breadcrumbs]
[Breadcrumb(0, "om.accountstatus.list", "~/CMSModules/ContactManagement/Pages/Tools/Configuration/AccountStatus/List.aspx", null)]
[Breadcrumb(1, ResourceString = "om.accountstatus.new", NewObject = true)]
[Breadcrumb(1, Text = "{% (EditedObject.SiteID > 0) ? EditedObject.DisplayName :  EditedObject.DisplayName + \" \" + GetResourceString(\"general.global\")%}", ExistingObject = true)]
// Title
[Title(ResourceString = "om.accountstatus.new", HelpTopic = "onlinemarketing_accountstatus_new", NewObject = true)]
[Title(ResourceString = "om.accountstatus.edit", HelpTopic = "onlinemarketing_accountstatus_edit", ExistingObject = true)]
public partial class CMSModules_ContactManagement_Pages_Tools_Configuration_AccountStatus_Edit : CMSContactManagementAccountStatusPage
{
    #region "Variables"

    private CurrentUserInfo currentUser;
    private AccountStatusInfo currentObject;
    private int siteID;
    private int currentObjectSiteId = SiteContext.CurrentSiteID;

    #endregion


    #region "Methods"

    protected override void OnPreInit(EventArgs e)
    {
        base.OnPreInit(e);
        EditForm.OnBeforeSave += EditForm_OnBeforeSave;

        siteID = QueryHelper.GetInteger("siteid", SiteContext.CurrentSiteID);
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        currentUser = MembershipContext.AuthenticatedUser;
        currentObject = (AccountStatusInfo)EditedObject;

        currentObjectSiteId = currentObject.AccountStatusID != 0 ? currentObject.AccountStatusSiteID : siteID;
        CheckReadPermission(currentObjectSiteId);

        // Preserve site info passed in query        
        PageBreadcrumbs.Items[0].RedirectUrl = AddSiteQuery(PageBreadcrumbs.Items[0].RedirectUrl, siteID);
        EditForm.RedirectUrlAfterSave = AddSiteQuery(EditForm.RedirectUrlAfterSave, siteID);

        AccountStatusInfo accountStatus = EditForm.EditedObject as AccountStatusInfo;

        // Set new site ID for new object
        if ((accountStatus == null) || (accountStatus.AccountStatusID < 1))
        {
            if ((siteID == UniSelector.US_GLOBAL_RECORD) && ModifyGlobalConfiguration)
            {
                EditForm.Data["AccountStatusSiteID"] = null;
            }
            else if (IsSiteManager && currentUser.CheckPrivilegeLevel(UserPrivilegeLevelEnum.GlobalAdmin))
            {
                EditForm.Data["AccountStatusSiteID"] = siteID;
            }
            else
            {
                EditForm.Data["AccountStatusSiteID"] = SiteContext.CurrentSiteID;
            }
        }
    }


    /// <summary>
    /// OnBeforeSave event handler.
    /// </summary>
    private void EditForm_OnBeforeSave(object sender, EventArgs e)
    {
        // Check permissions
        ConfigurationHelper.AuthorizedModifyConfiguration(currentObjectSiteId, true);
    }

    #endregion
}