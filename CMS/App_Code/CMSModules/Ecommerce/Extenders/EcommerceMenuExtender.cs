using System;
using System.Linq;

using CMS;
using CMS.Base;
using CMS.Core;
using CMS.Ecommerce;
using CMS.ExtendedControls;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.Helpers;

[assembly: RegisterCustomClass("EcommerceMenuExtender", typeof(EcommerceMenuExtender))]

/// <summary>
/// Extender for main E-commerce menu
/// </summary>
public class EcommerceMenuExtender : ControlExtender<UIToolbar>
{
    public override void OnInit()
    {
        Control.OnButtonCreating += OnButtonCreating;
        Control.OnButtonFiltered += OnButtonFiltered;
        Control.Page.PreRender += Page_PreRender;
    }


    protected void Page_PreRender(object sender, EventArgs e)
    {
        // Register method for propagate live URL of selected node to header link
        string script = @"
function SetLiveSiteURL(url){
    parent.SetLiveSiteURL(url);
}";
        ScriptHelper.RegisterClientScriptBlock(Control.Page, typeof(string), "SetLiveSiteURL", ScriptHelper.GetScript(script));
    }


    private void OnButtonCreating(object sender, UniMenuArgs eventArgs)
    {
        var element = eventArgs.UIElement;
        string elementName = element.ElementName;

        if (elementName.CompareToCSafe("products", true) == 0)
        {
            // Change URL if products management UI has product tree disabled by setting
            if (ECommerceSettings.ProductsTree(SiteContext.CurrentSiteName) == ProductsTreeModeEnum.None)
            {
                element.ElementTargetURL = "~/CMSModules/Ecommerce/Pages/Tools/Products/Product_List.aspx";
            }
        }
    }


    private bool OnButtonFiltered(object sender, UniMenuArgs eventArgs)
    {
        // Hide reports button when reporting module is not loaded
        if (eventArgs.UIElement.ElementName.CompareToCSafe("ecreports", true) == 0)
        {
            return ModuleEntryManager.IsModuleLoaded(ModuleName.REPORTING);
        }

        return true;
    }
}