using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using System.Threading;

using CMS.Localization;
using CMS.Membership;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.Controls;
using CMS.Helpers;
using CMS.Base;
using CMS.ExtendedControls;
using CMS.PortalEngine;
using CMS.PortalControls;
using CMS.DocumentEngine;
using CMS.MacroEngine;

using CultureInfo = System.Globalization.CultureInfo;

public partial class CMSModules_PortalEngine_Controls_Editable_EditableText : CMSUserControl
{
    #region "Variables"

    protected string mHtmlAreaToolbar = String.Empty;
    protected string mHtmlAreaToolbarLocation = String.Empty;

    protected XmlData mImageAutoResize = null;
    protected int mResizeToWidth = 0;
    protected int mResizeToHeight = 0;
    protected int mResizeToMaxSideSize = 0;
    protected bool mDimensionsLoaded = false;

    protected ISimpleDataContainer mIDataControl = null;
    protected IPageManager mIPageManager = null;

    protected CMSHtmlEditor htmlValue = null;
    protected CMSTextBox txtValue = null;

    protected Label lblTitle = null;
    protected Panel pnlEditor = null;
    protected Label lblError = null;

    protected bool mShowToolbar = false;

    protected Literal ltlContent = null;

    protected ViewModeEnum? mViewMode = null;

    protected PageInfo mCurrentPageInfo = null;
    private string mEditPageUrl = "~/CMSModules/PortalEngine/UI/OnSiteEdit/EditText.aspx";
    private MacroResolver mMacroResolver = null;

    #endregion


    #region "Public properties"

    /// <summary>
    /// Gets the current context resolver
    /// </summary>
    public MacroResolver ContextResolver
    {
        get
        {
            if (mMacroResolver == null)
            {
                mMacroResolver = MacroContext.CurrentResolver.CreateChild();
                mMacroResolver.Culture = Thread.CurrentThread.CurrentCulture.ToString();
            }
            return mMacroResolver;
        }
    }


    /// <summary>
    /// Get or sets editor's title
    /// </summary>
    public String Title
    {
        get;
        set;
    }


    /// <summary>
    /// Indicates whether CKEditor is used in inline mode.
    /// </summary>
    public bool UseInlineMode
    {
        get;
        set;
    }


    /// <summary>
    /// Editor instance
    /// </summary>
    public CMSHtmlEditor Editor
    {
        get
        {
            return htmlValue;
        }
    }


    /// <summary>
    /// ID of the control content
    /// </summary>
    public string ContentID
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets the value that indicates whether text is edited in dialog on-site mode
    /// </summary>
    public bool IsDialogEdit
    {
        get;
        set;
    }


    /// <summary>
    /// Gets the url of the page which ensures editing of the web part's editable content in the On-Site editing mode.
    /// </summary>
    public string EditPageUrl
    {
        get
        {
            return URLHelper.ResolveUrl(mEditPageUrl);
        }
    }


    /// <summary>
    /// Page mode of the current web part.
    /// </summary>
    public ViewModeEnum ViewMode
    {
        get
        {
            if (mViewMode == null)
            {
                if (PageManager != null)
                {
                    return PageManager.ViewMode;
                }

                return PortalContext.ViewMode;
            }
            return mViewMode.Value;
        }
        set
        {
            mViewMode = value;
        }
    }


