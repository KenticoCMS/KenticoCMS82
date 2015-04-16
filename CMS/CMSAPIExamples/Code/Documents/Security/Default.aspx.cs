using System;
using System.Data;

using CMS.DocumentEngine;
using CMS.Helpers;
using CMS.Base;
using CMS.SiteProvider;
using CMS.Membership;
using CMS.UIControls;
using CMS.DataEngine;

public partial class CMSAPIExamples_Code_Documents_Security_Default : CMSAPIExamplePage
{
    #region "Initialization"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Creating documents
        apiCreateDocumentStructure.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(CreateDocumentStructure);

        // Deleting documents
        apiDeleteDocumentStructure.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(DeleteDocumentStructure);

        // Setting permissions
        apiSetUserPermissions.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(SetUserPermissions);
        apiSetRolePermissions.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(SetRolePermissions);

        // Deleting document level permissions
        apiDeletePermissions.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(DeletePermissions);

        // Permission inheritance
        apiBreakPermissionInheritance.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(BreakPermissionInheritance);
        apiRestorePermissionInheritance.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(RestorePermissionInheritance);

        // Checking permissions
        apiCheckContentModulePermissions.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(CheckContentModulePermissions);
        apiCheckDocTypePermissions.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(CheckDocTypePermissions);
        apiCheckDocumentPermissions.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(CheckDocumentPermissions);
        apiFilterDataSet.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(FilterDataSet);
    }

    #endregion


    #region "Mass actions"

    /// <summary>
    /// Runs all creating and managing examples.
    /// </summary>
    public override void RunAll()
    {
        base.RunAll();

        // Creating documents
        apiCreateDocumentStructure.Run();

        // Setting permissions
        apiSetUserPermissions.Run();
        apiSetRolePermissions.Run();

        // Permission inheritance
        apiBreakPermissionInheritance.Run();
        apiRestorePermissionInheritance.Run();

        // Checking permissions
        apiCheckContentModulePermissions.Run();
        apiCheckDocTypePermissions.Run();
        apiCheckDocumentPermissions.Run();
        apiFilterDataSet.Run();
    }


    /// <summary>
    /// Runs all cleanup examples.
    /// </summary>
    public override void CleanUpAll()
    {
        base.CleanUpAll();

        // Deleting permissions
        apiDeletePermissions.Run();

        // Deleting documents
        apiDeleteDocumentStructure.Run();
    }

    #endregion


    #region "API examples - Documents"

    /// <summary>
    /// Creates the initial document strucutre used for the example. Called when the "Create document structure" button is pressed.
    /// </summary>
    private bool CreateDocumentStructure()
    {
        // Create new instance of the Tree provider
        TreeProvider tree = new TreeProvider(MembershipContext.AuthenticatedUser);

        // Get default culture code
        string culture = SettingsKeyInfoProvider.GetValue(SiteContext.CurrentSiteName + ".CMSDefaultCultureCode");

        // Get parent node
        TreeNode parentNode = tree.SelectSingleNode(SiteContext.CurrentSiteName, "/", culture);

        if (parentNode != null)
        {
            // Create the API Example document
            TreeNode newNode = TreeNode.New("CMS.MenuItem", tree);

            newNode.DocumentName = "API Example";
            newNode.DocumentCulture = culture;

            newNode.Insert(parentNode);

            parentNode = newNode;

            // Create the API Example subpage
            newNode = TreeNode.New("CMS.MenuItem", tree);

            newNode.DocumentName = "API Example subpage";
            newNode.DocumentCulture = culture;

            newNode.Insert(parentNode);

            return true;
        }

        return false;
    }


    /// <summary>
    /// Deletes the example document structure. Called when the "Delete document structure" button is pressed.
    /// Expects the "CreateDocumentStructure" method to be run first.
    /// </summary>
    private bool DeleteDocumentStructure()
    {
        // Create an instance of the Tree provider
        TreeProvider tree = new TreeProvider(MembershipContext.AuthenticatedUser);

        // Get default culture code
        string culture = SettingsKeyInfoProvider.GetValue(SiteContext.CurrentSiteName + ".CMSDefaultCultureCode");


        // Get the API Example document
        TreeNode node = tree.SelectSingleNode(SiteContext.CurrentSiteName, "/API-Example", culture);

        if (node != null)
        {
            // Delete the document and all child documents
            node.DeleteAllCultures();
        }

        return true;
    }

    #endregion


    #region "API examples - Setting document permissions"

    /// <summary>
    /// Expects the "CreateDocumentStructure" method to be run first.
    /// </summary>
    private bool SetUserPermissions()
    {
        // Create an instance of the Tree provider
        TreeProvider tree = new TreeProvider(MembershipContext.AuthenticatedUser);

        // Get default culture code
        string culture = SettingsKeyInfoProvider.GetValue(SiteContext.CurrentSiteName + ".CMSDefaultCultureCode");

        // Get the API Example document
        TreeNode node = tree.SelectSingleNode(SiteContext.CurrentSiteName, "/API-Example", culture);

        if (node != null)
        {
            // Get the user
            UserInfo user = UserInfoProvider.GetUserInfo("Andy");

            if (user != null)
            {
                // Prepare allowed / denied permissions
                int allowed = 0;
                int denied = 0;
                allowed += Convert.ToInt32(Math.Pow(2, Convert.ToInt32(NodePermissionsEnum.ModifyPermissions)));

                // Set user permissions
                AclItemInfoProvider.SetUserPermissions(node, allowed, denied, user);

                return true;
            }
        }

        return false;
    }


    /// <summary>
    /// Expects the "CreateDocumentStructure" method to be run first.
    /// </summary>
    private bool SetRolePermissions()
    {
        // Create an instance of the Tree provider
        TreeProvider tree = new TreeProvider(MembershipContext.AuthenticatedUser);

        // Get default culture code
        string culture = SettingsKeyInfoProvider.GetValue(SiteContext.CurrentSiteName + ".CMSDefaultCultureCode");

        // Get the API Example document
        TreeNode node = tree.SelectSingleNode(SiteContext.CurrentSiteName, "/API-Example", culture);

        if (node != null)
        {
            // Get the role ID
            RoleInfo role = RoleInfoProvider.GetRoleInfo("CMSDeskAdmin", SiteContext.CurrentSiteName);

            if (role != null)
            {
                // Prepare allowed / denied permissions
                int allowed = 0;
                int denied = 0;
                allowed += Convert.ToInt32(Math.Pow(2, Convert.ToInt32(NodePermissionsEnum.Modify)));

                // Set role permissions
                AclItemInfoProvider.SetRolePermissions(node, allowed, denied, role);

                return true;
            }
        }

        return false;
    }


    /// <summary>
    /// Expects the "CreateDocumentStructure" method to be run first.
    /// </summary>
    private bool DeletePermissions()
    {
        // Create an instance of the Tree provider
        TreeProvider tree = new TreeProvider(MembershipContext.AuthenticatedUser);

        // Get default culture code
        string culture = SettingsKeyInfoProvider.GetValue(SiteContext.CurrentSiteName + ".CMSDefaultCultureCode");

        // Get the API Example document
        TreeNode node = tree.SelectSingleNode(SiteContext.CurrentSiteName, "/API-Example", culture);

        if (node != null)
        {
            // Get ID of ACL used on API Example document
            int nodeACLID = ValidationHelper.GetInteger(node.GetValue("NodeACLID"), 0);

            // Delete all ACL items 
            AclItemInfoProvider.DeleteAclItems(nodeACLID);

            return true;
        }

        return false;
    }

    #endregion


    #region "API examples - Permission inheritance"

    /// <summary>
    /// Expects the "CreateDocumentStructure" method to be run first.
    /// </summary>
    private bool BreakPermissionInheritance()
    {
        // Create an instance of the Tree provider
        TreeProvider tree = new TreeProvider(MembershipContext.AuthenticatedUser);

        // Get default culture code
        string culture = SettingsKeyInfoProvider.GetValue(SiteContext.CurrentSiteName + ".CMSDefaultCultureCode");

        // Get the API Example document
        TreeNode node = tree.SelectSingleNode(SiteContext.CurrentSiteName, "/API-Example/API-Example-subpage", culture);

        if (node != null)
        {
            // Break permission inheritance (without copying parent permissions)
            bool copyParentPermissions = false;
            AclInfoProvider.BreakInherintance(node, copyParentPermissions);

            return true;
        }

        return false;
    }


    /// <summary>
    /// Expects the "CreateDocumentStructure" method to be run first.
    /// </summary>
    private bool RestorePermissionInheritance()
    {
        // Create an instance of the Tree provider
        TreeProvider tree = new TreeProvider(MembershipContext.AuthenticatedUser);

        // Get default culture code
        string culture = SettingsKeyInfoProvider.GetValue(SiteContext.CurrentSiteName + ".CMSDefaultCultureCode");

        // Get the API Example document
        TreeNode node = tree.SelectSingleNode(SiteContext.CurrentSiteName, "/API-Example/API-Example-subpage", culture);

        if (node != null)
        {
            // Restore permission inheritance
            AclInfoProvider.RestoreInheritance(node);

            return true;
        }

        return false;
    }

    #endregion


    #region "API examples - Checking permissions"

    /// <summary>
    /// Makes permission check for the specified module
    /// </summary>
    private bool CheckContentModulePermissions()
    {
        // Get the user
        UserInfo user = UserInfoProvider.GetUserInfo("Andy");

        if (user != null)
        {
            // Check permissions and perform an action according to the result
            if (UserInfoProvider.IsAuthorizedPerResource("CMS.Content", "Read", SiteContext.CurrentSiteName, user))
            {
                apiCheckContentModulePermissions.InfoMessage = "User 'Andy' is allowed to read module 'Content'.";
            }
            else
            {
                apiCheckContentModulePermissions.InfoMessage = "User 'Andy' is not allowed to read module 'Content'.";
            }

            return true;
        }

        return false;
    }


    /// <summary>
    /// Makes permission check for the specified document type
    /// </summary>
    private bool CheckDocTypePermissions()
    {
        // Get the user
        UserInfo user = UserInfoProvider.GetUserInfo("Andy");

        if (user != null)
        {
            // Check permissions and perform an action according to the result
            if (UserInfoProvider.IsAuthorizedPerClass("CMS.MenuItem", "Read", SiteContext.CurrentSiteName, user))
            {
                apiCheckDocTypePermissions.InfoMessage = "User 'Andy' is allowed to read page type 'MenuItem'.";
            }
            else
            {
                apiCheckDocTypePermissions.InfoMessage = "User 'Andy' is not allowed to read page type 'MenuItem'.";
            }

            return true;
        }

        return false;
    }


    /// <summary>
    /// Makes permission check for specified node - provides check in its ACLs, document type and Content module
    /// Expects the "CreateDocumentStructure" method to be run first.
    /// </summary>
    private bool CheckDocumentPermissions()
    {
        // Create an instance of the Tree provider
        TreeProvider tree = new TreeProvider(MembershipContext.AuthenticatedUser);

        // Get default culture code
        string culture = SettingsKeyInfoProvider.GetValue(SiteContext.CurrentSiteName + ".CMSDefaultCultureCode");

        // Get the API Example document
        TreeNode node = tree.SelectSingleNode(SiteContext.CurrentSiteName, "/API-Example", culture);

        if (node != null)
        {
            // Get the user
            UserInfo user = UserInfoProvider.GetUserInfo("Andy");

            if (user != null)
            {
                // Check permissions and perform an action according to the result
                if (TreeSecurityProvider.IsAuthorizedPerNode(node, NodePermissionsEnum.ModifyPermissions, user) == AuthorizationResultEnum.Allowed)
                {
                    apiCheckDocumentPermissions.InfoMessage = "User 'Andy' is allowed to modify permissions for the page 'API Example'.";
                }
                else
                {
                    apiCheckDocumentPermissions.InfoMessage = "User 'Andy' is not allowed to modify permissions for the page 'API Example'.";
                }

                return true;
            }
        }

        return false;
    }


    /// <summary>
    /// Changes permission inheritance for documents filtered by permission 'Modify permissions' 
    /// </summary>
    private bool FilterDataSet()
    {
        // Create an instance of the Tree provider
        TreeProvider tree = new TreeProvider(MembershipContext.AuthenticatedUser);

        // Set the parameters for getting documents
        string siteName = SiteContext.CurrentSiteName;
        string aliasPath = "/%";
        string culture = SettingsKeyInfoProvider.GetValue(SiteContext.CurrentSiteName + ".CMSDefaultCultureCode");
        bool combineWithDefaultCulture = true;

        // Get data set with documents
        DataSet documents = tree.SelectNodes(siteName, aliasPath, culture, combineWithDefaultCulture);

        // Get the user
        UserInfo user = UserInfoProvider.GetUserInfo("Andy");

        if (user != null)
        {
            // Filter the data set by the user permissions
            TreeSecurityProvider.FilterDataSetByPermissions(documents, NodePermissionsEnum.ModifyPermissions, user);

            if (!DataHelper.DataSourceIsEmpty(documents))
            {
                // Loop through filtered documents
                foreach (DataRow documentRow in documents.Tables[0].Rows)
                {
                    // Create a new Tree node from the data row
                    TreeNode node = TreeNode.New("CMS.MenuItem", documentRow, tree);

                    // Break permission inheritance (with copying parent permissions)
                    AclInfoProvider.BreakInherintance(node, true);
                }

                // Data set filtered successfully - permission inheritance broken for filtered items
                apiFilterDataSet.InfoMessage = "Data set with all pages filtered successfully by permission 'Modify permissions' for user 'Andy'. Permission inheritance broken for filtered items.";
            }
            else
            {
                // Data set filtered successfully - no items left in data set
                apiFilterDataSet.InfoMessage = "Data set with all pages filtered successfully by permission 'Modify permissions' for user 'Andy'. No items left in data set.";
            }

            return true;
        }

        return false;
    }

    #endregion
}