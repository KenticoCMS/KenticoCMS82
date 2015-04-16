using System;

using CMS.DataEngine;
using CMS.Ecommerce;
using CMS.Helpers;
using CMS.UIControls;

public partial class CMSModules_Ecommerce_FormControls_PaymentSelector : SiteSeparatedObjectSelector
{
    #region "Variables"

    private int mShippingOptionId;
    private ShippingOptionInfo mShippingOption;

    #endregion


    #region "Properties"

    /// <summary>
    ///  If true, selected value is PaymentName, if false, selected value is PaymentOptionID.
    /// </summary>
    public override bool UseNameForSelection
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("UsePaymentNameForSelection"), base.UseNameForSelection);
        }
        set
        {
            SetValue("UsePaymentNameForSelection", value);
            base.UseNameForSelection = value;
        }
    }


    /// <summary>
    /// Gets a column name for shipping option ID column.
    /// </summary>
    public string ShippingOptionIDColumnName
    {
        get
        {
            return GetValue("ShippingOptionIDColumnName", String.Empty);
        }
        set
        {
            SetValue("ShippingOptionIDColumnName", value);
        }
    }


    /// <summary>
    /// Shipping option ID.
    /// </summary>
    public int ShippingOptionID
    {
        get
        {
            if ((mShippingOptionId == 0) && (mShippingOption != null))
            {
                mShippingOptionId = ShippingOption.ShippingOptionID;
            }

            return mShippingOptionId;
        }
        set
        {
            mShippingOptionId = value;

            mShippingOption = null;
            SiteID = -1;
        }
    }


    /// <summary>
    /// Shipping option info object.
    /// </summary>
    public ShippingOptionInfo ShippingOption
    {
        get
        {
            if ((mShippingOption == null) && (mShippingOptionId != 0))
            {
                mShippingOption = ShippingOptionInfoProvider.GetShippingOptionInfo(ShippingOptionID);
            }

            return mShippingOption;
        }
        set
        {
            mShippingOption = value;

            mShippingOptionId = 0;
            SiteID = -1;
        }
    }


    /// <summary>
    /// Allows to access selector object.
    /// </summary>
    public override UniSelector UniSelector
    {
        get
        {
            return uniSelector;
        }
    }


    /// <summary>
    /// Allows to display payment methods only for specified site id. Use 0 for global methods. Default value is current site id.
    /// </summary>
    public override int SiteID
    {
        get
        {
            // If shipping option given use its site id
            if ((ShippingOption != null) && (base.SiteID != ShippingOption.ShippingOptionSiteID))
            {
                base.SiteID = ShippingOption.ShippingOptionSiteID;
            }

            return base.SiteID;
        }
        set
        {
            base.SiteID = value;
        }
    }


    /// <summary>
    /// Indicates if only payment options that are allowed to be used without shipping are displayed.
    /// </summary>
    public bool DisplayOnlyAllowedIfNoShipping
    {
        get;
        set;
    }

    #endregion


    #region "Life cycle"

    /// <summary>
    /// Sets up ShippingOptionIDColumnName property if shipping option id column name is configured.
    /// </summary>
    /// <param name="e"></param>
    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        if (!String.IsNullOrEmpty(ShippingOptionIDColumnName))
        {
            var shippingID = (Form != null) ? ValidationHelper.GetInteger(Form.Data.GetValue(ShippingOptionIDColumnName), 0) : 0;
            DisplayOnlyAllowedIfNoShipping = (shippingID == 0);
            ShippingOptionID = shippingID;
        }
    }
    
    #endregion


    #region "Methods"

    /// <summary>
    /// Convert given payment option name to its ID for specified site.
    /// </summary>
    /// <param name="name">Name of the payment option to be converted.</param>
    /// <param name="siteName">Name of the site of the payment option.</param>
    protected override int GetID(string name, string siteName)
    {
        var payment = PaymentOptionInfoProvider.GetPaymentOptionInfo(name, siteName);

        return (payment != null) ? payment.PaymentOptionID : 0;
    }


    /// <summary>
    /// Appends where condition filtering only payments marked with PaymentOptionAllowIfNoShipping flag when requested.
    /// </summary>
    /// <param name="where">Original where condition.</param>
    protected override string AppendExclusiveWhere(string where)
    {
        // Filter payment methods usable with shipping option if specified using ShippingOptionID/ShippingOption properties
        if (ShippingOptionID > 0)
        {
            where = SqlHelper.AddWhereCondition(where, "PaymentOptionID IN (SELECT PaymentOptionID FROM COM_PaymentShipping WHERE ShippingOptionID = " + ShippingOptionID + ")");
        }

        // Filter out only payment options that are allowed to be used without shipping
        if (DisplayOnlyAllowedIfNoShipping)
        {
            where = SqlHelper.AddWhereCondition(where, "PaymentOptionAllowIfNoShipping = 1");
        }

        return base.AppendExclusiveWhere(where);
    }

    #endregion
}