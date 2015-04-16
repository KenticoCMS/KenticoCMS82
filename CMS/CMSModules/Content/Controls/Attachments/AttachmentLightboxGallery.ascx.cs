using System;
using System.Text;
using System.Web.UI;

using CMS.Controls;
using CMS.Helpers;
using CMS.IO;
using CMS.MacroEngine;
using CMS.Base;
using CMS.DocumentEngine;
using CMS.UIControls;
using CMS.DataEngine;

public partial class CMSModules_Content_Controls_Attachments_AttachmentLightboxGallery : CMSUserControl
{
    #region "Variables"

    private string mPath = null;

    #endregion


    #region "LightBox properties"

    /// <summary>
    /// Gets or sets the external script path.
    /// </summary>
    public string LightBoxExternalScriptPath
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("LightBoxExternalScriptPath"), extGalleryLightbox.LightBoxExternalScriptPath);
        }
        set
        {
            SetValue("LightBoxExternalScriptPath", value);
            extGalleryLightbox.LightBoxExternalScriptPath = value;
        }
    }


    /// <summary>
    /// Gets or sets the transparency of shadow overlay.
    /// </summary>
    public float LightBoxOverlayOpacity
    {
        get
        {
            return (float)ValidationHelper.GetDouble(GetValue("LightBoxOverlayOpacity"), extGalleryLightbox.LightBoxOverlayOpacity);
        }
        set
        {
            SetValue("LightBoxOverlayOpacity", value);
            extGalleryLightbox.LightBoxOverlayOpacity = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether resizing should be animated.
    /// </summary>
    public bool LightBoxAnimate
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("LightBoxAnimate"), extGalleryLightbox.LightBoxAnimate);
        }
        set
        {
            SetValue("LightBoxAnimate", value);
            extGalleryLightbox.LightBoxAnimate = value;
        }
    }


    /// <summary>
    /// Gets or sets the window width.
    /// </summary>
    public int LightBoxWidth
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("LightBoxWidth"), extGalleryLightbox.LightBoxWidth);
        }
        set
        {
            SetValue("LightBoxWidth", value);
            extGalleryLightbox.LightBoxWidth = value;
        }
    }


    /// <summary>
    /// Gets or sets the window height.
    /// </summary>
    public int LightBoxHeight
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("LightBoxHeight"), extGalleryLightbox.LightBoxHeight);
        }
        set
        {
            SetValue("LightBoxHeight", value);
            extGalleryLightbox.LightBoxHeight = value;
        }
    }


    /// <summary>
    /// Gets or sets speed of resizing animations.
    /// </summary>
    public int LightBoxResizeSpeed
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("LightBoxResizeSpeed"), extGalleryLightbox.LightBoxResizeSpeed);
        }
        set
        {
            SetValue("LightBoxResizeSpeed", value);
            extGalleryLightbox.LightBoxResizeSpeed = value;
        }
    }


    /// <summary>
    /// Gets or sets the border size.
    /// </summary>
    public int LightBoxBorderSize
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("LightBoxBorderSize"), extGalleryLightbox.LightBoxBorderSize);
        }

        set
        {
            SetValue("LightBoxBorderSize", value);
            extGalleryLightbox.LightBoxBorderSize = value;
        }
    }


    /// <summary>
    /// Gets or sets the loading image URL.
    /// </summary>
    public string LightBoxLoadingImg
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("LightBoxLoadingImg"), extGalleryLightbox.LightBoxLoadingImg);
        }
        set
        {
            SetValue("LightBoxLoadingImg", value);
            extGalleryLightbox.LightBoxLoadingImg = value;
        }
    }


    /// <summary>
    /// Gets or sets the close image URL.
    /// </summary>
    public string LightBoxCloseImg
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("LightBoxCloseImg"), extGalleryLightbox.LightBoxCloseImg);
        }
        set
        {
            SetValue("LightBoxCloseImg", value);
            extGalleryLightbox.LightBoxCloseImg = value;
        }
    }


    /// <summary>
    /// Gets or sets the previous image URL.
    /// </summary>
    public string LightBoxPrevImg
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("LightBoxPrevImg"), extGalleryLightbox.LightBoxPrevImg);
        }
        set
        {
            SetValue("LightBoxPrevImg", value);
            extGalleryLightbox.LightBoxPrevImg = value;
        }
    }


    /// <summary>
    /// Gets or sets the next image URL.
    /// </summary>
    public string LightBoxNextImg
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("LightBoxNextImg"), extGalleryLightbox.LightBoxNextImg);
        }
        set
        {
            SetValue("LightBoxNextImg", value);
            extGalleryLightbox.LightBoxNextImg = value;
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
            extGalleryLightbox.LightBoxPermanentNavigation = value;
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
            extGalleryLightbox.LightBoxLoadDelay = value;
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
            return DataHelper.GetNotEmpty(GetValue("LightBoxGroup"), extGalleryLightbox.LightBoxGroup);
        }
        set
        {
            SetValue("LightBoxGroup", value);
            extGalleryLightbox.LightBoxGroup = value;
        }
    }


    /// <summary>
    /// Determines whether to check JavaScript collisions.
    /// </summary>
    public bool CheckCollision
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("CheckCollision"), false);
        }
        set
        {
            SetValue("CheckCollision", value);
            extGalleryLightbox.CheckCollision = value;
        }
    }

    #endregion


    #region "Basic repeater properties"

    /// <summary>
    /// Gets or sets AlternatingTemplate property.
    /// </summary>
    public string AlternatingTransformationName
    {
        get
        {
            return ValidationHelper.GetString(GetValue("AlternatingTransformationName"), string.Empty);
        }
        set
        {
            SetValue("AlternatingTransformationName", value);
        }
    }


    /// <summary>
    /// Gets or sets FooterTemplate property.
    /// </summary>
    public string FooterTransformationName
    {
        get
        {
            return ValidationHelper.GetString(GetValue("FooterTransformationName"), string.Empty);
        }
        set
        {
            SetValue("FooterTransformationName", value);
        }
    }


    /// <summary>
    /// Gets or sets HeaderTemplate property.
    /// </summary>
    public string HeaderTransformationName
    {
        get
        {
            return ValidationHelper.GetString(GetValue("HeaderTransformationName"), string.Empty);
        }
        set
        {
            SetValue("HeaderTransformationName", value);
        }
    }


    /// <summary>
    /// Gets or sets ItemTemplate property.
    /// </summary>
    public string TransformationName
    {
        get
        {
            return ValidationHelper.GetString(GetValue("TransformationName"), string.Empty);
        }
        set
        {
            SetValue("TransformationName", value);
        }
    }


    /// <summary>
    /// Gets or sets SelectedItemTransformationName property.
    /// </summary>
    public string SelectedItemTransformationName
    {
        get
        {
            return ValidationHelper.GetString(GetValue("SelectedItemTransformationName"), string.Empty);
        }
        set
        {
            SetValue("SelectedItemTransformationName", value);
        }
    }


    /// <summary>
    /// Gets or sets SeparatorTemplate property.
    /// </summary>
    public string SeparatorTransformationName
    {
        get
        {
            return ValidationHelper.GetString(GetValue("SeparatorTransformationName"), string.Empty);
        }
        set
        {
            SetValue("SeparatorTransformationName", value);
        }
    }


    /// <summary>
    /// Gets or sets HideControlForZeroRows property.
    /// </summary>
    public bool HideControlForZeroRows
    {
        get
        {
            return ucAttachments.HideControlForZeroRows;
        }
        set
        {
            ucAttachments.HideControlForZeroRows = value;
        }
    }


    /// <summary>
    /// Gets or sets ZeroRowsText property.
    /// </summary>
    public string ZeroRowsText
    {
        get
        {
            return ucAttachments.ZeroRowsText;
        }
        set
        {
            ucAttachments.ZeroRowsText = value;
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
            return ucAttachments.HidePagerForSinglePage;
        }
        set
        {
            ucAttachments.HidePagerForSinglePage = value;
        }
    }


    /// <summary>
    /// Gets or sets the number of records to display on a page.
    /// </summary>
    public int PageSize
    {
        get
        {
            return ucAttachments.PageSize;
        }
        set
        {
            ucAttachments.PageSize = value;
        }
    }


    /// <summary>
    /// Gets or sets the number of pages displayed for current page range.
    /// </summary>
    public int GroupSize
    {
        get
        {
            return ucAttachments.GroupSize;
        }
        set
        {
            ucAttachments.GroupSize = value;
        }
    }


    /// <summary>
    /// Gets or sets the pager mode.
    /// </summary>
    public UniPagerMode PagingMode
    {
        get
        {
            return ucAttachments.PagingMode;
        }
        set
        {
            ucAttachments.PagingMode = value;
        }
    }


    /// <summary>
    /// Gets or sets the querysting parameter.
    /// </summary>
    public string QueryStringKey
    {
        get
        {
            return ucAttachments.QueryStringKey;
        }
        set
        {
            ucAttachments.QueryStringKey = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether first and last item template are displayed dynamically based on current view.
    /// </summary>
    public bool DisplayFirstLastAutomatically
    {
        get
        {
            return ucAttachments.DisplayFirstLastAutomatically;
        }
        set
        {
            ucAttachments.DisplayFirstLastAutomatically = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether first and last item template are displayed dynamically based on current view.
    /// </summary>
    public bool DisplayPreviousNextAutomatically
    {
        get
        {
            return ucAttachments.DisplayPreviousNextAutomatically;
        }
        set
        {
            ucAttachments.DisplayPreviousNextAutomatically = value;
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
            return ucAttachments.PagesTemplate;
        }
        set
        {
            ucAttachments.PagesTemplate = value;
        }
    }


    /// <summary>
    /// Gets or sets the current page template.
    /// </summary>
    public string CurrentPageTemplate
    {
        get
        {
            return ucAttachments.CurrentPageTemplate;
        }
        set
        {
            ucAttachments.CurrentPageTemplate = value;
        }
    }


    /// <summary>
    /// Gets or sets the separator template.
    /// </summary>
    public string SeparatorTemplate
    {
        get
        {
            return ucAttachments.SeparatorTemplate;
        }
        set
        {
            ucAttachments.SeparatorTemplate = value;
        }
    }


    /// <summary>
    /// Gets or sets the first page template.
    /// </summary>
    public string FirstPageTemplate
    {
        get
        {
            return ucAttachments.FirstPageTemplate;
        }
        set
        {
            ucAttachments.FirstPageTemplate = value;
        }
    }


    /// <summary>
    /// Gets or sets the last page template.
    /// </summary>
    public string LastPageTemplate
    {
        get
        {
            return ucAttachments.LastPageTemplate;
        }
        set
        {
            ucAttachments.LastPageTemplate = value;
        }
    }


    /// <summary>
    /// Gets or sets the previous page template.
    /// </summary>
    public string PreviousPageTemplate
    {
        get
        {
            return ucAttachments.PreviousPageTemplate;
        }
        set
        {
            ucAttachments.PreviousPageTemplate = value;
        }
    }


    /// <summary>
    /// Gets or sets the next page template.
    /// </summary>
    public string NextPageTemplate
    {
        get
        {
            return ucAttachments.NextPageTemplate;
        }
        set
        {
            ucAttachments.NextPageTemplate = value;
        }
    }


    /// <summary>
    /// Gets or sets the previous group template.
    /// </summary>
    public string PreviousGroupTemplate
    {
        get
        {
            return ucAttachments.PreviousGroupTemplate;
        }
        set
        {
            ucAttachments.PreviousGroupTemplate = value;
        }
    }


    /// <summary>
    /// Gets or sets the next group template.
    /// </summary>
    public string NextGroupTemplate
    {
        get
        {
            return ucAttachments.NextGroupTemplate;
        }
        set
        {
            ucAttachments.NextGroupTemplate = value;
        }
    }


    /// <summary>
    /// Gets or sets the layout template.
    /// </summary>
    public string LayoutTemplate
    {
        get
        {
            return ucAttachments.LayoutTemplate;
        }
        set
        {
            ucAttachments.LayoutTemplate = value;
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
            return ucAttachments.WhereCondition;
        }
        set
        {
            ucAttachments.WhereCondition = value;
        }
    }


    /// <summary>
    /// Gets or sets top N.
    /// </summary>
    public int TopN
    {
        get
        {
            return ucAttachments.TopN;
        }
        set
        {
            ucAttachments.TopN = value;
        }
    }


    /// <summary>
    /// Gets or sets site name.
    /// </summary>
    public string SiteName
    {
        get
        {
            return ucAttachments.SiteName;
        }
        set
        {
            ucAttachments.SiteName = value;
        }
    }


    /// <summary>
    /// Gets or sets ORDER BY condition.
    /// </summary>
    public string OrderBy
    {
        get
        {
            return ucAttachments.OrderBy;
        }
        set
        {
            ucAttachments.OrderBy = value;
        }
    }


    /// <summary>
    /// Gets or sets the source filter name.
    /// </summary>
    public string FilterName
    {
        get
        {
            return ucAttachments.FilterName;
        }
        set
        {
            ucAttachments.FilterName = value;
        }
    }


    /// <summary>
    /// Gets or sets the cache item name.
    /// </summary>
    public string CacheItemName
    {
        get
        {
            return ucAttachments.CacheItemName;
        }
        set
        {
            ucAttachments.CacheItemName = value;
        }
    }


    /// <summary>
    /// Cache dependencies, each cache dependency on a new line.
    /// </summary>
    public string CacheDependencies
    {
        get
        {
            return ucAttachments.CacheDependencies;
        }
        set
        {
            ucAttachments.CacheDependencies = value;
        }
    }


    /// <summary>
    /// Gets or sets the cache minutes.
    /// </summary>
    public int CacheMinutes
    {
        get
        {
            return ucAttachments.CacheMinutes;
        }
        set
        {
            ucAttachments.CacheMinutes = value;
        }
    }


    /// <summary>
    /// Gets or sets selected columns.
    /// </summary>
    public string SelectedColumns
    {
        get
        {
            return ucAttachments.SelectedColumns;
        }
        set
        {
            ucAttachments.SelectedColumns = value;
        }
    }


    /// <summary>
    /// Tree provider instance used to access data. If no TreeProvider is assigned, a new TreeProvider instance is created.
    /// </summary>
    public TreeProvider TreeProvider
    {
        get
        {
            return ucAttachments.TreeProvider;
        }
        set
        {
            ucAttachments.TreeProvider = value;
        }
    }


    /// <summary>
    /// Indicates whether select also binary content of the attachments.
    /// </summary>
    public bool GetBinary
    {
        get
        {
            return ucAttachments.GetBinary;
        }
        set
        {
            ucAttachments.GetBinary = value;
        }
    }


    /// <summary>
    /// Group GUID (document field GUID) of the grouped attachments.
    /// </summary>
    public Guid AttachmentGroupGUID
    {
        get
        {
            return ucAttachments.AttachmentGroupGUID;
        }
        set
        {
            ucAttachments.AttachmentGroupGUID = value;
        }
    }


    /// <summary>
    /// Form GUID of the temporary attachments.
    /// </summary>
    public Guid AttachmentFormGUID
    {
        get
        {
            return ucAttachments.AttachmentFormGUID;
        }
        set
        {
            ucAttachments.AttachmentFormGUID = value;
        }
    }


    /// <summary>
    /// ID of version history.
    /// </summary>
    public int DocumentVersionHistoryID
    {
        get
        {
            return ucAttachments.DocumentVersionHistoryID;
        }
        set
        {
            ucAttachments.DocumentVersionHistoryID = value;
        }
    }


    /// <summary>
    /// Culture code, such as en-us.
    /// </summary>
    public string CultureCode
    {
        get
        {
            return ucAttachments.CultureCode;
        }
        set
        {
            ucAttachments.CultureCode = value;
        }
    }


    /// <summary>
    /// Indicates if the document should be selected eventually from the default culture.
    /// </summary>
    public bool CombineWithDefaultCulture
    {
        get
        {
            return ucAttachments.CombineWithDefaultCulture;
        }
        set
        {
            ucAttachments.CombineWithDefaultCulture = value;
        }
    }


    /// <summary>
    /// Gets or sets the alias path.
    /// </summary>
    public string Path
    {
        get
        {
            return mPath;
        }
        set
        {
            mPath = value;
            ucAttachments.Path = MacroResolver.ResolveCurrentPath(value);
        }
    }


    /// <summary>
    /// Allows you to specify whether to check permissions of the current user. If the value is 'false' (default value) no permissions are checked. Otherwise, only nodes for which the user has read permission are displayed.
    /// </summary>
    public bool CheckPermissions
    {
        get
        {
            return ucAttachments.CheckPermissions;
        }
        set
        {
            ucAttachments.CheckPermissions = value;
        }
    }


    /// <summary>
    /// Gets whethter datasource is empty or not.
    /// </summary>
    public bool HasData
    {
        get
        {
            return ucAttachments.HasData;
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
            ucAttachments.StopProcessing = value;
            extGalleryLightbox.StopProcessing = value;
        }
    }

    #endregion


    /// <summary>
    /// Sets value override.
    /// </summary>
    /// <param name="propertyName">Property name</param>
    /// <param name="value">Value</param>
    public override bool SetValue(string propertyName, object value)
    {
        if (propertyName != null)
        {
            switch (propertyName.ToLowerCSafe())
            {
                case "sitename":
                    SiteName = Convert.ToString(value);
                    break;

                case "path":
                    Path = Convert.ToString(value);
                    break;

                case "culturecode":
                    CultureCode = Convert.ToString(value);
                    break;

                case "orderby":
                    OrderBy = Convert.ToString(value);
                    break;

                case "pagesize":
                    PageSize = ValidationHelper.GetInteger(value, ucAttachments.PageSize);
                    break;

                case "getbinary":
                    GetBinary = ValidationHelper.GetBoolean(value, ucAttachments.GetBinary);
                    break;

                case "cacheminutes":
                    CacheMinutes = ValidationHelper.GetInteger(value, ucAttachments.CacheMinutes);
                    break;
            }
        }

        return base.SetValue(propertyName, value);
    }


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        extGalleryLightbox.OnItemSelected += extGalleryLightbox_OnItemSelected;

        // Reload data
        if (!RequestHelper.IsCallback() && !StopProcessing)
        {
            ReloadData();
        }
    }


    /// <summary>
    /// OnPrerender override (Set visibility).
    /// </summary>
    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        Visible = ucAttachments.Visible;

        if (HideControlForZeroRows && !HasData)
        {
            Visible = false;
        }

        extGalleryLightbox.StopProcessing = !Visible;
    }


    private string extGalleryLightbox_OnItemSelected(string selectedItem)
    {
        int idValue = ValidationHelper.GetInteger(selectedItem, 0);
        ucAttachments.StopProcessing = false;
        ucAttachments.TransformationName = SelectedItemTransformationName;
        string idColumnName = (DocumentVersionHistoryID == 0) ? "AttachmentID" : "AttachmentHistoryID";
        ucAttachments.WhereCondition = SqlHelper.AddWhereCondition(ucAttachments.WhereCondition, idColumnName + "=" + idValue);
        ucAttachments.ReloadData(true);

        // Render repeater data to String
        StringBuilder stringBuilder = new StringBuilder();
        StringWriter sw = new StringWriter(stringBuilder);
        Html32TextWriter writer = new Html32TextWriter(sw);
        ucAttachments.Repeater.RenderControl(writer);

        return TextHelper.EnsureLineEndings(stringBuilder.ToString(), string.Empty);
    }


    /// <summary>
    /// Reloads the data.
    /// </summary>
    public void ReloadData()
    {
        // Apply transformations if they exist
        ucAttachments.TransformationName = TransformationName;
        ucAttachments.AlternatingItemTransformationName = AlternatingTransformationName;
        ucAttachments.FooterTransformationName = FooterTransformationName;
        ucAttachments.HeaderTransformationName = HeaderTransformationName;
        ucAttachments.SeparatorTransformationName = SeparatorTransformationName;
        ucAttachments.SeparatorTemplate = SeparatorTemplate;
    }


    /// <summary>
    /// Clears control cache.
    /// </summary>
    public void ClearCache()
    {
        ucAttachments.ClearCache();
    }

    #endregion
}