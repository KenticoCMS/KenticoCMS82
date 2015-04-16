using System;
using System.Data;

using CMS.Helpers;
using CMS.Reporting;
using CMS.UIControls;

public partial class CMSAPIExamples_Code_Tools_Reporting_Default : CMSAPIExamplePage
{
    #region "Initialization"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Report category
        apiCreateReportCategory.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(CreateReportCategory);
        apiGetAndUpdateReportCategory.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(GetAndUpdateReportCategory);
        apiGetAndBulkUpdateReportCategories.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(GetAndBulkUpdateReportCategories);
        apiDeleteReportCategory.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(DeleteReportCategory);
        // Report
        apiCreateReport.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(CreateReport);
        apiGetAndUpdateReport.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(GetAndUpdateReport);
        apiGetAndBulkUpdateReports.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(GetAndBulkUpdateReports);
        apiDeleteReport.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(DeleteReport);

        // Report graph
        apiCreateReportGraph.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(CreateReportGraph);
        apiGetAndUpdateReportGraph.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(GetAndUpdateReportGraph);
        apiGetAndBulkUpdateReportGraphs.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(GetAndBulkUpdateReportGraphs);
        apiDeleteReportGraph.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(DeleteReportGraph);

        // Report table
        apiCreateReportTable.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(CreateReportTable);
        apiGetAndUpdateReportTable.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(GetAndUpdateReportTable);
        apiGetAndBulkUpdateReportTables.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(GetAndBulkUpdateReportTables);
        apiDeleteReportTable.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(DeleteReportTable);

        // Report value
        apiCreateReportValue.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(CreateReportValue);
        apiGetAndUpdateReportValue.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(GetAndUpdateReportValue);
        apiGetAndBulkUpdateReportValues.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(GetAndBulkUpdateReportValues);
        apiDeleteReportValue.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(DeleteReportValue);

