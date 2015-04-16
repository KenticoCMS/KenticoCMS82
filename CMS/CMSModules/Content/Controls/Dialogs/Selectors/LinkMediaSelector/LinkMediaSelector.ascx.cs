using System;
using System.Collections;
using System.Data;
using System.Linq;
using System.Web.UI;

using CMS.Base;
using CMS.DataEngine;
using CMS.DocumentEngine;
using CMS.ExtendedControls;
using CMS.Helpers;
using CMS.IO;
using CMS.Localization;
using CMS.Membership;
using CMS.PortalEngine;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.WorkflowEngine;

public partial class CMSModules_Content_Controls_Dialogs_Selectors_LinkMediaSelector_LinkMediaSelector : LinkMediaSelector
{
    #region "Constants"

    private const string NODE_COLUMNS = "ClassDisplayName, AttachmentName, AttachmentTitle, AttachmentDescription, AttachmentExtension, AttachmentImageWidth, AttachmentImageHeight, NodeSiteID, SiteName, NodeGUID, DocumentUrlPath, NodeAlias, NodeAliasPath, AttachmentGUID, AttachmentID, DocumentName, AttachmentSize, NodeClassID, DocumentModifiedWhen, NodeACLID, NodeHasChildren, DocumentCheckedOutVersionHistoryID, NodeOwner, DocumentExtensions, ClassName, DocumentLastVersionName, DocumentType, DocumentLastVersionType, NodeID";


    /// <summary>
    /// Short link to help page regarding media link insertion.
    /// </summary>
    private const string HELP_TOPIC_INSERT_MEDIA_LINK = "media_content_through_editor";


    /// <summary>
    /// Short link to help page regarding link insertion.
    /// </summary>
    private const string HELP_TOPIC_INSERT_LINK_LINK = "links_anchors";

    #endregion


    #region "Private variables"

    // Content variables
    private int mSiteID;
    private int mNodeID;

    private TreeNode mTreeNodeObj;

    #endregion


    #region "Public properties"

    /// <summary>
    /// Messages placeholder
    /// </summary>
    public override MessagesPlaceHolder MessagesPlaceHolder
    {
        get
        {
            return plcMess;
        }
    }


    /// <summary>
    /// Indicates if control is used on live site.
    /// </summary>
    public override bool IsLiveSite
    {
        get
        {
            return base.IsLiveSite;
        }
        set
        {
            plcMess.IsLiveSite = value;
            base.IsLiveSite = value;
        }
    }

    #endregion


    #region "Private properties"

    /// <summary>
    /// Gets or sets last searched value.
    /// </summary>
    private string LastSearchedValue
    {
        get
        {
            return hdnLastSearchedValue.Value;
        }
        set
        {
            hdnLastSearchedValue.Value = value;
        }
    }


    /// <summary>
    /// Gets current action name.
    /// </summary>
    private string CurrentAction
    {
        get
        {
            return hdnAction.Value.ToLowerCSafe().Trim();
        }
        set
        {
            hdnAction.Value = value;
        }
    }


    /// <summary>
    /// Gets current action argument value.
    /// </summary>
    private string CurrentArgument
    {
        get
        {
            return hdnArgument.Value;
        }
    }


    /// <summary>
    /// Returns current properties (according to OutputFormat).
    /// </summary>
    protected override ItemProperties Properties
    {
        get
        {
            switch (Config.OutputFormat)
            {
                case OutputFormatEnum.HTMLMedia:
                    return htmlMediaProp;

                case OutputFormatEnum.HTMLLink:
                    return htmlLinkProp;

                case OutputFormatEnum.BBMedia:
                    return bbMediaProp;

                case OutputFormatEnum.BBLink:
                    return bbLinkProp;

                case OutputFormatEnum.NodeGUID:
                    return nodeGuidProp;

                default:
                    if ((Config.CustomFormatCode == "copy") || (Config.CustomFormatCode == "move") || (Config.CustomFormatCode == "link") || (Config.CustomFormatCode == "linkdoc"))
                    {
                        return docCopyMoveProp;
                    }
                    return urlProp;
            }
        }
    }


    /// <summary>
    /// Update panel where properties control resides.
    /// </summary>
    protected override UpdatePanel PropertiesUpdatePanel
    {
        get
        {
            return pnlUpdateProperties;
        }
    }


    /// <summary>
    /// Gets ID of the site files are being displayed for.
    /// </summary>
    private int SiteID
    {
        get
        {
            if (mSiteID == 0)
            {
                mSiteID = siteSelector.SiteID;
            }
            return mSiteID;
        }
        set
        {
            mSiteID = value;
        }
    }


    /// <summary>
    /// Gets name of the site files are being displayed for.
    /// </summary>
    private string SiteName
    {
        get
        {
            return siteSelector.SiteName;
        }
        set
        {
            siteSelector.SiteName = value;
        }
    }


    /// <summary>
    /// Gets or sets ID of the node selected in the content tree.
    /// </summary>
    private int NodeID
    {
        get
        {
            if (mNodeID == 0)
            {
                mNodeID = ValidationHelper.GetInteger(hdnLastNodeSlected.Value, 0);
            }
            return mNodeID;
        }
        set
        {
            // Set node only if changed
            if (mNodeID != value)
            {
                mNodeID = value;
                hdnLastNodeSlected.Value = value.ToString();
                mTreeNodeObj = null;
            }
        }
    }


    /// <summary>
    /// Indicates whether the attachments are temporary.
    /// </summary>
    private bool AttachmentsAreTemporary
    {
        get
        {
            return ((Config.AttachmentFormGUID != Guid.Empty) && (Config.AttachmentDocumentID == 0));
        }
    }


    /// <summary>
    /// Gets the node attachments are related to.
    /// </summary>
    private TreeNode TreeNodeObj
    {
        get
        {
            if (mTreeNodeObj == null)
            {
                // Content tab
                if (SourceType == MediaSourceEnum.Content)
                {
                    if (NodeID > 0)
                    {
                        TreeProvider tree = new TreeProvider(MembershipContext.AuthenticatedUser)
                                                {
                                                    CombineWithDefaultCulture = true
                                                };
                        mTreeNodeObj = DocumentHelper.GetDocument(NodeID, TreeProvider.ALL_CULTURES, true, tree);
                    }
                }
                // Attachments tab
                else if (!AttachmentsAreTemporary)
                {
                    TreeProvider tree = new TreeProvider(MembershipContext.AuthenticatedUser);
                    mTreeNodeObj = DocumentHelper.GetDocument(Config.AttachmentDocumentID, tree);
                }

                mediaView.TreeNodeObj = mTreeNodeObj;
            }
            return mTreeNodeObj;
        }
        set
        {
            mTreeNodeObj = value;
        }
    }


    /// <summary>
    /// Gets or sets GUID of the recently selected attachment.
    /// </summary>
    private Guid LastAttachmentGuid
    {
        get
        {
            return ValidationHelper.GetGuid(ViewState["LastAttachmentGuid"], Guid.Empty);
        }
        set
        {
            ViewState["LastAttachmentGuid"] = value;
        }
    }


    /// <summary>
    /// Currently selected item.
    /// </summary>
    private AttachmentInfo CurrentAttachmentInfo
    {
        get;
        set;
    }


    /// <summary>
    /// Indicates whether the asynchronous postback occurs on the page.
    /// </summary>
    private bool IsInAsyncPostBack
    {
        get
        {
            var scriptManager = ScriptManager.GetCurrent(Page);
            return scriptManager != null && scriptManager.IsInAsyncPostBack;
        }
    }


    /// <summary>
    /// Indicates whether the post back is result of some hidden action.
    /// </summary>
    private bool IsAction
    {
        get;
        set;
    }


    /// <summary>
    /// Indicates if full listing mode is enabled. This mode enables navigation to child and parent folders/documents from current view.
    /// </summary>
    private bool IsFullListingMode
    {
        get
        {
            return mediaView.IsFullListingMode;
        }
        set
        {
            mediaView.IsFullListingMode = value;
        }
    }


    /// <summary>
    /// Gets or sets selected item to colorize.
    /// </summary>
    private Guid ItemToColorize
    {
        get
        {
            return ValidationHelper.GetGuid(ViewState["ItemToColorize"], Guid.Empty);
        }
        set
        {
            ViewState["ItemToColorize"] = value;
        }
    }


    /// <summary>
    /// Gets or sets ID of the node reflecting new root specified by starting path.
    /// </summary>
    private int StartingPathNodeID
    {
        get
        {
            return ValidationHelper.GetInteger(ViewState["StartingPathNodeID"], 0);
        }
        set
        {
            ViewState["StartingPathNodeID"] = value;
        }
    }


    /// <summary>
    /// Indicates if properties are displayed in full height mode.
    /// </summary>
    private bool IsFullDisplay
    {
        get
        {
            return ValidationHelper.GetBoolean(ViewState["IsFullDisplay"], false);
        }
        set
        {
            ViewState["IsFullDisplay"] = value;
        }
    }


    /// <summary>
    /// Indicates whether the control is displayed as part of the copy/move dialog.
    /// </summary>
    private bool IsCopyMoveLinkDialog
    {
        get
        {
            switch (Config.CustomFormatCode)
            {
                case "copy":
                case "move":
                case "link":
                case "linkdoc":
                case "selectpath":
                case "relationship":
                    return true;

                default:
                    return false;
            }
        }
    }


    /// <summary>
    /// Indicates whether the control output is link.
    /// </summary>
    private bool IsLinkOutput
    {
        get
        {
            return ((Config.OutputFormat == OutputFormatEnum.HTMLLink) || (Config.OutputFormat == OutputFormatEnum.BBLink) || (Config.OutputFormat == OutputFormatEnum.URL));
        }
    }

    #endregion


    #region "Page events"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        menuElem.Config = Config;
        mediaView.Config = Config;

        // Get source type according URL parameters
        SourceType = CMSDialogHelper.GetMediaSource(QueryHelper.GetString("source", "attachments"));
        mediaView.OutputFormat = Config.OutputFormat;

        // All sites for copy, move and link dialog
        siteSelector.OnlyRunningSites = !IsCopyMoveLinkDialog;

