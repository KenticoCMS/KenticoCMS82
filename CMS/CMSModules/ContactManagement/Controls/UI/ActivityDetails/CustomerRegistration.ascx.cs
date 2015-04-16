using System;

using CMS.Core;
using CMS.DataEngine;
using CMS.OnlineMarketing;
using CMS.WebAnalytics;

public partial class CMSModules_ContactManagement_Controls_UI_ActivityDetails_CustomerRegistration : ActivityDetail
{
    public override bool LoadData(ActivityInfo ai)
    {
        if ((ai == null) || (ai.ActivityType != PredefinedActivityType.CUSTOMER_REGISTRATION) || !ModuleManager.IsModuleLoaded(ModuleName.ECOMMERCE))
        {
            return false;
        }

        GeneralizedInfo customer = BaseAbstractInfoProvider.GetInfoById(PredefinedObjectType.CUSTOMER, ai.ActivityItemID);
        if (customer != null)
        {
            string name = UserNameFormatter.GetFriendlyUserName(customer.GetValue("CustomerFirstName") as string, null,
                                      customer.GetValue("CustomerLastName") as string,
                                      customer.GetValue("CustomerEmail") as string, null);

            ucDetails.AddRow("om.activitydetails.regcustomer", name);
        }

        return ucDetails.IsDataLoaded;
    }
}