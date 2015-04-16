using System;

using CMS.ExtendedControls;
using CMS.Helpers;
using CMS.UIControls;

public partial class CMSAdminControls_Debug_WebFarmLog : WebFarmLog
{
    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        Visible = false;

        var dt = GetLogData();
        if (dt != null)
        {
            Visible = true;

            gridQueries.SetHeaders("", "WebFarmLog.TaskType", "WebFarmLog.Target", "WebFarmLog.TextData", "General.Context");

            HeaderText = GetString("WebFarmLog.Info");

            gridQueries.DataSource = dt;
            gridQueries.DataBind();
        }
    }
}