using System;
using System.Linq;
using CMS.Helpers;
using CMS.Base;
using CMS.Localization;
using CMS.SiteProvider;
using CMS.Membership;
using CMS.UIControls;
using CMS.WorkflowEngine;
using CMS.DataEngine;

public partial class CMSAPIExamples_Code_Development_Workflows_Default : CMSAPIExamplePage
{
    #region "Initialization"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Workflow
        apiCreateWorkflow.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(CreateWorkflow);
        apiGetAndUpdateWorkflow.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(GetAndUpdateWorkflow);
        apiGetAndBulkUpdateWorkflows.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(GetAndBulkUpdateWorkflows);
        apiDeleteWorkflow.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(DeleteWorkflow);

        // Workflow step
        apiCreateWorkflowStep.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(CreateWorkflowStep);
        apiAddRoleToStep.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(AddRoleToStep);
        apiRemoveRoleFromStep.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(RemoveRoleFromStep);
        apiDeleteWorkflowStep.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(DeleteWorkflowStep);

        // Workflow scope
        apiCreateWorkflowScope.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(CreateWorkflowScope);
        apiGetAndUpdateWorkflowScope.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(GetAndUpdateWorkflowScope);
        apiDeleteWorkflowScope.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(DeleteWorkflowScope);

        // Advanced workflow
        apiConvertToAdvancedWorkflow.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(ConvertToAdvancedWorkflow);

        // Workflow actions
        apiCreateAction.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(CreateAction);
        apiGetAndUpdateAction.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(GetAndUpdateAction);
        apiGetAndBulkUpdateActions.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(GetAndBulkUpdateActions);
        apiDeleteAction.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(DeleteAction);

