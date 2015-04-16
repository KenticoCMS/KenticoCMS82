using System;

using CMS.DataEngine;
using CMS.Ecommerce;
using CMS.Helpers;
using CMS.PortalControls;
using CMS.Base;
using CMS.SiteProvider;

public partial class CMSWebParts_Ecommerce_Donations : CMSAbstractWebPart
{
    #region "Variables"

    private string mDonations = "ALL";
    private bool mDisplayOnlyPaidDonations = true;

    private int mTopN;
    private string mColumns = "COM_SKU.SKUName, COM_OrderItem.OrderItemTotalPriceInMainCurrency, COM_Customer.CustomerFirstName, COM_Customer.CustomerLastName, COM_Customer.CustomerCompany, COM_Order.OrderDate";
    private string mOrderBy;
    private string mWhereCondition;

    private string mTransformationName;
    private string mItemSeparator;

    #endregion


    #region "Properties"

    /// <summary>
    /// Type of donations to be displayed. Possible values: 'ALL' - both public and private donations, 'PUBLIC' - public donations, 'PRIVATE' - private donations.
    /// </summary>
    public string Donations
    {
        get
        {
            return ValidationHelper.GetString(GetValue("Donations"), mDonations);
        }
        set
        {
            SetValue("Donations", value);
            mDonations = value;
        }
    }


    /// <summary>
    /// Indicates if only donations from already paid orders are being displayed.
    /// </summary>
    public bool DisplayOnlyPaidDonations
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("DisplayOnlyPaidDonations"), mDisplayOnlyPaidDonations);
        }
        set
        {
            SetValue("DisplayOnlyPaidDonations", value);
            mDisplayOnlyPaidDonations = value;
        }
    }


    /// <summary>
    /// Number of records to select.
    /// </summary>
    public int TopN
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("TopN"), mTopN);
        }
        set
        {
            SetValue("TopN", value);
            mTopN = value;
        }
    }


    /// <summary>
    /// Columns to select.
    /// </summary>
    public string Columns
    {
        get
        {
            return ValidationHelper.GetString(GetValue("Columns"), mColumns);
        }
        set
        {
            SetValue("Columns", value);
            mColumns = value;
        }
    }


    /// <summary>
    /// Order by expression.
    /// </summary>
    public string OrderBy
    {
        get
        {
            return ValidationHelper.GetString(GetValue("OrderBy"), mOrderBy);
        }
        set
        {
            SetValue("OrderBy", value);
            mOrderBy = value;
        }
    }


    /// <summary>
    /// Where condition.
    /// </summary>
    public string WhereCondition
    {
        get
        {
            return ValidationHelper.GetString(GetValue("WhereCondition"), mWhereCondition);
        }
        set
        {
            SetValue("WhereCondition", value);
            mWhereCondition = value;
        }
    }


    /// <summary>
    /// Transformation name.
    /// </summary>
    public string TransformationName
    {
        get
        {
            return ValidationHelper.GetString(GetValue("TransformationName"), mTransformationName);
        }
        set
        {
            SetValue("TransformationName", value);
            mTransformationName = value;
            repeater.TransformationName = value;
        }
    }


    /// <summary>
    /// Item separator.
    /// </summary>
    public string ItemSeparator
    {
        get
        {
            return ValidationHelper.GetString(GetValue("ItemSeparator"), mItemSeparator);
        }
        set
        {
            SetValue("ItemSeparator", value);
            mItemSeparator = value;
            repeater.ItemSeparator = value;
        }
    }

    #endregion


    #region "Methods"

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
            repeater.StopProcessing = true;
            return;
        }

        // Initialize repeater
        repeater.TransformationName = TransformationName;
        repeater.ItemSeparator = ItemSeparator;
        ReloadRepeaterData();
    }


    /// <summary>
    /// Reloads the control data.
    /// </summary>
    public override void ReloadData()
    {
        base.ReloadData();

        SetupControl();
    }


    /// <summary>
    /// Reloads repeater data.
    /// </summary>
    private void ReloadRepeaterData()
    {
        repeater.DataSource = OrderItemInfoProvider.GetDonations(GetCompleteWhereCondition(), OrderBy, TopN, Columns);
        repeater.DataBind();
    }


    /// <summary>
    /// Returns complete where condition according to current property values.
    /// </summary>
    private string GetCompleteWhereCondition()
    {
        // Get only order items for current site
        var where = SqlHelper.AddWhereCondition(null, "COM_Order.OrderSiteID = " + SiteContext.CurrentSiteID);

        // Get only order items from paid orders
        if (DisplayOnlyPaidDonations)
        {
            where = SqlHelper.AddWhereCondition(where, "COM_Order.OrderIsPaid = 1");
        }

        // Get only public/private donations
        switch (Donations.ToUpperCSafe())
        {
            case "PUBLIC":
                where = SqlHelper.AddWhereCondition(where, "COM_OrderItem.OrderItemIsPrivate = 0");
                break;

            case "PRIVATE":
                where = SqlHelper.AddWhereCondition(where, "COM_OrderItem.OrderItemIsPrivate = 1");
                break;
        }

        // Add additional where condition
        if (!String.IsNullOrEmpty(WhereCondition))
        {
            where = SqlHelper.AddWhereCondition(where, WhereCondition);
        }

        return where;
    }

    #endregion
}