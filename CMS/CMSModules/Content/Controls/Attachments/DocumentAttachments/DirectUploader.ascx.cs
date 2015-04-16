using System;
using System.Data;
using System.Text;
using System.Web.UI.WebControls;
using System.Web.UI;

using CMS.ExtendedControls;
using CMS.FormControls;
using CMS.Helpers;
using CMS.Base;
using CMS.SiteProvider;
using CMS.Membership;
using CMS.DocumentEngine;
using CMS.UIControls;
using CMS.DataEngine;

public partial class CMSModules_Content_Controls_Attachments_DocumentAttachments_DirectUploader : AttachmentsControl, IUploaderControl
{
    #region "Variables"

    private string mInnerDivClass = "NewAttachment";

    private Guid attachmentGuid = Guid.Empty;
    private AttachmentInfo innerAttachment;

    private bool createTempAttachment;

    #endregion


    #region "Properties"

    /// <summary>
    /// CSS class of the new attachment link.
    /// </summary>
    public string InnerDivClass
    {
        get
        {
            return mInnerDivClass;
        }
        set
        {
            mInnerDivClass = value;
        }
    }


    /// <summary>
    /// Last performed action.
    /// </summary>
    public string LastAction
    {
        get
        {
            return ValidationHelper.GetString(ViewState["LastAction"], null);
        }
        set
        {
            ViewState["LastAction"] = value;
        }
    }


    /// <summary>
    /// Info label.
    /// </summary>
    public Label InfoLabel
    {
        get
        {
            return lblInfo;
        }
    }


    /// <summary>
    /// Inner attachment GUID (GUID of temporary attachment created for new culture version).
    /// </summary>
    public override Guid InnerAttachmentGUID
    {
        get
        {
            return ValidationHelper.GetGuid(ViewState["InnerAttachmentGUID"], base.InnerAttachmentGUID);
        }
        set
        {
            ViewState["InnerAttachmentGUID"] = value;
            base.InnerAttachmentGUID = value;
        }
    }


    /// <summary>
    /// Name of document attachment column.
    /// </summary>
    public override string GUIDColumnName
    {
        get
        {
            return base.GUIDColumnName;
        }
        set
        {
            base.GUIDColumnName = value;
            if ((dsAttachments != null) && (newAttachmentElem != null))
            {
                newAttachmentElem.AttachmentGUIDColumnName = value;
            }
        }
    }


    /// <summary>
    /// Width of the attachment.
    /// </summary>
    public override int ResizeToWidth
    {
        get
        {
            return base.ResizeToWidth;
        }
        set
        {
            base.ResizeToWidth = value;
            if (newAttachmentElem != null)
            {
                newAttachmentElem.ResizeToWidth = value;
            }
        }
    }


    /// <summary>
    /// Height of the attachment.
    /// </summary>
    public override int ResizeToHeight
    {
        get
        {
            return base.ResizeToHeight;
        }
        set
        {
            base.ResizeToHeight = value;
            if (newAttachmentElem != null)
            {
                newAttachmentElem.ResizeToHeight = value;
            }
        }
    }


    /// <summary>
    /// Maximal side size of the attachment.
    /// </summary>
    public override int ResizeToMaxSideSize
    {
        get
        {
            return base.ResizeToMaxSideSize;
        }
        set
        {
            base.ResizeToMaxSideSize = value;
            if (newAttachmentElem != null)
            {
                newAttachmentElem.ResizeToMaxSideSize = value;
            }
        }
    }


    /// <summary>
    /// List of allowed extensions.
    /// </summary>
    public override string AllowedExtensions
    {
        get
        {
            return base.AllowedExtensions;
        }
        set
        {
            base.AllowedExtensions = value;
            if (newAttachmentElem != null)
            {
                newAttachmentElem.AllowedExtensions = value;
            }
        }
    }


    /// <summary>
    /// Specifies the document for which the attachments should be displayed.
    /// </summary>
    public override int DocumentID
    {
        get
        {
            return base.DocumentID;
        }
        set
        {
            base.DocumentID = value;
            if (newAttachmentElem != null)
            {
                newAttachmentElem.DocumentID = value;
            }
        }
    }


