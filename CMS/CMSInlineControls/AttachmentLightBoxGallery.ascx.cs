using System;
using CMS.DataEngine;
using CMS.DocumentEngine;
using CMS.ExtendedControls;
using CMS.PortalEngine;
using CMS.Base;
using CMS.SiteProvider;
using CMS.UIControls;

public partial class CMSInlineControls_AttachmentLightBoxGallery : InlineUserControl
{
    #region "Variables"

    private CMSUserControl ucAttachments = null;

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        ucAttachments = Page.LoadUserControl("~/CMSModules/Content/Controls/Attachments/AttachmentLightboxGallery.ascx") as CMSUserControl;
        ucAttachments.ID = "ctrlAttachments";
        plcAttachmentLightBox.Controls.Add(ucAttachments);

        TreeNode currentDocument = DocumentContext.CurrentDocument;
        if (currentDocument != null)
        {
            // Get document type transformation
            string transformationName = currentDocument.NodeClassName + ".AttachmentLightbox";
            string selectedTransformationName = currentDocument.NodeClassName + ".AttachmentLightboxDetail";
            TransformationInfo ti = TransformationInfoProvider.GetTransformation(transformationName);

            // If transformation not present, use default from the Root document type
            if (ti == null)
            {
                transformationName = "cms.root.AttachmentLightbox";
                ti = TransformationInfoProvider.GetTransformation(transformationName);
            }
            if (ti == null)
            {
                throw new Exception("[DocumentAttachments]: Default transformation '" + transformationName + "' doesn't exist!");
            }
            ti = TransformationInfoProvider.GetTransformation(selectedTransformationName);

            // If transformation not present, use default from the Root document type
            if (ti == null)
            {
                selectedTransformationName = "cms.root.AttachmentLightboxDetail";
                ti = TransformationInfoProvider.GetTransformation(selectedTransformationName);
            }
            if (ti == null)
            {
                throw new Exception("[DocumentAttachments]: Default transformation '" + selectedTransformationName + "' doesn't exist!");
            }

            ucAttachments.SetValue("TransformationName", transformationName);
            ucAttachments.SetValue("SelectedItemTransformationName", selectedTransformationName);
            ucAttachments.SetValue("SiteName", SiteContext.CurrentSiteName);
            ucAttachments.SetValue("Path", currentDocument.NodeAliasPath);
            ucAttachments.SetValue("CultureCode", currentDocument.DocumentCulture);
            ucAttachments.SetValue("OrderBy", "AttachmentOrder, AttachmentName");
            ucAttachments.SetValue("PageSize", 0);
            ucAttachments.SetValue("GetBinary", false);
            ucAttachments.SetValue("CacheMinutes", SettingsKeyInfoProvider.GetIntValue(SiteContext.CurrentSite + ".CMSCacheMinutes"));
        }
    }

    #endregion
}