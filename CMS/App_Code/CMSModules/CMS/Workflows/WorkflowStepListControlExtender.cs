using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;

using CMS;
using CMS.Base;
using CMS.DataEngine;
using CMS.ExtendedControls;
using CMS.Helpers;
using CMS.UIControls;
using CMS.WorkflowEngine;
using CMS.WorkflowEngine.GraphConfig;


[assembly: RegisterCustomClass("WorkflowStepListControlExtender", typeof(WorkflowStepListControlExtender))]

/// <summary>
/// Permission edit control extender
/// </summary>
public class WorkflowStepListControlExtender : ControlExtender<UniGrid>
{
    /// <summary>
    /// OnInit event handler
    /// </summary>
    public override void OnInit()
    {
        Control.OnExternalDataBound += OnExternalDataBound;
        Control.OnAction += OnAction;
        Control.DataBinding += DataBinding;

        Control.Load += (sender, args) =>
        {
            var workflow = Control.UIContext.EditedObjectParent as WorkflowInfo;
            if (workflow != null && workflow.WorkflowAutoPublishChanges)
            {
                Control.ShowInformation(Control.GetString("Development-Workflow_Steps.CustomStepsCanNotBeCreated"));
            }
        };
    }


    /// <summary>
    /// DataBinding event handler
    /// </summary>
    protected void DataBinding(object sender, EventArgs e)
    {
        Control.GridView.Sort("StepOrder", SortDirection.Ascending);
    }


    /// <summary>
    /// OnExternalDataBound event handler
    /// </summary>
    protected object OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        WorkflowStepTypeEnum stepType;
        GridViewRow container;

        switch (sourceName.ToLowerCSafe())
        {
            case "allowaction":
                container = (GridViewRow)parameter;
                stepType = (WorkflowStepTypeEnum)ValidationHelper.GetInteger(DataHelper.GetDataRowViewValue((DataRowView)container.DataItem, "StepType"), 3);
                switch (stepType)
                {
                    case WorkflowStepTypeEnum.DocumentEdit:
                    case WorkflowStepTypeEnum.DocumentPublished:
                    case WorkflowStepTypeEnum.DocumentArchived:
                        ((CMSGridActionButton)sender).Visible = false;
                        break;
                }
                break;

            case "steptype":
                stepType = (WorkflowStepTypeEnum)ValidationHelper.GetInteger(parameter, 3);
                WorkflowNode node = WorkflowNode.GetInstance(stepType);
                return HTMLHelper.HTMLEncode(node.Name);

            case "#objectmenu":
                container = (GridViewRow)parameter;
                WorkflowStepInfo step = new WorkflowStepInfo(((DataRowView)container.DataItem).Row);
                if (step.StepIsDefault)
                {
                    CMSGridActionButton button = ((CMSGridActionButton)sender);
                    button.Visible = false;
                }
                break;
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
        int workflowstepid = Convert.ToInt32(actionArgument);

        if (actionName == "delete")
        {
            // Check if documents use the workflow
            WorkflowStepInfo si = WorkflowStepInfoProvider.GetWorkflowStepInfo(workflowstepid);
            if (si != null)
            {
                List<string> documentNames = new List<string>();
                if (WorkflowStepInfoProvider.CheckDependencies(workflowstepid, ref documentNames))
                {
                    // Encode and localize names
                    StringBuilder sb = new StringBuilder();
                    documentNames.ForEach(item => sb.Append("<br />", HTMLHelper.HTMLEncode(ResHelper.LocalizeString(item))));
                    Control.ShowError(Control.GetString("Workflow.CannotDeleteStepUsed"), Control.GetString("workflow.documentlist") + sb);
                }
                else
                {
                    // Delete the workflow step
                    WorkflowStepInfoProvider.DeleteWorkflowStepInfo(workflowstepid);
                }
            }
        }
        else if (actionName == "moveup")
        {
            WorkflowStepInfoProvider.MoveStepUp(WorkflowStepInfoProvider.GetWorkflowStepInfo(workflowstepid));
        }
        else if (actionName == "movedown")
        {
            WorkflowStepInfoProvider.MoveStepDown(WorkflowStepInfoProvider.GetWorkflowStepInfo(workflowstepid));
        }
    }
}