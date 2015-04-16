using System;
using CMS.Base;
using CMS.PortalEngine;

namespace CMS.Controls
{
    /// <summary>
    /// Base class for transformation methods
    /// </summary>
    public partial class CMSTransformation : CMSAbstractTransformation
    {
    }
}


/// <summary>
/// CMSTransforamtionModule
/// </summary>
[CMSTransformationModuleLoader]
public partial class CMSModuleLoader
{
    /// <summary>
    /// Module loader identify CMSTransformation class and set as default base class in default application
    /// </summary>
    private class CMSTransformationModuleLoader : CMSLoaderAttribute
    {
        /// <summary>
        /// Module Init
        /// </summary>
        public override void Init()
        {
            // Set CMSTransformation as a base class
            TransformationInfoProvider.TransformationBaseClass = "CMS.Controls.CMSTransformation";
            base.Init();
        }
    }
}
