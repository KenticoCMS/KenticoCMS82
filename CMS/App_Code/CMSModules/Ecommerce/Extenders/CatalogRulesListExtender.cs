using System;
using System.Linq;

using CMS;
using CMS.Base;
using CMS.ExtendedControls;
using CMS.UIControls;

[assembly: RegisterCustomClass("CatalogRulesListExtender", typeof(CatalogRulesListExtender))]

/// <summary>
/// Catalog rules list extender
/// </summary>
public class CatalogRulesListExtender : ControlExtender<UniGrid>
{
    public override void OnInit()
    {
        if ((Control != null) && (Control.InfoObject != null))
        {
            Control.InfoObject.SetValue("MacroRuleResourceName", "com.catalogdiscount");
        }
    }
}
