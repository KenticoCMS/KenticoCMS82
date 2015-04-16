using System;
using System.Web.UI.WebControls;

using CMS.Helpers;
using CMS.PortalEngine;
using CMS.Base;
using CMS.UIControls;
using CMS.DocumentEngine;

public partial class CMSModules_PortalEngine_UI_WebParts_WebPartProperties_header : CMSWebPartPropertiesPage
{
    #region "Variables"

    private WebPartTypeEnum type = WebPartTypeEnum.Standard;

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Initialize page title
        pageTitle.TitleText = GetString("WebpartProperties.Title");
        if (!RequestHelper.IsPostBack())
        {
            InitializeMenu();
        }

        tabsElem.OnTabCreated += tabElem_OnTabCreated;

        ScriptHelper.RegisterWOpenerScript(Page);
        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), ScriptHelper.NEWWINDOW_SCRIPT_KEY, ScriptHelper.NewWindowScript);
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        tabsElem.DoTabSelection();
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Initializes menu.
    /// </summary>
    protected void InitializeMenu()
    {
        if (webpartId != string.Empty)
        {
            // get pageinfo
            PageInfo pi = GetPageInfo(aliasPath, templateId, cultureCode);
            if (pi == null)
            {
                Visible = false;
                return;
            }

            PageTemplateInfo pti = pi.UsedPageTemplateInfo;
            if (pti != null)
            {
                // Get web part
                WebPartInstance webPart = pti.TemplateInstance.GetWebPart(instanceGuid, webpartId);
                
                // New webpart is loaded via WebPartID
                if ((webPart == null) && !isNew)
                {
                    pti.TemplateInstance.LoadVariants(false, VariantModeEnum.None);
                    webPart = pti.TemplateInstance.GetWebPart(instanceGuid, -1, 0);
                }

                WebPartInfo wi = (webPart != null) ? WebPartInfoProvider.GetWebPartInfo(webPart.WebPartType) :
                    WebPartInfoProvider.GetWebPartInfo(ValidationHelper.GetInteger(webpartId, 0));

                if (wi != null)
                {
                    type = (WebPartTypeEnum)wi.WebPartType;

                    // Generate documentation link
                    Literal ltr = new Literal();
                    string docScript = "NewWindow('" + ResolveUrl("~/CMSModules/PortalEngine/UI/WebParts/WebPartDocumentationPage.aspx") + "?webpartid=" + ScriptHelper.GetString(wi.WebPartName, false) + "', 'WebPartPropertiesDocumentation', 800, 800); return false;";
                    string tooltip = GetString("help.tooltip");
                    ltr.Text += String.Format
                        ("<div class=\"action-button\"><a onclick=\"{0}\" href=\"#\"><span class=\"sr-only\">{1}</span><i class=\"icon-modal-question cms-icon-80\" title=\"{1}\" aria-hidden=\"true\"></i></a></div>",
                            docScript, tooltip);

                    pageTitle.RightPlaceHolder.Controls.Add(ltr);
                    pageTitle.TitleText = GetString("WebpartProperties.Title") + " (" + HTMLHelper.HTMLEncode(ResHelper.LocalizeString(wi.WebPartDisplayName)) + ")";
                }
            }
        }

        tabsElem.UrlTarget = "webpartpropertiescontent";
    }

    #endregion


    #region "Control events"

    protected void tabElem_OnTabCreated(object sender, TabCreatedEventArgs e)
    {
        if (e.Tab == null)
        {
            return;
        }

        var tab = e.Tab;
        var element = e.UIElement;

        switch (element.ElementName.ToLowerCSafe())
        {

            case "webpartproperties.general":
                if (PortalContext.ViewMode.IsWireframe())
                {
                    tab.SkipCheckPermissions = true;
                }
                break;

            case "webpartproperties.datasource":
                if ((type != WebPartTypeEnum.BasicViewer) || isNew)
                {
                    e.Tab = null;
                    return;
                }
                break;

            case "webpartproperties.viewer":
                if ((type != WebPartTypeEnum.DataSource) || isNew)
                {
                    e.Tab = null;
                    return;
                }
                break;

            case "webpartproperties.variant":
                {
                    if ((variantId <= 0) || isNew || isNewVariant)
                    {
                        e.Tab = null;
                        return;
                    }
                }
                break;

            case "webpartzoneproperties.variant":
                if ((zoneVariantId <= 0) || isNew)
                {
                    e.Tab = null;
                    return;
                }
                break;

            case "webpartproperties.layout":
                if (isNew || isNewVariant)
                {
                    e.Tab = null;
                    return;
                }

                // Hide loader, it appears on wrong position because of small frame
                e.Tab.OnClientClick = "if (window.Loader) { window.Loader.hide(); }";
                break;
        }
    }

    #endregion
}