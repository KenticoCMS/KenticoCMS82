﻿using System;
using System.Linq;

using CMS.DocumentEngine;
using CMS.Helpers;
using CMS.PortalControls;
using CMS.PortalEngine;
using CMS.UIControls;


public partial class CMSModules_AdminControls_Pages_UIPage : CMSUIPage
{
    #region "Variables"

    private PageTemplateInfo mPageTemplate;

    private const string headerBeginTag = @"
<div id=""UIHeader"" class=""UIHeader"">";

    private const string headerEndTag = @"
    <div id=""CMSHeaderDiv"" class=""CMSHeaderDiv""></div>
    <div id=""CKToolbar"" class=""CKToolbar""></div>
</div>";

    private const string contentBeginTag = @"
<div id=""UIContent"" class=""UIContent scroll-area"">";

    private const string contentEndTag = @"
</div>";

    private const string footerBeginTag = @"
<div id=""UIFooter"" class=""UIFooter"">";

    private const string footerEndTag = @"
</div>";

    #endregion


    #region "Properties"

    /// <summary>
    /// Gets the current page template object.
    /// </summary>
    private PageTemplateInfo PageTemplate
    {
        get
        {
            if (mPageTemplate == null)
            {
                // If direct page template is set via query string use it instead of element's template
                String templateName = QueryHelper.GetString("templatename", String.Empty);
                mPageTemplate = !String.IsNullOrEmpty(templateName) ? PageTemplateInfoProvider.GetPageTemplateInfo(templateName) : PageTemplateInfoProvider.GetPageTemplateInfo(UIElement.ElementPageTemplateID);

                if (mPageTemplate == null)
                {
                    // Prevent loop page template cycling
                    RedirectToInformation(String.Format(GetString("ui.templatenotfound"), templateName));
                }
            }

            return mPageTemplate;
        }
    }


    /// <summary>
    /// Provides access to a dialog's footer. Is only rendered if the page is a dialog.
    /// </summary>
    public override IDialogFooter DialogFooter
    {
        get
        {
            return dialogFooter;
        }
        protected set
        {
            throw new NotSupportedException("[CMSPage.aspx:DialogFooter.get] Cannot assign DialogFooter.");
        }
    }

    #endregion


    #region "Page events"

    /// <summary>
    /// PreInit event handler
    /// </summary>
    protected override void OnPreInit(EventArgs e)
    {
        // Do not automatically switch masterpage if 'dialog=1' is present in query string, because
        // UIPage has its own MasterPage and it would break
        RequiresDialog = false;

        base.OnPreInit(e);

        InitPage();

        EnsureScriptManager();
    }


    /// <summary>
    /// Raises the PreRenderComplete event after the OnPreRenderComplete event and before the page is rendered.
    /// </summary>
    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        EnsureContentScrolling();
        ScriptHelper.RegisterModule(Page, "CMS/HeaderShadow");
    }


    /// <summary>
    /// Load event handler
    /// </summary>
    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        RegisterScripts();
    }

    #endregion


    #region "Private methods"

    /// <summary>
    /// Inits page
    /// </summary>
    private void InitPage()
    {
        // Disable debug for this page
        if (ValidationHelper.GetBoolean(UIContext["disabledebug"], false))
        {
            DisableDebugging();
        }

        // Master page must he addressed here to allow access to page controls on PreInit - DO NOT REMOVE!
        var master = Master as CMSMasterPage;

        if (UIContext.IsDialog)
        {
            master.PanelBody.CssClass = "DialogPageBody";
        }

        if (HandleHierarchy())
        {
            return;
        }

        switch (UIElement.ElementType)
        {
            case UIElementTypeEnum.PageTemplate:
                LoadTemplate();
                break;

            case UIElementTypeEnum.Url:
                {
                    string url = ContextResolver.ResolveMacros(UIElement.ElementTargetURL);

                    RedirectToUrl(url);
                }
                break;
        }

        if (PageTemplate.PageTemplateIsLayout)
        {
            // Layout template needs to propagate the return handler further. The return handler may be used in one of the child layout panes.
            var handlerName = QueryHelper.GetString("returnhandler", string.Empty);

            if (!string.IsNullOrEmpty(handlerName))
            {
                string script = string.Format(
@"
function {0}(parameterValue) {{
    if (wopener && wopener.{0}) {{
        wopener.{0}(parameterValue);
    }}
}}
", ScriptHelper.GetString(handlerName, false));

                ScriptHelper.RegisterStartupScript(this, typeof(string), "returnhandler", script, true);
            }
        }
    }


    /// <summary>
    /// Loads template based on UI element settings
    /// </summary>
    private void LoadTemplate()
    {
        // Init the page components
        manPortal.SetMainPagePlaceholder(plc);

        // Prepare the page info
        PageInfo pi = PageInfoProvider.GetVirtualPageInfo(UIElement.ElementPageTemplateID);
        pi.DocumentNamePath = "/" + ResHelper.GetString("edittabs.design");

        DocumentContext.CurrentPageInfo = pi;

        // Set the design mode
        PortalContext.SetRequestViewMode(ViewModeEnum.UI);
    }


    /// <summary>
    /// Creates the standard page structure: header, content, footer. Modifies web part zones and its properties to ensure the desired structure.
    /// This is the first step for enabling content scrolling.
    /// </summary>
    private void EnsureContentScrolling()
    {
        if (PageTemplate.PageTemplateIsLayout)
        {
            // Layout templates ensure scrolling themselves
            return;
        }

        SetupHeader();
        SetupFooter();
    }


    private void SetupFooter()
    {
        CMSWebPartZone footerZone = plc.WebPartZones.FirstOrDefault(z => ((z.ZoneType == WebPartZoneTypeEnum.Footer) || (z.ZoneType == WebPartZoneTypeEnum.DialogFooter)));
        if ((footerZone != null) && footerZone.IsVisible)
        {
            // Wrap the footer content into footer div
            footerZone.ContentBefore = contentEndTag + footerBeginTag + footerZone.ContentBefore;

            // Close 'UIFooter'
            footerZone.ContentAfter += footerEndTag;
        }
        else
        {
            // Close 'UIContent'
            ltlFooterBefore.Text += contentEndTag;

            // Display the dialog footer panel only in the top dialog frame
            if (UIContext.IsRootDialog)
            {
                ltlFooterBefore.Text += footerBeginTag;
                plcFooter.Visible = true;
                ltlFooterAfter.Text += footerEndTag;
            }
        }
    }


    private void SetupHeader()
    {
        var headerZone = plc.WebPartZones.FirstOrDefault(z => (z.ZoneType == WebPartZoneTypeEnum.Header));
        if ((headerZone != null) && headerZone.IsVisible)
        {
            // Wrap the header content into the header div
            headerZone.ContentBefore = headerBeginTag + headerZone.ContentBefore;

            // Close the header div and open the content div
            headerZone.ContentAfter += headerEndTag + contentBeginTag;
        }
        else
        {
            // Wrap the content into the content div
            plcHeader.Append(contentBeginTag);
        }
    }


    private void RegisterScripts()
    {
        // Register page scripts
        ScriptHelper.RegisterJQuery(this);
        ScriptHelper.RegisterScriptFile(this, URLHelper.ResolveUrl("~/CMSScripts/UI/UIPage.js"));

        if (UIContext.IsRootDialog)
        {
            // Register dialog manipulation functions
            ScriptHelper.RegisterDialogScript(this);
            ScriptHelper.RegisterWOpenerScript(this);
        }
    }

    #endregion
}
