using System;
using System.Data;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;

using CMS.CMSImportExport;
using CMS.Controls;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.PortalEngine;
using CMS.Base;
using CMS.UIControls;

public partial class CMSModules_ImportExport_Controls_ExportGridView : ImportExportGridView, IUniPageable
{
    #region "Variables"

    protected string codeNameColumnName = "";
    protected string displayNameColumnName = "";
    protected int pagerForceNumberOfResults = -1;

    private SiteExportSettings mSettings = null;

    #endregion


    #region "Public properties"

    /// <summary>
    /// Export settings.
    /// </summary>
    public SiteExportSettings Settings
    {
        get
        {
            return mSettings;
        }
        set
        {
            mSettings = value;
        }
    }


    /// <summary>
    /// Gets current page size from pager.
    /// </summary>
    protected int CurrentPageSize
    {
        get
        {
            return pagerElem.CurrentPageSize;
        }
    }


    /// <summary>
    /// Gets current offset.
    /// </summary>
    protected int CurrentOffset
    {
        get
        {
            return CurrentPageSize * (pagerElem.CurrentPage - 1);
        }
    }


    /// <summary>
    /// Pager control.
    /// </summary>
    public UIPager PagerControl
    {
        get
        {
            return pagerElem;
        }
    }

    #endregion


