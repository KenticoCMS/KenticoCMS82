using System;
using System.Data;
using System.Web.UI.WebControls;

using CMS.Core;
using CMS.Helpers;
using CMS.Newsletters;
using CMS.Base;
using CMS.PortalEngine;
using CMS.SiteProvider;
using CMS.Membership;
using CMS.UIControls;
using CMS.ExtendedControls;
using CMS.DataEngine;

[Title("newsletters.subscribers")]
[Action(0, "Subscriber_List.NewItemCaption", "Subscriber_New.aspx")]
[UIElement(ModuleName.NEWSLETTER, "Subscribers")]
public partial class CMSModules_Newsletters_Tools_Subscribers_Subscriber_List : CMSNewsletterPage
{
    #region "Variables"

    private int mBounceLimit;


    private bool mBounceInfoAvailable;

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        SiteInfo currentSite = SiteContext.CurrentSite;
        mBounceLimit = NewsletterHelper.BouncedEmailsLimit(currentSite.SiteName);
        mBounceInfoAvailable = NewsletterHelper.MonitorBouncedEmails(currentSite.SiteName);

        // Initialize unigrid
        UniGrid.OnAction += uniGrid_OnAction;
        UniGrid.OnExternalDataBound += uniGrid_OnExternalDataBound;
        UniGrid.WhereCondition = "SubscriberSiteID = " + currentSite.SiteID;
    }


    protected void Page_PreRender(object sender, EventArgs e)
    {
        // Hide columns with bounced emails if bounce info is not available
        UniGrid.NamedColumns["blocked"].Visible =
            UniGrid.NamedColumns["bounces"].Visible = mBounceInfoAvailable;

        if (UniGrid.RowsCount > 0)
        {
            int i = 0;
            DataView view = (DataView)UniGrid.GridView.DataSource;
            foreach (DataRow row in view.Table.Rows)
            {
                // Hide object menu for other than normal subscribers
                if (ValidationHelper.GetString(DataHelper.GetDataRowValue(row, "SubscriberType"), "") != "")
                {
                    if ((UniGrid.GridView.Rows[i].Cells.Count > 0) && (UniGrid.GridView.Rows[i].Cells[0].Controls.Count > 4)
                        && (UniGrid.GridView.Rows[i].Cells[0].Controls[4] is ContextMenuContainer))
                    {
                        UniGrid.GridView.Rows[i].Cells[0].Controls[4].Visible = false;
                    }
                }

                i++;
            }
        }
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

            case "email":
                return GetEmail(parameter as DataRowView);

            case "blocked":
                return GetBlocked(parameter as DataRowView);

            case "bounces":
                return GetBounces(parameter as DataRowView);

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
                // Edit subscriber
            case "edit":
                Edit(actionArgument);
                break;

                // Delete subscriber
            case "delete":
                Delete(actionArgument);
                break;

                // Block subscriber
            case "block":
                Block(actionArgument);
                break;

                // Un-block subscriber
            case "unblock":
                Unblock(actionArgument);
                break;
        }
    }


    /// <summary>
    /// Displays/hides block action button in unigrid.
    /// </summary>
    private object SetBlockAction(object sender, DataRowView rowView)
    {
        int bounces = GetBouncesFromRow(rowView);

        var imageButton = sender as CMSGridActionButton;
        if (imageButton != null)
        {
            imageButton.Visible = mBounceInfoAvailable && !IsMultiSubscriber(rowView)
                                              && ((mBounceLimit > 0 && bounces < mBounceLimit) || (mBounceLimit == 0 && bounces < int.MaxValue));
        }

        return null;
    }


    /// <summary>
    /// Displays/hides un-block action button in unigrid.
    /// </summary>
    private object SetUnblockAction(object sender, DataRowView rowView)
    {
        int bounces = GetBouncesFromRow(rowView);

        var imageButton = sender as CMSGridActionButton;
        if (imageButton != null)
        {
            imageButton.Visible = mBounceInfoAvailable && !IsMultiSubscriber(rowView)
                                              && ((mBounceLimit > 0 && bounces >= mBounceLimit) || (mBounceLimit == 0 && bounces == int.MaxValue));
        }

        return null;
    }


    /// <summary>
    /// Returns subscriber's e-mail address.
    /// </summary>
    private object GetEmail(DataRowView rowView)
    {
        // Try to get subscriber email
        string email = ValidationHelper.GetString(rowView.Row["SubscriberEmail"], string.Empty);
        if (string.IsNullOrEmpty(email))
        {
            // Try to get user email
            email = ValidationHelper.GetString(rowView.Row["Email"], string.Empty);
        }

        if (string.IsNullOrEmpty(email) && ValidationHelper.GetString(rowView.Row["SubscriberType"], string.Empty).EqualsCSafe(PredefinedObjectType.CONTACT))
        {
            // Add the field transformation control that handles the translation
            var tr = new ObjectTransformation("om.contact", ValidationHelper.GetInteger(rowView.Row["SubscriberRelatedID"], 0));
            tr.Transformation = "ContactEmail";

            return tr;
        }

        return email;
    }


    /// <summary>
    /// Returns colored yes/no or nothing according to subscriber's blocked info.
    /// </summary>
    private string GetBlocked(DataRowView rowView)
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

        if (IsMultiSubscriber(rowView))
        {
            return null;
        }

        int bounces = GetBouncesFromRow(rowView);

        return UniGridFunctions.ColoredSpanYesNoReversed((mBounceLimit > 0 && bounces >= mBounceLimit) || (mBounceLimit == 0 && bounces == int.MaxValue));
    }


    /// <summary>
    /// Returns number of bounces or nothing according to subscriber's bounce info.
    /// </summary>
    private string GetBounces(DataRowView rowView)
    {
        // Do not handle if bounce email monitoring is not available
        if (!mBounceInfoAvailable)
        {
            return null;
        }

        int bounces = GetBouncesFromRow(rowView);

        if (bounces == 0 || bounces == int.MaxValue || IsMultiSubscriber(rowView))
        {
            return null;
        }

        return bounces.ToString();
    }


    /// <summary>
    /// Edit selected subscriber.
    /// </summary>
    private void Edit(object actionArgument)
    {
        var url = UIContextHelper.GetElementUrl("cms.newsletter", "Newsletters.SubscriberProperties", false, ValidationHelper.GetInteger(actionArgument, 0));
        URLHelper.Redirect(url);
    }


    /// <summary>
    /// Delete selected subscriber.
    /// </summary>
    private void Delete(object actionArgument)
    {
        CheckAuthorization();
        SubscriberInfoProvider.DeleteSubscriberInfo(ValidationHelper.GetInteger(actionArgument, 0));
    }


    /// <summary>
    /// Block selected subscriber.
    /// </summary>
    private void Block(object actionArgument)
    {
        CheckAuthorization();
        SubscriberInfoProvider.BlockSubscriber(ValidationHelper.GetInteger(actionArgument, 0));
    }


    /// <summary>
    /// Un-block selected subscriber.
    /// </summary>
    private void Unblock(object actionArgument)
    {
        CheckAuthorization();
        SubscriberInfoProvider.UnblockSubscriber(ValidationHelper.GetInteger(actionArgument, 0));
    }


    /// <summary>
    /// Checks if the user has permission to manage subscribers.
    /// </summary>
    private static void CheckAuthorization()
    {
        if (!MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("cms.newsletter", "managesubscribers"))
        {
            RedirectToAccessDenied("cms.newsletter", "managesubscribers");
        }
    }


    /// <summary>
    /// Returns if type of the subscriber is "cms.role", "om.contactgroup" or "personas.persona".
    /// </summary>
    private static bool IsMultiSubscriber(DataRowView rowView)
    {
        string type = ValidationHelper.GetString(DataHelper.GetDataRowValue(rowView.Row, "SubscriberType"), string.Empty);
        return (type.EqualsCSafe(RoleInfo.OBJECT_TYPE, true) || type.EqualsCSafe(PredefinedObjectType.CONTACTGROUP, true) || type.EqualsCSafe(PredefinedObjectType.PERSONA, true));
    }


    /// <summary>
    /// Returns number of bounces of the subscriber.
    /// </summary>
    private static int GetBouncesFromRow(DataRowView rowView)
    {
        return ValidationHelper.GetInteger(DataHelper.GetDataRowValue(rowView.Row, "SubscriberBounces"), 0);
    }

    #endregion
}