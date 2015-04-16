using System;
using System.Linq;

using CMS;
using CMS.Base;
using CMS.UIControls;
using CMS.PortalEngine;
using CMS.Helpers;
using CMS.IO;

[assembly: RegisterCustomClass("WebPartHeaderControlExtender", typeof(WebPartHeaderControlExtender))]

/// <summary>
/// Extender class
/// </summary>
public class WebPartHeaderControlExtender : UITabsExtender
{
    public override void OnInit()
    {
        base.OnInit();

        ScriptHelper.RegisterClientScriptBlock(Control, typeof(string), "InfoScript", ScriptHelper.GetScript("function IsCMSDesk() { return true; }"));
    }


    /// <summary>
    /// Initialization of tabs.
    /// </summary>
    public override void OnInitTabs()
    {
        Control.OnTabCreated += OnTabCreated;
    }


    void OnTabCreated(object sender, TabCreatedEventArgs e)
    {
        if (e.Tab == null)
        {
            return;
        }

        var tab = e.Tab;

        switch (tab.TabName.ToLowerCSafe())
        {
            case "webpart.code":
                if (!ValidationHelper.GetBoolean(SettingsHelper.AppSettings["CMSDevelopmentMode"], false))
                {
                    e.Tab = null;
                }
                break;

            case "webpart.theme":
                var wpi = Control.UIContext.EditedObject as WebPartInfo;

                if ((wpi != null) && StorageHelper.IsExternalStorage(wpi.GetThemePath()))
                {
                    e.Tab = null;
                }
                break;
        }
    }
}