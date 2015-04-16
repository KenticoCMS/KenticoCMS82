using System;

using CMS.Base;
using CMS.DocumentEngine;

/// <summary>
/// Sample handler module. 
/// </summary>
[SampleHandlerModuleLoader]
public partial class CMSModuleLoader
{
    // You can copy additional samples from the \CodeSamples\App_Code Samples\ folder in your
    // Kentico installation directory (by default C:\Program Files\KenticoCMS\<version>).

    #region "Macro methods loader attribute"

    /// <summary>
    /// Module registration
    /// </summary>
    private class SampleHandlerModuleLoader : CMSLoaderAttribute
    {
        /// <summary>
        /// Initializes the module
        /// </summary>
        public override void Init()
        {
            // DocumentEvents.Insert.Before += Insert_Before;
        }


        /// <summary>
        /// Sample before insert handler which uppercases the document name
        /// </summary>
        private void Insert_Before(object sender, DocumentEventArgs e)
        {
            var doc = e.Node;
            doc.DocumentName = doc.DocumentName.ToUpper();
        }
    }

    #endregion
}