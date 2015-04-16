using System;

using CMS.Base;
using CMS.Core;
using CMS.Ecommerce;
using CMS.Helpers;
using CMS.PortalEngine;
using CMS.UIControls;


[Title("Manufacturer_Edit.ItemListLink")]
[Action(0, "Manufacturer_List.NewItemCaption", "{%UIContextHelper.GetElementUrl(\"CMS.Ecommerce\", \"NewManufacturer\", \"false\")|(encode)false%}")]
[UIElement(ModuleName.ECOMMERCE, "Manufacturers")]
public partial class CMSModules_Ecommerce_Pages_Tools_Manufacturers_Manufacturer_List : CMSManufacturersPage
{
    #region "Page Events"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        // Init Unigrid
        UniGrid.OnAction += uniGrid_OnAction;
        HandleGridsSiteIDColumn(UniGrid);
        UniGrid.WhereCondition = InitSiteWhereCondition("ManufacturerSiteID").ToString(true);
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
        int manufacturerId = ValidationHelper.GetInteger(actionArgument, 0);

        if (actionName == "edit")
        {
            var url = UIContextHelper.GetElementUrl("CMS.Ecommerce", "EditManufacturer", false, actionArgument.ToInteger(0));
            url = URLHelper.AddParameterToUrl(url, "action", "edit");
            URLHelper.Redirect(url);
        }
        else if (actionName == "delete")
        {
            // Check module permissions
            var manufacturerObj = ManufacturerInfoProvider.GetManufacturerInfo(manufacturerId);
            if (manufacturerObj != null)
            {
                if (!ECommerceContext.IsUserAuthorizedToModifyManufacturer(manufacturerObj))
                {
                    if (manufacturerObj.IsGlobal)
                    {
                        RedirectToAccessDenied("CMS.Ecommerce", "EcommerceGlobalModify");
                    }
                    else
                    {
                        RedirectToAccessDenied("CMS.Ecommerce", "EcommerceModify OR ModifyManufacturers");
                    }
                }

                // Check dependencies
                if (manufacturerObj.Generalized.CheckDependencies())
                {
                    // Show error message
                    ShowError(ECommerceHelper.GetDependencyMessage(manufacturerObj));

                    return;
                }

                // Delete ManufacturerInfo object from database
                ManufacturerInfoProvider.DeleteManufacturerInfo(manufacturerObj);
            }
        }
    }


    #endregion
}