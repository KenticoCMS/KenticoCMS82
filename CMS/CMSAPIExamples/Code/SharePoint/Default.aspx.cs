using System;
using System.Data;
using System.Linq;

using CMS.Base;
using CMS.IO;
using CMS.SharePoint;
using CMS.SiteProvider;
using CMS.UIControls;


public partial class CMSAPIExamples_Code_SharePoint_Default : CMSAPIExamplePage
{
    #region "Initialization"

    protected void Page_Load(object sender, EventArgs e)
    {
        // SharePoint connection
        apiCreateSharePointConnection.RunExampleSimple += CreateSharePointConnection;
        apiGetAndUpdateSharePointConnection.RunExampleSimple += GetAndUpdateSharePointConnection;
        apiDeleteSharePointConnection.RunExampleSimple += DeleteSharePointConnection;
        
        // SharePoint services
        apiGetAllLists.RunExampleSimple += GetAllLists;
        apiGetListItems.RunExampleSimple += GetListItems;
        apiGetFile.RunExampleSimple += GetFile;
    }

    #endregion


    #region "Mass actions"

    /// <summary>
    /// Runs all creating and managing examples.
    /// </summary>
    public override void RunAll()
    {
        base.RunAll();

        // SharePoint connection
        apiCreateSharePointConnection.Run();
        apiGetAndUpdateSharePointConnection.Run();
        
        // SharePoint services
        apiGetAllLists.Run();
        apiGetListItems.Run();
        apiGetFile.Run();
    }


    /// <summary>
    /// Runs all cleanup examples.
    /// </summary>
    public override void CleanUpAll()
    {
        base.CleanUpAll();

        // SharePoint connection
        apiDeleteSharePointConnection.Run();
    }

    #endregion


    #region "API examples - SharePoint connection"

    /// <summary>
    /// Creates SharePoint connection. Called when the "Create SharePoint connection" button is clicked.
    /// </summary>
    private void CreateSharePointConnection()
    {
        // Verify that SharePoint site URL has been provided.
        if (String.IsNullOrWhiteSpace(txtSiteUrl.Text))
        {
            throw new CMSAPIExampleException("Empty value for Site URL is not allowed.");
        }

        // Create new SharePoint connection object
        SharePointConnectionInfo newConnection = new SharePointConnectionInfo();

        // Set the properties
        newConnection.SharePointConnectionSiteUrl = txtSiteUrl.Text;
        newConnection.SharePointConnectionSiteID = SiteContext.CurrentSiteID;
        newConnection.SharePointConnectionDisplayName = "My new connection";
        newConnection.SharePointConnectionName = "MyNewConnection";
        newConnection.SharePointConnectionSharePointVersion = spServerVersion.Value.ToString();

        if (String.IsNullOrEmpty(txtUserName.Text))
        {
            // Credentials have not been provided, anonymous authentication mode is used
            newConnection.SharePointConnectionAuthMode = SharePointAuthMode.ANONYMOUS;
        }
        else
        {
            // Credentials have been provided, default authentication mode is used
            newConnection.SharePointConnectionAuthMode = SharePointAuthMode.DEFAULT;
            newConnection.SharePointConnectionUserName = txtUserName.Text;
            newConnection.SharePointConnectionDomain = txtDomain.Text;
            newConnection.SharePointConnectionPassword = txtPassword.Text;
        }

        // Save the SharePoint connection into DB
        SharePointConnectionInfoProvider.SetSharePointConnectionInfo(newConnection);
    }


    /// <summary>
    /// Gets and updates SharePoint connection. Called when the "Get and update SharePoint connection" button is clicked.
    /// Expects the CreateSharePointConnection method to be run first.
    /// </summary>
    private void GetAndUpdateSharePointConnection()
    {
        // Get the SharePoint connection from DB
        SharePointConnectionInfo connection = SharePointConnectionInfoProvider.GetSharePointConnectionInfo("MyNewConnection", SiteContext.CurrentSiteID);
        if (connection == null)
        {
            throw new CMSAPIExampleException("SharePoint connection 'My new connection' was not found.");
        }

        // Update the properties
        connection.SharePointConnectionDisplayName = connection.SharePointConnectionDisplayName.ToLowerCSafe();

        // Save the changes into DB
        SharePointConnectionInfoProvider.SetSharePointConnectionInfo(connection);
    }


    /// <summary>
    /// Deletes SharePoint connection. Called when the "Delete SharePoint connection" button is clicked.
    /// Expects the CreateSharePointConnection method to be run first.
    /// </summary>
    private void DeleteSharePointConnection()
    {
        // Get the SharePoint connection from DB
        SharePointConnectionInfo connection = SharePointConnectionInfoProvider.GetSharePointConnectionInfo("MyNewConnection", SiteContext.CurrentSiteID);
        if (connection == null)
        {
            throw new CMSAPIExampleException("SharePoint connection 'My new connection' was not found.");
        }

        // Delete the SharePoint connection from DB
        SharePointConnectionInfoProvider.DeleteSharePointConnectionInfo(connection);
    }

