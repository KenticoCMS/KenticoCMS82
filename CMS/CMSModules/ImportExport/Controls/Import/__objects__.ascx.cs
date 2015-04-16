using System;

using CMS.CMSImportExport;
using CMS.Helpers;
using CMS.Base;
using CMS.UIControls;

public partial class CMSModules_ImportExport_Controls_Import___objects__ : ImportExportControl
{
    #region "Properties"

    /// <summary>
    /// Import settings.
    /// </summary>
    public SiteImportSettings ImportSettings
    {
        get
        {
            if (Settings != null)
            {
                return (SiteImportSettings)Settings;
            }
            return null;
        }

        set
        {
            Settings = value;
        }
    }

    #endregion


    protected void Page_Load(object sender, EventArgs e)
    {
        if (!StopProcessing)
        {
            if (ImportSettings != null)
            {
                if (CheckVersion())
                {
                    // Show only if package is older than custom query export exception
                    plcOverwriteQueries.Visible = ImportSettings.IsQuerySpecialCaseRequired;

                    pnlWarning.Visible = true;
                    lblWarning.Text = GetString("ImportObjects.WarningVersion");
                }
                else if (CheckHotfixVersion())
                {
                    pnlWarning.Visible = true;
                    lblWarning.Text = GetString("ImportObjects.WarningHotfixVersion");
                }
            }

            lblInfo.Text = GetString("ImportObjects.Info");
            lblInfo2.Text = GetString("ImportObjects.Info2");

            lnkSelectAll.Text = GetString("ImportObjects.SelectAll");
            lnkSelectNone.Text = GetString("ImportObjects.SelectNone");
            lnkSelectNew.Text = GetString("ImportObjects.SelectNew");
            lnkSelectDefault.Text = GetString("ImportObjects.SelectDefault");

            // Confirmation for select all
            lnkSelectAll.OnClientClick = "return confirm(" + ScriptHelper.GetString(ResHelper.GetString("importobjects.selectallconfirm")) + ");";

            chkCopyFiles.Text = GetString("ImportObjects.CopyFiles");
            chkCopyGlobalFiles.Text = GetString("ImportObjects.CopyGlobalFiles");
            chkCopyAssemblies.Text = GetString("ImportObjects.CopyAssemblies");
            chkCopyCodeFiles.Text = GetString("ImportObjects.CopyCodeFiles");
            chkCopySiteFiles.Text = GetString("ImportObjects.CopySiteFiles");

            // Javascript
            string script = "var im_g_parent = document.getElementById('" + chkCopyFiles.ClientID + "'); \n" +
                            "var im_g_childIDs = ['" + chkCopyGlobalFiles.ClientID + "', '" + chkCopySiteFiles.ClientID + "', '" + chkCopyCodeFiles.ClientID + "','" + chkCopyAssemblies.ClientID + "']; \n" +
                            "var im_g_childIDNames = ['gl', 'site', 'code', 'asbl']; \n" +
                            "var im_g_isPrecompiled = " + (SystemContext.IsPrecompiledWebsite ? "true" : "false") + "; \n" +
                            "InitCheckboxes(); \n";

            ltlScript.Text = ScriptHelper.GetScript(script);

            chkCopyFiles.Attributes.Add("onclick", "CheckChange();");

            chkOverwriteSystemQueries.Text = GetString("ImportObjects.OverwriteQueries");
            chkSkipOrfans.Text = GetString("ImportObjects.SkipOrfans");
            chkImportTasks.Text = GetString("ImportObjects.ImportTasks");
            chkLogSync.Text = GetString("ImportObjects.LogSynchronization");
            chkLogInt.Text = GetString("ImportObjects.LogIntegration");
        }
    }


    /// <summary>
    /// Gets settings.
    /// </summary>
    public override void SaveSettings()
    {
        ImportSettings.SetSettings(ImportExportHelper.SETTINGS_DELETE_SITE, chkDeleteSite.Checked);
        ImportSettings.SetSettings(ImportExportHelper.SETTINGS_ADD_SITE_BINDINGS, chkBindings.Checked);
        ImportSettings.SetSettings(ImportExportHelper.SETTINGS_RUN_SITE, chkRunSite.Checked);
        ImportSettings.SetSettings(ImportExportHelper.SETTINGS_UPDATE_SITE_DEFINITION, chkUpdateSite.Checked);
        ImportSettings.SetSettings(ImportExportHelper.SETTINGS_OVERWRITE_SYSTEM_QUERIES, chkOverwriteSystemQueries.Checked);
        ImportSettings.SetSettings(ImportExportHelper.SETTINGS_SKIP_OBJECT_ON_TRANSLATION_ERROR, chkSkipOrfans.Checked);
        ImportSettings.SetSettings(ImportExportHelper.SETTINGS_TASKS, chkImportTasks.Checked);

        ImportSettings.CopyFiles = chkCopyFiles.Checked;
        ImportSettings.CopyCodeFiles = ImportSettings.CopyFiles && chkCopyCodeFiles.Checked;

        // Copy files property is stronger
        bool copyGlobal = chkCopyFiles.Checked && chkCopyGlobalFiles.Checked;
        bool copyAssemblies = chkCopyFiles.Checked && chkCopyAssemblies.Checked;
        bool copySite = chkCopyFiles.Checked && chkCopySiteFiles.Checked;

        ImportSettings.SetSettings(ImportExportHelper.SETTINGS_GLOBAL_FOLDERS, copyGlobal);
        ImportSettings.SetSettings(ImportExportHelper.SETTINGS_ASSEMBLIES, copyAssemblies);
        ImportSettings.SetSettings(ImportExportHelper.SETTINGS_SITE_FOLDERS, copySite);

        ImportSettings.LogSynchronization = chkLogSync.Checked;
        ImportSettings.LogIntegration = chkLogInt.Checked;
    }


