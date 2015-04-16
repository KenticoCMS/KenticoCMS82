using System;

using CMS.Helpers;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.WebAnalytics;
using CMS.ExtendedControls;

public partial class CMSModules_WebAnalytics_Tools_Disabled : CMSWebAnalyticsPage
{
    #region "Properties"

    /// <summary>
    /// Messages placeholder
    /// </summary>
    public override MessagesPlaceHolder MessagesPlaceHolder
    {
        get
        {
            return plcMess;
        }
    }

    #endregion


    protected void Page_Load(object sender, EventArgs e)
    {
        if (AnalyticsHelper.AnalyticsEnabled(SiteContext.CurrentSiteName))
        {
            URLHelper.Redirect("default.aspx");
        }
        else
        {
            ShowWarning(GetString("WebAnalytics.Disabled"));
        }
    }
}