using CMS.Core;
using CMS.UIControls;
using CMS.DataEngine;

[CheckLicence(FeatureEnum.WorkflowVersioning)]
[Security(Resource = "CMS.Content", UIElements = "CheckedOutDocs")]
[UIElement(ModuleName.CONTENT, "CheckedOutDocs")]
public partial class CMSModules_MyDesk_CheckedOut_Frameset : CMSContentManagementPage
{
    protected override void OnLoad(System.EventArgs e)
    {
        base.OnLoad(e);

        frameset.FrameHeight = TabsOnlyHeight;
    }
}
