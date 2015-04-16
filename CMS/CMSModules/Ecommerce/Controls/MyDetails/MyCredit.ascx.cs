using System;

using CMS.Ecommerce;
using CMS.Helpers;
using CMS.Globalization;
using CMS.Membership;
using CMS.Base;
using CMS.SiteProvider;
using CMS.UIControls;

public partial class CMSModules_Ecommerce_Controls_MyDetails_MyCredit : CMSAdminControl
{
    #region "Variables"

    private CurrencyInfo mCurrency = null;
    private double rate = 1;

    #endregion


    #region "Properties"

    /// <summary>
    /// Site default currency object.
    /// </summary>
    private CurrencyInfo Currency
    {
        get
        {
            if (mCurrency == null)
            {
                mCurrency = ECommerceContext.CurrentCurrency;
            }

            return mCurrency ?? (mCurrency = CurrencyInfoProvider.GetMainCurrency(SiteContext.CurrentSiteID));
        }
    }


    /// <summary>
    /// Customer ID.
    /// </summary>
    public int CustomerId
    {
        get;
        set;
    }


    /// <summary>
    /// If true, control does not process the data.
    /// </summary>
    public override bool StopProcessing
    {
        get
        {
            return ValidationHelper.GetBoolean(ViewState["StopProcessing"], false);
        }
        set
        {
            ViewState["StopProcessing"] = value;
        }
    }

    #endregion


    #region "Page events"

    /// <summary>
    /// Page load.
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!StopProcessing)
        {
            if (AuthenticationHelper.IsAuthenticated())
            {
                // Get site id of credits main currency
                int creditSiteId = ECommerceHelper.GetSiteID(SiteContext.CurrentSiteID, ECommerceSettings.USE_GLOBAL_CREDIT);

                gridCreditEvents.HideControlForZeroRows = true;
                gridCreditEvents.IsLiveSite = IsLiveSite;
                gridCreditEvents.OnExternalDataBound += gridCreditEvents_OnExternalDataBound;
                gridCreditEvents.OrderBy = "EventDate DESC, EventName ASC";
                gridCreditEvents.WhereCondition = "EventCustomerID = " + CustomerId + " AND ISNULL(EventSiteID, 0) = " + creditSiteId;

                // Get total credit value
                double credit = CreditEventInfoProvider.GetTotalCredit(CustomerId, SiteContext.CurrentSiteID);

                if (Currency != null)
                {
                    // Convert credit to current currency when using one
                    rate = (double)CurrencyConverter.GetExchangeRate(creditSiteId == 0, Currency.CurrencyCode, SiteContext.CurrentSiteID);
                    credit = ExchangeRateInfoProvider.ApplyExchangeRate(credit, rate);
                }

                lblCreditValue.Text = CurrencyInfoProvider.GetFormattedPrice(credit, Currency);
            }
            else
            {
                // Hide if user is not authenticated
                Visible = false;
            }
        }
    }

    #endregion


    #region "Event handlers"

    protected object gridCreditEvents_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        // Show only date part from date-time value
        switch (sourceName.ToLowerCSafe())
        {
            case "eventdate":
                var date = ValidationHelper.GetDateTime(parameter, DateTimeHelper.ZERO_TIME);
                if (date != DateTimeHelper.ZERO_TIME)
                {
                    return TimeZoneHelper.ConvertToUserTimeZone(date, true, MembershipContext.AuthenticatedUser, SiteContext.CurrentSite);
                }

                return String.Empty;

            case "eventcreditchange":
                var credit = ExchangeRateInfoProvider.ApplyExchangeRate(ValidationHelper.GetDouble(parameter, 0), rate);
                return CurrencyInfoProvider.GetFormattedPrice(credit, Currency);
        }

        return parameter;
    }

    #endregion


    #region "Other methods"

    /// <summary>
    /// Overridden SetValue - because of MyAccount webpart.
    /// </summary>
    /// <param name="propertyName">Name of the property to set</param>
    /// <param name="value">Value to set</param>
    public override bool SetValue(string propertyName, object value)
    {
        base.SetValue(propertyName, value);

        switch (propertyName.ToLowerCSafe())
        {
            case "customerid":
                CustomerId = ValidationHelper.GetInteger(value, 0);
                break;
        }

        return true;
    }

    #endregion
}