using System;
using System.Web.UI.WebControls;
using System.Web.UI;

using CMS.Controls;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.UIControls;

public partial class CMSModules_EventLog_Controls_EventFilter : CMSAbstractBaseFilterControl
{
    #region "Variables"

    private bool isAdvancedMode;

    #endregion


    #region "Public properties"

    /// <summary>
    /// Gets the where condition created using filtered parameters.
    /// </summary>
    public override string WhereCondition
    {
        get
        {
            return GenerateWhereCondition();
        }
    }


    /// <summary>
    /// Gets button used to toggle filter's advanced mode.
    /// </summary>
    public override IButtonControl ToggleAdvancedModeButton
    {
        get
        {
            EnsureFilterMode();

            // Advanced mode has already been switched this postback.
            return isAdvancedMode ? lnkShowAdvancedFilter : lnkShowSimpleFilter;
        }
    }

    #endregion


    #region "Page methods"

    protected void Page_Init(object sender, EventArgs e)
    {
        if (!URLHelper.IsPostback())
        {
            drpEventLogType.Value = QueryHelper.GetString("type", null);
        }
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        EnsureFilterMode();
        InitializeForm();
    }

    #endregion


    #region "UI methods"

    /// <summary>
    /// Shows/hides all elements for advanced or simple mode.
    /// </summary>
    /// <param name="showAdvanced">Whether to display advanced filter</param>
    private void ShowFilterElements(bool showAdvanced)
    {
        plcAdvancedSearch.Visible = showAdvanced;
        pnlAdvanced.Visible = showAdvanced;
        pnlSimple.Visible = !showAdvanced;
    }


    /// <summary>
    /// Initializes the layout of the form.
    /// </summary>
    private void InitializeForm()
    {
        // General UI
        lnkShowAdvancedFilter.Text = GetString("general.displayadvancedfilter");
        lnkShowSimpleFilter.Text = GetString("general.displaysimplefilter");
        plcAdvancedSearch.Visible = isAdvancedMode;
        pnlAdvanced.Visible = isAdvancedMode;
        pnlSimple.Visible = !isAdvancedMode;

        UniGrid grid = FilteredControl as UniGrid;
        if (grid != null && grid.RememberState)
        {
            grid.RememberDefaultState = true;
            btnReset.Text = GetString("general.reset");
            btnReset.Click += btnReset_Click;
        }
        else
        {
            btnReset.Visible = false;
        }
    }


    /// <summary>
    /// Ensures correct filter mode flag if filter mode was just changed.
    /// </summary>
    private void EnsureFilterMode()
    {
        if (URLHelper.IsPostback())
        {
            // Get current event target
            string uniqieId = ValidationHelper.GetString(Request.Params[Page.postEventSourceID], String.Empty);

            // If postback was fired by mode switch, update isAdvancedMode variable
            if (uniqieId == lnkShowAdvancedFilter.UniqueID)
            {
                isAdvancedMode = true;
            }
            else if (uniqieId == lnkShowSimpleFilter.UniqueID)
            {
                isAdvancedMode = false;
            }
            else
            {
                isAdvancedMode = ValidationHelper.GetBoolean(ViewState["IsAdvancedMode"], false);
            }
        }
    }


    /// <summary>
    /// Sets the advanced mode.
    /// </summary>
    protected void lnkShowAdvancedFilter_Click(object sender, EventArgs e)
    {
        isAdvancedMode = true;
        ViewState["IsAdvancedMode"] = isAdvancedMode;
        ShowFilterElements(isAdvancedMode);
    }


    /// <summary>
    /// Sets the simple mode.
    /// </summary>
    protected void lnkShowSimpleFilter_Click(object sender, EventArgs e)
    {
        isAdvancedMode = false;
        ViewState["IsAdvancedMode"] = isAdvancedMode;
        ShowFilterElements(isAdvancedMode);
    }

    #endregion


    #region "Search methods - where condition"

