using System;
using System.Data;
using System.Web;

using CMS.Base;
using CMS.EventLog;
using CMS.Helpers;
using CMS.Localization;
using CMS.Membership;
using CMS.PortalEngine;
using CMS.SiteProvider;
using CMS.UIControls;

public partial class CMSAdminControls_UI_UserMenu : CMSUserControl
{
    #region "Page events"

    /// <summary>
    /// Page_Load event handler.
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!StopProcessing)
        {
            SetupControl();
        }

        // Load JavaScript module
        ScriptHelper.RegisterModule(Page, "CMS/UserMenu", new
        {
            wrapperSelector = ".cms-navbar.js-user-menu-wrapper",
            checkChangesLinksSelector = ".js-check-changes"
        });
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Setups control.
    /// </summary>
    private void SetupControl()
    {
        // Check if main actions are visible
        plcTopDivider.Visible = EnsureMainItems();

        // Check if bottom actions are visible
        plcBottomDivider.Visible = EnsureBottomItems();

        // Do not show uniselector open dialog buttons.
        // Custom link buttons are used to open the dialog.
        ucUsers.DialogButton.Visible = false;
        ucUICultures.DialogButton.Visible = false;
    }


    /// <summary>
    /// Ensures main items in menu.
    /// </summary>
    /// <returns>Returns true if at least one item is visible</returns>
    private bool EnsureMainItems()
    {
        bool topDividerVisible = EnsureImpersonation();
        topDividerVisible |= EnsureUICultures();

        return topDividerVisible;
    }


    /// <summary>
    /// Ensures bottom items in menu.
    /// </summary>
    /// <returns>Returns true if at least one item is visible</returns>
    private bool EnsureBottomItems()
    {
        return EnsureSignOut();
    }

    #endregion


    #region "Items"

    /// <summary>
    /// Checks Sign out link settings.
    /// </summary>
    private bool EnsureSignOut()
    {
        if (RequestHelper.IsWindowsAuthentication())
        {
            // Hide sign out link
            lnkSignOut.Visible = false;
            return false;
        }

        // Get third party providers' logout scripts
        string logoutScript = AuthenticationHelper.GetSignOutOnClickScript(Page);
        if (!String.IsNullOrEmpty(logoutScript))
        {
            // If a script was provided, check changes first, before executing it
            logoutScript = "if (CheckChanges()) { " + logoutScript + " } return false; ";
        }
        else
        {
            // If no javascript logout was registered, just check changes
            logoutScript = "return CheckChanges();";
        }
        lnkSignOut.OnClientClick = logoutScript;

        return true;
    }


    /// <summary>
    /// Checks user impersonation settings.
    /// </summary>
    private bool EnsureImpersonation()
    {
        string originalUserName = ValidationHelper.GetString(SessionHelper.GetValue(RequestHelper.IsFormsAuthentication() ? "OriginalUserName" : "ImpersonateUserName"), "");

        // Show impersonate button for global admin only or impersonated user
        if (MembershipContext.AuthenticatedUser.IsGlobalAdministrator || !String.IsNullOrEmpty(originalUserName))
        {
            lnkUsers.Visible = true;
            lnkUsers.OnClientClick = ucUsers.GetSelectionDialogScript();

            // Show all users except global administrators and public user, in CMSDesk show only site users
            ucUsers.WhereCondition = "UserID IN (SELECT UserID FROM CMS_UserSite WHERE " + (IsCMSDesk ? "(SiteID = " + SiteContext.CurrentSiteID + ") AND" : "") + " (UserIsGlobalAdministrator = 0)) AND (UserID != " + MembershipContext.AuthenticatedUser.UserID + ") AND (UserName != N'public')";

            // Script for open uniselector modal dialog
            string impersonateScript = "function userImpersonateShowDialog () {US_SelectionDialog_" + ucUsers.ClientID + "()}";
            ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "ImpersonateContextMenu", ScriptHelper.GetScript(impersonateScript));

            string userName = HttpUtility.UrlDecode(ValidationHelper.GetString(ucUsers.Value, String.Empty));
            if (userName != String.Empty)
            {
                // Get selected user info
                UserInfo iui = UserInfoProvider.GetUserInfo(userName);
                if (!iui.IsGlobalAdministrator)
                {
                    // Indicates whether user will be able to continue in the administration interface    
                    bool keepAdimUI = iui.CheckPrivilegeLevel(UserPrivilegeLevelEnum.Editor, SiteContext.CurrentSiteName);

                    AuthenticationHelper.ImpersonateUser(iui, null, !keepAdimUI);
                    if (keepAdimUI)
                    {
                        UIContextHelper.RegisterAdminRedirectScript(Page);
                    }
                }
            }

            // Set visibility of Cancel impersonation item in menu
            if (!String.IsNullOrEmpty(originalUserName))
            {
                plcCancelImpersonate.Visible = true;
                lnkCancelImpersonate.Text = GetString("users.cancelimpersonation");
            }
            else
            {
                plcCancelImpersonate.Visible = false;
            }
            return true;
        }

        // Hide impersonate action in menu
        plcImpersonate.Visible = false;
        return false;
    }


    /// <summary>
    /// Checks UI culture settings.
    /// </summary>
    private bool EnsureUICultures()
    {
        DataSet ds = CultureInfoProvider.GetUICultures(topN: 0, columns: "COUNT (CultureID)");

        // Show selector only if there are more UI cultures than one
        if (!DataHelper.DataSourceIsEmpty(ds) && (ValidationHelper.GetInteger(ds.Tables[0].Rows[0][0], 0) > 1))
        {
            lnkUICultures.Visible = true;
            lnkUICultures.OnClientClick = ucUICultures.GetSelectionDialogScript();

            string cultureName = ValidationHelper.GetString(ucUICultures.Value, String.Empty);
            if (cultureName != string.Empty)
            {
                MembershipContext.AuthenticatedUser.PreferredUICultureCode = cultureName;
                UserInfoProvider.SetUserInfo(MembershipContext.AuthenticatedUser);

                // Set selected UI culture and refresh all pages
                CultureHelper.SetPreferredUICultureCode(cultureName);

                UIContextHelper.RegisterAdminRedirectScript(Page);
            }

            return true;
        }

        // Hide UI culture action in menu
        plcUICultures.Visible = false;
        return false;
    }

    #endregion


    #region "Control events"

    /// <summary>
    /// OnClick event handler for cancel impersonation.
    /// </summary>
    protected void lnkCancelImpersonate_OnClick(object sender, EventArgs e)
    {
        string originalUserName = ValidationHelper.GetString(SessionHelper.GetValue("OriginalUserName"), "");
        if (RequestHelper.IsFormsAuthentication())
        {
            UserInfo ui = UserInfoProvider.GetUserInfo(originalUserName);
            AuthenticationHelper.ImpersonateUser(ui, null, false);
            UIContextHelper.RegisterAdminRedirectScript(Page);
        }
        else
        {
            SessionHelper.SetValue("ImpersonateUserName", null);
            SessionHelper.SetValue("OriginalUserName", null);

            MembershipContext.AuthenticatedUser.Generalized.Invalidate(false);

            // Log event
            EventLogProvider.LogEvent(EventType.INFORMATION, "Administration", "Impersonate", "User " + originalUserName + " has returned to his account.", RequestContext.CurrentURL, 0, null, 0, null, null, SiteContext.CurrentSiteID);

            URLHelper.Redirect(RequestContext.CurrentURL);
        }
    }


    /// <summary>
    /// OnClick event handler for sign out.
    /// </summary>
    protected void lnkSignOut_OnClick(object sender, EventArgs e)
    {
        // Usual sign out
        string signOutUrl = SystemContext.ApplicationPath.TrimEnd('/') + "/default.aspx";

        // LiveID sign out URL is set if this LiveID session
        AuthenticationHelper.SignOut(signOutUrl);
    }

    #endregion
}