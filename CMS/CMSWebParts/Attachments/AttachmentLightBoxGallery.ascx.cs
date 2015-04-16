using System;

using CMS.Controls;
using CMS.Helpers;
using CMS.PortalControls;
using CMS.PortalEngine;
using CMS.Base;
using CMS.DocumentEngine;

public partial class CMSWebParts_Attachments_AttachmentLightBoxGallery : CMSAbstractWebPart
{
    #region "Basic repeater properties"

    /// <summary>
    /// Gets or sets AlternatingItemTemplate property.
    /// </summary>
    public string AlternatingTransformationName
    {
        get
        {
            return ValidationHelper.GetString(GetValue("AlternatingTransformationName"), galleryElem.AlternatingTransformationName);
        }
        set
        {
            SetValue("AlternatingTransformationName", value);
            galleryElem.AlternatingTransformationName = value;
        }
    }


    /// <summary>
    /// Gets or sets FooterTemplate property.
    /// </summary>
    public string FooterTransformationName
    {
        get
        {
            return ValidationHelper.GetString(GetValue("FooterTransformationName"), galleryElem.FooterTransformationName);
        }
        set
        {
            SetValue("FooterTransformationName", value);
            galleryElem.FooterTransformationName = value;
        }
    }


    /// <summary>
    /// Gets or sets HeaderTemplate property.
    /// </summary>
    public string HeaderTransformationName
    {
        get
        {
            return ValidationHelper.GetString(GetValue("HeaderTransformationName"), galleryElem.HeaderTransformationName);
        }
        set
        {
            SetValue("HeaderTransformationName", value);
            galleryElem.HeaderTransformationName = value;
        }
    }


    /// <summary>
    /// Gets or sets ItemTemplate property.
    /// </summary>
    public string TransformationName
    {
        get
        {
            return ValidationHelper.GetString(GetValue("TransformationName"), galleryElem.TransformationName);
        }
        set
        {
            SetValue("TransformationName", value);
            galleryElem.TransformationName = value;
        }
    }


    /// <summary>
    /// Gets or sets SelectedItemTransformationName property.
    /// </summary>
    public string SelectedItemTransformationName
    {
        get
        {
            return ValidationHelper.GetString(GetValue("SelectedItemTransformationName"), galleryElem.SelectedItemTransformationName);
        }
        set
        {
            SetValue("SelectedItemTransformationName", value);
            galleryElem.SelectedItemTransformationName = value;
        }
    }


    /// <summary>
    /// Gets or sets SeparatorTemplate property.
    /// </summary>
    public string SeparatorTransformationName
    {
        get
        {
            return ValidationHelper.GetString(GetValue("SeparatorTransformationName"), galleryElem.SeparatorTransformationName);
        }
        set
        {
            SetValue("SeparatorTransformationName", value);
            galleryElem.SeparatorTransformationName = value;
        }
    }


