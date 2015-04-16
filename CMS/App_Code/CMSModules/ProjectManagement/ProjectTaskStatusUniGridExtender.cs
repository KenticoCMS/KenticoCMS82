using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using CMS;
using CMS.Base;
using CMS.ExtendedControls;
using CMS.Helpers;
using CMS.UIControls;

[assembly: RegisterCustomClass("ProjectTaskStatusUniGridExtender", typeof(ProjectTaskStatusUniGridExtender))]

/// <summary>
/// Extends Unigrids used for projects from Project management module with additional abilities.
/// </summary>
public class ProjectTaskStatusUniGridExtender : ControlExtender<UniGrid>
{
    #region "life-cycle methods and event handlers"

    /// <summary>
    /// OnInit page event.
    /// </summary>
    public override void OnInit()
    {
        Control.OnExternalDataBound += Control_OnExternalDataBound;
    }


    /// <summary>
    /// Handles UniGrid's OnExternalDataBound event.
    /// </summary>
    /// <param name="sender">Sender object (image button if it is an external databoud for action button)</param>
    /// <param name="sourceName">External source name of the column specified in the grid XML</param>
    /// <param name="parameter">Source parameter (original value of the field)</param>
    private object Control_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        switch (sourceName.ToLowerCSafe())
        {
            case "taskstatusicon":
                string url = ValidationHelper.GetString(parameter, "");
                if (!String.IsNullOrEmpty(url))
                {
                    url = "<img alt=\"Status image\" src=\"" + HTMLHelper.HTMLEncode(Control.GetImageUrl(url)) + "\" style=\"max-width:50px; max-height: 50px;\"  />";
                    return url;
                }
                return "";
        }

        return parameter;
    }

    #endregion

}
