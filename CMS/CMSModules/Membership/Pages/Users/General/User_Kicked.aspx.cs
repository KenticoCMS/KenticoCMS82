using System;
using System.Data;
using System.Web.UI.WebControls;

using CMS.Helpers;
using CMS.LicenseProvider;
using CMS.Base;
using CMS.SiteProvider;
using CMS.Membership;
using CMS.UIControls;
using CMS.ExtendedControls;
using CMS.ExtendedControls.ActionsConfig;
using CMS.DataEngine;

public partial class CMSModules_Membership_Pages_Users_General_User_Kicked : CMSUsersPage
{
    #region "Page events"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        // Check permissions
        LicenseHelper.CheckFeatureAndRedirect(RequestContext.CurrentDomain, FeatureEnum.OnlineUsers);

        // If session management is disabled inform about it
        if (!SessionManager.OnlineUsersEnabled)
        {
            // Set disabled module info
            ucDisabledModule.SettingsKeys = "CMSUseSessionManagement";
            ucDisabledModule.InfoText = GetString("administration.users.online.disabled");

            gridElem.Visible = false;
        }
        else
        {
            SetHeaderActions();
            PrepareUnigrid();

            gridElem.HideFilterButton = true;
            gridElem.LoadGridDefinition();

            // Filter settings
            IUserFilter filter = (IUserFilter)gridElem.CustomFilter;
            filter.DisplayUserEnabled = false;
        }
    }


    protected override void CheckUIPermissions()
    {
        // Check UI Permissions for online marketing if needed
        if (QueryHelper.GetBoolean("isonlinemarketing", false))
        {
            var user = MembershipContext.AuthenticatedUser;
            if (!user.IsAuthorizedPerUIElement("CMS", "CMSDesk.OnlineMarketing"))
            {
                RedirectToUIElementAccessDenied("CMS", "CMSDesk.OnlineMarketing");
            }
            if (!user.IsAuthorizedPerUIElement("CMS.OnlineMarketing", new string[] {"ContactsFrameset", "On-line_users" }, SiteContext.CurrentSiteName))
            {
                RedirectToUIElementAccessDenied("CMS.OnlineMarketing", "ContactsFrameset;On-line_users");
            }
        }
        else
        {
            base.CheckUIPermissions();
        }
    }

    #endregion


    #region "Unigrid"

    /// <summary>
    /// Sets where condition before data binding.
    /// </summary>
    protected void gridElem_OnBeforeDataReload()
    {
        if (gridElem.QueryParameters == null)
        {
            gridElem.QueryParameters = new QueryDataParameters();
        }
        gridElem.QueryParameters.Add("@Now", DateTime.Now);
    }


    /// <summary>
    ///  On action event.
    /// </summary>
    private void gridElem_OnAction(string actionName, object actionArgument)
    {
        // Check "modify" permission
        if (!MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("CMS.Users", "Modify"))
        {
            RedirectToAccessDenied("CMS.Users", "Modify");
        }

        int userId = ValidationHelper.GetInteger(actionArgument, 0);
        switch (actionName.ToLowerCSafe())
        {
            // Undo kick action
            case "undokick":
                SessionManager.RemoveUserFromKicked(userId);
                PrepareUnigrid();
                gridElem.ReBind();
                ShowConfirmation(GetString("kicked.cancel"));
                break;
        }
    }


    /// <summary>
    ///  On external databound event.
    /// </summary>
    private object gridElem_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        switch (sourceName)
        {
            case "formattedusername":
                return GetFormattedUsername(parameter);

            case "undokick":
                return ModifyUndoKickButton(sender, parameter);

            default:
                return "";
        }
    }


    /// <summary>
    /// Grid definition loaded.
    /// </summary>
    void gridElem_OnLoadColumns()
    {
        gridElem.GridActions.Actions.RemoveAt(2);
        gridElem.GridActions.Actions.RemoveAt(0);
    }

    #endregion


    #region "Unigrid  data bound methods"

    /// <summary>
    /// Get formatted user name.
    /// </summary>
    private string GetFormattedUsername(object parameter)
    {
        var drv = (DataRowView)parameter;
        if (drv != null)
        {
            var ui = new UserInfo(drv.Row);
            string userName = Functions.GetFormattedUserName(ui.UserName);
            if (AuthenticationHelper.UserKicked(ui.UserID))
            {
                return HTMLHelper.HTMLEncode(userName) + " <span style=\"color:#ee0000;\">" + GetString("administration.users.onlineusers.kicked") + "</span>";
            }

            return HTMLHelper.HTMLEncode(userName);
        }
        return "";
    }


    /// <summary>
    /// Displays button for undo kicked users.
    /// </summary>
    private object ModifyUndoKickButton(object sender, object parameter)
    {
        int userID = ValidationHelper.GetInteger(((DataRowView)((GridViewRow)parameter).DataItem).Row["UserID"], 0);
        var button = (CMSGridActionButton)sender;
        button.Enabled = AuthenticationHelper.UserKicked(userID);

        return "";
    }

    #endregion


    #region "Private methods"

    /// <summary>
    /// Set header actions.
    /// </summary>
    private void SetHeaderActions()
    {
        HeaderAction action = new HeaderAction();
        action.Text = GetString("General.Refresh");
        action.RedirectUrl = RequestContext.CurrentURL;
        CurrentMaster.HeaderActions.AddAction(action);
    }


    /// <summary>
    /// Prepares unigrid.
    /// </summary>
    private void PrepareUnigrid()
    {
        gridElem.ObjectType = "cms.userlist";
        gridElem.WhereCondition = DataHelper.GetNotEmpty(SessionManager.GetKickedUsersWhere(), "1 = 0");

        // Setup unigrid events
        gridElem.OnAction += gridElem_OnAction;
        gridElem.OnBeforeDataReload += gridElem_OnBeforeDataReload;
        gridElem.OnExternalDataBound += gridElem_OnExternalDataBound;
        gridElem.ZeroRowsText = GetString("general.nodatafound");
        gridElem.OnLoadColumns += gridElem_OnLoadColumns;
    }

    #endregion
}