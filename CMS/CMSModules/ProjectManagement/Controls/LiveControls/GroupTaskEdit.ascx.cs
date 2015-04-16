using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.Helpers;
using CMS.Membership;
using CMS.ProjectManagement;
using CMS.Base;
using CMS.UIControls;
using CMS.ExtendedControls;
using CMS.ExtendedControls.ActionsConfig;

public partial class CMSModules_ProjectManagement_Controls_LiveControls_GroupTaskEdit : CMSAdminControl
{
    #region "Variables"

    private bool displayControlPerformed = false;

    #endregion


    #region "Properties"

    /// <summary>
    /// Gets or sets the ProjectID value.
    /// </summary>
    public int ProjectID
    {
        get
        {
            return ucTaskList.ProjectID;
        }
        set
        {
            EnsureChildControls();
            ucTaskList.ProjectID = value;
            ucTaskEdit.ProjectID = value;
        }
    }


    /// <summary>
    /// Gets or sets the ID of selected task.
    /// </summary>
    public int SelectedTaskID
    {
        get
        {
            return ucTaskList.SelectedItemID;
        }
        set
        {
            EnsureChildControls();
            ucTaskList.SelectedItemID = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether is live site.
    /// </summary>f
    public override bool IsLiveSite
    {
        get
        {
            return base.IsLiveSite;
        }
        set
        {
            EnsureChildControls();
            base.IsLiveSite = value;
            ucTaskList.IsLiveSite = value;
            ucTaskEdit.IsLiveSite = value;
        }
    }

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Handle control events
        ucTaskList.OnAction += ucTaskList_OnAction;
        ucTaskEdit.OnSaved += ucTaskEdit_OnSaved;

        // Breadcrumbs for edit
        lnkBackHidden.Click += lnkBackHidden_Click;

        InitializeBreadcrumbs();

        #region "New task link"

        // New item link
        actionsElem.AddAction(new HeaderAction
        { 
            Text = GetString("pm.projecttask.new"),
            CommandName = "new_task"
        });
        actionsElem.ActionPerformed += actionsElem_ActionPerformed;

        #endregion


        // Set control properties
        ucTaskList.UsePostbackOnEdit = true;
        ucTaskList.OrderByType = ProjectTaskOrderByEnum.ProjectOrder;
        ucTaskEdit.OnSaved += ucTaskEdit_OnSaved;

        ucTaskEdit.OnCheckPermissions += controls_OnCheckPermissions;
        ucTaskList.OnCheckPermissions += controls_OnCheckPermissions;
    }


    /// <summary>
    /// Check permission handler.
    /// </summary>
    private void controls_OnCheckPermissions(string permissionType, CMSAdminControl sender)
    {
        ProjectInfo pi = ProjectInfoProvider.GetProjectInfo(ProjectID);
        if (pi != null)
        {
            // If user is group admin => allow all actions
            if (!MembershipContext.AuthenticatedUser.IsGroupAdministrator(pi.ProjectGroupID))
            {
                sender.StopProcessing = true;
            }
        }
    }


    /// <summary>
    /// Task saved event handler.
    /// </summary>
    private void ucTaskEdit_OnSaved(object sender, EventArgs e)
    {
        // Set task id
        SelectedTaskID = ucTaskEdit.ItemID;
        // Display edit control
        DisplayControl("edit");
    }


    /// <summary>
    /// Edit clicked on list.
    /// </summary>
    private void ucTaskList_OnAction(object sender, CommandEventArgs e)
    {
        // Switch by command name
        switch (e.CommandName.ToString())
        {
                // Edit
            case "edit":
                // Set task id from command argument
                int taskID = ValidationHelper.GetInteger(e.CommandArgument, 0);
                ucTaskEdit.ItemID = taskID;
                SelectedTaskID = taskID;
                // Display edit control
                DisplayControl("edit");
                break;
        }
    }


    /// <summary>
    /// New task click handler.
    /// </summary>
    protected void actionsElem_ActionPerformed(object sender, CommandEventArgs e)
    {
        // Swrich by command name
        switch (e.CommandName.ToLowerCSafe())
        {
            case "new_task":
                // Display new control
                DisplayControl("new");
                break;
        }
    }


    /// <summary>
    /// Display given control.
    /// </summary>
    /// <param name="control">Type of control to display</param>
    private void DisplayControl(string control)
    {
        // Set display performed falg
        displayControlPerformed = true;

        // Hide all controls 
        pnlEdit.Visible = false;
        plcList.Visible = false;

        // Switch by display control type
        switch (control.ToLowerCSafe())
        {
                // List
            case "list":
                plcList.Visible = true;
                ucTaskList.ProjectID = ProjectID;
                ucTaskList.ReloadData();
                break;

                // New
            case "new":
                pnlEdit.Visible = true;
                ucTaskEdit.ProjectID = ProjectID;
                SelectedTaskID = -1;
                SetBreadcrumbs(0);
                ucTaskEdit.ItemID = 0;
                ucTaskEdit.ClearForm();
                ucTaskEdit.ReloadData(true);
                break;

                // Edit
            case "edit":
                pnlEdit.Visible = true;
                ucTaskEdit.ProjectID = ProjectID;
                SetBreadcrumbs(SelectedTaskID);
                ucTaskEdit.ClearForm();
                ucTaskEdit.ReloadData(true);
                break;
        }
    }


    /// <summary>
    /// OnPreRender override.
    /// </summary>
    protected override void OnPreRender(EventArgs e)
    {
        // If display method wasn't called ensure default displaying with dependence on selected item on tab control
        if (!displayControlPerformed)
        {
            switch (SelectedTaskID)
            {
                    // Default list
                case 0:
                    DisplayControl("list");
                    break;

                    // New item 
                case -1:
                    SetBreadcrumbs(0);
                    break;

                    // Edit item
                default:
                    SetBreadcrumbs(SelectedTaskID);
                    break;
            }
        }
    }


    /// <summary>
    /// Clear the selection.
    /// </summary>
    public void ClearControl()
    {
        // Un-select current task if exist
        SelectedTaskID = 0;
        // Display list control by default
        DisplayControl("list");
    }


    /// <summary>
    /// Sets breadcrumbs for edit.
    /// </summary>
    private void SetBreadcrumbs(int projectTaskID)
    {
        // If project task is defined display task specific breadcrumbs
        if (projectTaskID != 0)
        {
            // Load project name
            ProjectTaskInfo pi = ProjectTaskInfoProvider.GetProjectTaskInfo(projectTaskID);
            if (pi != null)
            {
                ucBreadcrumbs.Items[1].Text = HTMLHelper.HTMLEncode(pi.ProjectTaskDisplayName);
            }
        }
        // Display new task breadcrumb
        else
        {
            ucBreadcrumbs.Items[1].Text = GetString("pm.projecttask.new");
        }
    }


    /// <summary>
    /// Initialize breadcrumb items.
    /// </summary>
    private void InitializeBreadcrumbs()
    {
        ucBreadcrumbs.Items.Clear();

        ucBreadcrumbs.AddBreadcrumb(new BreadcrumbItem {
            Text = GetString("pm.tasks"),
            OnClientClick = ControlsHelper.GetPostBackEventReference(lnkBackHidden) + "; return false;"
        });

        ucBreadcrumbs.AddBreadcrumb(new BreadcrumbItem());
    }


    /// <summary>
    /// Breadcrumbs back link event handler.
    /// </summary>
    private void lnkBackHidden_Click(object sender, EventArgs e)
    {
        // Display list
        DisplayControl("list");
    }

    #endregion
}