        // Workflow step transitions
        apiCreateStepForTransition.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(CreateStepForTransition);
        apiCreateTransition.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(CreateTransition);
        apiDeleteTransition.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(DeleteTransition);
    }

    #endregion


    #region "Mass actions"

    /// <summary>
    /// Runs all creating and managing examples.
    /// </summary>
    public override void RunAll()
    {
        base.RunAll();

        // Workflow
        apiCreateWorkflow.Run();
        apiGetAndUpdateWorkflow.Run();
        apiGetAndBulkUpdateWorkflows.Run();

        // Workflow step
        apiCreateWorkflowStep.Run();
        apiAddRoleToStep.Run();

        // Workflow scope
        apiCreateWorkflowScope.Run();
        apiGetAndUpdateWorkflowScope.Run();

        // Advanced workflow
        apiConvertToAdvancedWorkflow.Run();

        // Workflow action
        apiCreateAction.Run();
        apiGetAndUpdateAction.Run();
        apiGetAndBulkUpdateActions.Run();

        // Workflow step transition
        apiCreateStepForTransition.Run();
        apiCreateTransition.Run();
    }


    /// <summary>
    /// Runs all cleanup examples.
    /// </summary>
    public override void CleanUpAll()
    {
        base.CleanUpAll();

        // Workflow step transition
        apiDeleteTransition.Run();

        // Workflow action
        apiDeleteAction.Run();

        // Workflow scope
        apiDeleteWorkflowScope.Run();

        // Workflow step
        apiRemoveRoleFromStep.Run();
        apiDeleteWorkflowStep.Run();

        // Workflow
        apiDeleteWorkflow.Run();
    }

    #endregion


    #region "API examples - Workflow"

    /// <summary>
    /// Creates workflow. Called when the "Create workflow" button is pressed.
    /// </summary>
    private bool CreateWorkflow()
    {
        // Create new workflow object
        WorkflowInfo newWorkflow = new WorkflowInfo();

        // Set the properties
        newWorkflow.WorkflowDisplayName = "My new workflow";
        newWorkflow.WorkflowName = "MyNewWorkflow";

        // Save the workflow
        WorkflowInfoProvider.SetWorkflowInfo(newWorkflow);

        // Create the three default workflow steps
        WorkflowStepInfoProvider.CreateDefaultWorkflowSteps(newWorkflow);

        return true;
    }


    /// <summary>
    /// Gets and updates workflow. Called when the "Get and update workflow" button is pressed.
    /// Expects the CreateWorkflow method to be run first.
    /// </summary>
    private bool GetAndUpdateWorkflow()
    {
        // Get the workflow
        WorkflowInfo updateWorkflow = WorkflowInfoProvider.GetWorkflowInfo("MyNewWorkflow");
        if (updateWorkflow != null)
        {
            // Update the properties
            updateWorkflow.WorkflowDisplayName = updateWorkflow.WorkflowDisplayName.ToLowerCSafe();

            // Save the changes
            WorkflowInfoProvider.SetWorkflowInfo(updateWorkflow);
            return true;
        }

        return false;
    }


    /// <summary>
    /// Gets and bulk updates workflows. Called when the "Get and bulk update workflows" button is pressed.
    /// Expects the CreateWorkflow method to be run first.
    /// </summary>
    private bool GetAndBulkUpdateWorkflows()
    {
        // Prepare the parameters
        string where = "WorkflowName LIKE N'MyNewWorkflow%'";

        // Get the data
        InfoDataSet<WorkflowInfo> workflows = WorkflowInfoProvider.GetWorkflows(where, null, 0, null);
        if (!DataHelper.DataSourceIsEmpty(workflows))
        {
            // Loop through the individual items
            foreach (WorkflowInfo modifyWorkflow in workflows)
            {
                // Update the properties
                modifyWorkflow.WorkflowDisplayName = modifyWorkflow.WorkflowDisplayName.ToUpper();

                // Save the changes
                WorkflowInfoProvider.SetWorkflowInfo(modifyWorkflow);
            }

            return true;
        }

        return false;
    }


    /// <summary>
    /// Deletes workflow. Called when the "Delete workflow" button is pressed.
    /// Expects the CreateWorkflow method to be run first.
    /// </summary>
    private bool DeleteWorkflow()
    {
        // Get the workflow
        WorkflowInfo deleteWorkflow = WorkflowInfoProvider.GetWorkflowInfo("MyNewWorkflow", WorkflowTypeEnum.Approval);

        // Delete the workflow
        WorkflowInfoProvider.DeleteWorkflowInfo(deleteWorkflow);

        return (deleteWorkflow != null);
    }

    #endregion


    #region "API Examples - Workflow step"

    /// <summary>
    /// Creates a new workflow step. Called when the "Create workflow step" button is pressed.
    /// Expects the CreateWorkflow method to be run first.
    /// </summary>
    private bool CreateWorkflowStep()
    {
        // Get the workflow
        WorkflowInfo myWorkflow = WorkflowInfoProvider.GetWorkflowInfo("MyNewWorkflow");
        if (myWorkflow != null)
        {
            // Create new workflow step object
            WorkflowStepInfo newStep = new WorkflowStepInfo();

            // Set the properties
            newStep.StepWorkflowID = myWorkflow.WorkflowID;
            newStep.StepName = "MyNewWorkflowStep";
            newStep.StepDisplayName = "My new workflow step";
            newStep.StepOrder = 1;
            newStep.StepType = WorkflowStepTypeEnum.Standard;

            // Save the step into database
            WorkflowStepInfoProvider.SetWorkflowStepInfo(newStep);

            // Ensure correct step order
            WorkflowStepInfoProvider.InitStepOrders(myWorkflow);

            return true;
        }

        return false;
    }


    /// <summary>
    /// Assings the CMS Editors role to a workflow step. Called when the "Add role to step" button is pressed.
    /// Expects the CreateWorkflow and CreateWorkflowStep methods to be run first.
    /// </summary>
    private bool AddRoleToStep()
    {
        // Get the workflow
        WorkflowInfo workflow = WorkflowInfoProvider.GetWorkflowInfo("MyNewWorkflow");

        if (workflow != null)
        {
            // Get the custom step
            WorkflowStepInfo step = WorkflowStepInfoProvider.GetWorkflowStepInfo("MyNewWorkflowStep", workflow.WorkflowID);

            if (step != null)
            {
                // Get the role to be assigned to the step
                RoleInfo role = RoleInfoProvider.GetRoleInfo("CMSEditor", SiteContext.CurrentSiteID);

                if (role != null)
                {
                    // Make the assignment
                    WorkflowStepRoleInfoProvider.AddRoleToWorkflowStep(step.StepID, role.RoleID);

                    return true;
                }
                else
                {
                    // Role was not found
                    apiAddRoleToStep.ErrorMessage = "Role 'CMS Editors' was not found.";
                }
            }
            else
            {
                // Step was not found
                apiAddRoleToStep.ErrorMessage = "Step 'My new workflow step' was not found.";
            }
        }

        return false;
    }


    /// <summary>
    /// Removes the assignment of the CMS Editors role from a workflow step. Called when the "Remove role from step" button is pressed.
    /// Expects the CreateWorkflow, CreateWorkflowStep and AddRoleToStep methods to be run first.
    /// </summary>
    private bool RemoveRoleFromStep()
    {
        // Get the workflow
        WorkflowInfo workflow = WorkflowInfoProvider.GetWorkflowInfo("MyNewWorkflow", WorkflowTypeEnum.Approval);

        if (workflow != null)
        {
            // Get the custom step
            WorkflowStepInfo step = WorkflowStepInfoProvider.GetWorkflowStepInfo("MyNewWorkflowStep", workflow.WorkflowID);

            if (step != null)
            {
                // Get the role to be assigned to the step
                RoleInfo role = RoleInfoProvider.GetRoleInfo("CMSEditor", SiteContext.CurrentSiteID);

                if (role != null)
                {
                    // Get the step - role relationship
                    WorkflowStepRoleInfo stepRoleInfo = WorkflowStepRoleInfoProvider.GetWorkflowStepRoleInfo(step.StepID, role.RoleID);

                    if (stepRoleInfo != null)
                    {
                        // Remove the assignment
                        WorkflowStepRoleInfoProvider.RemoveRoleFromWorkflowStep(step.StepID, role.RoleID);

                        return true;
                    }
                    else
                    {
                        // The role is not assigned to the step
                        apiRemoveRoleFromStep.ErrorMessage = "The 'CMS Editors' role is not assigned to the step.";
                    }
                }
                else
                {
                    // The role was not found
                    apiRemoveRoleFromStep.ErrorMessage = "The role 'CMS Editors' was not found.";
                }
            }
            else
            {
                // The step was not found
                apiRemoveRoleFromStep.ErrorMessage = "The step 'My new workflow step' was not found.";
            }
        }

        return false;
    }


    /// <summary>
    /// Deletes the workflow step. Called when the "Delete workflow step" button is pressed.
    /// Expects the CreateWorkflow and CreateWorkflowStep methods to be run first.
    /// </summary>
    private bool DeleteWorkflowStep()
    {
        // Get the workflow
        WorkflowInfo workflow = WorkflowInfoProvider.GetWorkflowInfo("MyNewWorkflow", WorkflowTypeEnum.Approval);

        if (workflow != null)
        {
            // Get the custom step
            WorkflowStepInfo deleteStep = WorkflowStepInfoProvider.GetWorkflowStepInfo("MyNewWorkflowStep", workflow.WorkflowID);

            if (deleteStep != null)
            {
                // Remove the step
                WorkflowStepInfoProvider.DeleteWorkflowStepInfo(deleteStep);
                return true;
            }
        }

        return false;
    }

    #endregion


    #region "API Examples - Workflow scope"

    /// <summary>
    /// Creates workflow scope. Called when the "Create scope" button is pressed.
    /// Expects the "CreateWorkflow" method to be run first.
    /// </summary>
    private bool CreateWorkflowScope()
    {
        // Get the workflow
        WorkflowInfo workflow = WorkflowInfoProvider.GetWorkflowInfo("MyNewWorkflow");

        if (workflow != null)
        {
            // Create new workflow scope object
            WorkflowScopeInfo newScope = new WorkflowScopeInfo();

            // Get the site default culture from settings
            string cultureCode = SettingsKeyInfoProvider.GetValue(SiteContext.CurrentSiteName + ".CMSDefaultCultureCode");
            CultureInfo culture = CultureInfoProvider.GetCultureInfo(cultureCode);

            // Get root document type class ID
            int classID = DataClassInfoProvider.GetDataClassInfo("CMS.Root").ClassID;

            // Set the properties
            newScope.ScopeStartingPath = "/";
            newScope.ScopeCultureID = culture.CultureID;
            newScope.ScopeClassID = classID;

            newScope.ScopeWorkflowID = workflow.WorkflowID;
            newScope.ScopeSiteID = SiteContext.CurrentSiteID;

            // Save the workflow scope
            WorkflowScopeInfoProvider.SetWorkflowScopeInfo(newScope);

            return true;
        }

        return false;
    }


    /// <summary>
    /// Gets and updates workflow scope. Called when the "Get and update scope" button is pressed.
    /// Expects the CreateWorkflowScope method to be run first.
    /// </summary>
    private bool GetAndUpdateWorkflowScope()
    {
        // Get the workflow
        WorkflowInfo workflow = WorkflowInfoProvider.GetWorkflowInfo("MyNewWorkflow");

        if (workflow != null)
        {
            // Get the workflow's scopes
            InfoDataSet<WorkflowScopeInfo> scopes = WorkflowScopeInfoProvider.GetWorkflowScopes(workflow.WorkflowID);

            if (!DataHelper.DataSourceIsEmpty(scopes))
            {
                // Create the scope info object
                WorkflowScopeInfo updateScope = scopes.First<WorkflowScopeInfo>();

                // Update the properties - the scope will include all cultures and document types
                updateScope.ScopeCultureID = 0;
                updateScope.ScopeClassID = 0;

                // Save the changes
                WorkflowScopeInfoProvider.SetWorkflowScopeInfo(updateScope);

                return true;
            }
            else
            {
                // No scope was found
                apiGetAndUpdateWorkflowScope.ErrorMessage = "The scope was not found.";
            }
        }

        return false;
    }


    /// <summary>
    /// Deletes workflow scope. Called when the "Delete scope" button is pressed.
    /// Expects the CreateWorkflowScope method to be run first.
    /// </summary>
    private bool DeleteWorkflowScope()
    {
        // Get the workflow
        WorkflowInfo workflow = WorkflowInfoProvider.GetWorkflowInfo("MyNewWorkflow", WorkflowTypeEnum.Approval);

        if (workflow != null)
        {
            // Get the workflow's scopes
            InfoDataSet<WorkflowScopeInfo> scopes = WorkflowScopeInfoProvider.GetWorkflowScopes(workflow.WorkflowID);

            if (!DataHelper.DataSourceIsEmpty(scopes))
            {
                // Create the scope info object
                WorkflowScopeInfo deleteScope = scopes.First<WorkflowScopeInfo>();

                // Delete the workflow scope
                WorkflowScopeInfoProvider.DeleteWorkflowScopeInfo(deleteScope);

                return true;
            }
            else
            {
                // No scope was found
                apiDeleteWorkflowScope.ErrorMessage = "The scope was not found.";
            }
        }

        return false;
    }

    #endregion


    #region "API Examples - Workflow actions"

    /// <summary>
    /// Creates workflow action. Called when the "Create action" button is pressed.
    /// </summary>
    private bool CreateAction()
    {
        // Create new workflow action
        WorkflowActionInfo newAction = new WorkflowActionInfo();

        // Set the properties
        newAction.ActionDisplayName = "My new action";
        newAction.ActionName = "MyNewAction";
        newAction.ActionAssemblyName = "MyNewActionAssembly";
        newAction.ActionClass = "MyNewActionClass";
        newAction.ActionEnabled = true;
        newAction.ActionWorkflowType = WorkflowTypeEnum.Approval;

        // Save the action
        WorkflowActionInfoProvider.SetWorkflowActionInfo(newAction);

        return true;
    }

    /// <summary>
    /// Gets and updates action. Called when the "Get and update action" button is pressed.
    /// Expects the CreateAction method to be run first.
    /// </summary>
    private bool GetAndUpdateAction()
    {
        // Get the action
        WorkflowActionInfo updateAction = WorkflowActionInfoProvider.GetWorkflowActionInfo("MyNewAction", WorkflowTypeEnum.Approval);
        if (updateAction != null)
        {
            // Update the properties
            updateAction.ActionDisplayName = updateAction.ActionDisplayName.ToLowerCSafe();

            // Save the changes
            WorkflowActionInfoProvider.SetWorkflowActionInfo(updateAction);
            return true;
        }

        return false;
    }


    /// <summary>
    /// Gets and bulk updates actions. Called when the "Get and bulk update actions" button is pressed.
    /// Expects the CreateAction method to be run first.
    /// </summary>
    private bool GetAndBulkUpdateActions()
    {
        // Prepare the parameters
        string where = "ActionName LIKE N'MyNewAction%'";

        // Get the data
        InfoDataSet<WorkflowActionInfo> actions = WorkflowActionInfoProvider.GetWorkflowActions(where, null);
        if (!DataHelper.DataSourceIsEmpty(actions))
        {
            // Loop through the individual items
            foreach (WorkflowActionInfo modifyAction in actions)
            {
                // Update the properties
                modifyAction.ActionDisplayName = modifyAction.ActionDisplayName.ToUpper();

                // Save the changes
                WorkflowActionInfoProvider.SetWorkflowActionInfo(modifyAction);
            }

            return true;
        }

        return false;
    }


    /// <summary>
    /// Deletes action. Called when the "Delete action" button is pressed.
    /// Expects the CreateAction method to be run first.
    /// </summary>
    private bool DeleteAction()
    {
        // Get the action
        WorkflowActionInfo deleteAction = WorkflowActionInfoProvider.GetWorkflowActionInfo("MyNewAction", WorkflowTypeEnum.Approval);

        // Delete the action
        WorkflowActionInfoProvider.DeleteWorkflowActionInfo(deleteAction);

        return (deleteAction != null);
    }

    #endregion


    #region "API Examples - Advanced workflow"

    /// <summary>
    /// Converts existing workflow to advanced workflow. Called when the "Convert to advanced workflow" button is pressed.
    /// Expects the CreateWorkflow method to be run first.
    /// </summary>
    private bool ConvertToAdvancedWorkflow()
    {
        // Get the workflow
        WorkflowInfo convertWorkflow = WorkflowInfoProvider.GetWorkflowInfo("MyNewWorkflow");
        if (convertWorkflow != null)
        {
            // Convert to advanced workflow
            WorkflowInfoProvider.ConvertToAdvancedWorkflow(convertWorkflow.WorkflowID);

            return true;
        }

        return false;
    }

    #endregion


    #region "API Examples - Workflow transitions"

    /// <summary>
    /// Creates workflow step. Called when the "Create step" button is pressed.
    /// Expects the CreateWorkflow method to be run first.
    /// </summary>
    private bool CreateStepForTransition()
    {
        // Get the workflow
        WorkflowInfo workflow = WorkflowInfoProvider.GetWorkflowInfo("MyNewWorkflow", WorkflowTypeEnum.Approval);

        if (workflow != null)
        {
            // Create new workflow step object and set its properties
            WorkflowStepInfo newStep = new WorkflowStepInfo()
            {
                StepWorkflowID = workflow.WorkflowID,
                StepName = "MyNewStep",
                StepDisplayName = "My new step",
                StepType = WorkflowStepTypeEnum.Standard
            };

            // Save the step
            WorkflowStepInfoProvider.SetWorkflowStepInfo(newStep);

            return true;
        }

        return false;
    }

    /// <summary>
    /// Creates workflow transitions. Called when the "Create transition" button is pressed.
    /// Expects the CreateWorklow and CreateStep method to be run first.
    /// </summary>
    private bool CreateTransition()
    {
        // Get the workflow
        WorkflowInfo worklow = WorkflowInfoProvider.GetWorkflowInfo("MyNewWorkflow", WorkflowTypeEnum.Approval);

        if (worklow != null)
        {
            // Get steps with codename 'MyNewStep' and 'Published'
            WorkflowStepInfo myNewStep = WorkflowStepInfoProvider.GetWorkflowStepInfo("MyNewStep", worklow.WorkflowID);
            WorkflowStepInfo publishedStep = WorkflowStepInfoProvider.GetWorkflowStepInfo("Published", worklow.WorkflowID);

            if ((myNewStep != null) && (publishedStep != null))
            {
                // Get existing transition leading to 'Published step'
                string where = "TransitionEndStepID = " + publishedStep.StepID;
                InfoDataSet<WorkflowTransitionInfo> transitions = WorkflowTransitionInfoProvider.GetWorkflowTransitions(worklow.WorkflowID, where, null, 1, null);
                WorkflowTransitionInfo existingTransition = transitions.First<WorkflowTransitionInfo>();

                // Change existing transition to leads from 'Start step' to 'My new step'
                existingTransition.TransitionEndStepID = myNewStep.StepID;

                // Save existing transition
                WorkflowTransitionInfoProvider.SetWorkflowTransitionInfo(existingTransition);

                // Connect 'My new step' step to 'Published' step
                myNewStep.ConnectTo(myNewStep.StepDefinition.SourcePoints[0].Guid, publishedStep);

                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Deletes workflow transition. Called when the "Delete transition" button is pressed.
    /// Expects the CreateWorklow and CreateTransitions method to be run first.
    /// </summary>
    private bool DeleteTransition()
    {
        // Get the process
        WorkflowInfo worklow = WorkflowInfoProvider.GetWorkflowInfo("MyNewWorkflow", WorkflowTypeEnum.Approval);

        if (worklow != null)
        {
            // Get step
            WorkflowStepInfo startStep = WorkflowStepInfoProvider.GetWorkflowStepInfo("MyNewStep", worklow.WorkflowID);

            if (startStep != null)
            {
                // Get existing transition leading from 'My new step'
                string where = "TransitionStartStepID = " + startStep.StepID;
                InfoDataSet<WorkflowTransitionInfo> transitions = WorkflowTransitionInfoProvider.GetWorkflowTransitions(worklow.WorkflowID, where, null, 1, null);
                WorkflowTransitionInfo existingTransition = transitions.First<WorkflowTransitionInfo>();

                if (existingTransition != null)
                {
                    // Delete transition
                    WorkflowTransitionInfoProvider.DeleteWorkflowTransitionInfo(existingTransition);

                    return true;
                }
            }
        }

        return false;
    }

    #endregion
}