using System;
using CMS.Helpers;
using CMS.PortalControls;
using CMS.PortalEngine;
using CMS.UIControls;

public partial class CMSModules_Widgets_Dialogs_WidgetProperties_Properties : CMSWidgetPropertiesPage
{
    #region "Page events"

    /// <summary>
    /// PreInit event handler.
    /// </summary>
    protected override void OnPreInit(EventArgs e)
    {
        EnsureDocumentManager = true;
        CurrentMaster.BodyClass += " WidgetsProperties";
        base.OnPreInit(e);
    }


    /// <summary>
    /// Load event handler.
    /// </summary>
    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        // WOpener is required for inline widgets insert script
        ScriptHelper.RegisterWOpenerScript(Page);

        // Initialize the control
        widgetProperties.AliasPath = aliasPath;
        widgetProperties.CultureCode = culture;
        widgetProperties.PageTemplateId = templateId;
        widgetProperties.WidgetId = widgetId;
        widgetProperties.ZoneId = zoneId;
        widgetProperties.InstanceGUID = instanceGuid;
        widgetProperties.IsNewWidget = isNewWidget;
        widgetProperties.IsNewVariant = isNewVariant;
        widgetProperties.IsInline = inline;
        widgetProperties.VariantID = variantId;
        widgetProperties.VariantMode = variantMode;
        widgetProperties.ZoneType = zoneType;
        widgetProperties.IsLiveSite = false;
        widgetProperties.CurrentPageInfo = PageInfo;

        // Ensure the design mode for the dialog
        if (String.IsNullOrEmpty(aliasPath))
        {
            PortalContext.SetRequestViewMode(ViewModeEnum.Design);
        }

        // Setup the current document
        EditedDocument = Node;

        widgetProperties.OnNotAllowed += widgetProperties_OnNotAllowed;

        // Register the OnSave event handler
        FramesManager.OnSave += (sender, arg) => { return widgetProperties.OnSave(); };

        // Register the OnSave event handler
        FramesManager.OnApply += (sender, arg) => { return widgetProperties.OnApply(); };
    }


    /// <summary>
    /// Handles the OnNotAllowed event of the widgetProperties control.
    /// </summary>
    protected void widgetProperties_OnNotAllowed(object sender, EventArgs e)
    {
        RedirectToAccessDenied(GetString("widgets.security.notallowed"));
    }

    #endregion
}