using System;
using System.Data;
using System.Web.UI.WebControls;

using CMS.ExtendedControls;
using CMS.Helpers;
using CMS.OnlineMarketing;
using CMS.Base;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.WebAnalytics;


public partial class CMSModules_ContactManagement_Controls_UI_ActivityType_List : CMSAdminListControl
{
    #region "Variables"

    private bool modifyPermission = false;

    #endregion


    #region "Properties"

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
            gridElem.IsLiveSite = value;
        }
    }

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        gridElem.OnExternalDataBound += gridElem_OnExternalDataBound;
        gridElem.OnAction += gridElem_OnAction;
        modifyPermission = ActivityHelper.AuthorizedManageActivity(SiteContext.CurrentSiteID, false);
    }


    void gridElem_OnAction(string actionName, object actionArgument)
    {
        switch (actionName)
        {
            case "delete":
                int activityTypeID = ValidationHelper.GetInteger(actionArgument, 0);
                var activityType = ActivityTypeInfoProvider.GetActivityTypeInfo(activityTypeID);
                if ((activityType == null) || !activityType.ActivityTypeIsCustom)
                {
                    RedirectToInformation("general.modifynotallowed");
                }
                if (ActivityHelper.AuthorizedManageActivity(SiteContext.CurrentSiteID, false))
                {
                    ActivityTypeInfoProvider.DeleteActivityTypeInfo(activityType);
                }
                break;
        }
    }


    private object gridElem_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        switch (sourceName.ToLowerCSafe())
        {
            case "delete":
                // Disable "delete" button for system objects
                bool iscustom = ValidationHelper.GetBoolean(((DataRowView)((GridViewRow)parameter).DataItem).Row["ActivityTypeIsCustom"], false);
                CMSGridActionButton button = ((CMSGridActionButton)sender);
                if (!iscustom)
                {
                    button.Enabled = false;
                }
                button.Visible = modifyPermission;
                break;
        }
        return sender;
    }


    protected void Page_PreRender(object sender, EventArgs e)
    {
        if (gridElem.RowsCount > 0)
        {
            int i = 0;
            DataView view = (DataView)gridElem.GridView.DataSource;
            foreach (DataRow row in view.Table.Rows)
            {
                // Hide object menu to system activity types (only custom activity types may be exported)
                if (!ValidationHelper.GetBoolean(DataHelper.GetDataRowValue(row, "ActivityTypeIsCustom"), false))
                {
                    if ((gridElem.GridView.Rows[i].Cells.Count > 0) && (gridElem.GridView.Rows[i].Cells[0].Controls.Count > 2)
                        && (gridElem.GridView.Rows[i].Cells[0].Controls[2] is ContextMenuContainer))
                    {
                        gridElem.GridView.Rows[i].Cells[0].Controls[2].Visible = false;
                    }
                }

                i++;
            }
        }
    }

    #endregion
}