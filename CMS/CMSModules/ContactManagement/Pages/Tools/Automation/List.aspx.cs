using System;
using System.Web.UI.WebControls;
using System.Linq;
using System.Text;

using CMS.Automation;
using CMS.Core;
using CMS.Helpers;
using CMS.PortalEngine;
using CMS.UIControls;
using CMS.ExtendedControls.ActionsConfig;
using CMS.Base;
using CMS.OnlineMarketing;
using CMS.WorkflowEngine;
using CMS.ExtendedControls;

public partial class CMSModules_ContactManagement_Pages_Tools_Automation_List : CMSAutomationPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // Set master page elements
        InitializeMasterPage();

        // Check manage permission for object menu
        if (!WorkflowStepInfoProvider.CanUserManageAutomationProcesses(CurrentUser, CurrentSiteName))
        {
            gridProcesses.ShowObjectMenu = false;
        }

        // Control initialization
        gridProcesses.OnAction += gridProcesses_OnAction;
        gridProcesses.OnExternalDataBound += gridProcesses_OnExternalDataBound;
        gridProcesses.ZeroRowsText = GetString("general.nodatafound");
        gridProcesses.RememberStateByParam = "";
    }


    /// <summary>
    ///  Initializes master page elements.
    /// </summary>
    private void InitializeMasterPage()
    {
        if (!CurrentUser.IsGlobalAdministrator)
        {
            PageTitle title = PageTitle;
            title.TitleText = GetString("ma.automationprocess.list");
        }

        if (WorkflowInfoProvider.IsMarketingAutomationAllowed())
        {
            HeaderAction newProcess = new HeaderAction
            {
                // New process link
                Text = GetString("ma.newprocess"),
                RedirectUrl = AddSiteQuery("Process/New.aspx", null),
                Enabled = WorkflowStepInfoProvider.CanUserManageAutomationProcesses(CurrentUser, CurrentSiteName)
            };

            AddHeaderAction(newProcess);
        }
    }


    protected object gridProcesses_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        switch (sourceName.ToLowerCSafe())
        {
            case "recurrencetype":
                var val = (ProcessRecurrenceTypeEnum)ValidationHelper.GetInteger(parameter, (int)ProcessRecurrenceTypeEnum.Recurring);
                return val.ToLocalizedString("cms.workflow.recurrency");

            case "delete":
                if (!WorkflowStepInfoProvider.CanUserManageAutomationProcesses(CurrentUser, CurrentSiteName))
                {
                    CMSGridActionButton btn = (CMSGridActionButton)sender;
                    btn.Enabled = false;
                }
                break;

            case "enabled":
                return UniGridFunctions.ColoredSpanYesNo(ValidationHelper.GetBoolean(parameter, true));
        }

        return parameter;
    }


    /// <summary>
    /// Handles the UniGrid's OnAction event.
    /// </summary>
    /// <param name="actionName">Name of item (button) that threw event</param>
    /// <param name="actionArgument">ID (value of Primary key) of corresponding data row</param>
    protected void gridProcesses_OnAction(string actionName, object actionArgument)
    {
        int processId = Convert.ToInt32(actionArgument);

        switch (actionName)
        {
            case "edit":
                var url = UIContextHelper.GetElementUrl(ModuleName.ONLINEMARKETING, "EditProcess");
                url = URLHelper.AddParameterToUrl(url, "displayTitle", "false");
                url = URLHelper.AddParameterToUrl(url, "objectId", processId.ToString());
                URLHelper.Redirect(AddSiteQuery(url, null));
                break;

            case "delete":
                if (AutomationHelper.CheckProcessDependencies(processId))
                {
                    ShowError(GetString("MA.process.CannotDeleteUsed"));

                    return;
                }

                if (!WorkflowStepInfoProvider.CanUserManageAutomationProcesses(CurrentUser, CurrentSiteName))
                {
                    RedirectToAccessDenied(ModuleName.ONLINEMARKETING, "ManageProcesses");
                }

                // Delete the workflow
                WorkflowInfoProvider.DeleteWorkflowInfo(processId);
                break;
        }
    }
}