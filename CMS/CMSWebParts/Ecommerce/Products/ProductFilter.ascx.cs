using System;

using CMS.Helpers;
using CMS.PortalControls;

public partial class CMSWebParts_Ecommerce_Products_ProductFilter : CMSAbstractWebPart
{
    #region "Properties"

    /// <summary>
    /// Show public status filter.
    /// </summary>
    public bool ShowPublicStatusFilter
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ShowPublicStatusFilter"), filterElem.ShowPublicStatusFilter);
        }
        set
        {
            SetValue("ShowPublicStatusFilter", value);
            filterElem.ShowPublicStatusFilter = value;
        }
    }


    /// <summary>
    /// Show manufacturer filter.
    /// </summary>
    public bool ShowManufacturerFilter
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ShowManufacturerFilter"), filterElem.ShowManufacturerFilter);
        }
        set
        {
            SetValue("ShowManufacturerFilter", value);
            filterElem.ShowManufacturerFilter = value;
        }
    }


    /// <summary>
    /// Show paging filter.
    /// </summary>
    public bool ShowPagingFilter
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ShowPagingFilter"), filterElem.ShowPagingFilter);
        }
        set
        {
            SetValue("ShowPagingFilter", value);
            filterElem.ShowPagingFilter = value;
        }
    }


    /// <summary>
    /// Show stock filter.
    /// </summary>
    public bool ShowStockFilter
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ShowStockFilter"), filterElem.ShowStockFilter);
        }
        set
        {
            SetValue("ShowStockFilter", value);
            filterElem.ShowStockFilter = value;
        }
    }


    /// <summary>
    /// Show sorting filter.
    /// </summary>
    public bool ShowSortingFilter
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ShowSortingFilter"), filterElem.ShowSortingFilter);
        }
        set
        {
            SetValue("ShowSortingFilter", value);
            filterElem.ShowSortingFilter = value;
        }
    }


    /// <summary>
    /// Show search filter.
    /// </summary>
    public bool ShowSearchFilter
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ShowSearchFilter"), filterElem.ShowSearchFilter);
        }
        set
        {
            SetValue("ShowSearchFilter", value);
            filterElem.ShowSearchFilter = value;
        }
    }


    /// <summary>
    /// Paging filter options (values separated by comma).
    /// </summary>
    public string SiteName
    {
        get
        {
            return ValidationHelper.GetString(GetValue("SiteName"), filterElem.SiteName);
        }
        set
        {
            SetValue("SiteName", value);
            filterElem.SiteName = value;
        }
    }


    /// <summary>
    /// Paging filter options (values separated by comma).
    /// </summary>
    public string PagingOptions
    {
        get
        {
            return ValidationHelper.GetString(GetValue("PagingOptions"), filterElem.PagingOptions);
        }
        set
        {
            SetValue("PagingOptions", value);
            filterElem.PagingOptions = value;
        }
    }


    /// <summary>
    /// Filter name.
    /// </summary>
    public string FilterName
    {
        get
        {
            return ValidationHelper.GetString(GetValue("FilterName"), filterElem.FilterName);
        }
        set
        {
            SetValue("FilterName", value);
            filterElem.FilterName = value;
        }
    }


    /// <summary>
    /// Filter by query parameters.
    /// </summary>
    public bool FilterByQuery
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("FilterByQuery"), filterElem.FilterByQuery);
        }
        set
        {
            SetValue("FilterByQuery", value);
            filterElem.FilterByQuery = value;
        }
    }

    #endregion

    /// <summary>
    /// Content loaded event handler.
    /// </summary>
    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        SetupControl();
    }


    /// <summary>
    /// Initializes the control properties.
    /// </summary>
    protected void SetupControl()
    {
        if (StopProcessing)
        {
            // Do nothing
            filterElem.StopProcessing = true;
        }
        else
        {
            filterElem.ShowManufacturerFilter = ShowManufacturerFilter;
            filterElem.ShowPagingFilter = ShowPagingFilter;
            filterElem.ShowPublicStatusFilter = ShowPublicStatusFilter;
            filterElem.ShowSortingFilter = ShowSortingFilter;
            filterElem.ShowStockFilter = ShowStockFilter;
            filterElem.ShowSearchFilter = ShowSearchFilter;
            filterElem.PagingOptions = PagingOptions;
            filterElem.SiteName = SiteName;
            filterElem.FilterName = FilterName;
            filterElem.FilterByQuery = FilterByQuery;
        }
    }


    /// <summary>
    /// Reload data.
    /// </summary>
    public override void ReloadData()
    {
        base.ReloadData();
        SetupControl();
    }
}