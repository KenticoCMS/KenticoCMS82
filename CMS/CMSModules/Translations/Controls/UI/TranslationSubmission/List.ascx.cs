using System;
using System.Data;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Web.UI.WebControls;

using CMS.DataEngine;
using CMS.ExtendedControls.ActionsConfig;
using CMS.FormControls;
using CMS.Localization;
using CMS.Membership;
using CMS.Base;
using CMS.Helpers;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.TranslationServices;
using CMS.ExtendedControls;

public partial class CMSModules_Translations_Controls_UI_TranslationSubmission_List : CMSAdminListControl
{
    #region "Constants"

    private const string PROCESS_ACTION = "process";
    private const string RESUBMIT_ACTION = "resubmit";
    private const string UPDATE_STATUSES_ACTION = "updatestatuses";

    #endregion


    #region "Variables"

    private bool modifyAllowed;

    #endregion


    #region "Properties"

    /// <summary>
    /// Inner grid.
    /// </summary>
    public UniGrid Grid
    {
        get
        {
            return gridElem;
        }
    }


    /// <summary>
    /// Indicates if the control should perform the operations.
    /// </summary>
    public override bool StopProcessing
    {
        get
        {
            return base.StopProcessing;
        }
        set
        {
            base.StopProcessing = value;
            gridElem.StopProcessing = value;
        }
    }


    /// <summary>
    /// Indicates if the control is used on the live site.
    /// </summary>
    public override bool IsLiveSite
    {
        get
        {
            return base.IsLiveSite;
        }
        set
        {
            base.IsLiveSite = value;
            gridElem.IsLiveSite = value;
        }
    }

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        modifyAllowed = MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("CMS.TranslationServices", "Modify");

