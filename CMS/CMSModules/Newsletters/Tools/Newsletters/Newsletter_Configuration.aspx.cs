using System;
using System.Web.UI.WebControls;

using CMS.Core;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.Membership;
using CMS.Newsletters;
using CMS.Scheduler;
using CMS.Base;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.ExtendedControls.ActionsConfig;

[UIElement(ModuleName.NEWSLETTER, "Newsletter.Configuration")]
[EditedObject(NewsletterInfo.OBJECT_TYPE, "objectid")]
public partial class CMSModules_Newsletters_Tools_Newsletters_Newsletter_Configuration : CMSNewsletterPage
{
    #region "Variables"

    /// <summary>
    /// It is true if edited newsletter is dynamic.
    /// </summary>
    protected bool isDynamic = false;


    /// <summary>
    /// Determines if newsletter tracking is enabled.
    /// </summary>
    private bool? mTrackingEnabled;

    private bool TrackingEnabled
    {
        get
        {
            if (mTrackingEnabled == null)
            {
                mTrackingEnabled = NewsletterHelper.IsTrackingAvailable();
            }

            return (bool)mTrackingEnabled;
        }
    }


    /// <summary>
    /// Determines if Online Marketing is enabled.
    /// </summary>
    private bool? mOnlineMarketingEnabled;

    private bool OnlineMarketingEnabled
    {
        get
        {
            if (mOnlineMarketingEnabled == null)
            {
                mOnlineMarketingEnabled = NewsletterHelper.OnlineMarketingAvailable(SiteContext.CurrentSiteName);
            }

            return (bool)mOnlineMarketingEnabled;
        }
    }


    private NewsletterInfo EditedNewsletter
    {
        get
        {
            return (NewsletterInfo)EditedObject;
        }
    }

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        if (EditedNewsletter == null)
        {
            RedirectToAccessDenied(GetString("general.invalidparameters"));
        }

        // Set validation messages
        rfvNewsletterDisplayName.ErrorMessage = GetString("general.requiresdisplayname");
        rfvNewsletterName.ErrorMessage = GetString("Newsletter_Edit.ErrorEmptyName");
        rfvNewsletterSenderName.ErrorMessage = GetString("Newsletter_Edit.ErrorEmptySenderName");
        rfvNewsletterSenderEmail.ErrorMessage = GetString("Newsletter_Edit.ErrorEmptySenderEmail");
        rfvNewsletterDynamicURL.ErrorMessage = GetString("newsletter_edit.sourcepageurlempty");

        // Hide detailed report as it could confuse non-technical users
        txtNewsletterDynamicURL.ShowDetailedError = false;
        txtNewsletterDynamicURL.StatusErrorMessage = GetString("general.pagenotfound");
        
        // Register save button
        CurrentMaster.HeaderActions.AddAction(new SaveAction(this));
        CurrentMaster.HeaderActions.ActionPerformed += HeaderActions_ActionPerformed;

