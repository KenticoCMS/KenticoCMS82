using System;
using CMS.Core;
using CMS.Membership;
using CMS.OnlineMarketing;
using CMS.UIControls;

[EditedObject(ContactInfo.OBJECT_TYPE, "contactID")]
public partial class CMSModules_ContactManagement_Pages_Tools_Contact_Tab_Scoring : CMSScorePage
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
        // Check UI personalization
        CheckUIElementAccessHierarchical(ModuleName.ONLINEMARKETING, "ContactScoring");

        // Set up control
        ContactInfo ci = (ContactInfo)EditedObject;
        if (ci == null)
        {
            RedirectToAccessDenied(GetString("general.invalidparameters"));
        }
        
        cScoring.ContactID = ci.ContactID;
        cScoring.SiteID = ci.ContactSiteID;
        cScoring.UniGrid.ZeroRowsText = GetString("om.score.notfound");
    }


    /// <summary>
    /// Check read permissions.
    /// </summary>
    protected override void CheckReadPermission()
    {
        // Check read permission for score or contact
        int siteID = ContactHelper.ObjectSiteID(EditedObject);
        var user = MembershipContext.AuthenticatedUser;
        if (!ContactHelper.AuthorizedReadContact(siteID, false) && !user.IsAuthorizedPerResource("CMS.Scoring", "Read"))
        {
            RedirectToAccessDenied(ModuleName.CONTACTMANAGEMENT, "ReadContacts");
        }
    }
}