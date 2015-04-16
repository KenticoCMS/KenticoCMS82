using System;
using System.Web.UI.WebControls;

using CMS.Controls;
using CMS.Ecommerce;
using CMS.ExtendedControls;
using CMS.Helpers;
using CMS.Base;
using CMS.SiteProvider;
using CMS.DataEngine;

public partial class CMSModules_Ecommerce_Controls_Filters_ProductFilter : CMSAbstractDataFilterControl
{
    #region "Variables"

    private bool mShowPublicStatusFilter = true;
    private bool mShowManufacturerFilter = true;
    private bool mShowStockFilter = true;
    private bool mShowSortingFilter = true;
    private bool mFilterByQuery = true;
    private bool? mUsingGlobalObjects;
    private string mPagingOptions;
    protected CMSButton button;


    #endregion


    #region "Properties"

    /// <summary>
    /// Show public status filter.
    /// </summary>
    public bool ShowPublicStatusFilter
    {
        get
        {
            return mShowPublicStatusFilter;
        }
        set
        {
            mShowPublicStatusFilter = value;
        }
    }


    /// <summary>
    /// Show manufacturer filter.
    /// </summary>
    public bool ShowManufacturerFilter
    {
        get
        {
            return mShowManufacturerFilter;
        }
        set
        {
            mShowManufacturerFilter = value;
        }
    }


    /// <summary>
    /// Show paging filter.
    /// </summary>
    public bool ShowPagingFilter
    {
        get;
        set;
    }


    /// <summary>
    /// Show stock filter.
    /// </summary>
    public bool ShowStockFilter
    {
        get
        {
            return mShowStockFilter;
        }
        set
        {
            mShowStockFilter = value;
        }
    }


    /// <summary>
    /// Show sorting filter.
    /// </summary>
    public bool ShowSortingFilter
    {
        get
        {
            return mShowSortingFilter;
        }
        set
        {
            mShowSortingFilter = value;
        }
    }


    /// <summary>
    /// Show search filter.
    /// </summary>
    public bool ShowSearchFilter
    {
        get;
        set;
    }


    /// <summary>
    /// Paging filter options (values separated by comma).
    /// </summary>
    public string PagingOptions
    {
        get
        {
            return mPagingOptions;
        }
        set
        {
            if (String.IsNullOrEmpty(mPagingOptions))
            {
                mPagingOptions = value;
                FillPagingDropDown();
            }
        }
    }


    /// <summary>
    /// Filter by query parameters.
    /// </summary>
    public bool FilterByQuery
    {
        get
        {
            return mFilterByQuery;
        }
        set
        {
            mFilterByQuery = value;
        }
    }


    /// <summary>
    /// Name of the site for which selectors are to be loaded. Uses current site name when empty or null.
    /// </summary>
    public override string SiteName
    {
        get
        {
            return base.SiteName;
        }
        set
        {
            base.SiteName = DataHelper.GetNotEmpty(value, SiteContext.CurrentSiteName);
            int siteId = SiteInfoProvider.GetSiteID(SiteName);

            // Clear flag
            mUsingGlobalObjects = null;

            // Propagate siteId to selectors
            manufacturerSelector.SiteID = siteId;
            statusSelector.SiteID = siteId;

            // Show global items
            manufacturerSelector.ReflectGlobalProductsUse = UsingGlobalObjects;
            statusSelector.ReflectGlobalProductsUse = UsingGlobalObjects;
        }
    }


    /// <summary>
    /// Returns true if site given by SiteID uses global products.
    /// </summary>
    protected bool UsingGlobalObjects
    {
        get
        {
            // Unknown yet
            if (!mUsingGlobalObjects.HasValue)
            {
                mUsingGlobalObjects = false;
                // Try to figure out from settings
                string siteName = SiteName;
                if (string.IsNullOrEmpty(siteName))
                {
                    siteName = SiteContext.CurrentSiteName;
                }

                mUsingGlobalObjects = ECommerceSettings.AllowGlobalProducts(siteName);
            }

            return mUsingGlobalObjects.Value;
        }
    }

    #endregion


