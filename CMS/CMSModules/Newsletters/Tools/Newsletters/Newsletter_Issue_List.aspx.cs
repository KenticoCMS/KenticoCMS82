using System;
using System.Linq;
using System.Data;
using System.Web.UI.WebControls;

using CMS.ExtendedControls.ActionsConfig;
using CMS.Helpers;
using CMS.Membership;
using CMS.Newsletters;
using CMS.Base;
using CMS.PortalEngine;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.MacroEngine;
using CMS.DataEngine;
using CMS.ExtendedControls;

/// <summary>
/// Displays a list of issues for a specified newsletter.
/// </summary>
[EditedObject(NewsletterInfo.OBJECT_TYPE, "parentobjectid")]
[UIElement("CMS.Newsletter", "Newsletter.Issues")]
public partial class CMSModules_Newsletters_Tools_Newsletters_Newsletter_Issue_List : CMSNewsletterPage
{
    #region "Variables"

    private bool mBounceMonitoringEnabled;
    private bool mOnlineMarketingEnabled;
    private bool mTrackingEnabled;
    private bool mABTestEnabled;
    private NewsletterInfo mNewsletter;

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        mNewsletter = EditedObject as NewsletterInfo;
     
        if (mNewsletter == null)
        {
            RedirectToAccessDenied(GetString("general.invalidparameters"));
        }

        if (!mNewsletter.CheckPermissions(PermissionsEnum.Read, CurrentSiteName, CurrentUser))
        {
            RedirectToAccessDenied(mNewsletter.TypeInfo.ModuleName, "Read");
        }
        
        mBounceMonitoringEnabled = NewsletterHelper.MonitorBouncedEmails(CurrentSiteName);
        mOnlineMarketingEnabled = NewsletterHelper.OnlineMarketingAvailable(CurrentSiteName);
        mTrackingEnabled = NewsletterHelper.IsTrackingAvailable();
        mABTestEnabled = NewsletterHelper.IsABTestingAvailable();

        ScriptHelper.RegisterDialogScript(this);

        string scriptBlock = string.Format(@"
            function RefreshPage() {{ document.location.replace(document.location); }}
            function ShowOpenedBy(id) {{ modalDialog('{0}?objectid=' + id, 'NewsletterIssueOpenedBy', '900px', '700px');  return false; }}
            function ViewClickedLinks(id) {{ modalDialog('{1}?objectid=' + id, 'NewsletterTrackedLinks', '900px', '700px'); return false; }}",
                                           ResolveUrl(@"~\CMSModules\Newsletters\Tools\Newsletters\Newsletter_Issue_OpenedBy.aspx"),
                                           ResolveUrl(@"~\CMSModules\Newsletters\Tools\Newsletters\Newsletter_Issue_TrackedLinks.aspx"));
        ScriptHelper.RegisterClientScriptBlock(this, GetType(), "Actions", scriptBlock, true);

        // Initialize unigrid
        UniGrid.WhereCondition = String.Format("IssueNewsletterID={0} AND IssueVariantOfIssueID IS NULL", mNewsletter.NewsletterID);
        UniGrid.ZeroRowsText = GetString("Newsletter_Issue_List.NoIssuesFound");
        UniGrid.OnAction += uniGrid_OnAction;
        UniGrid.OnBeforeDataReload += UniGrid_OnBeforeDataReload;
        UniGrid.OnExternalDataBound += UniGrid_OnExternalDataBound;

