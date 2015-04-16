using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.FormControls;

public partial class CMSFormControls_Images_ImagePosition : FormEngineUserControl
{
    #region "Properties"

    /// <summary>
    /// Selected position.
    /// </summary>
    public override object Value
    {
        get
        {
            return drpPosition.SelectedValue;
        }
        set
        {
            EnsureItems();

            if (value == null)
            {
                drpPosition.SelectedValue = "";
            }
            else
            {
                drpPosition.SelectedValue = value.ToString();
            }
        }
    }


    /// <summary>
    /// Enabled state.
    /// </summary>
    public override bool Enabled
    {
        get
        {
            return base.Enabled;
        }
        set
        {
            base.Enabled = value;

            drpPosition.Enabled = value;
        }
    }

    #endregion


    #region "Control methods"

    /// <summary>
    /// Ensures the DDL items.
    /// </summary>
    protected void EnsureItems()
    {
        if (drpPosition.Items.Count == 0)
        {
            // Fill the combo with versions
            drpPosition.Items.Add(new ListItem("Top left", "topleft"));
            drpPosition.Items.Add(new ListItem("Top center", "topcenter"));
            drpPosition.Items.Add(new ListItem("Top right", "topright"));
            drpPosition.Items.Add(new ListItem("Middle left", "middleleft"));
            drpPosition.Items.Add(new ListItem("Middle center", "middlecenter"));
            drpPosition.Items.Add(new ListItem("Middle right", "middleright"));
            drpPosition.Items.Add(new ListItem("Bottom left", "bottomleft"));
            drpPosition.Items.Add(new ListItem("Bottom center", "bottomcenter"));
            drpPosition.Items.Add(new ListItem("Bottom right", "bottomright"));
            drpPosition.Items.Add(new ListItem("Full size", "fullsize"));
            drpPosition.Items.Add(new ListItem("Centered maximum size", "centermaxsize"));
        }
    }

    #endregion
}