        gridElem.WhereCondition = "SubmissionSiteID = " + SiteContext.CurrentSiteID;
        gridElem.OnExternalDataBound += gridElem_OnExternalDataBound;
        gridElem.OnAction += gridElem_OnAction;
        ctlAsync.OnError += worker_OnError;
        ctlAsync.OnFinished += worker_OnFinished;
        HeaderActions.ActionPerformed += HeaderActions_ActionPerformed;
        Page.PreRender += (o, args) =>
        {
            ScriptHelper.RegisterDialogScript(Page);

            CreateButtons();

            HandleAsyncProgress();
        };
    }


    protected void HeaderActions_ActionPerformed(object sender, CommandEventArgs e)
    {
        // There is already a running thread
        if (IsRunningThread())
        {
            return;
        }

        switch (e.CommandName)
        {
            case UPDATE_STATUSES_ACTION:
                if (!MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("CMS.TranslationServices", "Modify"))
                {
                    RedirectToAccessDenied("CMS.TranslationServices", "Modify");
                }

                UpdateStatusesAsync();
                break;
        }
    }


    protected void gridElem_OnAction(string actionName, object actionArgument)
    {
        // There is already a running thread
        if (IsRunningThread())
        {
            return;
        }

        string err = null;
        string info = null;

        // Check modify permission for all actions except for download ZIP
        if (!actionName.EqualsCSafe("downloadzip", true))
        {
            if (!modifyAllowed)
            {
                RedirectToAccessDenied("CMS.TranslationServices", "Modify");
            }
        }

        // Get submission
        var submissionInfo = TranslationSubmissionInfoProvider.GetTranslationSubmissionInfo(ValidationHelper.GetInteger(actionArgument, 0));
        if (submissionInfo == null)
        {
            return;
        }

        switch (actionName.ToLowerCSafe())
        {
            case "downloadzip":
                TranslationServiceHelper.DownloadXLIFFinZIP(submissionInfo, Page.Response);
                break;

            case RESUBMIT_ACTION:
                ProcessActionAsync(actionName, submissionInfo);
                break;

            case PROCESS_ACTION:
                ProcessActionAsync(actionName, submissionInfo);
                break;

            case "cancel":
                err = TranslationServiceHelper.CancelSubmission(submissionInfo);
                info = GetString("translationservice.submissioncanceled");
                break;

            case "delete":
                err = TranslationServiceHelper.CancelSubmission(submissionInfo);
                if (String.IsNullOrEmpty(err))
                {
                    submissionInfo.Delete();
                }
                info = GetString("translationservice.submissiondeleted");
                break;
        }

        if (!string.IsNullOrEmpty(err))
        {
            ShowError(err);
        }
        else if (!string.IsNullOrEmpty(info))
        {
            ShowConfirmation(info);
        }
    }


    protected object gridElem_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        // For all actions
        if (sourceName.EndsWithCSafe("action", true))
        {
            var img = sender as CMSGridActionButton;
            if (img != null)
            {
                img.Enabled = !IsRunningThread();
            }
        }

        switch (sourceName.ToLowerCSafe())
        {
            case "resubmitaction":
            case "processaction":
            case "cancelaction":
                var img = sender as CMSGridActionButton;
                if (img != null)
                {
                    img.Enabled &= modifyAllowed;

                    var gvr = parameter as GridViewRow;
                    if (gvr == null)
                    {
                        return img;
                    }

                    var drv = gvr.DataItem as DataRowView;
                    if (drv == null)
                    {
                        return img;
                    }

                    var status = (TranslationStatusEnum)ValidationHelper.GetInteger(drv["SubmissionStatus"], 0);

                    switch (sourceName.ToLowerCSafe())
                    {
                        case "resubmitaction":
                            img.Enabled &= modifyAllowed && ((status == TranslationStatusEnum.WaitingForTranslation) || (status == TranslationStatusEnum.SubmissionError));
                            break;

                        case "processaction":
                            img.Enabled &= modifyAllowed && ((status == TranslationStatusEnum.TranslationReady) || (status == TranslationStatusEnum.TranslationCompleted) || (status == TranslationStatusEnum.ProcessingError));
                            break;

                        case "cancelaction":
                            var service = TranslationServiceInfoProvider.GetTranslationServiceInfo(ValidationHelper.GetInteger(drv["SubmissionServiceID"], 0));
                            if (service != null)
                            {
                                bool serviceSupportsCancel = service.TranslationServiceSupportsCancel;

                                img.Enabled &= modifyAllowed && (status == TranslationStatusEnum.WaitingForTranslation) && serviceSupportsCancel;

                                if (!serviceSupportsCancel)
                                {
                                    // Display tooltip for disabled cancel
                                    img.ToolTip = String.Format(GetString("translationservice.cancelnotsupported"), service.TranslationServiceDisplayName);
                                }
                            }
                            break;
                    }
                }
                return img;

            case "submissionstatus":
                TranslationStatusEnum submissionstatus = (TranslationStatusEnum)ValidationHelper.GetInteger(parameter, 0);
                return TranslationServiceHelper.GetFormattedStatusString(submissionstatus);

            case "submissionprice":
                string price = GetString("general.notavailable");
                double priceVal = ValidationHelper.GetDouble(parameter, -1);
                if (priceVal >= 0)
                {
                    price = priceVal.ToString();
                }
                return price;

            case "submissiontargetculture":
                {
                    string[] cultureCodes = ValidationHelper.GetString(parameter, "").Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

                    StringBuilder sb = new StringBuilder();

                    int count = cultureCodes.Length;
                    int counter = 0;
                    foreach (var cultureCode in cultureCodes)
                    {
                        // Limit cultures list
                        if (counter == 5)
                        {
                            sb.Append("&nbsp;");
                            sb.AppendFormat(ResHelper.GetString("translationservices.submissionnamesuffix"), count - 5);
                            break;
                        }
                        // Separate cultures by comma
                        if (counter > 0)
                        {
                            sb.Append(",&nbsp;");
                        }

                        var culture = CultureInfoProvider.GetCultureInfo(cultureCode);
                        if (culture == null)
                        {
                            continue;
                        }

                        sb.AppendFormat("<span title=\"{0}\"><img class=\"cms-icon-80\" src=\"{1}\" alt=\"{2}\" />&nbsp;{3}</span>", HTMLHelper.EncodeForHtmlAttribute(culture.CultureName), UIHelper.GetFlagIconUrl(null, culture.CultureCode, "16x16"), HTMLHelper.EncodeForHtmlAttribute(culture.CultureName), HTMLHelper.HTMLEncode(culture.CultureShortName));
                        ++counter;
                    }

                    return sb.ToString();
                }
        }

        return parameter;
    }


    private void worker_OnFinished(object sender, EventArgs e)
    {
        var parameters = ctlAsync.Parameter as Tuple<string, string, TranslationSubmissionInfo>;
        if (parameters == null)
        {
            return;
        }

        var commandName = parameters.Item1.ToLowerCSafe();
        var error = parameters.Item2;
        var submission = parameters.Item3;
        var submissionName = submission != null ? submission.SubmissionName : null;

        if (!string.IsNullOrEmpty(error))
        {
            // Show error from the service
            ShowError(error);
        }
        else
        {
            string message = String.Empty;

            // Get correct confirmation message
            switch (commandName)
            {
                case PROCESS_ACTION:
                    message = "translationservice.name.translationsimported";
                    break;

                case RESUBMIT_ACTION:
                    message = "translationservice.name.translationresubmitted";
                    break;

                case UPDATE_STATUSES_ACTION:
                    message = "translationservice.updatingstatusesfinished";
                    break;
            }

            if (!String.IsNullOrEmpty(message))
            {
                ShowConfirmation(string.Format(GetString(message), HTMLHelper.HTMLEncode(submissionName)));
            }
        }
    }


    private void worker_OnError(object sender, EventArgs e)
    {
        ShowError(GetString("translationservice.actionfailed"));
    }


    /// <summary>
    /// Processes the action asynchronously
    /// </summary>
    /// <param name="commandName">Command name</param>
    /// <param name="submission">Translation submission</param>
    private void ProcessActionAsync(string commandName, TranslationSubmissionInfo submission)
    {
        // Set flag
        submission.SubmissionStatus = commandName.EqualsCSafe(RESUBMIT_ACTION, true) ? TranslationStatusEnum.ResubmittingSubmission : TranslationStatusEnum.ProcessingSubmission;
        submission.Update();

        // Run action
        ctlAsync.Parameter = new Tuple<string, TranslationSubmissionInfo>(commandName, submission);
        ctlAsync.RunAsync(ProcessAction, WindowsIdentity.GetCurrent());
    }


    /// <summary>
    /// Processes action
    /// </summary>
    /// <param name="parameter">Parameter</param>
    private void ProcessAction(object parameter)
    {
        var parameters = parameter as Tuple<string, TranslationSubmissionInfo>;
        if (parameters == null)
        {
            return;
        }

        var commandName = parameters.Item1.ToLowerCSafe();
        var submissionInfo = parameters.Item2;
        var error = String.Empty;

        switch (commandName)
        {
            case RESUBMIT_ACTION:
                error = TranslationServiceHelper.ResubmitSubmission(submissionInfo);
                break;

            case PROCESS_ACTION:
                error = TranslationServiceHelper.ProcessSubmission(submissionInfo);
                break;
        }

        // Set result of the action
        ctlAsync.Parameter = new Tuple<string, string, TranslationSubmissionInfo>(commandName, error, submissionInfo);
    }


    /// <summary>
    /// Updates statuses of submissions asynchronously
    /// </summary>
    private void UpdateStatusesAsync()
    {
        // Run action
        ctlAsync.Parameter = SiteContext.CurrentSiteName;
        ctlAsync.RunAsync(UpdateStatuses, WindowsIdentity.GetCurrent());

        Grid.ReloadData();
    }


    /// <summary>
    /// Processes action
    /// </summary>
    /// <param name="parameter">Parameter</param>
    private void UpdateStatuses(object parameter)
    {
        var siteName = ValidationHelper.GetString(parameter, null);
        var error = TranslationServiceHelper.CheckAndDownloadTranslations(siteName);

        // Set result of the action
        ctlAsync.Parameter = new Tuple<string, string, TranslationSubmissionInfo>(UPDATE_STATUSES_ACTION, error, null);
    }


    /// <summary>
    /// Indicates if running thread exists
    /// </summary>
    private bool IsRunningThread()
    {
        return ctlAsync.Status == AsyncWorkerStatusEnum.Running;
    }


    /// <summary>
    /// Ensures changes in UI to indicate asynchronous progress
    /// </summary>
    private void HandleAsyncProgress()
    {
        if (Grid.IsEmpty)
        {
            return;
        }

        string statusCheck = SettingsKeyInfoProvider.GetValue(SiteContext.CurrentSiteName + ".CMSTranslationsLastStatusCheck");
        if (string.IsNullOrEmpty(statusCheck))
        {
            statusCheck = GetString("general.notavailable");
        }

        ShowInformation(string.Format(GetString("translationservice.laststatuscheck"), statusCheck));

        var page = Page as CMSPage;
        if (page == null)
        {
            return;
        }

        var threadRunning = IsRunningThread();
        if (threadRunning)
        {
            var label = LoadUserControl("~/CMSFormControls/Basic/LabelControl.ascx") as FormEngineUserControl;
            if (label != null)
            {
                label.ID = "lblStatus";

                string text = null;
                var parameters = ctlAsync.Parameter as Tuple<string, TranslationSubmissionInfo>;
                if (parameters != null)
                {
                    var status = parameters.Item1 == PROCESS_ACTION ? TranslationStatusEnum.ProcessingSubmission : TranslationStatusEnum.ResubmittingSubmission;
                    var submission = parameters.Item2;
                    if (submission != null)
                    {
                        text = string.Format(GetString(status.ToLocalizedString("translations.status.name")), HTMLHelper.HTMLEncode(submission.SubmissionName));
                    }
                }
                else
                {
                    text = GetString("translationservice.updatingstatuses");
                }

                label.Value = ScriptHelper.GetLoaderInlineHtml(Page, text);
                HeaderActions.AdditionalControls.Add(label);
                HeaderActions.AdditionalControlsCssClass = "header-actions-label control-group-inline";
                HeaderActions.ReloadAdditionalControls();
            }
        }

        HeaderActions.Enabled = !threadRunning;
    }


    /// <summary>
    /// Creates buttons in header actions
    /// </summary>
    private void CreateButtons()
    {
        var updateAction = new HeaderAction
        {
            ButtonStyle = ButtonStyle.Default,
            CommandName = UPDATE_STATUSES_ACTION,
            Tooltip = GetString("translationservice.updatestatusestooltip"),
            Text = GetString("translationservice.updatestatuses"),
            Enabled = MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("CMS.TranslationServices", "Modify") && !Grid.IsEmpty
        };

        string translateUrl = AuthenticationHelper.ResolveDialogUrl("~/CMSModules/Translations/Pages/TranslateDocuments.aspx") + "?select=1&dialog=1";
        translateUrl = URLHelper.AddParameterToUrl(translateUrl, "hash", QueryHelper.GetHash(URLHelper.GetQuery(translateUrl)));

        // Check if any human translation is enabled
        bool enabled = TranslationServiceInfoProvider.GetTranslationServices("(TranslationServiceEnabled = 1) AND (TranslationServiceIsMachine = 0)", null, 0, "TranslationServiceID, TranslationServiceName").Any(t => TranslationServiceHelper.IsServiceAvailable(t.TranslationServiceName, SiteContext.CurrentSiteName));

        var submitAction = new HeaderAction
        {
            OnClientClick = "modalDialog('" + translateUrl + "', 'SubmitTranslation', 988, 640);",
            Tooltip = GetString(enabled ? "translationservice.submittranslationtooltip" : "translationservice.noenabledservices"),
            Text = GetString("translationservice.submittranslation"),
            Enabled = enabled && MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("CMS.Content", "SubmitForTranslation")
        };

        AddHeaderAction(submitAction);
        AddHeaderAction(updateAction);

        HeaderActions.ReloadData();
    }

    #endregion
}