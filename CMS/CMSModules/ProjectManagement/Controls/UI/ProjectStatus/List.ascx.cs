using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.Helpers;
using CMS.ProjectManagement;
using CMS.Base;
using CMS.UIControls;
using CMS.ExtendedControls;

public partial class CMSModules_ProjectManagement_Controls_UI_Projectstatus_List : CMSAdminListControl
{
    #region "Properties"

    /// <summary>
    /// Messages placeholder
    /// </summary>
    public override MessagesPlaceHolder MessagesPlaceHolder
    {
        get
        {
            return plcMess;
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
            plcMess.IsLiveSite = value;
            gridElem.IsLiveSite = value;
        }
    }

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        CheckPermissions("CMS.ProjectManagement", ProjectManagementPermissionType.MANAGE_CONFIGURATION);

        if (StopProcessing)
        {
            return;
        }

        // Grid initialization                
        gridElem.OnAction += new OnActionEventHandler(gridElem_OnAction);
        gridElem.OnExternalDataBound += new OnExternalDataBoundEventHandler(gridElem_OnExternalDataBound);
    }


    /// <summary>
    /// Handles UniGrid's OnExternalDataBound event.
    /// </summary>
    /// <param name="sender">Sender object (image button if it is an external databoud for action button)</param>
    /// <param name="sourceName">External source name of the column specified in the grid XML</param>
    /// <param name="parameter">Source parameter (original value of the field)</param>
    protected object gridElem_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        switch (sourceName.ToLowerCSafe())
        {
            case "statusenabled":
                return UniGridFunctions.ColoredSpanYesNo(parameter);

            case "statusisnotstarted":
                return UniGridFunctions.ColoredSpanYesNo(parameter);

            case "statusicon":
                string url = ValidationHelper.GetString(parameter, "");
                if (!String.IsNullOrEmpty(url))
                {
                    url = "<img alt=\"Status image\" src=\"" + HTMLHelper.HTMLEncode(GetImageUrl(url)) + "\" style=\"max-width:50px; max-height: 50px;\"  />";
                    return url;
                }
                return "";

            case "statusisfinished":
                return UniGridFunctions.ColoredSpanYesNo(parameter);
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
        int projectstatusId = ValidationHelper.GetInteger(actionArgument, 0);
        if (projectstatusId > 0)
        {
            switch (actionName.ToLowerCSafe())
            {
                case "edit":
                    SelectedItemID = projectstatusId;
                    RaiseOnEdit();
                    break;

                case "delete":
                    if (!ProjectStatusInfoProvider.CheckDependencies(projectstatusId))
                    {
                        ShowError(GetString("pm.projectstatus.removedenied"));
                        return;
                    }

                    if (ProjectStatusInfoProvider.GetStatusCount() <= 1)
                    {
                        ltlInfo.Text = ScriptHelper.GetScript("alert('" + GetString("pm.projectstatus.deletealert") + "');");
                        return;
                    }

                    // Delete the object
                    ProjectStatusInfoProvider.DeleteProjectStatusInfo(projectstatusId);
                    RaiseOnDelete();

                    // Reload data
                    gridElem.ReloadData();
                    break;

                case "up":
                    ProjectStatusInfoProvider.MoveStatusUp(projectstatusId);
                    break;

                case "down":
                    ProjectStatusInfoProvider.MoveStatusDown(projectstatusId);
                    break;
            }
        }
    }

    #endregion
}