using System;
using System.Data;
using System.Web.UI.WebControls;

using CMS.Ecommerce;
using CMS.Helpers;
using CMS.Base;
using CMS.UIControls;
using CMS.ExtendedControls.ActionsConfig;
using CMS.ExtendedControls;

[Title("OrderStatus_List.HeaderCaption")]
public partial class CMSModules_Ecommerce_Pages_Tools_Configuration_OrderStatus_OrderStatus_List : CMSOrderStatusesPage
{
    #region "Page Events"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Init header actions
        var actions = CurrentMaster.HeaderActions;
        actions.ActionPerformed += HeaderActions_ActionPerformed;

        // New item action
        actions.ActionsList.Add(new HeaderAction
        {
            Text = GetString("OrderStatus_List.NewItemCaption"),
            RedirectUrl = ResolveUrl("OrderStatus_Edit.aspx?siteId=" + SiteID)
        });

        // Show copy from global link when not configuring global statuses.
        if (ConfiguredSiteID != 0)
        {
            // Show "Copy from global" link only if there is at least one global status
            DataSet ds = OrderStatusInfoProvider.GetOrderStatuses(0).TopN(1);
            if (!DataHelper.DataSourceIsEmpty(ds))
            {
                actions.ActionsList.Add(new HeaderAction
                {
                    Text = GetString("general.copyfromglobal"),
                    OnClientClick = "return ConfirmCopyFromGlobal();",
                    CommandName = "copyFromGlobal",
                    ButtonStyle = ButtonStyle.Default
                });

                // Register javascript to confirm generate 
                string script = "function ConfirmCopyFromGlobal() {return confirm(" + ScriptHelper.GetString(GetString("com.ConfirmOrderStatusFromGlobal")) + ");}";
                ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "ConfirmCopyFromGlobal", ScriptHelper.GetScript(script));
            }
        }

        gridElem.OnAction += gridElem_OnAction;
        gridElem.OnExternalDataBound += gridElem_OnExternalDataBound;
        gridElem.GridView.AllowSorting = false;

        // Show information about usage of global objects when used on site
        HandleGlobalObjectInformation(gridElem.ObjectType);

        // Filter records by site
        gridElem.WhereCondition = InitSiteWhereCondition("StatusSiteID").ToString(true);
    }


    private object gridElem_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        if (sourceName == "statusName")
        {
            var statusRow = parameter as DataRowView;
            if (statusRow == null)
            {
                return string.Empty;
            }

            // Create tag with status name and color
            var statusInfo = new OrderStatusInfo(statusRow.Row);
            return new Tag
                   {
                       Text = statusInfo.StatusDisplayName,
                       Color = statusInfo.StatusColor
                   };
        }

        return parameter;
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        // Check if at least one enabled status is present
        CheckEnabledStatusPresence();
    }

    #endregion


    #region "Event Handlers"

    protected void HeaderActions_ActionPerformed(object sender, CommandEventArgs e)
    {
        switch (e.CommandName.ToLowerCSafe())
        {
            case "copyfromglobal":
                // Check permissions
                CheckConfigurationModification();

                // Copy and reload
                OrderStatusInfoProvider.CopyFromGlobal(ConfiguredSiteID);
                gridElem.ReloadData();
                break;
        }
    }


    /// <summary>
    /// Handles the UniGrid's OnAction event.
    /// </summary>
    /// <param name="actionName">Name of item (button) that throws event</param>
    /// <param name="actionArgument">ID (value of Primary key) of corresponding data row</param>
    protected void gridElem_OnAction(string actionName, object actionArgument)
    {
        int id = actionArgument.ToInteger(0);

        switch (actionName.ToLowerCSafe())
        {
            case "edit":
                URLHelper.Redirect("OrderStatus_Edit.aspx?orderstatusid=" + id + "&siteId=" + SiteID);
                break;

            case "delete":
                CheckConfigurationModification();

                var status = OrderStatusInfoProvider.GetOrderStatusInfo(id);
                if (status != null)
                {
                    if (status.Generalized.CheckDependencies())
                    {
                        // Show error message
                        ShowError(ECommerceHelper.GetDependencyMessage(status));

                        return;
                    }

                    // Delete OrderStatusInfo object from database
                    OrderStatusInfoProvider.DeleteOrderStatusInfo(status);
                }

                break;

            case "moveup":
                CheckConfigurationModification();

                OrderStatusInfoProvider.MoveStatusUp(id);
                break;

            case "movedown":
                CheckConfigurationModification();

                OrderStatusInfoProvider.MoveStatusDown(id);
                break;
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Checks if at least one enabled order status is present.
    /// </summary>
    private void CheckEnabledStatusPresence()
    {
        DataSet ds = OrderStatusInfoProvider.GetOrderStatuses(ConfiguredSiteID, true);
        if (DataHelper.DataSourceIsEmpty(ds))
        {
            ShowWarning(GetString("com.orderstatus.noenabledfound"));
        }
    }

    #endregion
}