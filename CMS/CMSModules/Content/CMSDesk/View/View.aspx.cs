using System;

using CMS.ExtendedControls;
using CMS.Helpers;
using CMS.PortalEngine;
using CMS.UIControls;
using CMS.Base;

public partial class CMSModules_Content_CMSDesk_View_View : CMSContentPage
{
    #region "Properties"

    /// <summary>
    /// Overridden messages placeholder for correct positioning
    /// </summary>
    public override MessagesPlaceHolder MessagesPlaceHolder
    {
        get
        {
            return plcMess;
        }
        set
        {
            plcMess = value;
        }
    }

    #endregion


    #region "Page events"

    /// <summary>
    /// OnPreInit
    /// </summary>
    protected override void OnPreInit(EventArgs e)
    {
        PortalContext.ViewMode = ViewModeEnum.Preview;
        DocumentManager.RedirectForNonExistingDocument = false;

        base.OnPreInit(e);
    }


    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        ScriptHelper.RegisterJQuery(Page);
        ScriptHelper.RegisterModule(Page, "CMS/HeaderShadow");

        ucView.ViewPage = DocumentUIHelper.GetViewPageUrl();
        ucView.RotateDevice = ValidationHelper.GetBoolean(CookieHelper.GetValue(CookieName.CurrentDeviceProfileRotate), false);

        // Setup Edit menu
        bool preview = PortalContext.ViewMode.IsPreview();

        editMenu.ShowProperties = false;
        editMenu.ShowSpellCheck = true;
        editMenu.ShowSave = !preview;
        editMenu.ShowCheckOut = !preview;
        editMenu.ShowCheckIn = !preview;
        editMenu.ShowUndoCheckOut = !preview;
        editMenu.ShowApplyWorkflow = !preview;
        editMenu.NodeID = NodeID;
        editMenu.CultureCode = CultureCode;
        editMenu.UseSmallIcons = true;
        editMenu.IsLiveSite = false;

        // Bind external buttons (i.e. Persona selector)
        var extensionTarget = editMenu as IExtensibleEditMenu;
        extensionTarget.InitializeExtenders("Content");

        const string deviceRotateScript = @"
$cmsj(document).ready(function () {
    if (window.CMSDeviceProfile) {
        CMSDeviceProfile.OnRotationFunction = (function() {
            CMSView.InitializeFrame(CMSView.PreviewWidth, CMSView.PreviewHeight, !CMSView.Rotated);
            CMSView.DeviceWindowResize();
        });
    }
});";

        ScriptHelper.RegisterStartupScript(this, typeof(String), "deviceRotateScript", deviceRotateScript, true);
    }

    #endregion
}