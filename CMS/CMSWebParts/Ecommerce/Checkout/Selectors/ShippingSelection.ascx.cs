using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

using CMS.Base;
using CMS.Ecommerce;
using CMS.ExtendedControls;
using CMS.Helpers;
using CMS.Membership;

/// <summary>
/// Shipping selector web part
/// </summary>
public partial class CMSWebParts_Ecommerce_Checkout_Selectors_ShippingSelection : CMSCheckoutWebPart
{
    #region "Variables"

    private IList<int> mShippingOptionIds;

    #endregion


    #region "Properties"

    /// <summary>
    /// Returns the IDs of available Shipping options.
    /// </summary>
    protected IList<int> ShippingOptionIds
    {
        get
        {
            if (mShippingOptionIds == null)
            {
                mShippingOptionIds = new List<int>();
                DataSet dsOptions = ShippingOptionInfoProvider.GetShippingOptions(ShoppingCart.ShoppingCartSiteID, true).OrderBy("ShippingOptionDisplayName");

                if (!DataHelper.DataSourceIsEmpty(dsOptions))
                {
                    mShippingOptionIds = DataHelper.GetIntegerValues(dsOptions.Tables[0], "ShippingOptionID");
                }
            }

            return mShippingOptionIds;
        }
    }

    #endregion


    #region "Event handling"

    /// <summary>
    /// OnInit event handler.
    /// </summary>
    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        // Subscribe to the wizard events
        SubscribeToWizardEvents();
    }


    /// <summary>
    /// Load event handler.
    /// </summary>   
    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        SetupControl();
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);
        CMS.ExtendedControls.ControlsHelper.UpdateCurrentPanel(this);
    }


    /// <summary>
    /// Handles the shipping change event. Saving of the selected value into the shopping cart object.
    /// </summary>
    protected void selectShipping_ShippingChange(object sender, EventArgs e)
    {
        // Only if selection is different
        if (ShoppingCart.ShoppingCartShippingOptionID != drpShipping.SelectedID)
        {
            // Set currency for the shopping cart according to the selected value
            ShoppingCart.ShoppingCartShippingOptionID = drpShipping.SelectedID;
            // Raise the change event for all subscribed web parts
            ComponentEvents.RequestEvents.RaiseEvent(sender, e, SHOPPING_CART_CHANGED);
        }
    }


    /// <summary>
    /// Updates the web part according to the new the new shopping cart values.
    /// </summary>
    public void Update(object sender, EventArgs e)
    {
        SetupControl();
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Validates the data.
    /// </summary>
    protected override void ValidateStepData(object sender, StepEventArgs e)
    {
        base.ValidateStepData(sender, e);
        lblError.Visible = false;
        // If shipping selector is visible (needed) check if something is selected
        if (pnlShipping.Visible && (drpShipping.SelectedID == 0))
        {
            e.CancelEvent = true;
            lblError.Text = ResHelper.GetString("com.checkoutprocess.shippingneeded");
            lblError.Visible = true;
        }
    }


    /// <summary>
    /// Saves the wizard step data.
    /// </summary>
    protected override void SaveStepData(object sender, StepEventArgs e)
    {
        base.SaveStepData(sender, e);

        // Clear shipping option if cart does not need shipping
        if (!ShoppingCart.IsShippingNeeded)
        {
            ShoppingCart.ShoppingCartShippingOptionID = 0;
        }
    }


    /// <summary>
    /// Subscribes the web part to the wizard events.
    /// </summary>
    private void SubscribeToWizardEvents()
    {
        ComponentEvents.RequestEvents.RegisterForEvent(SHOPPING_CART_CHANGED, Update);
    }


    /// <summary>
    /// Preselects shipping option.
    /// </summary>
    protected void HandlePreselectShippingOption()
    {
        if ((ShippingOptionIds.Count == 1) && ShoppingCart.IsShippingNeeded)
        {
            // Only one shipping option is available, select it by default
            ShoppingCart.ShoppingCartShippingOptionID = ShippingOptionIds.FirstOrDefault();
        }

        if (ShoppingCart.ShoppingCartShippingOptionID <= 0)
        {
            UserInfo customer = (ShoppingCart.Customer != null) ? ShoppingCart.Customer.CustomerUser : null;
            int shippingOptionId = (customer != null) ? customer.GetUserPreferredShippingOptionID(ShoppingCart.SiteName) : 0;

            if (shippingOptionId > 0)
            {
                ShoppingCart.ShoppingCartShippingOptionID = shippingOptionId;
            }
        }

        drpShipping.Reload();
        drpShipping.SelectedID = ShoppingCart.ShoppingCartShippingOptionID;
    }


    /// <summary>
    /// Setting up the control.
    /// </summary>
    public void SetupControl()
    {
        if (!StopProcessing)
        {
            // Set up empty record text. The macro ResourcePrefix + .empty represents empty record value.
            drpShipping.UniSelector.ResourcePrefix = "com.livesiteselector";

            // Enable post back for the drop-down list and subscribe to the selection change
            drpShipping.AutoPostBack = true;
            drpShipping.Changed += selectShipping_ShippingChange;

            // Set shopping cart. Selector will use it to compute shipping price.
            drpShipping.ShoppingCart = ShoppingCart;

            // Initialize selector if needed
            if (!RequestHelper.IsPostBack() && drpShipping.HasData)
            {
                HandlePreselectShippingOption();
            }
        }
    }

    #endregion
}