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


[assembly: RegisterCustomClass("GroupEditTabsExtender", typeof(GroupEditTabsExtender))]

/// <summary>
/// Extender for edit group horizontal tabs
/// </summary>
public class GroupEditTabsExtender : UITabsExtender
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

        if (tab.TabName.ToLowerCSafe().EqualsCSafe("editgroupcustomfields"))
        {
            // Check custom fields of group
            FormInfo formInfo = FormHelper.GetFormInfo(PredefinedObjectType.GROUP, false);
            if (!formInfo.GetFormElements(true, false, true).Any())
            {
                e.Tab = null;
            }
        }
    }
}