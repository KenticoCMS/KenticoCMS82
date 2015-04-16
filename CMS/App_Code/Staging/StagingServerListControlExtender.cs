using System;
using System.Linq;

using CMS;
using CMS.Base;
using CMS.ExtendedControls;
using CMS.Synchronization;
using CMS.UIControls;


[assembly: RegisterCustomClass("StagingServerListControlExtender", typeof(StagingServerListControlExtender))]

/// <summary>
/// Permission edit control extender
/// </summary>
public class StagingServerListControlExtender : ControlExtender<UniGrid>
{
    /// <summary>
    /// OnInit event handler
    /// </summary>
    public override void OnInit()
    {
        Control.Load += (sender, args) =>
        {
            if (!String.IsNullOrEmpty(StagingTaskInfoProvider.ServerName))
            {
                Control.ShowInformation(String.Format(Control.GetString("staging.currentserver"), StagingTaskInfoProvider.ServerName));
            }
        };
    }
}