        // Initialize header actions
        InitHeaderActions();
    }


    protected void InitHeaderActions()
    {
        if (!mNewsletter.NewsletterType.EqualsCSafe(NewsletterType.Dynamic))
        {
            CurrentMaster.HeaderActions.AddAction(new HeaderAction
                {
                    RedirectUrl = "Newsletter_Issue_New.aspx?parentobjectid=" + mNewsletter.NewsletterID,
                    Text = GetString("Newsletter_Issue_List.NewItemCaption"),
                    Tooltip = GetString("Newsletter_Issue_List.NewItemCaption"),
                    Permission = "AuthorIssues",
                    ResourceName = mNewsletter.TypeInfo.ModuleName,
                });
        }
    }


    protected void UniGrid_OnBeforeDataReload()
    {
        // Hide opened emails if tracking is not available
        UniGrid.NamedColumns["openedemails"].Visible = mTrackingEnabled;

        // Hide bounced emails info if monitoring disabled or tracking is not available
        UniGrid.NamedColumns["bounces"].Visible = mBounceMonitoringEnabled;

        // Hide A/B test column for dynamic newsletters or if A/B testing is not available
        UniGrid.NamedColumns["isabtest"].Visible = mABTestEnabled && mNewsletter.NewsletterType.EqualsCSafe(NewsletterType.TemplateBased);
    }


    /// <summary>
    /// Handles the UniGrid's OnExternalDataBound event.
    /// </summary>
    /// <param name="sender">The sender</param>
    /// <param name="sourceName">Name of the source</param>
    /// <param name="parameter">The data row</param>
    /// <returns>Formatted value to be used in the UniGrid</returns>
    protected object UniGrid_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        switch (sourceName.ToLowerCSafe())
        {
            case "issuesubject":
                return HTMLHelper.HTMLEncode(MacroSecurityProcessor.RemoveSecurityParameters(parameter.ToString(), true, null));

            case "issueopenedemails":
                return GetOpenedEmails(parameter as DataRowView);

            case "issuestatus":
                IssueStatusEnum status = IssueStatusEnum.Idle;
                if ((parameter != DBNull.Value) && (parameter != null))
                {
                    status = (IssueStatusEnum)parameter;
                }
                return GetString(String.Format("newsletterissuestatus." + Convert.ToString(status)));

            case "viewclickedlinks":
                if (sender is CMSGridActionButton)
                {
                    // Hide 'view clicked links' action if tracking is not available or if the issue has no tracked links
                    CMSGridActionButton imageButton = sender as CMSGridActionButton;
                    if (!mTrackingEnabled)
                    {
                        imageButton.Visible = false;
                    }
                    else
                    {
                        GridViewRow gvr = parameter as GridViewRow;
                        if (gvr != null)
                        {
                            DataRowView drv = gvr.DataItem as DataRowView;
                            if (drv != null)
                            {
                                int issueId = ValidationHelper.GetInteger(drv["IssueID"], 0);
                                // Try to get one tracked link (only ID) of the issue
                                var links = LinkInfoProvider.GetLinks().WhereEquals("LinkIssueID", issueId).TopN(1).Column("LinkID");
                                if (!links.Any())
                                {
                                    imageButton.Visible = false;
                                }
                            }
                        }
                    }
                }
                return sender;

            default:
                return parameter;
        }
    }


    /// <summary>
    /// Handles the UniGrid's OnAction event.
    /// </summary>
    /// <param name="actionName">Name of item (button) that throws event</param>
    /// <param name="actionArgument">ID (value of Primary key) of corresponding data row</param>
    protected void uniGrid_OnAction(string actionName, object actionArgument)
    {
        switch (actionName)
        {
            case "edit":
                string url = UIContextHelper.GetElementUrl("cms.newsletter", "EditIssueProperties", false, actionArgument.ToInteger(0));
                url = URLHelper.AddParameterToUrl(url, "parentobjectid", Convert.ToString(mNewsletter.NewsletterID));
                URLHelper.Redirect(url);
                break;

            case "delete":
                DeleteIssue(ValidationHelper.GetInteger(actionArgument, 0));
                break;
        }
    }


    /// <summary>
    /// Gets a clickable opened emails counter based on the values from datasource.
    /// </summary>
    /// <param name="rowView">A <see cref="DataRowView" /> that represents one row from UniGrid's source</param>
    /// <returns>A link with detailed statistics about opened emails</returns>
    private string GetOpenedEmails(DataRowView rowView)
    {
        // Get issue ID
        int issueId = ValidationHelper.GetInteger(DataHelper.GetDataRowViewValue(rowView, "IssueID"), 0);

        // Get opened emails count from issue record
        int openedEmails = ValidationHelper.GetInteger(DataHelper.GetDataRowViewValue(rowView, "IssueOpenedEmails"), 0);
        if (mOnlineMarketingEnabled)
        {
            // Get number of emails opened by contact group members
            openedEmails += OpenedEmailInfoProvider.GetMultiSubscriberOpenedIssueActivityCount(issueId);
        }

        // Add winner variant data if it is an A/B test and a winner has been selected
        if (ValidationHelper.GetBoolean(DataHelper.GetDataRowViewValue(rowView, "IssueIsABTest"), false))
        {
            openedEmails += GetWinnerVariantOpenes(issueId);
        }

        if (openedEmails > 0)
        {
            return string.Format(@"<a href=""#"" onclick=""ShowOpenedBy({0})"">{1}</a>", issueId, openedEmails);
        }

        return "0";
    }


    /// <summary>
    /// Gets number of opened e-mails of winner variant issue.
    /// </summary>
    /// <param name="issueId">ID of parent issue</param>
    private int GetWinnerVariantOpenes(int issueId)
    {
        int openedEmails = 0;

        ABTestInfo test = ABTestInfoProvider.GetABTestInfoForIssue(issueId);
        if ((test != null) && (test.TestWinnerIssueID > 0))
        {
            IssueInfo winner = IssueInfoProvider.GetIssueInfo(test.TestWinnerIssueID);
            if (winner != null)
            {
                // Get opened emails count from winner issue
                openedEmails += winner.IssueOpenedEmails;

                if (mOnlineMarketingEnabled)
                {
                    // Get number of emails opened by contact group and persona members
                    openedEmails += OpenedEmailInfoProvider.GetMultiSubscriberOpenedIssueActivityCount(winner.IssueID);
                }
            }
        }

        return openedEmails;
    }


    /// <summary>
    /// Deletes an issue specified by its ID (if authorized).
    /// </summary>
    /// <param name="issueId">Issue's ID</param>
    private static void DeleteIssue(int issueId)
    {
        var issue = IssueInfoProvider.GetIssueInfo(issueId);

        if (issue == null)
        {
            RedirectToAccessDenied(GetString("general.invalidparameters"));
        }
        
        // User has to have both destroy and issue privileges to be able to delete the issue.
        if (!issue.CheckPermissions(PermissionsEnum.Delete, SiteContext.CurrentSiteName, MembershipContext.AuthenticatedUser))
        {
            RedirectToAccessDenied(issue.TypeInfo.ModuleName, "AuthorIssues");
        }

        IssueInfoProvider.DeleteIssueInfo(issue);
    }

    #endregion
}