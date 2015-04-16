using System;

using CMS.Helpers;
using CMS.Newsletters;
using CMS.Base;
using CMS.SiteProvider;
using CMS.Membership;
using CMS.UIControls;
using CMS.WebAnalytics;
using CMS.DataEngine;

public partial class CMSModules_Newsletters_CMSPages_Unsubscribe : CMSPage
{
    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Get data from query string
        Guid subscriberGuid = QueryHelper.GetGuid("subscriberguid", Guid.Empty);
        Guid newsletterGuid = QueryHelper.GetGuid("newsletterguid", Guid.Empty);
        string subscriptionHash = QueryHelper.GetString("subscriptionhash", string.Empty);
        Guid issueGuid = QueryHelper.GetGuid("issueGuid", Guid.Empty);
        int issueID = QueryHelper.GetInteger("issueid", 0);
        int contactId = QueryHelper.GetInteger("contactid", 0);
        bool unsubscribed = false;

        string requestTime = QueryHelper.GetString("datetime", string.Empty);
        DateTime datetime = DateTimeHelper.ZERO_TIME;

        // Get date and time
        if (!string.IsNullOrEmpty(requestTime))
        {
            try
            {
                datetime = DateTime.ParseExact(requestTime, SecurityHelper.EMAIL_CONFIRMATION_DATETIME_FORMAT, null);
            }
            catch
            {
                ShowError(GetString("newsletter.unsubscribefailed"));
                return;
            }
        }

        // Get site ID
        int siteId = 0;
        if (SiteContext.CurrentSite != null)
        {
            siteId = SiteContext.CurrentSiteID;
        }

        if ((subscriberGuid != Guid.Empty) && (newsletterGuid != Guid.Empty) && (siteId != 0))
        {
            SubscriberInfo subscriber = SubscriberInfoProvider.GetSubscriberInfo(subscriberGuid, siteId);
            if (subscriber == null)
            {
                ShowError(GetString("Unsubscribe.SubscriberDoesNotExist"));
                return;
            }
            // Show error message if subscriber type is 'Role'
            if (!string.IsNullOrEmpty(subscriber.SubscriberType) && subscriber.SubscriberType.EqualsCSafe(RoleInfo.OBJECT_TYPE, true))
            {
                ShowError(GetString("Unsubscribe.CannotUnsubscribeRole"));
                return;
            }

            NewsletterInfo newsletter = NewsletterInfoProvider.GetNewsletterInfo(newsletterGuid, siteId);
            if (newsletter == null)
            {
                ShowError(GetString("Unsubscribe.NewsletterDoesNotExist"));
                return;
            }

            // Check if subscriber with given GUID is subscribed to specified newsletter
            if (SubscriberInfoProvider.IsSubscribed(subscriber.SubscriberID, newsletter.NewsletterID))
            {
                bool isSubscribed = true;

                if (string.IsNullOrEmpty(subscriber.SubscriberType) 
                    || (!subscriber.SubscriberType.EqualsCSafe(PredefinedObjectType.CONTACTGROUP, true)
                        && !subscriber.SubscriberType.EqualsCSafe(PredefinedObjectType.PERSONA, true)))
                {
                    // Unsubscribe action
                    SubscriberInfoProvider.Unsubscribe(subscriber.SubscriberID, newsletter.NewsletterID);
                }
                else
                {
                    // Check if the contact group member has unsubscription activity for the specified newsletter
                    isSubscribed = (contactId > 0) && !ModuleCommands.OnlineMarketingIsContactUnsubscribed(contactId, newsletter.NewsletterID, siteId);
                }

                if (isSubscribed)
                {
                    // Log newsletter unsubscription activity
                    LogActivity(subscriber, newsletter.NewsletterID, issueID, contactId);

                    // Display confirmation
                    ShowInformation(GetString("Unsubscribe.Unsubscribed"));
                    unsubscribed = true;
                }
                else
                {
                    // Contact group member is already unsubscribed
                    ShowError(GetString("Unsubscribe.NotSubscribed"));
                }
            }
            else
            {
                ShowError(GetString("Unsubscribe.NotSubscribed"));
            }
        }
        // Check if subscription approval hash is supplied
        else if (!string.IsNullOrEmpty(subscriptionHash))
        {
            SubscriberNewsletterInfo sni = SubscriberNewsletterInfoProvider.GetSubscriberNewsletterInfo(subscriptionHash);
            // Check if hash is valid
            if ((sni != null) && sni.SubscriptionEnabled)
            {
                SubscriberInfoProvider.ApprovalResult result = SubscriberInfoProvider.Unsubscribe(subscriptionHash, true, SiteContext.CurrentSiteName, datetime);

                switch (result)
                {
                    // Approving subscription was successful
                    case SubscriberInfoProvider.ApprovalResult.Success:
                        bool isSubscribed = true;

                        // Get subscriber
                        SubscriberInfo subscriber = SubscriberInfoProvider.GetSubscriberInfo(sni.SubscriberID);
                        if ((subscriber != null) && !string.IsNullOrEmpty(subscriber.SubscriberType) && (subscriber.SubscriberType.EqualsCSafe(PredefinedObjectType.CONTACTGROUP, true) || subscriber.SubscriberType.EqualsCSafe(PredefinedObjectType.PERSONA, true)))
                        {
                            // Check if the contact group member has unsubscription activity for the specified newsletter
                            isSubscribed = (contactId > 0) && !ModuleCommands.OnlineMarketingIsContactUnsubscribed(contactId, sni.NewsletterID, siteId);
                        }

                        if (isSubscribed)
                        {
                            // Log newsletter unsubscription activity
                            LogActivity(subscriber, sni.NewsletterID, issueID, contactId);

                            // Display confirmation
                            ShowInformation(GetString("Unsubscribe.Unsubscribed"));
                            unsubscribed = true;
                        }
                        else
                        {
                            // Contact group member is already unsubscribed
                            ShowError(GetString("Unsubscribe.NotSubscribed"));
                        }
                        break;

                    // Subscription was already approved
                    case SubscriberInfoProvider.ApprovalResult.Failed:
                        ShowError(GetString("newsletter.unsubscribefailed"));
                        break;

                    case SubscriberInfoProvider.ApprovalResult.TimeExceeded:
                        ShowError(GetString("newsletter.approval_timeexceeded"));
                        break;

                    // Subscription not found
                    default:
                    case SubscriberInfoProvider.ApprovalResult.NotFound:
                        ShowError(GetString("Unsubscribe.NotSubscribed"));
                        break;
                }
            }
            else
            {
                ShowError(GetString("Unsubscribe.NotSubscribed"));
            }
        }
        else
        {
            if (subscriberGuid == Guid.Empty)
            {
                ShowError(GetString("Unsubscribe.SubscriberDoesNotExist"));
            }
            if (newsletterGuid == Guid.Empty)
            {
                ShowError(GetString("Unsubscribe.NewsletterDoesNotExist"));
            }
        }

