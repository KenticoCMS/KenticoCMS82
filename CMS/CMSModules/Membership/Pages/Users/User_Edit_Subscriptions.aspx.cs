using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.Helpers;
using CMS.SiteProvider;
using CMS.Membership;
using CMS.UIControls;

public partial class CMSModules_Membership_Pages_Users_User_Edit_Subscriptions : CMSUsersPage
{
    protected int userId;
    protected int mSiteID;

    public override int SiteID
    {
        get
        {
            if (mSiteID <= 0)
            {
                mSiteID = SiteContext.CurrentSiteID;
            }

            return mSiteID;
        }
        set
        {
            elemSubscriptions.SiteID = mSiteID = value;
        }
    }


    protected void Page_Init(object sender, EventArgs e)
    {
        userId = QueryHelper.GetInteger("userid", 0);
        if (userId > 0)
        {
            // Check that only global administrator can edit global administrator's accouns
            UserInfo ui = UserInfoProvider.GetUserInfo(userId);
            EditedObject = ui;
            CheckUserAvaibleOnSite(ui);

            if (!CheckGlobalAdminEdit(ui))
            {
                elemSubscriptions.StopProcessing = true;
                elemSubscriptions.Visible = false;
                ShowError(ResHelper.GetString("Administration-User_List.ErrorGlobalAdmin"));
                return;
            }

            elemSubscriptions.UserID = userId;
            elemSubscriptions.SiteID = SiteID;
            elemSubscriptions.IsLiveSite = false;
            elemSubscriptions.OnCheckPermissions += new CMSAdminControl.CheckPermissionsEventHandler(elemSubscriptions_OnCheckPermissions);
            elemSubscriptions.ReloadData();
        }
        else
        {
            elemSubscriptions.StopProcessing = true;
        }
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        // Show site selector
        CurrentMaster.DisplaySiteSelectorPanel = true;

        if ((SiteID > 0) && !MembershipContext.AuthenticatedUser.CheckPrivilegeLevel(UserPrivilegeLevelEnum.GlobalAdmin))
        {
            CurrentMaster.DisplaySiteSelectorPanel = false;
            return;
        }

        // Set site selector
        siteSelector.DropDownSingleSelect.AutoPostBack = true;
        siteSelector.OnlyRunningSites = false;
        siteSelector.UniSelector.OnSelectionChanged += new EventHandler(UniSelector_OnSelectionChanged);

        if (!RequestHelper.IsPostBack())
        {
            // If user is member of current site
            if (UserSiteInfoProvider.GetUserSiteInfo(userId, SiteID) != null)
            {
                // Force uniselector to preselect current site
                siteSelector.Value = SiteID;
            }

            // Force to load data
            siteSelector.Reload(true);
        }

        // Get truly selected item
        SiteID = ValidationHelper.GetInteger(siteSelector.Value, 0);
    }


    /// <summary>
    /// Handles site selection change event.
    /// </summary>
    protected void UniSelector_OnSelectionChanged(object sender, EventArgs e)
    {
        SiteID = ValidationHelper.GetInteger(siteSelector.Value, 0);

        elemSubscriptions.SiteID = SiteID;
        elemSubscriptions.ReloadData();
        updateContent.Update();
    }


    protected void elemSubscriptions_OnCheckPermissions(string permissionType, CMSAdminControl sender)
    {
        if (!MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("CMS.Users", CMSAdminControl.PERMISSION_MODIFY))
        {
            RedirectToAccessDenied("CMS.Users", CMSAdminControl.PERMISSION_MODIFY);
        }
    }
}