    /// <summary>
    /// If set, only published content is displayed on a live site.
    /// </summary>
    public bool SelectOnlyPublished
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("SelectOnlyPublished"), false);
        }
        set
        {
            SetValue("SelectOnlyPublished", value);
        }
    }


    /// <summary>
    /// Page place holder
    /// </summary>
    public CMSPagePlaceholder PagePlaceholder
    {
        get;
        set;
    }


    /// <summary>
    /// Current page info
    /// </summary>
    public new PageInfo CurrentPageInfo
    {
        get
        {
            if ((mCurrentPageInfo == null) && (PagePlaceholder != null))
            {
                mCurrentPageInfo = PagePlaceholder.PageInfo;
            }

            return mCurrentPageInfo;
        }
        set
        {
            mCurrentPageInfo = value;
        }
    }


    /// <summary>
    /// Gets or sets the design panel 
    /// </summary>
    public Panel DesignPanel
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets the IDataControl to get/set values
    /// </summary>
    public ISimpleDataContainer DataControl
    {
        get
        {
            if (mIDataControl == null)
            {
                mIDataControl = ControlsHelper.GetParentControl(this, typeof(ISimpleDataContainer)) as ISimpleDataContainer;
                if (mIDataControl == null)
                {
                    // ASPX mode - editable text in a dialog (On-site editing)
                    mIDataControl = this as ISimpleDataContainer;
                }
            }
            return mIDataControl;
        }
        set
        {
            mIDataControl = value;
        }
    }


    /// <summary>
    /// Parent page manager.
    /// </summary>
    public IPageManager PageManager
    {
        get
        {
            return mIPageManager;
        }
        set
        {
            mIPageManager = value;
        }
    }


    /// <summary>
    /// Gets or sets the error message when the input string does not comply with the input conditions
    /// </summary>
    public string ErrorMessage
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets the type of server control which is displayed in the editable region.
    /// </summary>
    [Category("Appearance"), Description("Gets or sets the type of server control which is displayed in the editable region.")]
    [DefaultValue(CMSEditableRegionTypeEnum.TextBox)]
    public virtual CMSEditableRegionTypeEnum RegionType
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets the control title which is displayed in the editable mode.
    /// </summary>
    [Category("Appearance"), Description("Gets or sets the control title which is displayed in the editable mode.")]
    public string RegionTitle
    {
        get
        {
            return ValidationHelper.GetString(GetValue("RegionTitle"), String.Empty);
        }
        set
        {
            SetValue("RegionTitle", value);
        }
    }


    /// <summary>
    /// Gets or sets the maximum length of the content.
    /// </summary>
    [Category("Behavior"), Description("Gets or sets the maximum length of the content.")]
    public int MaxLength
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("MaxLength"), 0);
        }
        set
        {
            SetValue("MaxLength", value);
        }
    }


    /// <summary>
    /// Gets or sets the minimum length of the content.
    /// </summary>
    [Category("Behavior"), Description("Gets or sets the minimum length of the content.")]
    public int MinLength
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("MinLength"), 0);
        }
        set
        {
            SetValue("MinLength", value);
        }
    }


    /// <summary>
    /// Gets or sets the height of the control.
    /// </summary>
    [Category("Appearance"), Description("Gets or sets the height of the control.")]
    public int DialogHeight
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("DialogHeight"), 0);
        }
        set
        {
            SetValue("DialogHeight", value);
        }
    }


    /// <summary>
    /// Gets or sets the width of the control.
    /// </summary>
    [Category("Appearance"), Description("Gets or sets the width of the control.")]
    public int DialogWidth
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("DialogWidth"), 0);
        }
        set
        {
            SetValue("DialogWidth", value);
        }
    }


    /// <summary>
    /// Gets or sets the name of the CSS style sheet used by the control (for HTML area RegionType).
    /// </summary>
    [Category("Appearance"), Description("Gets or sets the name of the CSS style sheet used by the control (for HTML area RegionType).")]
    public string HTMLEditorCssStylesheet
    {
        get
        {
            return ValidationHelper.GetString(GetValue("HTMLEditorCssStylesheet"), String.Empty);
        }
        set
        {
            SetValue("HTMLEditorCssStylesheet", value);
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether to wrap the text if using text area field.
    /// </summary>
    [Category("Appearance"), Description("Gets or sets the value that indicates whether to wrap the text if using text area field.")]
    [DefaultValue(true)]
    public bool WordWrap
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("WordWrap"), true);
        }
        set
        {
            SetValue("WordWrap", value);
        }
    }


    /// <summary>
    /// Gets or sets the name of the HTML editor toolbar.
    /// </summary>
    public string HtmlAreaToolbar
    {
        get
        {
            return mHtmlAreaToolbar;
        }
        set
        {
            if (value == null)
            {
                mHtmlAreaToolbar = "";
            }
            else
            {
                mHtmlAreaToolbar = value;
            }
        }
    }


    /// <summary>
    /// Gets or sets the location of the HTML editor toolbar.
    /// </summary>
    public string HtmlAreaToolbarLocation
    {
        get
        {
            return mHtmlAreaToolbarLocation;
        }
        set
        {
            if (value == null)
            {
                mHtmlAreaToolbarLocation = "";
            }
            else
            {
                mHtmlAreaToolbarLocation = value;
            }
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether the permissions are checked.
    /// </summary>
    public bool CheckPermissions
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("CheckPermissions"), false);
        }
        set
        {
            SetValue("CheckPermissions", value);
        }
    }


    /// <summary>
    /// Width the image should be automatically resized to after it is uploaded.
    /// </summary>
    public int ResizeToWidth
    {
        get
        {
            if (!mDimensionsLoaded)
            {
                // Use image auto resize settings
                Hashtable settings = ImageAutoResize.ConvertToHashtable();
                ImageHelper.GetAutoResizeDimensions(settings, SiteContext.CurrentSiteName, out mResizeToWidth, out mResizeToHeight, out mResizeToMaxSideSize);
                mDimensionsLoaded = true;
            }
            return mResizeToWidth;
        }
        set
        {
            mResizeToWidth = value;
            mDimensionsLoaded = true;
        }
    }


    /// <summary>
    /// Height the image should be automatically resized to after it is uploaded.
    /// </summary>
    public int ResizeToHeight
    {
        get
        {
            if (!mDimensionsLoaded)
            {
                // Use image auto resize settings
                Hashtable settings = ImageAutoResize.ConvertToHashtable();
                ImageHelper.GetAutoResizeDimensions(settings, SiteContext.CurrentSiteName, out mResizeToWidth, out mResizeToHeight, out mResizeToMaxSideSize);
                mDimensionsLoaded = true;
            }
            return mResizeToHeight;
        }
        set
        {
            mResizeToHeight = value;
            mDimensionsLoaded = true;
        }
    }


    /// <summary>
    /// Max side size the image should be automatically resized to after it is uploaded.
    /// </summary>
    public int ResizeToMaxSideSize
    {
        get
        {
            if (!mDimensionsLoaded)
            {
                // Use image auto resize settings
                Hashtable settings = ImageAutoResize.ConvertToHashtable();
                ImageHelper.GetAutoResizeDimensions(settings, SiteContext.CurrentSiteName, out mResizeToWidth, out mResizeToHeight, out mResizeToMaxSideSize);
                mDimensionsLoaded = true;
            }
            return mResizeToMaxSideSize;
        }
        set
        {
            mResizeToMaxSideSize = value;
            mDimensionsLoaded = true;
        }
    }


    /// <summary>
    /// Enables or disables resolving of inline controls.
    /// </summary>
    public bool ResolveDynamicControls
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ResolveDynamicControls"), true);
        }
        set
        {
            SetValue("ResolveDynamicControls", value);
        }
    }


    /// <summary>
    /// Default text displayed if no content filled.
    /// </summary>
    public string DefaultText
    {
        get
        {
            return ValidationHelper.GetString(GetValue("DefaultText"), "");
        }
        set
        {
            SetValue("DefaultText", value);
        }
    }

    #endregion


    #region "Private properties"

    /// <summary>
    /// Auto resize configuration.
    /// </summary>
    private XmlData ImageAutoResize
    {
        get
        {
            if (mImageAutoResize == null)
            {
                mImageAutoResize = new XmlData("AutoResize");
                mImageAutoResize.LoadData(ValidationHelper.GetString(GetValue("ImageAutoResize"), ""));
            }
            return mImageAutoResize;
        }
    }

    #endregion


    #region "Page Methods"

    /// <summary>
    /// PreRender event handler.
    /// </summary>
    private void Page_PreRender(object sender, EventArgs e)
    {
        if (!StopProcessing)
        {
            ViewModeEnum viewMode = (PortalContext.ViewMode.IsPreview()) ? ViewModeEnum.Preview : ViewMode;

            switch (viewMode)
            {
                case ViewModeEnum.Edit:
                case ViewModeEnum.EditDisabled:
                    // Set enabled
                    if (htmlValue != null)
                    {
                        htmlValue.Enabled = IsEnabled(viewMode);
                    }
                    if (txtValue != null)
                    {
                        txtValue.Enabled = IsEnabled(viewMode);
                    }

                    if (mShowToolbar && IsEnabled(viewMode))
                    {
                        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), ScriptHelper.TOOLBAR_SCRIPT_KEY, ScriptHelper.ToolbarScript);
                    }

                    if (lblError != null)
                    {
                        lblError.Visible = (lblError.Text != "");
                    }

                    if (lblTitle != null)
                    {
                        lblTitle.Text = RegionTitle;
                        lblTitle.Visible = (lblTitle.Text != "");
                    }

                    // Allow to select text in the source editor area
                    if (DesignPanel != null)
                    {
                        ScriptHelper.RegisterStartupScript(this, typeof(string), "onselectstart", "document.getElementById('" + DesignPanel.ClientID + "').parentNode.onselectstart = function() { return true; };", true);
                    }

                    break;
            }
        }
    }

    #endregion


    #region "Methods"


    /// <summary>
    /// Loads the control content.
    /// </summary>
    /// <param name="content">Content to load</param>
    /// <param name="forceReload">If true, the content is forced to reload</param>
    public void LoadContent(string content, bool forceReload)
    {
        if (!StopProcessing)
        {
            ApplySettings();

            content = ValidationHelper.GetString(content, "");

            // If content empty set default text
            if (String.IsNullOrEmpty(content))
            {
                content = DefaultText;
            }

            // Resolve URLs
            content = HTMLHelper.ResolveUrls(content, null);

            switch (ViewMode)
            {
                case ViewModeEnum.Edit:
                case ViewModeEnum.EditDisabled:
                    switch (RegionType)
                    {
                        case CMSEditableRegionTypeEnum.HtmlEditor:
                            // HTML editor
                            if ((htmlValue != null) && (forceReload || !RequestHelper.IsPostBack() || (ViewMode == ViewModeEnum.EditDisabled)))
                            {
                                htmlValue.ResolvedValue = content;
                            }
                            break;

                        case CMSEditableRegionTypeEnum.TextArea:
                        case CMSEditableRegionTypeEnum.TextBox:
                            // TextBox
                            if ((forceReload || !RequestHelper.IsPostBack()) && (txtValue != null))
                            {
                                txtValue.Text = content;
                            }
                            break;
                    }
                    break;


                default:
                    // Check authorization
                    bool isAuthorized = true;
                    if ((PageManager != null) && (CheckPermissions))
                    {
                        isAuthorized = PageManager.IsAuthorized;
                    }

                    // Only published
                    if ((PortalContext.ViewMode != ViewModeEnum.LiveSite) || !SelectOnlyPublished || ((CurrentPageInfo != null) && CurrentPageInfo.IsPublished))
                    {
                        if (isAuthorized)
                        {
                            if (ltlContent == null)
                            {
                                ltlContent = (Literal)FindControl("ltlContent");
                            }
                            if (ltlContent != null)
                            {
                                ltlContent.Text = ContextResolver.ResolveMacros(content);

                                // Resolve inline controls
                                if (ResolveDynamicControls)
                                {
                                    ControlsHelper.ResolveDynamicControls(this);
                                }
                            }
                        }
                    }
                    break;
            }
        }
    }


    /// <summary>
    /// Returns true if entered data is valid. If data is invalid, it returns false and displays an error message.
    /// </summary>
    public bool IsValid()
    {
        string textWithOut = "";
        bool mIsValid = true;
        string mError = "";

        switch (ViewMode)
        {
            case ViewModeEnum.Edit:
            case ViewModeEnum.EditDisabled:
                switch (RegionType)
                {
                    case CMSEditableRegionTypeEnum.HtmlEditor:
                        // HTML editor
                        if (htmlValue != null)
                        {
                            textWithOut = HTMLHelper.StripTags(htmlValue.ResolvedValue);
                            if (textWithOut != null)
                            {
                                if ((textWithOut.Length > MaxLength) && (MaxLength > 0))
                                {
                                    mError = String.Format(GetString("EditableText.ErrorMax"), textWithOut.Length, MaxLength);
                                    mIsValid = false;
                                }
                                if ((textWithOut.Length < MinLength) && (MinLength > 0))
                                {
                                    mError = String.Format(GetString("EditableText.ErrorMin"), textWithOut.Length, MinLength);
                                    mIsValid = false;
                                }
                            }
                        }
                        break;

                    case CMSEditableRegionTypeEnum.TextArea:
                    case CMSEditableRegionTypeEnum.TextBox:
                        // TextBox
                        if (txtValue != null)
                        {
                            textWithOut = HTMLHelper.StripTags(txtValue.Text);
                            if (textWithOut != null)
                            {
                                if ((textWithOut.Length > MaxLength) && (MaxLength > 0))
                                {
                                    mError = String.Format(GetString("EditableText.ErrorMax"), textWithOut.Length, MaxLength);
                                    mIsValid = false;
                                }
                                if ((textWithOut.Length < MinLength) && (MinLength > 0))
                                {
                                    mError = String.Format(GetString("EditableText.ErrorMin"), textWithOut.Length, MinLength);
                                    mIsValid = false;
                                }
                            }
                        }
                        break;
                }
                break;
        }

        if (!mIsValid)
        {
            lblError.Text = mError;
            ErrorMessage = mError;
        }

        return mIsValid;
    }


    /// <summary>
    /// Gets the current control content.
    /// </summary>
    public string GetContent()
    {
        if (!StopProcessing)
        {
            EnsureChildControls();

            switch (ViewMode)
            {
                case ViewModeEnum.Edit:
                case ViewModeEnum.EditDisabled:
                    switch (RegionType)
                    {
                        case CMSEditableRegionTypeEnum.HtmlEditor:
                            // HTML editor
                            if (htmlValue != null)
                            {
                                return htmlValue.ResolvedValue;
                            }
                            break;

                        case CMSEditableRegionTypeEnum.TextArea:
                        case CMSEditableRegionTypeEnum.TextBox:
                            // TextBox
                            if (txtValue != null)
                            {
                                return txtValue.Text;
                            }
                            break;
                    }
                    break;
            }
        }

        return null;
    }


    /// <summary>
    /// Returns the array list of the field IDs (Client IDs of the inner controls) that should be spell checked.
    /// </summary>
    public List<string> GetSpellCheckFields()
    {
        switch (ViewMode)
        {
            case ViewModeEnum.Edit:
                List<string> result = new List<string>();
                switch (RegionType)
                {
                    case CMSEditableRegionTypeEnum.HtmlEditor:
                        // HTML editor
                        if (htmlValue != null)
                        {
                            result.Add(htmlValue.ClientID);
                        }
                        break;

                    case CMSEditableRegionTypeEnum.TextArea:
                    case CMSEditableRegionTypeEnum.TextBox:
                        // TextBox
                        if (txtValue != null)
                        {
                            result.Add(txtValue.ClientID);
                        }
                        break;
                }
                return result;
        }
        return null;
    }


    protected void ApplySettings()
    {
        EnsureChildControls();

        if (!StopProcessing)
        {
            // Create controls by actual page mode
            switch (ViewMode)
            {
                case ViewModeEnum.Edit:
                case ViewModeEnum.EditDisabled:
                    {
                        // Edit mode
                        if (DialogWidth > 0)
                        {
                            pnlEditor.Style.Add(HtmlTextWriterStyle.Width, DialogWidth.ToString() + "px;");
                        }

                        // Display the region control based on the region type
                        switch (RegionType)
                        {
                            case CMSEditableRegionTypeEnum.HtmlEditor:
                                // HTML Editor
                                if (IsDialogEdit)
                                {
                                    htmlValue.Width = new Unit(100, UnitType.Percentage);
                                    htmlValue.Height = new Unit(100, UnitType.Percentage);
                                    htmlValue.ToolbarLocation = "out:CKToolbar";
                                    htmlValue.Title = Title;

                                    // Maximize editor to fill entire dialog
                                    htmlValue.RemoveButtons.Add("Maximize");

                                    if (!DeviceContext.CurrentDevice.IsMobile)
                                    {
                                        // Desktop browsers
                                        htmlValue.Config["on"] = "{ 'instanceReady' : function(e) { e.editor.execCommand( 'maximize' ); } }";
                                    }
                                }
                                else
                                {
                                    if (DialogWidth > 0)
                                    {
                                        htmlValue.Width = new Unit(DialogWidth);
                                    }
                                    if (DialogHeight > 0)
                                    {
                                        htmlValue.Height = new Unit(DialogHeight);
                                    }
                                }

                                // Set toolbar location
                                if (HtmlAreaToolbarLocation != "")
                                {
                                    // Show the toolbar
                                    if (HtmlAreaToolbarLocation.ToLowerCSafe() == "out:cktoolbar")
                                    {
                                        mShowToolbar = true;
                                    }

                                    htmlValue.ToolbarLocation = HtmlAreaToolbarLocation;
                                }

                                // Set the visual appearance
                                if (HtmlAreaToolbar != "")
                                {
                                    htmlValue.ToolbarSet = HtmlAreaToolbar;
                                }

                                // Get editor area css file
                                if (HTMLEditorCssStylesheet != "")
                                {
                                    htmlValue.EditorAreaCSS = CSSHelper.GetStylesheetUrl(HTMLEditorCssStylesheet);
                                }
                                else if (SiteContext.CurrentSite != null)
                                {
                                    htmlValue.EditorAreaCSS = CssStylesheetInfoProvider.GetHtmlEditorAreaCss(SiteContext.CurrentSiteName);
                                }

                                // Set "Insert image or media" dialog configuration                            
                                htmlValue.MediaDialogConfig.ResizeToHeight = ResizeToHeight;
                                htmlValue.MediaDialogConfig.ResizeToWidth = ResizeToWidth;
                                htmlValue.MediaDialogConfig.ResizeToMaxSideSize = ResizeToMaxSideSize;

                                // Set "Insert link" dialog configuration  
                                htmlValue.LinkDialogConfig.ResizeToHeight = ResizeToHeight;
                                htmlValue.LinkDialogConfig.ResizeToWidth = ResizeToWidth;
                                htmlValue.LinkDialogConfig.ResizeToMaxSideSize = ResizeToMaxSideSize;

                                // Set "Quickly insert image" configuration
                                htmlValue.QuickInsertConfig.ResizeToHeight = ResizeToHeight;
                                htmlValue.QuickInsertConfig.ResizeToWidth = ResizeToWidth;
                                htmlValue.QuickInsertConfig.ResizeToMaxSideSize = ResizeToMaxSideSize;

                                break;

                            case CMSEditableRegionTypeEnum.TextArea:
                            case CMSEditableRegionTypeEnum.TextBox:
                                // TextBox
                                if (RegionType == CMSEditableRegionTypeEnum.TextArea)
                                {
                                    txtValue.TextMode = TextBoxMode.MultiLine;
                                }
                                else
                                {
                                    txtValue.TextMode = TextBoxMode.SingleLine;
                                }

                                if (DialogWidth > 0)
                                {
                                    txtValue.Width = new Unit(DialogWidth - 8);
                                }
                                else
                                {
                                    // Default width is 100%
                                    txtValue.Width = new Unit(100, UnitType.Percentage);
                                }

                                if (DialogHeight > 0)
                                {
                                    txtValue.Height = new Unit(DialogHeight);
                                }

                                txtValue.Wrap = WordWrap;

                                break;
                        }
                    }
                    break;
            }
        }
    }


    /// <summary>
    /// Initializes the control properties.
    /// </summary>
    public void SetupControl()
    {
        // Do not hide for roles in edit or preview mode
        switch (ViewMode)
        {
            case ViewModeEnum.Edit:
            case ViewModeEnum.EditLive:
            case ViewModeEnum.EditDisabled:
            case ViewModeEnum.Design:
            case ViewModeEnum.DesignDisabled:
            case ViewModeEnum.EditNotCurrent:
            case ViewModeEnum.Preview:
                SetValue("DisplayToRoles", String.Empty);
                break;
        }

        if (!StopProcessing)
        {
            // Load the properties
            RegionTitle = DataHelper.GetNotEmpty(GetValue("RegionTitle"), RegionTitle);
            RegionType = CMSEditableRegionTypeEnumFunctions.GetRegionTypeEnum(DataHelper.GetNotEmpty(GetValue("RegionType"), CMSEditableRegionTypeEnumFunctions.GetRegionTypeString(RegionType)));
            DialogWidth = DataHelper.GetNotZero(GetValue("DialogWidth"), DialogWidth);
            DialogHeight = DataHelper.GetNotZero(GetValue("DialogHeight"), DialogHeight);
            HTMLEditorCssStylesheet = ValidationHelper.GetString(GetValue("HtmlEditorCssStylesheet"), HTMLEditorCssStylesheet);
            WordWrap = ValidationHelper.GetBoolean(GetValue("WordWrap"), WordWrap);
            MinLength = DataHelper.GetNotZero(GetValue("MinLength"), MinLength);
            MaxLength = DataHelper.GetNotZero(GetValue("MaxLength"), MaxLength);
            HtmlAreaToolbar = DataHelper.GetNotEmpty(GetValue("HtmlAreaToolbar"), HtmlAreaToolbar);
            HtmlAreaToolbarLocation = DataHelper.GetNotEmpty(GetValue("HtmlAreaToolbarLocation"), HtmlAreaToolbarLocation);
        }
    }


    /// <summary>
    /// Overridden CreateChildControls method.
    /// </summary>
    protected override void CreateChildControls()
    {
        SetupControl();

        Controls.Clear();
        base.CreateChildControls();

        if (!StopProcessing)
        {
            if (!CMSAbstractEditableControl.RequestEditViewMode(ViewMode, ContentID))
            {
                ViewMode = ViewModeEnum.Preview;
            }

            // Create controls by actual page mode
            switch (ViewMode)
            {
                case ViewModeEnum.Edit:
                case ViewModeEnum.EditDisabled:

                    // Main editor panel
                    pnlEditor = new Panel();
                    pnlEditor.ID = "pnlEditor";
                    pnlEditor.CssClass = "EditableTextEdit EditableText_" + ContentID;
                    pnlEditor.Attributes.Add("data-tracksavechanges", "true");
                    Controls.Add(pnlEditor);

                    // Title label
                    lblTitle = new Label();
                    lblTitle.EnableViewState = false;
                    lblTitle.CssClass = "EditableTextTitle";
                    pnlEditor.Controls.Add(lblTitle);

                    // Error label
                    lblError = new Label();
                    lblError.EnableViewState = false;
                    lblError.CssClass = "EditableTextError";
                    pnlEditor.Controls.Add(lblError);

                    // Display the region control based on the region type
                    switch (RegionType)
                    {
                        case CMSEditableRegionTypeEnum.HtmlEditor:
                            // HTML Editor
                            htmlValue = new CMSHtmlEditor();
                            htmlValue.IsLiveSite = false;
                            htmlValue.ID = "htmlValue";
                            htmlValue.AutoDetectLanguage = false;
                            htmlValue.DefaultLanguage = Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName;
                            htmlValue.Title = Title;

                            htmlValue.UseInlineMode = UseInlineMode;

                            // Set direction
                            htmlValue.Config["ContentsLangDirection"] = "ltr";

                            if (CultureHelper.IsPreferredCultureRTL())
                            {
                                htmlValue.Config["ContentsLangDirection"] = "rtl";
                            }

                            // Set the language
                            try
                            {
                                CultureInfo ci = CultureHelper.GetCultureInfo(DataHelper.GetNotEmpty(MembershipContext.AuthenticatedUser.PreferredUICultureCode, LocalizationContext.PreferredCultureCode));
                                htmlValue.DefaultLanguage = ci.TwoLetterISOLanguageName;
                            }
                            catch
                            {
                            }

                            htmlValue.AutoDetectLanguage = false;
                            htmlValue.Enabled = IsEnabled(ViewMode);

                            if (ViewMode == ViewModeEnum.EditDisabled)
                            {
                                pnlEditor.Controls.Add(new LiteralControl("<div style=\"width: 98%\">"));
                                pnlEditor.Controls.Add((Control)htmlValue);
                                pnlEditor.Controls.Add(new LiteralControl("</div>"));
                            }
                            else
                            {
                                pnlEditor.Controls.Add((Control)htmlValue);
                            }
                            break;

                        case CMSEditableRegionTypeEnum.TextArea:
                        case CMSEditableRegionTypeEnum.TextBox:
                            // TextBox
                            txtValue = new CMSTextBox();
                            txtValue.ID = "txtValue";
                            txtValue.CssClass = "EditableTextTextBox";

                            txtValue.Enabled = IsEnabled(ViewMode);
                            pnlEditor.Controls.Add(txtValue);
                            break;
                    }
                    break;

                default:
                    // Display content in non editing modes
                    ltlContent = new Literal();
                    ltlContent.ID = "ltlContent";
                    ltlContent.EnableViewState = false;
                    Controls.Add(ltlContent);
                    break;
            }
        }
    }


    /// <summary>
    /// Returns the value of the given web part property property.
    /// </summary>
    /// <param name="propertyName">Property name</param>
    public override object GetValue(string propertyName)
    {
        if ((DataControl != null) && (DataControl != this))
        {
            return DataControl.GetValue(propertyName);
        }

        return base.GetValue(propertyName);
    }


    /// <summary>
    /// Sets the property value of the control, setting the value affects only local property value.
    /// </summary>
    /// <param name="propertyName">Property name to set</param>
    /// <param name="value">New property value</param>
    public override bool SetValue(string propertyName, object value)
    {
        if ((DataControl != null) && (DataControl != this))
        {
            DataControl.SetValue(propertyName, value);
        }

        return base.SetValue(propertyName, value);
    }

    #endregion


    #region "Private methods"

    /// <summary>
    /// Indicates whether this control should be enabled and editing allowed.
    /// </summary>
    /// <param name="viewMode">The view mode.</param>
    private bool IsEnabled(ViewModeEnum viewMode)
    {
        return (viewMode.IsEdit()) && DocumentManager.AllowSave;
    }

    #endregion
}

