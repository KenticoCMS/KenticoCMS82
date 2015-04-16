using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.ExtendedControls;
using CMS.Globalization;
using CMS.Helpers;
using CMS.ProjectManagement;
using CMS.Base;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.DataEngine;
using CMS.DocumentEngine;

public partial class CMSModules_ProjectManagement_Controls_UI_Project_List : CMSAdminListControl, IPostBackEventHandler
{
    #region "Variables"

    private bool mUsePostbackOnEdit = false;
    private string mEditPageUrl = "Frameset.aspx";
    private int mProjectNodeID = 0;
    private string rowColor = null;
    private bool mRegisterEditScript = true;
    private string mProjectLinkTarget = String.Empty;
    private const int MAX_TITLE_LENGTH = 50;

    /// <summary>
    /// Build condition event.
    /// </summary>
    public delegate string BuildConditionEvent(object sender, string whereCondition);

    /// <summary>
    /// Build condition event is fired when where condition for list control is built.
    /// </summary>
    public event BuildConditionEvent BuildCondition;

    #endregion


    #region "Properties"

    /// <summary>
    /// Gets or sets the target for project link.
    /// </summary>
    public string ProjectLinkTarget
    {
        get
        {
            return mProjectLinkTarget;
        }
        set
        {
            mProjectLinkTarget = value;
        }
    }


    /// <summary>
    /// Gets or sets the edit page url.
    /// </summary>
    public string EditPageURL
    {
        get
        {
            string url = URLHelper.RemoveQuery(mEditPageUrl);
            if (CommunityGroupID > 0 && !IsLiveSite)
            {
                url = URLHelper.UpdateParameterInUrl(url, "groupid", CommunityGroupID.ToString());
                url += "&";
            }
            else
            {
                url += "?";
            }

            return url;
        }
        set
        {
            mEditPageUrl = value;
        }
    }


    /// <summary>
    /// If true control registers edit script.
    /// </summary>
    public bool RegisterEditScript
    {
        get
        {
            return mRegisterEditScript;
        }
        set
        {
            mRegisterEditScript = value;
        }
    }


    /// <summary>
    /// Inner grid.
    /// </summary>
    public UniGrid Grid
    {
        get
        {
            return gridElem;
        }
    }