    /// <summary>
    /// Reload data.
    /// </summary>
    public override void ReloadData()
    {
        if (ImportSettings != null)
        {
            bool singleObject = ValidationHelper.GetBoolean(ImportSettings.GetInfo(ImportExportHelper.INFO_SINGLE_OBJECT), false);

            chkCopyFiles.Checked = ImportSettings.CopyFiles;
            chkCopyCodeFiles.Checked = ImportSettings.CopyCodeFiles;
            chkCopyGlobalFiles.Checked = ValidationHelper.GetBoolean(ImportSettings.GetSettings(ImportExportHelper.SETTINGS_GLOBAL_FOLDERS), true);
            chkCopyAssemblies.Checked = ValidationHelper.GetBoolean(ImportSettings.GetSettings(ImportExportHelper.SETTINGS_ASSEMBLIES), false);
            chkCopySiteFiles.Checked = ValidationHelper.GetBoolean(ImportSettings.GetSettings(ImportExportHelper.SETTINGS_SITE_FOLDERS), true);

            if (SystemContext.IsPrecompiledWebsite)
            {
                // No code files or assemblies can be copied in precompiled website
                chkCopyAssemblies.Checked = chkCopyCodeFiles.Checked = false;
                chkCopyAssemblies.Enabled = chkCopyCodeFiles.Enabled = false;
                chkCopyAssemblies.ToolTip = chkCopyCodeFiles.ToolTip = GetString("importobjects.copyfiles.disabled");
            }

            chkBindings.Checked = ValidationHelper.GetBoolean(ImportSettings.GetSettings(ImportExportHelper.SETTINGS_ADD_SITE_BINDINGS), true);
            chkDeleteSite.Checked = ValidationHelper.GetBoolean(ImportSettings.GetSettings(ImportExportHelper.SETTINGS_DELETE_SITE), false);
            chkRunSite.Checked = ValidationHelper.GetBoolean(ImportSettings.GetSettings(ImportExportHelper.SETTINGS_RUN_SITE), !singleObject);
            chkUpdateSite.Checked = ValidationHelper.GetBoolean(ImportSettings.GetSettings(ImportExportHelper.SETTINGS_UPDATE_SITE_DEFINITION), !singleObject);
            chkOverwriteSystemQueries.Checked = ValidationHelper.GetBoolean(ImportSettings.GetSettings(ImportExportHelper.SETTINGS_OVERWRITE_SYSTEM_QUERIES), false);
            chkSkipOrfans.Checked = ValidationHelper.GetBoolean(ImportSettings.GetSettings(ImportExportHelper.SETTINGS_SKIP_OBJECT_ON_TRANSLATION_ERROR), false);
            chkImportTasks.Checked = ValidationHelper.GetBoolean(ImportSettings.GetSettings(ImportExportHelper.SETTINGS_TASKS), true);
            chkLogSync.Checked = ImportSettings.LogSynchronization;
            chkLogInt.Checked = ImportSettings.LogIntegration;

            Visible = true;

            if (ImportSettings.TemporaryFilesCreated)
            {
                if (ImportSettings.SiteIsIncluded && !singleObject)
                {
                    plcSite.Visible = true;

                    if (ImportSettings.ExistingSite)
                    {
                        plcExistingSite.Visible = true;
                        chkUpdateSite.Text = GetString("ImportObjects.UpdateSite");
                    }
                    plcSiteFiles.Visible = true;
                    chkBindings.Text = GetString("ImportObjects.Bindings");
                    chkRunSite.Text = GetString("ImportObjects.RunSite");
                    chkDeleteSite.Text = GetString("ImportObjects.DeleteSite");
                }
                else
                {
                    plcSite.Visible = false;
                }
            }
        }
        else
        {
            Visible = false;
        }
    }


    protected void lnkSelectAll_Click(object sender, EventArgs e)
    {
        ImportTypeEnum importType = ImportSettings.ImportType;

        ImportSettings.ImportType = ImportTypeEnum.All;
        ImportSettings.LoadDefaultSelection(false);

        SaveSettings();

        ImportSettings.ImportType = importType;

        lblInfo.Text = GetString("ImportObjects.AllSelected");
    }


    protected void lnkSelectNone_Click(object sender, EventArgs e)
    {
        ImportTypeEnum importType = ImportSettings.ImportType;

        ImportSettings.ImportType = ImportTypeEnum.None;
        ImportSettings.LoadDefaultSelection(false);

        SaveSettings();

        ImportSettings.ImportType = importType;

        lblInfo.Text = GetString("ImportObjects.NoneSelected");
    }


    protected void lnkSelectDefault_Click(object sender, EventArgs e)
    {
        ImportSettings.LoadDefaultSelection(false);

        SaveSettings();

        lblInfo.Text = GetString("ImportObjects.DefaultSelected");
    }


    protected void lnkSelectNew_Click(object sender, EventArgs e)
    {
        ImportTypeEnum importType = ImportSettings.ImportType;

        ImportSettings.ImportType = ImportTypeEnum.New;
        ImportSettings.LoadDefaultSelection(false);

        SaveSettings();

        ImportSettings.ImportType = importType;

        lblInfo.Text = GetString("ImportObjects.NewSelected");
    }
}