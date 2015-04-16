using System;
using System.Data;
using System.Linq;
using System.Web.UI.WebControls;

using CMS;
using CMS.Base;
using CMS.DataEngine;
using CMS.ExtendedControls;
using CMS.Helpers;
using CMS.Membership;
using CMS.UIControls;


[assembly: RegisterCustomClass("UserListExtender", typeof(UserListExtender))]

/// <summary>
/// User list unigrid extender
/// </summary>
public class UserListExtender : ControlExtender<UniGrid>
{
    #region "Variables"

    private CurrentUserInfo currentUser;

    #endregion


    #region "Properties"

    /// <summary>
    /// Gets the current user info.
    /// </summary>
    protected CurrentUserInfo CurrentUserObj
    {
        get
        {
            return currentUser ?? (currentUser = MembershipContext.AuthenticatedUser);
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// OnInit event.
    /// </summary>
    public override void OnInit()
    {
        Control.Columns = "UserID, UserName, FullName, Email, UserNickName, UserCreated, UserEnabled, (CASE WHEN UserPassword IS NULL OR UserPassword = '' THEN 0 ELSE 1 END) AS UserHasPassword, UserIsGlobalAdministrator, UserIsExternal";
        Control.IsLiveSite = false;

        Control.HideFilterButton = true;

        // Register scripts
        ScriptHelper.RegisterDialogScript(Control.Page);
        ScriptHelper.RegisterClientScriptBlock(Control.Page, typeof(string), "ManageRoles", ScriptHelper.GetScript(
            "function manageRoles(userId) {" +
            "    modalDialog('" + URLHelper.ResolveUrl("~/CMSModules/Membership/Pages/Users/User_ManageRoles.aspx") + "?userId=' + userId, 'ManageUserRoles', 800, 440);" +
            "}"));

        Control.OnExternalDataBound += OnExternalDataBound;
        Control.OnBeforeDataReload += OnBeforeDataReload;
        Control.OnAction += OnAction;
        Control.PreRender += Control_PreRender;
    }


    /// <summary>
    /// PreRender event.
    /// </summary>
    void Control_PreRender(object sender, EventArgs e)
    {
        // Force data reload
        Control.WhereCondition = Control.GetFilter();
        Control.ReloadData();
    }


    /// <summary>
    /// Handles setting the grid where condition before data binding.
    /// </summary>    
    protected void OnBeforeDataReload()
    {
        if (Control.QueryParameters == null)
        {
            Control.QueryParameters = new QueryDataParameters();
        }
        Control.QueryParameters.Add("@Now", DateTime.Now);
    }


    /// <summary>
    /// Handles external databound event of unigrid.
    /// </summary>
    protected object OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        int userID;
        bool isUserAdministrator;
        switch (sourceName.ToLowerCSafe())
        {
            case "userenabled":
                return UniGridFunctions.ColoredSpanYesNo(parameter);

            case "edit":
                // Edit action                
                userID = ValidationHelper.GetInteger(((DataRowView)((GridViewRow)parameter).DataItem).Row["UserID"], 0);
                isUserAdministrator = ValidationHelper.GetBoolean(((DataRowView)((GridViewRow)parameter).DataItem).Row["UserIsGlobalAdministrator"], false);
                if (!CurrentUserObj.CheckPrivilegeLevel(UserPrivilegeLevelEnum.GlobalAdmin) && isUserAdministrator && (userID != CurrentUserObj.UserID))
                {
                    CMSGridActionButton button = ((CMSGridActionButton)sender);
                    button.Enabled = false;
                }

                break;

            case "delete":
                // Delete action
                isUserAdministrator = ValidationHelper.GetBoolean(((DataRowView)((GridViewRow)parameter).DataItem).Row["UserIsGlobalAdministrator"], false);
                if (!CurrentUserObj.CheckPrivilegeLevel(UserPrivilegeLevelEnum.GlobalAdmin) && isUserAdministrator)
                {
                    CMSGridActionButton button = ((CMSGridActionButton)sender);
                    button.Enabled = false;
                }
                break;

            case "roles":
                // Roles action
                userID = ValidationHelper.GetInteger(((DataRowView)((GridViewRow)parameter).DataItem).Row["UserID"], 0);
                isUserAdministrator = ValidationHelper.GetBoolean(((DataRowView)((GridViewRow)parameter).DataItem).Row["UserIsGlobalAdministrator"], false);

                if (!CurrentUserObj.CheckPrivilegeLevel(UserPrivilegeLevelEnum.GlobalAdmin) && isUserAdministrator && (userID != CurrentUserObj.UserID))
                {
                    CMSGridActionButton button = ((CMSGridActionButton)sender);
                    button.Enabled = false;
                }

                break;

            case "haspassword":
                // Has password action
                {
                    CMSGridActionButton button = ((CMSGridActionButton)sender);

                    if (!CurrentUserObj.IsGlobalAdministrator)
                    {
                        button.Visible = false;
                    }
                    else
                    {
                        bool isExternal = ValidationHelper.GetBoolean(((DataRowView)((GridViewRow)parameter).DataItem).Row["UserIsExternal"], false);
                        bool isPublic = ValidationHelper.GetString(((DataRowView)((GridViewRow)parameter).DataItem).Row["UserName"], string.Empty).EqualsCSafe("public", true);
                        bool hasPassword = ValidationHelper.GetBoolean(((DataRowView)((GridViewRow)parameter).DataItem).Row["UserHasPassword"], true);

                        button.OnClientClick = "return false;";
                        button.Visible = !hasPassword && !isPublic && !isExternal;
                    }
                }
                break;

            case "formattedusername":
                return HTMLHelper.HTMLEncode(Functions.GetFormattedUserName(Convert.ToString(parameter)));

            case "#objectmenu":
                userID = ValidationHelper.GetInteger(((DataRowView)((GridViewRow)parameter).DataItem).Row["UserID"], 0);
                isUserAdministrator = ValidationHelper.GetBoolean(((DataRowView)((GridViewRow)parameter).DataItem).Row["UserIsGlobalAdministrator"], false);
                if (!CurrentUserObj.CheckPrivilegeLevel(UserPrivilegeLevelEnum.GlobalAdmin) && isUserAdministrator && (userID != CurrentUserObj.UserID))
                {
                    CMSGridActionButton button = ((CMSGridActionButton)sender);
                    button.Visible = false;
                }
                break;
        }
        return parameter;
    }


    /// <summary>
    /// Handles the UniGrid's OnAction event.
    /// </summary>
    /// <param name="actionName">Name of item (button) that threw event</param>
    /// <param name="actionArgument">ID (value of Primary key) of corresponding data row</param>
    protected void OnAction(string actionName, object actionArgument)
    {
        if (actionName == "delete")
        {
            // Check "modify" permission
            if (!MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("CMS.Users", "Modify"))
            {
                CMSPage.RedirectToAccessDenied("CMS.Users", "Modify");
            }

            try
            {
                int userId = Convert.ToInt32(actionArgument);
                UserInfo delUser = UserInfoProvider.GetUserInfo(userId);
                var CurrentUserObj = MembershipContext.AuthenticatedUser;

                if (delUser != null)
                {
                    // Global administrator account could be deleted only by global administrator
                    if (delUser.IsGlobalAdministrator && !CurrentUserObj.CheckPrivilegeLevel(UserPrivilegeLevelEnum.GlobalAdmin))
                    {
                        ((CMSPage)Control.Page).ShowError(ResHelper.GetString("Administration-User_List.ErrorNoGlobalAdmin"));
                        return;
                    }

                    // It is not possible to delete own user account
                    if (userId == CurrentUserObj.UserID)
                    {
                        ((CMSPage)Control.Page).ShowError(ResHelper.GetString("Administration-User_List.ErrorOwnAccount"));
                        return;
                    }

                    SessionManager.RemoveUser(userId);
                    UserInfoProvider.DeleteUser(delUser.UserName);
                }
            }
            catch (Exception ex)
            {
                ((CMSPage)Control.Page).ShowError(ex.Message);
            }
        }
    }

    #endregion
}