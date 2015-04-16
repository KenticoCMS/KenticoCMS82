using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.CMSImportExport;
using CMS.Helpers;
using CMS.UIControls;

public partial class CMSModules_ImportExport_Controls_Export_Site_board_board : ImportExportControl
{
    protected void Page_Load(object sender, EventArgs e)
    {
        chkObject.Text = GetString("CMSExport_Board.ExportBoardMessage");
    }


    /// <summary>
    /// Gets settings.
    /// </summary>
    public override void SaveSettings()
    {
        Settings.SetSettings(ImportExportHelper.SETTINGS_BOARD_MESSAGES, chkObject.Checked);
    }


    /// <summary>
    /// Reload data.
    /// </summary>
    public override void ReloadData()
    {
        chkObject.Checked = ValidationHelper.GetBoolean(Settings.GetSettings(ImportExportHelper.SETTINGS_BOARD_MESSAGES), false);
    }
}