using System;

using CMS.Helpers;
using CMS.OnlineMarketing;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.Membership;

// Edited object
[EditedObject(ContactStatusInfo.OBJECT_TYPE, "statusid")]
// Breadcrumbs
[Breadcrumbs()]
[Breadcrumb(0, "om.contactstatus.list", "~/CMSModules/ContactManagement/Pages/Tools/Configuration/ContactStatus/List.aspx", null)]
[Breadcrumb(1, ResourceString = "om.contactstatus.new", NewObject = true)]
[Breadcrumb(1, Text = "{% (EditedObject.SiteID > 0) ? EditedObject.DisplayName :  EditedObject.DisplayName + \" \" + GetResourceString(\"general.global\")%}", ExistingObject = true)]
// Title
[Title(ResourceString = "om.contactstatus.new", HelpTopic = "onlinemarketing_contactstatus_new", NewObject = true)]
[Title(ResourceString = "om.contactstatus.edit", HelpTopic = "onlinemarketing_contactstatus_edit", ExistingObject = true)]
public partial class CMSModules_ContactManagement_Pages_Tools_Configuration_ContactStatus_Edit : CMSContactManagementContactStatusPage
{
    #region "Variables"

    private CurrentUserInfo currentUser;
    private ContactStatusInfo currentObject;
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
        currentObject = (ContactStatusInfo)EditedObject;

        // Check read permission
        currentObjectSiteId = currentObject.ContactStatusID != 0 ? currentObject.ContactStatusSiteID : siteID;
        CheckReadPermission(currentObjectSiteId);

        // Preserve site info passed in query        
        PageBreadcrumbs.Items[0].RedirectUrl = AddSiteQuery(PageBreadcrumbs.Items[0].RedirectUrl, siteID);
        EditForm.RedirectUrlAfterSave = AddSiteQuery(EditForm.RedirectUrlAfterSave, siteID);

        ContactStatusInfo contactStatus = EditForm.EditedObject as ContactStatusInfo;

        // Set new site ID for new object
        if ((contactStatus == null) || (contactStatus.ContactStatusID < 1))
        {
            if ((siteID == UniSelector.US_GLOBAL_RECORD) && ModifyGlobalConfiguration)
            {
                EditForm.Data["ContactStatusSiteID"] = null;
            }
            else if (IsSiteManager && currentUser.CheckPrivilegeLevel(UserPrivilegeLevelEnum.GlobalAdmin))
            {
                EditForm.Data["ContactStatusSiteID"] = siteID;
            }
            else
            {
                EditForm.Data["ContactStatusSiteID"] = SiteContext.CurrentSiteID;
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