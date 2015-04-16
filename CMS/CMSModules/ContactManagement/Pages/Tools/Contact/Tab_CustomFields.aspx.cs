using System;

using CMS.Core;
using CMS.ExtendedControls;
using CMS.OnlineMarketing;
using CMS.UIControls;

// Edited object
[EditedObject(ContactInfo.OBJECT_TYPE, "contactId")]
[Security(Resource = ModuleName.ONLINEMARKETING, UIElements = "EditContact;ContactCustomFields")]
public partial class CMSModules_ContactManagement_Pages_Tools_Contact_Tab_CustomFields : CMSContactManagementContactsPage
{
    private int siteId = 0;

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

        if (EditedObject != null)
        {
            // Get edited contact object
            ContactInfo ci = (ContactInfo)EditedObject;
            siteId = ci.ContactSiteID;

            CheckReadPermission(siteId);
            // Initialize dataform
            formCustomFields.Info = ci;
            formCustomFields.HideSystemFields = true;
            formCustomFields.AlternativeFormFullName = ci.TypeInfo.ObjectClassName;
            formCustomFields.OnBeforeSave += formCustomFields_OnBeforeSave;
            formCustomFields.OnAfterSave += formCustomFields_OnAfterSave;
        }
        else
        {
            // Disable dataform
            formCustomFields.Enabled = false;
        }
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
        ContactHelper.AuthorizedModifyContact(siteId, true);
    }


    protected void formCustomFields_OnAfterSave(object sender, EventArgs e)
    {
        // Display 'changes saved' information
        ShowChangesSaved();
    }
}