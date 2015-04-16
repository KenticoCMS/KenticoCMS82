using System;
using System.Net;
using System.Web;

using CMS.Base;
using CMS.DataCom;
using CMS.Helpers;
using CMS.Membership;
using CMS.OnlineMarketing;
using CMS.WorkflowEngine;

public partial class CMSModules_ContactManagement_Pages_Tools_DataCom_Login : CMSDataComPage
{
    #region "Constants"

    /// <summary>
    /// Sign up url.
    /// </summary>
    const string SIGNUP_URL = "https://connect.data.com/registration/signup";

    #endregion


    #region "Properties"

    /// <summary>
    /// Gets return URL for logon page
    /// </summary>
    private string ReturnUrl
    {
        get
        {
            return QueryHelper.GetString("returnurl", string.Empty);
        }
    }


    /// <summary>
    /// Gets workflow step id (valid id if previous page was workflow step).
    /// </summary>
    private int WorkflowStepId
    {
        get
        {
            return QueryHelper.GetInteger("workflowstepid", 0);
        }
    }

    #endregion


    #region "Life cycle methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        InitializeControls();
    }


    protected void btnLogin_Click(object sender, EventArgs e)
    {
        var email = txtEmail.Text;
        var password = txtPassword.Text;

        if (!ValidationHelper.IsEmail(email))
        {
            ShowError(GetString("datacom.invalidemail"));
            return;
        }

        if (string.IsNullOrEmpty(password))
        {
            ShowError(GetString("general.requirespassword"));
            return;
        }

        var networkCredetial = new NetworkCredential { UserName = email, Password = password };

        // Check if user is valid on data.com
        try
        {
            var client = DataComHelper.CreateClient();
            if (!client.UserIsValid(networkCredetial))
            {
                ShowError(GetString("datacom.invalidcredential"));
                return;
            }
        }
        catch (Exception ex)
        {
            ErrorSummary.Report(ex);
            return;
        }

        var redirectUrl = GetRedirectUrl();
        if (redirectUrl != null)
        {
            var provider = GetCredentialProvider();

            if (provider != null)
            {
                provider.SetCredential(networkCredetial);
                URLHelper.Redirect(redirectUrl);
            }
            else
            {
                // Someone has changed url or step has been deleted, url is no longer valid
                ShowError(GetString("datacom.invalidreferring"));
            }
        }
        else
        {
            ShowError(GetString("datacom.invalidreferring"));
        }
    }

    #endregion


    #region "Private methods"

    /// <summary>
    /// Initializes controls.
    /// </summary>
    private void InitializeControls()
    {
        if (WorkflowStepId > 0)
        {
            btnLogin.Text = GetString("datacom.login.set");
        }
        else
        {
            btnLogin.Text = GetString("general.login");
        }
        linkSignUpHere.NavigateUrl = SIGNUP_URL;
    }


    /// <summary>
    /// Returns url to redirect after successful submit.
    /// </summary>
    private string GetRedirectUrl()
    {
        var url = HttpUtility.HtmlDecode(ReturnUrl);
        if (string.IsNullOrEmpty(url) || url.StartsWithCSafe("~") || url.StartsWithCSafe("/") || QueryHelper.ValidateHash("hash"))
        {
            return url;
        }
        return null;
    }


    /// <summary>
    /// Returns credential provider based on workflowstep ID parameter.
    /// </summary>
    private ICredentialProvider GetCredentialProvider()
    {
        if (WorkflowStepId > 0)
        {
            var step = WorkflowStepInfoProvider.GetWorkflowStepInfo(WorkflowStepId);
            if (step != null)
            {
                return new StepCredentialProvider(step);
            }

            return null;
        }
        return new UserCredentialProvider(MembershipContext.AuthenticatedUser);
    }

    #endregion
}
