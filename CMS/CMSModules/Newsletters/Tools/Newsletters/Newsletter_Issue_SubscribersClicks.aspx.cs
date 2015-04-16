using System;
using System.Linq;
using System.Data;

using CMS.Helpers;
using CMS.LicenseProvider;
using CMS.Newsletters;
using CMS.SiteProvider;
using CMS.Membership;
using CMS.UIControls;
using CMS.DataEngine;
using CMS.Modules;

/// <summary>
/// Displays a table of subscribers who clicked a link in a specified issue.
/// </summary>
public partial class CMSModules_Newsletters_Tools_Newsletters_Newsletter_Issue_SubscribersClicks : CMSToolsModalPage
{
    #region "Variables"

    private int linkId;

    // Default page size 15
    private const int PAGESIZE = 15;

    #endregion


    #region "Methods"

    protected void Page_Init(object sender, EventArgs e)
    {
        // Check the license
        if (!string.IsNullOrEmpty(DataHelper.GetNotEmpty(RequestContext.CurrentDomain, string.Empty)))
        {
            LicenseHelper.CheckFeatureAndRedirect(RequestContext.CurrentDomain, FeatureEnum.Newsletters);
        }

        // Check site availability
        if (!ResourceSiteInfoProvider.IsResourceOnSite("CMS.Newsletter", SiteContext.CurrentSiteName))
        {
            RedirectToResourceNotAvailableOnSite("CMS.Newsletter");
        }

        var user = MembershipContext.AuthenticatedUser;

        // Check permissions for CMS Desk -> Tools -> Newsletter
        if (!user.IsAuthorizedPerUIElement("CMS.Newsletter", "Newsletter"))
        {
            RedirectToUIElementAccessDenied("CMS.Newsletter", "Newsletter");
        }

        // Check 'NewsletterRead' permission
        if (!user.IsAuthorizedPerResource("CMS.Newsletter", "Read"))
        {
            RedirectToAccessDenied("CMS.Newsletter", "Read");
        }
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        PageTitle.TitleText = GetString("newsletter_issue_subscribersclicks.title");
        linkId = QueryHelper.GetInteger("linkid", 0);
        if (linkId == 0)
        {
            RequestHelper.EndResponse();
        }

        LinkInfo link = LinkInfoProvider.GetLinkInfo(linkId);
        EditedObject = link;

        IssueInfo issue = IssueInfoProvider.GetIssueInfo(link.LinkIssueID);
        EditedObject = issue;

        // Prevent accessing issues from sites other than current site
        if (issue.IssueSiteID != SiteContext.CurrentSiteID)
        {
            RedirectToResourceNotAvailableOnSite("Issue with ID " + link.LinkIssueID);
        }

        var listingWhereCondition = new WhereCondition().Where("LinkID", QueryOperator.Equals, linkId);

        // Link's issue is the main A/B test issue
        if (issue.IssueIsABTest && !issue.IssueIsVariant)
        {
            // Get A/B test and its winner issue ID
            ABTestInfo test = ABTestInfoProvider.GetABTestInfoForIssue(issue.IssueID);
            if (test != null)
            {
                // Get ID of the same link from winner issue
                var winnerLink = LinkInfoProvider.GetLinks()
                                                 .WhereEquals("LinkIssueID", test.TestWinnerIssueID)
                                                 .WhereEquals("LinkTarget", link.LinkTarget)
                                                 .WhereEquals("LinkDescription", link.LinkDescription)
                                                 .TopN(1)
                                                 .Column("LinkID")
                                                 .FirstOrDefault();

                if (winnerLink != null)
                {
                    if (winnerLink.LinkID > 0)
                    {
                        // Add link ID of winner issue link
                        listingWhereCondition.Or(new WhereCondition().Where("LinkID", QueryOperator.Equals, winnerLink.LinkID));
                    }
                }
            }
        }
        var filterCondition = new WhereCondition(fltOpenedBy.WhereCondition)
        {
            // True ensures that where condition will be placed into brackets when combining with different WHERE condition
            WhereIsComplex = true
        };
        listingWhereCondition.And(filterCondition);

        UniGrid.WhereCondition = listingWhereCondition.WhereCondition;
        UniGrid.QueryParameters = listingWhereCondition.Parameters;
        UniGrid.Pager.DefaultPageSize = PAGESIZE;
        UniGrid.Pager.ShowPageSize = false;
        UniGrid.FilterLimit = 1;
        UniGrid.OnExternalDataBound += UniGrid_OnExternalDataBound;
    }


    protected object UniGrid_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        DataRowView row = (DataRowView)parameter;
        int subscriberId;

        switch (sourceName)
        {
            case "name":
                subscriberId = ValidationHelper.GetInteger(DataHelper.GetDataRowValue(row.Row, "SubscriberID"), 0);
                if (subscriberId == 0)
                {
                    // Get full name for contact group member (contact)
                    string name = ValidationHelper.GetString(DataHelper.GetDataRowValue(row.Row, "SubscriberFullName"), string.Empty);

                    // Return encoded name
                    return HTMLHelper.HTMLEncode(name);
                }
                else
                {
                    // Add the field transformation control that handles the translation
                    var tr = new ObjectTransformation("newsletter.subscriber", subscriberId);
                    tr.Transformation = "SubscriberFullName";

                    return tr;
                }

            case "email":
                subscriberId = ValidationHelper.GetInteger(DataHelper.GetDataRowValue(row.Row, "SubscriberID"), 0);
                string email = null;
                if (subscriberId == 0)
                {
                    // Get email for contact group member (contact)
                    email = ValidationHelper.GetString(DataHelper.GetDataRowValue(row.Row, "SubscriberEmail"), string.Empty);
                }
                else
                {
                    SubscriberInfo subscriber = SubscriberInfoProvider.GetSubscriberInfo(subscriberId);
                    if (subscriber != null)
                    {
                        if (subscriber.SubscriberType == null)
                        {
                            // Get email for classic subscriber
                            email = subscriber.SubscriberEmail;
                        }
                        else
                        {
                            switch (subscriber.SubscriberType)
                            {
                                case UserInfo.OBJECT_TYPE:
                                    UserInfo user = UserInfoProvider.GetUserInfo(subscriber.SubscriberRelatedID);
                                    if (user != null)
                                    {
                                        // Get email for user subscriber
                                        email = user.Email;
                                    }
                                    break;
                                case PredefinedObjectType.CONTACT:
                                    DataSet ds = ModuleCommands.OnlineMarketingGetContactForNewsletters(subscriber.SubscriberRelatedID, "ContactEmail");
                                    if (!DataHelper.DataSourceIsEmpty(ds))
                                    {
                                        // Get email from contact subscriber
                                        email = ValidationHelper.GetString(ds.Tables[0].Rows[0]["ContactEmail"], string.Empty);
                                    }
                                    break;
                            }
                        }
                    }
                }

                if (!string.IsNullOrEmpty(email))
                {
                    // Return encoded email
                    email = HTMLHelper.HTMLEncode(email);
                }

                return email;

            default:
                return parameter;
        }
    }

    #endregion
}