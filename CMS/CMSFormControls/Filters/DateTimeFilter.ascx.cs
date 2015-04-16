using System;

using CMS.Base;
using CMS.DataEngine;
using CMS.ExtendedControls;
using CMS.FormControls;
using CMS.FormEngine;
using CMS.Globalization;
using CMS.Helpers;

public partial class CMSFormControls_Filters_DateTimeFilter : FormEngineUserControl
{
    protected string mSecondDateFieldName;


    #region "Properties"

    /// <summary>
    /// Gets or sets value.
    /// </summary>
    public override object Value
    {
        get
        {
            if (dtmTimeFrom.SelectedDateTime == DateTimeHelper.ZERO_TIME)
            {
                return null;
            }
            else
            {
                return dtmTimeFrom.SelectedDateTime;
            }
        }
        set
        {
            if (GetValue("timezonetype") != null)
            {
                dtmTimeFrom.TimeZone = (TimeZoneTypeEnum)GetValue<string>("timezonetype", String.Empty).ToEnum<TimeZoneTypeEnum>();
            }
            if (GetValue("timezone") != null)
            {
                dtmTimeFrom.CustomTimeZone = TimeZoneInfoProvider.GetTimeZoneInfo(GetValue<string>("timezone", ""));
            }

            string strValue = ValidationHelper.GetString(value, "");

            if (DateTimeHelper.IsNowOrToday(strValue))
            {
                dtmTimeFrom.SelectedDateTime = DateTime.Now;
            }
            else
            {
                dtmTimeFrom.SelectedDateTime = ValidationHelper.GetDateTimeSystem(value, DateTimeHelper.ZERO_TIME);
            }
        }
    }


    /// <summary>
    /// Gets or sets if calendar control enables to edit time.
    /// </summary>
    public bool EditTime
    {
        get
        {
            return dtmTimeFrom.EditTime;
        }
        set
        {
            dtmTimeFrom.EditTime = value;
            dtmTimeTo.EditTime = value;
        }
    }


    /// <summary>
    /// Gets name of the field for second date value. Default value is 'SecondDatetime'.
    /// </summary>
    protected string SecondDateFieldName
    {
        get
        {
            if (string.IsNullOrEmpty(mSecondDateFieldName))
            {
                // Get name of the field for second date value
                mSecondDateFieldName = DataHelper.GetNotEmpty(GetValue("SecondDateFieldName"), "SecondDatetime");
            }
            return mSecondDateFieldName;
        }
    }

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Setup control
        if (FieldInfo != null)
        {
            dtmTimeFrom.AllowEmptyValue = FieldInfo.AllowEmpty;
            dtmTimeTo.AllowEmptyValue = FieldInfo.AllowEmpty;
        }
        dtmTimeFrom.DisplayNow = ValidationHelper.GetBoolean(GetValue("displaynow"), true);
        dtmTimeTo.DisplayNow = ValidationHelper.GetBoolean(GetValue("displaynow"), true);
        dtmTimeFrom.EditTime = ValidationHelper.GetBoolean(GetValue("edittime"), EditTime);
        dtmTimeTo.EditTime = ValidationHelper.GetBoolean(GetValue("edittime"), EditTime);
        dtmTimeFrom.SupportFolder = "~/CMSAdminControls/Calendar";
        dtmTimeTo.SupportFolder = "~/CMSAdminControls/Calendar";
        dtmTimeFrom.DateTimeTextBox.AddCssClass("EditingFormCalendarTextBox");
        dtmTimeTo.DateTimeTextBox.AddCssClass("EditingFormCalendarTextBox");
        dtmTimeFrom.IsLiveSite = IsLiveSite;
        dtmTimeTo.IsLiveSite = IsLiveSite;

        if (!String.IsNullOrEmpty(CssClass))
        {
            dtmTimeFrom.AddCssClass(CssClass);
            dtmTimeTo.AddCssClass(CssClass);
            CssClass = null;
        }
        if (!String.IsNullOrEmpty(ControlStyle))
        {
            dtmTimeFrom.Attributes.Add("style", ControlStyle);
            dtmTimeTo.Attributes.Add("style", ControlStyle);
            ControlStyle = null;
        }

        CheckFieldEmptiness = false;

