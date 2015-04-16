using System;

using CMS.DataEngine;
using CMS.ExtendedControls;
using CMS.Helpers;
using CMS.Base;
using CMS.UIControls;


public partial class CMSInstall_Controls_WizardSteps_SeparationFinished : CMSUserControl
{
    #region "Variables"

    private const string COLLATION_CASE_INSENSITIVE = "SQL_Latin1_General_CP1_CI_AS";
    private ISqlServerCapabilities mSqlServerCapabilities;

    #endregion


    #region "Public properties"

    /// <summary>
    /// Error label.
    /// </summary>
    public LocalizedLabel ErrorLabel
    {
        get
        {
            return lblError;
        }
    }


    /// <summary>
    /// Error label for azure.
    /// </summary>
    public LocalizedLabel AzureErrorLabel
    {
        get
        {
            return lblAzureError;
        }
    }


    /// <summary>
    /// Info label.
    /// </summary>
    public LocalizedLabel InfoLabel
    {
        get
        {
            return lblCompleted;
        }
    }

    /// <summary>
    /// Connection string.
    /// </summary>
    public string ConnectionString
    {
        get;
        set;
    }


    /// <summary>
    /// Database.
    /// </summary>
    public string Database
    {
        get;
        set;
    }


    /// <summary>
    /// Indicates if old database should be deleted completely.
    /// </summary>
    public bool DeleteOldDB
    {
        get
        {
            return chkDeleteOldDB.Checked;
        }
    }


    /// <summary>
    /// Indicates if current process is separation.
    /// </summary>
    public bool IsSeparation
    {
        get;
        set;
    }


    /// <summary>
    /// Returns SQL server capabilities.
    /// </summary>
    private ISqlServerCapabilities SqlServerCapabilities
    {
        get
        {
            return mSqlServerCapabilities ?? (mSqlServerCapabilities = SqlServerCapabilitiesFactory.GetSqlServerCapabilities(ConnectionHelper.GetSqlConnectionString()));
        }
    }

    #endregion


    #region "Page events"

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        plcDeleteOldDB.Visible = !IsSeparation && SqlServerCapabilities.SupportsDatabaseDeletion;
        spanScreenReader.Text = iconHelp.ToolTip = GetString("separationDB.deleteolddbhelp");
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);
        if (!String.IsNullOrEmpty(lblAzureError.Text))
        {
            plcAzureError.Visible = true;
            plcContent.Visible = false;
        }
    }

    #endregion


    #region "Methods and event"

    /// <summary>
    /// Change collation clicked.
    /// </summary>
    protected void btnChangeCollation_Click(object sender, EventArgs e)
    {
        DatabaseHelper.ChangeDatabaseCollation(ConnectionString, Database, COLLATION_CASE_INSENSITIVE);
        lblCompleted.ResourceString = "separationDB.OK";
        btnChangeCollation.Visible = false;
    }


    /// <summary>
    /// Display collation dialog.
    /// </summary>
    public void DisplayCollationDialog()
    {
        string collation = DatabaseHelper.GetDatabaseCollation(ConnectionString);
        if (CMSString.Compare(collation, COLLATION_CASE_INSENSITIVE, true) != 0)
        {
            lblChangeCollation.ResourceString = string.Format(ResHelper.GetFileString("separationDB.collation"), collation);
            btnChangeCollation.ResourceString = string.Format(ResHelper.GetFileString("install.changecollation"), COLLATION_CASE_INSENSITIVE);
            plcChangeCollation.Visible = true;
        }
    }

    #endregion
}
