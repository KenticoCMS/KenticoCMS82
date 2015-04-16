using System;

using CMS.Helpers;
using CMS.PortalEngine;
using CMS.UIControls;

public partial class CMSModules_Widgets_LiveDialogs_WidgetProperties_Properties : CMSWidgetPropertiesLivePage
{
    /// <summary>
    /// OnInit event.
    /// </summary>
    protected override void OnPreInit(EventArgs e)
    {
        base.OnPreInit(e);

        CurrentMaster.BodyClass += " WidgetsProperties";
    }


    /// <summary>
    /// Load event handler.
    /// </summary>
    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

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
        widgetProperties.ZoneType = zoneType;
        widgetProperties.IsLiveSite = true;
        widgetProperties.CurrentPageInfo = CurrentPageInfo;

        widgetProperties.OnNotAllowed += widgetProperties_OnNotAllowed;

        // Register the OnSave event handler
        FramesManager.OnSave += (sender, arg) => { return widgetProperties.OnSave(); };

        // Register the OnApply event handler
        FramesManager.OnApply += (sender, arg) => { return widgetProperties.OnApply(); };
    }


    /// <summary>
    /// Handles the OnNotAllowed event of the widgetProperties control.
    /// </summary>
    protected void widgetProperties_OnNotAllowed(object sender, EventArgs e)
    {
        RedirectToAccessDenied(GetString("widgets.security.notallowed"));
    }
}