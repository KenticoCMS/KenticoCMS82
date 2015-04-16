using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.DataEngine;
using CMS.Helpers;
using CMS.Membership;
using CMS.PortalEngine;
using CMS.ProjectManagement;
using CMS.Base;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.DocumentEngine;
using CMS.ExtendedControls;
using CMS.ExtendedControls.ActionsConfig;

public partial class CMSModules_ProjectManagement_Controls_LiveControls_ProjectList : CMSAdminControl
{
    #region "Variables"

    private int mProjectID = -1;
    private ProjectInfo mProjectObj = null;
    private PageInfo pi = null;

    #endregion


    #region "Properties"

    /// <summary>
    /// Gets or sets the permission for creating new project.
    /// </summary>
    public string ProjectAccess
    {
        get
        {
            return ValidationHelper.GetString(GetValue("ProjectAccess"), "nobody");
        }
        set
        {
            SetValue("ProjectAccess", value);
            ucProjectEdit.ProjectAccess = value;
        }
    }


    /// <summary>
    /// Gest or sets the role names separated by semicolon which are authorized to create new project.
    /// </summary>
    public string AuthorizedRoles
    {
        get
        {
            return ValidationHelper.GetString(GetValue("AuthorizedRoles"), string.Empty);
        }
        set
        {
            SetValue("AuthorizedRoles", value);
            ucProjectEdit.AuthorizedRoles = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether finished projects should be displayed.
    /// </summary>
    public bool ShowFinishedProjects
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ShowFinishedProjects"), true);
        }
        set
        {
            SetValue("ShowFinishedProjects", value);
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
    /// Gets or sets the number of items per page.
    /// </summary>
    public int PageSize
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("PageSize"), 10);
        }
        set
        {
            SetValue("PageSize", value);
        }
    }


    /// <summary>
    /// Gets or sets the display mode of the control.
    /// </summary>
    public override ControlDisplayModeEnum DisplayMode
    {
        get
        {
            return base.DisplayMode;
        }
        set
        {
            base.DisplayMode = value;
            EnsureChildControls();
            ucProjectNew.DisplayMode = value;
            ucProjectList.DisplayMode = value;
            ucProjectEdit.DisplayMode = value;
        }
    }


    /// <summary>
    /// Gets the current project object if available.
    /// </summary>
    public ProjectInfo ProjectObj
    {
        get
        {
            if (mProjectObj == null)
            {
                ProjectInfo proj = ProjectInfoProvider.GetProjectInfo(ProjectID);
                if (proj != null)
                {
                    // If finished projects shouldn't be displayed
                    if ((!ShowFinishedProjects))
                    {
                        // Check whether current project status isn't in finished status and if so return null
                        ProjectStatusInfo projStat = ProjectStatusInfoProvider.GetProjectStatusInfo(proj.ProjectStatusID);
                        if ((projStat != null) && (projStat.StatusIsFinished))
                        {
                            return null;
                        }
                    }

                    // Check whether project is assigned to current document
                    if (proj.ProjectNodeID != DocumentContext.CurrentPageInfo.NodeID)
                    {
                        return null;
                    }

                    // Check whether user is authorized to see project details
                    if (IsAuthorizedPerCreateProject() || ProjectInfoProvider.IsAuthorizedPerProject(ProjectID, ProjectManagementPermissionType.READ, MembershipContext.AuthenticatedUser))
                    {
                        // If security is OK, keep project info
                        mProjectObj = proj;
                    }
                }
            }
            return mProjectObj;
        }
    }


    /// <summary>
    /// Gets the project id defined in querystring.
    /// </summary>
    public int ProjectID
    {
        get
        {
            if (mProjectID == -1)
            {
                mProjectID = QueryHelper.GetInteger("projectid", 0);
                if (mProjectID < 0)
                {
                    mProjectID = 0;
                }
            }
            return mProjectID;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether control is displayed on live site.
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
            EnsureChildControls();
            ucProjectNew.IsLiveSite = value;
            ucProjectList.IsLiveSite = value;
            ucProjectEdit.IsLiveSite = value;
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// OnLoad - base initialization.
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        // Set current page info
        pi = DocumentContext.CurrentPageInfo;

        // Display both controls by default
        plcProjectList.Visible = true;
        plcEdit.Visible = true;

        // Build condition where condition
        ucProjectList.BuildCondition += ucProjectList_BuildCondition;


        #region "Mode settings"

        // List view
        if (ProjectObj == null)
        {
            // If page info doesn't exist => control can't work => hide
            if (pi == null)
            {
                Visible = false;
                return;
            }

            // Set list properties
            ucProjectList.ProjectNodeID = pi.NodeID;
            ucProjectList.CommunityGroupID = pi.NodeGroupID;
            ucProjectList.EditPageURL = RequestContext.CurrentURL;

            ucProjectEdit.CommunityGroupID = pi.NodeGroupID;
            ucProjectNew.CommunityGroupID = pi.NodeGroupID;

            // Title settings
            titleElem.TitleText = GetString("pm.project.new");
            // Set OnSaved event handler
            ucProjectNew.OnSaved += ucProjectNew_OnSaved;

            // Hide edit part
            plcEdit.Visible = false;
        }
        // Edit view
        else
        {
            InitializeBreadcrumbs();

            // Set current project ID
            ucProjectEdit.ProjectID = ProjectObj.ProjectID;
            // Set community group id
            ucProjectEdit.CommunityGroupID = ProjectObj.ProjectGroupID;

            // Hide list part
            plcProjectList.Visible = false;
        }

        #endregion


        #region "New project link"

        // Check whether new project action should be displayed
        if (IsAuthorizedPerCreateProject())
        {
            // New item link
            actionsElem.AddAction(new HeaderAction
            { 
                Text = GetString("pm.project.new"),
                CommandName = "new_project"
            });
            actionsElem.ActionPerformed += actionsElem_ActionPerformed;
        }
        // If user is not authorized to create new project hide new project link
        else
        {
            pnlListActions.Visible = false;
        }

        #endregion


        SetupConditions();

        // Handle create project check permission
        ucProjectNew.OnCheckPermissions += ucProjectNew_OnCheckPermissions;

        // Handle delete project permission
        ucProjectList.OnCheckPermissions += ucProjectList_OnCheckPermissions;
    }


    /// <summary>
    /// Builds where condition.
    /// </summary>
    private string ucProjectList_BuildCondition(object sender, string whereCondition)
    {
        var cui = MembershipContext.AuthenticatedUser;

        // If finished projects shouldn't be displayed create where condition for selection
        if (!ShowFinishedProjects)
        {
            whereCondition = SqlHelper.AddWhereCondition("StatusIsFinished = 0", whereCondition);
        }

        if (!IsAuthorizedPerCreateProject())
        {
            // Access security
            whereCondition = ProjectInfoProvider.CombineSecurityWhereCondition(whereCondition, MembershipContext.AuthenticatedUser, SiteContext.CurrentSiteName);
        }

        return whereCondition;
    }


    /// <summary>
    /// Sets the conditions for current list.
    /// </summary>
    protected void SetupConditions()
    {
        // Paging
        ucProjectList.EnablePaging = EnablePaging;
        ucProjectList.PageSize = PageSize;
    }


    /// <summary>
    /// Creates child controls and ensures update panel load container.
    /// </summary>
    protected override void CreateChildControls()
    {
        // If project form is not defined, load update panel container
        if (ucProjectNew == null)
        {
            pnlUpdate.LoadContainer();
            pnlUpdateModal.LoadContainer();
        }

        base.CreateChildControls();
    }

    #endregion


    #region "Handler methods"

    /// <summary>
    /// New project link event handler.
    /// </summary>
    private void actionsElem_ActionPerformed(object sender, CommandEventArgs e)
    {
        // Clear project form
        ucProjectNew.ClearForm();
        // Load default data
        ucProjectNew.LoadData();
        // Show popup dialog
        ucPopupDialog.Show();
        // Updade popup update panel
        pnlUpdateModal.Update();
    }


    /// <summary>
    /// Project edit OK button click.
    /// </summary>
    protected void btnOK_Click(object sender, EventArgs e)
    {
        // Check whether page info is defined
        if (pi != null)
        {
            // Set current project ID
            ucProjectNew.ProjectNodeID = pi.NodeID;
            // Set current Group ID
            ucProjectNew.CommunityGroupID = pi.NodeGroupID;
            ucProjectEdit.CommunityGroupID = pi.NodeGroupID;
            // Call save method on project edit form
            ucProjectNew.Save();
            // Show popup dialog if some error occured on project edit form
            ucPopupDialog.Show();
        }
    }


    /// <summary>
    /// OnSave event handler - Redirect to the edit.
    /// </summary>
    private void ucProjectNew_OnSaved(object sender, EventArgs e)
    {
        // Getcurrent URL
        string url = RequestContext.CurrentURL;
        // Add project ID to the querystring
        url = URLHelper.UpdateParameterInUrl(url, "projectid", ucProjectNew.ItemID.ToString());
        // Redirect to the project edit page 
        URLHelper.Redirect(url);
    }


    /// <summary>
    /// Initialize breadcrumb items.
    /// </summary>
    private void InitializeBreadcrumbs()
    {
        string url = RequestContext.CurrentURL;
        url = URLHelper.RemoveParameterFromUrl(url, "projectid");
        url = URLHelper.RemoveParameterFromUrl(url, "taskid");

        ucBreadcrumbs.Items.Clear();

        ucBreadcrumbs.AddBreadcrumb(new BreadcrumbItem {
            Text = GetString("pm.project.list"),
            RedirectUrl = url
        });

        ucBreadcrumbs.AddBreadcrumb(new BreadcrumbItem {
            Text = ResHelper.LocalizeString(ProjectObj.ProjectDisplayName),
        });
    }
    
    #endregion


    #region "Security methods"

    /// <summary>
    /// Checks whether current user can create new project.
    /// </summary>
    protected bool IsAuthorizedPerCreateProject()
    {
        // Keep current user info object
        var cui = MembershipContext.AuthenticatedUser;

        // Global admin is allowed for all actions
        if (cui.IsGlobalAdministrator)
        {
            return true;
        }

        bool result = false;

        // Check whether page info is available, if not, user can't create new project
        if (pi != null)
        {
            // Community admin and group admin can create project on group pages
            if (pi.NodeGroupID > 0)
            {
                result = cui.IsGroupAdministrator(pi.NodeGroupID);
            }
            // Project management admin can create projects on regular pages
            else
            {
                result = cui.IsAuthorizedPerResource("CMS.ProjectManagement", PERMISSION_MANAGE);
            }
        }

        // Try combine with webpart settings
        if (!result)
        {
            return ucProjectEdit.IsAuthorizedPerProjectAccess();
        }

        return result;
    }


    /// <summary>
    ///  Check whether user can create project.
    /// </summary>
    private void ucProjectNew_OnCheckPermissions(string permissionType, CMSAdminControl sender)
    {
        if (!IsAuthorizedPerCreateProject())
        {
            // Set error message to the dialog
            ucProjectNew.SetError(GetString("pm.project.permission"));
            // Stop edit control processing
            sender.StopProcessing = true;

            // Set current project ID
            ucProjectNew.ProjectNodeID = pi.NodeID;

            // Set current Group ID
            ucProjectNew.CommunityGroupID = pi.NodeGroupID;

            // Show popup dialog if some error occured on project edit form
            ucPopupDialog.Show();
        }
    }


    /// <summary>
    /// Check whether user can delete project.
    /// </summary>
    private void ucProjectList_OnCheckPermissions(string permissionType, CMSAdminControl sender)
    {
        // Indicates whether current user is owner
        bool isOwner = false;

        // Get project id
        int projectId = ValidationHelper.GetInteger(sender.GetValue("DeletedItemID"), 0);
        // Check whether project id is valid
        if (projectId > 0)
        {
            // Try get project info
            ProjectInfo proj = ProjectInfoProvider.GetProjectInfo(projectId);

            // Check whethr peoject object exists and if current user is owner
            if ((proj != null) && (proj.ProjectOwner == MembershipContext.AuthenticatedUser.UserID))
            {
                isOwner = true;
            }
        }

        // If user is owner or can create project, allow delete project
        if (!isOwner && !IsAuthorizedPerCreateProject())
        {
            // Show error
            lblError.Visible = true;
            // Set error message
            lblError.Text = GetString("pm.project.permission");
            // Stop edit control processing
            sender.StopProcessing = true;
        }
    }

    #endregion
}