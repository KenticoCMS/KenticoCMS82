using System;
using System.Collections.Generic;
using System.Web.Mvc;

using CMS.DataEngine;
using CMS.DocumentEngine;
using CMS.Helpers;
using CMS.Localization;
using CMS.SiteProvider;
using CMS.DocumentEngine.Types;

namespace CMS.Controllers
{
    /// <summary>
    /// Sample controller for the news.
    /// </summary>
    public class NewsController : Controller
    {
        #region "Actions"

        /// <summary>
        /// Process the list action.
        /// </summary>
        public ActionResult List()
        {
            // Prepare the data for a view
            var newsList = GetNewsList();

            if (newsList != null)
            {
                if (DocumentContext.CurrentPageInfo != null)
                {
                    // Setup the page title
                    ViewBag.PageTitle = DocumentContext.CurrentPageInfo.DocumentPageTitle ?? "MVC news list";
                }

                // Create the news list view
                return View("~/Views/CorporateSite/News/List.cshtml", newsList);
            }

            return HttpNotFound();
        }


        /// <summary>
        /// Process the detail action.
        /// </summary>
        public ActionResult Detail(string alias)
        {
            // Prepare the data for a view
            var document = GetNewsDocument(alias);

            if (document != null)
            {
                // Setup the page title
                ViewBag.PageTitle = document.GetValue("DocumentPageTitle") ?? "MVC news detail";

                // Create the detail view for given document
                return View("~/Views/CorporateSite/News/Detail.cshtml", document);
            }

            // Document not found
            return HttpNotFound();
        }

        #endregion


        #region "Data retrieval methods"

        /// <summary>
        /// Gets the news list.
        /// </summary>
        private static InfoDataSet<News> GetNewsList()
        {
            // Try to get the data from cache
            var newsList = CacheHelper.Cache(
                cs =>
                {
                    // Define the news parent document
                    string newsNodeAlias = "/News";

                    // Get the news documents
                    var result = DocumentHelper.GetDocuments<News>()
                        .Path(newsNodeAlias, PathTypeEnum.Children)
                        .OnCurrentSite()
                        .Published()
                        .PublishedVersion()
                        .TypedResult;

                    // Setup the cache dependencies only when caching is active
                    if (cs.Cached)
                    {
                        // Get the news parent document
                        var rootDoc = DocumentHelper.GetDocuments("CMS.MenuItem")
                            .Path(newsNodeAlias)
                            .OnCurrentSite()
                            .Published()
                            .TopN(1)
                            .FirstObject;

                        if (rootDoc != null)
                        {
                            // Set the cache dependencies for the root news document. This will cover also all its child news documents by default.
                            var nodeDependencies = TreeProvider.GetDependencyCacheKeys(rootDoc, SiteContext.CurrentSiteName);
                            cs.CacheDependency = CacheHelper.GetCacheDependency(nodeDependencies);
                        }
                    }

                    return result;
                },
                new CacheSettings(GetCacheMinutes(), GetCacheKey("newslist"))
            );

            return newsList;
        }


        /// <summary>
        /// Gets a specific news document.
        /// </summary>
        /// <param name="aliasPath">The alias path</param>
        private static News GetNewsDocument(string aliasPath)
        {
            if (string.IsNullOrEmpty(aliasPath))
            {
                return null;
            }

            // Try to get the data from cache
            var document = CacheHelper.Cache(
                cs =>
                {
                    // Get the news document
                    var result = DocumentHelper.GetDocuments<News>()
                                .Path("/News/" + aliasPath)
                                .OnCurrentSite()
                                .Published()
                                .TopN(1)
                                .FirstObject;

                    // Setup the cache dependencies only when caching is active
                    if ((result != null) && cs.Cached)
                    {
                        // Set the cache dependencies
                        var nodeDependencies = TreeProvider.GetDependencyCacheKeys(result, SiteContext.CurrentSiteName);
                        cs.CacheDependency = CacheHelper.GetCacheDependency(nodeDependencies);
                    }

                    return result;
                },
                new CacheSettings(GetCacheMinutes(), GetCacheKey("newsdetail", aliasPath))
            );

            return document;
        }


        /// <summary>
        /// Generates the cache key according to the given cache key parameters.
        /// </summary>
        /// <param name="cacheItemNameParts">Cache key parts which will be used in the generated cache key</param>
        private static string GetCacheKey(params string[] cacheItemNameParts)
        {
            List<string> cacheKeyParts = new List<string>();

            // Set the default cache key prefix. This ensures that the cached data will be site, domain and culture specific.
            cacheKeyParts.Add(SiteContext.CurrentSiteName);
            cacheKeyParts.Add(RequestContext.CurrentDomain);
            cacheKeyParts.Add(LocalizationContext.PreferredCultureCode);

            // Combine cache name keys
            cacheKeyParts.AddRange(cacheItemNameParts);

            return CacheHelper.BuildCacheItemName(cacheKeyParts);
        }


        /// <summary>
        /// Gets the default content cache minutes set for the current site.
        /// </summary>
        private static int GetCacheMinutes()
        {
            return CacheHelper.CacheMinutes(SiteContext.CurrentSiteName);
        }

        #endregion
    }
}