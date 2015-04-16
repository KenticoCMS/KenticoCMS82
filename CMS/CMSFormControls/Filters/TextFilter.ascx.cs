using System;

using CMS.DataEngine;
using CMS.EventLog;
using CMS.ExtendedControls;
using CMS.FormControls;
using CMS.Helpers;
using CMS.Base;
using CMS.UIControls;

public partial class CMSFormControls_Filters_TextFilter : FormEngineUserControl
{
    protected string mOperatorFieldName = null;


    #region "Enumerations"

    #endregion


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
                mOperatorFieldName = DataHelper.GetNotEmpty(GetValue("OperatorFieldName"), Field + "Operator");
            }

            return mOperatorFieldName;
        }
    }

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        CheckFieldEmptiness = false;
        InitFilterDropDown();

        LoadOtherValues();
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        // Disable text field for 'Empty' and 'Not Empty' values
        TextCompareOperatorEnum op = (TextCompareOperatorEnum)Enum.Parse(typeof(TextCompareOperatorEnum), drpOperator.SelectedValue);
        txtText.Enabled = (op != TextCompareOperatorEnum.Empty) && (op != TextCompareOperatorEnum.NotEmpty);
        if (!txtText.Enabled)
        {
            txtText.Text = String.Empty;
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
    /// Loads the other fields values to the state of the form control
    /// </summary>
    public override void LoadOtherValues()
    {
        drpOperator.SelectedValue = ValidationHelper.GetString(GetColumnValue(OperatorFieldName), "0");
    }


    /// <summary>
    /// Initializes operator filter dropdown list.
    /// </summary>
    private void InitFilterDropDown()
    {
        if (drpOperator.Items.Count == 0)
        {
            ControlsHelper.FillListControlWithEnum<TextCompareOperatorEnum>(drpOperator, "filter");
        }
    }


    /// <summary>
    /// Gets where condition.
    /// </summary>
    public override string GetWhereCondition()
    {
        // Do not trim value for text filter
        string tempVal = ValidationHelper.GetString(Value, string.Empty);
        string textOp = null;

        TextCompareOperatorEnum op = (TextCompareOperatorEnum)Enum.Parse(typeof(TextCompareOperatorEnum), drpOperator.SelectedValue);

        if (!string.IsNullOrEmpty(tempVal))
        {
            tempVal = SqlHelper.EscapeQuotes(tempVal);
        }
        else if (op != TextCompareOperatorEnum.Empty && op != TextCompareOperatorEnum.NotEmpty)
        {
            // Value isn't set (doesn't have to be for empty and not empty)
            return null;
        }

        switch (op)
        {
            case TextCompareOperatorEnum.Like:
                textOp = WhereBuilder.LIKE;
                tempVal = "N'%" + SqlHelper.EscapeLikeText(tempVal) + "%'";
                break;

            case TextCompareOperatorEnum.NotLike:
                textOp = WhereBuilder.NOT_LIKE;
                tempVal = "N'%" + SqlHelper.EscapeLikeText(tempVal) + "%'";
                break;

            case TextCompareOperatorEnum.StartsWith:
                textOp = WhereBuilder.LIKE;
                tempVal = "N'" + SqlHelper.EscapeLikeText(tempVal) + "%'";
                break;

            case TextCompareOperatorEnum.NotStartsWith:
                textOp = WhereBuilder.NOT_LIKE;
                tempVal = "N'" + SqlHelper.EscapeLikeText(tempVal) + "%'";
                break;

            case TextCompareOperatorEnum.EndsWith:
                textOp = WhereBuilder.LIKE;
                tempVal = "N'%" + SqlHelper.EscapeLikeText(tempVal) + "'";
                break;

            case TextCompareOperatorEnum.NotEndsWith:
                textOp = WhereBuilder.NOT_LIKE;
                tempVal = "N'%" + SqlHelper.EscapeLikeText(tempVal) + "'";
                break;

            case TextCompareOperatorEnum.Equals:
                textOp = WhereBuilder.EQUAL;
                tempVal = "N'" + tempVal + "'";
                break;

            case TextCompareOperatorEnum.NotEquals:
                textOp = WhereBuilder.NOT_EQUAL;
                tempVal = "N'" + tempVal + "'";
                break;

            case TextCompareOperatorEnum.Empty:
                return string.Format("[{0}] IS NULL OR [{0}] = ''", FieldInfo.Name);

            case TextCompareOperatorEnum.NotEmpty:
                return string.Format("[{0}] IS NOT NULL AND [{0}] <> ''", FieldInfo.Name);

            case TextCompareOperatorEnum.LessThan:
                if (ValidationHelper.IsDouble(tempVal))
                {
                    return string.Format("CASE ISNUMERIC([{0}]) WHEN 1 THEN CAST([{0}] AS FLOAT) ELSE NULL END < CAST({1} AS FLOAT)", FieldInfo.Name, ValidationHelper.GetDouble(tempVal, 0, CultureHelper.EnglishCulture.Name));
                }
                return SqlHelper.NO_DATA_WHERE;

            case TextCompareOperatorEnum.GreaterThan:
                if (ValidationHelper.IsDouble(tempVal))
                {
                    return string.Format("CASE ISNUMERIC([{0}]) WHEN 1 THEN CAST([{0}] AS FLOAT) ELSE NULL END > CAST({1} AS FLOAT)", FieldInfo.Name, ValidationHelper.GetDouble(tempVal, 0, CultureHelper.EnglishCulture.Name));
                }
                return SqlHelper.NO_DATA_WHERE;
        }


        if (String.IsNullOrEmpty(WhereConditionFormat))
        {
            WhereConditionFormat = "ISNULL([{0}], '') {2} {1}";
        }

        try
        {
            // Format where condition
            return string.Format(WhereConditionFormat, FieldInfo.Name, tempVal, textOp);
        }
        catch (Exception ex)
        {
            // Log exception
            EventLogProvider.LogException("TextFilter", "GetWhereCondition", ex);

            ShowError("Failed to generate the filter where condition. See event log for more details. Event name 'TextFilter'.");
        }

        return null;
    }

    #endregion
}