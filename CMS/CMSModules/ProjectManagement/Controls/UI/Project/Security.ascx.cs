using System;
using System.Collections;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.DataEngine;
using CMS.ExtendedControls;
using CMS.Helpers;
using CMS.Membership;
using CMS.Modules;
using CMS.ProjectManagement;
using CMS.SiteProvider;
using CMS.UIControls;

public partial class CMSModules_ProjectManagement_Controls_UI_Project_Security : CMSAdminEditControl, IPostBackEventHandler
{
    #region "Variables"

    private int mProjectId;
    private bool mProcess = true;
    private bool mCreateMatrix;
    private bool mStopProcessing = false;
    private bool mEnable = true;

    private string[] mAllowedPermissions = new string[4]
                                              {
                                                  ProjectManagementPermissionType.READ,
                                                  ProjectManagementPermissionType.CREATE,
                                                  ProjectManagementPermissionType.MODIFY,
                                                  ProjectManagementPermissionType.DELETE
                                              };

    protected ProjectInfo mProject = null;
    protected ResourceInfo mResProjects = null;

    /// <summary>
    /// OnPage changed event.
    /// </summary>
    public event EventHandler OnPageChanged;


    // HashTable holding information on all permissions that 'OnlyAuthorizedRoles' access is selected for
    private Hashtable onlyAuth = new Hashtable();

    #endregion


    #region "Public properties"

    /// <summary>
    /// Gets or sets the ID of the project to edit.
    /// </summary>
    public int ProjectID
    {
        get
        {
            return mProjectId;
        }
        set
        {
            mProjectId = value;
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
            gridMatrix.StopProcessing = value;
            base.StopProcessing = value;
        }
    }


    /// <summary>
    /// Indicates whether permissions matrix is enabled.
    /// </summary>
    public bool Enable
    {
        get
        {
            return mEnable;
        }
        set
        {
            mEnable = value;
        }
    }

    #endregion


    #region "Page methods"

    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        gridMatrix.OnItemChanged += gridMatrix_OnItemChanged;
        gridMatrix.StopProcessing = true;
        gridMatrix.ColumnsPreferedOrder = string.Join(",", mAllowedPermissions);

        if (ProjectID > 0)
        {
            mProject = ProjectInfoProvider.GetProjectInfo(ProjectID);

            // Check whether the project still exists
            EditedObject = mProject;
        }

        // Handle page chnaged event
        gridMatrix.OnPageChanged += gridMatrix_OnPageChanged;

