using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Web.UI;

using CMS.Base;
using CMS.Controls;
using CMS.DataEngine;
using CMS.DocumentEngine;
using CMS.EventLog;
using CMS.ExtendedControls;
using CMS.Helpers;
using CMS.MacroEngine;
using CMS.SiteProvider;
using CMS.Synchronization;
using CMS.UIControls;


public partial class CMSModules_System_Controls_MacrosGrid : CMSUserControl, IUniPageable
{
    private const string TEST_PREFIX = "test";


    #region "MacroExpr"

    /// <summary>
    /// Structure holding macro expression for the list
    /// </summary>
    protected class MacroExpr
    {
        public string SignedBy;
        public string Expression;
        public bool SignatureValid;
        public bool Error;
        public string ObjectType;
        public int ObjectID;
        public string Field;
        public string RuleText;
        public string ErrorMessage;
    }

    #endregion


    #region "Variables"

    protected int mTotalItems = 0;

    #endregion


    #region "Properties"

    /// <summary>
    /// Returns the pager control.
    /// </summary>
    public UIPager PagerControl
    {
        get
        {
            return pagerItems;
        }
    }


    /// <summary>
    /// Returns total number of items.
    /// </summary>
    public int TotalItems
    {
        get
        {
            return mTotalItems;
        }
    }


    /// <summary>
    /// True, if the data is currently displayed
    /// </summary>
    private bool DisplayData
    {
        get
        {
            return ValidationHelper.GetBoolean(ViewState["DataDisplayed"], false);
        }

        set
        {
            ViewState["DataDisplayed"] = value;
        }
    }

    #endregion


    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        pagerItems.PagedControl = this;
        pagerItems.ShowDirectPageControl = false;
        pagerItems.ShowFirstLastButtons = false;

