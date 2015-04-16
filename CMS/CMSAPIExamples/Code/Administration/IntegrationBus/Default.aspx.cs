using System;
using System.Data;
using CMS.Helpers;
using CMS.Synchronization;
using CMS.UIControls;

public partial class CMSAPIExamples_Code_Administration_IntegrationBus_Default : CMSAPIExamplePage
{
    #region "Initialization"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Integration connector
        this.apiCreateIntegrationConnector.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(CreateIntegrationConnector);
        this.apiGetAndUpdateIntegrationConnector.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(GetAndUpdateIntegrationConnector);
        this.apiGetAndBulkUpdateIntegrationConnectors.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(GetAndBulkUpdateIntegrationConnectors);
        this.apiDeleteIntegrationConnector.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(DeleteIntegrationConnector);
    }

    #endregion


    #region "Mass actions"

    /// <summary>
    /// Runs all creating and managing examples.
    /// </summary>
    public override void RunAll()
    {
        base.RunAll();

        // Integration connector
        this.apiCreateIntegrationConnector.Run();
        this.apiGetAndUpdateIntegrationConnector.Run();
        this.apiGetAndBulkUpdateIntegrationConnectors.Run();
    }


    /// <summary>
    /// Runs all cleanup examples.
    /// </summary>
    public override void CleanUpAll()
    {
        base.CleanUpAll();

        // Integration connector
        this.apiDeleteIntegrationConnector.Run();
    }

    #endregion


    #region "API examples - Integration connector"

    /// <summary>
    /// Creates integration connector. Called when the "Create connector" button is pressed.
    /// </summary>
    private bool CreateIntegrationConnector()
    {
        // Create new integration connector object
        IntegrationConnectorInfo newConnector = new IntegrationConnectorInfo();

        // Set the properties
        newConnector.ConnectorDisplayName = "My new connector";
        newConnector.ConnectorName = "MyNewConnector";
        newConnector.ConnectorAssemblyName = "App_Code";
        newConnector.ConnectorClassName = "SampleIntegrationConnector";
        newConnector.ConnectorEnabled = false;

        // Save the integration connector
        IntegrationConnectorInfoProvider.SetIntegrationConnectorInfo(newConnector);

        return true;
    }


    /// <summary>
    /// Gets and updates integration connector. Called when the "Get and update connector" button is pressed.
    /// Expects the CreateIntegrationConnector method to be run first.
    /// </summary>
    private bool GetAndUpdateIntegrationConnector()
    {
        // Get the integration connector
        IntegrationConnectorInfo updateConnector = IntegrationConnectorInfoProvider.GetIntegrationConnectorInfo("MyNewConnector");
        if (updateConnector != null)
        {
            // Update the properties
            updateConnector.ConnectorDisplayName = updateConnector.ConnectorDisplayName.ToLower();

            // Save the changes
            IntegrationConnectorInfoProvider.SetIntegrationConnectorInfo(updateConnector);

            return true;
        }

        return false;
    }


    /// <summary>
    /// Gets and bulk updates integration connectors. Called when the "Get and bulk update connectors" button is pressed.
    /// Expects the CreateIntegrationConnector method to be run first.
    /// </summary>
    private bool GetAndBulkUpdateIntegrationConnectors()
    {
        // Prepare the parameters
        string where = "ConnectorName LIKE N'MyNewConnector%'";

        // Get the data
        DataSet connectors = IntegrationConnectorInfoProvider.GetIntegrationConnectors(where, null);
        if (!DataHelper.DataSourceIsEmpty(connectors))
        {
            // Loop through the individual items
            foreach (DataRow connectorDr in connectors.Tables[0].Rows)
            {
                // Create object from DataRow
                IntegrationConnectorInfo modifyConnector = new IntegrationConnectorInfo(connectorDr);

                // Update the properties
                modifyConnector.ConnectorDisplayName = modifyConnector.ConnectorDisplayName.ToUpper();

                // Save the changes
                IntegrationConnectorInfoProvider.SetIntegrationConnectorInfo(modifyConnector);
            }

            return true;
        }

        return false;
    }


    /// <summary>
    /// Deletes integration connector. Called when the "Delete connector" button is pressed.
    /// Expects the CreateIntegrationConnector method to be run first.
    /// </summary>
    private bool DeleteIntegrationConnector()
    {
        // Get the integration connector
        IntegrationConnectorInfo deleteConnector = IntegrationConnectorInfoProvider.GetIntegrationConnectorInfo("MyNewConnector");

        // Delete the integration connector
        IntegrationConnectorInfoProvider.DeleteIntegrationConnectorInfo(deleteConnector);

        return (deleteConnector != null);
    }

    #endregion
}