    #endregion


    #region "API Examples - SharePoint services"

    /// <summary>
    /// Gets metadata about all lists stored on the SharePoint server using "My new connection". Called when the "Get all lists" button is clicked.
    /// Expects the CreateSharePointConnection method to be run first.
    /// </summary>
    private void GetAllLists()
    {
        // Get the SharePoint connection from DB
        SharePointConnectionInfo connection = SharePointConnectionInfoProvider.GetSharePointConnectionInfo("MyNewConnection", SiteContext.CurrentSiteID);
        if (connection == null)
        {
            throw new CMSAPIExampleException("SharePoint connection 'My new connection' was not found.");
        }

        // Convert SharePointConnectionInfo object into connection data
        SharePointConnectionData connectionData = connection.ToSharePointConnectionData();

        // Get list service implementation
        ISharePointListService listService = SharePointServices.GetService<ISharePointListService>(connectionData);

        // Choose SharePoint list type that will be retrieved. 
        // You can use enum or template identifier (listed in http://msdn.microsoft.com/en-us/library/microsoft.sharepoint.splisttemplatetype.aspx)
        int listType = SharePointListType.ALL;

        try
        {
            // Get all lists of specified type (all list types are retrieved in this case)
            DataSet results = listService.GetLists(listType);

            if ((results.Tables.Count == 0) || (results.Tables[0].Rows.Count == 0))
            {
                throw new CMSAPIExampleException("No lists were retrieved from SharePoint server.");
            }
        }
        catch (Exception ex)
        {
            throw new CMSAPIExampleException(ex.Message);
        }
    }


    /// <summary>
    /// Gets all items of specified SharePoint list using "My new connection". Called when the "Get list items" button is clicked.
    /// Expects the CreateSharePointConnection method to be run first.
    /// </summary>
    private void GetListItems()
    {
        // Verify the list name has been provided.
        string listName = txtListName.Text;
        if (String.IsNullOrWhiteSpace(listName))
        {
            throw new CMSAPIExampleException("Empty value for List name is not allowed.");
        }

        // Get the SharePoint connection from DB
        SharePointConnectionInfo connection = SharePointConnectionInfoProvider.GetSharePointConnectionInfo("MyNewConnection", SiteContext.CurrentSiteID);
        if (connection == null)
        {
            throw new CMSAPIExampleException("SharePoint connection 'My new connection' was not found.");
        }

        // Convert SharePointConnectionInfo object into connection data
        SharePointConnectionData connectionData = connection.ToSharePointConnectionData();

        // Get list service implementation
        ISharePointListService listService = SharePointServices.GetService<ISharePointListService>(connectionData);

        try
        {
            // Get specified list's items
            DataSet results = listService.GetListItems(listName);

            if ((results.Tables.Count == 0) || (results.Tables[0].Rows.Count == 0))
            {
                throw new CMSAPIExampleException("No list's items were retrieved from SharePoint server.");
            }
        }
        catch (Exception ex)
        {
            throw new CMSAPIExampleException(ex.Message);
        }
    }


    /// <summary>
    /// Gets specified file from SharePoint server using "My new connection". Called when the "Get file" button is clicked.
    /// Expects the CreateSharePointConnection method to be run first.
    /// </summary>
    private void GetFile()
    {
        // Verify the file path has been provided.
        string filePath = txtFilePath.Text;
        if (String.IsNullOrWhiteSpace(filePath))
        {
            throw new CMSAPIExampleException("Empty value for File type is not allowed.");
        }

        // Get the SharePoint connection from DB
        SharePointConnectionInfo connection = SharePointConnectionInfoProvider.GetSharePointConnectionInfo("MyNewConnection", SiteContext.CurrentSiteID);
        if (connection == null)
        {
            throw new CMSAPIExampleException("SharePoint connection 'My new connection' was not found.");
        }

        // Convert SharePointConnectionInfo object into connection data
        SharePointConnectionData connectionData = connection.ToSharePointConnectionData();

        // Get file service implementation
        ISharePointFileService fileService = SharePointServices.GetService<ISharePointFileService>(connectionData);

        try
        {
            // Get file object
            ISharePointFile file = fileService.GetFile(filePath);

            // Get file metadata
            string extension = file.Extension;

            // Get stream of file's binary content
            Stream fileContentStream = file.GetContentStream();

            // Get byte array of file's binary content
            byte[] fileContentBytes = file.GetContentBytes();
        }
        catch (Exception ex)
        {
            throw new CMSAPIExampleException(ex.Message);
        }
    }

    #endregion

}
