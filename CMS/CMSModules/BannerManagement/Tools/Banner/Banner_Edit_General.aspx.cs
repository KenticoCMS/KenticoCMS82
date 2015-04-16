using System;

using CMS.Helpers;
using CMS.UIControls;
using CMS.BannerManagement;

// Edited object
[EditedObject(BannerInfo.OBJECT_TYPE, "objectid")]

// Parent object
[ParentObject(BannerCategoryInfo.OBJECT_TYPE, "parentobjectid")]
[UIElement("CMS.BannerManagement", "General_1")]
public partial class CMSModules_BannerManagement_Tools_Banner_Banner_Edit_General : CMSBannerManagementEditPage
{
}
