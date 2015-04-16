using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.Helpers;
using CMS.UIControls;
using CMS.WebFarmSync;

public partial class CMSPages_WebFarmUpdater : CMSPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // Process online update
        Guid taskGuid = QueryHelper.GetGuid("taskguid", Guid.Empty);
        if (taskGuid != Guid.Empty)
        {
            // Run specific task
            WebSyncHelper.ProcessTask(taskGuid);
        }
        else
        {
            // Process all tasks for the given server
            WebSyncHelper.ProcessMyTasks();
        }
    }
}