using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.Controls;
using CMS.Helpers;

public partial class CMSModules_Newsletters_Controls_TemplateFilter : CMSAbstractBaseFilterControl
{
    #region "Properties"

    /// <summary>
    /// Where condition.
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


    /// <summary>
    /// Reloads control.
    /// </summary>
    protected void Reload()
    {
        filter.Items.Clear();
        filter.Items.Add(new ListItem(ResHelper.GetString("general.selectall"), "-1"));
        filter.Items.Add(new ListItem(ResHelper.GetString("NewsletterTemplate_List.OptIn"), "D"));
        filter.Items.Add(new ListItem(ResHelper.GetString("NewsletterTemplate_List.Issue"), "I"));
        filter.Items.Add(new ListItem(ResHelper.GetString("NewsletterTemplate_List.subscription"), "S"));
        filter.Items.Add(new ListItem(ResHelper.GetString("NewsletterTemplate_List.Unsubscription"), "U"));
    }


    /// <summary>
    /// Generates WHERE condition.
    /// </summary>
    private string GenerateWhereCondition()
    {
        switch (filter.SelectedValue)
        {
                // All templates
            default:
            case "-1":
                break;

                // Double opt-in templates
            case "D":
                return "TemplateType = 'D'";

                // Issue templates
            case "I":
                return "TemplateType = 'I'";

                // Subscription templates
            case "S":
                return "TemplateType = 'S'";

                // Unsubscription templates
            case "U":
                return "TemplateType = 'U'";
        }
        return null;
    }

    #endregion


    #region "State management"

    /// <summary>
    /// Stores filter state to the specified object.
    /// </summary>
    /// <param name="state">The object that holds the filter state.</param>
    public override void StoreFilterState(FilterState state)
    {
        state.AddValue("type", filter.SelectedValue);
    }


    /// <summary>
    /// Restores filter state from the specified object.
    /// </summary>
    /// <param name="state">The object that holds the filter state.</param>
    public override void RestoreFilterState(FilterState state)
    {
        filter.SelectedValue = state.GetString("type");
    }


    /// <summary>
    /// Resets the filter settings.
    /// </summary>
    public override void ResetFilter()
    {
        filter.SelectedIndex = 0;
    }

    #endregion
}