    /// <summary>
    /// Gets or sets HideControlForZeroRows property.
    /// </summary>
    public bool HideControlForZeroRows
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("HideControlForZeroRows"), galleryElem.HideControlForZeroRows);
        }
        set
        {
            SetValue("HideControlForZeroRows", value);
            galleryElem.HideControlForZeroRows = value;
        }
    }


    /// <summary>
    /// Gets or sets ZeroRowsText property.
    /// </summary>
    public string ZeroRowsText
    {
        get
        {
            return ValidationHelper.GetString(GetValue("ZeroRowsText"), galleryElem.ZeroRowsText);
        }
        set
        {
            SetValue("ZeroRowsText", value);
            galleryElem.ZeroRowsText = value;
        }
    }

    #endregion


    #region "UniPager properties"

    /// <summary>
    /// Gets or sets the value that indicates whether pager should be hidden for single page.
    /// </summary>
    public bool HidePagerForSinglePage
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("HidePagerForSinglePage"), galleryElem.HidePagerForSinglePage);
        }
        set
        {
            SetValue("HidePagerForSinglePage", value);
            galleryElem.HidePagerForSinglePage = value;
        }
    }


    /// <summary>
    /// Gets or sets the number of records to display on a page.
    /// </summary>
    public int PageSize
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("PageSize"), galleryElem.PageSize);
        }
        set
        {
            SetValue("PageSize", value);
            galleryElem.PageSize = value;
        }
    }


    /// <summary>
    /// Gets or sets the number of pages displayed for current page range.
    /// </summary>
    public int GroupSize
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("GroupSize"), galleryElem.GroupSize);
        }
        set
        {
            SetValue("GroupSize", value);
            galleryElem.GroupSize = value;
        }
    }


    /// <summary>
    /// Gets or sets the pager mode.
    /// </summary>
    public string PagingMode
    {
        get
        {
            return ValidationHelper.GetString(GetValue("PagingMode"), "querystring");
        }
        set
        {
            if (value != null)
            {
                SetValue("PagingMode", value);
                switch (value.ToLowerCSafe())
                {
                    case "postback":
                        galleryElem.PagingMode = UniPagerMode.PostBack;
                        break;
                    default:
                        galleryElem.PagingMode = UniPagerMode.Querystring;
                        break;
                }
            }
        }
    }


    /// <summary>
    /// Gets or sets the querysting parameter.
    /// </summary>
    public string QueryStringKey
    {
        get
        {
            return ValidationHelper.GetString(GetValue("QueryStringKey"), galleryElem.QueryStringKey);
        }
        set
        {
            SetValue("QueryStringKey", value);
            galleryElem.QueryStringKey = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether first and last item template are displayed dynamically based on current view.
    /// </summary>
    public bool DisplayFirstLastAutomatically
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("DisplayFirstLastAutomatically"), galleryElem.DisplayFirstLastAutomatically);
        }
        set
        {
            SetValue("DisplayFirstLastAutomatically", value);
            galleryElem.DisplayFirstLastAutomatically = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether first and last item template are displayed dynamically based on current view.
    /// </summary>
    public bool DisplayPreviousNextAutomatically
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("DisplayPreviousNextAutomatically"), galleryElem.DisplayPreviousNextAutomatically);
        }
        set
        {
            SetValue("DisplayPreviousNextAutomatically", value);
            galleryElem.DisplayPreviousNextAutomatically = value;
        }
    }

    #endregion


    #region "UniPager Template properties"

    /// <summary>
    /// Gets or sets the pages template.
    /// </summary>
    public string PagesTemplate
    {
        get
        {
            return ValidationHelper.GetString(GetValue("Pages"), galleryElem.PagesTemplate);
        }
        set
        {
            SetValue("Pages", value);
            galleryElem.PagesTemplate = value;
        }
    }


    /// <summary>
    /// Gets or sets the current page template.
    /// </summary>
    public string CurrentPageTemplate
    {
        get
        {
            return ValidationHelper.GetString(GetValue("CurrentPage"), galleryElem.CurrentPageTemplate);
        }
        set
        {
            SetValue("CurrentPage", value);
            galleryElem.CurrentPageTemplate = value;
        }
    }


    /// <summary>
    /// Gets or sets the separator template.
    /// </summary>
    public string SeparatorTemplate
    {
        get
        {
            return ValidationHelper.GetString(GetValue("PageSeparator"), galleryElem.SeparatorTemplate);
        }
        set
        {
            SetValue("PageSeparator", value);
            galleryElem.SeparatorTemplate = value;
        }
    }


    /// <summary>
    /// Gets or sets the first page template.
    /// </summary>
    public string FirstPageTemplate
    {
        get
        {
            return ValidationHelper.GetString(GetValue("FirstPage"), galleryElem.FirstPageTemplate);
        }
        set
        {
            SetValue("FirstPage", value);
            galleryElem.FirstPageTemplate = value;
        }
    }


    /// <summary>
    /// Gets or sets the last page template.
    /// </summary>
    public string LastPageTemplate
    {
        get
        {
            return ValidationHelper.GetString(GetValue("LastPage"), galleryElem.LastPageTemplate);
        }
        set
        {
            SetValue("LastPage", value);
            galleryElem.LastPageTemplate = value;
        }
    }


    /// <summary>
    /// Gets or sets the previous page template.
    /// </summary>
    public string PreviousPageTemplate
    {
        get
        {
            return ValidationHelper.GetString(GetValue("PreviousPage"), galleryElem.PreviousPageTemplate);
        }
        set
        {
            SetValue("PreviousPage", value);
            galleryElem.PreviousPageTemplate = value;
        }
    }


    /// <summary>
    /// Gets or sets the next page template.
    /// </summary>
    public string NextPageTemplate
    {
        get
        {
            return ValidationHelper.GetString(GetValue("NextPage"), galleryElem.NextPageTemplate);
        }
        set
        {
            SetValue("NextPage", value);
            galleryElem.NextPageTemplate = value;
        }
    }


    /// <summary>
    /// Gets or sets the previous group template.
    /// </summary>
    public string PreviousGroupTemplate
    {
        get
        {
            return ValidationHelper.GetString(GetValue("PreviousGroup"), galleryElem.PreviousGroupTemplate);
        }
        set
        {
            SetValue("PreviousGroup", value);
            galleryElem.PreviousGroupTemplate = value;
        }
    }


    /// <summary>
    /// Gets or sets the next group template.
    /// </summary>
    public string NextGroupTemplate
    {
        get
        {
            return ValidationHelper.GetString(GetValue("NextGroup"), galleryElem.NextGroupTemplate);
        }
        set
        {
            SetValue("NextGroup", value);
            galleryElem.NextGroupTemplate = value;
        }
    }


    /// <summary>
    /// Gets or sets the layout template.
    /// </summary>
    public string LayoutTemplate
    {
        get
        {
            return ValidationHelper.GetString(GetValue("PagerLayout"), galleryElem.LayoutTemplate);
        }
        set
        {
            SetValue("PagerLayout", value);
            galleryElem.LayoutTemplate = value;
        }
    }

    #endregion


    #region "Data source properties"

    /// <summary>
    /// Gets or sets WHERE condition.
    /// </summary>
    public string WhereCondition
    {
        get
        {
            return ValidationHelper.GetString(GetValue("WhereCondition"), galleryElem.WhereCondition);
        }
        set
        {
            SetValue("WhereCondition", value);
            galleryElem.WhereCondition = value;
        }
    }


    /// <summary>
    /// Gets or sets top N.
    /// </summary>
    public int TopN
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("SelectTopN"), galleryElem.TopN);
        }
        set
        {
            SetValue("SelectTopN", value);
            galleryElem.TopN = value;
        }
    }


    /// <summary>
    /// Gets or sets site name.
    /// </summary>
    public string SiteName
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("SiteName"), galleryElem.SiteName);
        }
        set
        {
            SetValue("SiteName", value);
            galleryElem.SiteName = value;
        }
    }


    /// <summary>
    /// Gets or sets ORDER BY condition.
    /// </summary>
    public string OrderBy
    {
        get
        {
            return ValidationHelper.GetString(GetValue("OrderBy"), galleryElem.OrderBy);
        }
        set
        {
            SetValue("OrderBy", value);
            galleryElem.OrderBy = value;
        }
    }


    /// <summary>
    /// Gets or sets the source filter name.
    /// </summary>
    public string FilterName
    {
        get
        {
            return ValidationHelper.GetString(GetValue("FilterName"), galleryElem.FilterName);
        }
        set
        {
            SetValue("FilterName", value);
            galleryElem.FilterName = value;
        }
    }


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
            galleryElem.CacheItemName = value;
        }
    }


    /// <summary>
    /// Cache dependencies, each cache dependency on a new line.
    /// </summary>
    public override string CacheDependencies
    {
        get
        {
            return ValidationHelper.GetString(base.CacheDependencies, galleryElem.CacheDependencies);
        }
        set
        {
            base.CacheDependencies = value;
            galleryElem.CacheDependencies = value;
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
            galleryElem.CacheMinutes = value;
        }
    }


    /// <summary>
    /// Group GUID (document field GUID) of the grouped attachments.
    /// </summary>
    public Guid AttachmentGroupGUID
    {
        get
        {
            string guidAndText = ValidationHelper.GetString(GetValue("AttachmentGroupGUID"), string.Empty);
            string[] values = guidAndText.Split('|');
            return (values.Length >= 1) ? ValidationHelper.GetGuid(values[0], Guid.Empty) : Guid.Empty;
        }
        set
        {
            SetValue("AttachmentGroupGUID", value);
            galleryElem.AttachmentGroupGUID = value;
        }
    }


    /// <summary>
    /// Culture code, such as en-us.
    /// </summary>
    public string CultureCode
    {
        get
        {
            return ValidationHelper.GetString(GetValue("CultureCode"), galleryElem.CultureCode);
        }
        set
        {
            SetValue("CultureCode", value);
            galleryElem.CultureCode = value;
        }
    }


    /// <summary>
    /// Indicates if the document should be selected eventually from the default culture.
    /// </summary>
    public bool CombineWithDefaultCulture
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("CombineWithDefaultCulture"), true);
        }
        set
        {
            SetValue("CombineWithDefaultCulture", value);
            galleryElem.CombineWithDefaultCulture = value;
        }
    }


    /// <summary>
    /// Gets or sets the alias path.
    /// </summary>
    public string Path
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("Path"), galleryElem.Path);
        }
        set
        {
            SetValue("Path", value);
            galleryElem.Path = value;
        }
    }


    /// <summary>
    /// Allows you to specify whether to check permissions of the current user. If the value is 'false' (default value) no permissions are checked. Otherwise, only nodes for which the user has read permission are displayed.
    /// </summary>
    public bool CheckPermissions
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("CheckPermissions"), galleryElem.CheckPermissions);
        }
        set
        {
            SetValue("CheckPermissions", value);
            galleryElem.CheckPermissions = value;
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
            return DataHelper.GetNotEmpty(GetValue("LightBoxExternalScriptPath"), galleryElem.LightBoxExternalScriptPath);
        }
        set
        {
            SetValue("LightBoxExternalScriptPath", value);
            galleryElem.LightBoxExternalScriptPath = value;
        }
    }


    /// <summary>
    /// Gets or sets the transparency of shadow overlay.
    /// </summary>
    public float LightBoxOverlayOpacity
    {
        get
        {
            return (float)ValidationHelper.GetDoubleSystem(GetValue("LightBoxOverlayOpacity"), galleryElem.LightBoxOverlayOpacity);
        }
        set
        {
            SetValue("LightBoxOverlayOpacity", value);
            galleryElem.LightBoxOverlayOpacity = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether resizing should be animated.
    /// </summary>
    public bool LightBoxAnimate
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("LightBoxAnimate"), galleryElem.LightBoxAnimate);
        }
        set
        {
            SetValue("LightBoxAnimate", value);
            galleryElem.LightBoxAnimate = value;
        }
    }


    /// <summary>
    /// Gets or sets the window width.
    /// </summary>
    public int LightBoxWidth
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("LightBoxWidth"), galleryElem.LightBoxWidth);
        }
        set
        {
            SetValue("LightBoxWidth", value);
            galleryElem.LightBoxWidth = value;
        }
    }


    /// <summary>
    /// Gets or sets the window height.
    /// </summary>
    public int LightBoxHeight
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("LightBoxHeight"), galleryElem.LightBoxHeight);
        }
        set
        {
            SetValue("LightBoxHeight", value);
            galleryElem.LightBoxHeight = value;
        }
    }


    /// <summary>
    /// Gets or sets speed of resizing animations.
    /// </summary>
    public int LightBoxResizeSpeed
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("LightBoxResizeSpeed"), galleryElem.LightBoxResizeSpeed);
        }
        set
        {
            SetValue("LightBoxResizeSpeed", value);
            galleryElem.LightBoxResizeSpeed = value;
        }
    }


    /// <summary>
    /// Gets or sets the border size.
    /// </summary>
    public int LightBoxBorderSize
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("LightBoxBorderSize"), galleryElem.LightBoxBorderSize);
        }

        set
        {
            SetValue("LightBoxBorderSize", value);
            galleryElem.LightBoxBorderSize = value;
        }
    }


    /// <summary>
    /// Gets or sets the loading image URL.
    /// </summary>
    public string LightBoxLoadingImg
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("LightBoxLoadingImg"), galleryElem.LightBoxLoadingImg);
        }
        set
        {
            SetValue("LightBoxLoadingImg", value);
            galleryElem.LightBoxLoadingImg = value;
        }
    }


    /// <summary>
    /// Gets or sets the close image URL.
    /// </summary>
    public string LightBoxCloseImg
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("LightBoxCloseImg"), galleryElem.LightBoxCloseImg);
        }
        set
        {
            SetValue("LightBoxCloseImg", value);
            galleryElem.LightBoxCloseImg = value;
        }
    }


    /// <summary>
    /// Gets or sets the previous image URL.
    /// </summary>
    public string LightBoxPrevImg
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("LightBoxPrevImg"), galleryElem.LightBoxPrevImg);
        }
        set
        {
            SetValue("LightBoxPrevImg", value);
            galleryElem.LightBoxPrevImg = value;
        }
    }


    /// <summary>
    /// Gets or sets the next image URL.
    /// </summary>
    public string LightBoxNextImg
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("LightBoxNextImg"), galleryElem.LightBoxNextImg);
        }
        set
        {
            SetValue("LightBoxNextImg", value);
            galleryElem.LightBoxNextImg = value;
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
            galleryElem.LightBoxPermanentNavigation = value;
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
            galleryElem.LightBoxLoadDelay = value;
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
            galleryElem.LightBoxGroup = value;
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
            galleryElem.StopProcessing = value;
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
            galleryElem.StopProcessing = true;
        }
        else
        {
            galleryElem.ControlContext = ControlContext;

            // Basic control properties
            galleryElem.HideControlForZeroRows = HideControlForZeroRows;
            galleryElem.ZeroRowsText = ZeroRowsText;

            // Data source properties
            galleryElem.CombineWithDefaultCulture = CombineWithDefaultCulture;
            galleryElem.CultureCode = CultureCode;
            galleryElem.OrderBy = OrderBy;
            galleryElem.TopN = TopN;
            if (string.IsNullOrEmpty(Path))
            {
                Path = DocumentContext.CurrentAliasPath;
            }
            Path = TreePathUtils.EnsureSingleNodePath(Path);
            galleryElem.Path = Path;
            galleryElem.SiteName = SiteName;
            galleryElem.WhereCondition = WhereCondition;
            galleryElem.AttachmentGroupGUID = AttachmentGroupGUID;
            galleryElem.FilterName = FilterName;

            // System properties
            galleryElem.CacheItemName = CacheItemName;
            galleryElem.CacheDependencies = CacheDependencies;
            galleryElem.CacheMinutes = CacheMinutes;
            galleryElem.CheckPermissions = CheckPermissions;
            if (ParentZone != null)
            {
                galleryElem.CheckCollision = ParentZone.WebPartManagementRequired;
            }
            else
            {
                galleryElem.CheckCollision = PortalContext.IsDesignMode(PortalContext.ViewMode);
            }

            // UniPager properties
            galleryElem.PageSize = PageSize;
            galleryElem.GroupSize = GroupSize;
            galleryElem.QueryStringKey = QueryStringKey;
            galleryElem.DisplayFirstLastAutomatically = DisplayFirstLastAutomatically;
            galleryElem.DisplayPreviousNextAutomatically = DisplayPreviousNextAutomatically;
            galleryElem.HidePagerForSinglePage = HidePagerForSinglePage;

            switch (PagingMode.ToLowerCSafe())
            {
                case "postback":
                    galleryElem.PagingMode = UniPagerMode.PostBack;
                    break;
                default:
                    galleryElem.PagingMode = UniPagerMode.Querystring;
                    break;
            }


            #region "UniPager template properties"

            // UniPager template properties
            galleryElem.PagesTemplate = PagesTemplate;
            galleryElem.CurrentPageTemplate = CurrentPageTemplate;
            galleryElem.SeparatorTemplate = SeparatorTemplate;
            galleryElem.FirstPageTemplate = FirstPageTemplate;
            galleryElem.LastPageTemplate = LastPageTemplate;
            galleryElem.PreviousPageTemplate = PreviousPageTemplate;
            galleryElem.NextPageTemplate = NextPageTemplate;
            galleryElem.PreviousGroupTemplate = PreviousGroupTemplate;
            galleryElem.NextGroupTemplate = NextGroupTemplate;
            galleryElem.LayoutTemplate = LayoutTemplate;

            #endregion


            #region "Lightbox properties"

            galleryElem.LightBoxLoadDelay = LightBoxLoadDelay;
            galleryElem.LightBoxPermanentNavigation = LightBoxPermanentNavigation;
            galleryElem.LightBoxNextImg = LightBoxNextImg;
            galleryElem.LightBoxPrevImg = LightBoxPrevImg;
            galleryElem.LightBoxCloseImg = LightBoxCloseImg;
            galleryElem.LightBoxLoadingImg = LightBoxLoadingImg;
            galleryElem.LightBoxBorderSize = LightBoxBorderSize;
            galleryElem.LightBoxResizeSpeed = LightBoxResizeSpeed;
            galleryElem.LightBoxHeight = LightBoxHeight;
            galleryElem.LightBoxWidth = LightBoxWidth;
            galleryElem.LightBoxAnimate = LightBoxAnimate;
            galleryElem.LightBoxOverlayOpacity = LightBoxOverlayOpacity;
            galleryElem.LightBoxExternalScriptPath = LightBoxExternalScriptPath;
            galleryElem.LightBoxGroup = LightBoxGroup;

            #endregion


            // Transformation properties
            galleryElem.TransformationName = TransformationName;
            galleryElem.AlternatingTransformationName = AlternatingTransformationName;
            galleryElem.SelectedItemTransformationName = SelectedItemTransformationName;
            galleryElem.FooterTransformationName = FooterTransformationName;
            galleryElem.HeaderTransformationName = HeaderTransformationName;
            galleryElem.SeparatorTransformationName = SeparatorTransformationName;
        }
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
    /// Reload data.
    /// </summary>
    public override void ReloadData()
    {
        base.ReloadData();
        SetupControl();
    }


    /// <summary>
    /// Clears cache.
    /// </summary>
    public override void ClearCache()
    {
        galleryElem.ClearCache();
    }


    /// <summary>
    /// OnPrerender override (Set visibility).
    /// </summary>
    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        Visible = (!HideControlForZeroRows || galleryElem.HasData) && !galleryElem.StopProcessing;
    }

    #endregion
}