using System;

using CMS.Core;
using CMS.Helpers;
using CMS.OnlineMarketing;
using CMS.UIControls;

[EditedObject(ContactInfo.OBJECT_TYPE, "contactId")]
[Security(Resource = ModuleName.ONLINEMARKETING, UIElements = "EditContact;ContactGeneral")]
public partial class CMSModules_ContactManagement_Pages_Tools_Contact_Tab_General : CMSContactManagementContactsPage
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
        int siteID = ContactHelper.ObjectSiteID(EditedObject);
        CheckReadPermission(siteID);
    }
}