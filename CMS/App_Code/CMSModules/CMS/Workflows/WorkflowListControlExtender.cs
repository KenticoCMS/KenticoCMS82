using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

using CMS;
using CMS.Base;
using CMS.ExtendedControls;
using CMS.ExtendedControls.ActionsConfig;
using CMS.Helpers;
using CMS.PortalEngine;
using CMS.UIControls;
using CMS.WorkflowEngine;
using CMS.Core;


[assembly: RegisterCustomClass("WorkflowListControlExtender", typeof(WorkflowListControlExtender))]

/// <summary>
/// Permission edit control extender
/// </summary>
public class WorkflowListControlExtender : ControlExtender<UniGrid>
{
    /// <summary>
    /// OnInit event handler
    /// </summary>
    public override void OnInit()
    {
        Control.OnExternalDataBound += OnExternalDataBound;
        Control.OnAction += OnAction;

        if (WorkflowInfoProvider.IsAdvancedWorkflowAllowed())
        {
            Control.AddHeaderAction(new HeaderAction
            {
                // New advanced workflow link
                Text = Control.GetString("Development-Workflow_List.NewAdvancedWorkflow"),
                RedirectUrl = URLHelper.AppendQuery(UIContextHelper.GetElementUrl(ModuleName.CMS, "NewWorkflow", false), "type=1"),
                Index = 1
            });
        }
    }


    /// <summary>
    /// OnExternalDataBound event handler
    /// </summary>
    protected object OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        switch (sourceName.ToLowerCSafe())
        {
            case "type":
                DataRowView drv = (DataRowView)parameter;
                bool versioning = ValidationHelper.GetBoolean(drv.Row["WorkflowAutoPublishChanges"], false);
                int type = ValidationHelper.GetInteger(drv.Row["WorkflowType"], 0);
                return WorkflowHelper.GetWorkflowTypeString((WorkflowTypeEnum)type, versioning);
        }

        return parameter;
    }


    /// <summary>
    /// Handles the UniGrid's OnAction event.
    /// </summary>
    /// <param name="actionName">Name of item (button) that threw event</param>
    /// <param name="actionArgument">ID (value of Primary key) of corresponding data row</param>
    protected void OnAction(string actionName, object actionArgument)
    {
        int workflowid = Convert.ToInt32(actionArgument);

        switch (actionName)
        {
            case "delete":
                // Check if documents use the workflow
                List<string> documentNames = new List<string>();
                if (WorkflowInfoProvider.CheckDependencies(workflowid, ref documentNames))
                {
                    // Encode and localize names
                    StringBuilder sb = new StringBuilder();
                    documentNames.ForEach(item => sb.Append("<br />", HTMLHelper.HTMLEncode(ResHelper.LocalizeString(item))));
                    Control.ShowError(Control.GetString("Workflow.CannotDeleteUsed"), Control.GetString("workflow.documentlist") + sb);
                }
                else
                {
                    // Delete the workflow
                    WorkflowInfoProvider.DeleteWorkflowInfo(workflowid);
                }
                break;
        }
    }
}