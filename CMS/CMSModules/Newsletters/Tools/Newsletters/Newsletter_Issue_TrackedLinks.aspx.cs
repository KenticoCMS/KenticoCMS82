using System;
using System.Collections;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.Helpers;
using CMS.LicenseProvider;
using CMS.Newsletters;
using CMS.Base;
using CMS.SiteProvider;
using CMS.Membership;
using CMS.UIControls;
using CMS.ExtendedControls;
using CMS.DataEngine;
using CMS.Modules;

/// <summary>
/// Displays a table of clicked links.
/// </summary>
public partial class CMSModules_Newsletters_Tools_Newsletters_Newsletter_Issue_TrackedLinks : CMSToolsModalPage
{
    #region "Variables"

    private int issueId;

    private int sentEmails;

    private bool isMainABTestIssue;

    // Default page size 15
    private const int PAGESIZE = 15;

    private DataSet winnerData;

    // It's filled in grid's external data bound with info if action buttons should be visible
    private readonly Hashtable showGridAction = new Hashtable();

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
        PageTitle.TitleText = GetString("newsletter_issue_trackedlinks.title");
        issueId = QueryHelper.GetInteger("objectid", 0);
        if (issueId == 0)
        {
            RequestHelper.EndResponse();
        }

        IssueInfo issue = IssueInfoProvider.GetIssueInfo(issueId);
        EditedObject = issue;

        // Prevent accessing issues from sites other than current site
        if (issue.IssueSiteID != SiteContext.CurrentSiteID)
        {
            RedirectToResourceNotAvailableOnSite("Issue with ID " + issueId);
        }

        // Get number of sent emails
        sentEmails = issue.IssueSentEmails;

        ScriptHelper.RegisterDialogScript(this);

        string scriptBlock = string.Format(@"
            function OpenTarget(url) {{ window.open(url, 'LinkTarget'); return false; }}
            function ViewClicks(id) {{ modalDialog('{0}?linkid=' + id, 'NewsletterIssueSubscriberClicks', '900px', '700px');  return false; }}",
                                           ResolveUrl(@"~\CMSModules\Newsletters\Tools\Newsletters\Newsletter_Issue_SubscribersClicks.aspx"));
        ScriptHelper.RegisterClientScriptBlock(this, GetType(), "Actions", scriptBlock, true);

        // Issue is the main A/B test issue
        isMainABTestIssue = issue.IssueIsABTest && !issue.IssueIsVariant;
        if (isMainABTestIssue)
        {
            // Initialize variant selector in the filter
            fltLinks.IssueId = issue.IssueID;

            if (RequestHelper.IsPostBack())
            {
                // Get issue ID from variant selector
                issueId = fltLinks.IssueId;
            }
        }

        string whereCondition = string.Empty;
        if (issueId > 0)
        {
            // Filter by Issue ID (from querystring or variant selector in case of A/B test issue)
            whereCondition = SqlHelper.GetWhereCondition("IssueID", issueId);
        }

