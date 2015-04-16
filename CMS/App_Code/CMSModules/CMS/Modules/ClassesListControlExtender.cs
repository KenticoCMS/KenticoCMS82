using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CMS;
using CMS.Base;
using CMS.DataEngine;
using CMS.ExtendedControls;
using CMS.ExtendedControls.ActionsConfig;
using CMS.Helpers;
using CMS.Modules;
using CMS.UIControls;

[assembly: RegisterCustomClass("ClassesListControlExtender", typeof(ClassesListControlExtender))]

/// <summary>
/// Permission list control extender
/// </summary>
public class ClassesListControlExtender : ControlExtender<UniGrid>
{
    private const string UPDATE = "updateClasses";

    /// <summary>
    /// Gets the current resource
    /// </summary>
    public ResourceInfo Resource
    {
        get
        {
            return Control.UIContext.EditedObjectParent as ResourceInfo;
        }
    }


    /// <summary>
    /// OnInit event handler
    /// </summary>
    public override void OnInit()
    {
        // Add header actions
        InitHeaderActions();

        Control.OnAction += OnAction;
        Control.OnExternalDataBound += OnExternalDataBound;

        // Show warning when module is not editable
        Control.Load += (sender, args) =>
        {
            if ((Resource != null) && (Resource.ResourceId > 0) && !Resource.IsEditable)
            {
                Control.ShowInformation(Control.GetString("resource.classesinstalledresourcewarning"));
            }
        };
    }


    /// <summary>
    /// OnExternalDataBound event handler
    /// </summary>
    private object OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        switch (sourceName.ToLowerCSafe())
        {
            // Disable delete and clone options when module is not editable
            case "delete":
                ((CMSGridActionButton)sender).Enabled = (Resource != null) && (Resource.ResourceId > 0) && Resource.IsEditable;
                break;
            case "#objectmenu":
                ((CMSGridActionButton)sender).Visible = (Resource != null) && (Resource.ResourceId > 0) && Resource.IsEditable;
                break;
        }

        return parameter;
    }


    /// <summary>
    /// Handles the UniGrid's OnAction event.
    /// </summary>
    /// <param name="actionName">Name of item (button) that throws event</param>
    /// <param name="actionArgument">ID (value of Primary key) of corresponding data row</param>
    private void OnAction(string actionName, object actionArgument)
    {
        var classId = ValidationHelper.GetInteger(actionArgument, 0);

        switch (actionName)
        {
            case "delete":
                // Version and document dependencies
                if ((classId <= 0) || DataClassInfoProvider.CheckDependencies(classId))
                {
                    // Display error on deleting
                    Control.ShowError(Control.GetString("sysdev.class_list.delete.hasdependencies"));
                    return;
                }

                var databaseDependencies = DataClassInfoProvider.CheckDatabaseDependencies(classId);

                if (databaseDependencies.Count > 0)
                {
                    Control.ShowError(CreateDatabaseDependenciesErrorMessage(databaseDependencies));

                    return;
                }

                // Delete the class
                DataClassInfoProvider.DeleteDataClassInfo(classId);

                break;
        }
    }


    /// <summary>
    /// Adds header actions to page.
    /// </summary>
    private void InitHeaderActions()
    {
        if (SystemContext.DevelopmentMode)
        {
            // Update class list header action
            Control.AddHeaderAction(new HeaderAction
            {
                Index = 1,
                Text = Control.GetString("sysdev.class_list.updateclasses"),
                CommandName = UPDATE,
                OnClientClick = "return confirm(" + ScriptHelper.GetString(Control.GetString("sysdev.class_list.updateconfirm")) + ");"
            });
            ComponentEvents.RequestEvents.RegisterForEvent(UPDATE, (sender, args) => UpdateClasses());
        }
    }


    /// <summary>
    /// Updates default view for all document types.
    /// </summary>
    private void UpdateClasses()
    {
        TableManager tm = new TableManager(null);

        tm.RefreshDocumentViews();

        // Everything performed well as we get here (no exception was thrown by TableManager.UpdateClasses())
        // Let the system know everything succeeded
        Control.ShowConfirmation(Control.GetString("sysdev.class_list.updatesucc"));
    }


    /// <summary>
    /// Creates error message which contains dependant database object names.
    /// </summary>
    /// <param name="databaseDependencies"></param>
    private string CreateDatabaseDependenciesErrorMessage(List<string> databaseDependencies)
    {
        const string separator = "<br />";
        databaseDependencies = databaseDependencies.ConvertAll(HTMLHelper.HTMLEncode);

        return string.Format("{0}{1}{2}",
            Control.GetString("sysdev.class_list.delete.hasdatabasedependencies"),
            separator,
            TextHelper.Join(separator, databaseDependencies));
    }
}