using CMS;
using CMS.Base;
using CMS.ExtendedControls;
using CMS.UIControls;

[assembly: RegisterCustomClass("MacroRulesListExtender", typeof(MacroRulesListExtender))]

/// <summary>
/// Macro rules list extender
/// </summary>
public class MacroRulesListExtender : ControlExtender<UniGrid>
{
    public override void OnInit()
    {
        if ((Control != null) && (Control.InfoObject != null))
        {
            Control.InfoObject.SetValue("MacroRuleResourceName", "cms.reporting");
        }
    }
}