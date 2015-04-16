using System;

using CMS.Controls;
using CMS.DataEngine;
using CMS.ExtendedControls;


public partial class CMSAdminControls_UI_UniSelector_Controls_SearchControl : CMSAbstractBaseFilterControl
{
    /// <summary>
    /// OnLoad override - check wheter filter is set.
    /// </summary>
    protected override void OnLoad(EventArgs e)
    {
        LoadDropDown();
        base.OnLoad(e);
    }


    /// <summary>
    /// Loads dropdown list.
    /// </summary>
    private void LoadDropDown()
    {
        drpCondition.Items.Clear();
        ControlsHelper.FillListWithTextSqlOperators(drpCondition);
    }


    /// <summary>
    /// Generates where condition.
    /// </summary>
    protected static string GenerateWhereCondition(string text, string value)
    {
        string searchPhrase = SqlHelper.EscapeQuotes(text);

        switch (value)
        {
            case WhereBuilder.LIKE:
                return "LIKE N'%" + SqlHelper.EscapeLikeText(searchPhrase) + "%'";
            case WhereBuilder.NOT_LIKE:
                return "NOT LIKE N'%" + SqlHelper.EscapeLikeText(searchPhrase) + "%'";
            case WhereBuilder.EQUAL:
                return "= N'" + searchPhrase + "'";
            case WhereBuilder.NOT_EQUAL:
                return "<> N'" + searchPhrase + "'";
            default:
                return "LIKE N'%'";
        }
    }


    /// <summary>
    /// Select button handler.
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="e">EventArgs</param>
    protected void btnSelect_Click(object sender, EventArgs e)
    {
        // Set where condition
        WhereCondition = GenerateWhereCondition(txtSearch.Text, drpCondition.SelectedValue);
        //Raise OnFilterChange event
        RaiseOnFilterChanged();
    }
}