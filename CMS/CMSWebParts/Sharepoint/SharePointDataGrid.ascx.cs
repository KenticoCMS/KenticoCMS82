using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.Helpers;
using CMS.PortalControls;


public partial class CMSWebParts_SharePoint_SharePointDataGrid : CMSAbstractWebPart
{
    #region "SharePoint data source properties"

    /// <summary>
    /// Gets or sets value of username.
    /// </summary>
    public string Username
    {
        get
        {
            return ValidationHelper.GetString(GetValue("Username"), SPDataSource.Username);
        }
        set
        {
            SetValue("Username", value);
            SPDataSource.Username = value;
        }
    }


    /// <summary>
    /// Gets or sets value of password.
    /// </summary>
    public string Password
    {
        get
        {
            return ValidationHelper.GetString(GetValue("Password"), SPDataSource.Password);
        }
        set
        {
            SetValue("Password", value);
            SPDataSource.Password = value;
        }
    }


    /// <summary>
    /// Gets or sets value of SharePoint list name.
    /// </summary>
    public string ListName
    {
        get
        {
            return ValidationHelper.GetString(GetValue("ListName"), SPDataSource.ListName);
        }
        set
        {
            SetValue("ListName", value);
            SPDataSource.ListName = value;
        }
    }


    /// <summary>
    /// Gets or sets URL of SharePoint service (Eg. Lists.asmx, Imaging.asmx).
    /// </summary>
    public string SPServiceURL
    {
        get
        {
            return ValidationHelper.GetString(GetValue("SPServiceURL"), SPDataSource.SPServiceURL);
        }
        set
        {
            SetValue("SPServiceURL", value);
            SPDataSource.SPServiceURL = value;
        }
    }


