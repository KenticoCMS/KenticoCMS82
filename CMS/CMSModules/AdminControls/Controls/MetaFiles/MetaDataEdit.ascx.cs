using System;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.EventLog;
using CMS.Helpers;
using CMS.IO;
using CMS.DataEngine;
using CMS.Search;
using CMS.SiteProvider;
using CMS.Membership;
using CMS.DocumentEngine;
using CMS.UIControls;
using CMS.WorkflowEngine;
using CMS.ExtendedControls;

using TreeNode = CMS.DocumentEngine.TreeNode;

public partial class CMSModules_AdminControls_Controls_MetaFiles_MetaDataEdit : CMSUserControl
{
    #region "Variables"

    private string mObjectType;
    private bool mRenderTableTag = true;
    private bool mCheckPermissions = true;
    private TreeProvider mTreeProvider;
    private VersionManager mVersionManager;
    private TreeNode mNode;
    private bool nodeIsParent;
    private string mSiteName;
    private bool mEnabled = true;
    private bool mRenderAsForm = true;

    #endregion


    #region "Properties"

    /// <summary>
    /// Placeholder for messages
    /// </summary>
    public override MessagesPlaceHolder MessagesPlaceHolder
    {
        get
        {
            return plcMess;
        }
    }


    /// <summary>
    /// Object type.
    /// </summary>
    public string ObjectType
    {
        get
        {
            // Get object type from info object
            if ((mObjectType == null) && (InfoObject != null))
            {
                mObjectType = InfoObject.TypeInfo.ObjectType;
            }

            return mObjectType;
        }
        set
        {
            mObjectType = value;
        }
    }


    /// <summary>
    /// Object GUID.
    /// </summary>
    public Guid ObjectGuid
    {
        get;
        set;
    }


    /// <summary>
    /// Object title.
    /// </summary>
    public string ObjectTitle
    {
        get
        {
            return txtTitle.Text;
        }
        set
        {
            txtTitle.Text = value;
        }
    }


    /// <summary>
    /// Object description.
    /// </summary>
    public string ObjectDescription
    {
        get
        {
            return txtDescription.Text;
        }
        set
        {
            txtDescription.Text = value;
        }
    }


    /// <summary>
    /// Object filename (without extension).
    /// </summary>
    public string ObjectFileName
    {
        get
        {
            return txtFileName.Text;
        }
        set
        {
            txtFileName.Text = value;
        }
    }


    /// <summary>
    /// Object file extension.
    /// </summary>
    public string ObjectExtension
    {
        get
        {
            return lblExtension.Text;
        }
        set
        {
            lblExtension.Text = value;
        }
    }


    /// <summary>
    /// Object file size.
    /// </summary>
    public string ObjectSize
    {
        get
        {
            return lblSize.Text;
        }
        set
        {
            lblSize.Text = value;
        }
    }


    /// <summary>
    /// Indicates if show only title and description.
    /// </summary>
    public bool ShowOnlyTitleAndDescription
    {
        get;
        set;
    }


    /// <summary>
    /// Indicates, if wrapping table should be rendered.
    /// </summary>
    public bool RenderTableTag
    {
        get
        {
            return mRenderTableTag;
        }
        set
        {
            mRenderTableTag = value;
        }
    }


    /// <summary>
    /// Tree provider.
    /// </summary>
    public TreeProvider TreeProvider
    {
        get
        {
            return mTreeProvider ?? (mTreeProvider = new TreeProvider(MembershipContext.AuthenticatedUser));
        }
    }


    /// <summary>
    /// Version manager.
    /// </summary>
    public VersionManager VersionManager
    {
        get
        {
            return mVersionManager ?? (mVersionManager = VersionManager.GetInstance(TreeProvider));
        }
    }


    /// <summary>
    /// Version history ID.
    /// </summary>
    public int VersionHistoryID
    {
        get;
        set;
    }


    /// <summary>
    /// External control ID.
    /// </summary>
    public string ExternalControlID
    {
        get;
        set;
    }


    /// <summary>
    /// Site name. If is null, return current site name.
    /// </summary>
    public string SiteName
    {
        get
        {
            return mSiteName ?? (mSiteName = SiteContext.CurrentSiteName);
        }
        set
        {
            mSiteName = value;
        }
    }


    /// <summary>
    /// Indicates whether permissions should be checked.
    /// </summary>
    public bool CheckPermissions
    {
        get
        {
            return mCheckPermissions;
        }
        set
        {
            mCheckPermissions = value;
        }
    }


