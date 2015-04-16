using System;
using System.Data;
using System.Web.UI.WebControls;

using CMS.Automation;
using CMS.Core;
using CMS.PortalEngine;
using CMS.UIControls;
using CMS.Base;
using CMS.WorkflowEngine;
using CMS.Helpers;
using CMS.OnlineMarketing;
using CMS.SiteProvider;
using CMS.ExtendedControls;

public partial class CMSModules_ContactManagement_Controls_UI_Automation_Contacts : CMSAdminEditControl
{
    #region "Public properties"

    /// <summary>
    /// Gets or sets current identifier.
    /// </summary>
    public int ProcessID
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets current site id.
    /// </summary>
    public int SiteID
    {
        get;
        set;
    }

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!StopProcessing)
        {
            SetupControl();
        }
        else
        {
            listElem.StopProcessing = true;
        }
    }


    /// <summary>
    /// Setup control.
    /// </summary>
    private void SetupControl()
    {
        // Show site column only for these two values from selector
        if ((SiteID != UniSelector.US_GLOBAL_AND_SITE_RECORD) && (SiteID != UniSelector.US_ALL_RECORDS))
        {
            // Site column
            listElem.GridColumns.Columns[5].Visible = false;
        }

        listElem.EditActionUrl += "&siteid=" + SiteID;
        if (ProcessID > 0)
        {
            listElem.WhereCondition = "(StateWorkflowID = " + ProcessID + ") ";
        }

        // Prepare site filtering
        string siteCondition = "AND (StateSiteID {0})";

        switch (SiteID)
        {
            case UniSelector.US_GLOBAL_RECORD:
                siteCondition = String.Format(siteCondition, "IS NULL");
                break;

            case UniSelector.US_GLOBAL_AND_SITE_RECORD:
                siteCondition = String.Format(siteCondition, "IS NULL OR StateSiteID = " + CurrentSite.SiteID);
                break;

            case UniSelector.US_ALL_RECORDS:
                siteCondition = String.Empty;
                break;

            default:
                siteCondition = String.Format(siteCondition, "= " + SiteID);
                break;
        }

        listElem.WhereCondition += siteCondition;

        listElem.OnExternalDataBound += listElem_OnExternalDataBound;
        listElem.RememberStateByParam = String.Empty;

        // Register scripts for contact details dialog
        ScriptHelper.RegisterDialogScript(Page);
        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "ViewContactDetails", ScriptHelper.GetScript(
            "function Refresh() {" +
            "__doPostBack('" + this.ClientID + @"', '');" +
            "}"));
    }


    protected object listElem_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        CMSGridActionButton btn;
        switch (sourceName.ToLowerCSafe())
        {
            // Delete action
            case "delete":
                int siteId = SiteID;

                if (SiteID == UniSelector.US_GLOBAL_AND_SITE_RECORD)
                {
                    DataRowView drv = (parameter as GridViewRow).DataItem as DataRowView;
                    int contactSiteId = ValidationHelper.GetInteger(drv["StateSiteID"], 0);
                    if (contactSiteId > 0)
                    {
                        siteId = contactSiteId;
                    }
                }

                btn = (CMSGridActionButton)sender;
                btn.OnClientClick = "if(!confirm(" + ScriptHelper.GetString(string.Format(ResHelper.GetString("autoMenu.RemoveStateConfirmation"), HTMLHelper.HTMLEncode(TypeHelper.GetNiceObjectTypeName(ContactInfo.OBJECT_TYPE).ToLowerCSafe()))) + ")) { return false; }" + btn.OnClientClick;
                if (!WorkflowStepInfoProvider.CanUserRemoveAutomationProcess(CurrentUser, SiteInfoProvider.GetSiteName(siteId)))
                {
                    if (btn != null)
                    {
                        btn.Enabled = false;
                    }
                }
                break;

            case "view":
                btn = (CMSGridActionButton)sender;
                // Ensure accountID parameter value;
                var objectID = ValidationHelper.GetInteger(btn.CommandArgument, 0);
                // Contact detail URL
                string contactURL = UIContextHelper.GetElementDialogUrl(ModuleName.ONLINEMARKETING, "EditContact", objectID, "isSiteManager=" + ContactHelper.IsSiteManager);
                // Add modal dialog script to onClick action
                btn.OnClientClick = ScriptHelper.GetModalDialogScript(contactURL, "ContactDetail");
                break;

            // Process status column
            case "statestatus":
                return AutomationHelper.GetProcessStatus((ProcessStatusEnum)ValidationHelper.GetInteger(parameter, 0));             
        }

        return null;
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Reloads data in listing.
    /// </summary>
    /// <param name="forceReload">Whether to force complete reload</param>
    public override void ReloadData(bool forceReload)
    {
        listElem.ReloadData();
    }

    #endregion
}
