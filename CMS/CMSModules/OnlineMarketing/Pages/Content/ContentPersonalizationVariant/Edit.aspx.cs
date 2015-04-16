using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.OnlineMarketing;
using CMS.UIControls;

// Edited object
[EditedObject(ContentPersonalizationVariantInfo.OBJECT_TYPE, "variantid")]
// Breadcrumbs
[Breadcrumbs()]
[Breadcrumb(0, "contentpersonalizationvariant.list", "~/CMSModules/OnlineMarketing/Pages/Content/ContentPersonalizationVariant/List.aspx?nodeid={?nodeid?}", null)]
[Breadcrumb(1, Text = "{%EditedObject.DisplayName%}", ExistingObject = true)]
[Breadcrumb(1, ResourceString = "contentpersonalizationvariant.new", NewObject = true)]
[Help("cpvariant_edit")]
public partial class CMSModules_OnlineMarketing_Pages_Content_ContentPersonalizationVariant_Edit : CMSContentPersonalizationContentPage
{
}