    #region "Protected methods"

    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        // Render the available task IDs
        hdnAvailableItems.Value = AvailableItems.ToString();
    }


    protected string GetName(object codeNameObj, object displayNameObj)
    {
        string codeName = ValidationHelper.GetString(codeNameObj, "");
        string displayName = ValidationHelper.GetString(displayNameObj, "");

        if (string.IsNullOrEmpty(displayName))
        {
            return codeName;
        }

        return ResHelper.LocalizeString(displayName);
    }


    protected void btnAll_Click(object sender, EventArgs e)
    {
        // Load all selection
        DateTime originalTS = Settings.TimeStamp;

        Settings.TimeStamp = DateTimeHelper.ZERO_TIME;
        Settings.LoadDefaultSelection(ObjectType, SiteObject, ExportTypeEnum.All, true, false);
        Settings.TimeStamp = originalTS;

        RaiseButtonPressed(sender, e);
    }


    protected void btnNone_Click(object sender, EventArgs e)
    {
        // Load none selection
        Settings.LoadDefaultSelection(ObjectType, SiteObject, ExportTypeEnum.None, true, false);

        RaiseButtonPressed(sender, e);
    }


    protected void btnDefault_Click(object sender, EventArgs e)
    {
        // Load default selection
        Settings.LoadDefaultSelection(ObjectType, SiteObject, ExportTypeEnum.Default, true, false);

        RaiseButtonPressed(sender, e);
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        pagerElem.PagedControl = this;

        if (RequestHelper.IsPostBack())
        {
            if (Settings != null)
            {
                // Process the results of the available tasks
                string[] available = hdnAvailableItems.Value.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                if (available != null)
                {
                    foreach (string codeName in available)
                    {
                        string name = Request.Form.AllKeys.FirstOrDefault(x => x.EndsWith(GetCheckBoxName(codeName))) ?? string.Empty;

                        if (Request.Form[name] == null)
                        {
                            // Unchecked
                            Settings.Deselect(ObjectType, codeName, SiteObject);
                        }
                        else
                        {
                            // Checked
                            Settings.Select(ObjectType, codeName, SiteObject);
                        }
                    }
                }
            }
        }
    }

    #endregion


    #region "Public methods"

    /// <summary>
    /// Bind the data.
    /// </summary>
    public void Bind()
    {
        if (!string.IsNullOrEmpty(ObjectType))
        {
            pnlGrid.Visible = true;

            // Initialize strings
            btnAll.Text = GetString("ImportExport.All");
            btnNone.Text = GetString("export.none");
            btnDefault.Text = GetString("General.Default");

            // Get object info
            GeneralizedInfo info = ModuleManager.GetReadOnlyObject(ObjectType);
            if (info != null)
            {
                gvObjects.RowDataBound += gvObjects_RowDataBound;
                plcGrid.Visible = true;
                codeNameColumnName = info.CodeNameColumn;
                displayNameColumnName = info.DisplayNameColumn;

                // Get data source
                string where = GenerateWhereCondition();
                string orderBy = GetOrderByExpression(info);

                // Prepare the columns
                string columns = null;
                if (info.CodeNameColumn != ObjectTypeInfo.COLUMN_NAME_UNKNOWN)
                {
                    columns += info.CodeNameColumn;
                }

                if ((info.DisplayNameColumn.ToLowerCSafe() != info.CodeNameColumn.ToLowerCSafe()) && (info.DisplayNameColumn != ObjectTypeInfo.COLUMN_NAME_UNKNOWN))
                {
                    if (columns != null)
                    {
                        columns += ", ";
                    }
                    columns += info.DisplayNameColumn;
                }

                // Get the data query
                var q = info.GetDataQuery(
                    true,
                    s => s
                        .Where(where)
                        .OrderBy(orderBy)
                        .Columns(columns),
                    false
                );

                q.IncludeBinaryData = false;
                q.Offset = CurrentOffset;
                q.MaxRecords = CurrentPageSize;

                // Get the data
                DataSet ds = q.Result;
                pagerForceNumberOfResults = q.TotalRecords;

                // Set correct ID for direct page contol
                pagerElem.DirectPageControlID = ((float)pagerForceNumberOfResults / pagerElem.CurrentPageSize > 20.0f) ? "txtPage" : "drpPage";

                // Call page binding event
                if (OnPageBinding != null)
                {
                    OnPageBinding(this, null);
                }

                // Prepare checkbox field
                TemplateField checkBoxField = (TemplateField)gvObjects.Columns[0];
                checkBoxField.HeaderText = GetString("General.Export");

                // Prepare display name field
                TemplateField nameField = (TemplateField)gvObjects.Columns[1];
                nameField.HeaderText = GetString("general.displayname");

                // Load data
                if (!DataHelper.DataSourceIsEmpty(ds))
                {
                    plcObjects.Visible = true;
                    lblNoData.Visible = false;
                    gvObjects.DataSource = ds;
                    gvObjects.DataBind();
                }
                else
                {
                    plcObjects.Visible = false;
                    lblNoData.Visible = true;
                    lblNoData.Text = String.Format(GetString("ExportGridView.NoData"), GetString("objecttype." + ObjectType.Replace(".", "_").Replace("#", "_")));
                }
            }
            else
            {
                plcGrid.Visible = false;
            }
        }
        else
        {
            pnlGrid.Visible = false;
            gvObjects.DataSource = null;
            gvObjects.DataBind();
        }
    }


    /// <summary>
    /// Ensure objects preselection.
    /// </summary>
    /// <param name="codeName">Code name</param>
    public override bool IsChecked(object codeName)
    {
        string name = ValidationHelper.GetString(codeName, "");
        if (Settings.IsSelected(ObjectType, name, SiteObject))
        {
            return true;
        }
        return false;
    }

    #endregion


    #region "Private methods"

    /// <summary>
    /// On RowDataBound add CMSCheckbox to the row.
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="e">Events</param>
    private void gvObjects_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            var row = (DataRowView)e.Row.DataItem;
            string codeName = ValidationHelper.GetString(row[codeNameColumnName], "");

            AddAvailableItem(codeName);
            e.Row.Cells[0].Controls.Add(GetCheckBox(codeName));
        }
    }


    // Genearate where condition
    private string GenerateWhereCondition()
    {
        return Settings.GetObjectWhereCondition(ObjectType, SiteObject);
    }


    // Get orderby expression
    private static string GetOrderByExpression(GeneralizedInfo info)
    {
        switch (info.TypeInfo.ObjectType)
        {
            case PageTemplateInfo.OBJECT_TYPE:
                return "PageTemplateIsReusable DESC," + info.DisplayNameColumn;

            default:
                {
                    if (info.DisplayNameColumn != ObjectTypeInfo.COLUMN_NAME_UNKNOWN)
                    {
                        return info.DisplayNameColumn;
                    }

                    return info.CodeNameColumn;
                }
        }
    }

    #endregion


    #region "IUniPageable Members"


    /// <summary>
    /// Pager data item.
    /// </summary>
    public object PagerDataItem
    {
        get
        {
            return gvObjects.DataSource;
        }
        set
        {
            gvObjects.DataSource = value;
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


    public int PagerForceNumberOfResults
    {
        get
        {
            return pagerForceNumberOfResults;
        }
        set
        {
            pagerForceNumberOfResults = value;
        }
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
    public virtual void ReBind()
    {
        if (OnPageChanged != null)
        {
            OnPageChanged(this, null);
        }
    }
    
    #endregion
}