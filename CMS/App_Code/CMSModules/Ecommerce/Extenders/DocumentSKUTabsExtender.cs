using System;
using System.Linq;

using CMS;
using CMS.Base;
using CMS.Ecommerce;
using CMS.Helpers;
using CMS.UIControls;


[assembly: RegisterCustomClass("DocumentSKUTabsExtender", typeof(DocumentSKUTabsExtender))]

/// <summary>
/// Extender for SKU tab of product document detail
/// </summary>
public class DocumentSKUTabsExtender : UITabsExtender
{
    /// <summary>
    /// Initializes the extender
    /// </summary>
    public override void OnInit()
    {
        // Check security for content and ecommerce
        CMSContentPage.CheckSecurity("product");

        Control.Page.Load += OnLoad;
    }


    /// <summary>
    /// Initialization of tabs.
    /// </summary>
    public override void OnInitTabs()
    {
        
    }


    /// <summary>
    /// Fires when the page loads
    /// </summary>
    private void OnLoad(object sender, EventArgs eventArgs)
    {
        var page = (CMSPage)Control.Page;

        // Register script file
        ScriptHelper.RegisterEditScript(page, false);
    }
}
