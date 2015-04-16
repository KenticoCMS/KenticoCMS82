using System;
using System.Web.UI.WebControls;

using CMS.FormControls;
using CMS.Helpers;


public partial class CMSFormControls_Macros_MacroOperatorSelector : FormEngineUserControl
{
    #region "Properties"

    /// <summary>
    /// Returns value of the 
    /// </summary>
    public override object Value
    {
        get
        {
            return drpOperator.SelectedValue;
        }
        set
        {
            ReloadData();

            drpOperator.ClearSelection();

            var selected = drpOperator.Items.FindByValue(ValidationHelper.GetString(value, ""));
            if (selected != null)
            {
                selected.Selected = true;
            }
        }
    }
    
    #endregion


    /// <summary>
    /// Page load.
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        ReloadData();
    }


    /// <summary>
    /// Puts items into the drop down list.
    /// </summary>
    private void ReloadData()
    {
        var items = drpOperator.Items;
        if (items.Count == 0)
        {
            items.Add(new ListItem(GetString("filter.equals"), "=="));
            items.Add(new ListItem(GetString("filter.notequals"), "!="));
            items.Add(new ListItem(GetString("filter.greaterthan"), ">"));
            items.Add(new ListItem(GetString("filter.lessthan"), "<"));
            items.Add(new ListItem(GetString("filter.greaterorequal"), ">="));
            items.Add(new ListItem(GetString("filter.lessorequal"), "<="));
        }
    }


    /// <summary>
    /// Loads the other fields values to the state of the form control
    /// </summary>
    public override void LoadOtherValues()
    {
        // Display name must not be loaded, it is one-way only
    }


    /// <summary>
    /// Returns display name displayed in the MacroRuleEditor control parameters designer.
    /// </summary>
    public override object[,] GetOtherValues()
    {
        // Set properties names
        object[,] values = new object[1, 2];
        values[0, 0] = "DisplayName";
        values[0, 1] = (drpOperator.SelectedItem != null ? drpOperator.SelectedItem.Text : "");
        return values;
    }
}