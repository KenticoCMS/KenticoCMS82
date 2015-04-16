using System;
using System.Data;
using System.Collections;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.FormControls;
using CMS.Helpers;


public partial class CMSModules_Settings_FormControls_SelectEncoding : FormEngineUserControl
{
    private string encoding = "";


    protected void Page_Load(object sender, EventArgs e)
    {
        //ReloadData();
    }


    protected override void CreateChildControls()
    {
        base.CreateChildControls();
        ReloadData();
    }


    /// <summary>
    /// Gets or sets the enabled state of the control.
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
            drpSelectEncoding.Enabled = value;
        }
    }


    /// <summary>
    /// Gets or sets field value.
    /// </summary>
    public override object Value
    {
        get
        {
            return ValidationHelper.GetString(drpSelectEncoding.SelectedValue, "");
        }
        set
        {
            encoding = ValidationHelper.GetString(value, "");
            ReloadData();
        }
    }


    /// <summary>
    /// Returns true if user control is valid.
    /// </summary>
    public override bool IsValid()
    {
        return true;
    }


    /// <summary>
    /// Returns ClientID of the DropDownList with encoding.
    /// </summary>
    public override string ValueElementID
    {
        get
        {
            return drpSelectEncoding.ClientID;
        }
    }


    /// <summary>
    /// Loads drop down list with data.
    /// </summary>
    private void ReloadData()
    {
        if (drpSelectEncoding.Items.Count == 0)
        {
            EncodingInfo[] ei = Encoding.GetEncodings();
            SortedList sl = new SortedList();

            // Load sorted list
            for (int i = 0; i < ei.Length; i++)
            {
                if (!sl.Contains(ei[i].Name))
                {
                    sl.Add(ei[i].Name, ei[i].Name);
                }
            }

            // Populate dropdownlist with data from sorted list
            drpSelectEncoding.DataSource = sl;
            drpSelectEncoding.DataTextField = "Value";
            drpSelectEncoding.DataValueField = "Key";
            drpSelectEncoding.DataBind();

            // Preselect value
            ListItem selectedItem = drpSelectEncoding.Items.FindByValue(encoding);
            if (selectedItem != null)
            {
                selectedItem.Selected = true;
            }
            else
            {
                selectedItem = drpSelectEncoding.Items.FindByValue("utf-8");
                if (selectedItem != null)
                {
                    selectedItem.Selected = true;
                }
            }
        }
    }
}