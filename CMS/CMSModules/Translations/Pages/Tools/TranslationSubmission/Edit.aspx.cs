using System;
using System.Security.Principal;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.DataEngine;
using CMS.FormControls;
using CMS.Helpers;
using CMS.SiteProvider;
using CMS.Membership;
using CMS.UIControls;
using CMS.TranslationServices;
using CMS.ExtendedControls;
using CMS.ExtendedControls.ActionsConfig;


// Edited object
[EditedObject(TranslationSubmissionInfo.OBJECT_TYPE, "submissionId")]

// Breadcrumbs
[Breadcrumbs]
[Breadcrumb(0, "translationservice.translationsubmission.list", "~/CMSModules/Translations/Pages/Tools/TranslationSubmission/List.aspx", null)]
[Breadcrumb(1, Text = "{%EditedObject.DisplayName%}", ExistingObject = true)]
[Breadcrumb(1, ResourceString = "translationservice.translationsubmission.new", NewObject = true)]

// Title
[Title(ResourceString = "translationservice.translationsubmission.edit", HelpTopic = "translationservices_translationsubmission_edit", ExistingObject = true)]
[Title(ResourceString = "translationservice.translationsubmission.new", HelpTopic = "translationservices_translationsubmission_edit", NewObject = true)]

public partial class CMSModules_Translations_Pages_Tools_TranslationSubmission_Edit : CMSTranslationServicePage
{
    #region "Constants"

    private const string PROCESS_ACTION = "process";
    private const string RESUBMIT_ACTION = "resubmit";

    #endregion


    #region "Variables"

    TranslationSubmissionInfo mSubmissionInfo;

    #endregion


    #region "Properties"

    /// <summary>
    /// Returns submission info being edited.
    /// </summary>
    public TranslationSubmissionInfo SubmissionInfo
    {
        get
        {
            return mSubmissionInfo ?? (mSubmissionInfo = (TranslationSubmissionInfo)editElem.UIFormControl.EditedObject);
        }
    }

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        btnShowMessage.Click += btnShowMessage_Click;
        HeaderActions.ActionPerformed += HeaderActions_ActionPerformed;
        ctlAsync.OnError += worker_OnError;
        ctlAsync.OnFinished += worker_OnFinished;
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        if (SubmissionInfo == null)
        {
            return;
        }

        CreateButtons();
        HandleAsyncProgress();

