using System;
using System.Data;
using System.Text;
using System.Web.UI.WebControls;

using CMS.Core;
using CMS.Helpers;
using CMS.LicenseProvider;
using CMS.Base;
using CMS.PortalEngine;
using CMS.SiteProvider;
using CMS.Membership;
using CMS.UIControls;
using CMS.ExtendedControls.ActionsConfig;
using CMS.DataEngine;
using CMS.Modules;
using CMS.ExtendedControls;

public partial class CMSModules_Membership_Pages_Users_General_User_Online : CMSUsersPage
{
    #region "Variables"

    protected bool isInitiateChatEnabled;
    private string mSiteName;
    private bool? mOnlineMarketingEnabled;
    private IUserFilter filter;

    #endregion


    #region "Properties"

    /// <summary>
    /// Gets current site name.
    /// </summary>
    private new string SiteName
    {
        get
        {
            if (String.IsNullOrEmpty(mSiteName))
            {
                if (SiteID > 0)
                {
                    SiteInfo si = SiteInfoProvider.GetSiteInfo(SiteID);
                    if (si != null)
                    {
                        mSiteName = si.SiteName;
                    }
                }
            }
            return mSiteName;
        }
    }


    /// <summary>
    /// Indicates if online marketing is enabled.
    /// </summary>
    private bool OnlineMarketingEnabled
    {
        get
        {
            if (mOnlineMarketingEnabled == null)
            {
                mOnlineMarketingEnabled = LicenseHelper.CheckFeature(RequestContext.CurrentDomain, FeatureEnum.ContactManagement) &&
                    ResourceSiteInfoProvider.IsResourceOnSite("CMS.ContactManagement", SiteContext.CurrentSiteName) &&
                    SettingsKeyInfoProvider.GetBoolValue(SiteContext.CurrentSiteName + ".CMSEnableOnlineMarketing");

            }
            return (bool)mOnlineMarketingEnabled;
        }
    }


    /// <summary>
    /// Indicates if contacts can be displayed.
    /// </summary>
    private bool DisplayContacts
    {
        get
        {
            return SessionManager.StoreOnlineUsersInDatabase
             && ResourceSiteInfoProvider.IsResourceOnSite("CMS.ContactManagement", SiteContext.CurrentSiteName)
             && ModuleManager.IsModuleLoaded(ModuleName.CONTACTMANAGEMENT);
        }
    }


    /// <summary>
    /// Indicates if score can be displayed.
    /// </summary>
    private bool DisplayScore
    {
        get
        {
            return CurrentUser.IsAuthorizedPerUIElement("CMS.OnlineMarketing", "Scoring")
                && ResourceSiteInfoProvider.IsResourceOnSite("CMS.Scoring", SiteContext.CurrentSiteName)
                && LicenseHelper.CheckFeature(RequestContext.CurrentDomain, FeatureEnum.LeadScoring)
                && MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("CMS.Scoring", "Read");
        }
    }

    #endregion


    #region "Page events"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        if (DisplayContacts)
        {
            gridElem.GridName = "User_OnlineDB.xml";
            gridElem.OrderBy = "SessionFullName";
            gridElem.Columns = "SessionUserID, SessionFullName, SessionEmail, SessionUserName, SessionNickName, SessionUserCreated, SessionLocation, SessionContactID, SessionSiteID, SessionLastActive";
        }

        // Hide unigrid default filter button
        gridElem.HideFilterButton = true;
        gridElem.LoadGridDefinition();

        // Get custom filter control
        filter = (IUserFilter)gridElem.CustomFilter;

        if (!SessionManager.StoreOnlineUsersInDatabase)
        {
            // Where condition needs to be generated not only when user click "advanced filter show" button
            gridElem.ApplyFilter(this, new EventArgs());
        }

        filter.CurrentMode = "online";
        if (DisplayContacts)
        {
            filter.EnableDisplayingGuests = true;
        }

        if (DisplayContacts)
        {
            filter.SessionInsteadOfUser = true;
        }
        filter.DisplayUserEnabled = false;
        filter.EnableDisplayingHiddenUsers = true;
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        // Check permissions
        LicenseHelper.CheckFeatureAndRedirect(RequestContext.CurrentDomain, FeatureEnum.OnlineUsers);

        isInitiateChatEnabled = CheckInitiateChat();

