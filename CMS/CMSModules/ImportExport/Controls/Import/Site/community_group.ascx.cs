using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.CMSImportExport;
using CMS.Helpers;
using CMS.UIControls;

public partial class CMSModules_ImportExport_Controls_Import_Site_community_group : ImportExportControl
{
    /// <summary>
    /// True if import into existing site.
    /// </summary>
    protected bool ExistingSite
    {
        get
        {
            if (Settings != null)
            {
                return ((SiteImportSettings)Settings).ExistingSite;
            }
            return true;
        }
    }


    /// <summary>
    /// True if the data should be imported.
    /// </summary>
    protected bool Import
    {
        get
        {
            return chkObject.Checked;
        }
    }


    /// <summary>
    /// Initialize child controls
    /// </summary>
    /// <param name="sender">Ignored</param>
    /// <param name="e">Ignored</param>
    protected void Page_Init(object sender, EventArgs e)
    {
        mlSettings.Settings = Settings;

        chkObject.Visible = true;
        chkObject.Text = GetString("CMSImport_CommunityGroups.ImportGroupMembership");
    }


    /// <summary>
    /// Saves current settings.
    /// </summary>
    public override void SaveSettings()
    {
        Settings.SetSettings(ImportExportHelper.SETTINGS_GROUP_MEMBERSHIP, Import);

        mlSettings.SaveSettings();
    }


    /// <summary>
    /// Reloads data.
    /// </summary>
    public override void ReloadData()
    {
        chkObject.Checked = ValidationHelper.GetBoolean(Settings.GetSettings(ImportExportHelper.SETTINGS_GROUP_MEMBERSHIP), !ExistingSite);

        mlSettings.ReloadData();
    }
}