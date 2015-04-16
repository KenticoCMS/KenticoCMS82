using System;
using System.Collections.Generic;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Linq;

using CMS.Base;
using CMS.Controls;
using CMS.DataEngine;
using CMS.DocumentEngine;
using CMS.ExtendedControls;
using CMS.Globalization;
using CMS.Helpers;
using CMS.IO;
using CMS.Membership;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.WorkflowEngine;

using TreeNode = CMS.DocumentEngine.TreeNode;

public partial class CMSModules_Content_Controls_Dialogs_Selectors_LinkMediaSelector_InnerMediaView : CMSUserControl
{
    #region "Constants"

    private const double MaxThumbImgWidth = 160.0;
    private const double MaxThumbImgHeight = 95.0;

    #endregion


    #region "Private variables"

    private OutputFormatEnum mOutputFormat = OutputFormatEnum.HTMLMedia;
    private DialogConfiguration mConfig;
    private DialogViewModeEnum mViewMode = DialogViewModeEnum.ListView;
    private SelectableContentEnum mSelectableContent = SelectableContentEnum.AllContent;
    private MediaSourceEnum mSourceType = MediaSourceEnum.Content;

    private string mImagesPath = "";
    private string mFileIdColumn = "";
    private string mFileNameColumn = "";
    private string mFileExtensionColumn = "";
    private string mFileSizeColumn = "";
    private string mFileWidthColumn = "";
    private string mFileHeightColumn = "";
    private string mAllowedExtensions = "";
    private string mInfoText = "";
    private string mGMTTooltip;

    private string siteName = String.Empty;
    private int mVersionHistoryId = -1;
    private int mLastSiteId;
    private int mObjectSiteID = -1;
    private int mUpdateIconPanelWidth = 16;

    private bool columnUpdateVisible;

    private DataSet mDataSource;

    #endregion


    #region "Events & delegates"

    /// <summary>
    /// Delegate for an event occurring when argument set is required.
    /// </summary>
    /// <param name="data">DataRow holding information on currently processed file</param>    
    public delegate string OnGetArgumentSet(IDataContainer data);

    /// <summary>
    /// Event occurring when argument set is required.
    /// </summary>
    public event OnGetArgumentSet GetArgumentSet;

    /// <summary>
    /// Delegate for the event fired when URL for list image is required.
    /// </summary>
    /// <param name="data">DataRow holding information on currently processed file</param>   
    /// <param name="isPreview">Indicates whether the image is generated as part of preview</param>
    /// <param name="notAttachment">Indicates whether the URL is required for non-attachment item</param>
    public delegate string OnGetListItemUrl(IDataContainer data, bool isPreview, bool notAttachment);

    /// <summary>
    /// Event occurring when URL for list item image is required.
    /// </summary>
    public event OnGetListItemUrl GetListItemUrl;

    /// <summary>
    /// Delegate for the event fired when URL for thumbnails image is required.
    /// </summary>
    /// <param name="data">DataRow holding information on currently processed file</param>  
    /// <param name="isPreview">Indicates whether the image is generated as part of preview</param>
    /// <param name="width">Width of preview image</param>
    /// <param name="maxSideSize">Maximum size of the preview image. If full-size required parameter gets zero value</param>
    /// <param name="notAttachment">Indicates whether the URL is required for non-attachment item</param>
    /// <param name="height">Height of preview image</param>
    public delegate IconParameters OnGetThumbsItemUrl(IDataContainer data, bool isPreview, int height, int width, int maxSideSize, bool notAttachment);

    /// <summary>
    /// Event occurring when URL for thumbnails image is required.
    /// </summary>
    public event OnGetThumbsItemUrl GetThumbsItemUrl;

    /// <summary>
    /// Delegate for the event occurring when information on file import status is required.
    /// </summary>
    /// <param name="type">Type of the required information</param>
    /// <param name="parameter">Parameter related</param>
    public delegate object OnGetInformation(string type, object parameter);

    /// <summary>
    /// Event occurring when information on file import status is required.
    /// </summary>
    public event OnGetInformation GetInformation;

    /// <summary>
    /// Delegate for the event occurring when permission modify is required.
    /// </summary>
    /// <param name="data">DataRow holding information on currently processed file</param>
    public delegate bool OnGetModifyPermission(IDataContainer data);

    /// <summary>
    /// Event occurring when permission modify is required.
    /// </summary>
    public event OnGetModifyPermission GetModifyPermission;

    #endregion


    #region "Public properties"

    /// <summary>
    /// Gets or sets the OutputFormat (needed for correct dialog type recognition).
    /// </summary>
    public OutputFormatEnum OutputFormat
    {
        get
        {
            return mOutputFormat;
        }
        set
        {
            mOutputFormat = value;
        }
    }


    /// <summary>
    /// Gets current dialog configuration.
    /// </summary>
    public DialogConfiguration Config
    {
        get
        {
            return mConfig ?? (mConfig = DialogConfiguration.GetDialogConfiguration());
        }
    }


    /// <summary>
    /// Gets a UniGrid control used to display files in LIST view mode.
    /// </summary>
    public UniGrid ListViewControl
    {
        get
        {
            return gridList;
        }
    }


    /// <summary>
    /// Gets a repeater control used to display files in THUMBNAILS view mode.
    /// </summary>
    public BasicRepeater ThumbnailsViewControl
    {
        get
        {
            return repThumbnailsView;
        }
    }


    /// <summary>
    /// Gets list of names of selected files.
    /// </summary>
    public List<string> SelectedItems
    {
        get
        {
            return GetSelectedItems();
        }
    }


    /// <summary>
    /// Gets or sets a view mode used to display files.
    /// </summary>
    public DialogViewModeEnum ViewMode
    {
        get
        {
            return mViewMode;
        }
        set
        {
            mViewMode = value;
        }
    }


    /// <summary>
    /// Gets or sets source of the data for view controls.
    /// </summary>
    public DataSet DataSource
    {
        get
        {
            return mDataSource;
        }
        set
        {
            mDataSource = value;
        }
    }


    /// <summary>
    /// Gets or sets total records count for pagination.
    /// </summary>
    public int TotalRecords
    {
        get;
        set;
    }


    /// <summary>
    /// Type of the content which can be selected.
    /// </summary>
    public SelectableContentEnum SelectableContent
    {
        get
        {
            return mSelectableContent;
        }
        set
        {
            mSelectableContent = value;
        }
    }


    /// <summary>
    /// Gets or sets name of the column holding information on the file identifier.
    /// </summary>
    public string FileIdColumn
    {
        get
        {
            return mFileIdColumn;
        }
        set
        {
            mFileIdColumn = value;
        }
    }

    /// <summary>
    /// Gets or sets name of the column holding information on file name.
    /// </summary>
    public string FileNameColumn
    {
        get
        {
            return mFileNameColumn;
        }
        set
        {
            mFileNameColumn = value;
        }
    }


    /// <summary>
    /// Gets or sets name of the column holding information on file extension.
    /// </summary>
    public string FileExtensionColumn
    {
        get
        {
            return mFileExtensionColumn;
        }
        set
        {
            mFileExtensionColumn = value;
        }
    }


    /// <summary>
    /// Gets or sets name of the column holding information on file width.
    /// </summary>
    public string FileWidthColumn
    {
        get
        {
            return mFileWidthColumn;
        }
        set
        {
            mFileWidthColumn = value;
        }
    }


    /// <summary>
    /// Gets or sets name of the column holding information on file height.
    /// </summary>
    public string FileHeightColumn
    {
        get
        {
            return mFileHeightColumn;
        }
        set
        {
            mFileHeightColumn = value;
        }
    }


    /// <summary>
    /// Gets or sets name of the column holding information on file size.
    /// </summary>
    public string FileSizeColumn
    {
        get
        {
            return mFileSizeColumn;
        }
        set
        {
            mFileSizeColumn = value;
        }
    }


    /// <summary>
    /// Gets or sets text of the information label.
    /// </summary>
    public string InfoText
    {
        get
        {
            return mInfoText;
        }
        set
        {
            mInfoText = value;
        }
    }


    /// <summary>
    /// Gets the node attachments are related to.
    /// </summary>
    public TreeNode TreeNodeObj
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets information on source type.
    /// </summary>
    public MediaSourceEnum SourceType
    {
        get
        {
            return mSourceType;
        }
        set
        {
            mSourceType = value;
        }
    }


    /// <summary>
    /// Height of attachment.
    /// </summary>
    public int ResizeToHeight
    {
        get;
        set;
    }


    /// <summary>
    /// Width of attachment.
    /// </summary>
    public int ResizeToWidth
    {
        get;
        set;
    }


    /// <summary>
    /// Max side size of attachment.
    /// </summary>
    public int ResizeToMaxSideSize
    {
        get;
        set;
    }


    /// <summary>
    /// Returns current page size.
    /// </summary>
    public int CurrentPageSize
    {
        get
        {
            int pageSize = 0;

            switch (ViewMode)
            {
                case DialogViewModeEnum.ThumbnailsView:
                    pageSize = pagerElemThumbnails.CurrentPageSize;
                    break;

                case DialogViewModeEnum.ListView:
                    pageSize = (gridList.Pager.CurrentPageSize == -1) ? 0 : ValidationHelper.GetInteger(gridList.Pager.CurrentPageSize, 10);
                    break;
            }

            return pageSize;
        }
    }


    /// <summary>
    /// Returns current offset.
    /// </summary>
    public int CurrentOffset
    {
        get
        {
            int offset = 0;

            switch (ViewMode)
            {
                case DialogViewModeEnum.ThumbnailsView:
                    offset = pagerElemThumbnails.CurrentPageSize * (pagerElemThumbnails.UniPager.CurrentPage - 1);
                    break;

                case DialogViewModeEnum.ListView:
                    offset = gridList.Pager.CurrentPageSize * (gridList.Pager.CurrentPage - 1);
                    break;
            }

            return offset;
        }
    }


    /// <summary>
    /// Gets or sets current page.
    /// </summary>
    public int CurrentPage
    {
        get
        {
            int page = 0;

            switch (ViewMode)
            {
                case DialogViewModeEnum.ThumbnailsView:
                    page = pagerElemThumbnails.UniPager.CurrentPage;
                    break;

                case DialogViewModeEnum.ListView:
                    page = gridList.Pager.CurrentPage;
                    break;
            }

            return page;
        }
        set
        {
            switch (ViewMode)
            {
                case DialogViewModeEnum.ThumbnailsView:
                    pagerElemThumbnails.UniPager.CurrentPage = value;
                    break;

                case DialogViewModeEnum.ListView:
                    gridList.Pager.CurrentPage = value;
                    break;
            }
        }
    }


    /// <summary>
    /// Indicates if full listing mode is enabled. This mode enables navigation to child and parent folders/documents from current view.
    /// </summary>
    public bool IsFullListingMode
    {
        get
        {
            return ValidationHelper.GetBoolean(ViewState["IsFullListingMode"], false);
        }
        set
        {
            ViewState["IsFullListingMode"] = value;
        }
    }


    /// <summary>
    /// Indicates whether the control is displayed as part of the copy/move dialog.
    /// </summary>
    public bool IsCopyMoveLinkDialog
    {
        get;
        set;
    }


    /// <summary>
    /// ID of the related node version history.
    /// </summary>
    private int VersionHistoryID
    {
        get
        {
            if (mVersionHistoryId < 0)
            {
                mVersionHistoryId = 0;

                // Load the version history
                if (TreeNodeObj != null)
                {
                    // Get the node workflow
                    WorkflowManager wm = WorkflowManager.GetInstance(TreeNodeObj.TreeProvider);
                    WorkflowInfo wi = wm.GetNodeWorkflow(TreeNodeObj);
                    if (wi != null)
                    {
                        // Ensure the document version
                        VersionManager vm = VersionManager.GetInstance(TreeNodeObj.TreeProvider);
                        VersionHistoryID = vm.EnsureVersion(TreeNodeObj, TreeNodeObj.IsPublished);
                    }
                }
            }

            return mVersionHistoryId;
        }
        set
        {
            mVersionHistoryId = value;
        }
    }

    #endregion]


    #region "Private properties"

    /// <summary>
    /// Image relative path.
    /// </summary>
    private string ImagesPath
    {
        get
        {
            if (mImagesPath == "")
            {
                mImagesPath = GetImageUrl("Design/Controls/UniGrid/Actions/", IsLiveSite, true);
            }
            return mImagesPath;
        }
    }

    #endregion


    #region "Attachment properties"

    /// <summary>
    /// Gets all allowed extensions.
    /// </summary>
    public string AllowedExtensions
    {
        get
        {
            if (mAllowedExtensions == "")
            {
                mAllowedExtensions = QueryHelper.GetString("allowedextensions", "");
                if (mAllowedExtensions == "")
                {
                    mAllowedExtensions = SettingsKeyInfoProvider.GetValue(SiteContext.CurrentSiteName + ".CMSUploadExtensions");
                }
            }
            return mAllowedExtensions;
        }
        private set
        {
            mAllowedExtensions = value;
        }
    }


    /// <summary>
    /// Gets or sets ID of the parent node.
    /// </summary>
    public int NodeParentID
    {
        get;
        set;
    }

    #endregion


    #region "Constructors"