        // Increase unsubscribed count
        if (unsubscribed)
        {
            // If Issue ID was provided
            if (issueID > 0)
            {
                IssueInfoProvider.IncreaseUnsubscribeCount(issueID);
                return;
            }
            // Otherwise try using the Issue GUID
            if (issueGuid != Guid.Empty)
            {
                IssueInfo issue = IssueInfoProvider.GetIssueInfo(issueGuid, siteId);
                if (issue == null)
                {
                    return;
                }

                IssueInfoProvider.IncreaseUnsubscribeCount(issue.IssueID);
            }
        }
    }


    /// <summary>
    /// Logs activity for unsubscribing.
    /// </summary>
    /// <param name="subscriber">Subscriber</param>
    /// <param name="newsletterId">Newsletter ID</param>
    /// <param name="issueId">Issue ID</param>
    /// <param name="contactId">Contact ID is present if the mail is sent to a contact or a contact group</param>
    private void LogActivity(SubscriberInfo subscriber, int newsletterId, int issueId, int contactId)
    {
        if (subscriber == null)
        {
            throw new ArgumentNullException("subscriber");
        }
        if (contactId <= 0)
        {
            contactId = ActivityTrackingHelper.GetContactID(subscriber);
        }

        if (contactId > 0)
        {
            NewsletterInfo news = NewsletterInfoProvider.GetNewsletterInfo(newsletterId);
            bool isFromContactGroup = subscriber.SubscriberType.EqualsCSafe(PredefinedObjectType.CONTACTGROUP, true);
            bool isFromPersona = subscriber.SubscriberType.EqualsCSafe(PredefinedObjectType.PERSONA, true);
            Activity activity = new ActivityNewsletterUnsubscribing(subscriber, news, AnalyticsContext.ActivityEnvironmentVariables);
            if (activity.Data != null)
            {
                activity.Data.ContactID = contactId;
                activity.Data.ItemDetailID = issueId;
                if (isFromContactGroup)
                {
                    activity.Data.Value = "contactgroup(" + subscriber.SubscriberRelatedID + ")";
                }
                else if (isFromPersona)
                {
                    activity.Data.Value = "persona(" + subscriber.SubscriberRelatedID + ")";
                }
                activity.Log();
            }
        }
    }

    #endregion
}