    /// <summary>
    /// Gets document. If attachment is temporary, gets parent node.
    /// </summary>
    private TreeNode Node
    {
        get
        {
            if (mNode == null)
            {
                AttachmentInfo info = InfoObject as AttachmentInfo;
                if (info != null)
                {
                    if (info.AttachmentDocumentID > 0)
                    {
                        mNode = DocumentHelper.GetDocument(info.AttachmentDocumentID, TreeProvider);
                        nodeIsParent = false;
                    }
                    else
                    {
                        // Get parent node ID in case attachment is edited for document not created yet
                        int parentNodeId = QueryHelper.GetInteger("parentId", 0);
                        mNode = TreeProvider.SelectSingleNode(parentNodeId);
                        nodeIsParent = true;
                    }
                }
            }

            return mNode;
        }
    }


    public BaseInfo InfoObject
    {
        get;
        set;
    }


    /// <summary>
    /// Script literal.
    /// </summary>
    public Literal LtlScript
    {
        get
        {
            return ltlScript;
        }
        set
        {
            ltlScript = value;
        }
    }


    /// <summary>
    /// Indicates whether the control is enabled.
    /// </summary>
    public bool Enabled
    {
        get
        {
            return mEnabled;
        }
        set
        {
            mEnabled = value;
            txtFileName.Enabled = value;
            txtTitle.Enabled = value;
            txtDescription.Enabled = value;
        }
    }


    /// <summary>
    /// Gets or sets current attachment history identifier in attachment is under workflow.
    /// </summary>
    private int AttachmentHistoryID
    {
        get;
        set;
    }


    /// <summary>
    /// If false, control will not be rendered in form-horizontal class.
    /// Default is true.
    /// </summary>
    public bool RenderAsForm
    {
        get 
        {
            return mRenderAsForm;
        }
        set 
        {
            mRenderAsForm = value;
        }
    }

    #endregion


    #region "Events"

    /// <summary>
    /// Initializes info object.
    /// </summary>
    /// <param name="objectGuid">Object GUID</param>
    /// <param name="siteName">Site name</param>
    public delegate void OnInitializeObject(Guid objectGuid, string siteName);

    public event OnInitializeObject InitializeObject;


    /// <summary>
    /// Saves modified image data. Returns True if metadata were successfully saved.
    /// </summary>
    /// <param name="fileName">File name</param>
    /// <param name="title">Title</param>
    /// <param name="description">Description</param>
    public delegate bool OnSave(string fileName, string title, string description);

    public event OnSave Save;


    /// <summary>
    /// Sets title and description.
    /// </summary>
    public event EventHandler OnSetMetaData;


    /// <summary>
    /// Gets object extension.
    /// </summary>
    /// <param name="extension">Object extension</param>
    public delegate void OnGetObjectExtension(string extension);