    public CMSModules_Content_Controls_Dialogs_Selectors_LinkMediaSelector_InnerMediaView()
    {
        NodeParentID = 0;
        IsCopyMoveLinkDialog = false;
        ResizeToWidth = 0;
        ResizeToHeight = 0;
        TreeNodeObj = null;
        DataSource = null;
        ResizeToMaxSideSize = 0;
    }

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        Visible = !StopProcessing;
        if (!StopProcessing)
        {
            gridList.IsLiveSite = IsLiveSite;
            if (URLHelper.IsPostback())
            {
                Reload(true);
            }
        }
    }


    protected override void OnPreRender(EventArgs e)
    {
        // Display information on empty data
        bool isEmpty = DataHelper.DataSourceIsEmpty(DataSource);
        if (isEmpty)
        {
            plcViewArea.Visible = false;
        }
        else
        {
            lblInfo.Visible = false;
            plcViewArea.Visible = true;
        }

        // If info text is set display it
        if (!string.IsNullOrEmpty(InfoText))
        {
            lblInfo.Text = InfoText;
            lblInfo.Visible = true;
        }
        else if (isEmpty)
        {
            lblInfo.Text = (IsCopyMoveLinkDialog ? GetString("media.copymove.empty") : CMSDialogHelper.GetNoItemsMessage(Config, SourceType));
            lblInfo.Visible = true;
        }

        // Hide column 'Update' in media libraries
        if (((SourceType == MediaSourceEnum.MediaLibraries) || (SourceType == MediaSourceEnum.Content)) && !columnUpdateVisible)
        {
            if (gridList.NamedColumns.ContainsKey("extedit"))
            {
                gridList.NamedColumns["extedit"].Visible = false;
            }
        }

        base.OnPreRender(e);
    }

    #endregion


    #region "Public methods"

    /// <summary>
    /// Loads view controls according currently set view mode.
    /// </summary>
    public void LoadViewControls()
    {
        InfoText = "";

        // Select mode according required
        switch (ViewMode)
        {
            case DialogViewModeEnum.ListView:
                plcListView.Visible = true;

                // Stop processing                
                plcThumbnailsView.Visible = false;
                break;

            case DialogViewModeEnum.ThumbnailsView:
                plcThumbnailsView.Visible = true;
                repThumbnailsView.DataBindByDefault = false;
                pagerElemThumbnails.UniPager.ConnectToPagedControl(repThumbnailsView);

                // Stop processing
                plcListView.Visible = false;
                break;
        }

        // Display mass actions drop-down list if displayed for MediaLibrary UI
        if (!IsCopyMoveLinkDialog && (DisplayMode == ControlDisplayModeEnum.Simple))
        {
            pnlMassAction.Visible = true;

            InitializeMassActions();
        }
        else
        {
            pnlMassAction.Visible = false;
        }
    }

    #endregion


    #region "Mass actions methods"

    /// <summary>
    /// Initializes mass actions drop-down list with available actions.
    /// </summary>
    private void InitializeMassActions()
    {
        const string actionScript = @"
function RaiseMassAction(drpActionsClientId, drpActionFilesClientId) {
    var drpActions = document.getElementById(drpActionsClientId);
    var drpActionFiles = document.getElementById(drpActionFilesClientId);
    if((drpActions != null) && (drpActionFiles != null)) {
        var selectedFiles = drpActionFiles.options[drpActionFiles.selectedIndex];
        var selectedAction = drpActions.options[drpActions.selectedIndex];
        if((selectedAction != null) && (selectedFiles != null)) {
            var argument = selectedAction.value + '|' + selectedFiles.value;
            SetAction('massaction', argument);
            RaiseHiddenPostBack();
        }                   
    } 
}";

        ScriptHelper.RegisterStartupScript(Page, typeof(Page), "LibraryActionScript", ScriptHelper.GetScript(actionScript));

        if (drpActionFiles.Items.Count == 0)
        {
            // Actions dropdown
            drpActionFiles.Items.Add(new ListItem(GetString("media.file.list.lblactions"), "selected"));
            drpActionFiles.Items.Add(new ListItem(GetString("media.file.list.filesall"), "all"));
        }

        if (drpActions.Items.Count == 0)
        {
            // Actions dropdown
            drpActions.Items.Add(new ListItem(GetString("General.SelectAction"), ""));
            drpActions.Items.Add(new ListItem(GetString("media.file.copy"), "copy"));
            drpActions.Items.Add(new ListItem(GetString("media.file.move"), "move"));
            drpActions.Items.Add(new ListItem(GetString("General.Delete"), "delete"));
            drpActions.Items.Add(new ListItem(GetString("media.file.import"), "import"));
        }

        btnActions.OnClientClick = String.Format("if(MassConfirm('{0}', {1})) {{ RaiseMassAction('{0}', '{2}'); }} return false;", drpActions.ClientID, ScriptHelper.GetLocalizedString("General.ConfirmGlobalDelete"), drpActionFiles.ClientID);
    }


    /// <summary>
    /// Returns list of names of selected files.
    /// </summary>
    private List<string> GetSelectedItems()
    {
        switch (ViewMode)
        {
            case DialogViewModeEnum.ListView:
                return gridList.SelectedItems;

            case DialogViewModeEnum.ThumbnailsView:
                return GetThumbsSelectedItems();
        }

        return null;
    }


    /// <summary>
    /// Returns list of names of files selected in thumbnails view mode.
    /// </summary>
    private List<string> GetThumbsSelectedItems()
    {
        List<string> result = new List<string>();

        // Go through all repeater items and look for selected ones
        foreach (RepeaterItem item in repThumbnailsView.Items)
        {
            CMSCheckBox chkSelected = item.FindControl("chkSelected") as CMSCheckBox;
            if ((chkSelected != null) && chkSelected.Checked)
            {
                HiddenField hdnItemName = item.FindControl("hdnItemName") as HiddenField;
                if (hdnItemName != null)
                {
                    string alt = hdnItemName.Value;
                    result.Add(alt);
                }
            }
        }

        return result;
    }


    /// <summary>
    /// Ensures given file name in the way it is usable as ID.
    /// </summary>
    /// <param name="fileName">Name of the file to ensure</param>
    private static string EnsureFileName(string fileName)
    {
        if (!string.IsNullOrEmpty(fileName))
        {
            char[] specialChars = "#;&,.+*~':\"!^$[]()=>|/\\-%@`{}".ToCharArray();
            foreach (char specialChar in specialChars)
            {
                fileName = fileName.Replace(specialChar, '_');
            }
            return fileName.Replace(" ", "").ToLowerCSafe();
        }

        return fileName;
    }

    #endregion


    #region "Private methods"

    /// <summary>
    /// Initializes all nested controls.
    /// </summary>
    private void SetupControls()
    {
        // Initialize displaying controls according view mode
        LoadViewControls();

        InitializeControlScripts();

        // Select mode according required
        switch (ViewMode)
        {
            case DialogViewModeEnum.ListView:
                InitializeListView();
                ListViewControl.OnExternalDataBound += ListViewControl_OnExternalDataBound;
                ListViewControl.GridView.RowDataBound += GridView_RowDataBound;
                break;

            case DialogViewModeEnum.ThumbnailsView:
                InitializeThumbnailsView();
                ThumbnailsViewControl.ItemDataBound += ThumbnailsViewControl_ItemDataBound;
                break;
        }

        // Register delete confirmation script
        ltlScript.Text = ScriptHelper.GetScript(String.Format("function DeleteConfirmation(){{ return confirm({0}); }} function DeleteMediaFileConfirmation(){{ return confirm({1}); }}",
                                                              ScriptHelper.GetLocalizedString("attach.deleteconfirmation"),
                                                              ScriptHelper.GetLocalizedString("general.deleteconfirmation")));
    }


    /// <summary>
    /// Initializes all the necessary JavaScript blocks.
    /// </summary>
    private void InitializeControlScripts()
    {
        // Dialog for editing image and non-image
        string urlImage;
        string urlMeta;

        if (SourceType == MediaSourceEnum.MediaLibraries)
        {
            if (IsLiveSite)
            {
                if (AuthenticationHelper.IsAuthenticated())
                {
                    urlImage = ResolveUrl("~/CMSModules/MediaLibrary/CMSPages/ImageEditor.aspx");
                }
                else
                {
                    urlImage = ResolveUrl("~/CMSModules/MediaLibrary/CMSPages/PublicImageEditor.aspx");
                }
                urlMeta = AuthenticationHelper.ResolveDialogUrl("~/CMSModules/MediaLibrary/CMSPages/MetaDataEditor.aspx");
            }
            else
            {
                urlImage = ResolveUrl("~/CMSModules/MediaLibrary/Controls/MediaLibrary/ImageEditor.aspx");
                urlMeta = AuthenticationHelper.ResolveDialogUrl("~/CMSModules/MediaLibrary/Dialogs/MetaDataEditor.aspx");
            }
        }
        else
        {
            if (IsLiveSite)
            {
                if (AuthenticationHelper.IsAuthenticated())
                {
                    urlImage = ResolveUrl("~/CMSFormControls/LiveSelectors/ImageEditor.aspx");
                }
                else
                {
                    urlImage = ResolveUrl("~/CMSFormControls/LiveSelectors/PublicImageEditor.aspx");
                }
                urlMeta = AuthenticationHelper.ResolveDialogUrl("~/CMSModules/Content/Attachments/CMSPages/MetaDataEditor.aspx");
            }
            else
            {
                urlImage = ResolveUrl("~/CMSModules/Content/CMSDesk/Edit/ImageEditor.aspx");
                urlMeta = AuthenticationHelper.ResolveDialogUrl("~/CMSModules/Content/Attachments/Dialogs/MetaDataEditor.aspx");
            }
        }

        if (SourceType == MediaSourceEnum.MetaFile)
        {
            urlMeta = AuthenticationHelper.ResolveDialogUrl("~/CMSModules/AdminControls/Controls/MetaFiles/MetaDataEditor.aspx");
        }

        string script = String.Format(@"
var attemptNo = 0;
function ColorizeRow(itemId) {{
    if (itemId != null)
    {{
        var hdnField = document.getElementById('{0}');
        if (hdnField != null)
        {{
            // If some item was previously selected
            if ((hdnField.value != null) && (hdnField.value != ''))
            {{
                // Get selected item and reset its selection
                var lastColorizedElem = document.getElementById(hdnField.value);
                if (lastColorizedElem != null)
                {{
                    ColorizeElement(lastColorizedElem, true);
                }}
             }}            
             // Update field value
             hdnField.value = itemId;
        }}
        // Colorize currently selected item
        var elem = document.getElementById(itemId);
        if (elem != null)
        {{
            ColorizeElement(elem, false);
            attemptNo = 0;
        }}
        else
        {{
            if(attemptNo < 1)
            {{
                setTimeout('ColorizeRow(\'' + itemId + '\')', 300);
                attemptNo = attemptNo + 1;
            }}
            else
            {{
                attemptNo = 0;
            }}
        }}
     }}
 }}
 function ColorizeLastRow() {{
    var hdnField = document.getElementById('{0}');
    if (hdnField != null)
    {{
        // If some item was previously selected
        if ((hdnField.value != null) && (hdnField.value != ''))
        {{               
            // Get selected item and reset its selection
            var lastColorizedElem = document.getElementById(hdnField.value);
            if (lastColorizedElem != null)
            {{
                ColorizeElement(lastColorizedElem, false);
            }}
        }}
    }}
}}

function ColorizeElement(elem, clear) {{
    if(!clear){{
        elem.className += ' Selected';
    }}
    else {{
        elem.className = elem.className.replace(' Selected','');
    }}
}}

function ClearColorizedRow()
{{
    var hdnField = document.getElementById('{0}');
    if (hdnField != null) 
    {{
        // If some item was previously selected
        if ((hdnField.value != null) && (hdnField.value != '')) 
        {{               
            // Get selected item and reset its selection
            var lastColorizedElem = document.getElementById(hdnField.value);
            if (lastColorizedElem != null) 
            {{   
                ColorizeElement(lastColorizedElem, false);

                // Update field value
                hdnField.value = '';                                    
            }}
        }}
    }}                                
}}
function EditImage(param) {{
    var form = '';
    if (param.indexOf('?') != 0) {{ 
        param = '?' + param; 
    }}
    modalDialog('{1}' + param, 'imageEditorDialog', 905, 670, undefined, true); 
    return false; 
}}
function Edit(param) {{
    var form = '';
    if (param.indexOf('?') != 0) {{ 
        param = '?' + param; 
    }}
    modalDialog('{2}' + param, 'editorDialog', 700, 400, undefined, true); 
    return false; 
}}",
                                      hdnItemToColorize.ClientID,
                                      urlImage,
                                      urlMeta);

        ScriptHelper.RegisterStartupScript(this, GetType(), "DialogsScript", script, true);
    }


    /// <summary>
    /// Loads data from the data source property.
    /// </summary>
    public void ReloadData()
    {
        // Select mode according required
        switch (ViewMode)
        {
            case DialogViewModeEnum.ListView:
                ReloadListView();
                break;

            case DialogViewModeEnum.ThumbnailsView:
                ReloadThumbnailsView();
                break;
        }
    }


    /// <summary>
    /// Reloads control with data.
    /// </summary>
    /// <param name="forceSetup">Indicates whether the inner controls should be re-setuped</param>
    public void Reload(bool forceSetup)
    {
        Visible = !StopProcessing;
        if (Visible)
        {
            if (forceSetup)
            {
                // Initialize controls
                SetupControls();
            }

            // Load passed data
            ReloadData();
        }
    }


    /// <summary>
    /// Returns the sitename according to item info.
    /// </summary>
    /// <param name="data">Row containing information on the current item</param>
    /// <param name="isMediaFile">Indicates whether the file is media file</param>
    private string GetSiteName(IDataContainer data, bool isMediaFile)
    {
        int siteId = 0;
        string result = "";

        if (isMediaFile)
        {
            if (data.ContainsColumn("FileSiteID"))
            {
                // Imported media file
                siteId = ValidationHelper.GetInteger(data.GetValue("FileSiteID"), 0);
            }
            else
            {
                // Npt imported yet
                siteId = RaiseOnSiteIdRequired();
            }
        }
        else
        {
            if (data.ContainsColumn("SiteName"))
            {
                // Content file
                result = ValidationHelper.GetString(data.GetValue("SiteName"), "");
            }
            else
            {
                if (data.ContainsColumn("AttachmentSiteID"))
                {
                    // Non-versioned attachment
                    siteId = ValidationHelper.GetInteger(data.GetValue("AttachmentSiteID"), 0);
                }
                else if (TreeNodeObj != null)
                {
                    // Versioned attachment
                    siteId = TreeNodeObj.NodeSiteID;
                }
                else if (data.ContainsColumn("MetaFileSiteID"))
                {
                    // Metafile
                    siteId = ValidationHelper.GetInteger(data.GetValue("MetaFileSiteID"), 0);
                }
            }
        }

        if (result == "")
        {
            if (String.IsNullOrEmpty(siteName) || (mLastSiteId != siteId))
            {
                SiteInfo si = SiteInfoProvider.GetSiteInfo(siteId);
                if (si != null)
                {
                    mLastSiteId = si.SiteID;
                    result = si.SiteName;
                    siteName = result;
                }
            }
            else
            {
                result = siteName;
            }
        }

        return result;
    }


    private int GetObjectSiteID(string objectType, int objectID)
    {
        if (mObjectSiteID == -1)
        {
            BaseInfo info = BaseAbstractInfoProvider.GetInfoById(objectType, objectID);

            mObjectSiteID = info != null ? info.Generalized.ObjectSiteID : SiteContext.CurrentSiteID;
        }

        return mObjectSiteID;
    }



    /// <summary>
    /// Initializes attachment update control according current attachment data.
    /// </summary>
    /// <param name="dfuElem">Direct file uploader</param>
    /// <param name="data">Data container holding attachment data</param>
    private void GetAttachmentUpdateControl(ref DirectFileUploader dfuElem, IDataContainer data)
    {
        if (dfuElem != null)
        {
            string refreshType = CMSDialogHelper.GetMediaSource(SourceType);
            Guid formGuid = Guid.Empty;
            int documentId = ValidationHelper.GetInteger(data.GetValue("AttachmentDocumentID"), 0);

            // If attachment is related to the workflow 'AttachmentFormGUID' information isn't present
            if (data.ContainsColumn("AttachmentFormGUID"))
            {
                formGuid = ValidationHelper.GetGuid(data.GetValue("AttachmentFormGUID"), Guid.Empty);
            }

            if (SourceType == MediaSourceEnum.MetaFile)
            {
                dfuElem.ObjectID = Config.MetaFileObjectID;
                dfuElem.ObjectType = Config.MetaFileObjectType;
                dfuElem.Category = Config.MetaFileCategory;

                dfuElem.SiteID = GetObjectSiteID(Config.MetaFileObjectType, Config.MetaFileObjectID);

                dfuElem.SourceType = MediaSourceEnum.MetaFile;
                dfuElem.MetaFileID = ValidationHelper.GetInteger(data.GetValue("MetaFileID"), 0);
            }
            else
            {
                dfuElem.SourceType = MediaSourceEnum.DocumentAttachments;
                dfuElem.FormGUID = formGuid;
                dfuElem.DocumentID = documentId;
                if (TreeNodeObj != null)
                {
                    // if attachment node exists
                    dfuElem.NodeParentNodeID = TreeNodeObj.NodeParentID;
                    dfuElem.NodeClassName = TreeNodeObj.NodeClassName;
                }
                else
                {
                    // if attachment node doesn't exist
                    dfuElem.NodeParentNodeID = NodeParentID;
                    dfuElem.NodeClassName = "cms.file";
                }
                dfuElem.CheckPermissions = true;
                dfuElem.AttachmentGUID = ValidationHelper.GetGuid(data.GetValue("AttachmentGUID"), Guid.Empty);
                dfuElem.ResizeToWidth = ResizeToWidth;
                dfuElem.ResizeToHeight = ResizeToHeight;
                dfuElem.ResizeToMaxSideSize = ResizeToMaxSideSize;
                dfuElem.AllowedExtensions = AllowedExtensions;
            }

            dfuElem.ParentElemID = refreshType;
            dfuElem.ID = "dfuElem";
            dfuElem.ForceLoad = true;
            dfuElem.ControlGroup = "MediaView";
            dfuElem.ShowIconMode = true;
            dfuElem.InsertMode = false;
            dfuElem.IncludeNewItemInfo = true;
            dfuElem.IsLiveSite = IsLiveSite;

            // Setting of the direct single mode
            dfuElem.UploadMode = MultifileUploaderModeEnum.DirectSingle;
            dfuElem.MaxNumberToUpload = 1;
        }
    }


    /// <summary>
    /// Initializes upload control.
    /// When data is null, the control can be rendered as disabled only.
    /// </summary>
    /// <param name="dfuElem">Upload control to initialize</param>
    /// <param name="data">Data row with data on related media file</param>
    public void GetLibraryUpdateControl(ref DirectFileUploader dfuElem, IDataContainer data)
    {
        if (dfuElem != null)
        {
            if (data != null)
            {
                string siteName = GetSiteName(data, true);
                int fileId = ValidationHelper.GetInteger(data.GetValue("FileID"), 0);
                string fileName = EnsureFileName(Path.GetFileName(ValidationHelper.GetString(data.GetValue("FilePath"), "")));
                string folderPath = Path.GetDirectoryName(ValidationHelper.GetString(data.GetValue("FilePath"), ""));
                int libraryId = ValidationHelper.GetInteger(data.GetValue("FileLibraryID"), 0);

                AllowedExtensions = SettingsKeyInfoProvider.GetValue(siteName + ".CMSMediaFileAllowedExtensions");

                // Initialize library info
                dfuElem.LibraryID = libraryId;
                dfuElem.MediaFileID = fileId;
                dfuElem.MediaFileName = fileName;
                dfuElem.LibraryFolderPath = folderPath;
            }

            // Initialize general info
            dfuElem.CheckPermissions = true;
            dfuElem.SourceType = MediaSourceEnum.MediaLibraries;
            dfuElem.ID = "dfuElemLib";
            dfuElem.ForceLoad = true;
            dfuElem.DisplayInline = true;
            dfuElem.ControlGroup = "MediaView";
            dfuElem.ResizeToWidth = ResizeToWidth;
            dfuElem.ResizeToHeight = ResizeToHeight;
            dfuElem.ResizeToMaxSideSize = ResizeToMaxSideSize;
            dfuElem.AllowedExtensions = AllowedExtensions;
            dfuElem.ShowIconMode = true;
            dfuElem.InsertMode = false;
            dfuElem.ParentElemID = "LibraryUpdate";
            dfuElem.IncludeNewItemInfo = true;
            dfuElem.RaiseOnClick = true;
            dfuElem.IsLiveSite = IsLiveSite;

            // Setting of the direct single mode
            dfuElem.UploadMode = MultifileUploaderModeEnum.DirectSingle;
            dfuElem.Width = 16;
            dfuElem.Height = 16;
            dfuElem.MaxNumberToUpload = 1;
        }
    }


    /// <summary>
    /// Returns correct ID for the item (for colorizing the item when selected).
    /// </summary>
    /// <param name="dataItem">Container.DataItem</param>
    protected string GetID(object dataItem)
    {
        DataRowView dr = dataItem as DataRowView;
        if (dr != null)
        {
            IDataContainer data = new DataRowContainer(dr);
            return GetColorizeID(data);
        }

        return "";
    }


    /// <summary>
    /// Returns correct ID for the given item (for colorizing the item when selected).
    /// </summary>
    /// <param name="data">Item to get the ID of</param>
    protected string GetColorizeID(IDataContainer data)
    {
        string id = data.ContainsColumn(FileIdColumn) ? data.GetValue(FileIdColumn).ToString() : EnsureFileName(data.GetValue("FileName").ToString());

        if (String.IsNullOrEmpty(id))
        {
            // Content file
            id = data.GetValue("NodeGUID").ToString();
        }

        return id.ToLowerCSafe();
    }


    /// <summary>
    /// Gets file name according available columns.
    /// </summary>
    /// <param name="data">DataRow containing data</param>
    private string GetFileName(IDataContainer data)
    {
        string fileName = "";

        if (data != null)
        {
            if (data.ContainsColumn("FileExtension"))
            {
                fileName = String.Concat(data.GetValue("FileName"), data.GetValue("FileExtension"));
            }
            else
            {
                fileName = data.GetValue(FileNameColumn).ToString();
            }
        }

        return fileName;
    }


    /// <summary>
    /// Ensures correct format of extension being displayed.
    /// </summary>
    /// <param name="extension">Extension to normalize</param>
    private static string NormalizeExtenison(string extension)
    {
        if (!string.IsNullOrEmpty(extension))
        {
            if (extension.Trim() != "<dir>")
            {
                extension = "." + extension.ToLowerCSafe().TrimStart('.');
            }
            else
            {
                extension = HTMLHelper.HTMLEncode("<DIR>");
            }
        }

        return extension;
    }


    /// <summary>
    /// Gets title text.
    /// </summary>
    /// <param name="data">Source data</param>
    /// <param name="isContentFile">If true, the file is a content file</param>
    private string GetTitle(IDataContainer data, bool isContentFile)
    {
        string title = null;
        switch (SourceType)
        {
            case MediaSourceEnum.Attachment:
            case MediaSourceEnum.DocumentAttachments:
                title = ValidationHelper.GetString(data.GetValue("AttachmentTitle"), null);
                break;

            case MediaSourceEnum.Content:
                if (isContentFile)
                {
                    title = ValidationHelper.GetString(data.GetValue("AttachmentTitle"), null);
                }
                break;

            case MediaSourceEnum.MediaLibraries:
                title = ValidationHelper.GetString(data.GetValue("FileTitle"), null);
                break;

            case MediaSourceEnum.MetaFile:
                title = ValidationHelper.GetString(data.GetValue("MetaFileTitle"), null);
                break;
        }

        return title;
    }


    /// <summary>
    /// Gets description text.
    /// </summary>
    /// <param name="data">Source data</param>
    /// <param name="isContentFile">If true, the file is a content file</param>
    private string GetDescription(IDataContainer data, bool isContentFile)
    {
        string desc = null;
        switch (SourceType)
        {
            case MediaSourceEnum.Attachment:
            case MediaSourceEnum.DocumentAttachments:
                desc = ValidationHelper.GetString(data.GetValue("AttachmentDescription"), null);
                break;

            case MediaSourceEnum.Content:
                if (isContentFile)
                {
                    desc = ValidationHelper.GetString(data.GetValue("AttachmentDescription"), null);
                }
                break;

            case MediaSourceEnum.MediaLibraries:
                desc = ValidationHelper.GetString(data.GetValue("FileDescription"), null);
                break;

            case MediaSourceEnum.MetaFile:
                desc = ValidationHelper.GetString(data.GetValue("MetaFileDescription"), null);
                break;
        }

        return desc;
    }

    #endregion


    #region "List view methods"

    /// <summary>
    /// Initializes list view controls.
    /// </summary>
    private void InitializeListView()
    {
        switch (SourceType)
        {
            case MediaSourceEnum.Content:
                gridList.OrderBy = "NodeOrder";
                break;

            case MediaSourceEnum.MediaLibraries:
                gridList.OrderBy = "FileName";
                break;

            case MediaSourceEnum.DocumentAttachments:
                gridList.OrderBy = "AttachmentOrder, AttachmentName";
                break;

            case MediaSourceEnum.MetaFile:
                gridList.OrderBy = "MetaFileName, MetaFileID";
                break;
        }

        gridList.LoadGridDefinition();
        gridList.OnBeforeDataReload += gridList_OnBeforeDataReload;
    }


    private void gridList_OnBeforeDataReload()
    {
        gridList.PagerForceNumberOfResults = TotalRecords;
        gridList.DataSource = DataSource;
    }


    /// <summary>
    /// Reloads list view according source type.
    /// </summary>
    private void ReloadListView()
    {
        // Fill the grid data source
        if (!DataHelper.DataSourceIsEmpty(DataSource))
        {
            // Disable sorting if is copy/move dialog
            if ((IsCopyMoveLinkDialog) && (DisplayMode == ControlDisplayModeEnum.Simple))
            {
                gridList.GridView.AllowSorting = false;
            }
            gridList.ReloadData();
        }
    }


    /// <summary>
    /// Ensures no item is selected.
    /// </summary>
    public void ResetListSelection()
    {
        if (gridList != null)
        {
            gridList.ResetSelection();
        }
    }


    /// <summary>
    /// Ensures first page is displayed in the control displaying the content.
    /// </summary>
    public void ResetPageIndex()
    {
        if (ViewMode == DialogViewModeEnum.ListView)
        {
            ListViewControl.Pager.UniPager.CurrentPage = 1;
        }
        else
        {
            pagerElemThumbnails.CurrentPage = 1;
        }
    }


    /// <summary>
    /// Returns panel with image according extension of the processed file.
    /// </summary>
    /// <param name="ext">Extension of the file used to determine icon</param>
    /// <param name="className">Class name</param>
    /// <param name="url">Url of the original image</param>
    /// <param name="item">Control inserted as a file name</param>
    /// <param name="previewUrl">Preview URL</param>
    /// <param name="width">Image width</param>
    /// <param name="height">Image height</param>
    /// <param name="isSelectable">Determines whether it can be selected</param>
    /// <param name="generateTooltip">Determines if tooltip should be generated</param>
    /// <param name="title">File title</param>
    /// <param name="description">File description</param>
    /// <param name="name">File name without extension</param>
    private Panel GetListItem(string ext, string className, string url, string previewUrl, int width, int height, Control item, bool isSelectable, bool generateTooltip, string title, string description, string name)
    {
        var pnl = new Panel
        {
            CssClass = "DialogListItem" + (isSelectable ? "" : "Unselectable")
        };
        pnl.Controls.Add(new LiteralControl("<div class=\"DialogListItemNameRow\">"));

        // Process media library folder
        if ((SourceType == MediaSourceEnum.MediaLibraries) && ext.ToLowerCSafe() == "<dir>")
        {
            className = "icon-folder";
        }

        // Prepare image
        if ((className == "cms.file") || (className == ""))
        {
            var docImg = new Label();
            docImg.Text = UIHelper.GetFileIcon(Page, ext);

            // Tooltip
            if (generateTooltip)
            {
                UIHelper.EnsureTooltip(docImg, previewUrl, width, height, title, name, ext, description, null, 300);
            }
            pnl.Controls.Add(docImg);
        }
        else
        {
            string icon = null;
            var ci = DataClassInfoProvider.GetDataClassInfo(className);
            if (ci != null)
            {
                var iconClass = ValidationHelper.GetString(ci.GetValue("ClassIconClass"), String.Empty);
                icon = UIHelper.GetDocumentTypeIcon(Page, className, iconClass);
            }
            else
            {
                icon = UIHelper.GetAccessibleIconTag(className);
            }

            pnl.Controls.Add(new LiteralControl(icon));
        }

        if ((isSelectable) && (item is LinkButton))
        {
            // Create clickabe compelte panel
            pnl.Attributes["onclick"] = ((LinkButton)item).Attributes["onclick"];
            ((LinkButton)item).Attributes["onclick"] = null;
        }

        // Add file name                  
        pnl.Controls.Add(new LiteralControl(String.Format("<span class=\"DialogListItemName\" {0}>", (!isSelectable ? "style=\"cursor:default;\"" : ""))));
        pnl.Controls.Add(item);
        pnl.Controls.Add(new LiteralControl("</span></div>"));
        return pnl;
    }


    protected void GridView_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            DataRowView dr = (e.Row.DataItem as DataRowView);
            if (dr != null)
            {
                IDataContainer data = new DataRowContainer(dr);
                e.Row.Attributes["id"] = GetColorizeID(data);
            }
        }
    }


    protected object ListViewControl_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        object result = null;
        string argument = "";
        string url = "";

        bool isContent = (SourceType == MediaSourceEnum.Content);
        bool notAttachment;
        bool isContentFile;

        sourceName = sourceName.ToLowerCSafe();

        // Prepare the data
        IDataContainer data = null;
        if (parameter is DataRowView)
        {
            data = new DataRowContainer((DataRowView)parameter);
        }
        else
        {
            // Get the data row view from parameter
            GridViewRow gvr = (parameter as GridViewRow);
            if (gvr != null)
            {
                DataRowView dr = (DataRowView)gvr.DataItem;
                if (dr != null)
                {
                    data = new DataRowContainer(dr);
                }
            }
        }

        if (data == null)
        {
            return parameter;
        }

        CMSGridActionButton btnAction;
        DirectFileUploader dfuElem;

        string ext;
        string fileName;
        bool libraryFolder;
        string className = "";

        // Get site id
        int siteId;

        if (data.ContainsColumn("NodeSiteID"))
        {
            siteId = ValidationHelper.GetInteger(data.GetValue("NodeSiteID"), 0);
        }
        else if (data.ContainsColumn("AttachmentSiteID"))
        {
            siteId = ValidationHelper.GetInteger(data.GetValue("AttachmentSiteID"), 0);
        }
        else if (data.ContainsColumn("MetaFileSiteID"))
        {
            siteId = ValidationHelper.GetInteger(data.GetValue("MetaFileSiteID"), 0);
        }
        else
        {
            siteId = ValidationHelper.GetInteger(data.GetValue("FileSiteID"), 0);
        }

        // Check if site is running
        bool siteIsRunning = true;
        SiteInfo siteInfo = SiteContext.CurrentSite;
        if (siteId > 0)
        {
            siteInfo = SiteInfoProvider.GetSiteInfo(siteId);
            if (siteInfo != null)
            {
                siteIsRunning = siteInfo.Status == SiteStatusEnum.Running;
            }
        }

        UserInfo userInfo = MembershipContext.AuthenticatedUser;

        switch (sourceName)
        {
            #region "Select"

            case "select":
                btnAction = sender as CMSGridActionButton;
                if (btnAction != null)
                {
                    // Is current item CMS.File or attachment ?
                    isContentFile = isContent ? (data.GetValue("ClassName").ToString().ToLowerCSafe() == "cms.file") : false;
                    notAttachment = isContent && !(isContentFile && (ValidationHelper.GetGuid(data.GetValue("AttachmentGUID"), Guid.Empty) != Guid.Empty));
                    if (notAttachment)
                    {
                        className = DataClassInfoProvider.GetClassName((int)data.GetValue("NodeClassID"));
                    }

                    ext = HTMLHelper.HTMLEncode(notAttachment ? className : data.GetValue(FileExtensionColumn).ToString().TrimStart('.'));

                    // Get file name
                    fileName = GetFileName(data);
                    libraryFolder = ((SourceType == MediaSourceEnum.MediaLibraries) && IsFullListingMode && (data.GetValue(FileExtensionColumn).ToString().ToLowerCSafe() == "<dir>"));

                    if ((SourceType == MediaSourceEnum.MediaLibraries) && ((RaiseOnFileIsNotInDatabase(fileName) == null) && (DisplayMode == ControlDisplayModeEnum.Simple)))
                    {
                        // If folders are displayed as well show SELECT button icon
                        if (libraryFolder && !IsCopyMoveLinkDialog)
                        {
                            btnAction.IconCssClass = "icon-arrow-crooked-right";
                            btnAction.ToolTip = GetString("dialogs.list.actions.showsubfolders");
                            btnAction.OnClientClick = String.Format("SetAction('morefolderselect', {0}); RaiseHiddenPostBack(); return false;", ScriptHelper.GetString(fileName));
                        }
                        else if (libraryFolder && IsCopyMoveLinkDialog)
                        {
                            btnAction.IconCssClass = "icon-chevron-right";
                            btnAction.ToolTip = GetString("general.select");
                            btnAction.OnClientClick = String.Format("SetAction('copymoveselect', {0}); RaiseHiddenPostBack(); return false;", ScriptHelper.GetString(fileName));
                        }
                        else
                        {
                            // If media file not imported yet - display warning sign
                            btnAction.IconCssClass = "icon-exclamation-triangle";
                            btnAction.IconStyle = GridIconStyle.Warning;
                            btnAction.ToolTip = GetString("media.file.import");
                            btnAction.OnClientClick = String.Format("SetAction('importfile', {0}); RaiseHiddenPostBack(); return false;", ScriptHelper.GetString(data.GetValue("FileName").ToString()));
                        }
                    }
                    else
                    {
                        // Check if item is selectable, if not remove select action button
                        bool isSelectable = CMSDialogHelper.IsItemSelectable(SelectableContent, ext, isContentFile);
                        if (!isSelectable)
                        {
                            btnAction.Enabled = false;
                        }
                        else
                        {
                            argument = RaiseOnGetArgumentSet(data);

                            // Get item URL
                            url = RaiseOnGetListItemUrl(data, false, notAttachment);

                            // Initialize command
                            btnAction.OnClientClick = String.Format("ColorizeRow({0}); SetSelectAction({1}); return false;", ScriptHelper.GetString(GetColorizeID(data)), ScriptHelper.GetString(argument + "|URL|" + url));

                            result = btnAction;
                        }
                    }
                }
                break;

            #endregion


            #region "Select sub docs"

            case "selectsubdocs":
                btnAction = sender as CMSGridActionButton;
                if (btnAction != null)
                {
                    int nodeId = ValidationHelper.GetInteger(data.GetValue("NodeID"), 0);

                    if (IsFullListingMode)
                    {
                        // Check if item is selectable, if not remove select action button
                        // Initialize command
                        btnAction.OnClientClick = String.Format("SetParentAction('{0}'); return false;", nodeId);
                    }
                    else
                    {
                        btnAction.Visible = false;
                    }
                }
                break;

            #endregion


            #region "Select sub folders"

            case "selectsubfolders":
                btnAction = sender as CMSGridActionButton;
                if (btnAction != null)
                {
                    string folderName = ValidationHelper.GetString(data.GetValue("FileName"), "");
                    ext = ValidationHelper.GetString(data.GetValue(FileExtensionColumn), "");

                    if (IsCopyMoveLinkDialog || (IsFullListingMode && (DisplayMode == ControlDisplayModeEnum.Default) && (ext == "<dir>")))
                    {
                        // Initialize command
                        btnAction.OnClientClick = String.Format("SetLibParentAction({0}); return false;", ScriptHelper.GetString(folderName));
                    }
                    else
                    {
                        btnAction.Visible = false;
                    }
                }
                break;

            #endregion


            #region "View"

            case "view":
                btnAction = sender as CMSGridActionButton;
                if (btnAction != null)
                {
                    // Check if current item is library folder
                    fileName = GetFileName(data);
                    libraryFolder = ((SourceType == MediaSourceEnum.MediaLibraries) && IsFullListingMode && (data.GetValue(FileExtensionColumn).ToString().ToLowerCSafe() == "<dir>"));

                    // Is current item CMS.File or attachment ?
                    isContentFile = isContent && (data.GetValue("ClassName").ToString().ToLowerCSafe() == "cms.file");
                    notAttachment = isContent && !(isContentFile && (ValidationHelper.GetGuid(data.GetValue("AttachmentGUID"), Guid.Empty) != Guid.Empty));
                    if (!notAttachment && !libraryFolder && siteIsRunning)
                    {
                        // Get item URL
                        url = RaiseOnGetListItemUrl(data, false, false);

                        if (String.IsNullOrEmpty(url))
                        {
                            btnAction.Enabled = false;
                        }
                        else
                        {
                            // Add latest version requirement for live site
                            if (IsLiveSite)
                            {
                                // Check version history ID
                                int versionHistoryId = VersionHistoryID;
                                if (versionHistoryId > 0)
                                {
                                    // Add requirement for latest version of files for current document
                                    string newparams = "latestforhistoryid=" + versionHistoryId;
                                    newparams += "&hash=" + ValidationHelper.GetHashString("h" + versionHistoryId);

                                    url = URLHelper.AppendQuery(url, newparams);
                                }
                            }

                            string finalUrl = URLHelper.ResolveUrl(url);
                            btnAction.OnClientClick = String.Format("window.open({0}); return false;", ScriptHelper.GetString(finalUrl));
                        }
                    }
                    else
                    {
                        btnAction.Visible = false;
                    }
                }
                break;

            #endregion


            #region "Edit"

            case "edit":
                btnAction = sender as CMSGridActionButton;
                if (btnAction != null)
                {
                    // Is current item CMS.File or attachment ?
                    isContentFile = isContent ? (data.GetValue("ClassName").ToString().ToLowerCSafe() == "cms.file") : false;
                    notAttachment = isContent && !(isContentFile && (ValidationHelper.GetGuid(data.GetValue("AttachmentGUID"), Guid.Empty) != Guid.Empty));

                    // Get file extension
                    ext = (notAttachment ? "" : data.GetValue(FileExtensionColumn).ToString().TrimStart('.'));
                    Guid guid = Guid.Empty;
                    if ((SourceType == MediaSourceEnum.MediaLibraries) && siteIsRunning)
                    {
                        libraryFolder = (IsFullListingMode && (data.GetValue(FileExtensionColumn).ToString().ToLowerCSafe() == "<dir>"));

                        if (!libraryFolder)
                        {
                            guid = ValidationHelper.GetGuid(data.GetValue("FileGUID"), Guid.Empty);

                            btnAction.ScreenReaderDescription = String.Format("{0}|MediaFileGUID={1}&sitename={2}", ext, guid, GetSiteName(data, true));
                            btnAction.PreRender += img_PreRender;
                            btnAction.IconCssClass = "icon-edit";
                            btnAction.IconStyle = GridIconStyle.Allow;
                        }
                        else
                        {
                            btnAction.Visible = false;
                        }
                    }
                    else if (!notAttachment && siteIsRunning)
                    {
                        string nodeIdQuery = "";
                        if (SourceType == MediaSourceEnum.Content)
                        {
                            nodeIdQuery = "&nodeId=" + data.GetValue("NodeID");
                        }

                        // Get the node workflow
                        VersionHistoryID = ValidationHelper.GetInteger(data.GetValue("DocumentCheckedOutVersionHistoryID"), 0);
                        guid = ValidationHelper.GetGuid(data.GetValue("AttachmentGUID"), Guid.Empty);
                        btnAction.ScreenReaderDescription = String.Format("{0}|AttachmentGUID={1}&sitename={2}{3}&versionHistoryId={4}", ext, guid, GetSiteName(data, false), nodeIdQuery, VersionHistoryID);
                        btnAction.PreRender += img_PreRender;
                        btnAction.IconCssClass = "icon-edit";
                        btnAction.IconStyle = GridIconStyle.Allow;
                    }
                    else
                    {
                        btnAction.Visible = false;
                    }
                }
                break;

            #endregion


            #region "External edit"

            case "extedit":
                {
                    // Prepare the data
                    DataRowView dr = parameter as DataRowView;
                    if (dr != null)
                    {
                        data = new DataRowContainer(dr);
                    }

                    // Create placeholder
                    PlaceHolder plcUpd = new PlaceHolder();
                    plcUpd.ID = "plcUdateColumn";

                    Panel pnlBlock = new Panel();
                    pnlBlock.ID = "pnlBlock";

                    pnlBlock.CssClass = "TableCell";
                    plcUpd.Controls.Add(pnlBlock);

                    if (siteIsRunning)
                    {
                        if (ExternalEditHelper.LoadExternalEditControl(pnlBlock, GetFileType(SourceType), siteInfo != null ? siteInfo.SiteName : null, data, IsLiveSite, TreeNodeObj) != null)
                        {
                            columnUpdateVisible = true;
                        }

                        return plcUpd;
                    }
                }
                break;

            #endregion


            #region "Edit library ui"

            case "editlibraryui":
                btnAction = sender as CMSGridActionButton;
                if (btnAction != null)
                {
                    // Get file name
                    fileName = GetFileName(data);

                    bool notInDatabase = ((RaiseOnFileIsNotInDatabase(fileName) == null) && (DisplayMode == ControlDisplayModeEnum.Simple));

                    // Check if current item is library folder
                    libraryFolder = ((SourceType == MediaSourceEnum.MediaLibraries) && IsFullListingMode && notInDatabase && (data.GetValue(FileExtensionColumn).ToString().ToLowerCSafe() == "<dir>"));

                    ext = data.ContainsColumn("FileExtension") ? data.GetValue("FileExtension").ToString() : data.GetValue("Extension").ToString();

                    if (libraryFolder)
                    {
                        btnAction.Visible = false;
                    }
                    else if (notInDatabase)
                    {
                        btnAction.Enabled = false;
                    }
                    else
                    {
                        btnAction.OnClientClick = String.Format("$cmsj('#hdnFileOrigName').attr('value', {0}); SetAction('editlibraryui', {1}); RaiseHiddenPostBack(); return false;", ScriptHelper.GetString(EnsureFileName(fileName)), ScriptHelper.GetString(fileName));
                    }
                }
                break;

            #endregion


            #region "Delete"

            case "delete":
                btnAction = sender as CMSGridActionButton;
                if (btnAction != null)
                {
                    // Get file name
                    fileName = GetFileName(data);

                    // Check if current item is library folder
                    libraryFolder = ((SourceType == MediaSourceEnum.MediaLibraries) && IsFullListingMode && (data.GetValue(FileExtensionColumn).ToString().ToLowerCSafe() == "<dir>"));

                    btnAction.ToolTip = GetString("general.delete");

                    if (((RaiseOnFileIsNotInDatabase(fileName) == null) && (DisplayMode == ControlDisplayModeEnum.Simple)))
                    {
                        if (libraryFolder)
                        {
                            btnAction.Visible = false;
                        }
                        else
                        {
                            btnAction.OnClientClick = String.Format("if(DeleteMediaFileConfirmation() == false){{return false;}} SetAction('deletefile',{0}); RaiseHiddenPostBack(); return false;", ScriptHelper.GetString(fileName));
                        }
                    }
                    else
                    {
                        btnAction.OnClientClick = String.Format("if(DeleteMediaFileConfirmation() == false){{return false;}} SetAction('deletefile',{0}); RaiseHiddenPostBack(); return false;", ScriptHelper.GetString(fileName));
                    }
                }
                break;

            #endregion


            #region "Name"

            case "name":
                {
                    // Is current item CMS.File or attachment ?
                    isContentFile = isContent ? (data.GetValue("ClassName").ToString().ToLowerCSafe() == "cms.file") : false;
                    notAttachment = isContent && !(isContentFile && (ValidationHelper.GetGuid(data.GetValue("AttachmentGUID"), Guid.Empty) != Guid.Empty));

                    // Get name and extension                
                    string fileNameColumn = FileNameColumn;
                    if (notAttachment)
                    {
                        fileNameColumn = "DocumentName";
                    }
                    string name = HTMLHelper.HTMLEncode(data.GetValue(fileNameColumn).ToString());
                    ext = (notAttachment ? "" : data.GetValue(FileExtensionColumn).ToString().TrimStart('.'));

                    if ((SourceType == MediaSourceEnum.DocumentAttachments) || (SourceType == MediaSourceEnum.MetaFile))
                    {
                        name = Path.GetFileNameWithoutExtension(name);
                    }

                    // Width & height
                    int width = (data.ContainsColumn(FileWidthColumn) ? ValidationHelper.GetInteger(data.GetValue(FileWidthColumn), 0) : 0);
                    int height = (data.ContainsColumn(FileHeightColumn) ? ValidationHelper.GetInteger(data.GetValue(FileHeightColumn), 0) : 0);

                    string cmpltExt = (notAttachment ? "" : "." + ext);
                    fileName = (cmpltExt != "") ? name.Replace(cmpltExt, "") : name;
                    if (isContent && !notAttachment)
                    {
                        string attachmentName = Path.GetFileNameWithoutExtension(data.GetValue("AttachmentName").ToString());
                        if (fileName.ToLowerCSafe() != attachmentName.ToLowerCSafe())
                        {
                            fileName += String.Format(" ({0})", HTMLHelper.HTMLEncode(data.GetValue("AttachmentName").ToString()));
                        }
                    }

                    className = isContent ? HTMLHelper.HTMLEncode(data.GetValue("ClassName").ToString().ToLowerCSafe()) : "";
                    isContentFile = (className == "cms.file");

                    // Check if item is selectable
                    if (!CMSDialogHelper.IsItemSelectable(SelectableContent, ext, isContentFile))
                    {
                        LiteralControl ltlName = new LiteralControl(fileName);

                        // Get final panel
                        result = GetListItem(ext, className, "", "", width, height, ltlName, false, false, GetTitle(data, isContentFile), GetDescription(data, isContentFile), name);
                    }
                    else
                    {
                        // Make a file name link
                        LinkButton lnkBtn = new LinkButton()
                                                {
                                                    ID = "n",
                                                    Text = HTMLHelper.HTMLEncode(fileName)
                                                };

                        // Is current item CMS.File or attachment ?
                        isContentFile = isContent ? (data.GetValue("ClassName").ToString().ToLowerCSafe() == "cms.file") : false;
                        notAttachment = isContent && !(isContentFile && (ValidationHelper.GetGuid(data.GetValue("AttachmentGUID"), Guid.Empty) != Guid.Empty));

                        fileName = GetFileName(data);

                        // Try to get imported file row
                        bool isImported = true;
                        IDataContainer importedData = null;
                        if (DisplayMode == ControlDisplayModeEnum.Simple)
                        {
                            importedData = RaiseOnFileIsNotInDatabase(fileName);
                            isImported = (importedData != null);
                        }
                        else
                        {
                            importedData = data;
                        }

                        if (!isImported)
                        {
                            importedData = data;
                        }
                        else
                        {
                            // Update WIDTH
                            if (importedData.ContainsColumn(FileWidthColumn))
                            {
                                width = ValidationHelper.GetInteger(importedData[FileWidthColumn], 0);
                            }

                            // Update HEIGHT
                            if (importedData.ContainsColumn(FileHeightColumn))
                            {
                                height = ValidationHelper.GetInteger(importedData[FileHeightColumn], 0);
                            }
                        }

                        argument = RaiseOnGetArgumentSet(data);
                        url = RaiseOnGetListItemUrl(importedData, false, notAttachment);

                        string previewUrl = RaiseOnGetListItemUrl(importedData, true, notAttachment);
                        if (!String.IsNullOrEmpty(previewUrl))
                        {
                            // Add chset
                            string chset = Guid.NewGuid().ToString();
                            previewUrl = URLHelper.AddParameterToUrl(previewUrl, "chset", chset);
                        }

                        // Add latest version requirement for live site
                        if (!String.IsNullOrEmpty(previewUrl) && IsLiveSite)
                        {
                            int versionHistoryId = VersionHistoryID;
                            if (versionHistoryId > 0)
                            {
                                // Add requirement for latest version of files for current document
                                string newparams = "latestforhistoryid=" + versionHistoryId;
                                newparams += "&hash=" + ValidationHelper.GetHashString("h" + versionHistoryId);

                                //url = URLHelper.AppendQuery(url, newparams);
                                previewUrl = URLHelper.AppendQuery(previewUrl, newparams);
                            }
                        }

                        // Check if current item is library folder
                        libraryFolder = ((SourceType == MediaSourceEnum.MediaLibraries) && IsFullListingMode &&
                                         (data.GetValue(FileExtensionColumn).ToString().ToLowerCSafe() == "<dir>"));

                        if ((SourceType == MediaSourceEnum.MediaLibraries) && (!isImported && (DisplayMode == ControlDisplayModeEnum.Simple)))
                        {
                            if (libraryFolder && !IsCopyMoveLinkDialog)
                            {
                                lnkBtn.Attributes["onclick"] = String.Format("SetAction('morefolderselect', {0}); RaiseHiddenPostBack(); return false;", ScriptHelper.GetString(fileName));
                            }
                            else if (libraryFolder && IsCopyMoveLinkDialog)
                            {
                                lnkBtn.Attributes["onclick"] = String.Format("SetAction('copymoveselect', {0}); RaiseHiddenPostBack(); return false;", ScriptHelper.GetString(fileName));
                            }
                            else
                            {
                                lnkBtn.Attributes["onclick"] = String.Format("ColorizeRow({0}); SetAction('importfile', {1}); RaiseHiddenPostBack(); return false;", ScriptHelper.GetString(GetColorizeID(data)), ScriptHelper.GetString(fileName));
                            }
                        }
                        else
                        {
                            // Initialize command
                            lnkBtn.Attributes["onclick"] = String.Format("ColorizeRow({0}); SetSelectAction({1}); return false;", ScriptHelper.GetString(GetColorizeID(data)), ScriptHelper.GetString(String.Format("{0}|URL|{1}", argument, url)));
                        }

                        // Get final panel
                        result = GetListItem(ext, className, url, previewUrl, width, height, lnkBtn, true, isImported && siteIsRunning, GetTitle(data, isContentFile), GetDescription(data, isContentFile), name);
                    }
                }
                break;

            #endregion


            #region "Type"

            case "type":
                {
                    if (isContent)
                    {
                        // Is current item CMS.File or attachment ?
                        isContentFile = (data.GetValue("ClassName").ToString().ToLowerCSafe() == "cms.file");
                        notAttachment = !(isContentFile && (ValidationHelper.GetGuid(data.GetValue("AttachmentGUID"), Guid.Empty) != Guid.Empty));

                        if (notAttachment || (OutputFormat == OutputFormatEnum.HTMLLink) || (OutputFormat == OutputFormatEnum.BBLink))
                        {
                            return HTMLHelper.HTMLEncode(ResHelper.LocalizeString(data.GetValue("ClassDisplayName").ToString()));
                        }
                    }
                    result = NormalizeExtenison(data.GetValue(FileExtensionColumn).ToString());
                }
                break;

            #endregion


            #region "Size"

            case "size":
                {
                    // Is current item CMS.File or attachment ?
                    isContentFile = isContent && (data.GetValue("ClassName").ToString().ToLowerCSafe() == "cms.file");
                    notAttachment = isContent && !(isContentFile && (ValidationHelper.GetGuid(data.GetValue("AttachmentGUID"), Guid.Empty) != Guid.Empty));

                    if (!notAttachment)
                    {
                        long size = 0;
                        if (data.GetValue(FileExtensionColumn).ToString() != "<dir>")
                        {
                            if (data.ContainsColumn(FileSizeColumn))
                            {
                                size = ValidationHelper.GetLong(data.GetValue(FileSizeColumn), 0);
                            }
                            else if (data.ContainsColumn("Size"))
                            {
                                IDataContainer importedData = RaiseOnFileIsNotInDatabase(GetFileName(data));
                                size = ValidationHelper.GetLong((importedData != null) ? importedData["FileSize"] : data.GetValue("Size"), 0);
                            }
                        }
                        else
                        {
                            return "";
                        }
                        result = DataHelper.GetSizeString(size);
                    }
                }
                break;

            #endregion


            #region "Attachment modified"

            case "attachmentmodified":
            case "attachmentmodifiedtooltip":
                {
                    result = data.GetValue("AttachmentLastModified").ToString();

                    if (sourceName.EqualsCSafe("attachmentmodified", StringComparison.InvariantCultureIgnoreCase))
                    {
                        result = TimeZoneHelper.ConvertToUserTimeZone(ValidationHelper.GetDateTime(result, DateTimeHelper.ZERO_TIME), true, userInfo, siteInfo);
                    }
                    else
                    {
                        result = GetGMTTooltipString(userInfo, siteInfo);
                    }
                }
                break;

            #endregion


            #region "Attachment update"

            case "attachmentupdate":
                {
                    // Dynamically load uploader control
                    dfuElem = LoadDirectFileUploader();

                    var updatePanel = new Panel();
                    updatePanel.ID = "updatePanel";
                    updatePanel.PreRender += (senderObject, args) => updatePanel.Width = mUpdateIconPanelWidth;
                    updatePanel.Style.Add("margin", "0 auto");

                    // Initialize update control
                    GetAttachmentUpdateControl(ref dfuElem, data);

                    dfuElem.DisplayInline = true;
                    updatePanel.Controls.Add(dfuElem);

                    Guid formGUID = ValidationHelper.GetGuid(data.GetValue("AttachmentFormGUID"), Guid.Empty);

                    // Setup external edit if form GUID is empty
                    if (formGUID == Guid.Empty)
                    {
                        ExternalEditHelper.LoadExternalEditControl(updatePanel, FileTypeEnum.Attachment, null, data, IsLiveSite, TreeNodeObj);
                        mUpdateIconPanelWidth = 32;
                    }

                    result = updatePanel;
                }
                break;

            #endregion


            #region "Library update"

            case "libraryupdate":
                {
                    Panel updatePanel = new Panel();

                    updatePanel.Style.Add("margin", "0 auto");
                    updatePanel.PreRender += (senderObject, args) => updatePanel.Width = mUpdateIconPanelWidth;

                    libraryFolder = ((SourceType == MediaSourceEnum.MediaLibraries) && IsFullListingMode &&
                                     (data.GetValue(FileExtensionColumn).ToString().ToLowerCSafe() == "<dir>"));

                    // Get info on imported file
                    fileName = GetFileName(data);

                    IDataContainer existingData = RaiseOnFileIsNotInDatabase(fileName);
                    bool hasModifyPermission = RaiseOnGetModifyPermission(data);

                    // Dynamically load uploader control
                    dfuElem = LoadDirectFileUploader();
                    if (dfuElem != null)
                    {
                        // Initialize update control
                        GetLibraryUpdateControl(ref dfuElem, existingData);

                        updatePanel.Controls.Add(dfuElem);
                    }
                    if (hasModifyPermission && (existingData != null))
                    {
                        dfuElem.Enabled = true;
                    }
                    else
                    {
                        updatePanel.Visible = !libraryFolder || !hasModifyPermission;
                        dfuElem.Enabled = false;
                    }

                    if (existingData != null)
                    {
                        string siteName = GetSiteName(existingData, true);

                        // Setup external edit
                        var ctrl = ExternalEditHelper.LoadExternalEditControl(updatePanel, FileTypeEnum.MediaFile, siteName, existingData, IsLiveSite);

                        if (ctrl != null)
                        {
                            mUpdateIconPanelWidth = 32;
                        }
                    }

                    result = updatePanel;
                }
                break;

            #endregion


            #region "Attachment/MetaFile delete"

            case "metafiledelete":
            case "attachmentdelete":
                btnAction = sender as CMSGridActionButton;
                if (btnAction != null)
                {
                    // Initialize DELETE button
                    btnAction.OnClientClick = String.Format("if(DeleteConfirmation() == false){{return false;}} SetAction('{1}', '{0}'); RaiseHiddenPostBack(); return false;", data.GetValue(FileIdColumn), sourceName.ToLowerCSafe());
                }
                break;

            #endregion


            #region "Attachment moveup"

            case "attachmentmoveup":
                btnAction = sender as CMSGridActionButton;
                if (btnAction != null)
                {
                    // Get attachment ID
                    Guid attachmentGuid = ValidationHelper.GetGuid(data.GetValue("AttachmentGUID"), Guid.Empty);
                    btnAction.OnClientClick = String.Format("SetAction('attachmentmoveup', '{0}'); RaiseHiddenPostBack(); return false;", attachmentGuid);
                }
                break;

            #endregion


            #region "Attachment movedown"

            case "attachmentmovedown":
                btnAction = sender as CMSGridActionButton;
                if (btnAction != null)
                {
                    // Get attachment ID
                    Guid attachmentGuid = ValidationHelper.GetGuid(data.GetValue("AttachmentGUID"), Guid.Empty);
                    btnAction.OnClientClick = String.Format("SetAction('attachmentmovedown', '{0}'); RaiseHiddenPostBack(); return false;", attachmentGuid);
                }
                break;

            #endregion


            #region "Attachment edit"

            case "attachmentedit":
                btnAction = sender as CMSGridActionButton;
                if (btnAction != null)
                {
                    // Get file extension
                    string attExtension = ValidationHelper.GetString(data.GetValue("AttachmentExtension"), "").ToLowerCSafe();
                    Guid attGuid = ValidationHelper.GetGuid(data.GetValue("AttachmentGUID"), Guid.Empty);

                    btnAction.ScreenReaderDescription = String.Format("{0}|AttachmentGUID={1}&sitename={2}&versionHistoryId={3}", attExtension, attGuid, GetSiteName(data, false), VersionHistoryID);
                    btnAction.PreRender += img_PreRender;
                }
                break;

            #endregion


            #region "Library extension"

            case "extension":
                {
                    result = data.ContainsColumn("FileExtension") ? data.GetValue("FileExtension").ToString() : data.GetValue("Extension").ToString();
                    result = NormalizeExtenison(result.ToString());
                }
                break;

            #endregion


            #region "Modified"

            case "modified":
            case "modifiedtooltip":
                {
                    if (sourceName.EqualsCSafe("modified", StringComparison.InvariantCultureIgnoreCase))
                    {
                        result = data.ContainsColumn("FileModifiedWhen") ? data.GetValue("FileModifiedWhen").ToString() : data.GetValue("Modified").ToString();
                        result = TimeZoneHelper.ConvertToUserTimeZone(ValidationHelper.GetDateTime(result, DateTimeHelper.ZERO_TIME), true, userInfo, siteInfo);
                    }
                    else
                    {
                        result = GetGMTTooltipString(userInfo, siteInfo);
                    }
                }
                break;

            #endregion


            #region "Document modified when"

            case "documentmodifiedwhen":
            case "filemodifiedwhen":
            case "documentmodifiedwhentooltip":
            case "filemodifiedwhentooltip":
                {
                    if (sourceName.EqualsCSafe("documentmodifiedwhen", StringComparison.InvariantCultureIgnoreCase) || sourceName.EqualsCSafe("filemodifiedwhen", StringComparison.InvariantCultureIgnoreCase))
                    {
                        result = TimeZoneHelper.ConvertToUserTimeZone(ValidationHelper.GetDateTime(parameter, DateTimeHelper.ZERO_TIME), true, userInfo, siteInfo);
                    }
                    else
                    {
                        result = GetGMTTooltipString(userInfo, siteInfo);
                    }
                }
                break;

            #endregion


            #region "MetaFile edit"

            case "metafileedit":
                btnAction = sender as CMSGridActionButton;
                if (btnAction != null)
                {
                    // Get file extension
                    string metaExtension = ValidationHelper.GetString(data.GetValue("MetaFileExtension"), string.Empty).ToLowerCSafe();
                    Guid metaGuid = ValidationHelper.GetGuid(data.GetValue("MetaFileGUID"), Guid.Empty);

                    btnAction.ScreenReaderDescription = String.Format("{0}|metafileguid={1}", metaExtension, metaGuid);
                    btnAction.PreRender += img_PreRender;
                }
                break;

            #endregion


            #region "MetaFile update"

            case "metafileupdate":
                {
                    // Dynamically load uploader control
                    dfuElem = Page.LoadUserControl("~/CMSModules/Content/Controls/Attachments/DirectFileUploader/DirectFileUploader.ascx") as DirectFileUploader;

                    Panel updatePanel = new Panel();
                    updatePanel.ID = "updatePanel";
                    updatePanel.Width = mUpdateIconPanelWidth;
                    updatePanel.Style.Add("margin", "0 auto");

                    // Initialize update control
                    GetAttachmentUpdateControl(ref dfuElem, data);

                    dfuElem.DisplayInline = true;
                    updatePanel.Controls.Add(dfuElem);

                    result = updatePanel;
                }
                break;

            #endregion


            #region "MetaFile modified"

            case "metafilemodified":
            case "metafilemodifiedtooltip":
                {
                    if (sourceName.EqualsCSafe("metafilemodified", StringComparison.InvariantCultureIgnoreCase))
                    {
                        result = data.GetValue("MetaFileLastModified").ToString();
                        result = TimeZoneHelper.ConvertToUserTimeZone(ValidationHelper.GetDateTime(result, DateTimeHelper.ZERO_TIME), true, userInfo, siteInfo);
                    }
                    else
                    {
                        result = GetGMTTooltipString(userInfo, siteInfo);
                    }
                }
                break;

            #endregion
        }

        return result;
    }


    /// <summary>
    /// Gets the resource type based on the media source
    /// </summary>
    /// <param name="sourceType">Media source type</param>
    private FileTypeEnum GetFileType(MediaSourceEnum sourceType)
    {
        switch (sourceType)
        {
            case MediaSourceEnum.Attachment:
            case MediaSourceEnum.Content:
            case MediaSourceEnum.DocumentAttachments:
                return FileTypeEnum.Attachment;


            case MediaSourceEnum.MediaLibraries:
                return FileTypeEnum.MediaFile;

            case MediaSourceEnum.MetaFile:
                return FileTypeEnum.MetaFile;
        }

        return FileTypeEnum.Unknown;
    }


    /// <summary>
    /// Loads the direct file uploader control
    /// </summary>
    private DirectFileUploader LoadDirectFileUploader()
    {
        var dfuElem = Page.LoadUserControl("~/CMSModules/Content/Controls/Attachments/DirectFileUploader/DirectFileUploader.ascx") as DirectFileUploader;

        return dfuElem;
    }


    private string GetGMTTooltipString(IUserInfo userInfo, ISiteInfo siteInfo)
    {
        if (String.IsNullOrEmpty(mGMTTooltip))
        {
            mGMTTooltip = TimeZoneHelper.GetUTCLongStringOffset(userInfo, siteInfo);
        }

        return mGMTTooltip;
    }


    protected void img_PreRender(object sender, EventArgs e)
    {
        string[] args = null;
        var img = (CMSAccessibleButton)sender;
        args = img.ScreenReaderDescription.Split('|');
        img.ScreenReaderDescription = GetString("general.edit");

        if (args.Length == 2)
        {
            string refreshType = CMSDialogHelper.GetMediaSource(SourceType);
            int parentId = QueryHelper.GetInteger("parentId", 0);
            Guid formGuid = QueryHelper.GetGuid("formGuid", Guid.Empty);
            string query = String.Format("?clientid={0}{1}&refresh=1&refaction=0{2}&{3}", HTMLHelper.HTMLEncode(refreshType), ((parentId > 0) ? "&parentId=" + parentId : ""), ((formGuid != Guid.Empty) ? "&formGuid=" + formGuid : ""), args[1]);
            // Get validation hash for current image
            query = URLHelper.AddUrlParameter(query, "hash", QueryHelper.GetHash(query));

            img.OnClientClick = ImageHelper.IsSupportedByImageEditor(args[0]) ?
                String.Format("if(!($cmsj(this).hasClass('Edited'))){{ EditImage(\"{0}\"); }} return false;", query) :
                String.Format("if(!($cmsj(this).hasClass('Edited'))){{ Edit(\"{0}\"); }} return false;", query);

            img.ToolTip = GetString("general.edit");
        }
    }

    #endregion


    #region "Thumbnails view methods"

    /// <summary>
    /// Setups the UniPager
    /// </summary>
    /// <param name="pager">Pager to setup</param>
    private void SetupPager(UniPager pager)
    {
        pager.DisplayFirstLastAutomatically = false;
        pager.DisplayPreviousNextAutomatically = false;
        pager.HidePagerForSinglePage = true;
        pager.PagerMode = UniPagerMode.PostBack;
    }


    /// <summary>
    /// Initializes controls for the thumbnails view mode.
    /// </summary>
    private void InitializeThumbnailsView()
    {
        // Basic control properties
        repThumbnailsView.HideControlForZeroRows = true;

        // UniPager properties     
        SetupPager(pagerElemThumbnails.UniPager);

        // Initialize page size
        bool isAttachmentTab = SourceType == MediaSourceEnum.DocumentAttachments;
        pagerElemThumbnails.PageSizeOptions = isAttachmentTab ? "12,24,48,96" : "10,20,50,100";
        pagerElemThumbnails.DefaultPageSize = isAttachmentTab ? 12 : 10;
    }


    /// <summary>
    /// Loads content for media libraries thumbnails view element.
    /// </summary>
    private void ReloadThumbnailsView()
    {
        // Connects repeater with data source
        if (!DataHelper.DataSourceIsEmpty(DataSource))
        {
            repThumbnailsView.DataSource = DataSource;
            repThumbnailsView.PagerForceNumberOfResults = TotalRecords;
            repThumbnailsView.DataBind();
        }
    }


    protected void ThumbnailsViewControl_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        #region "Load the item data"

        var data = new DataRowContainer((DataRowView)e.Item.DataItem);

        string fileNameColumn = FileNameColumn;
        string className = "";

        bool isContent = (SourceType == MediaSourceEnum.Content);

        bool isContentFile = isContent && data.GetValue("ClassName").ToString().EqualsCSafe("cms.file", true);
        bool notAttachment = isContent && !(isContentFile && (data.GetValue("AttachmentGUID") != DBNull.Value));
        if (notAttachment)
        {
            className = DataClassInfoProvider.GetDataClassInfo((int)data.GetValue("NodeClassID")).ClassDisplayName;

            fileNameColumn = "DocumentName";
        }

        // Get information on file
        string fileName = HTMLHelper.HTMLEncode(data.GetValue(fileNameColumn).ToString());
        string ext = HTMLHelper.HTMLEncode(notAttachment ? className : data.GetValue(FileExtensionColumn).ToString().TrimStart('.'));
        string argument = RaiseOnGetArgumentSet(data);

        // Get full media library file name
        bool isInDatabase = true;
        string fullFileName = GetFileName(data);

        IDataContainer importedMediaData = null;

        if (SourceType == MediaSourceEnum.MediaLibraries)
        {
            importedMediaData = RaiseOnFileIsNotInDatabase(fullFileName);
            isInDatabase = (importedMediaData != null);
        }

        bool libraryFolder = ((SourceType == MediaSourceEnum.MediaLibraries) && IsFullListingMode && (data.GetValue(FileExtensionColumn).ToString().ToLowerCSafe() == "<dir>"));
        bool libraryUiFolder = libraryFolder && !((DisplayMode == ControlDisplayModeEnum.Simple) && isInDatabase);

        int width = 0;
        int height = 0;
        // Get thumb preview image dimensions
        int[] thumbImgDimension = { 0, 0 };
        if (ImageHelper.IsSupportedByImageEditor(ext))
        {
            // Width & height
            if (data.ContainsColumn(FileWidthColumn))
            {
                width = ValidationHelper.GetInteger(data.GetValue(FileWidthColumn), 0);
            }
            else if (isInDatabase && importedMediaData.ContainsColumn(FileWidthColumn))
            {
                width = ValidationHelper.GetInteger(importedMediaData.GetValue(FileWidthColumn), 0);
            }

            if (data.ContainsColumn(FileHeightColumn))
            {
                height = ValidationHelper.GetInteger(data.GetValue(FileHeightColumn), 0);
            }
            else if (isInDatabase && importedMediaData.ContainsColumn(FileHeightColumn))
            {
                height = ValidationHelper.GetInteger(importedMediaData.GetValue(FileHeightColumn), 0);
            }

            thumbImgDimension = CMSDialogHelper.GetThumbImageDimensions(height, width, MaxThumbImgHeight, MaxThumbImgWidth);
        }

        // Preview parameters
        IconParameters previewParameters = null;
        if ((SourceType == MediaSourceEnum.MediaLibraries) && isInDatabase)
        {
            previewParameters = RaiseOnGetThumbsItemUrl(importedMediaData, true, thumbImgDimension[0], thumbImgDimension[1], 0, notAttachment);
        }
        else
        {
            previewParameters = RaiseOnGetThumbsItemUrl(data, true, thumbImgDimension[0], thumbImgDimension[1], 0, notAttachment);
        }

        // Item parameters
        IconParameters selectUrlParameters = RaiseOnGetThumbsItemUrl(data, false, 0, 0, 0, notAttachment);
        bool isSelectable = CMSDialogHelper.IsItemSelectable(SelectableContent, ext, isContentFile);

        #endregion


        #region "Standard controls and actions"

        // Load file name
        Label lblName = e.Item.FindControl("lblFileName") as Label;
        if (lblName != null)
        {
            lblName.Text = fileName;
        }

        // Initialize SELECT button
        var btnWarning = e.Item.FindControl("btnWarning") as CMSAccessibleButton;
        if (btnWarning != null)
        {
            // If media file not imported yet - display warning sign
            if (isSelectable && (SourceType == MediaSourceEnum.MediaLibraries) && ((DisplayMode == ControlDisplayModeEnum.Simple) && !isInDatabase && !libraryFolder && !libraryUiFolder))
            {
                btnWarning.ToolTip = GetString("media.file.import");
                btnWarning.OnClientClick = String.Format("ColorizeRow({0}); SetAction('importfile',{1}); RaiseHiddenPostBack(); return false;", ScriptHelper.GetString(GetColorizeID(data)), ScriptHelper.GetString(fullFileName));
            }
            else
            {
                PlaceHolder plcWarning = e.Item.FindControl("plcWarning") as PlaceHolder;
                if (plcWarning != null)
                {
                    plcWarning.Visible = false;
                }
            }
        }

        // Initialize SELECTSUBDOCS button
        var btnSelectSubDocs = e.Item.FindControl("btnSelectSubDocs") as CMSAccessibleButton;
        if (btnSelectSubDocs != null)
        {
            if (IsFullListingMode && (SourceType == MediaSourceEnum.Content))
            {
                int nodeId = ValidationHelper.GetInteger(data.GetValue("NodeID"), 0);

                btnSelectSubDocs.ToolTip = GetString("dialogs.list.actions.showsubdocuments");

                // Check if item is selectable, if not remove select action button
                btnSelectSubDocs.OnClientClick = String.Format("SetParentAction('{0}'); return false;", nodeId);
            }
            else
            {
                PlaceHolder plcSelectSubDocs = e.Item.FindControl("plcSelectSubDocs") as PlaceHolder;
                if (plcSelectSubDocs != null)
                {
                    plcSelectSubDocs.Visible = false;
                }
            }
        }

        // Initialize VIEW button
        var btnView = e.Item.FindControl("btnView") as CMSAccessibleButton;
        if (btnView != null)
        {
            if (!notAttachment && !libraryFolder)
            {
                if (String.IsNullOrEmpty(selectUrlParameters.Url))
                {
                    btnView.OnClientClick = "return false;";
                    btnView.Attributes["style"] = "cursor:default;";
                    btnView.Enabled = false;
                }
                else
                {
                    btnView.ToolTip = GetString("dialogs.list.actions.view");
                    btnView.OnClientClick = String.Format("javascript: window.open({0}); return false;", ScriptHelper.GetString(URLHelper.ResolveUrl(selectUrlParameters.Url)));
                }
            }
            else
            {
                btnView.Visible = false;
            }
        }

        // Initialize EDIT button
        var btnContentEdit = e.Item.FindControl("btnContentEdit") as CMSAccessibleButton;
        if (btnContentEdit != null)
        {
            btnContentEdit.ToolTip = GetString("general.edit");

            Guid guid = Guid.Empty;

            if (SourceType == MediaSourceEnum.MediaLibraries && !libraryFolder && !libraryUiFolder)
            {
                // Media files coming from FS
                if (!data.ContainsColumn("FileGUID"))
                {
                    if ((DisplayMode == ControlDisplayModeEnum.Simple) && !isInDatabase)
                    {
                        btnContentEdit.Attributes["style"] = "cursor: default;";
                        btnContentEdit.Enabled = false;
                    }
                    else
                    {
                        btnContentEdit.OnClientClick = String.Format("$cmsj('#hdnFileOrigName').attr('value', {0}); SetAction('editlibraryui', {1}); RaiseHiddenPostBack(); return false;", ScriptHelper.GetString(EnsureFileName(fileName)), ScriptHelper.GetString(fileName));
                    }
                }
                else
                {
                    guid = ValidationHelper.GetGuid(data.GetValue("FileGUID"), Guid.Empty);
                    btnContentEdit.ScreenReaderDescription = String.Format("{0}|MediaFileGUID={1}&sitename={2}", ext, guid, GetSiteName(data, true));
                    btnContentEdit.PreRender += img_PreRender;
                }
            }
            else if (SourceType == MediaSourceEnum.MetaFile)
            {
                // If MetaFiles being displayed set EDIT action
                string metaExtension = ValidationHelper.GetString(data.GetValue("MetaFileExtension"), string.Empty).ToLowerCSafe();
                Guid metaGuid = ValidationHelper.GetGuid(data.GetValue("MetaFileGUID"), Guid.Empty);

                btnContentEdit.ScreenReaderDescription = String.Format("{0}|metafileguid={1}", metaExtension, metaGuid);
                btnContentEdit.PreRender += img_PreRender;
            }
            else if (!notAttachment && !libraryFolder && !libraryUiFolder)
            {
                string nodeid = "";
                if (SourceType == MediaSourceEnum.Content)
                {
                    nodeid = "&nodeId=" + data.GetValue("NodeID");

                    // Get the node workflow
                    VersionHistoryID = ValidationHelper.GetInteger(data.GetValue("DocumentCheckedOutVersionHistoryID"), 0);
                }

                guid = ValidationHelper.GetGuid(data.GetValue("AttachmentGUID"), Guid.Empty);
                btnContentEdit.ScreenReaderDescription = String.Format("{0}|AttachmentGUID={1}&sitename={2}{3}{4}", ext, guid, GetSiteName(data, false), nodeid, ((VersionHistoryID > 0) ? "&versionHistoryId=" + VersionHistoryID : ""));
                btnContentEdit.PreRender += img_PreRender;
            }
            else
            {
                btnContentEdit.Visible = false;
            }
        }

        #endregion


        #region "Special actions"

        // If attachments being displayed show additional actions
        if (SourceType == MediaSourceEnum.DocumentAttachments)
        {
            // Initialize EDIT button
            var btnEdit = e.Item.FindControl("btnEdit") as CMSAccessibleButton;
            if (btnEdit != null)
            {
                if (!notAttachment)
                {
                    btnEdit.ToolTip = GetString("general.edit");

                    // Get file extension
                    string extension = ValidationHelper.GetString(data.GetValue("AttachmentExtension"), "").ToLowerCSafe();
                    Guid guid = ValidationHelper.GetGuid(data.GetValue("AttachmentGUID"), Guid.Empty);

                    btnEdit.ScreenReaderDescription = String.Format("{0}|AttachmentGUID={1}&sitename={2}&versionHistoryId={3}", extension, guid, GetSiteName(data, false), VersionHistoryID);
                    btnEdit.PreRender += img_PreRender;
                }
            }

            // Initialize UPDATE button
            var dfuElem = e.Item.FindControl("dfuElem") as DirectFileUploader;
            if (dfuElem != null)
            {
                GetAttachmentUpdateControl(ref dfuElem, data);
            }

            // Setup external edit
            var ctrl = ExternalEditHelper.LoadExternalEditControl(e.Item.FindControl("plcExtEdit"), FileTypeEnum.Attachment, null, data, IsLiveSite, TreeNodeObj, true);
            if (ctrl != null)
            {
                ctrl.CssClass = null;
            }

            // Initialize DELETE button
            var btnDelete = e.Item.FindControl("btnDelete") as CMSAccessibleButton;
            if (btnDelete != null)
            {
                btnDelete.ToolTip = GetString("general.delete");

                // Initialize command
                btnDelete.OnClientClick = String.Format("if(DeleteConfirmation() == false){{return false;}} SetAction('attachmentdelete','{0}'); RaiseHiddenPostBack(); return false;", data.GetValue("AttachmentGUID"));
            }

            var plcContentEdit = e.Item.FindControl("plcContentEdit") as PlaceHolder;
            if (plcContentEdit != null)
            {
                plcContentEdit.Visible = false;
            }
        }
        else if ((SourceType == MediaSourceEnum.MediaLibraries) && !data.ContainsColumn("FileGUID") && ((DisplayMode == ControlDisplayModeEnum.Simple) && !libraryFolder && !libraryUiFolder))
        {
            // Initialize DELETE button
            var btnDelete = e.Item.FindControl("btnDelete") as CMSAccessibleButton;
            if (btnDelete != null)
            {
                btnDelete.ToolTip = GetString("general.delete");
                btnDelete.OnClientClick = String.Format("if(DeleteMediaFileConfirmation() == false){{return false;}} SetAction('deletefile',{0}); RaiseHiddenPostBack(); return false;", ScriptHelper.GetString(fullFileName));
            }

            // Hide attachment specific actions
            PlaceHolder plcAttachmentUpdtAction = e.Item.FindControl("plcAttachmentUpdtAction") as PlaceHolder;
            if (plcAttachmentUpdtAction != null)
            {
                plcAttachmentUpdtAction.Visible = false;
            }
        }
        else
        {
            PlaceHolder plcAttachmentActions = e.Item.FindControl("plcAttachmentActions") as PlaceHolder;
            if (plcAttachmentActions != null)
            {
                plcAttachmentActions.Visible = false;
            }
        }

        #endregion


        #region "Library update action"

        if ((SourceType == MediaSourceEnum.MediaLibraries) && (DisplayMode == ControlDisplayModeEnum.Simple))
        {
            // Initialize UPDATE button
            var dfuElemLib = e.Item.FindControl("dfuElemLib") as DirectFileUploader;
            if (dfuElemLib != null)
            {
                Panel pnlDisabledUpdate = (e.Item.FindControl("pnlDisabledUpdate") as Panel);
                if (pnlDisabledUpdate != null)
                {
                    bool hasModifyPermission = RaiseOnGetModifyPermission(data);
                    if (isInDatabase && hasModifyPermission)
                    {
                        GetLibraryUpdateControl(ref dfuElemLib, importedMediaData);

                        pnlDisabledUpdate.Visible = false;
                    }
                    else
                    {
                        pnlDisabledUpdate.Controls.Clear();

                        var disabledIcon = new CMSAccessibleButton
                        {
                            EnableViewState = false,
                            Enabled = false,
                            IconCssClass = "icon-arrow-up-line",
                            IconOnly = true
                        };

                        pnlDisabledUpdate.Controls.Add(disabledIcon);

                        dfuElemLib.Visible = false;
                    }
                }
            }

            // Setup external edit
            if (isInDatabase)
            {
                ExternalEditHelper.LoadExternalEditControl(e.Item.FindControl("plcExtEditMfi"), FileTypeEnum.MediaFile, GetSiteName(data, true), importedMediaData, IsLiveSite, null, true);
            }
        }
        else if (((SourceType == MediaSourceEnum.Content) && (DisplayMode == ControlDisplayModeEnum.Default) && !notAttachment && !libraryFolder && !libraryUiFolder))
        {
            // Setup external edit
            if (data.ContainsColumn("AttachmentGUID"))
            {
                LoadExternalEditControl(e.Item, FileTypeEnum.Attachment);
            }
        }
        else if (((SourceType == MediaSourceEnum.MediaLibraries) && (DisplayMode == ControlDisplayModeEnum.Default) && !libraryFolder && !libraryUiFolder))
        {
            // Setup external edit
            if (data.ContainsColumn("FileGUID"))
            {
                LoadExternalEditControl(e.Item, FileTypeEnum.MediaFile);
            }
        }
        else
        {
            var plcLibraryUpdtAction = e.Item.FindControl("plcLibraryUpdtAction") as PlaceHolder;
            if (plcLibraryUpdtAction != null)
            {
                plcLibraryUpdtAction.Visible = false;
            }
        }

        if ((SourceType == MediaSourceEnum.MediaLibraries) && libraryFolder && IsFullListingMode)
        {
            // Initialize SELECT SUB-FOLDERS button
            var btn = e.Item.FindControl("imgSelectSubFolders") as CMSAccessibleButton;
            if (btn != null)
            {
                btn.Visible = true;
                btn.ToolTip = GetString("dialogs.list.actions.showsubfolders");
                btn.OnClientClick = String.Format("SetLibParentAction({0}); return false;", ScriptHelper.GetString(fileName));
            }
        }
        else
        {
            var plcSelectSubFolders = e.Item.FindControl("plcSelectSubFolders") as PlaceHolder;
            if (plcSelectSubFolders != null)
            {
                plcSelectSubFolders.Visible = false;
            }
        }

        #endregion


        #region "File image"

        // Selectable area
        Panel pnlItemInageContainer = e.Item.FindControl("pnlThumbnails") as Panel;
        if (pnlItemInageContainer != null)
        {
            if (isSelectable)
            {
                if ((DisplayMode == ControlDisplayModeEnum.Simple) && !isInDatabase)
                {
                    if (libraryFolder || libraryUiFolder)
                    {
                        pnlItemInageContainer.Attributes["onclick"] = String.Format("SetAction('morefolderselect', {0}); RaiseHiddenPostBack(); return false;", ScriptHelper.GetString(fileName));
                    }
                    else
                    {
                        pnlItemInageContainer.Attributes["onclick"] = String.Format("ColorizeRow({0}); SetAction('importfile', {1}); RaiseHiddenPostBack(); return false;", ScriptHelper.GetString(GetColorizeID(data)), ScriptHelper.GetString(fullFileName));
                    }
                }
                else
                {
                    pnlItemInageContainer.Attributes["onclick"] = String.Format("ColorizeRow({0}); SetSelectAction({1}); return false;", ScriptHelper.GetString(GetColorizeID(data)), ScriptHelper.GetString(String.Format("{0}|URL|{1}", argument, selectUrlParameters.Url)));
                }
                pnlItemInageContainer.Attributes["style"] = "cursor:pointer;";
            }
            else
            {
                pnlItemInageContainer.Attributes["style"] = "cursor:default;";
            }
        }

        // Image area
        Image imgFile = e.Item.FindControl("imgFile") as Image;
        if (imgFile != null)
        {
            string chset = Guid.NewGuid().ToString();
            var previewUrl = previewParameters.Url;
            previewUrl = URLHelper.AddParameterToUrl(previewUrl, "chset", chset);

            // Add latest version requirement for live site
            int versionHistoryId = VersionHistoryID;
            if (IsLiveSite && (versionHistoryId > 0))
            {
                // Add requirement for latest version of files for current document
                string newparams = String.Format("latestforhistoryid={0}&hash={1}", versionHistoryId, ValidationHelper.GetHashString("h" + versionHistoryId));

                previewUrl += "&" + newparams;
            }

            if (String.IsNullOrEmpty(previewParameters.IconClass))
            {
                imgFile.ImageUrl = previewUrl;
                imgFile.AlternateText = TextHelper.LimitLength(fileName, 10);
                imgFile.Attributes["title"] = fileName.Replace("\"", "\\\"");

                // Ensure tooltip - only text description
                if (isInDatabase)
                {
                    UIHelper.EnsureTooltip(imgFile, previewUrl, width, height, GetTitle(data, isContentFile), fileName, ext, GetDescription(data, isContentFile), null, 300);
                }
            }
            else
            {
                var imgIcon = e.Item.FindControl(("imgFileIcon")) as Label;
                if ((imgIcon != null) && imgIcon.Controls.Count < 1)
                {
                    className = ValidationHelper.GetString(data.GetValue("ClassName"), String.Empty);
                    var icon = UIHelper.GetDocumentTypeIcon(null, className, previewParameters.IconClass, previewParameters.IconSize);

                    imgIcon.Controls.Add(new LiteralControl(icon));

                    // Ensure tooltip - only text description
                    if (isInDatabase)
                    {
                        UIHelper.EnsureTooltip(imgIcon, previewUrl, width, height, GetTitle(data, isContentFile), fileName, ext, GetDescription(data, isContentFile), null, 300);
                    }

                    imgFile.Visible = false;
                }
            }
        }

        #endregion


        // Display only for ML UI
        if ((DisplayMode == ControlDisplayModeEnum.Simple) && !libraryFolder)
        {
            PlaceHolder plcSelectionBox = e.Item.FindControl("plcSelectionBox") as PlaceHolder;
            if (plcSelectionBox != null)
            {
                plcSelectionBox.Visible = true;

                // Multiple selection check-box
                CMSCheckBox chkSelected = e.Item.FindControl("chkSelected") as CMSCheckBox;
                if (chkSelected != null)
                {
                    chkSelected.ToolTip = GetString("general.select");
                    chkSelected.InputAttributes["alt"] = fullFileName;

                    HiddenField hdnItemName = e.Item.FindControl("hdnItemName") as HiddenField;
                    if (hdnItemName != null)
                    {
                        hdnItemName.Value = fullFileName;
                    }
                }
            }
        }
    }

    #endregion


    #region "Raise events methods"

    /// <summary>
    /// Fires specific action and returns result provided by the parent control.
    /// </summary>
    /// <param name="data">Data related to the action</param>
    private string RaiseOnGetArgumentSet(IDataContainer data)
    {
        if (GetArgumentSet != null)
        {
            return GetArgumentSet(data);
        }
        return "";
    }


    /// <summary>
    /// Fires specific action and returns result provided by the parent control.
    /// </summary>
    /// <param name="data">Data related to the action</param>
    /// <param name="isPreview">Indicates whether the URL is required for preview item</param>
    /// <param name="notAttachment">Indicates whether the URL is required for non-attachment item</param>
    private string RaiseOnGetListItemUrl(IDataContainer data, bool isPreview, bool notAttachment)
    {
        if (GetListItemUrl != null)
        {
            return GetListItemUrl(data, isPreview, notAttachment);
        }
        return "";
    }


    /// <summary>
    /// Fires specific action and returns result provided by the parent control.
    /// </summary>
    /// <param name="data">Data related to the action</param>
    /// <param name="isPreview">Indicates whether the image is required as part of preview</param>
    /// <param name="width">Maximum width of the preview image</param>
    /// <param name="maxSideSize">Maximum size of the preview image. If full-size required parameter gets zero value</param>
    /// <param name="notAttachment">Indicates whether the URL is required for non-attachment item</param>
    /// <param name="height">Maximum height of the preview image</param>
    private IconParameters RaiseOnGetThumbsItemUrl(IDataContainer data, bool isPreview, int height, int width, int maxSideSize, bool notAttachment)
    {
        if (GetThumbsItemUrl != null)
        {
            return GetThumbsItemUrl(data, isPreview, height, width, maxSideSize, notAttachment);
        }
        return null;
    }


    /// <summary>
    /// Raises event when information on import status of specified file is required.
    /// </summary>
    /// <param name="fileName">Name of the file (including extension)</param>
    private IDataContainer RaiseOnFileIsNotInDatabase(string fileName)
    {
        if (GetInformation != null)
        {
            object result = GetInformation("fileisnotindatabase", fileName);
            if (result != null)
            {
                // Ensure the data container
                return (result is DataRow ? new DataRowContainer((DataRow)result) : (result is DataRowView ? new DataRowContainer((DataRowView)result) : (IDataContainer)result));
            }
            return null;
        }
        return null;
    }


    /// <summary>
    /// Raises event when ID of the current site is required.
    /// </summary>
    private int RaiseOnSiteIdRequired()
    {
        if (GetInformation != null)
        {
            return (int)GetInformation("siteidrequired", null);
        }

        return 0;
    }


    /// <summary>
    /// Raises event when modify permission is required.
    /// </summary>
    /// <param name="data">Data container</param>
    private bool RaiseOnGetModifyPermission(IDataContainer data)
    {
        if (GetModifyPermission != null)
        {
            return GetModifyPermission(data);
        }
        return true;
    }

    #endregion


    #region "External edit methods"

    /// <summary>
    /// Loads the external edit control and sets visibility of other controls
    /// </summary>
    /// <param name="repeaterItem">Repeater item</param>
    /// <param name="type">Source type</param>
    private void LoadExternalEditControl(RepeaterItem repeaterItem, FileTypeEnum type)
    {
        var plcAttachmentActions = repeaterItem.FindControl("plcAttachmentActions") as PlaceHolder;
        var plcAttachmentUpdtAction = repeaterItem.FindControl("plcAttachmentUpdtAction") as PlaceHolder;
        var plcLibraryUpdtAction = repeaterItem.FindControl("plcLibraryUpdtAction") as PlaceHolder;
        var plcExt = repeaterItem.FindControl("plcExtEdit") as PlaceHolder;
        var plcExtMfi = repeaterItem.FindControl("plcExtEditMfi") as PlaceHolder;
        var pnlDisabledUpdate = (repeaterItem.FindControl("pnlDisabledUpdate") as Panel);
        var dfuLib = repeaterItem.FindControl("dfuElemLib") as DirectFileUploader;
        var dfu = repeaterItem.FindControl("dfuElem") as DirectFileUploader;
        var btnEdit = repeaterItem.FindControl("btnEdit") as WebControl;
        var btnDelete = repeaterItem.FindControl("btnDelete") as WebControl;

        if ((plcAttachmentActions != null) && (plcLibraryUpdtAction != null) && (plcAttachmentUpdtAction != null) && (plcExt != null)
            && (plcExtMfi != null) && (pnlDisabledUpdate != null) && (dfuLib != null) && (dfu != null) && (btnEdit != null) && (btnDelete != null))
        {
            var data = new DataRowContainer((DataRowView)repeaterItem.DataItem);

            plcAttachmentActions.Visible = true;
            plcAttachmentUpdtAction.Visible = false;
            pnlDisabledUpdate.Visible = false;
            dfuLib.Visible = false;
            dfu.Visible = false;
            btnEdit.Visible = false;
            btnDelete.Visible = false;
            plcExt.Visible = false;
            plcExtMfi.Visible = false;

            plcLibraryUpdtAction.Visible = (type == FileTypeEnum.MediaFile);

            ExternalEditHelper.LoadExternalEditControl(plcExt, type, null, data, IsLiveSite, TreeNodeObj, true);
        }
    }

    #endregion
}