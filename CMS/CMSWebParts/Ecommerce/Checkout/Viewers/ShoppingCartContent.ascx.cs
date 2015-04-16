using System;

using CMS.Ecommerce;
using CMS.Controls;
using CMS.ExtendedControls;
using CMS.PortalEngine;
using CMS.Base;
using CMS.Helpers;

/// <summary>
/// Cart content web part
/// </summary>
public partial class CMSWebParts_Ecommerce_Checkout_Viewers_ShoppingCartContent : CMSCheckoutWebPart
{
    #region "Variables"

    private bool orderChanged;

    #endregion


    #region "Constants"

    private const string EMPTY_CART_REDIRECTED = "EMPTY_CART_REDIRECTED";

    #endregion


    #region "Properties"

    /// <summary>
    /// Gets or sets the name of the hierarchical transformation which is used for displaying the results.
    /// </summary>
    public string TransformationName
    {
        get
        {
            return ValidationHelper.GetString(GetValue("TransformationName"), "");
        }
        set
        {
            SetValue("TransformationName", value);
        }
    }

    #endregion


    #region "Life cycle"

    /// <summary>
    /// OnInit event handler.
    /// </summary>
    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        // Subscribe to the checkout events
        SubscribeToCheckoutEvents();
    }


    /// <summary>
    /// Load event handler.
    /// </summary>    
    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        SetupControl();
        // Clear Empty cart redirection flag
        SessionHelper.SetValue(EMPTY_CART_REDIRECTED, false);
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);
        ScriptHelper.RegisterDialogScript(Page);
        ControlsHelper.UpdateCurrentPanel(this);
    }

    #endregion


    #region "Event handling"

    /// <summary>
    /// Updates the web part according to the new the new shopping cart values.
    /// </summary>
    public void Update(object sender, EventArgs e)
    {
        orderChanged = true;

        var wasRedirectedOnce = ValidationHelper.GetBoolean(SessionHelper.GetValue(EMPTY_CART_REDIRECTED), false);
        // One full page reload for empty cart
        if (ShoppingCart.IsEmpty && !wasRedirectedOnce)
        {
            SessionHelper.SetValue(EMPTY_CART_REDIRECTED, true);
            URLHelper.Redirect(RequestContext.CurrentURL);
        }

        SetupControl();
    }

    #endregion


    #region "Methods"

    protected override void LoadStep(object sender, StepEventArgs e)
    {
        base.LoadStep(sender, e);
        ShoppingCart.InvalidateCalculations();
    }


    /// <summary>
    /// Validates the data.
    /// </summary>
    protected override void ValidateStepData(object sender, StepEventArgs e)
    {
        base.ValidateStepData(sender, e);

        if (!CheckShoppingCart())
        {
            e.CancelEvent = true;
        }
    }


    /// <summary>
    /// Subscribes the web part to the Checkout events.
    /// </summary>
    private void SubscribeToCheckoutEvents()
    {
        ComponentEvents.RequestEvents.RegisterForEvent(SHOPPING_CART_CHANGED, Update);
    }


    /// <summary>
    /// Performs an inventory check for all items in the shopping cart and show message if required.
    /// </summary>
    /// <returns>True if all items are available in sufficient quantities, otherwise false</returns>
    private bool CheckShoppingCart()
    {
        // Remove error message before reevaluation
        lblError.Text = string.Empty;

        if (ShoppingCart.IsEmpty)
        {
            lblError.Visible = true;
            lblError.Text = ResHelper.GetString("com.checkout.cartisempty");
            return false;
        }

        ShoppingCartCheckResult checkResult = ShoppingCartInfoProvider.CheckShoppingCart(ShoppingCart);

        // Try to show message through Message Panel web part
        if (checkResult.CheckFailed || orderChanged)
        {
            CMSEventArgs<string> args = new CMSEventArgs<string>();
            args.Parameter = checkResult.GetHTMLFormattedMessage();
            ComponentEvents.RequestEvents.RaiseEvent(this, args, MESSAGE_RAISED);

            // If Message Panel web part is not present (Parameter is cleared by web part after successful handling), show error message in error label
            if (!string.IsNullOrEmpty(args.Parameter) && checkResult.CheckFailed)
            {
                lblError.Visible = true;
                lblError.Text = checkResult.GetHTMLFormattedMessage();
            }
        }
        return !checkResult.CheckFailed;
    }


    /// <summary>
    /// Sets up the web part.
    /// </summary> 
    public void SetupControl()
    {
        if (!StopProcessing)
        {
            if (!ShoppingCart.IsEmpty)
            {
                shoppingCartUniView.DataSource = new GroupedEnumerable<ShoppingCartItemInfo>(ShoppingCart.CartItems, GetKey, GetLevel);
                LoadTransformations();
                // Check cart
                CheckShoppingCart();
            }
            else
            {
                // Display empty shopping cart message
                lblShoppingCartEmpty.Text = GetString("commercecart.emptycart");
                lblShoppingCartEmpty.Visible = true;
                // Update the UniView
                shoppingCartUniView.DataSource = null;
                shoppingCartUniView.ReBind();
            }
        }
    }


    /// <summary>
    /// Function for building the GroupedEnumerable. Returns the level on which the item should be placed in the object.
    /// </summary>
    protected int GetLevel(ShoppingCartItemInfo Item)
    {
        return (Item.IsProductOption || Item.IsBundleItem) ? 1 : 0;
    }


    /// <summary>
    /// Function for building the GroupedEnumerable. Returns the key according to which the hierarchy is build. If it is a top level item it returns 0.
    /// </summary>
    protected object GetKey(ShoppingCartItemInfo Item)
    {
        if (!(Item.IsProductOption || Item.IsBundleItem))
        {
            return 0;
        }

        return Item.CartItemParentGUID;
    }


    /// <summary>
    /// Load transformations with dependence.
    /// </summary>
    protected void LoadTransformations()
    {
        if (!String.IsNullOrEmpty(TransformationName))
        {
            TransformationInfo ti = TransformationInfoProvider.GetTransformation(TransformationName);

            if (ti != null)
            {
                // Setting up the common BasicUniView properties
                shoppingCartUniView.HierarchicalDisplayMode = HierarchicalDisplayModeEnum.Inner;
                shoppingCartUniView.RelationColumnID = "CartItemGuid";

                if (ti.TransformationIsHierarchical)
                {
                    // Setting up the hierarchical transformation.
                    HierarchicalTransformations ht = new HierarchicalTransformations("CartItemGUID");
                    ht.LoadFromXML(ti.TransformationHierarchicalXMLDocument);
                    // Setting up the BasicUniView
                    shoppingCartUniView.Transformations = ht;
                    shoppingCartUniView.UseNearestItemForHeaderAndFooter = true;
                }
                else
                {
                    // Setting up the BasicUniView with a non-hierarchical transformation
                    shoppingCartUniView.ItemTemplate = CMS.Controls.CMSDataProperties.LoadTransformation(shoppingCartUniView, ti.TransformationFullName);
                }

                // Makes sure new data is loaded if the date changes and transformation needs to be reloaded
                shoppingCartUniView.DataBind();
            }
        }
    }

    #endregion
}