    public event OnGetObjectExtension GetObjectExtension;

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        Initialize();
    }


    /// <summary>
    /// Initializes controls.
    /// </summary>
    private void Initialize()
    {
        txtTitle.IsLiveSite = IsLiveSite;
        txtDescription.IsLiveSite = IsLiveSite;

        switch (ObjectType)
        {
            // Attachment
            case AttachmentInfo.OBJECT_TYPE:
                InitializeAttachment();
                break;

            // Metafile
            case MetaFileInfo.OBJECT_TYPE:
                InitializeMetaFile();
                break;

            // Media file
            case PredefinedObjectType.MEDIAFILE:
                InitializeMediaFile();
                break;
        }

        if (!URLHelper.IsPostback())
        {
            InitializeForm();
        }

        if (!RenderAsForm)
        {
            pnlFormHorizontal.CssClass = String.Empty;
        }
    }


    /// <summary>
    /// Initializes form controls (title and description).
    /// </summary>
    private void InitializeForm()
    {
        switch (ObjectType)
        {
            // Attachment
            case AttachmentInfo.OBJECT_TYPE:
                AttachmentInfo attachmentInfo = InfoObject as AttachmentInfo;
                if (attachmentInfo != null)
                {
                    ObjectTitle = attachmentInfo.AttachmentTitle;
                    ObjectDescription = attachmentInfo.AttachmentDescription;
                    ObjectFileName = Path.GetFileNameWithoutExtension(attachmentInfo.AttachmentName);
                    ObjectExtension = attachmentInfo.AttachmentExtension;
                    ObjectSize = DataHelper.GetSizeString(attachmentInfo.AttachmentSize);
                }
                break;

            // Meta file
            case MetaFileInfo.OBJECT_TYPE:
                MetaFileInfo metaFileInfo = InfoObject as MetaFileInfo;
                if (metaFileInfo != null)
                {
                    ObjectTitle = metaFileInfo.MetaFileTitle;
                    ObjectDescription = metaFileInfo.MetaFileDescription;
                    ObjectFileName = Path.GetFileNameWithoutExtension(metaFileInfo.MetaFileName);
                    ObjectExtension = metaFileInfo.MetaFileExtension;
                    ObjectSize = DataHelper.GetSizeString(metaFileInfo.MetaFileSize);
                }
                break;

            // Media file
            case PredefinedObjectType.MEDIAFILE:
                if (OnSetMetaData != null)
                {
                    OnSetMetaData(this, null);
                }
                break;

            // Nothing
            default:
                break;
        }

        if (!Enabled)
        {
            // Disable controls
            txtFileName.Enabled = false;
            txtTitle.Enabled = false;
            txtDescription.Enabled = false;
        }
    }


    /// <summary>
    /// Initializes properties.
    /// </summary>
    private void InitializeAttachment()
    {
        AttachmentInfo attachmentInfo;

        if (InfoObject != null)
        {
            attachmentInfo = InfoObject as AttachmentInfo;
        }
        else
        {
            // If using workflow then get versioned attachment
            if (VersionHistoryID != 0)
            {
                // Get the versioned attachment with binary data
                AttachmentHistoryInfo attachmentHistory = VersionManager.GetAttachmentVersion(VersionHistoryID, ObjectGuid, false);

                // Create new attachment object
                attachmentInfo = (attachmentHistory != null) ? new AttachmentInfo(attachmentHistory.Generalized.DataClass) : null;
                if (attachmentInfo != null)
                {
                    // Save attachment history identifier for unique test
                    AttachmentHistoryID = attachmentHistory.AttachmentHistoryID;
                    attachmentInfo.AttachmentVersionHistoryID = VersionHistoryID;
                }
            }
            // Else get file without binary data
            else
            {
                attachmentInfo = AttachmentInfoProvider.GetAttachmentInfoWithoutBinary(ObjectGuid, SiteName);
            }

            InfoObject = attachmentInfo;
        }

        if (attachmentInfo != null)
        {
            // Check permissions
            if (CheckPermissions)
            {
                // If attachment is temporary, check 'Create' permission on parent node. Else check 'Modify' permission.
                NodePermissionsEnum permission = nodeIsParent ? NodePermissionsEnum.Create : NodePermissionsEnum.Modify;

                if (Node == null)
                {
                    RedirectToInformation(GetString("editeddocument.notexists"));
                }

                if (MembershipContext.AuthenticatedUser.IsAuthorizedPerDocument(Node, permission) != AuthorizationResultEnum.Allowed)
                {
                    RedirectToAccessDenied(GetString("metadata.errors.filemodify"));
                }
            }

            // Fire event GetObjectExtension
            if (GetObjectExtension != null)
            {
                GetObjectExtension(attachmentInfo.AttachmentExtension);
            }
        }
        else
        {
            RedirectToInformation(GetString("editedobject.notexists"));
        }
    }


    /// <summary>
    /// Initializes metafile.
    /// </summary>
    private void InitializeMetaFile()
    {
        MetaFileInfo metaFileInfo;

        if (InfoObject != null)
        {
            metaFileInfo = InfoObject as MetaFileInfo;
        }
        else
        {
            // Get metafile
            metaFileInfo = MetaFileInfoProvider.GetMetaFileInfoWithoutBinary(ObjectGuid, SiteName, true);
            InfoObject = metaFileInfo;
        }

        // If file is not null and current user is global administrator then set image
        if (metaFileInfo != null)
        {
            // Check modify permission
            if (CheckPermissions && !UserInfoProvider.IsAuthorizedPerObject(metaFileInfo.MetaFileObjectType, metaFileInfo.MetaFileObjectID, PermissionsEnum.Modify, SiteContext.CurrentSiteName, MembershipContext.AuthenticatedUser))
            {
                RedirectToAccessDenied(GetString("metadata.errors.filemodify"));
            }

            // Fire event GetObjectExtension
            if (GetObjectExtension != null)
            {
                GetObjectExtension(metaFileInfo.MetaFileExtension);
            }
        }
        else
        {
            URLHelper.Redirect(UIHelper.GetInformationUrl("editedobject.notexists"));
        }
    }


    /// <summary>
    /// Initializes media file.
    /// </summary>
    private void InitializeMediaFile()
    {
        if (InitializeObject != null)
        {
            InitializeObject(ObjectGuid, SiteName);
        }
    }


    /// <summary>
    /// Saves metadata.
    /// </summary>
    /// <returns>Returns True if object was successfully saved.</returns>
    public bool SaveMetadata()
    {
        // Validate file name
        string validationError;
        string newFileName = ValidateFileName(ObjectFileName, out validationError);

        // New attachment name is valid
        if (string.IsNullOrEmpty(validationError))
        {
            switch (ObjectType)
            {
                // Save attachment
                case AttachmentInfo.OBJECT_TYPE:
                    return SaveAttachment(newFileName);

                // Save meta file
                case MetaFileInfo.OBJECT_TYPE:
                    return SaveMetaFile(newFileName);

                // Save media file
                case PredefinedObjectType.MEDIAFILE:
                    return SaveMediaFile(newFileName);

                default:
                    return false;
            }
        }
        else
        {
            ShowError(validationError);
        }

        return false;
    }


    /// <summary>
    /// Validates file name. Returns new valid file name.
    /// </summary>
    /// <param name="fileName">New file name</param>
    /// <param name="result">Validation result</param>
    public string ValidateFileName(string fileName, out string result)
    {
        string newFileName = null;
        string validationResult = null;

        if (!ShowOnlyTitleAndDescription)
        {
            Validator validator = new Validator();
            fileName = ValidationHelper.GetSafeFileName(fileName.Trim());
            validationResult = validator.IsFileName(fileName, GetString("img.errors.filename")).Result;

            if (string.IsNullOrEmpty(validationResult))
            {
                newFileName = URLHelper.GetSafeFileName(fileName, SiteContext.CurrentSiteName);
                validationResult = validator.NotEmpty(newFileName, GetString("img.errors.filename")).Result;

                // Check that user input is bigger then "" and trim (DB field)
                if (string.IsNullOrEmpty(validationResult))
                {
                    if (newFileName.Length > 245)
                    {
                        newFileName = newFileName.Remove(245);
                    }
                }
            }
        }

        result = validationResult;
        return newFileName;
    }


    /// <summary>
    /// Saves metadata and file name of attachment.
    /// </summary>
    /// <param name="newFileName">New attachment file name</param>
    /// <returns>Returns True if attachment was successfully saved.</returns>
    private bool SaveAttachment(string newFileName)
    {
        bool saved = false;

        // Save new data
        try
        {
            AttachmentInfo attachmentInfo = InfoObject as AttachmentInfo;

            if (attachmentInfo != null)
            {
                // Set new file name
                if (!string.IsNullOrEmpty(newFileName))
                {
                    string name = newFileName + attachmentInfo.AttachmentExtension;

                    // Use correct identifier if attachment is under workflow
                    int identifier = VersionHistoryID > 0 ? AttachmentHistoryID : attachmentInfo.AttachmentID;
                   
                    if (IsAttachmentNameUnique(identifier, attachmentInfo.AttachmentExtension, name))
                    {
                        attachmentInfo.AttachmentName = name;
                    }
                    else
                    {
                        // Attachment already exists.
                        ShowError(GetString("img.errors.fileexists"));
                        return false;
                    }
                }

                // Ensure automatic check-in/ check-out
                bool useWorkflow = false;
                bool autoCheck = false;
                WorkflowManager workflowMan = WorkflowManager.GetInstance(TreeProvider);

                if (!nodeIsParent && (Node != null))
                {
                    // Get workflow info
                    WorkflowInfo wi = workflowMan.GetNodeWorkflow(Node);

                    // Check if the document uses workflow
                    if (wi != null)
                    {
                        useWorkflow = true;
                        autoCheck = !wi.UseCheckInCheckOut(SiteName);
                    }

                    // Check out the document
                    if (autoCheck)
                    {
                        VersionManager.CheckOut(Node, Node.IsPublished, true);
                        VersionHistoryID = Node.DocumentCheckedOutVersionHistoryID;
                    }

                    // Workflow has been lost, get published attachment
                    if (useWorkflow && (VersionHistoryID == 0))
                    {
                        attachmentInfo = AttachmentInfoProvider.GetAttachmentInfo(attachmentInfo.AttachmentGUID, SiteName);
                    }
                }

                if (attachmentInfo != null)
                {
                    // Set filename title and description
                    attachmentInfo.AttachmentTitle = ObjectTitle;
                    attachmentInfo.AttachmentDescription = ObjectDescription;

                    // Document uses workflow and document is already saved (attachment is not temporary)
                    if (!nodeIsParent && (VersionHistoryID > 0))
                    {
                        attachmentInfo.AllowPartialUpdate = true;
                        VersionManager.SaveAttachmentVersion(attachmentInfo, VersionHistoryID);
                    }
                    else
                    {
                        // Update without binary
                        attachmentInfo.AllowPartialUpdate = true;
                        AttachmentInfoProvider.SetAttachmentInfo(attachmentInfo);

                        // Log the synchronization and search task for the document
                        if (!nodeIsParent && (Node != null))
                        {
                            // Update search index for given document
                            if (DocumentHelper.IsSearchTaskCreationAllowed(Node))
                            {
                                SearchTaskInfoProvider.CreateTask(SearchTaskTypeEnum.Update, TreeNode.OBJECT_TYPE, SearchFieldsConstants.ID, Node.GetSearchID(), Node.DocumentID);
                            }

                            DocumentSynchronizationHelper.LogDocumentChange(Node, TaskTypeEnum.UpdateDocument, TreeProvider);
                        }
                    }

                    if (!nodeIsParent && (Node != null))
                    {
                        // Check in the document
                        if (autoCheck)
                        {
                            if (VersionManager != null)
                            {
                                if (VersionHistoryID > 0 && (Node != null))
                                {
                                    VersionManager.CheckIn(Node, null, null);
                                }
                            }
                        }
                    }

                    saved = true;

                    string fullRefresh = "false";

                    if (autoCheck || (Node == null))
                    {
                        fullRefresh = "true";
                    }

                    // Refresh parent update panel
                    LtlScript.Text = ScriptHelper.GetScript("RefreshMetaData(" + ScriptHelper.GetString(ExternalControlID) + ", '" + fullRefresh + "', '" + attachmentInfo.AttachmentGUID + "', 'refresh')");
                }
            }
        }
        catch (Exception ex)
        {
            ShowError(GetString("metadata.errors.processing"));
            EventLogProvider.LogException("Metadata editor", "SAVEATTACHMENT", ex);
        }

        return saved;
    }


    /// <summary>
    /// Saves metadata and file name of metafile.
    /// </summary>
    /// <param name="newFileName">New metafile file name</param>
    /// <returns>Returns True if metafile was successfully saved.</returns>
    private bool SaveMetaFile(string newFileName)
    {
        bool saved = false;

        MetaFileInfo metaFileInfo = InfoObject as MetaFileInfo;

        if (metaFileInfo != null)
        {
            try
            {
                // Set title and description
                metaFileInfo.MetaFileTitle = ObjectTitle;
                metaFileInfo.MetaFileDescription = ObjectDescription;

                // Set new file name
                if (!string.IsNullOrEmpty(newFileName))
                {
                    metaFileInfo.MetaFileName = newFileName + ObjectExtension;
                }

                // Save new metadata
                metaFileInfo.AllowPartialUpdate = true;
                MetaFileInfoProvider.SetMetaFileInfo(metaFileInfo);

                saved = true;
                LtlScript.Text = ScriptHelper.GetScript("RefreshMetaFile();");
            }
            catch (Exception ex)
            {
                ShowError(GetString("metadata.errors.processing"));
                EventLogProvider.LogException("Metadata editor", "SAVEMETAFILE", ex);
            }
        }

        return saved;
    }


    /// <summary>
    /// Saves metadata and file name of media file.
    /// </summary>
    /// <param name="newFileName">New file name</param>
    private bool SaveMediaFile(string newFileName)
    {
        if (Save != null)
        {
            return Save(newFileName, ObjectTitle, ObjectDescription);
        }

        return false;
    }


    /// <summary>
    /// Renders control.
    /// </summary>
    protected override void Render(HtmlTextWriter writer)
    {
        if (RenderTableTag)
        {
            writer.Write("<table id=\"metadataTable\" class=\"MetaDataTable\">");
        }
        base.Render(writer);

        if (RenderTableTag)
        {
            writer.Write("</table>");
        }
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        plcFileName.Visible = !ShowOnlyTitleAndDescription;
        plcExtensionAndSize.Visible = !ShowOnlyTitleAndDescription;
    }


    /// <summary>
    /// Checks whether the name is unique.
    /// </summary>
    private bool IsAttachmentNameUnique(int attachmentId, string attachmentExtension, string name)
    {
        if (attachmentId > 0)
        {
            // Check that the name is unique in the document or version context
            Guid attachmentFormGuid = QueryHelper.GetGuid("formguid", Guid.Empty);
            bool nameIsUnique;
            if (attachmentFormGuid == Guid.Empty)
            {
                // Get the node
                nameIsUnique = DocumentHelper.IsUniqueAttachmentName(Node, name, attachmentExtension, attachmentId, TreeProvider);
            }
            else
            {
                nameIsUnique = AttachmentInfoProvider.IsUniqueTemporaryAttachmentName(attachmentFormGuid, name, attachmentExtension, attachmentId);
            }

            return nameIsUnique;
        }

        return false;
    }

    #endregion
}