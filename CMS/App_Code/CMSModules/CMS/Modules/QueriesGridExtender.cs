using System;
using System.Linq;

using CMS;
using CMS.ExtendedControls;
using CMS.UIControls;
using CMS.Helpers;
using CMS.DataEngine;
using CMS.Modules;

using UniGridAction = CMS.UIControls.UniGridConfig.Action;

[assembly: RegisterCustomClass("QueriesGridExtender", typeof(QueriesGridExtender))]

/// <summary>
/// Permission edit control extender
/// </summary>
public class QueriesGridExtender : ControlExtender<UniGrid>
{
    // fields
    private CMSUIPage mPage;
    private bool? mQueriesCanBeDeleted;


    /// <summary>
    /// Gets the controls page cast to CMSUIPage
    /// Allows easy access to UIPages's properties
    /// </summary>
    private CMSUIPage Page
    {
        get
        {
            return mPage ?? (mPage = (CMSUIPage)Control.Page);
        }
    }


    /// <summary>
    /// Gets whether queries in the current UIContext can be deleted, cloned, created or modified.
    /// </summary>
    private bool QueriesCanBeDeleted
    {
        get
        {
            if (!mQueriesCanBeDeleted.HasValue)
            {
                DataClassInfo classInfo = ((DataClassInfo)Page.EditedObjectParent);
                bool result = classInfo.ClassShowAsSystemTable;
                if (!result)
                {
                    ResourceInfo resourceInfo = ResourceInfoProvider.GetResourceInfo(classInfo.ClassResourceID);
                    result = resourceInfo.IsEditable;
                }
                mQueriesCanBeDeleted = result;
            }
            return mQueriesCanBeDeleted.Value;
        }
    }


    /// <summary>
    /// Init method
    /// </summary>
    public override void OnInit() 
    {
        Control.OnExternalDataBound += Control_OnExternalDataBound;
        Control.OnAction += Control_OnAction;

        Control.ShowObjectMenu = QueriesCanBeDeleted;
    }


    /// <summary>
    /// Executes custom grid actions.
    /// </summary>
    /// <param name="actionName">Name of the action</param>
    /// <param name="actionArgument">Argument for the action</param>
    private void Control_OnAction(string actionName, object actionArgument)
    {
        switch (actionName)
        {
            case "delete":
                if (QueriesCanBeDeleted)
                {
                    QueryInfo queryInfo = QueryInfoProvider.GetQueryInfo(ValidationHelper.GetInteger(actionArgument, 0));
                    DataClassInfo classInfo = ((DataClassInfo)Page.EditedObjectParent);
                    if ((queryInfo != null) && (classInfo != null) && (queryInfo.ClassID == classInfo.ClassID))
                    {
                        queryInfo.Delete();
                    }
                    else
                    {
                        CMSPage.RedirectToInformation("editedobject.notexists");
                    }
                }
                else
                {
                    Control.ShowError(ResHelper.GetString("cms.query.customization.deletedisabled"));
                }

                break;
        }
    }


    /// <summary>
    /// External data bound event handler handles external data sources binding.
    /// </summary>
    /// <param name="sender">Sender or the event</param>
    /// <param name="sourceName">Name of the external data source</param>
    /// <param name="parameter">Binding parameter</param>
    /// <returns>What should be rendered in given column. Ignored for actions.</returns>
    private object Control_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        switch (sourceName)
        {
            case "deletequery":
                CMSGridActionButton gridButton = sender as CMSGridActionButton;
                if (gridButton != null)
                {
                    gridButton.Enabled = QueriesCanBeDeleted;
                    if (!QueriesCanBeDeleted)
                    {
                        gridButton.OnClientClick = String.Empty;
                    }
                }

                return gridButton;
        }

        return null;
    }
}