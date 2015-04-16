using System;

using CMS.Controls;
using CMS.DataEngine;
using CMS.ExtendedControls;
using CMS.Helpers;


public partial class CMSModules_Newsletters_Controls_SubscriberFilter : CMSAbstractBaseFilterControl
{
    /// <summary>
    /// Where condition.
    /// </summary>
    public override string WhereCondition
    {
        get
        {
            base.WhereCondition = GenerateWhereCondition(txtEmail.Text);
            return base.WhereCondition;
        }
        set
        {
            base.WhereCondition = value;
        }
    }


    #region "Methods"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        if (!RequestHelper.IsPostBack())
        {
            Reload();
        }
    }


    /// <summary>
    /// Reloads control.
    /// </summary>
    protected void Reload()
    {
        filter.Items.Clear();
        ControlsHelper.FillListWithTextSqlOperators(filter);
    }


    /// <summary>
    /// Generates WHERE condition.
    /// </summary>
    private string GenerateWhereCondition(string txtEmail)
    {
        if (String.IsNullOrEmpty(txtEmail))
        {
            return null;
        }
        else
        {
            string mOperator = WhereBuilder.LIKE;
            string email = SqlHelper.EscapeQuotes(txtEmail);

            // Get filter operator (LIKE, NOT LIKE, =, <>)
            if (filter.SelectedValue != null)
            {
                mOperator = filter.SelectedValue;
            }

            if ((mOperator == WhereBuilder.LIKE) || (mOperator == WhereBuilder.NOT_LIKE))
            {
                email = "%" + SqlHelper.EscapeLikeText(email) + "%";
            }

            return string.Format("((SubscriberEmail {0} '{1}') OR (Email {0} '{1}'))", mOperator, email);
        }
    }

    #endregion


    #region "State management"

    /// <summary>
    /// Stores filter state to the specified object.
    /// </summary>
    /// <param name="state">The object that holds the filter state.</param>
    public override void StoreFilterState(FilterState state)
    {
        state.AddValue("condition", filter.SelectedValue);
        state.AddValue("email", txtEmail.Text);
    }


    /// <summary>
    /// Restores filter state from the specified object.
    /// </summary>
    /// <param name="state">The object that holds the filter state.</param>
    public override void RestoreFilterState(FilterState state)
    {
        filter.SelectedValue = state.GetString("condition");
        txtEmail.Text = state.GetString("email");
    }


    /// <summary>
    /// Resets the filter settings.
    /// </summary>
    public override void ResetFilter()
    {
        filter.SelectedIndex = 0;
        txtEmail.Text = String.Empty;
    }

    #endregion

}