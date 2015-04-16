using System;

using CMS.UIControls;

[EditedObject("om.mvtest", "objectID")]
[Security(Resource = "CMS.MVTest", UIElements = "MVTestListing;Detail;General")]
public partial class CMSModules_OnlineMarketing_Pages_Content_MVTest_Edit : CMSMVTestPage
{
    #region "Methods"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        // Set disabled module info
        ucDisabledModule.SettingsKeys = "CMSAnalyticsEnabled;CMSMVTEnabled";
        ucDisabledModule.ParentPanel = pnlDisabled;
    }

    #endregion
}