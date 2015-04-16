using System;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.Controls;
using CMS.DataEngine;
using CMS.ExtendedControls;
using CMS.Helpers;
using CMS.UIControls;


public partial class CMSModules_AdminControls_Controls_Documents_OutdatedDocumentsFilter : CMSAbstractBaseFilterControl
{
    #region "Constants"

    private const string SOURCE_MODIFIEDWHEN = "DocumentModifiedWhen";

    private const string SOURCE_CLASSDISPLAYNAME = "ClassDisplayName";

    private const string SOURCE_DOCUMENTNAME = "DocumentName";

    private const string SOURCE_NODESITEID = "NodeSiteID";

    #endregion


    #region "Public properties"

    public override string WhereCondition
    {
        get
        {
            return GenerateWhereCondition();
        }
    }

    #endregion


    #region "Page events"

    protected override void OnInit(EventArgs e)
    {
        if (!StopProcessing)
        {
            SetupControl();
        }
    }

    #endregion


    #region "State management"

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
    protected void btnShow_Click(object sender, EventArgs e)
    {
        UniGrid grid = FilteredControl as UniGrid;
        if (grid != null)
        {
            grid.ApplyFilter(sender, e);
        }
    }


    /// <summary>
    /// Resets all controls to default.
    /// </summary>
    public override void ResetFilter()
    {
        txtFilter.Text = "1";
        drpFilter.SelectedIndex = 3;
        drpDocumentName.SelectedIndex = 0;
        drpDocumentType.SelectedIndex = 0;
        txtDocumentType.Text = String.Empty;
        txtDocumentName.Text = String.Empty;
    }

    #endregion


    #region "Private methods"

    /// <summary>
    /// Setups filter controls.
    /// </summary>
    private void SetupControl()
    {
        // Initialize controls
        if (!URLHelper.IsPostback())
        {
            // Fill the dropdown list
            drpFilter.Items.Add(GetString("MyDesk.OutdatedDocuments.Days"));
            drpFilter.Items.Add(GetString("MyDesk.OutdatedDocuments.Weeks"));
            drpFilter.Items.Add(GetString("MyDesk.OutdatedDocuments.Months"));
            drpFilter.Items.Add(GetString("MyDesk.OutdatedDocuments.Years"));

            // Load default value
            if (String.IsNullOrEmpty(txtFilter.Text))
            {
                txtFilter.Text = "1";
                drpFilter.SelectedIndex = 3;
            }

            // Bind dropdown lists
            BindDropDowns();
        }

        // Initialize Reset button
        UniGrid grid = FilteredControl as UniGrid;
        if (grid == null || !grid.RememberState)
        {
            btnReset.Visible = false;
        }
    }


    /// <summary>
    /// Generates where condition.
    /// </summary>
    private string GenerateWhereCondition()
    {
        // Get older than value
        DateTime olderThan = DateTime.Now.Date.AddDays(1);
        int dateTimeValue = ValidationHelper.GetInteger(txtFilter.Text, 0);

        switch (drpFilter.SelectedIndex)
        {
            case 0:
                olderThan = olderThan.AddDays(-dateTimeValue);
                break;

            case 1:
                olderThan = olderThan.AddDays(-dateTimeValue * 7);
                break;

            case 2:
                olderThan = olderThan.AddMonths(-dateTimeValue);
                break;

            case 3:
                olderThan = olderThan.AddYears(-dateTimeValue);
                break;
        }

        string where = "((DocumentCreatedByUserID = @UserID OR DocumentModifiedByUserID = @UserID OR DocumentCheckedOutByUserID = @UserID) AND " + SOURCE_MODIFIEDWHEN + "<= '" + olderThan + "' AND " + SOURCE_NODESITEID + "=@SiteID)";
        // Add where condition
        if (!string.IsNullOrEmpty(txtDocumentName.Text))
        {
            where = SqlHelper.AddWhereCondition(where, GetOutdatedWhereCondition(SOURCE_DOCUMENTNAME, drpDocumentName, txtDocumentName));
        }
        if (!string.IsNullOrEmpty(txtDocumentType.Text))
        {
            where = SqlHelper.AddWhereCondition(where, GetOutdatedWhereCondition(SOURCE_CLASSDISPLAYNAME, drpDocumentType, txtDocumentType));
        }

        return where;
    }


    /// <summary>
    /// Gets where condition based on value of given controls.
    /// </summary>
    /// <param name="column">Column to compare</param>
    /// <param name="drpOperator">List control with operator</param>
    /// <param name="valueBox">Text control with value</param>
    /// <returns>Where condition for outdated documents</returns>
    private string GetOutdatedWhereCondition(string column, ListControl drpOperator, ITextControl valueBox)
    {
        string condition = drpOperator.SelectedValue;
        string value = SqlHelper.EscapeQuotes(valueBox.Text);
        value = TextHelper.LimitLength(value, 100);

        string where = column + " ";
        if (string.IsNullOrEmpty(value))
        {
            where = string.Empty;
        }
        else
        {
            // Create condition based on operator
            switch (condition)
            {
                case WhereBuilder.LIKE:
                case WhereBuilder.NOT_LIKE:
                    where += condition + " N'%" + SqlHelper.EscapeLikeText(value) + "%'";
                    break;

                case WhereBuilder.EQUAL:
                case WhereBuilder.NOT_EQUAL:
                    where += condition + " N'" + value + "'";
                    break;

                default:
                    where = string.Empty;
                    break;
            }
        }
        return where;
    }


    /// <summary>
    /// Binds filter dropdown lists with conditions.
    /// </summary>
    private void BindDropDowns()
    {
        ControlsHelper.FillListWithTextSqlOperators(drpDocumentName);
        ControlsHelper.FillListWithTextSqlOperators(drpDocumentType);
    }

    #endregion
}
