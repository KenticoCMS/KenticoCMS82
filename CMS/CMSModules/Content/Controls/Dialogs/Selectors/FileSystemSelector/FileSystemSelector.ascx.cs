using System;
using System.Security;
using System.Text;
using System.Web.UI;
using System.Collections;

using CMS.ExtendedControls;
using CMS.Helpers;
using CMS.IO;
using CMS.Base;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.DataEngine;

public partial class CMSModules_Content_Controls_Dialogs_Selectors_FileSystemSelector_FileSystemSelector : CMSUserControl
{
    private const char ARG_SEPARATOR = '|';


    #region "Private variables"

    // Content variables
    private string mNodeID = string.Empty;
    protected FileSystemDialogConfiguration mConfig;
    private Hashtable mParameters;

    #endregion


    #region "Properties"

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
    protected ItemProperties Properties
    {
        get
        {
            return pathProperties;
        }
    }


    /// <summary>
    /// Update panel where properties control resides.
    /// </summary>
    protected UpdatePanel PropertiesUpdatePanel
    {
        get
        {
            return pnlUpdateProperties;
        }
    }


    /// <summary>
    /// Gets or sets ID of the node selected in the content tree.
    /// </summary>
    private string NodeID
    {
        get
        {
            if (String.IsNullOrEmpty(mNodeID))
            {
                mNodeID = ValidationHelper.GetString(hdnLastNodeSlected.Value, string.Empty);
            }

            return mNodeID;
        }
        set
        {
            if (!value.StartsWithCSafe(FullStartingPath, true))
            {
                value = FullStartingPath;
            }

            mNodeID = value;
            hdnLastNodeSlected.Value = value;
        }
    }


    /// <summary>
    /// Maximum number of tree nodes displayed within the tree.
    /// </summary>
    private int MaxTreeNodes
    {
        get
        {
            string siteName = SiteContext.CurrentSiteName;
            return SettingsKeyInfoProvider.GetIntValue((String.IsNullOrEmpty(siteName) ? string.Empty : siteName + ".") + "CMSMaxTreeNodes");
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
    /// Indicates whether the content tree is displaying more than max tree nodes.
    /// </summary>
    private bool IsDisplayMore
    {
        get
        {
            return fileSystemView.IsDisplayMore;
        }
        set
        {
            fileSystemView.IsDisplayMore = value;
        }
    }


    /// <summary>
    /// Gets or sets selected item to colorize.
    /// </summary>
    private String ItemToColorize
    {
        get
        {
            return ValidationHelper.GetString(ViewState["ItemToColorize"], String.Empty);
        }
        set
        {
            ViewState["ItemToColorize"] = value;
        }
    }


    /// <summary>
    /// Value of node under which more content should be displayed.
    /// </summary>
    private string MoreContentNode
    {
        get
        {
            return ValidationHelper.GetString(ViewState["MoreContentNode"], string.Empty);
        }
        set
        {
            ViewState["MoreContentNode"] = value;
        }
    }


    /// <summary>
    /// Dialog configuration.
    /// </summary>
    public FileSystemDialogConfiguration Config
    {
        get
        {
            return mConfig ?? (mConfig = new FileSystemDialogConfiguration());
        }
        set
        {
            mConfig = value;
        }
    }


    /// <summary>
    /// Full file system starting path of dialog. Expects physical file system path with backslashes.
    /// </summary>
    public string FullStartingPath
    {
        get
        {
            if (Config.StartingPath.StartsWithCSafe("~"))
            {
                return Server.MapPath(Config.StartingPath).TrimEnd('\\');
            }
            else
            {
                if (Config.StartingPath.EndsWithCSafe(":\\"))
                {
                    return Config.StartingPath;
                }
            }
            return Config.StartingPath.TrimEnd('\\');
        }
    }


    /// <summary>
    /// Messages placeholder
    /// </summary>
    public override MessagesPlaceHolder MessagesPlaceHolder
    {
        get
        {
            return fileSystemView.MessagesPlaceHolder;
        }
    }

    #endregion


    #region "Public properties"

    /// <summary>
    /// Dialog parameters collection.
    /// </summary>
    public Hashtable Parameters
    {
        get
        {
            if (mParameters == null)
            {
                // Try to get parameters from the session
                object dp = SessionHelper.GetValue("DialogParameters");
                if (dp != null)
                {
                    mParameters = (dp as Hashtable);
                }
            }
            return mParameters;
        }
        set
        {
            mParameters = value;
            SessionHelper.SetValue("DialogParameters", value);
        }
    }


    /// <summary>
    /// Indicates if help icon should be hidden.
    /// </summary>
    public bool RemoveHelpIcon
    {
        get
        {
            return menuElem.RemoveHelpIcon;
        }
        set
        {
            menuElem.RemoveHelpIcon = value;
        }
    }

    #endregion


    #region "Control methods"

    /// <summary>
    /// Init.
    /// </summary>
    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        InitFromQueryString();
    }


    /// <summary>
    /// Pre render.
    /// </summary>
    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        // High-light item being edited
        if (ItemToColorize != String.Empty)
        {
            ColorizeRow(ItemToColorize);
        }

        // Display info on listing more content
        if (IsDisplayMore && !Config.ShowFolders) //curently selected more object && (TreeNodeObj != null))
        {
            string closeLink = String.Format("<span class=\"ListingClose\" style=\"cursor: pointer;\" onclick=\"SetAction('closelisting', ''); RaiseHiddenPostBack(); return false;\">{0}</span>", GetString("general.close"));
            string currentPath = "<span class=\"ListingPath\">";

            // Display relative paths with tilda
            if (Config.StartingPath.StartsWithCSafe("~"))
            {
                string serverPath = Server.MapPath(Config.StartingPath).TrimEnd('\\');
                currentPath += NodeID.Replace(serverPath.Substring(0, serverPath.LastIndexOfCSafe('\\') + 1), string.Empty);
            }
            else
            {
                currentPath += NodeID.Replace(NodeID.Substring(0, Config.StartingPath.TrimEnd('\\').LastIndexOfCSafe('\\') + 1), string.Empty);
            }
            currentPath += "</span>";

            string listingMsg = string.Format(GetString("dialogs.filesystem.listinginfo"), currentPath, closeLink);
            fileSystemView.DisplayListingInfo(listingMsg);
        }

        folderActions.EnableDeleteFolder = !FullStartingPath.EqualsCSafe(NodeID, true);
    }


