using System;
using System.Data;
using System.Web.UI.WebControls;

using CMS.Core;
using CMS.Ecommerce;
using CMS.Helpers;
using CMS.Base;
using CMS.UIControls;
using CMS.ExtendedControls;

[ParentObject(ShippingOptionInfo.OBJECT_TYPE, "objectid")]
[UIElement(ModuleName.ECOMMERCE, "Configuration.ShippingOptions.ShippingCosts")]
[Action(0, "com.ui.shippingcost.edit_new", "ShippingOption_Edit_ShippingCosts_Edit.aspx?objectid={%EditedObjectParent.ID%}&siteId={?siteId?}")]
[Help("shippingcosts_list", "helpTopic")]
public partial class CMSModules_Ecommerce_Pages_Tools_Configuration_ShippingOptions_ShippingOption_Edit_ShippingCosts : CMSShippingOptionsPage
{
    #region "Variables"

    protected ShippingOptionInfo shippingOption = null;
    protected CurrencyInfo currency = null;

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        shippingOption = EditedObjectParent as ShippingOptionInfo;
        if (shippingOption != null)
        {
            CheckEditedObjectSiteID(shippingOption.ShippingOptionSiteID);
            currency = CurrencyInfoProvider.GetMainCurrency(shippingOption.ShippingOptionSiteID);
        }

        // Init unigrid
        gridElem.OnAction += gridElem_OnAction;
        gridElem.OnExternalDataBound += gridElem_OnExternalDataBound;
        gridElem.ZeroRowsText = GetString("com.ui.shippingcost.edit_nodata");
        gridElem.GridView.AllowSorting = false;
    }

    #endregion


    #region "Event handlers"

    /// <summary>
    /// Handles the UniGrid's OnAction event.
    /// </summary>
    /// <param name="actionName">Name of item (button) that throws event</param>
    /// <param name="actionArgument">ID (value of Primary key) of corresponding data row</param>
    protected void gridElem_OnAction(string actionName, object actionArgument)
    {
        switch (actionName.ToLowerCSafe())
        {
            case "delete":
                // Check permissions
                CheckConfigurationModification();

                // Delete ShippingCostInfo object from database
                ShippingCostInfoProvider.DeleteShippingCostInfo(Convert.ToInt32(actionArgument));
                break;
        }
    }


    /// <summary>
    /// Handles the UniGrid's OnExternalDataBound event.
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="sourceName">Source name</param>
    /// <param name="parameter">Parameter</param>
    protected object gridElem_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        if (sourceName.ToLowerCSafe().EqualsCSafe("shippingcostvalue"))
        {
            double value = ValidationHelper.GetDouble(parameter, 0);

            return CurrencyInfoProvider.GetFormattedPrice(value, currency);
        }
        return parameter;
    }

    #endregion
}