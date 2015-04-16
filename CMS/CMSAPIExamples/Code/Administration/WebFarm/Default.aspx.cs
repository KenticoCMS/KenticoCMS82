using System;
using System.Data;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.LicenseProvider;
using CMS.Base;
using CMS.UIControls;
using CMS.WebFarmSync;

public partial class CMSAPIExamples_Code_Administration_WebFarm_Default : CMSAPIExamplePage
{
    #region "Initialization"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Check license
        LicenseHelper.CheckFeatureAndRedirect(RequestContext.CurrentDomain, FeatureEnum.Webfarm);

        // Web farm server
        apiCreateWebFarmServer.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(CreateWebFarmServer);
        apiGetAndUpdateWebFarmServer.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(GetAndUpdateWebFarmServer);
        apiGetAndBulkUpdateWebFarmServers.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(GetAndBulkUpdateWebFarmServers);
        apiDeleteWebFarmServer.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(DeleteWebFarmServer);

        // Web farm task
        apiCreateTask.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(CreateTask);
        apiRunMyTasks.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(RunMyTasks);
    }

    #endregion


    #region "Mass actions"

    /// <summary>
    /// Runs all creating and managing examples.
    /// </summary>
    public override void RunAll()
    {
        base.RunAll();

        // Web farm server
        apiCreateWebFarmServer.Run();
        apiGetAndUpdateWebFarmServer.Run();
        apiGetAndBulkUpdateWebFarmServers.Run();

        // Web farm tasks
        apiCreateTask.Run();
        apiRunMyTasks.Run();
    }


    /// <summary>
    /// Runs all cleanup examples.
    /// </summary>
    public override void CleanUpAll()
    {
        base.CleanUpAll();

        // Web farm server
        apiDeleteWebFarmServer.Run();
    }

    #endregion


    #region "API examples - Web farm server"

    /// <summary>
    /// Creates web farm server. Called when the "Create server" button is pressed.
    /// </summary>
    private bool CreateWebFarmServer()
    {
        // Create new web farm server object
        WebFarmServerInfo newServer = new WebFarmServerInfo();

        // Set the properties
        newServer.ServerDisplayName = "My new server";
        newServer.ServerName = "MyNewServer";
        newServer.ServerEnabled = true;
        newServer.ServerURL = "http://localhost/KenticoCMS";

        // Save the web farm server
        WebFarmServerInfoProvider.SetWebFarmServerInfo(newServer);

        return true;
    }


    /// <summary>
    /// Gets and updates web farm server. Called when the "Get and update server" button is pressed.
    /// Expects the CreateWebFarmServer method to be run first.
    /// </summary>
    private bool GetAndUpdateWebFarmServer()
    {
        // Get the web farm server
        WebFarmServerInfo updateServer = WebFarmServerInfoProvider.GetWebFarmServerInfo("MyNewServer");
        if (updateServer != null)
        {
            // Update the properties
            updateServer.ServerDisplayName = updateServer.ServerDisplayName.ToLowerCSafe();

            // Save the changes
            WebFarmServerInfoProvider.SetWebFarmServerInfo(updateServer);

            return true;
        }

        return false;
    }


    /// <summary>
    /// Gets and bulk updates web farm servers. Called when the "Get and bulk update servers" button is pressed.
    /// Expects the CreateWebFarmServer method to be run first.
    /// </summary>
    private bool GetAndBulkUpdateWebFarmServers()
    {
        // Get the data
        DataSet servers = WebFarmServerInfoProvider.GetAllEnabledServers();
        if (!DataHelper.DataSourceIsEmpty(servers))
        {
            // Loop through the individual items
            foreach (DataRow serverDr in servers.Tables[0].Rows)
            {
                // Create object from DataRow
                WebFarmServerInfo modifyServer = new WebFarmServerInfo(serverDr);

                // Update the properties
                modifyServer.ServerDisplayName = modifyServer.ServerDisplayName.ToUpper();

                // Save the changes
                WebFarmServerInfoProvider.SetWebFarmServerInfo(modifyServer);
            }

            return true;
        }

        return false;
    }


    /// <summary>
    /// Deletes web farm server. Called when the "Delete server" button is pressed.
    /// Expects the CreateWebFarmServer method to be run first.
    /// </summary>
    private bool DeleteWebFarmServer()
    {
        // Get the web farm server
        WebFarmServerInfo deleteServer = WebFarmServerInfoProvider.GetWebFarmServerInfo("MyNewServer");

        // Delete the web farm server
        WebFarmServerInfoProvider.DeleteWebFarmServerInfo(deleteServer);

        return (deleteServer != null);
    }

    #endregion


    #region "API examples - Web farm tasks"

    /// <summary>
    /// Creates web farm server. Called when the "Create server" button is pressed.
    /// </summary>
    private bool CreateTask()
    {
        // Set the properties
        string taskTarget = "";
        string taskTextData = "MyWebFarmTask";
        byte[] taskBinaryData = null;
         
        // Create the web farm task
        WebFarmHelper.CreateTask(DataTaskType.ClearHashtables, taskTarget, taskTextData, taskBinaryData);

        return true;
    }


    /// <summary>
    /// Runs all my web farm tasks. Called when the "Run my tasks" button is pressed.
    /// </summary>
    private bool RunMyTasks()
    {
        WebSyncHelper.ProcessMyTasks();

        return true;
    }

    #endregion
}