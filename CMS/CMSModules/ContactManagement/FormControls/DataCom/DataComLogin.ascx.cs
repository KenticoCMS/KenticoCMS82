using System;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.UI.WebControls;

using CMS.DataCom;
using CMS.ExtendedControls;
using CMS.ExtendedControls.ActionsConfig;
using CMS.FormControls;
using CMS.Helpers;
using CMS.Base;
using CMS.WorkflowEngine;

public partial class CMSModules_ContactManagement_FormControls_DataCom_DataComLogin : FormEngineUserControl
{
    #region "Variables"

    private WorkflowStepInfo mWorkflowStep;
    private NetworkCredential mCredential;

    #endregion


    #region "Properties"

    /// <summary>
    /// Value of the control - Data.com UserName.
    /// </summary>
    public override object Value
    {
        get
        {
            return Credential.UserName;
        }
        set { }
    }


    /// <summary>
    /// Returns redirect URL to login page.
    /// </summary>
    private string LoginPageUrl
    {
        get
        {
            var url = "~/CMSModules/ContactManagement/Pages/Tools/DataCom/Login.aspx";
            url = URLHelper.AddParameterToUrl(url, "returnurl", HttpUtility.UrlEncode(RequestContext.CurrentURL));
            url = URLHelper.AddParameterToUrl(url, "workflowstepid", QueryHelper.GetString("workflowstepid", ""));
            return url;
        }
    }


    /// <summary>
    /// Returns workflow step.
    /// </summary>
    private WorkflowStepInfo WorkflowStep
    {
        get
        {
            return mWorkflowStep ?? (mWorkflowStep = WorkflowStepInfoProvider.GetWorkflowStepInfo(QueryHelper.GetInteger("workflowstepid", 0)));
        }
    }


    /// <summary>
    /// Returns credential saved in step. Null if no credentials were saved.
    /// </summary>
    private NetworkCredential Credential
    {
        get
        {
            if (mCredential == null)
            {
                mCredential = new StepCredentialProvider(WorkflowStep).GetCredential();
            }

            return mCredential;
        }
    }

    #endregion


    #region "Methods"

    protected override void OnLoad(EventArgs e)
    {
        HeaderActions.ActionPerformed += HeaderActions_ActionPerformed;

        ShowInfo();

        base.OnLoad(e);
    }


    /// <summary>
    /// Shows information and initializes remove action.
    /// </summary>
    private void ShowInfo()
    {
        bool validCredentials = false;
        try
        {
            validCredentials = CheckCredential(Credential);
        }
        catch (Exception ex)
        {
            DisableSaveAction();
            LogAndShowError(String.Format(GetString("datacom.serviceexception")), "DATACOM", ex);
            return;
        }

        if (validCredentials)
        {
            ShowInformation(string.Format(GetString("datacom.automation.stepisusingcredentials"), Credential.UserName));
        }
        else
        {
            if (Credential == null)
            {
                URLHelper.Redirect(LoginPageUrl);
                return;
            }

            ShowWarning(GetString("datacom.automation.stepisusinginvalidcredentials"));
            DisableSaveAction();
        }

        InitializeRemoveAction();
    }


    /// <summary>
    /// Adds logout button to header actions.
    /// </summary>
    private void InitializeRemoveAction()
    {
        AddHeaderAction(new HeaderAction
        {
            Text = string.Format(GetString("datacom.automation.removecredentials"), Credential.UserName),
            GenerateSeparatorBeforeAction = true,
            CommandName = "remove",
            OnClientClick = "return confirm('" + GetString("datacom.automation.confirmremoval") + "')"
        });
    }


    protected void HeaderActions_ActionPerformed(object sender, CommandEventArgs e)
    {
        switch (e.CommandName.ToLowerInvariant())
        {
            case "remove":
                var provider = new StepCredentialProvider(WorkflowStep);
                provider.SetCredential(new NetworkCredential());
                URLHelper.Redirect(LoginPageUrl);
                break;
            case "save":
                // Raises the save event of form. This must be used instead of Form.SaveData(), because this control is used in 
                // Marketing Automaton step, where multiple forms are present and Form.SaveData() simply doesn't work.
                ComponentEvents.RequestEvents.RaiseComponentEvent(this, e, ComponentName, e.CommandName);
                URLHelper.Redirect(URLHelper.AddParameterToUrl(RequestContext.CurrentURL, "saved", "1"));
                break;
        }
    }


    /// <summary>
    /// Checks if given user's DataCom credentials are valid. Makes API call to Data.com service.
    /// </summary>
    /// <param name="user">User to check credentials for</param>
    /// <rereturns>Returns true if valid</rereturns>
    private bool CheckCredential(NetworkCredential credential)
    {
        if (credential == null || String.IsNullOrEmpty(credential.UserName))
        {
            return false;
        }

        DataComClient client = DataComHelper.CreateClient();
        return client.UserIsValid(credential);
    }


    /// <summary>
    /// Disables save action.
    /// </summary>
    private void DisableSaveAction()
    {
        if (HeaderActions.ActionsList.Count > 0)
        {
            var saveAction = HeaderActions.ActionsList.FirstOrDefault(t => t.CommandName.ToLowerCSafe() == "save");
            if (saveAction != null)
            {
                saveAction.Enabled = false;
            }
        }
    }

    #endregion
}