        // Prepare help
        SetHelp();
    }


    /// <summary>
    /// Sets context help in dialog depending on current Config.
    /// </summary>
    private void SetHelp()
    {
        if (IsLiveSite)
        {
            return;
        }

        string helpTopic = String.Empty;
        if ((Config.OutputFormat == OutputFormatEnum.URL) || (Config.OutputFormat == OutputFormatEnum.HTMLLink) || (Config.OutputFormat == OutputFormatEnum.BBLink))
        {
            helpTopic = HELP_TOPIC_INSERT_LINK_LINK;
        }
        else if ((Config.OutputFormat == OutputFormatEnum.BBMedia) || (Config.OutputFormat == OutputFormatEnum.HTMLMedia))
        {
            helpTopic = HELP_TOPIC_INSERT_MEDIA_LINK;
        }

        if (!String.IsNullOrEmpty(helpTopic))
        {
            helpTopic = DocumentationHelper.GetDocumentationTopicUrl(helpTopic);
        }

        object options = new
        {
            helpName = "lnkMediaSelectorHelp",
            helpUrl = helpTopic
        };
        ScriptHelper.RegisterModule(this, "CMS/DialogContextHelpChange", options);
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        // High-light item being edited
        if (ItemToColorize != Guid.Empty)
        {
            ColorizeRow(ItemToColorize.ToString());
        }

        if (!URLHelper.IsPostback() && (siteSelector.DropDownSingleSelect.SelectedItem == null))
        {
            EnsureLoadedSite();
        }

        // Handle empty site selector
        HandleSiteEmpty();

        menuElem.ShowParentButton = (menuElem.ShowParentButton && (StartingPathNodeID != NodeID) && IsFullListingMode);
        pnlUpdateMenu.Update();

        // Display info on listing more content
        if (!IsCopyMoveLinkDialog && IsFullListingMode && (TreeNodeObj != null))
        {
            string closeLink = String.Format("<span class=\"ListingClose\" style=\"cursor: pointer;\" onclick=\"SetAction('closelisting', ''); RaiseHiddenPostBack(); return false;\">{0}</span>", GetString("general.close"));
            string docNamePath = String.Format("<span class=\"ListingPath\">{0}</span>", HTMLHelper.HTMLEncode(TreeNodeObj.DocumentNamePath));

            string listingMsg = string.Format(GetString("dialogs.content.listingInfo"), docNamePath, closeLink);
            mediaView.DisplayListingInfo(listingMsg);
        }

        if (IsCopyMoveLinkDialog)
        {
            DisplayMediaElements();
        }
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        if (!StopProcessing)
        {
            CheckPermissions();

            SetupProperties();

            InitializeDialogs();

            SetupControls();

            mediaView.Reload();

            EnsureLoadedData();
        }
        else
        {
            siteSelector.StopProcessing = true;
            Visible = false;
        }

        ScriptHelper.RegisterWOpenerScript(Page);
    }

    #endregion


    #region "Inherited methods"

    /// <summary>
    /// Initializes its properties according to the URL parameters.
    /// </summary>
    public void InitFromQueryString()
    {
        switch (Config.OutputFormat)
        {
            case OutputFormatEnum.HTMLMedia:
                SelectableContent = SelectableContentEnum.OnlyMedia;
                break;

            case OutputFormatEnum.HTMLLink:
                SelectableContent = SelectableContentEnum.AllContent;
                break;

            case OutputFormatEnum.BBMedia:
                SelectableContent = SelectableContentEnum.OnlyImages;
                break;

            case OutputFormatEnum.BBLink:
                SelectableContent = SelectableContentEnum.AllContent;
                break;

            case OutputFormatEnum.URL:
            case OutputFormatEnum.NodeGUID:
                string content = QueryHelper.GetString("content", "");
                SelectableContent = CMSDialogHelper.GetSelectableContent(content);
                break;
        }
    }


    /// <summary>
    /// Returns selected item parameters as name-value collection.
    /// </summary>
    public void GetSelectedItem()
    {
        // Clear unused information from session
        ClearSelectedItemInfo();
        ClearActionElems();
        if (Properties.Validate())
        {
            // Store tab information in the user's dialogs configuration
            StoreDialogsConfiguration();

            // Get selected item information
            Hashtable props = Properties.GetItemProperties();

            // Get JavaScript for inserting the item
            string script = GetInsertItem(props);
            if (!string.IsNullOrEmpty(script))
            {
                ScriptHelper.RegisterStartupScript(Page, typeof(Page), "insertItemScript", ScriptHelper.GetScript(script));
            }
            if ((Config.CustomFormatCode.ToLowerCSafe() == "copy") || (Config.CustomFormatCode.ToLowerCSafe() == "move") || (Config.CustomFormatCode.ToLowerCSafe() == "link") || (Config.CustomFormatCode.ToLowerCSafe() == "linkdoc"))
            {
                // Reload the iframe
                pnlUpdateProperties.Update();
            }
        }
        else
        {
            // Display error message
            pnlUpdateProperties.Update();
        }
    }

    #endregion


    #region "Dialog configuration"

    /// <summary>
    /// Stores current tab's configuration for the user.
    /// </summary>
    private void StoreDialogsConfiguration()
    {
        UserInfo ui = UserInfoProvider.GetUserInfo(MembershipContext.AuthenticatedUser.UserID);
        if (ui != null)
        {
            // Store configuration based on the current tab
            switch (SourceType)
            {
                case MediaSourceEnum.Content:
                    // Actualize configuration
                    ui.UserSettings.UserDialogsConfiguration["content.sitename"] = SiteName;
                    ui.UserSettings.UserDialogsConfiguration["content.path"] = GetContentPath(NodeID);
                    ui.UserSettings.UserDialogsConfiguration["content.viewmode"] = CMSDialogHelper.GetDialogViewMode(menuElem.SelectedViewMode);
                    break;

                case MediaSourceEnum.DocumentAttachments:
                    // Actualize configuration
                    ui.UserSettings.UserDialogsConfiguration["attachments.viewmode"] = CMSDialogHelper.GetDialogViewMode(menuElem.SelectedViewMode);
                    break;
            }
            ui.UserSettings.UserDialogsConfiguration["selectedtab"] = CMSDialogHelper.GetMediaSource(SourceType);

            UserInfoProvider.SetUserInfo(ui);
        }
    }


    /// <summary>
    /// Initializes dialogs according URL configuration, selected item or user configuration.
    /// </summary>
    private void InitializeDialogs()
    {
        if (!URLHelper.IsPostback())
        {
            LoadDialogConfiguration();

            // Item is selected in the editor
            if (MediaSource != null)
            {
                LoadItemConfiguration();
            }
            else if (!IsCopyMoveLinkDialog)
            {
                LoadUserConfiguration();
            }
        }
    }


    /// <summary>
    /// Loads dialogs according configuration coming from the URL.
    /// </summary>
    private void LoadDialogConfiguration()
    {
        if (SourceType == MediaSourceEnum.Content)
        {
            // Initialize site selector
            siteSelector.StopProcessing = false;
            string siteWhereCondition = GetSiteWhere();
            siteSelector.UniSelector.WhereCondition = siteWhereCondition;
            contentTree.Culture = Config.Culture;

            if (!string.IsNullOrEmpty(Config.ContentSelectedSite))
            {
                contentTree.SiteName = Config.ContentSelectedSite;
                siteSelector.SiteName = Config.ContentSelectedSite;
            }
            else
            {
                // Select default site
                string siteName = SiteContext.CurrentSiteName;
                // Try select current site
                if (!string.IsNullOrEmpty(siteName))
                {
                    contentTree.SiteName = siteName;
                    siteSelector.SiteName = siteName;
                }
                else
                {
                    // Select first site from users sites
                    DataSet ds = SiteInfoProvider.GetSites()
                        .Where(siteWhereCondition)
                        .OrderBy("SiteDisplayName");

                    if (!DataHelper.DataSourceIsEmpty(ds))
                    {
                        siteName = ValidationHelper.GetString(ds.Tables[0].Rows[0]["SiteName"], String.Empty);
                        if (!String.IsNullOrEmpty(siteName))
                        {
                            contentTree.SiteName = siteName;
                            siteSelector.SiteName = siteName;
                        }
                    }
                }
            }

            pnlUpdateSelectors.Update();
        }
    }


    /// <summary>
    /// Gets WHERE condition for available sites according field configuration.
    /// </summary>
    private string GetSiteWhere()
    {
        // First check configuration
        WhereCondition condition = new WhereCondition();

        if (!IsCopyMoveLinkDialog)
        {
            condition.WhereEquals("SiteStatus", "RUNNING");
        }

        switch (Config.ContentSites)
        {
            case AvailableSitesEnum.OnlySingleSite:
                condition.WhereEquals("SiteName", Config.ContentSelectedSite);
                break;

            case AvailableSitesEnum.OnlyCurrentSite:
                condition.WhereEquals("SiteName", SiteContext.CurrentSiteName);
                break;
        }

        // Get only current user's sites
        if (!MembershipContext.AuthenticatedUser.IsGlobalAdministrator)
        {
            condition.WhereIn("SiteID", new IDQuery<UserSiteInfo>(UserSiteInfo.TYPEINFO.SiteIDColumn)
                                                    .WhereEquals("UserID", MembershipContext.AuthenticatedUser.UserID));
        }
        return condition.ToString(true);
    }


    /// <summary>
    /// Loads selected item parameters into the selector.
    /// </summary>
    public void LoadItemConfiguration()
    {
        if (MediaSource != null)
        {
            IsItemLoaded = true;

            switch (MediaSource.SourceType)
            {
                case MediaSourceEnum.Content:
                    siteSelector.SiteID = MediaSource.SiteID;
                    contentTree.SiteName = SiteName;

                    // Try to select node in the tree
                    if (MediaSource.NodeID > 0)
                    {
                        NodeID = MediaSource.NodeID;
                        contentTree.SelectedNodeID = NodeID;
                    }
                    break;
            }

            // Reload HTML properties
            if (Config.OutputFormat == OutputFormatEnum.HTMLMedia)
            {
                // Force media properties control to load selected item
                htmlMediaProp.ViewMode = MediaSource.MediaType;
            }

            // Display properties in full size only when output format is not HTML link
            if ((SourceType == MediaSourceEnum.Content) && (Config.OutputFormat != OutputFormatEnum.HTMLLink))
            {
                DisplayFull();
            }

            htmlMediaProp.HistoryID = MediaSource.HistoryID;


            // Load properties
            Properties.LoadItemProperties(Parameters);
            pnlUpdateProperties.Update();

            // Remember item to colorize
            switch (SourceType)
            {
                case MediaSourceEnum.MetaFile:
                    LastAttachmentGuid = MediaSource.MetaFileGuid;
                    ColorizeRow(MediaSource.MetaFileID.ToString());
                    break;
                default:
                    ItemToColorize = (SourceType == MediaSourceEnum.Content) ? MediaSource.NodeGuid : MediaSource.AttachmentGuid;
                    LastAttachmentGuid = ItemToColorize;
                    break;
            }
        }

        ClearSelectedItemInfo();
    }


    /// <summary>
    /// Loads dialogs according user's configuration.
    /// </summary>
    private void LoadUserConfiguration()
    {
        var currentUser = MembershipContext.AuthenticatedUser;
        if ((currentUser.UserSettings.UserDialogsConfiguration != null) &&
            (currentUser.UserSettings.UserDialogsConfiguration.ColumnNames != null))
        {
            XmlData dialogConfig = currentUser.UserSettings.UserDialogsConfiguration;

            string siteName = "";
            string aliasPath = "";
            string viewMode = "";

            // Store configuration based on the current tab
            switch (SourceType)
            {
                case MediaSourceEnum.Content:
                    siteName = (dialogConfig.ContainsColumn("content.sitename") ? (string)dialogConfig["content.sitename"] : "");
                    aliasPath = (dialogConfig.ContainsColumn("content.path") ? (string)dialogConfig["content.path"] : "");
                    viewMode = (dialogConfig.ContainsColumn("content.viewmode") ? (string)dialogConfig["content.viewmode"] : "");
                    break;

                case MediaSourceEnum.DocumentAttachments:
                    viewMode = (dialogConfig.ContainsColumn("attachments.viewmode") ? (string)dialogConfig["attachments.viewmode"] : "");
                    break;
            }

            // Update dialog configuration (only if ContentSelectedSite is not set)
            if (!string.IsNullOrEmpty(siteName) && string.IsNullOrEmpty(Config.ContentSelectedSite))
            {
                // Check if site from user settings exists and is running
                SiteInfo si = SiteInfoProvider.GetSiteInfo(siteName);
                if ((si == null) || (si.Status == SiteStatusEnum.Stopped) || !currentUser.IsInSite(siteName))
                {
                    // If not, use current site
                    siteName = SiteContext.CurrentSiteName;
                }

                siteSelector.SiteName = siteName;
            }

            if (!string.IsNullOrEmpty(aliasPath))
            {
                NodeID = (aliasPath.StartsWithCSafe(Config.ContentStartingPath.ToLowerCSafe()) ? GetContentNodeId(aliasPath) : GetContentNodeId(Config.ContentStartingPath));

                // Initialize root node
                if (NodeID == 0)
                {
                    NodeID = GetContentNodeId("/");
                    menuElem.ShowParentButton = false;
                }

                // Select and expand node
                contentTree.SelectedNodeID = NodeID;
                contentTree.ExpandNodeID = NodeID;
            }
            else if (SourceType == MediaSourceEnum.Content)
            {
                SelectRootNode();
            }

            if (!string.IsNullOrEmpty(viewMode))
            {
                menuElem.SelectedViewMode = CMSDialogHelper.GetDialogViewMode(viewMode);
            }
        }
        else
        {
            // Initialize site selector
            if (!string.IsNullOrEmpty(Config.ContentSelectedSite))
            {
                siteSelector.SiteName = Config.ContentSelectedSite;
            }
            else
            {
                siteSelector.SiteID = SiteContext.CurrentSiteID;
            }
            SelectRootNode();
        }
    }

    #endregion


    #region "Private methods"

    /// <summary>
    /// Performs necessary permissions check.
    /// </summary>
    private void CheckPermissions()
    {
        // Check 'READ' permission for the specific document if attachments are being created
        if ((SourceType == MediaSourceEnum.DocumentAttachments) && (!AttachmentsAreTemporary))
        {
            if (MembershipContext.AuthenticatedUser.IsAuthorizedPerDocument(TreeNodeObj, NodePermissionsEnum.Read) != AuthorizationResultEnum.Allowed)
            {
                string errMsg = string.Format(GetString("cmsdesk.notauthorizedtoreaddocument"), TreeNodeObj.GetDocumentName());

                // Redirect to access denied page
                AccessDenied(errMsg);
            }
        }
    }


    /// <summary>
    /// Ensures that required data are displayed.
    /// </summary>
    private void EnsureLoadedData()
    {
        bool processLoad = true;
        bool isLink = (Config.OutputFormat == OutputFormatEnum.BBLink || Config.OutputFormat == OutputFormatEnum.HTMLLink) ||
                      (Config.OutputFormat == OutputFormatEnum.URL && SelectableContent == SelectableContentEnum.AllContent);

        // If all content is selectable do not select root by default - leave selection empty
        if ((SelectableContent == SelectableContentEnum.AllContent) && !isLink && !IsCopyMoveLinkDialog && !URLHelper.IsPostback())
        {
            // Even no file is selected by default load source for the Attachment tab
            processLoad = (SourceType == MediaSourceEnum.DocumentAttachments);

            NodeID = 0;
            contentTree.SelectedNodeID = NodeID;
            Properties.ClearProperties(true);
        }

        // Clear properties if link dialog is opened and no link is edited
        if (!URLHelper.IsPostback() && isLink && !IsItemLoaded)
        {
            Properties.ClearProperties(true);
        }

        // If no action takes place
        if ((CurrentAction == "") ||
            !(isLink && CurrentAction.Contains("edit")))
        {
            if (processLoad && URLHelper.IsPostback())
            {
                LoadDataSource();
            }

            // Select folder coming from user/selected item configuration
            if (!URLHelper.IsPostback() && processLoad)
            {
                HandleFolderAction(NodeID.ToString(), false, false);

                // Handle preselecting only for content links (from content tree)
                if ((SourceType == MediaSourceEnum.Content) && (IsCopyMoveLinkDialog || IsLinkOutput) && !IsItemLoaded)
                {
                    HandleDialogSelect();
                }
            }
        }
    }


    /// <summary>
    /// Ensures that loaded site is really the one selected in the drop-down list.
    /// </summary>
    private void EnsureLoadedSite()
    {
        if (SourceType != MediaSourceEnum.DocumentAttachments)
        {
            siteSelector.Reload(true);

            // Name of the site selected in the site DDL
            string siteName = "";

            int siteId = siteSelector.SiteID;
            if (siteId > 0)
            {
                SiteInfo si = SiteInfoProvider.GetSiteInfo(siteId);
                if (si != null)
                {
                    siteName = si.SiteName;
                }
            }

            if (siteName != SiteName)
            {
                SiteName = siteName;

                // Get site root by default
                TreeProvider tree = new TreeProvider(MembershipContext.AuthenticatedUser);
                TreeNodeObj = tree.SelectSingleNode(SiteName, "/", null, false, "cms.root", null, null, 1, true, TreeProvider.SELECTNODES_REQUIRED_COLUMNS);
                NodeID = TreeNodeObj.NodeID;

                InitializeContentTree();

                contentTree.SelectedNodeID = NodeID;
                contentTree.ExpandNodeID = NodeID;

                EnsureLoadedData();
            }
        }
    }


    /// <summary>
    /// Makes sure that media elements aren't active while no folder is selected.
    /// </summary>
    private void DisplayMediaElements()
    {
        if (!IsAction)
        {
            if (!mediaView.Visible)
            {
                mediaView.Visible = true;
            }

            EnsureLoadedData();
            mediaView.Reload();
        }
    }


    /// <summary>
    /// Initializes properties controls.
    /// </summary>
    private void SetupProperties()
    {
        htmlLinkProp.Visible = false;
        htmlMediaProp.Visible = false;
        bbLinkProp.Visible = false;
        bbMediaProp.Visible = false;
        urlProp.Visible = false;
        docCopyMoveProp.Visible = false;

        Properties.Visible = true;

        htmlLinkProp.StopProcessing = !htmlLinkProp.Visible;
        htmlMediaProp.StopProcessing = !htmlMediaProp.Visible;
        bbLinkProp.StopProcessing = !bbLinkProp.Visible;
        bbMediaProp.StopProcessing = !bbMediaProp.Visible;
        urlProp.StopProcessing = !urlProp.Visible;
        nodeGuidProp.StopProcessing = !nodeGuidProp.Visible;
        docCopyMoveProp.StopProcessing = !docCopyMoveProp.Visible;

        Properties.Config = Config;
    }


    /// <summary>
    /// Initializes additional controls.
    /// </summary>
    private void SetupControls()
    {
        // Generate permanent URLs whenever node GUID output required        
        if (Config.OutputFormat != OutputFormatEnum.NodeGUID)
        {
            UsePermanentUrls = DocumentURLProvider.UsePermanentUrls(SiteContext.CurrentSiteName);
        }
        else
        {
            // Filter sites only for users without global administrator privilege
            var currentUser = MembershipContext.AuthenticatedUser;
            siteSelector.UserId = currentUser.IsGlobalAdministrator ? 0 : MembershipContext.AuthenticatedUser.UserID;
            
            // Select current site and disable change
            siteSelector.SiteID = SiteID = SiteContext.CurrentSiteID;
            siteSelector.Enabled = false;
        }

        if (SourceType != MediaSourceEnum.DocumentAttachments)
        {
            siteSelector.DropDownSingleSelect.AutoPostBack = true;
            siteSelector.UniSelector.OnSelectionChanged += UniSelector_OnSelectionChanged;
            siteSelector.AdditionalDropDownCSSClass = "DialogSiteDropdown";
        }
        else
        {
            siteSelector.StopProcessing = true;
            pnlUpdateSelectors.Visible = false;
        }

        mediaView.UsePermanentUrls = UsePermanentUrls;
        // Set editor client id for properties
        Properties.EditorClientID = Config.EditorClientID;

        InitializeMenuElement();
        InitializeDesignScripts();

        mediaView.IsLiveSite = IsLiveSite;
        mediaView.SelectableContent = SelectableContent;
        mediaView.SourceType = SourceType;
        mediaView.ViewMode = menuElem.SelectedViewMode;
        mediaView.ResizeToHeight = Config.ResizeToHeight;
        mediaView.ResizeToMaxSideSize = Config.ResizeToMaxSideSize;
        mediaView.ResizeToWidth = Config.ResizeToWidth;
        mediaView.AttachmentNodeParentID = Config.AttachmentParentID;
        mediaView.ListReloadRequired += mediaView_ListReloadRequired;
        mediaView.ListViewControl.OnBeforeSorting += ListViewControl_OnBeforeSorting;

        Properties.IsLiveSite = IsLiveSite;
        Properties.SourceType = SourceType;

        if (!IsInAsyncPostBack)
        {
            // Initialize scripts
            InitializeControlScripts();
            if (URLHelper.IsPostback() && (SourceType != MediaSourceEnum.DocumentAttachments))
            {
                UniSelector_OnSelectionChanged(this, null);
            }
        }

        // Based on the required source type perform setting of necessary controls
        if (SourceType == MediaSourceEnum.Content)
        {
            if (!IsInAsyncPostBack)
            {
                // Initialize content tree control
                InitializeContentTree();
            }
            else
            {
                contentTree.Visible = false;
                contentTree.StopProcessing = true;
            }
        }
        else
        {
            // Hide and disable content related controls
            HideContentControls();
        }

        // If folder was changed reset current page index for control displaying content
        switch (CurrentAction)
        {
            case "morecontentselect":
            case "contentselect":
            case "parentselect":
                ResetPageIndex();
                break;
        }
    }

    protected void ListViewControl_OnBeforeSorting(object sender, EventArgs e)
    {
        // Reload data for new sorting
        LoadDataSource();
        mediaView.Reload();
    }


    /// <summary>
    /// Initialize design jQuery scripts.
    /// </summary>
    private void InitializeDesignScripts()
    {
        ScriptHelper.RegisterStartupScript(Page, typeof(Page), "designScript", ScriptHelper.GetScript(@"
setTimeout('InitializeDesign();',200);
$cmsj(window).unbind('resize').resize(function() { 
    InitializeDesign(); 
});"));
    }


    /// <summary>
    /// Initializes menu element.
    /// </summary>
    private void InitializeMenuElement()
    {
        // Let child controls now what the source type is
        menuElem.IsCopyMoveLinkDialog = IsCopyMoveLinkDialog;
        menuElem.DisplayMode = DisplayMode;
        menuElem.IsLiveSite = IsLiveSite;
        menuElem.SourceType = SourceType;
        menuElem.ResizeToHeight = Config.ResizeToHeight;
        menuElem.ResizeToMaxSideSize = Config.ResizeToMaxSideSize;
        menuElem.ResizeToWidth = Config.ResizeToWidth;

        // Based on the required source type perform setting of necessary controls
        if (SourceType == MediaSourceEnum.Content)
        {
            menuElem.NodeID = NodeID;
        }
        else
        {
            // Initialize menu element for attachments
            menuElem.DocumentID = Config.AttachmentDocumentID;
            menuElem.FormGUID = Config.AttachmentFormGUID;
            menuElem.ParentNodeID = Config.AttachmentParentID;
            menuElem.MetaFileObjectID = Config.MetaFileObjectID;
            menuElem.MetaFileObjectType = Config.MetaFileObjectType;
            menuElem.MetaFileCategory = Config.MetaFileCategory;
        }
        menuElem.UpdateViewMenu();
    }


    /// <summary>
    /// Initializes all the script required for communication between controls.
    /// </summary>
    private void InitializeControlScripts()
    {
        // Prepare for upload
        string refreshType = CMSDialogHelper.GetMediaSource(SourceType);
        string cmdName;
        switch (SourceType)
        {
            case MediaSourceEnum.DocumentAttachments:
                cmdName = "attachment";
                break;
            case MediaSourceEnum.MetaFile:
                cmdName = "metafile";
                break;
            default:
                cmdName = "content";
                break;
        }

        ltlScript.Text = ScriptHelper.GetScript(String.Format(@"
function SetAction(action, argument) {{
    var hdnAction = document.getElementById('{0}');
    var hdnArgument = document.getElementById('{1}');
    if ((hdnAction != null) && (hdnArgument != null)) {{
        if (action != null) {{
            hdnAction.value = action;
        }}
        if (argument != null) {{
            hdnArgument.value = argument;
        }}
    }}
}}
function InitRefresh_{2}(message, fullRefresh, refreshTree, itemInfo, action) {{
    if((message != null) && (message != ''))
    {{
        window.alert(message);
    }}
    else
    {{
        if(action == 'insert')
        {{
            SetAction('{3}created', itemInfo);
        }}
        else if(action == 'update')
        {{
            SetAction('{3}updated', itemInfo);
        }}
        else if(action == 'refresh')
        {{
            SetAction('{3}edit', itemInfo);
        }}
        RaiseHiddenPostBack();
    }}
}}
function imageEdit_AttachmentRefresh(arg){{
    SetAction('attachmentedit', arg);
    RaiseHiddenPostBack();
}}
function imageEdit_ContentRefresh(arg){{
    SetAction('contentedit', arg);
    RaiseHiddenPostBack();
}}
function RaiseHiddenPostBack(){{
    {4};
}}
", hdnAction.ClientID, hdnArgument.ClientID, refreshType, cmdName, ControlsHelper.GetPostBackEventReference(hdnButton, "")));
    }


    /// <summary>
    /// Loads all files for the view control.
    /// </summary>
    private void LoadDataSource()
    {
        switch (SourceType)
        {
            case MediaSourceEnum.Content:
                LoadContentDataSource(LastSearchedValue);
                break;

            case MediaSourceEnum.MetaFile:
                LoadMetaFileDataSource(LastSearchedValue);
                break;

            default:
                LoadAttachmentsDataSource(LastSearchedValue);
                break;
        }
    }


    private void mediaView_ListReloadRequired()
    {
        LoadDataSource();
        mediaView.Reload();
    }


    /// <summary>
    /// Performs actions necessary to select particular item from a list.
    /// </summary>
    private void SelectMediaItem(string argument)
    {
        if (string.IsNullOrEmpty(argument))
        {
            return;
        }

        Hashtable argTable = CMSModules_Content_Controls_Dialogs_Selectors_LinkMediaSelector_MediaView.GetArgumentsTable(argument);
        if (argTable.Count < 2)
        {
            return;
        }

        bool isMetaFile = (SourceType == MediaSourceEnum.MetaFile);
        string ext = (!isMetaFile ? argTable["attachmentextension"].ToString() : argTable["metafileextension"].ToString());

        // Check if selected file from tree is allowed to be selected for URL
        if (Config.OutputFormat == OutputFormatEnum.URL)
        {
            if ((SelectableContent == SelectableContentEnum.OnlyImages) && !ImageHelper.IsImage(ext))
            {
                return;
            }

            if ((SelectableContent == SelectableContentEnum.OnlyFlash) && !MediaHelper.IsFlash(ext))
            {
                return;
            }

            if (SelectableContent == SelectableContentEnum.OnlyMedia)
            {
                var isMedia = ImageHelper.IsImage(ext) || MediaHelper.IsAudio(ext) || MediaHelper.IsAudioVideo(ext) || MediaHelper.IsVideo(ext) || MediaHelper.IsFlash(ext);
                if (!isMedia)
                {
                    return;
                }
            }
        }

        // Get information from argument
        string name = argTable["name"].ToString();
        int imageWidth = (!isMetaFile ? ValidationHelper.GetInteger(argTable["attachmentimagewidth"], 0) : ValidationHelper.GetInteger(argTable["metafileimagewidth"], 0));
        int imageHeight = (!isMetaFile ? ValidationHelper.GetInteger(argTable["attachmentimageheight"], 0) : ValidationHelper.GetInteger(argTable["metafileimageheight"], 0));
        int nodeID = ValidationHelper.GetInteger(argTable["nodeid"], NodeID);
        long size = (!isMetaFile ? ValidationHelper.GetLong(argTable["attachmentsize"], 0) : ValidationHelper.GetInteger(argTable["metafilesize"], 0));
        string url = argTable["url"].ToString();
        string aliasPath = null;

        // Do not update properties when selecting recently edited image item
        bool avoidPropUpdate = false;

        // Remember last selected attachment GUID
        Guid attGuid;
        switch (SourceType)
        {
            case MediaSourceEnum.DocumentAttachments:
                attGuid = ValidationHelper.GetGuid(argTable["attachmentguid"], Guid.Empty);

                avoidPropUpdate = (LastAttachmentGuid == attGuid);

                LastAttachmentGuid = attGuid;
                ItemToColorize = LastAttachmentGuid;
                break;
            case MediaSourceEnum.MetaFile:
                LastAttachmentGuid = ValidationHelper.GetGuid(argTable["metafileguid"], Guid.Empty);
                break;
            default:
                aliasPath = argTable["nodealiaspath"].ToString();
                attGuid = ValidationHelper.GetGuid(argTable["attachmentguid"], Guid.Empty);
                Properties.SiteDomainName = mediaView.SiteObj.DomainName;

                avoidPropUpdate = (ItemToColorize == attGuid);

                ItemToColorize = attGuid;
                if (ItemToColorize == Guid.Empty)
                {
                    ItemToColorize = ValidationHelper.GetGuid(argTable["nodeguid"], Guid.Empty);
                }
                break;
        }

        avoidPropUpdate = (avoidPropUpdate && IsEditImage);
        if (avoidPropUpdate)
        {
            return;
        }

        if (SourceType == MediaSourceEnum.DocumentAttachments)
        {
            int versionHistoryId = 0;

            if (TreeNodeObj != null)
            {
                // Get the node workflow
                WorkflowManager wm = WorkflowManager.GetInstance(TreeNodeObj.TreeProvider);
                WorkflowInfo wi = wm.GetNodeWorkflow(TreeNodeObj);
                if (wi != null)
                {
                    // Get the document version
                    versionHistoryId = TreeNodeObj.DocumentCheckedOutVersionHistoryID;
                }
            }

            MediaItem item = InitializeMediaItem(name, ext, imageWidth, imageHeight, size, url, null, versionHistoryId, nodeID, aliasPath);

            SelectMediaItem(item);
        }
        else
        {
            // Select item
            SelectMediaItem(name, ext, imageWidth, imageHeight, size, url, null, nodeID, aliasPath);
        }
    }


    /// <summary>
    /// Performs actions necessary to select particular item from a list.
    /// </summary>
    private void SelectMediaItem(string docName, string url, string aliasPath)
    {
        SelectMediaItem(docName, null, 0, 0, 0, url, null, NodeID, aliasPath);
    }


    /// <summary>
    /// Selects root node of currently selected site.
    /// </summary>
    private void SelectRootNode()
    {
        // Reset selected node to root node
        NodeID = GetContentNodeId("/");
        contentTree.SelectedNodeID = NodeID;
        contentTree.ExpandNodeID = NodeID;
        menuElem.ShowParentButton = false;
    }


    /// <summary>
    /// Clears hidden control elements fo future use.
    /// </summary>
    private void ClearActionElems()
    {
        CurrentAction = "";
        hdnArgument.Value = "";
    }


    /// <summary>
    /// Displays properties in full size.
    /// </summary>
    private void DisplayFull()
    {
        if (divDialogView.Attributes["class"].StartsWithCSafe("DialogViewContent", true))
        {
            divDialogView.Attributes["class"] = "DialogElementHidden";
            divDialogResizer.Attributes["class"] = "DialogElementHidden";
            divDialogProperties.Attributes["class"] = "DialogPropertiesFullSize";

            if (IsFullDisplay)
            {
                pnlUpdateView.Update();
                pnlUpdateProperties.Update();
            }
            else
            {
                pnlUpdateContent.Update();
                IsFullDisplay = true;
            }
        }
    }


    /// <summary>
    /// Displays properties in default size.
    /// </summary>
    private void DisplayNormal()
    {
        if (divDialogView.Attributes["class"].EqualsCSafe("DialogElementHidden", true))
        {
            divDialogView.Attributes["class"] = "DialogViewContent scroll-area";
            divDialogResizer.Attributes["class"] = "DialogResizerVLine";
            divDialogProperties.Attributes["class"] = "DialogProperties";

            if (IsFullDisplay)
            {
                pnlUpdateContent.Update();
                IsFullDisplay = false;
            }
            else
            {
                pnlUpdateView.Update();
                pnlUpdateProperties.Update();
            }
        }
    }


    /// <summary>
    /// Ensures that filter is no more applied.
    /// </summary>
    private void ResetSearchFilter()
    {
        mediaView.ResetSearch();
        LastSearchedValue = "";
    }


    /// <summary>
    /// Ensures first page is displayed in the control displaying the content.
    /// </summary>
    private void ResetPageIndex()
    {
        mediaView.ResetPageIndex();
    }

    #endregion


    #region "Content methods"

    /// <summary>
    /// Hides and disables content related controls.
    /// </summary>
    private void HideContentControls()
    {
        pnlLeftContent.Visible = false;
        plcSeparator.Visible = false;
        pnlRightContent.CssClass = "DialogCompleteBlock";
        siteSelector.StopProcessing = true;
        contentTree.StopProcessing = true;
    }


    /// <summary>
    /// Initializes content tree element.
    /// </summary>
    private void InitializeContentTree()
    {
        contentTree.Visible = true;

        contentTree.DeniedNodePostback = false;
        contentTree.NodeTextTemplate = "<span class=\"ContentTreeItem\" onclick=\"SelectNode(##NODEID##, this); SetAction('contentselect', '##NODEID##'); RaiseHiddenPostBack(); return false;\">##ICON##<span class=\"Name\">##NODENAME##</span></span>";
        contentTree.SelectedNodeTextTemplate = "<span id=\"treeSelectedNode\" class=\"ContentTreeSelectedItem\" onclick=\"SelectNode(##NODEID##, this); SetAction('contentselect', '##NODEID##'); RaiseHiddenPostBack(); return false;\">##ICON##<span class=\"Name\">##NODENAME##</span></span>";
        contentTree.MaxTreeNodeText = String.Format("<span class=\"ContentTreeItem\" onclick=\"SetAction('morecontentselect', ##PARENTNODEID##); RaiseHiddenPostBack(); return false;\"><span class=\"Name\">{0}</span></span>", GetString("general.SeeListing"));
        contentTree.IsLiveSite = IsLiveSite;
        contentTree.SelectOnlyPublished = IsLiveSite;
        contentTree.SelectPublishedData = IsLiveSite;

        contentTree.SiteName = SiteName;

        // Starting path node ID
        StartingPathNodeID = GetStartNodeId();

        // Select root node for first request
        if (!RequestHelper.IsPostBack() && (NodeID == 0))
        {
            NodeID = StartingPathNodeID;
        }
    }


    /// <summary>
    /// Returns ID of the starting node according current starting path settings.
    /// </summary>
    private int GetStartNodeId()
    {
        if (!string.IsNullOrEmpty(Config.ContentStartingPath))
        {
            contentTree.Path = Config.ContentStartingPath;
        }
        else if (!string.IsNullOrEmpty(MembershipContext.AuthenticatedUser.UserStartingAliasPath))
        {
            contentTree.Path = MembershipContext.AuthenticatedUser.UserStartingAliasPath;
        }
        return GetContentNodeId(contentTree.Path);
    }


    /// <summary>
    /// Returns path of the node specified by its ID.
    /// </summary>
    /// <param name="nodeId">ID of the node</param>
    private static string GetContentPath(int nodeId)
    {
        if (nodeId > 0)
        {
            TreeProvider tree = new TreeProvider(MembershipContext.AuthenticatedUser);

            // Get node and return its alias path
            using (TreeNode node = tree.SelectSingleNode(nodeId))
            {
                if (node != null)
                {
                    return (!node.NodeHasChildren ? TreePathUtils.GetParentPath(node.NodeAliasPath) : node.NodeAliasPath).ToLowerCSafe();
                }
            }
        }

        return string.Empty;
    }


    /// <summary>
    /// Returns ID of the content node specified by its alias path.
    /// </summary>
    /// <param name="aliasPath">Alias path of the node</param>
    private int GetContentNodeId(string aliasPath)
    {
        if (!string.IsNullOrEmpty(aliasPath))
        {
            TreeProvider tree = new TreeProvider(MembershipContext.AuthenticatedUser);
            using (TreeNode node = tree.SelectSingleNode(SiteName, aliasPath, Config.Culture))
            {
                if (node != null)
                {
                    // Return node's ID
                    return node.NodeID;
                }
            }
        }

        return 0;
    }


    /// <summary>
    /// Applies loaded nodes to the view control.
    /// </summary>
    /// <param name="nodes">Nodes to load</param>
    private void LoadNodes(DataSet nodes)
    {
        bool originalNotEmpty = !DataHelper.DataSourceIsEmpty(nodes);
        if (!DataHelper.DataSourceIsEmpty(nodes))
        {
            mediaView.DataSource = nodes;
        }
        else if (originalNotEmpty && IsLiveSite)
        {
            mediaView.InfoText = GetString("dialogs.document.NotAuthorizedToViewAny");
        }
    }


    /// <summary>
    /// Gets all child nodes in the specified parent path.
    /// </summary>
    /// <param name="searchText">Text to filter searched nodes</param>
    /// <param name="parentAliasPath">Alias path of the parent</param>
    /// <param name="siteName">Name of the related site</param>
    /// <param name="totalRecords">Total records</param>
    private DataSet GetNodes(string searchText, string parentAliasPath, string siteName, ref int totalRecords)
    {
        // Create WHERE condition
        string where = "(NodeAliasPath <> '/')";

        bool searchEnabled = !string.IsNullOrEmpty(searchText);
        if (searchEnabled)
        {
            string searchTextSafe = SqlHelper.EscapeLikeText(SqlHelper.EscapeQuotes(searchText));
            where = SqlHelper.AddWhereCondition(where, String.Format("((AttachmentName LIKE N'%{0}%') OR (DocumentName LIKE N'%{0}%'))", searchTextSafe));
        }

        // If not all content is selectable and no additional content being displayed
        if ((SelectableContent != SelectableContentEnum.AllContent) && !IsFullListingMode)
        {
            where = SqlHelper.AddWhereCondition(where, "(ClassName = 'CMS.File')");
        }
        
        // Get files
        var query = DocumentHelper.GetDocuments()
                                    .Published(IsLiveSite)
                                    .PublishedVersion(IsLiveSite)
                                    .OnSite(siteName)
                                    .Path(parentAliasPath, PathTypeEnum.Children)
                                    .Culture(Config.Culture)
                                    .CombineWithDefaultCulture()
                                    .Where(where)
                                    .OrderBy(mediaView.ListViewControl.SortDirect)
                                    .NestingLevel(1)
                                    .Columns(SqlHelper.ParseColumnList(NODE_COLUMNS))
                                    .CheckPermissions();

        if (!searchEnabled)
        {
            // Don't use paged query for searching (works only for first page)
            query.Page(mediaView.CurrentPage - 1, mediaView.CurrentPageSize);
        }
        
        query.QueryName = IsLiveSite ? "selectattachments" : "selectattachmentsversions";

        DataSet nodes = query.Result;
        totalRecords = query.TotalRecords;

        return nodes;
    }


    /// <summary>
    /// Loads all files for the view control.
    /// </summary>
    /// <param name="searchText">Text to filter loaded files</param>
    private void LoadContentDataSource(string searchText)
    {
        DataSet nodes = null;
        int totalRecords = -1;

        // Load data
        if (NodeID > 0)
        {
            // Get selected node            
            TreeNode node = TreeNodeObj;
            if ((TreeNodeObj != null) && !(Config.OutputFormat == OutputFormatEnum.NodeGUID && node.NodeSiteID != SiteContext.CurrentSiteID))
            {
                // Get selected node site info
                SiteInfo si = SiteInfoProvider.GetSiteInfo(node.NodeSiteID);
                if (si != null)
                {
                    // Ensure culture prefix
                    if (URLHelper.UseLangPrefixForUrls(si.SiteName))
                    {
                        CultureInfo ci = CultureInfoProvider.GetCultureInfo(node.DocumentCulture);
                        if (ci != null)
                        {
                            RequestContext.CurrentURLLangPrefix = (String.IsNullOrEmpty(ci.CultureAlias) ? ci.CultureCode : ci.CultureAlias);
                        }
                    }

                    // List view
                    if (node.NodeClassName.ToLowerCSafe() != "cms.file")
                    {
                        if (node.TreeProvider.CheckDocumentUIPermissions(si.SiteName) && (MembershipContext.AuthenticatedUser.IsAuthorizedPerDocument(node, NodePermissionsEnum.Read) != AuthorizationResultEnum.Allowed))
                        {
                            return;
                        }

                        // Check permissions
                        if (MembershipContext.AuthenticatedUser.IsAuthorizedPerDocument(node, NodePermissionsEnum.ExploreTree) == AuthorizationResultEnum.Allowed)
                        {
                            nodes = GetNodes(searchText, node.NodeAliasPath, si.SiteName, ref totalRecords);

                            LoadNodes(nodes);

                            // If all content selectable
                            bool selectableAll = SelectableContent == SelectableContentEnum.AllContent;
                            if (selectableAll && !IsFullListingMode && (IsAction || !URLHelper.IsPostback()))
                            {
                                if ((ItemToColorize == Guid.Empty) || (ItemToColorize == node.NodeGUID))
                                {
                                    string fileExtension = TreePathUtils.GetUrlExtension();
                                    if (String.IsNullOrEmpty(fileExtension))
                                    {
                                        fileExtension = node.DocumentExtensions;
                                    }
                                    string url = mediaView.GetContentItemUrl(node.NodeGUID, node.DocumentUrlPath, node.NodeAlias,
                                                                             node.NodeAliasPath, node.IsLink, 0, 0, 0, true, fileExtension);

                                    ItemToColorize = node.NodeGUID;

                                    SelectMediaItem(node.DocumentName, url, node.NodeAliasPath);
                                }
                            }

                            // Display full-size properties if detailed view required
                            if (!IsCopyMoveLinkDialog && !IsFullListingMode && selectableAll && !node.NodeHasChildren && !IsLinkOutput)
                            {
                                DisplayFull();
                            }
                            else
                            {
                                DisplayNormal();
                            }
                        }
                        else
                        {
                            mediaView.InfoText = GetString("dialogs.document.NotAuthorizedToExpolore");
                        }
                    }
                    else
                    {
                        // Check permissions
                        if ((MembershipContext.AuthenticatedUser.IsAuthorizedPerDocument(node, NodePermissionsEnum.Read) == AuthorizationResultEnum.Allowed))
                        {
                            // Get attachment info and initialize displayed attachment properties
                            Guid attachmentGUID = ValidationHelper.GetGuid(node.GetValue("FileAttachment"), Guid.Empty);
                            if (attachmentGUID != Guid.Empty)
                            {
                                // Get the attachment
                                TreeProvider tree = new TreeProvider(MembershipContext.AuthenticatedUser) { UseCache = false };
                                AttachmentInfo atInfo = DocumentHelper.GetAttachment(node, attachmentGUID, tree, false);
                                if (atInfo != null)
                                {
                                    // Get the data
                                    string extension = atInfo.AttachmentExtension;
                                    bool isContentFile = (node.NodeClassName.ToLowerCSafe() == "cms.file");

                                    if (CMSDialogHelper.IsItemSelectable(SelectableContent, extension, isContentFile))
                                    {
                                        string fileExtension = (isContentFile ? TreePathUtils.GetFilesUrlExtension(SiteName) : TreePathUtils.GetUrlExtension(SiteName));
                                        if (String.IsNullOrEmpty(fileExtension))
                                        {
                                            fileExtension = node.DocumentExtensions;
                                        }
                                        // Set 'get file path'
                                        atInfo.AttachmentUrl = mediaView.GetContentItemUrl(node.NodeGUID, node.DocumentUrlPath, node.NodeAlias, node.NodeAliasPath, node.IsLink, 0, 0, 0, false, fileExtension);

                                        CurrentAttachmentInfo = atInfo;

                                        // Display full-sized properties only when media dialog is opened not in listing mode, node has no children, node is image and selectable are only images or output is not a link
                                        if (!IsCopyMoveLinkDialog && !IsFullListingMode && !node.NodeHasChildren && (!IsLinkOutput || ImageHelper.IsImage(extension) && (SelectableContent == SelectableContentEnum.OnlyImages)))
                                        {
                                            // Display properties in full size
                                            DisplayFull();
                                        }
                                        else
                                        {
                                            if (node.NodeHasChildren)
                                            {
                                                // Load child nodes                                            
                                                nodes = GetNodes(searchText, node.NodeAliasPath, si.SiteName, ref totalRecords);

                                                LoadNodes(nodes);
                                            }

                                            DisplayNormal();
                                        }
                                    }
                                    else
                                    {
                                        mediaView.InfoText = GetString("dialogs.item.notselectable");

                                        DisplayNormal();
                                    }
                                }
                            }
                            else
                            {
                                DisplayNormal();
                            }
                        }
                        else
                        {
                            DisplayNormal();

                            mediaView.InfoText = GetString("dialogs.document.NotAuthorizedToViewNode");
                        }
                    }
                }
            }
        }

        mediaView.DataSource = nodes;
        mediaView.TotalRecords = totalRecords;
    }


    /// <summary>
    /// Handles actions related to the folders.
    /// </summary>
    /// <param name="argument">Argument related to the folder action</param>
    /// <param name="reloadTree">Indicates whether to reload tree</param>
    /// <param name="callSelection">Indicates if selection should be called</param>
    private void HandleFolderAction(string argument, bool reloadTree, bool callSelection = true)
    {
        NodeID = ValidationHelper.GetInteger(argument, 0);

        // Update new folder information
        menuElem.NodeID = NodeID;
        menuElem.UpdateViewMenu();

        // Reload content tree if new folder was created
        if (reloadTree)
        {
            InitializeContentTree();

            // Fill with new info
            contentTree.SelectedNodeID = NodeID;
            contentTree.ExpandNodeID = NodeID;

            contentTree.ReloadData();
            pnlUpdateTree.Update();

            ScriptHelper.RegisterStartupScript(Page, typeof(Page), "EnsureTopWindow", ScriptHelper.GetScript("if (self.focus) { self.focus(); }"));
        }

        // Load new data 
        LoadDataSource();

        // Load selected item
        if (CurrentAttachmentInfo != null)
        {
            string fileName = Path.GetFileNameWithoutExtension(CurrentAttachmentInfo.AttachmentName);

            if (callSelection)
            {
                SelectMediaItem(fileName, CurrentAttachmentInfo.AttachmentExtension, CurrentAttachmentInfo.AttachmentImageWidth,
                                CurrentAttachmentInfo.AttachmentImageHeight, CurrentAttachmentInfo.AttachmentSize, CurrentAttachmentInfo.AttachmentUrl);
            }

            ItemToColorize = CurrentAttachmentInfo.AttachmentGUID;
            ColorizeRow(ItemToColorize.ToString());
        }
        else
        {
            ColorizeLastSelectedRow();
        }

        // Get parent node ID info
        int parentId = StartingPathNodeID != NodeID ? GetParentNodeID(NodeID) : 0;
        if (parentId > 0)
        {
            // Show parent button and setup correct parent node ID
            menuElem.ShowParentButton = true;
            menuElem.ParentNodeID = parentId;
        }
        else
        {
            // Parent button is not needed
            menuElem.ShowParentButton = false;
        }

        // Reload view control's content
        mediaView.Reload();
        pnlUpdateView.Update();

        ClearActionElems();
    }


    /// <summary>
    /// Returns path of the node specified by its ID.
    /// </summary>
    /// <param name="nodeId">ID of the node</param>
    private static int GetParentNodeID(int nodeId)
    {
        if (nodeId > 0)
        {
            TreeProvider tree = new TreeProvider(MembershipContext.AuthenticatedUser);
            // Get node and return its alias path
            using (TreeNode node = tree.SelectSingleNode(nodeId))
            {
                if (node != null)
                {
                    return node.NodeParentID;
                }
            }
        }

        return 0;
    }


    /// <summary>
    /// Handles actions occurring when new content (CMS.File) document was created.
    /// </summary>
    /// <param name="argument">Argument holding information on new document node ID</param>
    private void HandleContentFileCreatedAction(string argument)
    {
        string[] argArr = argument.Split('|');
        if (argArr.Length == 1)
        {
            HandleFolderAction(argArr[0], true);
        }
    }


    /// <summary>
    /// Handles attachment edit action.
    /// </summary>
    /// <param name="argument">Attachment GUID coming from view control</param>
    private void HandleContentEdit(string argument)
    {
        IsEditImage = true;

        if (!string.IsNullOrEmpty(argument))
        {
            Hashtable argTable = CMSModules_Content_Controls_Dialogs_Selectors_LinkMediaSelector_MediaView.GetArgumentsTable(argument);

            Guid attachmentGuid = ValidationHelper.GetGuid(argTable["attachmentguid"], Guid.Empty);

            // Node ID was specified
            int nodeId = 0;
            if (argTable.Count == 2)
            {
                nodeId = ValidationHelper.GetInteger(argTable["nodeid"], 0);
            }

            AttachmentInfo ai = AttachmentInfoProvider.GetAttachmentInfo(attachmentGuid, SiteName);
            if (ai != null)
            {
                // Get attachment node by ID
                TreeNode node = (nodeId > 0 ? TreeHelper.SelectSingleNode(nodeId) : TreeHelper.SelectSingleDocument(ai.AttachmentDocumentID));
                if (node != null)
                {
                    // Check node site
                    if (node.NodeSiteID != SiteContext.CurrentSiteID)
                    {
                        mediaView.SiteObj = SiteInfoProvider.GetSiteInfo(node.NodeSiteID);
                    }

                    string fileExt = TreePathUtils.GetFilesUrlExtension();
                    if (String.IsNullOrEmpty(fileExt))
                    {
                        fileExt = node.DocumentExtensions;
                    }

                    // Get node URL
                    string url = mediaView.GetContentItemUrl(node.NodeGUID, node.DocumentUrlPath, node.NodeAlias, node.NodeAliasPath, node.IsLink, 0, 0, 0, false, fileExt);

                    // Update properties if node is currently selected
                    if (attachmentGuid == ItemToColorize)
                    {
                        SelectMediaItem(ai.AttachmentName, ai.AttachmentExtension, ai.AttachmentImageWidth, ai.AttachmentImageHeight, ai.AttachmentSize, url, null, node.NodeID, node.NodeAliasPath);
                    }

                    // Update select action to reflect changes made during editing
                    LoadDataSource();
                    mediaView.Reload();
                    pnlUpdateView.Update();
                }
            }
        }

        ClearActionElems();
    }


    /// <summary>
    /// Ensures content tree is refreshed when new folder is created in Copy/Move dialog.
    /// </summary>
    private void RefreshContentTree()
    {
        ScriptHelper.RegisterWOpenerScript(Page);
        // Refresh content tree
        ScriptHelper.RegisterStartupScript(Page, typeof(Page), "RefreshContentTree", ScriptHelper.GetScript(@"
if (wopener == null) {
    wopener = opener;
}              
if (wopener.parent != null) {
    if (wopener.parent != null) {
        if (wopener.parent.RefreshTree != null) {
            wopener.parent.RefreshTree();
        }
    }
}"));
    }

    #endregion


    #region "Attachment methods"

    /// <summary>
    /// Loads all attachments for the view control.
    /// </summary>
    /// <param name="searchText">Text to filter loaded files</param>
    private void LoadAttachmentsDataSource(string searchText)
    {
        DataSet attachments = null;
        int totalRecords = -1;
        int currentPageSize = mediaView.CurrentPageSize;

        // Only unsorted attachments are being displayed
        string where = String.IsNullOrEmpty(searchText) ? "(AttachmentIsUnsorted = 1)" : SqlHelper.AddWhereCondition("(AttachmentIsUnsorted = 1)", String.Format("(AttachmentName LIKE N'%{0}%')", SqlHelper.EscapeLikeText(SqlHelper.EscapeQuotes(searchText))));

        // Get document attachments
        if (Config.AttachmentDocumentID != 0)
        {
            if (TreeNodeObj != null)
            {
                // Check permissions
                if (MembershipContext.AuthenticatedUser.IsAuthorizedPerDocument(TreeNodeObj, NodePermissionsEnum.Read) == AuthorizationResultEnum.Allowed)
                {
                    // Get the node workflow
                    var wi = TreeNodeObj.GetWorkflow();
                    int versionHistoryId = TreeNodeObj.DocumentCheckedOutVersionHistoryID;

                    // If document uses workflow, get the attachments from version history
                    if ((wi != null) && (versionHistoryId > 0))
                    {
                        // Get attachments for given version
                        var query = TreeNodeObj.VersionManager.GetVersionAttachments(versionHistoryId, where, mediaView.ListViewControl.SortDirect, false, -1, null).Page(mediaView.CurrentPage - 1, mediaView.CurrentPageSize);
                        attachments = query.Result;
                        totalRecords = query.TotalRecords;
                    }
                    else
                    {
                        // Get attachments for published document
                        where = SqlHelper.AddWhereCondition(where, "AttachmentDocumentID=" + TreeNodeObj.DocumentID);
                        var query = AttachmentInfoProvider.GetAttachments(where, mediaView.ListViewControl.SortDirect, false).Page(mediaView.CurrentPage - 1, mediaView.CurrentPageSize);
                        attachments = query.Result;
                        totalRecords = query.TotalRecords;
                    }
                }
            }
        }
        // Get form attachments
        else if (AttachmentsAreTemporary)
        {
            where = SqlHelper.AddWhereCondition(where, String.Format("(AttachmentFormGUID = '{0}')", Config.AttachmentFormGUID));
            var query = AttachmentInfoProvider.GetAttachments(where, "AttachmentOrder, AttachmentName", false, currentPageSize);
            query.Offset = mediaView.CurrentOffset;
            query.MaxRecords = currentPageSize;

            attachments = query.Result;
            mediaView.TotalRecords = query.TotalRecords;
        }

        if (DataHelper.DataSourceIsEmpty(attachments) && mediaView.CurrentPage > 1)
        {
            // Switch to previous page if current page has no data
            mediaView.CurrentPage -= 1;
            LoadAttachmentsDataSource(searchText);
            return;
        }

        mediaView.DataSource = attachments;
        mediaView.TotalRecords = totalRecords;
    }


    /// <summary>
    /// Checks attachment permissions.
    /// </summary>
    private string CheckAttachmentPermissions()
    {
        string message = "";

        // For new document
        if (Config.AttachmentFormGUID != Guid.Empty)
        {
            if (Config.AttachmentParentID == 0)
            {
                message = "Node parent node ID has to be set.";
            }

            if (!RaiseOnCheckPermissions("Create", this))
            {
                if (!MembershipContext.AuthenticatedUser.IsAuthorizedToCreateNewDocument(Config.AttachmentParentID, "CMS.File"))
                {
                    message = GetString("attach.actiondenied");
                }
            }
        }
        // For existing document
        else if (Config.AttachmentDocumentID > 0)
        {
            TreeProvider tree = new TreeProvider(MembershipContext.AuthenticatedUser);
            // Get document node
            using (TreeNode node = DocumentHelper.GetDocument(Config.AttachmentDocumentID, tree))
            {
                if (node == null)
                {
                    message = "Given page doesn't exist!";
                }
                if (!RaiseOnCheckPermissions("Modify", this))
                {
                    if (MembershipContext.AuthenticatedUser.IsAuthorizedPerDocument(node, NodePermissionsEnum.Modify) != AuthorizationResultEnum.Allowed)
                    {
                        message = GetString("attach.actiondenied");
                    }
                }
            }
        }

        return message;
    }


    /// <summary>
    /// Handles new attachment create action.
    /// </summary>
    /// <param name="argument">Argument coming from upload control</param>
    private void HandleAttachmentCreatedAction(string argument)
    {
        HandleAttachmentAction(argument, false);

        // Reload view
        LoadDataSource();

        mediaView.Reload();
        pnlUpdateView.Update();
    }


    /// <summary>
    /// Handles attachment update action.
    /// </summary>
    /// <param name="argument">Argument coming from upload control</param>
    private void HandleAttachmentUpdatedAction(string argument)
    {
        HandleAttachmentAction(argument, true);
    }


    /// <summary>
    /// Handles attachment edit action.
    /// </summary>
    /// <param name="argument">Attachment GUID coming from view control</param>
    private void HandleAttachmentEdit(string argument)
    {
        IsEditImage = true;

        if (!string.IsNullOrEmpty(argument))
        {
            string[] argArr = argument.Split('|');

            Guid attachmentGuid = ValidationHelper.GetGuid(argArr[1], Guid.Empty);

            AttachmentInfo ai = null;

            int versionHistoryId = 0;
            if (TreeNodeObj != null)
            {
                versionHistoryId = TreeNodeObj.DocumentCheckedOutVersionHistoryID;
            }

            if (versionHistoryId == 0)
            {
                ai = AttachmentInfoProvider.GetAttachmentInfo(attachmentGuid, SiteContext.CurrentSiteName);
            }
            else
            {
                VersionManager vm = VersionManager.GetInstance(TreeNodeObj.TreeProvider);
                if (vm != null)
                {
                    // Get the attachment version data
                    AttachmentHistoryInfo attachmentVersion = vm.GetAttachmentVersion(versionHistoryId, attachmentGuid, false);

                    // Create the attachment info from given data
                    ai = (attachmentVersion != null) ? new AttachmentInfo(attachmentVersion.Generalized.DataClass) : null;
                    if (ai != null)
                    {
                        ai.AttachmentLastHistoryID = versionHistoryId;
                    }
                }
            }

            if (ai != null)
            {
                string nodeAliasPath = "";
                if (TreeNodeObj != null)
                {
                    nodeAliasPath = TreeNodeObj.NodeAliasPath;
                }

                string url = mediaView.GetAttachmentItemUrl(ai.AttachmentGUID, ai.AttachmentName, nodeAliasPath, ai.AttachmentImageHeight, ai.AttachmentImageWidth, 0);

                if (LastAttachmentGuid == attachmentGuid)
                {
                    SelectMediaItem(ai.AttachmentName, ai.AttachmentExtension, ai.AttachmentImageWidth, ai.AttachmentImageHeight, ai.AttachmentSize, url);
                }

                // Update select action to reflect changes made during editing
                LoadDataSource();
                mediaView.Reload();
                pnlUpdateView.Update();
            }
        }

        ClearActionElems();
    }


    /// <summary>
    /// Handles attachment action.
    /// </summary>
    /// <param name="argument">Argument coming from upload control</param>
    /// <param name="isUpdate">Indicates if is update</param>
    private void HandleAttachmentAction(string argument, bool isUpdate)
    {
        // Get attachment URL first
        Guid attachmentGuid = ValidationHelper.GetGuid(argument, Guid.Empty);
        if (attachmentGuid != Guid.Empty)
        {
            // Get attachment info
            TreeProvider tree = new TreeProvider(MembershipContext.AuthenticatedUser);

            // Ensure site information
            SiteInfo si = SiteContext.CurrentSite;
            if ((TreeNodeObj != null) && (si.SiteID != TreeNodeObj.NodeSiteID))
            {
                si = SiteInfoProvider.GetSiteInfo(TreeNodeObj.NodeSiteID);
            }

            AttachmentInfo ai = DocumentHelper.GetAttachment(attachmentGuid, tree, si.SiteName, false);
            if (ai != null)
            {
                string nodeAliasPath = (TreeNodeObj != null) ? TreeNodeObj.NodeAliasPath : null;

                if (CMSDialogHelper.IsItemSelectable(SelectableContent, ai.AttachmentExtension))
                {
                    // Get attachment URL
                    string url = mediaView.GetAttachmentItemUrl(ai.AttachmentGUID, ai.AttachmentName, nodeAliasPath, 0, 0, 0);

                    // Remember last selected attachment GUID
                    if (SourceType == MediaSourceEnum.DocumentAttachments)
                    {
                        LastAttachmentGuid = ai.AttachmentGUID;
                    }

                    // Get the node workflow
                    int versionHistoryId = 0;
                    if (TreeNodeObj != null)
                    {
                        WorkflowManager wm = WorkflowManager.GetInstance(TreeNodeObj.TreeProvider);
                        WorkflowInfo wi = wm.GetNodeWorkflow(TreeNodeObj);
                        if (wi != null)
                        {
                            // Ensure the document version
                            VersionManager vm = VersionManager.GetInstance(TreeNodeObj.TreeProvider);
                            versionHistoryId = vm.EnsureVersion(TreeNodeObj, TreeNodeObj.IsPublished);
                        }
                    }

                    MediaItem item = InitializeMediaItem(ai.AttachmentName, ai.AttachmentExtension, ai.AttachmentImageWidth, ai.AttachmentImageHeight, ai.AttachmentSize, url, null, versionHistoryId, 0, "");

                    SelectMediaItem(item);

                    ItemToColorize = attachmentGuid;

                    ColorizeRow(ItemToColorize.ToString());
                }
                else
                {
                    // Unselect old attachment and clear properties
                    ColorizeRow("");
                    Properties.ClearProperties(true);
                    pnlUpdateProperties.Update();
                }

                mediaView.InfoText = (isUpdate ? GetString("dialogs.attachment.updated") : GetString("dialogs.attachment.created"));

                pnlUpdateView.Update();
            }
        }

        ClearActionElems();
    }


    /// <summary>
    /// Handles actions occurring when attachment is moved.
    /// </summary>
    /// <param name="argument">Argument holding information on attachment being moved</param>
    /// <param name="action">Action specifying whether the attachment is moved up/down</param>
    private void HandleAttachmentMoveAction(string argument, string action)
    {
        // Check permissions
        string errMsg = CheckAttachmentPermissions();

        if (errMsg == "")
        {
            Guid attachmentGuid = ValidationHelper.GetGuid(argument, Guid.Empty);
            if (attachmentGuid != Guid.Empty)
            {
                // Move temporary attachment
                if (!AttachmentsAreTemporary)
                {
                    if (action == "attachmentmoveup")
                    {
                        DocumentHelper.MoveAttachmentUp(attachmentGuid, TreeNodeObj);
                    }
                    else
                    {
                        DocumentHelper.MoveAttachmentDown(attachmentGuid, TreeNodeObj);
                    }
                }
                else
                {
                    if (action == "attachmentmoveup")
                    {
                        AttachmentInfoProvider.MoveAttachmentUp(attachmentGuid, 0);
                    }
                    else
                    {
                        AttachmentInfoProvider.MoveAttachmentDown(attachmentGuid, 0);
                    }
                }

                // Reload data
                LoadDataSource();

                mediaView.Reload();
            }
        }
        else
        {
            // Display error
            ShowError(errMsg);
        }

        ClearActionElems();

        ColorizeLastSelectedRow();

        pnlUpdateView.Update();
    }


    /// <summary>
    /// Handles actions occurring when some attachment is being removed.
    /// </summary>
    /// <param name="argument">Argument holding information on attachment</param>
    private void HandleDeleteAttachmentAction(string argument)
    {
        string errMsg = CheckAttachmentPermissions();

        if (errMsg == "")
        {
            Guid attachmentGuid = ValidationHelper.GetGuid(argument, Guid.Empty);
            if (attachmentGuid != Guid.Empty)
            {
                if (!AttachmentsAreTemporary)
                {
                    TreeProvider tree = new TreeProvider(MembershipContext.AuthenticatedUser);

                    DocumentHelper.DeleteAttachment(TreeNodeObj, attachmentGuid, tree);
                }
                else
                {
                    // Delete temporary attachment
                    AttachmentInfoProvider.DeleteTemporaryAttachment(attachmentGuid, SiteContext.CurrentSiteName);
                }

                // Reload data
                LoadDataSource();
                mediaView.Reload();

                // Selected attachment was removed
                if (LastAttachmentGuid == attachmentGuid)
                {
                    // Reset properties
                    Properties.ClearProperties();
                    pnlUpdateProperties.Update();
                }
                else
                {
                    ColorizeLastSelectedRow();
                }
            }
        }
        else
        {
            // Display error
            ShowError(errMsg);
        }

        pnlUpdateView.Update();
    }

    #endregion


    #region "MetaFile methods"

    /// <summary>
    /// Loads all metafiles for the view control.
    /// </summary>
    /// <param name="searchText">Text to filter loaded files</param>
    private void LoadMetaFileDataSource(string searchText)
    {
        string where = !String.IsNullOrEmpty(searchText) ? new WhereCondition().WhereContains("MetaFileName", searchText).ToString(true) : string.Empty;

        int totalRecords = -1;
        const string columns = "MetaFileID,MetaFileObjectType,MetaFileObjectID,MetaFileGroupName,MetaFileName,MetaFileExtension,MetaFileSize,MetaFileMimeType,MetaFileImageWidth,MetaFileImageHeight,MetaFileGUID,MetaFileLastModified,MetaFileSiteID,MetaFileTitle,MetaFileDescription";
        // Get metafiles
        mediaView.DataSource = MetaFileInfoProvider.GetMetaFiles(Config.MetaFileObjectID, Config.MetaFileObjectType, Config.MetaFileCategory, where, mediaView.ListViewControl.SortDirect, columns, -1, mediaView.CurrentOffset, mediaView.CurrentPageSize, ref totalRecords);
        mediaView.TotalRecords = totalRecords;
    }


    /// <summary>
    /// Checks metafile permissions.
    /// </summary>
    private string CheckMetaFilePermissions()
    {
        string message = string.Empty;

        if (!UserInfoProvider.IsAuthorizedPerObject(Config.MetaFileObjectType, Config.MetaFileObjectID, PermissionsEnum.Modify, SiteName, MembershipContext.AuthenticatedUser))
        {
            message = GetString("general.nopermission");
        }

        return message;
    }


    /// <summary>
    /// Handles actions occurring when some metafile is being removed.
    /// </summary>
    /// <param name="argument">Argument holding information on metafile</param>
    private void HandleMetaFileDelete(string argument)
    {
        // Check permissions
        string errMsg = CheckMetaFilePermissions();

        if (string.IsNullOrEmpty(errMsg))
        {
            // Get meta file ID
            int metaFileId = ValidationHelper.GetInteger(argument, 0);
            try
            {
                Guid mfGuid = Guid.Empty;

                MetaFileInfo mf = MetaFileInfoProvider.GetMetaFileInfo(metaFileId);
                if (mf != null)
                {
                    mfGuid = mf.MetaFileGUID;
                    // Delete meta file
                    MetaFileInfoProvider.DeleteMetaFileInfo(mf);
                }

                // Reload data
                LoadDataSource();
                mediaView.Reload();

                // Selected metafile was removed
                if (LastAttachmentGuid == mfGuid)
                {
                    // Reset properties
                    Properties.ClearProperties();
                    pnlUpdateProperties.Update();
                }
                else
                {
                    ColorizeLastSelectedRow();
                }
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }
        }
        else
        {
            // Display error
            ShowError(errMsg);
        }

        pnlUpdateView.Update();

        ClearActionElems();
    }


    /// <summary>
    /// Handles metafile edit action.
    /// </summary>
    /// <param name="argument">MetaFile GUID coming from view control in format 'attachmentguid|[MetaFileGUID]'</param>
    private void HandleMetaFileEdit(string argument)
    {
        IsEditImage = true;

        if (!string.IsNullOrEmpty(argument))
        {
            string[] argArr = argument.Split('|');

            // Get meta file GUID
            Guid mfGuid = ValidationHelper.GetGuid(argArr[1], Guid.Empty);

            MetaFileInfo mf = MetaFileInfoProvider.GetMetaFileInfo(mfGuid, SiteName, false);
            if (mf != null)
            {
                // Update select action to reflect changes made during editing
                LoadDataSource();
                mediaView.Reload();

                // Reload properties section
                if (LastAttachmentGuid == mfGuid)
                {
                    string url = mediaView.GetMetaFileItemUrl(mfGuid, mf.MetaFileName, mf.MetaFileImageHeight, mf.MetaFileImageWidth, 0);
                    SelectMediaItem(mf.MetaFileName, mf.MetaFileExtension, mf.MetaFileImageWidth, mf.MetaFileImageHeight, mf.MetaFileSize, url);
                }

                ColorizeLastSelectedRow();

                pnlUpdateView.Update();
            }
        }

        ClearActionElems();
    }


    /// <summary>
    /// Handles metafile update action.
    /// </summary>
    /// <param name="argument">MetaFile ID</param>
    private void HandleMetaFileUpdated(string argument)
    {
        if (!string.IsNullOrEmpty(argument))
        {
            // Get meta file ID
            int metaFileId = ValidationHelper.GetInteger(argument, 0);

            Guid mfGuid = Guid.Empty;

            MetaFileInfo mf = MetaFileInfoProvider.GetMetaFileInfo(metaFileId);
            if (mf != null)
            {
                mfGuid = mf.MetaFileGUID;
            }

            // Update select action to reflect changes made during update
            LoadDataSource();
            mediaView.Reload();

            // Reload properties section
            if ((LastAttachmentGuid == mfGuid) && (mf != null))
            {
                string url = mediaView.GetMetaFileItemUrl(mfGuid, mf.MetaFileName, mf.MetaFileImageHeight, mf.MetaFileImageWidth, 0);
                SelectMediaItem(mf.MetaFileName, mf.MetaFileExtension, mf.MetaFileImageWidth, mf.MetaFileImageHeight, mf.MetaFileSize, url);
            }

            ColorizeLastSelectedRow();
            pnlUpdateView.Update();
        }

        ClearActionElems();
    }

    #endregion


    #region "Common event methods"

    /// <summary>
    /// Handles actions occurring when some text is searched.
    /// </summary>
    /// <param name="argument">Argument holding information on searched text</param>
    private void HandleSearchAction(string argument)
    {
        LastSearchedValue = argument;

        // Load new data filtered by searched text 
        LoadDataSource();

        // Reload content
        mediaView.Reload();
        pnlUpdateView.Update();

        // Keep focus in search text box
        ScriptHelper.RegisterStartupScript(Page, typeof(string), "SetSearchFocus", ScriptHelper.GetScript("setTimeout('SetSearchFocus();', 200);"));
    }


    /// <summary>
    /// Handles actions occurring when some item is selected.
    /// </summary>
    /// <param name="argument">Argument holding information on selected item</param>
    private void HandleSelectAction(string argument)
    {
        // Create new selected media item
        SelectMediaItem(argument);

        // Forget recent action
        ClearActionElems();
    }


    /// <summary>
    /// Handles actions occurring when some item in copy/move/link/select path dialog is selected.
    /// </summary>
    private void HandleDialogSelect()
    {
        if (TreeNodeObj != null)
        {
            string columns = SqlHelper.MergeColumns((IsLiveSite ? TreeProvider.SELECTNODES_REQUIRED_COLUMNS : DocumentHelper.GETDOCUMENTS_REQUIRED_COLUMNS), NODE_COLUMNS);

            // Get files
            TreeNodeObj.TreeProvider.SelectQueryName = "selectattachments";

            DataSet nodeDetails;
            if (IsLiveSite)
            {
                // Get published files
                nodeDetails = TreeNodeObj.TreeProvider.SelectNodes(SiteName, TreeNodeObj.NodeAliasPath, Config.Culture, true, null, null, "DocumentName", 1, true, 1, columns);
            }
            else
            {
                // Get latest files
                nodeDetails = DocumentHelper.GetDocuments(SiteName, TreeNodeObj.NodeAliasPath, TreeProvider.ALL_CULTURES, true, null, null, "DocumentName", 1, false, 1, columns, TreeNodeObj.TreeProvider);
            }

            // If node details exists
            if (!DataHelper.IsEmpty(nodeDetails))
            {
                IDataContainer data = new DataRowContainer(nodeDetails.Tables[0].Rows[0]);

                string argument = mediaView.GetArgumentSet(data);
                bool notAttachment = (SourceType == MediaSourceEnum.Content) && !((data.GetValue("ClassName").ToString().ToLowerCSafe() == "cms.file") && (ValidationHelper.GetGuid(data.GetValue("AttachmentGUID"), Guid.Empty) != Guid.Empty));
                string url = mediaView.GetItemUrl(argument, 0, 0, 0, notAttachment);

                SelectMediaItem(String.Format("{0}|URL|{1}", argument, url));
            }

            ItemToColorize = TreeNodeObj.NodeGUID;
        }
        else
        {
            // Remove selected item
            ItemToColorize = Guid.Empty;
        }
        ClearColorizedRow();

        // Forget recent action
        ClearActionElems();
    }


    private void HandleSiteEmpty()
    {
        if ((SourceType != MediaSourceEnum.DocumentAttachments) && (SourceType != MediaSourceEnum.MetaFile) && String.IsNullOrEmpty(SiteName))
        {
            contentTree.Visible = false;
            siteSelector.Enabled = false;
            lblTreeInfo.Visible = true;

            // Disable menu
            ScriptHelper.RegisterStartupScript(Page, typeof(Page), "DialogsDisableMenuActions", ScriptHelper.GetScript("if(window.DisableNewFileBtn){ window.DisableNewFileBtn(); } if(window.DisableNewFolderBtn){ window.DisableNewFolderBtn(); }"));
        }
    }

    #endregion


    #region "Event handlers"

    protected void UniSelector_OnSelectionChanged(object sender, EventArgs e)
    {
        IsAction = true;

        // Update information on current site 
        SiteID = siteSelector.SiteID;
        if (SiteID > 0)
        {
            mediaView.SiteObj = SiteInfoProvider.GetSiteInfo(SiteID);
        }

        // Reset selected node to root node
        NodeID = StartingPathNodeID = GetStartNodeId();
        if (NodeID == 0)
        {
            NodeID = GetContentNodeId("/");
            menuElem.ShowParentButton = false;
        }

        if (SelectableContent != SelectableContentEnum.AllContent)
        {
            contentTree.SelectedNodeID = NodeID;
            contentTree.ExpandNodeID = NodeID;
        }
        else
        {
            mediaView.DataSource = null;
        }

        // Clear item to colorize identifier (site name was changed)
        ItemToColorize = Guid.Empty;

        // Clear properties from session to set new one later
        Properties.ClearProperties();

        // Reload media view for new site
        LoadDataSource();

        // Update information on parent node ID for new folder creation
        menuElem.NodeID = NodeID;
        menuElem.UpdateViewMenu();

        // Reload content tree for new site
        contentTree.SiteName = siteSelector.SiteName;
        InitializeContentTree();
        pnlUpdateTree.Update();

        // Load selected item
        if (CurrentAttachmentInfo != null)
        {
            SelectMediaItem(CurrentAttachmentInfo.AttachmentName, CurrentAttachmentInfo.AttachmentExtension,
                            CurrentAttachmentInfo.AttachmentImageWidth, CurrentAttachmentInfo.AttachmentImageHeight, CurrentAttachmentInfo.AttachmentSize, CurrentAttachmentInfo.AttachmentUrl);
        }

        // Setup media view
        mediaView.Reload();
        pnlUpdateView.Update();

        // Setup properties
        DisplayNormal();
        pnlUpdateProperties.Update();
    }


    /// <summary>
    /// Behaves as mediator in communication line between control taking action and the rest of the same level controls.
    /// </summary>
    protected void hdnButton_Click(object sender, EventArgs e)
    {
        IsAction = true;

        switch (CurrentAction)
        {
            case "insertitem":
                GetSelectedItem();
                break;

            case "search":
                HandleSearchAction(CurrentArgument);
                break;

            case "select":
                HandleSelectAction(CurrentArgument);
                break;

            case "morecontentselect":
            case "contentselect":
                ResetSearchFilter();

                if (IsLinkOutput)
                {
                    CurrentAttachmentInfo = null;
                }

                // If more content is requested, enable the full listing
                if (!IsFullListingMode)
                {
                    IsFullListingMode = (CurrentAction == "morecontentselect");
                }

                HandleFolderAction(CurrentArgument, IsFullListingMode);

                if (IsCopyMoveLinkDialog || IsLinkOutput)
                {
                    HandleDialogSelect();
                }
                break;

            case "parentselect":
                ResetSearchFilter();

                HandleFolderAction(CurrentArgument, true);

                if (IsCopyMoveLinkDialog)
                {
                    HandleDialogSelect();
                }
                break;

            case "refreshtree":
                ResetSearchFilter();
                HandleFolderAction(CurrentArgument, true);
                break;

            case "contentcreated":
                HandleContentFileCreatedAction(CurrentArgument);

                if (IsCopyMoveLinkDialog)
                {
                    HandleDialogSelect();
                }
                break;

            case "closelisting":
                IsFullListingMode = false;
                HandleFolderAction(NodeID.ToString(), false);
                break;

            case "newfolder":
                ResetSearchFilter();
                HandleFolderAction(CurrentArgument, true);

                if (IsCopyMoveLinkDialog)
                {
                    // Refresh content tree when new folder is created in Copy/Move dialog
                    RefreshContentTree();
                    HandleDialogSelect();
                }
                break;

            case "cancelfolder":
                ScriptHelper.RegisterStartupScript(Page, typeof(Page), "EnsureTopWindow", ScriptHelper.GetScript("if (self.focus) { self.focus(); }"));
                ClearActionElems();
                break;

            case "attachmentmoveup":
                HandleAttachmentMoveAction(CurrentArgument, CurrentAction);
                break;

            case "attachmentmovedown":
                HandleAttachmentMoveAction(CurrentArgument, CurrentAction);
                break;

            case "attachmentdelete":
                HandleDeleteAttachmentAction(CurrentArgument);
                break;

            case "attachmentcreated":
                HandleAttachmentCreatedAction(CurrentArgument);
                break;

            case "attachmentupdated":
                HandleAttachmentUpdatedAction(CurrentArgument);
                break;

            case "attachmentedit":
                HandleAttachmentEdit(CurrentArgument);
                break;

            case "contentedit":
                HandleContentEdit(CurrentArgument);
                break;

            case "metafiledelete":
                HandleMetaFileDelete(CurrentArgument);
                break;

            case "metafileedit":
                HandleMetaFileEdit(CurrentArgument);
                break;

            case "metafileupdated":
                HandleMetaFileUpdated(CurrentArgument);
                break;

            default:
                ColorizeLastSelectedRow();
                pnlUpdateView.Update();
                break;
        }
    }

    #endregion
}