        // Load newsletter configuration
        LoadData();
    }


    protected void LoadData()
    {
        if (!EditedNewsletter.CheckPermissions(PermissionsEnum.Modify, CurrentSiteName, CurrentUser))
        {
            RedirectToAccessDenied(EditedNewsletter.TypeInfo.ModuleName, "Configure");
        }

        int siteId = EditedNewsletter.NewsletterSiteID;

        // Initialize template selectors
        string whereTemplate = "TemplateType='{0}' AND TemplateSiteID=" + siteId;

        subscriptionTemplate.WhereCondition = String.Format(whereTemplate, EmailTemplateType.Subscription);
        unsubscriptionTemplate.WhereCondition = String.Format(whereTemplate, EmailTemplateType.Unsubscription);
        optInSelector.WhereCondition = String.Format(whereTemplate, EmailTemplateType.DoubleOptIn);

        issueTemplate.WhereCondition = String.Format(whereTemplate, EmailTemplateType.Issue);

        issueTemplate.WhereCondition = SqlHelper.AddWhereCondition(issueTemplate.WhereCondition, String.Format("TemplateID={0}", EditedNewsletter.NewsletterTemplateID), "OR");
        
        // Check if the newsletter is dynamic and adjust config dialog
        isDynamic = EditedNewsletter.NewsletterType.EqualsCSafe(NewsletterType.Dynamic, StringComparison.InvariantCultureIgnoreCase);

        // Display template/dynamic based newsletter config and online marketing config
        plcDynamic.Visible = isDynamic;
        plcTemplate.Visible = !isDynamic;
        plcTracking.Visible = TrackingEnabled;
        plcOM.Visible = OnlineMarketingEnabled;

        if (!RequestHelper.IsPostBack())
        {
            if (QueryHelper.GetBoolean("saved", false))
            {
                // If user was redirected from newsletter_new.aspx, display the 'Changes were saved' message
                ShowChangesSaved();
            }

            // Fill config dialog with newsletter data
            GetNewsletterValues(EditedNewsletter);

            if (!isDynamic)
            {
                // Initialize issue template selector
                issueTemplate.Value = EditedNewsletter.NewsletterTemplateID.ToString();
            }
            else
            {
                // Check if dynamic newsletter subject is empty
                bool subjectEmpty = string.IsNullOrEmpty(EditedNewsletter.NewsletterDynamicSubject);
                radPageTitle.Checked = subjectEmpty;
                radFollowing.Checked = !subjectEmpty;
                txtSubject.Enabled = radFollowing.Checked;

                if (!subjectEmpty)
                {
                    txtSubject.Text = EditedNewsletter.NewsletterDynamicSubject;
                }

                txtNewsletterDynamicURL.Value = EditedNewsletter.NewsletterDynamicURL;

                TaskInfo task = TaskInfoProvider.GetTaskInfo(EditedNewsletter.NewsletterDynamicScheduledTaskID);
                if (task != null)
                {
                    chkSchedule.Checked = true;
                    schedulerInterval.Visible = true;
                    schedulerInterval.ScheduleInterval = task.TaskInterval;
                }
                else
                {
                    chkSchedule.Checked = false;
                    schedulerInterval.Visible = false;
                }
            }
        }
    }


    /// <summary>
    /// Save button action.
    /// </summary>
    protected void HeaderActions_ActionPerformed(object sender, CommandEventArgs e)
    {
        switch (e.CommandName.ToLowerCSafe())
        {
            case "save":
                SaveData();

                break;
        }
    }


    /// <summary>
    /// Saves configuration changes.
    /// </summary>
    protected void SaveData()
    {
        // Check "configure" permission
        if (!MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("cms.newsletter", "configure"))
        {
            RedirectToAccessDenied("cms.newsletter", "configure");
        }

        string scheduledInterval = null;
        if (isDynamic && chkSchedule.Checked)
        {
            // Get scheduled interval for dynamic newsletter
            scheduledInterval = schedulerInterval.ScheduleInterval;
        }

        string errorMessage = ValidateNewsletterValues();
        if (!string.IsNullOrEmpty(errorMessage))
        {
            ShowError(errorMessage);
            return;
        }

        NewsletterInfo newsletterObj = NewsletterInfoProvider.GetNewsletterInfo(txtNewsletterName.Text.Trim(), SiteContext.CurrentSiteID);

        // Newsletter's code name must be unique
        if (newsletterObj != null && newsletterObj.NewsletterID != EditedNewsletter.NewsletterID)
        {
            ShowError(GetString("Newsletter_Edit.NewsletterNameExists"));
            return;
        }

        if (newsletterObj == null)
        {
            newsletterObj = NewsletterInfoProvider.GetNewsletterInfo(EditedNewsletter.NewsletterID);
        }

        SetNewsletterValues(newsletterObj);

        // Check if subscription template was selected
        int subscriptionTemplateValue = ValidationHelper.GetInteger(subscriptionTemplate.Value, 0);
        if (subscriptionTemplateValue == 0)
        {
            ShowError(GetString("Newsletter_Edit.NoSubscriptionTemplateSelected"));
            return;
        }
        newsletterObj.NewsletterSubscriptionTemplateID = subscriptionTemplateValue;

        // Check if double opt-in template was selected
        if (chkEnableOptIn.Checked)
        {
            int optInTemplateValue = ValidationHelper.GetInteger(optInSelector.Value, 0);
            if (optInTemplateValue == 0)
            {
                ShowError(GetString("Newsletter_Edit.NoOptInTemplateSelected"));
                return;
            }
            newsletterObj.NewsletterOptInTemplateID = optInTemplateValue;
        }
        else
        {
            newsletterObj.NewsletterOptInTemplateID = 0;
        }

        // Check if unsubscription template was selected
        int unsubscriptionTemplateValue = ValidationHelper.GetInteger(unsubscriptionTemplate.Value, 0);
        if (unsubscriptionTemplateValue == 0)
        {
            ShowError(GetString("Newsletter_Edit.NoUnsubscriptionTemplateSelected"));
            return;
        }
        newsletterObj.NewsletterUnsubscriptionTemplateID = unsubscriptionTemplateValue;

        // ID of scheduled task which should be deleted
        int deleteScheduledTaskId = 0;

        if (isDynamic)
        {
            newsletterObj.NewsletterType = NewsletterType.Dynamic;
            newsletterObj.NewsletterDynamicURL = txtNewsletterDynamicURL.Value.ToString();
            newsletterObj.NewsletterDynamicSubject = radFollowing.Checked ? txtSubject.Text : string.Empty;

            if ((String.IsNullOrEmpty(txtNewsletterDynamicURL.Value.ToString())))
            {
                // Dynamic URL cannot be empty
                ShowError(GetString("newsletter_edit.sourcepageurlempty"));
                return;
            }

            if (chkSchedule.Checked)
            {
                // Set info for scheduled task
                TaskInfo task = GetDynamicNewsletterTask(newsletterObj);

                if (!schedulerInterval.CheckOneDayMinimum())
                {
                    // If problem occurred while setting schedule interval
                    ShowError(GetString("Newsletter_Edit.NoDaySelected"));
                    return;
                }

                if (!DataTypeManager.IsValidDate(SchedulingHelper.DecodeInterval(scheduledInterval).StartTime))
                {
                    ShowError(GetString("Newsletter.IncorrectDate"));
                    return;
                }

                if ((newsletterObj.NewsletterDynamicScheduledTaskID == task.TaskID) && (task.TaskInterval == scheduledInterval))
                {
                    // No need to update anything, nothing has changed
                }
                else
                {
                    // Scheduled task either doesn't exist or was updated to new interval
                    task.TaskInterval = scheduledInterval;
                    task.TaskNextRunTime = SchedulingHelper.GetNextTime(task.TaskInterval, new DateTime(), new DateTime());
                    task.TaskDisplayName = GetString("DynamicNewsletter.TaskName") + newsletterObj.NewsletterDisplayName;
                    task.TaskName = "DynamicNewsletter_" + newsletterObj.NewsletterName;
                    // Set task for processing in external service
                    task.TaskAllowExternalService = true;
                    task.TaskUseExternalService = (SchedulingHelper.UseExternalService && NewsletterHelper.UseExternalServiceForDynamicNewsletters(SiteContext.CurrentSiteName));
                    TaskInfoProvider.SetTaskInfo(task);
                    newsletterObj.NewsletterDynamicScheduledTaskID = task.TaskID;
                }
            }
            else
            {
                if (newsletterObj.NewsletterDynamicScheduledTaskID > 0)
                {
                    // Store task ID for deletion
                    deleteScheduledTaskId = newsletterObj.NewsletterDynamicScheduledTaskID;
                }
                newsletterObj.NewsletterDynamicScheduledTaskID = 0;
                schedulerInterval.Visible = false;
            }
        }
        else
        {
            newsletterObj.NewsletterType = NewsletterType.TemplateBased;

            // Check if issue template was selected
            int issueTemplateValue = ValidationHelper.GetInteger(issueTemplate.Value, 0);
            if (issueTemplateValue == 0)
            {
                ShowError(GetString("Newsletter_Edit.NoEmailTemplateSelected"));
                return;
            }
            newsletterObj.NewsletterTemplateID = issueTemplateValue;
        }

        // Save changes to DB
        NewsletterInfoProvider.SetNewsletterInfo(newsletterObj);
        if (deleteScheduledTaskId > 0)
        {
            // Delete scheduled task if schedule mail-outs were unchecked
            TaskInfoProvider.DeleteTaskInfo(deleteScheduledTaskId);
        }

        ShowChangesSaved();

        // Update breadcrumbs
        ScriptHelper.RefreshTabHeader(Page, newsletterObj.NewsletterDisplayName);
    }


    protected void radSubject_CheckedChanged(object sender, EventArgs e)
    {
        txtSubject.Enabled = radFollowing.Checked;
    }


    protected void chkSchedule_CheckedChanged(object sender, EventArgs e)
    {
        // Show/hide scheduler
        schedulerInterval.Visible = chkSchedule.Checked;
    }


    protected void chkEnableOptIn_CheckedChanged(object sender, EventArgs e)
    {
        // Show/hide double opt-in options
        plcOptIn.Visible = chkEnableOptIn.Checked;
    }


    /// <summary>
    /// Initializes config form.
    /// </summary>
    /// <param name="newsletter">Newsletter object</param>
    private void GetNewsletterValues(NewsletterInfo newsletter)
    {
        txtNewsletterDisplayName.Text = newsletter.NewsletterDisplayName;
        txtNewsletterName.Text = newsletter.NewsletterName;
        txtNewsletterSenderName.Text = newsletter.NewsletterSenderName;
        txtNewsletterSenderEmail.Text = newsletter.NewsletterSenderEmail;
        txtNewsletterBaseUrl.Text = newsletter.NewsletterBaseUrl;
        txtNewsletterUnsubscribeUrl.Text = newsletter.NewsletterUnsubscribeUrl;
        txtDraftEmails.Text = newsletter.NewsletterDraftEmails;
        chkUseEmailQueue.Checked = newsletter.NewsletterUseEmailQueue;
        chkEnableResending.Checked = newsletter.NewsletterEnableResending;

        subscriptionTemplate.Value = newsletter.NewsletterSubscriptionTemplateID.ToString();
        unsubscriptionTemplate.Value = newsletter.NewsletterUnsubscriptionTemplateID.ToString();

        if (TrackingEnabled)
        {
            chkTrackOpenedEmails.Checked = newsletter.NewsletterTrackOpenEmails;
            chkTrackClickedLinks.Checked = newsletter.NewsletterTrackClickedLinks;
        }

        if (OnlineMarketingEnabled)
        {
            chkLogActivity.Checked = newsletter.NewsletterLogActivity;
        }

        chkEnableOptIn.Checked = plcOptIn.Visible = newsletter.NewsletterEnableOptIn;
        optInSelector.Value = newsletter.NewsletterOptInTemplateID;
        txtOptInURL.Text = newsletter.NewsletterOptInApprovalURL;
        chkSendOptInConfirmation.Checked = newsletter.NewsletterSendOptInConfirmation;
    }


    /// <summary>
    /// Validates newsletter config form.
    /// </summary>
    /// <returns>Returns error message in case of an error</returns>
    private string ValidateNewsletterValues()
    {
        return new Validator()
            .NotEmpty(txtNewsletterDisplayName.Text, GetString("general.requiresdisplayname"))
            .NotEmpty(txtNewsletterName.Text, GetString("Newsletter_Edit.ErrorEmptyName"))
            .NotEmpty(txtNewsletterSenderName.Text, GetString("Newsletter_Edit.ErrorEmptySenderName"))
            .NotEmpty(txtNewsletterSenderEmail.Text, GetString("Newsletter_Edit.ErrorEmptySenderEmail"))
            .IsEmail(txtNewsletterSenderEmail.Text.Trim(), GetString("Newsletter_Edit.ErrorEmailFormat"))
            .IsCodeName(txtNewsletterName.Text, GetString("general.invalidcodename"))
            .Result;
    }


    /// <summary>
    /// Sets newsletter object from config form data.
    /// </summary>
    /// <param name="newsletterObj">Newsletter object</param>
    private void SetNewsletterValues(NewsletterInfo newsletterObj)
    {
        newsletterObj.NewsletterDisplayName = txtNewsletterDisplayName.Text.Trim();
        newsletterObj.NewsletterName = txtNewsletterName.Text.Trim();
        newsletterObj.NewsletterSenderName = txtNewsletterSenderName.Text.Trim();
        newsletterObj.NewsletterSenderEmail = txtNewsletterSenderEmail.Text.Trim();
        newsletterObj.NewsletterBaseUrl = txtNewsletterBaseUrl.Text.Trim();
        newsletterObj.NewsletterUnsubscribeUrl = txtNewsletterUnsubscribeUrl.Text.Trim();
        newsletterObj.NewsletterDraftEmails = txtDraftEmails.Text;
        newsletterObj.NewsletterUseEmailQueue = chkUseEmailQueue.Checked;
        newsletterObj.NewsletterEnableResending = chkEnableResending.Checked;
        newsletterObj.NewsletterTrackOpenEmails = TrackingEnabled && chkTrackOpenedEmails.Checked;
        newsletterObj.NewsletterTrackClickedLinks = TrackingEnabled && chkTrackClickedLinks.Checked;
        newsletterObj.NewsletterLogActivity = OnlineMarketingEnabled && chkLogActivity.Checked;
        newsletterObj.NewsletterEnableOptIn = chkEnableOptIn.Checked && (ValidationHelper.GetInteger(optInSelector.Value, 0) > 0);
        newsletterObj.NewsletterOptInApprovalURL = txtOptInURL.Text.Trim();
        newsletterObj.NewsletterSendOptInConfirmation = chkSendOptInConfirmation.Checked;
    }


    /// <summary>
    /// Returns existing or new task info object.
    /// </summary>
    /// <param name="newsletterObj">Newsletter object</param>
    private static TaskInfo GetDynamicNewsletterTask(NewsletterInfo newsletterObj)
    {
        return TaskInfoProvider.GetTaskInfo(newsletterObj.NewsletterDynamicScheduledTaskID) ?? CreateDynamicNewsletterTask(newsletterObj);
    }


    /// <summary>
    /// Creates new task info object for given newsletter.
    /// </summary>
    /// <param name="newsletterObj">Newsletter object</param>
    private static TaskInfo CreateDynamicNewsletterTask(NewsletterInfo newsletterObj)
    {
        return new TaskInfo
                   {
                       TaskAssemblyName = "CMS.Newsletters",
                       TaskClass = "CMS.Newsletters.DynamicNewsletterSender",
                       TaskEnabled = true,
                       TaskLastResult = string.Empty,
                       TaskSiteID = SiteContext.CurrentSiteID,
                       TaskData = newsletterObj.NewsletterGUID.ToString(),
                       TaskType = ScheduledTaskTypeEnum.System
                   };
    }

    #endregion
}