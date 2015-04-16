using System;
using System.Linq;

using CMS.Automation;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.OnlineMarketing;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.WorkflowEngine;

public partial class CMSAPIExamples_Code_OnlineMarketing_Automation_Default : CMSAPIExamplePage
{
    #region "Initialization"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Process
        this.apiCreateProcess.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(CreateProcess);
        this.apiGetAndUpdateProcess.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(GetAndUpdateProcess);
        this.apiGetAndBulkUpdateProcesses.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(GetAndBulkUpdateProcesses);
        this.apiDeleteProcess.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(DeleteProcess);

        // Process step
        this.apiCreateProcessStep.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(CreateProcessStep);
        this.apiGetAndUpdateProcessStep.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(GetAndUpdateProcessStep);
        this.apiGetAndBulkUpdateProcessSteps.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(GetAndBulkUpdateProcessSteps);

        // Process transition
        this.apiCreateProcessTransitions.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(CreateProcessTransitions);

        // Process trigger
        this.apiCreateProcessTrigger.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(CreateProcessTrigger);
        this.apiGetAndBulkUpdateProcessTriggers.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(GetAndBulkUpdateProcessTriggers);

        // Contact management - contact
        this.apiCreateTemporaryObjects.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(CreateTemporaryObjects);
        this.apiDeleteTemporaryObjects.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(DeleteTemporaryObjects);

