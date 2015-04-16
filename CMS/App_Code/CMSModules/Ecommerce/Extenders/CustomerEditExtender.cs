using System;
using System.Linq;

using CMS;
using CMS.Ecommerce;
using CMS.ExtendedControls;
using CMS.FormControls;
using CMS.Helpers;

[assembly: RegisterCustomClass("CustomerEditExtender", typeof(CustomerEditExtender))]

/// <summary>
/// Extender for Customer edit - General tab
/// </summary>
public class CustomerEditExtender : ControlExtender<UIForm>
{
    public override void OnInit()
    {
        if (Control != null)
        {
            Control.OnAfterSave += Control_OnAfterSave;
        }
    }


    /// <summary>
    /// Refresh breadcrumbs after save, to ensure correct display text
    /// </summary>
    void Control_OnAfterSave(object sender, EventArgs e)
    {
        var customer = Control.EditedObject as CustomerInfo;

        if (customer != null)
        {
            ScriptHelper.RefreshTabHeader(Control.Page, customer.CustomerInfoName);
        }
    }
}