using System;
using System.Data;
using System.Web.UI;

using CMS.DocumentEngine;
using CMS.Helpers;
using CMS.Base;
using CMS.Membership;
using CMS.PortalEngine;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.Controls;

using TreeNode = CMS.DocumentEngine.TreeNode;

[UIElement("CMS", "Administration")]
public partial class Admin_CMSAdministration : CMSDeskPage
{
    #region "Variables"

    protected string infoMessageUrl = String.Empty;

    #endregion


    #region "Page events"

    /// <summary>
    /// OnPreInit event handler.
    /// </summary>
    /// <param name="e">Event argument</param>
    protected override void OnPreInit(EventArgs e)
    {
        // Do not check the site availability
        RequireSite = false;

        // Do not check document Read permission
        CheckDocPermissions = false;

        base.OnPreInit(e);
    }


    /// <summary>
    /// OnInit event handler.
    /// </summary>
    /// <param name="e">Event argument</param>
    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        // Ensure specific appId parameter in special cases (safari bug)
        if (QueryHelper.Contains("appId"))
        {
            // Remove appId parameter
            string url = URLHelper.RemoveParameterFromUrl(RequestContext.CurrentURL, "appId");
            // Get required application id
            
            Guid requiredApplicationId = QueryHelper.GetGuid("appId", Guid.Empty);
            if (requiredApplicationId != Guid.Empty)
            {
                url = url + "#" + requiredApplicationId;
            }

            URLHelper.Redirect(url);
        }

        paneFooter.Visible = SystemContext.DevelopmentMode;
        appListUniview.OnItemDataBound += appListUniview_OnItemDataBound;
        SetupAppList();
    }


    /// <summary>
    /// OnPreRender event handler.
    /// </summary>
    /// <param name="e">Event argument</param>
    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        // Register jQuery and Require.js
        ScriptHelper.RegisterJQuery(Page);

        RequestContext.ClientApplication.Add("isAppList", true);

        // Register CMSAppList module
        ScriptHelper.RegisterModule(Page, "CMS/AppList", new
        {
            applicationListBaseUrl = UIContextHelper.GetElementUrl(),
            defaultAppUrl = URLHelper.ResolveUrl("~/CMSModules/ApplicationDashboard/ApplicationDashboard.aspx"),
            defaultAppName = GetString("cms.dashboard"),
            indentLiveSite = plcLiveSite.Visible,
            launchAppWithQuery = String.IsNullOrEmpty(URLHelper.GetQuery(RequestContext.CurrentURL)) ? "" : URLHelper.UrlEncodeQueryString(URLHelper.GetQuery(RequestContext.CurrentURL)).Substring(1),
            screenLockInterval = SecurityHelper.GetSecondsToShowScreenLockAction(SiteContext.CurrentSiteName)
        });
        
        ScriptHelper.RegisterModule(Page, "CMS/GlobalEventsHandler");

        // Register CSS for jQuery scroller
        CSSHelper.RegisterCSSLink(Page, "~/CMSScripts/jquery/jquery-jscrollpane.css");

        // Register bootstrap tooltip for application list
        ScriptHelper.RegisterBootstrapTooltip(Page, ".js-filter-item a", "<div class=\"tooltip applist-tooltip\"><div class=\"tooltip-arrow\"></div><div class=\"tooltip-inner\"></div></div>");
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// OnItemDataBound event handler that ensures rendering of application list.
    /// </summary>
    /// <param name="sender">Sender object</param>
    /// <param name="item">Bound item</param>
    protected void appListUniview_OnItemDataBound(object sender, UniViewItem item)
    {
        DataRowView drv = item.DataItem as DataRowView;

        if (drv != null)
        {
            int level = ValidationHelper.GetInteger(drv["ElementLevel"], -1);
            bool isCategory = (level == 2);

            // Set placeholder based by category or application 
            String placeholderId = isCategory ? "plcCategoryTemplate" : "plcItemTemplate";

            // If current item is category then make visible appropriate placeholder to render category template. Otherwise make visible placeholder that renders item template           
            Control c = item.FindControl(placeholderId);
            if (c != null)
            {
                c.Visible = true;
            }
        }
    }


    /// <summary>
    /// Setups live site link.
    /// </summary>
    private void SetupLiveSiteLink()
    {
        // Initialize variables from query string 
        int nodeId = QueryHelper.GetInteger("nodeid", 0);
        string culture = QueryHelper.GetText("culture", null);
        string liveSiteUrl = "~";

        // Set URL to node from which CMSDesk was opened
        if ((nodeId > 0) && !String.IsNullOrEmpty(culture))
        {
            TreeProvider treeProvider = new TreeProvider(MembershipContext.AuthenticatedUser);
            TreeNode node = treeProvider.SelectSingleNode(nodeId, culture, false, false);
            if (node != null)
            {
                liveSiteUrl = DocumentURLProvider.GetUrl(node.NodeAliasPath, node.DocumentUrlPath);
            }
        }

        // Resolve URL and add live site view mode
        liveSiteUrl = ResolveUrl(liveSiteUrl);
        liveSiteUrl = URLHelper.AddParameterToUrl(liveSiteUrl, "viewmode", "livesite");
        liveSiteUrl = EnsureViewModeParam(liveSiteUrl, "viewmode");

        lnkLiveSite.NavigateUrl = liveSiteUrl;
        lnkLiveSite.Text = GetString("general.livesite");
        lnkLiveSite.ToolTip = GetString("applicationlist.livesite");
        plcLiveSite.Visible = true;
    }


    /// <summary>
    /// Ensures that the given url will be extended by returnviewmode parameter.
    /// </summary>
    /// <param name="url">The URL</param>
    /// <param name="paramName">The parameter name which the viewmode parameter will be assigned to</param>
    private string EnsureViewModeParam(string url, string paramName)
    {
        if (QueryHelper.Contains("returnviewmode"))
        {
            // Set the viewmode according to the "returnviewmode" parameter if set
            url = URLHelper.AddParameterToUrl(url, paramName, QueryHelper.GetString("returnviewmode", "livesite"));
        }

        return url;
    }


    /// <summary>
    /// Setup application list.
    /// </summary>
    private void SetupAppList()
    {
        DataSet ds = ApplicationHelper.LoadApplications();
        DataSet filteredDataSet = ApplicationHelper.FilterApplications(ds, CurrentUser, true);

        if ((filteredDataSet != null) && !DataHelper.DataSourceIsEmpty(filteredDataSet))
        {
            // Create grouped data source
            GroupedDataSource gds = new GroupedDataSource(filteredDataSet, "ElementParentID", "ElementLevel");
            appListUniview.DataSource = gds;
            appListUniview.ReloadData(true);
        }

        SiteInfo si = SiteContext.CurrentSite;
        if ((si != null) && (!si.SiteIsOffline))
        {
            SetupLiveSiteLink();
        }
    }

    #endregion
}