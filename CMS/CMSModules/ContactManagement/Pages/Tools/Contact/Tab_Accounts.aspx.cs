using System;

using CMS.Core;
using CMS.OnlineMarketing;
using CMS.UIControls;

[EditedObject(ContactInfo.OBJECT_TYPE, "contactId")]
[Security(Resource = ModuleName.ONLINEMARKETING, UIElements = "EditContact;ContactAccounts")]
public partial class CMSModules_ContactManagement_Pages_Tools_Contact_Tab_Accounts : CMSContactManagementContactsPage
{
    /// <summary>
    /// PreInit event handler
    /// </summary>
    protected override void OnPreInit(EventArgs e)
    {
        RequiresDialog = false;
        base.OnPreInit(e);
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        CurrentMaster.PanelContent.CssClass = String.Empty;

        int siteID = ContactHelper.ObjectSiteID(EditedObject);

        // Check read permission 
        if (!AccountHelper.AuthorizedReadAccount(siteID, false) && !ContactHelper.AuthorizedReadContact(siteID, false))
        {
            RedirectToAccessDenied(ModuleName.CONTACTMANAGEMENT, "ReadContacts");
        }
    }
}