    /// <summary>
    /// ID of groud where project belongs.
    /// </summary>
    public int CommunityGroupID
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("CommunityGroupID"), 0);
        }
        set
        {
            SetValue("CommunityGroupID", value);
        }
    }


    /// <summary>
    /// ID of document where project belongs.
    /// </summary>
    public int ProjectNodeID
    {
        get
        {
            return mProjectNodeID;
        }
        set
        {
            mProjectNodeID = value;
        }
    }


    /// <summary>
    /// Indicates if the control should perform the operations.
    /// </summary>
    public override bool StopProcessing
    {
        get
        {
            return base.StopProcessing;
        }
        set
        {
            base.StopProcessing = value;
            gridElem.StopProcessing = value;
        }
    }


    /// <summary>
    /// Indicates if the control is used on the live site.
    /// </summary>
    public override bool IsLiveSite
    {
        get
        {
            return base.IsLiveSite;
        }
        set
        {
            base.IsLiveSite = value;
            gridElem.IsLiveSite = value;
        }
    }


    /// <summary>
    /// Indicates if use postback on edit.
    /// </summary>
    public bool UsePostbackOnEdit
    {
        get
        {
            return mUsePostbackOnEdit;
        }
        set
        {
            mUsePostbackOnEdit = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether community group should be used for filtering
    /// This option is used for My projects web part
    /// </summary>
    public bool IgnoreCommunityGroup
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("IgnoreCommunityGroup"), false);
        }
        set
        {
            SetValue("IgnoreCommunityGroup", value);
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether paging should be enabled.
    /// </summary>
    public bool EnablePaging
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("EnablePaging"), true);
        }
        set
        {
            SetValue("EnablePaging", value);
        }
    }


    /// <summary>
    /// Sets the number of items per page.
    /// </summary>
    public int PageSize
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("PageSize"), 25);
        }
        set
        {
            SetValue("PageSize", value);

            // Pager settings
            if (!EnablePaging)
            {
                gridElem.PageSize = "##ALL##";
                gridElem.Pager.DefaultPageSize = -1;
            }
            else
            {
                gridElem.Pager.DefaultPageSize = value;
                gridElem.PageSize = value.ToString();
                gridElem.FilterLimit = value;
            }
        }
    }

    #endregion


    #region "Page methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Grid initialization                
        gridElem.OnAction += new OnActionEventHandler(gridElem_OnAction);
        gridElem.OnExternalDataBound += new OnExternalDataBoundEventHandler(gridElem_OnExternalDataBound);
        gridElem.GridView.RowDataBound += new GridViewRowEventHandler(GridView_RowDataBound);
        gridElem.GridView.RowCreated += new GridViewRowEventHandler(GridView_RowCreated);
        gridElem.ZeroRowsText = GetString("pm.projects.noprojectsfound");


        // Show projects of the current Site
        gridElem.WhereCondition = SqlHelper.AddWhereCondition(gridElem.WhereCondition, "ProjectSiteID = " + SiteContext.CurrentSiteID);

        //Project list webpart condition - usually used in project list
        if (ProjectNodeID != 0)
        {
            gridElem.WhereCondition = SqlHelper.AddWhereCondition(gridElem.WhereCondition, "ProjectNodeID =" + ProjectNodeID);
        }

        // Check whether community group should be checked
        if (!IgnoreCommunityGroup)
        {
            // Check groupId        
            if (CommunityGroupID > 0)
            {
                gridElem.WhereCondition = SqlHelper.AddWhereCondition(gridElem.WhereCondition, "ProjectGroupID=" + CommunityGroupID);
            }
            else
            {
                gridElem.WhereCondition = SqlHelper.AddWhereCondition(gridElem.WhereCondition, "ProjectGroupID IS NULL");
            }
        }

        QueryDataParameters parameters = new QueryDataParameters();
        parameters.Add("@Now", DateTime.Now);
        gridElem.QueryParameters = parameters;

        gridElem.WhereCondition = RaiseBuildCondition();
    }


    /// <summary>
    /// OnPreRender - reload list data.
    /// </summary>
    protected override void OnPreRender(EventArgs e)
    {
        if (RegisterEditScript)
        {
            ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "EditRedirectScript", ScriptHelper.GetScript(@"    function EditProject(id) {
                            var usePostback = '" + UsePostbackOnEdit + @"';
                            if (usePostback == 'False') {            
                            var url = " + ScriptHelper.GetString(EditPageURL) + @" 
                            url = url + 'projectid=' + id;
            
                            window.location.replace(url);
                            return false;           
                        }
                        return true;
                        }
                    "));
        }

        base.OnPreRender(e);
    }


    /// <summary>
    /// Raises build condition event.
    /// </summary>
    public string RaiseBuildCondition()
    {
        if (BuildCondition != null)
        {
            return BuildCondition(this, gridElem.WhereCondition);
        }

        return gridElem.WhereCondition;
    }

    #endregion


    #region "Grid events"

    /// <summary>
    /// Handles the RowCreated event of the GridView control.
    /// </summary>
    /// <param name="sender">The source of the event</param>
    /// <param name="e">The <see cref="System.Web.UI.WebControls.GridViewRowEventArgs"/> instance containing the event data</param>
    private void GridView_RowCreated(object sender, GridViewRowEventArgs e)
    {
        rowColor = null;
    }


    /// <summary>
    /// Handles the RowDataBound event of the GridView control.
    /// </summary>
    /// <param name="sender">The source of the event</param>
    /// <param name="e">The <see cref="System.Web.UI.WebControls.GridViewRowEventArgs"/> instance containing the event data</param>
    private void GridView_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            if (!String.IsNullOrEmpty(rowColor))
            {
                e.Row.Attributes.Add("style", "background-color: " + rowColor);
            }
        }
    }


    /// <summary>
    /// Handles UniGrid's OnExternalDataBound event.
    /// </summary>
    /// <param name="sender">Sender object (image button if it is an external databoud for action button)</param>
    /// <param name="sourceName">External source name of the column specified in the grid XML</param>
    /// <param name="parameter">Source parameter (original value of the field)</param>
    protected object gridElem_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        DataRowView row = null;

        switch (sourceName.ToLowerCSafe())
        {
            case "projectdisplayname":
                string displayname = parameter.ToString();
                if (displayname.Length > MAX_TITLE_LENGTH)
                {
                    return "<span title=\"" + HTMLHelper.HTMLEncode(displayname) + "\">" + HTMLHelper.HTMLEncode(TextHelper.LimitLength(displayname, MAX_TITLE_LENGTH)) + "</span>";
                }
                else
                {
                    return HTMLHelper.HTMLEncode(displayname);
                }

            case "projectprogress":
                row = (DataRowView)parameter;
                int progress = ValidationHelper.GetInteger(row["ProjectProgress"], 0);
                return ProjectTaskInfoProvider.GenerateProgressHtml(progress, true);

            case "statusicon":
                row = (DataRowView)parameter;
                string statusText = ValidationHelper.GetString(row["ProjectStatus"], "");
                statusText = HTMLHelper.HTMLEncode(statusText);
                string iconUrl = ValidationHelper.GetString(row["ProjectStatusIcon"], "");
                // Get row color
                rowColor = ValidationHelper.GetString(row["ProjectStatusColor"], "");

                if (!String.IsNullOrEmpty(iconUrl))
                {
                    return "<div style=\"text-align:center;\"><img alt=\"" + statusText + "\" title=\"" + statusText + "\" src=\"" + HTMLHelper.HTMLEncode(GetImageUrl(iconUrl)) + "\" style=\"max-width:50px; max-height: 50px;\"  /></div>";
                }
                return "";

            case "ownerfullname":
                row = (DataRowView)parameter;
                string userName = ValidationHelper.GetString(row["ProjectOwnerUserName"], String.Empty);
                string fullName = ValidationHelper.GetString(row["ProjectOwnerFullName"], String.Empty);
                return HTMLHelper.HTMLEncode(Functions.GetFormattedUserName(userName, fullName, IsLiveSite));

            case "projectdeadline":
                DateTime dt = ValidationHelper.GetDateTime(parameter, DateTimeHelper.ZERO_TIME);
                if (dt == DateTimeHelper.ZERO_TIME)
                {
                    return String.Empty;
                }
                return TimeZoneMethods.ConvertDateTime(dt, this);

            case "linkeddisplayname":
                row = (DataRowView)parameter;
                string displayName = HTMLHelper.HTMLEncode(ValidationHelper.GetString(row["ProjectDisplayName"], String.Empty));
                string path = ValidationHelper.GetString(row["NodeAliasPath"], String.Empty);
                int projectId = ValidationHelper.GetInteger(row["ProjectID"], 0);
                if (!String.IsNullOrEmpty(path))
                {
                    string target = String.Empty;
                    if (!String.IsNullOrEmpty(ProjectLinkTarget))
                    {
                        target = " target=\"" + ProjectLinkTarget + "\" ";
                    }

                    path = URLHelper.ResolveUrl(DocumentURLProvider.GetUrl(path));
                    return String.Format("<a href=\"{1}?projectid={2}\" {3} title=\"{4}\">{0}</a>", TextHelper.LimitLength(displayName, MAX_TITLE_LENGTH), path, projectId, target, HTMLHelper.HTMLEncode(displayName));
                }
                else
                {
                    return displayName;
                }

            case "editlinkdisplayname":
                row = (DataRowView)parameter;
                string editDisplayName = ValidationHelper.GetString(row["ProjectDisplayName"], String.Empty);
                editDisplayName = TextHelper.LimitLength(editDisplayName, MAX_TITLE_LENGTH);
                int editProjectTaskID = ValidationHelper.GetInteger(row["ProjectID"], 0);
                return String.Format("<a href=\"javascript:" + ControlsHelper.GetPostBackEventReference(this, editProjectTaskID.ToString()) + "\" title=\"{0}\">{1}</a>", HTMLHelper.HTMLEncode(editDisplayName), HTMLHelper.HTMLEncode(TextHelper.LimitLength(editDisplayName, MAX_TITLE_LENGTH)));
        }

        return parameter;
    }


    /// <summary>
    /// Handles UniGrid's OnAction event.
    /// </summary>
    /// <param name="actionName">Name of the action which should be performed</param>
    /// <param name="actionArgument">ID of the item the action should be performed with</param>
    protected void gridElem_OnAction(string actionName, object actionArgument)
    {
        RaiseOnAction(actionName, actionArgument);
        int projectId = ValidationHelper.GetInteger(actionArgument, 0);
        if (projectId > 0)
        {
            switch (actionName.ToLowerCSafe())
            {
                case "edit":
                    SelectedItemID = projectId;
                    RaiseOnEdit();
                    break;

                case "delete":
                    // Set deleted item id
                    SetValue("DeletedItemID", projectId);
                    if (CheckPermissions("CMS.ProjectManagement", PERMISSION_MANAGE))
                    {
                        // Use try/catch due to license check
                        try
                        {
                            // Delete the object
                            ProjectInfoProvider.DeleteProjectInfo(projectId);
                            RaiseOnDelete();
                        }
                        catch (Exception ex)
                        {
                            ShowError(ex.Message);
                        }
                    }
                    break;
            }
        }
    }

    #endregion


    #region "IPostBackEventHandler Members"

    public void RaisePostBackEvent(string eventArgument)
    {
        int ID = ValidationHelper.GetInteger(eventArgument, 0);
        if (ID != 0)
        {
            SelectedItemID = ID;
            RaiseOnEdit();
        }
    }

    #endregion
}