        ScriptHelper.RegisterClientScriptBlock(Page, typeof(string), "ShowUploadSuccess", "function ShowUploadSuccess() { " + ControlsHelper.GetPostBackEventReference(btnShowMessage) + " }", true);
    }


    protected void HeaderActions_ActionPerformed(object sender, CommandEventArgs e)
    {
        // There is already a running thread
        if (IsRunningThread())
        {
            return;
        }

        CheckModifyPermissions(true);

        switch (e.CommandName)
        {
            case ComponentEvents.SAVE:
                if (!editElem.UIFormControl.SaveData(null))
                {
                    ShowError(GetString("translationservice.savesubmissionfailed"));
                }
                break;

            case RESUBMIT_ACTION:
                ProcessActionAsync(e.CommandName);
                break;

            case "updateandresubmit":
                if (!editElem.UIFormControl.SaveData(null))
                {
                    ShowError(GetString("translationservice.savesubmissionfailed"));
                }
                else
                {
                    ProcessActionAsync("resubmit");
                }
                break;

            case "cancel":
                string errCancel = TranslationServiceHelper.CancelSubmission(SubmissionInfo);
                if (string.IsNullOrEmpty(errCancel))
                {
                    ShowConfirmation(GetString("translationservice.submissioncanceled"));
                }
                else
                {
                    ShowError(errCancel);
                }
                editElem.UIFormControl.ReloadData();
                break;

            case PROCESS_ACTION:
                ProcessActionAsync(e.CommandName);
                break;
        }
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
        var current = (submission != null) && (submission.SubmissionID == SubmissionInfo.SubmissionID);

        if (!string.IsNullOrEmpty(error))
        {
            // Show error from the service for current submission
            if (current)
            {
                ShowError(error);
            }
        }
        else
        {
            string message = String.Empty;

            // Get correct confirmation message
            switch (commandName)
            {
                case PROCESS_ACTION:
                    message = current ? "translationservice.translationsimported" : "translationservice.name.translationsimported";
                    break;

                case RESUBMIT_ACTION:
                    message = current ? "translationservice.translationresubmitted" : "translationservice.name.translationresubmitted";
                    break;
            }

            if (!String.IsNullOrEmpty(message))
            {
                var text = !current && (submission != null) ? string.Format(GetString(message), HTMLHelper.HTMLEncode(submission.SubmissionName)) : GetString(message);
                ShowConfirmation(text);
            }
        }

        // Reload is required because status of the submission could have been changed and form has to be reloaded
        editElem.UIFormControl.ReloadData();
    }


    private void worker_OnError(object sender, EventArgs e)
    {
        ShowError(GetString("translationservice.actionfailed"));

        // Reload is required because status of the submission could have been changed and form has to be reloaded
        editElem.UIFormControl.ReloadData();
    }


    protected void btnShowMessage_Click(object sender, EventArgs e)
    {
        ShowInformation(GetString("translationservice.translationuploadedsuccessfully"));

        // Reload is required because status of the submission could have been changed and form has to be reloaded
        editElem.UIFormControl.ReloadData();
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Processes the action asynchronously
    /// </summary>
    /// <param name="commandName">Command name</param>
    private void ProcessActionAsync(string commandName)
    {
        // Set flag
        SubmissionInfo.SubmissionStatus = commandName.EqualsCSafe(RESUBMIT_ACTION, true) ? TranslationStatusEnum.ResubmittingSubmission : TranslationStatusEnum.ProcessingSubmission;
        SubmissionInfo.Update();

        // Run action
        ctlAsync.Parameter = new Tuple<string, TranslationSubmissionInfo>(commandName, SubmissionInfo);
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
    /// Checks Modify permissions for given translation submission.
    /// </summary>
    /// <param name="redirect">If true, redirects user to Access denied</param>
    private bool CheckModifyPermissions(bool redirect)
    {
        var submission = EditedObject as TranslationSubmissionInfo;
        if (submission == null)
        {
            return true;
        }

        if (MembershipContext.AuthenticatedUser.IsAuthorizedPerObject(PermissionsEnum.Modify, submission, SiteInfoProvider.GetSiteName(submission.SubmissionSiteID)))
        {
            return true;
        }

        if (redirect)
        {
            RedirectToAccessDenied("CMS.TranslationServices", "Modify");
        }

        return false;
    }


    /// <summary>
    /// Ensures changes in UI to indicate asynchronous progress
    /// </summary>
    private void HandleAsyncProgress()
    {
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
                        var current = (submission.SubmissionID == SubmissionInfo.SubmissionID);
                        text = current ? GetString(status.ToLocalizedString("translations.status")) : string.Format(GetString(status.ToLocalizedString("translations.status.name")), HTMLHelper.HTMLEncode(submission.SubmissionName));
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
        editElem.UIFormControl.Enabled = !threadRunning;
    }


    /// <summary>
    /// Indicates if running thread exists
    /// </summary>
    private bool IsRunningThread()
    {
        return ctlAsync.Status == AsyncWorkerStatusEnum.Running;
    }


    /// <summary>
    /// Creates buttons in header actions
    /// </summary>
    private void CreateButtons()
    {
        bool allowed = CheckModifyPermissions(false);

        var processAction = new HeaderAction
        {
            ButtonStyle = ButtonStyle.Default,
            CommandName = PROCESS_ACTION,
            OnClientClick = "if (!confirm(" + ScriptHelper.GetString(GetString("translationservice.confirmprocesstranslations")) + ")) { return false; }",
            Tooltip = GetString("translationservice.importtranslationstooltip"),
            Text = GetString("translationservice.importtranslations"),
            Enabled = allowed && ((SubmissionInfo.SubmissionStatus == TranslationStatusEnum.TranslationReady) || (SubmissionInfo.SubmissionStatus == TranslationStatusEnum.TranslationCompleted) || (SubmissionInfo.SubmissionStatus == TranslationStatusEnum.ProcessingError))
        };

        var resubmitAction = new HeaderAction
        {
            ButtonStyle = ButtonStyle.Default,
            CommandName = RESUBMIT_ACTION,
            Tooltip = GetString("translationservice.resubmittooltip"),
            Text = GetString("translationservice.resubmit"),
            Enabled = allowed && ((SubmissionInfo.SubmissionStatus == TranslationStatusEnum.WaitingForTranslation) || (SubmissionInfo.SubmissionStatus == TranslationStatusEnum.SubmissionError))
        };

        var updateAction = new HeaderAction
        {
            ButtonStyle = ButtonStyle.Default,
            CommandName = "updateandresubmit",
            Tooltip = GetString("translationservice.updateandresubmittooltip"),
            Text = GetString("translationservice.updateandresubmit"),
            Enabled = allowed && ((SubmissionInfo.SubmissionStatus == TranslationStatusEnum.WaitingForTranslation) || (SubmissionInfo.SubmissionStatus == TranslationStatusEnum.SubmissionError))
        };

        var saveAction = new SaveAction(Page);
        saveAction.Enabled = allowed;

        var actions = HeaderActions.ActionsList;
        actions.AddRange(new[]
        {
            saveAction,
            updateAction,
            resubmitAction,
            processAction
        });

        // Check if current service supports canceling
        var service = TranslationServiceInfoProvider.GetTranslationServiceInfo(SubmissionInfo.SubmissionServiceID);
        if (service != null)
        {
            bool serviceSupportsCancel = service.TranslationServiceSupportsCancel;

            var cancelAction = new HeaderAction
            {
                ButtonStyle = ButtonStyle.Default,
                CommandName = "cancel",
                Tooltip = serviceSupportsCancel ? GetString("translationservice.cancelsubmissiontooltip") : String.Format(GetString("translationservice.cancelnotsupported"), service.TranslationServiceDisplayName),
                Text = GetString("translationservice.cancelsubmission"),
                Enabled = allowed && (SubmissionInfo.SubmissionStatus == TranslationStatusEnum.WaitingForTranslation) && serviceSupportsCancel
            };

            actions.Add(cancelAction);
        }

        HeaderActions.ReloadData();
    }

    #endregion
}
