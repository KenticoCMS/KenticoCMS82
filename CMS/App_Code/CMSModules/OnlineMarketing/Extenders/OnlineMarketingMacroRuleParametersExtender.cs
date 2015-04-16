using System;
using System.Linq;

using CMS;
using CMS.ExtendedControls;
using CMS.MacroEngine;
using CMS.OnlineMarketing;
using CMS.UIControls;


[assembly: RegisterCustomClass("OnlineMarketingMacroRuleParametersExtender", typeof(OnlineMarketingMacroRuleParametersExtender))]

public class OnlineMarketingMacroRuleParametersExtender : ControlExtender<BaseFieldEditor>
{
    public override void OnInit()
    {
        Control.Load += EditForm_OnAfterDataLoad;
    }


    private void EditForm_OnAfterDataLoad(object sender, EventArgs e)
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