    /// <summary>
    /// Specifies the version of the document (optional).
    /// </summary>
    public int VersionHistoryID
    {
        get
        {
            // When a new culture version is created, after an uploader action temporary attachment is handled
            if ((Form != null) && (Form.Mode == FormModeEnum.InsertNewCultureVersion) && !string.IsNullOrEmpty(LastAction))
            {
                return 0;
            }
            else if (Node != null)
            {
                return Node.DocumentCheckedOutVersionHistoryID;
            }

            return 0;
        }
    }


    /// <summary>
    /// Defines the form GUID; indicates that the temporary attachment will be handled.
    /// </summary>
    public override Guid FormGUID
    {
        get
        {
            return base.FormGUID;
        }
        set
        {
            base.FormGUID = value;
            if ((dsAttachments != null) && (newAttachmentElem != null))
            {
                dsAttachments.AttachmentFormGUID = value;
                newAttachmentElem.FormGUID = value;
            }
        }
    }


    /// <summary>
    /// Value of the control.
    /// </summary>
    public override object Value
    {
        get
        {
            return hdnAttachGuid.Value;
        }
        set
        {
            hdnAttachGuid.Value = (value == null) ? null : value.ToString();
        }
    }


    /// <summary>
    /// Name of the attachment.
    /// </summary>
    public string AttachmentName
    {
        get
        {
            return hdnAttachName.Value;
        }
    }


    /// <summary>
    /// If true, control does not process the data.
    /// </summary>
    public override bool StopProcessing
    {
        get
        {
            return base.StopProcessing;
        }
        set
        {
            base.StopProcessing = value;
            if ((dsAttachments != null) && (newAttachmentElem != null))
            {
                dsAttachments.StopProcessing = value;
                newAttachmentElem.StopProcessing = value;
            }
        }
    }

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Register script for tooltips
        if (ShowTooltip)
        {
            ScriptHelper.RegisterTooltip(Page);
        }

        // Ensure info message
        if ((Request[Page.postEventSourceID] == hdnPostback.ClientID) || Request[Page.postEventSourceID] == hdnFullPostback.ClientID)
        {
            string action = Request[Page.postEventArgumentID];

            if (action != null)
            {
                string[] values = action.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                action = values[0];
                if ((values.Length > 1) && ValidationHelper.IsGuid(values[1]))
                {
                    Value = values[1];
                }
            }

            LastAction = action;
        }


        #region "Scripts"

        // Refresh script
        StringBuilder script = new StringBuilder();
        script.AppendLine();
        script.AppendLine("function RefreshUpdatePanel_" + ClientID + @"(hiddenFieldID, action)");
        script.AppendLine("{");
        script.AppendLine("  var hiddenField = document.getElementById(hiddenFieldID);");
        script.AppendLine("  if (hiddenField)");
        script.AppendLine("  {");
        script.AppendLine("    __doPostBack(hiddenFieldID, action);");
        script.AppendLine("  }");
        script.AppendLine("}");

        script.AppendLine("function FullPageRefresh_" + ClientID + @"(guid)");
        script.AppendLine("{");
        script.AppendLine("  if (RefreshTree != null)");
        script.AppendLine("  {");
        script.AppendLine("    RefreshTree();");
        script.AppendLine("  }");
        script.AppendLine("  var hiddenField = document.getElementById('" + hdnFullPostback.ClientID + "');");
        script.AppendLine("  if (hiddenField)");
        script.AppendLine("  {");
        script.AppendLine("    __doPostBack('" + hdnFullPostback.ClientID + "', 'refresh|' + guid);");
        script.AppendLine("  }");
        script.AppendLine("}");

        // Initialize refresh script for update panel
        script.AppendLine("function InitRefresh_" + ClientID + "(msg, fullRefresh, refreshTree, guid, action)");
        script.AppendLine("{");
        script.AppendLine("  if ((msg != null) && (msg != \"\"))");
        script.AppendLine("  {");
        script.AppendLine("    alert(msg);");
        script.AppendLine("    action='error';");
        script.AppendLine("  }");
        if ((Node != null) && (Node.DocumentID > 0))
        {
            script.AppendLine(" if (window.RefreshTree && (fullRefresh || refreshTree)) { RefreshTree(" + NodeParentNodeID + ", " + NodeID + "); }");
            script.AppendLine("  RefreshUpdatePanel_" + ClientID + "(fullRefresh ? '" + hdnFullPostback.ClientID + "' : '" + hdnPostback.ClientID + "', action + '|' + guid);");
        }
        else
        {
            script.AppendLine("  RefreshUpdatePanel_" + ClientID + "('" + hdnPostback.ClientID + "', action + '|' + guid);");
        }
        script.AppendLine("}");

