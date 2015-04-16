using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.Helpers;
using CMS.Membership;
using CMS.PortalControls;
using CMS.ProjectManagement;
using CMS.Base;
using CMS.SiteProvider;
using CMS.UIControls;

public partial class CMSWebParts_ProjectManagement_TasksAssignedToMe : CMSAbstractWebPart
{
    #region "Properties"

    /// <summary>
    /// Gets or sets the sitenam.
    /// </summary>
    public string SiteName
    {
        get
        {
            string siteName = ValidationHelper.GetString(GetValue("SiteName"), String.Empty);
            if (CMSString.Compare(siteName, "#currentsite#", true) == 0)
            {
                siteName = SiteContext.CurrentSiteName;
                SetValue("SiteName", siteName);
            }
            return siteName;
        }
        set
        {
            SetValue("SiteName", value);
            ucTasks.SiteName = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether paging should be used.
    /// </summary>
    public bool EnablePaging
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("EnablePaging"), false);
        }
        set
        {
            SetValue("EnablePaging", value);
            ucTasks.EnablePaging = value;
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
            ucTasks.PageSize = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether task actions should be enabled.
    /// </summary>
    public bool AllowTaskActions
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("AllowTaskActions"), false);
        }
        set
        {
            SetValue("AllowTaskActions", value);
            ucTasks.AllowTaskActions = value;
        }
    }


    /// <summary>
    /// Status display type.
    /// </summary>
    public StatusDisplayTypeEnum StatusDisplayType
    {
        get
        {
            return (StatusDisplayTypeEnum)ValidationHelper.GetInteger(GetValue("ShowStatusAs"), 0);
        }
        set
        {
            SetValue("ShowStatusAs", value);
            ucTasks.StatusDisplayType = value;
        }
    }


    /// <summary>
    /// Show overdue tasks.
    /// </summary>
    public bool ShowOverdueTasks
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ShowOverdueTasks"), ucTasks.ShowOverdueTasks);
        }
        set
        {
            SetValue("ShowOverdueTasks", value);
            ucTasks.ShowOverdueTasks = value;
        }
    }


    /// <summary>
    /// Show overdue tasks.
    /// </summary>
    public bool ShowOnTimeTasks
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ShowOnTimeTasks"), ucTasks.ShowOnTimeTasks);
        }
        set
        {
            SetValue("ShowOnTimeTasks", value);
            ucTasks.ShowOnTimeTasks = value;
        }
    }


    /// <summary>
    /// Show overdue tasks.
    /// </summary>
    public bool ShowPrivateTasks
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ShowPrivateTasks"), ucTasks.ShowPrivateTasks);
        }
        set
        {
            SetValue("ShowPrivateTasks", value);
            ucTasks.ShowPrivateTasks = value;
        }
    }


    /// <summary>
    /// Show overdue tasks.
    /// </summary>
    public bool ShowFinishedTasks
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ShowFinishedTasks"), ucTasks.ShowFinishedTasks);
        }
        set
        {
            SetValue("ShowFinishedTasks", value);
            ucTasks.ShowFinishedTasks = value;
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// On content loaded.
    /// </summary>
    public override void OnContentLoaded()
    {
        base.OnContentLoaded();
        SetupControl();
    }


    /// <summary>
    /// Setup control.
    /// </summary>
    protected void SetupControl()
    {
        if (StopProcessing)
        {
            return;
        }

        // Set display peoperties 
        ucTasks.ShowFinishedTasks = ShowFinishedTasks;
        ucTasks.ShowOnTimeTasks = ShowOnTimeTasks;
        ucTasks.ShowOverdueTasks = ShowOverdueTasks;
        ucTasks.ShowPrivateTasks = ShowPrivateTasks;
        ucTasks.StatusDisplayType = StatusDisplayType;
        ucTasks.TasksDisplayType = TasksDisplayTypeEnum.TasksAssignedToMe;
        ucTasks.AllowTaskActions = AllowTaskActions;
        ucTasks.EnablePaging = EnablePaging;
        ucTasks.PageSize = PageSize;
        ucTasks.SiteName = SiteName;

        // Register security handler
        ucTasks.OnCheckPermissionsExtended += new CMSAdminControl.CheckPermissionsExtendedEventHandler(ucTasks_OnCheckPermissionsExtended);
    }


    /// <summary>
    /// Check permissions event handler.
    /// </summary>
    private void ucTasks_OnCheckPermissionsExtended(string permissionType, string modulePermissionType, CMSAdminControl sender)
    {
        // No permissions by default
        sender.StopProcessing = true;
        // Current item ID
        int taskId = 0;

        // Check permission for delete task
        if (permissionType == ProjectManagementPermissionType.DELETE)
        {
            // Get list object
            CMSAdminListControl listControl = sender as CMSAdminListControl;
            // Check whether list object is defined
            if (listControl != null)
            {
                taskId = listControl.SelectedItemID;
            }
        }
        // Check permision for task modify
        else if (permissionType == ProjectManagementPermissionType.MODIFY)
        {
            // Get edit object 
            CMSAdminEditControl editControl = sender as CMSAdminEditControl;
            // Check whether edit control is defined
            if (editControl != null)
            {
                taskId = editControl.ItemID;
            }
        }

        // Check permissions only for existing tasks
        if (taskId > 0)
        {
            // If user has no permission for current action, display error message
            if (ProjectTaskInfoProvider.IsAuthorizedPerTask(taskId, permissionType, MembershipContext.AuthenticatedUser, SiteContext.CurrentSiteID))
            {
                sender.StopProcessing = false;
            }
            else
            {
                messageElem.Visible = true;
                messageElem.ErrorMessage = ResHelper.GetString("pm.project.permission");
            }
        }
    }


    /// <summary>
    /// Reloads the control data.
    /// </summary>
    public override void ReloadData()
    {
        base.ReloadData();
        SetupControl();
    }

    #endregion
}