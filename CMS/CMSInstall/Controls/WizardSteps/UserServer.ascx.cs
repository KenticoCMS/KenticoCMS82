using System;
using System.Security.Principal;

using CMS.DataEngine;
using CMS.Helpers;
using CMS.UIControls;
using CMS.Base;

public partial class CMSInstall_Controls_WizardSteps_UserServer : CMSUserControl
{
    #region "Properties"

    /// <summary>
    /// Server name.
    /// </summary>
    public string ServerName
    {
        get
        {
            return txtServerName.Text.Trim();
        }
        set
        {
            txtServerName.Text = value;
        }
    }


    /// <summary>
    /// Database password.
    /// </summary>
    public string DBPassword
    {
        get
        {
            return txtDBPassword.Text.Trim();
        }
    }


    /// <summary>
    /// User name.
    /// </summary>
    public string DBUsername
    {
        get
        {
            return txtDBUsername.Text.Trim();
        }
    }


    /// <summary>
    /// Windows authentication checked
    /// </summary>
    public bool WindowsAuthenticationChecked
    {
        get
        {
            return radWindowsAuthentication.Checked;
        }
    }


    /// <summary>
    /// Indicates if is DB separation.
    /// </summary>
    public bool IsDBSeparation
    {
        get;
        set;
    }

    #endregion


    protected void Page_Load(object sender, EventArgs e)
    {
        lblDBPassword.Text = ResHelper.GetFileString("Install.lblPassword");
        lblDBUsername.Text = ResHelper.GetFileString("Install.lblUsername");
        lblServerName.Text = ResHelper.GetFileString("Install.lblServername");
        lblSQLServer.Text = IsDBSeparation ? GetString("separationDB.server") : ResHelper.GetFileString("Install.lblSQLServer");

        txtDBPassword.Enabled = radSQLAuthentication.Checked;
        txtDBUsername.Enabled = radSQLAuthentication.Checked;

        radSQLAuthentication.Text = ResHelper.GetFileString("Install.radSQlAuthentication");

        if (ValidationHelper.GetBoolean(SettingsHelper.AppSettings["CMSWWAGInstallation"], false))
        {
            radWindowsAuthentication.Text = ResHelper.GetFileString("Install.WAGradWindowsAuthentication");
            radWindowsAuthentication.Enabled = false;
        }
        else
        {
            radWindowsAuthentication.Text = ResHelper.GetFileString("Install.radWindowsAuthentication") + "<br /><span>" + String.Format(ResHelper.GetFileString("Install.Account"), WindowsIdentity.GetCurrent().Name) + "</span>";
        }
    }


    protected void Page_PreRender(object sender, EventArgs e)
    {
        txtDBPassword.Enabled = radSQLAuthentication.Checked;
        txtDBUsername.Enabled = radSQLAuthentication.Checked;

        if (IsDBSeparation)
        {
            plcRadSQL.Visible = false;
            plcWinAuth.Visible = false;

            if (SqlInstallationHelper.DatabaseIsSeparated())
            {
                DisplaySeparationError(GetString("separationDB.separationerror"));
            }
        }
    }


    /// <summary>
    /// Validates control, returns error if failed.
    /// </summary>
    public bool ValidateForSeparation()
    {
        // Check if separation is not already completed
        if (SqlInstallationHelper.DatabaseIsSeparated())
        {
            DisplaySeparationError(GetString("separationDB.separationerror"));
            return false;
        }

        // Check the server name
        if (String.IsNullOrEmpty(txtServerName.Text))
        {
            DisplaySeparationError(ResHelper.GetFileString("Install.ErrorServerEmpty"));
            return false;
        }

        // Check if it is possible to connect to the database
        string res = TestNewConnection();
        if (!string.IsNullOrEmpty(res))
        {
            DisplaySeparationError(res);
            return false;
        }

        return true;
    }


    /// <summary>
    /// Displays separation error.
    /// </summary>
    private void DisplaySeparationError(string error)
    {
        plcSeparationError.Visible = true;
        lblError.Text = HTMLHelper.HTMLEncode(error);
    }


    /// <summary>
    /// Test new connection.
    /// </summary>
    private string TestNewConnection()
    {
        SQLServerAuthenticationModeEnum authenticationType = radSQLAuthentication.Checked
                                                                 ? SQLServerAuthenticationModeEnum.
                                                                       SQLServerAuthentication
                                                                 : SQLServerAuthenticationModeEnum.
                                                                       WindowsAuthentication;

        string res = ConnectionHelper.TestConnection(authenticationType, txtServerName.Text.Trim(), "",
                                                     txtDBUsername.Text.Trim(), txtDBPassword.Text.Trim());
        return res;
    }
}