using System;
using System.Data;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.EmailEngine;
using CMS.EventLog;
using CMS.ExtendedControls;
using CMS.Helpers;
using CMS.ImportExport;
using CMS.IO;
using CMS.Reporting;

public partial class CMSModules_Reporting_Controls_ReportTable : AbstractReportControl
{
    #region "Variables"

    private UIGridView mUIGridView;
    private ReportTableInfo mTableInfo;
    private string mParameter = String.Empty;
    private ReportInfo mReportInfo;

    /// <summary>
    /// Indicates whether exception was thrown during data loading
    /// </summary>
    private bool mErrorOccurred;

    #endregion


    #region "Properties"

    /// <summary>
    /// Gets the UIGridView object.
    /// </summary>
    protected UIGridView UIGridViewObject
    {
        get
        {
            return mUIGridView ?? (mUIGridView = new UIGridView());
        }
    }


    /// <summary>
    /// Report table connection string
    /// </summary>
    public override string ConnectionString
    {
        get
        {
            String tableConn = (TableInfo == null) ? String.Empty : TableInfo.TableConnectionString;
            if (String.IsNullOrEmpty(tableConn))
            {
                return (mReportInfo == null) ? String.Empty : mReportInfo.ReportConnectionString;
            }

            return tableConn;
        }
    }


    /// <summary>
    /// Table name - prevent using viewstate (problems with displayreportcontrol and postback).
    /// </summary>
    public override string Parameter
    {
        get
        {
            return mParameter;
        }
        set
        {
            mParameter = value;
        }
    }


    /// <summary>
    /// Direct table info used by preview.
    /// </summary>
    public ReportTableInfo TableInfo
    {
        get
        {
            return mTableInfo ?? (mTableInfo = ReportTableInfoProvider.GetReportTableInfo(Parameter));
        }
        set
        {
            mTableInfo = value;
        }
    }


    /// <summary>
    /// Page size for paged tables
    /// </summary>
    public int PageSize
    {
        get;
        set;
    }


    /// <summary>
    /// Enables/disables paging (if null report settings is used)
    /// </summary>
    public bool? EnablePaging
    {
        get;
        set;
    }

    #endregion


    #region "Control events"

    /// <summary>
    /// Created grid view based on parameter from report table.
    /// </summary>
    protected override void CreateChildControls()
    {
        base.CreateChildControls();

        ItemType = ReportItemType.Table;
    }


    protected override void OnLoad(EventArgs e)
    {
        UIGridViewObject.RowDataBound += UIGridViewObject_RowDataBound;
        
        base.OnLoad(e);
    }


    protected override void OnPreRender(EventArgs e)
    {
        if (mReportInfo != null)
        {
            if (TableInfo != null)
            {
                EnableSubscription = (EnableSubscription && ValidationHelper.GetBoolean(TableInfo.TableSettings["SubscriptionEnabled"], true) && mReportInfo.ReportEnableSubscription);
                EnableExport = (EnableExport && ValidationHelper.GetBoolean(TableInfo.TableSettings["ExportEnabled"], false));
                // Register context menu for export - if allowed
                RegisterSubscriptionScript(TableInfo.TableReportID, "tableid", TableInfo.TableID, menuCont);
            }

            // Export data
            if (!mErrorOccurred)
            {
                ProcessExport(ValidationHelper.GetCodeName(mReportInfo.ReportDisplayName));
            }
        }

        base.OnPreRender(e);

        CSSHelper.RegisterBootstrap(Page);
    }


