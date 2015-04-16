using System;
using System.Web.UI.WebControls;
using System.Data;

using CMS.Automation;
using CMS.Core;
using CMS.UIControls;
using CMS.OnlineMarketing;
using CMS.Base;
using CMS.WorkflowEngine;
using CMS.ExtendedControls.ActionsConfig;
using CMS.Helpers;
using CMS.DataEngine;
using CMS.MacroEngine;
using CMS.ExtendedControls;

// Parent object
[EditedObject(WorkflowInfo.OBJECT_TYPE_AUTOMATION, "processId")]
[Security(Resource = ModuleName.ONLINEMARKETING, UIElements = "EditProcess;EditProcessTriggers")]
public partial class CMSModules_ContactManagement_Pages_Tools_Automation_Process_Trigger_List : CMSAutomationPage
{
    #region "Properties"

    /// <summary>
    ///  Whether adding triggers is allowed.
    /// </summary>
    protected bool IsAddingAllowed
    {
        get
        {
            return SelectedSiteID > 0 || SelectedSiteID == UniSelector.US_GLOBAL_RECORD;
        }
    }


    /// <summary>
    /// ID of selected site.
    /// </summary>
    protected int SelectedSiteID
    {
        get
        {
            if (IsSiteManager)
            {
                return siteSelector.SiteID;
            }
            else
            {
                return siteOrGlobalSelector.SiteID;
            }
        }
        set
        {
            if (IsSiteManager)
            {
                siteSelector.SiteID = value;
            }
            else
            {
                siteOrGlobalSelector.SiteID = value;
            }
        }
    }


    /// <summary>
    /// Where condition on selected site.
    /// </summary>
    protected string SiteWhereCondition
    {
        get
        {
            if (IsSiteManager)
            {
                return siteSelector.GetWhereCondition("TriggerSiteID");
            }
            else
            {
                return siteOrGlobalSelector.GetWhereCondition();
            }
        }
    }

    #endregion


    #region "Page events"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        siteOrGlobalSelector.TargetObjectType = ObjectWorkflowTriggerInfo.OBJECT_TYPE;
        siteOrGlobalSelector.Visible = !IsSiteManager;
        siteSelector.Visible = IsSiteManager;

        // Initialize site selector with site id from query string
        SelectedSiteID = SiteID;
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        int processId = QueryHelper.GetInteger("processId", 0);

        gridElem.WhereCondition = "TriggerWorkflowID = " + processId;

        // Add query parameters for breadcrumb to edit link
        gridElem.EditActionUrl = AddSiteQuery(gridElem.EditActionUrl, SelectedSiteID) + "&processId=" + processId;

        gridElem.OnExternalDataBound += gridElem_OnExternalDataBound;
        gridElem.OnBeforeDataReload += gridElem_OnBeforeDataReload;

        if (CurrentUser.IsGlobalAdministrator)
        {
            CurrentMaster.DisplaySiteSelectorPanel = true;
            gridElem.WhereCondition = SqlHelper.AddWhereCondition(gridElem.WhereCondition, SiteWhereCondition);
        }
        else
        {
            gridElem.WhereCondition = SqlHelper.AddWhereCondition(gridElem.WhereCondition, "TriggerSiteID = " + SiteID);
        }

        // Add new action
        headerActions.AddAction(new HeaderAction()
        {
            Text = GetString("ma.trigger.new"),
            RedirectUrl = AddSiteQuery("Edit.aspx?processId=" + processId, SelectedSiteID),
            Enabled = IsAddingAllowed
        });
    }


    protected void Page_PreRender(object sender, EventArgs e)
    {
        // Check permissions to create new record
        if (!WorkflowStepInfoProvider.CanUserManageAutomationProcesses(CurrentUser, CurrentSiteName))
        {
            headerActions.Enabled = false;
        }

        // Allow new button only for particular sites or (global) site
        else if (!IsAddingAllowed)
        {
            headerActions.Enabled = false;
            lblWarnNew.Visible = true;
        }
    }


    private void gridElem_OnBeforeDataReload()
    {
        // Show site column only when "all" or "global and this site" are selected
        gridElem.NamedColumns["sitename"].Visible = ((SelectedSiteID < 0) && (SelectedSiteID != UniSelector.US_GLOBAL_RECORD));
    }


    protected object gridElem_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        switch (sourceName.ToLowerCSafe())
        {
            case "condition":
                return MacroRuleTree.GetRuleText(ValidationHelper.GetString(parameter, String.Empty));

            case "type":
                DataRowView row = parameter as DataRowView;
                if (row != null)
                {
                    ObjectWorkflowTriggerInfo trigger = new ObjectWorkflowTriggerInfo(row.Row);
                    if (!string.IsNullOrEmpty(trigger.TriggerTargetObjectType))
                    {
                        return GetTriggerDescription(trigger);
                    }
                    else
                    {
                        return AutomationHelper.GetTriggerName(trigger.TriggerType, trigger.TriggerObjectType);
                    }
                }
                return parameter;

            case "delete":
                if (!WorkflowStepInfoProvider.CanUserManageAutomationProcesses(CurrentUser, CurrentSiteName))
                {
                    CMSGridActionButton btn = (CMSGridActionButton)sender;
                    btn.Enabled = false;
                }
                return parameter;

            default:
                return parameter;
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Returns transformation for trigger type.
    /// </summary>
    /// <param name="trigger">Trigger info object</param>
    private object GetTriggerDescription(ObjectWorkflowTriggerInfo trigger)
    {
        var objectTransformation = new ObjectTransformation(trigger.TriggerTargetObjectType, trigger.TriggerTargetObjectID);
        switch (trigger.TriggerObjectType)
        {
            case ScoreInfo.OBJECT_TYPE:
                objectTransformation.Transformation = string.Format(GetString("ma.trigger.scorereached.listing"), "{% DisplayName %}", trigger.TriggerParameters["ScoreValue"]);
                return objectTransformation;

            case ActivityInfo.OBJECT_TYPE:
                if (trigger.TriggerTargetObjectID == 0)
                {
                    return GetString("ma.trigger.anyActivityPerformed");
                }
                objectTransformation.Transformation = string.Format("{0} '{{% DisplayName %}}'", GetString("ma.trigger.performed"));
                return objectTransformation;
        }
        return null;
    }

    #endregion
}