    /// <summary>
    /// Enables or disables showing CAML on output.
    /// </summary>
    public bool ShowReturnedCAML
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ShowReturnedCAML"), SPDataSource.ShowReturnedCAML);
        }
        set
        {
            SetValue("ShowReturnedCAML", value);
            SPDataSource.ShowReturnedCAML = value;
        }
    }


    /// <summary>
    /// Gets or set the row limit.
    /// </summary>
    public int RowLimit
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("RowLimit"), SPDataSource.RowLimit);
        }
        set
        {
            SetValue("RowLimit", value);
            SPDataSource.RowLimit = value;
        }
    }


    /// <summary>
    /// Gets or sets query to specify which document should be retrieved (like where condition).
    /// </summary>
    public string Query
    {
        get
        {
            return ValidationHelper.GetString(GetValue("Query"), SPDataSource.Query);
        }
        set
        {
            SetValue("Query", value);
            SPDataSource.Query = value;
        }
    }


    /// <summary>
    /// Gets or sets document fields which should be retrieved.
    /// </summary>
    public string ViewFields
    {
        get
        {
            return ValidationHelper.GetString(GetValue("ViewFields"), SPDataSource.ViewFields);
        }
        set
        {
            SetValue("ViewFields", value);
            SPDataSource.ViewFields = value;
        }
    }


    /// <summary>
    /// Enables or disables using of classic dataset as data source for ASCX transformation.
    /// </summary>
    public bool UseClassicDataset
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("UseClassicDataset"), SPDataSource.UseClassicDataset);
        }
        set
        {
            SetValue("UseClassicDataset", value);
            SPDataSource.UseClassicDataset = value;
        }
    }


    /// <summary>
    /// Gets or sets fields which should be included in dataset
    /// Note: Only if UseClassicDataset is enabled
    /// </summary>
    public string Fields
    {
        get
        {
            return ValidationHelper.GetString(GetValue("Fields"), SPDataSource.Fields);
        }
        set
        {
            SetValue("Fields", value);
            SPDataSource.Fields = value;
        }
    }


    /// <summary>
    /// Gets or sets the mode which specifies what this webpart exactly do.
    /// </summary>
    public string Mode
    {
        get
        {
            return ValidationHelper.GetString(GetValue("Mode"), SPDataSource.Mode);
        }
        set
        {
            SetValue("Mode", value);
            SPDataSource.Mode = value;
        }
    }


    /// <summary>
    /// Gets or sets the name of field for selecting item.
    /// </summary>
    public string ItemIDField
    {
        get
        {
            return ValidationHelper.GetString(GetValue("ItemIDField"), SPDataSource.ItemIDField);
        }
        set
        {
            SetValue("ItemIDField", value);
            SPDataSource.ItemIDField = value;
        }
    }


    /// <summary>
    /// Gets or sets the type of field for selecting item.
    /// </summary>
    public string ItemIDFieldType
    {
        get
        {
            return ValidationHelper.GetString(GetValue("ItemIDFieldType"), SPDataSource.ItemIDFieldType);
        }
        set
        {
            SetValue("ItemIDFieldType", value);
            SPDataSource.ItemIDFieldType = value;
        }
    }

    #endregion


    #region "Basic data grid properties"

    /// <summary>
    /// Gets or sets HideControlForZeroRows property.
    /// </summary>
    public bool HideControlForZeroRows
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("HideControlForZeroRows"), BasicDataGrid.HideControlForZeroRows);
        }
        set
        {
            SetValue("HideControlForZeroRows", value);
            BasicDataGrid.HideControlForZeroRows = value;
        }
    }


    /// <summary>
    /// Gets or sets ZeroRowsText property.
    /// </summary>
    public string ZeroRowsText
    {
        get
        {
            return ValidationHelper.GetString(GetValue("ZeroRowsText"), BasicDataGrid.ZeroRowsText);
        }
        set
        {
            SetValue("ZeroRowsText", value);
            BasicDataGrid.ZeroRowsText = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that inidcates whether header will be displayed.
    /// </summary>
    public bool ShowHeader
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ShowHeader"), BasicDataGrid.ShowHeader);
        }
        set
        {
            SetValue("ShowHeader", value);
            BasicDataGrid.ShowHeader = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether footer will be displayed.
    /// </summary>
    public bool ShowFooter
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ShowFooter"), BasicDataGrid.ShowFooter);
        }
        set
        {
            SetValue("ShowFooter", value);
            BasicDataGrid.ShowFooter = value;
        }
    }


    /// <summary>
    /// Gets or sets the tool tip text.
    /// </summary>
    public string ToolTip
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("ToolTip"), BasicDataGrid.ToolTip);
        }
        set
        {
            SetValue("ToolTip", value);
            BasicDataGrid.ToolTip = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether first page is sets when sort of some column is changed.
    /// </summary>
    public bool SetFirstPageAfterSortChange
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("SetFirstPageAfterSortChange"), BasicDataGrid.SetFirstPageAfterSortChange);
        }
        set
        {
            SetValue("SetFirstPageAfterSortChange", value);
            BasicDataGrid.SetFirstPageAfterSortChange = value;
        }
    }


    /// <summary>
    /// Gets or sets names of columns which will be used in header.
    /// </summary>
    public string HeaderColumnsNames
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("HeaderColumnsNames"), "");
        }
        set
        {
            SetValue("HeaderColumnsNames", value);
        }
    }


    /// <summary>
    /// Gets or sets the SkinID which should be used.
    /// </summary>
    public override string SkinID
    {
        get
        {
            return base.SkinID;
        }
        set
        {
            base.SkinID = value;
            if ((BasicDataGrid != null) && (PageCycle < PageCycleEnum.Initialized))
            {
                BasicDataGrid.SkinID = SkinID;
            }
        }
    }

    #endregion


    #region "Basic data grid paging properties"

    /// <summary>
    /// Gets or sets the value that indicates whether paging will be allowed.
    /// </summary>
    public bool AllowPaging
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("AllowPaging"), BasicDataGrid.AllowPaging);
        }
        set
        {
            SetValue("AllowPaging", value);
            BasicDataGrid.AllowPaging = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether custom paging is enabled.
    /// </summary>
    public bool AllowCustomPaging
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("AllowCustomPaging"), BasicDataGrid.AllowCustomPaging);
        }
        set
        {
            SetValue("AllowCustomPaging", value);
            BasicDataGrid.AllowCustomPaging = value;
        }
    }


    /// <summary>
    /// Gets or sets the size of the page if the paging is allowed.
    /// </summary>
    public int PageSize
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("PageSize"), BasicDataGrid.PageSize);
        }
        set
        {
            SetValue("PageSize", value);
            BasicDataGrid.PageSize = value;
        }
    }


    /// <summary>
    /// Gets or sets the paging mode.
    /// </summary>
    public PagerMode PagingMode
    {
        get
        {
            return BasicDataGrid.GetPagerMode(ValidationHelper.GetString(GetValue("PagingMode"), ""));
        }
        set
        {
            SetValue("PagingMode", value);
        }
    }

    #endregion


    #region "Basic data grid sorting properties"

    /// <summary>
    /// Gets or sets the value that indicates whether sorting is ascending at default.
    /// </summary>
    public bool SortAscending
    {
        get
        {
            return (ValidationHelper.GetBoolean(GetValue("SortAscending"), BasicDataGrid.SortAscending));
        }
        set
        {
            SetValue("SortAscending", value);
            BasicDataGrid.SortAscending = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether sorting is allowed.
    /// </summary>
    public bool AllowSorting
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("AllowSorting"), BasicDataGrid.AllowSorting);
        }
        set
        {
            SetValue("AllowSorting", value);
            BasicDataGrid.AllowSorting = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that inidcates whether sorting process is proceeded in the code.
    /// </summary>
    public bool ProcessSorting
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ProcessSorting"), BasicDataGrid.ProcessSorting);
        }
        set
        {
            SetValue("ProcessSorting", value);
            BasicDataGrid.ProcessSorting = value;
        }
    }


    /// <summary>
    /// Gets or sets the default sort field.
    /// </summary>
    public string SortField
    {
        get
        {
            return DataHelper.GetNotEmpty(ValidationHelper.GetString(GetValue("SortField"), BasicDataGrid.SortField), BasicDataGrid.SortField);
        }
        set
        {
            SetValue("SortField", value);
            BasicDataGrid.SortField = value;
        }
    }

    #endregion


    #region "Overridden properties"

    /// <summary>
    /// Gets or sets the cache item name.
    /// </summary>
    public override string CacheItemName
    {
        get
        {
            return base.CacheItemName;
        }
        set
        {
            base.CacheItemName = value;
            SPDataSource.CacheItemName = value;
        }
    }


    /// <summary>
    /// Cache dependencies, each cache dependency on a new line.
    /// </summary>
    public override string CacheDependencies
    {
        get
        {
            return ValidationHelper.GetString(base.CacheDependencies, SPDataSource.CacheDependencies);
        }
        set
        {
            base.CacheDependencies = value;
            SPDataSource.CacheDependencies = value;
        }
    }


    /// <summary>
    /// Gets or sets the cache minutes.
    /// </summary>
    public override int CacheMinutes
    {
        get
        {
            return base.CacheMinutes;
        }
        set
        {
            base.CacheMinutes = value;
            SPDataSource.CacheMinutes = value;
        }
    }


    /// <summary>
    /// Indicates if the control should perform the operations.
    /// </summary>
    public override bool StopProcessing
    {
        get
        {
            return base.StopProcessing;
        }
        set
        {
            base.StopProcessing = value;
            SPDataSource.StopProcessing = value;
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Content loaded event handler.
    /// </summary>
    public override void OnContentLoaded()
    {
        base.OnContentLoaded();
        SetupControl();
    }


    /// <summary>
    /// Initializes the control properties.
    /// </summary>
    protected void SetupControl()
    {
        if (StopProcessing)
        {
            // Do nothing
        }
        else
        {
            // Classic repeater+dataset
            if (UseClassicDataset)
            {
                // Set SharePoint datasource properties
                SPDataSource.Username = Username;
                SPDataSource.Password = EncryptionHelper.DecryptData(Password);
                SPDataSource.ListName = ListName;
                SPDataSource.SPServiceURL = SPServiceURL;
                SPDataSource.ShowReturnedCAML = ShowReturnedCAML;
                SPDataSource.RowLimit = RowLimit;
                SPDataSource.Query = Query;
                SPDataSource.ViewFields = ViewFields;
                SPDataSource.UseClassicDataset = UseClassicDataset;
                SPDataSource.Fields = Fields;
                SPDataSource.Mode = Mode;
                SPDataSource.ItemIDField = ItemIDField;
                SPDataSource.ItemIDFieldType = ItemIDFieldType;
                SPDataSource.CacheItemName = CacheItemName;
                SPDataSource.CacheDependencies = CacheDependencies;
                SPDataSource.CacheMinutes = CacheMinutes;

                // Set basic datagrid properties
                BasicDataGrid.DataBindByDefault = false;
                BasicDataGrid.EnableViewState = true;

                BasicDataGrid.HideControlForZeroRows = HideControlForZeroRows;
                BasicDataGrid.ZeroRowsText = ZeroRowsText;

                if (!StandAlone && (PageCycle < PageCycleEnum.Initialized) && (ValidationHelper.GetString(Page.StyleSheetTheme, "") == ""))
                {
                    BasicDataGrid.SkinID = SkinID;
                }

                BasicDataGrid.SortAscending = SortAscending;
                BasicDataGrid.AllowSorting = AllowSorting;
                BasicDataGrid.ProcessSorting = ProcessSorting;
                BasicDataGrid.SortField = SortField;

                BasicDataGrid.AllowPaging = AllowPaging;
                BasicDataGrid.AllowCustomPaging = AllowCustomPaging;
                BasicDataGrid.PageSize = PageSize;
                BasicDataGrid.PagerStyle.Mode = PagingMode;

                BasicDataGrid.ShowHeader = ShowHeader;
                BasicDataGrid.ShowFooter = ShowFooter;
                BasicDataGrid.ToolTip = ToolTip;
                BasicDataGrid.SetFirstPageAfterSortChange = SetFirstPageAfterSortChange;

                // Load and bind data
                LoadData();
            }
            else
            {
                throw new Exception("XSLT transformation not supported in this webpart. Use SharePoint repeater.");
            }
        }
    }


    /// <summary>
    /// Reload data.
    /// </summary>
    public override void ReloadData()
    {
        base.ReloadData();
        SetupControl();
        BindData();
    }


    /// <summary>
    /// OnPreRender override.
    /// </summary>
    protected override void OnPreRender(EventArgs e)
    {
        if (!string.IsNullOrEmpty(SPDataSource.ErrorMessage))
        {
            BasicDataGrid.Visible = false;
            DisplayError(SPDataSource.ErrorMessage);
        }
        else
        {
            //DataSet
            if (UseClassicDataset && HideControlForZeroRows && !SPDataSource.HasData)
            {
                Visible = false;
            }
        }

        base.OnPreRender(e);
    }


    /// <summary>
    /// OnLoad override.
    /// </summary>    
    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        BindData();
    }


    /// <summary>
    /// Loads SharePoint data.
    /// </summary>
    protected void LoadData()
    {
        // Connect data grid with data source                    
        BasicDataGrid.DataSource = SPDataSource.DataSource;
    }


    /// <summary>
    /// Binds data to BasicDataGrid.
    /// </summary>
    protected void BindData()
    {
        // Create columns
        InitColumns(Fields, HeaderColumnsNames);

        if (!DataHelper.DataSourceIsEmpty(BasicDataGrid.DataSource))
        {
            try
            {
                BasicDataGrid.DataBind();
            }
            catch (Exception ex)
            {
                DisplayError(string.Format(GetString("sharepoint.DataBindError"), ex.Message));
            }
        }
    }


    /// <summary>
    /// Bounds grid columns.
    /// </summary>
    /// <param name="columns">Input string with column name separated by semicolon</param>    
    /// <param name="headerNames">Header columns names</param>
    protected void InitColumns(string columns, string headerNames)
    {
        // There are already some bounded columns
        if (BasicDataGrid.Columns.Count > 0)
        {
            return;
        }

        if (String.IsNullOrEmpty(columns))
        {
            BasicDataGrid.AutoGenerateColumns = true;
            return;
        }

        // Custom columns will be used
        BasicDataGrid.AutoGenerateColumns = false;

        string[] cols = columns.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
        string[] headerCols = headerNames.Split(new char[] { ';' });

        int i = 0;
        foreach (string col in cols)
        {
            string currentCol = col.Trim();
            if (!string.IsNullOrEmpty(currentCol))
            {
                BoundColumn column = new BoundColumn();
                column.DataField = currentCol;

                // Set header
                if ((headerCols.Length > i) && !String.IsNullOrEmpty(headerCols[i]))
                {
                    column.HeaderText = ResHelper.LocalizeString(headerCols[i]);
                }
                else
                {
                    // Use dataset column name
                    column.HeaderText = col;
                }

                if (BasicDataGrid.AllowSorting)
                {
                    column.SortExpression = currentCol;
                }

                BasicDataGrid.Columns.Add(column);
                i++;
            }
        }
    }


    /// <summary>
    /// Applies given stylesheet skin.
    /// </summary>
    public override void ApplyStyleSheetSkin(Page page)
    {
        BasicDataGrid.SkinID = SkinID;
        base.ApplyStyleSheetSkin(page);
    }


    /// <summary>
    /// Shows error with message.
    /// </summary>
    /// <param name="message">Message</param>
    private void DisplayError(string message)
    {
        Label lblError = new Label();
        lblError.Text = message;
        lblError.CssClass = "ErrorLabel";

        Controls.Add(lblError);
    }

    #endregion
}