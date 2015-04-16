using System;
using System.Collections;
using System.Data;

using CMS.DataCom;
using CMS.EventLog;
using CMS.Helpers;
using CMS.OnlineMarketing;

/// <summary>
/// A dialog page where user can search Data.com for a company, view company details and select it.
/// </summary>
public partial class CMSModules_ContactManagement_Pages_Tools_DataCom_SelectCompany : CMSDataComDialogPage
{
    #region "Constants"

    /// <summary>
    /// Maximum number of pages that the grid control can display.
    /// </summary>
    private readonly int MAX_PAGE_COUNT = 5;

    #endregion


    #region "Properties"

    /// <summary>
    /// Gets or sets the selected Data.com company.
    /// </summary>
    private Company CurrentCompany
    {
        get
        {
            return ViewState["CurrentCompany"] as Company;
        }
        set
        {
            ViewState["CurrentCompany"] = value;
        }
    }


    /// <summary>
    /// Gets or sets the current company filter.
    /// </summary>
    private CompanyFilter CurrentCompanyFilter
    {
        get
        {
            return ViewState["CurrentFilter"] as CompanyFilter;
        }
        set
        {
            ViewState["CurrentFilter"] = value;
        }
    }


    /// <summary>
    /// Gets or sets the filter parameter.
    /// </summary>
    private CompanyFilter FilterParameter { get; set; }


    /// <summary>
    /// Gets or sets the site identifier parameter.
    /// </summary>
    private int SiteIdentifierParameter { get; set; }

    #endregion


    #region "Life cycle methods"

    protected override void OnInit(EventArgs e)
    {
        SetSaveJavascript("DataCom_ConfirmSelection(); return false;");
        base.OnInit(e);
        ScriptHelper.RegisterWOpenerScript(Page);
        ScriptHelper.RegisterJQuery(Page);
        CompanyGrid.OnDataReload += CompanyGrid_OnDataReload;
        CompanyGrid.OnAction += CompanyGrid_OnAction;
        CompanyFilterControl.Search += CompanyFilterControl_Search;
        PageTitle.TitleText = GetString("datacom.companies.title");
        AuthorizeRequest();
    }


    protected override void OnLoad(EventArgs e)
    {
        try
        {
            RestoreParameters();
            if (!RequestHelper.IsPostBack())
            {
                CurrentCompanyFilter = FilterParameter;
                CompanyFilterControl.Filter = FilterParameter;
            }
        }
        catch (Exception exception)
        {
            HandleException(exception);
        }
        base.OnLoad(e);
    }


    protected void CompanyFilterControl_Search(object sender, EventArgs e)
    {
        CurrentCompanyFilter = CompanyFilterControl.Filter;
        CompanyGrid.Pager.Reset();
    }


    protected void CompanyGrid_OnAction(string actionName, object actionArgument)
    {
        if (actionName == "select")
        {
            long companyId = ValidationHelper.GetLong(actionArgument, 0);
            if (companyId > 0)
            {
                try
                {
                    IDataComConfiguration configuration = DataComHelper.GetConfiguration(SiteIdentifierParameter);
                    DataComClient client = DataComHelper.CreateClient();
                    ICompanyProvider provider = DataComHelper.CreateCompanyProvider(client, configuration);
                    Company company = provider.GetCompany(companyId);
                    if (company != null)
                    {
                        CurrentCompany = company;
                    }
                }
                catch (Exception exception)
                {
                    HandleException(exception);
                }
            }
        }
    }


    protected DataSet CompanyGrid_OnDataReload(string completeWhere, string currentOrder, int currentTopN, string columns, int currentOffset, int currentPageSize, ref int totalRecords)
    {
        try
        {
            IDataComConfiguration configuration = DataComHelper.GetConfiguration(SiteIdentifierParameter);
            DataComClient client = DataComHelper.CreateClient();
            ICompanyProvider provider = DataComHelper.CreateCompanyProvider(client, configuration);
            CompanySearchResults response = provider.SearchCompanies(CurrentCompanyFilter, currentOffset / currentPageSize, currentPageSize);
            DataTable table = new DataTable("Companies");
            table.Columns.Add("CompanyId", typeof(long));
            table.Columns.Add("Name", typeof(string));
            table.Columns.Add("Country", typeof(string));
            table.Columns.Add("City", typeof(string));
            foreach (CompanySearchResult result in response.Results)
            {
                DataRow row = table.NewRow();
                row["CompanyId"] = result.CompanyId;
                row["Name"] = result.Name;
                row["Country"] = result.Country;
                row["City"] = result.City;
                table.Rows.Add(row);
            }
            DataSet dataSet = new DataSet();
            dataSet.Tables.Add(table);
            int maxHitCount = currentPageSize * MAX_PAGE_COUNT;
            int hitCount = (int)response.TotalHits;
            if (hitCount > maxHitCount)
            {
                hitCount = maxHitCount;
                ShowWarning(GetString("datacom.toomanycompanies"), null, null);
            }
            totalRecords = hitCount;
            return dataSet;
        }
        catch (Exception exception)
        {
            HandleException(exception);
        }
        totalRecords = 0;

        return null;
    }


    protected override void OnPreRender(EventArgs e)
    {
        try
        {
            CompanyGrid.ReloadData();
            CompanyControl.Company = CurrentCompany;
            if (CurrentCompany != null)
            {
                JsonSerializer serializer = new JsonSerializer();
                CompanyHiddenField.Value = serializer.Serialize(CurrentCompany);
                headTitle.Visible = true;
            }
            else
            {
                EmptySelectionControl.Visible = true;
            }
        }
        catch (Exception exception)
        {
            HandleException(exception);
        }
        base.OnPreRender(e);
    }

    #endregion


    #region "Private methods"


    /// <summary>
    /// Restores parameters that were passed on to this dialog.
    /// </summary>
    private void RestoreParameters()
    {
        // Validate parameters
        if (!QueryHelper.ValidateHash("hash"))
        {
            throw new Exception("[DataComSelectCompanyPage.RestoreParameters]: Invalid query hash.");
        }
        Hashtable parameters = WindowHelper.GetItem(QueryHelper.GetString("pid", null)) as Hashtable;
        if (parameters == null)
        {
            throw new Exception("[DataComSelectCompanyPage.RestoreParameters]: The dialog page parameters are missing, the session might have been lost.");
        }

        // Restore filter
        string content = parameters["Filter"] as string;
        if (String.IsNullOrEmpty(content))
        {
            FilterParameter = new CompanyFilter();
        }
        else
        {
            JsonSerializer serializer = new JsonSerializer();
            FilterParameter = serializer.Unserialize<CompanyFilter>(content);
        }

        // Restore site identifier
        SiteIdentifierParameter = ValidationHelper.GetInteger(parameters["SiteID"], 0);
    }


    /// <summary>
    /// Displays an error message and logs the specified exception to the event log.
    /// </summary>
    /// <param name="exception">The exception to handle.</param>
    private void HandleException(Exception exception)
    {
        ErrorSummary.Report(exception);
        EventLogProvider.LogException("Data.com Connector", "SelectCompanyPage", exception);
    }

    #endregion
}