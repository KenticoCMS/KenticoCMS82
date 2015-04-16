using System;

using CMS.Base;
using CMS.Core;
using CMS.DataEngine;

/// <summary>
/// Application methods.
/// </summary>
public class Global : CMSHttpApplication
{
    #region "Methods"

    public Global()
    {
#if DEBUG
        // Set debug mode
        SystemContext.IsWebProjectDebug = true;
#endif

        ApplicationEvents.PreInitialized.Execute += EnsureDynamicModules;
    }


    /// <summary>
    /// Ensures that modules from the App code assembly are registered.
    /// </summary>
    private static void EnsureDynamicModules(object sender, EventArgs e)
    {
        ModuleEntryManager.EnsureModule<CMSModuleLoader>();

        var discovery = new ModuleDiscovery();
        var assembly = typeof(CMSModuleLoader).Assembly;
        foreach (var module in discovery.GetModules(assembly))
        {
            ModuleEntryManager.EnsureModule(module);
        }
    }

    #endregion
}