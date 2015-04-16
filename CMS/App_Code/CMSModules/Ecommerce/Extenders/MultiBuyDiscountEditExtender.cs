using System;
using System.Data;
using System.Linq;
using System.Web.UI.WebControls;

using CMS;
using CMS.Base;
using CMS.DataEngine;
using CMS.Ecommerce;
using CMS.ExtendedControls;
using CMS.FormControls;
using CMS.Helpers;
using CMS.PortalEngine;


[assembly: RegisterCustomClass("MultiBuyDiscountEditExtender", typeof(MultiBuyDiscountEditExtender))]

/// <summary>
/// Extender for MultiBuy Discount new/edit page
/// </summary>
public class MultiBuyDiscountEditExtender : ControlExtender<UIForm>
{
    #region "Constants"

    private const string REDIRECTION_ELEMENT = "EditBuyXGetY";
    private const string COUPON_CODE_ELEMENT = "MultiBuyCouponsCodes";

    #endregion


    #region "Variables"

    string mSwapBuyType = string.Empty;
    private HiddenField mUsesCouponsDefaultValue;
    private bool? mUsesCouponsChecked;
    private bool mRedirectAfterNewDiscountCreated;

    #endregion


    #region "Properties"

    /// <summary>
    /// Returns edited discount info object.
    /// </summary>
    private MultiBuyDiscountInfo Discount
    {
        get
        {
            return Control.EditedObject as MultiBuyDiscountInfo;
        }
    }


    /// <summary>
    /// Remembers original value of uses coupon check box.
    /// </summary>
    private HiddenField UsesCouponsDefaultValue
    {
        get
        {
            if (mUsesCouponsDefaultValue == null)
            {
                mUsesCouponsDefaultValue = new HiddenField { ID = "usesCouponsDefaultValue" };
            }

            return mUsesCouponsDefaultValue;
        }
    }


    /// <summary>
    /// Returns original value of Uses coupons checkbox.
    /// </summary>
    private bool UsesCouponsChecked
    {
        get
        {
            if (mUsesCouponsChecked == null)
            {
                mUsesCouponsChecked = ValidationHelper.GetBoolean(UsesCouponsDefaultValue.Value, true);
            }

            return mUsesCouponsChecked.Value;
        }
    }


    /// <summary>
    /// Returns current value of uses coupons check box.
    /// </summary>
    private bool UsesCouponsCheckedByUser
    {
        get
        {
            return ValidationHelper.GetBoolean(Control.FieldControls["MultiBuyDiscountUsesCoupons"].Value, false);
        }
    }


    /// <summary>
    /// Indicates whether status of "discount uses coupons" field was changed from disabled to enabled.
    /// </summary>
    private bool RedirectionEnabled
    {
        get
        {
            return (!UsesCouponsChecked && UsesCouponsCheckedByUser);
        }
    }


    /// <summary>
    /// Indicates whether status of "discount uses coupons" field was changed from enabled to disabled.
    /// </summary>
    private bool CouponCodesUnchecked
    {
        get
        {
            return (UsesCouponsChecked && !UsesCouponsCheckedByUser);
        }
    }

    #endregion


    #region "Page events"

    public override void OnInit()
    {
        if (Control != null)
        {
            Control.Page.Load += Page_Load;
            Control.PreRender += Control_PreRender;
            Control.OnBeforeSave += Control_OnBeforeSave;
            Control.OnAfterSave += Control_OnAfterSave;
            Control.OnBeforeRedirect += Control_OnBeforeRedirect;
        }
    }


