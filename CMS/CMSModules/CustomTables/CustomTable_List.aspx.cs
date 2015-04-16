using System;

using CMS.Core;
using CMS.CustomTables;
using CMS.Helpers;
using CMS.PortalEngine;
using CMS.UIControls;
using CMS.DataEngine;
using CMS.Modules;

[Title("customtable.list.Title")]
[Action(0, "customtable.list.NewCustomTable", "CustomTable_New.aspx")]
[UIElement(ModuleName.CUSTOMTABLES, "Development.CustomTables")]
public partial class CMSModules_CustomTables_CustomTable_List : CMSCustomTablesPage
{
    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Initialize grid
        uniGrid.OnAction += uniGrid_OnAction;
        uniGrid.ZeroRowsText = GetString("general.nodatafound");
        uniGrid.EditActionUrl = GetEditUrl();
    }

    #endregion


    #region "Grid events"

    /// <summary>
    /// Handles the UniGrid's OnAction event.
    /// </summary>
    /// <param name="actionName">Name of item (button) that throws event</param>
    /// <param name="actionArgument">ID (value of Primary key) of corresponding data row</param>
    protected void uniGrid_OnAction(string actionName, object actionArgument)
    {
        if (actionName == "edit")
        {
            URLHelper.Redirect("CustomTable_Edit.aspx?customtableid=" + actionArgument);
        }
        else if (actionName == "delete")
        {
            int classId = ValidationHelper.GetInteger(actionArgument, 0);

            if (classId > 0)
            {
                // If no item depends on the current class
                if (!DataClassInfoProvider.CheckDependencies(classId))
                {
                    // Delete the class
                    DataClassInfoProvider.DeleteDataClassInfo(classId);
                    CustomTableItemProvider.ClearLicensesCount();
                }
            }
            else
            {
                // Display error on deleting
                ShowError(GetString("customtable.delete.hasdependencies"));
            }
        }
    }

    #endregion

    
    #region "Private methods"

    /// <summary>
    /// Creates URL for editing.
    /// </summary>
    private String GetEditUrl()
    {
        UIElementInfo uiChild = UIElementInfoProvider.GetUIElementInfo("CMS.CustomTables", "EditCustomTable");
        if (uiChild != null)
        {
            return URLHelper.AppendQuery(UIContextHelper.GetElementUrl(uiChild, false), "objectid={0}");
        }

        return String.Empty;
    }

    #endregion
}