using System;

using CMS.Helpers;
using CMS.DocumentEngine;
using CMS.SiteProvider;
using CMS.UIControls;

public partial class CMSPages_BlogRss : XMLPage
{
    protected override void OnPreInit(EventArgs e)
    {
        base.OnPreInit(e);

        Response.ContentType = "text/xml";

        // Get parent blog alias path from url
        string blogAliasPath = QueryHelper.GetString("aliaspath", String.Empty);
        string validPath = TreePathUtils.GetSafeNodeAliasPath(blogAliasPath, SiteContext.CurrentSiteName).TrimEnd('/') + "/%";

        // Set blog path to the repetet to get appropiate posts
        repeater.Path = validPath;
    }
}