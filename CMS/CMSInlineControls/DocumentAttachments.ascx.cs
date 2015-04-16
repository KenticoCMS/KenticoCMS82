using System;
using CMS.DataEngine;
using CMS.DocumentEngine;
using CMS.ExtendedControls;
using CMS.PortalEngine;
using CMS.Base;
using CMS.SiteProvider;

public partial class CMSInlineControls_DocumentAttachments : InlineUserControl
{
    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        TreeNode currentDocument = DocumentContext.CurrentDocument;
        if (currentDocument != null)
        {
            // Get document type transformation
            string transformationName = currentDocument.NodeClassName + ".attachment";
            TransformationInfo ti = TransformationInfoProvider.GetTransformation(transformationName);
            // If transformation not present, use default from the Root document type
            if (ti == null)
            {
                transformationName = "cms.root.attachment";
                ti = TransformationInfoProvider.GetTransformation(transformationName);
            }

            if (ti == null)
            {
                throw new Exception("[DocumentAttachments]: Default transformation '" + transformationName + "' doesn't exist!");
            }

            ucAttachments.TransformationName = transformationName;
            ucAttachments.SiteName = SiteContext.CurrentSiteName;
            ucAttachments.Path = currentDocument.NodeAliasPath;
            ucAttachments.CultureCode = currentDocument.DocumentCulture;
            ucAttachments.OrderBy = "AttachmentOrder, AttachmentName";
            ucAttachments.PageSize = 0;
            ucAttachments.GetBinary = false;
            ucAttachments.CacheMinutes = SettingsKeyInfoProvider.GetIntValue(SiteContext.CurrentSite + ".CMSCacheMinutes");
        }
    }

    #endregion
}