    /// <summary>
    /// Handles paging on live site.
    /// </summary>
    protected void GridViewObject_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
    }


    protected void UIGridViewObject_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        // Hide time information for all dates in day interval mode
        if (SelectedInterval.ToLowerCSafe() == "day")
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                DataRow drv = ((DataRowView)e.Row.DataItem).Row;

                foreach (DataColumn column in drv.Table.Columns)
                {
                    if (column.DataType.FullName.ToLowerCSafe() == "system.datetime")
                    {
                        DateTime date = ValidationHelper.GetDateTime(drv[column.Ordinal], DateTime.Now);

                        e.Row.Cells[column.Ordinal].Text = date.Date.ToShortDateString();
                    }
                }
            }
        }
    }

    #endregion


    #region "Public methods"

    /// <summary>
    /// Reload data.
    /// </summary>
    public override void ReloadData(bool forceLoad)
    {
        if ((TableInfo == null) || ((GraphImageWidth != 0) && (ComputedWidth == 0)))
        {
            // Graph width is computed no need to create graph
            return;
        }

        Visible = true;

        EnsureChildControls();

        LoadTable();

        mReportInfo = ReportInfoProvider.GetReportInfo(TableInfo.TableReportID);
        if (mReportInfo == null)
        {
            return;
        }

        // Check security settings
        if (!(CheckReportAccess(mReportInfo) && CheckEmailModeSubscription(mReportInfo, ValidationHelper.GetBoolean(TableInfo.TableSettings["SubscriptionEnabled"], true))))
        {
            Visible = false;
            return;
        }

        // Prepare query attributes
        QueryIsStoredProcedure = TableInfo.TableQueryIsStoredProcedure;
        QueryText = TableInfo.TableQuery;

        // Init parameters
        InitParameters(mReportInfo.ReportParameters);

        // Init macro resolver
        InitResolver();

        mErrorOccurred = false;
        DataSet ds = null;

        try
        {
            // Load data
            ds = LoadData();
        }
        catch (Exception ex)
        {
            // Display error message, if data load fail
            lblError.Visible = true;
            lblError.Text = "[ReportTable.ascx] Error loading the data: " + ex.Message;
            EventLogProvider.LogException("Report table", "E", ex);
            mErrorOccurred = true;
        }

        // If no data load, set empty dataset
        if (DataHelper.DataSourceIsEmpty(ds))
        {
            if (EmailMode && SendOnlyNonEmptyDataSource)
            {
                Visible = false;
                return;
            }

            string noRecordText = ValidationHelper.GetString(TableInfo.TableSettings["QueryNoRecordText"], String.Empty);
            if (!String.IsNullOrEmpty(noRecordText))
            {
                UIGridViewObject.Visible = false;
                lblInfo.Text = ResolveMacros(noRecordText);
                lblInfo.Visible = true;
                EnableExport = false;
                return;
            }

            if (!EmailMode)
            {
                Visible = false;
                return;
            }
        }
        else
        {
            UIGridViewObject.Visible = true;
            // Resolve macros in column names
            int i = 0;
            foreach (DataColumn dc in ds.Tables[0].Columns)
            {
                if (dc.ColumnName == "Column" + (i + 1))
                {
                    dc.ColumnName = ResolveMacros(ds.Tables[0].Rows[0][i].ToString());
                }
                else
                {
                    dc.ColumnName = ResolveMacros(dc.ColumnName);
                }
                i++;
            }

            // Resolve macros in dataset
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                foreach (DataColumn dc in ds.Tables[0].Columns)
                {
                    if (dc.DataType.FullName.ToLowerCSafe() == "system.string")
                    {
                        dr[dc.ColumnName] = ResolveMacros(ValidationHelper.GetString(dr[dc.ColumnName], String.Empty));
                    }
                }
            }

            if (EmailMode)
            {
                // For some email formats, export data in csv format
                EmailFormatEnum format = EmailHelper.GetEmailFormat(ReportSubscriptionSiteID);

                if ((format == EmailFormatEnum.Both) || (format == EmailFormatEnum.PlainText))
                {
                    using (MemoryStream ms = MemoryStream.New())
                    {
                        DataExportHelper deh = new DataExportHelper(ds);
                        byte[] data = deh.ExportToCSV(ds, 0, ms, true);
                        ReportSubscriptionSender.AddToRequest(mReportInfo.ReportName, "t" + TableInfo.TableName, data);
                    }
                }

                // For plain text email show table only as attachment
                if (format == EmailFormatEnum.PlainText)
                {
                    menuCont.Visible = false;
                    ltlEmail.Visible = true;
                    ltlEmail.Text = String.Format(GetString("reportsubscription.attachment"), TableInfo.TableName);
                    return;
                }

                GenerateTableForEmail(ds);
                menuCont.Visible = false;
                return;
            }
        }

        // Databind to gridview control
        UIGridViewObject.DataSource = ds;
        EnsurePageIndex();
        UIGridViewObject.DataBind();

        if ((TableFirstColumnWidth != Unit.Empty) && (UIGridViewObject.Rows.Count > 0))
        {
            UIGridViewObject.Rows[0].Cells[0].Width = TableFirstColumnWidth;
        }
    }


    /// <summary>
    /// Returns true if graph belongs to report.
    /// </summary>
    /// <param name="report">Report to validate</param>
    public override bool IsValid(ReportInfo report)
    {
        ReportTableInfo rti = TableInfo;

        if ((report != null) && (rti != null) && (report.ReportID == rti.TableReportID))
        {
            return true;
        }

        return false;
    }

    #endregion


    #region "Private methods"

    /// <summary>
    /// Adds GridView to the controls collection.
    /// </summary>
    private void LoadTable()
    {
        if (TableInfo == null)
        {
            return;
        }

        // Enable paging has higher priority - if not set use report settings
        UIGridViewObject.AllowPaging = (!EmailMode && (EnablePaging.HasValue ? EnablePaging.Value : ValidationHelper.GetBoolean(TableInfo.TableSettings["enablepaging"], false)));

        if (UIGridViewObject.AllowPaging)
        {
            // Webpart - higher priority
            UIGridViewObject.PageSize = (PageSize > 0) ? PageSize : ValidationHelper.GetInteger(TableInfo.TableSettings["pagesize"], 10);
            UIGridViewObject.PagerSettings.Mode = (PagerButtons)ValidationHelper.GetInteger(TableInfo.TableSettings["pagemode"], (int)PagerButtons.Numeric);
            UIGridViewObject.PageIndexChanging += GridViewObject_PageIndexChanging;
        }
        UIGridViewObject.AllowSorting = false;

        // Get SkinID from reportTable custom data
        string skinId = ValidationHelper.GetString(TableInfo.TableSettings["skinid"], "ReportGridAnalytics");
        if (skinId != String.Empty)
        {
            if (String.IsNullOrEmpty((UIGridViewObject.SkinID)))
            {
                UIGridViewObject.SkinID = skinId;
            }
        }

        UIGridViewObject.ID = "reportGrid";

        // Add grid view control to the page
        plcGrid.Controls.Clear();
        plcGrid.Controls.Add(UIGridViewObject);

        if ((RenderCssClasses) && (String.IsNullOrEmpty(UIGridViewObject.SkinID)))
        {
            //Clear the css styles to eliminate control state
            UIGridViewObject.HeaderStyle.CssClass = String.Empty;
            UIGridViewObject.CssClass = String.Empty;
            UIGridViewObject.RowStyle.CssClass = String.Empty;
            UIGridViewObject.AlternatingRowStyle.CssClass = String.Empty;
        }
    }


    /// <summary>
    /// Generates table for email
    /// </summary>
    /// <param name="ds">Dataset with table data</param>
    private void GenerateTableForEmail(DataSet ds)
    {
        if (DataHelper.DataSourceIsEmpty(ds))
        {
            return;
        }

        ltlEmail.Visible = true;

        StringBuilder sb = new StringBuilder();

        // Generate header
        sb.Append("<table class=\"table\"><thead><tr class=\"unigrid-head\">");
        DataTable dt = ds.Tables[0];

        foreach (DataColumn dc in dt.Columns)
        {
            sb.Append("<th scope=\"col\">" + dc.ColumnName + "</th>");
        }

        sb.Append("</tr></thead><tbody>");

        int rowNumber = 1;

        // Generate rows
        foreach (DataRow dr in dt.Rows)
        {
            rowNumber++;
            string cssClass = (rowNumber % 2 == 0) ? "even-row" : "odd-row";
            sb.AppendFormat("<tr class=\"{0}\">", cssClass);

            for (int i = 0; i < dt.Columns.Count; i++)
            {
                sb.AppendFormat("<td>{0}</td>", dr[i]);
            }

            sb.Append("</tr>");
        }

        sb.Append("</tbody></table>");

        ltlEmail.Text = sb.ToString();
    }


    /// <summary>
    /// Ensures the current page index with dependenco on request data du to different contol's life cycle.
    /// </summary>
    private void EnsurePageIndex()
    {
        if ((UIGridViewObject != null) && (UIGridViewObject.AllowPaging))
        {
            // Get current postback target
            string eventTarget = Request.Params[Page.postEventSourceID];

            // Handle paging manually because of lifecycle of the control
            if (CMSString.Compare(eventTarget, UIGridViewObject.UniqueID, true) == 0)
            {
                // Get the current page value
                string eventArg = ValidationHelper.GetString(Request.Params[Page.postEventArgumentID], String.Empty);

                string[] args = eventArg.Split('$');
                if ((args.Length == 2) && (CMSString.Compare(args[0], "page", true) == 0))
                {
                    string pageValue = args[1];
                    int pageIndex = 0;
                    // Switch by page value  0,1.... first,last
                    switch (pageValue.ToLowerCSafe())
                    {
                        // Last item
                        case "last":
                            // Check whether page count is available
                            if (UIGridViewObject.PageCount > 0)
                            {
                                pageIndex = UIGridViewObject.PageCount - 1;
                            }
                            // if page count is not defined, try compute page count
                            else
                            {
                                DataSet ds = UIGridViewObject.DataSource as DataSet;
                                if (!DataHelper.DataSourceIsEmpty(ds))
                                {
                                    pageIndex = ds.Tables[0].Rows.Count / UIGridViewObject.PageSize;
                                }
                            }
                            break;

                        case "next":
                            pageIndex = UIGridViewObject.PageIndex + 1;
                            break;

                        case "prev":
                            pageIndex = UIGridViewObject.PageIndex - 1;
                            break;

                        // Page number
                        default:
                            pageIndex = ValidationHelper.GetInteger(pageValue, 1) - 1;
                            break;
                    }

                    UIGridViewObject.PageIndex = pageIndex;
                }
            }
        }
    }

    #endregion
}