    /// <summary>
    /// Generates complete filter where condition.
    /// </summary>    
    private string GenerateWhereCondition()
    {
        // Get mode from viewstate
        EnsureFilterMode();

        string whereCond = "";

        // Create WHERE condition for basic filter
        string eventType = ValidationHelper.GetString(drpEventLogType.Value, null);
        if (!String.IsNullOrEmpty(eventType))
        {
            whereCond = "EventType='" + eventType + "'";
        }

        whereCond = SqlHelper.AddWhereCondition(whereCond, fltSource.GetCondition());
        whereCond = SqlHelper.AddWhereCondition(whereCond, fltEventCode.GetCondition());
        whereCond = SqlHelper.AddWhereCondition(whereCond, fltTimeBetween.GetCondition());

        // Create WHERE condition for advanced filter (id needed)
        if (isAdvancedMode)
        {
            whereCond = SqlHelper.AddWhereCondition(whereCond, fltUserName.GetCondition());
            whereCond = SqlHelper.AddWhereCondition(whereCond, fltIPAddress.GetCondition());
            whereCond = SqlHelper.AddWhereCondition(whereCond, fltDocumentName.GetCondition());
            whereCond = SqlHelper.AddWhereCondition(whereCond, fltMachineName.GetCondition());
            whereCond = SqlHelper.AddWhereCondition(whereCond, fltEventURL.GetCondition());
            whereCond = SqlHelper.AddWhereCondition(whereCond, fltEventURLRef.GetCondition());
            whereCond = SqlHelper.AddWhereCondition(whereCond, fltDescription.GetCondition());
            whereCond = SqlHelper.AddWhereCondition(whereCond, fltUserAgent.GetCondition());
        }

        return whereCond;
    }

    #endregion


    #region "State management"

    /// <summary>
    /// Stores filter state to the specified object.
    /// </summary>
    /// <param name="state">The object that holds the filter state.</param>
    public override void StoreFilterState(FilterState state)
    {
        base.StoreFilterState(state);
        state.AddValue("AdvancedMode", isAdvancedMode);
        state.AddValue("TimeBetweenFrom", fltTimeBetween.ValueFromTime);
        state.AddValue("TimeBetweenTo", fltTimeBetween.ValueToTime);
    }


    /// <summary>
    /// Restores filter state from the specified object.
    /// </summary>
    /// <param name="state">The object that holds the filter state.</param>
    public override void RestoreFilterState(FilterState state)
    {
        base.RestoreFilterState(state);
        isAdvancedMode = state.GetBoolean("AdvancedMode");
        ViewState["IsAdvancedMode"] = isAdvancedMode;
        ShowFilterElements(isAdvancedMode);

        fltTimeBetween.ValueFromTime = state.GetDateTime("TimeBetweenFrom");
        fltTimeBetween.ValueToTime = state.GetDateTime("TimeBetweenTo");
    }


    /// <summary>
    /// Resets filter to the default state.
    /// </summary>
    public override void ResetFilter()
    {
        drpEventLogType.Value = String.Empty;
        fltEventCode.ResetFilter();
        fltSource.ResetFilter();
        fltUserName.ResetFilter();
        fltIPAddress.ResetFilter();
        fltDocumentName.ResetFilter();
        fltMachineName.ResetFilter();
        fltEventURL.ResetFilter();
        fltEventURLRef.ResetFilter();
        fltDescription.ResetFilter();
        fltUserAgent.ResetFilter();
        fltTimeBetween.Clear();
    }


    /// <summary>
    /// Resets the associated UniGrid control.
    /// </summary>
    protected void btnReset_Click(object sender, EventArgs e)
    {
        UniGrid grid = FilteredControl as UniGrid;
        if (grid != null)
        {
            grid.Reset();
        }
    }


    /// <summary>
    /// Applies the filter.
    /// </summary>
    protected void btnSearch_Click(object sender, EventArgs e)
    {
        UniGrid grid = FilteredControl as UniGrid;
        if (grid != null)
        {
            grid.ApplyFilter(sender, e);
        }

        ShowFilterElements(isAdvancedMode);
    }
    #endregion
}