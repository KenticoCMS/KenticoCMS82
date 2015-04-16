using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.UIControls;
using CMS.Helpers;
using CMS.FormEngine;
using CMS.FormControls;

[Security()]
[HashValidation(null)]
[Title("General.EditProperty")]
public partial class CMSModules_PortalEngine_UI_WebParts_EditProperty : CMSModalPage
{
    /// <summary>
    /// Form control
    /// </summary>
    protected FormEngineUserControl FormControl
    {
        get;
        set;
    }


    /// <summary>
    /// Underlying element ID
    /// </summary>
    protected string ElementID
    {
        get;
        set;
    }


    /// <summary>
    /// Init event handler
    /// </summary>
    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        Save += btnOK_Click;

        ElementID = QueryHelper.GetString("elementid", "");

        if (!RequestHelper.IsPostBack())
        {
            // Load the initialization script which pulls the value from the opener
            ScriptHelper.RegisterStartupScript(this, typeof(string), "LoadValue", ScriptHelper.GetScript(String.Format(
                "document.getElementById('{0}').value = wopener.document.getElementById({1}).value; {2}",
                hdnValue.ClientID,
                ScriptHelper.GetString(ElementID),
                ClientScript.GetPostBackEventReference(btnLoad, null)
            )));
        }
        else
        {
            // Load the form control
            string formControl = QueryHelper.GetString("formcontrol", "");

            // Load the form control
            var ctrl = FormControlsHelper.LoadFormControl(Page, formControl, "");
            if (ctrl != null)
            {
                plcControl.Controls.Add(ctrl);
            }

            FormControl = ctrl;
        }
    }


    /// <summary>
    /// Loads the form control value from the hidden field
    /// </summary>
    protected void btnLoad_Click(object sender, EventArgs e)
    {
        FormControl.Value = hdnValue.Value;
    }


    /// <summary>
    /// Saves the form control value back to the element
    /// </summary>
    protected void btnOK_Click(object sender, EventArgs e)
    {
        string newValue = ValidationHelper.GetString(FormControl.Value, "");

        // Check if empty
        if (String.IsNullOrEmpty(HTMLHelper.StripTags(newValue).Trim()))
        {
            ShowError(GetString("general.requiresvalue"));
            return;
        }
        
        hdnValue.Value = newValue;
        FormControl.Visible = false;

        ScriptHelper.RegisterStartupScript(this, typeof(string), "LoadValue", ScriptHelper.GetScript(String.Format(
@"
var e = wopener.document.getElementById('{1}');
e.value = document.getElementById('{0}').value;
if (e.save) e.save();
CloseDialog();
",
            hdnValue.ClientID,
            ElementID
        )));
    }
}
