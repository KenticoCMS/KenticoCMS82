using System;
using System.Collections.Generic;
using System.Linq;

using CMS.DataEngine;
using CMS.Ecommerce;
using CMS.ExtendedControls;
using CMS.FormControls;
using CMS.Helpers;
using CMS.SiteProvider;

public partial class CMSModules_Ecommerce_Controls_UI_PriceSelector : FormEngineUserControl
{
    #region "Properties"

    /// <summary>
    /// Gets or sets the enabled state of the control.
    /// </summary>
    public override bool Enabled
    {
        get
        {
            return base.Enabled;
        }
        set
        {
            base.Enabled = value;
            txtPrice.Enabled = value;
        }
    }


    /// <summary>
    /// Gets or sets the price value.
    /// When set, the price value is formatted according to the specified currency.
    /// </summary>
    public override object Value
    {
        get
        {
            if (string.IsNullOrEmpty(txtPrice.Text))
            {
                return null;
            }
            return ValidationHelper.GetDouble(txtPrice.Text, 0);
        }
        set
        {
            if (ValidationHelper.IsDouble(value))
            {
                txtPrice.Text = GetFormattedPrice(ValidationHelper.GetDouble(value, 0));
            }
            else
            {
                txtPrice.Text = null;
            }
        }
    }


    /// <summary>
    /// Gets or sets the price value.
    /// When set, the price value is formatted according to the specified currency.
    /// </summary>
    public double Price
    {
        get
        {
            return ValidationHelper.GetDouble(Value, 0);
        }
        set
        {
            Value = value;
        }
    }


    /// <summary>
    /// Gets a value that indicates if price is being edited for product option.
    /// </summary>
    public bool IsProductOption
    {
        get
        {
            // Try to get the value from SKU form data
            if ((Form != null) && Form.Data.ContainsColumn("SKUOptionCategoryID"))
            {
                return ValidationHelper.GetInteger(Form.Data.GetValue("SKUOptionCategoryID"), 0) > 0;
            }

            return false;
        }
    }


    /// <summary>
    /// Gets or sets the ID of the site specifying which main currency has to be used for the price formatting.
    /// The default value is the current site ID.
    /// </summary>
    public int CurrencySiteID
    {
        get
        {
            if (mCurrencySiteID >= 0)
            {
                return mCurrencySiteID;
            }

            // Try to get the value from SKU form data
            if (Form != null)
            {
                if (Form.Data.ContainsColumn("SKUSiteID"))
                {
                    int siteId = ValidationHelper.GetInteger(Form.Data.GetValue("SKUSiteID"), 0);
                    if (siteId >= 0)
                    {
                        return siteId;
                    }
                }

                // Get ID of the site of edited object
                BaseInfo info = Form.Data as BaseInfo;
                if ((info != null) && (info.TypeInfo.SiteIDColumn != ObjectTypeInfo.COLUMN_NAME_UNKNOWN))
                {
                    return info.Generalized.ObjectSiteID;
                }

                // Get ID of the site from parent of edited object
                UIForm uiForm = Form as UIForm;
                if (uiForm != null)
                {
                    // Get ID of the site of parent object
                    BaseInfo parent = uiForm.ParentObject;
                    if ((parent != null) && (parent.TypeInfo.SiteIDColumn != ObjectTypeInfo.COLUMN_NAME_UNKNOWN))
                    {
                        return parent.Generalized.ObjectSiteID;
                    }
                }
            }

            return SiteContext.CurrentSiteID;
        }
        set
        {
            mCurrencySiteID = value;
            mCurrency = null;
        }
    }
    private int mCurrencySiteID = -1;


    /// <summary>
    /// Gets or sets the currency used for the price formatting.
    /// </summary>
    public CurrencyInfo Currency
    {
        get
        {
            return mCurrency ?? (mCurrency = CurrencyInfoProvider.GetMainCurrency(CurrencySiteID));
        }
        set
        {
            mCurrency = value;
            mCurrencySiteID = value != null ? mCurrency.CurrencySiteID : -1;
        }
    }
    private CurrencyInfo mCurrency = null;


    /// <summary>
    /// Gets or sets the value that indicates if validation errors are to be shown inline by the control itself.
    /// </summary>
    public bool ShowErrors
    {
        get
        {
            return plcMessages.Visible;
        }
        set
        {
            plcMessages.Visible = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether the validation messages should be displayed under the textbox or not.
    /// The default value is false.
    /// </summary>
    public bool ValidatorOnNewLine
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ShowErrorsOnNewLine"), mMessagesOnNewLine);
        }
        set
        {
            SetValue("ShowErrorsOnNewLine", value);
            mMessagesOnNewLine = value;
            Controls.Add(plcMessages);
        }
    }
    private bool mMessagesOnNewLine = false;


    /// <summary>
    /// Gets or sets the message which is displayed by the required field validator of the price textbox.
    /// </summary>
    public string EmptyErrorMessage
    {
        get
        {
            return ValidationHelper.GetString(GetValue("EmptyErrorMessage"), GetString("general.requiresvalue"));
        }
        set
        {
            SetValue("EmptyErrorMessage", value);
        }
    }


    /// <summary>
    /// Gets or sets the message which is displayed by the range field validator of the price textbox.
    /// </summary>
    public string ValidationErrorMessage
    {
        get
        {
            return ValidationHelper.GetString(GetValue("RangeErrorMessage"), GetString("com.invalidprice"));
        }
        set
        {
            SetValue("RangeErrorMessage", value);
        }
    }


