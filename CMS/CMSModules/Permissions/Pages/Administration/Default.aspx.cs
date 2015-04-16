using System;
using System.Web.UI.WebControls;

using CMS.Core;
using CMS.Helpers;
using CMS.Membership;
using CMS.Modules;
using CMS.SiteProvider;
using CMS.UIControls;

public partial class CMSModules_Permissions_Pages_Administration_Default : CMSAdministrationPage
{
    #region "Page Events"

    protected override void OnPreInit(EventArgs e)
    {
        ((Panel)CurrentMaster.PanelBody.FindControl("pnlContent")).CssClass = String.Empty;

        SiteID = CurrentUser.CheckPrivilegeLevel(UserPrivilegeLevelEnum.GlobalAdmin) ? 0 : SiteContext.CurrentSiteID;

        // Set site id for the control
        prmhdrHeader.SiteID = SiteID;

        var uiElement = new UIElementAttribute(ModuleName.PERMISSIONS, "Permissions");
        uiElement.Check(this);

        base.OnPreInit(e);
    }


    /// <summary>
    /// Init event handler
    /// </summary>
    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        CurrentUserInfo user = MembershipContext.AuthenticatedUser;
        if (user != null)
        {
            // Check site availability
            if (!user.IsGlobalAdministrator)
            {
                if (!ResourceSiteInfoProvider.IsResourceOnSite("CMS.Permissions", SiteContext.CurrentSiteName))
                {
                    RedirectToResourceNotAvailableOnSite("CMS.Permissions");
                }
            }

            // Check "read" permission
            if (!user.IsAuthorizedPerResource("CMS.Permissions", "Read"))
            {
                RedirectToAccessDenied("CMS.Permissions", "Read");
            }
        }
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        // Setup page title text and image
        PageTitle.TitleText = GetString("Administration-Permissions_Header.PermissionsTitle");
        if (RequestHelper.IsCallback())
        {
            // First initialization for callback actions
            InitializeMatrix();
        }
    }


    protected void Page_PreRender(object sender, EventArgs e)
    {
        // Second initialization for cases when values in filter changed 
        InitializeMatrix();
    }

    #endregion


    #region "Private methods"

    /// <summary>
    /// Initialize permission matrix control.
    /// </summary>
    private void InitializeMatrix()
    {
        if (prmhdrHeader.HasSites)
        {
            // If global roles selected - Set sideID to zero
            int siteID = (SiteID > 0) ? SiteID : prmhdrHeader.SelectedSiteID;

            if (siteID == ValidationHelper.GetInteger(prmhdrHeader.GlobalRecordValue, 0))
            {
                siteID = 0;
            }

            prmMatrix.SelectedID = prmhdrHeader.SelectedID;
            prmMatrix.CornerText = GetString("Administration.Permissions.Role");
            if (prmMatrix.SelectedID != "0")
            {
                prmMatrix.SelectedType = prmhdrHeader.SelectedType;
            }
            prmMatrix.SiteID = siteID;
            prmMatrix.GlobalRoles = (siteID == 0) && MembershipContext.AuthenticatedUser.CheckPrivilegeLevel(UserPrivilegeLevelEnum.GlobalAdmin);
            prmMatrix.SelectedUserID = prmhdrHeader.SelectedUserID;
            prmMatrix.UserRolesOnly = prmhdrHeader.UserRolesOnly;

            prmMatrix.FilterChanged = prmhdrHeader.FilterChanged;
        }
    }

    #endregion
}