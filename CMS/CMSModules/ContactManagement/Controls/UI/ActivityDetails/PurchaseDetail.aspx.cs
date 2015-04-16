using System;

using CMS.Core;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.OnlineMarketing;
using CMS.SiteProvider;
using CMS.UIControls;

[Title("om.activitydetails.viewinvoicedetail")]
[Security(Resource = ModuleName.CONTACTMANAGEMENT, Permission = "ReadActivities")]
public partial class CMSModules_ContactManagement_Controls_UI_ActivityDetails_PurchaseDetail : CMSModalPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (ActivityHelper.AuthorizedReadActivity(SiteContext.CurrentSiteID, true))
        {
            if (!QueryHelper.ValidateHash("hash"))
            {
                return;
            }

            if (!ModuleManager.IsModuleLoaded(ModuleName.ECOMMERCE))
            {
                return;
            }

            int orderId = QueryHelper.GetInteger("orderid", 0);

            // Get order object
            BaseInfo order = BaseAbstractInfoProvider.GetInfoById(PredefinedObjectType.ORDER, orderId);
            if (order != null)
            {
                ltl.Text = order.GetStringValue("OrderInvoice", "");
            }
        }
    }
}