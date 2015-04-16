using System;
using System.Data;
using System.Web.UI.WebControls;

using CMS.Helpers;
using CMS.Membership;
using CMS.Newsletters;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.ExtendedControls;

[UIElement("cms.newsletter", "SubscriberProperties.Users")]
public partial class CMSModules_Newsletters_Tools_Subscribers_Subscriber_Users : CMSNewsletterPage
{
    #region "Variables"

    private int mBounceLimit;


    private bool mBounceInfoAvailable;

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        string siteName = SiteContext.CurrentSiteName;
        // Get bounce limit
        mBounceLimit = NewsletterHelper.BouncedEmailsLimit(siteName);
        // Get info if bounced e-mail tracking is available
        mBounceInfoAvailable = NewsletterHelper.MonitorBouncedEmails(siteName);

        // Check if parent object exist
        SubscriberInfo sb = SubscriberInfoProvider.GetSubscriberInfo(QueryHelper.GetInteger("objectid", 0));
        EditedObject = sb;

        // Initialize unigrid
        UniGrid.OnAction += uniGrid_OnAction;
        UniGrid.OnExternalDataBound += uniGrid_OnExternalDataBound;
        UniGrid.WhereCondition = "((RoleID = " + QueryHelper.GetInteger("roleid", 0) + ") AND (SiteID = " + SiteContext.CurrentSiteID + "))";
    }


    protected void Page_PreRender(object sender, EventArgs e)
    {
        // Hide columns with bounced emails if bounce info is not available
        UniGrid.GridView.Columns[0].Visible =
            UniGrid.GridView.Columns[3].Visible =
            UniGrid.GridView.Columns[4].Visible = mBounceInfoAvailable;
    }


    /// <summary>
    /// Unigrid external databound event handler.
    /// </summary>
    protected object uniGrid_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        switch (sourceName)
        {
            case "block":
                var gridViewRow = parameter as GridViewRow;
                if (gridViewRow != null)
                {
                    return SetBlockAction(sender, (gridViewRow.DataItem) as DataRowView);
                }

                break;

            case "unblock":
                var viewRow = parameter as GridViewRow;
                if (viewRow != null)
                {
                    return SetUnblockAction(sender, (viewRow.DataItem) as DataRowView);
                }

                break;

            case "blocked":
                return GetBlocked(parameter);

            case "bounces":
                return GetBounces(parameter);

            default:
                return null;
        }

        return null;
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
            case "block":
                Block(actionArgument);
                break;

            case "unblock":
                Unblock(actionArgument);
                break;
        }
    }


    private object SetBlockAction(object sender, DataRowView rowView)
    {
        int bounces = GetBouncesFromRow(rowView);

        var imageButton = sender as CMSGridActionButton;
        if (imageButton != null)
        {
            imageButton.Visible = mBounceInfoAvailable && ((mBounceLimit > 0 && bounces < mBounceLimit) || (mBounceLimit == 0 && bounces < int.MaxValue));
        }

        return null;
    }


    private object SetUnblockAction(object sender, DataRowView rowView)
    {
        int bounces = GetBouncesFromRow(rowView);

        var imageButton = sender as CMSGridActionButton;
        if (imageButton != null)
        {
            imageButton.Visible = mBounceInfoAvailable && ((mBounceLimit > 0 && bounces >= mBounceLimit) || (mBounceLimit == 0 && bounces == int.MaxValue));
        }

        return null;
    }


    private string GetBlocked(object parameter)
    {
        // Do not handle if bounce email monitoring is not available
        if (!mBounceInfoAvailable)
        {
            return null;
        }

        // If bounce limit is not a natural number, then the feature is considered disabled
        if (mBounceLimit < 0)
        {
            return UniGridFunctions.ColoredSpanYesNoReversed(false);
        }

        int bounces = ValidationHelper.GetInteger(parameter, 0);

        return UniGridFunctions.ColoredSpanYesNoReversed((mBounceLimit > 0 && bounces >= mBounceLimit) || (mBounceLimit == 0 && bounces == int.MaxValue));
    }


    private string GetBounces(object parameter)
    {
        // Do not handle if bounce email monitoring is not available
        if (!mBounceInfoAvailable)
        {
            return null;
        }

        int bounces = ValidationHelper.GetInteger(parameter, 0);

        if (bounces == 0 || bounces == int.MaxValue)
        {
            return string.Empty;
        }

        return bounces.ToString();
    }


    private void Block(object actionArgument)
    {
        CheckAuthorization();
        SubscriberInfoProvider.BlockUser(ValidationHelper.GetInteger(actionArgument, 0));
    }


    private void Unblock(object actionArgument)
    {
        CheckAuthorization();
        SubscriberInfoProvider.UnblockUser(ValidationHelper.GetInteger(actionArgument, 0));
    }


    private static void CheckAuthorization()
    {
        if (!MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("cms.newsletter", "managesubscribers"))
        {
            RedirectToAccessDenied("cms.newsletter", "managesubscribers");
        }
    }


    private static int GetBouncesFromRow(DataRowView rowView)
    {
        return ValidationHelper.GetInteger(DataHelper.GetDataRowValue(rowView.Row, "UserBounces"), 0);
    }

    #endregion
}