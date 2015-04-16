using System;
using System.Linq;

using CMS;
using CMS.Base;
using CMS.Core;
using CMS.DataEngine;
using CMS.FormEngine;
using CMS.Helpers;
using CMS.LicenseProvider;
using CMS.Membership;
using CMS.Modules;
using CMS.SiteProvider;
using CMS.UIControls;


[assembly: RegisterCustomClass("EditUserTabsExtender", typeof(EditUserTabsExtender))]

/// <summary>
/// Extender for edit user horizontal tabs UIElement
/// </summary>
public class EditUserTabsExtender : UITabsExtender
{
    /// <summary>
    /// Initialization of tabs.
    /// </summary>
    public override void OnInitTabs()
    {
        Control.OnTabCreated += OnTabCreated;
    }


    /// <summary>
    /// Event handling creation of tabs.
    /// </summary>
    private void OnTabCreated(object sender, TabCreatedEventArgs e)
    {
        if (e.Tab == null)
        {
            return;
        }

        var tab = e.Tab;

        switch (tab.TabName.ToLowerCSafe())
        {
            case "customfields":
                // Check custom fields of user
                {
                    int customFields = 0;
                    var userId = QueryHelper.GetInteger("objectid", 0);
                    UserInfo ui = UserInfoProvider.GetUserInfo(userId);
                    if (ui != null)
                    {
                        if (!MembershipContext.AuthenticatedUser.IsGlobalAdministrator && !ui.IsInSite(SiteContext.CurrentSiteName))
                        {
                            URLHelper.SeeOther(UIHelper.GetInformationUrl(ResHelper.GetString("user.notinsite")));
                        }

                        // Get user form information and check for visible non-system fields
                        FormInfo formInfo = FormHelper.GetFormInfo(ui.ClassName, false);
                        customFields = (formInfo.GetFormElements(true, false, true).Any() ? 1 : 0);

                        // Check custom fields of user settings if needed
                        if ((customFields == 0) && (ui.UserSettings != null))
                        {
                            // Get user settings form information and check for visible non-system fields
                            formInfo = FormHelper.GetFormInfo(ui.UserSettings.ClassName, false);
                            customFields = (formInfo.GetFormElements(true, false, true).Any() ? 1 : 0);
                        }
                    }

                    if (customFields == 0)
                    {
                        e.Tab = null;
                    }
                }
                break;

            case "notifications":
                // Display notifications tab ?
                if (!LicenseHelper.IsFeatureAvailableInUI(FeatureEnum.Notifications, ModuleName.NOTIFICATIONS))
                {
                    e.Tab = null;
                }
                break;

            case "languages":
                // Display languages tab ?
                if (!LicenseKeyInfoProvider.IsFeatureAvailable(FeatureEnum.Multilingual))
                {
                    e.Tab = null;
                }
                break;

            case "membership":
                // Display membership tab ?
                if (!LicenseKeyInfoProvider.IsFeatureAvailable(FeatureEnum.Membership))
                {
                    e.Tab = null;
                }
                break;

            case "departments":
                {
                    // Is E-commerce on site ?
                    bool ecommerceOnSite = false;
                    if (SiteContext.CurrentSiteName != null)
                    {
                        // Check if E-commerce module is installed
                        ecommerceOnSite = ModuleEntryManager.IsModuleLoaded(ModuleName.ECOMMERCE) && ResourceSiteInfoProvider.IsResourceOnSite("CMS.Ecommerce", SiteContext.CurrentSiteName);
                    }

                    if (!ecommerceOnSite)
                    {
                        e.Tab = null;
                    }
                }
                break;

            case "sites":
                {
                    bool showSites = false;
                    if (MembershipContext.AuthenticatedUser.IsGlobalAdministrator)
                    {
                        int sitesCount = SiteInfoProvider.GetSitesCount();
                        if (sitesCount > 0)
                        {
                            showSites = true;
                        }
                    }

                    if (!showSites)
                    {
                        e.Tab = null;
                    }
                }
                break;

            case "friends":
                if (!LicenseHelper.IsFeatureAvailableInUI(FeatureEnum.Friends, ModuleName.COMMUNITY))
                {
                    e.Tab = null;
                }
                break;
        }
    }
}