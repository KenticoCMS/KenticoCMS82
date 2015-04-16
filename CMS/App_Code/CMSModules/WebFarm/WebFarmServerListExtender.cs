using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using CMS;
using CMS.Base;
using CMS.ExtendedControls;
using CMS.Helpers;
using CMS.UIControls;
using CMS.WebFarmSync;


/// <summary>
/// Custom class registration.
/// </summary>
[assembly: RegisterCustomClass("WebFarmServerListExtender", typeof(WebFarmServerListExtender))]

/// <summary>
/// Web farm server unigrid extender.
/// </summary>
public class WebFarmServerListExtender : ControlExtender<UniGrid>
{
    /// <summary>
    /// OnInit event.
    /// </summary>
    public override void OnInit()
    {
        Control.OnAction += OnAction;
        Control.OnExternalDataBound += OnExternalDataBound;
        Control.ZeroRowsText = ResHelper.GetString("general.nodatafound");

        if (WebSyncHelper.WebFarmInstanceEnabled && !String.IsNullOrEmpty(WebSyncHelper.ServerName))
        {
            if (SystemContext.IsRunningOnAzure)
            {
                Control.ShowInformation(String.Format(ResHelper.GetString("WebFarm.EnabledAzure"), WebSyncHelper.ServerName));
            }
            else
            {
                Control.ShowInformation(String.Format(ResHelper.GetString("WebFarm.Enabled"), WebSyncHelper.ServerName));
            }
        }
        else
        {
            Control.ShowInformation(ResHelper.GetString("WebFarm.Disabled"));
        }
    }


    /// <summary>
    /// Handles action event of unigrid.
    /// </summary>
    protected void OnAction(string actionName, object actionArgument)
    {
        if (actionName == "edit")
        {
            URLHelper.Redirect("WebFarm_Server_Edit.aspx?serverid=" + ValidationHelper.GetString(actionArgument, String.Empty));
        }
        else if (actionName == "delete")
        {
            // Delete WebFarmServerInfo object from database
            WebFarmServerInfoProvider.DeleteWebFarmServerInfo(ValidationHelper.GetInteger(actionArgument, 0));
        }
    }


    /// <summary>
    /// Handles external databound event of unigrid.
    /// </summary>
    protected object OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        switch (sourceName.ToLowerCSafe())
        {
            case "serverenabled":
                return UniGridFunctions.ColoredSpanYesNo(parameter);
        }
        return parameter;
    }
}