using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using CMS;
using CMS.Base;
using CMS.ExtendedControls;
using CMS.FormControls;
using CMS.Reporting;
using CMS.Helpers;

[assembly: RegisterCustomClass("ReportNewControlExtender", typeof(ReportNewControlExtender))]

/// <summary>
/// New report control extender
/// </summary>
public class ReportNewControlExtender : ControlExtender<UIForm>
{
    public override void OnInit()
    {
        Control.OnBeforeValidate += new EventHandler(Control_OnBeforeValidate);
    }


    void Control_OnBeforeValidate(object sender, EventArgs e)
    {
        if (Control.EditedObject is ReportInfo)
        {
            ReportInfo ri = (ReportInfo)Control.EditedObject;

            // Get information from checkbox
            bool allowforall = ValidationHelper.GetBoolean(Control.FieldControls["ReportAccess"].Value, false);

            // Set proper integer value
            ri.ReportAccess = allowforall ? ReportAccessEnum.All : ReportAccessEnum.Authenticated;

            // Disable the item to prevent integer validation (value is boolean)
            Control.FieldControls["ReportAccess"].Enabled = false;           
            Control.ProcessDisabledFields = false;
        }
    }
}