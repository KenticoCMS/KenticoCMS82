using System;
using System.Data;

using CMS.Helpers;
using CMS.PortalEngine;
using CMS.SiteProvider;
using CMS.UIControls;

public partial class CMSAPIExamples_Code_Development_InlineControls_Default : CMSAPIExamplePage
{
    #region "Initialization"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Inline control
        apiCreateInlineControl.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(CreateInlineControl);
        apiGetAndUpdateInlineControl.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(GetAndUpdateInlineControl);
        apiGetAndBulkUpdateInlineControls.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(GetAndBulkUpdateInlineControls);
        apiDeleteInlineControl.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(DeleteInlineControl);

        // Inline control on site
        apiAddInlineControlToSite.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(AddInlineControlToSite);
        apiRemoveInlineControlFromSite.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(RemoveInlineControlFromSite);
    }

    #endregion


    #region "Mass actions"

    /// <summary>
    /// Runs all creating and managing examples.
    /// </summary>
    public override void RunAll()
    {
        base.RunAll();

        // Inline control
        apiCreateInlineControl.Run();
        apiGetAndUpdateInlineControl.Run();
        apiGetAndBulkUpdateInlineControls.Run();

        // Inline control on site
        apiAddInlineControlToSite.Run();
    }


    /// <summary>
    /// Runs all cleanup examples.
    /// </summary>
    public override void CleanUpAll()
    {
        base.CleanUpAll();

        // Inline control on site
        apiRemoveInlineControlFromSite.Run();

        // Inline control
        apiDeleteInlineControl.Run();
    }

    #endregion


    #region "API examples - Inline control"

    /// <summary>
    /// Creates inline control. Called when the "Create control" button is pressed.
    /// </summary>
    private bool CreateInlineControl()
    {
        // Create new inline control object
        InlineControlInfo newControl = new InlineControlInfo();

        // Set the properties
        newControl.ControlDisplayName = "My new control";
        newControl.ControlName = "MyNewControl";
        newControl.ControlFileName = "~/CMSModules/Polls/InlineControls/PollControl.ascx";
        newControl.ControlParameterName = "Poll name";
        newControl.ControlDescription = "My new inline control description";

        // Save the inline control
        InlineControlInfoProvider.SetInlineControlInfo(newControl);

        return true;
    }


    /// <summary>
    /// Gets and updates inline control. Called when the "Get and update control" button is pressed.
    /// Expects the CreateInlineControl method to be run first.
    /// </summary>
    private bool GetAndUpdateInlineControl()
    {
        // Get the inline control
        InlineControlInfo updateControl = InlineControlInfoProvider.GetInlineControlInfo("MyNewControl");
        if (updateControl != null)
        {
            // Update the properties
            updateControl.ControlDisplayName = updateControl.ControlDisplayName.ToLower();

            // Save the changes
            InlineControlInfoProvider.SetInlineControlInfo(updateControl);

            return true;
        }

        return false;
    }


    /// <summary>
    /// Gets and bulk updates inline controls. Called when the "Get and bulk update controls" button is pressed.
    /// Expects the CreateInlineControl method to be run first.
    /// </summary>
    private bool GetAndBulkUpdateInlineControls()
    {
        // Prepare the parameters
        string where = "ControlName LIKE N'MyNewControl%'";

        // Get the data
        DataSet controls = InlineControlInfoProvider.GetInlineControls().Where(where);
        if (!DataHelper.DataSourceIsEmpty(controls))
        {
            // Loop through the individual items
            foreach (DataRow controlDr in controls.Tables[0].Rows)
            {
                // Create object from DataRow
                InlineControlInfo modifyControl = new InlineControlInfo(controlDr);

                // Update the properties
                modifyControl.ControlDisplayName = modifyControl.ControlDisplayName.ToUpper();

                // Save the changes
                InlineControlInfoProvider.SetInlineControlInfo(modifyControl);
            }

            return true;
        }

        return false;
    }


    /// <summary>
    /// Deletes inline control. Called when the "Delete control" button is pressed.
    /// Expects the CreateInlineControl method to be run first.
    /// </summary>
    private bool DeleteInlineControl()
    {
        // Get the inline control
        InlineControlInfo deleteControl = InlineControlInfoProvider.GetInlineControlInfo("MyNewControl");

        // Delete the inline control
        InlineControlInfoProvider.DeleteInlineControlInfo(deleteControl);

        return (deleteControl != null);
    }

    #endregion


    #region "API examples - Inline control on site"

    /// <summary>
    /// Adds inline control to current site. Called when the "Add control to site" button is pressed.
    /// Expects the CreateInlineControl method to be run first.
    /// </summary>
    private bool AddInlineControlToSite()
    {
        // Get the inline control
        InlineControlInfo control = InlineControlInfoProvider.GetInlineControlInfo("MyNewControl");
        if (control != null)
        {
            int controlId = control.ControlID;
            int siteId = SiteContext.CurrentSiteID;

            // Save the binding
            InlineControlSiteInfoProvider.AddInlineControlToSite(controlId, siteId);

            return true;
        }

        return false;
    }


    /// <summary>
    /// Removes inline control from site. Called when the "Remove control from site" button is pressed.
    /// Expects the AddInlineControlToSite method to be run first.
    /// </summary>
    private bool RemoveInlineControlFromSite()
    {
        // Get the inline control
        InlineControlInfo removeControl = InlineControlInfoProvider.GetInlineControlInfo("MyNewControl");
        if (removeControl != null)
        {
            int siteId = SiteContext.CurrentSiteID;

            // Get the binding
            InlineControlSiteInfo inlineControlSite = InlineControlSiteInfoProvider.GetInlineControlSiteInfo(removeControl.ControlID, siteId);

            // Delete the binding
            InlineControlSiteInfoProvider.DeleteInlineControlSiteInfo(inlineControlSite);

            return true;
        }

        return false;
    }

    #endregion
}