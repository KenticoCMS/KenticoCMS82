using System;
using System.Data;

using CMS.Base;
using CMS.UIControls;
using CMS.Controls;
using CMS.Helpers;
using CMS.DataEngine;

public partial class CMSModules_ContactManagement_Controls_UI_IP_Filter : CMSAbstractBaseFilterControl
{
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
    /// If true, site selector is visible.
    /// </summary>
    public bool ShowSiteFilter
    {
        get;
        set;
    }


    /// <summary>
    /// If true, contact filter (for contact name) is visible
    /// </summary>
    public bool ShowContactNameFilter
    {
        get;
        set;
    }

    #endregion


    #region "Methods"

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        plcSite.Visible = ShowSiteFilter;
        plcCon.Visible = ShowContactNameFilter;
        btnReset.Text = GetString("general.reset");
        btnReset.Click += btnReset_Click;
        btnSearch.Click += btnSearch_Click;
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
    /// Applies filter on associated UniGrid control.
    /// </summary>
    protected void btnSearch_Click(object sender, EventArgs e)
    {
        UniGrid grid = FilteredControl as UniGrid;
        if (grid != null)
        {
            grid.ApplyFilter(sender, e);
        }
    }


    /// <summary>
    /// Generates complete filter where condition.
    /// </summary>    
    private string GenerateWhereCondition()
    {
        string where = null;

        if (ShowContactNameFilter)
        {
            where = SqlHelper.AddWhereCondition(where, fltContact.GetCondition());
        }

        if (ShowSiteFilter)
        {
            if (siteSelector.SiteID > 0)
            {
                where = SqlHelper.AddWhereCondition(where, "ContactSiteID = " + siteSelector.SiteID);
            }
        }

        where = SqlHelper.AddWhereCondition(where, fltIP.GetCondition());
        where = SqlHelper.AddWhereCondition(where, dtFromTo.GetCondition());

        return where;
    }

    #endregion


    #region "State management"

    /// <summary>
    /// Resets filter to the default state.
    /// </summary>
    public override void ResetFilter()
    {
        siteSelector.Value = null;
        fltContact.ResetFilter();
        fltIP.ResetFilter();
        dtFromTo.Clear();
    }


    /// <summary>
    /// Stores filter state to the specified object.
    /// </summary>
    /// <param name="state">The object that holds the filter state.</param>
    public override void StoreFilterState(FilterState state)
    {
        state.AddValue("ToTime", dtFromTo.ValueToTime);
        state.AddValue("FromTime", dtFromTo.ValueFromTime);
        base.StoreFilterState(state);
    }


    /// <summary>
    /// Restores filter state from the specified object.
    /// </summary>
    /// <param name="state">The object that holds the filter state.</param>
    public override void RestoreFilterState(FilterState state)
    {
        base.RestoreFilterState(state);
        dtFromTo.ValueFromTime = state.GetDateTime("FromTime");
        dtFromTo.ValueToTime = state.GetDateTime("ToTime");
    }

    #endregion
}