        LoadOtherValues();
    }


    /// <summary>
    /// Loads the other fields values to the state of the form control
    /// </summary>
    public override void LoadOtherValues()
    {
        // User defined extensions
        if (ContainsColumn(SecondDateFieldName))
        {
            dtmTimeTo.SelectedDateTime = ValidationHelper.GetDateTime(Form.Data.GetValue(SecondDateFieldName), DateTimeHelper.ZERO_TIME, "en-us");
        }
    }


    /// <summary>
    /// Returns true if user control is valid.
    /// </summary>
    public override bool IsValid()
    {
        // Check value
        string strValueFrom = dtmTimeFrom.DateTimeTextBox.Text.Trim();
        string strValueTo = dtmTimeTo.DateTimeTextBox.Text.Trim();
        bool required = (FieldInfo != null) && !FieldInfo.AllowEmpty;
        bool checkEmptiness = (Form == null) || Form.CheckFieldEmptiness;

        if (required && checkEmptiness && (String.IsNullOrEmpty(strValueFrom) && String.IsNullOrEmpty(strValueTo)))
        {
            // Empty error
            if (ErrorMessage != null)
            {
                if (ErrorMessage != ResHelper.GetString("BasicForm.InvalidInput"))
                {
                    ValidationError = ErrorMessage;
                }
                else
                {
                    ValidationError += ResHelper.GetString("BasicForm.ErrorEmptyValue");
                }
            }
            return false;
        }

        bool checkForEmptiness = (required && checkEmptiness) || (!String.IsNullOrEmpty(strValueTo) && !String.IsNullOrEmpty(strValueFrom));

        if (checkForEmptiness)
        {
            if (!ValidationHelper.IsDateTime(strValueTo) || !ValidationHelper.IsDateTime(strValueFrom))
            {
                if (dtmTimeFrom.EditTime)
                {
                    // Error invalid DateTime
                    ValidationError += String.Format("{0} {1}.", ResHelper.GetString("BasicForm.ErrorInvalidDateTime"), DateTime.Now);
                }
                else
                {
                    // Error invalid date
                    ValidationError += String.Format("{0} {1}.", ResHelper.GetString("BasicForm.ErrorInvalidDate"), DateTime.Today.ToString("d"));
                }

                return false;
            }
        }

        if (!dtmTimeFrom.IsValidRange() || !dtmTimeTo.IsValidRange())
        {
            ValidationError += GetString("general.errorinvaliddatetimerange");
            return false;
        }

        return true;
    }


    /// <summary>
    /// Returns other values related to this form control.
    /// </summary>
    /// <returns>Returns an array where first dimension is attribute name and the second dimension is its value.</returns>
    public override object[,] GetOtherValues()
    {
        if (Form.Data is DataRowContainer)
        {
            if (!ContainsColumn(SecondDateFieldName))
            {
                Form.DataRow.Table.Columns.Add(SecondDateFieldName);
            }
        }

        // Set properties names
        object[,] values = new object[3, 2];
        values[0, 0] = SecondDateFieldName;
        values[0, 1] = dtmTimeTo.SelectedDateTime;
        return values;
    }


    /// <summary>
    /// Gets where condition.
    /// </summary>
    public override string GetWhereCondition()
    {
        string fromDate = FormHelper.GetDateTimeValueInSystemCulture(ValidationHelper.GetString(dtmTimeFrom.SelectedDateTime, string.Empty).Trim());
        string toDate = FormHelper.GetDateTimeValueInSystemCulture(ValidationHelper.GetString(dtmTimeTo.SelectedDateTime, string.Empty).Trim());
        string opFrom = ">=";
        string opTo = "<=";
        string where = null;

        if (String.IsNullOrEmpty(WhereConditionFormat))
        {
            WhereConditionFormat = "[{0}] {2} '{1}'";
        }

        if (!String.IsNullOrEmpty(fromDate) && (fromDate != DateTimeHelper.ZERO_TIME.ToString()))
        {
            where = string.Format(WhereConditionFormat, (FieldInfo != null) ? FieldInfo.Name : null, fromDate, opFrom);
        }

        if (!String.IsNullOrEmpty(toDate) && (toDate != DateTimeHelper.ZERO_TIME.ToString()))
        {
            where = SqlHelper.AddWhereCondition(where, string.Format(WhereConditionFormat, (FieldInfo != null) ? FieldInfo.Name : null, toDate, opTo));
        }

        return where;
    }

    #endregion
}