    #region "Page cycle"
    
    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        PageContext.InitComplete += PageHelper_InitComplete;
        PageContext.CurrentPage.LoadComplete += CurrentPage_LoadComplete;
    }
            

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        FilterData();
    }

    #endregion


    #region "Page events"

    protected void PageHelper_InitComplete(object sender, EventArgs e)
    {
        if (StopProcessing)
        {
            statusSelector.StopProcessing = true;
            manufacturerSelector.StopProcessing = true;
            return;
        }

        SetupControl();

        if (!RequestHelper.IsPostBack() && FilterByQuery)
        {
            // Get filter settings from query
            GetFilterPartsFromQueryString();
        }
    }


    protected void CurrentPage_LoadComplete(object sender, EventArgs e)
    {
        // Raise event at the end of load event
        RaiseOnFilterChanged();
    }


    protected void btnFilter_Click(object sender, EventArgs e)
    {
        if (FilterByQuery)
        {
            // Redirect with new query parameters
            URLHelper.Redirect(GetUrlWithFilterQueryParameters());
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Setups the control.
    /// </summary>
    private void SetupControl()
    {
        // Show/Hide manufacturer filter
        if (ShowManufacturerFilter)
        {
            manufacturerSelector.InnerControl.CssClass = "DropDownList";
        }
        else
        {
            lblManufacturer.Visible = false;
            manufacturerSelector.Visible = false;
        }

        // Show/Hide public status filter
        if (ShowPublicStatusFilter)
        {
            statusSelector.InnerControl.CssClass = "DropDownList";
        }
        else
        {
            lblStatus.Visible = false;
            statusSelector.Visible = false;
        }

        // Show/Hide stock filter
        if (ShowStockFilter)
        {
            chkStock.Text = GetString("ecommerce.filter.product.stock");
        }
        else
        {
            chkStock.Visible = false;
        }

        // Show/Hide paging filter
        if (!ShowPagingFilter)
        {
            lblPaging.Visible = false;
            drpPaging.Visible = false;
        }

        // Show/Hide sorting filter
        if (!ShowSortingFilter)
        {
            lblSort.Visible = false;
            drpSort.Visible = false;
        }

        // Show/Hide search filter
        if (!ShowSearchFilter)
        {
            lblSearch.Visible = false;
            txtSearch.Visible = false;
        }

        // Show/Hide filter rows
        bool firstRowVisible = (ShowSearchFilter || ShowPublicStatusFilter || ShowManufacturerFilter || ShowStockFilter);
        bool secondRowVisible = (ShowPagingFilter || ShowSortingFilter);

        plcFirstRow.Visible = firstRowVisible;
        plcSecondRow.Visible = secondRowVisible;
        plcFirstButton.Visible = (firstRowVisible && !secondRowVisible);
        plcSecButton.Visible = secondRowVisible;
        button = secondRowVisible ? btnSecFilter : btnFirstFilter;

        pnlContainer.DefaultButton = button.ID;

        // Section 508 validation
        lblManufacturer.AssociatedControlClientID = manufacturerSelector.ValueElementID;
        lblStatus.AssociatedControlClientID = statusSelector.ValueElementID;
        lblSearch.AssociatedControlClientID = txtSearch.ClientID;
    }


    /// <summary>
    /// Creates filter condition and raise filter change event.
    /// </summary>
    private void FilterData()
    {
        WhereCondition where = new WhereCondition();
        int paging = 0;
        string order = string.Empty;

        // Build where condition according to drop-downs settings
        if (statusSelector.SelectedID > 0)
        {
            where.WhereEquals("SKUPublicStatusID", statusSelector.SelectedID);
        }

        if (manufacturerSelector.SelectedID > 0)
        {
            where.WhereEquals("SKUManufacturerID", manufacturerSelector.SelectedID);
        }

        if (chkStock.Checked)
        {
            where.Where(w => w.WhereNull("SKUTrackInventory")
                              .Or()
                              .WhereGreaterThan("SKUAvailableItems", 0)
                              .Or()
                              .WhereIn("SKUID", new IDQuery<SKUInfo>("SKUParentSKUID").WhereGreaterThan("SKUAvailableItems", 0)));
        }

        if (!string.IsNullOrEmpty(txtSearch.Text))
        {
            where.WhereContains("SKUName", txtSearch.Text);
        }

        // Process drpSort drop-down
        if (ValidationHelper.GetInteger(drpPaging.SelectedValue, 0) > 0)
        {
            paging = ValidationHelper.GetInteger(drpPaging.SelectedValue, 0);
        }

        if (!string.IsNullOrEmpty(drpSort.SelectedValue))
        {
            order = drpSort.SelectedValue;
        }

        // Set where condition
        WhereCondition = where.ToString(true);

        if (paging > 0)
        { 
            // Set paging
            PageSize = paging;
        }

        if (!string.IsNullOrEmpty(order))
        {
            // Set sorting
            OrderBy = GetOrderBy(order);
        }
    }


    /// <summary>
    /// Reads filter configuration from query string.
    /// </summary>
    private void GetFilterPartsFromQueryString()
    {
        int status = QueryHelper.GetInteger("statusid", 0);
        int manufacturer = QueryHelper.GetInteger("manufacturerid", 0);
        bool stock = QueryHelper.GetBoolean("available", false);
        int paging = QueryHelper.GetInteger("pagesize", 0);
        string order = QueryHelper.GetString("order", string.Empty);
        string search = QueryHelper.GetString("search", string.Empty);

        // Set internal status if in query
        if (status > 0)
        {
            statusSelector.SelectedID = status;
        }

        // Set manufacturer if in query
        if (manufacturer > 0)
        {
            manufacturerSelector.SelectedID = manufacturer;
        }

        // Set search if in query
        if (!string.IsNullOrEmpty(search))
        {
            txtSearch.Text = search;
        }

        // Set only in stock if in query
        if (stock)
        {
            chkStock.Checked = true;
        }

        // Set paging if in query
        if (paging > 0)
        {
            if (drpPaging.Items.Contains(drpPaging.Items.FindByValue(paging.ToString())))
            {
                drpPaging.SelectedValue = paging.ToString();
            }
        }

        // Set order if in query
        if (!string.IsNullOrEmpty(order))
        {
            if (drpSort.Items.Contains(drpSort.Items.FindByValue(order)))
            {
                drpSort.SelectedValue = order;
            }
        }
    }


    /// <summary>
    /// Creates url with query parameters representing configuration of filter.
    /// </summary>
    /// <returns></returns>
    private string GetUrlWithFilterQueryParameters()
    {
        // Handle all query parameters
        string url = RequestContext.RawURL;

        // Reset pager
        url = URLHelper.RemoveParameterFromUrl(url, "page");

        url = URLHelper.RemoveParameterFromUrl(url, "statusid");
        if (statusSelector.SelectedID > 0)
        {
            url = URLHelper.AddParameterToUrl(url, "statusid", statusSelector.SelectedID.ToString());
        }

        url = URLHelper.RemoveParameterFromUrl(url, "manufacturerid");
        if (manufacturerSelector.SelectedID > 0)
        {
            url = URLHelper.AddParameterToUrl(url, "manufacturerid", manufacturerSelector.SelectedID.ToString());
        }

        url = URLHelper.RemoveParameterFromUrl(url, "available");
        if (chkStock.Checked)
        {
            url = URLHelper.AddParameterToUrl(url, "available", "1");
        }

        url = URLHelper.RemoveParameterFromUrl(url, "pagesize");
        if (ValidationHelper.GetInteger(drpPaging.SelectedValue, 0) > 0)
        {
            url = URLHelper.AddParameterToUrl(url, "pagesize", drpPaging.SelectedValue);
        }

        url = URLHelper.RemoveParameterFromUrl(url, "order");
        if (drpSort.SelectedValue != string.Empty)
        {
            url = URLHelper.AddParameterToUrl(url, "order", drpSort.SelectedValue);
        }

        url = URLHelper.RemoveParameterFromUrl(url, "search");
        if (!string.IsNullOrEmpty(txtSearch.Text))
        {
            url = URLHelper.AddParameterToUrl(url, "search", txtSearch.Text);
        }

        return url;
    }

    #endregion


    #region "Helper methods"

    /// <summary>
    /// Fill page size filter drop-down with settings from web part configuration.
    /// </summary>
    private void FillPagingDropDown()
    {
        drpPaging.Items.Add(new ListItem(GetString("General.SelectAll"), "0"));

        if (!string.IsNullOrEmpty(PagingOptions))
        {
            // Parse PagingOptions string and fill dropdown
            string[] pageOptions = PagingOptions.Split(',');
            foreach (string pageOption in pageOptions)
            {
                if (pageOption.Trim() != string.Empty)
                {
                    drpPaging.Items.Add(new ListItem(pageOption.Trim()));
                }
            }
        }

        // Hide paging
        if (drpPaging.Items.Count < 1)
        {
            drpPaging.Visible = false;
            lblPaging.Visible = false;
        }
    }


    /// <summary>
    /// Creates order by part of filter condition.
    /// </summary>
    /// <param name="selectedValue">Value of product filter sorting dd list which represents required order. One of "nameasc", "namedesc", "priceasc", "pricedesc"</param>
    private string GetOrderBy(string selectedValue)
    {
        if (string.IsNullOrEmpty(selectedValue))
        {
            return null;
        }

        string order = "";

        switch (selectedValue.ToLowerCSafe())
        {
            case "nameasc":
                order = "SKUName";
                break;

            case "namedesc":
                order = "SKUName DESC";
                break;

            case "priceasc":
                order = "SKUPrice";
                break;

            case "pricedesc":
                order = "SKUPrice DESC";
                break;
        }

        return order;
    }

    #endregion
}