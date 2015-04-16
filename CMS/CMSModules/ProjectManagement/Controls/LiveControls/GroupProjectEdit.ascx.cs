using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.Controls.Configuration;
using CMS.Helpers;
using CMS.Membership;
using CMS.Base;
using CMS.UIControls;

public partial class CMSModules_ProjectManagement_Controls_LiveControls_GroupProjectEdit : CMSAdminControl
{
    #region "Variables"

    private bool displayControlPerformed = false;

    #endregion


    #region "Properties"

    /// <summary>
    /// Gets or sets the community group id.
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
            ucProjectEdit.CommunityGroupID = value;
        }
    }

    /// <summary>
    /// Gets or sets the control's display mode.
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
            ucProjectEdit.DisplayMode = value;
            ucSecurity.DisplayMode = value;
            ucTaskProperties.DisplayMode = value;
        }
    }


    /// <summary>
    /// Gets or sets the project ID.
    /// </summary>
    public int ProjectID
    {
        get
        {
            return ValidationHelper.GetInteger(ViewState["ProjectID"], 0);
        }
        set
        {
            ViewState["ProjectID"] = value;
            ucSecurity.ProjectID = value;
            ucTaskProperties.ProjectID = value;
            ucProjectEdit.ProjectID = value;
        }
    }


    /// <summary>
    /// Indicates if control is on live site.
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
            ucSecurity.IsLiveSite = value;
            ucTaskProperties.IsLiveSite = value;
            ucProjectEdit.IsLiveSite = value;
        }
    }

    #endregion


    #region "Page methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        #region "Tab settings"
        
        tabControlElem.AddTab(new TabItem
        {
            Text = GetString("pm.tasks"),
        });
        tabControlElem.AddTab(new TabItem
        {
            Text = GetString("general.general"),
        });
        tabControlElem.AddTab(new TabItem
        {
            Text = GetString("general.security"),
        });

        tabControlElem.UsePostback = true;
        tabControlElem.OnTabClicked += new EventHandler(tabControlElem_OnTabClicked);

        #endregion


        #region "Sub-control settings"

        // Propagate project id
        ucSecurity.ProjectID = ProjectID;
        ucTaskProperties.ProjectID = ProjectID;
        ucProjectEdit.ProjectID = ProjectID;

        // propagate display mode
        ucProjectEdit.DisplayMode = DisplayMode;
        ucSecurity.DisplayMode = DisplayMode;
        ucTaskProperties.DisplayMode = DisplayMode;

        ucSecurity.OnCheckPermissions += new CheckPermissionsEventHandler(controls_OnCheckPermissions);
        ucProjectEdit.OnCheckPermissions += new CheckPermissionsEventHandler(controls_OnCheckPermissions);

        #endregion
    }


    /// <summary>
    /// Check permission handler.
    /// </summary>
    private void controls_OnCheckPermissions(string permissionType, CMSAdminControl sender)
    {
        // If user is group admin => allow all actions
        if (!MembershipContext.AuthenticatedUser.IsGroupAdministrator(CommunityGroupID))
        {
            sender.StopProcessing = true;
        }
    }


    /// <summary>
    /// OnPrerender - display default.
    /// </summary>
    protected override void OnPreRender(EventArgs e)
    {
        // If display method wasn't called, dispaly control with dependence on current selected item on tabs
        if (!displayControlPerformed)
        {
            TabSelect();
        }
        base.OnPreRender(e);
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Tab click handler.
    /// </summary>
    private void tabControlElem_OnTabClicked(object sender, EventArgs e)
    {
        // Clear properties control
        ucTaskProperties.ClearControl();
        // Select control with depenendence on selected tab
        TabSelect();
    }


    /// <summary>
    /// Ensures controls displaying with dependence on tab selection.
    /// </summary>
    private void TabSelect()
    {
        // Get  selected tab index
        int index = tabControlElem.SelectedTab;

        // Switch by tab index
        switch (index)
        {
                // Tasks
            case 0:
                ucTaskProperties.ProjectID = ProjectID;
                DisplayControl("tasks");
                break;

                // General
            case 1:
                ucProjectEdit.ProjectID = ProjectID;
                DisplayControl("general");
                break;

                // Security
            case 2:
                ucSecurity.ProjectID = ProjectID;
                DisplayControl("security");
                break;
        }
    }


    /// <summary>
    /// Display given control.
    /// </summary>
    /// <param name="control">Control to display</param>
    public void DisplayControl(string control)
    {
        // Set display control flag
        displayControlPerformed = true;

        // Hide all controls by default
        pnlSecurity.Visible = false;
        pnlProjects.Visible = false;
        pnlTasks.Visible = false;

        // Switch by display type
        switch (control.ToLowerCSafe())
        {
                // Tasks
            case "tasks":
                pnlTasks.Visible = true;
                ucTaskProperties.ProjectID = ProjectID;
                ucTaskProperties.ReloadData();
                tabControlElem.SelectedTab = 0;
                break;

                // General
            case "general":
                pnlProjects.Visible = true;
                ucProjectEdit.ProjectID = ProjectID;
                ucProjectEdit.ReloadData();
                tabControlElem.SelectedTab = 1;
                break;

                // Security
            case "security":
                pnlSecurity.Visible = true;
                ucSecurity.ProjectID = ProjectID;
                ucSecurity.ReloadData();
                tabControlElem.SelectedTab = 2;
                break;
        }
    }


    /// <summary>
    /// Reloads data.
    /// </summary>
    public override void ReloadData()
    {
        // Clear sub controls
        ucTaskProperties.ClearControl();
        // Display tasks by default
        DisplayControl("tasks");
        // Call base method
        base.ReloadData();
    }


    /// <summary>
    /// Clears current form and sub-forms.
    /// </summary>
    public override void ClearForm()
    {
        // Clear project edit form
        ucProjectEdit.ClearForm();
        // Call base method
        base.ClearForm();
    }

    #endregion
}