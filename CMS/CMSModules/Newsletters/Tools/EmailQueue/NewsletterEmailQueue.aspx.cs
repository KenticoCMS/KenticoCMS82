using System;
using System.Web.UI.WebControls;

using CMS.EmailEngine;
using CMS.Helpers;
using CMS.Membership;
using CMS.Newsletters;
using CMS.Base;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.ExtendedControls.ActionsConfig;
using CMS.ExtendedControls;
using CMS.DataEngine;

[Title("newsletters.emailqueue")]
[UIElement("CMS.Newsletter", "EmailQueue")]
public partial class CMSModules_Newsletters_Tools_EmailQueue_NewsletterEmailQueue : CMSNewsletterPage
{
    #region "Private variables"

    private int siteId;

    private bool emailsEnabled;

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        siteId = SiteContext.CurrentSiteID;

        emailsEnabled = EmailHelper.Settings.EmailsEnabled(SiteContext.CurrentSiteName);
        emailsEnabled |= SettingsKeyInfoProvider.GetBoolValue(SiteContext.CurrentSiteName + ".CMSGenerateNewsletters");

        // Display disabled information
        if (!emailsEnabled)
        {
            ShowWarning(GetString("NewsletterEmailQueue_List.EmailsDisabled"));
        }

        // Initialize unigrid
        gridElem.OnAction += gridElem_OnAction;
        gridElem.OnExternalDataBound += gridElem_OnExternalDataBound;
        gridElem.WhereCondition = "EmailSiteID = @SiteID";

        QueryDataParameters parameters = new QueryDataParameters();
        parameters.Add("@SiteID", siteId);

        gridElem.QueryParameters = parameters;

        InitializeActionMenu();
    }

    #endregion


    #region "Unigrid events"

    /// <summary>
    /// Handles Unigrid's OnExternalDataBound event.
    /// </summary>
    protected object gridElem_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        switch (sourceName.ToLowerCSafe())
        {
            case "subject":
                return TextHelper.LimitLength(HTMLHelper.HTMLEncode(parameter.ToString()), 50);

            case "result":
                return TextHelper.LimitLength(HTMLHelper.HTMLEncode(parameter.ToString()), 50);

            case "subjecttooltip":
            case "resulttooltip":
                return parameter.ToString().Replace("\r\n", "<br />").Replace("\n", "<br />");
        }

        return null;
    }


    /// <summary>
    /// Handles the UniGrid's OnAction event.
    /// </summary>
    /// <param name="actionName">Name of item (button) that throws event</param>
    /// <param name="actionArgument">ID (value of Primary key) of corresponding data row</param>
    protected void gridElem_OnAction(string actionName, object actionArgument)
    {
        if (!MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("cms.newsletter", "configure"))
        {
            RedirectToAccessDenied("cms.newsletter", "configure");
        }

        switch (actionName.ToLowerCSafe())
        {
            case "resend":
                // Resend an issue from the queue
                EmailQueueManager.ResendEmail(Convert.ToInt32(actionArgument));
                break;

            case "delete":
                // Delete EmailQueueItem object from database
                EmailQueueItemInfoProvider.DeleteEmailQueueItem(Convert.ToInt32(actionArgument));
                break;
        }
    }

    #endregion


    #region "Private methods"

    /// <summary>
    /// Initializes action menu.
    /// </summary>
    protected void InitializeActionMenu()
    {
        HeaderActions actions = CurrentMaster.HeaderActions;
        actions.ActionsList.Clear();

        // Resend all failed
        actions.ActionsList.Add(new HeaderAction
        {
            Text = GetString("NewsletterEmailQueue_List.ResendAllFailed"),
            OnClientClick = "if (!confirm(" + ScriptHelper.GetString(GetString("NewsletterEmailQueue_List.ResendAllFailedConfirmationMessage")) + ")) return false;",
            CommandName = "resendallfailed"
        });

        // Resend all
        actions.ActionsList.Add(new HeaderAction
        {
            Text = GetString("NewsletterEmailQueue_List.ResendAll"),
            OnClientClick = "if (!confirm(" + ScriptHelper.GetString(GetString("EmailQueue.ResendAllConfirmation")) + ")) return false;",
            CommandName = "resendall",
            ButtonStyle = ButtonStyle.Default,
        });

        // Delete all failed
        actions.ActionsList.Add(new HeaderAction
        {
            Text = GetString("NewsletterEmailQueue_List.DeleteAllFailed"),
            OnClientClick = "if (!confirm(" + ScriptHelper.GetString(GetString("NewsletterEmailQueue_List.DeleteAllFailedConfirmationMessage")) + ")) return false;",
            CommandName = "deleteallfailed",
            ButtonStyle = ButtonStyle.Default,
        });

        // Delete all
        actions.ActionsList.Add(new HeaderAction
        {
            Text = GetString("NewsletterEmailQueue_List.DeleteAll"),
            OnClientClick = "if (!confirm(" + ScriptHelper.GetString(GetString("NewsletterEmailQueue_List.DeleteAllConfirmationMessage")) + ")) return false;",
            CommandName = "deleteall",
            ButtonStyle = ButtonStyle.Default,
        });

        // Refresh
        actions.ActionsList.Add(new HeaderAction
        {
            Text = GetString("general.refresh"),
            CommandName = "refresh"
        });

        actions.ActionPerformed += HeaderActions_ActionPerformed;
        actions.PreRender += HeaderActions_PreRender;
    }

    #endregion


    #region "Header action events"

    protected void HeaderActions_PreRender(object sender, EventArgs e)
    {
        bool enabled = (gridElem.GridView.Rows.Count > 0);
        bool resending = enabled && (ThreadEmailSender.SendingThreads <= 0) && emailsEnabled;

        HeaderActions actions = CurrentMaster.HeaderActions;

        if (actions.ActionsList.Count > 4)
        {
            // Resend all failed
            actions.ActionsList[0].Enabled = resending;

            // Resend all
            actions.ActionsList[1].Enabled = resending;

            // Delete all failed
            actions.ActionsList[2].Enabled = enabled;

            // Delete all
            actions.ActionsList[3].Enabled = enabled;
        }
    }


    protected void HeaderActions_ActionPerformed(object sender, CommandEventArgs e)
    {
        // Check user permission (for complex operations only)
        if (e.CommandName != "refresh")
        {
            if (!MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("cms.newsletter", "configure"))
            {
                RedirectToAccessDenied("cms.newsletter", "configure");
            }
        }

        switch (e.CommandName.ToLowerCSafe())
        {
            case "resendall":
                {
                    EmailQueueManager.SendAllEmails(true, true, 0);
                    gridElem.ReloadData();
                    ShowInformation(GetString("EmailQueue.SendingEmails"));
                }
                break;

            case "resendallfailed":
                {
                    EmailQueueManager.SendAllEmails(true, false, 0);
                    gridElem.ReloadData();
                    ShowInformation(GetString("EmailQueue.SendingEmails"));
                }
                break;

            case "deleteall":
                {
                    EmailQueueItemInfoProvider.DeleteEmailQueueItem(siteId);
                    gridElem.ReloadData();
                }
                break;

            case "deleteallfailed":
                {
                    EmailQueueItemInfoProvider.DeleteFailedEmailQueueItem(siteId);
                    gridElem.ReloadData();
                }
                break;

            case "refresh":
                {
                    gridElem.ReloadData();
                }
                break;
        }
    }

    #endregion
}