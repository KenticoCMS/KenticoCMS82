using System;
using System.Text;
using System.Web.UI.WebControls;
using System.Collections.Generic;

using CMS.ExtendedControls;
using CMS.FormControls;
using CMS.FormEngine;
using CMS.Helpers;
using CMS.Base;
using CMS.SiteProvider;
using CMS.Membership;
using CMS.DocumentEngine;
using CMS.UIControls;
using CMS.WorkflowEngine;
using CMS.ExtendedControls.ActionsConfig;
using CMS.DataEngine;

using TreeNode = CMS.DocumentEngine.TreeNode;

public partial class CMSModules_DocumentLibrary_Controls_LibraryContextMenu : CMSUserControl
{
    #region "Constants"

    private const string CMS_FILE = "cms.file";

    #endregion


    #region "Variables"

    private MouseButtonEnum mMouseButton = MouseButtonEnum.Right;
    protected VerticalPositionEnum mVerticalPosition = VerticalPositionEnum.Cursor;
    protected HorizontalPositionEnum mHorizontalPosition = HorizontalPositionEnum.Cursor;

    private FormFieldInfo mFieldInfo;
    private DataClassInfo mDataClass;

    protected int mOffsetX = 0;
    protected int mOffsetY = 0;
    protected string mActiveItemCssClass = "SelectedRowContext";
    protected int mActiveItemOffset = 0;
    protected string mActiveItemInactiveCssClass = "Row";
    protected string mJavaScriptPrefix = null;

    #endregion


    #region "Properties"

    /// <summary>
    /// Prefix used to avoid java-script function name collision.
    /// </summary>
    public string JavaScriptPrefix
    {
        get
        {
            return mJavaScriptPrefix ?? (mJavaScriptPrefix = "DLCM_" + Parent.ClientID + "_");
        }
        set
        {
            mJavaScriptPrefix = value;
        }
    }


    /// <summary>
    /// Gets or sets handled mouse button(s).
    /// </summary>
    public MouseButtonEnum MouseButton
    {
        get
        {
            return mMouseButton;
        }
        set
        {
            mMouseButton = value;
            libraryMenuElem.MouseButton = value;
        }
    }


    /// <summary>
    /// Vertical position.
    /// </summary>
    public VerticalPositionEnum VerticalPosition
    {
        get
        {
            return mVerticalPosition;
        }
        set
        {
            mVerticalPosition = value;
        }
    }


    /// <summary>
    /// Horizontal position.
    /// </summary>
    public HorizontalPositionEnum HorizontalPosition
    {
        get
        {
            return mHorizontalPosition;
        }
        set
        {
            mHorizontalPosition = value;
        }
    }


    /// <summary>
    /// Offset X of the menu.
    /// </summary>
    public int OffsetX
    {
        get
        {
            return mOffsetX;
        }
        set
        {
            mOffsetX = value;
            libraryMenuElem.OffsetX = value;
        }
    }


    /// <summary>
    /// Offset Y of the menu.
    /// </summary>
    public int OffsetY
    {
        get
        {
            return mOffsetY;
        }
        set
        {
            mOffsetY = value;
            libraryMenuElem.OffsetY = value;
        }
    }


    /// <summary>
    /// Active item CSS class.
    /// </summary>
    public string ActiveItemCssClass
    {
        get
        {
            return mActiveItemCssClass;
        }
        set
        {
            mActiveItemCssClass = value;
        }
    }


    /// <summary>
    /// CSS class of highlighted element when context menu is closed.
    /// </summary>
    public string ActiveItemInactiveCssClass
    {
        get
        {
            return mActiveItemInactiveCssClass;
        }
        set
        {
            mActiveItemInactiveCssClass = value;
        }
    }


    /// <summary>
    /// Active item level offset.
    /// </summary>
    public int ActiveItemOffset
    {
        get
        {
            return mActiveItemOffset;
        }
        set
        {
            mActiveItemOffset = value;
        }
    }


    /// <summary>
    /// Tree provider instance.
    /// </summary>
    public TreeProvider TreeProvider
    {
        get
        {
            DocumentManager.Tree.UseCache = false;
            return DocumentManager.Tree;
        }
    }