    /// <summary>
    /// Indicates if the currency code is displayed next to the price textbox.
    /// The default value is true.
    /// </summary>
    public bool ShowCurrencyCode
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ShowCurrencyCode"), lblCurrencyCode.Visible);
        }
        set
        {
            SetValue("ShowCurrencyCode", value);
            lblCurrencyCode.Visible = value;
        }
    }


    /// <summary>
    /// Indicates if zero price is considered to be a valid value.
    /// The default value is true.
    /// </summary>
    public bool AllowZero
    {
        get
        {
            bool allowZero = ValidationHelper.GetBoolean(ViewState["AllowZero"], true);
            return ValidationHelper.GetBoolean(GetValue("AllowZero"), allowZero);
        }
        set
        {
            SetValue("AllowZero", value);
            ViewState["AllowZero"] = value;
        }
    }


    /// <summary>
    /// Indicates if zero price is considered to be a valid value.
    /// The default value is false.
    /// </summary>
    public bool AllowNegative
    {
        get
        {
            bool allowNegative = ValidationHelper.GetBoolean(ViewState["AllowNegative"], false);
            return ValidationHelper.GetBoolean(GetValue("AllowNegative"), allowNegative);
        }
        set
        {
            SetValue("AllowNegative", value);
            ViewState["AllowNegative"] = value;
        }
    }


    /// <summary>
    /// Indicates if empty price is considered to be a valid value.
    /// The default value is false.
    /// </summary>
    public bool AllowEmpty
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("AllowEmpty"), mAllowEmpty);
        }
        set
        {
            SetValue("AllowEmpty", value);
            mAllowEmpty = value;
        }
    }
    private bool mAllowEmpty = false;


    /// <summary>
    /// Indicates if price should be formatted.
    /// The default value is false.
    /// </summary>
    public bool FormattedPrice
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("FormattedPrice"), false);
        }
        set
        {
            SetValue("FormattedPrice", value);
        }
    }


    /// <summary>
    /// Indicates if value is formatted as integer in case it is a whole number.
    /// The default is false.
    /// </summary>
    public bool FormatValueAsInteger
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("FormatValueAsInteger"), mFormatValueAsInteger);
        }
        set
        {
            SetValue("FormatValueAsInteger", value);
            mFormatValueAsInteger = value;
        }
    }
    private bool mFormatValueAsInteger = false;


    /// <summary>
    /// Gets the messages placeholder.
    /// </summary>
    public override MessagesPlaceHolder MessagesPlaceHolder
    {
        get
        {
            return plcMessages;
        }
    }

    #endregion


    #region "Lifecycle methods"

    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        lblCurrencyCode.Visible = ShowCurrencyCode;
        lblCurrencyCode.Text = Currency != null ? string.Format("{0}", HTMLHelper.HTMLEncode(Currency.CurrencyCode)) : null;
    }

    #endregion


    #region "Validation methods"

    /// <summary>
    /// Returns true if the price value is valid, otherwise returns false.
    /// </summary>
    public override bool IsValid()
    {
        ErrorMessage = Validate(IsProductOption);
        return string.IsNullOrEmpty(ErrorMessage);
    }


    /// <summary>
    /// Validates the price value and returns an error message if the price value is invalid, otherwise returns an empty string.
    /// </summary>
    public string Validate(bool isProductOption)
    {
        List<string> errors = ValidateInternal(isProductOption).ToList();
        bool isValid = errors.Count == 0;
        if (!isValid)
        {
            ErrorMessage = errors.First();
            ShowError(ErrorMessage);
            return ErrorMessage;
        }
        return string.Empty;
    }


    private IEnumerable<string> ValidateInternal(bool isProductOption)
    {
        string price = txtPrice.Text.Trim();

        // Validate empty value
        if (string.IsNullOrEmpty(price))
        {
            if (AllowEmpty)
            {
                yield break;
            }
            else
            {
                yield return GetString(EmptyErrorMessage);
            }
        }

        if (isProductOption || AllowNegative)
        {
            // The price value can be negative for a product option
            if (!ValidationHelper.IsDouble(price))
            {
                yield return GetString(ValidationErrorMessage);
            }
        }
        else
        {
            // The price value cannot be negative for a basic product
            if (ValidationHelper.GetDouble(price, -1) < 0)
            {
                yield return GetString(ValidationErrorMessage);
            }
        }

        // Validate zero value
        if (!AllowZero && ValidationHelper.GetDouble(price, 0) == 0)
        {
            yield return GetString(ValidationErrorMessage);
        }
    }

    #endregion


    #region "Helper methods"

    /// <summary>
    /// Returns the formatted price value with the number of digits according to the currency significant digits settings.
    /// Formats the price value as an integer if enabled and if the price value is a whole number.
    /// </summary>
    /// <param name="value">The price value to be formatted</param>
    private string GetFormattedPrice(double value)
    {
        // Do not format price if it is not required
        if (!FormattedPrice)
        {
            return ValidationHelper.GetString(value, "0");
        }

        // Format the price value as an integer
        if (FormatValueAsInteger)
        {
            double truncatedValue = Math.Truncate(value);
            if (value == truncatedValue)
            {
                return truncatedValue.ToString("0");
            }
        }

        // Format the price value according to the currency
        if (Currency != null)
        {
            int digits = Currency.CurrencyRoundTo;
            string format = "0." + new string('0', digits);
            return value.ToString(format);
        }

        return value.ToString();
    }

    #endregion
}