    /// <summary>
    /// Set up dummy form fields.
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Discount == null)
        {
            return;
        }

        if (!RequestHelper.IsPostBack())
        {
            Control.FieldControls["BuyDepartmentOrProduct"].Value = Discount.UseDepartments ? "departments" : "products";
            Control.FieldControls["GetSpecificOrCheapestUnit"].Value = (Discount.MultiBuyDiscountApplyToSKUID > 0) ? "specificUnit" : "cheapestUnit";
        }

        // Remember if discount uses coupons (remember value stored in the database);
        if (String.IsNullOrEmpty(UsesCouponsDefaultValue.Value))
        {
            // Insert value to the form to remember original checkbox value
            UsesCouponsDefaultValue.Value = ValidationHelper.GetString(Control.Data.GetValue("MultiBuyDiscountUsesCoupons"), "").ToLower();
            Panel pnlHidden = new Panel();
            pnlHidden.ID = "pnlHidden";
            pnlHidden.CssClass = "discountUsesCouponsValue";
            pnlHidden.Controls.Add(UsesCouponsDefaultValue);
            pnlHidden.Attributes["style"] = "display: none";

            Control.Page.Form.Controls.Add(pnlHidden);
        }

        // Register script hiding redirection message
        ScriptHelper.RegisterModule(Control, "CMS.Ecommerce/Discounts");
    }


    /// <summary>
    /// Displays/hides redirection message if validation failed.
    /// </summary>
    protected void Control_PreRender(object sender, EventArgs e)
    {
        if (RedirectionEnabled)
        {
            ScriptHelper.RegisterStartupScript(Control.Page, typeof(string), "DisplayMessage", "$cmsj('#CouponsInfoLabel').show();", true);
        }
    }


    /// <summary>
    /// Clears MultiBuyDiscountApplyToSKUID (GET Y product) if discount is configured to get cheapest product from set.
    /// Sets a flag to determine whether discount type was switched.
    /// </summary>
    protected void Control_OnBeforeSave(object sender, EventArgs e)
    {
        if (Discount == null)
        {
            return;
        }

        if (string.Equals(Control.FieldControls["GetSpecificOrCheapestUnit"].Value, "cheapestUnit"))
        {
            Discount.SetValue("MultiBuyDiscountApplyToSKUID", null);
        }

        // Only if editing an existing discount
        if (Discount.MultiBuyDiscountID != 0)
        {
            // Set old value of Buy type field
            if (Discount.UseDepartments && "products".Equals(Control.FieldControls["BuyDepartmentOrProduct"].Value))
            {
                // Clear department selector
                Control.FieldControls["BuyDepartmentSet"].Value = null;
                mSwapBuyType = "departmentsToProducts";
            }
            else if (Discount.UseProducts && "departments".Equals(Control.FieldControls["BuyDepartmentOrProduct"].Value))
            {
                // Clear product selector
                Control.FieldControls["BuyProductSet"].Value = null;
                mSwapBuyType = "productsToDepartments";
            }
        }

        // Set priority to new discount
        SetDiscountPriority();

    }


    /// <summary>
    /// Removes bindings after switching the type of MultiBuy Discount.
    /// Clears cache which stores type of application (departments/products).
    /// </summary>
    protected void Control_OnAfterSave(object sender, EventArgs e)
    {
        if (Discount == null)
        {
            return;
        }

        // Buy type has been switched.
        if (!string.Empty.Equals(mSwapBuyType))
        {
            switch (mSwapBuyType)
            {
                case "departmentsToProducts":
                    MultiBuyDiscountDepartmentInfoProvider.RemoveAllDepartments(Discount.MultiBuyDiscountID);
                    MultiBuyDiscountDepartmentInfoProvider.Clear(true);
                    break;
                case "productsToDepartments":
                    MultiBuyDiscountSKUInfoProvider.RemoveAllProducts(Discount.MultiBuyDiscountID);
                    MultiBuyDiscountSKUInfoProvider.Clear(true);
                    break;
            }

            // Invalidate value of cache which stores buy type of MultiBuyDiscountInfo object
            CacheHelper.TouchKey("ecommerce.multibuydiscount|touch|" + Discount.MultiBuyDiscountID);
        }

        // Redirect to coupon codes generation
        if (RedirectionEnabled)
        {
            RedirectIfDiscountIsEdited();
        }
        else if (CouponCodesUnchecked)
        {
            // Update original value
            UsesCouponsDefaultValue.Value = "false";

            // Refresh tabs if "discount uses coupons" field was unchecked and discount don´t have any coupon codes
            if (Discount.CouponCodes.Count == 0)
            {
                RedirectIfDiscountIsEdited();
            }
        }
    }


    /// <summary>
    /// Redirects to the Coupons tab after a new discount is created.
    /// The redirection needs to be performed here because OnAfterSave
    /// of this control is executed _before_ OnAfterSave of MultiObjectBinding
    /// control, so a redirect in OnAfterSave skips correct saving of binding
    /// objects.
    /// </summary>
    private void Control_OnBeforeRedirect(object sender, EventArgs e)
    {
        if (mRedirectAfterNewDiscountCreated)
        {
            URLHelper.Redirect(GenerateRedirectionUrl(REDIRECTION_ELEMENT, Discount.MultiBuyDiscountID, true));
        }
    }

    #endregion


    #region "Helper methods"

    /// <summary>
    /// Redirects to coupon codes generation if discount is edited.
    /// For new discount sets mRedirectAfterNewDiscountCreated variable
    /// to true, what causes the redirection in OnBeforRedirect.
    /// </summary>
    private void RedirectIfDiscountIsEdited()
    {
        string currentElementName = Control.UIContext.UIElement.ElementName.ToLowerCSafe();

        switch (currentElementName.ToLowerCSafe())
        {
            case "newbuyxgetydiscount":
                mRedirectAfterNewDiscountCreated = true;
                break;

            case "editbuyxgetydiscount":
                // Parent element needs to be redirected
                ExecuteParentWindowLocationRedirect(GenerateRedirectionUrl(REDIRECTION_ELEMENT, Discount.MultiBuyDiscountID, true));
                break;
        }
    }


    /// <summary>
    /// Ensures correct redirection of parent element.
    /// </summary>
    /// <param name="redirectUrl">Url to redirect to.</param>
    private void ExecuteParentWindowLocationRedirect(string redirectUrl)
    {
        ScriptHelper.RegisterClientScriptBlock(Control.Page, typeof(string), "OrderCouponRedirect", "parent.window.location='" + redirectUrl + "';", true);
    }


    /// <summary>
    /// Generate redirection url.
    /// </summary>
    /// <param name="elementName">Element where user will be redirected</param>
    /// <param name="objectId">ID of edited object</param>
    /// <param name="saved">Show saved info message if true</param>
    /// <returns></returns>
    private string GenerateRedirectionUrl(string elementName, int objectId, bool saved)
    {
        string url = UIContextHelper.GetElementUrl("cms.ecommerce", elementName, false);

        url = URLHelper.AddParameterToUrl(url, "objectid", objectId.ToString());
        if (RedirectionEnabled)
        {
            url = URLHelper.AddParameterToUrl(url, "tabname", COUPON_CODE_ELEMENT);
        }
        if (saved)
        {
            url = URLHelper.AddParameterToUrl(url, "saved", 1.ToString());
        }

        return url;
    }


    /// <summary>
    /// Sets discount priority according maximal value increased by 10.
    /// Priority is set to new discounts.
    /// </summary>
    private void SetDiscountPriority()
    {
        // Discount is new
        if (Discount.MultiBuyDiscountID < 1)
        {
            DataSet ds = MultiBuyDiscountInfoProvider.GetMultiBuyDiscounts(Discount.MultiBuyDiscountSiteID)
                                                     .Column(new AggregatedColumn(AggregationType.Max, "MultiBuyDiscountPriority").As("MaxPriority"));

            if (!DataHelper.IsEmpty(ds))
            {
                Discount.MultiBuyDiscountPriority = ValidationHelper.GetDouble(ds.Tables[0].Rows[0]["MaxPriority"], 0) + 10;
            }
            else
            {
                Discount.MultiBuyDiscountPriority = 10;
            }
        }
    }

    #endregion
}