        // Automation state
        this.apiCreateAutomationState.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(CreateAutomationState);
        this.apiMoveContactToNextStep.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(MoveContactToNextStep);
        this.apiMoveContactToPreviousStep.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(MoveContactToPreviousStep);
        this.apiMoveContactToSpecificStep.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(MoveContactToSpecificStep);
        this.apiRemoveContactFromProcess.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(RemoveContactFromProcess);
    }


    #endregion

    #region "Mass actions"

    /// <summary>
    /// Runs all creating and managing examples.
    /// </summary>
    public override void RunAll()
    {
        base.RunAll();

        // Temporary objects
        this.apiCreateTemporaryObjects.Run();

        // Process
        this.apiCreateProcess.Run();
        this.apiGetAndUpdateProcess.Run();
        this.apiGetAndBulkUpdateProcesses.Run();

        // Process step
        this.apiCreateProcessStep.Run();
        this.apiGetAndUpdateProcessStep.Run();
        this.apiGetAndBulkUpdateProcessSteps.Run();

        // Process transition
        this.apiCreateProcessTransitions.Run();

        // Process trigger
        this.apiCreateProcessTrigger.Run();
        this.apiGetAndBulkUpdateProcessTriggers.Run();

        // Automation state
        this.apiCreateAutomationState.Run();
        this.apiMoveContactToNextStep.Run();
        this.apiMoveContactToPreviousStep.Run();
        this.apiMoveContactToSpecificStep.Run();
    }


    /// <summary>
    /// Runs all cleanup examples.
    /// </summary>
    public override void CleanUpAll()
    {
        base.CleanUpAll();

        // Remove contact from process
        this.apiRemoveContactFromProcess.Run();

        // Process
        this.apiDeleteProcess.Run();

        // Temporary objects
        this.apiDeleteTemporaryObjects.Run();
    }

    #endregion

    #region "API examples - Temporary objects"

    /// <summary>
    /// Creates temporary contact management - contact. Called when the "Create contact" button is pressed.
    /// </summary>
    private bool CreateTemporaryObjects()
    {
        // Create new contact object
        ContactInfo newContact = new ContactInfo()
        {
            ContactLastName = "My New Contact",
            ContactFirstName = "My New Firstname",
            ContactSiteID = SiteContext.CurrentSiteID,
            ContactIsAnonymous = true
        };

        // Save the contact
        ContactInfoProvider.SetContactInfo(newContact);

        return true;
    }

    /// <summary>
    /// Creates temporary contact management - contact. Called when the "Create temporary objects" button is pressed.
    /// Expects the CreateTemporaryObjects method to be run first.
    /// </summary>
    private bool DeleteTemporaryObjects()
    {
        // Get dataset of contacts
        string where = "ContactLastName LIKE N'My New Contact%'";
        var contacts = ContactInfoProvider.GetContacts().Where(where);

        if (!DataHelper.DataSourceIsEmpty(contacts))
        {
            // Loop through the individual items
            foreach (ContactInfo contact in contacts)
            {
                // Delete the contact
                ContactInfoProvider.DeleteContactInfo(contact);
            }

            return true;
        }

        return false;
    }

    #endregion

    #region "API examples - Process"

    /// <summary>
    /// Creates process. Called when the "Create process" button is pressed.
    /// </summary>
    private bool CreateProcess()
    {
        // Create new process object and set its properties
        WorkflowInfo newProcess = new WorkflowInfo()
        {
            WorkflowDisplayName = "My new process",
            WorkflowName = "MyNewProcess",
            WorkflowType = WorkflowTypeEnum.Automation,
            WorkflowRecurrenceType = ProcessRecurrenceTypeEnum.Recurring
        };

        // Save the process
        WorkflowInfoProvider.SetWorkflowInfo(newProcess);

        // Create default steps
        WorkflowStepInfoProvider.CreateDefaultWorkflowSteps(newProcess);

        // Get the step with codename 'Finished' and allow Move to previous
        WorkflowStepInfo finishedStep = WorkflowStepInfoProvider.GetWorkflowStepInfo("Finished", newProcess.WorkflowID);
        finishedStep.StepAllowReject = true;

        // Save the 'Finished' step
        WorkflowStepInfoProvider.SetWorkflowStepInfo(finishedStep);

        return true;
    }


    /// <summary>
    /// Gets and updates process. Called when the "Get and update process" button is pressed.
    /// Expects the CreateProcess method to be run first.
    /// </summary>
    private bool GetAndUpdateProcess()
    {
        // Get the process
        WorkflowInfo modifyProcess = WorkflowInfoProvider.GetWorkflowInfo("MyNewProcess", WorkflowTypeEnum.Automation);

        if (modifyProcess != null)
        {
            // Update the properties
            modifyProcess.WorkflowDisplayName = modifyProcess.WorkflowDisplayName.ToLower();

            // Save the changes
            WorkflowInfoProvider.SetWorkflowInfo(modifyProcess);

            return true;
        }

        return false;
    }


    /// <summary>
    /// Gets and bulk updates process. Called when the "Get and bulk update processes" button is pressed.
    /// Expects the CreateProcess method to be run first.
    /// </summary>
    private bool GetAndBulkUpdateProcesses()
    {
        // Prepare the parameters
        string where = "WorkflowName LIKE N'MyNewProcess%'";

        // Get the data
        InfoDataSet<WorkflowInfo> processes = WorkflowInfoProvider.GetWorkflows(where, null, 0, null);

        if (!DataHelper.DataSourceIsEmpty(processes))
        {
            // Loop through the individual items
            foreach (WorkflowInfo modifyProcess in processes)
            {
                // Update the properties
                modifyProcess.WorkflowDisplayName = modifyProcess.WorkflowDisplayName.ToUpper();

                // Save the changes
                WorkflowInfoProvider.SetWorkflowInfo(modifyProcess);
            }

            return true;
        }

        return false;
    }


    /// <summary>
    /// Deletes process. Called when the "Delete process" button is pressed.
    /// Expects the CreateProcess method to be run first.
    /// </summary>
    private bool DeleteProcess()
    {
        // Get the process
        WorkflowInfo deleteProcess = WorkflowInfoProvider.GetWorkflowInfo("MyNewProcess", WorkflowTypeEnum.Automation);

        // Delete the process
        WorkflowInfoProvider.DeleteWorkflowInfo(deleteProcess);

        return (deleteProcess != null);
    }

    #endregion

    #region "API examples - Process step"

    /// <summary>
    /// Creates process step. Called when the "Create step" button is pressed.
    /// Expects the CreateProcess method to be run first.
    /// </summary>
    private bool CreateProcessStep()
    {
        // Get the process
        WorkflowInfo process = WorkflowInfoProvider.GetWorkflowInfo("MyNewProcess", WorkflowTypeEnum.Automation);

        if (process != null)
        {
            // Create new process step object and set its properties
            WorkflowStepInfo newStep = new WorkflowStepInfo()
            {
                StepWorkflowID = process.WorkflowID,
                StepName = "MyNewProcessStep",
                StepDisplayName = "My new step",
                StepType = WorkflowStepTypeEnum.Standard
            };

            // Save the process step
            WorkflowStepInfoProvider.SetWorkflowStepInfo(newStep);

            return true;
        }

        return false;
    }


    /// <summary>
    /// Gets and updates process step. Called when the "Get and update step" button is pressed.
    /// Expects the CreateProcessStep method to be run first.
    /// </summary>
    private bool GetAndUpdateProcessStep()
    {
        // Get the process
        WorkflowInfo process = WorkflowInfoProvider.GetWorkflowInfo("MyNewProcess", WorkflowTypeEnum.Automation);
        if (process != null)
        {
            // Get the process step
            WorkflowStepInfo modifyStep = WorkflowStepInfoProvider.GetWorkflowStepInfo("MyNewProcessStep", process.WorkflowID);
            if (modifyStep != null)
            {
                // Update the properties
                modifyStep.StepDisplayName = modifyStep.StepDisplayName.ToLower();

                // Save the changes
                WorkflowStepInfoProvider.SetWorkflowStepInfo(modifyStep);

                return true;
            }
        }

        return false;
    }


    /// <summary>
    /// Gets and bulk updates process steps. Called when the "Get and bulk update steps" button is pressed.
    /// Expects the CreateProcessStep method to be run first.
    /// </summary>
    private bool GetAndBulkUpdateProcessSteps()
    {
        // Get the data
        var steps = WorkflowStepInfoProvider.GetWorkflowSteps().Where("StepName LIKE N'MyNewProcessStep%'");

        if (!DataHelper.DataSourceIsEmpty(steps))
        {
            // Loop through the individual items
            foreach (WorkflowStepInfo modifyStep in steps)
            {
                // Update the properties
                modifyStep.StepDisplayName = modifyStep.StepDisplayName.ToUpper();

                // Save the changes
                WorkflowStepInfoProvider.SetWorkflowStepInfo(modifyStep);
            }

            return true;
        }

        return false;
    }

    #endregion

    #region "API examples - Process transition"

    /// <summary>
    /// Creates process transitions. Called when the "Create transitions" button is pressed.
    /// Expects the CreateProcess and CreateProcessStep method to be run first.
    /// </summary>
    private bool CreateProcessTransitions()
    {
        // Get the process
        WorkflowInfo process = WorkflowInfoProvider.GetWorkflowInfo("MyNewProcess", WorkflowTypeEnum.Automation);

        if (process != null)
        {
            // Get the previously created process step
            WorkflowStepInfo step = WorkflowStepInfoProvider.GetWorkflowStepInfo("MyNewProcessStep", process.WorkflowID);

            // Get the step with codename 'Finished'
            WorkflowStepInfo finishedStep = WorkflowStepInfoProvider.GetWorkflowStepInfo("Finished", process.WorkflowID);

            if ((step != null) && (finishedStep != null))
            {
                // Get existed transition from 'Start' step to 'Finished' step
                InfoDataSet<WorkflowTransitionInfo> transitions = WorkflowTransitionInfoProvider.GetWorkflowTransitions(process.WorkflowID, null, null, 1, null);
                WorkflowTransitionInfo existedTransition = transitions.First<WorkflowTransitionInfo>();

                // Change existed transition to leads from 'Start' step to 'My new step' step
                existedTransition.TransitionEndStepID = step.StepID;
                // Save existed transition
                WorkflowTransitionInfoProvider.SetWorkflowTransitionInfo(existedTransition);

                // Connect 'My new step' step to 'Finished' step
                step.ConnectTo(step.StepDefinition.SourcePoints[0].Guid, finishedStep);

                return true;
            }
        }

        return false;
    }

    #endregion

    #region "API examples - Process trigger"

    /// <summary>
    /// Creates process trigger. Called when the "Create trigger" button is pressed.
    /// Expects the CreateProcess method to be run first.
    /// </summary>
    private bool CreateProcessTrigger()
    {
        // Get the process
        WorkflowInfo process = WorkflowInfoProvider.GetWorkflowInfo("MyNewProcess", WorkflowTypeEnum.Automation);

        if (process != null)
        {
            // Create new process trigger object and set its properties
            ObjectWorkflowTriggerInfo newTrigger = new ObjectWorkflowTriggerInfo()
            {
                TriggerDisplayName = "My new trigger",
                TriggerType = WorkflowTriggerTypeEnum.Change,
                TriggerSiteID = SiteContext.CurrentSiteID,
                TriggerWorkflowID = process.WorkflowID,
                TriggerObjectType = "om.contact"
            };

            // Save the process trigger
            ObjectWorkflowTriggerInfoProvider.SetObjectWorkflowTriggerInfo(newTrigger);

            return true;
        }

        return false;
    }


    /// <summary>
    /// Gets and bulk updates process triggers. Called when the "Get and bulk update triggers" button is pressed.
    /// Expects the CreateProcessTrigger method to be run first.
    /// </summary>
    private bool GetAndBulkUpdateProcessTriggers()
    {
        // Prepare the parameters
        string where = "TriggerDisplayName LIKE N'My new trigger'";

        // Get the data
        InfoDataSet<ObjectWorkflowTriggerInfo> triggers = ObjectWorkflowTriggerInfoProvider.GetObjectWorkflowTriggers(where, null);

        if (!DataHelper.DataSourceIsEmpty(triggers))
        {
            // Loop through the individual items
            foreach (ObjectWorkflowTriggerInfo modifyTrigger in triggers)
            {
                // Update the properties
                modifyTrigger.TriggerDisplayName = modifyTrigger.TriggerDisplayName.ToUpper();

                // Save the changes
                ObjectWorkflowTriggerInfoProvider.SetObjectWorkflowTriggerInfo(modifyTrigger);
            }

            return true;
        }

        return false;
    }

    #endregion

    #region "API examples - automation state"

    /// <summary>
    /// Creates automation state. Called when the "Start process" button is pressed.
    /// Expects the CreateProcess, CreateProcessStep and CreateTemporaryObjects method to be run first.
    /// </summary>
    private bool CreateAutomationState()
    {
        // Get dataset of contacts
        string where = "ContactLastName LIKE N'My New Contact%'";
        int topN = 1;
        var contacts = ContactInfoProvider.GetContacts().Where(where).TopN(topN);

        // Get the process
        WorkflowInfo process = WorkflowInfoProvider.GetWorkflowInfo("MyNewProcess", WorkflowTypeEnum.Automation);

        if (!DataHelper.DataSourceIsEmpty(contacts) && (process != null))
        {
            // Get the contact from dataset
            ContactInfo contact = contacts.First<ContactInfo>();

            // Get the instance of automation manager
            AutomationManager manager = AutomationManager.GetInstance(CurrentUser);

            // Start the process
            manager.StartProcess(contact, process.WorkflowID);

            return true;
        }

        return false;
    }

    /// <summary>
    /// Gets the automation state and move contact to next step. Called when the "Move to next step" button is pressed.
    /// Expects the CreateAutomationState method to be run first.
    /// </summary>
    private bool MoveContactToNextStep()
    {
        // Get dataset of contacts
        string where = "ContactLastName LIKE N'My New Contact%'";
        int topN = 1;
        var contacts = ContactInfoProvider.GetContacts().Where(where).TopN(topN);

        // Get the process
        WorkflowInfo process = WorkflowInfoProvider.GetWorkflowInfo("MyNewProcess", WorkflowTypeEnum.Automation);

        if (!DataHelper.DataSourceIsEmpty(contacts) && (process != null))
        {
            // Get the contact from dataset
            ContactInfo contact = contacts.First<ContactInfo>();

            // Get the instance of automation manager
            AutomationManager manager = AutomationManager.GetInstance(CurrentUser);

            // Get the process state
            AutomationStateInfo state = contact.Processes.FirstItem as AutomationStateInfo;

            if (state != null)
            {
                // Move contact to next step                
                manager.MoveToNextStep(contact, state, "Move to next step");

                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Gets the automation state and move contact to previous step. Called when the "Move to previous step" button is pressed.
    /// Expects the CreateAutomationState method to be run first.
    /// </summary>
    private bool MoveContactToPreviousStep()
    {
        // Get dataset of contacts
        string where = "ContactLastName LIKE N'My New Contact%'";
        int topN = 1;
        var contacts = ContactInfoProvider.GetContacts().Where(where).TopN(topN);

        // Get the process
        WorkflowInfo process = WorkflowInfoProvider.GetWorkflowInfo("MyNewProcess", WorkflowTypeEnum.Automation);

        if (!DataHelper.DataSourceIsEmpty(contacts) && (process != null))
        {
            // Get the contact from dataset
            ContactInfo contact = contacts.First<ContactInfo>();

            // Get the instance of automation manager
            AutomationManager manager = AutomationManager.GetInstance(CurrentUser);

            // Get the process state
            AutomationStateInfo state = contact.Processes.FirstItem as AutomationStateInfo;

            if (state != null)
            {
                // Move contact to next step                
                manager.MoveToPreviousStep(contact, state, "Move to previous step");

                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Gets the automation state and move contact to specific step. Called when the "Move to specific step" button is pressed.
    /// Expects the CreateAutomationState method to be run first.
    /// </summary>
    private bool MoveContactToSpecificStep()
    {
        // Get dataset of contacts
        string where = "ContactLastName LIKE N'My New Contact%'";
        int topN = 1;
        var contacts = ContactInfoProvider.GetContacts().Where(where).TopN(topN);

        // Get the process
        WorkflowInfo process = WorkflowInfoProvider.GetWorkflowInfo("MyNewProcess", WorkflowTypeEnum.Automation);

        if (!DataHelper.DataSourceIsEmpty(contacts) && (process != null))
        {
            // Get the contact from dataset
            ContactInfo contact = contacts.First<ContactInfo>();

            // Get the instance of automation manager
            AutomationManager manager = AutomationManager.GetInstance(CurrentUser);

            // Get the automation state
            AutomationStateInfo state = contact.Processes.FirstItem as AutomationStateInfo;

            if (state != null)
            {
                // Get the finished step
                WorkflowStepInfo finishedStep = manager.GetFinishedStep(contact, state);

                // Move contact to specific step
                manager.MoveToSpecificStep(contact, state, finishedStep, "Move to specific step");

                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Remove contact from process. Called when the "Remove contact from process" button is pressed.
    /// Expects the CreateAutomationState method to be run first.
    /// </summary>
    private bool RemoveContactFromProcess()
    {
        // Get dataset of contacts
        string where = "ContactLastName LIKE N'My New Contact%'";
        int topN = 1;
        var contacts = ContactInfoProvider.GetContacts().Where(where).TopN(topN);

        // Get the process
        WorkflowInfo process = WorkflowInfoProvider.GetWorkflowInfo("MyNewProcess", WorkflowTypeEnum.Automation);

        if (DataHelper.DataSourceIsEmpty(contacts) || (process == null))
        {
            return false;
        }

        // Get the contact from dataset
        ContactInfo contact = contacts.First<ContactInfo>();

        // Get the instance of automation manager
        AutomationManager manager = AutomationManager.GetInstance(CurrentUser);

        // Get the states
        var states = AutomationStateInfoProvider.GetAutomationStates()
                                                .WhereEquals("StateWorkflowID", process.WorkflowID)
                                                .WhereEquals("StateObjectID", contact.ContactID)
                                                .WhereEquals("StateObjectType", PredefinedObjectType.CONTACT);
        if (states.Any())
        {
            // Loop through the individual items
            foreach (AutomationStateInfo state in states)
            {
                // Remove contact from process
                manager.RemoveProcess(contact, state);
            }

            return true;
        }

        return false;
    }

    #endregion

}