        UniGrid.WhereCondition = SqlHelper.AddWhereCondition(whereCondition, fltLinks.WhereCondition);
        UniGrid.Pager.DefaultPageSize = PAGESIZE;
        UniGrid.Pager.ShowPageSize = false;
        UniGrid.FilterLimit = 1;
        UniGrid.OnExternalDataBound += UniGrid_OnExternalDataBound;
        UniGrid.OnAction += UniGrid_OnAction;
    }


    protected object UniGrid_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        switch (sourceName)
        {
            case "clickrate":
            case "uniqueclicks":
            case "totalclicks":
                {
                    DataRowView row = (DataRowView)parameter;
                    int value;
                    if (sourceName.EqualsCSafe("totalclicks", true))
                    {
                        // Get total clicks value
                        value = ValidationHelper.GetInteger(DataHelper.GetDataRowValue(row.Row, "TotalClicks"), 0);
                    }
                    else
                    {
                        // Get unique clicks value
                        value = ValidationHelper.GetInteger(DataHelper.GetDataRowValue(row.Row, "UniqueClicks"), 0);
                    }

                    // If A/B test links are shown...
                    if (isMainABTestIssue)
                    {
                        // Get selected issue (main or variant)
                        IssueInfo issue = IssueInfoProvider.GetIssueInfo(issueId);
                        if ((issue != null) && (!issue.IssueIsVariant))
                        {
                            // Get current link target and description
                            string linkTarget = ValidationHelper.GetString(DataHelper.GetDataRowValue(row.Row, "LinkTarget"), string.Empty);
                            string linkDescription = ValidationHelper.GetString(DataHelper.GetDataRowValue(row.Row, "LinkDescription"), string.Empty);

                            // If the issue is main issue add winner variant statistics
                            if (DataHelper.DataSourceIsEmpty(winnerData) && (UniGrid.InfoObject != null))
                            {
                                int winnerIssueId = 0;
                                ABTestInfo test = ABTestInfoProvider.GetABTestInfoForIssue(issue.IssueID);
                                if (test != null)
                                {
                                    // Get winner variant issue ID
                                    winnerIssueId = test.TestWinnerIssueID;
                                }

                                winnerData = UniGrid.InfoObject.GetData(null, "IssueID=" + winnerIssueId, null, -1, "LinkTarget,LinkDescription,UniqueClicks,TotalClicks", false);
                            }

                            if (!DataHelper.DataSourceIsEmpty(winnerData))
                            {
                                // Select data with matching link target and description
                                DataRow[] selectedRows = winnerData.Tables[0].Select(string.Format("LinkTarget='{0}' AND LinkDescription='{1}'", linkTarget, linkDescription));
                                if ((selectedRows.Length > 0))
                                {
                                    if (sourceName.EqualsCSafe("totalclicks", true))
                                    {
                                        // Get total clicks value
                                        value += ValidationHelper.GetInteger(DataHelper.GetDataRowValue(selectedRows[0], "TotalClicks"), 0);

                                        // Store if grid action should be visible (hide it if total clicks is lower than 0)
                                        showGridAction.Add(ValidationHelper.GetInteger(DataHelper.GetDataRowValue(row.Row, "LinkID"), 0), (value > 0));
                                    }
                                    else
                                    {
                                        // Get unique clicks value
                                        value += ValidationHelper.GetInteger(DataHelper.GetDataRowValue(selectedRows[0], "UniqueClicks"), 0);
                                    }
                                }
                            }
                        }
                    }

                    if (sourceName.EqualsCSafe("clickrate", true))
                    {
                        // Return formated click rate
                        return string.Format("{0:F2}", (ValidationHelper.GetDouble(value, 0) / sentEmails) * 100);
                    }
                    else
                    {
                        // Return unique or total clicks
                        return value;
                    }
                }

            case "linktarget":
                return string.Format(@"<a href=""#"" onclick=""OpenTarget('{0}')"">{1}</a>",
                                     parameter,
                                     HTMLHelper.HTMLEncode(TextHelper.LimitLength(parameter.ToString(), 50)));

            case "linktargettooltip":
                return HTMLHelper.HTMLEncode(parameter.ToString());

            case "linkdescription":
                return HTMLHelper.HTMLEncode(TextHelper.LimitLength(parameter.ToString(), 25));

            case "linkdescriptiontooltip":
                return HTMLHelper.HTMLEncode(parameter.ToString());

            case "view":
                if (sender is CMSGridActionButton)
                {
                    CMSGridActionButton imageButton = sender as CMSGridActionButton;
                    // Register for prerender event to hide actions for links without any clicks
                    imageButton.PreRender += ActionButton_PreRender;
                }
                return sender;

            case "deleteoutdated":
                if (sender is CMSGridActionButton)
                {
                    CMSGridActionButton imageButton = sender as CMSGridActionButton;
                    GridViewRow gvr = parameter as GridViewRow;
                    if (gvr != null)
                    {
                        DataRowView drv = gvr.DataItem as DataRowView;
                        if (drv != null)
                        {
                            bool isOutdated = ValidationHelper.GetBoolean(drv["LinkOutdated"], false);
                            if (!isOutdated)
                            {
                                // Hide delete button for links that are not outdated
                                imageButton.Visible = false;
                            }
                        }
                    }
                }
                return sender;

            default:
                return parameter;
        }
    }


    protected void ActionButton_PreRender(object sender, EventArgs e)
    {
        CMSGridActionButton action = sender as CMSGridActionButton;
        if (action != null)
        {
            int linkId = ValidationHelper.GetInteger(action.CommandArgument, 0);

            if (!ValidationHelper.GetBoolean(showGridAction[linkId], true))
            {
                // Hide view action button for links with no clicks
                action.Visible = false;
            }
        }
    }


    protected void UniGrid_OnAction(string actionName, object actionArgument)
    {
        switch (actionName)
        {
            case "deleteoutdated":
                // Delete link that is not used in current version of the issue
                int linkId = ValidationHelper.GetInteger(actionArgument, 0);
                LinkInfoProvider.DeleteLinkInfo(linkId);
                break;
        }
    }

    #endregion
}