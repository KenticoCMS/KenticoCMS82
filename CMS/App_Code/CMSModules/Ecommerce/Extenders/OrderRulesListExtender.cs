using System;
using System.Linq;

using CMS;
using CMS.Base;
using CMS.ExtendedControls;
using CMS.UIControls;

[assembly: RegisterCustomClass("OrderRulesListExtender", typeof(OrderRulesListExtender))]

/// <summary>
/// Order rules list extender
/// </summary>
public class OrderRulesListExtender : ControlExtender<UniGrid>
{
    public override void OnInit()
    {
        if ((Control != null) && (Control.InfoObject != null))
        {
            Control.InfoObject.SetValue("MacroRuleResourceName", "com.orderdiscount");
        }
    }
}