        // If session management is disabled inform about it
        if (!SessionManager.OnlineUsersEnabled)
        {
            // Set disabled module info
            ucDisabledModule.SettingsKeys = "CMSUseSessionManagement";
            ucDisabledModule.InfoText = GetString("administration.users.online.disabled");

            ucDisabledModule.Visible = true;
            gridElem.Visible = false;
            lblGeneralInfo.Visible = false;
        }
        else
        {
            SetHeaderActions();
            PrepareUnigrid();
        }
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);
        SetCountOfUsers();
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
            if (!user.IsAuthorizedPerUIElement("CMS.OnlineMarketing", new[] { "ContactsFrameset", "On-line_users" }, SiteContext.CurrentSiteName))
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

        bool guestByDefault = QueryHelper.GetBoolean("guest", false);

        if (DisplayContacts)
        {
            gridElem.GridView.Columns[3].Visible = !guestByDefault;
            gridElem.GridView.Columns[4].Visible = !guestByDefault;
            gridElem.GridView.Columns[8].Visible = filter.DisplayGuests;
            gridElem.GridView.Columns[9].Visible = DisplayScore && (filter.SelectedScore > 0);
            gridElem.GridView.Columns[10].Visible = String.IsNullOrEmpty(SiteName) && (filter.SelectedSite <= 0);

            ScriptHelper.RegisterDialogScript(Page);
        }
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
            // Kick action
            case "kick":
                if (IsNotAdminNorAlreadyKicked(userId))
                {
                    SessionManager.KickUser(userId);
                    ShowConfirmation(GetString("kicked.user"));
                }
                break;

            // Undo kick action
            case "undokick":
                SessionManager.RemoveUserFromKicked(userId);
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

            case "fullname":
                return GetFullName(parameter);

            case "kick":
                return ModifyKickedButton(sender, parameter);

            case "undokick":
                return ModifyUndoKickButton(sender, parameter);

            case "initiatechat":
                return ModifyChatButton(sender, parameter);

            case "isguest":
                return GetIsGuest(parameter);

            case "editcontact":
                return ModifyContactButton(sender, parameter);

            default:
                return "";
        }
    }


    DataSet gridElem_OnAfterRetrieveData(DataSet ds)
    {
        if (!DataHelper.DataSourceIsEmpty(ds) && (filter != null) && (filter.SelectedScore > 0))
        {
            return GetScore(ds);
        }
        return ds;
    }

    #endregion


    #region "Unigrid data bound methods"

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
    /// Returns full name of the visitor.
    /// </summary>
    private object GetFullName(object parameter)
    {
        string namePlaceholder = GetString("usersonline.anonymous");
        if (parameter == DBNull.Value)
        {
            return namePlaceholder;
        }

        return HTMLHelper.HTMLEncode(parameter.ToString(namePlaceholder));
    }


    /// <summary>
    /// Displays button for kicking users.
    /// </summary>
    private object ModifyKickedButton(object sender, object parameter)
    {
        bool? userIsAdmin = null;
        int userID;
        if (DisplayContacts)
        {
            userID = ValidationHelper.GetInteger(((DataRowView)((GridViewRow)parameter).DataItem).Row["SessionUserID"], 0);
        }
        else
        {
            userID = ValidationHelper.GetInteger(((DataRowView)((GridViewRow)parameter).DataItem).Row["UserID"], 0);
            userIsAdmin = ValidationHelper.GetBoolean(((DataRowView)((GridViewRow)parameter).DataItem).Row["UserIsGlobalAdministrator"], false);
        }

        var button = ((CMSGridActionButton)sender);
        button.Enabled = IsNotAdminNorAlreadyKicked(userID, userIsAdmin);

        return "";
    }


    /// <summary>
    /// Displays button for undo kicked users.
    /// </summary>
    private object ModifyUndoKickButton(object sender, object parameter)
    {
        int userID = ValidationHelper.GetInteger(((DataRowView)((GridViewRow)parameter).DataItem).Row["UserID"], 0);

        var button = ((CMSGridActionButton)sender);
        button.Enabled = AuthenticationHelper.UserKicked(userID);

        return "";
    }


    /// <summary>
    /// Display chat button.
    /// </summary>
    private object ModifyChatButton(object sender, object parameter)
    {
        CMSGridActionButton button = ((CMSGridActionButton)sender);

        if (!isInitiateChatEnabled)
        {
            button.Visible = false;
        }
        else
        {
            DataRow row = ((DataRowView)((GridViewRow)parameter).DataItem).Row;

            int contactID;
            int userID;

            if (DisplayContacts)
            {
                contactID = ValidationHelper.GetInteger(row["SessionContactID"], 0);
                userID = ValidationHelper.GetInteger(row["SessionUserID"], 0);
            }
            else
            {
                contactID = 0;
                userID = ValidationHelper.GetInteger(row["UserID"], 0);
            }

            // If userID and contactID are not known (can happen if online marketing is turned off), hide initiate chat button
            if ((userID <= 0) && (contactID <= 0))
            {
                button.Visible = false;
            }
            else
            {
                button.OnClientClick = string.Format("if (window.top.ChatSupportManager) {{window.top.ChatSupportManager.initiateChat({0}, {1});}} return false;", userID, contactID);
            }
        }
        return "";
    }


    /// <summary>
    /// Modify contact button.
    /// </summary>
    private object ModifyContactButton(object sender, object parameter)
    {
        var button = ((CMSGridActionButton)sender);
        int contactID = ValidationHelper.GetInteger(((DataRowView)((GridViewRow)parameter).DataItem).Row["SessionContactID"], 0);
        if ((contactID > 0) && OnlineMarketingEnabled)
        {
            string contactURL = UIContextHelper.GetElementDialogUrl(ModuleName.ONLINEMARKETING, "EditContact", contactID);
            button.OnClientClick = ScriptHelper.GetModalDialogScript(contactURL, "ContactDetail");
        }
        else
        {
            button.Visible = false;
        }
        return "";
    }


    /// <summary>
    /// Returns if online user is guest.
    /// </summary>
    private object GetIsGuest(object parameter)
    {
        int userID = ValidationHelper.GetInteger(parameter, 0);
        if (userID == 0)
        {
            return GetString("general.yes");
        }
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
    /// Sets number of online users.
    /// </summary>
    private void SetCountOfUsers()
    {
        int publicUsers;
        int authenticatedUsers;
        SessionManager.GetUsersNumber(SiteName, null, true, false, out publicUsers, out authenticatedUsers);
        lblGeneralInfo.Text =
            String.Format(GetString("OnlineUsers.GeneralInfo"), publicUsers + authenticatedUsers, publicUsers,
                          authenticatedUsers) + "<br /><br />";
    }


    /// <summary>
    /// Prepares unigrid.
    /// </summary>
    private void PrepareUnigrid()
    {
        // Online users in DB
        if (DisplayContacts)
        {
            gridElem.ObjectType = "cms.onlineuser";
        }
        // On-line users in hashtable
        else
        {
            string usersWhere = ValidationHelper.GetString(SessionManager.GetUsersWhereCondition(null, SiteName, true, true), String.Empty);

            if (!String.IsNullOrEmpty(usersWhere))
            {
                gridElem.ObjectType = "cms.userlist";
                gridElem.WhereCondition = usersWhere;
            }
            else
            {
                // Clear the object type
                gridElem.ObjectType = string.Empty;
            }
        }

        // Setup unigrid events
        gridElem.OnAction += gridElem_OnAction;
        gridElem.OnBeforeDataReload += gridElem_OnBeforeDataReload;
        gridElem.OnExternalDataBound += gridElem_OnExternalDataBound;
        gridElem.ZeroRowsText = GetString("general.nodatafound");
        gridElem.OnAfterRetrieveData += gridElem_OnAfterRetrieveData;

    }


    /// <summary>
    /// Gets score for specified contacts.
    /// </summary>
    private DataSet GetScore(DataSet ds)
    {
        ds.Tables[0].Columns.Add("Score");

        StringBuilder contacts = new StringBuilder();
        foreach (DataRow dr in ds.Tables[0].Rows)
        {
            int contactID = ValidationHelper.GetInteger(dr["SessionContactID"], 0);
            if (contactID > 0)
            {
                contacts.Append(contactID + ",");
            }
        }
        string contactList = contacts.ToString();
        if (!String.IsNullOrEmpty(contactList))
        {
            contactList = contactList.Substring(0, contactList.Length - 1);
            GeneralizedInfo contactScore = ModuleManager.GetReadOnlyObject("om.contactscorelist");
            DataSet result = contactScore.GetData(null, String.Format("ContactID IN ({0}) AND ScoreID = {1}", contactList, filter.SelectedScore), null, -1, "ContactID,ScoreValue", false);

            if (!DataHelper.DataSourceIsEmpty(result))
            {
                foreach (DataRow dr in result.Tables[0].Rows)
                {
                    DataRow[] row = ds.Tables[0].Select("SessionContactID =" + dr["ContactID"]);
                    row[0]["Score"] = dr["ScoreValue"];
                }
            }
        }
        return ds;
    }


    /// <summary>
    /// Indicates whether does it make sense to kick the user with given ID. One does not kick global administrators.
    /// </summary>
    /// <param name="userID">ID of the user to be kicked.</param>
    /// <param name="isAdmin">User with given ID is global administrator. If null is passed, value is retrieved from the database.</param>
    private bool IsNotAdminNorAlreadyKicked(int userID, bool? isAdmin = null)
    {
        if ((isAdmin == null) && (userID > 0))
        {
            UserInfo user = UserInfoProvider.GetUserInfo(userID);
            if (user != null)
            {
                isAdmin = user.UserIsGlobalAdministrator;
            }
        }

        return (userID > 0) && !(isAdmin ?? false) && !AuthenticationHelper.UserKicked(userID);
    }

    #endregion
}