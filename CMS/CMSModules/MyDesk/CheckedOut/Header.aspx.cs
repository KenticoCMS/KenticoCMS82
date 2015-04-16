using CMS.UIControls;
using CMS.Base;
using CMS.DataEngine;

[CheckLicence(FeatureEnum.WorkflowVersioning)]
[Security(Resource = "CMS.Content", UIElements = "CheckedOutDocs")]
[Tabs("CMS.Content", "CheckedOutDocs", "content")]
public partial class CMSModules_MyDesk_CheckedOut_Header : CMSContentManagementPage
{
    protected override void OnLoad(System.EventArgs e)
    {
        base.OnLoad(e);

        var title = GetString("MyDesk.CheckedOutTitle");
        Title = title;
        PageTitle.TitleText = title;
    }
}
