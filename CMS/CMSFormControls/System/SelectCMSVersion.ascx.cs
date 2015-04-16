using System;
using System.Web.UI.WebControls;

using CMS.FormControls;
using CMS.Helpers;

public partial class CMSFormControls_System_SelectCMSVersion : FormEngineUserControl
{
    #region "Properties"

    /// <summary>
    /// Indicates if value of form control could be empty.
    /// </summary>
    public bool AllowEmpty
    {
        get;
        set;
    }


    /// <summary>
    /// Selected version (e.g. '5.5', '5.5R2',...).
    /// </summary>
    public override object Value
    {
        get
        {
            EnsureList();
            return (drpVersion.SelectedItem == null) ? "" : drpVersion.SelectedItem.Value;
        }
        set
        {
            EnsureList();
            drpVersion.SelectedValue = (value == null) ? "" : value.ToString();
        }
    }

    #endregion


    #region "Control methods"

    private void EnsureList()
    {
        EnsureChildControls();

        if (drpVersion.Items.Count == 0)
        {
            // Fill the combo with versions
            drpVersion.Items.Add(new ListItem("(none)", ""));
            drpVersion.Items.Add(new ListItem("CMS 3.0", "3.0"));
            drpVersion.Items.Add(new ListItem("CMS 3.1", "3.1"));
            drpVersion.Items.Add(new ListItem("CMS 3.1a", "3.1a"));
            drpVersion.Items.Add(new ListItem("CMS 4.0", "4.0"));
            drpVersion.Items.Add(new ListItem("CMS 4.1", "4.1"));
            drpVersion.Items.Add(new ListItem("CMS 5.0", "5.0"));
            drpVersion.Items.Add(new ListItem("CMS 5.5", "5.5"));
            drpVersion.Items.Add(new ListItem("CMS 5.5R2", "5.5R2"));
            drpVersion.Items.Add(new ListItem("CMS 6.0", "6.0"));
            drpVersion.Items.Add(new ListItem("CMS 7.0", "7.0"));
            drpVersion.Items.Add(new ListItem("CMS 8.0", "8.0"));
            drpVersion.Items.Add(new ListItem("CMS 8.1", "8.1"));
            drpVersion.Items.Add(new ListItem("CMS 8.2", "8.2"));
        }
    }

    protected override void OnInit(EventArgs e)
    {
        EnsureList();
        base.OnInit(e);
    }


    protected void Page_Load(object sender, EventArgs e)
    {
    }


    /// <summary>
    /// Validates the return value of form control.
    /// </summary>
    public override bool IsValid()
    {
        if (!AllowEmpty && (drpVersion.SelectedValue == ""))
        {
            ValidationError = ResHelper.GetString("general.requirescmsversion");
            return false;
        }

        return true;
    }

    #endregion
}