    /// <summary>
    /// Page load.
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!StopProcessing)
        {
            SetupControls();
            EnsureLoadedData();
        }
        else
        {
            Visible = false;
        }
    }

    #endregion


    #region "Control initialization"

    /// <summary>
    /// Initializes additional controls.
    /// </summary>
    private void SetupControls()
    {
        // Initialize design scripts
        InitializeDesignScripts();

        fileSystemView.IsLiveSite = IsLiveSite;
        fileSystemView.ViewMode = menuElem.SelectedViewMode;

        InitializeFileSystemView();

        InitializeMenuElem();

        pathProperties.DialogConfig = Config;

        // Initialize content tree control
        InitializeFileSystemTree();

        if (!RequestHelper.IsAsyncPostback())
        {
            // Initialize scripts
            InitializeControlScripts();

            string parameter = FullStartingPath;
            if (!String.IsNullOrEmpty(Config.DefaultPath))
            {
                parameter = Path.Combine(parameter, Path.EnsureBackslashes(Config.DefaultPath));
            }

            // Handle the folder action
            HandleFolderAction(parameter.Replace("'", "\\'"), false);
            if (!String.IsNullOrEmpty(Config.SelectedPath) || Config.ShowFolders)
            {
                if (Config.ShowFolders && String.IsNullOrEmpty(Config.SelectedPath))
                {
                    Config.SelectedPath = Config.StartingPath;
                }
                string fsPath = Config.SelectedPath;
                if (Config.StartingPath.StartsWithCSafe("~"))
                {
                    try
                    {
                        fsPath = Server.MapPath(fsPath);
                    }
                    catch
                    {
                        // Set default path
                        fsPath = string.Empty;
                    }
                }

                bool isFile = File.Exists(fsPath);
                bool exist = isFile || Directory.Exists(fsPath);

                // If folder try to select default or starting folder
                if (!exist && Config.ShowFolders)
                {
                    try
                    {
                        fsPath = parameter;
                        exist = Directory.Exists(fsPath);
                        if (!exist)
                        {
                            fsPath = Server.MapPath(Config.StartingPath);
                            exist = Directory.Exists(fsPath);
                        }
                    }
                    catch
                    {
                    }
                }
                if (exist)
                {
                    string size = string.Empty;
                    try
                    {
                        if (isFile)
                        {
                            FileInfo fi = FileInfo.New(fsPath);
                            size = DataHelper.GetSizeString(ValidationHelper.GetLong(fi.Length, 0));
                        }
                    }
                    catch
                    {
                    }

                    SelectMediaItem(String.Format("{0}{1}{2}{1}{3}", fsPath, ARG_SEPARATOR, size, isFile));
                }
            }
        }
    }


    /// <summary>
    /// Initialize design jQuery scripts.
    /// </summary>
    private void InitializeDesignScripts()
    {
        CMSDialogHelper.RegisterDialogHelper(Page);
        StringBuilder sb = new StringBuilder();
        sb.Append("setTimeout('InitializeDesign();',10);");
        sb.Append("$cmsj(window).resize(function() { InitializeDesign(); });");
        ScriptManager.RegisterStartupScript(Page, typeof(Page), "designScript", sb.ToString(), true);
    }


    /// <summary>
    /// Initializes all the script required for communication between controls.
    /// </summary>
    private void InitializeControlScripts()
    {
        // SetAction function setting action name and passed argument
        string script = String.Format(@"
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
function imageEdit_FileSystemRefresh(arg){{{{
    SetAction('imageedit', arg);
    RaiseHiddenPostBack();
}}}}", hdnAction.ClientID, hdnArgument.ClientID);

        // Get reference causing postback to hidden button
        script += String.Format("function RaiseHiddenPostBack(){{{0};}}\n", ControlsHelper.GetPostBackEventReference(hdnButton, string.Empty));

        ltlScript.Text = ScriptHelper.GetScript(script);
    }


    /// <summary>
    /// Initialization of file grid control.
    /// </summary>
    private void InitializeFileSystemView()
    {
        fileSystemView.Config = Config;
    }


    /// <summary>
    /// Initializes content tree element.
    /// </summary>
    private void InitializeFileSystemTree()
    {
        treeFileSystem.Visible = true;

        treeFileSystem.DeniedNodePostback = false;
        treeFileSystem.NodeTextTemplate = String.Format("<span class=\"ContentTreeItem\" onclick=\"SelectNode('##NODEID##', this); SetAction('contentselect', '##NODEID##{0}##NODECHILDNODESCOUNT##'); RaiseHiddenPostBack(); return false;\">##ICON##<span class=\"Name\">##NODENAME##</span></span>", ARG_SEPARATOR);
        treeFileSystem.SelectedNodeTextTemplate = String.Format("<span id=\"treeSelectedNode\" class=\"ContentTreeSelectedItem\" onclick=\"SelectNode('##NODEID##', this); SetAction('contentselect', '##NODEID##{0}##NODECHILDNODESCOUNT##'); RaiseHiddenPostBack(); return false;\">##ICON##<span class=\"Name\">##NODENAME##</span></span>", ARG_SEPARATOR);
        treeFileSystem.MaxTreeNodeText = String.Format("<span class=\"ContentTreeItem\" onclick=\"SetAction('morecontentselect', '##PARENTNODEID##'); RaiseHiddenPostBack(); return false;\"><span class=\"Name\">{0}</span></span>", GetString("general.SeeListing"));
        treeFileSystem.IsLiveSite = IsLiveSite;
        treeFileSystem.ExpandDefaultPath = true;
        treeFileSystem.StartingPath = Config.StartingPath;
        if (treeFileSystem.DefaultPath == String.Empty)
        {
            treeFileSystem.DefaultPath = Config.DefaultPath;
        }
        treeFileSystem.AllowedFolders = Config.AllowedFolders;
        treeFileSystem.ExcludedFolders = Config.ExcludedFolders;
    }


    /// <summary>
    /// Ensures that required data are displayed.
    /// </summary>
    private void EnsureLoadedData()
    {
        // If no action takes place
        if ((CurrentAction == string.Empty) && (URLHelper.IsPostback()))
        {
            fileSystemView.StartingPath = NodeID;
        }
    }

    #endregion


    #region "Common event methods"

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

            case "refresh":
                HandleRefreshAction();
                break;

            case "refreshtree":
                HandleTreeRefreshAction(CurrentArgument);
                break;

            case "delete":
                HandleDeleteAction(CurrentArgument);
                break;

            case "morecontentselect":
            case "contentselect":
                // Reset previous filter value
                ResetSearchFilter();

                string[] argArr = CurrentArgument.Split(ARG_SEPARATOR);
                int childNodesCnt = argArr.Length == 2 ? ValidationHelper.GetInteger(argArr[1], 0) : 0;

                // If more content is requested
                IsDisplayMore = (!IsDisplayMore ? ((CurrentAction == "morecontentselect") || CheckIfDisplayMore(childNodesCnt)) : IsDisplayMore);
                HandleFolderAction(argArr[0], IsDisplayMore);
                if (Config.ShowFolders)
                {
                    HandleSelectAction(String.Format("{0}{1}{1}false", argArr[0], ARG_SEPARATOR));
                }
                break;

            case "parentselect":
                try
                {
                    DirectoryInfo dir = DirectoryInfo.New(CurrentArgument);
                    int childNodes = dir.GetDirectories().Length;
                    if (CheckIfDisplayMore(childNodes))
                    {
                        IsDisplayMore = true;
                    }
                }
                // If error occurred don't do a thing
                catch
                {
                }

                HandleFolderAction(CurrentArgument, true);
                break;

            case "closelisting":
                IsDisplayMore = false;
                MoreContentNode = null;
                HandleFolderAction(NodeID, false);
                break;

            case "cancelfolder":
                ScriptManager.RegisterStartupScript(Page, typeof(Page), "EnsureTopWindow", "if (self.focus) { self.focus(); }", true);
                ClearActionElems();
                break;

            case "imageedit":
                HandleRefreshAction();
                ColorizeLastSelectedRow();
                break;

            default:
                ColorizeLastSelectedRow();
                pnlUpdateView.Update();
                break;
        }
    }


    /// <summary>
    /// Checks if tree is displaying more items than MaxTreeNodes.
    /// </summary>
    /// <param name="childNodes">Number of children folders</param>
    private bool CheckIfDisplayMore(int childNodes)
    {
        return (childNodes > MaxTreeNodes) && (MaxTreeNodes > 0);
    }


    /// <summary>
    /// Handles actions occurring when some text is searched.
    /// </summary>
    /// <param name="argument">Argument holding information on searched text</param>
    private void HandleSearchAction(string argument)
    {
        LastSearchedValue = argument;

        // Load new data filtered by searched text 
        fileSystemView.SearchText = argument;
        fileSystemView.StartingPath = NodeID;
        // Reload content
        fileSystemView.Reload();
        pnlUpdateView.Update();

        // Keep focus in search text box
        ScriptManager.RegisterStartupScript(Page, typeof(Page), "SetSearchFocus", "setTimeout('SetSearchFocus();', 200);", true);

        // Forget recent action
        ClearActionElems();
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
    /// Initializes menu element.
    /// </summary>
    private void InitializeMenuElem()
    {
        plcFolderActions.Visible = plcMenu.Visible = Config.AllowManage;

        folderActions.TargetFolderPath = menuElem.TargetFolderPath = NodeID;

        menuElem.TargetFolderPath = NodeID;
        menuElem.AllowedExtensions = Config.AllowedExtensions;
        menuElem.NewTextFileExtension = Config.NewTextFileExtension;
    }


    /// <summary>
    /// Handles actions occurring when refresh is requested.
    /// </summary>
    private void HandleRefreshAction()
    {
        // Load new data filtered by searched text 
        fileSystemView.SearchText = LastSearchedValue;
        fileSystemView.StartingPath = NodeID;

        // Reload the file system view
        fileSystemView.Reload();
        pnlUpdateView.Update();

        // Forget recent action
        ClearActionElems();
    }


    /// <summary>
    /// Handles actions occurring when tree refresh is requested.
    /// </summary>
    /// <param name="argument">Argument holding information on requested item</param>
    private void HandleTreeRefreshAction(string argument)
    {
        InitializeFileSystemTree();

        // Fill with new info
        NodeID = argument;

        treeFileSystem.DefaultPath = NodeID;
        treeFileSystem.ExpandDefaultPath = true;

        treeFileSystem.ReloadData();
        pnlUpdateTree.Update();
        pnlUpdateMenu.Update();

        // Reload the file system view
        fileSystemView.StartingPath = NodeID;
        fileSystemView.Reload();

        pnlUpdateView.Update();

        InitializeMenuElem();
        folderActions.EnableDeleteFolder = !FullStartingPath.EqualsCSafe(NodeID, true);
        folderActions.Update();
        menuElem.UpdateActionsMenu();


        // Forget recent action
        ClearActionElems();
    }


    /// <summary>
    /// Handles actions occurring when some item is deleted.
    /// </summary>
    /// <param name="argument">Argument holding information on deleted item</param>
    private void HandleDeleteAction(string argument)
    {
        if (!string.IsNullOrEmpty(argument))
        {
            string[] argArr = argument.Split(ARG_SEPARATOR);
            if (argArr.Length >= 2)
            {
                // Get information from argument
                string path = argArr[0];
                bool isFile = ValidationHelper.GetBoolean(argArr[2], true);

                if (path.StartsWithCSafe(NodeID, true))
                {
                    if (isFile && File.Exists(path))
                    {
                        File.Delete(path);
                    }
                    else if (Directory.Exists(path))
                    {
                        Directory.Delete(path);
                    }
                }
                else
                {
                    ShowError(GetString("dialogs.filesystem.invalidfilepath"));
                }

                // Load new data filtered by searched text 
                fileSystemView.SearchText = LastSearchedValue;
                fileSystemView.StartingPath = NodeID;

                // Reload the file system view
                fileSystemView.Reload();
                pnlUpdateView.Update();

                // Clear selected item
                Properties.ClearProperties();
                pnlUpdateProperties.Update();
            }
        }

        // Forget recent action
        ClearActionElems();
    }


    /// <summary>
    /// Handles actions related to the folders.
    /// </summary>
    /// <param name="argument">Argument related to the folder action</param>
    /// <param name="forceReload">Indicates if load should be forced</param>
    private void HandleFolderAction(string argument, bool forceReload)
    {
        NodeID = ValidationHelper.GetString(argument, string.Empty);

        // Reload content tree if necessary
        if (forceReload)
        {
            InitializeFileSystemTree();

            // Fill with new info
            treeFileSystem.DefaultPath = NodeID;
            treeFileSystem.ExpandDefaultPath = true;

            treeFileSystem.ReloadData();
            pnlUpdateTree.Update();

            ScriptManager.RegisterStartupScript(Page, typeof(Page), "EnsureTopWindow", "if (self.focus) { self.focus(); }", true);
        }

        ColorizeLastSelectedRow();

        // Get parent node ID info
        string parentId = string.Empty;
        if (FullStartingPath.ToLowerCSafe() != NodeID.ToLowerCSafe())
        {
            try
            {
                parentId = (DirectoryInfo.New(NodeID)).Parent.FullName;
            }
            // Access denied to parent
            catch (SecurityException)
            {
            }
        }

        menuElem.ShowParentButton = !String.IsNullOrEmpty(parentId);
        menuElem.NodeParentID = parentId;

        fileSystemView.Config = Config;

        // Load new data
        if ((Config.ShowFolders) && (NodeID.LastIndexOfCSafe('\\') != -1))
        {
            fileSystemView.StartingPath = NodeID.Substring(0, argument.LastIndexOfCSafe('\\') + 1);
        }

        fileSystemView.StartingPath = NodeID;

        // Reload view control's content
        fileSystemView.Reload();
        pnlUpdateView.Update();

        InitializeMenuElem();
        menuElem.UpdateActionsMenu();
        folderActions.Update();
        pnlUpdateMenu.Update();

        ClearActionElems();
    }

    #endregion


    #region "Helper methods"

    /// <summary>
    /// Returns selected item parameters as name-value collection.
    /// </summary>
    public void GetSelectedItem()
    {
        if (Properties.Validate())
        {
            // Get selected item information
            Hashtable properties = Properties.GetItemProperties();

            // Get JavaScript for inserting the item
            string script = CMSDialogHelper.GetFileSystemItem(properties);
            if (!string.IsNullOrEmpty(script))
            {
                ScriptManager.RegisterStartupScript(Page, typeof(Page), "insertItemScript", script, true);
            }
        }
        else
        {
            // Display error message
            pnlUpdateProperties.Update();
        }
    }


    /// <summary>
    /// Performs actions necessary to select particular item from a list.
    /// </summary>
    private void SelectMediaItem(string argument)
    {
        if (!string.IsNullOrEmpty(argument))
        {
            string[] argArr = argument.Split(ARG_SEPARATOR);
            if (argArr.Length >= 2)
            {
                // Get information from argument
                string path = argArr[0];
                string size = ValidationHelper.GetString(argArr[1], string.Empty);
                bool isFile = ValidationHelper.GetBoolean(argArr[2], true);

                bool avoidPropUpdate = ItemToColorize.EqualsCSafe(path, true);

                if ((isFile) && (File.Exists(path)))
                {
                    FileInfo fi = FileInfo.New(path);
                    path = fi.FullName;
                }
                else
                {
                    if (Directory.Exists(path))
                    {
                        DirectoryInfo di = DirectoryInfo.New(path);
                        path = di.FullName;
                    }
                }

                ItemToColorize = path.Replace("\\", "\\\\").Replace("'", "\\'").ToLowerCSafe();

                if (!avoidPropUpdate)
                {
                    // Get selected properties from session
                    Hashtable selectedParameters = SessionHelper.GetValue("DialogSelectedParameters") as Hashtable;
                    if (selectedParameters == null)
                    {
                        selectedParameters = new Hashtable();
                    }

                    // Update selected properties
                    selectedParameters[DialogParameters.ITEM_PATH] = path;
                    selectedParameters[DialogParameters.ITEM_RESOLVED_PATH] = URLHelper.ResolveUrl(path);
                    selectedParameters[DialogParameters.ITEM_SIZE] = size;
                    selectedParameters[DialogParameters.ITEM_ISFILE] = isFile;
                    selectedParameters[DialogParameters.ITEM_RELATIVEPATH] = Config.StartingPath.StartsWithCSafe("~");

                    // Force media properties control to load selected item
                    Properties.LoadItemProperties(selectedParameters);

                    SessionHelper.SetValue("DialogSelectedParameters", selectedParameters);
                    // Update properties panel
                    PropertiesUpdatePanel.Update();
                }
            }
        }
    }


    /// <summary>
    /// Highlights item specified by its ID.
    /// </summary>
    /// <param name="itemId">String representation of item ID</param>
    protected void ColorizeRow(string itemId)
    {
        // Keep item selected
        ScriptManager.RegisterStartupScript(Page, typeof(Page), "ColorizeSelectedRow", String.Format("function tryColorizeRow(itemId) {{ if (window.ColorizeRow){{ ColorizeRow(itemId); }} else {{ setTimeout(\'tryColorizeRow(\"{0}\");\', 500); }} }}; tryColorizeRow(\"{0}\");", itemId), true);
    }


    /// <summary>
    /// Clears hidden control elements fo future use.
    /// </summary>
    private void ClearActionElems()
    {
        CurrentAction = string.Empty;
        hdnArgument.Value = string.Empty;
    }


    /// <summary> 
    /// Highlights row recently selected.
    /// </summary>
    protected void ColorizeLastSelectedRow()
    {
        // Keep item selected
        ScriptManager.RegisterStartupScript(Page, typeof(Page), "ColorizeLastSelectedRow", "if (window.ColorizeLastRow) { window.ColorizeLastRow(); }", true);
    }


    /// <summary>
    /// Loads selected item parameters into the selector.
    /// </summary>
    public void LoadItemConfiguration()
    {
        // Load properties
        Properties.LoadItemProperties(Parameters);
        pnlUpdateProperties.Update();

        // Remember item to colorize
        ItemToColorize = NodeID.Replace("\\", "\\\\").Replace("'", "\\'");
    }


    /// <summary>
    /// Initialization from query string.
    /// </summary>
    public void InitFromQueryString()
    {
        // Get allowed and excluded folders and extensions        
        FileSystemDialogConfiguration config = Config;

        config.AllowManage = QueryHelper.GetBoolean("allow_manage", config.AllowManage);
        config.AllowedExtensions = QueryHelper.GetString("allowed_extensions", config.AllowedExtensions);
        config.NewTextFileExtension = QueryHelper.GetString("newfile_extension", config.NewTextFileExtension);
        config.AllowedFolders = QueryHelper.GetString("allowed_folders", config.AllowedFolders);
        config.ExcludedExtensions = QueryHelper.GetString("excluded_extensions", config.ExcludedExtensions);
        config.ExcludedFolders = QueryHelper.GetString("excluded_folders", config.ExcludedFolders);
        config.AllowNonApplicationPath = QueryHelper.GetBoolean("allow_nonapp_path", config.AllowNonApplicationPath);

        // Get starting path
        var startingPath = QueryHelper.GetString("starting_path", "~/");
        if (startingPath.StartsWithCSafe("~") && (startingPath != "~/"))
        {
            startingPath = startingPath.TrimEnd('/');
        }
        else
        {
            // If only application path allowed, set it
            if (!config.AllowNonApplicationPath)
            {
                string startPath = startingPath;
                if (startingPath.StartsWithCSafe("~"))
                {
                    startPath = Server.MapPath(startingPath);
                }
                if (!startPath.StartsWithCSafe(Server.MapPath("~/")))
                {
                    startingPath = "~/";
                }
            }
            else if (!startingPath.EndsWithCSafe(":\\"))
            {
                startingPath = startingPath.TrimEnd('\\');
            }
        }

        config.StartingPath = startingPath;

        // Get selected path
        var selectedPath = QueryHelper.GetString("selected_path", String.Empty);

        // If starting path under website try to map selected path
        if (startingPath.StartsWithCSafe("~") && !String.IsNullOrEmpty(selectedPath))
        {
            try
            {
                selectedPath = Server.MapPath(selectedPath).Replace(Server.MapPath(startingPath).TrimEnd('\\'), startingPath);
            }
            catch
            {
            }
        }

        // Fix slashes
        selectedPath = selectedPath.StartsWithCSafe("~") ? Path.EnsureSlashes(selectedPath, true) : Path.EnsureBackslashes(selectedPath, true);

        config.SelectedPath = selectedPath;

        // Get default path
        config.DefaultPath = QueryHelper.GetString("default_path", String.Empty);
        string origDefaultPath = config.DefaultPath;

        if (selectedPath.StartsWithCSafe(startingPath))
        {
            // item to be selected
            string selectedItem = config.SelectedPath.Replace(startingPath, string.Empty);
            char slashChar = '\\';
            if (selectedPath.StartsWithCSafe("~"))
            {
                slashChar = '/';
            }

            selectedItem = selectedItem.TrimStart(slashChar).TrimEnd(slashChar);
            if (selectedItem.LastIndexOfCSafe(slashChar) != -1)
            {
                selectedItem = selectedItem.Substring(0, selectedItem.LastIndexOfCSafe(slashChar));
            }

            config.DefaultPath = selectedItem;
        }


        string defaultPath = String.Format("{0}\\{1}", startingPath, config.DefaultPath);
        if (startingPath.StartsWithCSafe("~"))
        {
            try
            {
                defaultPath = Server.MapPath(defaultPath);
            }
            catch
            {
                // Set default path
                defaultPath = string.Empty;
            }
        }

        if (!Directory.Exists(defaultPath))
        {
            try
            {
                defaultPath = Server.MapPath(String.Format("{0}\\{1}", startingPath, origDefaultPath));
            }
            catch
            {
                // Set default path
                defaultPath = string.Empty;
            }

            config.DefaultPath = Directory.Exists(defaultPath) ? origDefaultPath : string.Empty;
        }

        // Get mode
        config.ShowFolders = QueryHelper.GetBoolean("show_folders", false);
    }


    /// <summary>
    /// Ensures that filter is no more applied.
    /// </summary>
    private void ResetSearchFilter()
    {
        fileSystemView.ResetSearch();
        LastSearchedValue = string.Empty;
    }

    #endregion
}