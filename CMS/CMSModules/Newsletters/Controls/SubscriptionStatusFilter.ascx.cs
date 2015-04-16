using System;
using System.Web.UI.WebControls;

using CMS.Controls;
using CMS.Helpers;

public partial class CMSModules_Newsletters_Controls_SubscriptionStatusFilter : CMSAbstractBaseFilterControl
{
    #region "Properties"

    private const string ALL = "all";
    private const string APPROVED = "approved";
    private const string WAITING = "waiting";
    private const string UNSUBSCRIBED = "unsubscribed";

    /// <summary>
    /// Gets or sets the where condition.
    /// </summary>
    public override string WhereCondition
    {
        get
        {
            base.WhereCondition = GenerateWhereCondition();
            return base.WhereCondition;
        }
        set
        {
            base.WhereCondition = value;
        }
    }

    #endregion


    #region "Methods"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        if (!RequestHelper.IsPostBack())
        {
            ReloadItems();
        }
    }


    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        // Css class is not stored in the viewstate
        LoadStyles();
    }


    /// <summary>
    /// Reloads control.
    /// </summary>
    protected void ReloadItems()
    {
        ddlStatusFilter.Items.Clear();
        ddlStatusFilter.Items.Add(new ListItem(ResHelper.GetString("general.selectall"), ALL));
        ddlStatusFilter.Items.Add(new ListItem(ResHelper.GetString("general.approved"), APPROVED));
        ddlStatusFilter.Items.Add(new ListItem(ResHelper.GetString("administration.users_header.myapproval"), WAITING));
        ddlStatusFilter.Items.Add(new ListItem(ResHelper.GetString("newsletterview.headerunsubscribed"), UNSUBSCRIBED));
    }


    /// <summary>
    /// Loads CSS styles
    /// </summary>
    private void LoadStyles()
    {
        if (ddlStatusFilter.Items.Count < 4)
        {
            return;
        }

        ddlStatusFilter.Items[1].Attributes.Add("Class", "alert-status-success");
        ddlStatusFilter.Items[2].Attributes.Add("Class", "alert-status-warning");
        ddlStatusFilter.Items[3].Attributes.Add("Class", "alert-status-error");
    }


    /// <summary>
    /// Generates WHERE condition.
    /// </summary>
    private string GenerateWhereCondition()
    {
        switch (ddlStatusFilter.SelectedValue)
        {
            case APPROVED:
                return "(SubscriptionApproved = 1 OR SubscriptionApproved IS NULL) AND (SubscriptionEnabled = 1 OR SubscriptionEnabled IS NULL)";

            case WAITING:
                return "SubscriptionApproved = 0 AND SubscriptionEnabled = 1";

            case UNSUBSCRIBED:
                return "SubscriptionEnabled = 0";

            default:
                return null;
        }
    }


    /// <summary>
    /// Resets the filter settings.
    /// </summary>
    public override void ResetFilter()
    {
        ddlStatusFilter.SelectedIndex = 0;
    }

    #endregion
}
