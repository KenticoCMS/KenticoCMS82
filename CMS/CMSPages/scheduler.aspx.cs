using System;
using System.Threading;

using CMS.Helpers;
using CMS.Base;
using CMS.Scheduler;
using CMS.SiteProvider;
using CMS.UIControls;

public partial class CMSPages_scheduler : CMSPage
{
    protected override void OnPreInit(EventArgs e)
    {
        base.OnPreInit(e);

        if (!DebugHelper.DebugScheduler)
        {
            DisableDebugging();
        }
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        Response.Cache.SetNoStore();

        // Run the tasks
        SchedulingExecutorParameters schedulingParams = new SchedulingExecutorParameters() { SiteName = SiteContext.CurrentSiteName, ServerName = WebFarmHelper.ServerName };
        ThreadStart threadStartObj = new ThreadStart(schedulingParams.ExecuteScheduledTasks);
        // Create synchronous thread
        CMSThread schedulerThread = new CMSThread(threadStartObj, true, ThreadModeEnum.Sync);
        schedulerThread.Start();
    }
}