    /// <summary>
    /// Workflow manager instance.
    /// </summary>
    public WorkflowManager WorkflowManager
    {
        get
        {
            return DocumentManager.WorkflowManager;
        }
    }


    /// <summary>
    /// If true, the access permissions to the items are checked.
    /// </summary>
    public bool CheckPermissions
    {
        get
        {
            return ValidationHelper.GetBoolean(ViewState["CheckPermissions"], true);
        }
        set
        {
            ViewState["CheckPermissions"] = value;
        }
    }


    /// <summary>
    /// Class field info for the attachment.
    /// </summary>
    private FormFieldInfo FieldInfo
    {
        get
        {
            if (mFieldInfo == null)
            {
                FormInfo fi;

                if (!string.IsNullOrEmpty(DocumentForm))
                {
                    fi = FormHelper.GetFormInfo(DocumentForm, true);
                }
                else
                {
                    fi = FormHelper.GetFormInfo(DataClass.ClassName, true);
                }

                // Get valid extensions from form field info
                mFieldInfo = fi.GetFormField("FileAttachment");
            }
            return mFieldInfo;
        }
    }


    /// <summary>
    /// Data class info for the cms.file.
    /// </summary>
    private DataClassInfo DataClass
    {
        get
        {
            if (mDataClass == null)
            {
                // Get document type ('cms.file') settings
                mDataClass = DataClassInfoProvider.GetDataClassInfo(CMS_FILE);
                if (mDataClass == null)
                {
                    throw new Exception("[DocumentLibrary.DataClass]: Class '" + CMS_FILE + "' is missing!");
                }
            }
            return mDataClass;
        }
    }


    /// <summary>
    /// Specifies the form used for editing document properties.
    /// </summary>
    public string DocumentForm
    {
        get;
        set;
    }

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        string commMenuID = "commMenu_" + ClientID;
        cmcApp.Parameter = "GetContextMenuParameter('libraryMenu_" + ClientID + "')" + string.Format(" + '|{0}'", DocumentComponentEvents.APPROVE);
        cmcApp.MenuID = commMenuID;
        cmcRej.Parameter = "GetContextMenuParameter('libraryMenu_" + ClientID + "')" + string.Format(" + '|{0}'", DocumentComponentEvents.REJECT);
        cmcRej.MenuID = commMenuID;
        cmcCheck.Parameter = "GetContextMenuParameter('libraryMenu_" + ClientID + "')" + string.Format(" + '|{0}'", DocumentComponentEvents.CHECKIN);
        cmcCheck.MenuID = commMenuID;
        cmcArch.Parameter = "GetContextMenuParameter('libraryMenu_" + ClientID + "')" + string.Format(" + '|{0}'", DocumentComponentEvents.ARCHIVE);
        cmcArch.MenuID = commMenuID;

        libraryMenuElem.LoadingContent = menuComm.LoadingContent = new ContextMenuItem { ResourceString = "ContextMenu.Loading" }.GetRenderedHTML();
        menuComm.OnReloadData += menuComm_OnReloadData;
        menuComm.MenuID = commMenuID;

        // Initialize menu element
        libraryMenuElem.OnReloadData += libraryMenuElem_OnReloadData;

        libraryMenuElem.MenuID = "libraryMenu_" + ClientID;
        libraryMenuElem.MouseButton = MouseButton;
        libraryMenuElem.OffsetX = OffsetX;
        libraryMenuElem.OffsetY = OffsetY;
        libraryMenuElem.VerticalPosition = VerticalPosition;
        libraryMenuElem.HorizontalPosition = HorizontalPosition;
        libraryMenuElem.ActiveItemCssClass = ActiveItemCssClass;
        libraryMenuElem.ActiveItemOffset = ActiveItemOffset;
        libraryMenuElem.ActiveItemInactiveCssClass = ActiveItemInactiveCssClass;

