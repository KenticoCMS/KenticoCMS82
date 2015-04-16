using System;
using System.Collections;
using System.Text;
using System.Linq;

using CMS.ExtendedControls;
using CMS.Helpers;
using CMS.IO;
using CMS.Base;
using CMS.SiteProvider;
using CMS.DocumentEngine;
using CMS.UIControls;
using CMS.PortalEngine;
using CMS.DataEngine;

public partial class CMSModules_Content_Controls_Dialogs_Selectors_LinkMediaSelector_MediaView : MediaView
{
    #region "Private variables"

    private SiteInfo mSiteObj;
    private TreeNode mTreeNodeObj;

    protected string mSaveText = null;

    #endregion


    #region "Public properties"

    /// <summary>
    /// Gets or sets a view mode used to display files.
    /// </summary>
    public override DialogViewModeEnum ViewMode
    {
        get
        {
            return base.ViewMode;
        }
        set
        {
            base.ViewMode = value;
            innermedia.ViewMode = value;
        }
    }

    /// <summary>
    /// Gets or sets the OutputFormat (needed for correct dialog type reckognition).
    /// </summary>
    public OutputFormatEnum OutputFormat
    {
        get
        {
            return innermedia.OutputFormat;
        }
        set
        {
            innermedia.OutputFormat = value;
        }
    }


    /// <summary>
    /// Gets or sets text of the information label.
    /// </summary>
    public string InfoText
    {
        get
        {
            return innermedia.InfoText;
        }
        set
        {
            innermedia.InfoText = value;
        }
    }


    /// <summary>
    /// Gets currently selected page size.
    /// </summary>
    public int CurrentPageSize
    {
        get
        {
            return innermedia.CurrentPageSize;
        }
    }


    /// <summary>
    /// Gets currently selected page size.
    /// </summary>
    public int CurrentOffset
    {
        get
        {
            return innermedia.CurrentOffset;
        }
    }


    /// <summary>
    /// Gets or sets currently selected page.
    /// </summary>
    public int CurrentPage
    {
        get
        {
            return innermedia.CurrentPage;
        }
        set
        {
            innermedia.CurrentPage = value;
        }
    }


    /// <summary>
    /// Gets or sets ID of the parent node.
    /// </summary>
    public int AttachmentNodeParentID
    {
        get;
        set;
    }


    /// <summary>
    /// Gets a UniGrid control used to display files in LIST view mode.
    /// </summary>
    public UniGrid ListViewControl
    {
        get
        {
            return innermedia.ListViewControl;
        }
    }


    /// <summary>
    /// Gets the node attachments are related to.
    /// </summary>
    public TreeNode TreeNodeObj
    {
        get
        {
            return mTreeNodeObj;
        }
        set
        {
            mTreeNodeObj = value;
            innermedia.TreeNodeObj = value;
        }
    }


    /// <summary>
    /// Gets the site attachments are related to.
    /// </summary>
    public SiteInfo SiteObj
    {
        get
        {
            if (mSiteObj == null)
            {
                mSiteObj = TreeNodeObj != null ? SiteInfoProvider.GetSiteInfo(TreeNodeObj.NodeSiteID) : SiteContext.CurrentSite;
            }
            return mSiteObj;
        }
        set
        {
            mSiteObj = value;
        }
    }


    /// <summary>
    /// Indicates whether the content tree is displaying more than max tree nodes.
    /// </summary>
    public bool IsFullListingMode
    {
        get
        {
            return innermedia.IsFullListingMode;
        }
        set
        {
            innermedia.IsFullListingMode = value;
        }
    }

    #endregion


