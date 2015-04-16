using CMS.Core;
using CMS.DataEngine;
using CMS.Base;
using CMS.UIControls;

[CheckLicence(FeatureEnum.WorkflowVersioning)]
[Security(Resource = "CMS.Content", UIElements = "CheckedOutDocs;CheckedOutObjects")]
[UIElement(ModuleName.CONTENT, "CheckedOutDocs")]
public partial class CMSModules_MyDesk_CheckedOut_Objects : CMSContentManagementPage
{
    protected override void OnLoad(System.EventArgs e)
    {
        base.OnLoad(e);

        ucDisabledModuleInfo.InfoText = GetString("teamdevelopment.checkoutnotenabled");
        ucDisabledModuleInfo.ParentPanel = pnlDisabled;
    }
}
