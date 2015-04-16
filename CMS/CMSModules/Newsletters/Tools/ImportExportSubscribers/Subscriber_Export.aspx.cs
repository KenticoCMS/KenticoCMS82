using System;
using System.Collections.Generic;

using CMS.ExtendedControls;
using CMS.Helpers;
using CMS.Membership;
using CMS.Newsletters;
using CMS.SiteProvider;
using CMS.UIControls;

[Title("newsletters.exportsubscribers")]
[UIElement("CMS.Newsletter", "ExportSubscribers")]
public partial class CMSModules_Newsletters_Tools_ImportExportSubscribers_Subscriber_Export : CMSNewsletterPage
{
    /// <summary>
    /// Messages placeholder
    /// </summary>
    public override MessagesPlaceHolder MessagesPlaceHolder
    {
        get
        {
            return plcMessages;
        }
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        // Initialize newsletter selector
        usNewsletters.WhereCondition = "NewsletterSiteID = " + SiteContext.CurrentSiteID;

        // Initialize update panel progress
        pnlUpdate.ShowProgress = true;

        // Initialize radio button list items text
        rblExportList.Items[0].Text = GetString("newsletter.allsubscribers");
        rblExportList.Items[1].Text = GetString("general.approved");
        rblExportList.Items[2].Text = GetString("general.notapproved");

        if (!RequestHelper.IsPostBack())
        {
            rblExportList.SelectedIndex = 0;
        }
    }


    /// <summary>
    /// Handles export button click.
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="e">Event arguments</param>
    protected void btnExport_Click(object sender, EventArgs e)
    {
        // Check "manage subscribers" permission
        if (!MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("cms.newsletter", "managesubscribers"))
        {
            RedirectToAccessDenied("cms.newsletter", "managesubscribers");
        }

        // Get selected newsletters
        List<int> newsletterIds = new List<int>();
        string values = ValidationHelper.GetString(usNewsletters.Value, null);
        if (!String.IsNullOrEmpty(values))
        {
            string[] newItems = values.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            if (newItems != null)
            {
                foreach (string item in newItems)
                {
                    newsletterIds.Add(ValidationHelper.GetInteger(item, 0));
                }
            }
        }

        // Export subscribers
        string subscribers = null;
        if (SiteContext.CurrentSite != null)
        {
            subscribers = SubscriberInfoProvider.ExportSubscribersFromSite(newsletterIds, SiteContext.CurrentSiteID, true, ValidationHelper.GetInteger(rblExportList.SelectedValue, 0));
        }

        // No subscribers exported
        if (string.IsNullOrEmpty(subscribers))
        {
            ShowInformation(GetString("Subscriber_Export.noSubscribersExported"));
            txtExportSub.Enabled = false;
        }
        else
        {
            ShowInformation(GetString("Subscriber_Export.subscribersExported"));
            txtExportSub.Enabled = true;
        }

        txtExportSub.Text = subscribers;
    }
}