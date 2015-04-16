using System;

using CMS.Helpers;
using CMS.PortalControls;
using CMS.skmMenuControl;

using Menu = CMS.skmMenuControl.Menu;

public partial class CMSWebParts_Navigation_cmsmenu : CMSAbstractWebPart
{
    #region "Document properties"

    /// <summary>
    /// Gets or sets the value that indicates whether menu sub items in the RTL culture are opened to the other side.
    /// </summary>
    public bool EnableRTLBehaviour
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("EnableRTLBehaviour"), menuElem.EnableRTLBehaviour);
        }
        set
        {
            SetValue("EnableRTLBehaviour", value);
            menuElem.EnableRTLBehaviour = ValidationHelper.GetBoolean(value, false);
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether up and down mouse css classes are applied.
    /// </summary>
    public bool EnableMouseUpDownClass
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("EnableMouseUpDownClass"), menuElem.EnableMouseUpDownClass);
        }
        set
        {
            SetValue("EnableMouseUpDownClass", value);
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether document menu item properties are applied.
    /// </summary>
    public bool ApplyMenuDesign
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ApplyMenuDesign"), menuElem.ApplyMenuDesign);
        }
        set
        {
            SetValue("ApplyMenuDesign", value);
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether item image is displayed for highlighted item when highlighted image is not specified.
    /// </summary>
    public bool UseItemImagesForHiglightedItem
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("UseItemImagesForHiglightedItem"), menuElem.UseItemImagesForHighlightedItem);
        }
        set
        {
            SetValue("UseItemImagesForHiglightedItem", value);
        }
    }


    /// <summary>
    /// Gets or sets the cache minutes.
    /// </summary>
    public override int CacheMinutes
    {
        get
        {
            return base.CacheMinutes;
        }
        set
        {
            base.CacheMinutes = value;
            menuElem.CacheMinutes = value;
        }
    }


    /// <summary>
    /// Gets or sets the cache item dependencies.
    /// </summary>
    public override string CacheDependencies
    {
        get
        {
            return base.CacheDependencies;
        }
        set
        {
            base.CacheDependencies = value;
            menuElem.CacheDependencies = value;
        }
    }


    /// <summary>
    /// Gets or sets the name of the cache item. If not explicitly specified, the name is automatically 
    /// created based on the control unique ID
    /// </summary>
    public override string CacheItemName
    {
        get
        {
            return base.CacheItemName;
        }
        set
        {
            base.CacheItemName = value;
            menuElem.CacheItemName = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether permissions are checked.
    /// </summary>
    public bool CheckPermissions
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("CheckPermissions"), menuElem.CheckPermissions);
        }
        set
        {
            SetValue("CheckPermissions", value);
            menuElem.CheckPermissions = value;
        }
    }


    /// <summary>
    /// Gets or sets the class names.
    /// </summary>
    public string ClassNames
    {
        get
        {
            return DataHelper.GetNotEmpty(ValidationHelper.GetString(GetValue("Classnames"), menuElem.ClassNames), menuElem.ClassNames);
        }
        set
        {
            SetValue("ClassNames", value);
            menuElem.ClassNames = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether selected documents are combined with default culture.
    /// </summary>
    public bool CombineWithDefaultCulture
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("CombineWithDefaultCulture"), menuElem.CombineWithDefaultCulture);
        }
        set
        {
            SetValue("CombineWithDefaultCulture", value);
            menuElem.CombineWithDefaultCulture = value;
        }
    }


    /// <summary>
    /// Gets or sets the culture code.
    /// </summary>
    public string CultureCode
    {
        get
        {
            return DataHelper.GetNotEmpty(ValidationHelper.GetString(GetValue("CultureCode"), menuElem.CultureCode), menuElem.CultureCode);
        }
        set
        {
            SetValue("CultureCode", value);
            menuElem.CultureCode = value;
        }
    }


    /// <summary>
    /// Gets or sets the maximal relative level.
    /// </summary>
    public int MaxRelativeLevel
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("MaxRelativeLevel"), menuElem.MaxRelativeLevel);
        }
        set
        {
            SetValue("MaxRelativeLevel", value);
            menuElem.MaxRelativeLevel = value;
        }
    }


    /// <summary>
    /// Gets or sets the order by clause.
    /// </summary>
    public string OrderBy
    {
        get
        {
            return DataHelper.GetNotEmpty(ValidationHelper.GetString(GetValue("OrderBy"), menuElem.OrderBy), menuElem.OrderBy);
        }
        set
        {
            SetValue("OrderBy", value);
            menuElem.OrderBy = value;
        }
    }


    /// <summary>
    /// Gets or sets the nodes path.
    /// </summary>
    public string Path
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("Path"), menuElem.Path);
        }
        set
        {
            SetValue("Path", value);
            menuElem.Path = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether selected documents must be published.
    /// </summary>
    public bool SelectOnlyPublished
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("SelectOnlyPublished"), menuElem.SelectOnlyPublished);
        }
        set
        {
            SetValue("SelectOnlyPublished", value);
            menuElem.SelectOnlyPublished = value;
        }
    }


    /// <summary>
    /// Gets or sets the site name.
    /// </summary>
    public string SiteName
    {
        get
        {
            return DataHelper.GetNotEmpty(ValidationHelper.GetString(GetValue("SiteName"), menuElem.SiteName), menuElem.SiteName);
        }
        set
        {
            SetValue("SiteName", value);
            menuElem.SiteName = value;
        }
    }


    /// <summary>
    /// Gets or sets the where condition.
    /// </summary>
    public string WhereCondition
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("WhereCondition"), menuElem.WhereCondition);
        }
        set
        {
            SetValue("WhereCondition", value);
            menuElem.WhereCondition = value;
        }
    }

    #endregion


    #region "Public properties"

    /// <summary>
    /// Gets or sets the value that indicates whether text can be wrapped or space is replaced with non breakable space.
    /// </summary>
    public bool WordWrap
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("WordWrap"), menuElem.WordWrap);
        }
        set
        {
            SetValue("WordWrap", value);
            menuElem.WordWrap = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether alternate text for image will be rendered.
    /// </summary>
    public bool RenderImageAlt
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("RenderImageAlt"), menuElem.RenderImageAlt);
        }
        set
        {
            SetValue("RenderImageAlt", value);
            menuElem.RenderImageAlt = value;
        }
    }


    /// <summary>
    /// Gets or sets the mouse cursor (pointer, hand etc.).
    /// </summary>
    public MouseCursor Cursor
    {
        get
        {
            return (MouseCursor)ValidationHelper.GetInteger(GetValue("Cursor"), (int)menuElem.Cursor);
        }
        set
        {
            SetValue("Cursor", value.ToString());
            menuElem.Cursor = value;
        }
    }


    /// <summary>
    /// Gets or sets the css prefix. For particular levels can be used several values separated with semicolon (;).
    /// </summary>
    public string CSSPrefix
    {
        get
        {
            return DataHelper.GetNotEmpty(ValidationHelper.GetString(GetValue("CSSPrefix"), menuElem.CSSPrefix), menuElem.CSSPrefix);
        }
        set
        {
            SetValue("CSSPrefix", value);
            menuElem.CSSPrefix = value;
        }
    }


    /// <summary>
    /// Gets or sets the external script path.
    /// </summary>
    public string ExternalScriptPath
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("ExternalScriptPath"), menuElem.ExternalScriptPath);
        }
        set
        {
            SetValue("ExternalScriptPath", value);
            menuElem.ExternalScriptPath = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicate whether all items in path will be highlighted.
    /// </summary>
    public bool HighlightAllItemsInPath
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("HighlightAllItemsInPath"), menuElem.HighlightAllItemsInPath);
        }
        set
        {
            SetValue("HighlightAllItemsInPath", value);
            menuElem.HighlightAllItemsInPath = value;
        }
    }


    /// <summary>
    /// Gets or sets the nodes path which indicates path, where items in this path are highlighted (at default current alias path).
    /// </summary>
    public string HighlightedNodePath
    {
        get
        {
            return DataHelper.GetNotEmpty(ValidationHelper.GetString(GetValue("HighlightedNodePath"), menuElem.HighlightedNodePath), menuElem.HighlightedNodePath);
        }
        set
        {
            SetValue("HighlightedNodePath", value);
            menuElem.HighlightedNodePath = value;
        }
    }


    /// <summary>
    /// Gets or sets the menu layout (horizontal, vertical).
    /// </summary>
    public MenuLayout Layout
    {
        get
        {
            return Menu.GetLayout(DataHelper.GetNotEmpty(GetValue("Layout"), menuElem.Layout.ToString()));
        }
        set
        {
            SetValue("Layout", value);
            menuElem.Layout = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether itemname attribute is added to the item.
    /// <remarks>If you switch this property to true, the resulting HTML code will not be valid.</remarks>
    /// </summary>
    public bool RenderItemName
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("RenderItemName"), menuElem.RenderItemName);
        }
        set
        {
            SetValue("RenderItemName", value);
            menuElem.RenderItemName = value;
        }
    }


    /// <summary>
    /// Gets or sets the url to the image which is assigned as sub menu indicator.
    /// </summary>
    public string SubMenuIndicator
    {
        get
        {
            return DataHelper.GetNotEmpty(ValidationHelper.GetString(GetValue("SubmenuIndicator"), menuElem.SubmenuIndicator), menuElem.SubmenuIndicator);
        }
        set
        {
            SetValue("SubmenuIndicator", value);
            menuElem.SubmenuIndicator = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether alternating styles are used.
    /// </summary>
    public bool UseAlternatingStyles
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("UseAlternatingStyles"), menuElem.UseAlternatingStyles);
        }
        set
        {
            SetValue("UseAlternatingStyles", value);
            menuElem.UseAlternatingStyles = value;
        }
    }


    /// <summary>
    /// Gets or sets the menu table padding.
    /// </summary>
    public int Padding
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("Padding"), menuElem.Padding);
        }
        set
        {
            SetValue("Padding", value);
            menuElem.Padding = value;
        }
    }


    /// <summary>
    /// Gets or sets the menu table spacing.
    /// </summary>
    public int Spacing
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("Spacing"), menuElem.Spacing);
        }
        set
        {
            SetValue("Spacing", value);
            menuElem.Spacing = value;
        }
    }


    /// <summary>
    /// Gets or sets the class that is applied to the separator.
    /// </summary>
    public string SeparatorCSS
    {
        get
        {
            return ValidationHelper.GetString(GetValue("SeparatorCSS"), menuElem.SeparatorCssClass);
        }
        set
        {
            SetValue("", value);
            menuElem.SeparatorCssClass = value;
        }
    }


    /// <summary>
    /// Gets or sets the separator height.
    /// </summary>
    public int SeparatorHeight
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("SeparatorHeight"), menuElem.SeparatorHeight);
        }
        set
        {
            SetValue("SeparatorHeight", value);
            menuElem.SeparatorHeight = value;
        }
    }


    /// <summary>
    /// Gets or sets the separator text.
    /// </summary>
    public string SeparatorText
    {
        get
        {
            return ValidationHelper.GetString(GetValue("SeparatorText"), menuElem.SeparatorText);
        }
        set
        {
            SetValue("SeparatorText", value);
            menuElem.SeparatorText = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether control should be hidden if no data found.
    /// </summary>
    public bool HideControlForZeroRows
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("HideControlForZeroRows"), menuElem.HideControlForZeroRows);
        }
        set
        {
            SetValue("HideControlForZeroRows", value);
            menuElem.HideControlForZeroRows = value;
        }
    }


    /// <summary>
    /// Gets or sets the text which is displayed for zero rows results.
    /// </summary>
    public string ZeroRowsText
    {
        get
        {
            return ValidationHelper.GetString(GetValue("ZeroRowsText"), menuElem.ZeroRowsText);
        }
        set
        {
            SetValue("ZeroRowsText", value);
            menuElem.ZeroRowsText = value;
        }
    }


    /// <summary>
    /// Filter name.
    /// </summary>
    public string FilterName
    {
        get
        {
            return ValidationHelper.GetString(GetValue("FilterName"), menuElem.FilterName);
        }
        set
        {
            SetValue("FilterName", value);
            menuElem.FilterName = value;
        }
    }


    /// <summary>
    /// Gets or sets property which indicates if menu caption should be HTML encoded.
    /// </summary>
    public bool EncodeMenuCaption
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("EncodeMenuCaption"), menuElem.EncodeMenuCaption);
        }
        set
        {
            SetValue("EncodeMenuCaption", value);
            menuElem.EncodeMenuCaption = value;
        }
    }


    /// <summary>
    /// Gets or sets the columns to be retrieved from database.
    /// </summary>  
    public string Columns
    {
        get
        {
            return ValidationHelper.GetString(GetValue("Columns"), menuElem.Columns);
        }
        set
        {
            SetValue("Columns", value);
            menuElem.Columns = value;
        }
    }

    #endregion


    #region "Stop processing"

    /// <summary>
    /// Returns true if the control processing should be stopped.
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
            menuElem.StopProcessing = value;
        }
    }

    #endregion


    /// <summary>
    /// Content loaded event handler.
    /// </summary>
    public override void OnContentLoaded()
    {
        base.OnContentLoaded();
        SetupControl();
    }


    /// <summary>
    /// Initializes the control properties.
    /// </summary>
    protected void SetupControl()
    {
        if (StopProcessing)
        {
            menuElem.StopProcessing = true;
        }
        else
        {
            menuElem.ControlContext = ControlContext;

            // Set properties from Webpart form        
            menuElem.ApplyMenuDesign = ApplyMenuDesign;
            menuElem.UseItemImagesForHighlightedItem = UseItemImagesForHiglightedItem;
            menuElem.CacheItemName = CacheItemName;
            menuElem.CacheDependencies = CacheDependencies;
            menuElem.CacheMinutes = CacheMinutes;
            menuElem.CheckPermissions = CheckPermissions;
            menuElem.ClassNames = ClassNames;
            menuElem.CombineWithDefaultCulture = CombineWithDefaultCulture;
            menuElem.CSSPrefix = CSSPrefix;
            menuElem.CultureCode = CultureCode;
            // Cursor is integer to MouseCursorEnum
            menuElem.Cursor = Cursor;
            menuElem.ExternalScriptPath = ExternalScriptPath;
            menuElem.HighlightAllItemsInPath = HighlightAllItemsInPath;
            menuElem.HighlightedNodePath = HighlightedNodePath;
            // Layout is integer to MenuLayoutEnum
            menuElem.Layout = Layout;
            menuElem.MaxRelativeLevel = MaxRelativeLevel;
            menuElem.OrderBy = OrderBy;
            menuElem.RenderItemName = RenderItemName;
            menuElem.Path = Path;
            menuElem.SelectOnlyPublished = SelectOnlyPublished;
            menuElem.SiteName = SiteName;
            menuElem.SubmenuIndicator = SubMenuIndicator;
            menuElem.UseAlternatingStyles = UseAlternatingStyles;
            menuElem.WhereCondition = WhereCondition;
            menuElem.Padding = Padding;
            menuElem.Spacing = Spacing;
            menuElem.SeparatorCssClass = SeparatorCSS;
            menuElem.SeparatorHeight = SeparatorHeight;
            menuElem.SeparatorText = SeparatorText;
            menuElem.EnableRTLBehaviour = EnableRTLBehaviour;
            menuElem.RenderImageAlt = RenderImageAlt;
            menuElem.EnableMouseUpDownClass = EnableMouseUpDownClass;
            menuElem.WordWrap = WordWrap;

            menuElem.HideControlForZeroRows = HideControlForZeroRows;
            menuElem.ZeroRowsText = ZeroRowsText;
            menuElem.EncodeMenuCaption = EncodeMenuCaption;

            menuElem.FilterName = FilterName;

            menuElem.Columns = Columns;
        }
    }


    /// <summary>
    /// Reload data.
    /// </summary>
    public override void ReloadData()
    {
        SetupControl();
        menuElem.ReloadData(true);
        base.ReloadData();
    }


    /// <summary>
    /// OnPreRender override (Set visibility).
    /// </summary>
    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        Visible = menuElem.Visible;

        if (DataHelper.DataSourceIsEmpty(menuElem.DataSource) && (menuElem.HideControlForZeroRows))
        {
            Visible = false;
        }
    }
}