    #region "Page methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        // If processing the request should not continue
        if (StopProcessing)
        {
            Visible = false;
        }
        else
        {
            Visible = true;

            // Initialize controls
            SetupControls();
        }
    }

    #endregion


    #region "Public methods"

    /// <summary>
    /// Loads control's content.
    /// </summary>
    public void Reload()
    {
        // Initialize controls
        SetupControls();
        ReloadData();
    }


    /// <summary>
    /// Displays listing info message.
    /// </summary>
    /// <param name="infoMsg">Info message to display</param>
    public void DisplayListingInfo(string infoMsg)
    {
        if (!string.IsNullOrEmpty(infoMsg))
        {
            plcListingInfo.Visible = true;
            lblListingInfo.Text = infoMsg;
        }
    }

    #endregion


    #region "Private methods"

    /// <summary>
    /// Initializes all nested controls.
    /// </summary>
    private void SetupControls()
    {
        InitializeControlScripts();

        // Initialize inner view control
        innermedia.ViewMode = ViewMode;
        innermedia.DataSource = DataSource;
        innermedia.TotalRecords = TotalRecords;
        innermedia.SelectableContent = SelectableContent;
        innermedia.SourceType = SourceType;
        innermedia.IsLiveSite = IsLiveSite;
        innermedia.NodeParentID = AttachmentNodeParentID;

        innermedia.ResizeToHeight = ResizeToHeight;
        innermedia.ResizeToMaxSideSize = ResizeToMaxSideSize;
        innermedia.ResizeToWidth = ResizeToWidth;

        // Set grid definition according source type
        string gridName;
        if (SourceType == MediaSourceEnum.DocumentAttachments)
        {
            gridName = "~/CMSModules/Content/Controls/Dialogs/Selectors/LinkMediaSelector/AttachmentsListView.xml";
        }
        else if (SourceType == MediaSourceEnum.MetaFile)
        {
            gridName = "~/CMSModules/Content/Controls/Dialogs/Selectors/LinkMediaSelector/MetaFileListView.xml";
        }
        else
        {
            string[] linkCustomFormatCodes = { "linkdoc", "relationship", "selectpath" };

            if ((OutputFormat == OutputFormatEnum.HTMLLink) || (OutputFormat == OutputFormatEnum.BBLink) || ((OutputFormat == OutputFormatEnum.Custom) && linkCustomFormatCodes.Contains(Config.CustomFormatCode)))
            {
                gridName = "~/CMSModules/Content/Controls/Dialogs/Selectors/LinkMediaSelector/ContentListView_Link.xml";
            }
            else
            {
                gridName = "~/CMSModules/Content/Controls/Dialogs/Selectors/LinkMediaSelector/ContentListView.xml";
            }
        }
        innermedia.ListViewControl.GridName = gridName;

        innermedia.ListViewControl.OnPageChanged -= ListViewControl_OnPageChanged;
        innermedia.ListViewControl.OnPageChanged += ListViewControl_OnPageChanged;


        if (innermedia.ThumbnailsViewControl.UniPagerControl != null)
        {
            innermedia.ThumbnailsViewControl.UniPagerControl.OnPageChanged -= UniPagerControl_OnPageChanged;
            innermedia.ThumbnailsViewControl.UniPagerControl.OnPageChanged += UniPagerControl_OnPageChanged;
        }

        // Set inner control binding columns
        if (SourceType != MediaSourceEnum.MetaFile)
        {
            innermedia.FileIdColumn = "AttachmentGUID";
            innermedia.FileNameColumn = (SourceType == MediaSourceEnum.DocumentAttachments) ? "AttachmentName" : "DocumentName";
            innermedia.FileExtensionColumn = "AttachmentExtension";
            innermedia.FileSizeColumn = "AttachmentSize";
            innermedia.FileWidthColumn = "AttachmentImageWidth";
            innermedia.FileHeightColumn = "AttachmentImageHeight";
        }
        else
        {
            innermedia.FileIdColumn = "MetaFileID";
            innermedia.FileNameColumn = "MetaFileName";
            innermedia.FileExtensionColumn = "MetaFileExtension";
            innermedia.FileSizeColumn = "MetaFileSize";
            innermedia.FileWidthColumn = "MetaFileImageWidth";
            innermedia.FileHeightColumn = "MetaFileImageHeight";
        }

        // Register for inner media events
        innermedia.GetArgumentSet += innermedia_GetArgumentSet;
        innermedia.GetListItemUrl += innermedia_GetListItemUrl;
        innermedia.GetThumbsItemUrl += innermedia_GetThumbsItemUrl;
    }


    /// <summary>
    /// Initializes scripts used by the control.
    /// </summary>
    private void InitializeControlScripts()
    {
        ScriptHelper.RegisterStartupScript(this, GetType(), "DialogsSelectAction", ScriptHelper.GetScript(@"
function SetSelectAction(argument) {
    // Raise select action
    SetAction('select', argument);
    RaiseHiddenPostBack();
}
function SetParentAction(argument) {
    // Raise select action
    SetAction('parentselect', argument);
    RaiseHiddenPostBack();
}"));
    }


    /// <summary>
    /// Loads data from data source property.
    /// </summary>
    private void ReloadData()
    {
        innermedia.Reload(true);
    }

    #endregion


    #region "Inner media view event handlers"

    /// <summary>
    /// Returns argument set according passed DataRow and flag indicating whether the set is obtained for selected item.
    /// </summary>
    /// <param name="data">DataRow with all the item data</param>
    private string innermedia_GetArgumentSet(IDataContainer data)
    {
        // Return required argument set
        return GetArgumentSet(data);
    }


    private string innermedia_GetListItemUrl(IDataContainer data, bool isPreview, bool notAttachment)
    {
        // Get set of important information
        string arg = GetArgumentSet(data);

        // Get URL of the list item image
        return GetItemUrl(arg, 0, 0, 0, notAttachment);
    }


    private IconParameters innermedia_GetThumbsItemUrl(IDataContainer data, bool isPreview, int height, int width, int maxSideSize, bool notAttachment)
    {
        IconParameters parameters = new IconParameters();

        string ext = (SourceType != MediaSourceEnum.MetaFile) ? data.GetValue("AttachmentExtension").ToString() : data.GetValue("MetaFileExtension").ToString();
        string arg = GetArgumentSet(data);

        // If image is requested for preview
        if (isPreview)
        {
            if (!ImageHelper.IsImage(ext) || notAttachment)
            {
                string className = (SourceType == MediaSourceEnum.Content) ? data.GetValue("ClassName").ToString().ToLowerCSafe() : "";
                if (className == "cms.file")
                {
                    // File isn't image and no preview exists - get default file icon
                    parameters.IconClass = UIHelper.GetFileIconClass(ext);
                }
                else if (((SourceType == MediaSourceEnum.DocumentAttachments) || (SourceType == MediaSourceEnum.Attachment) || (SourceType == MediaSourceEnum.MetaFile)) && !String.IsNullOrEmpty(ext))
                {
                    // Get file icon for attachment
                    parameters.IconClass = UIHelper.GetFileIconClass(ext);
                }
                else
                {
                    var dataClass = DataClassInfoProvider.GetDataClassInfo(className);
                    parameters.Url = UIHelper.GetDocumentTypeIconUrl(Page, className, "48x48");

                    if (dataClass != null)
                    {
                        parameters.IconClass = (string)dataClass.GetValue("ClassIconClass");
                    }
                }

                // Set font icon size
                if (!string.IsNullOrEmpty(parameters.IconClass))
                {
                    parameters.IconSize = FontIconSizeEnum.Dashboard;
                }
            }
            else
            {
                // Try to get preview or image itself
                parameters.Url = GetItemUrl(arg, height, width, maxSideSize, notAttachment);
            }
        }
        else
        {
            parameters.Url = GetItemUrl(arg, 0, 0, 0, notAttachment);
        }

        return parameters;
    }


    private void ListViewControl_OnPageChanged(object sender, EventArgs e)
    {
        RaiseListReloadRequired();
    }


    void UniPagerControl_OnPageChanged(object sender, int pageNumber)
    {
        RaiseListReloadRequired();
    }

    #endregion


    #region "Helper methods"

    /// <summary>
    /// Returns argument set for the passed file data row.
    /// </summary>
    /// <param name="data">Data row object holding all the data on current file</param>
    public string GetArgumentSet(IDataContainer data)
    {
        string className = ValidationHelper.GetString(data.GetValue("ClassName"), String.Empty).ToLowerCSafe();
        string name;

        // Get file name with extension
        switch (SourceType)
        {
            case MediaSourceEnum.DocumentAttachments:
                name = AttachmentHelper.GetFullFileName(Path.GetFileNameWithoutExtension(data.GetValue("AttachmentName").ToString()), data.GetValue("AttachmentExtension").ToString());
                break;
            case MediaSourceEnum.MetaFile:
                name = MetaFileInfoProvider.GetFullFileName(Path.GetFileNameWithoutExtension(data.GetValue("MetaFileName").ToString()), data.GetValue("MetaFileExtension").ToString());
                break;
            default:
                name = data.GetValue("DocumentName").ToString();
                break;
        }

        StringBuilder sb = new StringBuilder();

        // Common information for both content & attachments
        sb.Append("name|" + CMSDialogHelper.EscapeArgument(name));

        // Load attachment info only for CMS.File document type
        if (((SourceType != MediaSourceEnum.Content) && (SourceType != MediaSourceEnum.MetaFile)) || (className == "cms.file"))
        {
            sb.Append("|AttachmentExtension|" + CMSDialogHelper.EscapeArgument(data.GetValue("AttachmentExtension")));
            sb.Append("|AttachmentImageWidth|" + CMSDialogHelper.EscapeArgument(data.GetValue("AttachmentImageWidth")));
            sb.Append("|AttachmentImageHeight|" + CMSDialogHelper.EscapeArgument(data.GetValue("AttachmentImageHeight")));
            sb.Append("|AttachmentSize|" + CMSDialogHelper.EscapeArgument(data.GetValue("AttachmentSize")));
            sb.Append("|AttachmentGUID|" + CMSDialogHelper.EscapeArgument(data.GetValue("AttachmentGUID")));
        }
        else if (SourceType == MediaSourceEnum.MetaFile)
        {
            sb.Append("|MetaFileExtension|" + CMSDialogHelper.EscapeArgument(data.GetValue("MetaFileExtension")));
            sb.Append("|MetaFileImageWidth|" + CMSDialogHelper.EscapeArgument(data.GetValue("MetaFileImageWidth")));
            sb.Append("|MetaFileImageHeight|" + CMSDialogHelper.EscapeArgument(data.GetValue("MetaFileImageHeight")));
            sb.Append("|MetaFileSize|" + CMSDialogHelper.EscapeArgument(data.GetValue("MetaFileSize")));
            sb.Append("|MetaFileGUID|" + CMSDialogHelper.EscapeArgument(data.GetValue("MetaFileGUID")));
            sb.Append("|SiteID|" + CMSDialogHelper.EscapeArgument(data.GetValue("MetaFileSiteID")));
        }
        else
        {
            sb.Append("|AttachmentExtension||AttachmentImageWidth||AttachmentImageHeight||AttachmentSize||AttachmentGUID|");
        }

        // Get source type specific information
        if (SourceType == MediaSourceEnum.Content)
        {
            sb.Append("|NodeSiteID|" + CMSDialogHelper.EscapeArgument(data.GetValue("NodeSiteID")));
            sb.Append("|SiteName|" + CMSDialogHelper.EscapeArgument(data.GetValue("SiteName")));
            sb.Append("|NodeGUID|" + CMSDialogHelper.EscapeArgument(data.GetValue("NodeGUID")));
            sb.Append("|NodeID|" + CMSDialogHelper.EscapeArgument(data.GetValue("NodeID")));
            sb.Append("|NodeAlias|" + CMSDialogHelper.EscapeArgument(data.GetValue("NodeAlias")));
            sb.Append("|NodeAliasPath|" + CMSDialogHelper.EscapeArgument(data.GetValue("NodeAliasPath")));
            sb.Append("|DocumentUrlPath|" + CMSDialogHelper.EscapeArgument(data.GetValue("DocumentUrlPath")));
            sb.Append("|DocumentExtensions|" + CMSDialogHelper.EscapeArgument(data.GetValue("DocumentExtensions")));
            sb.Append("|ClassName|" + CMSDialogHelper.EscapeArgument(data.GetValue("ClassName")));
            sb.Append("|NodeLinkedNodeID|" + CMSDialogHelper.EscapeArgument(data.GetValue("NodeLinkedNodeID")));
        }
        else if (SourceType != MediaSourceEnum.MetaFile)
        {
            string formGuid = data.ContainsColumn("AttachmentFormGUID") ? data.GetValue("AttachmentFormGUID").ToString() : Guid.Empty.ToString();
            string siteId = data.ContainsColumn("AttachmentSiteID") ? data.GetValue("AttachmentSiteID").ToString() : "0";

            sb.Append("|SiteID|" + CMSDialogHelper.EscapeArgument(siteId));
            sb.Append("|FormGUID|" + CMSDialogHelper.EscapeArgument(formGuid));
            sb.Append("|AttachmentDocumentID|" + CMSDialogHelper.EscapeArgument(data.GetValue("AttachmentDocumentID")));
        }

        return sb.ToString();
    }


    /// <summary>
    /// Returns arguments table for the passed argument.
    /// </summary>
    /// <param name="argument">Argument containing information on current media item</param>
    public static Hashtable GetArgumentsTable(string argument)
    {
        Hashtable table = new Hashtable();

        string[] argArr = argument.Split('|');
        try
        {
            // Fill table
            for (int i = 0; i < argArr.Length; i = i + 2)
            {
                table[argArr[i].ToLowerCSafe()] = CMSDialogHelper.UnEscapeArgument(argArr[i + 1]);
            }
        }
        catch
        {
            throw new Exception("[Media view]: Error loading arguments table.");
        }

        return table;
    }


    /// <summary>
    /// Returns URL of the media item according site settings.
    /// </summary>
    /// <param name="argument">Argument containing information on current media item</param>
    /// <param name="height">Item height in px</param>
    /// <param name="width">Item width in px</param>
    /// <param name="maxSideSize">Maximum dimension for images displayed for thumbnails view</param>
    /// <param name="notAttachment">True if this is not an attachement</param>
    public string GetItemUrl(string argument, int height, int width, int maxSideSize, bool notAttachment)
    {
        Hashtable argTable = GetArgumentsTable(argument);
        if (argTable.Count >= 2)
        {
            string url;

            // Get image URL
            switch (SourceType)
            {
                case MediaSourceEnum.Content:
                    {
                        // Get information from argument
                        Guid nodeGuid = ValidationHelper.GetGuid(argTable["nodeguid"], Guid.Empty);
                        string documentUrlPath = argTable["documenturlpath"].ToString();
                        string nodeAlias = argTable["nodealias"].ToString();
                        string nodeAliasPath = argTable["nodealiaspath"].ToString();
                        string documentExtensions = argTable["documentextensions"].ToString();
                        string documentClass = argTable["classname"].ToString();
                        bool nodeIsLink = (ValidationHelper.GetInteger(argTable["nodelinkednodeid"], 0) != 0);
                        string siteName = argTable["sitename"].ToString();

                        // Get default url extension for current item
                        string fileExt = !String.IsNullOrEmpty(documentClass) && (documentClass.ToLowerCSafe() == "cms.file") ? TreePathUtils.GetFilesUrlExtension(siteName) : TreePathUtils.GetUrlExtension(siteName);

                        // If extension less try to get custom document extension
                        if (String.IsNullOrEmpty(fileExt))
                        {
                            fileExt = (!String.IsNullOrEmpty(documentExtensions) ? documentExtensions.Split(';')[0] : String.Empty);
                        }

                        // Get content item URL
                        url = GetContentItemUrl(nodeGuid, documentUrlPath, nodeAlias, nodeAliasPath, nodeIsLink, height, width, maxSideSize, notAttachment, fileExt);
                    }
                    break;
                case MediaSourceEnum.MetaFile:
                    {
                        // Get information from argument
                        Guid attachmentGuid = ValidationHelper.GetGuid(argTable["metafileguid"], Guid.Empty);
                        string attachmentName = argTable["name"].ToString();

                        // Get item URL
                        url = GetMetaFileItemUrl(attachmentGuid, attachmentName, height, width, maxSideSize);
                    }
                    break;
                default:
                    {
                        // Get information from argument
                        Guid attachmentGuid = ValidationHelper.GetGuid(argTable["attachmentguid"], Guid.Empty);
                        string attachmentName = argTable["name"].ToString();
                        string nodeAliasPath = string.Empty;
                        if (TreeNodeObj != null)
                        {
                            nodeAliasPath = TreeNodeObj.NodeAliasPath;
                        }

                        // Get item URL
                        url = GetAttachmentItemUrl(attachmentGuid, attachmentName, nodeAliasPath, height, width, maxSideSize);
                    }
                    break;
            }

            return url;
        }

        return string.Empty;
    }


    /// <summary>
    /// Ensures no item is selected.
    /// </summary>
    public void ResetSearch()
    {
        dialogSearch.ResetSearch();
    }


    /// <summary>
    /// Ensures first page is displayed in the control displaying the content.
    /// </summary>
    public void ResetPageIndex()
    {
        innermedia.ResetPageIndex();
    }


    /// <summary>
    /// Ensure no item is selected in list view.
    /// </summary>
    public void ResetListSelection()
    {
        innermedia.ResetListSelection();
    }


    /// <summary>
    /// Returns URL, resolved if not in media selector, with size parameters added.
    /// </summary>
    /// <param name="url">Media file URL</param>
    /// <param name="height">Height parameter that should be added to the URL</param>
    /// <param name="width">Width parameter that should be added to the URL</param>
    /// <param name="maxSideSize">Max side size parameter that should be added to the URL</param>
    public string AddURLDimensionsAndResolve(string url, int height, int width, int maxSideSize)
    {
        if (string.IsNullOrEmpty(url))
        {
            return url;
        }

        string result = url;

        // If image dimensions are specified
        if (maxSideSize > 0)
        {
            result = URLHelper.AddParameterToUrl(result, "maxsidesize", maxSideSize.ToString());
        }
        if (height > 0)
        {
            result = URLHelper.AddParameterToUrl(result, "height", height.ToString());
        }
        if (width > 0)
        {
            result = URLHelper.AddParameterToUrl(result, "width", width.ToString());
        }

        // Media selector should returns non-resolved URL in all cases
        bool isMediaSelector = (OutputFormat == OutputFormatEnum.URL) && (SelectableContent == SelectableContentEnum.OnlyMedia);

        return (isMediaSelector || ((Config != null) && Config.ContentUseRelativeUrl)) ? result : URLHelper.ResolveUrl(result, true, false);
    }

    #endregion


    #region "Content methods"

    /// <summary>
    /// Returns URL of the media item according site settings.
    /// </summary>
    /// <param name="nodeGuid">Node GUID of the current attachment node</param>    
    /// <param name="documentUrlPath">URL path of the current attachment document</param>
    /// <param name="nodeAlias">Node alias of the current attachment node</param>
    /// <param name="nodeAliasPath">Node alias path of the current attachment node</param>
    /// <param name="nodeIsLink">Indicates if node is linked node.</param>
    /// <param name="height">Height of the attachment</param>
    /// <param name="width">Width of the attachment</param>
    /// <param name="maxSideSize">Maximum dimension for images displayed for thumbnails view</param>
    /// <param name="notAttachment">Indicates if item is not attachment</param>
    /// <param name="documentExtension">Document extension</param>
    public string GetContentItemUrl(Guid nodeGuid, string documentUrlPath, string nodeAlias, string nodeAliasPath, bool nodeIsLink, int height, int width, int maxSideSize, bool notAttachment, string documentExtension)
    {
        string result;

        if (documentExtension.Contains(";"))
        {
            documentExtension = documentExtension.Split(';')[0];
        }

        // Generate URL
        if (UsePermanentUrls)
        {
            bool isLink = ((OutputFormat == OutputFormatEnum.BBLink) || (OutputFormat == OutputFormatEnum.HTMLLink)) ||
                          ((OutputFormat == OutputFormatEnum.URL) && (SelectableContent == SelectableContentEnum.AllContent));

            if (String.IsNullOrEmpty(nodeAlias))
            {
                nodeAlias = "default";
            }

            if (notAttachment || isLink)
            {
                result = DocumentURLProvider.GetPermanentDocUrl(nodeGuid, nodeAlias, SiteObj.SiteName, null, documentExtension);
            }
            else
            {
                result = AttachmentInfoProvider.GetPermanentAttachmentUrl(nodeGuid, nodeAlias, documentExtension);
            }
        }
        else
        {
            string docUrlPath = nodeIsLink ? null : documentUrlPath;

            // Ensure live site view mode for URLs edited in on-site edit mode
            if (PortalContext.ViewMode.IsEditLive())
            {
                PortalContext.SetRequestViewMode(ViewModeEnum.LiveSite);
            }

            result = DocumentURLProvider.GetUrl(nodeAliasPath, docUrlPath, SiteObj.SiteName, null, documentExtension);
        }

        // Make URL absolute if required
        int currentSiteId = SiteContext.CurrentSiteID;
        if (Config.UseFullURL || (currentSiteId != SiteObj.SiteID) || (currentSiteId != GetCurrentSiteId()))
        {
            result = URLHelper.GetAbsoluteUrl(result, SiteObj.DomainName, URLHelper.GetApplicationUrl(SiteObj.DomainName), null);
        }

        return AddURLDimensionsAndResolve(result, height, width, maxSideSize);
    }

    #endregion


    #region "Attachment methods"

    /// <summary>
    /// Returns URL for the attachment specified by arguments.
    /// </summary>
    /// <param name="attachmentGuid">GUID of the attachment</param>
    /// <param name="attachmentName">Name of the attachment</param>
    /// <param name="attachmentNodeAlias">Attachement node alias</param>
    /// <param name="height">Height of the attachment</param>
    /// <param name="width">Width of the attachment</param>
    /// <param name="maxSideSize">Maximum size of the item if attachment is image</param>
    public string GetAttachmentItemUrl(Guid attachmentGuid, string attachmentName, string attachmentNodeAlias, int height, int width, int maxSideSize)
    {
        string result;

        if (UsePermanentUrls || string.IsNullOrEmpty(attachmentNodeAlias))
        {
            result = AttachmentInfoProvider.GetAttachmentUrl(attachmentGuid, attachmentName);
        }
        else
        {
            //string safeFileName = URLHelper.GetSafeFileName(attachmentName, attachmentSite.SiteName);
            string safeFileName = URLHelper.GetSafeFileName(attachmentName, SiteObj.SiteName);

            result = AttachmentInfoProvider.GetAttachmentUrl(safeFileName, attachmentNodeAlias);
        }

        // If current site is different from attachment site make URL absolute (domain included)
        if (Config.UseFullURL || (SiteContext.CurrentSiteID != SiteObj.SiteID) || (SiteContext.CurrentSiteID != GetCurrentSiteId()))
        {
            result = URLHelper.GetAbsoluteUrl(result, SiteObj.DomainName, URLHelper.GetApplicationUrl(SiteObj.DomainName), null);
        }

        return AddURLDimensionsAndResolve(result, height, width, maxSideSize);
    }

    #endregion


    #region "MetaFile methods"

    /// <summary>
    /// Returns URL for the metafile specified by arguments.
    /// </summary>
    /// <param name="attachmentGuid">GUID of the metafile</param>
    /// <param name="attachmentName">Name of the metafile</param>
    /// <param name="height">Height parameter that should be added to the URL</param>
    /// <param name="width">Width parameter that should be added to the URL</param>
    /// <param name="maxSideSize">Maximum size of the item if metafile is image</param>
    public string GetMetaFileItemUrl(Guid attachmentGuid, string attachmentName, int height, int width, int maxSideSize)
    {
        string result = MetaFileURLProvider.GetMetaFileUrl(attachmentGuid, attachmentName);

        // If current site is different from attachment site make URL absolute (domain included)
        if (Config.UseFullURL || (SiteContext.CurrentSiteID != SiteObj.SiteID) || (SiteContext.CurrentSiteID != GetCurrentSiteId()))
        {
            result = URLHelper.GetAbsoluteUrl(result, SiteObj.DomainName, URLHelper.GetApplicationUrl(SiteObj.DomainName), null);
        }

        return AddURLDimensionsAndResolve(result, height, width, maxSideSize);
    }

    #endregion
}