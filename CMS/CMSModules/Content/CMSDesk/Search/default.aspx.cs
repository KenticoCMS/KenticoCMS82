using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Collections;

using CMS.Base;
using CMS.Controls;
using CMS.Helpers;
using CMS.SiteProvider;
using CMS.Membership;
using CMS.DocumentEngine;
using CMS.UIControls;
using CMS.Globalization;

using TimeZoneInfo = CMS.Globalization.TimeZoneInfo;
using CMS.Search;

public partial class CMSModules_Content_CMSDesk_Search_default : CMSContentPage, ITimeZoneManager
{
    #region "Variables"

    private string searchindex = null;
    private bool timeZoneLoaded = false;
    private TimeZoneInfo usedTimeZone = null;

    #endregion


    #region "Methods"

    protected override void OnPreInit(EventArgs e)
    {
        base.OnPreInit(e);

        DocumentManager.RegisterSaveChangesScript = false;
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        pagerElem.PagerMode = UniPagerMode.Querystring;
        pagerElem.HidePagerForSinglePage = true;
        pagerElem.QueryStringKey = "pagesearchresults";

        // Setup page title text and image
        PageTitle.TitleText = GetString("Content.SearchTitle");
        if (!RequestHelper.IsPostBack())
        {
            searchindex = QueryHelper.GetString("searchindex", null);
            SetRepeaters();
        }
        else
        {
            repSearchSQL.Visible = false;
            repSearchSQL.StopProcessing = true;
            repSmartSearch.Visible = false;
            repSmartSearch.StopProcessing = true;
        }

        CreateBreadcrumbs();
    }


    /// <summary>
    /// Creates breadcrumbs
    /// </summary>
    private void CreateBreadcrumbs()
    {
        EnsureDocumentBreadcrumbs(CurrentMaster.Title.Breadcrumbs, null, GetString("contentmenu.search"));
    }


    /// <summary>
    /// Sets repeaters.
    /// </summary>
    private void SetRepeaters()
    {
        // Display SQL results
        if (searchindex.EqualsCSafe("##SQL##", false))
        {
            repSearchSQL.Visible = true;
            repSearchSQL.StopProcessing = false;
            repSmartSearch.Visible = false;
            repSmartSearch.StopProcessing = true;

            // Hide original pager and set the UI Pager
            pagerElem.ShowPageSize = false;
            repSearchSQL.PagerControl.Visible = false;
            repSearchSQL.PagerControl.OnDataSourceChanged += PagerControl_OnDataSourceChanged;

            repSearchSQL.SelectOnlyPublished = QueryHelper.GetBoolean("searchpublished", true);

            string culture = QueryHelper.GetString("searchculture", "##ANY##");
            string mode = QueryHelper.GetString("searchlanguage", null);
            if ((culture == "##ANY##") || (mode == "<>"))
            {
                culture = TreeProvider.ALL_CULTURES;
            }
            else
            {
                repSearchSQL.CombineWithDefaultCulture = false;
            }
            repSearchSQL.WhereCondition = searchDialog.WhereCondition;
            repSearchSQL.CultureCode = culture;
            repSearchSQL.TransformationName = "CMS.Root.CMSDeskSQLSearchResults";
        }
        // Display Smart search results
        else
        {
            repSearchSQL.Visible = false;
            repSearchSQL.StopProcessing = true;
            repSmartSearch.Visible = true;
            repSmartSearch.StopProcessing = false;
            repSmartSearch.Indexes = searchindex;
            repSmartSearch.TransformationName = "CMS.Root.CMSDeskSmartSearchResults";
            repSmartSearch.PageSize = 10;
            repSmartSearch.PagingMode = UniPagerMode.Querystring;
            repSmartSearch.HidePagerForSinglePage = true;
            repSmartSearch.UniPagerControl = pagerElem.UniPager;

            pagerElem.PagedControl = repSmartSearch;
            pagerElem.ShowPageSize = true;

            string culture = QueryHelper.GetString("searchculture", "##ANY##");
            if (culture == "##ANY##")
            {
                culture = "##all##";
            }
            repSmartSearch.CultureCode = culture;
        }
    }


    void PagerControl_OnDataSourceChanged(object sender, EventArgs e)
    {
        UniPagerConnector connectorElem = new UniPagerConnector();
        connectorElem.PagerForceNumberOfResults = repSearchSQL.PagerControl.TotalRecords;
        pagerElem.PagedControl = connectorElem;
    }
    

    /// <summary>
    /// Returns column value for current search result item.
    /// </summary>
    /// <param name="columnName">Column</param>
    public object GetSearchValue(string columnName)
    {
        // Get id of the current row
        string id = ValidationHelper.GetString(Eval("id"), String.Empty);

        // Get Datarows for current results
        var resultRows = SearchContext.CurrentSearchResults;

        // Check whether id and datarow collection exists
        if ((id != String.Empty) && (resultRows != null))
        {
            // Get current datarow
            DataRow dr = resultRows[id];

            // Check whether datarow exists and contains required column
            if ((dr != null) && (dr.Table.Columns.Contains(columnName)))
            {
                // Return column value
                return dr[columnName];
            }
        }

        // Return nothing by default
        return null;
    }

    #endregion


    #region "ITimeZoneManager Members"

    /// <summary>
    /// Gets time zone type for this page.
    /// </summary>
    public TimeZoneTypeEnum TimeZoneType
    {
        get
        {
            return TimeZoneTypeEnum.Custom;
        }
    }


    /// <summary>
    /// Gets current time zone for UI.
    /// </summary>
    public TimeZoneInfo CustomTimeZone
    {
        get
        {
            // Get time zone for first request only
            if (!timeZoneLoaded)
            {
                usedTimeZone = TimeZoneHelper.GetTimeZoneInfo(MembershipContext.AuthenticatedUser, SiteContext.CurrentSite);
                timeZoneLoaded = true;
            }
            return usedTimeZone;
        }
    }

    #endregion
}