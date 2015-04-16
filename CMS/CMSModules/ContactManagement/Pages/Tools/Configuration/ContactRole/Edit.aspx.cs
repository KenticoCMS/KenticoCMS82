using System;

using CMS.Helpers;
using CMS.OnlineMarketing;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.Membership;

// Edited object
[EditedObject(ContactRoleInfo.OBJECT_TYPE, "roleid")]
// Breadcrumbs
[Breadcrumbs()]
[Breadcrumb(0, "om.contactrole.list", "~/CMSModules/ContactManagement/Pages/Tools/Configuration/ContactRole/List.aspx", null)]
[Breadcrumb(1, ResourceString = "om.contactrole.new", NewObject = true)]
[Breadcrumb(1, Text = "{% (EditedObject.SiteID > 0) ? EditedObject.DisplayName :  EditedObject.DisplayName + \" \" + GetResourceString(\"general.global\")%}", ExistingObject = true)]
// Title
[Title(ResourceString = "om.contactrole.new", HelpTopic = "onlinemarketing_contactrole_new", NewObject = true)]
[Title(ResourceString = "om.contactrole.edit", HelpTopic = "onlinemarketing_contactrole_edit", ExistingObject = true)]
public partial class CMSModules_ContactManagement_Pages_Tools_Configuration_ContactRole_Edit : CMSContactManagementContactRolePage
{
    #region "Variables"

    private ContactRoleInfo currentObject;
    private CurrentUserInfo currentUser;
    private int siteID;
    private int currentObjectSiteId = SiteContext.CurrentSiteID;

    #endregion


    #region "Methods"

    protected override void OnPreInit(EventArgs e)
    {
        base.OnPreInit(e);
        EditForm.OnBeforeSave += new EventHandler(EditForm_OnBeforeSave);
        siteID = QueryHelper.GetInteger("siteid", SiteContext.CurrentSiteID);
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        currentUser = MembershipContext.AuthenticatedUser;
        currentObject = (ContactRoleInfo)EditedObject;

        // Check read permission
        currentObjectSiteId = currentObject.ContactRoleID != 0 ? currentObject.ContactRoleSiteID : siteID;
        CheckReadPermission(currentObjectSiteId);

        // Preserve site info passed in query        
        PageBreadcrumbs.Items[0].RedirectUrl = AddSiteQuery(PageBreadcrumbs.Items[0].RedirectUrl, siteID);
        EditForm.RedirectUrlAfterSave = AddSiteQuery(EditForm.RedirectUrlAfterSave, siteID);

        ContactRoleInfo contactRole = EditForm.EditedObject as ContactRoleInfo;

        // Set new site ID for new object
        if ((contactRole == null) || (contactRole.ContactRoleID < 1))
        {
            if ((siteID == UniSelector.US_GLOBAL_RECORD) && ModifyGlobalConfiguration)
            {
                EditForm.Data["ContactRoleSiteID"] = null;
            }
            else if (IsSiteManager && currentUser.CheckPrivilegeLevel(UserPrivilegeLevelEnum.GlobalAdmin))
            {
                EditForm.Data["ContactRoleSiteID"] = siteID;
            }
            else
            {
                EditForm.Data["ContactRoleSiteID"] = SiteContext.CurrentSiteID;
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