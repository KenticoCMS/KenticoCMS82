using System;
using System.Data;

using CMS.ExtendedControls;
using CMS.Helpers;
using CMS.IO;
using CMS.UIControls;

public partial class CMSAdminControls_Debug_FilesLog : FilesLog
{
    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        Visible = false;

        var dt = GetLogData();
        if (dt != null)
        {
            DataView dv;
            var providerSet = !String.IsNullOrEmpty(ProviderName);

            lock (dt)
            {
                dv = new DataView(dt);

                if (providerSet)
                {
                    dv.RowFilter = "ProviderName = '" + ProviderName + "'";
                }
            }

            if (!DataHelper.DataSourceIsEmpty(dv))
            {
                Visible = true;

                gridStates.SetHeaders("", "FilesLog.Operation", "FilesLog.FilePath", "FilesLog.OperationType", "General.Context");

                // Hide the operation type column if only specific operation type is selected
                if (providerSet)
                {
                    gridStates.Columns[3].Visible = false;
                }

                HeaderText = GetString("FilesLog.Info");

                gridStates.DataSource = dv;
                gridStates.DataBind();
            }
        }
    }
}