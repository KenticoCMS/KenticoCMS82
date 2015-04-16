using System;

using CMS.Core;
using CMS.ExtendedControls;
using CMS.OnlineMarketing;
using CMS.UIControls;

// Edited object
[EditedObject(AccountInfo.OBJECT_TYPE, "objectid")]
public partial class CMSModules_ContactManagement_Pages_Tools_Account_Tab_CustomFields : CMSContactManagementAccountsPage
{
    private int siteId;


    /// <summary>
    /// PreInit event handler
    /// </summary>
    protected override void OnPreInit(EventArgs e)
    {
        RequiresDialog = false;
        base.OnPreInit(e);
    }


    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        // Check UI elements
        CheckUIElementAccessHierarchical(ModuleName.ONLINEMARKETING, "account.customfields");

        if (EditedObject != null)
        {
            // Get edited account
            AccountInfo ai = (AccountInfo)EditedObject;
            siteId = ai.AccountSiteID;
            // Initialize dataform
            formCustomFields.Info = ai;
            formCustomFields.HideSystemFields = true;
            formCustomFields.AlternativeFormFullName = ai.TypeInfo.ObjectClassName;
            formCustomFields.OnBeforeSave += formCustomFields_OnBeforeSave;
            formCustomFields.OnAfterSave += formCustomFields_OnAfterSave;
        }
        else
        {
            // Disable dataform
            formCustomFields.Enabled = false;
        }
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        // Check read permission for account
        CheckReadPermission(siteId);
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        if (formCustomFields != null)
        {
            // Set submit button's css class
            formCustomFields.SubmitButton.ButtonStyle = ButtonStyle.Default;
        }
    }


    protected void formCustomFields_OnBeforeSave(object sender, EventArgs e)
    {
        // Check modify permissions
        AccountHelper.AuthorizedModifyAccount(siteId, true);
    }


    protected void formCustomFields_OnAfterSave(object sender, EventArgs e)
    {
        // Display 'changes saved' information
        ShowChangesSaved();
    }
}