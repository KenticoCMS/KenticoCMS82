using System;
using System.Linq;

using CMS;
using CMS.Base;
using CMS.MacroEngine;
using CMS.OnlineMarketing;


[assembly: RegisterCustomClass("OnlineMarketingMacroRuleEditExtender", typeof(OnlineMarketingMacroRuleEditExtender))]

/// <summary>
/// Extends Macro rule UIForm in Online Marketing - Contact Management - Configuration - Macro rules
/// </summary>
public class OnlineMarketingMacroRuleEditExtender : MacroRuleEditExtender
{
    public override void OnInit()
    {
        // Use PreRender event to handle the warning on both loading the page and saving new values
        Control.PreRender += ShowMacroWarning;
        base.OnInit();
    }


    private void ShowMacroWarning(object sender, EventArgs e)
    {
        MacroRuleInfo info = Control.EditedObject as MacroRuleInfo;
        if (info != null)
        {
            string macroName = info.MacroRuleName;
            if (!MacroRuleMetadataContainer.IsTranslatorAvailable(macroName))
            {
                Control.ShowWarning(Control.GetString("om.configuration.macro.slow"));
            }
        }
    }
}