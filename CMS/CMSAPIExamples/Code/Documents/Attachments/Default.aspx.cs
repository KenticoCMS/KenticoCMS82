using System;
using System.Data;

using CMS.DocumentEngine;
using CMS.Helpers;
using CMS.Membership;
using CMS.SiteProvider;
using CMS.UIControls;

public partial class CMSAPIExamples_Code_Documents_Attachments_Default : CMSAPIExamplePage
{
    #region "Initialization"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Preparation
        apiCreateExampleDocument.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(CreateExampleDocument);

        // Inserting attachments
        apiInsertUnsortedAttachment.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(InsertUnsortedAttachment);
        apiInsertFieldAttachment.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(InsertFieldAttachment);

        // Managing attachments
        apiMoveAttachmentDown.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(MoveAttachmentDown);
        apiMoveAttachmentUp.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(MoveAttachmentUp);
        apiEditMetadata.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(EditMetadata);

        // Cleanup
        apiDeleteAttachments.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(DeleteAttachments);
        apiDeleteExampleDocument.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(DeleteExampleDocument);
    }

    #endregion


    #region "Mass actions"

    /// <summary>
    /// Runs all creating and managing examples.
    /// </summary>
    public override void RunAll()
    {
        base.RunAll();

        // Inserting attachments
        apiCreateExampleDocument.Run();
        apiInsertUnsortedAttachment.Run();
        apiInsertFieldAttachment.Run();

        // Managing attachments
        apiMoveAttachmentDown.Run();
        apiMoveAttachmentUp.Run();
        apiEditMetadata.Run();
    }


    /// <summary>
    /// Runs all cleanup examples.
    /// </summary>
    public override void CleanUpAll()
    {
        base.CleanUpAll();

        // Delete attachments
        apiDeleteAttachments.Run();

        // Delete example document
        apiDeleteExampleDocument.Run();
    }

    #endregion


    #region "API examples - Preparation"

    private bool CreateExampleDocument()
    {
        // Create a new instance of the Tree provider
        TreeProvider tree = new TreeProvider(MembershipContext.AuthenticatedUser);

        // Get site root node
        TreeNode parentNode = tree.SelectSingleNode(SiteContext.CurrentSiteName, "/", "en-us");

        if (parentNode != null)
        {
            // Create the document
            TreeNode node = TreeNode.New("CMS.MenuItem", tree);

            node.DocumentName = "API Example";
            node.DocumentCulture = "en-us";

            node.Insert(parentNode);

            return true;
        }

        return false;
    }

    #endregion


    #region "API examples - Inserting attachments"

    /// <summary>
    /// Inserts an unsorted attachment to the example document. Called when the "Insert unsorted attachment" button is pressed.
    /// Expects the "Create example document" method to be run first.
    /// </summary>
    private bool InsertUnsortedAttachment()
    {
        // Create a new instance of the Tree provider
        TreeProvider tree = new TreeProvider(MembershipContext.AuthenticatedUser);

        // Get the document
        TreeNode node = tree.SelectSingleNode(SiteContext.CurrentSiteName, "/API-Example", "en-us");

        // Path to the file to be inserted. This example uses an explicitly defined file path. However, you can use an object of the HttpPostedFile type (uploaded via an upload control).
        string postedFile = Server.MapPath("Files/file.png");

        if (node != null)
        {
            // Insert the attachment
            return (DocumentHelper.AddUnsortedAttachment(node, Guid.NewGuid(), postedFile, tree, ImageHelper.AUTOSIZE, ImageHelper.AUTOSIZE, ImageHelper.AUTOSIZE) != null);
        }

        return false;
    }


    /// <summary>
    /// Inserts an attachment into the Teaser image field. Called when the "Insert field attachment" button is pressed.
    /// Expects the "Create example document" method to be run first.
    /// </summary>
    private bool InsertFieldAttachment()
    {
        // Create a new instance of the Tree provider
        TreeProvider tree = new TreeProvider(MembershipContext.AuthenticatedUser);

        // Get the example document
        TreeNode node = tree.SelectSingleNode(SiteContext.CurrentSiteName, "/API-Example", "en-us");

        if (node != null)
        {
            AttachmentInfo newAttachment = null;

            // Path to the file to be inserted. This example uses an explicitly defined file path. However, you can use an object of the HttpPostedFile type (uploaded via an upload control).
            string postedFile = Server.MapPath("Files/file.png");

            // Insert the attachment and update the document with its GUID
            newAttachment = DocumentHelper.AddAttachment(node, "MenuItemTeaserImage", postedFile, tree);
            node.Update();

            if (newAttachment != null)
            {
                return true;
            }

            apiInsertFieldAttachment.ErrorMessage = "Couldn't insert the attachment.";
        }

        return false;
    }

    #endregion


    #region "API examples - Managing attachments"

    /// <summary>
    /// Moves an unsorted attachment down in the list. Called when the "Move attachment down" button is pressed.
    /// Expects the "Create example document" and "Insert unsorted attachment" methods to be run first.
    /// </summary>
    private bool MoveAttachmentDown()
    {
        // Create a new instance of the Tree provider
        TreeProvider tree = new TreeProvider(MembershipContext.AuthenticatedUser);

        // Get the example document
        TreeNode node = tree.SelectSingleNode(SiteContext.CurrentSiteName, "/API-Example", "en-us");

        if (node != null)
        {
            string where = "AttachmentIsUnsorted = 1";
            string orderBy = "AttachmentLastModified DESC";

            // Get the document's unsorted attachments with the latest on top
            DataSet attachments = DocumentHelper.GetAttachments(node, where, orderBy, false, tree);

            if (!DataHelper.DataSourceIsEmpty(attachments))
            {
                // Create attachment info object from the first DataRow
                AttachmentInfo attachment = new AttachmentInfo(attachments.Tables[0].Rows[0]);

                // Move the attachment
                DocumentHelper.MoveAttachmentDown(attachment.AttachmentGUID, node);

                return true;
            }
            else
            {
                apiMoveAttachmentDown.ErrorMessage = "No attachments were found.";
            }
        }

        return false;
    }


    /// <summary>
    /// Moves an unsorted attachment up in the list. Called when the "Move attachment up" button is pressed.
    /// Expects the "Create example document" and "Insert unsorted attachment" methods to be run first.
    /// </summary>
    private bool MoveAttachmentUp()
    {
        // Create a new instance of the Tree provider
        TreeProvider tree = new TreeProvider(MembershipContext.AuthenticatedUser);

        // Get the example document
        TreeNode node = tree.SelectSingleNode(SiteContext.CurrentSiteName, "/API-Example", "en-us");

        if (node != null)
        {
            string where = "AttachmentIsUnsorted = 1";
            string orderBy = "AttachmentLastModified DESC";

            // Get the document's unsorted attachments with the latest on top
            DataSet attachments = DocumentHelper.GetAttachments(node, where, orderBy, false, tree);

            if (!DataHelper.DataSourceIsEmpty(attachments))
            {
                // Create attachment info object from the first DataRow
                AttachmentInfo attachment = new AttachmentInfo(attachments.Tables[0].Rows[0]);

                // Move the attachment
                DocumentHelper.MoveAttachmentUp(attachment.AttachmentGUID, node);

                return true;
            }
            else
            {
                apiMoveAttachmentDown.ErrorMessage = "No attachments were found.";
            }
        }

        return false;
    }


    /// <summary>
    /// Gets an attachment and modifies its metadata(name, title and description). Called when the "Edit attachment metadata" button is pressed.
    /// Expects the "Create example document" and "Insert unsorted attachment" methods to be run first.
    /// </summary>
    private bool EditMetadata()
    {
        // Create a new instance of the Tree provider
        TreeProvider tree = new TreeProvider(MembershipContext.AuthenticatedUser);

        // Get the example document
        TreeNode node = tree.SelectSingleNode(SiteContext.CurrentSiteName, "/API-Example", "en-us");

        if (node != null)
        {
            string where = "AttachmentIsUnsorted = 1";
            string orderBy = "AttachmentLastModified DESC";

            // Get the document's unsorted attachments with the latest on top
            DataSet attachments = DocumentHelper.GetAttachments(node, where, orderBy, false, tree);

            if (!DataHelper.DataSourceIsEmpty(attachments))
            {
                // Create attachment info object from the first DataRow
                AttachmentInfo attachment = new AttachmentInfo(attachments.Tables[0].Rows[0]);

                // Edit its metadata
                attachment.AttachmentName += " - modified";
                attachment.AttachmentTitle += "Example title";
                attachment.AttachmentDescription += "This is an example of an unsorted attachment.";

                // Ensure that the attachment can be updated without supplying its binary data.
                attachment.AllowPartialUpdate = true;

                // Save the object into database
                AttachmentInfoProvider.SetAttachmentInfo(attachment);

                return true;
            }
            else
            {
                apiEditMetadata.ErrorMessage = "No attachments were found.";
            }
        }

        return false;
    }

    #endregion


    #region "API examples - Cleanup"

    /// <summary>
    /// Deletes all the example document's attachments. Called when the "Delete attachments" button is pressed.
    /// Expects the "CreateExampleDocument" and "InsertUnsortedAttachment" or "InsertFieldAttachment" method to be run first.
    /// </summary>
    private bool DeleteAttachments()
    {
        // Create a new instance of the Tree provider
        TreeProvider tree = new TreeProvider(MembershipContext.AuthenticatedUser);

        // Get the example document
        TreeNode node = tree.SelectSingleNode(SiteContext.CurrentSiteName, "/API-Example", "en-us");

        if (node != null)
        {
            // Get the document's unsorted attachments with the latest on top
            DataSet attachments = DocumentHelper.GetAttachments(node, null, null, false, tree);

            if (!DataHelper.DataSourceIsEmpty(attachments))
            {
                foreach (DataRow attachmentRow in attachments.Tables[0].Rows)
                {
                    // Create attachment info object from the first DataRow
                    AttachmentInfo attachment = new AttachmentInfo(attachmentRow);

                    // Delete the attachment
                    DocumentHelper.DeleteAttachment(node, attachment.AttachmentGUID, tree);
                }

                return true;
            }
            else
            {
                apiDeleteAttachments.ErrorMessage = "No attachments found.";
            }
        }

        return false;
    }


    /// <summary>
    /// Deletes the example document. Called when the "Delete example document" button is pressed.
    /// Expects the "CreateExampleDocument" method to be run first.
    /// </summary>
    private bool DeleteExampleDocument()
    {
        // Create a new instance of the Tree provider
        TreeProvider tree = new TreeProvider(MembershipContext.AuthenticatedUser);

        // Get the example document
        TreeNode node = tree.SelectSingleNode(SiteContext.CurrentSiteName, "/API-Example", "en-us");

        if (node != null)
        {
            // Delete the document
            node.Delete();

            return true;
        }

        return false;
    }

    #endregion
}