        ExternalEditHelper.RenderScripts(this);
    }

    #endregion


    #region "Dynamic menu loading"

    protected void menuComm_OnReloadData(object sender, EventArgs e)
    {
        var parameters = (menuComm.Parameter ?? string.Empty).Split(new [] { '|' }, StringSplitOptions.RemoveEmptyEntries);
        if (parameters.Length == 3)
        {
            // Parse identifier and document culture from library parameter
            int nodeId = ValidationHelper.GetInteger(parameters[0], 0);
            string cultureCode = ValidationHelper.GetString(parameters[1], string.Empty);
            string action = ValidationHelper.GetString(parameters[2], string.Empty);

            DocumentManager.Mode = FormModeEnum.Update;
            DocumentManager.ClearNode();
            DocumentManager.DocumentID = 0;
            DocumentManager.NodeID = nodeId;
            DocumentManager.CultureCode = cultureCode;

            TreeNode node = DocumentManager.Node;

            string parameterScript = "GetContextMenuParameter('" + libraryMenuElem.MenuID + "')";

            CommentAction comment = new CommentAction(Page, action);
            lblComment.Text = comment.Text;
            lblComment.RefreshText();
            pnlComment.Attributes.Add("onclick", JavaScriptPrefix + "PerformAction(" + parameterScript + ", 'RefreshGridSimple');" + DocumentManager.GetJSFunction(ComponentEvents.COMMENT, string.Join("|", new [] { "'" + action + "'", node.DocumentID.ToString() }), null) + ";");
        }
    }


    protected void libraryMenuElem_OnReloadData(object sender, EventArgs e)
    {
        var parameters = (libraryMenuElem.Parameter ?? string.Empty).Split(new [] { '|' }, StringSplitOptions.RemoveEmptyEntries);
        if (parameters.Length == 2)
        {
            // Parse identifier and document culture from library parameter
            int nodeId = ValidationHelper.GetInteger(parameters[0], 0);
            string cultureCode = ValidationHelper.GetString(parameters[1], string.Empty);

            DocumentManager.Mode = FormModeEnum.Update;
            DocumentManager.ClearNode();
            DocumentManager.DocumentID = 0;
            DocumentManager.NodeID = nodeId;
            DocumentManager.CultureCode = cultureCode;

            TreeNode node = DocumentManager.Node;

            bool contextMenuVisible = false;
            bool localizeVisible = false;
            bool editVisible = false;
            bool uploadVisible = false;
            bool copyVisible = false;
            bool deleteVisible = false;
            bool openVisible = false;
            bool propertiesVisible = false;
            bool permissionsVisible = false;
            bool versionHistoryVisible = false;

            bool checkOutVisible = false;
            bool checkInVisible = false;
            bool undoCheckoutVisible = false;
            bool submitToApprovalVisible = false;
            bool rejectVisible = false;
            bool archiveVisible = false;

            if ((node != null) && (!CheckPermissions || (MembershipContext.AuthenticatedUser.IsAuthorizedPerDocument(node, NodePermissionsEnum.Read) == AuthorizationResultEnum.Allowed)))
            {
                // Get original node (in case of linked documents)
                TreeNode originalNode = TreeProvider.GetOriginalNode(node);

                string siteName = SiteContext.CurrentSiteName;
                string currentDocumentCulture = DocumentContext.CurrentDocumentCulture.CultureCode;

                if (SiteContext.CurrentSiteID != originalNode.NodeSiteID)
                {
                    SiteInfo si = SiteInfoProvider.GetSiteInfo(originalNode.NodeSiteID);
                    siteName = si.SiteName;
                }

                if (!DocumentManager.ProcessingAction)
                {
                    // Get permissions
                    const bool authorizedToRead = true;
                    bool authorizedToDelete = (MembershipContext.AuthenticatedUser.IsAuthorizedPerDocument(node, NodePermissionsEnum.Delete) == AuthorizationResultEnum.Allowed);
                    bool authorizedToModify = (MembershipContext.AuthenticatedUser.IsAuthorizedPerDocument(node, NodePermissionsEnum.Modify) == AuthorizationResultEnum.Allowed);
                    bool authorizedCultureToModify = (MembershipContext.AuthenticatedUser.IsAuthorizedPerDocument(node, NodePermissionsEnum.Modify, false) == AuthorizationResultEnum.Allowed) && TreeSecurityProvider.HasUserCultureAllowed(NodePermissionsEnum.Modify, currentDocumentCulture, MembershipContext.AuthenticatedUser, siteName);
                    bool authorizedToModifyPermissions = (MembershipContext.AuthenticatedUser.IsAuthorizedPerDocument(node, NodePermissionsEnum.ModifyPermissions) == AuthorizationResultEnum.Allowed);
                    bool authorizedToCreate = MembershipContext.AuthenticatedUser.IsAuthorizedToCreateNewDocument(node.NodeParentID, node.NodeClassName);

                    // Hide menu when user has no 'Read' permissions on document
                    libraryMenuElem.Visible = authorizedToRead;

                    // First evaluation of control's visibility
                    bool differentCulture = (CMSString.Compare(node.DocumentCulture, currentDocumentCulture, true) != 0);

                    localizeVisible = differentCulture && authorizedToCreate && authorizedCultureToModify;
                    uploadVisible = authorizedToModify && DocumentManager.AllowSave;
                    copyVisible = authorizedToCreate && authorizedToModify;
                    deleteVisible = authorizedToDelete;
                    openVisible = authorizedToRead;
                    propertiesVisible = authorizedToModify;
                    permissionsVisible = authorizedToModifyPermissions;
                    versionHistoryVisible = authorizedToModify;

                    editVisible = authorizedToModify;

                    // Get next step info
                    var stps = new List<WorkflowStepInfo>();
                    var workflow = DocumentManager.Workflow;

                    bool basicWorkflow = true;
                    if (workflow != null)
                    {
                        basicWorkflow = workflow.IsBasic;
                        stps = WorkflowManager.GetNextStepInfo(node);
                    }
                    var appSteps = stps.FindAll(s => !s.StepIsArchived);
                    var archSteps = stps.FindAll(s => s.StepIsArchived);

                    // Workflow actions
                    submitToApprovalVisible = DocumentManager.IsActionAllowed(DocumentComponentEvents.APPROVE) && (appSteps.Count > 0);
                    rejectVisible = DocumentManager.IsActionAllowed(DocumentComponentEvents.REJECT);
                    archiveVisible = DocumentManager.IsActionAllowed(DocumentComponentEvents.ARCHIVE) && ((archSteps.Count > 0) || basicWorkflow);
                    checkOutVisible = DocumentManager.IsActionAllowed(DocumentComponentEvents.CHECKOUT);
                    checkInVisible = DocumentManager.IsActionAllowed(DocumentComponentEvents.CHECKIN);
                    undoCheckoutVisible = DocumentManager.IsActionAllowed(DocumentComponentEvents.UNDO_CHECKOUT);

                    string parameterScript = "GetContextMenuParameter('" + libraryMenuElem.MenuID + "')";

                    // Initialize edit menu item
                    Guid attachmentGuid = ValidationHelper.GetGuid(node.GetValue("FileAttachment"), Guid.Empty);

                    // If attachment field doesn't allow empty value and the value is empty
                    if ((FieldInfo != null) && !FieldInfo.AllowEmpty && (attachmentGuid == Guid.Empty))
                    {
                        submitToApprovalVisible = false;
                        archiveVisible = false;
                        checkInVisible = false;
                    }

                    // Get attachment
                    var ai = DocumentHelper.GetAttachment(attachmentGuid, TreeProvider, siteName, false);

                    Panel currentPanel = pnlEdit;

                    if (editVisible)
                    {
                        if (ai != null)
                        {
                            // Setup external editing
                            var ctrl = ExternalEditHelper.LoadExternalEditControl(pnlEditPadding, FileTypeEnum.Attachment, null, ai, IsLiveSite, node);
                            if (ctrl != null)
                            {
                                ctrl.ID = "editAttachment";

                                ctrl.AttachmentFieldName = "FileAttachment";

                                ctrl.LabelText = GetString("general.edit");

                                ctrl.RefreshScript = JavaScriptPrefix + "PerformAction(" + parameterScript + ", 'ExternalRefresh');";

                                ctrl.ReloadData(true);

                                pnlEditPadding.CssClass = ctrl.EnabledResult ? "webdav-edit-item ItemPadding" : "webdav-edit-item webdav-edit-item-disabled ItemPaddingDisabled";
                            }
                            else
                            {
                                pnlEditPadding.Visible = false;
                            }
                        }
                        else
                        {
                            editVisible = false;
                            openVisible = false;
                        }
                    }

                    Panel previousPanel = currentPanel;
                    currentPanel = pnlUpload;

                    // Initialize upload menu item
                    if (authorizedToModify)
                    {
                        // Initialize direct file uploader
                        updateAttachment.Text = GetString("general.update");
                        updateAttachment.InnerElementClass = "LibraryContextUploader";
                        updateAttachment.DocumentID = node.DocumentID;
                        updateAttachment.ParentElemID = ClientID;
                        updateAttachment.SourceType = MediaSourceEnum.Attachment;
                        updateAttachment.AttachmentGUIDColumnName = "FileAttachment";
                        updateAttachment.IsLiveSite = IsLiveSite;

                        // Set allowed extensions
                        if ((FieldInfo != null) && ValidationHelper.GetString(FieldInfo.Settings["extensions"], "") == "custom")
                        {
                            // Load allowed extensions
                            updateAttachment.AllowedExtensions = ValidationHelper.GetString(FieldInfo.Settings["allowed_extensions"], "");
                        }
                        else
                        {
                            // Use site settings
                            updateAttachment.AllowedExtensions = SettingsKeyInfoProvider.GetValue(siteName + ".CMSUploadExtensions");
                        }

                        updateAttachment.ReloadData();
                        SetupPanelClasses(currentPanel, previousPanel);
                    }

                    previousPanel = currentPanel;
                    currentPanel = pnlLocalize;

                    // Initialize localize menu item
                    if (localizeVisible)
                    {
                        lblLocalize.RefreshText();
                        pnlLocalize.Attributes.Add("onclick", JavaScriptPrefix + "PerformAction(" + parameterScript + ", 'Localize');");
                        SetupPanelClasses(currentPanel, previousPanel);
                    }

                    previousPanel = null;
                    currentPanel = pnlCopy;

                    // Initialize copy menu item
                    if (copyVisible)
                    {
                        lblCopy.RefreshText();
                        pnlCopy.Attributes.Add("onclick", JavaScriptPrefix + "PerformAction(" + parameterScript + ",'Copy');");
                        SetupPanelClasses(currentPanel, previousPanel);
                    }

                    previousPanel = currentPanel;
                    currentPanel = pnlDelete;

                    // Initialize delete menu item
                    if (deleteVisible)
                    {
                        lblDelete.RefreshText();
                        pnlDelete.Attributes.Add("onclick", JavaScriptPrefix + "PerformAction(" + parameterScript + ", 'Delete');");
                        SetupPanelClasses(currentPanel, previousPanel);
                    }

                    previousPanel = currentPanel;
                    currentPanel = pnlOpen;

                    // Initialize open menu item
                    if (openVisible)
                    {
                        lblOpen.RefreshText();
                        if (ai != null)
                        {
                            // Get document URL
                            string attachmentUrl = AuthenticationHelper.ResolveUIUrl(AttachmentURLProvider.GetPermanentAttachmentUrl(node.NodeGUID, node.NodeAlias));
                            if (authorizedToModify)
                            {
                                attachmentUrl = URLHelper.AddParameterToUrl(attachmentUrl, "latestfordocid", ValidationHelper.GetString(node.DocumentID, string.Empty));
                                attachmentUrl = URLHelper.AddParameterToUrl(attachmentUrl, "hash", ValidationHelper.GetHashString("d" + node.DocumentID));
                            }
                            attachmentUrl = URLHelper.UpdateParameterInUrl(attachmentUrl, "chset", Guid.NewGuid().ToString());

                            if (!string.IsNullOrEmpty(attachmentUrl))
                            {
                                pnlOpen.Attributes.Add("onclick", "location.href = " + ScriptHelper.GetString(attachmentUrl) + ";");
                            }
                        }
                        SetupPanelClasses(currentPanel, previousPanel);
                    }

                    previousPanel = null;
                    currentPanel = pnlProperties;

                    // Initialize properties menu item
                    lblProperties.RefreshText();
                    pnlProperties.Attributes.Add("onclick", JavaScriptPrefix + "PerformAction(" + parameterScript + ", 'Properties');");
                    SetupPanelClasses(currentPanel, previousPanel);

                    previousPanel = currentPanel;
                    currentPanel = pnlPermissions;

                    // Initialize permissions menu item
                    lblPermissions.RefreshText();
                    pnlPermissions.Attributes.Add("onclick", JavaScriptPrefix + "PerformAction(" + parameterScript + ", 'Permissions');");
                    SetupPanelClasses(currentPanel, previousPanel);

                    previousPanel = currentPanel;
                    currentPanel = pnlVersionHistory;

                    // Initialize version history menu item
                    lblVersionHistory.RefreshText();
                    pnlVersionHistory.Attributes.Add("onclick", JavaScriptPrefix + "PerformAction(" + parameterScript + ", 'VersionHistory');");
                    SetupPanelClasses(currentPanel, previousPanel);

                    previousPanel = null;
                    currentPanel = pnlCheckOut;

                    // Initialize checkout menu item
                    if (checkOutVisible)
                    {
                        lblCheckOut.RefreshText();
                        pnlCheckOut.Attributes.Add("onclick", JavaScriptPrefix + "PerformAction(" + parameterScript + ", 'CheckOut');");
                        SetupPanelClasses(currentPanel, previousPanel);
                    }

                    previousPanel = currentPanel;
                    currentPanel = pnlCheckIn;

                    // Initialize check in menu item
                    if (checkInVisible)
                    {
                        lblCheckIn.RefreshText();
                        pnlCheckIn.Attributes.Add("onclick", JavaScriptPrefix + "PerformAction(" + parameterScript + ", 'CheckIn');");
                        SetupPanelClasses(currentPanel, previousPanel);
                    }

                    previousPanel = currentPanel;
                    currentPanel = pnlUndoCheckout;

                    // Initialize undo checkout menu item
                    if (undoCheckoutVisible)
                    {
                        lblUndoCheckout.RefreshText();
                        pnlUndoCheckout.Attributes.Add("onclick", JavaScriptPrefix + "PerformAction(" + parameterScript + ", 'UndoCheckout');");
                        SetupPanelClasses(currentPanel, previousPanel);
                    }

                    previousPanel = currentPanel;
                    currentPanel = pnlSubmitToApproval;

                    // Initialize submit to approval / publish menu item
                    if (submitToApprovalVisible)
                    {
                        // Only one next step
                        if (appSteps.Count == 1)
                        {
                            if (appSteps[0].StepIsPublished)
                            {
                                // Set 'Publish' label
                                lblSubmitToApproval.ResourceString = "general.publish";
                                cmcApp.Parameter = "GetContextMenuParameter('libraryMenu_" + ClientID + "')" + string.Format(" + '|{0}'", DocumentComponentEvents.PUBLISH);
                            }
                            pnlSubmitToApproval.Attributes.Add("onclick", JavaScriptPrefix + "PerformAction(" + parameterScript + ", 'SubmitToApproval');");
                        }
                        // Multiple steps - display dialog
                        else
                        {
                            pnlSubmitToApproval.Attributes.Add("onclick", JavaScriptPrefix + "PerformAction(" + parameterScript + ", 'RefreshGridSimple');" + DocumentManager.GetJSFunction(ComponentEvents.COMMENT, string.Join("|", new [] { "'" + DocumentComponentEvents.APPROVE + "'", node.DocumentID.ToString() }), null) + ";");
                            cmcApp.Enabled = false;
                        }

                        lblSubmitToApproval.RefreshText();
                        SetupPanelClasses(currentPanel, previousPanel);
                    }

                    previousPanel = currentPanel;
                    currentPanel = pnlReject;

                    // Initialize reject menu item
                    if (rejectVisible)
                    {
                        lblReject.RefreshText();
                        pnlReject.Attributes.Add("onclick", JavaScriptPrefix + "PerformAction(" + parameterScript + ", 'Reject');");
                        SetupPanelClasses(currentPanel, previousPanel);
                    }

                    previousPanel = currentPanel;
                    currentPanel = pnlArchive;

                    // Initialize archive menu item
                    if (archiveVisible)
                    {
                        // Only one archive step
                        if ((archSteps.Count == 1) || basicWorkflow)
                        {
                            pnlArchive.Attributes.Add("onclick", JavaScriptPrefix + "PerformAction(" + parameterScript + ", 'Archive');");
                        }
                        // Multiple archive steps - display dialog
                        else
                        {
                            pnlArchive.Attributes.Add("onclick", JavaScriptPrefix + "PerformAction(" + parameterScript + ", 'RefreshGridSimple');" + DocumentManager.GetJSFunction(ComponentEvents.COMMENT, string.Join("|", new [] { "'" + DocumentComponentEvents.ARCHIVE + "'", node.DocumentID.ToString() }), null) + ";");
                            cmcArch.Enabled = false;
                        }

                        lblArchive.RefreshText();
                        SetupPanelClasses(currentPanel, previousPanel);
                    }

                    // Set up visibility of menu items
                    pnlLocalize.Visible = localizeVisible;
                    pnlUpload.Visible = uploadVisible;
                    pnlDelete.Visible = deleteVisible;
                    pnlCopy.Visible = copyVisible;
                    pnlOpen.Visible = openVisible;
                    pnlProperties.Visible = propertiesVisible;
                    pnlPermissions.Visible = permissionsVisible;
                    pnlVersionHistory.Visible = versionHistoryVisible;
                    pnlEdit.Visible = editVisible;

                    pnlCheckOut.Visible = checkOutVisible;
                    pnlCheckIn.Visible = checkInVisible;
                    pnlUndoCheckout.Visible = undoCheckoutVisible;
                    pnlSubmitToApproval.Visible = submitToApprovalVisible;
                    pnlReject.Visible = rejectVisible;
                    pnlArchive.Visible = archiveVisible;

                    // Set up visibility of whole menu
                    contextMenuVisible = true;
                }

                if (DocumentManager.ProcessingAction)
                {
                    // Setup 'No action available' menu item
                    pnlNoAction.Visible = true;
                    lblNoAction.ResourceString = null;
                    lblNoAction.Text = DocumentManager.GetDocumentInfo(true);
                    lblNoAction.RefreshText();
                }
                else
                {
                    // Set up visibility of separators
                    bool firstGroupVisible = editVisible || uploadVisible || localizeVisible;
                    bool secondGroupVisible = copyVisible || deleteVisible || openVisible;
                    bool thirdGroupVisible = propertiesVisible || permissionsVisible || versionHistoryVisible;
                    bool fourthGroupVisible = checkOutVisible || checkInVisible || undoCheckoutVisible || submitToApprovalVisible || rejectVisible || archiveVisible;

                    pnlSep1.Visible = firstGroupVisible && secondGroupVisible;
                    pnlSep2.Visible = secondGroupVisible && thirdGroupVisible;
                    pnlSep3.Visible = thirdGroupVisible && fourthGroupVisible;

                    // Setup 'No action available' menu item
                    pnlNoAction.Visible = !contextMenuVisible;
                    lblNoAction.RefreshText();
                }
            }
        }
    }


    /// <summary>
    /// Sets up CSS classes of menu items.
    /// </summary>
    /// <param name="currentPanel">Current menu item</param>
    /// <param name="previousPanel">Previous menu item</param>
    private static void SetupPanelClasses(Panel currentPanel, Panel previousPanel)
    {
        if (previousPanel == null)
        {
            if (currentPanel != null)
            {
                currentPanel.CssClass = "Item";
            }
        }
        else
        {
            previousPanel.CssClass = "Item";
            if (currentPanel != null)
            {
                currentPanel.CssClass = "item-last";
            }
        }
    }
    
    #endregion
}