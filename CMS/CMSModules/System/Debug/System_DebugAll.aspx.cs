using System;
using System.Web.UI;

using CMS.Base;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.IO;
using CMS.MacroEngine;
using CMS.UIControls;

public partial class CMSModules_System_Debug_System_DebugAll : CMSDebugPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        btnClear.Text = GetString("DebugAll.ClearLog");

        ReloadData();
    }


    protected void ReloadData()
    {
        if (!RequestDebug.Settings.Enabled)
        {
            ShowWarning(GetString("DebugRequests.NotConfigured"));
        }
        else
        {
            plcLogs.Controls.Clear();

            for (int i = RequestDebug.Settings.LastLogs.Count - 1; i >= 0; i--)
            {
                try
                {
                    // Get the request log
                    var log = RequestDebug.Settings.LastLogs[i];
                    if (log != null)
                    {
                        // Load the control only if there is more than only request log
                        var logs = log.ParentLogs;
                        if (logs != null)
                        {
                            AllLog logCtrl = (AllLog)LoadLogControl(log, "~/CMSAdminControls/Debug/AllLog.ascx", i);

                            logCtrl.Logs = logs;
                            logCtrl.ShowCompleteContext = chkCompleteContext.Checked;

                            // Add to the output
                            plcLogs.Append(logCtrl);
                        }
                    }
                }
                catch
                {
                }
            }
        }
    }


    protected void btnClear_Click(object sender, EventArgs e)
    {
        DebugHelper.ClearLogs();

        ReloadData();
    }
}