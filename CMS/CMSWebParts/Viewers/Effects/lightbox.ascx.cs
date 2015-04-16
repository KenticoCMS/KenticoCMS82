using System;
using System.Text;
using System.Web.UI;

using CMS.Controls;
using CMS.Helpers;
using CMS.IO;
using CMS.PortalControls;
using CMS.PortalEngine;

public partial class CMSWebParts_Viewers_Effects_lightbox : CMSAbstractWebPart
{
    #region "Document properties"

    /// <summary>
    /// Gets or sets the cache item name.
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
            repItems.CacheItemName = value;
        }
    }


    /// <summary>
    /// Cache dependencies, each cache dependency on a new line.
    /// </summary>
    public override string CacheDependencies
    {
        get
        {
            return ValidationHelper.GetString(base.CacheDependencies, repItems.CacheDependencies);
        }
        set
        {
            base.CacheDependencies = value;
            repItems.CacheDependencies = value;
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
            repItems.CacheMinutes = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether permissions should be checked.
    /// </summary>
    public bool CheckPermissions
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("CheckPermissions"), repItems.CheckPermissions);
        }
        set
        {
            SetValue("CheckPermissions", value);
            repItems.CheckPermissions = value;
        }
    }


    /// <summary>
    /// Gets or sets the class names which should be displayed.
    /// </summary>
    public string ClassNames
    {
        get
        {
            return DataHelper.GetNotEmpty(ValidationHelper.GetString(GetValue("Classnames"), repItems.ClassNames), repItems.ClassNames);
        }
        set
        {
            SetValue("ClassNames", value);
            repItems.ClassNames = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether documents are combined with default culture version.
    /// </summary>
    public bool CombineWithDefaultCulture
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("CombineWithDefaultCulture"), repItems.CombineWithDefaultCulture);
        }
        set
        {
            SetValue("CombineWithDefaultCulture", value);
            repItems.CombineWithDefaultCulture = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether filter out duplicate documents.
    /// </summary>
    public bool FilterOutDuplicates
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("FilterOutDuplicates"), repItems.FilterOutDuplicates);
        }
        set
        {
            SetValue("FilterOutDuplicates", value);
            repItems.FilterOutDuplicates = value;
        }
    }


    /// <summary>
    /// Gets or sets the culture code of the documents.
    /// </summary>
    public string CultureCode
    {
        get
        {
            return DataHelper.GetNotEmpty(ValidationHelper.GetString(GetValue("CultureCode"), repItems.CultureCode), repItems.CultureCode);
        }
        set
        {
            SetValue("CultureCode", value);
            repItems.CultureCode = value;
        }
    }


    /// <summary>
    /// Gets or sets the maximal relative level of the documents to be shown.
    /// </summary>
    public int MaxRelativeLevel
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("MaxRelativeLevel"), repItems.MaxRelativeLevel);
        }
        set
        {
            SetValue("MaxRelativeLevel", value);
            repItems.MaxRelativeLevel = value;
        }
    }


    /// <summary>
    /// Gets or sets the order by clause.
    /// </summary>
    public string OrderBy
    {
        get
        {
            return DataHelper.GetNotEmpty(ValidationHelper.GetString(GetValue("OrderBy"), repItems.OrderBy), repItems.OrderBy);
        }
        set
        {
            SetValue("OrderBy", value);
            repItems.OrderBy = value;
        }
    }


    /// <summary>
    /// Gets or sets the path of the documents.
    /// </summary>
    public string Path
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("Path"), null);
        }
        set
        {
            SetValue("Path", value);
            repItems.Path = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether only published documents are selected.
    /// </summary>
    public bool SelectOnlyPublished
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("SelectOnlyPublished"), repItems.SelectOnlyPublished);
        }
        set
        {
            SetValue("SelectOnlyPublished", value);
            repItems.SelectOnlyPublished = value;
        }
    }


    /// <summary>
    /// Gets or sets the site name.
    /// </summary>
    public string SiteName
    {
        get
        {
            return DataHelper.GetNotEmpty(ValidationHelper.GetString(GetValue("SiteName"), repItems.SiteName), repItems.SiteName);
        }
        set
        {
            SetValue("SiteName", value);
            repItems.SiteName = value;
        }
    }


    /// <summary>
    /// Gets or sets the where condition.
    /// </summary>
    public string WhereCondition
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("WhereCondition"), repItems.WhereCondition);
        }
        set
        {
            SetValue("WhereCondition", value);
            repItems.WhereCondition = value;
        }
    }


    /// <summary>
    /// Gets or sets the number which indicates how many documents should be displayed.
    /// </summary>
    public int SelectTopN
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("SelectTopN"), repItems.SelectTopN);
        }
        set
        {
            SetValue("SelectTopN", value);
            repItems.SelectTopN = value;
        }
    }


    /// <summary>
    /// Gets or sets the columns to get.
    /// </summary>
    public string Columns
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("Columns"), repItems.Columns);
        }
        set
        {
            SetValue("Columns", value);
            repItems.Columns = value;
        }
    }

    #endregion


    #region "Pager properties"

    /// <summary>
    /// Gets or sets the value that indicates whether paging is enabled.
    /// </summary>
    public bool EnablePaging
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("EnablePaging"), repItems.EnablePaging);
        }
        set
        {
            SetValue("EnablePaging", value);
            repItems.EnablePaging = value;
        }
    }


    /// <summary>
    /// Gets or sets the pager position.
    /// </summary>
    public PagingPlaceTypeEnum PagerPosition
    {
        get
        {
            return repItems.PagerControl.GetPagerPosition(DataHelper.GetNotEmpty(GetValue("PagerPosition"), repItems.PagerControl.PagerPosition.ToString()));
        }
        set
        {
            SetValue("PagerPosition", value.ToString());
            repItems.PagerControl.PagerPosition = value;
        }
    }


    /// <summary>
    /// Gets or sets the number of the documents displayed on each sigle page.
    /// </summary>
    public int PageSize
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("PageSize"), repItems.PagerControl.PageSize);
        }
        set
        {
            SetValue("PageSize", value);
            repItems.PagerControl.PageSize = value;
        }
    }


    /// <summary>
    /// Gets or sets the pager query string key.
    /// </summary>
    public string QueryStringKey
    {
        get
        {
            return DataHelper.GetNotEmpty(ValidationHelper.GetString(GetValue("QueryStringKey"), repItems.PagerControl.QueryStringKey), repItems.PagerControl.QueryStringKey);
        }
        set
        {
            SetValue("QueryStringKey", value);
            repItems.PagerControl.QueryStringKey = value;
        }
    }


    /// <summary>
    /// Gets or sets the paging mode.
    /// </summary>
    public PagingModeTypeEnum PagingMode
    {
        get
        {
            return repItems.PagerControl.GetPagingMode(DataHelper.GetNotEmpty(GetValue("PagingMode"), repItems.PagerControl.PagingMode.ToString()));
        }
        set
        {
            SetValue("PagingMode", value.ToString());
            repItems.PagerControl.PagingMode = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether  first and last page is shown if paging is allowed.
    /// </summary>
    public bool ShowFirstLast
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ShowFirstLast"), repItems.PagerControl.ShowFirstLast);
        }
        set
        {
            SetValue("ShowFirstLast", value);
            repItems.PagerControl.ShowFirstLast = value;
        }
    }

    #endregion


    #region "Relationships properties"

    /// <summary>
    /// Gets or sets the related node is on the left side.
    /// </summary>
    public bool RelatedNodeIsOnTheLeftSide
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("RelatedNodeIsOnTheLeftSide"), repItems.RelatedNodeIsOnTheLeftSide);
        }
        set
        {
            SetValue("RelatedNodeIsOnTheLeftSide", value);
            repItems.RelatedNodeIsOnTheLeftSide = value;
        }
    }


    /// <summary>
    /// Gets or sets the relationship name.
    /// </summary>
    public string RelationshipName
    {
        get
        {
            return DataHelper.GetNotEmpty(ValidationHelper.GetString(GetValue("RelationshipName"), repItems.RelationshipName), repItems.RelationshipName);
        }
        set
        {
            SetValue("RelationshipName", value);
            repItems.RelationshipName = value;
        }
    }


    /// <summary>
    /// Gets or sets the relationship with node GUID.
    /// </summary>
    public Guid RelationshipWithNodeGUID
    {
        get
        {
            return ValidationHelper.GetGuid(GetValue("RelationshipWithNodeGuid"), repItems.RelationshipWithNodeGuid);
        }
        set
        {
            SetValue("RelationshipWithNodeGuid", value);
            repItems.RelationshipWithNodeGuid = value;
        }
    }

    #endregion


    #region "Transformation properties"

    /// <summary>
    /// Gets or sets the name of the transforamtion which is used for displaying the results.
    /// </summary>
    public string TransformationName
    {
        get
        {
            return DataHelper.GetNotEmpty(ValidationHelper.GetString(GetValue("TransformationName"), repItems.TransformationName), repItems.TransformationName);
        }
        set
        {
            SetValue("TransformationName", value);
            repItems.TransformationName = value;
        }
    }


    /// <summary>
    /// Gets or sets the name of the transforamtion which is used for displaying the alternate results.
    /// </summary>
    public string AlternatingTransformationName
    {
        get
        {
            return DataHelper.GetNotEmpty(ValidationHelper.GetString(GetValue("AlternatingTransformationName"), repItems.AlternatingTransformationName), repItems.AlternatingTransformationName);
        }
        set
        {
            SetValue("AlternatingTransformationName", value);
            repItems.AlternatingTransformationName = value;
        }
    }


    /// <summary>
    /// Gets or sets the name of the transforamtion which is used for displaying the results of the selected item.
    /// </summary>
    public string SelectedItemTransformationName
    {
        get
        {
            return DataHelper.GetNotEmpty(ValidationHelper.GetString(GetValue("SelectedItemTransformationName"), repItems.SelectedItemTransformationName), repItems.SelectedItemTransformationName);
        }
        set
        {
            SetValue("SelectedItemTransformationName", value);
            repItems.SelectedItemTransformationName = value;
        }
    }

    #endregion


    #region "Public properties"

    /// <summary>
    /// Gets or sets the nested controls IDs. Use ';' like separator.
    /// </summary>
    public string NestedControlsID
    {
        get
        {
            return ValidationHelper.GetString(GetValue("NestedControlsID"), repItems.NestedControlsID);
        }
        set
        {
            SetValue("NestedControlsID", value);
            repItems.NestedControlsID = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether control should be hidden if no data found.
    /// </summary>
    public bool HideControlForZeroRows
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("HideControlForZeroRows"), repItems.HideControlForZeroRows);
        }
        set
        {
            SetValue("HideControlForZeroRows", value);
            repItems.HideControlForZeroRows = value;
        }
    }


    /// <summary>
    /// Gets or sets the text which is displayed for zero rows result.
    /// </summary>
    public string ZeroRowsText
    {
        get
        {
            return DataHelper.GetNotEmpty(ValidationHelper.GetString(GetValue("ZeroRowsText"), repItems.ZeroRowsText), repItems.ZeroRowsText);
        }
        set
        {
            SetValue("ZeroRowsText", value);
            repItems.ZeroRowsText = value;
        }
    }


    /// <summary>
    /// Gets or sets the separator (tetx, html code) which is displayed between displayed items.
    /// </summary>
    public string ItemSeparator
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("ItemSeparator"), repItems.ItemSeparator);
        }
        set
        {
            SetValue("ItemSeparator", value);
            repItems.ItemSeparator = value;
        }
    }

    #endregion


    #region "LightBox properties"

    /// <summary>
    /// Gets or sets the external script path.
    /// </summary>
    public string LightBoxExternalScriptPath
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("LightBoxExternalScriptPath"), extLightbox.LightBoxExternalScriptPath);
        }
        set
        {
            SetValue("LightBoxExternalScriptPath", value);
            extLightbox.LightBoxExternalScriptPath = value;
        }
    }


    /// <summary>
    /// Gets or sets the transparency of shadow overlay.
    /// </summary>
    public float LightBoxOverlayOpacity
    {
        get
        {
            return (float)ValidationHelper.GetDoubleSystem(GetValue("LightBoxOverlayOpacity"), extLightbox.LightBoxOverlayOpacity);
        }
        set
        {
            SetValue("LightBoxOverlayOpacity", value);
            extLightbox.LightBoxOverlayOpacity = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether resizing should be animated.
    /// </summary>
    public bool LightBoxAnimate
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("LightBoxAnimate"), extLightbox.LightBoxAnimate);
        }
        set
        {
            SetValue("LightBoxAnimate", value);
            extLightbox.LightBoxAnimate = value;
        }
    }


    /// <summary>
    /// Gets or sets the window width.
    /// </summary>
    public int LightBoxWidth
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("LightBoxWidth"), extLightbox.LightBoxWidth);
        }
        set
        {
            SetValue("LightBoxWidth", value);
            extLightbox.LightBoxWidth = value;
        }
    }


    /// <summary>
    /// Gets or sets the window height.
    /// </summary>
    public int LightBoxHeight
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("LightBoxHeight"), extLightbox.LightBoxHeight);
        }
        set
        {
            SetValue("LightBoxHeight", value);
            extLightbox.LightBoxHeight = value;
        }
    }


    /// <summary>
    /// Gets or sets speed of resizing animations.
    /// </summary>
    public int LightBoxResizeSpeed
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("LightBoxResizeSpeed"), extLightbox.LightBoxResizeSpeed);
        }
        set
        {
            SetValue("LightBoxResizeSpeed", value);
            extLightbox.LightBoxResizeSpeed = value;
        }
    }


    /// <summary>
    /// Gets or sets the border size.
    /// </summary>
    public int LightBoxBorderSize
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("LightBoxBorderSize"), extLightbox.LightBoxBorderSize);
        }

        set
        {
            SetValue("LightBoxBorderSize", value);
            extLightbox.LightBoxBorderSize = value;
        }
    }


    /// <summary>
    /// Gets or sets the loading image URL.
    /// </summary>
    public string LightBoxLoadingImg
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("LightBoxLoadingImg"), extLightbox.LightBoxLoadingImg);
        }
        set
        {
            SetValue("LightBoxLoadingImg", value);
            extLightbox.LightBoxLoadingImg = value;
        }
    }


    /// <summary>
    /// Gets or sets the close image URL.
    /// </summary>
    public string LightBoxCloseImg
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("LightBoxCloseImg"), extLightbox.LightBoxCloseImg);
        }
        set
        {
            SetValue("LightBoxCloseImg", value);
            extLightbox.LightBoxCloseImg = value;
        }
    }


    /// <summary>
    /// Gets or sets the previous image URL.
    /// </summary>
    public string LightBoxPrevImg
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("LightBoxPrevImg"), extLightbox.LightBoxPrevImg);
        }
        set
        {
            SetValue("LightBoxPrevImg", value);
            extLightbox.LightBoxPrevImg = value;
        }
    }


    /// <summary>
    /// Gets or sets the next image URL.
    /// </summary>
    public string LightBoxNextImg
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("LightBoxNextImg"), extLightbox.LightBoxNextImg);
        }
        set
        {
            SetValue("LightBoxNextImg", value);
            extLightbox.LightBoxNextImg = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether the navigation buttons are still visible,
    /// not only on mouse over
    /// </summary>
    public bool LightBoxPermanentNavigation
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("LightBoxPermanentNavigation"), false);
        }
        set
        {
            SetValue("LightBoxPermanentNavigation", value);
            extLightbox.LightBoxPermanentNavigation = value;
        }
    }


    /// <summary>
    /// Gets or sets the load delay time(in miliseconds). If you are using automatic resizing,
    /// this value indicates how long will be lightbox runtime wait for taking element size.
    /// If you have problem with displaying some content with lightbox, try use upper value.
    /// </summary>
    public int LightBoxLoadDelay
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("LightBoxLoadDelay"), 250);
        }
        set
        {
            SetValue("LightBoxLoadDelay", value);
            extLightbox.LightBoxLoadDelay = value;
        }
    }


    /// <summary>
    /// Gets or sets lightbox group name. It is necessary to set this property if you would like
    /// to display multiple lightboxes for different doc.types on a single page.
    /// </summary>
    public string LightBoxGroup
    {
        get
        {
            return ValidationHelper.GetString(GetValue("LightBoxGroup"), null);
        }
        set
        {
            SetValue("LightBoxGroup", value);
            extLightbox.LightBoxGroup = value;
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
            repItems.StopProcessing = value;
            extLightbox.StopProcessing = value;
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Initializes the control properties.
    /// </summary>
    protected void SetupControl()
    {
        if (StopProcessing)
        {
            repItems.StopProcessing = true;
            extLightbox.StopProcessing = true;
        }
        else
        {
            repItems.ControlContext = ControlContext;

            // Document properties
            repItems.CacheItemName = CacheItemName;
            repItems.CacheDependencies = CacheDependencies;
            repItems.CacheMinutes = CacheMinutes;
            repItems.CheckPermissions = CheckPermissions;
            repItems.ClassNames = ClassNames;
            repItems.CombineWithDefaultCulture = CombineWithDefaultCulture;
            repItems.CultureCode = CultureCode;
            repItems.MaxRelativeLevel = MaxRelativeLevel;
            repItems.OrderBy = OrderBy;
            repItems.SelectTopN = SelectTopN;
            repItems.Columns = Columns;
            repItems.SelectOnlyPublished = SelectOnlyPublished;
            repItems.FilterOutDuplicates = FilterOutDuplicates;
            repItems.Path = Path;
            repItems.SiteName = SiteName;
            repItems.WhereCondition = WhereCondition;

            // Pager
            repItems.EnablePaging = EnablePaging;
            repItems.PagerControl.PagerPosition = PagerPosition;
            repItems.PagerControl.PageSize = PageSize;
            repItems.PagerControl.QueryStringKey = QueryStringKey;
            repItems.PagerControl.PagingMode = PagingMode;
            repItems.PagerControl.ShowFirstLast = ShowFirstLast;


            #region "Lightbox properties"

            extLightbox.LightBoxLoadDelay = LightBoxLoadDelay;
            extLightbox.LightBoxPermanentNavigation = LightBoxPermanentNavigation;
            extLightbox.LightBoxNextImg = LightBoxNextImg;
            extLightbox.LightBoxPrevImg = LightBoxPrevImg;
            extLightbox.LightBoxCloseImg = LightBoxCloseImg;
            extLightbox.LightBoxLoadingImg = LightBoxLoadingImg;
            extLightbox.LightBoxBorderSize = LightBoxBorderSize;
            extLightbox.LightBoxResizeSpeed = LightBoxResizeSpeed;
            extLightbox.LightBoxHeight = LightBoxHeight;
            extLightbox.LightBoxWidth = LightBoxWidth;
            extLightbox.LightBoxAnimate = LightBoxAnimate;
            extLightbox.LightBoxOverlayOpacity = LightBoxOverlayOpacity;
            extLightbox.LightBoxExternalScriptPath = LightBoxExternalScriptPath;
            extLightbox.LightBoxGroup = LightBoxGroup;
            if (ParentZone != null)
            {
                extLightbox.CheckCollision = ParentZone.WebPartManagementRequired;
            }
            else
            {
                extLightbox.CheckCollision = PortalContext.IsDesignMode(ViewMode, false);
            }

            #endregion


            // Relationships
            repItems.RelatedNodeIsOnTheLeftSide = RelatedNodeIsOnTheLeftSide;
            repItems.RelationshipName = RelationshipName;
            repItems.RelationshipWithNodeGuid = RelationshipWithNodeGUID;

            // Transformation properties
            repItems.TransformationName = TransformationName;
            repItems.AlternatingTransformationName = AlternatingTransformationName;
            repItems.SelectedItemTransformationName = SelectedItemTransformationName;

            // Public properties
            repItems.HideControlForZeroRows = HideControlForZeroRows;
            repItems.ZeroRowsText = ZeroRowsText;
            repItems.ItemSeparator = ItemSeparator;
            repItems.NestedControlsID = NestedControlsID;

            // Add repeater to the filter collection
            CMSControlsHelper.SetFilter(ValidationHelper.GetString(GetValue("WebPartControlID"), ID), repItems);
        }
    }


    protected override void OnLoad(EventArgs e)
    {
        // Initialize lightbox extender
        extLightbox.OnItemSelected += extLightbox_OnItemSelected;
        base.OnLoad(e);
    }


    private string extLightbox_OnItemSelected(string selectedItem)
    {
        repItems.StopProcessing = false;
        repItems.TransformationName = repItems.SelectedItemTransformationName;
        repItems.Path = selectedItem;
        repItems.ReloadData(true);

        // Render repeater data to String
        StringBuilder stringBuilder = new StringBuilder();
        StringWriter sw = new StringWriter(stringBuilder);
        Html32TextWriter writer = new Html32TextWriter(sw);
        repItems.RenderControl(writer);

        return TextHelper.EnsureLineEndings(stringBuilder.ToString(), string.Empty);
    }


    /// <summary>
    /// Content loaded event handler.
    /// </summary>
    public override void OnContentLoaded()
    {
        base.OnContentLoaded();
        SetupControl();
    }


    /// <summary>
    /// OnPrerender override (Set visibility).
    /// </summary>
    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        Visible = repItems.Visible && !StopProcessing;

        if (DataHelper.DataSourceIsEmpty(repItems.DataSource) && repItems.HideControlForZeroRows)
        {
            Visible = false;
        }

        extLightbox.StopProcessing = !Visible;
    }


    /// <summary>
    /// Reload data.
    /// </summary>
    public override void ReloadData()
    {
        base.ReloadData();
        SetupControl();
        repItems.ReloadData(true);
    }


    /// <summary>
    /// Clears cache.
    /// </summary>
    public override void ClearCache()
    {
        repItems.ClearCache();
    }

    #endregion
}