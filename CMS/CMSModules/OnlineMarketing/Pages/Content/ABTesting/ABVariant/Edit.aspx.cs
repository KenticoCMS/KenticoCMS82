using System;

using CMS.UIControls;

[Help("variant_edit")]
[EditedObject("om.abvariant", "variantId")]
[Breadcrumb(0, ResourceString = "abtesting.variant.list", TargetUrl = "~/CMSModules/OnlineMarketing/Pages/Content/ABTesting/ABVariant/List.aspx?objectID={?abTestID?}&nodeid={?nodeID?}")]
[Breadcrumb(1, Text = "{%EditedObject.DisplayName%}", ExistingObject = true)]
[Breadcrumb(1, ResourceString = "abtesting.variant.new", NewObject = true)]
[Security(Resource = "CMS.ABTest", UIElements = "Variants")]
public partial class CMSModules_OnlineMarketing_Pages_Content_ABTesting_ABVariant_Edit : CMSABTestPage
{
    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Set disabled module info
        ucDisabledModule.SettingsKeys = "CMSAnalyticsEnabled;CMSABTestingEnabled";
        ucDisabledModule.ParentPanel = pnlDisabled;
    }

    #endregion
}