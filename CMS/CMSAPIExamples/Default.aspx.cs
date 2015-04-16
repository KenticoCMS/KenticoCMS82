using System;

using CMS.Helpers;
using CMS.PortalEngine;
using CMS.UIControls;

public partial class CMSAPIExamples_Default : CMSAPIExamplePage
{
    /// <summary>
    /// Gets the Api examples application URL
    /// </summary>
    protected string ApiExamplesApplicationUrl
    {
        get
        {
            return URLHelper.ResolveUrl(UIContextHelper.GetApplicationUrl("cms", "APIExamples"));
        }
    }
}