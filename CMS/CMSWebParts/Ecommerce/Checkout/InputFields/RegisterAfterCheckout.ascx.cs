using System;

using CMS.Ecommerce;
using CMS.ExtendedControls;
using CMS.Helpers;
using CMS.PortalEngine;
using CMS.SiteProvider;

public partial class CMSWebParts_Ecommerce_Checkout_InputFields_RegisterAfterCheckout : CMSCheckoutWebPart
{
    // Indicates that webpart settings are used, not e-commerce settings
    private bool useWebpartSettings;


    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        SetupControl();
    }


    protected override void SaveStepData(object sender, StepEventArgs e)
    {
        if (useWebpartSettings)
        {
            ShoppingCart.RegisterAfterCheckout = chkRegister.Checked;
            ShoppingCart.RegisterAfterCheckoutTemplate = GetStringValue("EmailTemplate", "");
        }

        base.SaveStepData(sender, e);
    }


    /// <summary>
    /// Sets up the control.
    /// </summary>
    public void SetupControl()
    {
        if (StopProcessing)
        {
            return;
        }

        // Do not use webpart settings if e-commerce setting AutomaticCustomerRegistration is true
        if (ECommerceSettings.AutomaticCustomerRegistration(SiteContext.CurrentSiteID))
        {
            // Show error message only on Page and Design tabs
            if ((PortalContext.ViewMode == ViewModeEnum.Design) || (PortalContext.ViewMode == ViewModeEnum.Edit))
            {
                pnlCheckBox.Visible = false;
                pnlError.Visible = true;
                lblError.Text = GetString("com.checkout.registeraftercheckouterror");
            }
            else
            {
                IsVisible = false;
            }
        }
        else if (CurrentUser.IsPublic())
        {
            // Show the control
            chkRegister.Text = GetStringValue("Text", "");
            chkRegister.Checked = ValidationHelper.GetBoolean(GetValue("Checked"), false);

            // Use webpart settings, not e-commerce settings
            useWebpartSettings = true;
        }
        else
        {
            IsVisible = false;
        }
    }
}