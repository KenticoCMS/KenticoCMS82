using System;

using CMS.Core;
using CMS.OnlineMarketing;
using CMS.UIControls;

// Edited object
[EditedObject(ContactInfo.OBJECT_TYPE, "contactId")]
[Security(Resource = ModuleName.ONLINEMARKETING, UIElements = "EditContact;ContactMerge;ChooseContact")]
public partial class CMSModules_ContactManagement_Pages_Tools_Contact_Merge_Choose : CMSContactManagementContactsPage
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
        // Check read permission
        CheckReadPermission(ContactHelper.ObjectSiteID(EditedObject));
    }
}