        // Report actions
        apiInsertElementsToLayout.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(InsertElementsToLayout);
    }

    #endregion


    #region "Mass actions"

    /// <summary>
    /// Runs all creating and managing examples.
    /// </summary>
    public override void RunAll()
    {
        base.RunAll();

        // Report category
        apiCreateReportCategory.Run();
        apiGetAndUpdateReportCategory.Run();
        apiGetAndBulkUpdateReportCategories.Run();

        // Report
        apiCreateReport.Run();
        apiGetAndUpdateReport.Run();
        apiGetAndBulkUpdateReports.Run();

        // Report graph
        apiCreateReportGraph.Run();
        apiGetAndUpdateReportGraph.Run();
        apiGetAndBulkUpdateReportGraphs.Run();

        // Report table
        apiCreateReportTable.Run();
        apiGetAndUpdateReportTable.Run();
        apiGetAndBulkUpdateReportTables.Run();

        // Report value
        apiCreateReportValue.Run();
        apiGetAndUpdateReportValue.Run();
        apiGetAndBulkUpdateReportValues.Run();

        // Report actions
        apiInsertElementsToLayout.Run();
    }


    /// <summary>
    /// Runs all cleanup examples.
    /// </summary>
    public override void CleanUpAll()
    {
        base.CleanUpAll();

        // Report graph
        apiDeleteReportGraph.Run();

        // Report table
        apiDeleteReportTable.Run();

        // Report value
        apiDeleteReportValue.Run();

        // Report
        apiDeleteReport.Run();

        // Report category
        apiDeleteReportCategory.Run();
    }

    #endregion


    #region "API examples - Report category"

    /// <summary>
    /// Creates report category. Called when the "Create category" button is pressed.
    /// </summary>
    private bool CreateReportCategory()
    {
        // Create new report category object
        ReportCategoryInfo newCategory = new ReportCategoryInfo();

        // Set the properties
        newCategory.CategoryDisplayName = "My new category";
        newCategory.CategoryCodeName = "MyNewCategory";

        // Save the report category
        ReportCategoryInfoProvider.SetReportCategoryInfo(newCategory);

        return true;
    }


    /// <summary>
    /// Gets and updates report category. Called when the "Get and update category" button is pressed.
    /// Expects the CreateReportCategory method to be run first.
    /// </summary>
    private bool GetAndUpdateReportCategory()
    {
        // Get the report category
        ReportCategoryInfo updateCategory = ReportCategoryInfoProvider.GetReportCategoryInfo("MyNewCategory");
        if (updateCategory != null)
        {
            // Update the properties
            updateCategory.CategoryDisplayName = updateCategory.CategoryDisplayName.ToLower();

            // Save the changes
            ReportCategoryInfoProvider.SetReportCategoryInfo(updateCategory);

            return true;
        }

        return false;
    }


    /// <summary>
    /// Gets and bulk updates report categories. Called when the "Get and bulk update categories" button is pressed.
    /// Expects the CreateReportCategory method to be run first.
    /// </summary>
    private bool GetAndBulkUpdateReportCategories()
    {
        // Prepare the parameters
        string where = "CategoryCodeName LIKE N'MyNewCategory%'";

        // Get the data
        DataSet categories = ReportCategoryInfoProvider.GetCategories(where, null);
        if (!DataHelper.DataSourceIsEmpty(categories))
        {
            // Loop through the individual items
            foreach (DataRow categoryDr in categories.Tables[0].Rows)
            {
                // Create object from DataRow
                ReportCategoryInfo modifyCategory = new ReportCategoryInfo(categoryDr);

                // Update the properties
                modifyCategory.CategoryDisplayName = modifyCategory.CategoryDisplayName.ToUpper();

                // Save the changes
                ReportCategoryInfoProvider.SetReportCategoryInfo(modifyCategory);
            }

            return true;
        }

        return false;
    }


    /// <summary>
    /// Deletes report category. Called when the "Delete category" button is pressed.
    /// Expects the CreateReportCategory method to be run first.
    /// </summary>
    private bool DeleteReportCategory()
    {
        // Get the report category
        ReportCategoryInfo deleteCategory = ReportCategoryInfoProvider.GetReportCategoryInfo("MyNewCategory");

        // Delete the report category
        ReportCategoryInfoProvider.DeleteReportCategoryInfo(deleteCategory);

        return (deleteCategory != null);
    }

    #endregion


    #region "API examples - Report"

    /// <summary>
    /// Creates report. Called when the "Create report" button is pressed.
    /// </summary>
    private bool CreateReport()
    {
        // Get the report category
        ReportCategoryInfo category = ReportCategoryInfoProvider.GetReportCategoryInfo("MyNewCategory");
        if (category != null)
        {
            // Create new report object
            ReportInfo newReport = new ReportInfo();

            // Set the properties
            newReport.ReportDisplayName = "My new report";
            newReport.ReportName = "MyNewReport";
            newReport.ReportCategoryID = category.CategoryID;
            newReport.ReportAccess = ReportAccessEnum.All;
            newReport.ReportLayout = "";
            newReport.ReportParameters = "";

            // Save the report
            ReportInfoProvider.SetReportInfo(newReport);

            return true;
        }
        return false;
    }


    /// <summary>
    /// Gets and updates report. Called when the "Get and update report" button is pressed.
    /// Expects the CreateReport method to be run first.
    /// </summary>
    private bool GetAndUpdateReport()
    {
        // Get the report
        ReportInfo updateReport = ReportInfoProvider.GetReportInfo("MyNewReport");
        if (updateReport != null)
        {
            // Update the properties
            updateReport.ReportDisplayName = updateReport.ReportDisplayName.ToLower();

            // Save the changes
            ReportInfoProvider.SetReportInfo(updateReport);

            return true;
        }

        return false;
    }


    /// <summary>
    /// Gets and bulk updates reports. Called when the "Get and bulk update reports" button is pressed.
    /// Expects the CreateReport method to be run first.
    /// </summary>
    private bool GetAndBulkUpdateReports()
    {
        // Prepare the parameters
        string where = "ReportName LIKE N'MyNewReport%'";
        string orderby = "";
        int topN = 0;
        string columns = "";

        // Get the data
        DataSet reports = ReportInfoProvider.GetReports(where, orderby, topN, columns);
        if (!DataHelper.DataSourceIsEmpty(reports))
        {
            // Loop through the individual items
            foreach (DataRow reportDr in reports.Tables[0].Rows)
            {
                // Create object from DataRow
                ReportInfo modifyReport = new ReportInfo(reportDr);

                // Update the properties
                modifyReport.ReportDisplayName = modifyReport.ReportDisplayName.ToUpper();

                // Save the changes
                ReportInfoProvider.SetReportInfo(modifyReport);
            }

            return true;
        }

        return false;
    }


    /// <summary>
    /// Deletes report. Called when the "Delete report" button is pressed.
    /// Expects the CreateReport method to be run first.
    /// </summary>
    private bool DeleteReport()
    {
        // Get the report
        ReportInfo deleteReport = ReportInfoProvider.GetReportInfo("MyNewReport");

        // Delete the report
        ReportInfoProvider.DeleteReportInfo(deleteReport);

        return (deleteReport != null);
    }

    #endregion


    #region "API examples - Report graph"

    /// <summary>
    /// Creates report graph. Called when the "Create graph" button is pressed.
    /// </summary>
    private bool CreateReportGraph()
    {
        // Get report object by report code name
        ReportInfo report = ReportInfoProvider.GetReportInfo("MyNewReport");

        // If report exists
        if (report != null)
        {
            // Create new report graph object
            ReportGraphInfo newGraph = new ReportGraphInfo();

            // Set the properties
            newGraph.GraphDisplayName = "My new graph";
            newGraph.GraphName = "MyNewGraph";
            newGraph.GraphQuery = "SELECT TOP 10 DocumentName, DocumentID FROM CMS_Document";
            newGraph.GraphReportID = report.ReportID;
            newGraph.GraphQueryIsStoredProcedure = false;
            newGraph.GraphType = "bar";

            // Save the report graph
            ReportGraphInfoProvider.SetReportGraphInfo(newGraph);

            return true;
        }

        return false;
    }


    /// <summary>
    /// Gets and updates report graph. Called when the "Get and update graph" button is pressed.
    /// Expects the CreateReportGraph method to be run first.
    /// </summary>
    private bool GetAndUpdateReportGraph()
    {
        // Get the report graph
        ReportGraphInfo updateGraph = ReportGraphInfoProvider.GetReportGraphInfo("MyNewGraph");
        if (updateGraph != null)
        {
            // Update the properties
            updateGraph.GraphDisplayName = updateGraph.GraphDisplayName.ToLower();

            // Save the changes
            ReportGraphInfoProvider.SetReportGraphInfo(updateGraph);

            return true;
        }

        return false;
    }


    /// <summary>
    /// Gets and bulk updates report graphs. Called when the "Get and bulk update graphs" button is pressed.
    /// Expects the CreateReportGraph method to be run first.
    /// </summary>
    private bool GetAndBulkUpdateReportGraphs()
    {
        // Prepare the parameters
        string where = "GraphName LIKE N'MyNewGraph%'";

        // Get the data
        DataSet graphs = ReportGraphInfoProvider.GetGraphs(where, null);
        if (!DataHelper.DataSourceIsEmpty(graphs))
        {
            // Loop through the individual items
            foreach (DataRow graphDr in graphs.Tables[0].Rows)
            {
                // Create object from DataRow
                ReportGraphInfo modifyGraph = new ReportGraphInfo(graphDr);

                // Update the properties
                modifyGraph.GraphDisplayName = modifyGraph.GraphDisplayName.ToUpper();

                // Save the changes
                ReportGraphInfoProvider.SetReportGraphInfo(modifyGraph);
            }

            return true;
        }
        return false;
    }


    /// <summary>
    /// Deletes report graph. Called when the "Delete graph" button is pressed.
    /// Expects the CreateReportGraph method to be run first.
    /// </summary>
    private bool DeleteReportGraph()
    {
        // Get the report graph
        ReportGraphInfo deleteGraph = ReportGraphInfoProvider.GetReportGraphInfo("MyNewGraph");

        // Delete the report graph
        ReportGraphInfoProvider.DeleteReportGraphInfo(deleteGraph);

        return (deleteGraph != null);
    }

    #endregion


    #region "API examples - Report table"

    /// <summary>
    /// Creates report table. Called when the "Create table" button is pressed.
    /// </summary>
    private bool CreateReportTable()
    {
        // Get report object by report code name
        ReportInfo report = ReportInfoProvider.GetReportInfo("MyNewReport");

        // If report exists
        if (report != null)
        {
            // Create new report table object
            ReportTableInfo newTable = new ReportTableInfo();

            // Set the properties
            newTable.TableDisplayName = "My new table";
            newTable.TableName = "MyNewTable";
            newTable.TableQuery = "SELECT TOP 10 DocumentName, DocumentID FROM CMS_Document";
            newTable.TableReportID = report.ReportID;
            newTable.TableQueryIsStoredProcedure = false;

            // Save the report table
            ReportTableInfoProvider.SetReportTableInfo(newTable);

            return true;
        }
        return false;
    }


    /// <summary>
    /// Gets and updates report table. Called when the "Get and update table" button is pressed.
    /// Expects the CreateReportTable method to be run first.
    /// </summary>
    private bool GetAndUpdateReportTable()
    {
        // Get the report table
        ReportTableInfo updateTable = ReportTableInfoProvider.GetReportTableInfo("MyNewTable");
        if (updateTable != null)
        {
            // Update the properties
            updateTable.TableDisplayName = updateTable.TableDisplayName.ToLower();

            // Save the changes
            ReportTableInfoProvider.SetReportTableInfo(updateTable);

            return true;
        }

        return false;
    }


    /// <summary>
    /// Gets and bulk updates report tables. Called when the "Get and bulk update tables" button is pressed.
    /// Expects the CreateReportTable method to be run first.
    /// </summary>
    private bool GetAndBulkUpdateReportTables()
    {
        // Prepare the parameters
        string where = "TableName LIKE N'MyNewTable%'";

        // Get the data
        DataSet tables = ReportTableInfoProvider.GetTables(where, null);
        if (!DataHelper.DataSourceIsEmpty(tables))
        {
            // Loop through the individual items
            foreach (DataRow tableDr in tables.Tables[0].Rows)
            {
                // Create object from DataRow
                ReportTableInfo modifyTable = new ReportTableInfo(tableDr);

                // Update the properties
                modifyTable.TableDisplayName = modifyTable.TableDisplayName.ToUpper();

                // Save the changes
                ReportTableInfoProvider.SetReportTableInfo(modifyTable);
            }

            return true;
        }

        return false;
    }


    /// <summary>
    /// Deletes report table. Called when the "Delete table" button is pressed.
    /// Expects the CreateReportTable method to be run first.
    /// </summary>
    private bool DeleteReportTable()
    {
        // Get the report table
        ReportTableInfo deleteTable = ReportTableInfoProvider.GetReportTableInfo("MyNewTable");

        // Delete the report table
        ReportTableInfoProvider.DeleteReportTableInfo(deleteTable);

        return (deleteTable != null);
    }

    #endregion


    #region "API examples - Report value"

    /// <summary>
    /// Creates report value. Called when the "Create value" button is pressed.
    /// </summary>
    private bool CreateReportValue()
    {
        // Get report object by report code name
        ReportInfo report = ReportInfoProvider.GetReportInfo("MyNewReport");

        // If report exists
        if (report != null)
        {
            // Create new report value object
            ReportValueInfo newValue = new ReportValueInfo();

            // Set the properties
            newValue.ValueDisplayName = "My new value";
            newValue.ValueName = "MyNewValue";
            newValue.ValueQuery = "SELECT COUNT(DocumentName) FROM CMS_Document";
            newValue.ValueQueryIsStoredProcedure = false;
            newValue.ValueReportID = report.ReportID;

            // Save the report value
            ReportValueInfoProvider.SetReportValueInfo(newValue);

            return true;
        }
        return false;
    }


    /// <summary>
    /// Gets and updates report value. Called when the "Get and update value" button is pressed.
    /// Expects the CreateReportValue method to be run first.
    /// </summary>
    private bool GetAndUpdateReportValue()
    {
        // Get the report value
        ReportValueInfo updateValue = ReportValueInfoProvider.GetReportValueInfo("MyNewValue");
        if (updateValue != null)
        {
            // Update the properties
            updateValue.ValueDisplayName = updateValue.ValueDisplayName.ToLower();

            // Save the changes
            ReportValueInfoProvider.SetReportValueInfo(updateValue);

            return true;
        }

        return false;
    }


    /// <summary>
    /// Gets and bulk updates report values. Called when the "Get and bulk update values" button is pressed.
    /// Expects the CreateReportValue method to be run first.
    /// </summary>
    private bool GetAndBulkUpdateReportValues()
    {
        // Prepare the parameters
        string where = "ValueName LIKE N'MyNewValue%'";

        // Get the data
        DataSet values = ReportValueInfoProvider.GetValues(where, null);
        if (!DataHelper.DataSourceIsEmpty(values))
        {
            // Loop through the individual items
            foreach (DataRow valueDr in values.Tables[0].Rows)
            {
                // Create object from DataRow
                ReportValueInfo modifyValue = new ReportValueInfo(valueDr);

                // Update the properties
                modifyValue.ValueDisplayName = modifyValue.ValueDisplayName.ToUpper();

                // Save the changes
                ReportValueInfoProvider.SetReportValueInfo(modifyValue);
            }

            return true;
        }

        return false;
    }


    /// <summary>
    /// Deletes report value. Called when the "Delete value" button is pressed.
    /// Expects the CreateReportValue method to be run first.
    /// </summary>
    private bool DeleteReportValue()
    {
        // Get the report value
        ReportValueInfo deleteValue = ReportValueInfoProvider.GetReportValueInfo("MyNewValue");

        // Delete the report value
        ReportValueInfoProvider.DeleteReportValueInfo(deleteValue);

        return (deleteValue != null);
    }

    #endregion


    #region "API examples - Report actions"

    private bool InsertElementsToLayout()
    {
        // Get report object by report code name
        ReportInfo report = ReportInfoProvider.GetReportInfo("MyNewReport");

        // If report exists
        if (report != null)
        {
            ReportGraphInfo graph = ReportGraphInfoProvider.GetReportGraphInfo("MyNewGraph");
            if (graph != null)
            {
                report.ReportLayout += "<br/>%%control:Report" + ReportItemType.Graph + "?" + report.ReportName + "." + graph.GraphName + "%%<br/>";
            }

            ReportTableInfo table = ReportTableInfoProvider.GetReportTableInfo("MyNewTable");
            if (table != null)
            {
                report.ReportLayout += "<br/>%%control:Report" + ReportItemType.Table + "?" + report.ReportName + "." + table.TableName + "%%<br/>";
            }

            ReportValueInfo value = ReportValueInfoProvider.GetReportValueInfo("MyNewValue");
            if (value != null)
            {
                report.ReportLayout += "<br/>%%control:Report" + ReportItemType.Value + "?" + report.ReportName + "." + value.ValueName + "%%<br/>";
            }

            ReportInfoProvider.SetReportInfo(report);

            return true;
        }
        return false;
    }

    #endregion
}