        // Initialize deletion confirmation
        script.AppendLine("function DeleteConfirmation()");
        script.AppendLine("{");
        script.AppendLine("  return confirm(" + ScriptHelper.GetLocalizedString("attach.deleteconfirmation") + ");");
        script.AppendLine("}");

        #endregion


        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "AttachmentScripts_" + ClientID, ScriptHelper.GetScript(script.ToString()));

        // Register dialog script
        ScriptHelper.RegisterDialogScript(Page);

        // Register jQuery script for thumbnails updating
        ScriptHelper.RegisterJQuery(Page);

        // Grid initialization
        gridAttachments.OnExternalDataBound += GridDocsOnExternalDataBound;
        gridAttachments.OnAction += GridAttachmentsOnAction;
        gridAttachments.IsLiveSite = IsLiveSite;
        gridAttachments.Pager.PageSizeOptions = "10";
        gridAttachments.Pager.DefaultPageSize = 10;

        if (DocumentManager != null)
        {
            DocumentManager.OnAfterAction += DocumentManager_OnAfterAction;
        }

        pnlAttachments.CssClass = "AttachmentsList SingleAttachment";
        pnlGrid.Attributes.Add("style", "padding-top: 2px;");

        // Ensure to raise the events
        if (RequestHelper.IsPostBack())
        {
            switch (LastAction)
            {
                case "delete":
                    RaiseDeleteFile(this, e);
                    break;

                case "update":
                    RaiseUploadFile(this, e);
                    break;
            }

            InnerAttachmentGUID = ValidationHelper.GetGuid(Value, Guid.Empty);
        }

        // Load data
        ReloadData(false);
    }


    void DocumentManager_OnAfterAction(object sender, DocumentManagerEventArgs e)
    {
        // Refresh control state after undo checkout action or when the workflow is finished
        if ((e.ActionName == DocumentComponentEvents.UNDO_CHECKOUT) || e.WorkflowFinished)
        {
            ReloadData(true);
        }
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        if (StopProcessing)
        {
            // Do nothing
        }
        else
        {
            lblError.Visible = (lblError.Text != "");
            lblInfo.Visible = (lblInfo.Text != "");

            // Ensure uploader button
            newAttachmentElem.Enabled = Enabled;

            // Hide actions
            gridAttachments.GridView.Columns[0].Visible = !HideActions;
            gridAttachments.GridView.Columns[1].Visible = !HideActions;
            if (Enabled)
            {
                newAttachmentElem.Visible = !HideActions;
            }
            else
            {
                newAttachmentElem.Visible = !HideActions && (attachmentGuid == Guid.Empty);
            }

            // Ensure correct layout
            bool gridHasData = !DataHelper.DataSourceIsEmpty(gridAttachments.DataSource);
            Visible = gridHasData || !HideActions;
            pnlGrid.Visible = gridHasData;

            // Initialize button for adding attachments
            plcUploader.Visible = (attachmentGuid == Guid.Empty) || !gridHasData;

            // Dialog for editing attachment
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("function Edit_" + ClientID + "(attachmentGUID, formGUID, versionHistoryID, parentId, hash, image) { ");
            sb.AppendLine("var form = '';");
            sb.AppendLine("if (formGUID != '') { form = '&formguid=' + formGUID + '&parentid=' + parentId; }");
            sb.AppendLine((((Node != null) && (Node.DocumentID > 0)) ? "else{form = '&siteid=' + " + Node.NodeSiteID + ";}" : ""));
            sb.AppendLine("if (image) {");
            sb.AppendLine("modalDialog('" + ResolveUrl((IsLiveSite ? "~/CMSFormControls/LiveSelectors/ImageEditor.aspx" : "~/CMSModules/Content/CMSDesk/Edit/ImageEditor.aspx") + "?attachmentGUID=' + attachmentGUID + '&refresh=1&versionHistoryID=' + versionHistoryID + form + '&clientid=" + ClientID + "&hash=' + hash") + ", 'editorDialog', 905, 670); }");
            sb.AppendLine("else {");
            sb.AppendLine("modalDialog('" + AuthenticationHelper.ResolveDialogUrl((IsLiveSite ? "~/CMSModules/Content/Attachments/CMSPages/MetaDataEditor.aspx" : "~/CMSModules/Content/Attachments/Dialogs/MetaDataEditor.aspx") + "?attachmentGUID=' + attachmentGUID + '&refresh=1&versionHistoryID=' + versionHistoryID + form + '&clientid=" + ClientID + "&hash=' + hash") + ", 'editorDialog', 700, 400); }");
            sb.AppendLine("return false; }");

            // Register script for editing attachment
            ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "AttachmentEditScripts_" + ClientID, ScriptHelper.GetScript(sb.ToString()));
        }
    }

    #endregion


    #region "Public methods"

    /// <summary>
    /// Indicates if the control contains some data.
    /// </summary>
    public override bool HasData()
    {
        return !DataHelper.DataSourceIsEmpty(dsAttachments.DataSource);
    }


    /// <summary>
    /// Clears data.
    /// </summary>
    public void Clear()
    {
        Value = Guid.Empty;
        ReloadData(true);
    }

    #endregion


    #region "Private & protected methods"

    public override void ReloadData(bool forceReload)
    {
        Visible = !StopProcessing;
        if (StopProcessing)
        {
            dsAttachments.StopProcessing = true;
            newAttachmentElem.StopProcessing = true;
            // Do nothing
        }
        else
        {
            if ((Node != null) && (Node.DocumentID > 0))
            {
                dsAttachments.Path = Node.NodeAliasPath;
                dsAttachments.CultureCode = Node.DocumentCulture;
                dsAttachments.SiteName = SiteInfoProvider.GetSiteName(Node.OriginalNodeSiteID);
            }

            // Get attachment GUID
            attachmentGuid = ValidationHelper.GetGuid(Value, Guid.Empty);
            if (attachmentGuid == Guid.Empty)
            {
                dsAttachments.StopProcessing = true;
            }

            dsAttachments.DocumentVersionHistoryID = VersionHistoryID;
            dsAttachments.AttachmentFormGUID = FormGUID;
            dsAttachments.AttachmentGUID = attachmentGuid;

            // Force reload datasource
            if (forceReload)
            {
                dsAttachments.DataSource = null;
                dsAttachments.DataBind();
            }

            // Ensure right column name (for attachments under workflow)
            if (!DataHelper.DataSourceIsEmpty(dsAttachments.DataSource))
            {
                DataSet ds = (DataSet)dsAttachments.DataSource;
                if (ds != null)
                {
                    DataTable dt = (ds).Tables[0];
                    if (!dt.Columns.Contains("AttachmentFormGUID"))
                    {
                        dt.Columns.Add("AttachmentFormGUID");
                    }

                    // Get inner attachment
                    innerAttachment = new AttachmentInfo(dt.Rows[0]);
                    Value = innerAttachment.AttachmentGUID;
                    hdnAttachName.Value = innerAttachment.AttachmentName;

                    // Check if temporary attachment should be created
                    createTempAttachment = ((DocumentID == 0) && (DocumentID != innerAttachment.AttachmentDocumentID));
                }
            }

            // Initialize button for adding attachments
            newAttachmentElem.SourceType = MediaSourceEnum.Attachment;
            newAttachmentElem.DocumentID = DocumentID;
            newAttachmentElem.NodeParentNodeID = NodeParentNodeID;
            newAttachmentElem.NodeClassName = NodeClassName;
            newAttachmentElem.ResizeToWidth = ResizeToWidth;
            newAttachmentElem.ResizeToHeight = ResizeToHeight;
            newAttachmentElem.ResizeToMaxSideSize = ResizeToMaxSideSize;
            newAttachmentElem.FormGUID = FormGUID;
            newAttachmentElem.AttachmentGUIDColumnName = GUIDColumnName;
            newAttachmentElem.AllowedExtensions = AllowedExtensions;
            newAttachmentElem.ParentElemID = ClientID;
            newAttachmentElem.ForceLoad = true;
            newAttachmentElem.Text = GetString("attach.uploadfile");
            newAttachmentElem.InnerElementClass = InnerDivClass;
            newAttachmentElem.InnerLoadingElementClass = InnerLoadingDivClass;
            newAttachmentElem.IsLiveSite = IsLiveSite;
            newAttachmentElem.IncludeNewItemInfo = true;
            newAttachmentElem.CheckPermissions = CheckPermissions;
            newAttachmentElem.NodeSiteName = SiteName;

            // Bind UniGrid to DataSource
            gridAttachments.DataSource = dsAttachments.DataSource;
            gridAttachments.LoadGridDefinition();
            gridAttachments.ReloadData();
        }
    }


    /// <summary>
    /// UniGrid action buttons event handler.
    /// </summary>
    protected void GridAttachmentsOnAction(string actionName, object actionArgument)
    {
        if (Enabled && !HideActions)
        {
            // Check the permissions


            #region "Check permissions"

            if (CheckPermissions)
            {
                if (FormGUID != Guid.Empty)
                {
                    if (!RaiseOnCheckPermissions("Create", this))
                    {
                        if (!MembershipContext.AuthenticatedUser.IsAuthorizedToCreateNewDocument(NodeParentNodeID, NodeClassName))
                        {
                            lblError.Text = GetString("attach.actiondenied");
                            return;
                        }
                    }
                }
                else
                {
                    if (!RaiseOnCheckPermissions("Modify", this))
                    {
                        if (MembershipContext.AuthenticatedUser.IsAuthorizedPerDocument(Node, NodePermissionsEnum.Modify) == AuthorizationResultEnum.Denied)
                        {
                            lblError.Text = GetString("attach.actiondenied");
                            return;
                        }
                    }
                }
            }

            #endregion


            Guid attGuid = Guid.Empty;

            // Get action argument (Guid or int)
            if (ValidationHelper.IsGuid(actionArgument))
            {
                attGuid = ValidationHelper.GetGuid(actionArgument, Guid.Empty);
            }

            // Process proper action
            switch (actionName.ToLowerCSafe())
            {
                case "delete":
                    if (!createTempAttachment)
                    {
                        if (attGuid != Guid.Empty)
                        {
                            // Delete attachment
                            if (FormGUID == Guid.Empty)
                            {
                                // Ensure automatic check-in/ check-out
                                VersionManager vm = null;

                                // Check out the document
                                if (AutoCheck)
                                {
                                    vm = VersionManager.GetInstance(TreeProvider);
                                    vm.CheckOut(Node, Node.IsPublished, true);
                                }

                                // If the GUID column is set, use it to process additional actions for field attachments
                                if (GUIDColumnName != null)
                                {
                                    DocumentHelper.DeleteAttachment(Node, GUIDColumnName, TreeProvider);
                                }
                                else
                                {
                                    DocumentHelper.DeleteAttachment(Node, attGuid, TreeProvider);
                                }
                                DocumentHelper.UpdateDocument(Node, TreeProvider);

                                // Ensure full page refresh
                                if (AutoCheck)
                                {
                                    ScriptHelper.RegisterStartupScript(Page, typeof(Page), "deleteRefresh", ScriptHelper.GetScript("InitRefresh_" + ClientID + "('', true, true, '" + attGuid + "', 'delete');"));
                                }
                                else
                                {
                                    string script = "if (window.RefreshTree) { RefreshTree(" + Node.NodeParentID + ", " + Node.NodeID + "); }";
                                    ScriptHelper.RegisterStartupScript(Page, typeof(Page), "refreshTree", ScriptHelper.GetScript(script));
                                }

                                // Check in the document
                                if (AutoCheck)
                                {
                                    if (vm != null)
                                    {
                                        vm.CheckIn(Node, null);
                                    }
                                }

                                // Log synchronization task if not under workflow
                                if (!UsesWorkflow)
                                {
                                    DocumentSynchronizationHelper.LogDocumentChange(Node, TaskTypeEnum.UpdateDocument, TreeProvider);
                                }
                            }
                            else
                            {
                                AttachmentInfoProvider.DeleteTemporaryAttachment(attGuid, SiteContext.CurrentSiteName);
                            }
                        }
                    }

                    LastAction = "delete";
                    Value = Guid.Empty;
                    break;
            }

            // Force reload data
            ReloadData(true);
        }
    }


    /// <summary>
    /// Overloaded ReloadData.
    /// </summary>
    public override void ReloadData()
    {
        ReloadData(false);
    }


    /// <summary>
    /// UniGrid external data bound.
    /// </summary>
    protected object GridDocsOnExternalDataBound(object sender, string sourceName, object parameter)
    {
        string attName;
        string attachmentExt;
        DataRowView drv;

        switch (sourceName.ToLowerCSafe())
        {
            case "update":
                {
                    drv = parameter as DataRowView;
                    PlaceHolder plcUpd = new PlaceHolder();
                    plcUpd.ID = "plcUdateAction";
                    Panel pnlBlock = new Panel();
                    pnlBlock.ID = "pnlBlock";

                    plcUpd.Controls.Add(pnlBlock);

                    // Add update control
                    // Dynamically load uploader control
                    DirectFileUploader dfuElem = Page.LoadUserControl("~/CMSModules/Content/Controls/Attachments/DirectFileUploader/DirectFileUploader.ascx") as DirectFileUploader;

                    // Set uploader's properties
                    if (dfuElem != null)
                    {
                        dfuElem.ID = "dfuElem" + DocumentID;
                        dfuElem.SourceType = MediaSourceEnum.Attachment;
                        dfuElem.DisplayInline = true;

                        if (!createTempAttachment)
                        {
                            dfuElem.AttachmentGUID = ValidationHelper.GetGuid(drv["AttachmentGUID"], Guid.Empty);
                        }

                        dfuElem.ForceLoad = true;
                        dfuElem.FormGUID = FormGUID;
                        dfuElem.AttachmentGUIDColumnName = GUIDColumnName;
                        dfuElem.DocumentID = DocumentID;
                        dfuElem.NodeParentNodeID = NodeParentNodeID;
                        dfuElem.NodeClassName = NodeClassName;
                        dfuElem.ResizeToWidth = ResizeToWidth;
                        dfuElem.ResizeToHeight = ResizeToHeight;
                        dfuElem.ResizeToMaxSideSize = ResizeToMaxSideSize;
                        dfuElem.AllowedExtensions = AllowedExtensions;
                        dfuElem.ShowIconMode = true;
                        dfuElem.InsertMode = false;
                        dfuElem.ParentElemID = ClientID;
                        dfuElem.IncludeNewItemInfo = true;
                        dfuElem.CheckPermissions = CheckPermissions;
                        dfuElem.NodeSiteName = SiteName;
                        dfuElem.IsLiveSite = IsLiveSite;
                        // Setting of the direct single mode
                        dfuElem.UploadMode = MultifileUploaderModeEnum.DirectSingle;
                        dfuElem.MaxNumberToUpload = 1;

                        dfuElem.PreRender += dfuElem_PreRender;
                        pnlBlock.Controls.Add(dfuElem);
                    }

                    int nodeGroupId = ((Node != null) && (Node.DocumentID > 0)) ? Node.GetValue("NodeGroupID", 0) : 0;
                    bool displayGroupAdmin = true;

                    // Check group admin for live site
                    if (IsLiveSite && (nodeGroupId > 0))
                    {
                        displayGroupAdmin = MembershipContext.AuthenticatedUser.IsGroupAdministrator(nodeGroupId);
                    }

                    // Check if external editing allowed by the form
                    bool allowExt = (Form == null) || Form.AllowExternalEditing;

                    if (allowExt && (FormGUID == Guid.Empty) && displayGroupAdmin)
                    {
                        var ctrl = ExternalEditHelper.LoadExternalEditControl(pnlBlock, FileTypeEnum.Attachment, SiteName, new DataRowContainer(drv), IsLiveSite, Node);
                        if (ctrl != null)
                        {
                            ctrl.ID = "extEdit" + DocumentID;

                            // Ensure form identification
                            if ((Form != null) && (Form.Parent != null))
                            {
                                ctrl.FormID = Form.Parent.ClientID;
                            }
                            ctrl.SiteName = SiteName;

                            if (FieldInfo != null)
                            {
                                ctrl.AttachmentFieldName = FieldInfo.Name;
                            }

                            ctrl.PreRender += extEdit_PreRender;

                            // Adjust the styles
                            bool isRTL = (IsLiveSite && CultureHelper.IsPreferredCultureRTL()) || (!IsLiveSite && CultureHelper.IsUICultureRTL());
                            pnlBlock.Style.Add("text-align", isRTL ? "right" : "left");
                        }
                    }

                    return plcUpd;
                }

            case "edit":
                {
                    // Get file extension
                    string extension = ValidationHelper.GetString(((DataRowView)((GridViewRow)parameter).DataItem).Row["AttachmentExtension"], string.Empty).ToLowerCSafe();

                    // Get attachment GUID
                    attachmentGuid = ValidationHelper.GetGuid(((DataRowView)((GridViewRow)parameter).DataItem).Row["AttachmentGUID"], Guid.Empty);
                    if (sender is CMSGridActionButton)
                    {
                        CMSGridActionButton img = (CMSGridActionButton)sender;
                        if (createTempAttachment)
                        {
                            img.Visible = false;
                        }
                        else
                        {
                            img.ScreenReaderDescription = extension;
                            img.ToolTip = attachmentGuid.ToString();
                            img.PreRender += img_PreRender;
                        }
                    }
                }
                break;

            case "delete":
                if (sender is CMSGridActionButton)
                {
                    CMSGridActionButton imgDelete = (CMSGridActionButton)sender;
                    // Turn off validation
                    imgDelete.CausesValidation = false;
                    imgDelete.PreRender += imgDelete_PreRender;
                }
                break;

            case "attachmentname":
                {
                    drv = parameter as DataRowView;

                    // Get attachment GUID
                    attachmentGuid = ValidationHelper.GetGuid(drv["AttachmentGUID"], Guid.Empty);

                    // Get attachment extension
                    attachmentExt = ValidationHelper.GetString(drv["AttachmentExtension"], string.Empty);
                    bool isImage = ImageHelper.IsImage(attachmentExt);
                    bool isTemp = ValidationHelper.GetGuid(drv["AttachmentFormGUID"], Guid.Empty) != Guid.Empty;

                    // Get link for attachment
                    string attachmentUrl = null;
                    attName = ValidationHelper.GetString(drv["AttachmentName"], string.Empty);
                    int documentId = DocumentID;

                    if (documentId > 0)
                    {
                        int versionHistoryId = IsLiveSite ? 0 : VersionHistoryID;
                        attachmentUrl = AuthenticationHelper.ResolveUIUrl(AttachmentURLProvider.GetAttachmentUrl(attachmentGuid, URLHelper.GetSafeFileName(attName, SiteContext.CurrentSiteName), null, versionHistoryId));
                    }
                    else
                    {
                        attachmentUrl = ResolveUrl(DocumentHelper.GetAttachmentUrl(attachmentGuid, 0));
                    }
                    attachmentUrl = URLHelper.UpdateParameterInUrl(attachmentUrl, "chset", Guid.NewGuid().ToString());

                    // Ensure correct URL for non-temporary attachments
                    if ((OriginalNodeSiteName != SiteContext.CurrentSiteName) && !isTemp)
                    {
                        attachmentUrl = URLHelper.AddParameterToUrl(attachmentUrl, "sitename", OriginalNodeSiteName);
                    }

                    // Add latest version requirement for live site
                    if (IsLiveSite && (documentId > 0))
                    {
                        // Add requirement for latest version of files for current document
                        string newparams = "latestfordocid=" + documentId;
                        newparams += "&hash=" + ValidationHelper.GetHashString("d" + documentId);

                        attachmentUrl += "&" + newparams;
                    }

                    // Optionally trim attachment name
                    string attachmentName = TextHelper.LimitLength(attName, ATTACHMENT_NAME_LIMIT);

                    // Tooltip
                    string tooltip = null;
                    if (ShowTooltip)
                    {
                        string title = ValidationHelper.GetString(drv["AttachmentTitle"], string.Empty);
                        string description = ValidationHelper.GetString(drv["AttachmentDescription"], string.Empty);

                        int imageWidth = ValidationHelper.GetInteger(drv["AttachmentImageWidth"], 0);
                        int imageHeight = ValidationHelper.GetInteger(drv["AttachmentImageHeight"], 0);

                        tooltip = UIHelper.GetTooltipAttributes(attachmentUrl, imageWidth, imageHeight, title, attachmentName, attachmentExt, description, null, 300);
                    }

                    // Icon
                    string iconTag = UIHelper.GetFileIcon(Page, attachmentExt, tooltip: attachmentName);

                    if (isImage)
                    {
                        return "<a href=\"#\" onclick=\"javascript: window.open('" + attachmentUrl + "'); return false;\" class=\"cms-icon-link\"><span " + tooltip + ">" + iconTag + attachmentName + "</span></a>";
                    }
                    else
                    {
                        attachmentUrl = URLHelper.AddParameterToUrl(attachmentUrl, "disposition", "attachment");

                        // NOTE: OnClick here is needed to avoid loader to show because even for download links, the pageUnload event is fired
                        return String.Format("<a href=\"{0}\" onclick=\"javascript: {5}\" class=\"cms-icon-link\"><span id=\"{1}\" {2}>{3}{4}</span></a>", attachmentUrl, attachmentGuid, tooltip, iconTag, attachmentName, ScriptHelper.GetDisableProgressScript());
                    }
                }

            case "attachmentsize":
                {
                    long size = ValidationHelper.GetLong(parameter, 0);
                    return DataHelper.GetSizeString(size);
                }
        }

        return parameter;
    }


    #region "Grid actions events"

    protected void extEdit_PreRender(object sender, EventArgs e)
    {
        var extEdit = sender as ExternalEditControl;
        if (extEdit != null)
        {
            extEdit.Enabled = Enabled;
        }
    }


    protected void dfuElem_PreRender(object sender, EventArgs e)
    {
        var dfuElem = (DirectFileUploader)sender;

        dfuElem.ForceLoad = true;
        dfuElem.FormGUID = FormGUID;
        dfuElem.AttachmentGUIDColumnName = GUIDColumnName;
        dfuElem.DocumentID = DocumentID;
        dfuElem.NodeParentNodeID = NodeParentNodeID;
        dfuElem.NodeClassName = NodeClassName;
        dfuElem.ResizeToWidth = ResizeToWidth;
        dfuElem.ResizeToHeight = ResizeToHeight;
        dfuElem.ResizeToMaxSideSize = ResizeToMaxSideSize;
        dfuElem.AllowedExtensions = AllowedExtensions;
        dfuElem.ShowIconMode = true;
        dfuElem.InsertMode = false;
        dfuElem.ParentElemID = ClientID;
        dfuElem.IncludeNewItemInfo = true;
        dfuElem.CheckPermissions = CheckPermissions;
        dfuElem.NodeSiteName = SiteName;
        dfuElem.IsLiveSite = IsLiveSite;

        // Setting of the direct single mode
        dfuElem.UploadMode = MultifileUploaderModeEnum.DirectSingle;
        dfuElem.Width = 16;
        dfuElem.Height = 16;
        dfuElem.MaxNumberToUpload = 1;

        dfuElem.Enabled = Enabled;
    }


    protected void img_PreRender(object sender, EventArgs e)
    {
        var img = (CMSGridActionButton)sender;

        if (AuthenticationHelper.IsAuthenticated())
        {
            if (!Enabled)
            {
                // Disable edit icon
                img.Enabled = false;
            }
            else
            {
                string strForm = (FormGUID == Guid.Empty) ? "" : FormGUID.ToString();

                // Create security hash
                string form = null;
                if (!String.IsNullOrEmpty(strForm))
                {
                    form = "&formguid=" + strForm + "&parentid=" + NodeParentNodeID;
                }
                else if ((Node != null) && (Node.DocumentID > 0))
                {
                    form += "&siteid=" + Node.NodeSiteID;
                }
                string parameters = "?attachmentGUID=" + img.ToolTip + "&refresh=1&versionHistoryID=" + VersionHistoryID + form + "&clientid=" + ClientID;
                string validationHash = QueryHelper.GetHash(parameters);

                string isImage = ImageHelper.IsSupportedByImageEditor(img.ScreenReaderDescription) ? "true" : "false";

                img.OnClientClick = "Edit_" + ClientID + "('" + img.ToolTip + "', '" + strForm + "', '" + VersionHistoryID + "', " + NodeParentNodeID + ", '" + validationHash + "', " + isImage + ");return false;";
            }

            img.ToolTip = GetString("general.edit");
        }
        else
        {
            img.Visible = false;
        }
    }


    private void imgDelete_PreRender(object sender, EventArgs e)
    {
        CMSGridActionButton btnDelete = (CMSGridActionButton)sender;
        if (!Enabled || !AllowDelete)
        {
            // Disable delete icon in case that editing is not allowed
            btnDelete.Enabled = false;
            btnDelete.Style.Add("cursor", "default");
        }
    }

    #endregion


    #endregion
}