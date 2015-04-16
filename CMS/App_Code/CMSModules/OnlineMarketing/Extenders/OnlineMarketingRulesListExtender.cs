using System;
using System.Linq;

using CMS;
using CMS.ExtendedControls;
using CMS.OnlineMarketing;
using CMS.UIControls;

[assembly: RegisterCustomClass("OnlineMarketingRulesListExtender", typeof(OnlineMarketingRulesListExtender))]

/// <summary>
/// Macro rule list extender in OM/Configuration. Sets "MacroRuleResourceName" to OM to the edited object, so that
/// right permissions (Contact Management -> Read global configuration) can be set in <see cref="OnlineMarketingModule.CheckMacroPermissions"/>.
/// </summary>
public class OnlineMarketingRulesListExtender : ControlExtender<UniGrid>
{
    public override void OnInit()
    {
        if ((Control != null) && (Control.InfoObject != null))
        {
            Control.InfoObject.SetValue("MacroRuleResourceName", "cms.onlinemarketing");
        }
    }
}
