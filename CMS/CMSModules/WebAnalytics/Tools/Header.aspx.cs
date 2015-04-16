using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.UIControls;

public partial class CMSModules_WebAnalytics_Tools_Header : CMSWebAnalyticsPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        Title = "Web analytics header";

        PageTitle.TitleText = GetString("tools.ui.webanalytics");
        // Register script for unimenu button selection
        AddMenuButtonSelectScript(this, "WebAnalytics", null, "menu");
    }
}