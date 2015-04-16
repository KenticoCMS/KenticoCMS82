using System;
using System.Data;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.Core;
using CMS.DataEngine;
using CMS.Ecommerce;
using CMS.Helpers;
using CMS.PortalEngine;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.ExtendedControls;

[Title("Customers_Edit.ItemListLink")]
[Action(0, "Customers_List.NewItemCaption", "{%UIContext.GetElementUrl(\"cms.ecommerce\",\"NewCustomer\", false)|(encode)false%}")]
[UIElement(ModuleName.ECOMMERCE, "Customers")]
public partial class CMSModules_Ecommerce_Pages_Tools_Customers_Customer_List : CMSCustomersPage
{
    #region "Page Events"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        // Init Unigrid
        UniGrid.OnAction += uniGrid_OnAction;
        UniGrid.OnExternalDataBound += UniGrid_OnExternalDataBound;

        string anonymWhere = "(CustomerUserID IS NULL) AND CustomerSiteID = " + SiteContext.CurrentSiteID;
        string regWhere = string.Format("(CustomerUserID IS NOT NULL) AND (CustomerUserID IN (SELECT UserID FROM CMS_UserSite WHERE SiteID = {0}))", SiteContext.CurrentSiteID);
        UniGrid.WhereCondition = SqlHelper.AddWhereCondition(anonymWhere, regWhere, "OR");

        // Check if user is global administrator
        if (CurrentUser.IsGlobalAdministrator)
        {
            // Display customers without site binding
            UniGrid.WhereCondition = SqlHelper.AddWhereCondition(UniGrid.WhereCondition, "(CustomerUserID IS NULL) AND (CustomerSiteID IS NULL)", "OR");
        }
    }


    protected void Page_Load(object sender, EventArgs e)
    {

    }

    #endregion


    #region "Events"

    protected object UniGrid_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        switch (sourceName.ToLowerCSafe())
        {
            case "hassitebinding":
                // Has site binding action
                {
                    CMSGridActionButton button = ((CMSGridActionButton)sender);

                    if (!IsGlobalStoreAdmin)
                    {
                        button.Visible = false;
                    }
                    else
                    {
                        button.OnClientClick = "return false;";
                        button.Style.Add("cursor", "default");

                        var customerRow = ((DataRowView)((GridViewRow)parameter).DataItem).Row;

                        // Show warning for customers not bound to site
                        int userId = ValidationHelper.GetInteger(customerRow["CustomerUserID"], 0);
                        int siteId = ValidationHelper.GetInteger(customerRow["CustomerSiteID"], 0);
                        button.Visible = (userId == 0) && (siteId == 0);
                    }
                }
                break;
        }

        return parameter;
    }


    /// <summary>
    /// Handles the UniGrid's OnAction event.
    /// </summary>
    /// <param name="actionName">Name of item (button) that throws event</param>
    /// <param name="actionArgument">ID (value of Primary key) of corresponding data row</param>
    protected void uniGrid_OnAction(string actionName, object actionArgument)
    {
        var id = ValidationHelper.GetInteger(actionArgument, 0);

        if (actionName == "edit")
        {
            URLHelper.Redirect(UIContextHelper.GetElementUrl("CMS.Ecommerce", "EditCustomersProperties", false, id));
        }
        else if (actionName == "delete")
        {
            // Check module permissions
            if (!ECommerceContext.IsUserAuthorizedToModifyCustomer())
            {
                RedirectToAccessDenied("CMS.Ecommerce", "EcommerceModify OR ModifyCustomers");
                return;
            }

            // Get customer to be deleted
            var customer = CustomerInfoProvider.GetCustomerInfo(id);

            // Check customers dependencies
            if ((customer != null) && customer.Generalized.CheckDependencies())
            {
                ShowError(ECommerceHelper.GetDependencyMessage(customer));
                return;
            }

            // Delete CustomerInfo object from database
            CustomerInfoProvider.DeleteCustomerInfo(customer);

            UniGrid.ReloadData();
        }
    }

    #endregion
}