using System.Web.UI.WebControls;

using CMS.FormControls;
using CMS.Helpers;

public partial class CMSModules_REST_FormControls_SelectRESTServiceLevel : FormEngineUserControl
{
    protected override void CreateChildControls()
    {
        base.CreateChildControls();
        if (!StopProcessing)
        {
            ReloadData();
        }
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
            rblType.Enabled = value;
        }
    }


    /// <summary>
    /// Gets or sets field value.
    /// </summary>
    public override object Value
    {
        get
        {
            return rblType.SelectedValue;
        }
        set
        {
            EnsureChildControls();
            rblType.SelectedValue = ValidationHelper.GetString(value, "0");
        }
    }


    /// <summary>
    /// ClientID of the radio button list.
    /// </summary>
    public override string ValueElementID
    {
        get
        {
            return rblType.ClientID;
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
    /// Selects correct value.
    /// </summary>
    private void ReloadData()
    {
        if ((rblType.Items == null) || (rblType.Items.Count <= 0))
        {
            rblType.Items.Add(new ListItem(GetString("rest.enableobjectsonly"), "1"));
            rblType.Items.Add(new ListItem(GetString("rest.enabledocumentsonly"), "2"));
            rblType.Items.Add(new ListItem(GetString("rest.enableboth"), "0"));
        }
    }
}