using System;
using System.Web.UI;
using System.Data;

using CMS.ExtendedControls;
using CMS.Helpers;
using CMS.IO;
using CMS.UIControls;
using CMS.Base;

public partial class CMSModules_Content_Controls_Dialogs_Selectors_FileSystemSelector_FileSystemView : CMSUserControl
{
    private const char ARG_SEPARATOR = '|';


    #region "Private variables"

    private FileSystemDialogConfiguration mConfig;
    private string mStartingPath = "";
    private string mSearchText = "";

    protected string mSaveText;

    #endregion


    #region "Public properties"

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
    /// Gets current dialog configuration.
    /// </summary>
    public FileSystemDialogConfiguration Config
    {
        get
        {
            if (mConfig == null)
            {
                mConfig = new FileSystemDialogConfiguration();
            }
            return mConfig;
        }
        set
        {
            mConfig = value;
        }
    }


    /// <summary>
    /// Indicates whether the content tree is displaying more than max tree nodes.
    /// </summary>
    public bool IsDisplayMore
    {
        get
        {
            return innermedia.IsDisplayMore;
        }
        set
        {
            innermedia.IsDisplayMore = value;
        }
    }


    /// <summary>
    /// Gets or sets starting path of control.
    /// </summary>
    public string StartingPath
    {
        get
        {
            return mStartingPath;
        }
        set
        {
            mStartingPath = value.StartsWithCSafe("~/") ? Server.MapPath(value) : value;
        }
    }


    /// <summary>
    /// Search text to filter data.
    /// </summary>
    public string SearchText
    {
        get
        {
            mSearchText = ValidationHelper.GetString(ViewState["SearchText"], "");
            return mSearchText;
        }
        set
        {
            mSearchText = value;
            ViewState["SearchText"] = mSearchText;
        }
    }


    /// <summary>
    /// Data source
    /// </summary>
    public DataSet DataSource
    {
        get
        {
            return innermedia.DataSource;
        }
        set
        {
            innermedia.DataSource = value;
        }
    }


    /// <summary>
    /// Gets or sets a view mode used to display files.
    /// </summary>
    public DialogViewModeEnum ViewMode
    {
        get
        {
            return innermedia.ViewMode;
        }
        set
        {
            innermedia.ViewMode = value;
        }
    }


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

    #endregion


    #region "Control events"

    protected void Page_Load(object sender, EventArgs e)
    {
        // If processing the request should not continue
        if (StopProcessing)
        {
            Visible = false;
        }
        else
        {
            // Initialize controls
            SetupControls();
        }
    }


    /// <summary>
    /// PreRender event handler
    /// </summary>
    /// <param name="e">Event arguments</param>
    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        if (String.IsNullOrEmpty(lblListingInfo.Text))
        {
            RefreshPathInfo();
        }
    }


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
        innermedia.FileSystemPath = StartingPath;

        // Set grid definition
        innermedia.ListViewControl.GridName = Config.ShowFolders ? "~/CMSModules/Content/Controls/Dialogs/Selectors/FileSystemSelector/FolderView.xml" : "~/CMSModules/Content/Controls/Dialogs/Selectors/FileSystemSelector/FileSystemView.xml";

        // Set inner control binding columns
        innermedia.FileIdColumn = "path";
        innermedia.FileNameColumn = "name";
        innermedia.FileExtensionColumn = "type";
        innermedia.FileSizeColumn = "size";
        innermedia.SearchText = SearchText;

        // Register for inner media events
        innermedia.GetArgumentSet += innermedia_GetArgumentSet;

        innermedia.Configuration = Config;
        innermedia.ViewMode = ViewMode;
    }


    /// <summary>
    /// Initializes scrips used by the control.
    /// </summary>
    private void InitializeControlScripts()
    {
        const string script = @"
        function SetTreeRefreshAction(path) {
            SetAction('refreshtree', path);
            RaiseHiddenPostBack();
        }
        function SetRefreshAction() {
            SetAction('refresh', '');
            RaiseHiddenPostBack();
        }
        function SetDeleteAction(argument) {
            SetAction('delete', argument);
            RaiseHiddenPostBack();
        }
        function SetSelectAction(argument) {
            SetAction('select', argument);
            RaiseHiddenPostBack();
        }
        function SetParentAction(argument) {
            SetAction('parentselect', argument);
            RaiseHiddenPostBack();
        }";

        ScriptManager.RegisterStartupScript(this, GetType(), "DialogsSelectAction", script, true);
    }


    /// <summary>
    /// Loads data from data source property.
    /// </summary>
    private void ReloadData()
    {
        innermedia.Reload(true);

        RefreshPathInfo();
    }


    /// <summary>
    /// Refreshes the path information
    /// </summary>
    private void RefreshPathInfo()
    {
        // Get relative path
        string path = StartingPath;
        if (!String.IsNullOrEmpty(path))
        {
            string appPath = SystemContext.WebApplicationPhysicalPath;

            if (path.StartsWithCSafe(appPath, true))
            {
                path = "~" + Path.EnsureSlashes(path.Substring(appPath.Length));
            }

            // Display information about current path
            string info = String.Format(GetString("FileSystemSelector.Info"), path);
            DisplayListingInfo(info);
        }
    }

    #endregion


    #region "Inner media view event handlers"

    /// <summary>
    /// Returns argument set according passed DataRow and flag indicating whether the set is obtained for selected item.
    /// </summary>
    /// <param name="dr">DataRow with all the item data</param>
    /// <param name="isSelected">Indicates whether the set is required for an selected item</param>
    private string innermedia_GetArgumentSet(DataRow dr)
    {
        // Return required argument set
        return GetArgumentSet(dr);
    }

    #endregion


    #region "Helper methods"

    /// <summary>
    /// Returns argument set for the passed file data row.
    /// </summary>
    /// <param name="dr">Data row object holding all the data on current file</param>
    private string GetArgumentSet(DataRow dr)
    {
        // Common information for both content & attachments
        string result = String.Format("{1}{0}{2}{0}{3}", ARG_SEPARATOR, dr[innermedia.FileIdColumn].ToString(), DataHelper.GetSizeString(ValidationHelper.GetLong(dr[innermedia.FileSizeColumn], 0)), dr["isfile"]);

        return result;
    }


    /// <summary>
    /// Ensures no item is selected.
    /// </summary>
    public void ResetSearch()
    {
        dialogSearch.ResetSearch();
        SearchText = "";
    }

    #endregion
}