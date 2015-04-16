using System;

using CMS.Core;
using CMS.OnlineMarketing;
using CMS.SiteProvider;
using CMS.UIControls;

// Edited object
[EditedObject(ContactInfo.OBJECT_TYPE, "contactId")]
[Security(Resource = ModuleName.ONLINEMARKETING, UIElements = "EditContact;IPs")]
public partial class CMSModules_ContactManagement_Pages_Tools_Contact_Tab_IPs : CMSContactManagementContactsPage
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
        ContactInfo ci = (ContactInfo)EditedObject;

        // Check permission read
        int siteID = ContactHelper.ObjectSiteID(EditedObject);
        CheckReadPermission(siteID);

        bool globalContact = (ci.ContactSiteID == 0);
        bool mergedContact = (ci.ContactMergedWithContactID > 0);

        listElem.ShowContactNameColumn = globalContact;
        listElem.ShowSiteNameColumn = IsSiteManager && globalContact;
        listElem.ShowRemoveButton = !mergedContact && !globalContact && ContactHelper.AuthorizedModifyContact(ci.ContactSiteID, false);
        listElem.IsGlobalContact = globalContact;
        listElem.IsMergedContact = mergedContact;
        listElem.ContactID = ci.ContactID;

        // Restrict site IDs in CMSDesk
        if (!IsSiteManager)
        {
            listElem.WhereCondition = "ContactSiteID = " + SiteContext.CurrentSiteID;
        }
    }
}