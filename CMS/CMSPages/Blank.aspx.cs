using System;
using System.Web.UI;

using CMS.URLRewritingEngine;

/// <summary>
/// Blank page used for full page rewrite to finish the request properly
/// </summary>
public partial class CMSPages_Blank : Page
{
    /// <summary>
    /// PreInit event handler
    /// </summary>
    protected override void OnPreInit(EventArgs e)
    {
        // Handle the rewrite interception
        if (URLRewriter.FixRewriteRedirect && URLRewriter.HandleRewriteRedirect())
        {
            URLRewriter.PerformPlannedRedirect();
        }

        base.OnPreInit(e);
    }
}
