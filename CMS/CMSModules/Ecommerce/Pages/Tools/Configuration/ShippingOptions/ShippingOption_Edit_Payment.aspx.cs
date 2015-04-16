using System;
using System.Data;

using CMS.Core;
using CMS.Ecommerce;
using CMS.Helpers;
using CMS.UIControls;
using CMS.DataEngine;

[EditedObject("ecommerce.shippingOption", "objectid")]
[UIElement(ModuleName.ECOMMERCE, "Configuration.ShippingOptions.PaymentMethods")]
public partial class CMSModules_Ecommerce_Pages_Tools_Configuration_ShippingOptions_ShippingOption_Edit_Payment : CMSShippingOptionsPage
{
    protected int mShippingOptionId = 0;
    protected string mCurrentValues = string.Empty;
    protected ShippingOptionInfo mShippingOptionInfoObj = null;


    protected void Page_Load(object sender, EventArgs e)
    {
        bool offerGlobalPaymentMethods = false;

        mShippingOptionId = QueryHelper.GetInteger("objectid", 0);
        if (mShippingOptionId > 0)
        {
            mShippingOptionInfoObj = ShippingOptionInfoProvider.GetShippingOptionInfo(mShippingOptionId);
            EditedObject = mShippingOptionInfoObj;

            if (mShippingOptionInfoObj != null)
            {
                int editedSiteId = mShippingOptionInfoObj.ShippingOptionSiteID;
                // Check object's site id
                CheckEditedObjectSiteID(editedSiteId);

                // Offer global payment methods when allowed
                offerGlobalPaymentMethods = ECommerceSettings.AllowGlobalPaymentMethods(editedSiteId);

                DataSet ds = PaymentOptionInfoProvider.GetPaymentOptionsForShipping(mShippingOptionId).OrderBy("PaymentOptionDisplayName");
                if (!DataHelper.DataSourceIsEmpty(ds))
                {
                    mCurrentValues = TextHelper.Join(";", DataHelper.GetStringValues(ds.Tables[0], "PaymentOptionID"));
                }

                if (!RequestHelper.IsPostBack())
                {
                    uniSelector.Value = mCurrentValues;
                }
            }
        }

        uniSelector.OnSelectionChanged += uniSelector_OnSelectionChanged;
        uniSelector.WhereCondition = GetSelectorWhereCondition(offerGlobalPaymentMethods);
    }


    protected void uniSelector_OnSelectionChanged(object sender, EventArgs e)
    {
        SaveItems();
    }


    protected void SaveItems()
    {
        if (mShippingOptionInfoObj == null)
        {
            return;
        }

        // Check permissions
        CheckConfigurationModification();

        // Remove old items
        string newValues = ValidationHelper.GetString(uniSelector.Value, null);
        string items = DataHelper.GetNewItemsInList(newValues, mCurrentValues);
        if (!String.IsNullOrEmpty(items))
        {
            string[] newItems = items.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            // Add all new items to user
            foreach (string item in newItems)
            {
                int paymentId = ValidationHelper.GetInteger(item, 0);
                PaymentShippingInfoProvider.RemovePaymentFromShipping(paymentId, mShippingOptionId);
            }
        }

        // Add new items
        items = DataHelper.GetNewItemsInList(mCurrentValues, newValues);
        if (!String.IsNullOrEmpty(items))
        {
            string[] newItems = items.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            // Add all new items to user
            foreach (string item in newItems)
            {
                int paymentId = ValidationHelper.GetInteger(item, 0);
                PaymentShippingInfoProvider.AddPaymentToShipping(paymentId, mShippingOptionId);
            }
        }

        // Show message
        ShowChangesSaved();
    }


    /// <summary>
    /// Returns where condition for uniselector. This condition filters records contained in currently selected values,
    /// global records according to offerGlobalObjects paramter and site-specific records according to edited objects 
    /// site Id.
    /// </summary>
    /// <param name="offerGlobalObjects">Indicates if global records will be selected</param>
    protected string GetSelectorWhereCondition(bool offerGlobalObjects)
    {
        // Select nothing
        string where = "(1=0)";

        // Add global records
        if (offerGlobalObjects)
        {
            where = SqlHelper.AddWhereCondition(where, "PaymentOptionSiteID IS NULL", "OR");
        }

        // Add site specific records
        if (mShippingOptionInfoObj != null)
        {
            where = SqlHelper.AddWhereCondition(where, "PaymentOptionSiteID = " + mShippingOptionInfoObj.ShippingOptionSiteID, "OR");
        }

        where = SqlHelper.AddWhereCondition(where, "PaymentOptionEnabled = 1");

        // Add records which are used by parent object
        if (!string.IsNullOrEmpty(mCurrentValues))
        {
            where = SqlHelper.AddWhereCondition(where, "PaymentOptionID IN (" + mCurrentValues.Replace(';', ',') + ")", "OR");
        }

        return where;
    }
}