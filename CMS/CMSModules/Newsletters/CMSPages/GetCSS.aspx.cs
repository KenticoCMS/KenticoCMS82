using System;
using System.Web;

using CMS.Helpers;
using CMS.Newsletters;
using CMS.PortalEngine;
using CMS.Base;
using CMS.SiteProvider;
using CMS.UIControls;

public partial class CMSModules_Newsletters_CMSPages_GetCSS : GetFilePage
{
    #region "Variables"

    protected bool useClientCache = true;

    protected CMSOutputResource outputFile = null;
    protected CssStylesheetInfo si = null;
    protected EmailTemplateInfo et = null;
    private string newsletterTemplateName;

    #endregion


    #region "Properties"

    /// <summary>
    /// Returns true if the process allows cache.
    /// </summary>
    public override bool AllowCache
    {
        get
        {
            if (mAllowCache == null)
            {
                // By default, cache for the newsletter CSS is always enabled (even outside of the live site)
                mAllowCache = ValidationHelper.GetBoolean(SettingsHelper.AppSettings["CMSAlwaysCacheNewsletterCSS"], true) || IsLiveSite;
            }

            return mAllowCache.Value;
        }
        set
        {
            mAllowCache = value;
        }
    }

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Check the site
        if (string.IsNullOrEmpty(CurrentSiteName))
        {
            throw new Exception("[GetCSS.aspx]: Site not running.");
        }

        newsletterTemplateName = QueryHelper.GetString("newslettertemplatename", string.Empty);
        string cacheKey = string.Format("getnewslettercss|{0}|{1}", SiteContext.CurrentSiteName, newsletterTemplateName);

        // Try to get data from cache
        using (var cs = new CachedSection<CMSOutputResource>(ref outputFile, CacheMinutes, true, cacheKey))
        {
            if (cs.LoadData)
            {
                // Process the file
                ProcessStylesheet();

                // Ensure the cache settings
                if ((outputFile != null) && cs.Cached)
                {
                    // Add cache dependency
                    var cd = CacheHelper.GetCacheDependency(new[] { "newsletter.emailtemplate|byname|" + newsletterTemplateName.ToLowerCSafe() });

                    // Cache the data
                    cs.CacheDependency = cd;
                }

                cs.Data = outputFile;
            }
        }

        if (outputFile != null)
        {
            // Send the data
            SendFile(outputFile);
        }
    }


    /// <summary>
    /// Processes the stylesheet.
    /// </summary>
    protected void ProcessStylesheet()
    {
        // Newsletter template stylesheet
        if (!string.IsNullOrEmpty(newsletterTemplateName))
        {
            // Get the template
            et = EmailTemplateInfoProvider.GetEmailTemplateInfo(newsletterTemplateName, SiteContext.CurrentSiteID);
            if (et != null)
            {
                // Create the output file
                outputFile = new CMSOutputResource
                {
                    Name = RequestContext.URL.ToString(),
                    Data = HTMLHelper.ResolveCSSUrls(et.TemplateStylesheetText, SystemContext.ApplicationPath),
                    Etag = et.TemplateName
                };
            }
        }
    }


    /// <summary>
    /// Sends the given file within response.
    /// </summary>
    /// <param name="file">File to send</param>
    protected void SendFile(CMSOutputResource file)
    {
        // Clear response.
        CookieHelper.ClearResponseCookies();
        Response.Clear();

        Response.Cache.SetRevalidation(HttpCacheRevalidation.AllCaches);

        // Send the file
        if ((file != null) && (file.Data != null))
        {
            // Prepare response
            Response.ContentType = "text/css";

            // Client caching - only on the live site
            if (useClientCache && AllowCache && CacheHelper.CacheImageEnabled(CurrentSiteName) && ETagsMatch(file.Etag, file.LastModified))
            {
                RespondNotModified(file.Etag);
                return;
            }

            if (useClientCache && AllowCache && CacheHelper.CacheImageEnabled(CurrentSiteName))
            {
                SetTimeStamps(file.LastModified);

                Response.Cache.SetETag(file.Etag);
            }

            // Add the file data
            Response.Write(file.Data);
        }

        CompleteRequest();
    }

    #endregion
}