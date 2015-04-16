using System;
using System.Linq;

using CMS.ApplicationDashboard;
using CMS.Core;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.WebAnalytics;

[assembly: RegisterLiveTileModelProvider(ModuleName.WEBANALYTICS, "CMSWebAnalytics", typeof (WebAnalyticsLiveTileModelProvider))]

namespace CMS.WebAnalytics
{
    /// <summary>
    /// Provides live model for the Web analytics dashboard tile.
    /// </summary>
    internal class WebAnalyticsLiveTileModelProvider : ILiveTileModelProvider
    {
        /// <summary>
        /// Loads number of visits in the last week.
        /// </summary>
        /// <param name="liveTileContext">Context of the live tile. Contains information about the user and the site the model is requested for</param>
        /// <exception cref="ArgumentNullException"><paramref name="liveTileContext"/> is null</exception>
        /// <returns>Live tile model</returns>
        public LiveTileModel GetModel(LiveTileContext liveTileContext)
        {
            if (liveTileContext == null)
            {
                throw new ArgumentNullException("liveTileContext");
            }

            return CacheHelper.Cache(() =>
            {
                if (!AnalyticsHelper.AnalyticsEnabled(liveTileContext.SiteInfo.SiteName))
                {
                    return null;
                }

                var weekVisitsCount = GetWeekVisitsCount(liveTileContext.SiteInfo.SiteID);

                return new LiveTileModel
                {
                    Value = weekVisitsCount,
                    Description = ResHelper.GetString("webanalytics.livetiledescription")
                };
            }, new CacheSettings(2, "WebAnalyticsLiveTileModelProvider", liveTileContext.SiteInfo.SiteID));
        }


        /// <summary>
        /// Gets number of visits in the last week.
        /// </summary>
        /// <param name="siteID">ID of the site</param>
        /// <returns>Number of visits</returns>
        private int GetWeekVisitsCount(int siteID)
        {
            return GetVisitCount(siteID, HitLogProvider.VISITORS_FIRST) + GetVisitCount(siteID, HitLogProvider.VISITORS_RETURNING);
        }


        /// <summary>
        /// Gets number of visitors for the last week based on statistics with given staticstics type.
        /// </summary>
        /// <param name="siteId">ID of the site</param>
        /// <param name="statisticsCode">Statistics code</param>
        /// <exception cref="ArgumentException"><paramref name="statisticsCode"/> is null or empty.</exception>
        /// <returns>Visitors count</returns>
        private int GetVisitCount(int siteId, string statisticsCode)
        {
            if (string.IsNullOrEmpty(statisticsCode))
            {
                throw new ArgumentException("statisticsCode");
            }

            var statistics = StatisticsInfoProvider.GetStatistics()
                                                   .OnSite(siteId)
                                                   .Column("StatisticsID")
                                                   .WhereEquals("StatisticsCode", statisticsCode);

            var hits = HitsDayInfoProvider.GetHitsDays()
                                          .WhereIn("HitsStatisticsID", statistics)
                                          .WhereGreaterThan("HitsStartTime", DateTime.Now.AddDays(-7).Date)
                                          .Column(new AggregatedColumn(AggregationType.Sum, "HitsCount"));

            int count = 0;
            if (!DataHelper.DataSourceIsEmpty(hits))
            {
                count = ValidationHelper.GetInteger(hits.Tables[0].Rows[0][0], 0);
            }

            return count;
        }
    }
}