using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.DataEngine;
using CMS.FormControls;
using CMS.FormEngine;
using CMS.Helpers;
using CMS.SiteProvider;

public partial class CMSModules_Content_Controls_Filters_DocumentCultureFilter : FormEngineUserControl
{
    #region "Variables"

    private DataSet mSiteCultures;
    private string mDefaultSiteCulture;
    private string currentSiteName;
    private string mOperatorFieldName;

    #endregion


    #region "Properties"

    /// <summary>
    /// Gets or sets value of culture selector.
    /// </summary>
    public override object Value
    {
        get
        {
            return cultureElem.Value;
        }
        set
        {
            cultureElem.Value = value;
        }
    }


    /// <summary>
    /// Default culture of the site.
    /// </summary>
    private string DefaultSiteCulture
    {
        get
        {
            return mDefaultSiteCulture ?? (mDefaultSiteCulture = CultureHelper.GetDefaultCultureCode(currentSiteName));
        }
    }


    /// <summary>
    /// Gets name of the field for operator value. Default value is 'Operator'.
    /// </summary>
    protected string OperatorFieldName
    {
        get
        {
            if (string.IsNullOrEmpty(mOperatorFieldName))
            {
                // Get name of the field for operator value
                mOperatorFieldName = DataHelper.GetNotEmpty(GetValue("OperatorFieldName"), Field + "Operator");
            }

            return mOperatorFieldName;
        }
    }


    /// <summary>
    /// Site cultures.
    /// </summary>
    private DataSet SiteCultures
    {
        get
        {
            if (mSiteCultures == null)
            {
                mSiteCultures = CultureSiteInfoProvider.GetSiteCultures(currentSiteName).Copy();
                if (!DataHelper.DataSourceIsEmpty(mSiteCultures))
                {
                    DataTable cultureTable = mSiteCultures.Tables[0];
                    DataRow[] defaultCultureRow = cultureTable.Select("CultureCode='" + DefaultSiteCulture + "'");

                    // Ensure default culture to be first
                    DataRow dr = cultureTable.NewRow();
                    if (defaultCultureRow.Length > 0)
                    {
                        dr.ItemArray = defaultCultureRow[0].ItemArray;
                        cultureTable.Rows.InsertAt(dr, 0);
                        cultureTable.Rows.Remove(defaultCultureRow[0]);
                    }
                }
            }
            return mSiteCultures;
        }
    }
    
    #endregion


    #region "Page events"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        currentSiteName = SiteContext.CurrentSiteName;
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        // Fill drop down lists
        InitDropdownLists();
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Initializes drop down lists.
    /// </summary>
    private void InitDropdownLists()
    {
        // Init cultures
        cultureElem.AdditionalDropDownCSSClass = "FilterSelectorDropDown";
        cultureElem.AllowDefault = false;
        cultureElem.UpdatePanel.RenderMode = UpdatePanelRenderMode.Inline;

        var specialFields = cultureElem.UniSelector.SpecialFields;

        specialFields.Add(new SpecialField { Text = GetString("transman.anyculture"), Value = "##ANY##" });
        specialFields.Add(new SpecialField { Text = GetString("transman.allcultures"), Value = "##ALL##" });
        
        // Init operands
        var items = drpLanguage.Items;
        if (items.Count == 0)
        {
            items.Add(new ListItem(GetString("transman.translatedto"), "="));
            items.Add(new ListItem(GetString("transman.nottranslatedto"), "<>"));
        }

        LoadOtherValues();
    }


    /// <summary>
    /// Returns other values related to this form control.
    /// </summary>
    /// <returns>Returns an array where first dimension is attribute name and the second dimension is its value.</returns>
    public override object[,] GetOtherValues()
    {
        if (Form.Data is DataRowContainer)
        {
            if (!ContainsColumn(OperatorFieldName))
            {
                Form.DataRow.Table.Columns.Add(OperatorFieldName);
            }
        }

        // Set properties names
        object[,] values = new object[3, 2];

        values[0, 0] = OperatorFieldName;
        values[0, 1] = drpLanguage.SelectedValue;

        return values;
    }


    /// <summary>
    /// Loads the other fields values to the state of the form control
    /// </summary>
    public override void LoadOtherValues()
    {
        drpLanguage.SelectedValue = ValidationHelper.GetString(GetColumnValue(OperatorFieldName), "=");
    }


    /// <summary>
    /// Creates where condition according to values selected in filter.
    /// </summary>
    public override string GetWhereCondition()
    {
        string where = string.Empty;

        string val = ValidationHelper.GetString(cultureElem.Value, string.Empty);
        if (val == string.Empty)
        {
            val = "##ANY##";
        }

        if (val != "##ANY##")
        {
            switch (val)
            {
                case "##ALL##":
                    where = SqlHelper.AddWhereCondition(where, "((SELECT COUNT(*) FROM View_CMS_Tree_Joined AS TreeView WHERE TreeView.NodeID = View_CMS_Tree_Joined_Versions.NodeID) " + SqlHelper.EscapeQuotes(drpLanguage.SelectedValue) + " " + SiteCultures.Tables[0].Rows.Count + ")");
                    break;

                default:
                    string oper = (drpLanguage.SelectedValue == "<>") ? "NOT" : "";
                    where = SqlHelper.AddWhereCondition(where, "NodeID " + oper + " IN (SELECT NodeID FROM View_CMS_Tree_Joined AS TreeView WHERE TreeView.NodeID = NodeID AND DocumentCulture = '" + SqlHelper.EscapeQuotes(val) + "')");
                    break;
            }
        }
        else if (drpLanguage.SelectedValue == "<>")
        {
            where = SqlHelper.NO_DATA_WHERE;
        }

        return where;
    }
    
    #endregion
}
