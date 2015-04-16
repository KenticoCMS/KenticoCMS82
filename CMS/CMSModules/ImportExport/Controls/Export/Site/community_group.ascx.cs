using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.CMSImportExport;
using CMS.Helpers;
using CMS.UIControls;

public partial class CMSModules_ImportExport_Controls_Export_Site_community_group : ImportExportControl
{
    protected void Page_Init(object sender, EventArgs e)
    {
        mlSettings.Settings = Settings;
    }


    /// <summary>
    /// Saves current settings.
    /// </summary>
    public override void SaveSettings()
    {
        mlSettings.SaveSettings();
    }


    /// <summary>
    /// Reloads data.
    /// </summary>
    public override void ReloadData()
    {
        mlSettings.ReloadData();
    }
}