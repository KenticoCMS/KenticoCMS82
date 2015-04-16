using System;
using System.Linq;
using System.Threading;
using System.Web.UI.WebControls;

using CMS;
using CMS.Base;
using CMS.DataEngine;
using CMS.DocumentEngine;
using CMS.ExtendedControls;
using CMS.Helpers;
using CMS.IO;
using CMS.Membership;
using CMS.Search;
using CMS.UIControls;

[assembly: RegisterCustomClass("SearchIndexListControlExtender", typeof(SearchIndexListControlExtender))]

/// <summary>
/// Search index list control extender
/// </summary>
public class SearchIndexListControlExtender : ControlExtender<UniGrid>
{
    private bool disableActions = false;


    public override void OnInit()
    {
        Control.OnExternalDataBound += Control_OnExternalDataBound;
        Control.OnAction += Control_OnAction;
        Control.ZeroRowsText = Control.GetString("general.nodatafound");
        Control.OrderBy = "IndexDisplayName";

        string indexPath = Path.Combine(SystemContext.WebApplicationPhysicalPath, SearchHelper.SearchPath);
        if (indexPath.Length > SearchHelper.MAX_INDEX_PATH)
        {
            Control.ShowError(Control.GetString("srch.pathtoolong"));
            disableActions = true;
        }
    }


    /// <summary>
    /// On external databound.
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="sourceName">Source name</param>
    /// <param name="parameter">Parameter</param>    
    object Control_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        switch (sourceName.ToLowerCSafe())
        {
            // Get index status
            case "indexstatus":
                int indexID = ValidationHelper.GetInteger(parameter, 0);
                SearchIndexInfo sii = SearchIndexInfoProvider.GetSearchIndexInfo(indexID);
                if (sii != null)
                {
                    return Control.GetString("srch.status." + sii.IndexFilesStatus);
                }
                break;

            case "indextype":
                string type = ValidationHelper.GetString(parameter, String.Empty);
                return Control.GetString("smartsearch.indextype." + type.ToLowerCSafe());               
        }

        // Disable all actions
        if (disableActions)
        {
            CMSGridActionButton button = null;
            switch (sourceName.ToLowerCSafe())
            {
                case "edit":
                    button = ((CMSGridActionButton)sender);
                    button.Enabled = false;
                    break;
                case "delete":
                    button = ((CMSGridActionButton)sender);
                    button.Enabled = false;
                    break;
                case "rebuild":
                    button = ((CMSGridActionButton)sender);
                    button.Enabled = false;
                    break;
            }
        }

        return null;
    }


    /// <summary>
    /// Handles the UniGrid's OnAction event.
    /// </summary>
    /// <param name="actionName">Name of item (button) that throws event</param>
    /// <param name="actionArgument">ID (value of Primary key) of corresponding data row</param>
    void Control_OnAction(string actionName, object actionArgument)
    {
        switch (actionName)
        {
            case "edit":
                break;

            case "delete":
                // Delete search index info object from database with it's dependencies
                SearchIndexInfoProvider.DeleteSearchIndexInfo(Convert.ToInt32(actionArgument));
                break;

            case "rebuild":
                if (SearchIndexInfoProvider.SearchEnabled)
                {
                    int indexID = ValidationHelper.GetInteger(actionArgument, 0);

                    if (SearchHelper.CreateRebuildTask(indexID))
                    {
                        Control.ShowInformation(Control.GetString("srch.index.rebuildstarted"));
                        // Sleep
                        Thread.Sleep(100);
                    }
                    else
                    {
                        Control.ShowError(Control.GetString("index.nocontent"));
                    }
                }
                else
                {
                    Control.ShowError(Control.GetString("srch.index.searchdisabled"));
                }
                break;
        }
    }
}
