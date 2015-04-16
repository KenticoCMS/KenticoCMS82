using System;

using CMS.ApplicationDashboard;
using CMS.Core;
using CMS.Helpers;
using CMS.SiteProvider;
using CMS.Membership;
using CMS.DataEngine;
using CMS.SettingsProvider;

[assembly: RegisterLiveTileModelProvider("CMS.Users", "Administration.Users", typeof(UsersLiveTileModelProvider))]

namespace CMS.Membership
{
    /// <summary>
    /// Provides live model for the Users dashboard tile.
    /// </summary>
    internal class UsersLiveTileModelProvider : ILiveTileModelProvider
    {
        /// <summary>
        /// Loads model for the dashboard live tile.
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
                bool isApprovalEnabled = SettingsKeyInfoProvider.GetBoolValue("CMSRegistrationAdministratorApproval", liveTileContext.SiteInfo.SiteID);

                // Calculate only if approval process is enabled for new users
                if (isApprovalEnabled)
                {
                    var usersWaitingForApprovalCount = GetUsersWaitingForApprovalCount(liveTileContext.SiteInfo);

                    if (usersWaitingForApprovalCount != 0)
                    {
                        return new LiveTileModel
                        {
                            Value = usersWaitingForApprovalCount,
                            Description = ResHelper.GetString("users.livetiledescription"),
                        };
                    }
                    else return null;
                }
                else return null;

            }, new CacheSettings(5, "UsersLiveTileModelProvider", liveTileContext.SiteInfo.SiteID));
        }


        /// <summary>
        /// Gets number of users waiting for approval.
        /// </summary>
        /// <param name="site">Tile's site</param>
        /// <returns>Number of users waiting for approval</returns>
        private static int GetUsersWaitingForApprovalCount(SiteInfo site)
        {
            return UserInfoProvider.GetUsers()
                                   .Source(s => s.Join<UserSettingsInfo>("UserID", "UserSettingsUserID"))
                                   .OnSite(site.SiteID)
                                   .WhereEquals("UserWaitingForApproval", 1)
                                   .Count;
        }
    }
}