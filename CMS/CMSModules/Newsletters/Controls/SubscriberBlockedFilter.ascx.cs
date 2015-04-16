using System;
using System.Web.UI.WebControls;

using CMS.Controls;
using CMS.Helpers;
using CMS.SiteProvider;
using CMS.Membership;
using CMS.Newsletters;
using CMS.DataEngine;

public partial class CMSModules_Newsletters_Controls_SubscriberBlockedFilter : CMSAbstractBaseFilterControl
{
    #region "Constants"

    private const string All = "ALL";


    private const string Active = "ACTIVE";


    private const string Blocked = "BLOCKED";

    #endregion


    #region "Variables"

    private int mBounceLimit;

    #endregion


    #region "Properties"

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
            Reload();
        }
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        mBounceLimit = NewsletterHelper.BouncedEmailsLimit(SiteContext.CurrentSiteName);
    }


    /// <summary>
    /// Reloads control.
    /// </summary>
    protected void Reload()
    {
        ddlBounceFilter.Items.Clear();
        ddlBounceFilter.Items.Add(new ListItem(ResHelper.GetString("general.selectall"), All));
        ddlBounceFilter.Items.Add(new ListItem(ResHelper.GetString("newsletter.filter.bouncedemails.active"), Active));
        ddlBounceFilter.Items.Add(new ListItem(ResHelper.GetString("newsletter.filter.bouncedemails.blocked"), Blocked));
    }


    /// <summary>
    /// Generates WHERE condition.
    /// </summary>
    private string GenerateWhereCondition()
    {
        if (mBounceLimit <= 0)
        {
            return null;
        }

        switch (ddlBounceFilter.SelectedValue)
        {
            case All:
                return null;

            case Active:
                return string.Format("(((SubscriberType IS NULL OR SubscriberType='{0}' OR SubscriberType='{1}') AND (SubscriberBounces IS NULL OR SubscriberBounces<{2})) OR (SubscriberType = '{3}') OR (SubscriberType = '{4}'))",
                                     UserInfo.OBJECT_TYPE, PredefinedObjectType.CONTACT, mBounceLimit, RoleInfo.OBJECT_TYPE, PredefinedObjectType.CONTACTGROUP);

            case Blocked:
                return string.Format("((SubscriberType IS NULL OR SubscriberType='{0}' OR SubscriberType='{1}') AND (SubscriberBounces>={2}))", UserInfo.OBJECT_TYPE, PredefinedObjectType.CONTACT, mBounceLimit);

            default:
                return null;
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
        state.AddValue("blocked", ddlBounceFilter.SelectedValue);
    }


    /// <summary>
    /// Restores filter state from the specified object.
    /// </summary>
    /// <param name="state">The object that holds the filter state.</param>
    public override void RestoreFilterState(FilterState state)
    {
        ddlBounceFilter.SelectedValue = state.GetString("blocked");
    }


    /// <summary>
    /// Resets the filter settings.
    /// </summary>
    public override void ResetFilter()
    {
        ddlBounceFilter.SelectedIndex = 0;
    }

    #endregion
}