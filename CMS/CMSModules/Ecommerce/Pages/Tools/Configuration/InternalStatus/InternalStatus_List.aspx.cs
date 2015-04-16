using System;
using System.Data;
using System.Web.UI.WebControls;

using CMS.Ecommerce;
using CMS.Helpers;
using CMS.Base;
using CMS.UIControls;
using CMS.ExtendedControls.ActionsConfig;
using CMS.ExtendedControls;

[Title("InternalStatus_List.HeaderCaption")]
public partial class CMSModules_Ecommerce_Pages_Tools_Configuration_InternalStatus_InternalStatus_List : CMSInternalStatusesPage
{
    #region "Page Events"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Init header actions
        HeaderActions actions = CurrentMaster.HeaderActions;
        actions.ActionPerformed += HeaderActions_ActionPerformed;

        // New item action
        actions.ActionsList.Add(new HeaderAction
        {
            Text = GetString("InternalStatus_List.NewItemCaption"),
            RedirectUrl = ResolveUrl("InternalStatus_Edit.aspx?siteId=" + SiteID)
        });

        // Show copy from global link when not configuring global statuses.
        if (ConfiguredSiteID != 0)
        {
            // Show "Copy from global" link only if there is at least one global status
            DataSet ds = InternalStatusInfoProvider.GetInternalStatuses(0).TopN(1);
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
                string script = "function ConfirmCopyFromGlobal() {return confirm(" + ScriptHelper.GetString(GetString("com.ConfirmInternalStatusFromGlobal")) + ");}";
                ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "ConfirmCopyFromGlobal", ScriptHelper.GetScript(script));
            }
        }

        gridElem.OnAction += gridElem_OnAction;

        // Show information about usage of global objects when used on site
        HandleGlobalObjectInformation(gridElem.ObjectType);

        // Filter records by site
        gridElem.WhereCondition = InitSiteWhereCondition("InternalStatusSiteID").ToString(true);
    }

    #endregion


    #region "Event Handlers"

    protected void HeaderActions_ActionPerformed(object sender, CommandEventArgs e)
    {
        switch (e.CommandName.ToLowerCSafe())
        {
            case "copyfromglobal":
                CopyFromGlobal();
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

        if (actionName == "edit")
        {
            URLHelper.Redirect("InternalStatus_Edit.aspx?statusid=" + id + "&siteId=" + SiteID);
        }
        else if (actionName == "delete")
        {
            CheckConfigurationModification();

            var status = InternalStatusInfoProvider.GetInternalStatusInfo(id);
            if (status != null)
            {
                if (status.Generalized.CheckDependencies())
                {
                    // Show error message
                    ShowError(ECommerceHelper.GetDependencyMessage(status));

                    return;
                }

                // delete InternalStatusInfo object from database
                InternalStatusInfoProvider.DeleteInternalStatusInfo(status);
            }
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    ///  Copies site-specific status options from global status list.
    /// </summary>
    protected void CopyFromGlobal()
    {
        CheckConfigurationModification();
        InternalStatusInfoProvider.CopyFromGlobal(ConfiguredSiteID);
    }

    #endregion
}