        up.ProgressText = GetString("Macros.Collecting");
    }


    protected void btnSearch_Click(object sender, EventArgs e)
    {
        StartSearch();
    }


    protected void btnView_Click(object sender, EventArgs e)
    {
        pagerItems.UniPager.CurrentPage = 1;

        DisplayData = true;
    }


    private void StartSearch()
    {
        DisplayData = false;

        lblInfo.ResourceString = "macroreport.loading";
        lblInfo.Visible = true;

        plcItems.Visible = false;

        ControlsHelper.RaisePostback(btnView);
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        if (!RequestHelper.IsPostBack())
        {
            StartSearch();
        }

        if (DisplayData)
        {
            plcItems.Visible = true;
            ReloadData();
        }
    }


    public void ReloadData()
    {
        // Prepare the indexes for paging
        int pageSize = pagerItems.CurrentPageSize;

        int startIndex = (pagerItems.CurrentPage - 1) * pageSize;
        int endIndex = startIndex + pageSize;

        int i = 0;

        // Process the macros
        if (!StopProcessing && Visible)
        {
            try
            {
                int maxRecords = pageSize == -1 ? -1 : (pageSize * (pagerItems.CurrentPage + 11));
                var macros = GetMacros(startIndex, endIndex, maxRecords, ref mTotalItems);
                foreach (MacroExpr expression in macros)
                {
                    RenderItem(expression);
                    i++;
                }
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);

                EventLogProvider.LogException("Macro report", "GETMACROS", ex);
            }
        }

        plcItems.Visible = (i > 0);
        lblInfo.Visible = (i <= 0);

        // Call page binding event
        if (OnPageBinding != null)
        {
            OnPageBinding(this, null);
        }
    }


    /// <summary>
    /// Gets the macros from the system
    /// </summary>
    /// <param name="startIndex">Start index</param>
    /// <param name="endIndex">End index</param>
    /// <param name="maxTotalRecords">Maximum number of total records to process</param>
    /// <param name="totalRecords">Returns the total number of records found</param>
    private IEnumerable<MacroExpr> GetMacros(int startIndex, int endIndex, int maxTotalRecords, ref int totalRecords)
    {
        var index = 0;
        IEnumerable<string> objectTypes = null;

        // Get object types to search
        var selectedType = ValidationHelper.GetString(selObjectType.Value, "");
        if (!String.IsNullOrEmpty(selectedType))
        {
            if (ObjectTypeManager.GetRegisteredTypeInfo(selectedType) != null)
            {
                objectTypes = new List<string> { selectedType };
            }
        }

        if (objectTypes == null)
        {
            objectTypes = ObjectTypeManager.ObjectTypesWithMacros;
        }

        var result = new List<MacroExpr>();
        var search = txtFilter.Text;

        var invalid = chkInvalid.Checked;
        var skipTesting = SystemContext.DevelopmentMode && chkSkipTesting.Checked;

        var type = drpType.Text;

        foreach (var objectType in objectTypes)
        {
            // Skip certain object types
            switch (objectType)
            {
                case ObjectVersionHistoryInfo.OBJECT_TYPE:
                case VersionHistoryInfo.OBJECT_TYPE:
                case StagingTaskInfo.OBJECT_TYPE:
                case IntegrationTaskInfo.OBJECT_TYPE:
                    continue;
            }

            // Process all objects of the given type
            var infos = new ObjectQuery(objectType)
                .TopN(maxTotalRecords)
                .BinaryData(false);

            var typeInfo = infos.TypeInfo;

            if (skipTesting)
            {
                ExcludeTestingObjects(infos);
            }

            if (!String.IsNullOrEmpty(search))
            {
                // Search particular expression
                infos.WhereAnyColumnContains(search);
            }
            else
            {
                // Search just type
                infos.WhereAnyColumnContains("{" + type);
            }

            Action<DataRow> collectMacros = dr =>
            {
                var drc = new DataRowContainer(dr);

                // Process all expressions
                MacroProcessor.ProcessMacros(drc, (expression, colName) =>
                    {
                        var originalExpression = expression;

                        // Decode macro from XML
                        if (MacroProcessor.IsXMLColumn(colName))
                        {
                            expression = HTMLHelper.HTMLDecode(expression);
                        }

                        MacroExpr e = null;
                        bool add = false;

                        if (String.IsNullOrEmpty(search) || (expression.IndexOfCSafe(search, true) >= 0))
                        {
                            // If not tracking errors, count immediately
                            if (!invalid)
                            {
                                // Apply paging. Endindex is -1 when paging is off
                                if ((endIndex < 0) || ((index >= startIndex) && (index <= endIndex)))
                                {
                                    e = GetMacroExpr(expression);
                                    add = true;
                                }

                                index++;
                            }
                            else
                            {
                                e = GetMacroExpr(expression);

                                // Filter invalid signature / syntax
                                var pass = !e.SignatureValid || e.Error;
                                if (pass)
                                {
                                    // Apply paging. Endindex is -1 when paging is off
                                    if ((endIndex < 0) || ((index >= startIndex) && (index <= endIndex)))
                                    {
                                        add = true;
                                    }

                                    index++;
                                }
                            }
                        }

                        if (add)
                        {
                            // Fill in the object information
                            e.ObjectType = objectType;
                            e.ObjectID = (typeInfo.IDColumn == ObjectTypeInfo.COLUMN_NAME_UNKNOWN) ? 0 : ValidationHelper.GetInteger(dr[typeInfo.IDColumn], 0);
                            e.Field = colName;

                            result.Add(e);
                        }

                        return originalExpression;
                    },
                    new List<string> { type }
                );

                if ((maxTotalRecords != -1) && (index >= maxTotalRecords))
                {
                    // Enough data - cancel enumeration
                    throw new ActionCancelledException();
                }
            };

            using (var scope = new CMSConnectionScope())
            {
                scope.Connection.CommandTimeout = ConnectionHelper.LongRunningCommandTimeout;

                infos.ForEachRow(collectMacros);
            }

            if (((maxTotalRecords != -1) && (index >= maxTotalRecords)) || !CMSHttpContext.Current.Response.IsClientConnected)
            {
                break;
            }
        }

        totalRecords = index;
        return result;
    }


    /// <summary>
    /// Excludes the testing objects from the given infos query
    /// </summary>
    /// <param name="infos">Query</param>
    private static void ExcludeTestingObjects(ObjectQuery infos)
    {
        var typeInfo = infos.TypeInfo;

        // Add condition for testing names
        if (typeInfo.CodeNameColumn != ObjectTypeInfo.COLUMN_NAME_UNKNOWN)
        {
            infos.WhereNotStartsWith(typeInfo.CodeNameColumn, TEST_PREFIX);
        }
        if (typeInfo.DisplayNameColumn != ObjectTypeInfo.COLUMN_NAME_UNKNOWN)
        {
            infos.WhereNotStartsWith(typeInfo.DisplayNameColumn, TEST_PREFIX);
        }

        var sites = GetNonTestingObjects(SiteInfo.OBJECT_TYPE);

        // Add site condition (exclude testing site)
        if (typeInfo.SiteIDColumn != ObjectTypeInfo.COLUMN_NAME_UNKNOWN)
        {
            infos.WhereIn(typeInfo.SiteIDColumn, sites);
        }

        // Add condition for testing parent (exclude testing parents)
        var parentObjectType = typeInfo.ParentObjectType;
        if (!String.IsNullOrEmpty(parentObjectType))
        {
            var parents = GetNonTestingObjects(parentObjectType);
            var parentTypeInfo = parents.TypeInfo;

            // Add site condition for parent (exclude parents from testing site)
            if (parentTypeInfo.SiteIDColumn != ObjectTypeInfo.COLUMN_NAME_UNKNOWN)
            {
                parents.WhereIn(parentTypeInfo.SiteIDColumn, sites);
            }

            infos.WhereIn(typeInfo.ParentIDColumn, parents);
        }
    }


    /// <summary>
    /// Gets the query for testing objects of the given type
    /// </summary>
    /// <param name="objectType">Object type</param>
    private static ObjectQuery GetNonTestingObjects(string objectType)
    {
        var results = new ObjectQuery(objectType);

        var typeInfo = results.TypeInfo;
        var where = new WhereCondition();

        // Add condition for testing names
        if (typeInfo.CodeNameColumn != ObjectTypeInfo.COLUMN_NAME_UNKNOWN)
        {
            where.WhereNotStartsWith(typeInfo.CodeNameColumn, TEST_PREFIX);
        }
        if (typeInfo.DisplayNameColumn != ObjectTypeInfo.COLUMN_NAME_UNKNOWN)
        {
            where.WhereNotStartsWith(typeInfo.DisplayNameColumn, TEST_PREFIX);
        }

        results.Where(where);

        return results;
    }


    /// <summary>
    /// Gets the macro expression from the given object
    /// </summary>
    /// <param name="expression">Expression</param>
    private static MacroExpr GetMacroExpr(string expression)
    {
        var macroExpr = new MacroExpr();

        try
        {
            // Handle security
            macroExpr.Expression = MacroSecurityProcessor.RemoveMacroSecurityParams(expression, out macroExpr.SignedBy);
            macroExpr.SignatureValid = MacroSecurityProcessor.CheckMacroIntegrity(expression, macroExpr.SignedBy);

            // Parse rule text
            macroExpr.RuleText = MacroRuleTree.GetRuleText(macroExpr.Expression, true, true);
            macroExpr.Expression = MacroRuleTree.GetRuleCondition(macroExpr.Expression, true);
        }
        catch (Exception ex)
        {
            macroExpr.Error = true;
            macroExpr.ErrorMessage = ex.Message + "\r\n\r\n" + ex.StackTrace;
        }
        return macroExpr;
    }


    /// <summary>
    /// Renders the particular macro expression
    /// </summary>
    protected void RenderItem(MacroExpr expression)
    {
        var sb = new StringBuilder();

        sb.Append("<tr>");

        // Expression
        string exprTag = expression.RuleText ?? TextHelper.EnsureHTMLLineEndings(HTMLHelper.HTMLEncode(TextHelper.LimitLength(expression.Expression, 500)));

        sb.Append("<td class=\"wrap-normal\"><span class=\"MacroExpression\" title=\"", HTMLHelper.HTMLEncode(expression.Expression), "\">", exprTag, "</span></td>");

        // Syntax valid
        var errorText = UniGridFunctions.ColoredSpanYesNo(!expression.Error);
        if (!String.IsNullOrEmpty(expression.ErrorMessage))
        {
            errorText = String.Format("<span title=\"{0}\">{1}</span>", HTMLHelper.HTMLEncode(expression.ErrorMessage), errorText);
        }

        sb.Append("<td class=\"text-center\">", errorText, "</td>");

        // Signed by
        sb.Append("<td>", HTMLHelper.HTMLEncode(expression.SignedBy), "</td>");

        // Signature valid
        sb.Append("<td class=\"text-center\">", UniGridFunctions.ColoredSpanYesNo(expression.SignatureValid), "</td><td>");

        sb.Append();

        plcRows.Controls.Add(new LiteralControl(sb.ToString()));
        sb.Clear();

        // Object
        var tr = new ObjectTransformation(expression.ObjectType, expression.ObjectID)
        {
            UseEmptyInfoForObjectLimitedByLicense = true,
            Transformation = "{% Object.GetFullObjectName(true, true) %}"
        };

        plcRows.Controls.Add(tr);

        // Column
        //sb.Append(" (", expression.ObjectID, ")");
        sb.Append("</td><td>", expression.Field, "</td></tr>");

        plcRows.Controls.Add(new LiteralControl(sb.ToString()));
        sb.Clear();
    }


    #region "IUniPageable Members"

    /// <summary>
    /// Pager data item object.
    /// </summary>
    public object PagerDataItem
    {
        get
        {
            return null;
        }
        set
        {
        }
    }


    /// <summary>
    /// Pager control.
    /// </summary>
    public UniPager UniPagerControl
    {
        get;
        set;
    }


    /// <summary>
    /// Occurs when the control bind data.
    /// </summary>
    public event EventHandler<EventArgs> OnPageBinding;


    /// <summary>
    /// Occurs when the pager change the page and current mode is postback => reload data
    /// </summary>
    public event EventHandler<EventArgs> OnPageChanged;


    /// <summary>
    /// Evokes control databind.
    /// </summary>
    public void ReBind()
    {
        if (OnPageChanged != null)
        {
            OnPageChanged(this, null);
        }
    }


    /// <summary>
    /// Gets or sets the number of result. Enables proceed "fake" datasets, where number 
    /// of results in the dataset is not correspondent to the real number of results
    /// This property must be equal -1 if should be disabled
    /// </summary>
    public int PagerForceNumberOfResults
    {
        get
        {
            return mTotalItems;
        }
        set
        {
        }
    }

    #endregion
}