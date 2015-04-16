using System;

using CMS.ExtendedControls;
using CMS.Helpers;
using CMS.Newsletters;
using CMS.PortalEngine;
using CMS.Scheduler;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.Base;

[Breadcrumbs]
[Breadcrumb(0, ResourceString = "Newsletter_Edit.ItemListLink", TargetUrl = "~/CMSModules/Newsletters/Tools/Newsletters/Newsletter_List.aspx")]
[Breadcrumb(1, ResourceString = "Newsletter_Edit.NewItemCaption")]
[EditedObject("newsletter.newsletter", "objectid")]
[Title("newsletters.newsletters")]
[UIElement("CMS.Newsletter", "AddANewNewsletter")]
public partial class CMSModules_Newsletters_Tools_Newsletters_Newsletter_New : CMSNewsletterPage
{
    #region "Properties"

    protected NewsletterInfo TypedEditedObject
    {
        get
        {
            return EditedObject as NewsletterInfo;
        }
        set
        {
            EditedObject = value;
        }
    }

    #endregion


    #region "Events"

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!URLHelper.IsPostback())
        {
            SetDefaultValues();
        }

        // Hide detailed report as it could confuse non-technical users
        NewForm.FieldControls["NewsletterDynamicURL"].SetValue("ShowDetailedError", false);
        NewForm.FieldControls["NewsletterDynamicURL"].SetValue("StatusErrorMessage", GetString("general.pagenotfound"));

        ShowCustomRequiredMarks();
    }


    protected void Page_PreRender(object sender, EventArgs e)
    {
        SetVisibility();
    }


    protected void BeforeSave(object sender, EventArgs e)
    {
        // Set site ID
        NewForm.Data.SetValue("NewsletterSiteID", SiteContext.CurrentSiteID);

        // Clear other possibilities
        if (GetUpperNewsletterType() == NewsletterType.Dynamic)
        {
            NewForm.Data.SetValue("NewsletterTemplateID", null);
        }
        else
        {
            NewForm.Data.SetValue("NewsletterDynamicURL", null);
            NewForm.Data.SetValue("NewsletterDynamicScheduledTaskID", null);
        }
    }


    protected void ValidateValues(object sender, EventArgs e)
    {
        // Validate unsubscription template
        if (ValidationHelper.GetInteger(NewForm.GetFieldValue("NewsletterUnsubscriptionTemplateID"), 0) == 0)
        {
            ShowError(GetString("Newsletter_Edit.NoUnsubscriptionTemplateSelected"));
            NewForm.StopProcessing = true;
        }

        // Validate subscription template
        if (ValidationHelper.GetInteger(NewForm.GetFieldValue("NewsletterSubscriptionTemplateID"), 0) == 0)
        {
            ShowError(GetString("Newsletter_Edit.NoSubscriptionTemplateSelected"));
            NewForm.StopProcessing = true;
        }

        // If Template based, validate template
        if (GetUpperNewsletterType() == NewsletterType.TemplateBased && ValidationHelper.GetInteger(NewForm.GetFieldValue("NewsletterTemplateID"), 0) == 0)
        {
            ShowError(GetString("Newsletter_Edit.NoEmailTemplateSelected"));
            NewForm.StopProcessing = true;
        }

        // If Dynamic, validate schedule interval and Source page URL
        if (GetUpperNewsletterType() == NewsletterType.Dynamic)
        {
            if (ValidationHelper.GetString(NewForm.GetFieldValue("NewsletterDynamicURL"), string.Empty) == string.Empty)
            {
                // Source page URL can not be empty
                ShowError(GetString("newsletter_edit.sourcepageurlempty"));
                NewForm.StopProcessing = true;
            }

            if (chkSchedule.Checked)
            {
                if (!ScheduleInterval.CheckOneDayMinimum())
                {
                    // Problem occurred while setting schedule interval for dynamic newsletter
                    ShowError(GetString("Newsletter_Edit.NoDaySelected"));
                    NewForm.StopProcessing = true;
                }

                if (SchedulingHelper.DecodeInterval(ScheduleInterval.ScheduleInterval).StartTime == DateTime.MinValue)
                {
                    ShowError(GetString("Newsletter.IncorrectDate"));
                    NewForm.StopProcessing = true;
                }
            }
        }
    }


    protected void AfterSave(object sender, EventArgs e)
    {
        if ((GetUpperNewsletterType() == NewsletterType.Dynamic) && chkSchedule.Checked)
        {
            // If Scheduling is enabled, create task
            CreateTask();
        }
        else
        {
            // Redirect to newly created newsletter
            Redirect();
        }
    }


    protected void chkSchedule_CheckedChanged(object sender, EventArgs e)
    {
        // Set visibility for schedule interval control
        ScheduleInterval.Visible = chkSchedule.Checked;
    }

    #endregion


    #region "Private methods"

    /// <summary>
    /// Sets visibility of controls.
    /// </summary>
    private void SetVisibility()
    {
        plcSchedule.Visible = (GetUpperNewsletterType() == NewsletterType.Dynamic);
        ScheduleInterval.Visible = chkSchedule.Checked && plcSchedule.Visible;
    }


    /// <summary>
    /// Sets default values of controls.
    /// </summary>
    private void SetDefaultValues()
    {
        if (ScheduleInterval.StartTime.SelectedDateTime == DateTimeHelper.ZERO_TIME)
        {
            ScheduleInterval.StartTime.SelectedDateTime = DateTime.Now;
        }
    }


    /// <summary>
    /// Shows required field marks not handled automatically.
    /// </summary>
    private void ShowCustomRequiredMarks()
    {
        // Show required marks for fields visible only if the template-based type is selected
        LocalizedLabel templateLabel = NewForm.FieldLabels["NewsletterTemplateID"];
        if (templateLabel != null)
        {
            templateLabel.ShowRequiredMark = true;
        }

        // Show required marks for fields visible only if the dynamic type is selected
        LocalizedLabel dynamicUrlLabel = NewForm.FieldLabels["NewsletterDynamicURL"];
        if (dynamicUrlLabel != null)
        {
            dynamicUrlLabel.ShowRequiredMark = true;
        }
    }


    /// <summary>
    /// Returns newsletter type (upper).
    /// </summary>
    /// <returns>Newsletter Type string</returns>
    private string GetUpperNewsletterType()
    {
        return ValidationHelper.GetString(NewForm.GetFieldValue("NewsletterType"), "").ToUpperCSafe();
    }


    /// <summary>
    /// Redirects to newly created newsletter.
    /// </summary>
    private void Redirect()
    {
        if (TypedEditedObject != null)
        {
            string url = UIContextHelper.GetElementUrl("cms.newsletter", "EditNewsletterProperties", false);
            url = URLHelper.AddParameterToUrl(url, "objectid", Convert.ToString(TypedEditedObject.NewsletterID));
            url = URLHelper.AddParameterToUrl(url, "tabindex", "1");
            url = URLHelper.AddParameterToUrl(url, "saved", "1");
            URLHelper.Redirect(url);
        }
    }


    /// <summary>
    /// Create schedule task.
    /// </summary>
    private void CreateTask()
    {
        try
        {
            var editedObject = TypedEditedObject;
            var newsletterGUID = editedObject.NewsletterGUID.ToString();
            var maxLength = 200;

            TaskInfo task = new TaskInfo();
            task.TaskAssemblyName = "CMS.Newsletters";
            task.TaskClass = "CMS.Newsletters.DynamicNewsletterSender";
            task.TaskDisplayName = TextHelper.LimitLength(GetString("DynamicNewsletter.TaskName") + editedObject.NewsletterDisplayName, maxLength, CutTextEnum.End);
            task.TaskEnabled = true;
            task.TaskInterval = ScheduleInterval.ScheduleInterval;
            task.TaskLastResult = string.Empty;
            task.TaskName = ValidationHelper.GetCodeName("DynamicNewsletter." + editedObject.NewsletterName, "_", maxLength - newsletterGUID.Length) + newsletterGUID;
            task.TaskSiteID = SiteContext.CurrentSiteID;
            task.TaskNextRunTime = SchedulingHelper.GetNextTime(task.TaskInterval, new DateTime(), new DateTime());
            task.TaskData = newsletterGUID;
            // Set task for processing in external service
            task.TaskAllowExternalService = true;
            task.TaskUseExternalService = (SchedulingHelper.UseExternalService && NewsletterHelper.UseExternalServiceForDynamicNewsletters(SiteContext.CurrentSiteName));
            task.TaskType = ScheduledTaskTypeEnum.System;

            TaskInfoProvider.SetTaskInfo(task);

            editedObject.NewsletterDynamicScheduledTaskID = task.TaskID;
            NewsletterInfoProvider.SetNewsletterInfo(editedObject);

            Redirect();
        }
        catch (Exception ex)
        {
            ShowError(GetString(ex.Message));
        }
    }

    #endregion
}