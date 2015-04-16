using System;
using System.Data;

using CMS.Core;
using CMS.DataEngine;
using CMS.Ecommerce;
using CMS.Helpers;
using CMS.Base;
using CMS.PortalEngine;
using CMS.SiteProvider;
using CMS.UIControls;

[Title("COM_ShippingOption_List.HeaderCaption")]
[Action(0, "COM_ShippingOption_List.NewItemCaption", "{%UIContext.GetElementUrl(\"cms.ecommerce\",\"new.Configuration.ShippingOptions\", false)|(encode)false%}")]
[UIElement(ModuleName.ECOMMERCE, "Configuration.ShippingOptions")]
public partial class CMSModules_Ecommerce_Pages_Tools_Configuration_ShippingOptions_ShippingOption_List : CMSShippingOptionsPage
{
    #region "Page Events"

    protected void Page_Load(object sender, EventArgs e)
    {       
        // Init Unigrid
        UniGrid.OnAction += uniGrid_OnAction;
        UniGrid.OnExternalDataBound += UniGrid_OnExternalDataBound;

        InitWhereCondition();
    }

    #endregion


    #region "Event Handlers"

    /// <summary>
    /// Handles the UniGrid's OnAction event.
    /// </summary>
    /// <param name="actionName">Name of item (button) that throws event</param>
    /// <param name="actionArgument">ID (value of Primary key) of corresponding data row</param>
    protected void uniGrid_OnAction(string actionName, object actionArgument)
    {
        if (actionName == "edit")
        {
            URLHelper.Redirect(UIContextHelper.GetElementUrl(ModuleName.ECOMMERCE, "Edit.Configuration.ShippingOptionProperties", false, actionArgument.ToInteger(0)));
        }
        else if (actionName == "delete")
        {
            var shippingInfoObj = ShippingOptionInfoProvider.GetShippingOptionInfo(ValidationHelper.GetInteger(actionArgument, 0));
            // Nothing to delete
            if (shippingInfoObj == null)
            {
                return;
            }

            // Check permissions
            CheckConfigurationModification();

            // Check dependencies
            if (shippingInfoObj.Generalized.CheckDependencies())
            {
                // Show error message
                ShowError(ECommerceHelper.GetDependencyMessage(shippingInfoObj));

                return;
            }
            // Delete ShippingOptionInfo object from database
            ShippingOptionInfoProvider.DeleteShippingOptionInfo(shippingInfoObj);
        }
    }


    private object UniGrid_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        switch (sourceName.ToLowerCSafe())
        {
            case "shippingoptioncharge":
                DataRowView row = (DataRowView)parameter;
                double value = ValidationHelper.GetDouble(row["ShippingOptionCharge"], 0);
                int siteId = ValidationHelper.GetInteger(row["ShippingOptionSiteID"], 0);

                return CurrencyInfoProvider.GetFormattedPrice(value, siteId);
        }

        return parameter;
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Creates where condition for UniGrid and reloads it.
    /// </summary>
    private void InitWhereCondition()
    {
        var where = new WhereCondition().WhereEquals("ShippingOptionSiteID", SiteContext.CurrentSiteID);
        UniGrid.WhereCondition = where.ToString(true);
    }

    #endregion
}