using System;

using CMS.Helpers;
using CMS.PortalControls;

public partial class CMSWebParts_Navigation_cmstreemenu : CMSAbstractWebPart
{
    #region "Document properties"

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
    /// Gets or sets the value that indicates whether all subitems should be generated.
    /// </summary>
    public bool GenerateAllSubItems
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("GenerateAllSubItems"), menuElem.GenerateAllSubItems);
        }
        set
        {
            SetValue("GenerateAllSubItems", value);
            menuElem.GenerateAllSubItems = value;
        }
    }


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
    /// Gets or sets the value that indicates whether image alternate text is rendered.
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
    /// Gets or sets the CSS prefix.
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
    /// Gets or sets the item ID prefix.
    /// </summary>
    public string ItemIDPrefix
    {
        get
        {
            return ValidationHelper.GetString(GetValue("ItemIdPrefix"), menuElem.ItemIdPrefix);
        }
        set
        {
            SetValue("ItemIdPrefix", value);
            menuElem.ItemIdPrefix = value;
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
    /// Gets or sets the value that indicates whether highlighted item is displayed as link.
    /// </summary>
    public bool DisplayHighlightedItemAsLink
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("DisplayHighlightedItemAsLink"), menuElem.DisplayHighlightedItemAsLink);
        }
        set
        {
            SetValue("DisplayHighlightedItemAsLink", value);
            menuElem.DisplayHighlightedItemAsLink = value;
        }
    }


    /// <summary>
    /// Gets or sets the OnMouseOut javascript action.
    /// </summary>
    public string OnMouseOutScript
    {
        get
        {
            return DataHelper.GetNotEmpty(ValidationHelper.GetString(GetValue("OnMouseOutScript"), menuElem.OnMouseOutScript), menuElem.OnMouseOutScript);
        }
        set
        {
            SetValue("OnMouseOutScript", value);
            menuElem.OnMouseOutScript = value;
        }
    }


    /// <summary>
    /// Gets or sets the OnMouseOver javascript action.
    /// </summary>
    public string OnMouseOverScript
    {
        get
        {
            return DataHelper.GetNotEmpty(ValidationHelper.GetString(GetValue("OnMouseOverScript"), menuElem.OnMouseOverScript), menuElem.OnMouseOverScript);
        }
        set
        {
            SetValue("OnMouseOverScript", value);
            menuElem.OnMouseOverScript = value;
        }
    }


    /// <summary>
    /// Gets or sets the URL to the image which is applied as submenu indicator.
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
    /// Gets or sets the URL link target.
    /// </summary>
    public string UrlTarget
    {
        get
        {
            return DataHelper.GetNotEmpty(ValidationHelper.GetString(GetValue("UrlTarget"), menuElem.UrlTarget), menuElem.UrlTarget);
        }
        set
        {
            SetValue("UrlTarget", value);
            menuElem.UrlTarget = value;
        }
    }


    /// <summary>
    /// Gets or sets the table CellPadding.
    /// </summary>
    public int CellPadding
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("CellPadding"), menuElem.CellPadding);
        }
        set
        {
            SetValue("CellPadding", value);
            menuElem.CellPadding = value;
        }
    }


    /// <summary>
    /// Gets or sets the table CellSpacing.
    /// </summary>
    public int CellSpacing
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("CellSpacing"), menuElem.CellSpacing);
        }
        set
        {
            SetValue("CellSpacing", value);
            menuElem.CellSpacing = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether selected node is collapsed on click.
    /// </summary>
    public bool CollapseSelectedNodeOnClick
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("CollapseSelectedNodeOnClick"), menuElem.CollapseSelectedNodeOnClick);
        }
        set
        {
            SetValue("CollapseSelectedNodeOnClick", value);
            menuElem.CollapseSelectedNodeOnClick = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether link contain set of '&nbsp;' before text value with dependency to current indent level.
    /// </summary>
    public bool GenerateIndentationInsideLink
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("GenerateIndentationInsideLink"), menuElem.GenerateIndentationInsideLink);
        }
        set
        {
            SetValue("GenerateIndentationInsideLink", value);
            menuElem.GenerateIndentationInsideLink = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether only outer link is generated per each menu item.
    /// </summary>
    public bool GenerateOnlyOuterLink
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("GenerateOnlyOuterLink"), menuElem.GenerateOnlyOuterLink);
        }
        set
        {
            SetValue("GenerateOnlyOuterLink", value);
            menuElem.GenerateOnlyOuterLink = value;
        }
    }


    /// <summary>
    /// Gets or sets the number of spaces that will be placed before each level of menu items.
    /// </summary>
    public int Indentation
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("Indentation"), menuElem.Indentation);
        }
        set
        {
            SetValue("Indentation", value);
            menuElem.Indentation = value;
        }
    }


    /// <summary>
    /// Gets or sets the menu item image URL.
    /// </summary>
    public string MenuItemImageUrl
    {
        get
        {
            return DataHelper.GetNotEmpty(ValidationHelper.GetString(GetValue("MenuItemImageUrl"), menuElem.MenuItemImageUrl), menuElem.MenuItemImageUrl);
        }
        set
        {
            SetValue("MenuItemImageUrl", value);
            menuElem.MenuItemImageUrl = value;
        }
    }


    /// <summary>
    /// Gets or sets the menu item OpenImage URL.
    /// </summary>
    public string MenuItemOpenImageUrl
    {
        get
        {
            return DataHelper.GetNotEmpty(ValidationHelper.GetString(GetValue("MenuItemOpenImageUrl"), menuElem.MenuItemOpenImageUrl), menuElem.MenuItemOpenImageUrl);
        }
        set
        {
            SetValue("MenuItemOpenImageUrl", value);
            menuElem.MenuItemOpenImageUrl = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether link title element will be rendered.
    /// </summary>
    public bool RenderLinkTitle
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("RenderLinkTitle"), menuElem.RenderLinkTitle);
        }
        set
        {
            SetValue("RenderLinkTitle", value);
            menuElem.RenderLinkTitle = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether the item id prefix is set up automatically with according to control client id
    /// If this property is true, the property ItemIdPrefix has no effect
    /// </summary>
    public bool GenerateUniqueIDs
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("GenerateUniqueIDs"), true);
        }
        set
        {
            SetValue("GenerateUniqueIDs", value);
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether under the selected item should be rendered (visible) sub-items.
    /// </summary>
    public bool RenderSubItems
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("RenderSubItems"), true);
        }
        set
        {
            SetValue("RenderSubItems", value);
            menuElem.RenderSubItems = value;
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


    /// <summary>
    /// Content loaded event handler.
    /// </summary>
    public override void OnContentLoaded()
    {
        base.OnContentLoaded();
        SetupControl();
    }


    /// <summary>
    /// Reloads data.
    /// </summary>
    public override void ReloadData()
    {
        base.ReloadData();
        SetupControl();
        menuElem.ReloadData(true);
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

            // Document properties
            menuElem.ApplyMenuDesign = ApplyMenuDesign;
            menuElem.UseItemImagesForHighlightedItem = UseItemImagesForHiglightedItem;
            menuElem.CacheItemName = CacheItemName;
            menuElem.CacheDependencies = CacheDependencies;
            menuElem.CacheMinutes = CacheMinutes;
            menuElem.WhereCondition = WhereCondition;
            menuElem.OrderBy = OrderBy;
            menuElem.Path = Path;
            menuElem.SelectOnlyPublished = SelectOnlyPublished;
            menuElem.SiteName = SiteName;
            menuElem.CheckPermissions = CheckPermissions;
            menuElem.ClassNames = ClassNames;
            menuElem.CultureCode = CultureCode;
            menuElem.CombineWithDefaultCulture = CombineWithDefaultCulture;
            menuElem.MaxRelativeLevel = MaxRelativeLevel;

            // Public properties
            menuElem.CSSPrefix = CSSPrefix;
            menuElem.DisplayHighlightedItemAsLink = DisplayHighlightedItemAsLink;
            menuElem.HighlightAllItemsInPath = HighlightAllItemsInPath;
            menuElem.HighlightedNodePath = HighlightedNodePath;
            menuElem.OnMouseOutScript = OnMouseOutScript;
            menuElem.OnMouseOverScript = OnMouseOverScript;
            menuElem.SubmenuIndicator = SubMenuIndicator;
            menuElem.UrlTarget = UrlTarget;
            menuElem.UseAlternatingStyles = UseAlternatingStyles;
            menuElem.EncodeMenuCaption = EncodeMenuCaption;

            menuElem.CellPadding = CellPadding;
            menuElem.CellSpacing = CellSpacing;

            menuElem.CollapseSelectedNodeOnClick = CollapseSelectedNodeOnClick;
            menuElem.GenerateIndentationInsideLink = GenerateIndentationInsideLink;
            menuElem.GenerateOnlyOuterLink = GenerateOnlyOuterLink;
            menuElem.Indentation = Indentation;
            menuElem.MenuItemImageUrl = MenuItemImageUrl;
            menuElem.MenuItemOpenImageUrl = MenuItemOpenImageUrl;
            menuElem.RenderImageAlt = RenderImageAlt;
            menuElem.RenderLinkTitle = RenderLinkTitle;
            menuElem.GenerateAllSubItems = GenerateAllSubItems;

            menuElem.ItemIdPrefix = ItemIDPrefix;

            if (GenerateUniqueIDs)
            {
                menuElem.ItemIdPrefix = ClientID;
            }

            menuElem.WordWrap = WordWrap;
            menuElem.RenderSubItems = RenderSubItems;

            menuElem.HideControlForZeroRows = HideControlForZeroRows;
            menuElem.ZeroRowsText = ZeroRowsText;

            menuElem.FilterName = FilterName;

            menuElem.Columns = Columns;
        }
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