        // Disable permission matrix if user has no MANAGE rights
        if (!ProjectInfoProvider.IsAuthorizedPerProject(ProjectID, PERMISSION_MANAGE, MembershipContext.AuthenticatedUser))
        {
            Enable = false;
            gridMatrix.Enabled = false;
            lblError.Text = String.Format(GetString("general.accessdeniedonpermissionname"), "Manage");
            lblError.Visible = true;
        }
    }


    /// <summary>
    /// PreRender event handler.
    /// </summary>
    protected override void OnPreRender(EventArgs e)
    {
        LoadData();
        if (ProjectID > 0)
        {
            gridMatrix.StopProcessing = true;

            if (!IsLiveSite && mProcess)
            {
                gridMatrix.StopProcessing = mStopProcessing;
                // Render permission matrix
                CreateMatrix();
            }
            else if (mCreateMatrix)
            {
                gridMatrix.StopProcessing = mStopProcessing;
                CreateMatrix();
                mCreateMatrix = false;
            }
            else if (IsLiveSite && mProcess && RequestHelper.IsPostBack())
            {
                gridMatrix.StopProcessing = mStopProcessing;
                CreateMatrix();
                mCreateMatrix = false;
            }
        }

        base.OnPreRender(e);
    }


    /// <summary>
    /// Load data.
    /// </summary>
    public void LoadData()
    {
        mProcess = true;
        if (!Visible || StopProcessing)
        {
            EnableViewState = false;
            mProcess = false;
        }

        IsLiveSite = false;

        if (ProjectID > 0)
        {
            // Get information on current project
            mProject = ProjectInfoProvider.GetProjectInfo(ProjectID);
        }

        // Get project resource
        mResProjects = ResourceInfoProvider.GetResourceInfo("CMS.ProjectManagement");

        if ((mResProjects != null) && (mProject != null))
        {
            QueryDataParameters parameters = new QueryDataParameters();
            parameters.Add("@ID", mResProjects.ResourceId);
            parameters.Add("@ProjectID", mProject.ProjectID);
            parameters.Add("@SiteID", SiteContext.CurrentSiteID);

            string where;
            int groupId = mProject.ProjectGroupID;

            // Build where condition
            if (groupId > 0)
            {
                where = "RoleGroupID=" + groupId + " AND PermissionDisplayInMatrix = 0";
            }
            else
            {
                where = "RoleGroupID IS NULL AND PermissionDisplayInMatrix = 0";
            }

            // Setup matrix control    
            gridMatrix.IsLiveSite = IsLiveSite;
            gridMatrix.QueryParameters = parameters;
            gridMatrix.WhereCondition = where;
            gridMatrix.CssClass = "permission-matrix";
        }
    }


    /// <summary>
    /// Page changed event habdler.
    /// </summary>
    private void gridMatrix_OnPageChanged(object sender, EventArgs e)
    {
        // Raise on page changed event
        if (OnPageChanged != null)
        {
            OnPageChanged(this, null);
        }
    }


    /// <summary>
    /// Clears the security matrix.
    /// </summary>
    public void Clear()
    {
        gridMatrix.ResetMatrix();
    }

    #endregion


    #region "Page event handlers"

    /// <summary>
    /// On item changed event.
    /// </summary>    
    protected void gridMatrix_OnItemChanged(object sender, int roleId, int permissionId, bool allow)
    {
        if (!CheckPermissions("CMS.ProjectManagement", PERMISSION_MANAGE))
        {
            return;
        }

        // Delete permission hash tables
        ProjectInfoProvider.ClearProjectPermissionTable(ProjectID, MembershipContext.AuthenticatedUser);

        if (allow)
        {
            ProjectRolePermissionInfoProvider.AddRelationship(ProjectID, roleId, permissionId);
        }
        else
        {
            ProjectRolePermissionInfoProvider.RemoveRelationship(ProjectID, roleId, permissionId);
        }
    }

    #endregion


    #region "Private methods"

    /// <summary>
    /// Generates the permission matrix for the cutrrent project.
    /// </summary>
    private void CreateMatrix()
    {
        // Get project resource info     
        if (mResProjects == null)
        {
            mResProjects = ResourceInfoProvider.GetResourceInfo("CMS.ProjectManagement");
        }

        // Get project object
        if ((mProject == null) && (ProjectID > 0))
        {
            mProject = ProjectInfoProvider.GetProjectInfo(ProjectID);
        }

        if ((mResProjects != null) && (mProject != null))
        {
            // Get permissions for the current project resource                       
            DataSet permissions = PermissionNameInfoProvider.GetResourcePermissions(mResProjects.ResourceId);
            if (DataHelper.DataSourceIsEmpty(permissions))
            {
                lblInfo.Text = GetString("general.emptymatrix");
                lblInfo.Visible = true;
            }
            else
            {
                TableRow headerRow = new TableRow();
                headerRow.TableSection = TableRowSection.TableHeader;
                headerRow.CssClass = "unigrid-head";

                TableHeaderCell newHeaderCell = new TableHeaderCell();
                newHeaderCell.CssClass = "first-column";
                headerRow.Cells.Add(newHeaderCell);

                foreach (string permission in mAllowedPermissions)
                {
                    DataRow[] drArray = permissions.Tables[0].DefaultView.Table.Select("PermissionName = '" + permission + "'");
                    if (drArray.Length > 0)
                    {
                        DataRow dr = drArray[0];
                        newHeaderCell = new TableHeaderCell();
                        newHeaderCell.Text = dr["PermissionDisplayName"].ToString();
                        newHeaderCell.ToolTip = dr["PermissionDescription"].ToString();
                        headerRow.Cells.Add(newHeaderCell);
                    }
                    else
                    {
                        throw new Exception("[Security matrix] Column '" + permission + "' cannot be found.");
                    }
                }

                tblMatrix.Rows.Add(headerRow);

                // Render project access permissions
                object[,] accessNames = new object[5, 2];
                accessNames[0, 0] = GetString("security.nobody");
                accessNames[0, 1] = SecurityAccessEnum.Nobody;
                accessNames[1, 0] = GetString("security.allusers");
                accessNames[1, 1] = SecurityAccessEnum.AllUsers;
                accessNames[2, 0] = GetString("security.authenticated");
                accessNames[2, 1] = SecurityAccessEnum.AuthenticatedUsers;
                accessNames[3, 0] = GetString("security.groupmembers");
                accessNames[3, 1] = SecurityAccessEnum.GroupMembers;
                accessNames[4, 0] = GetString("security.authorizedroles");
                accessNames[4, 1] = SecurityAccessEnum.AuthorizedRoles;

                TableRow newRow;
                int rowIndex = 0;
                for (int access = 0; access <= accessNames.GetUpperBound(0); access++)
                {
                    SecurityAccessEnum currentAccess = ((SecurityAccessEnum)accessNames[access, 1]);

                    // If the security isn't displayed as part of group section
                    if ((currentAccess == SecurityAccessEnum.GroupMembers) && (mProject.ProjectGroupID == 0))
                    {
                        // Do not render this access item
                    }
                    else
                    {
                        // Generate cell holding access item name
                        newRow = new TableRow();
                        TableCell newCell = new TableCell();
                        newCell.Text = accessNames[access, 0].ToString();
                        newCell.CssClass = "matrix-header";
                        newRow.Cells.Add(newCell);
                        rowIndex++;

                        // Render the permissions access items
                        int permissionIndex = 0;
                        for (int permission = 0; permission < (tblMatrix.Rows[0].Cells.Count - 1); permission++)
                        {
                            newCell = new TableCell();

                            // Check if the currently processed access is applied for permission
                            bool isAllowed = CheckPermissionAccess(currentAccess, permission, tblMatrix.Rows[0].Cells[permission + 1].Text);

                            // Disable column in roles grid if needed
                            if ((currentAccess == SecurityAccessEnum.AuthorizedRoles) && !isAllowed)
                            {
                                gridMatrix.DisableColumn(permissionIndex);
                            }

                            // Insert the radio button for the current permission
                            var radio = new CMSRadioButton
                            {
                                Checked = isAllowed,
                                Enabled = Enable,
                            };
                            radio.Attributes.Add("onclick", ControlsHelper.GetPostBackEventReference(this, permission + ";" + Convert.ToInt32(currentAccess)));
                            newCell.Controls.Add(radio);

                            newRow.Cells.Add(newCell);
                            permissionIndex++;
                        }

                        // Add the access row to the table
                        tblMatrix.Rows.Add(newRow);
                    }
                }

                // Check if project has some roles assigned           
                headTitle.Visible = gridMatrix.HasData;
            }
        }
    }


    /// <summary>
    /// Indicates the permission acess.
    /// </summary>
    /// <param name="currentAccess">Currently processed integer representation of item from SecurityAccessEnum</param>    
    /// <param name="currentPermission">Currently processed integer representation of permission to check</param>    
    private bool CheckPermissionAccess(SecurityAccessEnum currentAccess, int currentPermission, string currentPermissionName)
    {
        bool result = false;

        if (mProject != null)
        {
            switch (currentPermission)
            {
                case 0:
                    // Process 'AllowRead' permission and check by current access
                    result = (mProject.AllowRead == currentAccess);
                    break;

                case 1:
                    // Set 'AttachCreate' permission and check by current access
                    result = (mProject.AllowCreate == currentAccess);
                    break;

                case 2:
                    // Set 'AllowModify' permission and check by current access
                    result = (mProject.AllowModify == currentAccess);
                    break;

                case 3:
                    // Set 'AllowDelete' permission and check by current access
                    result = (mProject.AllowDelete == currentAccess);
                    break;

                default:
                    break;
            }
        }

        // Make note about type of permission with access set to 'OnlyAuthorizedRoles'
        if (result && (currentAccess == SecurityAccessEnum.AuthorizedRoles))
        {
            onlyAuth[currentPermissionName] = true;
        }
        return result;
    }

    #endregion


    #region "PostBack event handler"

    public void RaisePostBackEvent(string eventArgument)
    {
        if (!CheckPermissions("CMS.ProjectManagement", PERMISSION_MANAGE))
        {
            return;
        }

        string[] args = eventArgument.Split(';');
        if (args.Length == 2)
        {
            // Get info on currently selected item
            int permission = ValidationHelper.GetInteger(args[0], 0);
            int access = ValidationHelper.GetInteger(args[1], 0);

            if (mProject != null)
            {
                // Update project permission access information
                switch (permission)
                {
                    case 0:
                        // Set 'AllowRead' permission to specified access
                        mProject.AllowRead = (SecurityAccessEnum)access;
                        break;

                    case 1:
                        // Set 'AttachCreate' permission to specified access
                        mProject.AllowCreate = ((SecurityAccessEnum)access);
                        break;

                    case 2:
                        // Set 'AllowModify' permission to specified access
                        mProject.AllowModify = (SecurityAccessEnum)access;
                        break;

                    case 3:
                        // Set 'AllowDelete' permission to specified access
                        mProject.AllowDelete = ((SecurityAccessEnum)access);
                        break;
                }

                // Delete permission hash tables
                ProjectInfoProvider.ClearProjectPermissionTable(ProjectID, MembershipContext.AuthenticatedUser);

                // Use try/catch due to license check
                try
                {
                    // Save changes to the project
                    ProjectInfoProvider.SetProjectInfo(mProject);
                }
                catch (Exception ex)
                {
                    lblError.Visible = true;
                    lblError.Text = ex.Message;
                }

                mCreateMatrix = true;
            }
        }

        RaiseOnSaved();
    }


    /// <summary>
    /// Reloads data.
    /// </summary>
    public override void ReloadData()
    {
        base.ReloadData();
        mCreateMatrix = true;

        // Ensure viewstate
        EnableViewState = true;
    }

    #endregion
}