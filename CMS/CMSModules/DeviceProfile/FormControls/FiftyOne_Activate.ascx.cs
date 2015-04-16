using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.FormControls;
using CMS.ExtendedControls;

public partial class CMSModules_DeviceProfile_FormControls_FiftyOne_Activate : ReadOnlyFormEngineUserControl
{
    #region "Events"

    protected void Page_Load(object sender, EventArgs e)
    {
        elemActivate.CssClass = "FiftyOneContainer";
        elemActivate.LogoEnabled = false;
        elemActivate.ButtonCssClass = "btn btn-default";
        elemActivate.FooterCssClass = "FiftyOneFooter";
        elemActivate.ActivateFileUploadData.CssClass = "FiftyOneUpload";

        elemActivate.ActivateButtonText = GetString("device_profile.settings.activatebuttontext");
        elemActivate.ActivatedMessageHtml = GetString("device_profile.settings.activatedmessagehtml");
        elemActivate.ActivateInstructionsHtml = GetString("device_profile.settings.activateinstructionshtml");
        elemActivate.ActivationDataInvalidHtml = GetString("device_profile.settings.activationdatainvalidhtml");
        elemActivate.ActivationFailureCouldNotUpdateConfigHtml = GetString("device_profile.settings.activationdatainvalidhtml");
        elemActivate.ActivationFailureCouldNotWriteDataFileHtml = GetString("device_profile.settings.activationfailurecouldnotwritedatafilehtml");
        elemActivate.ActivationFailureCouldNotWriteLicenceFileHtml = GetString("device_profile.settings.activationfailurecouldnotwritelicencefilehtml");
        elemActivate.ActivationFailureGenericHtml = GetString("device_profile.settings.activationfailuregenerichtml");
        elemActivate.ActivationFailureHttpHtml = GetString("device_profile.settings.activationfailurehttphtml");
        elemActivate.ActivationFailureInvalidHtml = GetString("device_profile.settings.activationfailureinvalidhtml");
        elemActivate.ActivationStreamFailureHtml = GetString("device_profile.settings.activationstreamfailurehtml");
        elemActivate.ActivationSuccessHtml = GetString("device_profile.settings.activationsuccesshtml");

        elemActivate.UploadInstructionsHtml = GetString("device_profile.settings.uploadinstructionshtml");
        elemActivate.RefreshButtonText = GetString("device_profile.settings.refreshbuttontext");

        elemActivate.ValidationFileErrorText = GetString("device_profile.settings.validationfileerrortext");
        elemActivate.ValidationRequiredErrorText = GetString("device_profile.settings.validationrequirederrortext");
        elemActivate.ValidationRegExErrorText = GetString("device_profile.settings.validationregexerrortext");
        
        elemActivate.ShareUsageText = GetString("device_profile.settings.shareusagetext");
        elemActivate.ShareUsageTrueHtml = GetString("device_profile.settings.shareusagetruehtml");
        elemActivate.ShareUsageLinkText = GetString("device_profile.settings.shareusagelinktext");
        elemActivate.ShareUsageErrorHtml = GetString("device_profile.settings.shareusageerrorhtml");

        elemActivate.TextBoxCssClass = "form-control";
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        Button btnRefresh = ControlsHelper.GetChildControl(elemActivate, typeof(Button), "ButtonRefresh") as Button;
        if (btnRefresh != null)
        {
            btnRefresh.CssClass = "btn btn-default";
            btnRefresh.Text = "ContentButton";
        }
    }

    #endregion
}