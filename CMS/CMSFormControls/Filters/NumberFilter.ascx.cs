using System;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.DataEngine;
using CMS.EventLog;
using CMS.FormControls;
using CMS.Helpers;

public partial class CMSFormControls_Filters_NumberFilter : FormEngineUserControl
{
    protected string mOperatorFieldName = null;


    #region "Properties"

    /// <summary>
    /// Gets or sets value.
    /// </summary>
    public override object Value
    {
        get
        {
            return txtText.Text;
        }
        set
        {
            // Load default value on insert
            if ((value != null) && (FieldInfo != null))
            {
                switch (FieldInfo.DataType)
                {
                    case FieldDataType.Double:
                    case FieldDataType.Decimal:
                        {
                            // Convert double and decimal to proper values
                            Double dblVal = ValidationHelper.GetDoubleSystem(value, Double.NaN);
                            txtText.Text = !Double.IsNaN(dblVal) ? dblVal.ToString() : String.Empty;
                        }
                        return;
                    case FieldDataType.TimeSpan:
                        {
                            // Convert timespan to proper value
                            TimeSpan spanVal = ValidationHelper.GetTimeSpanSystem(value, TimeSpan.MinValue);
                            txtText.Text = (spanVal != TimeSpan.MinValue) ? spanVal.ToString() : String.Empty;
                        }
                        return;
                }
            }

            txtText.Text = ValidationHelper.GetString(value, null);
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
                mOperatorFieldName = DataHelper.GetNotEmpty(GetValue("OperatorFieldName"), "Operator");
            }
            return mOperatorFieldName;
        }
    }


    /// <summary>
    /// Gets or sets default operator to use for the first initialization of the control.
    /// </summary>
    public string DefaultOperator
    {
        get;
        set;
    }

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        CheckFieldEmptiness = false;
        InitFilterDropDown();

        if (ContainsColumn(OperatorFieldName))
        {
            LoadOtherValues();
        }
        else
        {
            // Set default operator
            if (!RequestHelper.IsPostBack() && (DefaultOperator != null))
            {
                drpOperator.SelectedValue = DefaultOperator;
            }
        }
    }


    /// <summary>
    /// Loads the other fields values to the state of the form control
    /// </summary>
    public override void LoadOtherValues()
    {
        if (ContainsColumn(OperatorFieldName))
        {
            drpOperator.SelectedValue = ValidationHelper.GetString(Form.Data.GetValue(OperatorFieldName), "0");
        }
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
        values[0, 1] = drpOperator.SelectedValue;

        return values;
    }


    /// <summary>
    /// Initializes operator filter dropdown list.
    /// </summary>
    private void InitFilterDropDown()
    {
        ListItemCollection items = drpOperator.Items;
        if (items.Count == 0)
        {
            items.Add(new ListItem(GetString("filter.equals"), "="));
            items.Add(new ListItem(GetString("filter.notequals"), "<>"));
            items.Add(new ListItem(GetString("filter.lessthan"), "<"));
            items.Add(new ListItem(GetString("filter.lessorequal"), "<="));
            items.Add(new ListItem(GetString("filter.greaterthan"), ">"));
            items.Add(new ListItem(GetString("filter.greaterorequal"), ">="));
        }
    }


    /// <summary>
    /// Gets where condition.
    /// </summary>
    public override string GetWhereCondition()
    {
        var value = ValidationHelper.GetString(Value, String.Empty);
        string op = drpOperator.SelectedValue;
        bool isTimeSpan = false;

        // No condition
        if (String.IsNullOrWhiteSpace(value) || String.IsNullOrEmpty(op) || !(ValidationHelper.IsDouble(value) || (isTimeSpan = ValidationHelper.IsTimeSpan(value))))
        {
            return null;
        }

        if (String.IsNullOrEmpty(WhereConditionFormat))
        {
            WhereConditionFormat = !isTimeSpan ? "[{0}] {2} {1}" : "[{0}] {2} '{1}'";
        }

        try
        {
            // Format where condition
            return String.Format(WhereConditionFormat, FieldInfo.Name, DataHelper.ConvertValueToDefaultCulture(value, !isTimeSpan ? typeof(double) : typeof(TimeSpan)), op);
        }
        catch (Exception ex)
        {
            // Log exception
            EventLogProvider.LogException("NumberFilter", "GetWhereCondition", ex);
        }

        return null;
    }

    #endregion
}